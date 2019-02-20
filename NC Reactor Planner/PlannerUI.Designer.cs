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
            this.paletteLabel = new System.Windows.Forms.Label();
            this.reactorGrid = new System.Windows.Forms.Panel();
            this.layerScrollBar = new System.Windows.Forms.VScrollBar();
            this.layerLabel = new System.Windows.Forms.Label();
            this.statsLabel = new System.Windows.Forms.Label();
            this.saveReactor = new System.Windows.Forms.Button();
            this.loadReactor = new System.Windows.Forms.Button();
            this.PaletteActive = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.viewStyleSwitch = new System.Windows.Forms.Button();
            this.saveAsImage = new System.Windows.Forms.Button();
            this.imageScale = new System.Windows.Forms.NumericUpDown();
            this.fuelBaseRFLabel = new System.Windows.Forms.Label();
            this.fuelBasePower = new System.Windows.Forms.TextBox();
            this.fuelBaseHeatLabel = new System.Windows.Forms.Label();
            this.fuelBaseHeat = new System.Windows.Forms.TextBox();
            this.fuelSelector = new System.Windows.Forms.ComboBox();
            this.stats = new System.Windows.Forms.RichTextBox();
            this.OpenConfig = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.reactorWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactorHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageScale)).BeginInit();
            this.SuspendLayout();
            // 
            // reactorWidth
            // 
            this.reactorWidth.Location = new System.Drawing.Point(152, 57);
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
            5,
            0,
            0,
            0});
            this.reactorWidth.Enter += new System.EventHandler(this.reactorWidth_Enter);
            // 
            // reactorLength
            // 
            this.reactorLength.Location = new System.Drawing.Point(282, 57);
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
            5,
            0,
            0,
            0});
            this.reactorLength.Enter += new System.EventHandler(this.reactorLength_Enter);
            // 
            // reactorHeight
            // 
            this.reactorHeight.Location = new System.Drawing.Point(218, 57);
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
            this.sizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sizeLabel.Location = new System.Drawing.Point(152, 12);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(168, 20);
            this.sizeLabel.TabIndex = 7;
            this.sizeLabel.Text = "Reactor dimensions";
            // 
            // x1
            // 
            this.x1.AutoSize = true;
            this.x1.Location = new System.Drawing.Point(198, 59);
            this.x1.Name = "x1";
            this.x1.Size = new System.Drawing.Size(14, 13);
            this.x1.TabIndex = 8;
            this.x1.Text = "X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(262, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(215, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(153, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(279, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Z";
            // 
            // resetLayout
            // 
            this.resetLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resetLayout.Location = new System.Drawing.Point(150, 83);
            this.resetLayout.Name = "resetLayout";
            this.resetLayout.Size = new System.Drawing.Size(170, 34);
            this.resetLayout.TabIndex = 4;
            this.resetLayout.Text = "Reset layout";
            this.resetLayout.UseVisualStyleBackColor = true;
            this.resetLayout.Click += new System.EventHandler(this.resetLayout_Click);
            // 
            // paletteLabel
            // 
            this.paletteLabel.AutoSize = true;
            this.paletteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.paletteLabel.Location = new System.Drawing.Point(153, 120);
            this.paletteLabel.Name = "paletteLabel";
            this.paletteLabel.Size = new System.Drawing.Size(57, 16);
            this.paletteLabel.TabIndex = 14;
            this.paletteLabel.Text = "Palette";
            // 
            // reactorGrid
            // 
            this.reactorGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reactorGrid.AutoScroll = true;
            this.reactorGrid.Location = new System.Drawing.Point(376, 37);
            this.reactorGrid.MinimumSize = new System.Drawing.Size(200, 200);
            this.reactorGrid.Name = "reactorGrid";
            this.reactorGrid.Size = new System.Drawing.Size(518, 592);
            this.reactorGrid.TabIndex = 16;
            this.reactorGrid.MouseEnter += new System.EventHandler(this.reactorGrid_MouseEnter);
            // 
            // layerScrollBar
            // 
            this.layerScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.layerScrollBar.Enabled = false;
            this.layerScrollBar.LargeChange = 1;
            this.layerScrollBar.Location = new System.Drawing.Point(338, 8);
            this.layerScrollBar.Maximum = 1;
            this.layerScrollBar.Minimum = 1;
            this.layerScrollBar.Name = "layerScrollBar";
            this.layerScrollBar.Size = new System.Drawing.Size(30, 644);
            this.layerScrollBar.TabIndex = 0;
            this.layerScrollBar.Value = 1;
            this.layerScrollBar.ValueChanged += new System.EventHandler(this.layerScrollBar_ValueChanged);
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.layerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layerLabel.Location = new System.Drawing.Point(457, 8);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(61, 24);
            this.layerLabel.TabIndex = 17;
            this.layerLabel.Text = "Layer";
            // 
            // statsLabel
            // 
            this.statsLabel.AutoSize = true;
            this.statsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statsLabel.Location = new System.Drawing.Point(12, 369);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(52, 20);
            this.statsLabel.TabIndex = 19;
            this.statsLabel.Text = "Stats";
            // 
            // saveReactor
            // 
            this.saveReactor.Enabled = false;
            this.saveReactor.Location = new System.Drawing.Point(7, 99);
            this.saveReactor.Name = "saveReactor";
            this.saveReactor.Size = new System.Drawing.Size(124, 23);
            this.saveReactor.TabIndex = 11;
            this.saveReactor.Text = "Save Reactor";
            this.saveReactor.UseVisualStyleBackColor = true;
            this.saveReactor.Click += new System.EventHandler(this.saveReactor_Click);
            // 
            // loadReactor
            // 
            this.loadReactor.Location = new System.Drawing.Point(7, 129);
            this.loadReactor.Name = "loadReactor";
            this.loadReactor.Size = new System.Drawing.Size(124, 23);
            this.loadReactor.TabIndex = 12;
            this.loadReactor.Text = "Load Reactor";
            this.loadReactor.UseVisualStyleBackColor = true;
            this.loadReactor.Click += new System.EventHandler(this.loadReactor_Click);
            // 
            // PaletteActive
            // 
            this.PaletteActive.AutoSize = true;
            this.PaletteActive.Location = new System.Drawing.Point(303, 122);
            this.PaletteActive.Name = "PaletteActive";
            this.PaletteActive.Size = new System.Drawing.Size(15, 14);
            this.PaletteActive.TabIndex = 15;
            this.PaletteActive.UseVisualStyleBackColor = true;
            this.PaletteActive.CheckedChanged += new System.EventHandler(this.PaletteActive_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(250, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "Active";
            // 
            // viewStyleSwitch
            // 
            this.viewStyleSwitch.Enabled = false;
            this.viewStyleSwitch.Location = new System.Drawing.Point(376, 9);
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
            this.saveAsImage.Size = new System.Drawing.Size(124, 23);
            this.saveAsImage.TabIndex = 10;
            this.saveAsImage.Text = "Save PNG";
            this.saveAsImage.UseVisualStyleBackColor = true;
            this.saveAsImage.Click += new System.EventHandler(this.saveAsImage_Click);
            // 
            // imageScale
            // 
            this.imageScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageScale.Enabled = false;
            this.imageScale.Location = new System.Drawing.Point(843, 12);
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
            this.fuelBaseRFLabel.Location = new System.Drawing.Point(493, 636);
            this.fuelBaseRFLabel.Name = "fuelBaseRFLabel";
            this.fuelBaseRFLabel.Size = new System.Drawing.Size(124, 16);
            this.fuelBaseRFLabel.TabIndex = 21;
            this.fuelBaseRFLabel.Text = "Fuel base Power";
            // 
            // fuelBasePower
            // 
            this.fuelBasePower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBasePower.Enabled = false;
            this.fuelBasePower.Location = new System.Drawing.Point(623, 635);
            this.fuelBasePower.Name = "fuelBasePower";
            this.fuelBasePower.ReadOnly = true;
            this.fuelBasePower.Size = new System.Drawing.Size(74, 20);
            this.fuelBasePower.TabIndex = 7;
            // 
            // fuelBaseHeatLabel
            // 
            this.fuelBaseHeatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeatLabel.AutoSize = true;
            this.fuelBaseHeatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fuelBaseHeatLabel.Location = new System.Drawing.Point(703, 636);
            this.fuelBaseHeatLabel.Name = "fuelBaseHeatLabel";
            this.fuelBaseHeatLabel.Size = new System.Drawing.Size(114, 16);
            this.fuelBaseHeatLabel.TabIndex = 23;
            this.fuelBaseHeatLabel.Text = "Fuel base Heat";
            // 
            // fuelBaseHeat
            // 
            this.fuelBaseHeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelBaseHeat.Enabled = false;
            this.fuelBaseHeat.Location = new System.Drawing.Point(823, 635);
            this.fuelBaseHeat.Name = "fuelBaseHeat";
            this.fuelBaseHeat.ReadOnly = true;
            this.fuelBaseHeat.Size = new System.Drawing.Size(74, 20);
            this.fuelBaseHeat.TabIndex = 8;
            // 
            // fuelSelector
            // 
            this.fuelSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fuelSelector.Enabled = false;
            this.fuelSelector.FormattingEnabled = true;
            this.fuelSelector.Location = new System.Drawing.Point(376, 635);
            this.fuelSelector.Name = "fuelSelector";
            this.fuelSelector.Size = new System.Drawing.Size(111, 21);
            this.fuelSelector.TabIndex = 6;
            this.fuelSelector.SelectedIndexChanged += new System.EventHandler(this.fuelSelector_SelectedIndexChanged);
            // 
            // stats
            // 
            this.stats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stats.Location = new System.Drawing.Point(7, 392);
            this.stats.MaximumSize = new System.Drawing.Size(325, 607);
            this.stats.MinimumSize = new System.Drawing.Size(325, 100);
            this.stats.Name = "stats";
            this.stats.ReadOnly = true;
            this.stats.Size = new System.Drawing.Size(325, 259);
            this.stats.TabIndex = 24;
            this.stats.TabStop = false;
            this.stats.Text = "";
            // 
            // OpenConfig
            // 
            this.OpenConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OpenConfig.Location = new System.Drawing.Point(7, 24);
            this.OpenConfig.Name = "OpenConfig";
            this.OpenConfig.Size = new System.Drawing.Size(124, 23);
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
            this.label5.Location = new System.Drawing.Point(783, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 26;
            this.label5.Text = "Scale:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 180);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 23);
            this.button1.TabIndex = 27;
            this.button1.Text = "Load Reactor";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // PlannerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(902, 664);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PaletteActive);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.reactorWidth);
            this.Controls.Add(this.OpenConfig);
            this.Controls.Add(this.reactorHeight);
            this.Controls.Add(this.stats);
            this.Controls.Add(this.x1);
            this.Controls.Add(this.fuelSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fuelBaseHeat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fuelBaseHeatLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fuelBasePower);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fuelBaseRFLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.imageScale);
            this.Controls.Add(this.paletteLabel);
            this.Controls.Add(this.saveAsImage);
            this.Controls.Add(this.resetLayout);
            this.Controls.Add(this.viewStyleSwitch);
            this.Controls.Add(this.reactorLength);
            this.Controls.Add(this.layerScrollBar);
            this.Controls.Add(this.loadReactor);
            this.Controls.Add(this.saveReactor);
            this.Controls.Add(this.statsLabel);
            this.Controls.Add(this.layerLabel);
            this.Controls.Add(this.reactorGrid);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.MinimumSize = new System.Drawing.Size(918, 542);
            this.Name = "PlannerUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NC Reactor Planner";
            this.Load += new System.EventHandler(this.Form1_Load);
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
        private System.Windows.Forms.Label paletteLabel;
        private System.Windows.Forms.Panel reactorGrid;
        private System.Windows.Forms.VScrollBar layerScrollBar;
        private System.Windows.Forms.Label layerLabel;
        private System.Windows.Forms.Label statsLabel;
        private System.Windows.Forms.Button saveReactor;
        private System.Windows.Forms.Button loadReactor;
        private System.Windows.Forms.Button viewStyleSwitch;
        private System.Windows.Forms.Button saveAsImage;
        private System.Windows.Forms.NumericUpDown imageScale;
        private System.Windows.Forms.Label fuelBaseRFLabel;
        private System.Windows.Forms.TextBox fuelBasePower;
        private System.Windows.Forms.Label fuelBaseHeatLabel;
        private System.Windows.Forms.TextBox fuelBaseHeat;
        private System.Windows.Forms.ComboBox fuelSelector;
        private System.Windows.Forms.RichTextBox stats;
        private System.Windows.Forms.Button OpenConfig;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox PaletteActive;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
    }
}

