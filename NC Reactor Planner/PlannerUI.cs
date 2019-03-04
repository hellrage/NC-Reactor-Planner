using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using fNbt;

namespace NC_Reactor_Planner
{
    public partial class PlannerUI : Form
    {
        public Panel ReactorGrid { get => reactorGrid; }
        public decimal DrawingScale { get => imageScale.Value; }
        public Point PalettePanelLocation { get => new Point(resetLayout.Location.X - Palette.PalettePanel.spacing, resetLayout.Location.Y + resetLayout.Size.Height); }
        public FileInfo LoadedSaveFile { get; set; }


        public static readonly Pen ErrorPen = new Pen(Brushes.Red, 3);

        public static ToolTip uiToolTip = new ToolTip
            {
                AutoPopDelay = 2000,
                InitialDelay = 0,
                ReshowDelay = 0,
            };
        public static ToolTip gridToolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 1200,
                ReshowDelay = 1000,
            };
        public static int blockSize;
        private static ConfigurationUI configurationUI;
        public static bool drawAllLayers;
        string appName;
        public static Block[,] layerBuffer;

        private decimal defaultReactorX;
        private decimal defaultReactorY;
        private decimal defaultReactorZ;

        public PlannerUI()
        {
            InitializeComponent();
            Version aVersion = Assembly.GetExecutingAssembly().GetName().Version;
            appName = string.Format("NC Reactor Planner v{0}.{1}.{2} ", aVersion.Major, aVersion.Minor, aVersion.Build);
            this.Text = appName;
            
            SetUIToolTips();
            resetLayout.MouseLeave += new EventHandler(ResetButtonFocusLost);
            resetLayout.LostFocus += new EventHandler(ResetButtonFocusLost);

            blockSize = (int)(Palette.Textures.First().Value.Size.Height * imageScale.Value);

            drawAllLayers = true;
            defaultReactorX = 9;
            defaultReactorY = 5;
            defaultReactorZ = 9;
            SetupReactorSizeControls(defaultReactorX, defaultReactorY, defaultReactorZ);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if !DEBUG
            SetUpdateAvailableTextAsync();
#endif
            fuelSelector.Items.AddRange(Palette.FuelPalette.Values.ToArray());
            statsLabel.Location = new Point(statsLabel.Location.X, PalettePanelLocation.Y + Palette.PaletteControl.Size.Height);
            stats.Location = new Point(stats.Location.X, PalettePanelLocation.Y + Palette.PaletteControl.Size.Height + statsLabel.Size.Height);
            stats.Size = new Size(stats.Size.Width, this.ClientSize.Height - stats.Location.Y - 5);

            ResetLayout(LoadedSaveFile != null);
        }

        private void SetUIToolTips()
        {
            uiToolTip.SetToolTip(imageScale, "Scale of blocks' textures. Also affects saved PNG scale.");
            uiToolTip.SetToolTip(reactorHeight, "Reactor hight (number of internal layers)");
            uiToolTip.SetToolTip(reactorLength, "Reactor length (Z axis internal size)");
            uiToolTip.SetToolTip(reactorWidth, "Reactor width (X axis internal size)");
            uiToolTip.SetToolTip(layerScrollBar, "Scrolls through reactor layers. Scrollwheel works, so do arrow keys");
            uiToolTip.SetToolTip(viewStyleSwitch, "Toggles between drawing layers one-by-one or all at once.");
            uiToolTip.SetToolTip(saveAsImage, "Saves an image of the reactor. Stats are also added to the output so you have a full description in one picture ^-^");
            uiToolTip.SetToolTip(resetLayout, "Create a new reactor with the specified dimensions. Click again to confirm (overwrites your current layout! Save if you want to keep it.)");
            uiToolTip.SetToolTip(generateBGString, "Generates a string for use with Building Gadgets' copy-paste tool");
            uiToolTip.SetToolTip(generateSchematic, "Generates a .schematic file for use with Schematica or any other appropriate tool");
        }

        private void SetupReactorSizeControls(decimal X, decimal Y, decimal Z)
        {
            int minSize = Configuration.Fission.MinSize;
            int maxSize = Configuration.Fission.MaxSize;

            reactorHeight.Maximum = maxSize;
            reactorHeight.Minimum = minSize;
            if (Y < minSize)
                reactorHeight.Value = minSize;
            else if(Y > maxSize)
                reactorHeight.Value = maxSize;
            else
                reactorHeight.Value = Y;

            reactorWidth.Maximum = maxSize;
            reactorWidth.Minimum = minSize;
            if (X < minSize)
                reactorWidth.Value = minSize;
            else if (X > maxSize)
                reactorWidth.Value = maxSize;
            else
                reactorWidth.Value = X;

            reactorLength.Maximum = maxSize;
            reactorLength.Minimum = minSize;
            if (Z < minSize)
                reactorLength.Value = minSize;
            else if (Z > maxSize)
                reactorLength.Value = maxSize;
            else
                reactorLength.Value = Z;
        }

        private void ResetButtonFocusLost(object sender, EventArgs e)
        {
            resetLayout.Text = "Reset layout";
        }

        private void resetLayout_Click(object sender, EventArgs e)
        {
            if (resetLayout.Text == "Confirm reset?")
            {

                resetLayout.Text = "RESETTING...";
                LoadedSaveFile = null;
                ResetLayout(false);
                resetLayout.Text = "Reset layout";
            }
            else
                resetLayout.Text = "Confirm reset?";
        }

        public void ResetLayout(bool loading)
        {
            EnableUIElements();

            gridToolTip.RemoveAll();
            
            reactorGrid.Controls.Clear();

            if (LoadedSaveFile == null && !loading)
            {
                Reactor.InitializeReactor((int)reactorWidth.Value, (int)reactorHeight.Value, (int)reactorLength.Value);
            }
            else
            {
                reactorWidth.Value = (int)Reactor.interiorDims.X;
                reactorHeight.Value = (int)Reactor.interiorDims.Y;
                reactorLength.Value = (int)Reactor.interiorDims.Z;
                Reactor.ConstructLayers();
            }

            UpdateWindowTitle();
            fuelSelector.SelectedItem = Reactor.usedFuel;
            Reactor.UpdateStats();

            if (drawAllLayers)
            {
                Reactor.Redraw();
                foreach (ReactorGridLayer layer in Reactor.layers)
                    UpdateLocation(layer);
                reactorGrid.Controls.AddRange(Reactor.layers.ToArray());
            }
            else
            {
                ReactorGridLayer layer = Reactor.layers[layerScrollBar.Value - 1];
                UpdateLocation(layer);
                reactorGrid.Controls.Add(layer);
                layerScrollBar.Maximum = (int)Reactor.interiorDims.Y;
            }
            

        }

        private void EnableUIElements()
        {
            if (!drawAllLayers)
                layerScrollBar.Enabled = true;

            layerScrollBar.Value = 1;
            layerLabel.Text = "Layer 1";
            saveReactor.Enabled = true;
            viewStyleSwitch.Enabled = true;
            saveAsImage.Enabled = true;
            imageScale.Enabled = true;
            fuelSelector.Enabled = true;
            fuelBasePower.Enabled = true;
            fuelBaseHeat.Enabled = true;
            openConfig.Enabled = true;
        }

        private void ClearDisposeLayers()
        {
            if (reactorGrid.Controls.Count > 0)
            {
                reactorGrid.Controls.Clear();
                foreach (Control c in Reactor.layers)
                    c.Dispose();
            }
        }

        private void SwitchToPerLayer()
        {
            drawAllLayers = false;
            ResetLayout(true);
            viewStyleSwitch.Text = "Per layer";
            layerScrollBar.Enabled = true;
            layerLabel.Show();
        }

        private void Redraw()
        {
            if (drawAllLayers)
                Reactor.Redraw();
            else
            {
                ReactorGridLayer layer;

                reactorGrid.Controls.Clear();
                layer = Reactor.layers[layerScrollBar.Value - 1];

                UpdateLocation(layer);
                reactorGrid.Controls.Add(layer);
                layer.Refresh();
            }
        }

        private void layerScrollBar_ValueChanged(object sender, EventArgs e)
        {
            layerLabel.Text = "Layer " + layerScrollBar.Value;

            Redraw();

            gridToolTip.Active = false;
            gridToolTip.Active = true;
        }

        private void reactorGrid_MouseEnter(object sender, EventArgs e)
        {
            if (configurationUI != null && !configurationUI.IsDisposed)
                return;
            if (drawAllLayers)
                reactorGrid.Focus();
            else
                layerScrollBar.Focus();
        }

        private void saveReactor_Click(object sender, EventArgs e)
        {
            SaveReactor();
        }

        private void SaveReactor()
        {
            using (SaveFileDialog fileDialog = new SaveFileDialog { Filter = "JSON files(*.json)|*.json" })
            {
                fileDialog.FileName = ConstructSaveFileName();
                fileDialog.AddExtension = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadedSaveFile = new FileInfo(fileDialog.FileName);
                    Reactor.Save(LoadedSaveFile);
                }
            }
        }

        private string ConstructSaveFileName()
        {
            return (LoadedSaveFile == null)
                                      ? string.Format("{0} {1} x {2} x {3}", ((Fuel)fuelSelector.SelectedItem).Name, Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z)
                                      : LoadedSaveFile.Name;
        }

        private void loadReactor_Click(object sender, EventArgs e)
        {
            LoadReactor();
        }

        private void LoadReactor()
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog { Filter = "NuclearCraft Reactor files(*.ncr)|*.ncr|JSON files(*.json)|*.json" })
            {
                fileDialog.FilterIndex = 2;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadedSaveFile = new FileInfo(fileDialog.FileName);
                    Reactor.Load(LoadedSaveFile);
                }
                else
                    return;
            }

            reactorHeight.Value = (decimal)Reactor.interiorDims.Y;
            reactorLength.Value = (decimal)Reactor.interiorDims.Z;
            reactorWidth.Value = (decimal)Reactor.interiorDims.X;

            UpdateSelectedFuel();

            ResetLayout(true);
        }

        private void UpdateSelectedFuel()
        {
            fuelBasePower.Text = Reactor.usedFuel.BasePower.ToString();
            fuelBaseHeat.Text = Reactor.usedFuel.BaseHeat.ToString();
            fuelSelector.Text = Reactor.usedFuel.Name;
        }

        private void refreshStats_Click(object sender, EventArgs e)
        {
            RefreshStats();
        }

        public void RefreshStats(bool includeClustersInStats = true)
        {
            stats.Text = Reactor.GetStatString();
        }

        private void viewStyleSwitch_Click(object sender, EventArgs e)
        {
            if (drawAllLayers)
                SwitchToPerLayer();
            else
                SwitchToDrawAllLayers();
        }

        private void SwitchToDrawAllLayers()
        {
            drawAllLayers = true;

            viewStyleSwitch.Text = "All layers";

            ResetLayout(true);

            layerScrollBar.Enabled = false;
            layerLabel.Hide();
        }

        private void saveAsImage_Click(object sender, EventArgs e)
        {
            string fileName = null;
            using (SaveFileDialog fileDialog = new SaveFileDialog { Filter = "Image files (*.png)|*.png" })
            {
                string autoFileName = "";
                if (LoadedSaveFile == null)
                {
                    if (drawAllLayers)
                        autoFileName = string.Format("{0} {1} x {2} x {3}", ((Fuel)fuelSelector.SelectedItem).Name, Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z);
                    else
                        autoFileName = string.Format("{0} {1} x {2} x {3} layer {4}", ((Fuel)fuelSelector.SelectedItem).Name, Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z, layerScrollBar.Value);
                }
                else
                {
                    if (drawAllLayers)
                        autoFileName = LoadedSaveFile.Name.Replace(".json", "");
                    else
                        autoFileName = (LoadedSaveFile.Name + " layer " + layerScrollBar.Value).Replace(".json", "");
                }
                fileDialog.FileName = autoFileName;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                    fileName = fileDialog.FileName;
            }
            if (fileName != null)
            {
                if (drawAllLayers)
                    Reactor.SaveReactorAsImage(fileName, stats.Lines.Length);
                else
                    Reactor.SaveLayerAsImage(layerScrollBar.Value, fileName);
            }
        }

        private void imageScale_ValueChanged(object sender, EventArgs e)
        {
            blockSize = (int)(Palette.Textures.First().Value.Size.Height * imageScale.Value);
            
            foreach (ReactorGridLayer layer in Reactor.layers)
            {
                layer.Rescale();
                UpdateLocation(layer);
            }
                
        }

        private void UpdateLocation(ReactorGridLayer layer)
        {
            Point origin;
            if (drawAllLayers)
            {
                int layersPerRow = (int)Math.Ceiling(Math.Sqrt(Reactor.interiorDims.Y));
                origin = new Point((layer.Y - 1) % layersPerRow * layer.Size.Width + (layer.Y - 1) % layersPerRow * 16,
                                    (layer.Y - 1) / layersPerRow * layer.Size.Height + (layer.Y - 1) / layersPerRow * 16);
            }
            else
            {
                origin = new Point(Math.Max(0, (int)(reactorGrid.Size.Width / 2 - Reactor.interiorDims.X * blockSize / 2)),
                                            Math.Max(0, (int)(reactorGrid.Size.Height / 2 - Reactor.interiorDims.Z * blockSize / 2)));
            }
            layer.Location = origin;
        }

        public void fuelSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fuel selectedFuel = (Fuel)fuelSelector.SelectedItem;
            if(selectedFuel == null)
            {
                fuelSelector.Text = "";
                return;
            }
            fuelBasePower.Text = selectedFuel.BasePower.ToString();
            fuelBaseHeat.Text = selectedFuel.BaseHeat.ToString();
            Reactor.usedFuel = selectedFuel; //[TODO]Change to a method you criminal

            Reactor.UpdateStats();
            Reactor.Redraw();
            RefreshStats();
            Palette.SelectedFuel = selectedFuel; //[TODO]Change to a method you criminal
        }

        private void reactorWidth_Enter(object sender, EventArgs e)
        {
            reactorWidth.Select(0, reactorWidth.Value.ToString().Length);
        }

        private void reactorHeight_Enter(object sender, EventArgs e)
        {
            reactorHeight.Select(0, reactorHeight.Value.ToString().Length);
        }

        private void reactorLength_Enter(object sender, EventArgs e)
        {
            reactorLength.Select(0, reactorLength.Value.ToString().Length);
        }

        private void OpenConfiguration(object sender, EventArgs e)
        {
            if (configurationUI == null || configurationUI.IsDisposed)
            {
                configurationUI = new ConfigurationUI();
                configurationUI.FormClosed += new FormClosedEventHandler(ConfigurationClosed);
            }
                configurationUI.ShowDialog(this);
        }

        private void ConfigurationClosed(object sender, FormClosedEventArgs e)
        {
            fuelSelector.Items.Clear();
            fuelSelector.Items.AddRange(Palette.FuelPalette.Values.ToArray());
            SetupReactorSizeControls(reactorWidth.Value, reactorHeight.Value, reactorLength.Value);
            Reactor.Redraw();
            RefreshStats();
        }

        private void UpdateWindowTitle()
        {
            if (LoadedSaveFile != null)
                this.Text = appName + "   " + LoadedSaveFile.FullName;
            else
                this.Text = appName;
        }

        private void PlannerUI_Leave(object sender, EventArgs e)
        {
            gridToolTip.Hide(reactorGrid);
        }

        private async void checkForUpdates_Click(object sender, EventArgs e)
        {
            Tuple<bool, Version, string> updateInfo = await Updater.CheckForUpdateAsync();
            if (updateInfo.Item1)
            {
                DialogResult updatePropmpt = MessageBox.Show("Download " + Updater.ShortVersionString(updateInfo.Item2) + "? Last commit message:\r\n\r\n" + updateInfo.Item3, "Update available!", MessageBoxButtons.YesNo);
                if (updatePropmpt == DialogResult.Yes)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.FileName = Updater.ExecutableName(updateInfo.Item2);
                    DialogResult saveResult = saveDialog.ShowDialog();
                    if (saveResult == DialogResult.OK)
                        Updater.PerformFullUpdate(updateInfo.Item2, saveDialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("You are using the latest version: " + Updater.ShortVersionString(Reactor.saveVersion), "No updates");
            }
        }

        private async void SetUpdateAvailableTextAsync()
        {
            Tuple<bool, Version,string> updateInfo = await Updater.CheckForUpdateAsync();
            if (updateInfo.Item1)
            {
                checkForUpdates.Font = new Font(checkForUpdates.Font, FontStyle.Bold);
                checkForUpdates.Text = Updater.ShortVersionString(updateInfo.Item2) + " Available!";
            }
        }

        private void generateBGString_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(BGExportHelper.FormBGExportString());
            MessageBox.Show("Copied export string to clipboard!");
        }

        private void generateSchematic_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog { Filter = "Schematic files | *.schematic"};
            saveDialog.FileName = "New schematic";
            DialogResult saveResult = saveDialog.ShowDialog();
            if (saveResult == DialogResult.OK)
            {
                var myFile = new NbtFile(SchematicaExportHelper.ExportReactor());
                myFile.SaveToFile(saveDialog.FileName, NbtCompression.GZip);
            }
            
        }
    }
}
