using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using NC_Reactor_Planner.Properties;

namespace NC_Reactor_Planner
{
    public static class Palette
    {
        public class PalettePanel : Panel
        {
            private int cellX;
            private int cellZ;
            private int Xhighlight;
            private int Zhighlight;
            private CheckBox activeCooler;
            public static readonly int blockSide = 32;
            public static readonly int spacing = 3;
            public static readonly int namestripHeight = 30;
            private static readonly Font nameStringFont = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

            public PalettePanel()
            {
                int height = (int)Math.Ceiling(((double)(BlockPalette.Keys.Count ) / (Width / (blockSide + 2 * spacing)))) * (blockSide + 2 * spacing);
                Size = new Size(200, height + namestripHeight);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

                activeCooler = new CheckBox();
                activeCooler.Text = "Active";
                activeCooler.Font = nameStringFont;
                activeCooler.Location = new Point(Width - (activeCooler.Width - 20), spacing);
                activeCooler.CheckedChanged += new EventHandler(ActiveCooler_CheckedChanged);

                PlannerUI.uiToolTip.SetToolTip(activeCooler, "Toggles between placing a passive or active (fluid-filled) cooler");
                Controls.Add(activeCooler);

                cellX = -1;
                cellZ = -1;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Redraw(e.Graphics);
            }

            public void Redraw(Graphics g)
            {
                DrawNamestring(g, selectedBlock.DisplayName);
                if(selectedBlock.BlockType == BlockTypes.Cooler && ((Cooler)selectedBlock).Active)
                    using (Pen activeCoolerPen = new Pen(Color.Green, 3))
                        DrawHighlightRectangle(g, Xhighlight, Zhighlight, activeCoolerPen);
                else
                    using (Pen highlightPen = new Pen(Color.Blue, 3))
                        DrawHighlightRectangle(g, Xhighlight, Zhighlight, highlightPen);

                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                int x = spacing;
                int y = namestripHeight + spacing;
                foreach (var block in blocks)
                {
                    g.DrawImage(block.Key.Texture, x, y, blockSide, blockSide);
                    x += blockSide + 2 * spacing;
                    if(x+blockSide+spacing > Width)
                    {
                        x = spacing;
                        y += blockSide + 2 * spacing;
                    }
                }
            }
            internal void ResetHighlight()
            {
                Xhighlight = 0;
                Zhighlight = 0;
            }

            private void DrawNamestring(Graphics g, string name)
            {
                g.FillRectangle(new SolidBrush(DefaultBackColor), new Rectangle(0, 0, Width, namestripHeight));
                g.DrawString(name, nameStringFont, Brushes.Black, new PointF(spacing, spacing));
            }

            private void DrawHighlightRectangle(Graphics g, int cellX, int cellZ, Pen pen)
            {
                g.DrawRectangle(pen, cellX * (blockSide + 2 * spacing), namestripHeight + cellZ * (blockSide + 2 * spacing), blockSide + 2 * spacing, blockSide + 2 * spacing);
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
                    int blockIndex = cellZ * (Width / (blockSide + 2 * spacing)) + cellX;
                    if (blockIndex < blocks.Count)
                    {
                        DrawNamestring(CreateGraphics(), blocks.Keys.ElementAt(blockIndex).DisplayName);
                        paletteToolTip.Show(blocks.Keys.ElementAt(blockIndex).GetToolTip(), this, (cellX + 1) * (blockSide + 2 * spacing), (cellZ + 1) * (blockSide + 2 * spacing) + namestripHeight);
                    }
                }
                base.OnMouseMove(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                paletteToolTip.RemoveAll();
                paletteToolTip.Hide(this);
                cellX = -1;
                cellZ = -1;
                base.OnMouseLeave(e);
                Refresh();
            }

            private Tuple<int, int> ConvertCellCoordinates(MouseEventArgs e)
            {
                return Tuple.Create(((e.X > Width - spacing - Width % (blockSide+2*spacing)) ? Width - (blockSide + 2 * spacing) : e.X) / (blockSide + 2*spacing),
                                    ((e.Y - namestripHeight > Height) ? Height : e.Y - namestripHeight) / (blockSide + 2*spacing));
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                Tuple<int, int> cellCoords = ConvertCellCoordinates(e);
                int newCellX = cellCoords.Item1;
                int newCellZ = cellCoords.Item2;

                int blockIndex = cellZ * (Width / (blockSide + 2 * spacing)) + cellX;
                if (blockIndex < blocks.Count)
                {
                    selectedBlock = blocks.Keys.ElementAt(blockIndex);
                    Xhighlight = newCellX;
                    Zhighlight = newCellZ;
                }
                Refresh();
                base.OnMouseClick(e);
            }

            private void ActiveCooler_CheckedChanged(object sender, EventArgs e)
            {
                LoadPalette(activeCooler.Checked);
                string selected = selectedBlock.BlockType.ToString();
                if (selectedBlock.BlockType == BlockTypes.Cooler)
                    selected = ((Cooler)selectedBlock).CoolerType.ToString();
                else if (selectedBlock.BlockType == BlockTypes.Moderator)
                    selected = ((Moderator)selectedBlock).ModeratorType.ToString();
                selectedBlock = BlockPalette[selected];
                Refresh();
            }
        }

        public static Dictionary<string, Bitmap> Textures { get; private set; }
        public static Dictionary<string, Block> BlockPalette { get; private set; }
        public static Dictionary<string, Fuel> FuelPalette { get; private set; }
        public static readonly Point3D dummyPosition = new Point3D(-1, -1, -1);
        public static Fuel SelectedFuel { get; set; }
        public static PalettePanel PaletteControl { get; private set; }

        private static Dictionary<Block, BlockTypes> blocks;
        private static List<Cooler> coolers;
        private static List<Moderator> moderators;
        private static ToolTip paletteToolTip;
        private static Block selectedBlock;


        public static void Load()
        {
            if(Textures == null)
                LoadTextures();
            LoadPalette();
            selectedBlock = BlockPalette["Air"];
            PaletteControl = new PalettePanel();
            paletteToolTip = new ToolTip();

            selectedBlock = BlockPalette["Air"];
            PaletteControl.ResetHighlight();
            PaletteControl.Refresh();
        }

        private static void LoadTextures()
        {
            Textures = new Dictionary<string, Bitmap>();

            PropertyInfo[] resourcesProps = typeof(Resources).GetProperties();
            foreach (PropertyInfo resource in resourcesProps)
            {
                if (resource.PropertyType == typeof(Bitmap))
                    Textures.Add(resource.Name, (Bitmap)resource.GetValue(null));
            }
            return;
        }

        public static void LoadPalette(bool active = false)
        {
            blocks = new Dictionary<Block, BlockTypes>();
            BlockPalette = new Dictionary<string, Block>();
            FuelPalette = new Dictionary<string, Fuel>();
            coolers = new List<Cooler>();
            moderators = new List<Moderator>();

            PopulateCoolers(active);
            PopulateModerators();
            PopulateBlocks();

            PopulateBlockPalette();
            PopulateFuelPalette();

            ReloadValuesFromConfig();
        }

        public static void ReloadValuesFromConfig()
        {
            foreach (KeyValuePair<Block, BlockTypes> blockEntry in blocks)
                blockEntry.Key.ReloadValuesFromConfig();

            foreach (KeyValuePair<string, Block> blockEntry in BlockPalette)
                blockEntry.Value.ReloadValuesFromConfig();

            foreach(KeyValuePair<string, Fuel> fuelEntry in FuelPalette)
                fuelEntry.Value.ReloadValuesFromConfig();
        }

        private static void PopulateBlockPalette()
        {
            BlockPalette.Add("Air", new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition));
            BlockPalette.Add("FuelCell", new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition));

            foreach (Cooler cooler in coolers)
                BlockPalette.Add(cooler.CoolerType.ToString(), cooler);
            foreach (Moderator moderator in moderators)
                BlockPalette.Add(moderator.ModeratorType.ToString(), moderator);
        }

        private static void PopulateBlocks()
        {
            blocks.Add(new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition), BlockTypes.Air);
            blocks.Add(new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition), BlockTypes.FuelCell);

            foreach (Cooler cooler in coolers)
                blocks.Add(cooler, BlockTypes.Cooler);
            foreach (Moderator moderator in moderators)
                blocks.Add(moderator, BlockTypes.Moderator);
        }

        private static void PopulateCoolers(bool active = false)
        {
            foreach (KeyValuePair<string, CoolerValues> coolerEntry in Configuration.Coolers)
            {
                CoolerValues cv = coolerEntry.Value;
                CoolerTypes parsedType;
                if (Enum.TryParse(coolerEntry.Key, out parsedType))
                    coolers.Add(new Cooler(coolerEntry.Key, Textures[coolerEntry.Key], parsedType, cv.HeatActive, cv.HeatPassive, cv.Requirements, dummyPosition, active));
                else
                    throw new ArgumentException("Unexpected cooler type in config!");
            }
        }

        private static void PopulateModerators()
        {
            moderators.Add(new Moderator("Graphite", ModeratorTypes.Graphite, Textures["Graphite"], dummyPosition));
            moderators.Add(new Moderator("Beryllium", ModeratorTypes.Beryllium, Textures["Beryllium"], dummyPosition));
        }

        private static void PopulateFuelPalette()
        {
            foreach (KeyValuePair<string, FuelValues> fuel in Configuration.Fuels)
                FuelPalette.Add(fuel.Key, new Fuel(fuel.Key, fuel.Value.BasePower, fuel.Value.BaseHeat, fuel.Value.FuelTime));
        }

        public static Block BlockToPlace(Block previousBlock)
        {
            switch (selectedBlock.BlockType)
            {
                case BlockTypes.Air:
                    return new Block("Air", BlockTypes.Air, Textures["Air"], previousBlock.Position);
                case BlockTypes.Cooler:
                    return new Cooler((Cooler)selectedBlock, previousBlock.Position);
                case BlockTypes.Moderator:
                    return new Moderator((Moderator)selectedBlock, previousBlock.Position);
                case BlockTypes.FuelCell:
                    return new FuelCell((FuelCell)selectedBlock, previousBlock.Position);
                default:
                    return new Block("Air", BlockTypes.Air, Textures["Air"], previousBlock.Position);
            }
        }

        public static bool PlacingSameBlock(Block block, MouseButtons placementMethod)
        {
            switch (placementMethod)
            {
                case MouseButtons.Left:
                    if (block.BlockType == selectedBlock.BlockType)
                    {
                        if (block.BlockType == BlockTypes.Cooler)
                            return ((Cooler)block).CoolerType == ((Cooler)selectedBlock).CoolerType & ((Cooler)block).Active == ((Cooler)selectedBlock).Active;
                        else
                            return true;
                    }
                    else
                        return false;
                case MouseButtons.Right:
                    return block.BlockType == BlockTypes.Air;
                case MouseButtons.Middle:
                    return block.BlockType == BlockTypes.FuelCell;
                case MouseButtons.XButton1:
                case MouseButtons.None:
                case MouseButtons.XButton2:
                default:
                    return false;
            }
        }
    }

    public enum BlockTypes
    {
        Air,
        Cooler,
        Moderator,
        FuelCell,
        Casing,
    }
}
