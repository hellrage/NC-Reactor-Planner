using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class FuelCell : Block
    {
        public double HeatProducedPerTick { get => HeatMultiplier * UsedFuel.BaseHeat;}
        public double HeatMultiplier { get => AdjacentModeratorLines;}
        public List<FuelCell> AdjacentCells { get; private set; }
        public int AdjacentModeratorLines { get; private set; }
        public bool Active { get; private set; }
        public override bool Valid { get => Active; }
        public bool Shielded { get; private set; }
        public Fuel UsedFuel { get; private set; }
        public double PositionalEfficiency { get; private set; }
        public double ModeratedNeutronFlux { get; private set; }
        public double Efficiency { get => PositionalEfficiency * UsedFuel.BaseEfficiency * (1/(1 + Math.Exp(3*(PositionalEfficiency-UsedFuel.CriticalityFactor-3)))); }
        public double FuelDuration { get => UsedFuel.FuelTime * Reactor.clusters[Cluster].FuelDurationMultiplier / Configuration.Fission.FuelUse; }
        public bool FirstPass { get; set; }
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
            AdjacentModeratorLines = 0;
            PositionalEfficiency = 0;
            FirstPass = true;
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
                return string.Format("{0}" +
                                    ((Reactor.state == ReactorStates.Running)?(Reactor.clusters[Cluster].Valid?"Has casing connection\r\n":" Invalid cluster!\r\n"):"") +
                                    " Fuel: {5}\r\n" +
                                    (Active?" Active\r\n":" Inactive!\r\n") +
                                    " Adjacent cells: {1}\r\n" +
                                    " Adjacent moderator lines: {2}\r\n" +
                                    " Heat multiplier: {3} %\r\n" +
                                    " Heat produced: {4} HU/t\r\n" +
                                    " Efficiency: {6} %\r\n" +
                                    " Positional Eff.: {7} %\r\n" +
                                    (Primed?"Primed":""
                                    ),
                                    base.GetToolTip(), AdjacentCells.Count, AdjacentModeratorLines, (int)(HeatMultiplier*100), HeatProducedPerTick, UsedFuel.Name, (int)(Efficiency*100), (int)(PositionalEfficiency*100));
        }

        public void UpdateStats()
        {
            
        }

        public List<FuelCell> FindModeratorsThenAdjacentCells()
        {
            List<FuelCell> moderatorAdjacentCells = new List<FuelCell>();
            FuelCell fuelCell;
            foreach (Vector3D offset in Reactor.sixAdjOffsets)
            {
                if ((fuelCell = FindModeratorThenAdjacentCell(offset)) != null)
                {
                    moderatorAdjacentCells.Add(fuelCell);
                    AdjacentModeratorLines++;
                }
            }
            AdjacentCells.AddRange(moderatorAdjacentCells);
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
                if (Reactor.BlockAt(pos) is Moderator moderator)
                {
                    sumModeratorFlux += moderator.FluxFactor;
                    sumModeratorEfficiency += moderator.EfficiencyFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
                }
                else if (Reactor.BlockAt(pos) is FuelCell fuelCell && i > 1)
                {
                    fuelCell.PositionalEfficiency += sumModeratorEfficiency / moderatorsInLine;
                    fuelCell.ModeratedNeutronFlux += sumModeratorFlux / moderatorsInLine;
                    if (fuelCell.ModeratedNeutronFlux >= fuelCell.UsedFuel.CriticalityFactor)
                        if(fuelCell.FirstPass)
                        {
                            //this.PositionalEfficiency += moderatedNeutronFlux / moderatorsInLine;
                            fuelCell.FirstPass = false;
                            fuelCell.Activate();
                            fuelCell.AddAdjacentFuelCell(this);
                            fuelCell.AddAdjacentModeratorLine();
                            return fuelCell;
                        }
                        else
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
