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
using Newtonsoft.Json.Linq;

namespace NC_Reactor_Planner
{
    public struct CompressedSaveFile
    {
        public Version SaveVersion;
        public List<Dictionary<string, List<Point3D>>> CompressedReactor;
        public List<Tuple<Point3D, string, bool>> FuelCells;
        public Size3D InteriorDimensions;
        //public Fuel UsedFuel;

        public CompressedSaveFile(Version sv, List<Dictionary<string, List<Point3D>>> cr, List<Tuple<Point3D, string, bool>> fc, Size3D id)
        {
            SaveVersion = sv;
            CompressedReactor = cr;
            FuelCells = fc;
            InteriorDimensions = id;
           // UsedFuel = uf;
        }
    }

    public static class Reactor
    {
        public static readonly PlannerUI UI;

        public static Block[,,] blocks;
        public static List<Cluster> clusters;
        public static List<ConductorGroup> conductorGroups;
        public static List<ReactorGridLayer> layers;
        public static Size3D interiorDims;
        public static readonly Version saveVersion;


        public static Dictionary<string, List<HeatSink>> heatSinks;
        public static List<FuelCell> fuelCells;
        public static Dictionary<string, List<Moderator>> moderators;
        public static List<Conductor> conductors;
        public static int totalCasings;

        public static List<string> updateOrder = new List<string> { "Water", "Iron", "Redstone", "Glowstone", "Lapis", "Enderium", "Cryotheum", "Magnesium", "Manganese", "Quartz", "Obsidian", "Gold", "Prismarine", "Copper", "Tin", "Lead", "Silver", "Helium", "Purpur", "Diamond", "Emerald", "Boron", "Lithium", "Aluminum"};

        public static List<Vector3D> sixAdjOffsets = new List<Vector3D> { new Vector3D(-1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), new Vector3D(0, 0, 1) };// x+-1, y+-1, z+-1
        public static List<Fuel> fuels;

        public static double totalCoolingPerTick = 0;
        public static Dictionary<string, double> totalCoolingPerType;
        public static double totalHeatPerTick = 0;
        public static double totalOutputPerTick = 0;
        
        public static double heatMultiplier = 0;
        public static double efficiency = 0;


        public static ReactorStates state = ReactorStates.Setup;

        static Reactor()
        {
            saveVersion = Assembly.GetEntryAssembly().GetName().Version;
            UI = new PlannerUI();
            PopulateFuels();
        }

        public static void PopulateFuels()
        {
            fuels = new List<Fuel>();
            foreach (KeyValuePair<string, FuelValues> fuelEntry in Configuration.Fuels)
            {
                FuelValues fev = fuelEntry.Value;
                fuels.Add(new Fuel(fuelEntry.Key, fev.BaseEfficiency, fev.BaseHeat, fev.FuelTime, fev.CriticalityFactor));
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
            {
                layer.Refresh();
            }
        }

        public static void Run()
        {
            state = ReactorStates.Running;
            Update();
            Redraw();
        }

        public static void RevertToSetup()
        {
            state = ReactorStates.Setup;
            Update();
            Redraw();
        }

        public static void Update()
        {
            RegenerateTypedLists();
            clusters = new List<Cluster>();

            foreach (FuelCell fuelCell in fuelCells)
                fuelCell.RevertToSetup();

            foreach (KeyValuePair<string, List<Moderator>> moderators in moderators)
                foreach (Moderator moderator in moderators.Value)
                moderator.RevertToSetup();

            foreach (KeyValuePair<string, List<HeatSink>> heatSinks in heatSinks)
                foreach (HeatSink heatSink in heatSinks.Value)
                    heatSink.RevertToSetup();

            foreach (Conductor conductor in conductors)
                conductor.RevertToSetup();

            switch (state)
            {
                case ReactorStates.Setup:
                    foreach (FuelCell fuelCell in fuelCells)
                    {
                        fuelCell.Activate();
                    }
                    break;
                case ReactorStates.Running:
                    foreach (FuelCell fuelCell in fuelCells)
                    {
                        if (fuelCell.Primed)
                            fuelCell.Activate();
                    }
                    RunFuelCellActivation();
                    break;
                default:
                    break;
            }

            UpdateModerators();

            OrderedUpdateHeatSinks();

            FormConductorGroups();

            if (state == ReactorStates.Running)
            {
                FormClusters();
                foreach (Cluster cluster in clusters)
                    cluster.UpdateStats();
            }

            UpdateStats();
            
        }

        private static void RegenerateTypedLists()
        {
            heatSinks = new Dictionary<string, List<HeatSink>>();
            fuelCells = new List<FuelCell>();
            moderators = new Dictionary<string, List<Moderator>>
            {
                { "Graphite", new List<Moderator>() },
                { "Beryllium", new List<Moderator>() }
            };
            conductors = new List<Conductor>();


            foreach (Block block in blocks)
            {
                if (block is HeatSink heatSink)
                {

                    if (heatSinks.ContainsKey(heatSink.DisplayName))
                        heatSinks[heatSink.DisplayName].Add(heatSink);
                    else
                        heatSinks.Add(heatSink.DisplayName, new List<HeatSink> { heatSink });
                }
                else if (block is FuelCell fuelCell)
                {
                    fuelCells.Add(fuelCell);
                }
                else if (block is Moderator moderator)
                {
                    if (moderators.ContainsKey(moderator.DisplayName))
                        moderators[moderator.DisplayName].Add(moderator);
                    else
                        moderators.Add(moderator.DisplayName, new List<Moderator> { moderator });
                }
                else if (block is Conductor conductor)
                {
                        conductors.Add(conductor);
                }

            }
        }

        private static void RunFuelCellActivation()
        {
            List<FuelCell> visited = new List<FuelCell>();
            List<FuelCell> activeFuelCells = fuelCells.FindAll(delegate (FuelCell fc) { return fc.Active; });
            foreach (FuelCell activeFuelCell in activeFuelCells)
            {
                activeFuelCell.FirstPass = false;
                List<FuelCell> queue = new List<FuelCell>();
                queue.Add(activeFuelCell);
                FuelCell fuelCell;
                while (queue.Count > 0 && (fuelCell = queue.First()) != null)
                {
                    queue.Remove(fuelCell);
                    visited.Add(fuelCell);
                    foreach (FuelCell fc in fuelCell.FindModeratorsThenAdjacentCells())
                    {
                        if (!visited.Contains(fc))
                            queue.Add(fc);
                    }
                }
            }
        }

        private static void UpdateModerators()
        {
            foreach (KeyValuePair<string, List<Moderator>> moderators in moderators)
                foreach (Moderator moderator in moderators.Value)
                    moderator.UpdateStats();
        }

        private static void FormClusters()
        {
            bool FormCluster(Block root, int id)
            {
                if (root.Cluster != -1)
                    return false;

                List<Block> queue = new List<Block>{root};
                clusters.Add(new Cluster(id));

                while (queue.Count > 0)
                {
                    root = queue.First();
                    root.SetCluster(id);
                    if(!clusters[id].blocks.Contains(root))
                        clusters[id].AddBlock(root);

                    foreach(Vector3D offset in sixAdjOffsets)
                    {
                        Point3D pos = root.Position + offset;
                        Block neighbour = BlockAt(pos);
                        if(!(neighbour is Moderator) & !(neighbour is Conductor) & (neighbour.BlockType != BlockTypes.Air)& (neighbour.BlockType != BlockTypes.Casing) & root.Valid)
                        {
                            if(neighbour.Cluster == -1)
                                queue.Add(neighbour);
                        }
                        else if(neighbour is Conductor conductor)
                        {
                            if (conductorGroups[conductor.GroupID].HasPathToCasing)
                                clusters[id].HasPathToCasing = true;
                        }
                        else if (neighbour is Casing casing)
                        {
                                clusters[id].HasPathToCasing = true;
                        }
                    }
                    queue.Remove(root);
                }
                return true;
            }

            int clusterID = 0;
            foreach (FuelCell fuelCell in fuelCells)
            {
                if (FormCluster(fuelCell, clusterID))
                    clusterID++;
            }
        }

        public static void FormConductorGroups()
        {
            bool FormConductorGroup(Conductor root, int id)
            {
                if (root.GroupID != -1)
                    return false;
                List<Conductor> queue = new List<Conductor>();
                queue.Add(root);
                conductorGroups.Add(new ConductorGroup(id));
                while (queue.Count > 0)
                {
                    root = queue.First();
                    root.GroupID = id;
                    conductorGroups[id].conductors.Add(root);
                    foreach (Vector3D offset in sixAdjOffsets)
                    {
                        if (BlockAt(root.Position + offset) is Conductor conductor)
                        {
                            if (conductor.GroupID == -1)
                            {
                                conductor.GroupID = id;
                                queue.Add(conductor);
                            }
                        }
                        else if(BlockAt(root.Position + offset) is Casing casing)
                        {
                            root.HasPathToCasing = true;
                            conductorGroups[id].HasPathToCasing = true;
                        }
                    }
                    queue.Remove(root);
                }
                return true;
            }

            conductorGroups = new List<ConductorGroup>();
            int groupId = 0;
            foreach(Conductor conductor in conductors)
            {
                if (FormConductorGroup(conductor, groupId))
                {
                    if (conductorGroups[groupId].HasPathToCasing)
                        foreach (Conductor c in conductorGroups[groupId].conductors)
                            c.HasPathToCasing = true;
                    groupId++;
                }
            }
        }

        private static void OrderedUpdateHeatSinks()
        {
            System.Diagnostics.Debug.WriteLine("Updated Heatsinks");
            foreach (string type in updateOrder)
            {
                if (heatSinks.ContainsKey(type))
                    foreach (HeatSink heatSink in heatSinks[type])
                        heatSink.UpdateStats();
            }
        }

        private static void UpdateStats()
        {
            totalCoolingPerTick = 0;
            totalCoolingPerType = new Dictionary<string, double>();
            totalHeatPerTick = 0;
            totalOutputPerTick = 0;

            totalCasings = 0;
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Z);
            totalCasings += (int)(2 * interiorDims.X * interiorDims.Y);
            totalCasings += (int)(2 * interiorDims.Z * interiorDims.Y);

            int activeFuelCells = 0;
            double sumEfficiency = 0;
            efficiency = 0;
            double sumHeatMultiplier = 0;
            heatMultiplier = 0;

            foreach (Cluster cluster in clusters)
            {
                if (!cluster.Valid)
                    continue;

                totalOutputPerTick += cluster.TotalOutput;
                totalCoolingPerTick += cluster.TotalCoolingPerTick;
                totalHeatPerTick += cluster.TotalHeatPerTick;
            }

            foreach (FuelCell fuelCell in fuelCells)
            {
                if (!fuelCell.Active)
                    continue;
                activeFuelCells++;
                sumEfficiency += fuelCell.Efficiency;
                sumHeatMultiplier += fuelCell.HeatMultiplier;
            }

            efficiency = sumEfficiency / activeFuelCells;
            heatMultiplier = sumHeatMultiplier / activeFuelCells;

            totalHeatPerTick *= Configuration.Fission.HeatGeneration;
            totalOutputPerTick *= Configuration.Fission.Power;
        }

        public static string GetStatString(bool includeClusterInfo = true)
        {
            if (state == ReactorStates.Setup)
                return "Run the reactor to get stats";

            string report = string.Format("Overall reactor stats:\r\n" +
                                        "Total output: {5} mb/t\r\n" +
                                        "Total Heat: {0} HU/t\r\n" +
                                        "Total Cooling: {1} HU/t\r\n" +
                                        "Net Heat: {2} HU/t\r\n" +
                                        "Overall Efficiency: {3} %\r\n" +
                                        "Overall Heat Multiplier: {4} %\r\n\r\n",
                                        totalHeatPerTick,totalCoolingPerTick,totalHeatPerTick-totalCoolingPerTick,(int)(efficiency*100),(int)(heatMultiplier*100), (int)(totalOutputPerTick/16)
                );

            if(includeClusterInfo)
                foreach (Cluster cluster in clusters)
                {
                    report += cluster.GetStatString();
                }
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
                //CompressedSaveFile csf = new CompressedSaveFile(saveVersion, CompressReactor(out List<Tuple<Point3D, string, bool>> fuelCells), fuelCells, interiorDims);
                jss.Serialize(tw, ComposeSaveData());
            }
        }

        private static SaveData ComposeSaveData()
        {
            Dictionary<string, List<Point3D>> saveHeatSinks = new Dictionary<string, List<Point3D>>();
            Dictionary<string, List<Point3D>> saveModerators = new Dictionary<string, List<Point3D>>();
            List<Point3D> saveConductors = new List<Point3D>();
            Dictionary<string, List<Point3D>> saveFuelCells = new Dictionary<string, List<Point3D>>();

            foreach (KeyValuePair<string, List<HeatSink>> kvp in heatSinks)
            {
                if (!saveHeatSinks.ContainsKey(kvp.Key))
                    saveHeatSinks.Add(kvp.Key, new List<Point3D>());
                foreach (HeatSink hs in kvp.Value)
                    saveHeatSinks[kvp.Key].Add(hs.Position);
            }

            foreach (KeyValuePair<string, List<Moderator>> kvp in moderators)
            {
                if (!saveModerators.ContainsKey(kvp.Key))
                    saveModerators.Add(kvp.Key, new List<Point3D>());
                foreach (Moderator md in kvp.Value)
                    saveModerators[kvp.Key].Add(md.Position);
            }

            foreach (Conductor cd in conductors)
                saveConductors.Add(cd.Position);

            foreach (FuelCell fc in fuelCells)
            {
                if (!saveFuelCells.ContainsKey(fc.ToSaveString()))
                    saveFuelCells.Add(fc.ToSaveString(), new List<Point3D>());
                saveFuelCells[fc.ToSaveString()].Add(fc.Position);
            }

            return new SaveData(saveVersion, saveHeatSinks, saveModerators, saveConductors, saveFuelCells, interiorDims);
        }

        public static ValidationResult Load(FileInfo saveFile)
        {
            SaveData save;
            using (StreamReader sr = File.OpenText(saveFile.FullName))
            {
                JsonSerializer js = new JsonSerializer();
                save = (SaveData)js.Deserialize(sr, typeof(SaveData));
            }
            ValidationResult vr = save.PerformValidation();
            if(vr.Successful)
                LoadFromSaveData(save);
            return vr;
        }

        private static void LoadFromSaveData(SaveData save)
        {
            InitializeReactor(save.InteriorDimensions);

            foreach (KeyValuePair<string, List<Point3D>> kvp in save.HeatSinks)
                foreach (Point3D pos in kvp.Value)
                    SetBlock(Palette.blockPalette[kvp.Key].Copy(pos), pos);

            foreach (KeyValuePair<string, List<Point3D>> kvp in save.Moderators)
                foreach (Point3D pos in kvp.Value)
                    SetBlock(Palette.blockPalette[kvp.Key].Copy(pos), pos);

            foreach (KeyValuePair<string, List<Point3D>> kvp in save.FuelCells)
            {
                FuelCell restoredFuelCell;
                foreach (Point3D pos in kvp.Value)
                {
                    List<string> props = kvp.Key.Split(';').ToList();
                    switch (props.Count)
                    {
                        case 0:
                        case 1:
                            throw new ArgumentException("Tried to load an invalid FuelCell: " + kvp.Key);
                        case 2:
                            restoredFuelCell = new FuelCell("FuelCell", Palette.textures["FuelCell"], pos, GetFuel(props[0]),Convert.ToBoolean(props[1]));
                            break;
                        default:
                            throw new ArgumentException("Tried to load an unexpected FuelCell: " + kvp.Key);
                    }
                    SetBlock(restoredFuelCell, pos);
                }
            }

            foreach (Point3D pos in save.Conductors)
                SetBlock(new Conductor("Conductor", Palette.textures["Conductor"], pos), pos);

            ReloadValuesFromConfig();
            ConstructLayers();

        }

        public static void ReloadValuesFromConfig()
        {
            ReloadBlockValues();
            ReloadFuelValues();
        }

        private static void ReloadBlockValues()
        {
            foreach (KeyValuePair<Block, BlockTypes> kvp in Palette.blocks)
                    kvp.Key.ReloadValuesFromConfig();

            if (blocks == null) return;
            foreach (Block block in blocks)
                    block.ReloadValuesFromConfig();
        }

        private static void ReloadFuelValues()
        {
            foreach (Fuel fuel in fuels)
            {
                fuel.ReloadValuesFromConfig();
            }
        }

        public static Fuel GetFuel(string fuelName)
        {
            foreach (Fuel fuel in fuels)
            {
                if (fuel.Name == fuelName)
                    return fuel;
            }
            throw new ArgumentException("Tried to get wrong fuel! Looked for: " + fuelName);
        }

        public static void SaveLayerAsImage(int layer, string fileName)
        {
            Bitmap layerImage = layers[layer - 1].DrawToImage();
            layerImage.Save(fileName);
            layerImage.Dispose();
        }

        public static void SaveReactorAsImage(string fileName, int statStringLines, int scale = 2, bool large = false, int fontSize = 24)
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
                gr.DrawString(GetStatString(), new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel), Brushes.Black, 0, 0);
            }
            reactorImage.Save(fileName);
            reactorImage.Dispose();
        }

        public static Block BlockAt(Point3D position)
        {
            return blocks[(int)position.X, (int)position.Y, (int)position.Z];
        }

        public static void SetBlock(Block block, Point3D position)
        {
            blocks[(int)position.X, (int)position.Y, (int)position.Z] = block;
        }

        public static void ClearLayer(ReactorGridLayer layer)
        {
            for (int x = 0; x < interiorDims.X; x++)
                for (int z = 0; z < interiorDims.Z; z++)
                    SetBlock(new Block("Air", BlockTypes.Air, Palette.textures["Air"], new Point3D(x + 1, layer.Y, z + 1)), new Point3D(x + 1, layer.Y, z + 1));
            Update();
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
                    SetBlock(PlannerUI.layerBuffer[x, z], new Point3D(x + 1, layer.Y, z + 1));
                }
            Update();
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
                    if(((x == 0 | x == interiorDims.X + 1)&(z > 0 & z < interiorDims.Z + 1)) || ((z == 0 | z == interiorDims.Z + 1) & (x > 0 & x < interiorDims.X + 1)))
                        newReactor[x, y, z] = new Casing("Casing", null, new Point3D(x, y, z));
                    else
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

    public enum ReactorStates
    {
        Setup,
        Running,
    }
}

