using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;

namespace NC_Reactor_Planner
{
    public struct CompressedSaveFile
    {
        public Version SaveVersion;
        public List<Dictionary<string, List<Point3D>>> CompressedReactor;
        public Size3D InteriorDimensions;
        public Fuel UsedFuel;

        public CompressedSaveFile(Version sv, List<Dictionary<string, List<Point3D>>> cr, Size3D id, Fuel uf)
        {
            SaveVersion = sv;
            CompressedReactor = cr;
            InteriorDimensions = id;
            UsedFuel = uf;
        }
    }

    public static class Reactor
    {
        public static Block[,,] blocks;
        public static List<ReactorGridLayer> layers;
        public static Size3D interiorDims;
        private static Version saveVersion;


        public static Dictionary<string, List<Cooler>> coolers;
        public static List<FuelCell> fuelCells;
        public static Dictionary<string, List<Moderator>> moderators;

        public static List<string> checkOrder = new List<string> { "Water", "Redstone", "Quartz", "Magnesium", "Emerald", "Enderium", "Gold", "Lapis", "Glowstone", "Diamond", "Cryotheum", "Tin", "Helium", "Copper", "Iron" };

        public static List<Vector3D> sixAdjOffsets = new List<Vector3D> { new Vector3D(-1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), new Vector3D(0, 0, 1) };// x+-1, y+-1, z+-1
        public static List<Fuel> fuels = new List<Fuel>();

        public static double totalCoolingPerTick = 0;
        public static Dictionary<string, double> totalCoolingPerType;
        public static double totalHeatPerTick = 0;
        public static double totalEnergyPerTick = 0;

        public static double energyMultiplier = 0;
        public static double heatMultiplier = 0;
        public static double efficiency = 0;
        public static double heatMulti = 0;

        public const double fuelEnergyMulti = 3;
        public const double fuelHeatMulti = 1.2;

        public static Fuel usedFuel;

        public static string fuelName = "LEU-235";
        public static double fuelBasePower = 120;
        public static double fuelBaseHeat = 50;

        static Reactor()
        {
            PopulateFuels();
        }

        public static void PopulateFuels()
        {
            fuels.Add(new Fuel("TBU", "Th-232", "—", 60, 18, 144000));
            fuels.Add(new Fuel("TBU Oxide", "Th-232", "—", 84, 22.5, 144000));
            fuels.Add(new Fuel("LEU-233", "U-233", "U-238", 144, 60, 64000));
            fuels.Add(new Fuel("LEU-233 Oxide", "U-233", "U-238", 201.6, 75, 64000));
            fuels.Add(new Fuel("HEU-233", "U-233", "U-238", 576, 360, 64000));
            fuels.Add(new Fuel("HEU-233 Oxide", "U-233", "U-238", 806.4, 450, 64000));
            fuels.Add(new Fuel("LEU-235", "U-235", "U-238", 120, 50, 72000));
            fuels.Add(new Fuel("LEU-235 Oxide", "U-235", "U-238", 168, 62.5, 72000));
            fuels.Add(new Fuel("HEU-235", "U-235", "U-238", 480, 300, 72000));
            fuels.Add(new Fuel("HEU-235 Oxide", "U-235", "U-238", 672, 375, 72000));
            fuels.Add(new Fuel("LEN-236", "Np-236", "Np-237", 90, 36, 102000));
            fuels.Add(new Fuel("LEN-236 Oxide", "Np-236", "Np-237", 126, 45, 102000));
            fuels.Add(new Fuel("HEN-236", "Np-236", "Np-237", 360, 216, 102000));
            fuels.Add(new Fuel("HEN-236 Oxide", "Np-236", "Np-237", 504, 270, 102000));
            fuels.Add(new Fuel("LEP-239", "Pu-239", "Pu-242", 105, 40, 92000));
            fuels.Add(new Fuel("LEP-239 Oxide", "Pu-239", "Pu-242", 147, 50, 92000));
            fuels.Add(new Fuel("HEP-239", "Pu-239", "Pu-242", 420, 240, 92000));
            fuels.Add(new Fuel("HEP-239 Oxide", "Pu-239", "Pu-242", 588, 300, 92000));
            fuels.Add(new Fuel("LEP-241", "Pu-241", "Pu-242", 165, 70, 60000));
            fuels.Add(new Fuel("LEP-241 Oxide", "Pu-241", "Pu-242", 231, 87.5, 60000));
            fuels.Add(new Fuel("HEP-241", "Pu-241", "Pu-242", 660, 420, 60000));
            fuels.Add(new Fuel("HEP-241 Oxide", "Pu-241", "Pu-242", 924, 525, 60000));
            fuels.Add(new Fuel("MOX-239", "Pu-239", "U-238", 155.4, 57.5, 84000));
            fuels.Add(new Fuel("MOX-241", "Pu-241", "U-238", 243.6, 97.5, 56000));
            fuels.Add(new Fuel("LEA-242", "Am-242", "Am-243", 192, 94, 54000));
            fuels.Add(new Fuel("LEA-242 Oxide", "Am-242", "Am-243", 268.8, 117.5, 54000));
            fuels.Add(new Fuel("HEA-242", "Am-242", "Am-243", 768, 564, 54000));
            fuels.Add(new Fuel("HEA-242 Oxide", "Am-242", "Am-243", 1075.2, 705, 54000));
            fuels.Add(new Fuel("LECm-243", "Cm-243", "Cm-246", 210, 112, 52000));
            fuels.Add(new Fuel("LECm-243 Oxide", "Cm-243", "Cm-246", 294, 140, 52000));
            fuels.Add(new Fuel("HECm-243", "Cm-243", "Cm-246", 840, 672, 52000));
            fuels.Add(new Fuel("HECm-243 Oxide", "Cm-243", "Cm-246", 1176, 840, 52000));
            fuels.Add(new Fuel("LECm-245", "Cm-245", "Cm-246", 162, 68, 68000));
            fuels.Add(new Fuel("LECm-245 Oxide", "Cm-245", "Cm-246", 226.8, 85, 68000));
            fuels.Add(new Fuel("HECm-245", "Cm-245", "Cm-246", 648, 408, 68000));
            fuels.Add(new Fuel("HECm-245 Oxide", "Cm-245", "Cm-246", 907.2, 510, 68000));
            fuels.Add(new Fuel("LECm-247", "Cm-247", "Cm-246", 138, 54, 78000));
            fuels.Add(new Fuel("LECm-247 Oxide", "Cm-247", "Cm-246", 193.2, 67.5, 78000));
            fuels.Add(new Fuel("HECm-247", "Cm-247", "Cm-246", 552, 324, 78000));
            fuels.Add(new Fuel("HECm-247 Oxide", "Cm-247", "Cm-246", 772.8, 405, 78000));
            fuels.Add(new Fuel("LEB-248", "Bk-248", "Bk-247", 135, 52, 86000));
            fuels.Add(new Fuel("LEB-248 Oxide", "Bk-248", "Bk-247", 189, 65, 86000));
            fuels.Add(new Fuel("HEB-248", "Bk-248", "Bk-247", 540, 312, 86000));
            fuels.Add(new Fuel("HEB-248 Oxide", "Bk-248", "Bk-247", 756, 390, 86000));
            fuels.Add(new Fuel("LECf-249", "Cf-249", "Cf-252", 216, 116, 60000));
            fuels.Add(new Fuel("LECf-249 Oxide", "Cf-249", "Cf-252", 302.4, 145, 60000));
            fuels.Add(new Fuel("HECf-249", "Cf-249", "Cf-252", 864, 696, 60000));
            fuels.Add(new Fuel("HECf-249 Oxide", "Cf-249", "Cf-252", 1209.6, 870, 60000));
            fuels.Add(new Fuel("LECf-251", "Cf-251", "Cf-252", 225, 120, 58000));
            fuels.Add(new Fuel("LECf-251 Oxide", "Cf-251", "Cf-252", 315, 150, 58000));
            fuels.Add(new Fuel("HECf-251", "Cf-251", "Cf-252", 900, 720, 58000));
            fuels.Add(new Fuel("HECf-251 Oxide", "Cf-251", "Cf-252", 1260, 900, 58000));
        }

        public static void InitializeReactor(int interiorX, int interiorY, int interiorZ)
        {
            interiorDims = new Size3D(interiorX, interiorY, interiorZ);
            blocks = new Block[interiorX + 2, interiorY + 2, interiorZ + 2];
            saveVersion = Assembly.GetEntryAssembly().GetName().Version;

            for (int x = 0; x < interiorX + 2; x++)
                for (int y = 0; y < interiorY + 2; y++)
                    for (int z = 0; z < interiorZ + 2; z++)
                        blocks[x, y, z] = new Block("Air", BlockTypes.Air, Palette.textures["Air"], new Point3D(x, y, z));

            for (int y = 1; y < interiorY + 1; y++)
                for (int z = 1; z < interiorZ + 1; z++)
                {
                    blocks[0, y, z] = new Casing("Casing", null, new Point3D(0, y, z));
                    blocks[interiorX + 1, y, z] = new Casing("Casing", null, new Point3D(interiorX + 1, y, z));
                }
            for (int x = 1; x < interiorX + 1; x++)
                for (int z = 1; z < interiorZ + 1; z++)
                {
                    blocks[x, 0, z] = new Casing("Casing", null, new Point3D(x, 0, z));
                    blocks[x, interiorY + 1, z] = new Casing("Casing", null, new Point3D(x, interiorY + 1, z));
                }
            for (int y = 1; y < interiorY + 1; y++)
                for (int x = 1; x < interiorX + 1; x++)
                {
                    blocks[x, y, interiorZ + 1] = new Casing("Casing", null, new Point3D(x, y, interiorZ + 1));
                    blocks[x, y, 0] = new Casing("Casing", null, new Point3D(x, y, 0));
                }
            usedFuel = fuels.First();
            UpdateStats();
            ConstructLayers();
        }

        public static void InitializeReactor(Size3D interiorDims)
        {
            InitializeReactor((int)interiorDims.X, (int)interiorDims.Y, (int)interiorDims.Z);
        }

        public static void ConstructLayers()
        {
            layers = new List<ReactorGridLayer>();
            for (int y = 1; y <= interiorDims.Y; y++)
            {
                layers.Add(new ReactorGridLayer(y));
            }
        }

        public static void ConstructLayer(int layer)
        {
            layers = new List<ReactorGridLayer>();
            layers.Add(new ReactorGridLayer(layer));
        }

        public static void CauseRedraw(object sender, EventArgs e)
        {
            if (PlannerUI.drawAllLayers)
                RedrawAllLayers();
            else
            {
                if (sender is ReactorGridLayer layer)
                    layer.Redraw();
                else if (sender is ReactorGridCell cell)
                    layers[(int)cell.block.Position.Y - 1].Redraw();
            }
        }

        public static void RecursiveRedraw(Point3D origin)
        {
            //layers[(int)origin.Y][(int)origin.X, (int)origin.Z].
        }

        public static void RedrawAllLayers()
        {
            foreach (ReactorGridLayer layer in layers)
            {
                layer.Redraw();
            }
        }

        public static void UpdateStats()
        {
            coolers = new Dictionary<string, List<Cooler>>();
            fuelCells = new List<FuelCell>();
            moderators = new Dictionary<string, List<Moderator>>
            {
                { "Graphite", new List<Moderator>() },
                { "Beryllium", new List<Moderator>() }
            };

            totalCoolingPerTick = 0;
            totalCoolingPerType = new Dictionary<string, double>();
            totalHeatPerTick = 0;
            totalEnergyPerTick = 0;

            efficiency = 0;
            energyMultiplier = 0;
            heatMultiplier = 0;

            foreach (Block block in blocks)
            {
                if (block is Cooler)
                {
                    if (coolers.ContainsKey(block.DisplayName))
                        coolers[block.DisplayName].Add((Cooler)block);
                    else
                        coolers.Add(block.DisplayName, new List<Cooler> { (Cooler)block });
                }
                else if (block is FuelCell)
                {
                    fuelCells.Add((FuelCell)block);
                    ((FuelCell)block).UpdateStats();
                }
                else if (block is Moderator)
                {
                    if (moderators.ContainsKey(block.DisplayName))
                        moderators[block.DisplayName].Add((Moderator)block);
                    else
                        moderators.Add(block.DisplayName, new List<Moderator> { (Moderator)block });
                    ((Moderator)block).UpdateStats();
                }
            }

            OrderedUpdateCoolerStats();

            foreach (KeyValuePair<string, List<Cooler>> kvp in coolers)
            {
                if (kvp.Value.Count == 0)
                    continue;
                int passiveCooling = 0;
                foreach (Cooler cooler in kvp.Value)
                    if (cooler.Active)
                        passiveCooling += cooler.HeatPassive;
                totalCoolingPerType.Add(kvp.Key, passiveCooling);
            }

            foreach (KeyValuePair<string, double> coolingPerType in totalCoolingPerType)
            {
                totalCoolingPerTick += coolingPerType.Value;
            }

            efficiency = (fuelCells.Count == 0) ? 0 : 100 * energyMultiplier / fuelCells.Count;
            heatMulti = (fuelCells.Count == 0) ? 0 : 100 * heatMultiplier / fuelCells.Count;
        }

        private static void OrderedUpdateCoolerStats()
        {
            foreach (string type in checkOrder)
            {
                if (!(coolers.ContainsKey(type)))
                    continue;
                foreach (Cooler cooler in coolers[type])
                {
                    cooler.UpdateStats();
                    //cooler.NewCheckPlacementValid();
                    //cooler.CheckPlacementValid();
                }
            }
        }

        public static string GetStatString()
        {
            string report = "";
            report += "Coolers:\r\n";
            foreach (KeyValuePair<Block, BlockTypes> pb in Palette.blocks)
            {
                if (!(pb.Key is Cooler))
                    continue;
                if (!totalCoolingPerType.ContainsKey(pb.Key.DisplayName))
                    continue;
                report += string.Format("{0,-15}\t{1,-10}\t{2,5}\t\t{3}\r\n", pb.Key.DisplayName, coolers[pb.Key.DisplayName].Count, "*  " + ((Cooler)pb.Key).HeatPassive, (int)totalCoolingPerType[pb.Key.DisplayName] + " HU/t");
            }

            report += "\r\n";
            report += "Heat:\r\n";
            report += string.Format("{0,-15}\t{1,-10}\r\n", "Fuel cells", fuelCells.Count);

            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Heat gen.", (int)totalHeatPerTick + " HU/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Cooling", (int)totalCoolingPerTick + " HU/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Heat diff.", (int)(totalHeatPerTick - totalCoolingPerTick) + " HU/t");

            report += "\r\n";
            report += "Moderators:\r\n";
            foreach (KeyValuePair<string, List<Moderator>> kvp in moderators)
            {
                if (kvp.Value.Count == 0)
                    continue;
                report += string.Format("{0,-15}\t{1,-10}\r\n", kvp.Key, kvp.Value.Count);
            }

            report += "\r\n";
            report += "Energy:\r\n";
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Energy gen.", (int)totalEnergyPerTick + " RF/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Efficiency", (int)efficiency + " %");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Heat mult.", (int)heatMulti + " %");

            report += "\r\n";
            report += "Misc:\r\n";
            int totalCasings = 0;
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Z);
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Y);
            totalCasings += (int)(2 * interiorDims.Z * interiorDims.Y);
            report += string.Format("{0,-15}\t{1,-10}\r\n", "Casings", totalCasings);
            return report;
        }

        public static void Save(FileInfo saveFile)
        {
            using (TextWriter tw = File.CreateText(saveFile.FullName))
            {
                JsonSerializer jss = new JsonSerializer();
                jss.NullValueHandling = NullValueHandling.Ignore;
                jss.Formatting = Formatting.Indented;
                jss.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;

                CompressedSaveFile csf = new CompressedSaveFile(saveVersion, CompressReactor(), interiorDims, usedFuel);
                jss.Serialize(tw, csf);
            }
        }

        public static void Load(FileInfo saveFile)
        {
            if (saveFile.Extension == ".json")
            {
                LoadCompressedReactor(saveFile.FullName);
            }
            else if (saveFile.Extension == ".ncr")
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    using (Stream stream = File.Open(saveFile.FullName, FileMode.Open))
                    {
                        saveVersion = (Version)formatter.Deserialize(stream);
                        blocks = (Block[,,])formatter.Deserialize(stream);
                        interiorDims = (Size3D)formatter.Deserialize(stream);
                        double fBP = (double)formatter.Deserialize(stream);
                        double fBH = (double)formatter.Deserialize(stream);
                        usedFuel = new Fuel("OldFormat", "--", "--", fBP, fBH, 0);
                        
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\nThis savefile was created before save versioning was in place, unable to load, sorry \"^^");
                    InitializeReactor(5, 5, 5);
                }
            }
            else
                throw new FileFormatException("Unknown save file format!");

            FinalizeLoading();
        }

        private static void FinalizeLoading()
        {
            ReloadBlockTextures();
            ReloadValuesFromSettings();
            UpdateStats();
            ConstructLayers();
        }

        private static void ReloadBlockTextures()
        {
            foreach (Block block in blocks)
            {
                if (block is Casing)
                    continue;
                block.Texture = Palette.textures[block.DisplayName];
            }
        }

        public static void ReloadValuesFromSettings()
        {
            ReloadCoolerValues();
            ReloadFuelValues();
        }

        private static void ReloadCoolerValues()
        {
            foreach (KeyValuePair<Block, BlockTypes> kvp in Palette.blocks)
                if (kvp.Key is Cooler cooler)
                    cooler.ReloadValuesFromSetttings();

            foreach (Block block in blocks)
                if (block is Cooler cooler)
                    cooler.ReloadValuesFromSetttings();

        }

        private static void ReloadFuelValues()
        {
            foreach (Fuel fuel in fuels)
            {
                fuel.ReloadValuesFromSettings();
            }
        }

        public static void SaveLayerAsImage(int layer, string fileName, int scale = 2)
        {
            Bitmap layerImage = layers[layer - 1].DrawToImage(scale);
            layerImage.Save(fileName);
            layerImage.Dispose();
        }

        public static void SaveReactorAsImage(string fileName, int statStringLines, int scale = 2, int fontSize = 24)
        {
            int layersPerRow = (int)Math.Ceiling(Math.Sqrt(interiorDims.Y));
            int rows = (int)Math.Ceiling((interiorDims.Y / layersPerRow));
            int bs = PlannerUI.blockSize;

            Point StatsRectSize = new Point(28 * fontSize, statStringLines * (fontSize + 2));

            Bitmap reactorImage = new Bitmap(Math.Max(StatsRectSize.X, (int)(layersPerRow * interiorDims.X * bs + (layersPerRow - 1) * bs)),
                                             StatsRectSize.Y + bs + (int)(rows * interiorDims.Z * bs + (rows - 1) * bs));
            using (Graphics gr = Graphics.FromImage(reactorImage))
            {
                gr.Clear(Color.LightGray);
                foreach (ReactorGridLayer layer in layers)
                {
                    Bitmap layerImage = layer.DrawToImage(scale);
                    int y = layer.Y - 1;
                    gr.DrawImage(layerImage,
                                    new Rectangle((int)((y % layersPerRow) * interiorDims.X * bs + (y % layersPerRow) * bs),
                                                StatsRectSize.Y + bs + (int)((y / layersPerRow) * interiorDims.Z * bs + (y / layersPerRow) * bs),
                                                (int)(interiorDims.X * bs), (int)(interiorDims.Z * bs)),
                                    new Rectangle(0, 0, layerImage.Size.Width, layerImage.Size.Height),
                                    GraphicsUnit.Pixel);
                    layerImage.Dispose();
                }

                gr.DrawString(GetStatString(), new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel), Brushes.Black, 0, 0);
            }
            reactorImage.Save(fileName);
            reactorImage.Dispose();
        }

        public static Block BlockAt(Point3D position)
        {
            return blocks[(int)position.X, (int)position.Y, (int)position.Z];
        }

        private static List<Dictionary<string, List<Point3D>>> CompressReactor()
        {
            int DLContainsType(string type, List<Dictionary<string, List<Point3D>>> dl)
            {
                Dictionary<string, List<Point3D>> d;
                for(int i = 0; i < dl.Count; i++)
                {
                    d = dl[i];
                    if (d.ContainsKey(type))
                    {
                        return i;
                    }
                }
                return -1;
            }

            List<Dictionary<string, List<Point3D>>> cr = new List<Dictionary<string, List<Point3D>>>();
            int n;
            foreach (Block block in blocks)
            {
                if (block is Casing | block.BlockType == BlockTypes.Air)
                    continue;

                string btype;
                if (block is Cooler cooler)
                {
                    btype = cooler.CoolerType.ToString();
                    if ((n = DLContainsType(btype, cr)) != -1)
                        cr[n][btype].Add(block.Position);
                    else
                        cr.Add(new Dictionary<string, List<Point3D>> { { btype, new List<Point3D> { block.Position } } });
                }
                else if (block is Moderator moderator)
                {
                    btype = moderator.ModeratorType.ToString();
                    if ((n = DLContainsType(btype, cr)) != -1)
                        cr[n][btype].Add(block.Position);
                    else
                        cr.Add(new Dictionary<string, List<Point3D>> { { btype, new List<Point3D> { block.Position } } });
                }
                else if (block is FuelCell)
                {
                    btype = "FuelCell";
                    if ((n = DLContainsType(btype, cr)) != -1)
                        cr[n][btype].Add(block.Position);
                    else
                        cr.Add(new Dictionary<string, List<Point3D>> { { btype, new List<Point3D> { block.Position } } });
                }

            }
            return cr;
        }

        private static void LoadCompressedReactor(string fileName)
        {
            Block restoreBlock(string type, Point3D position)
            {
                if (type == "FuelCell")
                    return new FuelCell("FuelCell", Palette.textures["FuelCell"], position);
                else if (type == "Beryllium" | type == "Graphite")
                    return new Moderator((Moderator)Palette.blockPalette[type], position);
                else
                    return new Cooler((Cooler)Palette.blockPalette[type], position);
                throw new ArgumentException("Tried to restore an invalid block");
            }

            CompressedSaveFile csf;
            using (StreamReader sr = File.OpenText(fileName))
            {
                JsonSerializer js = new JsonSerializer();
                csf = (CompressedSaveFile)js.Deserialize(sr, typeof(CompressedSaveFile));
            }

            InitializeReactor(csf.InteriorDimensions);

            foreach (Dictionary<string, List<Point3D>> d in csf.CompressedReactor)
            {
                foreach (KeyValuePair<string, List<Point3D>> kvp in d)
                {
                    foreach(Point3D pos in kvp.Value)
                        SetBlock(restoreBlock(kvp.Key, pos), pos);
                }
            }

            FinalizeLoading();
        }

        public static void SetBlock(Block block, Point3D position)
        {
            blocks[(int)position.X, (int)position.Y, (int)position.Z] = block;
        }
    }
}

