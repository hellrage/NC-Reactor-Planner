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
            public static readonly int blockSide = 32;
            public static readonly int spacing = 3;
            public static readonly int namestripHeight = 23;

            public PalettePanel()
            {
                ResetSize();
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                cellX = -1;
                cellZ = -1;
            }

            public void ResetSize()
            {
                int height = (int)Math.Ceiling(((double)(BlockPalette.Keys.Count) / (Width / (blockSide + 2 * spacing)))) * (blockSide + 2 * spacing);
                Size = new Size(200, height + namestripHeight);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Redraw(e.Graphics);
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
                foreach (var block in BlockPalette)
                {
                    g.DrawImage(block.Value.Texture, x, y, blockSide, blockSide);
                    x += blockSide + 2 * spacing;
                    if(x+blockSide+spacing > Width)
                    {
                        x = spacing;
                        y += blockSide + 2 * spacing;
                    }
                }
            }

            private void DrawNamestring(Graphics g, string name)
            {
                g.FillRectangle(new SolidBrush(DefaultBackColor), new Rectangle(0, 0, Width, namestripHeight));
                g.DrawString(name, new Font(Font, FontStyle.Bold), Brushes.Black, new PointF(spacing, spacing));
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
                    if (blockIndex < BlockPalette.Count)
                    {
                        DrawNamestring(CreateGraphics(), BlockPalette.Values.ElementAt(blockIndex).DisplayName);
                        paletteToolTip.Show(BlockPalette.Values.ElementAt(blockIndex).GetToolTip(), this, (cellX + 1) * (blockSide + 2 * spacing), (cellZ + 1) * (blockSide + 2 * spacing) + namestripHeight);
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
                if (blockIndex < BlockPalette.Count)
                {
                    selectedBlock = BlockPalette.Values.ElementAt(blockIndex);
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
        public static readonly Casing dummyCasing = new Casing("Casing", null, dummyPosition);
        public static Fuel SelectedFuel { get; set; }
        public static PalettePanel PaletteControl { get; private set; }
        
        private static ToolTip paletteToolTip;
        private static Block selectedBlock;

        public static List<string> UpdateOrder { get; private set; }

        private static List<string> T0Blocks = new List<string>() { "Air", "FuelCell", "Moderator", "Reflector", "Casing" };

        public static void Load()
        {
            if(Textures == null)
                LoadTextures();
            LoadPalette();
            selectedBlock = BlockPalette["Air"];
            PaletteControl = new PalettePanel();
            paletteToolTip = new ToolTip();
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

            if (!Directory.Exists("Textures"))
                return;

            string[] customTextures = Directory.GetFiles("Textures", "*.png");
            foreach (var textureFile in customTextures)
            {
                FileInfo fi = new FileInfo(textureFile);
                Bitmap ct = new Bitmap(fi.FullName);
                if (ct.Size.Height != 16 || ct.Size.Width != 16)
                {
                    MessageBox.Show(textureFile + "Is not a 16x16 image!");
                    ct.Dispose();
                    continue;
                }
                else
                {
                    string textureName = fi.Name.Replace(".png","");
                    if (Textures.ContainsKey(textureName))
                    {
                        Textures[textureName].Dispose();
                        Textures[textureName] = ct;
                    }
                    else
                        Textures.Add(textureName, ct);
                }
            }
        }

        public static void LoadPalette()
        {
            BlockPalette = new Dictionary<string, Block>();
            FuelPalette = new Dictionary<string, Fuel>();

            PopulateBlockPalette();
            PopulateFuelPalette();

            ReloadValuesFromConfig();
        }

        public static void ReloadValuesFromConfig()
        {
            foreach (var heatsink in Configuration.HeatSinks)
            {
                if (!BlockPalette.ContainsKey(heatsink.Key))
                    BlockPalette.Add(heatsink.Key, new HeatSink(heatsink.Key, (Textures.ContainsKey(heatsink.Key)) ? Textures[heatsink.Key] : Textures["NoTexture"], heatsink.Key, heatsink.Value.HeatPassive, heatsink.Value.Requirements, dummyPosition));
            }

            foreach (KeyValuePair<string, Block> blockEntry in BlockPalette)
            {
                blockEntry.Value.ReloadValuesFromConfig();
                if (blockEntry.Value is HeatSink hs)
                    hs.ConstructValidators();
            }

            foreach(KeyValuePair<string, Fuel> fuelEntry in FuelPalette)
                fuelEntry.Value.ReloadValuesFromConfig();
        }

        private static void PopulateBlockPalette()
        {
            BlockPalette.Add("Air", new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition));
            BlockPalette.Add("FuelCell", new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition, new Fuel()));

            foreach (KeyValuePair<string, HeatSinkValues> heatSinkEntry in Configuration.HeatSinks)
            {
                HeatSinkValues cv = heatSinkEntry.Value;
                BlockPalette.Add(heatSinkEntry.Key, new HeatSink(heatSinkEntry.Key, (Textures.ContainsKey(heatSinkEntry.Key)) ? Textures[heatSinkEntry.Key] : Textures["NoTexture"], heatSinkEntry.Key, cv.HeatPassive, cv.Requirements, dummyPosition));
            }

            foreach (KeyValuePair<string, ModeratorValues> moderatorEntry in Configuration.Moderators)
            {
                ModeratorValues mv = moderatorEntry.Value;
                ModeratorTypes parsedType;
                if (Enum.TryParse(moderatorEntry.Key, out parsedType))
                    BlockPalette.Add(moderatorEntry.Key, new Moderator(moderatorEntry.Key, parsedType, Textures[moderatorEntry.Key], dummyPosition, mv.FluxFactor, mv.EfficiencyFactor));
                else
                    throw new ArgumentException("Unexpected moderator type in config: " + moderatorEntry.Key);
            }

            BlockPalette.Add("Conductor", new Conductor("Conductor", Textures["Conductor"], dummyPosition));
            BlockPalette.Add("Reflector", new Reflector("Reflector", Textures["Reflector"], dummyPosition));
        }

        private static void PopulateFuelPalette()
        {
            var fuelList = new List<KeyValuePair<string, Fuel>>();
            foreach (KeyValuePair<string, FuelValues> fuel in Configuration.Fuels)
                fuelList.Add(new KeyValuePair<string, Fuel>(fuel.Key, new Fuel(fuel.Key, fuel.Value.BaseEfficiency, fuel.Value.BaseHeat, fuel.Value.FuelTime, fuel.Value.CriticalityFactor)));
            fuelList.Sort((x, y) => x.Value.CriticalityFactor.CompareTo(y.Value.CriticalityFactor));
            FuelPalette = new Dictionary<string, Fuel>();
            foreach (var kvp in fuelList)
                FuelPalette.Add(kvp.Key, kvp.Value);
        }

        public static void SetHeatSinkUpdateOrder()
        {
            UpdateOrder = new List<string>();
            List<string> deps;
            foreach (var entry in BlockPalette)
            {
                if (!(entry.Value is HeatSink hs))
                    continue;

                deps = new List<string>(hs.Dependencies);

                foreach (string dep in T0Blocks)
                    deps.Remove(dep);
                int index = 0;
                while (deps.Count != 0 & index < UpdateOrder.Count)
                {
                    deps.Remove(UpdateOrder[index]);
                    ++index;
                }
                UpdateOrder.Insert(index, entry.Key);

            }
        }

        public static Block BlockToPlace(Block previousBlock)
        {
            switch (selectedBlock.BlockType)
            {
                case BlockTypes.Air:
                    return new Block("Air", BlockTypes.Air, Textures["Air"], previousBlock.Position);
                case BlockTypes.HeatSink:
                    return new HeatSink((HeatSink)selectedBlock, previousBlock.Position);
                case BlockTypes.Moderator:
                    return new Moderator((Moderator)selectedBlock, previousBlock.Position);
                case BlockTypes.FuelCell:
                    return new FuelCell((FuelCell)selectedBlock, previousBlock.Position, SelectedFuel);
                case BlockTypes.Conductor:
                    return new Conductor("Conductor", Textures["Conductor"], previousBlock.Position);
                case BlockTypes.Reflector:
                    return new Reflector("Reflector", Textures["Reflector"], previousBlock.Position);
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
                if ((selectedBlock is FuelCell fuelCell) && block is FuelCell placedFuelCell)
                    if (fuelCell.UsedFuel == placedFuelCell.UsedFuel)
                        return true;
                    else
                        return false;
                else
                    return true;

            return false;
        }
    }

    public enum BlockTypes
    {
        Air,
        HeatSink,
        Moderator,
        FuelCell,
        Casing,
        Conductor,
        Reflector,
    }
}
