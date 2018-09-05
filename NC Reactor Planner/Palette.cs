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

        public static Dictionary<Block, BlockTypes> blocks = new Dictionary<Block, BlockTypes>();
        public static readonly Dictionary<string, Block> blockPalette;
        public static List<Block> miscBlocks = new List<Block>();
        public static List<Cooler> coolers = new List<Cooler>();
        public static List<Moderator> moderators = new List<Moderator>();
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

            blockPalette = new Dictionary<string, Block>();
            LoadPalette();

        }

        public static void LoadPalette()
        {
            PopulateBlocks();

            PopulateBlockPalette();

            ReloadValuesFromConfig();
        }

        public static void ReloadValuesFromConfig()
        {
            if (Properties.Settings.Default.Exist)
                foreach (KeyValuePair<Block, BlockTypes> blockEntry in blocks)
                    blockEntry.Key.ReloadValuesFromSetttings();
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

            blockPalette.Add("Beryllium", new Moderator("Beryllium", textures["Beryllium"], dummyPosition));
            blockPalette.Add("Graphite", new Moderator("Graphite", textures["Graphite"], dummyPosition));
        }

        private static void PopulateBlocks()
        {
            PopulateCoolers();

            blocks.Add(new Block("Air", BlockTypes.Air, textures["Air"], dummyPosition), BlockTypes.Air);
            blocks.Add(new FuelCell("FuelCell", textures["FuelCell"], dummyPosition), BlockTypes.FuelCell);

            foreach (Cooler cooler in coolers)
            {
                blocks.Add(cooler, BlockTypes.Cooler);
            }

            blocks.Add(new Moderator("Beryllium", textures["Beryllium"], dummyPosition), BlockTypes.Moderator);
            blocks.Add(new Moderator("Graphite", textures["Graphite"], dummyPosition), BlockTypes.Moderator);
        }

        //private static void PopulateMisc()
        //{
        //    miscBlocks.Add(new Block("Air", BlockTypes.Air, textures["Air"], dummyPosition));
        //    miscBlocks.Add(new FuelCell("FuelCell", textures["FuelCell"], dummyPosition));
        //}

        private static void PopulateCoolers()
        {
            coolers.Add(new Cooler("Water", textures["Water"], CoolerTypes.Water, 200, 20, "At least one Reactor Cell or active Moderator", dummyPosition));
            coolers.Add(new Cooler("Redstone", textures["Redstone"], CoolerTypes.Redstone, 4000, 80, "At least one Reactor Cell", dummyPosition));
            coolers.Add(new Cooler("Quartz", textures["Quartz"], CoolerTypes.Quartz, 6000, 80, "At least one active Moderator", dummyPosition));
            coolers.Add(new Cooler("Gold", textures["Gold"], CoolerTypes.Gold, 7000, 120, "At least one active Water cooler & active Redstone cooler", dummyPosition));
            coolers.Add(new Cooler("Glowstone", textures["Glowstone"], CoolerTypes.Glowstone, 8000, 120, "At least two active Moderators", dummyPosition));
            coolers.Add(new Cooler("Lapis", textures["Lapis"], CoolerTypes.Lapis, 9000, 100, "At least one Reactor Cell and one Reactor Casing", dummyPosition));
            coolers.Add(new Cooler("Diamond", textures["Diamond"], CoolerTypes.Diamond, 14000, 120, "One active Water cooler and one active Quartz cooler", dummyPosition));
            coolers.Add(new Cooler("Helium", textures["Helium"], CoolerTypes.Helium, 13200, 120, "ONLY one active Redstone cooler and at least one Reactor Casing", dummyPosition));
            coolers.Add(new Cooler("Enderium", textures["Enderium"], CoolerTypes.Enderium, 11000, 140, "Three Reactor Casings (has to be in a corner)", dummyPosition));
            coolers.Add(new Cooler("Cryotheum", textures["Cryotheum"], CoolerTypes.Cryotheum, 13000, 140, "At least two Reactor Cells", dummyPosition));
            coolers.Add(new Cooler("Iron", textures["Iron"], CoolerTypes.Iron, 6800, 60, "At least one active Gold cooler", dummyPosition));
            coolers.Add(new Cooler("Emerald", textures["Emerald"], CoolerTypes.Emerald, 11000, 140, "At least one active Moderator and one Reactor Cell", dummyPosition));
            coolers.Add(new Cooler("Copper", textures["Copper"], CoolerTypes.Copper, 4500, 60, "At least one active Glowstone Cooler", dummyPosition));
            coolers.Add(new Cooler("Tin", textures["Tin"], CoolerTypes.Tin, 5000, 80, "Two Lapis Coolers on opposite sides (same axis)", dummyPosition));
            coolers.Add(new Cooler("Magnesium", textures["Magnesium"], CoolerTypes.Magnesium, 8000, 100, "At least one Reactor Casing and one active Moderator", dummyPosition));

        }

        //private static void PopulateModerators()
        //{
        //    moderators.Add(new Moderator("Beryllium", textures["Beryllium"],  dummyPosition));
        //    moderators.Add(new Moderator("Graphite", textures["Graphite"],  dummyPosition));
        //}

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
            return block.DisplayName == blockToPlace;
        }

        public static Cooler GetCooler(string displayName)
        {
            foreach (Cooler cooler in coolers)
            {
                if (cooler.DisplayName == displayName)
                    return cooler;
            }
            return new Cooler(coolers.First(), dummyPosition);
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
