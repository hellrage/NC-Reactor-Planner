using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public class Casing : Block
    {
        public Casing(string displayName, Bitmap texture, Vector3 position): base(displayName, BlockTypes.Casing, texture, position)
        {

        }

        public Casing(Casing parent, Vector3 position) : base(parent.DisplayName, BlockTypes.Casing, parent.Texture, position)
        {

        }

        public override Block Copy(Vector3 newPosition)
        {
            return new Casing(this, newPosition);
        }

    }
}
