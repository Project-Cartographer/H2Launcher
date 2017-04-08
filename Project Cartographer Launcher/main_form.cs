using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//non default dependencies
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Diagnostics;

namespace Cartographer_Launcher
{
    public partial class launcher_form : Form
    {
        #region Dependencies
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WS_MINIMIZEBOX = 0x20000;
        public const int CS_DBLCLKS = 0x8;

        public delegate void MouseMovedEvent();
        private bool login_check;
        private bool register_check;
        private bool settings_check;
        private bool update_check;
        private bool sPanel_check;

        FontFamily ff;
        Font font;

        Color tt = Color.FromArgb(193, 218, 248); //#C1DAF8
        Color bt = Color.FromArgb(184, 205, 224); //#B8CDE0
        Color bb_hover = Color.FromArgb(33, 63, 132); //#213F84
        Color bb_click = Color.FromArgb(46, 83, 166); //#2E53A6
        Color mt = Color.FromArgb(94, 109, 139); //#5E6D8B
        Color mt_hover = Color.FromArgb(126, 147, 178); //#7E93B2
        Color mt_click = Color.FromArgb(178, 211, 246); //#B2D3F6
        //Color pn = Color.FromArgb(0, 12, 45); //#000C2D

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                parameters.Style |= WS_MINIMIZEBOX;
                parameters.ClassStyle |= CS_DBLCLKS;
                parameters.ExStyle = parameters.ExStyle | 0x2000000; //Prevent flickering
                return parameters;
            }
        }

        public class GlobalMouseHandler : IMessageFilter
        {
            private const int WM_MOUSEMOVE = 0x0200;
            public event MouseMovedEvent MouseMovement;

            #region IMessageFilter Members      
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEMOVE)
                    if (MouseMovement != null)
                        MouseMovement();
                return false;
            }
            #endregion
        }
        #endregion

        public launcher_form()
        {
            GlobalMouseHandler gmh = new GlobalMouseHandler();
            gmh.MouseMovement += new MouseMovedEvent(CursorPositionStyleSheet);
            Application.AddMessageFilter(gmh);

            InitializeComponent();
            HandelGothicMedium();
            StyleSheet();
            CursorPositionStyleSheet();

            login_check = false;
            register_check = false;
            settings_check = false;
            update_check = false;
            sPanel_check = false;

            this.settings_panel.Width = 0;
        }

        private void CursorPositionStyleSheet()
        {
            Point cur_pos = Cursor.Position;
            var rel_pos = this.PointToClient(cur_pos);

            #region Login
            if (rel_pos.X >= login_label.Location.X
                && rel_pos.X <= login_label.Location.X + login_label.Size.Width
                && rel_pos.Y >= login_label.Location.Y
                && rel_pos.Y <= login_label.Location.Y + login_label.Size.Height)
            {
                if (!login_check)
                    this.login_label.ForeColor = mt_hover;
                else
                    this.login_label.ForeColor = mt_click;
            }
            else
            {
                this.login_label.ForeColor = mt;
                login_check = false;
            }
            #endregion

            #region Register
            if (rel_pos.X >= register_label.Location.X
                && rel_pos.X <= register_label.Location.X + register_label.Size.Width
                && rel_pos.Y >= register_label.Location.Y
                && rel_pos.Y <= register_label.Location.Y + register_label.Size.Height)
            {
                if (!register_check)
                    this.register_label.ForeColor = mt_hover;
                else
                    this.register_label.ForeColor = mt_click;
            }
            else
            {
                this.register_label.ForeColor = mt;
                register_check = false;
            }
            #endregion

            #region Settings
            if (rel_pos.X >= settings_label.Location.X
                && rel_pos.X <= settings_label.Location.X + settings_label.Size.Width
                && rel_pos.Y >= settings_label.Location.Y
                && rel_pos.Y <= settings_label.Location.Y + settings_label.Size.Height)
            {
                if (!settings_check)
                    this.settings_label.ForeColor = mt_hover;
                else
                    this.settings_label.ForeColor = mt_click;
            }
            else
            {
                this.settings_label.ForeColor = mt;
                settings_check = false;
            }
            #endregion

            #region Check Updates
            if (rel_pos.X >= update_label.Location.X
                && rel_pos.X <= update_label.Location.X + update_label.Size.Width
                && rel_pos.Y >= update_label.Location.Y
                && rel_pos.Y <= update_label.Location.Y + update_label.Size.Height)
            {
                if (!update_check)
                    this.update_label.ForeColor = mt_hover;
                else
                    this.update_label.ForeColor = mt_click;
            }
            else
            {
                this.update_label.ForeColor = mt;
                update_check = false;
            }
            #endregion
        }

        private void HandelGothicMedium()
        {

            byte[] fontArray = Cartographer_Launcher.Properties.Resources.handel_gothic_medium;
            int dataLength = Cartographer_Launcher.Properties.Resources.handel_gothic_medium.Length;

            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);

            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);

            Marshal.FreeCoTaskMem(ptrData);
            ff = pfc.Families[0];
            font = new Font(ff, 15f, FontStyle.Bold);
        }


        public void StyleSheet()
        {
            //Transparency Fix
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            //End Transparency Fix

            this.minimize_button.ForeColor = bt;
            this.minimize_button.FlatAppearance.MouseOverBackColor = bb_hover;
            this.minimize_button.FlatAppearance.MouseDownBackColor = bb_click;

            this.close_button.ForeColor = bt;
            this.close_button.FlatAppearance.MouseOverBackColor = bb_hover;
            this.close_button.FlatAppearance.MouseDownBackColor = bb_click;

            this.title_label.Font = new Font(ff, 20);
            this.title_label.ForeColor = tt;

            this.login_label.Font = new Font(ff, 18);
            this.login_label.ForeColor = mt;

            this.register_label.Font = new Font(ff, 18);
            this.register_label.ForeColor = mt;

            this.settings_label.Font = new Font(ff, 18);
            this.settings_label.ForeColor = mt;

            this.update_label.Font = new Font(ff, 18);
            this.update_label.ForeColor = mt;

            #region Panel Settings
            this.sPanel_title_label.Font = new Font(ff, 20);
            this.sPanel_title_label.ForeColor = tt;
            this.sPanel_close_label.Font = new Font(ff, 28);
            this.sPanel_close_label.ForeColor = tt;
            this.sPanel_setting1_label.Font = new Font(ff, 14);
            this.sPanel_setting1_label.ForeColor = mt;
            this.sPanel_setting2_label.Font = new Font(ff, 14);
            this.sPanel_setting2_label.ForeColor = mt;
            this.sPanel_setting3_label.Font = new Font(ff, 14);
            this.sPanel_setting3_label.ForeColor = mt;
            this.sPanel_setting4_label.Font = new Font(ff, 14);
            this.sPanel_setting4_label.ForeColor = mt;
            this.sPanel_setting5_label.Font = new Font(ff, 14);
            this.sPanel_setting5_label.ForeColor = mt;
            #endregion
        }

        #region Event Handlers
        private void main_form_MouseDown(object sender, MouseEventArgs e)
        {
            //Handles mouse down event for left click to allow form to be moved
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void login_label_Click(object sender, EventArgs e)
        {
            login_check = true;
        }

        private void register_label_Click(object sender, EventArgs e)
        {
            register_check = true;
            Process.Start("http://www.cartographer.h2pc.org/");
        }

        private void settings_label_Click(object sender, EventArgs e)
        {
            settings_check = true;
            panel_slide.Enabled = true;
            panel_slide.Start();
        }

        private void update_label_Click(object sender, EventArgs e)
        {
            update_check = true;
        }

        private void sPanel_close_label_Click(object sender, EventArgs e)
        {
            settings_check = true;
            panel_slide.Enabled = true;
            panel_slide.Start();
        }
        #endregion

        private void panel_slide_Tick(object sender, EventArgs e)
        {
            if (!sPanel_check)
            {
                if (settings_panel.Width >= 250)
                {
                    panel_slide.Stop();
                    panel_slide.Enabled = false;
                    sPanel_check = true;
                }
                else
                {
                    settings_panel.Width += 70;
                    this.Refresh();
                }
            }
            else
            {
                if (settings_panel.Width == 0)
                {
                    panel_slide.Stop();
                    panel_slide.Enabled = false;
                    sPanel_check = false;
                }
                else
                    settings_panel.Width -= 60;
            }
        }

    }
}
