namespace NC_Reactor_Planner
{
    partial class ConfigurationUI
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
            this.components = new System.ComponentModel.Container();
            this.settingTabs = new System.Windows.Forms.TabControl();
            this.fissionPage = new System.Windows.Forms.TabPage();
            this.maxSize = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.minSize = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.neutronReach = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.moderatorExtraHeat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.moderatorExtraPower = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.heatGeneration = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fuelUse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.power = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.coolersPage = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.fuelsPage = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.resourceCostsTab = new System.Windows.Forms.TabPage();
            this.blockSelector = new System.Windows.Forms.ComboBox();
            this.Save = new System.Windows.Forms.Button();
            this.LoadConfig = new System.Windows.Forms.Button();
            this.ApplyConfig = new System.Windows.Forms.Button();
            this.Import = new System.Windows.Forms.Button();
            this.ttValidation = new System.Windows.Forms.ToolTip(this.components);
            this.settingTabs.SuspendLayout();
            this.fissionPage.SuspendLayout();
            this.coolersPage.SuspendLayout();
            this.fuelsPage.SuspendLayout();
            this.resourceCostsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingTabs
            // 
            this.settingTabs.Controls.Add(this.fissionPage);
            this.settingTabs.Controls.Add(this.coolersPage);
            this.settingTabs.Controls.Add(this.fuelsPage);
            this.settingTabs.Controls.Add(this.resourceCostsTab);
            this.settingTabs.Location = new System.Drawing.Point(12, 12);
            this.settingTabs.Name = "settingTabs";
            this.settingTabs.SelectedIndex = 0;
            this.settingTabs.Size = new System.Drawing.Size(646, 395);
            this.settingTabs.TabIndex = 1;
            // 
            // fissionPage
            // 
            this.fissionPage.Controls.Add(this.maxSize);
            this.fissionPage.Controls.Add(this.label14);
            this.fissionPage.Controls.Add(this.minSize);
            this.fissionPage.Controls.Add(this.label13);
            this.fissionPage.Controls.Add(this.neutronReach);
            this.fissionPage.Controls.Add(this.label6);
            this.fissionPage.Controls.Add(this.moderatorExtraHeat);
            this.fissionPage.Controls.Add(this.label5);
            this.fissionPage.Controls.Add(this.moderatorExtraPower);
            this.fissionPage.Controls.Add(this.label4);
            this.fissionPage.Controls.Add(this.heatGeneration);
            this.fissionPage.Controls.Add(this.label3);
            this.fissionPage.Controls.Add(this.fuelUse);
            this.fissionPage.Controls.Add(this.label2);
            this.fissionPage.Controls.Add(this.power);
            this.fissionPage.Controls.Add(this.label1);
            this.fissionPage.Location = new System.Drawing.Point(4, 22);
            this.fissionPage.Name = "fissionPage";
            this.fissionPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.fissionPage.Size = new System.Drawing.Size(638, 369);
            this.fissionPage.TabIndex = 0;
            this.fissionPage.Text = "Fission";
            this.fissionPage.UseVisualStyleBackColor = true;
            // 
            // maxSize
            // 
            this.maxSize.Location = new System.Drawing.Point(10, 304);
            this.maxSize.Name = "maxSize";
            this.maxSize.Size = new System.Drawing.Size(174, 20);
            this.maxSize.TabIndex = 13;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(7, 287);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(105, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Reactor Max size";
            // 
            // minSize
            // 
            this.minSize.Location = new System.Drawing.Point(10, 264);
            this.minSize.Name = "minSize";
            this.minSize.Size = new System.Drawing.Size(174, 20);
            this.minSize.TabIndex = 11;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(7, 247);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Reactor Min size";
            // 
            // neutronReach
            // 
            this.neutronReach.Location = new System.Drawing.Point(10, 224);
            this.neutronReach.Name = "neutronReach";
            this.neutronReach.Size = new System.Drawing.Size(174, 20);
            this.neutronReach.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(7, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Fission neutron reach";
            // 
            // moderatorExtraHeat
            // 
            this.moderatorExtraHeat.Location = new System.Drawing.Point(10, 184);
            this.moderatorExtraHeat.Name = "moderatorExtraHeat";
            this.moderatorExtraHeat.Size = new System.Drawing.Size(174, 20);
            this.moderatorExtraHeat.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(7, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Fission Moderator extra heat";
            // 
            // moderatorExtraPower
            // 
            this.moderatorExtraPower.Location = new System.Drawing.Point(10, 144);
            this.moderatorExtraPower.Name = "moderatorExtraPower";
            this.moderatorExtraPower.Size = new System.Drawing.Size(174, 20);
            this.moderatorExtraPower.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(7, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Fission Moderator extra power";
            // 
            // heatGeneration
            // 
            this.heatGeneration.Location = new System.Drawing.Point(10, 104);
            this.heatGeneration.Name = "heatGeneration";
            this.heatGeneration.Size = new System.Drawing.Size(174, 20);
            this.heatGeneration.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(7, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Fission heat generation";
            // 
            // fuelUse
            // 
            this.fuelUse.Location = new System.Drawing.Point(10, 64);
            this.fuelUse.Name = "fuelUse";
            this.fuelUse.Size = new System.Drawing.Size(174, 20);
            this.fuelUse.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Fission fuel use";
            // 
            // power
            // 
            this.power.Location = new System.Drawing.Point(10, 24);
            this.power.Name = "power";
            this.power.Size = new System.Drawing.Size(174, 20);
            this.power.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fission power";
            // 
            // coolersPage
            // 
            this.coolersPage.AutoScroll = true;
            this.coolersPage.Controls.Add(this.label12);
            this.coolersPage.Controls.Add(this.label8);
            this.coolersPage.Controls.Add(this.label7);
            this.coolersPage.Location = new System.Drawing.Point(4, 22);
            this.coolersPage.Name = "coolersPage";
            this.coolersPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.coolersPage.Size = new System.Drawing.Size(638, 369);
            this.coolersPage.TabIndex = 1;
            this.coolersPage.Text = "Coolers";
            this.coolersPage.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(281, 3);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Requirements";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(161, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Active cooling rate";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(80, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Cooling rate";
            // 
            // fuelsPage
            // 
            this.fuelsPage.AutoScroll = true;
            this.fuelsPage.Controls.Add(this.label11);
            this.fuelsPage.Controls.Add(this.label10);
            this.fuelsPage.Controls.Add(this.label9);
            this.fuelsPage.Location = new System.Drawing.Point(4, 22);
            this.fuelsPage.Name = "fuelsPage";
            this.fuelsPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.fuelsPage.Size = new System.Drawing.Size(638, 369);
            this.fuelsPage.TabIndex = 2;
            this.fuelsPage.Text = "Fuels";
            this.fuelsPage.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(312, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Fuel Time";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(240, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Base Heat";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(160, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Base Power";
            // 
            // resourceCostsTab
            // 
            this.resourceCostsTab.Controls.Add(this.blockSelector);
            this.resourceCostsTab.Location = new System.Drawing.Point(4, 22);
            this.resourceCostsTab.Name = "resourceCostsTab";
            this.resourceCostsTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.resourceCostsTab.Size = new System.Drawing.Size(638, 369);
            this.resourceCostsTab.TabIndex = 3;
            this.resourceCostsTab.Text = "Resource Costs";
            this.resourceCostsTab.UseVisualStyleBackColor = true;
            // 
            // blockSelector
            // 
            this.blockSelector.FormattingEnabled = true;
            this.blockSelector.Location = new System.Drawing.Point(7, 7);
            this.blockSelector.Name = "blockSelector";
            this.blockSelector.Size = new System.Drawing.Size(306, 21);
            this.blockSelector.TabIndex = 0;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(174, 415);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // LoadConfig
            // 
            this.LoadConfig.Location = new System.Drawing.Point(93, 415);
            this.LoadConfig.Name = "LoadConfig";
            this.LoadConfig.Size = new System.Drawing.Size(75, 23);
            this.LoadConfig.TabIndex = 4;
            this.LoadConfig.Text = "Load";
            this.LoadConfig.UseVisualStyleBackColor = true;
            this.LoadConfig.Click += new System.EventHandler(this.Load_Click);
            // 
            // ApplyConfig
            // 
            this.ApplyConfig.Location = new System.Drawing.Point(12, 415);
            this.ApplyConfig.Name = "ApplyConfig";
            this.ApplyConfig.Size = new System.Drawing.Size(75, 23);
            this.ApplyConfig.TabIndex = 5;
            this.ApplyConfig.Text = "Apply";
            this.ApplyConfig.UseVisualStyleBackColor = true;
            this.ApplyConfig.Click += new System.EventHandler(this.ApplyConfig_Click);
            // 
            // Import
            // 
            this.Import.Location = new System.Drawing.Point(255, 415);
            this.Import.Name = "Import";
            this.Import.Size = new System.Drawing.Size(75, 23);
            this.Import.TabIndex = 6;
            this.Import.Text = "Import";
            this.Import.UseVisualStyleBackColor = true;
            this.Import.Click += new System.EventHandler(this.Import_Click);
            // 
            // ConfigurationUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 450);
            this.Controls.Add(this.Import);
            this.Controls.Add(this.ApplyConfig);
            this.Controls.Add(this.LoadConfig);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.settingTabs);
            this.Name = "ConfigurationUI";
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.ConfigurationUI_Load);
            this.settingTabs.ResumeLayout(false);
            this.fissionPage.ResumeLayout(false);
            this.fissionPage.PerformLayout();
            this.coolersPage.ResumeLayout(false);
            this.coolersPage.PerformLayout();
            this.fuelsPage.ResumeLayout(false);
            this.fuelsPage.PerformLayout();
            this.resourceCostsTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl settingTabs;
        private System.Windows.Forms.TabPage fissionPage;
        private System.Windows.Forms.TextBox neutronReach;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox moderatorExtraHeat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox moderatorExtraPower;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox heatGeneration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox power;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage coolersPage;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage fuelsPage;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox maxSize;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox minSize;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox fuelUse;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button LoadConfig;
        private System.Windows.Forms.Button ApplyConfig;
        private System.Windows.Forms.TabPage resourceCostsTab;
        private System.Windows.Forms.ComboBox blockSelector;
        private System.Windows.Forms.Button Import;
        private System.Windows.Forms.ToolTip ttValidation;
    }
}