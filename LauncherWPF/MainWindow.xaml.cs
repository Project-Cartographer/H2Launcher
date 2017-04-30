using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Cartographer_Launcher.Includes;
using Cartographer_Launcher.Includes.Dependencies;
using Cartographer_Launcher.Includes.Settings;
using H2Shield.Includes;

namespace LauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Global Declares
        private string RegisterURL = @"http://www.cartographer.h2pc.org/";
        private string AppealURL = @"http://www.halo2vista.com/forums/viewforum.php?f=45";
        private string DateTimeStamp = DateTime.Now.ToString("M/dd/yyyy (HH:mm)");
        private string LogFilePath = Globals.LogFile;
        private string ExLogFilePath = Globals.ExLogFile;
        private string DefaultDisplay;
        private bool LoginPanelCheck, SettingsPanelCheck, UpdatePanelCheck, Vsync, GameSound, DebugLog, VoiceChat, MapDownloading, fpsEnable, RememberMe, StartupCredits;
        private static bool NoTextInput(string NumericText)
        {
            Regex r = new Regex("[^0-9.-]+");
            return !r.IsMatch(NumericText);
        }

        BitmapImage TitleLogo = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/h2logo.png"));
        SolidColorBrush MenuItemSelect = new SolidColorBrush(Color.FromRgb(178, 211, 246));
        SolidColorBrush MenuItem = new SolidColorBrush(Color.FromRgb(94, 109, 139));
        Launcher LauncherSettings = new Launcher();
        ProjectCartographer ProjectSettings = new ProjectCartographer();

        //Disable ALT+Space shortcut for startmenu
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Space) e.Handled = true;
            else base.OnKeyDown(e);
        }
        //End Disable ALT+Space
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            LogFile("Log file initialized.");
            LogFile("Game install directory: " + Globals.GameDirectory);
            LogFile("Launcher file directory: " + Globals.H2vHubDirectory);

            LoginPanelCheck = false;
            SettingsPanelCheck = false;
            UpdatePanelCheck = false;
            usProgressLabel.Tag = "{0}/100";
            usProgressLabel.Content = "100/100";
            try
            {
                LauncherSettings.LoadSettings();
                ProjectSettings.LoadSettings();
                LoadSettings();
            }
            catch (Exception Ex) { ExLogFile(Ex.ToString()); }
        }

        private void main_form_Initialized(object sender, EventArgs e)
        {
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
            if (File.Exists(ExLogFilePath)) File.Delete(ExLogFilePath);
        }

        private void main_form_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            main_grid.Focus();
        }

        private void LoginVerification()
        {
            SaveSettings();
            var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);
            if (loginResult.LoginResultEnum != LoginResultEnum.Successfull)
            {
                lsUser.Text = "";
                lsPass.Password = "";
            }

            switch (loginResult.LoginResultEnum)
            {
                case LoginResultEnum.Successfull:
                    {
                        LauncherSettings.PlayerTag = lsUser.Text;
                        ProjectSettings.LoginToken = loginResult.LoginToken;
                        LauncherRuntime.StartHalo(lsUser.Text, loginResult.LoginToken, this);
                        LogFile("Login successful, game starting...");
                        break;
                    }
                case LoginResultEnum.InvalidLoginToken:
                    {
                        MessageBox.Show(this, "This login token is no longer valid.\r\nPlease re-enter your login information and try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
                        ProjectSettings.LoginToken = "";
                        LogFile("Login Token was invalid");
                        break;
                    }
                case LoginResultEnum.InvalidUsernameOrPassword:
                    {
                        MessageBox.Show(this, "The playertag or password entered is invalid.\r\nPlease try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
                        LogFile("Playertag or the entered password was incorrect.");
                        break;
                    }
                case LoginResultEnum.Banned:
                    {
                        if (MessageBox.Show(this, "You have been banned, please visit the forum to appeal your ban.\r\nWould you like us to open the forums for you?.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            Process.Start(AppealURL);
                        LogFile("This machine is banned. Please seek help on the discord or forum");
                        break;
                    }
                case LoginResultEnum.GenericFailure:
                    {
                        if (LoginPanel.Margin.Top == -140)
                        {
                            PanelAnimation("sbShowLoginMenu", LoginPanel);
                            LoginPanelCheck = true;
                        }
                        LogFile("General login failure, please try again");
                        break;
                    }
            }
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

        public void LogFile(string Message)
        {
            StreamWriter log;
            if (!File.Exists(LogFilePath)) log = new StreamWriter(LogFilePath);
            else log = File.AppendText(LogFilePath);

            log.WriteLine("Date: " + DateTimeStamp + " - " + Message);

            log.Flush();
            log.Dispose();
            log.Close();
        }

        public void ExLogFile(string Message)
        {
            StreamWriter exlog;
            if (!File.Exists(LogFilePath)) exlog = new StreamWriter(ExLogFilePath);
            else exlog = File.AppendText(ExLogFilePath);

            exlog.WriteLine("Date: " + DateTimeStamp);
            exlog.WriteLine(Message);
            exlog.WriteLine("/r/n");

            exlog.Flush();
            exlog.Dispose();
            exlog.Close();
        }

        private void PanelAnimation(string Storyboard, StackPanel sp)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(sp);
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !NoTextInput(e.Text);
        }

        private void NumericPasteCheck(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String numeric_text = (String)e.DataObject.GetData(typeof(String));
                if (!NoTextInput(numeric_text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        private void control_minimize_Click(object sender, RoutedEventArgs e)
        {
            main_form.WindowState = WindowState.Minimized;
        }

        private void control_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (lsPass.Password == "")
            {
                if (!SettingsPanelCheck && !UpdatePanelCheck && !LoginPanelCheck)
                {
                    if (LoginPanel.Margin.Top == -140)
                    {
                        PanelAnimation("sbShowLoginMenu", LoginPanel);
                        LoginPanelCheck = true;
                    }
                    if (LoginPanel.Margin.Top == 0)
                    {
                        PanelAnimation("sbHideLoginMenu", LoginPanel);
                        LoginPanelCheck = false;
                    }
                }
            }
            if (!SettingsPanelCheck && !UpdatePanelCheck && !LoginPanelCheck) LoginVerification();
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
            Process.Start(RegisterURL);
        }

        private void PanelClose_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsPanelCheck)
            {
                if (SettingPanel.Margin.Right == 0)
                {
                    PanelAnimation("sbHideSettingsMenu", SettingPanel);
                    SettingsPanelCheck = false;
                }
            }
            else if (UpdatePanelCheck)
            {
                if (UpdatePanel.Margin.Bottom == 0)
                {
                    PanelAnimation("sbHideUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = false;
                }
            }
            else if (LoginPanelCheck)
            {
                if (LoginPanel.Margin.Top == 0)
                {
                    PanelAnimation("sbHideLoginMenu", LoginPanel);
                    LoginPanelCheck = false;
                }
            }
        }

        #region Settings Panel
        private void LoadSettings()
        {
            //
            //Login
            //
            if (LauncherSettings.RememberMe == 1)
            {
                lsRememberMe.IsChecked = true;
                lsUser.Text = LauncherSettings.PlayerTag;
            }
            else
            {
                lsRememberMe.IsChecked = false;
                lsUser.Text = "";
                ProjectSettings.LoginToken = "";
            }
            if (ProjectSettings.LoginToken != "")
            {
                if (LauncherSettings.PlayerTag != "") lsPass.Password = "PASSWORDHOLDER";
                else ProjectSettings.LoginToken = "";
            }
            //
            //Display Mode
            //
            switch (LauncherSettings.DisplayMode)
            {
                case SettingsDisplayMode.Fullscreen:
                    {
                        psFullScreen.IsChecked = true;
                        psWindowed.IsChecked = false;
                        psBorderless.IsChecked = false;
                        break;
                    }
                case SettingsDisplayMode.Windowed:
                    {
                        psFullScreen.IsChecked = false;
                        psWindowed.IsChecked = true;
                        psBorderless.IsChecked = false;
                        break;
                    }
                case SettingsDisplayMode.Borderless:
                    {
                        psFullScreen.IsChecked = false;
                        psWindowed.IsChecked = true;
                        psBorderless.IsChecked = true;
                        break;
                    }
            }
            //
            //Custom Resolution :: Width
            //
            if (LauncherSettings.ResolutionWidth.ToString() != null)
            {
                psResX.Text = LauncherSettings.ResolutionWidth.ToString();
                psResx.IsChecked = true;
                psResX.Foreground = MenuItemSelect;
            }
            else psResx.IsChecked = false;
            //
            //Custom Resolution :: Height
            //
            if (LauncherSettings.ResolutionHeight.ToString() != null)
            {
                psResY.Text = LauncherSettings.ResolutionHeight.ToString();
                psResy.IsChecked = true;
                psResY.Foreground = MenuItemSelect;
            }
            else psResx.IsChecked = false;
            //
            //Ports
            //
            psPorts.IsChecked = true;
            psPortNumber.Foreground = MenuItemSelect;
            psPortNumber.Text = ProjectSettings.Ports.ToString();
            //
            //Default Display
            //
            for (int s = 0; s < System.Windows.Forms.Screen.AllScreens.Length; s++)
            {
                psMonitorSelect.Items.Add((s + 1).ToString() + ((System.Windows.Forms.Screen.AllScreens[s].Primary) ? "*" : ""));
                if (s == LauncherSettings.DefaultDisplay) psMonitorSelect.SelectedIndex = s;
            }
            //
            //Vertical Sync
            //
            if (LauncherSettings.VerticalSync == 1) psVsync.IsChecked = true;
            else psVsync.IsChecked = false;
            //
            //Game Sound
            //
            if (LauncherSettings.GameSound == 1) psSound.IsChecked = true;
            else psSound.IsChecked = false;
            //
            //Debug Log
            //
            if (ProjectSettings.DebugLog == 1) psDebug.IsChecked = true;
            else psDebug.IsChecked = false;
            //
            //Map Downloading
            //
            if (ProjectSettings.MapDownload == 1) psMaps.IsChecked = true;
            else psMaps.IsChecked = false;
            //
            //FPS
            //
            if (ProjectSettings.FPSCap == 1) psFPS.IsChecked = true;
            psFPSLimit.Text = ProjectSettings.FPSLimit.ToString();
            //
            //Default Display
            //
            psMonitor.IsChecked = true;
            //
            //Voice Chat
            //
            if (ProjectSettings.VoiceChat == 1) psVoice.IsChecked = true;
            else psVoice.IsChecked = false;
            //
            //Startup Credits
            //
            if (LauncherSettings.StartupCredits == 1) psIntroMovies.IsChecked = true;
            else psIntroMovies.IsChecked = false;
        }

        public void SaveSettings()
        {
            if (lsUser.Text == "") LauncherSettings.RememberMe = 0;
            else LauncherSettings.RememberMe = (RememberMe) ? 1 : 0;

            LauncherSettings.PlayerTag = lsUser.Text;
            LauncherSettings.ResolutionHeight = int.Parse(psResY.Text);
            LauncherSettings.ResolutionWidth = int.Parse(psResX.Text);

            LauncherSettings.DisplayMode = (SettingsDisplayMode)Enum.Parse(typeof(SettingsDisplayMode), DefaultDisplay.ToString());
            LauncherSettings.DefaultDisplay = psMonitorSelect.SelectedIndex;
            LauncherSettings.VerticalSync = (Vsync) ? 1 : 0;
            LauncherSettings.GameSound = (GameSound) ? 1 : 0;
            LauncherSettings.StartupCredits = (StartupCredits) ? 1 : 0;
            ProjectSettings.Ports = int.Parse(psPortNumber.Text);
            ProjectSettings.FPSLimit = int.Parse(psFPSLimit.Text);
            ProjectSettings.DebugLog = (DebugLog) ? 1 : 0;
            ProjectSettings.FPSCap = (fpsEnable) ? 1 : 0;
            ProjectSettings.VoiceChat = (VoiceChat) ? 1 : 0;
            ProjectSettings.MapDownload = (MapDownloading) ? 1 : 0;

            if (psIntroMovies.IsChecked == false)
            {
                if (!Directory.Exists(Globals.GameDirectory + "\\movie.bak"))
                {
                    Directory.Move(Globals.GameDirectory + "\\movie", Globals.GameDirectory + "\\movie.bak");
                    Directory.CreateDirectory(Globals.GameDirectory + "\\movie");
                    File.Create(Globals.GameDirectory + "\\movie\\credits_60.wmv").Close();
                    File.Create(Globals.GameDirectory + "\\movie\\intro_60.wmv").Close();
                    File.Create(Globals.GameDirectory + "\\movie\\intro_low_60.wmv").Close();
                }
            }
            else
            {
                if (Directory.Exists(Globals.GameDirectory + "\\movie.bak"))
                {
                    Directory.Delete(Globals.GameDirectory + "\\movie", true);
                    Directory.Move(Globals.GameDirectory + "\\movie.bak", Globals.GameDirectory + "\\movie");
                }
            }

            LauncherSettings.SaveSettings();
            ProjectSettings.SaveSettings();
            LogFile("Settings saved");
        }

        private void lsRememberMe_Checked(object sender, RoutedEventArgs e)
        {
            RememberMe = true;
            LogFile("Autologin enabled.");
        }

        private void lsRememberMe_Unchecked(object sender, RoutedEventArgs e)
        {
            RememberMe = false;
            LogFile("Autologin disabled.");
        }

        private void psChangePlayer_Checked(object sender, RoutedEventArgs e)
        {
            if (SettingPanel.Margin.Right == 0)
            {
                PanelAnimation("sbHideSettingsMenu", SettingPanel);
                SettingsPanelCheck = false;
            }

            if (!SettingsPanelCheck && !UpdatePanelCheck)
            {
                if (LoginPanel.Margin.Top == -140)
                {
                    PanelAnimation("sbShowLoginMenu", LoginPanel);
                    LoginPanelCheck = true;
                }
            }
            psChangePlayer.IsChecked = false;
        }

        private void psChangePlayer_Unchecked(object sender, RoutedEventArgs e)
        {
            psChangePlayer.IsChecked = false;
        }

        private void psVsync_Checked(object sender, RoutedEventArgs e)
        {
            Vsync = true;
            LogFile("Halo 2 Launch Parameter: -novsync removed from game launch.");
        }

        private void psVsync_Unchecked(object sender, RoutedEventArgs e)
        {
            Vsync = false;
            LogFile("Halo 2 Launch Parameter: -novsync added to game launch.");
        }

        private void psSound_Checked(object sender, RoutedEventArgs e)
        {
            GameSound = true;
            LogFile("Halo 2 Launch Parameter: -nosound removed from game launch.");
        }

        private void psSound_Unchecked(object sender, RoutedEventArgs e)
        {
            GameSound = false;
            LogFile("Halo 2 Launch Parameter: -nosound added to game launch.");
        }

        private void psDebug_Checked(object sender, RoutedEventArgs e)
        {
            DebugLog = true;
            LogFile("Project Cartographer: Trace logs enabled.");
        }

        private void psDebug_Unchecked(object sender, RoutedEventArgs e)
        {
            DebugLog = false;
            LogFile("Project Cartographer: Trace logs disabled.");
        }

        private void psFPS_Checked(object sender, RoutedEventArgs e)
        {
            fpsEnable = true;
            psFPSLimit.IsEnabled = true;
            psFPSLimit.Foreground = MenuItemSelect;
            LogFile("Project Cartographer: FPS limiter enabled.");
        }

        private void psFPS_Unchecked(object sender, RoutedEventArgs e)
        {
            fpsEnable = false;
            psFPSLimit.IsEnabled = false;
            LogFile("Project Cartographer: FPS limiter disabled.");
        }

        private void psMaps_Checked(object sender, RoutedEventArgs e)
        {
            MapDownloading = true;
            LogFile("Project Cartographer: Custom map downloading enabled.");
        }

        private void psPorts_Unchecked(object sender, RoutedEventArgs e)
        {
            psPorts.IsChecked = true;
        }

        private void psPortNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogFile("Project Cartographer: Game base port changed to " + psPortNumber.Text.ToString());
        }

        private void psFPSLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogFile("Project Cartographer: Maximum frames allowed changed to " + psFPSLimit.Text.ToString());
        }

        private void psMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogFile("Halo 2 Launch Parameter: -monitor:" + psMonitorSelect.SelectedIndex.ToString());
        }

        private void psMonitor_Unchecked(object sender, RoutedEventArgs e)
        {
            psMonitor.IsChecked = true;
        }

        private void lsUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lsUser.Text != LauncherSettings.PlayerTag)
            {
                lsPass.Password = "";
                lsRememberMe.IsChecked = false;
            }
        }

        private void psRes_Unchecked(object sender, RoutedEventArgs e)
        {
            psResx.IsChecked = true;
            psResy.IsChecked = true;
        }

        private void psIntroMovies_Unchecked(object sender, RoutedEventArgs e)
        {
            StartupCredits = false;
            LogFile("Launcher: Startup Credits disabled");
        }

        private void psIntroMovies_Checked(object sender, RoutedEventArgs e)
        {
            StartupCredits = true;
            LogFile("Launcher: Startup Credits enabled");
        }

        private void psMaps_Unchecked(object sender, RoutedEventArgs e)
        {
            MapDownloading = false;
            LogFile("Project Cartographer: Custom map downloading disabled");
        }

        private void psVoice_Checked(object sender, RoutedEventArgs e)
        {
            VoiceChat = true;
            LogFile("Project Cartographer: Voice chat enabled.");
        }

        private void psVoice_Unchecked(object sender, RoutedEventArgs e)
        {
            VoiceChat = false;
            LogFile("Project Cartographer: Voice chat disabled.");
        }

        private void psFullScreen_Checked(object sender, RoutedEventArgs e)
        {
            if (psWindowed.IsChecked == true || psBorderless.IsChecked == true)
            {
                psWindowed.IsChecked = false;
                psBorderless.IsChecked = false;
            }
            DefaultDisplay = "Fullscreen";
            LogFile("Display Mode: Full Screen enabled.");
        }

        private void psFullScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            if (psWindowed.IsChecked == false) psFullScreen.IsChecked = true;
        }

        private void psBorderless_Checked(object sender, RoutedEventArgs e)
        {
            if (psFullScreen.IsChecked == true)
            {
                psFullScreen.IsChecked = false;
                psWindowed.IsChecked = true;
                psBorderless.IsChecked = true;
            }
            DefaultDisplay = "Borderless";
            LogFile("Display Mode: Borderless window enabled.");
        }

        private void psWindowed_Checked(object sender, RoutedEventArgs e)
        {
            if (psFullScreen.IsChecked == true) psFullScreen.IsChecked = false;
            DefaultDisplay = "Windowed";
            LogFile("Display Mode: Windowed enabled.");
        }

        private void psWindowed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (psBorderless.IsChecked == true) psBorderless.IsChecked = false;
            if (psBorderless.IsChecked == false && psFullScreen.IsChecked == false) psWindowed.IsChecked = true;
        }
        #endregion

        private void main_form_Closed(object sender, EventArgs e)
        {
            try { SaveSettings(); }
            catch (Exception Ex) { ExLogFile(Ex.ToString()); }
        }
    }
}
