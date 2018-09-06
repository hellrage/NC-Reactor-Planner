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
    public partial class ConfigurationUI : Form
    {
        private Dictionary<string, List<Control>> cIFR; //cooler input field rows
        private Dictionary<string, List<Control>> fIFR; //fuel input field rows

        public ConfigurationUI()
        {
            InitializeComponent();
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
                fields.Add(new Label { Text = coolerEntry.Key, Location = new Point(3, y), Size = new Size(70, 13) });
                fields.Add(new TextBox { Text = cv.HeatPassive.ToString(), Location = new Point(85, y), Size = new Size(70, 14) });
                fields.Add(new TextBox { Text = cv.HeatActive.ToString(), Location = new Point(165, y), Size = new Size(110, 14) });
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
                fields.Add(new TextBox { Text = fv.BasePower.ToString(), Location = new Point(160, y), Size = new Size(75, 14) });
                fields.Add(new TextBox { Text = fv.BaseHeat.ToString(), Location = new Point(240, y), Size = new Size(70, 14) });
                fields.Add(new TextBox { Text = fv.FuelTime.ToString(), Location = new Point(312, y), Size = new Size(70, 14) });
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
                Reactor.ReloadValuesFromConfig();
                ReloadTabs();
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

        //[TODO] Add validation for input fields
    }
}
