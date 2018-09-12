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
        public static readonly Version saveVersion;


        public static Dictionary<string, List<Cooler>> coolers;
        public static List<FuelCell> fuelCells;
        public static Dictionary<string, List<Moderator>> moderators;

        public static List<string> checkOrder = new List<string> { "Water", "Redstone", "Quartz", "Magnesium", "Emerald", "Enderium", "Gold", "Lapis", "Glowstone", "Diamond", "Cryotheum", "Tin", "Helium", "Copper", "Iron" };

        public static List<Vector3D> sixAdjOffsets = new List<Vector3D> { new Vector3D(-1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), new Vector3D(0, 0, 1) };// x+-1, y+-1, z+-1
        public static List<Fuel> fuels;

        public static double totalCoolingPerTick = 0;
        public static Dictionary<string, double> totalCoolingPerType;
        public static double totalHeatPerTick = 0;
        public static double totalEnergyPerTick = 0;

        public static double energyMultiplier = 0;
        public static double heatMultiplier = 0;
        public static double efficiency = 0;
        public static double heatMulti = 0;

        public static Fuel usedFuel;

        static Reactor()
        {
            saveVersion = Assembly.GetEntryAssembly().GetName().Version;
            PopulateFuels();
        }

        public static void PopulateFuels()
        {
            fuels = new List<Fuel>();
            foreach (KeyValuePair<string, FuelValues> fuelEntry in Configuration.Fuels)
            {
                FuelValues fev = fuelEntry.Value;
                fuels.Add(new Fuel(fuelEntry.Key, fev.BasePower, fev.BaseHeat, fev.FuelTime));
            }
        }

        public static void InitializeReactor(int interiorX, int interiorY, int interiorZ)
        {
            interiorDims = new Size3D(interiorX, interiorY, interiorZ);
            blocks = new Block[interiorX + 2, interiorY + 2, interiorZ + 2];

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
            DisposeClearLayers();
            layers = new List<ReactorGridLayer>();

            for (int y = 1; y <= interiorDims.Y; y++)
            {
                layers.Add(new ReactorGridLayer(y));
            }
        }

        public static void ConstructLayer(int layer)
        {
            DisposeClearLayers();
            layers = new List<ReactorGridLayer>{new ReactorGridLayer(layer)};
        }

        private static void DisposeClearLayers()
        {
            if (layers != null)
            {
                foreach (ReactorGridLayer layer in layers)
                    layer.Dispose();
                layers.Clear();
            }
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
                double passiveCooling = 0;
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
                    cooler.UpdateStats();
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
            report += "Moderators:\r\n";
            foreach (KeyValuePair<string, List<Moderator>> kvp in moderators)
            {
                if (kvp.Value.Count == 0)
                    continue;
                report += string.Format("{0,-15}\t{1,-10}\r\n", kvp.Key, kvp.Value.Count);
            }

            report += "\r\n";
            report += string.Format("{0,-15}\t{1,-10}\r\n", "Fuel cells", fuelCells.Count);

            report += "\r\n";
            report += "Heat:\r\n";
            int heatDiff = (int)(totalHeatPerTick - totalCoolingPerTick);
            int reactorVolume = (int)(interiorDims.X * interiorDims.Y * interiorDims.Z);
            int blockHeatCapacity = 25000;
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Heat gen.", (int)totalHeatPerTick + " HU/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Cooling", (int)totalCoolingPerTick + " HU/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Heat diff.", heatDiff + " HU/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Meltdown time", (heatDiff <= 0) ? "Safe" : ((reactorVolume * blockHeatCapacity) / (20 * heatDiff)).ToString() + " s");


            report += "\r\n";
            report += "Energy:\r\n";
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Energy gen.", (int)totalEnergyPerTick + " RF/t");
            report += string.Format("{0,-15}\t\t\t\t{1,-10}\r\n", "Effective E. gen.", ((heatDiff <= 0) ? ((int)totalEnergyPerTick).ToString() : ((int)((totalEnergyPerTick * -totalCoolingPerTick)/(-totalCoolingPerTick - heatDiff))).ToString()) + " RF/t");
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
                JsonSerializer jss = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                };

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
                        /*saveVersion = (Version)*/formatter.Deserialize(stream); //Version is now only updated when saving
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
            ReloadValuesFromConfig();
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

        public static void ReloadValuesFromConfig()
        {
            ReloadCoolerValues();
            ReloadFuelValues();
        }

        private static void ReloadCoolerValues()
        {
            foreach (KeyValuePair<Block, BlockTypes> kvp in Palette.blocks)
                if (kvp.Key is Cooler cooler)
                    cooler.ReloadValuesFromConfig();

            foreach (Block block in blocks)
                if (block is Cooler cooler)
                    cooler.ReloadValuesFromConfig();
        }

        private static void ReloadFuelValues()
        {
            foreach (Fuel fuel in fuels)
            {
                fuel.ReloadValuesFromConfig();
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

            Point StatsRectSize = new Point(28 * fontSize, (statStringLines + 4) * (fontSize + 2));

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
                string usedFuel = string.Format("Fuel used:\t{0}\r\nBase Power:\t{1} RF/t\r\nBase Heat:\t{2} HU/t\r\n", Reactor.usedFuel.Name, Reactor.usedFuel.BasePower.ToString(), Reactor.usedFuel.BaseHeat.ToString());
                gr.DrawString(usedFuel + "\r\n" + GetStatString(), new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel), Brushes.Black, 0, 0);
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
            blocks[(int)position.X, (int)position.Y, (int)position.Z] = block.Copy(position);
        }

        public static void ClearLayer(ReactorGridLayer layer)
        {
            for (int x = 0; x < interiorDims.X; x++)
                for (int z = 0; z < interiorDims.Z; z++)
                    SetBlock(new Block("Air", BlockTypes.Air, Palette.textures["Air"], new Point3D(x + 1, layer.Y, z + 1)), new Point3D(x + 1, layer.Y, z + 1));
            UpdateStats();
            layer.Redraw();
        }

        public static void CopyLayer(ReactorGridLayer layer)
        {
            PlannerUI.layerBuffer = new Block[layer.X, layer.Z];
            for (int x = 0; x < layer.X; x++)
                for (int z = 0; z < layer.Z; z++)
                {
                    PlannerUI.layerBuffer[x, z] = blocks[x + 1, layer.Y, z + 1];
                }
        }

        public static void PasteLayer(ReactorGridLayer layer)
        {
            if (PlannerUI.layerBuffer == null)
                return;
            if (PlannerUI.layerBuffer.Length != layer.X * layer.Z)
            {
                System.Windows.Forms.MessageBox.Show("Buffered layer size doesn't match the layout!");
                return;
            }

            for (int x = 0; x < layer.X; x++)
                for (int z = 0; z < layer.Z; z++)
                {
                    SetBlock(PlannerUI.layerBuffer[x, z], new Point3D(x + 1, layer.Y, z + 1));
                }
            UpdateStats();
            layer.Redraw();
        }

        public static void DeleteLayer(int y)
        {
            if (y == 0 | y == interiorDims.Y + 1)
                throw new ArgumentException("Tried to delete a casing layer!");

            Block[,,] newReactor = new Block[(int)interiorDims.X + 2, (int)interiorDims.Y + 1, (int)interiorDims.Z + 2];
            for (int layer = 0; layer < y; layer++)
            {
                for (int x = 0; x < interiorDims.X+2; x++)
                {
                    for (int z = 0; z < interiorDims.Z+2; z++)
                    {
                        newReactor[x, layer, z] = blocks[x, layer, z].Copy(new Point3D(x, layer, z));
                    }
                }
            }
            for (int layer = y + 1; layer <= interiorDims.Y+1; layer++)
            {
                for (int x = 0; x < interiorDims.X + 2; x++)
                {
                    for (int z = 0; z < interiorDims.Z + 2; z++)
                    {
                        newReactor[x, layer-1, z] = blocks[x, layer, z].Copy(new Point3D(x, layer-1, z));
                    }
                }
            }

            blocks = newReactor;
            interiorDims = new Size3D(interiorDims.X, interiorDims.Y - 1, interiorDims.Z);

        }

        public static void InsertLayer(int y)
        {
            Block[,,] newReactor = new Block[(int)interiorDims.X + 2, (int)interiorDims.Y + 3, (int)interiorDims.Z + 2];
            for (int layer = 0; layer < y; layer++)
            {
                for (int x = 0; x < interiorDims.X + 2; x++)
                {
                    for (int z = 0; z < interiorDims.Z + 2; z++)
                    {
                        newReactor[x, layer, z] = blocks[x, layer, z].Copy(new Point3D(x, layer, z));
                    }
                }
            }
            for (int x = 0; x < interiorDims.X + 2; x++)
            {
                for (int z = 0; z < interiorDims.Z + 2; z++)
                {
                    newReactor[x, y, z] = new Block("Air", BlockTypes.Air, Palette.textures["Air"], new Point3D(x, y, z));
                }
            }
            for (int layer = y + 1; layer < interiorDims.Y + 3; layer++)
            {
                for (int x = 0; x < interiorDims.X + 2; x++)
                {
                    for (int z = 0; z < interiorDims.Z + 2; z++)
                    {
                        newReactor[x, layer, z] = blocks[x, layer-1, z].Copy(new Point3D(x, layer, z));
                    }
                }
            }

            blocks = newReactor;
            interiorDims = new Size3D(interiorDims.X, interiorDims.Y + 1, interiorDims.Z);
        }
    }
}

