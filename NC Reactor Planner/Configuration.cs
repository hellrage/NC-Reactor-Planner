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
        public Dictionary<string, CoolerValues> Coolers;

        public ConfigFile(Version sv, FissionValues fs, Dictionary<string, FuelValues> f, Dictionary<string, CoolerValues> c, CraftingMaterials cm)
        {
            saveVersion = sv;
            Fission = fs;
            Fuels = f;
            Coolers = c;
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

    public struct CoolerValues
    {
        public double HeatPassive;
        public string Requirements;

        public CoolerValues(double hp, string req)
        {
            HeatPassive = hp;
            Requirements = req;
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
        public double ModeratorExtraPower;
        public double ModeratorExtraHeat;

        public FissionValues(double p, double fu, double hg, int ms, int mxs, int nr, double mep, double meh)
        {
            Power = p;
            FuelUse = fu;
            HeatGeneration = hg;
            MinSize = ms;
            MaxSize = mxs;
            NeutronReach = nr;
            ModeratorExtraPower = mep;
            ModeratorExtraHeat = meh;
        }
    }

    public struct CraftingMaterials
    {
        public Dictionary<string, Dictionary<string, int>> CoolerCosts;
        public Dictionary<string, Dictionary<string, int>> ModeratorCosts;
        public Dictionary<string, int> FuelCellCosts;
        public Dictionary<string, int> CasingCosts;

        public CraftingMaterials(Dictionary<string, Dictionary<string, int>> clc, Dictionary<string, Dictionary<string, int>> mc, Dictionary<string, int> fcc, Dictionary<string, int> csc)
        {
            CoolerCosts = clc;
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
        public static Dictionary<string, CoolerValues> Coolers;

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
            if((cf.Fuels == null) | (cf.Coolers == null))
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
            Coolers = cf.Coolers;
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

                ConfigFile cf = new ConfigFile(Reactor.saveVersion, Fission, Fuels, Coolers, ResourceCosts);
                jss.Serialize(tw, cf);
            }
        }

        public static void ResetToDefaults()
        {
            configFileInfo = null;

            Coolers = new Dictionary<string, CoolerValues>();
            SetDefaultCoolers();

            Fuels = new Dictionary<string, FuelValues>();
            SetDefaultFuels();

            SetDefaultFission();

            //SetDefaultResourceCosts();
        }

        private static void SetDefaultFuels()
        {
            Fuels.Add("TBU", new FuelValues(1, 18, 144000, 1));
            Fuels.Add("TBU Oxide", new FuelValues(1, 22.5, 144000, 1));
            Fuels.Add("LEU-233", new FuelValues(1, 60, 64000, 1));
            Fuels.Add("LEU-233 Oxide", new FuelValues(1, 75, 64000, 1));
            Fuels.Add("HEU-233", new FuelValues(1, 360, 64000, 1));
            Fuels.Add("HEU-233 Oxide", new FuelValues(1, 450, 64000, 1));
            Fuels.Add("LEU-235", new FuelValues(1, 50, 72000, 1));
            Fuels.Add("LEU-235 Oxide", new FuelValues(1, 62.5, 72000, 1));
            Fuels.Add("HEU-235", new FuelValues(1, 300, 72000, 1));
            Fuels.Add("HEU-235 Oxide", new FuelValues(1, 375, 72000, 1));
            Fuels.Add("LEN-236", new FuelValues(1, 36, 102000, 1));
            Fuels.Add("LEN-236 Oxide", new FuelValues(1, 45, 102000, 1));
            Fuels.Add("HEN-236", new FuelValues(1, 216, 102000, 1));
            Fuels.Add("HEN-236 Oxide", new FuelValues(1, 270, 102000, 1));
            Fuels.Add("LEP-239", new FuelValues(1, 40, 92000, 1));
            Fuels.Add("LEP-239 Oxide", new FuelValues(1, 50, 92000, 1));
            Fuels.Add("HEP-239", new FuelValues(1, 240, 92000, 1));
            Fuels.Add("HEP-239 Oxide", new FuelValues(1, 300, 92000, 1));
            Fuels.Add("LEP-241", new FuelValues(1, 70, 60000, 1));
            Fuels.Add("LEP-241 Oxide", new FuelValues(1, 87.5, 60000, 1));
            Fuels.Add("HEP-241", new FuelValues(1, 420, 60000, 1));
            Fuels.Add("HEP-241 Oxide", new FuelValues(1, 525, 60000, 1));
            Fuels.Add("MOX-239", new FuelValues(1, 57.5, 84000, 1));
            Fuels.Add("MOX-241", new FuelValues(1, 97.5, 56000, 1));
            Fuels.Add("LEA-242", new FuelValues(1, 94, 54000, 1));
            Fuels.Add("LEA-242 Oxide", new FuelValues(1, 117.5, 54000, 1));
            Fuels.Add("HEA-242", new FuelValues(1, 564, 54000, 1));
            Fuels.Add("HEA-242 Oxide", new FuelValues(1, 705, 54000, 1));
            Fuels.Add("LECm-243", new FuelValues(1, 112, 52000, 1));
            Fuels.Add("LECm-243 Oxide", new FuelValues(1, 140, 52000, 1));
            Fuels.Add("HECm-243", new FuelValues(1, 672, 52000, 1));
            Fuels.Add("HECm-243 Oxide", new FuelValues(1, 840, 52000, 1));
            Fuels.Add("LECm-245", new FuelValues(1, 68, 68000, 1));
            Fuels.Add("LECm-245 Oxide", new FuelValues(1, 85, 68000, 1));
            Fuels.Add("HECm-245", new FuelValues(1, 408, 68000, 1));
            Fuels.Add("HECm-245 Oxide", new FuelValues(1, 510, 68000, 1));
            Fuels.Add("LECm-247", new FuelValues(1, 54, 78000, 1));
            Fuels.Add("LECm-247 Oxide", new FuelValues(1, 67.5, 78000, 1));
            Fuels.Add("HECm-247", new FuelValues(1, 324, 78000, 1));
            Fuels.Add("HECm-247 Oxide", new FuelValues(1, 405, 78000, 1));
            Fuels.Add("LEB-248", new FuelValues(1, 52, 86000, 1));
            Fuels.Add("LEB-248 Oxide", new FuelValues(1, 65, 86000, 1));
            Fuels.Add("HEB-248", new FuelValues(1, 312, 86000, 1));
            Fuels.Add("HEB-248 Oxide", new FuelValues(1, 390, 86000, 1));
            Fuels.Add("LECf-249", new FuelValues(1, 116, 60000, 1));
            Fuels.Add("LECf-249 Oxide", new FuelValues(1, 145, 60000, 1));
            Fuels.Add("HECf-249", new FuelValues(1, 696, 60000, 1));
            Fuels.Add("HECf-249 Oxide", new FuelValues(1, 870, 60000, 1));
            Fuels.Add("LECf-251", new FuelValues(1, 120, 58000, 1));
            Fuels.Add("LECf-251 Oxide", new FuelValues(1, 150, 58000, 1));
            Fuels.Add("HECf-251", new FuelValues(1, 720, 58000, 1));
            Fuels.Add("HECf-251 Oxide", new FuelValues(1, 900, 58000, 1));
        }

        private static void SetDefaultCoolers()
        {
            Coolers.Add("Water", new CoolerValues(55, "One FuelCell"));
            Coolers.Add("Iron", new CoolerValues(60, "One Moderator"));
            Coolers.Add("Redstone", new CoolerValues(85, "One FuelCell and one Moderator"));
            Coolers.Add("Quartz", new CoolerValues(90, "One Magnesium"));
            Coolers.Add("Obsidian", new CoolerValues(80, "One Glowstone and one Casing"));
            Coolers.Add("Glowstone", new CoolerValues(115, "Two Moderators"));
            Coolers.Add("Lapis", new CoolerValues(100, "One FuelCell and one Casing"));
            Coolers.Add("Gold", new CoolerValues(110, "Two Iron"));
            Coolers.Add("Prismarine", new CoolerValues(125, "Two Water"));
            Coolers.Add("Diamond", new CoolerValues(130, "One Gold and one FuelCell"));
            Coolers.Add("Emerald", new CoolerValues(135, "One Prismarine and one Moderator"));
            Coolers.Add("Copper", new CoolerValues(65, "One Water"));
            Coolers.Add("Tin", new CoolerValues(75, "Two Lapis"));
            Coolers.Add("Lead", new CoolerValues(70, "One Iron"));
            Coolers.Add("Bronze", new CoolerValues(105, "One Copper and one Tin"));
            Coolers.Add("Boron", new CoolerValues(95, "One Bronze"));
            Coolers.Add("Magnesium", new CoolerValues(120, "One Lead and one Casing"));
            Coolers.Add("Helium", new CoolerValues(150, "Two Redstone and one Casing"));
            Coolers.Add("Enderium", new CoolerValues(140, "Three Moderators"));
            Coolers.Add("Cryotheum", new CoolerValues(145, "Three FuelCells"));

        }

        private static void SetDefaultFission()
        {
            Fission.Power = 1.0;
            Fission.FuelUse = 1.0;
            Fission.HeatGeneration = 1.0;
            Fission.MinSize = 1;
            Fission.MaxSize = 24;
            Fission.ModeratorExtraPower = 1.0;
            Fission.ModeratorExtraHeat = 2.0;
            Fission.NeutronReach = 4;
        }

        private static void SetDefaultResourceCosts()
        {
            ResourceCosts.FuelCellCosts = DefaultFuelCellCosts();
            ResourceCosts.CasingCosts = DefaultCasingCosts();
            ResourceCosts.ModeratorCosts = DefaultModeratorCosts();
            ResourceCosts.CoolerCosts = DefaultCoolerCosts();
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

        private static Dictionary<string, Dictionary<string, int>> DefaultCoolerCosts()
        {
            Dictionary<string, Dictionary<string, int>> dcc = new Dictionary<string, Dictionary<string, int>>();

            foreach (HeatSink cooler in Palette.coolers)
            {
                dcc.Add(cooler.DisplayName, new Dictionary<string, int>());
                dcc[cooler.DisplayName].Add("Empty HeatSink", 1);
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

            dcc["Bronze"].Add("Bronze Ingot", 8);

            dcc["Enderium"].Add("Enderium Ingot", 8);

            dcc["Cryotheum"].Add("Cryotheum Dust", 8);



            return dcc;
        }

        public static Dictionary<string, int> CalculateTotalResourceCosts()
        {
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (KeyValuePair<string,List<HeatSink>> c in Reactor.heatSinks)
            {
                foreach (KeyValuePair<string,int> resource in ResourceCosts.CoolerCosts[c.Key])
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
