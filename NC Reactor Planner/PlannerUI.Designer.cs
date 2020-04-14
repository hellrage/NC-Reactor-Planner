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
            this.drawOverlay = new System.Windows.Forms.CheckBox();
            this.discordPB = new System.Windows.Forms.PictureBox();
            this.githubPB = new System.Windows.Forms.PictureBox();
            this.fuelTextLabel = new System.Windows.Forms.Label();
            this.coolantRecipeTextLabel = new System.Windows.Forms.Label();
            this.coolantRecipeSelector = new System.Windows.Forms.ComboBox();
            this.coolantHeatCapacityLabel = new System.Windows.Forms.Label();
            this.coolantHeatCapacity = new System.Windows.Forms.TextBox();
            this.coolantRecipeOutToInRatioLabel = new System.Windows.Forms.Label();
            this.coolantOutToInRatio = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.reactorWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubPB)).BeginInit();
            this.SuspendLayout();
            // 
            // reactorWidth
            // 
            this.reactorWidth.Location = new System.Drawing.Point(154, 59);
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
            this.reactorWidth.Size = new System.Drawing.Size(40, 20);
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
            this.reactorLength.Location = new System.Drawing.Point(284, 59);
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
            this.reactorLength.Size = new System.Drawing.Size(40, 20);
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
            this.reactorHeight.Location = new System.Drawing.Point(220, 59);
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
            this.reactorHeight.Size = new System.Drawing.Size(40, 20);
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
            this.sizeLabel.Location = new System.Drawing.Point(163, 16);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(159, 18);
            this.sizeLabel.TabIndex = 7;
            this.sizeLabel.Text = "Reactor dimensions";
            // 
            // x1
            // 
            this.x1.AutoSize = true;
            this.x1.Location = new System.Drawing.Point(200, 61);
            this.x1.Name = "x1";
            this.x1.Size = new System.Drawing.Size(15, 15);
            this.x1.TabIndex = 8;
            this.x1.Text = "X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(229, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(163, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(292, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 18);
            this.label4.TabIndex = 12;
            this.label4.Text = "Z";
            // 
            // resetLayout
            // 
            this.resetLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resetLayout.Location = new System.Drawing.Point(154, 85);
            this.resetLayout.Name = "resetLayout";
            this.resetLayout.Size = new System.Drawing.Size(170, 34);
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
            this.reactorGrid.Location = new System.Drawing.Point(391, 37);
            this.reactorGrid.MinimumSize = new System.Drawing.Size(200, 200);
            this.reactorGrid.Name = "reactorGrid";
            this.reactorGrid.Size = new System.Drawing.Size(580, 574);
            this.reactorGrid.TabIndex = 16;
            this.reactorGrid.MouseEnter += new System.EventHandler(this.reactorGrid_MouseEnter);
            // 
            // layerScrollBar
            // 
            this.layerScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.layerScrollBar.Enabled = false;
            this.layerScrollBar.LargeChange = 1;
            this.layerScrollBar.Location = new System.Drawing.Point(358, 7);
            this.layerScrollBar.Maximum = 1;
            this.layerScrollBar.Minimum = 1;
            this.layerScrollBar.Name = "layerScrollBar";
            this.layerScrollBar.Size = new System.Drawing.Size(30, 696);
            this.layerScrollBar.TabIndex = 0;
            this.layerScrollBar.Value = 1;
            this.layerScrollBar.ValueChanged += new System.EventHandler(this.layerScrollBar_ValueChanged);
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.layerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layerLabel.Location = new System.Drawing.Point(472, 7);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(71, 25);
            this.layerLabel.TabIndex = 17;
            this.layerLabel.Text = "Layer";
            // 
            // statsLabel
            // 
            this.statsLabel.AutoSize = true;
            this.statsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statsLabel.Location = new System.Drawing.Point(16, 398);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(56, 22);
            this.statsLabel.TabIndex = 19;
            this.statsLabel.Text = "Stats";
            // 
            // saveReactor
            // 
            this.saveReactor.Enabled = false;
            this.saveReactor.Location = new System.Drawing.Point(7, 104);
            this.saveReactor.Name = "saveReactor";
            this.saveReactor.Size = new System.Drawing.Size(136, 28);
            this.saveReactor.TabIndex = 11;
            this.saveReactor.Text = "Save Reactor";
            this.saveReactor.UseVisualStyleBackColor = true;
            this.saveReactor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.saveReactor_MouseClick);
            // 
            // loadReactor
            // 
            this.loadReactor.Location = new System.Drawing.Point(7, 138);
            this.loadReactor.Name = "loadReactor";
            this.loadReactor.Size = new System.Drawing.Size(136, 28);
            this.loadReactor.TabIndex = 12;
            this.loadReactor.Text = "Load Reactor";
            this.loadReactor.UseVisualStyleBackColor = true;
            this.loadReactor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.loadReactor_MouseClick);
            // 
            // showClusterInfo
            // 
            this.showClusterInfo.AutoSize = true;
            this.showClusterInfo.Checked = true;
            this.showClusterInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showClusterInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.showClusterInfo.Location = new System.Drawing.Point(196, 400);
            this.showClusterInfo.Name = "showClusterInfo";
            this.showClusterInfo.Size = new System.Drawing.Size(143, 20);
            this.showClusterInfo.TabIndex = 30;
            this.showClusterInfo.Text = "Show cluster info";
            this.showClusterInfo.UseVisualStyleBackColor = true;
            this.showClusterInfo.CheckedChanged += new System.EventHandler(this.showClusterInfo_CheckedChanged);
            // 
            // viewStyleSwitch
            // 
            this.viewStyleSwitch.Enabled = false;
            this.viewStyleSwitch.Location = new System.Drawing.Point(391, 8);
            this.viewStyleSwitch.Name = "viewStyleSwitch";
            this.viewStyleSwitch.Size = new System.Drawing.Size(75, 23);
            this.viewStyleSwitch.TabIndex = 5;
            this.viewStyleSwitch.Text = "Per layer";
            this.viewStyleSwitch.UseVisualStyleBackColor = true;
            this.viewStyleSwitch.Click += new System.EventHandler(this.viewStyleSwitch_Click);
            // 
            // saveAsImage
            // 
            this.saveAsImage.Enabled = false;
            this.saveAsImage.Location = new System.Drawing.Point(7, 70);
            this.saveAsImage.Name = "saveAsImage";
            this.saveAsImage.Size = new System.Drawing.Size(136, 28);
            this.saveAsImage.TabIndex = 10;
            this.saveAsImage.Text = "Save PNG";
            this.saveAsImage.UseVisualStyleBackColor = true;
            this.saveAsImage.Click += new System.EventHandler(this.saveAsImage_Click);
            // 
            // imageScale
            // 
            this.imageScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageScale.DecimalPlaces = 1;
            this.imageScale.Enabled = false;
            this.imageScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.imageScale.Location = new System.Drawing.Point(920, 12);
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
            this.imageScale.Size = new System.Drawing.Size(47, 20);
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
            this.fuelBaseRFLabel.Location = new System.Drawing.Point(436, 669);
            this.fuelBaseRFLabel.Name = "fuelBaseRFLabel";
            this.fuelBaseRFLabel.Size = new System.Drawing.Size(81, 18);
            this.fuelBaseRFLabel.TabIndex = 21;
            this.fuelBaseRFLabel.Text = "Efficiency";
            // 
            // fuelBaseEfficiency
            // 
            this.fuelBaseEfficiency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseEfficiency.Enabled = false;
            this.fuelBaseEfficiency.Location = new System.Drawing.Point(457, 688);
            this.fuelBaseEfficiency.Name = "fuelBaseEfficiency";
            this.fuelBaseEfficiency.ReadOnly = true;
            this.fuelBaseEfficiency.Size = new System.Drawing.Size(30, 20);
            this.fuelBaseEfficiency.TabIndex = 7;
            // 
            // fuelBaseHeatLabel
            // 
            this.fuelBaseHeatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeatLabel.AutoSize = true;
            this.fuelBaseHeatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelBaseHeatLabel.Location = new System.Drawing.Point(518, 669);
            this.fuelBaseHeatLabel.Name = "fuelBaseHeatLabel";
            this.fuelBaseHeatLabel.Size = new System.Drawing.Size(43, 18);
            this.fuelBaseHeatLabel.TabIndex = 23;
            this.fuelBaseHeatLabel.Text = "Heat";
            // 
            // fuelBaseHeat
            // 
            this.fuelBaseHeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeat.Enabled = false;
            this.fuelBaseHeat.Location = new System.Drawing.Point(521, 688);
            this.fuelBaseHeat.Name = "fuelBaseHeat";
            this.fuelBaseHeat.ReadOnly = true;
            this.fuelBaseHeat.Size = new System.Drawing.Size(35, 20);
            this.fuelBaseHeat.TabIndex = 8;
            // 
            // fuelSelector
            // 
            this.fuelSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelSelector.DropDownHeight = 500;
            this.fuelSelector.Enabled = false;
            this.fuelSelector.Font = new System.Drawing.Font("Consolas", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelSelector.FormattingEnabled = true;
            this.fuelSelector.IntegralHeight = false;
            this.fuelSelector.Location = new System.Drawing.Point(419, 644);
            this.fuelSelector.Name = "fuelSelector";
            this.fuelSelector.Size = new System.Drawing.Size(237, 23);
            this.fuelSelector.TabIndex = 6;
            this.fuelSelector.SelectedIndexChanged += new System.EventHandler(this.fuelSelector_SelectedIndexChanged);
            // 
            // stats
            // 
            this.stats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stats.Location = new System.Drawing.Point(7, 425);
            this.stats.MaximumSize = new System.Drawing.Size(348, 607);
            this.stats.MinimumSize = new System.Drawing.Size(348, 100);
            this.stats.Name = "stats";
            this.stats.ReadOnly = true;
            this.stats.Size = new System.Drawing.Size(348, 278);
            this.stats.TabIndex = 24;
            this.stats.TabStop = false;
            this.stats.Text = "";
            // 
            // OpenConfig
            // 
            this.OpenConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OpenConfig.Location = new System.Drawing.Point(7, 16);
            this.OpenConfig.Name = "OpenConfig";
            this.OpenConfig.Size = new System.Drawing.Size(136, 28);
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
            this.label5.Location = new System.Drawing.Point(860, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 18);
            this.label5.TabIndex = 26;
            this.label5.Text = "Scale:";
            // 
            // fuelCriticalityFactorLabel
            // 
            this.fuelCriticalityFactorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelCriticalityFactorLabel.AutoSize = true;
            this.fuelCriticalityFactorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelCriticalityFactorLabel.Location = new System.Drawing.Point(565, 669);
            this.fuelCriticalityFactorLabel.Name = "fuelCriticalityFactorLabel";
            this.fuelCriticalityFactorLabel.Size = new System.Drawing.Size(78, 18);
            this.fuelCriticalityFactorLabel.TabIndex = 27;
            this.fuelCriticalityFactorLabel.Text = "Criticality";
            // 
            // fuelCriticalityFactor
            // 
            this.fuelCriticalityFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelCriticalityFactor.Location = new System.Drawing.Point(584, 688);
            this.fuelCriticalityFactor.Name = "fuelCriticalityFactor";
            this.fuelCriticalityFactor.ReadOnly = true;
            this.fuelCriticalityFactor.Size = new System.Drawing.Size(30, 20);
            this.fuelCriticalityFactor.TabIndex = 28;
            // 
            // checkForUpdates
            // 
            this.checkForUpdates.Location = new System.Drawing.Point(7, 176);
            this.checkForUpdates.Name = "checkForUpdates";
            this.checkForUpdates.Size = new System.Drawing.Size(136, 28);
            this.checkForUpdates.TabIndex = 31;
            this.checkForUpdates.Text = "Check for Updates";
            this.checkForUpdates.UseVisualStyleBackColor = true;
            this.checkForUpdates.Click += new System.EventHandler(this.checkForUpdates_Click);
            // 
            // drawOverlay
            // 
            this.drawOverlay.AutoSize = true;
            this.drawOverlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.drawOverlay.Location = new System.Drawing.Point(584, 12);
            this.drawOverlay.Name = "drawOverlay";
            this.drawOverlay.Size = new System.Drawing.Size(184, 21);
            this.drawOverlay.TabIndex = 32;
            this.drawOverlay.Text = "Heatsink type overlay";
            this.drawOverlay.UseVisualStyleBackColor = true;
            this.drawOverlay.CheckedChanged += new System.EventHandler(this.drawOverlay_CheckedChanged);
            // 
            // discordPB
            // 
            this.discordPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.discordPB.Image = ((System.Drawing.Image)(resources.GetObject("discordPB.Image")));
            this.discordPB.Location = new System.Drawing.Point(7, 210);
            this.discordPB.Name = "discordPB";
            this.discordPB.Size = new System.Drawing.Size(136, 36);
            this.discordPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.discordPB.TabIndex = 33;
            this.discordPB.TabStop = false;
            this.discordPB.Click += new System.EventHandler(this.discordPB_Click);
            // 
            // githubPB
            // 
            this.githubPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.githubPB.Image = global::NC_Reactor_Planner.Properties.Resources.GitHub_Logo;
            this.githubPB.Location = new System.Drawing.Point(7, 252);
            this.githubPB.Name = "githubPB";
            this.githubPB.Size = new System.Drawing.Size(136, 28);
            this.githubPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.githubPB.TabIndex = 34;
            this.githubPB.TabStop = false;
            this.githubPB.Click += new System.EventHandler(this.githubPB_Click);
            // 
            // fuelTextLabel
            // 
            this.fuelTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelTextLabel.AutoSize = true;
            this.fuelTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTextLabel.Location = new System.Drawing.Point(503, 614);
            this.fuelTextLabel.Name = "fuelTextLabel";
            this.fuelTextLabel.Size = new System.Drawing.Size(65, 29);
            this.fuelTextLabel.TabIndex = 35;
            this.fuelTextLabel.Text = "Fuel";
            // 
            // coolantRecipeTextLabel
            // 
            this.coolantRecipeTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantRecipeTextLabel.AutoSize = true;
            this.coolantRecipeTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coolantRecipeTextLabel.Location = new System.Drawing.Point(754, 616);
            this.coolantRecipeTextLabel.Name = "coolantRecipeTextLabel";
            this.coolantRecipeTextLabel.Size = new System.Drawing.Size(184, 29);
            this.coolantRecipeTextLabel.TabIndex = 35;
            this.coolantRecipeTextLabel.Text = "Coolant recipe";
            // 
            // coolantRecipeSelector
            // 
            this.coolantRecipeSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantRecipeSelector.DropDownHeight = 500;
            this.coolantRecipeSelector.Font = new System.Drawing.Font("Consolas", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.coolantRecipeSelector.FormattingEnabled = true;
            this.coolantRecipeSelector.IntegralHeight = false;
            this.coolantRecipeSelector.Location = new System.Drawing.Point(714, 644);
            this.coolantRecipeSelector.Name = "coolantRecipeSelector";
            this.coolantRecipeSelector.Size = new System.Drawing.Size(237, 23);
            this.coolantRecipeSelector.TabIndex = 6;
            this.coolantRecipeSelector.SelectedIndexChanged += new System.EventHandler(this.CoolantRecipeSelector_SelectedIndexChanged);
            // 
            // coolantHeatCapacityLabel
            // 
            this.coolantHeatCapacityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantHeatCapacityLabel.AutoSize = true;
            this.coolantHeatCapacityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.coolantHeatCapacityLabel.Location = new System.Drawing.Point(711, 669);
            this.coolantHeatCapacityLabel.Name = "coolantHeatCapacityLabel";
            this.coolantHeatCapacityLabel.Size = new System.Drawing.Size(113, 18);
            this.coolantHeatCapacityLabel.TabIndex = 27;
            this.coolantHeatCapacityLabel.Text = "Heat Capacity";
            // 
            // coolantHeatCapacity
            // 
            this.coolantHeatCapacity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantHeatCapacity.Location = new System.Drawing.Point(749, 688);
            this.coolantHeatCapacity.Name = "coolantHeatCapacity";
            this.coolantHeatCapacity.ReadOnly = true;
            this.coolantHeatCapacity.Size = new System.Drawing.Size(30, 20);
            this.coolantHeatCapacity.TabIndex = 28;
            // 
            // coolantRecipeOutToInRatioLabel
            // 
            this.coolantRecipeOutToInRatioLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantRecipeOutToInRatioLabel.AutoSize = true;
            this.coolantRecipeOutToInRatioLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.coolantRecipeOutToInRatioLabel.Location = new System.Drawing.Point(845, 669);
            this.coolantRecipeOutToInRatioLabel.Name = "coolantRecipeOutToInRatioLabel";
            this.coolantRecipeOutToInRatioLabel.Size = new System.Drawing.Size(108, 18);
            this.coolantRecipeOutToInRatioLabel.TabIndex = 27;
            this.coolantRecipeOutToInRatioLabel.Text = "Out / In Ratio";
            // 
            // coolantOutToInRatio
            // 
            this.coolantOutToInRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.coolantOutToInRatio.Location = new System.Drawing.Point(883, 688);
            this.coolantOutToInRatio.Name = "coolantOutToInRatio";
            this.coolantOutToInRatio.ReadOnly = true;
            this.coolantOutToInRatio.Size = new System.Drawing.Size(30, 20);
            this.coolantOutToInRatio.TabIndex = 28;
            // 
            // PlannerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(979, 716);
            this.Controls.Add(this.coolantRecipeTextLabel);
            this.Controls.Add(this.fuelTextLabel);
            this.Controls.Add(this.githubPB);
            this.Controls.Add(this.discordPB);
            this.Controls.Add(this.drawOverlay);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.reactorWidth);
            this.Controls.Add(this.checkForUpdates);
            this.Controls.Add(this.reactorHeight);
            this.Controls.Add(this.showClusterInfo);
            this.Controls.Add(this.x1);
            this.Controls.Add(this.coolantOutToInRatio);
            this.Controls.Add(this.coolantHeatCapacity);
            this.Controls.Add(this.fuelCriticalityFactor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.coolantRecipeOutToInRatioLabel);
            this.Controls.Add(this.coolantHeatCapacityLabel);
            this.Controls.Add(this.fuelCriticalityFactorLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OpenConfig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.stats);
            this.Controls.Add(this.resetLayout);
            this.Controls.Add(this.coolantRecipeSelector);
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
            this.MinimumSize = new System.Drawing.Size(995, 601);
            this.Name = "PlannerUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NC Reactor Planner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Leave += new System.EventHandler(this.PlannerUI_Leave);
            this.Resize += new System.EventHandler(this.PlannerUI_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.reactorWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discordPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.githubPB)).EndInit();
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
        private System.Windows.Forms.CheckBox drawOverlay;
        private System.Windows.Forms.PictureBox discordPB;
        private System.Windows.Forms.PictureBox githubPB;
        private System.Windows.Forms.Label fuelTextLabel;
        private System.Windows.Forms.Label coolantRecipeTextLabel;
        private System.Windows.Forms.ComboBox coolantRecipeSelector;
        private System.Windows.Forms.Label coolantHeatCapacityLabel;
        private System.Windows.Forms.TextBox coolantHeatCapacity;
        private System.Windows.Forms.Label coolantRecipeOutToInRatioLabel;
        private System.Windows.Forms.TextBox coolantOutToInRatio;
    }
}

