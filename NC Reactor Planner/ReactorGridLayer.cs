using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public class ReactorGridLayer : Panel
    {
        private MenuStrip menu;
        private int cellX;
        private int cellZ;

        public int HighlightedCluster { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public ReactorGridLayer(int y) : base()
        {
            X = (int)Reactor.interiorDims.X;
            Y = y;
            Z = (int)Reactor.interiorDims.Z;

            Reactor.UI.GridToolTip.RemoveAll();
            cellX = -1;
            cellZ = -1;
            HighlightedCluster = -1;

            MouseEnter += new EventHandler((sender, e) => { Reactor.UI.ReactorGrid.Focus(); Reactor.UI.MousedOverLayer = this; });
            MouseLeave += new EventHandler((sender, e) => { if(Reactor.UI.MousedOverLayer == this) Reactor.UI.MousedOverLayer = null; });

            Width = X * Reactor.UI.BlockSize;
            Visible = true;
            BorderStyle = BorderStyle.FixedSingle;

            ConstructMenu();
            Height = Z * Reactor.UI.BlockSize + menu.Height;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Refresh();
        }

        private void ConstructMenu()
        {
            menu = new MenuStrip();
            menu.Dock = DockStyle.None;
            ToolStripMenuItem editMenu = new ToolStripMenuItem { Name = "Edit", Text = "Edit" };
            editMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Clear", Text = "Clear layer" });
            editMenu.DropDownItems["Clear"].Click += new EventHandler(MenuClear);
            editMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Copy", Text = "Copy layer" });
            editMenu.DropDownItems["Copy"].Click += new EventHandler(MenuCopy);
            editMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Paste", Text = "Paste layer" });
            editMenu.DropDownItems["Paste"].Click += new EventHandler(MenuPaste);
            menu.Items.Add(editMenu);

            ToolStripMenuItem manageMenu = new ToolStripMenuItem { Name = "Manage", Text = "Manage" };
            manageMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Delete", Text = "Delete layer" });
            manageMenu.DropDownItems["Delete"].Click += new EventHandler(MenuDelete);
            manageMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Insert after", Text = "Insert a new layer after this one" });
            manageMenu.DropDownItems["Insert after"].Click += new EventHandler(MenuInsertAfter);
            manageMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Insert before", Text = "Insert a new layer before this one" });
            manageMenu.DropDownItems["Insert before"].Click += new EventHandler(MenuInsertBefore);
            menu.Items.Add(manageMenu);

            ToolStripMenuItem layerLabel = new ToolStripMenuItem { Name = "LayerLabel", Text = "Layer " + Y.ToString() };
            menu.Items.Add(layerLabel);

            ResetRescaleMenu();

            menu.Location = new Point(0, 0);
            menu.Visible = true;
            Controls.Add(menu);
        }

        public void Rescale()
        {
            int bs = Reactor.UI.BlockSize;
            Size = new Size(bs * X, bs * Z + menu.Height);
            ResetRescaleMenu();
            Refresh();
        }

        private void ResetRescaleMenu()
        {
            foreach (ToolStripMenuItem item in menu.Items)
                item.AutoSize = true;
            menu.Items["Edit"].Text = "Edit";
            menu.Items["Manage"].Text = "Manage";
            menu.Items["LayerLabel"].Text = "Layer " + Y.ToString();
            if (this.Width < menu.Width)
            {
                menu.Items["Edit"].AutoSize = false;
                menu.Items["Edit"].Text = "E";
                menu.Items["Edit"].Size = new Size(Width / 4, menu.Items["Edit"].Size.Height);
                menu.Items["Manage"].AutoSize = false;
                menu.Items["Manage"].Text = "M";
                menu.Items["Manage"].Size = new Size(Width / 4, menu.Items["Manage"].Size.Height);
                menu.Items["LayerLabel"].AutoSize = false;
                menu.Items["LayerLabel"].Text = "L " + Y.ToString();
                menu.Items["LayerLabel"].Size = new Size(Width / 2, menu.Items["LayerLabel"].Size.Height);
            }

        }

        public int GetClusterToHighlight()
        {
            if (cellX <= Reactor.interiorDims.X & cellX > 0 & cellZ <= Reactor.interiorDims.Z & cellZ >0)
                return Reactor.BlockAt(new Point3D(cellX, Y, cellZ)).Cluster;
            else
                return -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            FullRedraw(e.Graphics);
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                DrawClusterHighlight(e.Graphics, HighlightedCluster);
        }

        public void FullRedraw(Graphics g, bool forExport = false)
        {
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (int x = 1; x <= X; x++)
                for (int z = 1; z <= Z; z++)
                {
                    RedrawCell(x, z, g, false, forExport);
                }
        }

        public void RedrawCell(int x, int z, Graphics g, bool noChecking = false, bool forExport = false)
        {
            int bs = Reactor.UI.BlockSize;
            int ds = (int)Reactor.UI.DrawingScale;
            Point location;
            location = new Point(bs * (x - 1), (forExport ? 0 : menu.Height) + bs * (z - 1));
            Rectangle cellRect = new Rectangle(location, new Size(bs, bs));

            Block block = Reactor.BlockAt(new Point3D(x, Y, z));

            g.DrawImage(block.Texture, cellRect);

            if (PlannerUI.HeatsinkTypeOverlay && block.BlockType == BlockTypes.HeatSink && PlannerUI.OverlayedTypes.Contains(((HeatSink)block).HeatSinkType))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                g.DrawString(block.DisplayName.Substring(0, 1), new Font(FontFamily.GenericSansSerif, (float)(Reactor.UI.BlockSize/2.7), FontStyle.Bold), Brushes.Black, cellRect, sf);
                g.CompositingMode = CompositingMode.SourceCopy;
            }

            if (noChecking)
                return;

            if (!(block is Moderator) && !block.Valid)
                g.DrawRectangle(PlannerUI.ErrorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
            if (block.Cluster != -1 && !Reactor.clusters[block.Cluster].HasPathToCasing)
                g.DrawRectangle(PlannerUI.InactiveClusterPen, location.X + 2 * ds, location.Y + 2 * ds, bs - 4 * ds, bs - 4 * ds);
            if (block is FuelCell fuelCell && fuelCell.Primed)
                g.DrawEllipse(PlannerUI.PrimedFuelCellPen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);
            if (block is Moderator moderator)
            {
                if (!moderator.Active & !moderator.HasAdjacentValidFuelCell)
                    g.DrawRectangle(PlannerUI.ErrorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
                if (moderator.Valid)
                    g.DrawRectangle(PlannerUI.ValidModeratorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
            }
        }

        public void DrawClusterHighlight(Graphics g, int clusterID)
        {
            if (clusterID == -1 | clusterID > Reactor.clusters.Count-1)
                return;

            int bs = Reactor.UI.BlockSize;
            Tuple<Point, Point> Line(Point3D position, Vector3D offset)
            {
                position = new Point3D(position.X - 1, position.Y, position.Z - 1);
                if (offset == new Vector3D(1, 0, 0))
                    return Tuple.Create(new Point((int)(position.X + 1)*bs - 3, (int)position.Z * bs + menu.Height), new Point((int)(position.X + 1) * bs - 3, (int)(position.Z + 1) * bs + menu.Height));
                if (offset == new Vector3D(-1, 0, 0))
                    return Tuple.Create(new Point((int)position.X * bs + 3, (int)position.Z * bs + menu.Height), new Point((int)position.X * bs + 3, (int)(position.Z + 1) * bs + menu.Height));
                if (offset == new Vector3D(0, 0, 1))
                    return Tuple.Create(new Point((int)position.X * bs, (int)(position.Z + 1) * bs + menu.Height - 3), new Point((int)(position.X + 1) * bs, (int)(position.Z + 1) * bs + menu.Height - 3));
                if (offset == new Vector3D(0, 0, -1))
                    return Tuple.Create(new Point((int)(position.X + 1) * bs, (int)position.Z * bs + menu.Height + 3), new Point((int)position.X * bs, (int)position.Z * bs + menu.Height + 3));
                throw new ArgumentException();
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("Redrawing cluster {0} highlight on layer {1}", clusterID, Y));
#endif
            foreach (Block block in Reactor.clusters[clusterID].blocks)
            {
                if (block.Position.Y != Y)
                    continue;
                foreach (Vector3D offset in new List<Vector3D> { new Vector3D(-1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), new Vector3D(0, 0, 1) })
                {
                    int neighbourCluster = Reactor.BlockAt(block.Position + offset).Cluster;
                    if (neighbourCluster != clusterID)
                    {
                        Tuple<Point, Point> points = Line(block.Position, offset);
                        g.DrawLine(PlannerUI.PrimedFuelCellPen, points.Item1, points.Item2);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point cellCoords = ConvertCellCoordinates(e);
            cellX = cellCoords.X;
            cellZ = cellCoords.Y;

            Point3D position = new Point3D(cellX, Y, cellZ);
            HandleMouse(e.Button, position);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point cellCoords = ConvertCellCoordinates(e);
            int newCellX = cellCoords.X;
            int newCellZ = cellCoords.Y;

            if (cellX != newCellX | cellZ != newCellZ)
            {
                cellX = newCellX;
                cellZ = newCellZ;

                if (cellX > X || cellZ > Z || cellX < 1 || cellZ < 1)
                    return;
                Point3D position = new Point3D(cellX, Y, cellZ);
                HandleMouse(e.Button, position);
                Block block = Reactor.BlockAt(position);
                if((ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    if (block.Cluster != HighlightedCluster)
                    {
                        Reactor.UI.HighlightCluster(block.Cluster);
                    }
                }
                Reactor.UI.GridToolTip.Show(block.GetToolTip(), this, cellX * Reactor.UI.BlockSize + 16, menu.Height + cellZ * Reactor.UI.BlockSize + 16);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Reactor.UI.GridToolTip.RemoveAll();

            if (Reactor.UI.drawAllLayers)
                foreach (ReactorGridLayer layer in Reactor.layers)
                {
                    layer.HighlightedCluster = -1;
                    layer.Refresh();
                }
            else
            {
                HighlightedCluster = -1;
                Refresh();
            }
            cellX = -1;
            cellZ = -1;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Point cellCoords = ConvertCellCoordinates(e);
            cellX = cellCoords.X;
            cellZ = cellCoords.Y;

            Reactor.Update();
            Reactor.UI.RefreshStats();
            Point3D position = new Point3D(cellX, Y, cellZ);
            Reactor.UI.GridToolTip.Show(Reactor.BlockAt(position).GetToolTip(), this, cellX * Reactor.UI.BlockSize + 16, menu.Height + cellZ * Reactor.UI.BlockSize + 16);
            Reactor.Redraw();
            base.OnMouseUp(e);
        }

        private Point ConvertCellCoordinates(MouseEventArgs e)
        {
            int newX;
            int newZ;
            if (e.X > Width)
                newX = Width;
            else if (e.X < 0)
                newX = 0;
            else
                newX = e.X;

            if (e.Y - menu.Height > Height)
                newZ = Height - menu.Height;
            else if (e.Y - menu.Height < 0)
                newZ = 0;
            else
                newZ = e.Y - menu.Height;

            return new Point((newX / Reactor.UI.BlockSize) + 1, (newZ / Reactor.UI.BlockSize) + 1);
        }

        private void HandleMouse(MouseButtons button, Point3D position)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    if ((ModifierKeys & Keys.Shift) != 0 && Reactor.BlockAt(position) is FuelCell fuelCell)
                    {
                        if (fuelCell.CanBePrimed())
                            fuelCell.TogglePrimed();
                        else
                            Reactor.UI.UIToolTip.Show("This FuelCell can't be primed! Has no LOS to a casing.", Reactor.UI.ReactorGrid, cellX * Reactor.UI.BlockSize + 16, menu.Height + cellZ * Reactor.UI.BlockSize + 16, 1500);
                    }
                    else
                        PlaceBlock(cellX, cellZ, Palette.BlockToPlace(Reactor.BlockAt(position)));
                    break;
                case MouseButtons.Right:
                    PlaceBlock(cellX, cellZ, new Block("Air", BlockTypes.Air, Palette.Textures["Air"], position));
                    break;
                case MouseButtons.Middle:
                    PlaceBlock(cellX, cellZ, new FuelCell((FuelCell)Palette.BlockPalette["FuelCell"], position, Palette.SelectedFuel));
                    break;
                case MouseButtons.XButton1:
                case MouseButtons.XButton2:
                case MouseButtons.None:
                default:
                    return;
            }
            Graphics g = CreateGraphics();
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            RedrawCell(cellX, cellZ, g, true);
        }

        private void PlaceBlock(int x, int z, Block block)
        {
            Reactor.SetBlock(block, new Point3D(x, Y, z));
            if (block is FuelCell)
                Reactor.Update();
        }

        public Bitmap DrawToImage()
        {
            int bs = Reactor.UI.BlockSize;
            Bitmap layerImage = new Bitmap(X * bs, Z * bs);
            using (Graphics g = Graphics.FromImage(layerImage))
            {
                FullRedraw(g, true);
            }
            return layerImage;
        }

        private void MenuClear(object sender, EventArgs e)
        {
            Reactor.ClearLayer(this);
            Reactor.UI.RefreshStats();
        }

        private void MenuCopy(object sender, EventArgs e)
        {
            Reactor.CopyLayer(this);
        }

        private void MenuPaste(object sender, EventArgs e)
        {
            Reactor.PasteLayer(this);
            Reactor.UI.RefreshStats();
        }

        private void MenuDelete(object sender, EventArgs e)
        {
            if (Reactor.layers.Count <= 1)
                return;
            Reactor.DeleteLayer(Y);
            Reactor.UI.ResetLayout(true);
        }

        private void MenuInsertBefore(object sender, EventArgs e)
        {
            if (Reactor.layers.Count >= Configuration.Fission.MaxSize)
            {
                MessageBox.Show("Reactor at max size!");
                return;
            }
            Reactor.InsertLayer(Y);
            Reactor.UI.ResetLayout(true);
        }

        private void MenuInsertAfter(object sender, EventArgs e)
        {
            if (Reactor.layers.Count >= Configuration.Fission.MaxSize)
            {
                MessageBox.Show("Reactor at max size!");
                return;
            }
            Reactor.InsertLayer(Y + 1);
            Reactor.UI.ResetLayout(true);
        }
    }
}