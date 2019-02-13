using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace NC_Reactor_Planner
{
    public partial class PlannerUI : Form
    {

        ToolTip paletteToolTip;
        public static ToolTip gridToolTip;
        public static int blockSize;
        public static int paletteBlockSize = 40;
        private static int totalBlocks;
        private static ConfigurationUI configurationUI;
        Graphics borderGraphics;
        Pen passiveHighlightPen;
        Pen activeHighlightPen;
        public static bool drawAllLayers = false;
        string appName;
        string loadedSaveFileName = null;
        FileInfo loadedSaveFileInfo = null;
        public static Block[,] layerBuffer;

        public PlannerUI()
        {
            InitializeComponent();

            Version aVersion = Assembly.GetExecutingAssembly().GetName().Version;
            appName = string.Format("NC Reactor Planner v{0}.{1}.{2} ", aVersion.Major, aVersion.Minor, aVersion.Build);
            this.Text = appName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            blockSize = (int)(Palette.textures.First().Value.Size.Height * imageScale.Value);

            borderGraphics = paletteTable.CreateGraphics();
            passiveHighlightPen = new Pen(Color.Blue, 4);
            activeHighlightPen = new Pen(Color.Green, 4);

            resetLayout.MouseLeave += new EventHandler(ResetButtonFocusLost);
            resetLayout.LostFocus += new EventHandler(ResetButtonFocusLost);

            SetUpToolTips();

            SetUpPalette();

            this.MouseMove += new MouseEventHandler(PaletteBlockLostFocus);

            fuelSelector.Items.AddRange(Reactor.fuels.ToArray());

            SetUIToolTips();

            NewResetLayout(false); // [TODO] I don't even know anymore okay?
            NewResetLayout(false); // This makes tooltips work right away but it's still inconsistent
            //wasted hours on this
        }

        private void SetUpToolTips()
        {
            paletteToolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 0,
                ReshowDelay = 0,
            };

            gridToolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 1200,
                ReshowDelay = 1000,
            };
        }

        private void SetUIToolTips()
        {
            paletteToolTip.SetToolTip(imageScale, "Scale of blocks' textures. Also affects saved PNG scale.");
            paletteToolTip.SetToolTip(reactorHeight, "Reactor hight (number of internal layers)");
            paletteToolTip.SetToolTip(reactorLength, "Reactor length (Z axis internal size)");
            paletteToolTip.SetToolTip(reactorWidth, "Reactor width (X axis internal size)");
            paletteToolTip.SetToolTip(layerScrollBar, "Scrolls through reactor layers. Scrollwheel works, so do arrow keys");
            paletteToolTip.SetToolTip(viewStyleSwitch, "Toggles between drawing layers one-by-one or all at once. Laggy and crash-prone at extreme reactor sizes in \"All layers\" mode (you'll get a warning)");
            paletteToolTip.SetToolTip(saveAsImage, "Saves an image of the reactor. Stats are also added to the output so you have a full description in one picture ^-^");
            paletteToolTip.SetToolTip(resetLayout, "Create a new reactor with the specified dimensions. Doubleclick to confirm (overwrites your current layout! Save if you want to keep it.)");
        }

        private void SetUpPalette()
        {
            paletteLabel.Text = "Palette";
            paletteTable.Controls.Clear();

            foreach (KeyValuePair<Block, BlockTypes> kvp in Palette.blocks)
            {
                paletteTable.Controls.Add(new ReactorGridCell { block = kvp.Key, Image = kvp.Key.Texture, SizeMode = PictureBoxSizeMode.Zoom });
            }

            foreach (ReactorGridCell paletteBlock in paletteTable.Controls)
            {
                paletteBlock.Click += new EventHandler(PaletteBlockClicked);
                paletteBlock.MouseEnter += new EventHandler(PaletteBlockHighlighted);
            }

            UpdatePaletteTooltips();

            Palette.selectedBlock = (ReactorGridCell)paletteTable.Controls[0];
            Palette.selectedType = Palette.selectedBlock.block.BlockType;

            paletteTable.MouseLeave += new EventHandler(PaletteBlockLostFocus);
        }

        private void UpdatePaletteTooltips()
        {
            foreach (ReactorGridCell paletteBlock in paletteTable.Controls)
            {
                paletteToolTip.SetToolTip(paletteBlock, paletteBlock.block.GetToolTip());
            }
        }

        private void ResetButtonFocusLost(object sender, EventArgs e)
        {
            resetLayout.Text = "Reset layout";
        }

        private void PaletteBlockLostFocus(object sender, EventArgs e)
        {
            borderGraphics.Clear(SystemColors.Control);

            ReactorGridCell selected = Palette.selectedBlock;
            paletteLabel.Text = selected.block.DisplayName;
            borderGraphics.DrawRectangle(passiveHighlightPen, selected.Location.X - 3, selected.Location.Y - 3, paletteBlockSize, paletteBlockSize);

        }

        private void PaletteBlockHighlighted(object sender, EventArgs e)
        {
            borderGraphics.Clear(SystemColors.Control);

            ReactorGridCell paletteBox = (ReactorGridCell)sender;
            paletteLabel.Text = paletteBox.block.DisplayName;

            borderGraphics.DrawRectangle(passiveHighlightPen, paletteBox.Location.X - 3, paletteBox.Location.Y - 3, paletteBlockSize, paletteBlockSize);
                
        }

        private void PaletteBlockClicked(object sender, EventArgs e)
        {
            borderGraphics.Clear(SystemColors.Control);

            ReactorGridCell paletteBox = (ReactorGridCell)sender;

            Palette.selectedBlock = paletteBox;
            Palette.selectedType = (Palette.blocks[paletteBox.block]);
            borderGraphics.DrawRectangle(passiveHighlightPen, paletteBox.Location.X - 3, paletteBox.Location.Y - 3, paletteBlockSize, paletteBlockSize);         
        }

        private void resetLayout_Click(object sender, EventArgs e)
        {
            if (resetLayout.Text == "Confirm reset?")
            {
                if (!IsCrashRobust())
                {
                    resetLayout.Text = "Reset layout";
                    return;
                }
                resetLayout.Text = "RESETTING...";
                NewResetLayout(false);
                resetLayout.Text = "Reset layout";
            }
            else
                resetLayout.Text = "Confirm reset?";
        }

        public void NewResetLayout(bool loading)
        {
            totalBlocks = (int)(reactorLength.Value * reactorWidth.Value * reactorHeight.Value);

            if (drawAllLayers && !IsCrashRobust())
                SwitchToPerLayer();

            EnableUIElements();

            gridToolTip.RemoveAll();

            //ClearDisposeLayers(); //Layers are handlel by the reactor
            reactorGrid.Controls.Clear();

            if (!loading)
            {
                loadedSaveFileInfo = null;
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
            fuelSelector.SelectedItem = fuelSelector.Items[0];
            //Reactor.Update();

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
                layer.Redraw();
                reactorGrid.Controls.Add(layer);
                layerScrollBar.Maximum = (int)Reactor.interiorDims.Y;
            }

            RefreshStats();

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
            fuelBaseEfficiency.Enabled = true;
            fuelBaseHeat.Enabled = true;
            OpenConfig.Enabled = true;
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
            NewResetLayout(true);
            viewStyleSwitch.Text = "Per layer";
            layerScrollBar.Enabled = true;
            layerLabel.Show();
        }

        private void NewRedraw()
        {
            if (drawAllLayers)
                Reactor.Redraw();
            else
            {
                ReactorGridLayer layer;
                if (totalBlocks > 9500)
                {
                    ClearDisposeLayers();
                    Reactor.ConstructLayer(layerScrollBar.Value);
                    layer = Reactor.layers.First();
                }
                else
                {
                    reactorGrid.Controls.Clear();
                    layer = Reactor.layers[layerScrollBar.Value - 1];
                }

                UpdateLocation(layer);
                reactorGrid.Controls.Add(layer);
                layer.Redraw();
            }
        }

        private void layerScrollBar_ValueChanged(object sender, EventArgs e)
        {
            layerLabel.Text = "Layer " + layerScrollBar.Value;

            NewRedraw();

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
                    loadedSaveFileInfo = new FileInfo(fileDialog.FileName);
                    Reactor.Save(loadedSaveFileInfo);
                }
            }
        }

        private string ConstructSaveFileName()
        {
            return (loadedSaveFileInfo == null)
                                      ? string.Format("{0} {1} x {2} x {3}", (fuelSelector.SelectedItem == null) ? "Custom" : fuelSelector.SelectedItem.ToString(), Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z)
                                      : loadedSaveFileInfo.Name;
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
                    loadedSaveFileInfo = new FileInfo(fileDialog.FileName);
                    Reactor.Load(loadedSaveFileInfo);
                }
                else
                    return;
            }

            reactorHeight.Value = (decimal)Reactor.interiorDims.Y;
            reactorLength.Value = (decimal)Reactor.interiorDims.Z;
            reactorWidth.Value = (decimal)Reactor.interiorDims.X;

            NewResetLayout(true);
        }

        private void refreshStats_Click(object sender, EventArgs e)
        {
            RefreshStats();
        }

        public void RefreshStats()
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
            if (!IsCrashRobust())
            {
                drawAllLayers = false;
                return;
            }
            viewStyleSwitch.Text = "All layers";

            NewResetLayout(true);

            layerScrollBar.Enabled = false;
            layerLabel.Hide();
        }

        private bool IsCrashRobust()
        {
            if (!drawAllLayers)
                return true;

            if (totalBlocks >= 9500)
            {
                MessageBox.Show("Unfortunately my choice of platform was poor for this type of a task so there can only exist 10k Control objects in a Form before it blows up. You've reached this limit! Switching to per-layer. Sorry.");
                return false;
            }
            return true;
        }

        private void saveAsImage_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Save the entire reactor? Selecting \"No\" will save the currently displayed (or first) layer", "Save entire structure?", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel)
                return;
            bool saveAll = (res == DialogResult.Yes) ? true : false;
            string fileName = null;
            using (SaveFileDialog fileDialog = new SaveFileDialog { Filter = "Image files (*.png)|*.png" })
            {
                string autoFileName = "";
                if(loadedSaveFileName == null)
                {
                    if (saveAll)
                        autoFileName = string.Format("{0} {1} x {2} x {3}", (fuelSelector.SelectedItem == null) ? "Custom" : fuelSelector.SelectedItem.ToString(), Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z);
                    else
                        autoFileName = string.Format("{0} {1} x {2} x {3} layer {4}", (fuelSelector.SelectedItem == null) ? "Custom" : fuelSelector.SelectedItem.ToString(), Reactor.interiorDims.X, Reactor.interiorDims.Y, Reactor.interiorDims.Z, layerScrollBar.Value);
                }
                else
                {
                    if(saveAll)
                        autoFileName = loadedSaveFileInfo.Name;
                    else
                        autoFileName = loadedSaveFileInfo.Name + " layer " + layerScrollBar.Value;
                }
                fileDialog.FileName = autoFileName;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                    fileName = fileDialog.FileName;
            }
            if (fileName != null)
            {
                if (saveAll)
                {
                    if(drawAllLayers | totalBlocks <= 9500)
                        Reactor.SaveReactorAsImage(fileName, stats.Lines.Length, (int)imageScale.Value);
                    else
                    {
                        Reactor.SaveReactorAsImage(fileName, stats.Lines.Length, (int)imageScale.Value, true);
                        NewResetLayout(true);
                    }
                }
                else
                    Reactor.SaveLayerAsImage(layerScrollBar.Value, fileName, (int)imageScale.Value);
            }
        }

        private void imageScale_ValueChanged(object sender, EventArgs e)
        {
            blockSize = (int)(Palette.textures["Air"].Size.Height * imageScale.Value);

            reactorGrid.Hide();
            foreach (ReactorGridLayer layer in Reactor.layers)
            {
                layer.Rescale();
                UpdateLocation(layer);
            }
            reactorGrid.Show();
                
        }

        private void UpdateLocation(ReactorGridLayer layer)
        {
            Point origin;
            if (drawAllLayers)
            {
                int layersPerRow = (int)Math.Ceiling(Math.Sqrt(Reactor.interiorDims.Y));
                origin = new Point((layer.Y - 1) % layersPerRow * layer.Size.Width + (layer.Y - 1) % layersPerRow * blockSize,
                                    (layer.Y - 1) / layersPerRow * layer.Size.Height + (layer.Y - 1) / layersPerRow * blockSize);
            }
            else
            {
                origin = new Point(Math.Max(0, (int)(reactorGrid.Size.Width / 2 - Reactor.interiorDims.X * blockSize / 2)),
                                            Math.Max(0, (int)(reactorGrid.Size.Height / 2 - Reactor.interiorDims.Z * blockSize / 2)));
            }
            layer.Location = origin;
        }

        private void fuelSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fuel selectedFuel = (Fuel)fuelSelector.SelectedItem;
            if(selectedFuel == null)
            {
                fuelSelector.Text = "";
                return;
            }
            fuelBaseEfficiency.Text = selectedFuel.BaseEfficiency.ToString();
            fuelBaseHeat.Text = selectedFuel.BaseHeat.ToString();
            fuelCriticalityFactor.Text = selectedFuel.CriticalityFactor.ToString();
            Palette.selectedFuel = selectedFuel; //[TODO]Change to a method you criminal

            //Reactor.Update();
            //Reactor.Redraw();//[TODO]Change redraw logic so it only does the active layer
            //RefreshStats();
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
                configurationUI.Show();
            }
            else
                configurationUI.Focus();
        }

        private void ConfigurationClosed(object sender, FormClosedEventArgs e)
        {
            fuelSelector.Items.Clear();
            fuelSelector.Items.AddRange(Reactor.fuels.ToArray());
            UpdatePaletteTooltips();
            Reactor.Redraw();
            RefreshStats();
        }

        private void UpdateWindowTitle()
        {
            if (loadedSaveFileInfo != null)
                this.Text = appName + "   " + loadedSaveFileInfo.FullName;
            else
                this.Text = appName;
        }

        private void PaletteActive_CheckedChanged(object sender, EventArgs e)
        {
            Palette.LoadPalette();
            if (Palette.selectedBlock.block is HeatSink)
            {
                ReactorGridCell oldSelected = Palette.selectedBlock;
                SetUpPalette();
                oldSelected.block = Palette.GetCooler(oldSelected.block.DisplayName);
                PaletteBlockClicked(oldSelected, new EventArgs());
            }
            else
                SetUpPalette();
            paletteTable.Invalidate();
        }

        private void RunReactor_Click(object sender, EventArgs e)
        {
            switch(Reactor.state)
            {
                case ReactorStates.Setup:
                    Reactor.Run();
                    RunReactor.BackColor = Color.Red;
                    RunReactor.Text = "Running";
                    break;
                case ReactorStates.Running:
                    Reactor.RevertToSetup();
                    RunReactor.BackColor = Color.Chartreuse;
                    RunReactor.Text = "Run Reactor";
                    break;
            }
        }
    }
}
