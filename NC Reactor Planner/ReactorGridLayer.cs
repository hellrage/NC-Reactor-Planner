using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace NC_Reactor_Planner
{
    public class ReactorGridLayer : Panel
    {
        private ReactorGridCell[,] cells;
        private MenuStrip menu;
        private int _x;
        private int _y;
        private int _z;

        public int X { get => _x; private set => _x = value; }
        public int Y { get => _y; private set => _y = value; }
        public int Z { get => _z; private set => _z = value; }

        public ReactorGridLayer(int y) : base()
        {
            X = (int)Reactor.interiorDims.X;
            Y = y;
            Z = (int)Reactor.interiorDims.Z;

            Width = X * PlannerUI.blockSize;
            Visible = true;
            BorderStyle = BorderStyle.FixedSingle;

            ConstructMenu();
            Height = Z * PlannerUI.blockSize + menu.Height;
            ReloadCells();
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

        public void ReloadCells()
        {
            if (cells != null)
                foreach (ReactorGridCell c in cells)
                {
                    c.Dispose();
                    Controls.Remove(c);
                }
            cells = new ReactorGridCell[X, Z];
            Point location;

            for (int x = 0; x < X; x++)
                for (int z = 0; z < Z; z++)
                {
                    location = new Point(PlannerUI.blockSize * x, menu.Height + PlannerUI.blockSize * z);
                    ReactorGridCell cell = (new ReactorGridCell
                    {
                        Size = new Size(PlannerUI.blockSize, PlannerUI.blockSize),
                        Visible = true,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        ClientSize = new Size(PlannerUI.blockSize, PlannerUI.blockSize),
                        Location = location,
                        block = Reactor.blocks[x + 1, Y, z + 1]
                    });
                    cell.Click += new EventHandler(cell.Clicked);
                    cell.MouseDown += new MouseEventHandler(cell.Mouse_Down);
                    cell.MouseMove += new MouseEventHandler(cell.Mouse_Move);
                    cell.MouseUp += new MouseEventHandler(Reactor.CauseRedraw);
                    cell.Image = new Bitmap(cell.block.Texture);
                    cells[x, z] = cell;
                    Controls.Add(cells[x, z]);
                }

        }

        public void Rescale()
        {
            int bs = PlannerUI.blockSize;
            Size = new Size(bs * X, bs * Z + menu.Height);
            Point location;

            foreach (ReactorGridCell cell in cells)
            {
                location = new Point(bs * ((int)cell.block.Position.X - 1), menu.Height + bs * ((int)cell.block.Position.Z - 1));
                cell.Location = location;
                cell.Size = new Size(bs, bs);
            }

            RescaleMenu();
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

        public void Redraw()
        {
            foreach (ReactorGridCell cell in cells)
            {
                cell.RedrawSelf();
            }
        }

        public Bitmap DrawToImage(int scale = 2)
        {
            Redraw();
            int bs = PlannerUI.blockSize;
            Bitmap layerImage = new Bitmap(X * bs, Z * bs);
            using (Graphics g = Graphics.FromImage(layerImage))
                foreach (ReactorGridCell rgc in cells)
                {
                    System.Windows.Media.Media3D.Point3D pos = rgc.block.Position;
                    g.DrawImage(rgc.Image,
                                new Rectangle((int)(pos.X - 1) * bs, (int)(pos.Z - 1) * bs, bs, bs),
                                new Rectangle(0, 0, bs / scale, bs / scale),
                                GraphicsUnit.Pixel);
                }
            return layerImage;
        }

        public void ResetRedrawn()
        {
            foreach (ReactorGridCell cell in cells)
            {
                cell.ResetRedrawn();
            }
        }

        private void MenuClear(object sender, EventArgs e)
        {
            Reactor.ClearLayer(this);
            ((PlannerUI)(Parent.Parent)).RefreshStats();
        }

        private void MenuCopy(object sender, EventArgs e)
        {
            Reactor.CopyLayer(this);
        }

        private void MenuPaste(object sender, EventArgs e)
        {
            Reactor.PasteLayer(this);
            ((PlannerUI)(Parent.Parent)).RefreshStats();
        }

        private void MenuDelete(object sender, EventArgs e)
        {
            if (Reactor.layers.Count <= 1)
                return;
            Reactor.DeleteLayer(Y);
            ((PlannerUI)(Parent.Parent)).NewResetLayout(true);//And another thing? THIS  U G L Y
        }

        private void MenuInsertBefore(object sender, EventArgs e)
        {
            if (Reactor.layers.Count >= Configuration.Fission.MaxSize)
            {
                MessageBox.Show("Reactor at max size!");
                return;
            }
            Reactor.InsertLayer(Y);
            ((PlannerUI)(Parent.Parent)).NewResetLayout(true);//And another thing? THIS  U G L Y
        }

        private void MenuInsertAfter(object sender, EventArgs e)
        {
            if (Reactor.layers.Count >= Configuration.Fission.MaxSize)
            {
                MessageBox.Show("Reactor at max size!");
                return;
            }
            Reactor.InsertLayer(Y+1);
            ((PlannerUI)(Parent.Parent)).NewResetLayout(true);//And another thing? THIS  U G L Y
        }
    }
}