﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;

namespace NC_Reactor_Planner
{
    /// <summary>
    /// This class is responsible for handling a single Y-layer of the reactor, including the layer menu.
    /// Several of these are added to PlannerUI's reactorGrid. They are initially created in Reactor.ConstructLayers
    /// It also handles drawing the layer to an image for PNG export.
    /// Mouse clicks and the layer menu are handled here.
    /// </summary>
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

            //TODO: Fix scrolling when in per-layer mode
            MouseEnter += new EventHandler((sender, e) => {
                if (Reactor.UI.drawAllLayers)
                    Reactor.UI.ReactorGrid.Focus();
                else Reactor.UI.LayerScrollBar.Focus();
                Reactor.UI.MousedOverLayer = this;
                });
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

            ToolStripMenuItem shrinkMenu = new ToolStripMenuItem { Name = "Shrink", Text = "Shrink" };
            shrinkMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Delete", Text = "Delete this layer" });
            shrinkMenu.DropDownItems["Delete"].Click += new EventHandler(MenuDelete);
            shrinkMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Shrink right", Text = "Delete right column" });
            shrinkMenu.DropDownItems["Shrink right"].Click += new EventHandler(MenuModifySize);
            shrinkMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Shrink left", Text = "Delete left column" });
            shrinkMenu.DropDownItems["Shrink left"].Click += new EventHandler(MenuModifySize);
            shrinkMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Shrink top", Text = "Delete top row" });
            shrinkMenu.DropDownItems["Shrink top"].Click += new EventHandler(MenuModifySize);
            shrinkMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Shrink bottom", Text = "Delete bottom row" });
            shrinkMenu.DropDownItems["Shrink bottom"].Click += new EventHandler(MenuModifySize);

            manageMenu.DropDownItems.Add(shrinkMenu);

            ToolStripMenuItem expandMenu = new ToolStripMenuItem { Name = "Expand", Text = "Expand" };
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Insert after", Text = "Insert a new layer after this one" });
            expandMenu.DropDownItems["Insert after"].Click += new EventHandler(MenuInsertAfter);
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Insert before", Text = "Insert a new layer before this one" });
            expandMenu.DropDownItems["Insert before"].Click += new EventHandler(MenuInsertBefore);
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Expand right", Text = "Insert a column on the right" });
            expandMenu.DropDownItems["Expand right"].Click += new EventHandler(MenuModifySize);
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Expand left", Text = "Insert a column on the left" });
            expandMenu.DropDownItems["Expand left"].Click += new EventHandler(MenuModifySize);
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Expand top", Text = "Insert a row on top" });
            expandMenu.DropDownItems["Expand top"].Click += new EventHandler(MenuModifySize);
            expandMenu.DropDownItems.Add(new ToolStripMenuItem { Name = "Expand bottom", Text = "Insert a row on the bottom" });
            expandMenu.DropDownItems["Expand bottom"].Click += new EventHandler(MenuModifySize);
            manageMenu.DropDownItems.Add(expandMenu);

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
                return Reactor.BlockAt(new Vector3(cellX, Y, cellZ)).Cluster;
            else
                return -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            FullRedraw(e.Graphics);
            DrawClusterHighlights(e.Graphics);
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                DrawClusterHighlight(e.Graphics, HighlightedCluster, PlannerUI.PrimedFuelCellOrangePen);
        }

        private void DrawClusterHighlights(Graphics g, bool forExport = false)
        {
            //TODO: Consolidate checks with Block and Heatsink
            foreach (Cluster cluster in Reactor.clusters)
            {
                if (cluster.NetHeatClass == NetHeatClass.Overheating)
                    DrawClusterHighlight(g, cluster.ID, PlannerUI.ClusterOverheatPen, forExport);
                else if (cluster.NetHeatClass == NetHeatClass.Overcooled)
                    DrawClusterHighlight(g, cluster.ID, PlannerUI.ClusterOvercoolPen, forExport);
                else if(cluster.NetHeatClass == NetHeatClass.HeatPositive)
                    DrawClusterHighlight(g, cluster.ID, PlannerUI.ClusterHeatPositivePen, forExport);
            }
        }

        public void FullRedraw(Graphics g, bool forExport = false)
        {
            //g.CompositingMode = CompositingMode.SourceCopy;
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

            Block block = Reactor.BlockAt(new Vector3(x, Y, z));

            g.DrawImage(block.Texture, cellRect);

            if (PlannerUI.HeatsinkTypeOverlay && block.BlockType == BlockTypes.HeatSink && PlannerUI.OverlayedTypes.Contains(((HeatSink)block).HeatSinkType))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                g.DrawString(block.DisplayName.Substring(0, 1), new Font(FontFamily.GenericSansSerif, (float)(Reactor.UI.BlockSize/2.7), FontStyle.Bold), Brushes.Black, cellRect, sf);
            }

            if (noChecking)
                return;

            if (!(block is Moderator) && !block.Valid)
                g.DrawRectangle(PlannerUI.ErrorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
            if (block.Cluster != -1 && !Reactor.clusters[block.Cluster].HasPathToCasing)
                g.DrawRectangle(PlannerUI.InactiveClusterPen, location.X + 2 * ds, location.Y + 2 * ds, bs - 4 * ds, bs - 4 * ds);

            if (block is FuelCell fuelCell && fuelCell.Primed)
            {
                switch (fuelCell.NeutronSource)
                {
                    case "Ra-Be":
                        g.DrawEllipse(PlannerUI.PrimedFuelCellOrangePen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);  
                        break;
                    case "Po-Be":
                        g.DrawEllipse(PlannerUI.PrimedFuelCellYellowPen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);
                        break;
                    case "Cf-252":
                        g.DrawEllipse(PlannerUI.PrimedFuelCellGreenPen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);
                        break;
                    default:
                        g.DrawEllipse(PlannerUI.PaletteHighlightPen, location.X + 3 * ds, location.Y + 3 * ds, bs - 6 * ds, bs - 6 * ds);
                        break;
                }
            }
            else if (block is Moderator moderator)
            {
                if (!moderator.Active & !moderator.HasAdjacentValidFuelCell)
                    g.DrawRectangle(PlannerUI.ErrorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
                if (moderator.Valid)
                    g.DrawRectangle(PlannerUI.ValidModeratorPen, location.X + ds, location.Y + ds, bs - 2 * ds, bs - 2 * ds);
            }
            else if(block.BlockType == BlockTypes.HeatSink || block.BlockType == BlockTypes.Irradiator || block.BlockType == BlockTypes.NeutronShield || block.BlockType == BlockTypes.Conductor)
            {
                if(block.Cluster == -1)
                    g.DrawRectangle(PlannerUI.InactiveClusterPen, location.X + 2 * ds, location.Y + 2 * ds, bs - 4 * ds, bs - 4 * ds);
            }
        }

        public void DrawClusterHighlight(Graphics g, int clusterID, Pen pen, bool forExport = false)
        {
            if (clusterID == -1 | clusterID > Reactor.clusters.Count-1)
                return;

            int bs = Reactor.UI.BlockSize;
            Tuple<Point, Point> Line(Vector3 position, Vector3 offset)
            {
                position = new Vector3(position.X - 1, position.Y, position.Z - 1);
                int menuOffset = (forExport ? 0 : menu.Height);
                if (offset == new Vector3(1, 0, 0))
                    return Tuple.Create(new Point((int)(position.X + 1)*bs - 2, (int)position.Z * bs + menuOffset), new Point((int)(position.X + 1) * bs - 2, (int)(position.Z + 1) * bs + menuOffset));
                if (offset == new Vector3(-1, 0, 0))
                    return Tuple.Create(new Point((int)position.X * bs + 2, (int)position.Z * bs + menuOffset), new Point((int)position.X * bs + 2, (int)(position.Z + 1) * bs + menuOffset));
                if (offset == new Vector3(0, 0, 1))
                    return Tuple.Create(new Point((int)position.X * bs, (int)(position.Z + 1) * bs + menuOffset - 2), new Point((int)(position.X + 1) * bs, (int)(position.Z + 1) * bs + menuOffset - 2));
                if (offset == new Vector3(0, 0, -1))
                    return Tuple.Create(new Point((int)(position.X + 1) * bs, (int)position.Z * bs + menuOffset + 2), new Point((int)position.X * bs, (int)position.Z * bs + menuOffset + 2));
                throw new ArgumentException();
            }
//#if DEBUG
            //System.Diagnostics.Debug.WriteLine(string.Format("Redrawing cluster {0} highlight on layer {1}", clusterID, Y));
//#endif
            foreach (Block block in Reactor.clusters[clusterID].Blocks)
            {
                if (block.Position.Y != Y)
                    continue;
                foreach (Vector3 offset in new List<Vector3> { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1) })
                {
                    int neighbourCluster = Reactor.BlockAt(block.Position + offset).Cluster;
                    if (neighbourCluster != clusterID)
                    {
                        Tuple<Point, Point> points = Line(block.Position, offset);
                        g.DrawLine(pen, points.Item1, points.Item2);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point cellCoords = ConvertCellCoordinates(e);
            cellX = cellCoords.X;
            cellZ = cellCoords.Y;

            Vector3 position = new Vector3(cellX, Y, cellZ);
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
                Vector3 position = new Vector3(cellX, Y, cellZ);
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
            Vector3 position = new Vector3(cellX, Y, cellZ);
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

        private void HandleMouse(MouseButtons button, Vector3 position)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    if ((ModifierKeys & Keys.Shift) != 0)
                    {
                        if (Reactor.BlockAt(position) is FuelCell fuelCell)
                        {
                            if (fuelCell.CanBePrimed())
                                fuelCell.CyclePrimed();
                            else
                            {
                                Reactor.UI.UIToolTip.Show("This FuelCell can't be primed! Has no LOS to a casing.", Reactor.UI.ReactorGrid, cellX * Reactor.UI.BlockSize + 16, menu.Height + cellZ * Reactor.UI.BlockSize + 16, 1500);
                                fuelCell.UnPrime();
                            }
                        }
                        else if(Reactor.BlockAt(position) is NeutronShield neutronShield)
                        {
                            if (neutronShield.Active)
                                neutronShield.Deactivate();
                            else
                                neutronShield.Activate();
                        }
                    }
                    else
                        PlaceBlock(cellX, cellZ, Palette.BlockToPlace(Reactor.BlockAt(position)));
                    break;
                case MouseButtons.Right:
                    PlaceBlock(cellX, cellZ, new Block("Air", BlockTypes.Air, Palette.Textures["Air"], position));
                    break;
                case MouseButtons.Middle:
                    if(Reactor.BlockAt(position) is FuelCell previousFuelCell)
                        PlaceBlock(cellX, cellZ, new FuelCell("FuelCell", Palette.Textures["FuelCell"], position, Palette.SelectedFuel, previousFuelCell.Primed, previousFuelCell.NeutronSource));
                    else
                        PlaceBlock(cellX, cellZ, new FuelCell("FuelCell", Palette.Textures["FuelCell"], position, Palette.SelectedFuel));
                    break;
                case MouseButtons.XButton1:
                case MouseButtons.XButton2:
                case MouseButtons.None:
                default:
                    return;
            }
            using (Graphics g = CreateGraphics())
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                RedrawCell(cellX, cellZ, g, true);
            }
        }

        private void PlaceBlock(int x, int z, Block block)
        {
            Reactor.SetBlock(block, new Vector3(x, Y, z));
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
                DrawClusterHighlights(g, true);
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

        private void MenuModifySize(object sender, EventArgs e)
        {
            string[] arguments = ((ToolStripItem)sender).Name.Split(' ');
            int X = (int)Reactor.interiorDims.X;
            int Y = (int)Reactor.interiorDims.Y;
            int Z = (int)Reactor.interiorDims.Z;
            if (arguments[0] == "Expand")
            {
                switch (arguments[1])
                {
                    case "right":
                        if(X+1 > Configuration.Fission.MaxSize)
                        {
                            MessageBox.Show("Reactor at max size!");
                            return;
                        }
                        Reactor.ModifySize(X + 1, Y, Z, new Point(1, 1), new Point(1, 1));
                        break;
                    case "left":
                        if (X + 1 > Configuration.Fission.MaxSize)
                        {
                            MessageBox.Show("Reactor at max size!");
                            return;
                        }
                        Reactor.ModifySize(X + 1, Y, Z, new Point(1, 1), new Point(2, 1));
                        break;
                    case "top":
                        if (Z + 1 > Configuration.Fission.MaxSize)
                        {
                            MessageBox.Show("Reactor at max size!");
                            return;
                        }
                        Reactor.ModifySize(X, Y, Z + 1, new Point(1, 1), new Point(1, 2));
                        break;
                    case "bottom":
                        if (Z + 1 > Configuration.Fission.MaxSize)
                        {
                            MessageBox.Show("Reactor at max size!");
                            return;
                        }
                        Reactor.ModifySize(X, Y, Z + 1, new Point(1, 1), new Point(1, 1));
                        break;
                    default:
                        throw new ArgumentException("Unexpected expansion direction: " + arguments[1]);
                }

            }
            else if(arguments[0] == "Shrink")
            {
                switch (arguments[1])
                {
                    case "right":
                        if(X==1)
                        {
                            MessageBox.Show("Can't remove the last blocks!");
                            return;
                        }
                        Reactor.ModifySize(X - 1, Y, Z, new Point(1, 1), new Point(1, 1));
                        break;
                    case "left":
                        if (X == 1)
                        {
                            MessageBox.Show("Can't remove the last blocks!");
                            return;
                        }
                        Reactor.ModifySize(X - 1, Y, Z, new Point(2, 1), new Point(1, 1));
                        break;
                    case "top":
                        if (Z == 1)
                        {
                            MessageBox.Show("Can't remove the last blocks!");
                            return;
                        }
                        Reactor.ModifySize(X, Y, Z - 1, new Point(1, 2), new Point(1, 1));
                        break;
                    case "bottom":
                        if (Z == 1)
                        {
                            MessageBox.Show("Can't remove the last blocks!");
                            return;
                        }
                        Reactor.ModifySize(X, Y, Z - 1, new Point(1, 1), new Point(1, 1));
                        break;
                    default:
                        throw new ArgumentException("Unexpected shrinking direction: " + arguments[1]);
                }
            }
            Reactor.UI.ResetLayout(true);
        }
    }
}