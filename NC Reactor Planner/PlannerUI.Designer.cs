namespace NC_Reactor_Planner
{
    partial class PlannerUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlannerUI));
            this.reactorWidth = new System.Windows.Forms.NumericUpDown();
            this.reactorLength = new System.Windows.Forms.NumericUpDown();
            this.reactorHeight = new System.Windows.Forms.NumericUpDown();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.x1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.resetLayout = new System.Windows.Forms.Button();
            this.reactorGrid = new System.Windows.Forms.Panel();
            this.layerScrollBar = new System.Windows.Forms.VScrollBar();
            this.layerLabel = new System.Windows.Forms.Label();
            this.statsLabel = new System.Windows.Forms.Label();
            this.saveReactor = new System.Windows.Forms.Button();
            this.loadReactor = new System.Windows.Forms.Button();
            this.showClusterInfo = new System.Windows.Forms.CheckBox();
            this.viewStyleSwitch = new System.Windows.Forms.Button();
            this.saveAsImage = new System.Windows.Forms.Button();
            this.imageScale = new System.Windows.Forms.NumericUpDown();
            this.fuelBaseRFLabel = new System.Windows.Forms.Label();
            this.fuelBaseEfficiency = new System.Windows.Forms.TextBox();
            this.fuelBaseHeatLabel = new System.Windows.Forms.Label();
            this.fuelBaseHeat = new System.Windows.Forms.TextBox();
            this.fuelSelector = new System.Windows.Forms.ComboBox();
            this.stats = new System.Windows.Forms.RichTextBox();
            this.OpenConfig = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.fuelCriticalityFactorLabel = new System.Windows.Forms.Label();
            this.fuelCriticalityFactor = new System.Windows.Forms.TextBox();
            this.checkForUpdates = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.reactorWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageScale)).BeginInit();
            this.SuspendLayout();
            // 
            // reactorWidth
            // 
            this.reactorWidth.Location = new System.Drawing.Point(200, 73);
            this.reactorWidth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reactorWidth.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.reactorWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.reactorWidth.Name = "reactorWidth";
            this.reactorWidth.Size = new System.Drawing.Size(53, 22);
            this.reactorWidth.TabIndex = 1;
            this.reactorWidth.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.reactorWidth.Enter += new System.EventHandler(this.reactorWidth_Enter);
            // 
            // reactorLength
            // 
            this.reactorLength.Location = new System.Drawing.Point(373, 73);
            this.reactorLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reactorLength.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.reactorLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.reactorLength.Name = "reactorLength";
            this.reactorLength.Size = new System.Drawing.Size(53, 22);
            this.reactorLength.TabIndex = 3;
            this.reactorLength.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.reactorLength.Enter += new System.EventHandler(this.reactorLength_Enter);
            // 
            // reactorHeight
            // 
            this.reactorHeight.Location = new System.Drawing.Point(288, 73);
            this.reactorHeight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reactorHeight.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.reactorHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.reactorHeight.Name = "reactorHeight";
            this.reactorHeight.Size = new System.Drawing.Size(53, 22);
            this.reactorHeight.TabIndex = 2;
            this.reactorHeight.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.reactorHeight.Enter += new System.EventHandler(this.reactorHeight_Enter);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sizeLabel.Location = new System.Drawing.Point(217, 20);
            this.sizeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(176, 20);
            this.sizeLabel.TabIndex = 7;
            this.sizeLabel.Text = "Reactor dimensions";
            // 
            // x1
            // 
            this.x1.AutoSize = true;
            this.x1.Location = new System.Drawing.Point(261, 75);
            this.x1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.x1.Name = "x1";
            this.x1.Size = new System.Drawing.Size(17, 17);
            this.x1.TabIndex = 8;
            this.x1.Text = "X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(347, 75);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(284, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(201, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(369, 49);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Z";
            // 
            // resetLayout
            // 
            this.resetLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resetLayout.Location = new System.Drawing.Point(197, 105);
            this.resetLayout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.resetLayout.Name = "resetLayout";
            this.resetLayout.Size = new System.Drawing.Size(227, 42);
            this.resetLayout.TabIndex = 4;
            this.resetLayout.Text = "Reset layout";
            this.resetLayout.UseVisualStyleBackColor = true;
            this.resetLayout.Click += new System.EventHandler(this.resetLayout_Click);
            // 
            // reactorGrid
            // 
            this.reactorGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reactorGrid.AutoScroll = true;
            this.reactorGrid.Location = new System.Drawing.Point(501, 46);
            this.reactorGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reactorGrid.MinimumSize = new System.Drawing.Size(267, 246);
            this.reactorGrid.Name = "reactorGrid";
            this.reactorGrid.Size = new System.Drawing.Size(793, 793);
            this.reactorGrid.TabIndex = 16;
            this.reactorGrid.MouseEnter += new System.EventHandler(this.reactorGrid_MouseEnter);
            // 
            // layerScrollBar
            // 
            this.layerScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.layerScrollBar.Enabled = false;
            this.layerScrollBar.LargeChange = 1;
            this.layerScrollBar.Location = new System.Drawing.Point(451, 10);
            this.layerScrollBar.Maximum = 1;
            this.layerScrollBar.Minimum = 1;
            this.layerScrollBar.Name = "layerScrollBar";
            this.layerScrollBar.Size = new System.Drawing.Size(30, 857);
            this.layerScrollBar.TabIndex = 0;
            this.layerScrollBar.Value = 1;
            this.layerScrollBar.ValueChanged += new System.EventHandler(this.layerScrollBar_ValueChanged);
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.layerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layerLabel.Location = new System.Drawing.Point(609, 10);
            this.layerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(77, 29);
            this.layerLabel.TabIndex = 17;
            this.layerLabel.Text = "Layer";
            // 
            // statsLabel
            // 
            this.statsLabel.AutoSize = true;
            this.statsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statsLabel.Location = new System.Drawing.Point(21, 490);
            this.statsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(62, 25);
            this.statsLabel.TabIndex = 19;
            this.statsLabel.Text = "Stats";
            // 
            // saveReactor
            // 
            this.saveReactor.Enabled = false;
            this.saveReactor.Location = new System.Drawing.Point(9, 122);
            this.saveReactor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.saveReactor.Name = "saveReactor";
            this.saveReactor.Size = new System.Drawing.Size(165, 28);
            this.saveReactor.TabIndex = 11;
            this.saveReactor.Text = "Save Reactor";
            this.saveReactor.UseVisualStyleBackColor = true;
            this.saveReactor.Click += new System.EventHandler(this.saveReactor_Click);
            // 
            // loadReactor
            // 
            this.loadReactor.Location = new System.Drawing.Point(9, 159);
            this.loadReactor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.loadReactor.Name = "loadReactor";
            this.loadReactor.Size = new System.Drawing.Size(165, 28);
            this.loadReactor.TabIndex = 12;
            this.loadReactor.Text = "Load Reactor";
            this.loadReactor.UseVisualStyleBackColor = true;
            this.loadReactor.Click += new System.EventHandler(this.loadReactor_Click);
            // 
            // showClusterInfo
            // 
            this.showClusterInfo.AutoSize = true;
            this.showClusterInfo.Checked = true;
            this.showClusterInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showClusterInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.showClusterInfo.Location = new System.Drawing.Point(261, 492);
            this.showClusterInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.showClusterInfo.Name = "showClusterInfo";
            this.showClusterInfo.Size = new System.Drawing.Size(154, 21);
            this.showClusterInfo.TabIndex = 30;
            this.showClusterInfo.Text = "Show cluster info";
            this.showClusterInfo.UseVisualStyleBackColor = true;
            this.showClusterInfo.CheckedChanged += new System.EventHandler(this.showClusterInfo_CheckedChanged);
            // 
            // viewStyleSwitch
            // 
            this.viewStyleSwitch.Enabled = false;
            this.viewStyleSwitch.Location = new System.Drawing.Point(501, 11);
            this.viewStyleSwitch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.viewStyleSwitch.Name = "viewStyleSwitch";
            this.viewStyleSwitch.Size = new System.Drawing.Size(100, 28);
            this.viewStyleSwitch.TabIndex = 5;
            this.viewStyleSwitch.Text = "Per layer";
            this.viewStyleSwitch.UseVisualStyleBackColor = true;
            this.viewStyleSwitch.Click += new System.EventHandler(this.viewStyleSwitch_Click);
            // 
            // saveAsImage
            // 
            this.saveAsImage.Enabled = false;
            this.saveAsImage.Location = new System.Drawing.Point(9, 86);
            this.saveAsImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.saveAsImage.Name = "saveAsImage";
            this.saveAsImage.Size = new System.Drawing.Size(165, 28);
            this.saveAsImage.TabIndex = 10;
            this.saveAsImage.Text = "Save PNG";
            this.saveAsImage.UseVisualStyleBackColor = true;
            this.saveAsImage.Click += new System.EventHandler(this.saveAsImage_Click);
            // 
            // imageScale
            // 
            this.imageScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageScale.Enabled = false;
            this.imageScale.Location = new System.Drawing.Point(1227, 15);
            this.imageScale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.imageScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.imageScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.imageScale.Name = "imageScale";
            this.imageScale.Size = new System.Drawing.Size(63, 22);
            this.imageScale.TabIndex = 9;
            this.imageScale.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.imageScale.ValueChanged += new System.EventHandler(this.imageScale_ValueChanged);
            // 
            // fuelBaseRFLabel
            // 
            this.fuelBaseRFLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseRFLabel.AutoSize = true;
            this.fuelBaseRFLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelBaseRFLabel.Location = new System.Drawing.Point(825, 847);
            this.fuelBaseRFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fuelBaseRFLabel.Name = "fuelBaseRFLabel";
            this.fuelBaseRFLabel.Size = new System.Drawing.Size(92, 20);
            this.fuelBaseRFLabel.TabIndex = 21;
            this.fuelBaseRFLabel.Text = "Efficiency";
            // 
            // fuelBaseEfficiency
            // 
            this.fuelBaseEfficiency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseEfficiency.Enabled = false;
            this.fuelBaseEfficiency.Location = new System.Drawing.Point(933, 846);
            this.fuelBaseEfficiency.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fuelBaseEfficiency.Name = "fuelBaseEfficiency";
            this.fuelBaseEfficiency.ReadOnly = true;
            this.fuelBaseEfficiency.Size = new System.Drawing.Size(39, 22);
            this.fuelBaseEfficiency.TabIndex = 7;
            // 
            // fuelBaseHeatLabel
            // 
            this.fuelBaseHeatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeatLabel.AutoSize = true;
            this.fuelBaseHeatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelBaseHeatLabel.Location = new System.Drawing.Point(981, 847);
            this.fuelBaseHeatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fuelBaseHeatLabel.Name = "fuelBaseHeatLabel";
            this.fuelBaseHeatLabel.Size = new System.Drawing.Size(49, 20);
            this.fuelBaseHeatLabel.TabIndex = 23;
            this.fuelBaseHeatLabel.Text = "Heat";
            // 
            // fuelBaseHeat
            // 
            this.fuelBaseHeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeat.Enabled = false;
            this.fuelBaseHeat.Location = new System.Drawing.Point(1044, 847);
            this.fuelBaseHeat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fuelBaseHeat.Name = "fuelBaseHeat";
            this.fuelBaseHeat.ReadOnly = true;
            this.fuelBaseHeat.Size = new System.Drawing.Size(45, 22);
            this.fuelBaseHeat.TabIndex = 8;
            // 
            // fuelSelector
            // 
            this.fuelSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelSelector.Enabled = false;
            this.fuelSelector.FormattingEnabled = true;
            this.fuelSelector.Location = new System.Drawing.Point(501, 847);
            this.fuelSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fuelSelector.Name = "fuelSelector";
            this.fuelSelector.Size = new System.Drawing.Size(315, 24);
            this.fuelSelector.TabIndex = 6;
            this.fuelSelector.SelectedIndexChanged += new System.EventHandler(this.fuelSelector_SelectedIndexChanged);
            // 
            // stats
            // 
            this.stats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stats.Location = new System.Drawing.Point(9, 523);
            this.stats.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.stats.MaximumSize = new System.Drawing.Size(432, 746);
            this.stats.MinimumSize = new System.Drawing.Size(432, 122);
            this.stats.Name = "stats";
            this.stats.ReadOnly = true;
            this.stats.Size = new System.Drawing.Size(432, 341);
            this.stats.TabIndex = 24;
            this.stats.TabStop = false;
            this.stats.Text = "";
            // 
            // OpenConfig
            // 
            this.OpenConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OpenConfig.Location = new System.Drawing.Point(9, 30);
            this.OpenConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OpenConfig.MaximumSize = new System.Drawing.Size(165, 28);
            this.OpenConfig.Name = "OpenConfig";
            this.OpenConfig.Size = new System.Drawing.Size(165, 28);
            this.OpenConfig.TabIndex = 25;
            this.OpenConfig.Text = "Open configuration";
            this.OpenConfig.UseVisualStyleBackColor = true;
            this.OpenConfig.Click += new System.EventHandler(this.OpenConfiguration);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(1147, 17);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 26;
            this.label5.Text = "Scale:";
            // 
            // fuelCriticalityFactorLabel
            // 
            this.fuelCriticalityFactorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelCriticalityFactorLabel.AutoSize = true;
            this.fuelCriticalityFactorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelCriticalityFactorLabel.Location = new System.Drawing.Point(1099, 848);
            this.fuelCriticalityFactorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fuelCriticalityFactorLabel.Name = "fuelCriticalityFactorLabel";
            this.fuelCriticalityFactorLabel.Size = new System.Drawing.Size(90, 20);
            this.fuelCriticalityFactorLabel.TabIndex = 27;
            this.fuelCriticalityFactorLabel.Text = "Criticality";
            // 
            // fuelCriticalityFactor
            // 
            this.fuelCriticalityFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelCriticalityFactor.Location = new System.Drawing.Point(1203, 847);
            this.fuelCriticalityFactor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fuelCriticalityFactor.Name = "fuelCriticalityFactor";
            this.fuelCriticalityFactor.ReadOnly = true;
            this.fuelCriticalityFactor.Size = new System.Drawing.Size(39, 22);
            this.fuelCriticalityFactor.TabIndex = 28;
            // 
            // checkForUpdates
            // 
            this.checkForUpdates.Location = new System.Drawing.Point(9, 217);
            this.checkForUpdates.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkForUpdates.Name = "checkForUpdates";
            this.checkForUpdates.Size = new System.Drawing.Size(165, 28);
            this.checkForUpdates.TabIndex = 31;
            this.checkForUpdates.Text = "Check for Updates";
            this.checkForUpdates.UseVisualStyleBackColor = true;
            this.checkForUpdates.Click += new System.EventHandler(this.checkForUpdates_Click);
            // 
            // PlannerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1305, 881);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.reactorWidth);
            this.Controls.Add(this.checkForUpdates);
            this.Controls.Add(this.reactorHeight);
            this.Controls.Add(this.showClusterInfo);
            this.Controls.Add(this.x1);
            this.Controls.Add(this.fuelCriticalityFactor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fuelCriticalityFactorLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OpenConfig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.stats);
            this.Controls.Add(this.resetLayout);
            this.Controls.Add(this.fuelSelector);
            this.Controls.Add(this.reactorLength);
            this.Controls.Add(this.fuelBaseHeat);
            this.Controls.Add(this.fuelBaseHeatLabel);
            this.Controls.Add(this.fuelBaseEfficiency);
            this.Controls.Add(this.fuelBaseRFLabel);
            this.Controls.Add(this.imageScale);
            this.Controls.Add(this.saveAsImage);
            this.Controls.Add(this.viewStyleSwitch);
            this.Controls.Add(this.layerScrollBar);
            this.Controls.Add(this.loadReactor);
            this.Controls.Add(this.saveReactor);
            this.Controls.Add(this.statsLabel);
            this.Controls.Add(this.layerLabel);
            this.Controls.Add(this.reactorGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1321, 730);
            this.Name = "PlannerUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NC Reactor Planner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Leave += new System.EventHandler(this.PlannerUI_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.reactorWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown reactorWidth;
        private System.Windows.Forms.NumericUpDown reactorLength;
        private System.Windows.Forms.NumericUpDown reactorHeight;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Label x1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button resetLayout;
        private System.Windows.Forms.VScrollBar layerScrollBar;
        private System.Windows.Forms.Label layerLabel;
        private System.Windows.Forms.Label statsLabel;
        private System.Windows.Forms.Button saveReactor;
        private System.Windows.Forms.Button loadReactor;
        private System.Windows.Forms.Button viewStyleSwitch;
        private System.Windows.Forms.Button saveAsImage;
        private System.Windows.Forms.NumericUpDown imageScale;
        private System.Windows.Forms.Label fuelBaseRFLabel;
        private System.Windows.Forms.TextBox fuelBaseEfficiency;
        private System.Windows.Forms.Label fuelBaseHeatLabel;
        private System.Windows.Forms.TextBox fuelBaseHeat;
        private System.Windows.Forms.ComboBox fuelSelector;
        private System.Windows.Forms.RichTextBox stats;
        private System.Windows.Forms.Button OpenConfig;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label fuelCriticalityFactorLabel;
        private System.Windows.Forms.TextBox fuelCriticalityFactor;
        private System.Windows.Forms.CheckBox showClusterInfo;
        private System.Windows.Forms.Panel reactorGrid;
        private System.Windows.Forms.Button checkForUpdates;
    }
}

