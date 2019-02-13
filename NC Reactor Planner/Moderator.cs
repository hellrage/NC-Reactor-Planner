using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    [Serializable()]
    public class Moderator : Block
    {
        private bool _active;
        private ModeratorTypes _moderatorType;
        private double _fluxFactor;

        public bool Active { get => _active; private set => _active = value; }
        public ModeratorTypes ModeratorType { get => _moderatorType; private set => _moderatorType = value; }
        public double FluxFactor { get => _fluxFactor; private set => _fluxFactor = value; }

        public Moderator(string displayName, ModeratorTypes type, Bitmap texture, Point3D position, double fluxFactor) : base(displayName, BlockTypes.Moderator, texture, position)
        {
            FluxFactor = fluxFactor;
            Active = false;
            ModeratorType = type;
        }

        public Moderator(Moderator parent, Point3D position) : this(parent.DisplayName, parent.ModeratorType, parent.Texture, position, parent.FluxFactor)
        {
            ModeratorType = parent.ModeratorType;
        }

        public void UpdateStats()
        {
            Active = FindAdjacentFuelCells() > 0;
        }

        public int FindAdjacentFuelCells()
        {
            int adjCells = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
                if (Reactor.BlockAt(Position + o) is FuelCell fuelCell)
                    if(fuelCell.Active)
                        adjCells++;
            return adjCells;
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " Moderator\r\n";
            if (Position != Palette.dummyPosition)
            {
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
                if (!Active)
                    toolTip += "INACTIVE!!!\r\n";
            }
            toolTip += string.Format("Flux Factor: {0}\r\n", FluxFactor);
            return toolTip;
        }

        public override bool IsValid()
        {
            return Active;
        }

        public override Block Copy(Point3D newPosition)
        {
            return new Moderator(this, newPosition);
        }

    }

    public enum ModeratorTypes
    {
        Beryllium,
        Graphite,
        //NotAModerator,
    }
}
