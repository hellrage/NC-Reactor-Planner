using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class FuelCell : Block
    {
        public double HeatProducedPerTick { get => HeatMultiplier * UsedFuel.BaseHeat;}
        public double HeatMultiplier { get => AdjacentModeratorLines;}
        public List<FuelCell> AdjacentCells { get; private set; }
        public List<Reflector> AdjacentReflectors { get; private set; }
        public int AdjacentModeratorLines { get; private set; }
        public bool Active { get; private set; }
        public override bool Valid { get => Active; }
        public bool Shielded { get; private set; }
        public Fuel UsedFuel { get; private set; }
        public double PositionalEfficiency { get; private set; }
        public double ModeratedNeutronFlux { get; private set; }
        public double Efficiency { get => PositionalEfficiency * UsedFuel.BaseEfficiency * (1 / (1 + Math.Exp(2 * (ModeratedNeutronFlux - 2 * UsedFuel.CriticalityFactor)))); }
        public double FuelDuration { get => UsedFuel.FuelTime * Reactor.clusters[Cluster].FuelDurationMultiplier / Configuration.Fission.FuelUse; }
        public bool Primed { get; set; }

        public FuelCell(string displayName, Bitmap texture, Point3D position, Fuel usedFuel, bool primed = false) : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            UsedFuel = usedFuel;
            Primed = primed;
            RevertToSetup();
        }

        public override void RevertToSetup()
        {
            SetCluster(-1);
            AdjacentCells = new List<FuelCell>();
            AdjacentReflectors = new List<Reflector>();
            AdjacentModeratorLines = 0;
            PositionalEfficiency = 0;
            ModeratedNeutronFlux = 0;
            Active = false;
        }

        public FuelCell(FuelCell parent, Point3D position, Fuel usedFuel, bool primed = false) : this(parent.DisplayName, parent.Texture, position, usedFuel, primed)
        {
        }

        public override string GetToolTip()
        {
            if (Position == Palette.dummyPosition)
                return base.GetToolTip();
            else
            {
#if DEBUG
                string adjCells = "";
                foreach(FuelCell fc in AdjacentCells)
                {
                    adjCells += "   " + fc.Position.ToString() + "\r\n";
                }
#endif
                return string.Format("{0}" +
                                    ((Cluster!=-1)?
                                    (Reactor.clusters[Cluster].Valid ? " Has casing connection\r\n" : "--Invalid cluster!\r\n"):"") +
                                    " Fuel: {5}\r\n" +
                                    (Active ? " Active\r\n" : "--Inactive!\r\n") +
                                    " Adjacent cells: {1}\r\n" +
#if DEBUG
                                    adjCells +
#endif
                                    " Adjacent moderator lines: {2}\r\n" +
                                    " Adjacent reflectors: {10}\r\n" +
                                    " Heat multiplier: {3} %\r\n" +
                                    " Heat produced: {4} HU/t\r\n" +
                                    " Efficiency: {6} %\r\n" +
                                    " Positional Eff.: {7} %\r\n" +
                                    " Total Neutron Flux: {8}\r\n" +
                                    " Criticality factor: {9}\r\n" +
                                    (Primed ? " Primed" : ""
                                    ),
                                    base.GetToolTip(), AdjacentCells.Count, AdjacentModeratorLines, (int)(HeatMultiplier * 100), HeatProducedPerTick, UsedFuel.Name, (int)(Efficiency * 100), (int)(PositionalEfficiency * 100), ModeratedNeutronFlux, UsedFuel.CriticalityFactor, AdjacentReflectors.Count);
            }
        }

        public List<FuelCell> FindModeratorsThenAdjacentCells()
        {
            List<FuelCell> moderatorAdjacentCells = new List<FuelCell>();
            FuelCell fuelCell;

            foreach (Vector3D offset in Reactor.sixAdjOffsets)
                if ((fuelCell = FindModeratorThenAdjacentCell(offset)) != null)
                    if (!moderatorAdjacentCells.Contains(fuelCell))
                        moderatorAdjacentCells.Add(fuelCell);

            return moderatorAdjacentCells;
        }

        public FuelCell FindModeratorThenAdjacentCell(Vector3D offset)
        {
            double sumModeratorFlux = 0;
            double sumModeratorEfficiency = 0;
            int moderatorsInLine = 0;
            Point3D pos = Position + offset;
            int i = 1;
            while (Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z & pos.X > 0 & pos.Y > 0 & pos.Z > 0 & i <= Configuration.Fission.NeutronReach + 1)
            {
                Block block = Reactor.BlockAt(pos);
                if (block is Moderator moderator)
                {
                    sumModeratorFlux += moderator.FluxFactor;
                    sumModeratorEfficiency += moderator.EfficiencyFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
                }
                else if (block is FuelCell fuelCell && i > 1 && !fuelCell.AdjacentCells.Contains(this))
                {
                    fuelCell.PositionalEfficiency += sumModeratorEfficiency / moderatorsInLine;
                    fuelCell.ModeratedNeutronFlux += sumModeratorFlux;
                    fuelCell.AddAdjacentFuelCell(this);
                    if (Math.Round(fuelCell.ModeratedNeutronFlux,2) >= fuelCell.UsedFuel.CriticalityFactor)
                    {
                        ((Moderator)Reactor.BlockAt(Position + offset)).Active = true;
                        ((Moderator)Reactor.BlockAt(pos - offset)).Active = true;
                        fuelCell.Activate();
                        return fuelCell;
                    }
                    else
                        return null;
                }
                else if(block.BlockType == BlockTypes.Reflector)
                {
                    Reflector reflector = block as Reflector;
                    if (AdjacentReflectors.Contains(reflector))
                    {
                        if (Active)
                            reflector.Active = true;
                        return null;
                    }
                    if (i < 2 || i > (Configuration.Fission.NeutronReach / 2) + 1)
                        return null;
                    ModeratedNeutronFlux += 2 * sumModeratorFlux;
                    PositionalEfficiency += sumModeratorEfficiency / (2*moderatorsInLine);
                    reflector.AddAdjacentFuelCell(this);
                    if (Math.Round(ModeratedNeutronFlux, 2) >= UsedFuel.CriticalityFactor)
                    {
                        Activate();
                        reflector.Active = true;
                    }
                    AdjacentReflectors.Add(reflector);
                    return null;
                }
                else
                    return null;

            }
            return null;
        }

        public void AddAdjacentFuelCell(FuelCell fuelCell)
        {
            AdjacentCells.Add(fuelCell);
        }

        public void AddAdjacentModeratorLine()
        {
            AdjacentModeratorLines++;
        }

        public void FilterAdjacentStuff()
        {
            if(!Active)
            {
                AdjacentCells.Clear();
                AdjacentModeratorLines = 0;
                return;
            }
            AdjacentCells = AdjacentCells.FindAll(fc => fc.Active);
            AdjacentModeratorLines = AdjacentCells.Count + AdjacentReflectors.Count;
        }

        public bool CanBePrimed()
        {
            bool primable;
            foreach (Vector3D offset in Reactor.sixAdjOffsets)
            {
                int i = 1;
                primable = true;
                Point3D pos = Position + i * offset;
                while (Reactor.interiorDims.X + 1 >= pos.X & Reactor.interiorDims.Y + 1 >= pos.Y & Reactor.interiorDims.Z + 1 >= pos.Z & pos.X >= 0 & pos.Y >= 0 & pos.Z >= 0)
                {
                    if (Reactor.BlockAt(pos).BlockType == BlockTypes.FuelCell | Reactor.BlockAt(pos).BlockType == BlockTypes.Reflector)
                    {
                        primable = false;
                        break;
                    }
                    pos = Position + ++i * offset;
                }
                if(primable)
                    return true;
            }
            return false;
        }

        public void TogglePrimed()
        {
            Primed = !Primed;
        }

        public void Activate()
        {
            Active = true;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new FuelCell(this, newPosition, this.UsedFuel, Primed);
        }

        public string ToSaveString()
        {
            return string.Join(";",UsedFuel.Name,Primed.ToString());
        }
    }
}
