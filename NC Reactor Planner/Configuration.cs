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

            if(cf.saveVersion < new Version(2,0,0))
            {
                System.Windows.Forms.MessageBox.Show("Pre-overhaul configurations aren't supported!\r\nDelete your DefaultConfig.json to regenerate a new one.");
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
            Fuels.Add("TBU", new FuelValues(1, 18, 144000, 1));
            Fuels.Add("TBU Oxide", new FuelValues(1.01, 22.5, 144000, 1));
            Fuels.Add("LEU-233", new FuelValues(1.05, 60, 102000, 1.2));
            Fuels.Add("LEU-233 Oxide", new FuelValues(1.06, 75, 102000, 1.2));
            Fuels.Add("HEU-233", new FuelValues(1.38, 360, 102000, 2.5));
            Fuels.Add("HEU-233 Oxide", new FuelValues(1.48, 450, 102000, 2.9));
            Fuels.Add("LEU-235", new FuelValues(1.04, 50, 91920, 1.2));
            Fuels.Add("LEU-235 Oxide", new FuelValues(1.05, 62.5, 91920, 1.2));
            Fuels.Add("HEU-235", new FuelValues(1.32, 300, 91920, 2.3));
            Fuels.Add("HEU-235 Oxide", new FuelValues(1.4, 375, 91920, 2.6));
            Fuels.Add("LEN-236", new FuelValues(1.02, 36, 91920, 1.1));
            Fuels.Add("LEN-236 Oxide", new FuelValues(1.03, 45, 91920, 1.1));
            Fuels.Add("HEN-236", new FuelValues(1.22, 216, 91920, 1.9));
            Fuels.Add("HEN-236 Oxide", new FuelValues(1.28, 270, 91920, 2.1));
            Fuels.Add("LEP-239", new FuelValues(1.02, 40, 86040, 1.1));
            Fuels.Add("LEP-239 Oxide", new FuelValues(1.04, 50, 86040, 1.2));
            Fuels.Add("HEP-239", new FuelValues(1.25, 240, 86040, 2));
            Fuels.Add("HEP-239 Oxide", new FuelValues(1.32, 300, 86040, 2.3));
            Fuels.Add("LEP-241", new FuelValues(1.06, 70, 84000, 1.2));
            Fuels.Add("LEP-241 Oxide", new FuelValues(1.08, 87.5, 78000, 1.3));
            Fuels.Add("HEP-241", new FuelValues(1.45, 420, 78000, 2.8));
            Fuels.Add("HEP-241 Oxide", new FuelValues(1.57, 525, 78000, 3.3));
            Fuels.Add("MOX-239", new FuelValues(1.04, 57.5, 78000, 1.2));
            Fuels.Add("MOX-241", new FuelValues(1.09, 97.5, 72000, 1.4));
            Fuels.Add("LEA-242", new FuelValues(1.09, 94, 72000, 1.4));
            Fuels.Add("LEA-242 Oxide", new FuelValues(1.11, 117.5, 72000, 1.4));
            Fuels.Add("HEA-242", new FuelValues(1.61, 564, 72000, 3.4));
            Fuels.Add("HEA-242 Oxide", new FuelValues(1.77, 705, 67920, 4.1));
            Fuels.Add("LECm-243", new FuelValues(1.11, 112, 67920, 1.4));
            Fuels.Add("LECm-243 Oxide", new FuelValues(1.14, 140, 67920, 1.6));
            Fuels.Add("HECm-243", new FuelValues(1.73, 672, 67920, 3.9));
            Fuels.Add("HECm-243 Oxide", new FuelValues(1.92, 840, 63960, 4.7));
            Fuels.Add("LECm-245", new FuelValues(1.06, 68, 63960, 1.2));
            Fuels.Add("LECm-245 Oxide", new FuelValues(1.08, 85, 63960, 1.3));
            Fuels.Add("HECm-245", new FuelValues(1.44, 408, 63960, 2.8));
            Fuels.Add("HECm-245 Oxide", new FuelValues(1.55, 510, 60000, 3.2));
            Fuels.Add("LECm-247", new FuelValues(1.04, 54, 60000, 1.2));
            Fuels.Add("LECm-247 Oxide", new FuelValues(1.06, 67.5, 60000, 1.2));
            Fuels.Add("HECm-247", new FuelValues(1.34, 324, 60000, 2.4));
            Fuels.Add("HECm-247 Oxide", new FuelValues(1.43, 405, 57960, 2.7));
            Fuels.Add("LEB-248", new FuelValues(1.04, 52, 57960, 1.2));
            Fuels.Add("LEB-248 Oxide", new FuelValues(1.05, 65, 57960, 1.2));
            Fuels.Add("HEB-248", new FuelValues(1.33, 312, 57960, 2.3));
            Fuels.Add("HEB-248 Oxide", new FuelValues(1.43, 398, 55920, 2.7));
            Fuels.Add("LECf-249", new FuelValues(1.11, 116, 54000, 1.4));
            Fuels.Add("LECf-249 Oxide", new FuelValues(1.14, 145, 54000, 1.6));
            Fuels.Add("HECf-249", new FuelValues(1.76, 696, 54000, 4));
            Fuels.Add("HECf-249 Oxide", new FuelValues(1.96, 870, 54000, 4.8));
            Fuels.Add("LECf-251", new FuelValues(1.11, 120, 51960, 1.4));
            Fuels.Add("LECf-251 Oxide", new FuelValues(1.15, 150, 51960, 1.6));
            Fuels.Add("HECf-251", new FuelValues(1.79, 720, 51960, 4.2));
            Fuels.Add("HECf-251 Oxide", new FuelValues(1.99, 900, 51960, 5));

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
            Moderators.Add("Beryllium", new ModeratorValues(1.0, 1.2));
            Moderators.Add("Graphite", new ModeratorValues(1.2, 1.0));
            Moderators.Add("HeavyWater", new ModeratorValues(1.1, 1.1));
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
