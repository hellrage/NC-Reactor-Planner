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
            private static int blockSide = 32;
            private static int spacing = 3;
            private static int namestripHeight = 23;

            public PalettePanel()
            {
                Location = new Point(137, 140);
                Size = new Size(200, 252);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                cellX = -1;
                cellZ = -1;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Redraw(e.Graphics);
            }

            internal void ResetHighlight()
            {
                Xhighlight = 0;
                Zhighlight = 0;
            }

            public void Redraw(Graphics g)
            {
                DrawNamestring(g, selectedBlock.DisplayName);
                DrawHighlightRectangle(g, Xhighlight, Zhighlight);
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
                    if (x + blockSide + spacing > Width)
                    {
                        x = spacing;
                        y += blockSide + 2 * spacing;
                    }
                }
            }

            private void DrawNamestring(Graphics g, string name)
            {
                g.FillRectangle(new SolidBrush(DefaultBackColor), new Rectangle(0, 0, Width, namestripHeight));
                g.DrawString(name, new Font(Font.FontFamily, 10, FontStyle.Bold), Brushes.Black, new PointF(spacing, spacing));
            }

            private void DrawHighlightRectangle(Graphics g, int cellX, int cellZ)
            {
                g.DrawRectangle(new Pen(Color.Blue, 3), cellX * (blockSide + 2 * spacing), namestripHeight + cellZ * (blockSide + 2 * spacing), blockSide + 2 * spacing, blockSide + 2 * spacing);
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
                return Tuple.Create(((e.X > Width - spacing - Width % (blockSide + 2 * spacing)) ? Width - (blockSide + 2 * spacing) : e.X) / (blockSide + 2 * spacing),
                                    ((e.Y - namestripHeight > Height) ? Height : e.Y - namestripHeight) / (blockSide + 2 * spacing));
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
            if (Textures == null)
                LoadTextures();
            PaletteControl = new PalettePanel();
            paletteToolTip = new ToolTip();
            LoadPalette();
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

            PopulateHeatSinks(active);
            PopulateModerators();
            PopulateBlocks();

            PopulateBlockPalette();
            PopulateFuelPalette();

            ReloadValuesFromConfig();

            selectedBlock = BlockPalette["Air"];
            PaletteControl.ResetHighlight();
            PaletteControl.Refresh();
        }

        public static void ReloadValuesFromConfig()
        {
            foreach (KeyValuePair<Block, BlockTypes> blockEntry in blocks)
                blockEntry.Key.ReloadValuesFromConfig();

            foreach (KeyValuePair<string, Block> blockEntry in BlockPalette)
                blockEntry.Value.ReloadValuesFromConfig();

            foreach (KeyValuePair<string, Fuel> fuelEntry in FuelPalette)
                fuelEntry.Value.ReloadValuesFromConfig();
        }

        private static void PopulateBlockPalette()
        {
            BlockPalette.Add("Air", new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition));
            BlockPalette.Add("FuelCell", new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition));

            foreach (Cooler heatSink in coolers)
                BlockPalette.Add(heatSink.DisplayName, heatSink);
            foreach (Moderator moderator in moderators)
                BlockPalette.Add(moderator.DisplayName, moderator);
        }

        private static void PopulateBlocks()
        {
            blocks.Add(new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition), BlockTypes.Air);
            blocks.Add(new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition), BlockTypes.FuelCell);

            foreach (Cooler heatSink in coolers)
                blocks.Add(heatSink, BlockTypes.Cooler);
            foreach (Moderator moderator in moderators)
                blocks.Add(moderator, BlockTypes.Moderator);
        }

        private static void PopulateHeatSinks(bool active = false)
        {
            foreach (KeyValuePair<string, CoolerValues> heatSinkEntry in Configuration.Coolers)
            {
                CoolerValues cv = heatSinkEntry.Value;
                CoolerTypes parsedType;
                if (Enum.TryParse(heatSinkEntry.Key, out parsedType))
                    coolers.Add(new Cooler(heatSinkEntry.Key, Textures[heatSinkEntry.Key], parsedType, cv.HeatActive, cv.HeatPassive, cv.Requirements, dummyPosition, active));
                else
                    throw new ArgumentException("Unexpected heatsink type in config!");
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
            string blockToPlace = "Null";
            switch (placementMethod)
            {
                case MouseButtons.Left:
                    blockToPlace = selectedBlock.DisplayName;
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    blockToPlace = "Air";
                    break;
                case MouseButtons.Middle:
                    blockToPlace = "FuelCell";
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
            if (block.DisplayName == blockToPlace)
                return true;
            else
                return false;
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
