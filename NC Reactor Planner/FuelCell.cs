using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace NC_Reactor_Planner
{
    [Serializable()]
    public class FuelCell : Block
    {
        [field: NonSerialized()]
        private List<FuelCell> _adjacentCells;
        [field: NonSerialized()]
        private int _adjacentModeratorLines;
        [field: NonSerialized()]
        private double _positionalEfficiency;

        private bool _active;
        private bool _shielded;
        private Fuel _usedFuel;


        
        public double HeatProducedPerTick { get => HeatMultiplier * _usedFuel.BaseHeat;}
        public double HeatMultiplier { get => _adjacentModeratorLines;}
        public List<FuelCell> AdjacentCells { get => _adjacentCells; private set => _adjacentCells = value; }
        public int AdjacentModeratorLines { get => _adjacentModeratorLines; private set => _adjacentModeratorLines = value; }
        public bool Active { get => _active; private set => _active = value; }
        public bool Shielded { get => _shielded; private set => _shielded = value; }
        public Fuel UsedFuel { get => _usedFuel; private set => _usedFuel = value; }
        public double PositionalEfficiency { get => _positionalEfficiency; private set => _positionalEfficiency = value; }
        public double Efficiency { get => _positionalEfficiency * _usedFuel.BaseEfficiency; }

        public bool FirstPass;
        public bool Primed;

        public FuelCell(string displayName, Bitmap texture, Point3D position, Fuel usedFuel) : base(displayName, BlockTypes.FuelCell, texture, position)
        {
            UsedFuel = usedFuel;
            RevertToSetup();
        }

        public FuelCell(FuelCell parent, Point3D position, Fuel usedFuel) : this(parent.DisplayName, parent.Texture, position, usedFuel)
        {
        }

        public override string GetToolTip()
        {
            if (Position == Palette.dummyPosition)
                return base.GetToolTip();
            else
                return string.Format("{0}" +
                                    " Fuel: {5}\r\n" +
                                    (Active?" Active\r\n":" Inactive!\r\n") +
                                    " Adjacent cells: {1}\r\n" +
                                    " Adjacent moderator lines: {2}\r\n" +
                                    " Heat multiplier: {3} %\r\n" +
                                    " Heat produced: {4} HU/t\r\n",
                                    base.GetToolTip(), AdjacentCells.Count, AdjacentModeratorLines, (int)(HeatMultiplier*100), HeatProducedPerTick, UsedFuel.Name);
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
            double moderatedNeutronFlux = 0;
            int moderatorsInLine = 0;
            Point3D pos = Position + offset;
            int i = 1;
            while (Reactor.interiorDims.X >= pos.X & Reactor.interiorDims.Y >= pos.Y & Reactor.interiorDims.Z >= pos.Z & pos.X > 0 & pos.Y > 0 & pos.Z > 0 & i <= Configuration.Fission.NeutronReach + 1)
            {
                if (Reactor.BlockAt(pos) is Moderator moderator)
                {
                    moderatedNeutronFlux += moderator.FluxFactor;
                    moderatorsInLine++;
                    i++;
                    pos = Position + offset * i;
                    continue;
                }
                else if (Reactor.BlockAt(pos) is FuelCell fuelCell && i > 1)
                {
                    if (moderatedNeutronFlux / moderatorsInLine >= fuelCell.UsedFuel.CriticalityFactor)
                        if(fuelCell.FirstPass)
                        {
                            fuelCell.FirstPass = false;
                            fuelCell.Activate();
                            fuelCell.AddAdjacentFuelCell(this);
                            fuelCell.AddAdjacentModerator();
                            _positionalEfficiency += moderatedNeutronFlux / moderatorsInLine;
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

        public void AddAdjacentModerator()
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

        public override void RevertToSetup()
        {
            SetCluster(-1);
            AdjacentCells = new List<FuelCell>();
            AdjacentModeratorLines = 0;
            FirstPass = true;
            Active = false;
        }

        public override bool IsValid()
        {
            return Active;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new FuelCell(this, newPosition, this.UsedFuel);
        }
    }
}
