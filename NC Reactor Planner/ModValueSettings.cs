using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NC_Reactor_Planner
{
    public partial class ModValueSettings : Form
    {
        private Dictionary<string, List<Control>> cIFR; //cooler input field rows
        private Dictionary<string, List<Control>> fIFR; //fuel input field rows

        public ModValueSettings()
        {
            InitializeComponent();
            PopulateCoolersTab();
            PopulateFuelsTab();
            ReloadFromCurrentValues();
        }

        private void ClosingForm(FormClosingEventArgs e)
        {
            DialogResult save = MessageBox.Show("Closing form, save your settings?", "Save settings?", MessageBoxButtons.YesNoCancel);
            if (save == DialogResult.Yes)
                SaveAllSettings();
            else if (save == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            SaveAllSettings();
        }

        private void SaveAllSettings()
        {
            WriteCoolerSettings();
            WriteFuelSettings();
            //General setting are attached to application settings at design-time (generalPage in settingTabs)
            Properties.Settings.Default.Save();
        }

        private void PopulateCoolersTab()
        {
            cIFR = new Dictionary<string, List<Control>>(); //cooler input field rows
            int row = 0;
            foreach (Cooler cooler in Palette.coolers)
            {
                row++;
                int y = 3 + row * 20;
                List<Control> fields = new List<Control>();
                fields.Add(new Label { Text = cooler.DisplayName, Location = new Point(3, y), Size = new Size(70, 13)});
                fields.Add(new TextBox { Location = new Point(80, y), Size = new Size(100,14)});
                fields.Add(new TextBox { Location = new Point(200, y)});
                cIFR.Add(cooler.DisplayName, fields);
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

        private void WriteCoolerSettings()
        {
            for (int i = 0; i < cIFR.Count; i++)
            {
                string setting = "";
                List<Control> fields = cIFR.ElementAt(i).Value;
                for (int f = 1; f < fields.Count; f++)
                {
                    setting += fields[f].Text;
                    if (f != fields.Count) setting += ";";
                }
                Properties.Settings.Default[cIFR.ElementAt(i).Key] = setting;
            }
        }

        private void PopulateFuelsTab()
        {
            fIFR = new Dictionary<string, List<Control>>(); //cooler input field rows
            int row = 0;
            foreach (Fuel fuel in Reactor.fuels)
            {
                row++;
                int y = 3 + row * 20;
                List<Control> fields = new List<Control>();
                fields.Add(new Label { Text = fuel.Name, Location = new Point(3, y), Size = new Size(150, 13) });
                fields.Add(new TextBox { Location = new Point(160, y), Size = new Size(75, 14) });
                fields.Add(new TextBox { Location = new Point(240, y), Size = new Size(70, 14) });
                fields.Add(new TextBox { Location = new Point(312, y), Size = new Size(70, 14) });
                fIFR.Add(fuel.saveSafeName, fields);

            }

            foreach (KeyValuePair<string, List<Control>> slkvp in fIFR)
            {
                foreach (Control c in slkvp.Value)
                {
                    settingTabs.TabPages["fuelsPage"].Controls.Add(c);
                }
            }
        }

        private void WriteFuelSettings()
        {
            for (int i = 0; i < fIFR.Count; i++)
            {
                string setting = "";
                List<Control> fields = fIFR.ElementAt(i).Value;
                for (int f = 1; f < fields.Count; f++)
                {
                    setting += fields[f].Text;
                    if (f < fields.Count - 1) setting += ";";
                }
                Properties.Settings.Default[fIFR.ElementAt(i).Key] = setting;
            }
        }

        private void ReloadFromCurrentValues()
        {
            foreach (Fuel fuel in Reactor.fuels)
            {
                fIFR[fuel.saveSafeName][1].Text = fuel.BasePower.ToString(); ;
                fIFR[fuel.saveSafeName][2].Text = fuel.BaseHeat.ToString(); ;
                fIFR[fuel.saveSafeName][3].Text = fuel.FuelTime.ToString();
            }

            foreach (KeyValuePair<string, List<Control>> kvp in cIFR)
            {
                kvp.Value[1].Text = Palette.GetCooler(kvp.Key).HeatPassive.ToString();
                kvp.Value[2].Text = Palette.GetCooler(kvp.Key).HeatActive.ToString();
            }
        }

        public static List<string> RetrieveSplitSettings(string settingName)
        {
            return ((string)Properties.Settings.Default[settingName]).Split(';').ToList();
        }

        private void ModValueSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosingForm(e);
        }
    }
}
