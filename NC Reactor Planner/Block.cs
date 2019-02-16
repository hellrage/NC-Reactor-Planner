using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Block
    {

        public string DisplayName { get ; private set; }
        public Bitmap Texture { get; set ; }
        public Point3D Position { get; private set; }
        public BlockTypes BlockType { get; private set; }
        public int Cluster { get ; private set; }
        public virtual bool Valid { get; protected set ; }

        public Block()
        {
            DisplayName = "Air";
            Texture = new Bitmap(Palette.textures["Air"]);
            Position = Palette.dummyPosition;
        }

        public Block(string displayName, BlockTypes blockType, Bitmap texture, Point3D position, int clusterID = -1)
        {
            DisplayName = displayName;
            Texture = texture;
            Position = position;
            BlockType = blockType;
            Cluster = clusterID;
        }

        public virtual void SetCluster(int clusterID)
        {
            Cluster = clusterID;
        }

        public virtual void RevertToSetup()
        {
        }

        public virtual string GetToolTip()
        {
            string toolTip = DisplayName + "\r\n";
            if (Position != Palette.dummyPosition)
            {
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
                if (Cluster != -1)
                    toolTip += string.Format("Cluster: {0}\r\n", Cluster);
                else if (Reactor.state == ReactorStates.Running)
                    toolTip += "No cluster!\r\n";
            }
            return toolTip;
        }

        public virtual void ReloadValuesFromConfig()
        {
        }

        public virtual Block Copy(Point3D newPosition)
        {
            return new Block(DisplayName, BlockType, Texture, newPosition);
        }

        public virtual Dictionary<string, int> GetResourceCosts()
        {
            return new Dictionary<string, int>();
        }
    }
}
