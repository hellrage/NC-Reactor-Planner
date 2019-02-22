using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Moderator : Block
    {
        public override bool Valid { get; protected set; }
        public double HeatGenerationPerTick { get; private set; }
        public ModeratorTypes ModeratorType { get; private set; }

        public Moderator(string displayName, ModeratorTypes type, Bitmap texture, Point3D position) : base(displayName, BlockTypes.Moderator, texture, position)
        {
            HeatGenerationPerTick = 0;
            Valid = false;
            ModeratorType = type;
        }

        public Moderator(Moderator parent, Point3D position) : this(parent.DisplayName, parent.ModeratorType, parent.Texture, position)
        {
            ModeratorType = parent.ModeratorType;
        }

        public void UpdateStats()
        {
            if (FindAdjacentFuelCells() == 0)
            {
                HeatGenerationPerTick = Reactor.usedFuel.BaseHeat * Configuration.Fission.HeatGeneration;
                Reactor.totalHeatPerTick += HeatGenerationPerTick;
                Valid = false;
            }
            else
            {
                HeatGenerationPerTick = 0;
                Valid = true;
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
                if (!Valid)
                    toolTip += "INACTIVE!!!";
            }
            return toolTip;
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
