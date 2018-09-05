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
    public class Block
    {
        private string _displayName;
        [NonSerialized()]
        private Bitmap _texture;
        private Point3D _position;
        private BlockTypes _type;

        public string DisplayName { get => _displayName; private set => _displayName = value; }
        public Bitmap Texture { get => _texture; set => _texture = value; }
        public Point3D Position { get => _position; private set => _position = value; }
        public BlockTypes BlockType { get => _type; private set => _type = value; }

        public Block()
        {
            DisplayName = "Air";
            Texture = new Bitmap(Palette.textures["Air"]);
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

        public virtual void ReloadValuesFromSetttings()
        {
        }

        public virtual bool NeedsRedraw()
        {
            return false;
        }

        public virtual bool IsActive()
        {
            return false;
        }
    }
}
