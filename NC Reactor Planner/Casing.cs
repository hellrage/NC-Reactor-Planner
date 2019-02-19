using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Casing : Block
    {
        public Casing(string displayName, Bitmap texture, Point3D position): base(displayName, BlockTypes.Casing, texture, position)
        {

        }

        public Casing(Casing parent, Point3D position) : base(parent.DisplayName, BlockTypes.Casing, parent.Texture, position)
        {

        }

        public override Block Copy(Point3D newPosition)
        {
            return new Casing(this, newPosition);
        }

    }
}
