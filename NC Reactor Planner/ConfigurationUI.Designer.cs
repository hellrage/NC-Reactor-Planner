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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationUI));
            this.settingTabs = new System.Windows.Forms.TabControl();
            this.Save = new System.Windows.Forms.Button();
            this.LoadConfig = new System.Windows.Forms.Button();
            this.ApplyConfig = new System.Windows.Forms.Button();
            this.Import = new System.Windows.Forms.Button();
            this.ttValidation = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // settingTabs
            // 
            this.settingTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingTabs.Location = new System.Drawing.Point(12, 12);
            this.settingTabs.Name = "settingTabs";
            this.settingTabs.SelectedIndex = 0;
            this.settingTabs.Size = new System.Drawing.Size(646, 395);
            this.settingTabs.TabIndex = 1;
            // 
            // Save
            // 
            this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.LoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.ApplyConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.Import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigurationUI";
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.ConfigurationUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl settingTabs;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button LoadConfig;
        private System.Windows.Forms.Button ApplyConfig;
        private System.Windows.Forms.Button Import;
        private System.Windows.Forms.ToolTip ttValidation;
    }
}