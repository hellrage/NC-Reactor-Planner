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
using System.Reflection;

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
        private Dictionary<string, Dictionary<string, List<Control>>> IFRs;
        private List<Control> fissionInputs;
        private static readonly FieldInfo[] configurationPages = typeof(Configuration).GetFields();

        public ConfigurationUI()
        {
            InitializeComponent();
            IFRs = new Dictionary<string, Dictionary<string, List<Control>>>();
            foreach (FieldInfo page in configurationPages)
                if (page.Name != "ResourceCosts")
                    if (page.Name != "Fission")
                        IFRs.Add(page.Name, new Dictionary<string, List<Control>>());
            fissionInputs = new List<Control>();
            ReloadTabs();
        }

        private void ConfigurationUI_Load(object sender, EventArgs e)
        {
        }

        private void ReloadTabs()
        {
            settingTabs.TabPages.Clear();
            foreach (FieldInfo page in configurationPages)
            {
                if (page.Name != "ResourceCosts")
                {
                    settingTabs.TabPages.Add(page.Name, page.Name);
                    settingTabs.TabPages[page.Name].AutoScroll = true;
                    if (page.Name != "Fission")
                        ReloadTab(page, IFRs[page.Name]);
                }
            }
            ReloadFissionTab();
            //ReloadResourceCostTab();
            //blockSelector.SelectedIndexChanged += new EventHandler(SelectedBlockChanged);
        }

        private void ReloadFissionTab()
        {
            settingTabs.TabPages["Fission"].Controls.Clear();
            FieldInfo[] fieldInfos = typeof(FissionValues).GetFields();
            fissionInputs.Clear();
            int row = 0;
            foreach (FieldInfo fi in fieldInfos)
            {
                settingTabs.TabPages["Fission"].Controls.Add(new Label { Text = fi.Name, Location = new Point(3, row++ * 20),Size = new Size(200, 14), Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold) });
                fissionInputs.Add(new TextBox { Text = fi.GetValue(Configuration.Fission).ToString(), Location = new Point(3, row++*20), Size = new Size(80, 14), CausesValidation = true, Tag = fi.FieldType }.Set(val => { val.Validating += ValidateValue; }));
            }
            foreach (Control c in fissionInputs)
                settingTabs.TabPages["Fission"].Controls.Add(c);
        }

        private void ReloadTab(FieldInfo tabInfo, Dictionary<string, List<Control>> IFR)
        {
            settingTabs.TabPages[tabInfo.Name].Controls.Clear();
            IFR.Clear();
            FieldInfo[] fieldsInfo;
            int row = 0;
            int x = 150;

            MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(tabInfo.FieldType);
            dynamic configurationPage = castMethod.Invoke(null, new object[] { tabInfo.GetValue(null) });
            fieldsInfo = configurationPage.GetType().GetGenericArguments()[1].GetFields();

            AddLabels();
            foreach (var entry in configurationPage)
            {
                dynamic v = entry.Value;
                FillInputRow(row++, entry.Key, v);
            }
            
            void AddLabels()
            {
                foreach (FieldInfo fi in fieldsInfo)
                {
                    settingTabs.TabPages[tabInfo.Name].Controls.Add(new Label { Text = fi.Name, Location = new Point(x, 3), Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold) });
                    int lastIndex = settingTabs.TabPages[tabInfo.Name].Controls.Count - 1;
                    x += settingTabs.TabPages[tabInfo.Name].Controls[lastIndex].Size.Width;
                }
            }


            void FillInputRow(int rowNum, string entry, object ob)
            {
                int y = 8 + row * 20;
                x = 150;
                List<Control> fields = new List<Control>();
                settingTabs.TabPages[tabInfo.Name].Controls.Add(new Label { Text = entry, Location = new Point(3, y), Size = new Size(x, 14) });
                int index = 0;
                foreach (FieldInfo property in fieldsInfo)
                {
                    fields.Add(new TextBox {Text = property.GetValue(ob).ToString(), Location = new Point(x, y), Size = new Size(property.Name=="Requirements"?300:80, 14), CausesValidation = true, Tag = property.FieldType }.Set(val => { val.Validating += ValidateValue; }));
                    x += settingTabs.TabPages[tabInfo.Name].Controls[index].Size.Width;
                }
                IFR.Add(entry, fields);
            }

            foreach (KeyValuePair<string, List<Control>> controlRow in IFR)
            {
                foreach (Control c in controlRow.Value)
                {
                    settingTabs.TabPages[tabInfo.Name].Controls.Add(c);
                }
            }
        }

        //private void ReloadResourceCostTab()
        //{
        //    blockSelector.Items.Add(new ResourceCostComboboxItem("Fuel cell", Configuration.ResourceCosts.FuelCellCosts));
        //    blockSelector.Items.Add(new ResourceCostComboboxItem("Casing", Configuration.ResourceCosts.CasingCosts));

        //    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in Configuration.ResourceCosts.HeatSinkCosts)
        //    {
        //        blockSelector.Items.Add(new ResourceCostComboboxItem(kvp.Key + " HeatSink", kvp.Value));
        //    }

        //    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in Configuration.ResourceCosts.ModeratorCosts)
        //    {
        //        blockSelector.Items.Add(new ResourceCostComboboxItem(kvp.Key + " Moderator", kvp.Value));
        //    }

        //}

        //private void SelectedBlockChanged(object sender, EventArgs e)
        //{
        //    if (rDC != null)
        //    {
        //        foreach (KeyValuePair<string, List<Control>> kvp in rDC)
        //            foreach (Control c in kvp.Value)
        //            { 
        //                c.Dispose();
        //                resourceCostsPage.Controls.Remove(c);
        //            }
        //    }
        //    rDC = new Dictionary<string, List<Control>>();

        //    int row = 1;
        //    foreach(KeyValuePair<string, int> resource in ((ResourceCostComboboxItem)blockSelector.SelectedItem).Resources)
        //    {
        //        rDC[resource.Key] = new List<Control>();
        //        rDC[resource.Key].Add(new TextBox { Text = resource.Key, Location = new Point(10, row * 30) });
        //        rDC[resource.Key].Add(new NumericUpDown { Value = resource.Value, Location = new Point(130, row * 30) });
        //        row++;
        //        //rDC.Add(new Button { Text = resource.Key, Location = new Point(10, row++ * 20) });
        //        resourceCostsPage.Controls.AddRange(rDC[resource.Key].ToArray());
        //    }

            
        //}

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
                        Reactor.Update();
                        Reactor.UI.fuelSelector_SelectedIndexChanged(null, null);
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
                Reactor.UI.fuelSelector_SelectedIndexChanged(null, null);
                Reactor.Update();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nCould not apply configuration, resetting to defaults!");
                Configuration.ResetToDefaults();
                ReloadTabs();
                Reactor.ReloadValuesFromConfig();
                Reactor.UI.fuelSelector_SelectedIndexChanged(null, null);
                Reactor.Update();
                return false;
            }
        }

        private void ApplyConfiguration()
        {
            ApplyFission();
            ApplyValues();
        }

        private void ApplyFission()
        {
            List<object> values = new List<object>();
            foreach (Control value in fissionInputs)
            {
                values.Add(value.Text);
            }
            Configuration.Fission = new FissionValues(values);
        }

        private void ApplyValues()
        {
            foreach (FieldInfo field in configurationPages)
            {
                MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(field.FieldType);

                if (field.Name == "ResourceCosts" | field.Name == "Fission")
                    continue;
                List<object> values;
                dynamic configurationPage = castMethod.Invoke(null, new object[] { field.GetValue(null) });
                foreach (KeyValuePair<string, List<Control>> valueRow in IFRs[field.Name])
                {
                    values = new List<object>();
                    foreach (Control c in valueRow.Value)
                        values.Add(c.Text);

                    dynamic val = Activator.CreateInstance(configurationPage[valueRow.Key].GetType(), new object[] { values });
                    configurationPage[valueRow.Key] = val;
                }
            }
        }

        public static T Cast<T>(object o)
        {
            return (T)o;
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
            //using (OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Nuclear craft config|nuclearcraft.cfg|Any config file|*.cfg|All Files|*.*" })
            //{
            //    if (fileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        var config = NuclearcraftConfigImport.ImportConfig(new FileInfo(fileDialog.FileName));
            //        if (config == null || !config.HasBlock("fission"))
            //        {
            //            MessageBox.Show("Configuration could not be imported, check the file can be loaded by minecraft, or has come from the modpack you want to load");
            //            return;
            //        }

            //        if (!string.IsNullOrWhiteSpace(config.LastError))
            //        {
            //            MessageBox.Show("We had the following errors while parsing the config file, please check that it's correct (not all values may import)");
            //        }

            //        Configuration.Fission.Power = config.Get<double>("fission", "fission_power");
            //        Configuration.Fission.HeatGeneration = config.Get<double>("fission", "fission_heat_generation");
            //        Configuration.Fission.FuelUse = config.Get<double>("fission", "fission_fuel_use");
            //        Configuration.Fission.MinSize = config.Get<int>("fission", "fission_min_size");
            //        Configuration.Fission.MaxSize = config.Get<int>("fission", "fission_max_size");
            //        Configuration.Fission.NeutronReach = config.Get<int>("fission", "fission_neutron_reach");

            //        SetFuelValues(config, new[] { "TBU", "TBU Oxide" }, "thorium");
            //        SetFuelValues(config,
            //            new[] { "LEU-233", "LEU-233 Oxide", "HEU-233", "HEU-233 Oxide", "LEU-235", "LEU-235 Oxide", "HEU-235", "HEU-235 Oxide" },
            //            "uranium");
            //        SetFuelValues(config, new[] { "LEN-236", "LEN-236 Oxide", "HEN-236", "HEN-236 Oxide" },
            //            "neptunium");
            //        SetFuelValues(config, new[] { "LEP-239", "LEP-239 Oxide", "HEP-239", "HEP-239 Oxide", "LEP-241", "LEP-241 Oxide", "HEP-241", "HEP-241 Oxide" },
            //            "plutonium");
            //        SetFuelValues(config, new[] { "MOX-239", "MOX-241" },
            //            "mox");
            //        SetFuelValues(config, new[] { "LEA-242", "LEA-242 Oxide", "HEA-242", "HEA-242 Oxide" },
            //            "americium");
            //        SetFuelValues(config, new[] { "LECm-243", "LECm-243 Oxide", "HECm-243", "HECm-243 Oxide", "LECm-245", "LECm-245 Oxide", "HECm-245", "HECm-245 Oxide", "LECm-247", "LECm-247 Oxide", "HECm-247", "HECm-247 Oxide" },
            //            "curium");
            //        SetFuelValues(config, new[] { "LEB-248", "LEB-248 Oxide", "HEB-248", "HEB-248 Oxide" },
            //            "berkelium");
            //        SetFuelValues(config, new[] { "LECf-249", "LECf-249 Oxide", "HECf-249", "HECf-249 Oxide", "LECf-251", "LECf-251 Oxide", "HECf-251", "HECf-251 Oxide" },
            //            "californium");

            //        SetCoolingRates(config);

            //        ReloadTabs();
            //        Reactor.ReloadValuesFromConfig();
            //        //Reactor.Update();
            //        MessageBox.Show("Loaded and applied, please save as a json file");
            //    }
            //    else
            //        return;
            //}
        }

        private static void SetCoolingRates(NuclearcraftConfigImport config)
        {
            var items = new[] { "Water", "Redstone", "Quartz", "Gold", "Glowstone", "Lapis", "Diamond", "Helium", "Enderium", "Cryotheum", "Iron", "Emerald", "Copper", "Tin", "Magnesium" };
            for (int i = 0; i < items.Length; i++)
            {
                var item = Configuration.HeatSinks[items[i]];
                item.HeatPassive = config.GetItem<double>("fission", "fission_cooling_rate", i);
                //item.HeatActive = config.GetItem<double>("fission", "fission_active_cooling_rate", i);
                Configuration.HeatSinks[items[i]] = item;
            }
        }

        private static void SetFuelValues(NuclearcraftConfigImport config, string[] items, string element)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = Configuration.Fuels[items[i]];
                item.FuelTime = config.GetItem<double>("fission", "fission_" + element + "_fuel_time", i);
                //item.BasePower = config.GetItem<double>("fission", "fission_" + element + "_power", i);
                item.BaseHeat = config.GetItem<double>("fission", "fission_" + element + "_heat_generation", i);
                Configuration.Fuels[items[i]] = item;
            }
        }

        private void ValidateValue(object sender, EventArgs args)
        {
            switch (((Type)((TextBox)sender).Tag).Name)
            {
                case "Double":
                    CheckDoubleValue(sender, args);
                    break;
                case "Int":
                    CheckIntValue(sender, args);
                    break;
                default:
                    break;
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
