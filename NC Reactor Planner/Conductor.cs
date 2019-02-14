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

        public Conductor(string displayName, Bitmap texture, Point3D position) : base(displayName, BlockTypes.Conductor, texture, position)
        {
            GroupID = -1;
        }

        public override bool IsValid()
        {
            return HasPathToCasing;
        }

        public override void RevertToSetup()
        {
            GroupID = -1;
            HasPathToCasing = false;
            SetCluster(-1);
        }

        public override string GetToolTip()
        {
            return string.Format("Conductor \r\n" +
                                "Group: " + GroupID.ToString() +"\r\n" +
                                (HasPathToCasing?"Has path to casing":"Has no path to casing!"));
        }

    }
}
