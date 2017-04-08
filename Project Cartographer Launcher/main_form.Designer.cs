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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(launcher_form));
            this.close_button = new System.Windows.Forms.Button();
            this.minimize_button = new System.Windows.Forms.Button();
            this.title_label = new System.Windows.Forms.Label();
            this.login_label = new System.Windows.Forms.Label();
            this.update_label = new System.Windows.Forms.Label();
            this.register_label = new System.Windows.Forms.Label();
            this.settings_label = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::Cartographer_Launcher.Properties.Resources.h2logo;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(121, 103);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(510, 70);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // launcher_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

