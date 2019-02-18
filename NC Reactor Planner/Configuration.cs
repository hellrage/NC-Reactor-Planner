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
    }

    public struct FissionValues
    {
        public double Power;
        public double FuelUse;
        public double HeatGeneration;
        public int MinSize;
        public int MaxSize;
        public int NeutronReach;

        public FissionValues(double p, double fu, double hg, int ms, int mxs, int nr)
        {
            Power = p;
            FuelUse = fu;
            HeatGeneration = hg;
            MinSize = ms;
            MaxSize = mxs;
            NeutronReach = nr;
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
            if(cf.saveVersion == new Version(2,0,0,0))
            {
                System.Windows.Forms.MessageBox.Show("Ignoring old config file, please delete BetaConfig.json");
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
            Fuels.Add("[OX]LEU-233", new FuelValues(1.1, 144, 34800, 7.8));
            Fuels.Add("[OX]HEU-233", new FuelValues(1.1, 432, 34800, 3.9));
            Fuels.Add("[OX]LEU-235", new FuelValues(1, 120, 41700, 8.5));
            Fuels.Add("[OX]HEU-235", new FuelValues(1, 360, 41700, 4.2));
            Fuels.Add("[OX]LEN-236", new FuelValues(1.1, 90, 55500, 9.4));
            Fuels.Add("[OX]HEN-236", new FuelValues(1.1, 270, 55500, 4.7));
            Fuels.Add("[OX]LEP-239", new FuelValues(1.2, 105, 47600, 8.9));
            Fuels.Add("[OX]HEP-239", new FuelValues(1.2, 315, 47600, 4.4));
            Fuels.Add("[OX]LEP-241", new FuelValues(1.25, 165, 30300, 7.3));
            Fuels.Add("[OX]HEP-241", new FuelValues(1.25, 495, 30300, 3.6));
            Fuels.Add("[OX]MOX-239", new FuelValues(1.05, 112, 44600, 8.7));
            Fuels.Add("[OX]MOX-241", new FuelValues(1.15, 142, 35200, 7.9));
            Fuels.Add("[OX]LEA-242", new FuelValues(1.35, 192, 26100, 6.5));
            Fuels.Add("[OX]HEA-242", new FuelValues(1.35, 576, 26100, 3.2));
            Fuels.Add("[OX]LECm-243", new FuelValues(1.45, 210, 23800, 6.2));
            Fuels.Add("[OX]HECm-243", new FuelValues(1.45, 630, 23800, 3.1));
            Fuels.Add("[OX]LECm-245", new FuelValues(1.5, 162, 30900, 7.4));
            Fuels.Add("[OX]HECm-245", new FuelValues(1.5, 486, 30900, 3.7));
            Fuels.Add("[OX]LECm-247", new FuelValues(1.55, 138, 36200, 8));
            Fuels.Add("[OX]HECm-247", new FuelValues(1.55, 414, 36200, 4));
            Fuels.Add("[OX]LEB-248", new FuelValues(1.65, 135, 37000, 8.1));
            Fuels.Add("[OX]HEB-248", new FuelValues(1.65, 405, 37000, 4));
            Fuels.Add("[OX]LECf-249", new FuelValues(1.75, 216, 23200, 6.1));
            Fuels.Add("[OX]HECf-249", new FuelValues(1.75, 648, 23200, 3));
            Fuels.Add("[OX]LECf-251", new FuelValues(1.8, 225, 22200, 5.8));
            Fuels.Add("[OX]HECf-251", new FuelValues(1.8, 675, 22200, 2.9));

            Fuels.Add("[NI]TBU", new FuelValues(1, 32, 156260, 22));
            Fuels.Add("[NI]LEU-233", new FuelValues(1.1, 115, 43500, 9.8));
            Fuels.Add("[NI]HEU-233", new FuelValues(1.1, 346, 43500, 4.9));
            Fuels.Add("[NI]LEU-235", new FuelValues(1, 96, 52120, 10.6));
            Fuels.Add("[NI]HEU-235", new FuelValues(1, 288, 52120, 5.3));
            Fuels.Add("[NI]LEN-236", new FuelValues(1.1, 72, 69380, 11.8));
            Fuels.Add("[NI]HEN-236", new FuelValues(1.1, 216, 69380, 5.9));
            Fuels.Add("[NI]LEP-239", new FuelValues(1.2, 84, 59500, 11.1));
            Fuels.Add("[NI]HEP-239", new FuelValues(1.2, 252, 59500, 5.5));
            Fuels.Add("[NI]LEP-241", new FuelValues(1.25, 132, 37880, 9.1));
            Fuels.Add("[NI]HEP-241", new FuelValues(1.25, 396, 37880, 4.5));
            Fuels.Add("[NI]MNI-239", new FuelValues(1.05, 90, 55760, 10.9));
            Fuels.Add("[NI]MNI-241", new FuelValues(1.15, 114, 44000, 9.9));
            Fuels.Add("[NI]LEA-242", new FuelValues(1.35, 154, 32620, 8.1));
            Fuels.Add("[NI]HEA-242", new FuelValues(1.35, 461, 32620, 4));
            Fuels.Add("[NI]LECm-243", new FuelValues(1.45, 168, 29760, 7.8));
            Fuels.Add("[NI]HECm-243", new FuelValues(1.45, 504, 29760, 3.9));
            Fuels.Add("[NI]LECm-245", new FuelValues(1.5, 130, 38620, 9.3));
            Fuels.Add("[NI]HECm-245", new FuelValues(1.5, 389, 38620, 4.6));
            Fuels.Add("[NI]LECm-247", new FuelValues(1.55, 110, 45260, 10));
            Fuels.Add("[NI]HECm-247", new FuelValues(1.55, 331, 45260, 5));
            Fuels.Add("[NI]LEB-248", new FuelValues(1.65, 108, 46260, 10.1));
            Fuels.Add("[NI]HEB-248", new FuelValues(1.65, 324, 46260, 5));
            Fuels.Add("[NI]LECf-249", new FuelValues(1.75, 173, 29000, 7.6));
            Fuels.Add("[NI]HECf-249", new FuelValues(1.75, 518, 29000, 3.8));
            Fuels.Add("[NI]LECf-251", new FuelValues(1.8, 180, 27760, 7.3));
            Fuels.Add("[NI]HECf-251", new FuelValues(1.8, 540, 27760, 3.6));

            Fuels.Add("[ZA]TBU", new FuelValues(1, 48, 106260, 15));
            Fuels.Add("[ZA]LEU-233", new FuelValues(1.1, 173, 29580, 6.6));
            Fuels.Add("[ZA]HEU-233", new FuelValues(1.1, 518, 29580, 3.3));
            Fuels.Add("[ZA]LEU-235", new FuelValues(1, 144, 35440, 7.2));
            Fuels.Add("[ZA]HEU-235", new FuelValues(1, 432, 35440, 3.6));
            Fuels.Add("[ZA]LEN-236", new FuelValues(1.1, 108, 47180, 8));
            Fuels.Add("[ZA]HEN-236", new FuelValues(1.1, 324, 47180, 4));
            Fuels.Add("[ZA]LEP-239", new FuelValues(1.2, 126, 40460, 7.6));
            Fuels.Add("[ZA]HEP-239", new FuelValues(1.2, 378, 40460, 3.7));
            Fuels.Add("[ZA]LEP-241", new FuelValues(1.25, 198, 25760, 6.2));
            Fuels.Add("[ZA]HEP-241", new FuelValues(1.25, 594, 25760, 3.1));
            Fuels.Add("[ZA]MZA-239", new FuelValues(1.05, 134, 37920, 7.4));
            Fuels.Add("[ZA]MZA-241", new FuelValues(1.15, 170, 29920, 6.7));
            Fuels.Add("[ZA]LEA-242", new FuelValues(1.35, 230, 22180, 5.5));
            Fuels.Add("[ZA]HEA-242", new FuelValues(1.35, 691, 22180, 2.7));
            Fuels.Add("[ZA]LECm-243", new FuelValues(1.45, 252, 20240, 5.3));
            Fuels.Add("[ZA]HECm-243", new FuelValues(1.45, 756, 20240, 2.6));
            Fuels.Add("[ZA]LECm-245", new FuelValues(1.5, 194, 26260, 6.3));
            Fuels.Add("[ZA]HECm-245", new FuelValues(1.5, 583, 26260, 3.1));
            Fuels.Add("[ZA]LECm-247", new FuelValues(1.55, 166, 30780, 6.8));
            Fuels.Add("[ZA]HECm-247", new FuelValues(1.55, 497, 30780, 3.4));
            Fuels.Add("[ZA]LEB-248", new FuelValues(1.65, 162, 31460, 6.9));
            Fuels.Add("[ZA]HEB-248", new FuelValues(1.65, 486, 31460, 3.4));
            Fuels.Add("[ZA]LECf-249", new FuelValues(1.75, 259, 19720, 5.2));
            Fuels.Add("[ZA]HECf-249", new FuelValues(1.75, 778, 19720, 2.6));
            Fuels.Add("[ZA]LECf-251", new FuelValues(1.8, 270, 18880, 4.9));
            Fuels.Add("[ZA]HECf-251", new FuelValues(1.8, 810, 18880, 2.5));

        }

        private static void SetDefaultHeatSinks()
        {
            HeatSinks = new Dictionary<string, HeatSinkValues>();
            HeatSinks.Add("Water", new HeatSinkValues(50, "One FuelCell"));
            HeatSinks.Add("Iron", new HeatSinkValues(55, "One Moderator"));
            HeatSinks.Add("Redstone", new HeatSinkValues(70, "One FuelCell and one Moderator"));
            HeatSinks.Add("Quartz", new HeatSinkValues(75, "One Redstone heatsink"));
            HeatSinks.Add("Obsidian", new HeatSinkValues(80, "Two Glowstone heatsinks on the same axis"));
            HeatSinks.Add("Glowstone", new HeatSinkValues(110, "Two Moderators"));
            HeatSinks.Add("Lapis", new HeatSinkValues(95, "One FuelCell and one Casing"));
            HeatSinks.Add("Gold", new HeatSinkValues(105, "Two Iron heatsinks"));
            HeatSinks.Add("Prismarine", new HeatSinkValues(100, "Two Water heatsinks"));
            HeatSinks.Add("Purpur", new HeatSinkValues(90, "One Obsidian heatsink"));
            HeatSinks.Add("Diamond", new HeatSinkValues(145, "One Gold and one FuelCell"));
            HeatSinks.Add("Emerald", new HeatSinkValues(150, "One Prismarine heatsink and one Moderator"));
            HeatSinks.Add("Copper", new HeatSinkValues(60, "One Water heatsink"));
            HeatSinks.Add("Tin", new HeatSinkValues(85, "Two Lapis heatsinks on the same axis"));
            HeatSinks.Add("Lead", new HeatSinkValues(65, "One Iron heatsink"));
            HeatSinks.Add("Boron", new HeatSinkValues(120, "One Copper heatsink and one Tin heatsink"));
            HeatSinks.Add("Lithium", new HeatSinkValues(115, "One Lead heatsink and one Casing"));
            HeatSinks.Add("Magnesium", new HeatSinkValues(135, "One Moderator and one Casing"));
            HeatSinks.Add("Manganese", new HeatSinkValues(130, "Two FuelCells"));
            HeatSinks.Add("Aluminum", new HeatSinkValues(125, "One Quartz heatsink and one Casing"));
            HeatSinks.Add("Silver", new HeatSinkValues(140, "One Glowstone heatsink and one Lapis heatsink"));
            HeatSinks.Add("Helium", new HeatSinkValues(165, "Exactly two Redstone heatsinks and at least one Casing"));
            HeatSinks.Add("Enderium", new HeatSinkValues(155, "Exactly three Moderators"));
            HeatSinks.Add("Cryotheum", new HeatSinkValues(160, "Exactly three FuelCells"));

        }

        private static void SetDefaultModerators()
        {
            Moderators = new Dictionary<string, ModeratorValues>();
            Moderators.Add("Beryllium", new ModeratorValues(1.4, 1.1));
            Moderators.Add("Graphite", new ModeratorValues(10, 1.2));
            Moderators.Add("HeavyWater", new ModeratorValues(1.8, 1.0));
        }

        private static void SetDefaultFission()
        {
            Fission.Power = 1.0;
            Fission.FuelUse = 1.0;
            Fission.HeatGeneration = 1.0;
            Fission.MinSize = 1;
            Fission.MaxSize = 24;
            Fission.NeutronReach = 4;
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
