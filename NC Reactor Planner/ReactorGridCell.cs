using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NC_Reactor_Planner
{
    public class ReactorGridCell : PictureBox
    {
        public Block block;

        public void Clicked(object sender, EventArgs e)
        {

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
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
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
                            g.DrawRectangle(errorPen, 2, 2, Image.Size.Width - 5, Image.Size.Height - 5);
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
                        using (Pen primedPen = new Pen(Color.Orange, 1))
                            g.DrawRectangle(primedPen, 3, 3, Image.Size.Width - 7, Image.Size.Height - 7);
                    }
                }


                if (block.BlockType != BlockTypes.Air & block.BlockType != BlockTypes.Moderator & block.BlockType != BlockTypes.Conductor)
                    if (block.Cluster != -1)
                    {
                        if (!Reactor.clusters[block.Cluster].HasPathToCasing)
                            using (Pen inactiveClusterPen = new Pen(Color.LightPink, 1))
                                g.DrawRectangle(inactiveClusterPen, 1, 1, Image.Size.Width - 3, Image.Size.Height - 3);
                    }
                    else
                    {
                        g.DrawRectangle(errorPen, 1, 1, Image.Size.Width - 3, Image.Size.Height - 3);
                    }

                errorPen.Dispose();
            }
            ResetToolTip();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            PlannerUI.gridToolTip.RemoveAll();
            PlannerUI.gridToolTip.Hide(this);
            base.OnMouseLeave(e);
        }

        public void ResetToolTip()
        {
            PlannerUI.gridToolTip.SetToolTip(this, block.GetToolTip());
        }
    }
}
