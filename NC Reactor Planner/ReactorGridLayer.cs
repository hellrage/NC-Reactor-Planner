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

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public ReactorGridLayer(int y) : base()
        {
            X = (int)Reactor.interiorDims.X;
            Y = y;
            Z = (int)Reactor.interiorDims.Z;

            PlannerUI.gridToolTip.RemoveAll();
            PlannerUI.gridToolTip.Hide(Reactor.UI.ReactorGrid);
            cellX = -1;
            cellZ = -1;

            Width = X * PlannerUI.blockSize;
            Visible = true;
            BorderStyle = BorderStyle.FixedSingle;

            ConstructMenu();
            Height = Z * PlannerUI.blockSize + menu.Height;

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

            RescaleMenu();

            menu.Location = new Point(0, 0);
            menu.Visible = true;
            Controls.Add(menu);
            Refresh();
        }

        public void Rescale()
        {
            int bs = PlannerUI.blockSize;
            Size = new Size(bs * X, bs * Z + menu.Height);
            RescaleMenu();
            Refresh();
        }

        private void RescaleMenu()
        {
            if (this.Width < 130)
            {
                menu.Items["Edit"].Text = "E";
                menu.Items["Manage"].Text = "M";
                menu.Items["LayerLabel"].Text = "L " + Y.ToString();
            }
            else
            {
                menu.Items["Edit"].Text = "Edit";
                menu.Items["Manage"].Text = "Manage";
                menu.Items["LayerLabel"].Text = "Layer " + Y.ToString();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            FullRedraw(e.Graphics);
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
            int bs = PlannerUI.blockSize;
            int ds = (int)Reactor.UI.DrawingScale;
            Point location;
            location = new Point(bs * (x - 1), (forExport?0:menu.Height) + bs * (z - 1));
            Rectangle cellRect = new Rectangle(location, new Size(bs, bs));
            
            Block block = Reactor.BlockAt(new Point3D(x, Y, z));

            g.DrawImage(block.Texture, cellRect);

            if (noChecking)
                return;

            if (!block.Valid)
                g.DrawRectangle(PlannerUI.ErrorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
            if (block.Cluster != -1 && !Reactor.clusters[block.Cluster].HasPathToCasing)
                g.DrawRectangle(PlannerUI.InactiveClusterPen, location.X + 2 * ds, location.Y + 2 * ds, bs - 4 * ds, bs - 4 * ds);
            if (block is FuelCell fuelCell && fuelCell.Primed)
                g.DrawEllipse(PlannerUI.PrimedFuelCellPen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Tuple<int, int> cellCoords = ConvertCellCoordinates(e);
            cellX = cellCoords.Item1;
            cellZ = cellCoords.Item2;

            Point3D position = new Point3D(cellX, Y, cellZ);
            HandleMouse(e.Button, position);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Tuple<int, int> cellCoords = ConvertCellCoordinates(e);
            int newCellX = cellCoords.Item1;
            int newCellZ = cellCoords.Item2;


            if (cellX != newCellX | cellZ != newCellZ)
            {
                cellX = newCellX;
                cellZ = newCellZ;

                if (cellX > X || cellZ > Z || cellX < 1 || cellZ < 1)
                    return;
                Point3D position = new Point3D(cellX, Y, cellZ);
                HandleMouse(e.Button, position);
                PlannerUI.gridToolTip.Show(Reactor.BlockAt(position).GetToolTip(), this, cellX * PlannerUI.blockSize + 16, menu.Height + cellZ * PlannerUI.blockSize + 16);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            PlannerUI.gridToolTip.RemoveAll();
            PlannerUI.gridToolTip.Hide(Reactor.UI.ReactorGrid);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Tuple<int, int> cellCoords = ConvertCellCoordinates(e);
            cellX = cellCoords.Item1;
            cellZ = cellCoords.Item2;

            Reactor.Update();
            Reactor.UI.RefreshStats();
            Point3D position = new Point3D(cellX, Y, cellZ);
            PlannerUI.gridToolTip.Show(Reactor.BlockAt(position).GetToolTip(), this, cellX * PlannerUI.blockSize + 16, menu.Height + cellZ * PlannerUI.blockSize + 16);
            Reactor.Redraw();
            base.OnMouseUp(e);
        }

        private Tuple<int,int> ConvertCellCoordinates(MouseEventArgs e)
        {
            return Tuple.Create((((e.X > Width) ? Width : e.X) / PlannerUI.blockSize)+1,
                                (((((e.Y - menu.Height) > Height) ? Height : e.Y) - menu.Height) / PlannerUI.blockSize)+1);
        }

        private void HandleMouse(MouseButtons button, Point3D position)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    if ((ModifierKeys & Keys.Shift) != 0 && Reactor.BlockAt(position) is FuelCell fuelCell)
                        fuelCell.TogglePrimed();
                    else
                        PlaceBlock(cellX, cellZ, Palette.BlockToPlace(Reactor.BlockAt(position)));
                    break;
                case MouseButtons.Right:
                    PlaceBlock(cellX, cellZ, new Block("Air", BlockTypes.Air, Palette.textures["Air"], position));
                    break;
                case MouseButtons.Middle:
                    PlaceBlock(cellX, cellZ, new FuelCell((FuelCell)Palette.blockPalette["FuelCell"], position, Palette.selectedFuel));
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
            int bs = PlannerUI.blockSize;
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