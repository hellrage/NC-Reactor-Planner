using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NC_Reactor_Planner
{
    public class ReactorGridCell : PictureBox
    {
        public Block block;
        private bool redrawn;

        public void Clicked(object sender, EventArgs e)
        {
            MouseButtons button = ((MouseEventArgs)e).Button;
            int x = (int)block.Position.X;
            int y = (int)block.Position.Y;
            int z = (int)block.Position.Z;

            switch (button)
            {
                case MouseButtons.Left:
                    Reactor.blocks[x, y, z] = Palette.BlockToPlace(block);
                    break;
                case MouseButtons.None:
                    return;
                case MouseButtons.Right:
                    Reactor.blocks[x, y, z] = new Block("Air", BlockTypes.Air, Palette.textures["Air"], block.Position);
                    break;
                case MouseButtons.Middle:
                    Reactor.blocks[x, y, z] = new FuelCell("FuelCell", Palette.textures["FuelCell"], block.Position);
                    break;
                default:
                    return;
            }
            block = Reactor.blocks[x, y, z];

            Reactor.UpdateStats();

            PlannerUI.gridToolTip.Hide(this);

            ((PlannerUI)Parent.Parent.Parent).RefreshStats();
        }

        public void Mouse_Move(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.None)
                return;
            if (Palette.PlacingSameBlock(block, ((MouseEventArgs)e).Button))
                return;
            Clicked(sender, e);
            RedrawSelf();
        }

        public void Mouse_Down(object sender, EventArgs e)
        {
            Capture = false;
            PlannerUI.gridToolTip.Hide(this);
        }

        public void RedrawSelf()
        {
            if (!Image.Equals(block.Texture))
            {
                Image.Dispose();
                Image = new Bitmap(block.Texture);
            }
            if (block is Cooler cooler && !cooler.Active)
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    Pen errorPen = new Pen(Color.Red, 1);
                    g.DrawRectangle(errorPen, 0, 0, Image.Size.Width-1, Image.Size.Height-1);
                }
            }
            redrawn = true;
            ResetToolTip();
        }

        public void ResetToolTip()
        {
            PlannerUI.gridToolTip.SetToolTip(this, block.GetToolTip());
        }

        public bool NeedsRedraw()
        {
            return redrawn & block.NeedsRedraw();
        }

        public void ResetRedrawn()
        {
            redrawn = false;
        }
    }
}
