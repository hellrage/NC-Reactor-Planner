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
        public virtual bool Valid { get => true; protected set => _valid = value; }

        public Block()
        {
            DisplayName = "Air";
            Texture = Palette.Textures["Air"];
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
