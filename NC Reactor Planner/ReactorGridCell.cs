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

        public void Clicked(object sender, EventArgs e)
        {
            if(Reactor.state == ReactorStates.Running)
            {
                MessageBox.Show("No changes to the layout allowed while reactor is running!");
                return;
            }

            MouseButtons button = ((MouseEventArgs)e).Button;
            int x = (int)block.Position.X;
            int y = (int)block.Position.Y;
            int z = (int)block.Position.Z;

            switch (button)
            {
                case MouseButtons.Left:
                    if(((ModifierKeys & Keys.Shift)!=0) & block.BlockType == BlockTypes.FuelCell)
                    {
                        ((FuelCell)block).TogglePrimed();
                        break;
                    }
                    Reactor.blocks[x, y, z] = Palette.BlockToPlace(block);
                    break;
                case MouseButtons.None:
                    return;
                case MouseButtons.Right:
                    Reactor.blocks[x, y, z] = new Block("Air", BlockTypes.Air, Palette.textures["Air"], block.Position);
                    break;
                case MouseButtons.Middle:
                    Reactor.blocks[x, y, z] = new FuelCell("FuelCell", Palette.textures["FuelCell"], block.Position, Palette.selectedFuel);
                    break;
                default:
                    return;
            }

            Reactor.Update();
            
            PlannerUI.gridToolTip.Active = false;
            RedrawSelf();
            PlannerUI.gridToolTip.Active = true;
        }

        public void Mouse_Move(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.None)
                return;
            if (Palette.PlacingSameBlock(block, ((MouseEventArgs)e).Button))
                return;
            if ((ModifierKeys & Keys.Shift) != 0)
                return;
            Clicked(sender, e);
        }

        public void Mouse_Down(object sender, EventArgs e)
        {
            Capture = false;
            PlannerUI.gridToolTip.Active = false;
        }

        public void RedrawSelf()
        {
            block = Reactor.BlockAt(block.Position);
            if (!Image.Equals(block.Texture))
            {
                Image.Dispose();
                Image = new Bitmap(block.Texture);
            }

            using (Graphics g = Graphics.FromImage(Image))
            {
                Pen errorPen = new Pen(Color.Red, 1);
                if (block is HeatSink | block is Conductor)
                {
                    if (!block.Valid)
                    { 
                        g.DrawRectangle(errorPen, 0, 0, Image.Size.Width - 1, Image.Size.Height - 1);
                    }
                }
                else if(block is Moderator moderator)
                {
                    if(!moderator.Active)
                    {
                            g.DrawRectangle(errorPen, 2, 2, Image.Size.Width - 4, Image.Size.Height - 4);
                    }
                }
                else if (block is FuelCell fuelCell)
                {
                    if (!fuelCell.Active & !fuelCell.Primed)
                    {
                            g.DrawRectangle(errorPen, 0, 0, Image.Size.Width - 1, Image.Size.Height - 1);
                    }
                    if (fuelCell.Primed)
                    {
                            Pen primedPen = new Pen(Color.Orange, 2);
                            g.DrawRectangle(primedPen, 2, 2, Image.Size.Width - 4, Image.Size.Height - 4);
                    }
                }
                //if(block.BlockType != BlockTypes.Air & block.BlockType != BlockTypes.Moderator)
                //    g.DrawString(block.Cluster.ToString(), new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold), Brushes.Orange, 3, 3);
            }
            ResetToolTip();
        }

        public void ResetToolTip()
        {
            PlannerUI.gridToolTip.SetToolTip(this, block.GetToolTip());
        }
    }
}
