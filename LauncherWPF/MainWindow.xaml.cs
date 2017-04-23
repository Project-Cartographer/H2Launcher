using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Cartographer_Launcher.Includes;

namespace LauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Global Declares
        private string register_url = "http://www.cartographer.h2pc.org/";
        private bool LoginPanelCheck, SettingsPanelCheck, UpdatePanelCheck;
        BitmapImage TitleLogo = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/h2logo.png"));
        string TimeStamp = DateTime.Now.ToString("M/dd/yyyy (HH:mm)");
        private string LogFilePath = Globals.LogFile;
        Cartographer_Launcher.Includes.Settings.Launcher Launcher = new Cartographer_Launcher.Includes.Settings.Launcher();
        Cartographer_Launcher.Includes.Settings.ProjectCartographer Project = new Cartographer_Launcher.Includes.Settings.ProjectCartographer();

        //Disable ALT+Space shortcut for startmenu
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Space)
                e.Handled = true;
            else
                base.OnKeyDown(e);
        }
        //End Disable ALT+Space
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            
            LogFile("Log file initialized.");
            LogFile("Game install directory: " + Globals.GameDirectory);
            LogFile("Launcher file directory: " + Globals.H2vHubDirectory);

            this.LoginPanelCheck = false;
            this.SettingsPanelCheck = false;
            this.UpdatePanelCheck = false;


            //switch(Launcher.DisplayMode)
            //{
            //    case Cartographer_Launcher.Includes.Dependencies.SettingsDisplayMode.Fullscreen:
            //        {
            //            psFullScreen.IsChecked = true;
            //            psWindowed.IsChecked = false;
            //            psBorderless.IsChecked = false;
            //            break;
            //        }
            //    case Cartographer_Launcher.Includes.Dependencies.SettingsDisplayMode.Windowed:
            //        {
            //            psFullScreen.IsChecked = false;
            //            psWindowed.IsChecked = true;
            //            psBorderless.IsChecked = false;
            //            break;
            //        }
            //    case Cartographer_Launcher.Includes.Dependencies.SettingsDisplayMode.Borderless:
            //        {
            //            psFullScreen.IsChecked = false;
            //            psWindowed.IsChecked = true;
            //            psBorderless.IsChecked = true;
            //            break;
            //        }
            //}

            for (int s = 0; s < System.Windows.Forms.Screen.AllScreens.Length; s++)
            {
                psMonitorSelect.Items.Add((s + 1).ToString() + ((System.Windows.Forms.Screen.AllScreens[s].Primary) ? "*" : ""));
                if (s == Launcher.DefaultDisplay) psMonitorSelect.SelectedIndex = s;
            }
        }

        private void main_form_Initialized(object sender, EventArgs e)
        {
            if (File.Exists(LogFilePath))
                File.Delete(LogFilePath);
        }

        public void LogFile(string Message)
        {
            StreamWriter log;
            if (!File.Exists(LogFilePath))
                log = new StreamWriter(LogFilePath);
            else
                log = File.AppendText(LogFilePath);
            log.WriteLine("Date: " + TimeStamp + " - " + Message);
            log.Flush();
            log.Dispose();
            log.Close();
        }

        private void PanelAnimation(string Storyboard, StackPanel sp)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(sp);
        }

        private static bool IsTextAllowed(string numeric_text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(numeric_text);
        }

        private void SaveSettings()
        {
            Launcher.ResolutionHeight = int.Parse(this.psResY.Text);
            Launcher.ResolutionWidth = int.Parse(this.psResX.Text);
            //Launcher.DisplayMode = (Cartographer_Launcher.Includes.Dependencies.SettingsDisplayMode)Enum.Parse(typeof(Cartographer_Launcher.Includes.Dependencies.SettingsDisplayMode));
            Launcher.DefaultDisplay = this.psMonitorSelect.SelectedIndex;
            Launcher.VerticalSync = this.psVsync.IsChecked ? 1 : 0; //HAVING TROUBLE HERE

            Launcher.SaveSettings();
            Project.SaveSettings();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri("pack://application:,,,/Resources/Images/h2logo.png");
            b.EndInit();

            var image = sender as Image;
            image.Source = b;
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void NumericPasteCheck(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String numeric_text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(numeric_text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        private void main_form_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            main_grid.Focus();
        }

        private void control_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void control_minimize_Click(object sender, RoutedEventArgs e)
        {
            main_form.WindowState = WindowState.Minimized;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsPanelCheck && !UpdatePanelCheck)
            {
                if (LoginPanel.Margin.Top == -140)
                {
                    PanelAnimation("sbShowLoginMenu", LoginPanel);
                    LoginPanelCheck = true;
                }
                else if (LoginPanel.Margin.Top == 0)
                {
                    PanelAnimation("sbHideLoginMenu", LoginPanel);
                    LoginPanelCheck = false;
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LoginPanelCheck && !UpdatePanelCheck)
            {
                if (SettingPanel.Margin.Right == -220)
                {
                    PanelAnimation("sbShowSettingsMenu", SettingPanel);
                    SettingsPanelCheck = true;
                }
                else if (SettingPanel.Margin.Right == 0)
                {
                    PanelAnimation("sbHideSettingsMenu", SettingPanel);
                    SettingsPanelCheck = false;
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LoginPanelCheck && !SettingsPanelCheck)
            {
                if (UpdatePanel.Margin.Bottom == -250)
                {
                    PanelAnimation("sbShowUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = true;
                }
                else if (UpdatePanel.Margin.Bottom == 0)
                {
                    PanelAnimation("sbHideUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = false;
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(register_url);
        }

        private void main_form_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void psFullScreen_Checked(object sender, RoutedEventArgs e)
        {
            if (psWindowed.IsChecked == true || psBorderless.IsChecked == true)
            {
                psWindowed.IsChecked = false;
                psBorderless.IsChecked = false;
            }
            LogFile("Display Mode: Full Screen enabled.");
        }

        private void psFullScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            if (psWindowed.IsChecked == false)
                psFullScreen.IsChecked = true;
        }

        private void psBorderless_Checked(object sender, RoutedEventArgs e)
        {
            if (psFullScreen.IsChecked == true)
            {
                psFullScreen.IsChecked = false;
                psWindowed.IsChecked = true;
                psBorderless.IsChecked = true;
            }
            LogFile("Display Mode: Borderless window enabled.");
        }

        private void psWindowed_Checked(object sender, RoutedEventArgs e)
        {
            if (psFullScreen.IsChecked == true)
                psFullScreen.IsChecked = false;
            LogFile("Display Mode: Windowed enabled.");
        }

        private void psWindowed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (psBorderless.IsChecked == true)
                psBorderless.IsChecked = false;
            if (psBorderless.IsChecked == false && psFullScreen.IsChecked == false)
                psWindowed.IsChecked = true;
        }
    }
}
