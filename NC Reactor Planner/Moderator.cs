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
        //[field: NonSerialized()]
        //public double powerMulti = (double)1 / (double)6;
        //[field: NonSerialized()]
        //public double heatMulti = (double)1 / (double)3;
        private bool _active;
        private double _heatGenerationPerTick;
        private ModeratorTypes _moderatorType;

        public bool Active { get => _active; private set => _active = value; }
        public double HeatGenerationPerTick { get => _heatGenerationPerTick; private set => _heatGenerationPerTick = value; }
        public ModeratorTypes ModeratorType { get => _moderatorType; private set => _moderatorType = value; }

        public Moderator(string displayName, ModeratorTypes type, Bitmap texture, Point3D position) : base(displayName, BlockTypes.Moderator, texture, position)
        {
            HeatGenerationPerTick = 0;
            Active = false;
        }

        public Moderator(Moderator parent, Point3D position) : this(parent.DisplayName, parent.ModeratorType, parent.Texture, position)
        {
        }

        public void UpdateStats()
        {
            if (FindAdjacentFuelCells() == 0)
            {
                HeatGenerationPerTick = Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration;
                Reactor.totalHeatPerTick += HeatGenerationPerTick;
                Active = false;
            }
            else
            {
                HeatGenerationPerTick = 0;
                Active = true;
            }
        }

        public int FindAdjacentFuelCells()
        {
            int adjCells = 0;
            foreach (Vector3D o in Reactor.sixAdjOffsets)
            {
                if (Reactor.BlockAt(Position + o) is FuelCell)
                    adjCells++;
            }
            return adjCells;
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " moderator\r\n";
            if (Position != Palette.dummyPosition)
            {
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
                if (!Active)
                    toolTip += "INACTIVE!!!";
            }
            return toolTip;
        }

        public override bool IsActive()
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
