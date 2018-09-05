namespace NC_Reactor_Planner
{
    partial class ModValueSettings
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
            this.settingTabs = new System.Windows.Forms.TabControl();
            this.generalPage = new System.Windows.Forms.TabPage();
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
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.fuelsPage = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SaveSettings = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.settingTabs.SuspendLayout();
            this.generalPage.SuspendLayout();
            this.coolersPage.SuspendLayout();
            this.fuelsPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingTabs
            // 
            this.settingTabs.Controls.Add(this.generalPage);
            this.settingTabs.Controls.Add(this.coolersPage);
            this.settingTabs.Controls.Add(this.fuelsPage);
            this.settingTabs.Location = new System.Drawing.Point(3, 3);
            this.settingTabs.Name = "settingTabs";
            this.settingTabs.SelectedIndex = 0;
            this.settingTabs.Size = new System.Drawing.Size(519, 411);
            this.settingTabs.TabIndex = 0;
            // 
            // generalPage
            // 
            this.generalPage.Controls.Add(this.neutronReach);
            this.generalPage.Controls.Add(this.label6);
            this.generalPage.Controls.Add(this.moderatorExtraHeat);
            this.generalPage.Controls.Add(this.label5);
            this.generalPage.Controls.Add(this.moderatorExtraPower);
            this.generalPage.Controls.Add(this.label4);
            this.generalPage.Controls.Add(this.heatGeneration);
            this.generalPage.Controls.Add(this.label3);
            this.generalPage.Controls.Add(this.fuelUse);
            this.generalPage.Controls.Add(this.label2);
            this.generalPage.Controls.Add(this.power);
            this.generalPage.Controls.Add(this.label1);
            this.generalPage.Location = new System.Drawing.Point(4, 22);
            this.generalPage.Name = "generalPage";
            this.generalPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalPage.Size = new System.Drawing.Size(511, 385);
            this.generalPage.TabIndex = 0;
            this.generalPage.Text = "General";
            this.generalPage.UseVisualStyleBackColor = true;
            // 
            // neutronReach
            // 
            this.neutronReach.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "NeutronReach", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.neutronReach.Location = new System.Drawing.Point(10, 224);
            this.neutronReach.Name = "neutronReach";
            this.neutronReach.Size = new System.Drawing.Size(174, 20);
            this.neutronReach.TabIndex = 6;
            this.neutronReach.Text = global::NC_Reactor_Planner.Properties.Settings.Default.NeutronReach;
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
            this.moderatorExtraHeat.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "ModeratorExtraHeat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.moderatorExtraHeat.Location = new System.Drawing.Point(10, 184);
            this.moderatorExtraHeat.Name = "moderatorExtraHeat";
            this.moderatorExtraHeat.Size = new System.Drawing.Size(174, 20);
            this.moderatorExtraHeat.TabIndex = 5;
            this.moderatorExtraHeat.Text = global::NC_Reactor_Planner.Properties.Settings.Default.ModeratorExtraHeat;
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
            this.moderatorExtraPower.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "ModeratorExtraPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.moderatorExtraPower.Location = new System.Drawing.Point(10, 144);
            this.moderatorExtraPower.Name = "moderatorExtraPower";
            this.moderatorExtraPower.Size = new System.Drawing.Size(174, 20);
            this.moderatorExtraPower.TabIndex = 4;
            this.moderatorExtraPower.Text = global::NC_Reactor_Planner.Properties.Settings.Default.ModeratorExtraPower;
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
            this.heatGeneration.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "HeatGeneration", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.heatGeneration.Location = new System.Drawing.Point(10, 104);
            this.heatGeneration.Name = "heatGeneration";
            this.heatGeneration.Size = new System.Drawing.Size(174, 20);
            this.heatGeneration.TabIndex = 3;
            this.heatGeneration.Text = global::NC_Reactor_Planner.Properties.Settings.Default.HeatGeneration;
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
            this.fuelUse.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "FuelUse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.fuelUse.Location = new System.Drawing.Point(10, 64);
            this.fuelUse.Name = "fuelUse";
            this.fuelUse.Size = new System.Drawing.Size(174, 20);
            this.fuelUse.TabIndex = 2;
            this.fuelUse.Text = global::NC_Reactor_Planner.Properties.Settings.Default.FuelUse;
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
            this.power.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NC_Reactor_Planner.Properties.Settings.Default, "FIssionPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.power.Location = new System.Drawing.Point(10, 24);
            this.power.Name = "power";
            this.power.Size = new System.Drawing.Size(174, 20);
            this.power.TabIndex = 1;
            this.power.Text = global::NC_Reactor_Planner.Properties.Settings.Default.FissionPower;
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
            this.coolersPage.Controls.Add(this.label8);
            this.coolersPage.Controls.Add(this.label7);
            this.coolersPage.Location = new System.Drawing.Point(4, 22);
            this.coolersPage.Name = "coolersPage";
            this.coolersPage.Padding = new System.Windows.Forms.Padding(3);
            this.coolersPage.Size = new System.Drawing.Size(511, 385);
            this.coolersPage.TabIndex = 1;
            this.coolersPage.Text = "Coolers";
            this.coolersPage.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(200, 3);
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
            this.fuelsPage.Padding = new System.Windows.Forms.Padding(3);
            this.fuelsPage.Size = new System.Drawing.Size(511, 385);
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
            // SaveSettings
            // 
            this.SaveSettings.Location = new System.Drawing.Point(419, 420);
            this.SaveSettings.Name = "SaveSettings";
            this.SaveSettings.Size = new System.Drawing.Size(99, 23);
            this.SaveSettings.TabIndex = 1;
            this.SaveSettings.Text = "Save settings";
            this.SaveSettings.UseVisualStyleBackColor = true;
            this.SaveSettings.Click += new System.EventHandler(this.SaveSettings_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(12, 425);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(349, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Settings are saved and applied when you close this window!";
            // 
            // ModValueSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 450);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.SaveSettings);
            this.Controls.Add(this.settingTabs);
            this.Name = "ModValueSettings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModValueSettings_FormClosing);
            this.settingTabs.ResumeLayout(false);
            this.generalPage.ResumeLayout(false);
            this.generalPage.PerformLayout();
            this.coolersPage.ResumeLayout(false);
            this.coolersPage.PerformLayout();
            this.fuelsPage.ResumeLayout(false);
            this.fuelsPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl settingTabs;
        private System.Windows.Forms.TabPage generalPage;
        private System.Windows.Forms.TabPage coolersPage;
        private System.Windows.Forms.TabPage fuelsPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveSettings;
        private System.Windows.Forms.TextBox neutronReach;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox moderatorExtraHeat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox moderatorExtraPower;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox heatGeneration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fuelUse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox power;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
    }
}