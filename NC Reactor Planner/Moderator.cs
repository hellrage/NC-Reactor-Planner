using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Moderator : Block
    {
        public override bool Valid { get; protected set; }
        public bool Active { get; set; }
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

        public void Validate()
        {
            Valid = true;
        }

        public void Invalidate()
        {
            Valid = false;
        }

        public override string GetToolTip()
        {
            string toolTip = DisplayName + " moderator\r\n";
            if (Position != Palette.dummyPosition)
            {
                if (Active)
                    toolTip += "Active, can support coolers!";
                else if (Valid)
                    toolTip += "Valid, cannot support coolers!";
                else
                    toolTip += "Invalid! No adjacent cells or\r\n" +
                        "in an invalid line";
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
