using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class Block
    {
        private bool _valid;
        public string DisplayName { get; private set; }
        public Bitmap Texture { get; set; }
        public Point3D Position { get; private set; }
        public BlockTypes BlockType { get; private set; }
        public int Cluster { get; private set; }
        public virtual bool Valid { get => true; protected set => _valid = value; }

        public Block()
        {
            DisplayName = "Air";
            Texture = Palette.Textures["Air"];
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
                if (Cluster != -1)
                    toolTip += string.Format("Cluster: {0}\r\n", Cluster);
                else if (BlockType != BlockTypes.Air && BlockType != BlockTypes.Reflector)
                    toolTip += "--No cluster!\r\n";
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
