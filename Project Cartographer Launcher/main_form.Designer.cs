namespace Cartographer_Launcher
{
    partial class launcher_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(launcher_form));
            this.close_button = new System.Windows.Forms.Button();
            this.minimize_button = new System.Windows.Forms.Button();
            this.title_label = new System.Windows.Forms.Label();
            this.login_label = new System.Windows.Forms.Label();
            this.update_label = new System.Windows.Forms.Label();
            this.register_label = new System.Windows.Forms.Label();
            this.settings_label = new System.Windows.Forms.Label();
            this.logo_picturebox = new System.Windows.Forms.PictureBox();
            this.settings_panel = new System.Windows.Forms.Panel();
            this.panel_slide = new System.Windows.Forms.Timer(this.components);
            this.sPanel_title_label = new System.Windows.Forms.Label();
            this.sPanel_setting1_label = new System.Windows.Forms.Label();
            this.sPanel_setting2_label = new System.Windows.Forms.Label();
            this.sPanel_setting4_label = new System.Windows.Forms.Label();
            this.sPanel_setting3_label = new System.Windows.Forms.Label();
            this.sPanel_setting5_label = new System.Windows.Forms.Label();
            this.sPanel_close_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logo_picturebox)).BeginInit();
            this.settings_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // close_button
            // 
            this.close_button.BackColor = System.Drawing.Color.Transparent;
            this.close_button.FlatAppearance.BorderSize = 0;
            this.close_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.close_button.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.close_button.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.close_button.Location = new System.Drawing.Point(704, 12);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(34, 34);
            this.close_button.TabIndex = 0;
            this.close_button.Text = "X";
            this.close_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.close_button.UseVisualStyleBackColor = false;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // minimize_button
            // 
            this.minimize_button.BackColor = System.Drawing.Color.Transparent;
            this.minimize_button.FlatAppearance.BorderSize = 0;
            this.minimize_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.minimize_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimize_button.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.minimize_button.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.minimize_button.Location = new System.Drawing.Point(664, 12);
            this.minimize_button.Name = "minimize_button";
            this.minimize_button.Size = new System.Drawing.Size(34, 34);
            this.minimize_button.TabIndex = 1;
            this.minimize_button.Text = "—";
            this.minimize_button.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.minimize_button.UseVisualStyleBackColor = false;
            this.minimize_button.Click += new System.EventHandler(this.minimize_button_Click);
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.BackColor = System.Drawing.Color.Transparent;
            this.title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.title_label.Location = new System.Drawing.Point(209, 185);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(169, 13);
            this.title_label.TabIndex = 3;
            this.title_label.Text = "PROJECT CARTOGRAPHER";
            // 
            // login_label
            // 
            this.login_label.AutoSize = true;
            this.login_label.BackColor = System.Drawing.Color.Transparent;
            this.login_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.login_label.Location = new System.Drawing.Point(342, 262);
            this.login_label.Name = "login_label";
            this.login_label.Size = new System.Drawing.Size(45, 13);
            this.login_label.TabIndex = 4;
            this.login_label.Text = "LOGIN";
            this.login_label.Click += new System.EventHandler(this.login_label_Click);
            // 
            // update_label
            // 
            this.update_label.AutoSize = true;
            this.update_label.BackColor = System.Drawing.Color.Transparent;
            this.update_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.update_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.update_label.Location = new System.Drawing.Point(277, 362);
            this.update_label.Name = "update_label";
            this.update_label.Size = new System.Drawing.Size(110, 13);
            this.update_label.TabIndex = 6;
            this.update_label.Text = "CHECK UPDATES";
            this.update_label.Click += new System.EventHandler(this.update_label_Click);
            // 
            // register_label
            // 
            this.register_label.AutoSize = true;
            this.register_label.BackColor = System.Drawing.Color.Transparent;
            this.register_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.register_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.register_label.Location = new System.Drawing.Point(317, 295);
            this.register_label.Name = "register_label";
            this.register_label.Size = new System.Drawing.Size(70, 13);
            this.register_label.TabIndex = 7;
            this.register_label.Text = "REGISTER";
            this.register_label.Click += new System.EventHandler(this.register_label_Click);
            // 
            // settings_label
            // 
            this.settings_label.AutoSize = true;
            this.settings_label.BackColor = System.Drawing.Color.Transparent;
            this.settings_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.settings_label.Location = new System.Drawing.Point(318, 329);
            this.settings_label.Name = "settings_label";
            this.settings_label.Size = new System.Drawing.Size(69, 13);
            this.settings_label.TabIndex = 8;
            this.settings_label.Text = "SETTINGS";
            this.settings_label.Click += new System.EventHandler(this.settings_label_Click);
            // 
            // logo_picturebox
            // 
            this.logo_picturebox.BackColor = System.Drawing.Color.Transparent;
            this.logo_picturebox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("logo_picturebox.BackgroundImage")));
            this.logo_picturebox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.logo_picturebox.Location = new System.Drawing.Point(121, 103);
            this.logo_picturebox.Name = "logo_picturebox";
            this.logo_picturebox.Size = new System.Drawing.Size(510, 70);
            this.logo_picturebox.TabIndex = 9;
            this.logo_picturebox.TabStop = false;
            // 
            // settings_panel
            // 
            this.settings_panel.BackColor = System.Drawing.Color.Transparent;
            this.settings_panel.BackgroundImage = global::Cartographer_Launcher.Properties.Resources.panel_opacity;
            this.settings_panel.Controls.Add(this.sPanel_close_label);
            this.settings_panel.Controls.Add(this.sPanel_setting5_label);
            this.settings_panel.Controls.Add(this.sPanel_setting4_label);
            this.settings_panel.Controls.Add(this.sPanel_setting3_label);
            this.settings_panel.Controls.Add(this.sPanel_setting2_label);
            this.settings_panel.Controls.Add(this.sPanel_setting1_label);
            this.settings_panel.Controls.Add(this.sPanel_title_label);
            this.settings_panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.settings_panel.Location = new System.Drawing.Point(500, 0);
            this.settings_panel.Name = "settings_panel";
            this.settings_panel.Size = new System.Drawing.Size(250, 450);
            this.settings_panel.TabIndex = 10;
            // 
            // panel_slide
            // 
            this.panel_slide.Interval = 10;
            this.panel_slide.Tick += new System.EventHandler(this.panel_slide_Tick);
            // 
            // sPanel_title_label
            // 
            this.sPanel_title_label.AutoSize = true;
            this.sPanel_title_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_title_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_title_label.Location = new System.Drawing.Point(15, 12);
            this.sPanel_title_label.Name = "sPanel_title_label";
            this.sPanel_title_label.Size = new System.Drawing.Size(69, 13);
            this.sPanel_title_label.TabIndex = 11;
            this.sPanel_title_label.Text = "SETTINGS";
            // 
            // sPanel_setting1_label
            // 
            this.sPanel_setting1_label.AutoSize = true;
            this.sPanel_setting1_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_setting1_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_setting1_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_setting1_label.Location = new System.Drawing.Point(30, 63);
            this.sPanel_setting1_label.Name = "sPanel_setting1_label";
            this.sPanel_setting1_label.Size = new System.Drawing.Size(73, 13);
            this.sPanel_setting1_label.TabIndex = 12;
            this.sPanel_setting1_label.Text = "NO SOUND";
            // 
            // sPanel_setting2_label
            // 
            this.sPanel_setting2_label.AutoSize = true;
            this.sPanel_setting2_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_setting2_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_setting2_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_setting2_label.Location = new System.Drawing.Point(30, 93);
            this.sPanel_setting2_label.Name = "sPanel_setting2_label";
            this.sPanel_setting2_label.Size = new System.Drawing.Size(104, 13);
            this.sPanel_setting2_label.TabIndex = 13;
            this.sPanel_setting2_label.Text = "VERTICAL SYNC";
            // 
            // sPanel_setting4_label
            // 
            this.sPanel_setting4_label.AutoSize = true;
            this.sPanel_setting4_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_setting4_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_setting4_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_setting4_label.Location = new System.Drawing.Point(30, 157);
            this.sPanel_setting4_label.Name = "sPanel_setting4_label";
            this.sPanel_setting4_label.Size = new System.Drawing.Size(119, 13);
            this.sPanel_setting4_label.TabIndex = 15;
            this.sPanel_setting4_label.Text = "WINDOWED MODE";
            // 
            // sPanel_setting3_label
            // 
            this.sPanel_setting3_label.AutoSize = true;
            this.sPanel_setting3_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_setting3_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_setting3_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_setting3_label.Location = new System.Drawing.Point(30, 125);
            this.sPanel_setting3_label.Name = "sPanel_setting3_label";
            this.sPanel_setting3_label.Size = new System.Drawing.Size(116, 13);
            this.sPanel_setting3_label.TabIndex = 14;
            this.sPanel_setting3_label.Text = "STARTUP MOVIES";
            // 
            // sPanel_setting5_label
            // 
            this.sPanel_setting5_label.AutoSize = true;
            this.sPanel_setting5_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_setting5_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_setting5_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_setting5_label.Location = new System.Drawing.Point(30, 187);
            this.sPanel_setting5_label.Name = "sPanel_setting5_label";
            this.sPanel_setting5_label.Size = new System.Drawing.Size(119, 13);
            this.sPanel_setting5_label.TabIndex = 16;
            this.sPanel_setting5_label.Text = "DEFAULT DISPLAY";
            // 
            // sPanel_close_label
            // 
            this.sPanel_close_label.AutoSize = true;
            this.sPanel_close_label.BackColor = System.Drawing.Color.Transparent;
            this.sPanel_close_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sPanel_close_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.sPanel_close_label.Location = new System.Drawing.Point(221, 3);
            this.sPanel_close_label.Name = "sPanel_close_label";
            this.sPanel_close_label.Size = new System.Drawing.Size(19, 13);
            this.sPanel_close_label.TabIndex = 17;
            this.sPanel_close_label.Text = "➔";
            this.sPanel_close_label.Click += new System.EventHandler(this.sPanel_close_label_Click);
            // 
            // launcher_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.ControlBox = false;
            this.Controls.Add(this.settings_panel);
            this.Controls.Add(this.logo_picturebox);
            this.Controls.Add(this.settings_label);
            this.Controls.Add(this.register_label);
            this.Controls.Add(this.update_label);
            this.Controls.Add(this.login_label);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.minimize_button);
            this.Controls.Add(this.close_button);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "launcher_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.main_form_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.logo_picturebox)).EndInit();
            this.settings_panel.ResumeLayout(false);
            this.settings_panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button close_button;
        private System.Windows.Forms.Button minimize_button;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label login_label;
        private System.Windows.Forms.Label update_label;
        private System.Windows.Forms.Label register_label;
        private System.Windows.Forms.Label settings_label;
        private System.Windows.Forms.PictureBox logo_picturebox;
        private System.Windows.Forms.Panel settings_panel;
        private System.Windows.Forms.Timer panel_slide;
        private System.Windows.Forms.Label sPanel_title_label;
        private System.Windows.Forms.Label sPanel_setting5_label;
        private System.Windows.Forms.Label sPanel_setting4_label;
        private System.Windows.Forms.Label sPanel_setting3_label;
        private System.Windows.Forms.Label sPanel_setting2_label;
        private System.Windows.Forms.Label sPanel_setting1_label;
        private System.Windows.Forms.Label sPanel_close_label;
    }
}

