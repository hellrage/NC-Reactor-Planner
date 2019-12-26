using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace NC_Reactor_Planner
{
    public struct ConfigFile
    {
        public Version saveVersion;
        public FissionValues Fission;
        public CraftingMaterials ResourceCosts;
        public Dictionary<string, FuelValues> Fuels;
        public Dictionary<string, HeatSinkValues> HeatSinks;
        public Dictionary<string, ModeratorValues> Moderators;

        public ConfigFile(Version sv, FissionValues fs, Dictionary<string, FuelValues> f, Dictionary<string, HeatSinkValues> c, Dictionary<string, ModeratorValues> m, CraftingMaterials cm)
        {
            saveVersion = sv;
            Fission = fs;
            Fuels = f;
            HeatSinks = c;
            Moderators = m;
            ResourceCosts = cm;
        }
    }

    public struct FuelValues
    {
        public double BaseEfficiency;
        public double BaseHeat;
        public double FuelTime;
        public int CriticalityFactor;

        public FuelValues(double be, double bh, double ft, int cf)
        {
            BaseEfficiency = be;
            BaseHeat = bh;
            FuelTime = ft;
            CriticalityFactor = cf;
        }

        public FuelValues(List<object> values)
        {
            BaseEfficiency = Convert.ToDouble(values[0]);
            BaseHeat = Convert.ToDouble(values[1]);
            FuelTime = Convert.ToDouble(values[2]);
            CriticalityFactor = Convert.ToInt32(values[3]);
        }
    }

    public struct HeatSinkValues
    {
        public double HeatPassive;
        public string Requirements;

        public HeatSinkValues(double hp, string req)
        {
            HeatPassive = hp;
            Requirements = req;
        }

        public HeatSinkValues(List<object> values)
        {
            HeatPassive = Convert.ToDouble(values[0]);
            Requirements = Convert.ToString(values[1]);
        }
    }

    public struct ModeratorValues
    {
        public int FluxFactor;
        public double EfficiencyFactor;

        public ModeratorValues(int ff, double ef)
        {
            FluxFactor = ff;
            EfficiencyFactor = ef;
        }

        public ModeratorValues(List<object> fieldValues)
        {
            FluxFactor = Convert.ToInt32(fieldValues[0]);
            EfficiencyFactor = Convert.ToDouble(fieldValues[1]);
        }
    }

    public struct FissionValues
    {
        public double Power;
        public double FuelUse;
        public double HeatGeneration;
        public int MinSize;
        public int MaxSize;
        public int NeutronReach;
        public double ReflectorEfficiency;
        public double MaxSparsityPenaltyMultiplier;
        public double SparsityPenaltyThreshold;
        public double CoolingPenaltyLeniency;

        public FissionValues(double p, double fu, double hg, int ms, int mxs, int nr, double re, double mspm, double spt, double cpl)
        {
            Power = p;
            FuelUse = fu;
            HeatGeneration = hg;
            MinSize = ms;
            MaxSize = mxs;
            NeutronReach = nr;
            ReflectorEfficiency = re;
            MaxSparsityPenaltyMultiplier = mspm;
            SparsityPenaltyThreshold = spt;
            CoolingPenaltyLeniency = cpl;
        }

        public FissionValues(List<object> values)
        {
            Power = Convert.ToDouble(values[0]);
            FuelUse = Convert.ToDouble(values[1]);
            HeatGeneration = Convert.ToDouble(values[2]);
            MinSize = Convert.ToInt32(values[3]);
            MaxSize = Convert.ToInt32(values[4]);
            NeutronReach = Convert.ToInt32(values[5]);
            ReflectorEfficiency = Convert.ToDouble(values[6]);
            MaxSparsityPenaltyMultiplier = Convert.ToDouble(values[7]);
            SparsityPenaltyThreshold = Convert.ToDouble(values[8]);
            CoolingPenaltyLeniency = Convert.ToDouble(values[9]);
        }
    }

    public struct CraftingMaterials
    {
        public Dictionary<string, Dictionary<string, int>> HeatSinkCosts;
        public Dictionary<string, Dictionary<string, int>> ModeratorCosts;
        public Dictionary<string, int> FuelCellCosts;
        public Dictionary<string, int> CasingCosts;

        public CraftingMaterials(Dictionary<string, Dictionary<string, int>> clc, Dictionary<string, Dictionary<string, int>> mc, Dictionary<string, int> fcc, Dictionary<string, int> csc)
        {
            HeatSinkCosts = clc;
            ModeratorCosts = mc;
            FuelCellCosts = fcc;
            CasingCosts = csc;
        }
    }

    public static class Configuration
    {
        //[TODO] incapsulation .\_/.
        public static FissionValues Fission;
        public static CraftingMaterials ResourceCosts;
        public static Dictionary<string, FuelValues> Fuels;
        public static Dictionary<string, HeatSinkValues> HeatSinks;
        public static Dictionary<string, ModeratorValues> Moderators;

        private static FileInfo configFileInfo;

        public static bool Load(FileInfo file)
        {
            configFileInfo = file;
            ConfigFile cf;
            using (StreamReader sr = File.OpenText(file.FullName))
            {
                JsonSerializer jss = new JsonSerializer();
                try
                {
                    cf = (ConfigFile)jss.Deserialize(sr, typeof(ConfigFile));
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\nConfig file was corrupt or there were major changes to structure!");
                    return false;
                }
            }

            if(cf.saveVersion < new Version(2,0,0,0))
            {
                System.Windows.Forms.MessageBox.Show("Pre-overhaul configurations aren't supported!\r\nDelete your BetaConfig.json to regenerate a new one.");
                return false;
            }
            if(cf.saveVersion < new Version(2, 0, 27, 0))
            {
                System.Windows.Forms.MessageBox.Show("Ignoring old config file as the values have changed, please overwrite BetaConfig.json");
                return false;
            }

            if((cf.Fuels == null) | (cf.HeatSinks == null))
            {
                System.Windows.Forms.MessageBox.Show("Invalid config file contents!");
                return false;
            }

            Fission = cf.Fission;
            ResourceCosts = cf.ResourceCosts;
            if (ResourceCosts.CasingCosts == null)
                SetDefaultResourceCosts();
            Fuels = cf.Fuels;
            HeatSinks = cf.HeatSinks;
            if (cf.saveVersion >= new Version(2, 0, 0))
                Moderators = cf.Moderators;
            else
                SetDefaultModerators();

            Palette.Load();
            Reactor.ReloadValuesFromConfig();
            Palette.SetHeatSinkUpdateOrder();
            Palette.PaletteControl.ResetSize();
            Reactor.UI.UpdateStatsUIPosition();
            return true;
        }

        public static void Save(FileInfo file)
        {
            configFileInfo = file;
            using (TextWriter tw = File.CreateText(file.FullName))
            {
                JsonSerializer jss = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                };

                ConfigFile cf = new ConfigFile(Reactor.saveVersion, Fission, Fuels, HeatSinks, Moderators, ResourceCosts);
                jss.Serialize(tw, cf);
            }
        }

        public static void ResetToDefaults()
        {
            configFileInfo = null;

            SetDefaultHeatSinks();

            SetDefaultFuels();

            SetDefaultModerators();

            SetDefaultFission();

            SetDefaultResourceCosts();

            Palette.Load();
            Palette.SetHeatSinkUpdateOrder();
        }

        private static void SetDefaultFuels()
        {
            Fuels = new Dictionary<string, FuelValues>();
            Fuels.Add("[OX]TBU", new FuelValues(1, 40, 14400, 234));
            Fuels.Add("[OX]LEU-233", new FuelValues(1.1, 216, 2666, 78));
            Fuels.Add("[OX]HEU-233", new FuelValues(1.1, 648, 2666, 39));
            Fuels.Add("[OX]LEU-235", new FuelValues(1, 120, 4800, 102));
            Fuels.Add("[OX]HEU-235", new FuelValues(1, 360, 4800, 51));
            Fuels.Add("[OX]LEN-236", new FuelValues(1.1, 292, 1972, 70));
            Fuels.Add("[OX]HEN-236", new FuelValues(1.1, 876, 1972, 35));
            Fuels.Add("[OX]LEP-239", new FuelValues(1.2, 126, 4572, 99));
            Fuels.Add("[OX]HEP-239", new FuelValues(1.2, 378, 4572, 49));
            Fuels.Add("[OX]LEP-241", new FuelValues(1.25, 182, 3164, 84));
            Fuels.Add("[OX]HEP-241", new FuelValues(1.25, 546, 3164, 42));
            Fuels.Add("[OX]MOX-239", new FuelValues(1.05, 132, 4354, 94));
            Fuels.Add("[OX]MOX-241", new FuelValues(1.15, 192, 3014, 80));
            Fuels.Add("[OX]LEA-242", new FuelValues(1.35, 390, 1476, 65));
            Fuels.Add("[OX]HEA-242", new FuelValues(1.35, 1170, 1476, 32));
            Fuels.Add("[OX]LECm-243", new FuelValues(1.45, 384, 1500, 66));
            Fuels.Add("[OX]HECm-243", new FuelValues(1.45, 1152, 1500, 33));
            Fuels.Add("[OX]LECm-245", new FuelValues(1.5, 238, 2420, 75));
            Fuels.Add("[OX]HECm-245", new FuelValues(1.5, 714, 2420, 37));
            Fuels.Add("[OX]LECm-247", new FuelValues(1.55, 268, 2150, 72));
            Fuels.Add("[OX]HECm-247", new FuelValues(1.55, 804, 2150, 36));
            Fuels.Add("[OX]LEB-248", new FuelValues(1.65, 266, 2166, 73));
            Fuels.Add("[OX]HEB-248", new FuelValues(1.65, 798, 2166, 36));
            Fuels.Add("[OX]LECf-249", new FuelValues(1.75, 540, 1066, 60));
            Fuels.Add("[OX]HECf-249", new FuelValues(1.75, 1620, 1066, 30));
            Fuels.Add("[OX]LECf-251", new FuelValues(1.8, 288, 2000, 71));
            Fuels.Add("[OX]HECf-251", new FuelValues(1.8, 864, 2000, 35));

            Fuels.Add("[NI]TBU", new FuelValues(1, 32, 18000, 293));
            Fuels.Add("[NI]LEU-233", new FuelValues(1.1, 172, 3348, 98));
            Fuels.Add("[NI]HEU-233", new FuelValues(1.1, 516, 3348, 49));
            Fuels.Add("[NI]LEU-235", new FuelValues(1, 96, 6000, 128));
            Fuels.Add("[NI]HEU-235", new FuelValues(1, 288, 6000, 64));
            Fuels.Add("[NI]LEN-236", new FuelValues(1.1, 234, 2462, 88));
            Fuels.Add("[NI]HEN-236", new FuelValues(1.1, 702, 2462, 44));
            Fuels.Add("[NI]LEP-239", new FuelValues(1.2, 100, 5760, 124));
            Fuels.Add("[NI]HEP-239", new FuelValues(1.2, 300, 5760, 62));
            Fuels.Add("[NI]LEP-241", new FuelValues(1.25, 146, 3946, 105));
            Fuels.Add("[NI]HEP-241", new FuelValues(1.25, 438, 3946, 52));
            Fuels.Add("[NI]MNI-239", new FuelValues(1.05, 106, 5486, 118));
            Fuels.Add("[NI]MNI-241", new FuelValues(1.15, 154, 3758, 100));
            Fuels.Add("[NI]LEA-242", new FuelValues(1.35, 312, 1846, 81));
            Fuels.Add("[NI]HEA-242", new FuelValues(1.35, 936, 1846, 40));
            Fuels.Add("[NI]LECm-243", new FuelValues(1.45, 308, 1870, 83));
            Fuels.Add("[NI]HECm-243", new FuelValues(1.45, 924, 1870, 41));
            Fuels.Add("[NI]LECm-245", new FuelValues(1.5, 190, 3032, 94));
            Fuels.Add("[NI]HECm-245", new FuelValues(1.5, 570, 3032, 47));
            Fuels.Add("[NI]LECm-247", new FuelValues(1.55, 214, 2692, 90));
            Fuels.Add("[NI]HECm-247", new FuelValues(1.55, 642, 2692, 45));
            Fuels.Add("[NI]LEB-248", new FuelValues(1.65, 212, 2716, 91));
            Fuels.Add("[NI]HEB-248", new FuelValues(1.65, 636, 2716, 45));
            Fuels.Add("[NI]LECf-249", new FuelValues(1.75, 432, 1334, 75));
            Fuels.Add("[NI]HECf-249", new FuelValues(1.75, 1296, 1334, 37));
            Fuels.Add("[NI]LECf-251", new FuelValues(1.8, 230, 2504, 89));
            Fuels.Add("[NI]HECf-251", new FuelValues(1.8, 690, 2504, 44));

            Fuels.Add("[ZA]TBU", new FuelValues(1.05, 50, 11520, 199));
            Fuels.Add("[ZA]LEU-233", new FuelValues(1.15, 270, 2134, 66));
            Fuels.Add("[ZA]HEU-233", new FuelValues(1.15, 810, 2134, 33));
            Fuels.Add("[ZA]LEU-235", new FuelValues(1.05, 150, 3840, 87));
            Fuels.Add("[ZA]HEU-235", new FuelValues(1.05, 450, 3840, 43));
            Fuels.Add("[ZA]LEN-236", new FuelValues(1.15, 366, 1574, 60));
            Fuels.Add("[ZA]HEN-236", new FuelValues(1.15, 1098, 1574, 30));
            Fuels.Add("[ZA]LEP-239", new FuelValues(1.25, 158, 3646, 84));
            Fuels.Add("[ZA]HEP-239", new FuelValues(1.25, 474, 3646, 42));
            Fuels.Add("[ZA]LEP-241", new FuelValues(1.3, 228, 2526, 71));
            Fuels.Add("[ZA]HEP-241", new FuelValues(1.3, 684, 2526, 35));
            Fuels.Add("[ZA]MZA-239", new FuelValues(1.1, 166, 3472, 80));
            Fuels.Add("[ZA]MZA-241", new FuelValues(1.2, 240, 2406, 68));
            Fuels.Add("[ZA]LEA-242", new FuelValues(1.4, 488, 1180, 55));
            Fuels.Add("[ZA]HEA-242", new FuelValues(1.4, 1464, 1180, 27));
            Fuels.Add("[ZA]LECm-243", new FuelValues(1.5, 480, 1200, 56));
            Fuels.Add("[ZA]HECm-243", new FuelValues(1.5, 1440, 1200, 28));
            Fuels.Add("[ZA]LECm-245", new FuelValues(1.55, 298, 1932, 64));
            Fuels.Add("[ZA]HECm-245", new FuelValues(1.55, 894, 1932, 32));
            Fuels.Add("[ZA]LECm-247", new FuelValues(1.6, 336, 1714, 61));
            Fuels.Add("[ZA]HECm-247", new FuelValues(1.6, 1008, 1714, 30));
            Fuels.Add("[ZA]LEB-248", new FuelValues(1.7, 332, 1734, 62));
            Fuels.Add("[ZA]HEB-248", new FuelValues(1.7, 996, 1734, 31));
            Fuels.Add("[ZA]LECf-249", new FuelValues(1.8, 676, 852, 51));
            Fuels.Add("[ZA]HECf-249", new FuelValues(1.8, 2028, 852, 25));
            Fuels.Add("[ZA]LECf-251", new FuelValues(1.85, 360, 1600, 60));
            Fuels.Add("[ZA]HECf-251", new FuelValues(1.85, 1080, 1600, 30));

            Fuels.Add("[F4]TBU", new FuelValues(2, 32, 18000, 234));
            Fuels.Add("[F4]LEU-233", new FuelValues(2.2, 172, 3348, 78));
            Fuels.Add("[F4]HEU-233", new FuelValues(2.2, 516, 3348, 39));
            Fuels.Add("[F4]LEU-235", new FuelValues(2, 96, 6000, 102));
            Fuels.Add("[F4]HEU-235", new FuelValues(2, 288, 6000, 51));
            Fuels.Add("[F4]LEN-236", new FuelValues(2.2, 234, 2462, 70));
            Fuels.Add("[F4]HEN-236", new FuelValues(2.2, 702, 2462, 35));
            Fuels.Add("[F4]LEP-239", new FuelValues(2.4, 100, 5760, 99));
            Fuels.Add("[F4]HEP-239", new FuelValues(2.4, 300, 5760, 49));
            Fuels.Add("[F4]LEP-241", new FuelValues(2.5, 146, 3946, 84));
            Fuels.Add("[F4]HEP-241", new FuelValues(2.5, 438, 3946, 42));
            Fuels.Add("[F4]MF4-239", new FuelValues(2.1, 106, 5486, 94));
            Fuels.Add("[F4]MF4-241", new FuelValues(2.3, 154, 3758, 80));
            Fuels.Add("[F4]LEA-242", new FuelValues(2.7, 312, 1846, 65));
            Fuels.Add("[F4]HEA-242", new FuelValues(2.7, 936, 1846, 32));
            Fuels.Add("[F4]LECm-243", new FuelValues(2.9, 308, 1870, 66));
            Fuels.Add("[F4]HECm-243", new FuelValues(2.9, 924, 1870, 33));
            Fuels.Add("[F4]LECm-245", new FuelValues(3, 190, 3032, 75));
            Fuels.Add("[F4]HECm-245", new FuelValues(3, 570, 3032, 37));
            Fuels.Add("[F4]LECm-247", new FuelValues(3.1, 214, 2692, 72));
            Fuels.Add("[F4]HECm-247", new FuelValues(3.1, 642, 2692, 36));
            Fuels.Add("[F4]LEB-248", new FuelValues(3.3, 212, 2716, 73));
            Fuels.Add("[F4]HEB-248", new FuelValues(3.3, 636, 2716, 36));
            Fuels.Add("[F4]LECf-249", new FuelValues(3.5, 432, 1334, 60));
            Fuels.Add("[F4]HECf-249", new FuelValues(3.5, 1296, 1334, 30));
            Fuels.Add("[F4]LECf-251", new FuelValues(3.6, 230, 2504, 71));
            Fuels.Add("[F4]HECf-251", new FuelValues(3.6, 690, 2504, 35));
        }

        private static void SetDefaultHeatSinks()
        {
            HeatSinks = new Dictionary<string, HeatSinkValues>();
            HeatSinks.Add("Water", new HeatSinkValues(50, "One FuelCell"));
            HeatSinks.Add("Iron", new HeatSinkValues(55, "One Moderator"));
            HeatSinks.Add("Redstone", new HeatSinkValues(85, "One FuelCell; One Moderator"));
            HeatSinks.Add("Quartz", new HeatSinkValues(75, "One Redstone heatsink"));
            HeatSinks.Add("Obsidian", new HeatSinkValues(70, "Axial Glowstone heatsinks"));
            HeatSinks.Add("NetherBrick", new HeatSinkValues(100, "One Obsidian heatsink"));
            HeatSinks.Add("Glowstone", new HeatSinkValues(110, "Two Moderators"));
            HeatSinks.Add("Lapis", new HeatSinkValues(95, "One FuelCell; One Casing"));
            HeatSinks.Add("Gold", new HeatSinkValues(105, "Two Iron heatsinks"));
            HeatSinks.Add("Prismarine", new HeatSinkValues(115, "Two Water heatsinks"));
            HeatSinks.Add("Slime", new HeatSinkValues(135, "Exactly One Water heatsink; One Reflector"));
            HeatSinks.Add("EndStone", new HeatSinkValues(60, "One Reflector"));
            HeatSinks.Add("Purpur", new HeatSinkValues(90, "Exactly One Iron heatsink; One EndStone heatsink"));
            HeatSinks.Add("Diamond", new HeatSinkValues(190, "One Gold heatsink; One FuelCell"));
            HeatSinks.Add("Emerald", new HeatSinkValues(195, "One Prismarine heatsink; One Moderator"));
            HeatSinks.Add("Copper", new HeatSinkValues(80, "One Water heatsink"));
            HeatSinks.Add("Tin", new HeatSinkValues(120, "Axial Lapis heatsinks"));
            HeatSinks.Add("Lead", new HeatSinkValues(65, "One Iron heatsink"));
            HeatSinks.Add("Boron", new HeatSinkValues(165, "Exactly One Quartz heatsink; One Casing"));
            HeatSinks.Add("Lithium", new HeatSinkValues(125, "Exactly Two Lead heatsinks; One Casing"));
            HeatSinks.Add("Magnesium", new HeatSinkValues(130, "Exactly One Moderator; One Casing"));
            HeatSinks.Add("Manganese", new HeatSinkValues(140, "Two FuelCells"));
            HeatSinks.Add("Aluminum", new HeatSinkValues(185, "One Quartz heatsink; One Lapis heatsink"));
            HeatSinks.Add("Silver", new HeatSinkValues(170, "One Glowstone heatsink; One Tin heatsink"));
            HeatSinks.Add("Fluorite", new HeatSinkValues(155, "One Gold heatsink; One Prismarine heatsink"));
            HeatSinks.Add("Villiaumite", new HeatSinkValues(160, "One Reflector; One Redstone heatsink"));
            HeatSinks.Add("Carobbiite", new HeatSinkValues(150, "One Copper heatsink; One EndStone heatsink"));
            HeatSinks.Add("Arsenic", new HeatSinkValues(145, "Axial Reflectors"));
            HeatSinks.Add("Nitrogen", new HeatSinkValues(180, "One Copper heatsinks; Two Lead heatsinks"));
            HeatSinks.Add("Helium", new HeatSinkValues(200, "Exactly Two Redstone heatsinks; One Casing"));
            HeatSinks.Add("Enderium", new HeatSinkValues(175, "Three Moderators"));
            HeatSinks.Add("Cryotheum", new HeatSinkValues(205, "Three FuelCells"));

        }

        private static void SetDefaultModerators()
        {
            Moderators = new Dictionary<string, ModeratorValues>();
            Moderators.Add("Beryllium", new ModeratorValues(22, 1.05));
            Moderators.Add("Graphite", new ModeratorValues(10, 1.1));
            Moderators.Add("HeavyWater", new ModeratorValues(36, 1.0));
        }

        private static void SetDefaultFission()
        {
            Fission.Power = 1.0;
            Fission.FuelUse = 1.0;
            Fission.HeatGeneration = 1.0;
            Fission.MinSize = 1;
            Fission.MaxSize = 24;
            Fission.NeutronReach = 4;
            Fission.ReflectorEfficiency = 0.5;
            Fission.MaxSparsityPenaltyMultiplier = 0.5;
            Fission.SparsityPenaltyThreshold = 0.75;
            Fission.CoolingPenaltyLeniency = 10;
        }

        private static void SetDefaultResourceCosts()
        {
            ResourceCosts.FuelCellCosts = DefaultFuelCellCosts();
            ResourceCosts.CasingCosts = DefaultCasingCosts();
            ResourceCosts.ModeratorCosts = DefaultModeratorCosts();
            ResourceCosts.HeatSinkCosts = DefaultHeatSinkCosts();
        }

        private static Dictionary<string, int> DefaultFuelCellCosts()
        {
            Dictionary<string, int> dfcc = new Dictionary<string, int>();
            dfcc.Add("Glass", 4);
            dfcc.Add("Tough Alloy", 4);
            return dfcc;
        }

        private static Dictionary<string, int> DefaultCasingCosts()
        {
            Dictionary<string, int> dcc = new Dictionary<string, int>();
            dcc.Add("Tough Alloy", 1);
            dcc.Add("Basic Plating", 4);
            return dcc;
        }

        private static Dictionary<string, Dictionary<string, int>> DefaultModeratorCosts()
        {
            Dictionary<string, Dictionary<string, int>> dmc = new Dictionary<string, Dictionary<string, int>>();
            dmc.Add("Graphite", new Dictionary<string, int>());
            dmc["Graphite"].Add("Graphite Ingot", 9);
            dmc.Add("Beryllium", new Dictionary<string, int>());
            dmc["Beryllium"].Add("Beryllium Ingot", 9);
            return dmc;
        }

        private static Dictionary<string, Dictionary<string, int>> DefaultHeatSinkCosts()
        {
            Dictionary<string, Dictionary<string, int>> dcc = new Dictionary<string, Dictionary<string, int>>();

            foreach (KeyValuePair<string, HeatSinkValues> kvp in HeatSinks)
            {
                string heatSinkName = kvp.Key;
                dcc.Add(heatSinkName, new Dictionary<string, int>());
                dcc[heatSinkName].Add("Empty HeatSink", 1);
            }

            dcc["Water"].Add("Water Bucket", 1);

            dcc["Redstone"].Add("Redstone", 2);
            dcc["Redstone"].Add("Block of Redstone", 2);

            dcc["Quartz"].Add("Block of Quartz", 2);
            dcc["Quartz"].Add("Crushed Quartz", 2);

            dcc["Gold"].Add("Gold Ingot", 8);

            dcc["Glowstone"].Add("Glowstone", 2);
            dcc["Glowstone"].Add("Glowstone Dust", 6);

            dcc["Lapis"].Add("Lapis Lazuli Block", 2);

            dcc["Diamond"].Add("Diamond", 8);

            dcc["Helium"].Add("Liquid Helium Bucket", 1);

            dcc["Iron"].Add("Iron Ingot", 8);

            dcc["Emerald"].Add("Emerald", 6);

            dcc["Copper"].Add("Copper Ingot", 8);

            dcc["Tin"].Add("Tin Ingot", 8);

            dcc["Magnesium"].Add("Magnesium Ingot", 8);

            dcc["Boron"].Add("Boron Ingot", 8);

            dcc["Obsidian"].Add("Obsidian", 8);

            dcc["Prismarine"].Add("Prismarine Shard", 8);

            dcc["Lead"].Add("Lead Ingot", 8);

            dcc["Enderium"].Add("Enderium Ingot", 8);

            dcc["Cryotheum"].Add("Cryotheum Dust", 8);



            return dcc;
        }

        public static Dictionary<string, int> CalculateTotalResourceCosts()
        {
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (KeyValuePair<string,List<HeatSink>> c in Reactor.heatSinks)
            {
                foreach (KeyValuePair<string,int> resource in ResourceCosts.HeatSinkCosts[c.Key])
                {
                    if (!totals.ContainsKey(resource.Key))
                        totals.Add(resource.Key, 0);
                    totals[resource.Key] += resource.Value * c.Value.Count();
                }
            }

            if(Reactor.fuelCells.Count >0)
                foreach (KeyValuePair<string, int> resource in ResourceCosts.FuelCellCosts)
                {
                    if (!totals.ContainsKey(resource.Key))
                        totals.Add(resource.Key, 0);
                    totals[resource.Key] += resource.Value * Reactor.fuelCells.Count;
                }

            foreach (KeyValuePair<string, int> resource in ResourceCosts.CasingCosts)
            {
                if (!totals.ContainsKey(resource.Key))
                    totals.Add(resource.Key, 0);
                totals[resource.Key] += resource.Value * Reactor.totalCasings;
            }


            foreach (KeyValuePair<string, List<Moderator>> m in Reactor.moderators)
            {
                if(m.Value.Count > 0)
                    foreach (KeyValuePair<string, int> resource in ResourceCosts.ModeratorCosts[m.Key])
                    {
                        if (!totals.ContainsKey(resource.Key))
                            totals.Add(resource.Key, 0);
                        totals[resource.Key] += resource.Value * m.Value.Count;
                    }
            }

            return totals;
        }
    }
}
