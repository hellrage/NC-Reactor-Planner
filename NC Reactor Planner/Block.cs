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
        private bool _valid;
        public string DisplayName { get; private set; }
        public Bitmap Texture { get; set; }
        public Point3D Position { get; private set; }
        public BlockTypes BlockType { get; private set; }
        public virtual bool Valid { get => true; protected set => _valid = value; }

        public Block()
        {
            DisplayName = "Air";
            Texture = new Bitmap(Palette.Textures["Air"]);
            Position = Palette.dummyPosition;
        }

        public Block(string displayName, BlockTypes blockType, Bitmap texture, Point3D position)
        {
            DisplayName = displayName;
            Texture = texture;
            Position = position;
            BlockType = blockType;
        }

        public virtual string GetToolTip()
        {
            string toolTip = DisplayName + "\r\n";
            if(Position != Palette.dummyPosition)
                toolTip += string.Format("at: X: {0} Y: {1} Z: {2}\r\n", Position.X, Position.Y, Position.Z);
            return toolTip;
        }

        public virtual void ReloadValuesFromConfig()
        {
        }

        public virtual Block Copy(Point3D newPosition)
        {
            return new Block(DisplayName, BlockType, Texture, newPosition);
        }

        public virtual bool NeedsRedraw()
        {
            return false;
        }

        public virtual bool IsValid()
        {
            return false;
        }

        public virtual Dictionary<string, int> GetResourceCosts()
        {
            return new Dictionary<string, int>();
        }
    }
}
