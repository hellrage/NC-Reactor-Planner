using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace NC_Reactor_Planner
{
    public static class Palette
    {
        public static BlockTypes selectedType = BlockTypes.Air;
        public static ReactorGridCell selectedBlock;

        public static Dictionary<Block, BlockTypes> blocks;
        public static Dictionary<string, Block> blockPalette;
        //public static List<Block> miscBlocks = new List<Block>();
        public static List<Cooler> coolers;
        //public static List<Moderator> moderators = new List<Moderator>();
        public static Dictionary<string, Bitmap> textures;

        public static Point3D dummyPosition = new Point3D(-1, -1, -1);

        static Palette()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            ResourceReader rReader = new ResourceReader(assembly.GetManifestResourceStream("NC_Reactor_Planner.Properties.Resources.resources"));
            IDictionaryEnumerator textureEnumerator = rReader.GetEnumerator();

            textures = new Dictionary<string, Bitmap>();
            while (textureEnumerator.MoveNext())
                textures.Add((string)textureEnumerator.Key, (Bitmap)textureEnumerator.Value);

            LoadPalette();
        }

        public static void LoadPalette(bool Active = false)
        {
            blocks = new Dictionary<Block, BlockTypes>();
            blockPalette = new Dictionary<string, Block>();
            coolers = new List<Cooler>();

            PopulateBlocks(Active);

            PopulateBlockPalette();

            ReloadValuesFromConfig();
        }

        public static void ReloadValuesFromConfig()
        {
            foreach (KeyValuePair<Block, BlockTypes> blockEntry in blocks)
                blockEntry.Key.ReloadValuesFromConfig();

            foreach (KeyValuePair<string, Block> blockEntry in blockPalette)
                blockEntry.Value.ReloadValuesFromConfig();
        }

        private static void PopulateBlockPalette()
        {
            //PopulateCoolers();

            blockPalette.Add("Air", new Block("Air", BlockTypes.Air, textures["Air"], dummyPosition));
            blockPalette.Add("FuelCell", new FuelCell("FuelCell", textures["FuelCell"], dummyPosition));

            foreach (Cooler cooler in coolers)
            {
                blockPalette.Add(cooler.DisplayName, cooler);
            }

            blockPalette.Add("Beryllium", new Moderator("Beryllium", ModeratorTypes.Beryllium, textures["Beryllium"], dummyPosition));
            blockPalette.Add("Graphite", new Moderator("Graphite", ModeratorTypes.Graphite, textures["Graphite"], dummyPosition));
        }

        private static void PopulateBlocks(bool Active = false)
        {
            PopulateCoolers(Active);

            blocks.Add(new Block("Air", BlockTypes.Air, textures["Air"], dummyPosition), BlockTypes.Air);
            blocks.Add(new FuelCell("FuelCell", textures["FuelCell"], dummyPosition), BlockTypes.FuelCell);

            foreach (Cooler cooler in coolers)
            {
                blocks.Add(cooler, BlockTypes.Cooler);
            }

            blocks.Add(new Moderator("Beryllium", ModeratorTypes.Beryllium, textures["Beryllium"], dummyPosition), BlockTypes.Moderator);
            blocks.Add(new Moderator("Graphite", ModeratorTypes.Graphite, textures["Graphite"], dummyPosition), BlockTypes.Moderator);
        }

        private static void PopulateCoolers(bool Active = false)
        {
            foreach (KeyValuePair<string, CoolerValues> coolerEntry in Configuration.Coolers)
            {
                CoolerValues cv = coolerEntry.Value;
                CoolerTypes parsedType;
                if (Enum.TryParse(coolerEntry.Key, out parsedType))
                    coolers.Add(new Cooler(coolerEntry.Key, textures[coolerEntry.Key], parsedType, cv.HeatActive, cv.HeatPassive, cv.Requirements, dummyPosition, Active));
                else
                    throw new ArgumentException("Unexpected cooler type in config!");
            }
        }

        public static Block BlockToPlace(Block previousBlock)
        {
            switch (selectedType)
            {
                case BlockTypes.Air:
                    return new Block("Air", BlockTypes.Air, textures["Air"], previousBlock.Position);
                case BlockTypes.Cooler:
                    return new Cooler((Cooler)selectedBlock.block, previousBlock.Position);
                case BlockTypes.Moderator:
                    return new Moderator((Moderator)selectedBlock.block, previousBlock.Position);
                case BlockTypes.FuelCell:
                    return new FuelCell((FuelCell)selectedBlock.block, previousBlock.Position);
                default:
                    return new Block("Air", BlockTypes.Air, textures["Air"], previousBlock.Position);
            }

        }

        public static bool PlacingSameBlock(Block block, MouseButtons placementMethod)
        {
            string blockToPlace = "Null";
            switch (placementMethod)
            {
                case MouseButtons.Left:
                    blockToPlace = selectedBlock.block.DisplayName;
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
                if ((selectedBlock.block is Cooler selCooler) && block is Cooler placedCooler)
                {
                    if (selCooler.Active & placedCooler.Active)
                        return true;
                }
                else
                    return true;


            return false;
        }

        public static Cooler GetCooler(string displayName)
        {
            foreach (Cooler cooler in coolers)
            {
                if (cooler.DisplayName == displayName)
                    return cooler;
            }
            //return new Cooler(coolers.First(), dummyPosition);
            throw new ArgumentException("No such cooler! Looked for: " + displayName);
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
