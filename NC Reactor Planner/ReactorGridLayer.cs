using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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

            Size = new Size(X * PlannerUI.blockSize, Z * PlannerUI.blockSize);
            Visible = true;
            BorderStyle = BorderStyle.FixedSingle;

            ReloadCells();
        }

        public void ReloadCells()
        {
            Controls.Clear();
            if (cells != null)
                foreach (ReactorGridCell c in cells)
                    c.Dispose();
            cells = new ReactorGridCell[X, Z];
            Point location;

            for (int x = 0; x < X; x++)
                for (int z = 0; z < Z; z++)
                {
                    location = new Point(PlannerUI.blockSize * x, PlannerUI.blockSize * z);
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
            Size = new Size(bs * X, bs * Z);
            Point location;

            foreach (ReactorGridCell cell in Controls)
            {
                location = new Point(bs * ((int)cell.block.Position.X - 1), bs * ((int)cell.block.Position.Z - 1));
                cell.Location = location;
                cell.Size = new Size(bs, bs);
            }
        }

        public void Redraw()
        {
            foreach (ReactorGridCell cell in Controls)
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
                foreach (ReactorGridCell rgc in Controls)
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
    }
}