using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NC_Reactor_Planner
{
    public struct ResourceCostComboboxItem
    {
        public string DisplayText;
        public Dictionary<string,int> Resources;

        public ResourceCostComboboxItem(string dt, Dictionary<string, int> rs)
        {
            DisplayText = dt;
            Resources = rs;
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
    public partial class ConfigurationUI : Form
    {
        private Dictionary<string, List<Control>> cIFR; //cooler input field rows
        private Dictionary<string, List<Control>> fIFR; //fuel input field rows
        private Dictionary<string, List<Control>> rDC; //resource Disposable Controls

        public ConfigurationUI()
        {
            InitializeComponent();

            power.Validating += CheckDoubleValue;
            fuelUse.Validating += CheckDoubleValue;
            heatGeneration.Validating += CheckDoubleValue;
            moderatorExtraPower.Validating += CheckDoubleValue;
            moderatorExtraHeat.Validating += CheckDoubleValue;
            neutronReach.Validating += CheckIntValue;
            minSize.Validating += CheckIntValue;
            maxSize.Validating += CheckIntValue;
        }

        private void ConfigurationUI_Load(object sender, EventArgs e)
        {
            ReloadTabs();
        }

        private void ReloadTabs()
        {
            ReloadCoolersTab();
            ReloadFuelsTab();
            ReloadFissionTab();
            ReloadResourceCostTab();
            blockSelector.SelectedIndexChanged += new EventHandler(SelectedBlockChanged);
        }

        private void ReloadCoolersTab()
        {
            if(cIFR != null)
                DisposeAndClear(cIFR);
            cIFR = new Dictionary<string, List<Control>>(); //cooler input field rows
            int row = 0;
            foreach (KeyValuePair<string, CoolerValues> coolerEntry in Configuration.Coolers)
            {
                CoolerValues cv = coolerEntry.Value;
                row++;
                int y = 3 + row * 20;
                List<Control> fields = new List<Control>();
                fields.Add(new Label { Text = coolerEntry.Key, Location = new Point(3, y), Size = new Size(70, 13), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fields.Add(new TextBox { Text = cv.HeatPassive.ToString(), Location = new Point(85, y), Size = new Size(70, 14), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fields.Add(new TextBox { Text = cv.HeatActive.ToString(), Location = new Point(165, y), Size = new Size(110, 14), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fields.Add(new TextBox { Text = cv.Requirements, Location = new Point(285, y), Size = new Size(350, 14) });
                cIFR.Add(coolerEntry.Key, fields);
                //fields.Clear();
            }

            foreach (KeyValuePair<string, List<Control>> slkvp in cIFR)
            {
                foreach (Control c in slkvp.Value)
                {
                    settingTabs.TabPages["coolersPage"].Controls.Add(c);
                }
            }
        }

        private void ReloadFuelsTab()
        {
            if (fIFR != null)
                DisposeAndClear(fIFR);
            fIFR = new Dictionary<string, List<Control>>(); //Fuel input field rows
            int row = 0;
            foreach (KeyValuePair<string, FuelValues> fuelEntry in Configuration.Fuels)
            {
                FuelValues fv = fuelEntry.Value;
                row++;
                int y = 3 + row * 20;
                List<Control> fields = new List<Control>();
                fields.Add(new Label { Text = fuelEntry.Key, Location = new Point(3, y), Size = new Size(150, 13) });
                fields.Add(new TextBox { Text = fv.BasePower.ToString(), Location = new Point(160, y), Size = new Size(75, 14), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fields.Add(new TextBox { Text = fv.BaseHeat.ToString(), Location = new Point(240, y), Size = new Size(70, 14), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fields.Add(new TextBox { Text = fv.FuelTime.ToString(), Location = new Point(312, y), Size = new Size(70, 14), CausesValidation = true }.Set(x => { x.Validating += CheckDoubleValue; }));
                fIFR.Add(fuelEntry.Key, fields);
            }

            foreach (KeyValuePair<string, List<Control>> slkvp in fIFR)
            {
                foreach (Control c in slkvp.Value)
                {
                    settingTabs.TabPages["fuelsPage"].Controls.Add(c);
                }
            }
        }

        private void ReloadFissionTab()
        {
            power.Text = Configuration.Fission.Power.ToString();
            fuelUse.Text = Configuration.Fission.FuelUse.ToString();
            heatGeneration.Text = Configuration.Fission.HeatGeneration.ToString();
            moderatorExtraPower.Text = Configuration.Fission.ModeratorExtraPower.ToString();
            moderatorExtraHeat.Text = Configuration.Fission.ModeratorExtraHeat.ToString();
            neutronReach.Text = Configuration.Fission.NeutronReach.ToString();
            minSize.Text = Configuration.Fission.MinSize.ToString();
            maxSize.Text = Configuration.Fission.MaxSize.ToString();
        }

        private void ReloadResourceCostTab()
        {
            blockSelector.Items.Add(new ResourceCostComboboxItem("Fuel cell", Configuration.ResourceCosts.FuelCellCosts));
            blockSelector.Items.Add(new ResourceCostComboboxItem("Casing", Configuration.ResourceCosts.CasingCosts));

            foreach (KeyValuePair<string, Dictionary<string, int>> kvp in Configuration.ResourceCosts.CoolerCosts)
            {
                blockSelector.Items.Add(new ResourceCostComboboxItem(kvp.Key + " Cooler", kvp.Value));
            }

            foreach (KeyValuePair<string, Dictionary<string, int>> kvp in Configuration.ResourceCosts.ModeratorCosts)
            {
                blockSelector.Items.Add(new ResourceCostComboboxItem(kvp.Key + " Moderator", kvp.Value));
            }

        }

        private void SelectedBlockChanged(object sender, EventArgs e)
        {
            if (rDC != null)
            {
                foreach (KeyValuePair<string, List<Control>> kvp in rDC)
                    foreach (Control c in kvp.Value)
                    { 
                        c.Dispose();
                        resourceCostsTab.Controls.Remove(c);
                    }
            }
            rDC = new Dictionary<string, List<Control>>();

            int row = 1;
            foreach(KeyValuePair<string, int> resource in ((ResourceCostComboboxItem)blockSelector.SelectedItem).Resources)
            {
                rDC[resource.Key] = new List<Control>();
                rDC[resource.Key].Add(new TextBox { Text = resource.Key, Location = new Point(10, row * 30) });
                rDC[resource.Key].Add(new NumericUpDown { Value = resource.Value, Location = new Point(130, row * 30) });
                row++;
                //rDC.Add(new Button { Text = resource.Key, Location = new Point(10, row++ * 20) });
                resourceCostsTab.Controls.AddRange(rDC[resource.Key].ToArray());
            }

            
        }
        
        private void DisposeAndClear(Dictionary<string, List<Control>> cl)
        {
            foreach (KeyValuePair<string, List<Control>> cle in cl)
            {
                foreach (Control c in cle.Value)
                {
                    c.Dispose();
                }
                cle.Value.Clear();
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog { Filter = "JSON config files(*.json)|*.json" })
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if(Configuration.Load(new FileInfo(fileDialog.FileName)))
                    {
                        ReloadTabs();
                        Reactor.ReloadValuesFromConfig();
                        Reactor.UpdateStats();
                        MessageBox.Show("Loaded and applied!");
                        Close();
                    }
                }
                else
                    return;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fileDialog = new SaveFileDialog { Filter = "JSON config files(*.json)|*.json" })
            {
                if (!(fileDialog.ShowDialog() == DialogResult.OK))
                    return;
                if (ApplyAndReload())
                {
                    Configuration.Save(new FileInfo(fileDialog.FileName));
                    MessageBox.Show("Saved and applied!");
                    Close();
                }
            }
        }

        private bool ApplyAndReload()
        {
            try
            {
                ApplyConfiguration();
                ReloadTabs();
                Reactor.ReloadValuesFromConfig();
                Reactor.UpdateStats();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nThere were invalid values in the config, validation is NYI so i had to reset to defaults, sorry!");
                Configuration.ResetToDefaults();
                ReloadTabs();
                Reactor.ReloadValuesFromConfig();
                Reactor.UpdateStats();
                return false;
            }
        }

        private void ApplyConfiguration()
        {
            Configuration.Fission.Power = Convert.ToDouble(power.Text);
            Configuration.Fission.HeatGeneration = Convert.ToDouble(heatGeneration.Text);
            Configuration.Fission.FuelUse = Convert.ToDouble(fuelUse.Text);
            Configuration.Fission.MinSize = Convert.ToInt32(minSize.Text);
            Configuration.Fission.MaxSize = Convert.ToInt32(maxSize.Text);
            Configuration.Fission.ModeratorExtraHeat = Convert.ToDouble(moderatorExtraHeat.Text);
            Configuration.Fission.ModeratorExtraPower = Convert.ToDouble(moderatorExtraPower.Text);
            Configuration.Fission.NeutronReach = Convert.ToInt32(neutronReach.Text);

            foreach (KeyValuePair<string, List<Control>> kvp in cIFR)
            {
                CoolerValues cv = new CoolerValues(Convert.ToDouble(kvp.Value[1].Text), Convert.ToDouble(kvp.Value[2].Text), kvp.Value[3].Text);
                Configuration.Coolers[kvp.Key] = cv;
            }

            foreach (KeyValuePair<string, List<Control>> kvp in fIFR)
            {
                FuelValues fv = new FuelValues(Convert.ToDouble(kvp.Value[1].Text), Convert.ToDouble(kvp.Value[2].Text), Convert.ToDouble(kvp.Value[3].Text));
                Configuration.Fuels[kvp.Key] = fv;
            }
        }

        private void ApplyConfig_Click(object sender, EventArgs e)
        {
            if (ApplyAndReload())
            {
                MessageBox.Show("Applied!");
                Close();
            }
        }

        private void Import_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Nuclear craft config|nuclearcraft.cfg|Any config file|*.cfg|All Files|*.*" })
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var config = NuclearcraftConfigImport.ImportConfig(new FileInfo(fileDialog.FileName));
                    if (config == null || !config.HasBlock("fission"))
                    {
                        MessageBox.Show("Configuration could not be imported, check the file can be loaded by minecraft, or has come from the modpack you want to load");
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(config.LastError))
                    {
                        MessageBox.Show("We had the following errors while parsing the config file, please check that it's correct (not all values may import)");
                    }

                    Configuration.Fission.Power = config.Get<double>("fission", "fission_power");
                    Configuration.Fission.HeatGeneration = config.Get<double>("fission", "fission_heat_generation");
                    Configuration.Fission.FuelUse = config.Get<double>("fission", "fission_fuel_use");
                    Configuration.Fission.MinSize = config.Get<int>("fission", "fission_min_size");
                    Configuration.Fission.MaxSize = config.Get<int>("fission", "fission_max_size");
                    Configuration.Fission.ModeratorExtraHeat = config.Get<double>("fission", "fission_moderator_extra_heat");
                    Configuration.Fission.ModeratorExtraPower = config.Get<double>("fission", "fission_moderator_extra_power");
                    Configuration.Fission.NeutronReach = config.Get<int>("fission", "fission_neutron_reach");

                    SetFuelValues(config, new[] { "TBU", "TBU Oxide" }, "thorium");
                    SetFuelValues(config,
                        new[] { "LEU-233", "LEU-233 Oxide", "HEU-233", "HEU-233 Oxide", "LEU-235", "LEU-235 Oxide", "HEU-235", "HEU-235 Oxide" },
                        "uranium");
                    SetFuelValues(config, new[] { "LEN-236", "LEN-236 Oxide", "HEN-236", "HEN-236 Oxide" },
                        "neptunium");
                    SetFuelValues(config, new[] { "LEP-239", "LEP-239 Oxide", "HEP-239", "HEP-239 Oxide", "LEP-241", "LEP-241 Oxide", "HEP-241", "HEP-241 Oxide" },
                        "plutonium");
                    SetFuelValues(config, new[] { "MOX-239", "MOX-241" },
                        "mox");
                    SetFuelValues(config, new[] { "LEA-242", "LEA-242 Oxide", "HEA-242", "HEA-242 Oxide" },
                        "americium");
                    SetFuelValues(config, new[] { "LECm-243", "LECm-243 Oxide", "HECm-243", "HECm-243 Oxide", "LECm-245", "LECm-245 Oxide", "HECm-245", "HECm-245 Oxide", "LECm-247", "LECm-247 Oxide", "HECm-247", "HECm-247 Oxide" },
                        "curium");
                    SetFuelValues(config, new[] { "LEB-248", "LEB-248 Oxide", "HEB-248", "HEB-248 Oxide" },
                        "berkelium");
                    SetFuelValues(config, new[] { "LECf-249", "LECf-249 Oxide", "HECf-249", "HECf-249 Oxide", "LECf-251", "LECf-251 Oxide", "HECf-251", "HECf-251 Oxide" },
                        "californium");

                    SetCoolingRates(config);

                    ReloadTabs();
                    Reactor.ReloadValuesFromConfig();
                    Reactor.UpdateStats();
                    MessageBox.Show("Loaded and applied, please save as a json file");
                }
                else
                    return;
            }
        }

        private static void SetCoolingRates(NuclearcraftConfigImport config)
        {
            var items = new[] { "Water", "Redstone", "Quartz", "Gold", "Glowstone", "Lapis", "Diamond", "Helium", "Enderium", "Cryotheum", "Iron", "Emerald", "Copper", "Tin", "Magnesium" };
            for (int i = 0; i < items.Length; i++)
            {
                var item = Configuration.Coolers[items[i]];
                item.HeatPassive = config.GetItem<double>("fission", "fission_cooling_rate", i);
                item.HeatActive = config.GetItem<double>("fission", "fission_active_cooling_rate", i);
                Configuration.Coolers[items[i]] = item;
            }
        }

        private static void SetFuelValues(NuclearcraftConfigImport config, string[] items, string element)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = Configuration.Fuels[items[i]];
                item.FuelTime = config.GetItem<double>("fission", "fission_" + element + "_fuel_time", i);
                item.BasePower = config.GetItem<double>("fission", "fission_" + element + "_power", i);
                item.BaseHeat = config.GetItem<double>("fission", "fission_" + element + "_heat_generation", i);
                Configuration.Fuels[items[i]] = item;
            }
        }

        private void CheckDoubleValue(object sender, EventArgs args)
        {
            var control = sender as TextBox;
            if (control == null)
                return;

            var data = control.Text;
            if (string.IsNullOrWhiteSpace(data))
            {
                control.BackColor = Color.LightSalmon;
                this.ttValidation.SetToolTip(control, "Please enter a value");
                return;
            }

            if (double.TryParse(data, out double value))
            {
                control.BackColor = SystemColors.Window;
                control.Text = value.ToString();
                this.ttValidation.SetToolTip(control, null);
            }
            else
            {
                control.BackColor = Color.LightSalmon;
                this.ttValidation.SetToolTip(control, "The value entered is not a valid number");
            }
        }

        private void CheckIntValue(object sender, EventArgs args)
        {
            var control = sender as TextBox;
            if (control == null)
                return;

            var data = control.Text;
            if (string.IsNullOrWhiteSpace(data))
            {
                control.BackColor = Color.LightSalmon;
                this.ttValidation.SetToolTip(control, "Please enter a value");
                return;
            }

            if (int.TryParse(data, out int value))
            {
                control.BackColor = SystemColors.Window;
                control.Text = value.ToString();
                this.ttValidation.SetToolTip(control, null);
            }
            else
            {
                control.BackColor = Color.LightSalmon;
                this.ttValidation.SetToolTip(control, "The value entered is not a valid number");
            }
        }

        private void CalculateTotals_Click(object sender, EventArgs e)
        {
            string totals = "";
            foreach(KeyValuePair<string, int> resource in Configuration.CalculateTotalResourceCosts())
            {
                totals += String.Format("{0,-30}\t{1,-30}\r\n", resource.Key, resource.Value);
            }
            MessageBox.Show(totals);
        }
    }

    public static class ObjectExtensions
    {
        public static T Set<T>(this T item, Action<T> setter)
        {
            setter?.Invoke(item);
            return item;
        }
    }
}
