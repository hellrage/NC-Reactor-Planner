using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Text;

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
        public int ModeratedNeutronFlux { get; private set; }
        public double Efficiency { get => CalculateEfficiency(); }
        public double FuelDuration { get => UsedFuel.FuelTime * Reactor.clusters[Cluster].FuelDurationMultiplier / Configuration.Fission.FuelUse; }
        public bool Primed { get; set; }
        public string NeutronSource { get; private set; }

        public FuelCell(string displayName, Bitmap texture, Vector3 position, Fuel usedFuel, bool primed = false, string neutronSource = "None") : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            UsedFuel = usedFuel;
            Primed = primed;
            NeutronSource = neutronSource;
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

        public FuelCell(FuelCell parent, Vector3 position, Fuel usedFuel, bool primed = false) : this(parent.DisplayName, parent.Texture, position, usedFuel, primed)
        {
        }

        public override string GetToolTip()
        {
            if (Position == Palette.dummyPosition)
                return base.GetToolTip();
            else
            {
                StringBuilder tb = new StringBuilder(); //TooltipBuilder
                tb.Append(base.GetToolTip());
                if(Cluster != -1)
                    tb.Append(Reactor.clusters[Cluster].Valid ? " Has casing connection\r\n" : "--Invalid cluster!\r\n--No casing connection");
                tb.Append(String.Format(" Fuel : {0}\r\n", UsedFuel.Name));
                tb.Append(Active ? " Active\r\n" : "--Inactive!\r\n");
                tb.Append(String.Format(" Adjacent cells: {0}\r\n", AdjacentCells.Count));
#if DEBUG
                string adjCells = "";
                foreach(FuelCell fc in AdjacentCells)
                    adjCells += "   " + fc.Position.ToString() + "\r\n";
                tb.Append(adjCells.ToString());
#endif
                tb.Append(String.Format(" Adjacent moderator lines: {0}\r\n", AdjacentModeratorLines));
                tb.Append(String.Format(" Adjacent reflectors: {0}\r\n", AdjacentReflectors.Count));
                tb.Append(String.Format(" Heat multiplier: {0} %\r\n", (int)(HeatMultiplier * 100)));
                tb.Append(String.Format(" Heat produced: {0} HU/t\r\n", HeatProducedPerTick));
                tb.Append(String.Format(" Efficiency: {0} %\r\n", (int)(Efficiency * 100)));
                tb.Append(String.Format(" Positional Eff.: {0} %\r\n", (int)(PositionalEfficiency * 100)));
                tb.Append(String.Format(" Total Neutron Flux: {0}\r\n", ModeratedNeutronFlux));
                tb.Append(String.Format(" Criticality factor: {0}\r\n", UsedFuel.CriticalityFactor));
                if(Primed)
                {
                    tb.Append("Primed\r\n");
                    tb.Append(String.Format(" Neutron source: {0}", NeutronSource));
                }

                return tb.ToString();
            }
        }

        public List<FuelCell> FindModeratorsThenAdjacentCells()
        {
            List<FuelCell> moderatorAdjacentCells = new List<FuelCell>();
            FuelCell fuelCell;

            foreach (Vector3 offset in Reactor.sixAdjOffsets)
                if ((fuelCell = FindModeratorThenAdjacentCell(offset)) != null)
                    if (!moderatorAdjacentCells.Contains(fuelCell))
                        moderatorAdjacentCells.Add(fuelCell);

            return moderatorAdjacentCells;
        }

        public FuelCell FindModeratorThenAdjacentCell(Vector3 offset)
        {
            int sumModeratorFlux = 0;
            double sumModeratorEfficiency = 0;
            int moderatorsInLine = 0;
            Vector3 pos = Position + offset;
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
                    if (fuelCell.ModeratedNeutronFlux >= fuelCell.UsedFuel.CriticalityFactor)
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
                    PositionalEfficiency += Configuration.Fission.ReflectorEfficiency * sumModeratorEfficiency / moderatorsInLine;
                    reflector.AddAdjacentFuelCell(this);
                    if (ModeratedNeutronFlux >= UsedFuel.CriticalityFactor)
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
                --Reactor.functionalBlocks;
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
            foreach (Vector3 offset in Reactor.sixAdjOffsets)
            {
                int i = 1;
                primable = true;
                Vector3 pos = Position + i * offset;
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

        public void UnPrime()
        {
            Primed = false;
            NeutronSource = "None";
        }

        public void CyclePrimed()
        {
            if(Primed)
            {
                int currIndex = Palette.NeutronSourceNames.IndexOf(NeutronSource);
                if (currIndex != -1 && currIndex < Palette.NeutronSourceNames.Count - 1)
                    NeutronSource = Palette.NeutronSourceNames[currIndex + 1];
                else
                {
                    NeutronSource = "None";
                    Primed = false;
                    return;
                }
            }
            else
            {
                if(Palette.NeutronSourceNames.Count == 0)
                    throw new IndexOutOfRangeException("There were no Neutron Sources in the configuration!");
                NeutronSource = Palette.NeutronSourceNames[0];
                Primed = true;
            }
        }

        private double CalculateEfficiency()
        {
            double eff = PositionalEfficiency * UsedFuel.BaseEfficiency * (1 / (1 + Math.Exp(2 * (ModeratedNeutronFlux - 2 * UsedFuel.CriticalityFactor))));
            if(NeutronSource != "None")
                eff *= Configuration.NeutronSources[NeutronSource].Efficiency;
            return eff;
        }

        public void Activate()
        {
            Active = true;
        }

        public override Block Copy(Vector3 newPosition)
        {
            return new FuelCell(this, newPosition, this.UsedFuel, Primed);
        }

        public string ToSaveString()
        {
            return string.Join(";", UsedFuel.Name, Primed.ToString(), NeutronSource);
        }
    }
}
