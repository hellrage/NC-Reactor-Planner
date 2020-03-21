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
using System.Numerics;
using NC_Reactor_Planner.Properties;

namespace NC_Reactor_Planner
{
    /// <summary>
    /// This class holds all the dummy blocks and textures. It is repopulated as configuration changes.
    /// Various classes refer to this for creating blocks, checking placement validity, checking the currently selected fuel, etc.
    /// This also holds the update order for heatsinks.
    /// Down below is an Enum for BlockTypes!
    /// </summary>
    public static class Palette
    {
        /// <summary>
        /// This is an element of UI that handles the different block types. It is loaded into PlannerUI.
        /// </summary>
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
                Size = new Size(200, height + namestripHeight + 6);
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
                //g.CompositingMode = CompositingMode.SourceCopy;
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
                int blockIndex = -1;
                try
                {
                    if (cellX != newCellX | cellZ != newCellZ)
                    {
                        cellX = newCellX;
                        cellZ = newCellZ;
                        blockIndex = cellZ * (Width / (blockSide + 2 * spacing)) + cellX;
                        if (blockIndex < BlockPalette.Count && blockIndex >= 0)
                        {
                            using (Graphics g = CreateGraphics())
                            {
                                DrawNamestring(g, BlockPalette.Values.ElementAt(blockIndex).DisplayName);
                                paletteToolTip.Show(BlockPalette.Values.ElementAt(blockIndex).GetToolTip(), this, (cellX + 1) * (blockSide + 2 * spacing), (cellZ + 1) * (blockSide + 2 * spacing) + namestripHeight);
                            }
                        }
                    }
                    //throw new ArgumentException("message!");
                }
                catch(Exception exception)
                {
                    StringBuilder report = new StringBuilder();
                    report.AppendLine($"cellCoords: {newCellX}; {newCellZ}");
                    report.AppendLine($"blockIndex: {blockIndex}");
                    report.AppendLine($"Blocks in palette: {BlockPalette.Count}");
                    report.AppendLine(exception.Message);
                    MessageBox.Show(report.ToString());
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
                int newX = 0;
                int newZ = 0;

                if (e.X >= Width - spacing - Width % (blockSide + 2 * spacing))
                    newX = Width - (blockSide + 2 * spacing);
                else
                    newX = Math.Max(spacing, e.X);
                newX = (int)Math.Floor((double)newX / (blockSide + 2 * spacing));

                if (e.Y >= Height)
                    newZ = Height - namestripHeight - spacing;
                else
                    newZ = Math.Max(e.Y - namestripHeight, namestripHeight);
                newZ = (int)Math.Floor((double)newZ / (blockSide + 2 * spacing));

                //return Tuple.Create(((e.X > Width - spacing - Width % (blockSide+2*spacing)) ? Width - (blockSide + 2 * spacing) : e.X) / (blockSide + 2*spacing),
                //                    ((e.Y - namestripHeight > Height) ? Height : e.Y - namestripHeight) / (blockSide + 2*spacing));
                return Tuple.Create(newX, newZ);
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
        public static readonly Vector3 dummyPosition = new Vector3(-1, -1, -1);
        public static readonly Casing dummyCasing = new Casing("Casing", null, dummyPosition);
        public static readonly Block dummyReflector = new Block("Dummy reflector", BlockTypes.Reflector, null, dummyPosition);
        public static readonly Block dummyModerator = new Block("Dummy moderator", BlockTypes.Moderator, null, dummyPosition);
        public static Fuel SelectedFuel { get; set; }
        public static PalettePanel PaletteControl { get; private set; }
        
        private static ToolTip paletteToolTip;
        private static Block selectedBlock;

        public static List<string> UpdateOrder { get; private set; }
        public static List<string> NeutronSourceNames { get; private set; }
        public static Dictionary<string, List<Func<Vector3, List<string>, bool>>> HeatSinkValidators { get; private set; }

        private static readonly List<string> T0Blocks = new List<string>() { "Air", "FuelCell", "Moderator", "Reflector", "Casing" };

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
                    MessageBox.Show(textureFile + " Is not a 16x16 image!");
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
            
            UpdateNeutronSourceNames();
        }

        private static void PopulateBlockPalette()
        {
            BlockPalette.Add("Air", new Block("Air", BlockTypes.Air, Textures["Air"], dummyPosition));
            BlockPalette.Add("FuelCell", new FuelCell("FuelCell", Textures["FuelCell"], dummyPosition, new Fuel()));

            HeatSinkValidators = new Dictionary<string, List<Func<Vector3, List<string>, bool>>>();
            foreach (KeyValuePair<string, HeatSinkValues> heatSinkEntry in Configuration.HeatSinks)
            {
                HeatSinkValues cv = heatSinkEntry.Value;
                BlockPalette.Add(heatSinkEntry.Key, new HeatSink(heatSinkEntry.Key, (Textures.ContainsKey(heatSinkEntry.Key)) ? Textures[heatSinkEntry.Key] : Textures["NoTexture"], heatSinkEntry.Key, dummyPosition, ConstructValidatorsAndDependencies(heatSinkEntry.Key)));
            }

            foreach (KeyValuePair<string, ModeratorValues> moderatorEntry in Configuration.Moderators)
            {
                ModeratorValues mv = moderatorEntry.Value;
                BlockPalette.Add(moderatorEntry.Key, new Moderator(moderatorEntry.Key, moderatorEntry.Key, Textures[moderatorEntry.Key], dummyPosition));
            }

            BlockPalette.Add("Conductor", new Conductor("Conductor", Textures["Conductor"], dummyPosition));
            foreach (KeyValuePair<string, ReflectorValues> reflectorEntry in Configuration.Reflectors)
            {
                ReflectorValues mv = reflectorEntry.Value;
                BlockPalette.Add(reflectorEntry.Key, new Reflector(reflectorEntry.Key, reflectorEntry.Key, Textures[reflectorEntry.Key.Replace('-', '_')], dummyPosition));
            }
        }

        private static void PopulateFuelPalette()
        {
            var fuelList = new List<KeyValuePair<string, Fuel>>();
            foreach (KeyValuePair<string, FuelValues> fuel in Configuration.Fuels)
                fuelList.Add(new KeyValuePair<string, Fuel>(fuel.Key, new Fuel(fuel.Key)));
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

        public static List<string> ConstructValidatorsAndDependencies(string heatSinkType)
        {
            List<Func<Vector3, List<string>, bool>> Validators = new List<Func<Vector3, List<string>, bool>>();
            List<string> Dependencies = new List<string>();

            string[] numberStrings = new string[] { "One", "Two", "Three", "Four", "Five", "Six" };
            Dictionary<string, byte> nums = new Dictionary<string, byte>();
            for (int i = 0; i < 6; i++)
                nums.Add(numberStrings[i], (byte)(i + 1));

            string[] rules = Configuration.HeatSinks[heatSinkType].Requirements.Split(';');
            foreach (string rule in rules)
            {
                var words = rule.Trim().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                switch (words[0])
                {
                    case "Exactly":
                        if (!rule.Contains("heatsink"))
                            if (nums[words[1]] > 1)
                                words[2] = words[2].Substring(0, words[2].Length - 1);
                        if (!Dependencies.Contains(words[2]))
                            Dependencies.Add(words[2]);

                        if (words[2] == "Moderator")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyModerator, nums[words[1]], true); });
                        else if (words[2] == "Casing")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyCasing, nums[words[1]], true); });
                        else if (words[2] == "Reflector")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyReflector, nums[words[1]], true); });
                        else
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.BlockPalette[words[2]], nums[words[1]], true); });
                        break;
                    case "Vertex":
                        List<Block> vertexBlocks = new List<Block>();
                        List<string> vbNames = new List<string>() { words[3].Replace(",", ""), words[words.IndexOf("One", 3) + 1].Replace(",", ""), words[words.IndexOf("One", 6) + 1] };
                        foreach (string name in vbNames)
                        {
                            if (!Dependencies.Contains(name))
                                Dependencies.Add(name);

                            if (name == "Moderator")
                                vertexBlocks.Add(Palette.dummyModerator);
                            else if (name == "Casing")
                                vertexBlocks.Add(Palette.dummyCasing);
                            else if (words[1] == "Reflector")
                                vertexBlocks.Add(Palette.dummyReflector);
                            else
                                vertexBlocks.Add(Palette.BlockPalette[name]);
                        }
                        Validators.Add((pos, errs) => { return HeatSink.HasVertex(pos, errs, vertexBlocks); });
                        break;
                    case "Axial":
                        if (!rule.Contains("heatsink"))
                            words[1] = words[1].Substring(0, words[1].Length - 1);

                        if (!Dependencies.Contains(words[1]))
                            Dependencies.Add(words[1]);

                        if (words[1] == "Moderator")
                            Validators.Add((pos, errs) => { return HeatSink.HasAxial(pos, errs, Palette.dummyModerator); });
                        else if (words[1] == "Casing")
                            Validators.Add((pos, errs) => { return HeatSink.HasAxial(pos, errs, Palette.dummyCasing); });
                        else if(words[1] == "Reflector")
                            Validators.Add((pos, errs) => { return HeatSink.HasAxial(pos, errs, Palette.dummyReflector); });
                        else
                            Validators.Add((pos, errs) => { return HeatSink.HasAxial(pos, errs, Palette.BlockPalette[words[1]]); });
                        break;
                    case "One":
                    case "Two":
                    case "Three":
                    case "Four":
                    case "Five":
                    case "Six":
                        if (!rule.Contains("heatsink"))
                            if (nums[words[0]] > 1)
                                words[1] = words[1].Substring(0, words[1].Length - 1);

                        if (!Dependencies.Contains(words[1]))
                            Dependencies.Add(words[1]);

                        if (words[1] == "Moderator")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyModerator, nums[words[0]], false); });
                        else if (words[1] == "Casing")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyCasing, nums[words[0]], false); });
                        else if (words[1] == "Reflector")
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.dummyReflector, nums[words[0]], false); });
                        else
                            Validators.Add((pos, errs) => { return HeatSink.HasAdjacent(pos, errs, Palette.BlockPalette[words[1]], nums[words[0]], false); });
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show(string.Format("Invalid rule string in {0}!\r\n{1}", heatSinkType, rule));
                        Validators.Clear();
                        HeatSinkValidators[heatSinkType].Clear();
                        return new List<string>();
                }
            }
            HeatSinkValidators[heatSinkType] = Validators;
            return Dependencies;
        }

        public static void UpdateNeutronSourceNames()
        {
            NeutronSourceNames = new List<string>();
            foreach (var ns in Configuration.NeutronSources)
            {
                NeutronSourceNames.Add(ns.Key);
            }
            NeutronSourceNames.Sort();
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
                    return new Reflector((Reflector)selectedBlock, previousBlock.Position);
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
