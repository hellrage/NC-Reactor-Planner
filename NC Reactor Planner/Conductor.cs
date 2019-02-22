using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class Conductor : Block
    {
        public bool HasPathToCasing { get; set; }
        public int GroupID { get; set; }
        public override bool Valid { get => HasPathToCasing; }

        public Conductor(string displayName, Bitmap texture, Point3D position) : base(displayName, BlockTypes.Conductor, texture, position)
        {
            GroupID = -1;
        }

        public Conductor(Conductor parent, Point3D newPosition) : this(parent.DisplayName, parent.Texture, newPosition)
        {

        }

        public override void RevertToSetup()
        {
            GroupID = -1;
            HasPathToCasing = false;
        }

        public override string GetToolTip()
        {
            if (Position == Palette.dummyPosition)
                return base.GetToolTip() + "Conducts heat from clusters,\r\nuse these to make a path\r\nto a casing if a cluster isn't\r\ntouching one directly.";

            return string.Format("Conductor \r\n" +
                                "Group: " + GroupID.ToString() +"\r\n" +
                                (HasPathToCasing?"Has path to casing":"--Has no path to casing!"));
        }

        public override Block Copy(Point3D newPosition)
        {
            return new Conductor(this, newPosition);
        }

    }
}
