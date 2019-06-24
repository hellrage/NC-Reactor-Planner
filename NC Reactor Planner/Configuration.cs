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
        public double CriticalityFactor;

        public FuelValues(double be, double bh, double ft, double cf)
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
            CriticalityFactor = Convert.ToDouble(values[3]);
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
        public double FluxFactor;
        public double EfficiencyFactor;

        public ModeratorValues(double ff, double ef)
        {
            FluxFactor = ff;
            EfficiencyFactor = ef;
        }

        public ModeratorValues(List<object> fieldValues)
        {
            FluxFactor = Convert.ToDouble(fieldValues[0]);
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

        public FissionValues(double p, double fu, double hg, int ms, int mxs, int nr, double re)
        {
            Power = p;
            FuelUse = fu;
            HeatGeneration = hg;
            MinSize = ms;
            MaxSize = mxs;
            NeutronReach = nr;
            ReflectorEfficiency = re;        }

        public FissionValues(List<object> values)
        {
            Power = Convert.ToDouble(values[0]);
            FuelUse = Convert.ToDouble(values[1]);
            HeatGeneration = Convert.ToDouble(values[2]);
            MinSize = Convert.ToInt32(values[3]);
            MaxSize = Convert.ToInt32(values[4]);
            NeutronReach = Convert.ToInt32(values[5]);
            ReflectorEfficiency = Convert.ToDouble(values[6]);
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
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\nConfig file was corrupt!");
                    return false;
                }
            }

            if(cf.saveVersion < new Version(2,0,0,0))
            {
                System.Windows.Forms.MessageBox.Show("Pre-overhaul configurations aren't supported!\r\nDelete your DefaultConfig.json to regenerate a new one.");
                return false;
            }
            if(cf.saveVersion < new Version(2, 0, 15, 0))
            {
                System.Windows.Forms.MessageBox.Show("Ignoring old config file as the values have changed, please overwrite BetaConfig.json");
                return false;
            }

            if((cf.Fuels == null) | (cf.HeatSinks == null))
            {
                System.Windows.Forms.MessageBox.Show("Invalid config file contents!");
                return false;
            }

            if(cf.saveVersion <= new Version(1,2,3))
            {
                var fuelvalue = cf.Fuels["HELP-239  Oxide"];
                cf.Fuels.Remove("HELP-239  Oxide");
                cf.Fuels.Add("HEP-239 Oxide", fuelvalue);
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
            Reactor.ReloadValuesFromConfig();
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
        }

        private static void SetDefaultFuels()
        {
            Fuels = new Dictionary<string, FuelValues>();
            Fuels.Add("[OX]TBU", new FuelValues(1, 40, 125000, 17.6));
            Fuels.Add("[OX]LEU-233", new FuelValues(1.1, 144, 34700, 7.8));
            Fuels.Add("[OX]HEU-233", new FuelValues(1.1, 432, 34700, 3.9));
            Fuels.Add("[OX]LEU-235", new FuelValues(1, 120, 41700, 8.5));
            Fuels.Add("[OX]HEU-235", new FuelValues(1, 360, 41700, 4.2));
            Fuels.Add("[OX]LEN-236", new FuelValues(1.1, 90, 55600, 9.4));
            Fuels.Add("[OX]HEN-236", new FuelValues(1.1, 270, 55600, 4.7));
            Fuels.Add("[OX]LEP-239", new FuelValues(1.2, 105, 47600, 8.9));
            Fuels.Add("[OX]HEP-239", new FuelValues(1.2, 315, 47600, 4.4));
            Fuels.Add("[OX]LEP-241", new FuelValues(1.25, 165, 30300, 7.3));
            Fuels.Add("[OX]HEP-241", new FuelValues(1.25, 495, 30300, 3.6));
            Fuels.Add("[OX]MOX-239", new FuelValues(1.05, 112, 44600, 8.7));
            Fuels.Add("[OX]MOX-241", new FuelValues(1.15, 142, 35200, 7.9));
            Fuels.Add("[OX]LEA-242", new FuelValues(1.35, 192, 26000, 6.5));
            Fuels.Add("[OX]HEA-242", new FuelValues(1.35, 576, 26000, 3.2));
            Fuels.Add("[OX]LECm-243", new FuelValues(1.45, 210, 23800, 6.2));
            Fuels.Add("[OX]HECm-243", new FuelValues(1.45, 630, 23800, 3.1));
            Fuels.Add("[OX]LECm-245", new FuelValues(1.5, 162, 30900, 7.4));
            Fuels.Add("[OX]HECm-245", new FuelValues(1.5, 486, 30900, 3.7));
            Fuels.Add("[OX]LECm-247", new FuelValues(1.55, 138, 36200, 8));
            Fuels.Add("[OX]HECm-247", new FuelValues(1.55, 414, 36200, 4));
            Fuels.Add("[OX]LEB-248", new FuelValues(1.65, 135, 37000, 8.1));
            Fuels.Add("[OX]HEB-248", new FuelValues(1.65, 405, 37000, 4));
            Fuels.Add("[OX]LECf-249", new FuelValues(1.75, 216, 23100, 6.1));
            Fuels.Add("[OX]HECf-249", new FuelValues(1.75, 648, 23100, 3));
            Fuels.Add("[OX]LECf-251", new FuelValues(1.8, 225, 22200, 5.8));
            Fuels.Add("[OX]HECf-251", new FuelValues(1.8, 675, 22200, 2.9));

            Fuels.Add("[NI]TBU", new FuelValues(1, 32, 156300, 22));
            Fuels.Add("[NI]LEU-233", new FuelValues(1.1, 115, 43500, 9.8));
            Fuels.Add("[NI]HEU-233", new FuelValues(1.1, 345, 43500, 4.9));
            Fuels.Add("[NI]LEU-235", new FuelValues(1, 96, 52100, 10.6));
            Fuels.Add("[NI]HEU-235", new FuelValues(1, 288, 52100, 5.3));
            Fuels.Add("[NI]LEN-236", new FuelValues(1.1, 72, 69400, 11.8));
            Fuels.Add("[NI]HEN-236", new FuelValues(1.1, 216, 69400, 5.9));
            Fuels.Add("[NI]LEP-239", new FuelValues(1.2, 84, 59500, 11.1));
            Fuels.Add("[NI]HEP-239", new FuelValues(1.2, 252, 59500, 5.5));
            Fuels.Add("[NI]LEP-241", new FuelValues(1.25, 132, 37900, 9.1));
            Fuels.Add("[NI]HEP-241", new FuelValues(1.25, 396, 37900, 4.5));
            Fuels.Add("[NI]MNI-239", new FuelValues(1.05, 90, 55600, 10.9));
            Fuels.Add("[NI]MNI-241", new FuelValues(1.15, 114, 43900, 9.9));
            Fuels.Add("[NI]LEA-242", new FuelValues(1.35, 154, 32500, 8.1));
            Fuels.Add("[NI]HEA-242", new FuelValues(1.35, 462, 32500, 4));
            Fuels.Add("[NI]LECm-243", new FuelValues(1.45, 168, 29800, 7.8));
            Fuels.Add("[NI]HECm-243", new FuelValues(1.45, 504, 29800, 3.9));
            Fuels.Add("[NI]LECm-245", new FuelValues(1.5, 130, 38500, 9.3));
            Fuels.Add("[NI]HECm-245", new FuelValues(1.5, 390, 38500, 4.6));
            Fuels.Add("[NI]LECm-247", new FuelValues(1.55, 110, 45500, 10));
            Fuels.Add("[NI]HECm-247", new FuelValues(1.55, 330, 45500, 5));
            Fuels.Add("[NI]LEB-248", new FuelValues(1.65, 108, 46300, 10.1));
            Fuels.Add("[NI]HEB-248", new FuelValues(1.65, 324, 46300, 5));
            Fuels.Add("[NI]LECf-249", new FuelValues(1.75, 173, 28900, 7.6));
            Fuels.Add("[NI]HECf-249", new FuelValues(1.75, 519, 28900, 3.8));
            Fuels.Add("[NI]LECf-251", new FuelValues(1.8, 180, 27800, 7.3));
            Fuels.Add("[NI]HECf-251", new FuelValues(1.8, 540, 27800, 3.6));

            Fuels.Add("[ZA]TBU", new FuelValues(1, 48, 104200, 15));
            Fuels.Add("[ZA]LEU-233", new FuelValues(1.1, 173, 28900, 6.6));
            Fuels.Add("[ZA]HEU-233", new FuelValues(1.1, 519, 28900, 3.3));
            Fuels.Add("[ZA]LEU-235", new FuelValues(1, 144, 34700, 7.2));
            Fuels.Add("[ZA]HEU-235", new FuelValues(1, 432, 34700, 3.6));
            Fuels.Add("[ZA]LEN-236", new FuelValues(1.1, 108, 46300, 8));
            Fuels.Add("[ZA]HEN-236", new FuelValues(1.1, 324, 46300, 4));
            Fuels.Add("[ZA]LEP-239", new FuelValues(1.2, 126, 39700, 7.6));
            Fuels.Add("[ZA]HEP-239", new FuelValues(1.2, 378, 39700, 3.8));
            Fuels.Add("[ZA]LEP-241", new FuelValues(1.25, 198, 25300, 6.2));
            Fuels.Add("[ZA]HEP-241", new FuelValues(1.25, 594, 25300, 3.1));
            Fuels.Add("[ZA]MZA-239", new FuelValues(1.05, 134, 37300, 7.4));
            Fuels.Add("[ZA]MZA-241", new FuelValues(1.15, 170, 29400, 6.7));
            Fuels.Add("[ZA]LEA-242", new FuelValues(1.35, 230, 21700, 5.5));
            Fuels.Add("[ZA]HEA-242", new FuelValues(1.35, 690, 21700, 2.7));
            Fuels.Add("[ZA]LECm-243", new FuelValues(1.45, 252, 19800, 5.3));
            Fuels.Add("[ZA]HECm-243", new FuelValues(1.45, 756, 19800, 2.6));
            Fuels.Add("[ZA]LECm-245", new FuelValues(1.5, 194, 25800, 6.3));
            Fuels.Add("[ZA]HECm-245", new FuelValues(1.5, 582, 25800, 3.1));
            Fuels.Add("[ZA]LECm-247", new FuelValues(1.55, 166, 30100, 6.8));
            Fuels.Add("[ZA]HECm-247", new FuelValues(1.55, 498, 30100, 3.4));
            Fuels.Add("[ZA]LEB-248", new FuelValues(1.65, 162, 30900, 6.9));
            Fuels.Add("[ZA]HEB-248", new FuelValues(1.65, 486, 30900, 3.4));
            Fuels.Add("[ZA]LECf-249", new FuelValues(1.75, 259, 19300, 5.2));
            Fuels.Add("[ZA]HECf-249", new FuelValues(1.75, 777, 19300, 2.6));
            Fuels.Add("[ZA]LECf-251", new FuelValues(1.8, 270, 18500, 4.9));
            Fuels.Add("[ZA]HECf-251", new FuelValues(1.8, 810, 18500, 2.5));

        }

        private static void SetDefaultHeatSinks()
        {
            HeatSinks = new Dictionary<string, HeatSinkValues>();
            HeatSinks.Add("Water", new HeatSinkValues(50, "One FuelCell"));
            HeatSinks.Add("Iron", new HeatSinkValues(55, "One Moderator"));
            HeatSinks.Add("Redstone", new HeatSinkValues(85, "One FuelCell and one Moderator"));
            HeatSinks.Add("Quartz", new HeatSinkValues(75, "One Redstone heatsink"));
            HeatSinks.Add("Obsidian", new HeatSinkValues(70, "Two Glowstone heatsinks on the same axis"));
            HeatSinks.Add("Glowstone", new HeatSinkValues(115, "Two Moderators"));
            HeatSinks.Add("Lapis", new HeatSinkValues(95, "One FuelCell and one Casing"));
            HeatSinks.Add("Gold", new HeatSinkValues(100, "Two Iron heatsinks"));
            HeatSinks.Add("Prismarine", new HeatSinkValues(110, "Two Water heatsinks"));
            HeatSinks.Add("Purpur", new HeatSinkValues(90, "Exactly one Iron heatsink and at least one EndStone heatsink"));
            HeatSinks.Add("Diamond", new HeatSinkValues(180, "One Gold and one FuelCell"));
            HeatSinks.Add("Emerald", new HeatSinkValues(190, "One Prismarine heatsink and one Moderator"));
            HeatSinks.Add("Copper", new HeatSinkValues(80, "One Water heatsink"));
            HeatSinks.Add("Tin", new HeatSinkValues(120, "Two Lapis heatsinks on the same axis"));
            HeatSinks.Add("Lead", new HeatSinkValues(65, "One Iron heatsink"));
            HeatSinks.Add("Boron", new HeatSinkValues(165, "Exactly one Quartz heatsink and at least one Casing"));
            HeatSinks.Add("Lithium", new HeatSinkValues(130, "Exactly one Lead heatsink and at least one Casing"));
            HeatSinks.Add("Magnesium", new HeatSinkValues(135, "One Moderator and one Casing"));
            HeatSinks.Add("Manganese", new HeatSinkValues(140, "Two FuelCells"));
            HeatSinks.Add("Aluminum", new HeatSinkValues(175, "One Quartz heatsink and one Tin heatsink"));
            HeatSinks.Add("Silver", new HeatSinkValues(170, "One Glowstone heatsink and one Lapis heatsink"));
            HeatSinks.Add("Helium", new HeatSinkValues(195, "Exactly two Redstone heatsinks and at least one Casing"));
            HeatSinks.Add("Enderium", new HeatSinkValues(185, "Three Moderators"));
            HeatSinks.Add("Cryotheum", new HeatSinkValues(200, "Three FuelCells"));
            HeatSinks.Add("Carobbiite", new HeatSinkValues(160, "One Copper heatsink and one EndStone heatsink"));
            HeatSinks.Add("Fluorite", new HeatSinkValues(150, "One Gold heatsink and one Prismarine heatsink"));
            HeatSinks.Add("Villiaumite", new HeatSinkValues(155, "One Reflector and one Redstone"));
            HeatSinks.Add("Arsenic", new HeatSinkValues(145, "Two Reflectors on the same axis"));
            HeatSinks.Add("TCAlloy", new HeatSinkValues(205, "One FuelCell, one Moderator and one Reflector on the same vertex"));
            HeatSinks.Add("EndStone", new HeatSinkValues(60, "One Reflector"));
            HeatSinks.Add("Slime", new HeatSinkValues(125, "Exactly one Water heatsink and at least one Reflector"));
            HeatSinks.Add("NetherBrick", new HeatSinkValues(105, "One Obsidian heatsink"));

        }

        private static void SetDefaultModerators()
        {
            Moderators = new Dictionary<string, ModeratorValues>();
            Moderators.Add("Beryllium", new ModeratorValues(2.2, 1.1));
            Moderators.Add("Graphite", new ModeratorValues(1.0, 1.2));
            Moderators.Add("HeavyWater", new ModeratorValues(3.6, 1.0));
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
