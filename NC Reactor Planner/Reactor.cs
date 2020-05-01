using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public struct CompressedSaveFile
    {
        public Version SaveVersion;
        public Dictionary<string, List<Vector3>> CompressedReactor;
        public Vector3 InteriorDimensions;
        public Fuel UsedFuel;

        public CompressedSaveFile(Version sv, Dictionary<string, List<Vector3>> cr, Vector3 id, Fuel uf)
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
        public static Vector3 interiorDims;
        public static readonly Version saveVersion;

        public static PlannerUI UI { get; private set; }

        public static Dictionary<string, List<Cooler>> passiveCoolers;
        public static Dictionary<string, List<Cooler>> activeCoolers;
        public static List<FuelCell> fuelCells;
        public static Dictionary<string, List<Moderator>> moderators;
        public static int totalCasings;

        public static List<string> checkOrder = new List<string> { "Water", "Redstone", "Quartz", "Magnesium", "Emerald", "Enderium", "Gold", "Lapis", "Glowstone", "Diamond", "Cryotheum", "Tin", "Helium", "Copper", "Iron" };

        public static List<Vector3> sixAdjOffsets = new List<Vector3> { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1) };// x+-1, y+-1, z+-1

        public static double totalCoolingPerTick = 0;
        public static Dictionary<string, double> totalPassiveCoolingPerType;
        public static Dictionary<string, double> totalActiveCoolingPerType;
        public static double totalHeatPerTick = 0;
        public static double totalEnergyPerTick = 0;

        public static double energyMultiplier = 0;
        public static double heatMultiplier = 0;
        public static double efficiency = 0;
        public static double heatMulti = 0;

        public static Fuel usedFuel;
        public static double maxBaseHeat = 0;
        public static double fuelDuration = 0;

        static Reactor()
        {
            saveVersion = Assembly.GetEntryAssembly().GetName().Version;
            UI = new PlannerUI();
            UI.Controls.Add(Palette.PaletteControl);
            Palette.PaletteControl.Parent = UI;
            Palette.PaletteControl.Location = UI.PalettePanelLocation;
        }

        public static void InitializeReactor(int interiorX, int interiorY, int interiorZ)
        {
            interiorDims = new Vector3(interiorX, interiorY, interiorZ);
            blocks = new Block[interiorX + 2, interiorY + 2, interiorZ + 2];

            for (int x = 0; x < interiorX + 2; x++)
                for (int y = 0; y < interiorY + 2; y++)
                    for (int z = 0; z < interiorZ + 2; z++)
                        blocks[x, y, z] = new Block("Air", BlockTypes.Air, Palette.Textures["Air"], new Vector3(x, y, z));

            for (int y = 1; y < interiorY + 1; y++)
                for (int z = 1; z < interiorZ + 1; z++)
                {
                    blocks[0, y, z] = new Casing("Casing", null, new Vector3(0, y, z));
                    blocks[interiorX + 1, y, z] = new Casing("Casing", null, new Vector3(interiorX + 1, y, z));
                }
            for (int x = 1; x < interiorX + 1; x++)
                for (int z = 1; z < interiorZ + 1; z++)
                {
                    blocks[x, 0, z] = new Casing("Casing", null, new Vector3(x, 0, z));
                    blocks[x, interiorY + 1, z] = new Casing("Casing", null, new Vector3(x, interiorY + 1, z));
                }
            for (int y = 1; y < interiorY + 1; y++)
                for (int x = 1; x < interiorX + 1; x++)
                {
                    blocks[x, y, interiorZ + 1] = new Casing("Casing", null, new Vector3(x, y, interiorZ + 1));
                    blocks[x, y, 0] = new Casing("Casing", null, new Vector3(x, y, 0));
                }

            usedFuel = Palette.FuelPalette.Values.First();
            UpdateStats();
            ConstructLayers();
        }

        public static void InitializeReactor(Vector3 interiorDims)
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

        private static void DisposeClearLayers()
        {
            if (layers != null)
            {
                foreach (ReactorGridLayer layer in layers)
                    layer.Dispose();
                layers.Clear();
            }
        }

        public static void Redraw()
        {
            foreach (ReactorGridLayer layer in layers)
                layer.Refresh();
        }

        public static void UpdateStats()
        {
            passiveCoolers = new Dictionary<string, List<Cooler>>();
            activeCoolers = new Dictionary<string, List<Cooler>>();
            fuelCells = new List<FuelCell>();
            moderators = new Dictionary<string, List<Moderator>>
            {
                { "Graphite", new List<Moderator>() },
                { "Beryllium", new List<Moderator>() }
            };

            totalCoolingPerTick = 0;
            totalPassiveCoolingPerType = new Dictionary<string, double>();
            totalActiveCoolingPerType = new Dictionary<string, double>();
            totalHeatPerTick = 0;
            totalEnergyPerTick = 0;

            totalCasings = 0;
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Z);
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Y);
            totalCasings += (int)(2 * interiorDims.Z * interiorDims.Y);

            efficiency = 0;
            energyMultiplier = 0;
            heatMultiplier = 0;

            foreach (Block block in blocks)
            {
                if (block is Cooler cooler)
                {
                    if (cooler.Active)
                    {
                        if (activeCoolers.ContainsKey(cooler.CoolerType.ToString()))
                            activeCoolers[cooler.CoolerType.ToString()].Add(cooler);
                        else
                            activeCoolers.Add(cooler.CoolerType.ToString(), new List<Cooler> { cooler });
                    }
                    else
                        if (passiveCoolers.ContainsKey(cooler.CoolerType.ToString()))
                            passiveCoolers[cooler.CoolerType.ToString()].Add(cooler);
                        else
                            passiveCoolers.Add(cooler.CoolerType.ToString(), new List<Cooler> { cooler });
                }
                else if (block is FuelCell fuelcell)
                {
                    fuelCells.Add(fuelcell);
                }
                else if (block is Moderator moderator)
                {
                    if (moderators.ContainsKey(moderator.ModeratorType.ToString()))
                        moderators[moderator.ModeratorType.ToString()].Add(moderator);
                    else
                        moderators.Add(moderator.ModeratorType.ToString(), new List<Moderator> { moderator });
                }
            }

            foreach (var moderatorGroup in moderators)
                foreach (Moderator moderator in moderatorGroup.Value)
                {
                    moderator.Active = false;
                    moderator.Invalidate();
                }
            
            foreach (FuelCell fuelcell in fuelCells)
                fuelcell.UpdateStats();

            OrderedUpdateCoolerStats();

            foreach (KeyValuePair<string, List<Cooler>> kvp in passiveCoolers)
            {
                if (kvp.Value.Count == 0)
                    continue;
                double passiveCooling = 0;
                foreach (Cooler cooler in kvp.Value)
                    if (cooler.Valid)
                            passiveCooling += cooler.Cooling;
                totalPassiveCoolingPerType.Add(kvp.Key, passiveCooling);
            }

            foreach (KeyValuePair<string, List<Cooler>> kvp in activeCoolers)
            {
                if (kvp.Value.Count == 0)
                    continue;
                double activeCooling = 0;
                foreach (Cooler cooler in kvp.Value)
                    if (cooler.Valid)
                        activeCooling += cooler.Cooling;
                totalActiveCoolingPerType.Add(kvp.Key, activeCooling);
            }

            foreach (KeyValuePair<string, double> coolingPerType in totalPassiveCoolingPerType)
            {
                totalCoolingPerTick += coolingPerType.Value;
            }
            foreach (KeyValuePair<string, double> coolingPerType in totalActiveCoolingPerType)
            {
                totalCoolingPerTick += coolingPerType.Value;
            }

            efficiency = (fuelCells.Count == 0) ? 0 : 100 * energyMultiplier / fuelCells.Count;
            heatMulti = (fuelCells.Count == 0) ? 0 : 100 * heatMultiplier / fuelCells.Count;

            maxBaseHeat = (fuelCells.Count == 0) ? 0 : 100 * totalCoolingPerTick / (fuelCells.Count * heatMulti);
            fuelDuration = (fuelCells.Count == 0) ? 0 : usedFuel.FuelTime / (fuelCells.Count * Configuration.Fission.FuelUse);
        }

        private static void OrderedUpdateCoolerStats()
        {
            foreach (string type in checkOrder)
            {
                if (passiveCoolers.ContainsKey(type))
                    foreach (Cooler cooler in passiveCoolers[type])
                        cooler.UpdateStats();
                if (activeCoolers.ContainsKey(type))
                    foreach (Cooler cooler in activeCoolers[type])
                        cooler.UpdateStats();
            }
        }

        public static string GetStatString()
        {
            string report = "";
            if (passiveCoolers.Count > 0)
            {
                report += "Passive coolers:\r\n";
                foreach (KeyValuePair<string, List<Cooler>> coolerType in passiveCoolers)
                {
                    report += string.Format("{0}{1}{2}{3}\r\n", coolerType.Key.PadRight(12), passiveCoolers[coolerType.Key].Count.ToString().PadRight(5), " * " + (coolerType.Value)[0].Cooling.ToString().PadRight(8), (int)totalPassiveCoolingPerType[coolerType.Key] + " HU/t");
                }
            }

            if (activeCoolers.Count > 0)
            {
                report += "\r\n";
                report += "Active coolers:\r\n";
                foreach (KeyValuePair<string, List<Cooler>> coolerType in activeCoolers)
                {
                    report += string.Format("{0}{1}{2}{3}\r\n", coolerType.Key.PadRight(12), activeCoolers[coolerType.Key].Count.ToString().PadRight(5), " * " + (coolerType.Value)[0].Cooling.ToString().PadRight(8), (int)totalActiveCoolingPerType[coolerType.Key] + " HU/t");
                }
            }

            if (moderators.Count > 0)
            {
                report += "\r\n";
                report += "Moderators:\r\n";
                foreach (KeyValuePair<string, List<Moderator>> kvp in moderators)
                {
                    if (kvp.Value.Count == 0)
                        continue;
                    report += string.Format("{0}{1}\r\n", kvp.Key.PadRight(12), kvp.Value.Count.ToString().PadRight(5));
                }
            }

            report += "\r\n";
            report += string.Format("{0}{1}\r\n", "Fuel cells".PadRight(12), fuelCells.Count.ToString().PadRight(5));

            report += "\r\n";
            //report += "Heat:\r\n";
            int heatDiff = (int)(totalHeatPerTick - totalCoolingPerTick);
            int reactorVolume = (int)(interiorDims.X * interiorDims.Y * interiorDims.Z);
            int blockHeatCapacity = 25000;
            report += string.Format("{0}{1,-10}\r\n", "Heat gen.".PadRight(20), (int)totalHeatPerTick + " HU/t");
            report += string.Format("{0}{1}\r\n", "Cooling".PadRight(20), (int)totalCoolingPerTick + " HU/t");
            report += string.Format("{0}{1}\r\n", "Heat diff.".PadRight(20), heatDiff + " HU/t");
            report += string.Format("{0}{1}\r\n", "Max base heat".PadRight(20), Math.Round(maxBaseHeat,2) + " HU/t");
            report += string.Format("{0}{1}\r\n", "Fuel pellet dur.".PadRight(20), Math.Round(fuelDuration/20,2) + " s");
            report += string.Format("{0}{1}\r\n", "Meltdown time".PadRight(20), (heatDiff <= 0) ? "Safe" : ((reactorVolume * blockHeatCapacity) / (20 * heatDiff)).ToString() + " s");

            report += "\r\n";
            //report += "Energy:\r\n";
            report += string.Format("{0}{1}\r\n", "Energy gen.".PadRight(20), (int)totalEnergyPerTick + " RF/t");
            report += string.Format("{0}{1}\r\n", "Effective E. gen.".PadRight(20), ((heatDiff <= 0) ? ((int)totalEnergyPerTick).ToString() : ((int)((totalEnergyPerTick * -totalCoolingPerTick)/(-totalCoolingPerTick - heatDiff))).ToString()) + " RF/t");
            report += string.Format("{0}{1}\r\n", "Efficiency".PadRight(20), (int)efficiency + " %");
            report += string.Format("{0}{1}\r\n", "Energy per pellet".PadRight(20),(int)totalEnergyPerTick*fuelDuration + " RF");
            report += string.Format("{0}{1}\r\n", "Heat mult.".PadRight(20), (int)heatMulti + " %");

            report += "\r\n";
            //report += "Misc:\r\n";

            report += string.Format("{0}{1}\r\n", "Casings".PadRight(15), totalCasings);
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
                //jss.Serialize(tw, usedFuel);
            }
        }

        public static string SaveToString()
        {
            using (StringWriter sw = new StringWriter())
            {
                JsonSerializer jss = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                };

                CompressedSaveFile csf = new CompressedSaveFile(saveVersion, CompressReactor(), interiorDims, usedFuel);
                jss.Serialize(sw, csf);
                return sw.ToString();
            }
        }

        public static void Load(FileInfo saveFile)
        {
            if (saveFile.Extension == ".json")
                LoadCompressedReactor(saveFile.OpenText().ReadToEnd());
            else
                throw new FileFormatException("Unknown save file format!");

            FinalizeLoading();
        }

        public static void LoadFromString(string jsonString)
        {
            LoadCompressedReactor(jsonString);
            FinalizeLoading();
        }

        private static void FinalizeLoading()
        {
            ReloadValuesFromConfig();
            UpdateStats();
            ConstructLayers();
        }

        public static void ReloadValuesFromConfig()
        {
            Palette.ReloadValuesFromConfig();
            ReloadBlockValues();
        }

        private static void ReloadBlockValues()
        {
            if (blocks == null) return;
            foreach (Block block in blocks)
                    block.ReloadValuesFromConfig();
        }

        public static void SaveLayerAsImage(int layer, string fileName)
        {
            Bitmap layerImage = layers[layer - 1].DrawToImage();
            using (FileStream fs = File.OpenWrite(fileName))
                layerImage.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            layerImage.Dispose();
        }

        public static void SaveReactorAsImage(string fileName, int statStringLines, int fontSize = 24)
        {
            int layersPerRow = (int)Math.Ceiling(Math.Sqrt(interiorDims.Y));
            int rows = (int)Math.Ceiling((interiorDims.Y / layersPerRow));
            int bs = PlannerUI.blockSize;

            Point StatsRectSize = new Point(28 * fontSize, (statStringLines + 5) * (fontSize + 2));

            Bitmap reactorImage = new Bitmap(Math.Max(StatsRectSize.X, (int)(layersPerRow * interiorDims.X * bs + (layersPerRow - 1) * bs)),
                                             StatsRectSize.Y + bs + (int)(rows * interiorDims.Z * bs + (rows - 1) * bs));
            using (Graphics gr = Graphics.FromImage(reactorImage))
            {
                gr.Clear(Color.LightGray);

                foreach (ReactorGridLayer layer in layers)
                {
                    Bitmap layerImage = layer.DrawToImage();
                    int y = layer.Y - 1;
                    gr.DrawImage(layerImage,
                                    new Rectangle((int)((y % layersPerRow) * interiorDims.X * bs + (y % layersPerRow) * bs),
                                                StatsRectSize.Y + bs + (int)((y / layersPerRow) * interiorDims.Z * bs + (y / layersPerRow) * bs),
                                                (int)(interiorDims.X * bs), (int)(interiorDims.Z * bs)),
                                    new Rectangle(0, 0, layerImage.Size.Width, layerImage.Size.Height),
                                    GraphicsUnit.Pixel);
                    layerImage.Dispose();
                }
                string report = string.Format("Planner version: {0}\r\n",Updater.ShortVersionString(saveVersion));
                report += string.Format("Used fuel: {0} \r\nBase heat: {1}\r\nBase power: {2}\r\n\r\n", usedFuel.Name, usedFuel.BaseHeat, usedFuel.BasePower);
                report += GetStatString();
                gr.DrawString(report, new Font(FontFamily.GenericMonospace, fontSize, GraphicsUnit.Pixel), Brushes.Black, 0, 0);
            }
            using (FileStream fs = File.OpenWrite(fileName))
            {
                reactorImage.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            }
            reactorImage.Dispose();
        }

        public static Block BlockAt(Vector3 position)
        {
            return blocks[(int)position.X, (int)position.Y, (int)position.Z];
        }

        private static Dictionary<string, List<Vector3>> CompressReactor()
        {
            Dictionary<string, List<Vector3>> cr = new Dictionary<string, List<Vector3>>();
            foreach (Block block in blocks)
            {
                if (block is Casing | block.BlockType == BlockTypes.Air)
                    continue;

                string btype = null;
                if (block is Cooler cooler)
                    btype = (cooler.Active?"Active ":"") + cooler.CoolerType.ToString();
                else if (block is Moderator moderator)
                    btype = moderator.ModeratorType.ToString();
                else if (block is FuelCell)
                    btype = "FuelCell";

                if (btype == null)
                    continue;
                if (cr.ContainsKey(btype))
                    cr[btype].Add(block.Position);
                else
                    cr.Add(btype, new List<Vector3> { block.Position });
            }
            return cr;
        }

        private static void LoadCompressedReactor(string jsonText)
        {
            Block restoreBlock(string type, Vector3 position)
            {
                if (type == "FuelCell")
                    return new FuelCell("FuelCell", Palette.Textures["FuelCell"], position);
                else if (type == "Beryllium" | type == "Graphite")
                    return new Moderator((Moderator)Palette.BlockPalette[type], position);
                else if(type.Contains("Active"))
                    return new Cooler((Cooler)Palette.BlockPalette[type.Split(' ')[1]], position, true);
                else
                    return new Cooler((Cooler)Palette.BlockPalette[type], position, false);
                throw new ArgumentException("Tried to restore an invalid block");
            }

            CompressedSaveFile csf;
            JObject saveJSONObject = JObject.Parse(jsonText);
            Version v;
            v = saveJSONObject["SaveVersion"].ToObject<Version>();
            JsonSerializer js = new JsonSerializer();

            if (v.Major == 2)
            {
                System.Windows.Forms.MessageBox.Show("Can't load post-overhaul savefiles!");
                return;
            }
            else if(v >= new Version(1,2,23))
            {
                csf = (CompressedSaveFile)js.Deserialize(new StringReader(jsonText), typeof(CompressedSaveFile));
            }
            else
            {
                Dictionary<string, List<Vector3>> cr = new Dictionary<string, List<Vector3>>();
                JToken crToken = saveJSONObject["CompressedReactor"];
                foreach (var child in crToken.Children())
                {
                    JProperty entry = child.First().ToObject<JProperty>();
                    string name = entry.Name;
                    var children = entry.First().Children().ToList();
                    cr.Add(name, new List<Vector3>());
                    foreach (var coord in children)
                    {
                        string[] posV = coord.ToObject<string>().Split(',');
                        cr[name].Add(new Vector3(Convert.ToInt32(posV[0]), Convert.ToInt32(posV[1]), Convert.ToInt32(posV[2])));
                    }
                }

                string[] interiorDims = saveJSONObject["InteriorDimensions"].ToObject<string>().Split(',');
                Vector3 inDimsVector = new Vector3(Convert.ToInt32(interiorDims[0]), Convert.ToInt32(interiorDims[1]), Convert.ToInt32(interiorDims[2]));

                Fuel fuel = saveJSONObject["UsedFuel"].ToObject<Fuel>();
                csf = new CompressedSaveFile(v, cr, inDimsVector, fuel);
            }
           

            InitializeReactor(csf.InteriorDimensions);

            foreach (KeyValuePair<string, List<Vector3>> kvp in csf.CompressedReactor)
            {
                foreach(Vector3 pos in kvp.Value)
                    SetBlock(restoreBlock(kvp.Key, pos), pos);
            }

            usedFuel = csf.UsedFuel;
            FinalizeLoading();
        }

        public static void SetBlock(Block block, Vector3 position)
        {
            blocks[(int)position.X, (int)position.Y, (int)position.Z] = block;
        }

        public static void ClearLayer(ReactorGridLayer layer)
        {
            for (int x = 0; x < interiorDims.X; x++)
                for (int z = 0; z < interiorDims.Z; z++)
                    SetBlock(new Block("Air", BlockTypes.Air, Palette.Textures["Air"], new Vector3(x + 1, layer.Y, z + 1)), new Vector3(x + 1, layer.Y, z + 1));
            UpdateStats();
            Redraw();
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
                    Vector3 position = new Vector3(x + 1, layer.Y, z + 1);
                    SetBlock(PlannerUI.layerBuffer[x, z].Copy(position), position);
                }
            UpdateStats();
            Redraw();
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
                        newReactor[x, layer, z] = blocks[x, layer, z].Copy(new Vector3(x, layer, z));
                    }
                }
            }
            for (int layer = y + 1; layer <= interiorDims.Y+1; layer++)
            {
                for (int x = 0; x < interiorDims.X + 2; x++)
                {
                    for (int z = 0; z < interiorDims.Z + 2; z++)
                    {
                        newReactor[x, layer-1, z] = blocks[x, layer, z].Copy(new Vector3(x, layer-1, z));
                    }
                }
            }

            blocks = newReactor;
            interiorDims = new Vector3(interiorDims.X, interiorDims.Y - 1, interiorDims.Z);

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
                        newReactor[x, layer, z] = blocks[x, layer, z].Copy(new Vector3(x, layer, z));
                    }
                }
            }

            for (int x = 0; x < interiorDims.X + 2; x++)
            {
                for (int z = 0; z < interiorDims.Z + 2; z++)
                {
                    if(((x == 0 | x == interiorDims.X + 1)&(z > 0 & z < interiorDims.Z + 1)) || ((z == 0 | z == interiorDims.Z + 1) & (x > 0 & x < interiorDims.X + 1)))
                        newReactor[x, y, z] = new Casing("Casing", null, new Vector3(x, y, z));
                    else
                        newReactor[x, y, z] = new Block("Air", BlockTypes.Air, Palette.Textures["Air"], new Vector3(x, y, z));
                }
            }

            for (int layer = y + 1; layer < interiorDims.Y + 3; layer++)
            {
                for (int x = 0; x < interiorDims.X + 2; x++)
                {
                    for (int z = 0; z < interiorDims.Z + 2; z++)
                    {
                        newReactor[x, layer, z] = blocks[x, layer-1, z].Copy(new Vector3(x, layer, z));
                    }
                }
            }

            blocks = newReactor;
            interiorDims = new Vector3(interiorDims.X, interiorDims.Y + 1, interiorDims.Z);

            using (TextWriter tw = File.CreateText("Debug.txt"))
            {
                for(int layer = 0; layer < interiorDims.Y + 2; layer++)
                {
                    for(int x = 0; x < interiorDims.X + 2; x++)
                    {
                        for (int z = 0; z < interiorDims.Z + 2; z++)
                            tw.Write(string.Format("{0,10}", blocks[x, layer, z].BlockType.ToString()));
                        tw.WriteLine();
                    }
                    tw.WriteLine();
                }
            }
        }
    }
}

