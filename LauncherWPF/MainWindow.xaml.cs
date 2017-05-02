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
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System.Reflection;

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
        private string Intro = Globals.GameDirectory + "\\movie\\intro_60.wmv";
        private string IntroBak = Globals.GameDirectory + "\\movie\\intro_60.wmv.bak";
        private string IntroLow = Globals.GameDirectory + "\\movie\\intro_low_60.wmv";
        private string IntroLowBak = Globals.GameDirectory + "\\movie\\intro_low_60.wmv.bak";
        private string DefaultDisplay;
        private bool LoginPanelCheck, SettingsPanelCheck, UpdatePanelCheck, Vsync, GameSound, DebugLog, VoiceChat, MapDownloading, fpsEnable, RememberMe, StartupCredits;
        private static bool NoTextInput(string NumericText)
        {
            Regex r = new Regex("[^0-9.-]+");
            return !r.IsMatch(NumericText);
        }

        #region Update
        private volatile string _Halo2Version = "1.00.00.11122";
        private volatile bool _LauncherUpdated = false; //Flags the launcher to need a restart, to replace the current version with H2Launcher_temp.exe
        private UpdateCollection _RemoteUpdateCollection;
        private UpdateCollection _LocalUpdateCollection;
        private UpdateCollection _UpdateCollection;
        delegate void AddToDetailsCallback(string text);
        delegate void UpdateProgressCallback(int Precentage);
        delegate void UpdaterFinishedCallback();
        #endregion

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
            UpdateController _UpdateController = new UpdateController();
            CheckInstallPath();

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
            CheckUpdates();
        }

        private void main_form_Initialized(object sender, EventArgs e)
        {
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
            if (File.Exists(ExLogFilePath)) File.Delete(ExLogFilePath);
        }

        private void main_form_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
            main_grid.Focus();
        }

        public void CheckInstallPath()
        {
            if (Globals.GameDirectory == "")
            {
                string BaseFolder;

                if (Environment.Is64BitOperatingSystem) BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                else BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.InitialDirectory = BaseFolder;
                    ofd.Title = "Navigate to Halo 2 Install Path";
                    ofd.Filter = "Halo 2 Executable|halo2.exe";
                    ofd.FilterIndex = 1;

                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Globals.GameDirectory = ofd.FileName.Replace(ofd.SafeFileName, "");
                        LauncherSettings.SaveSettings();
                    }
                    else CheckInstallPath();
                }
            }
            else
            {
                if (!Directory.Exists(Globals.GameDirectory))
                {
                    MessageBox.Show("The game directory was not found, please locate it to continue.", "", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
                    Globals.GameDirectory = "";
                    CheckInstallPath();
                }
            }
        }

        #region Updates
        public void AddToDetails(string Message)
        {
            if (usTextBox.Dispatcher.CheckAccess())
            {
                AddToDetailsCallback Update = new AddToDetailsCallback(AddToDetails);
                Dispatcher.Invoke(Update, new object[] { Message });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    string DateStamp = "[" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "]\r\n";
                    usTextBox.Text += DateStamp + Message + "\r\n" + "\r\n";
                });
                //usTextBox.Text.SelectionStart = usTextBox.Text.Length;
                //usTextBox.Text.ScrollToCaret();
            }
        }

        public void UpdateProgress(int Percentage)
        {
            if (usProgressBar.Dispatcher.CheckAccess() & usProgressLabel.Dispatcher.CheckAccess())
            {
                UpdateProgressCallback Update = new UpdateProgressCallback(UpdateProgress);
                Dispatcher.Invoke(Update, new object[] { Percentage });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    usProgressBar.Value = Percentage;
                    usProgressLabel.Content = usProgressLabel.Tag.ToString().Replace("{0}", Percentage.ToString());
                });
            }
        }

        public void UpdaterFinished()
        {
            if (Dispatcher.CheckAccess())
            {
                UpdaterFinishedCallback Update = new UpdaterFinishedCallback(UpdaterFinished);
                Dispatcher.Invoke(Update);
            }
        }

        public bool LoadRemoteUpdateCollection()
        {
            try
            {
                AddToDetails("Downloading remote update XML file....");
                WebClient Client = new WebClient();
                bool _isDownloading = false;
                if (File.Exists(Globals.Files + "RemoteUpdate.xml")) File.Delete(Globals.Files + "RemoteUpdate.xml");

                Client.DownloadFileCompleted += (s, e) =>
                {
                    UpdateProgress(100);
                    AddToDetails("RemoteUpdate XML file download complete.");

                    Client.Dispose();
                    Client = null;
                    _isDownloading = false;
                };
                Client.DownloadProgressChanged += (s, e) => { UpdateProgress(e.ProgressPercentage); };
                    Client.DownloadFileAsync(new Uri(Globals.RemoteUpdateXML), Globals.Files + "RemoteUpdate.xml");
                    _isDownloading = true;
                while (_isDownloading) { }
                XDocument RemoteXML = XDocument.Load(Globals.Files + "RemoteUpdate.xml");
                UpdateCollection tUpdateColleciton = new UpdateCollection();
                //replaceoriginal = (XmlRoot.Element("localpath").HasAttributes) ? ((XmlRoot.Element("localpath").Attribute("replaceoriginal") != null) ? true : false) : false
                foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
                                       select
                                           new UpdateObject
                                           {
                                               localpath = (string)XmlRoot.Element("localpath"),
                                               remotepath = (string)XmlRoot.Element("remotepath"),
                                               version = (string)XmlRoot.Element("version"),
                                               name = (string)XmlRoot.Element("name")
                                           }
                                    )
                )
                    tUpdateColleciton.AddObject((UpdateObject)UO);
                _RemoteUpdateCollection = tUpdateColleciton;
                return true;

            }
            catch (Exception)
            {
                AddToDetails("There was an issue loading the remote updates, try restarting the launcher to fix the issue.");
                return false;
            }
        }

        public bool LoadLocalUpdateCollection()
        {
            try
            {
                if (File.Exists(Globals.Files + "LocaleUpdate.xml"))
                {
                    //await Task.Delay(0);
                    XDocument RemoteXML = XDocument.Load(Globals.Files + "LocaleUpdate.xml");
                    UpdateCollection tUpdateColleciton = new UpdateCollection();
                    foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
                                           select
                                               new UpdateObject
                                               {
                                                   localpath = (string)XmlRoot.Element("localpath"),
                                                   remotepath = (string)XmlRoot.Element("remotepath"),
                                                   version = (string)XmlRoot.Element("version"),
                                                   name = (string)XmlRoot.Element("name")
                                                   //replaceoriginal = (bool)XmlRoot.Element("localpath").Attribute("replaceoriginal")
                                               }
                                        )
                    )
                        if (File.Exists(((UpdateObject)UO).localpath.Replace("_temp", ""))) tUpdateColleciton.AddObject((UpdateObject)UO);
                    _LocalUpdateCollection = tUpdateColleciton;
                    return true;
                }
                else return true;
            }
            catch (Exception)
            {
                AddToDetails("There was an issue loading the local updates, try restarting the launcher to fix the issue.");
                return false;
            }
        }

        public async void CheckUpdates()
        {
            if (!LoginPanelCheck && !SettingsPanelCheck)
            {
                if (UpdatePanel.Margin.Bottom == -250)
                {
                    PanelAnimation("sbShowUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = true;
                }
            }
            await Task.Run(() =>
            {
                if (UpdateGameToLatest())
                {
                    if (LoadLocalUpdateCollection())
                    {
                        if (LoadRemoteUpdateCollection())
                        {
                            if (NeedToUpdate())
                            {
                                DownloadUpdates();
                                AddToDetails("All necessary updates downloaded.");
                                Task.Delay(1000);
                                Finished();
                            }
                            else
                            {
                                AddToDetails("No updates found.");
                                Task.Delay(1000);
                                UpdaterFinished();
                            }
                        }
                        else
                        {
                            AddToDetails("Failure to load Remote Update XML.");
                            Task.Delay(1000);
                            Finished();
                        }
                    }
                    else
                    {
                        AddToDetails("Failure to load Local Update XML.");
                        Task.Delay(1000);
                        Finished();
                    }
                }
                else
                {
                    AddToDetails("Failure to update game version.");
                    Task.Delay(1000);
                    Finished();
                }
            });
        }

        private bool UpdateGameToLatest()
        {
            string CurrentHalo2Version = FileVersionInfo.GetVersionInfo(Globals.GameDirectory + "halo2.exe").FileVersion;
            AddToDetails(string.Format("Halo 2 Version Current Version: {0}\r\nHalo 2 Expected Version: {1}", CurrentHalo2Version, _Halo2Version));

            if (_Halo2Version != CurrentHalo2Version)
            {
                AddToDetails("Updating Halo 2 to the latest version.");

                WebClient Client = new WebClient();
                bool _isDownloading = false;
                Client.DownloadFileCompleted += (s, e) =>
                {
                    UpdateProgress(100);
                    AddToDetails("Game update downloaded.");
                    Client.Dispose();
                    Client = null;
                    _isDownloading = false;
                };
                Client.DownloadProgressChanged += (s, e) => { UpdateProgress(e.ProgressPercentage); };

                try
                {
                    Client.DownloadFileAsync(new Uri(Globals.RemoteUpdate + "halo2/Update.exe"), Globals.Downloads + "\\Update.exe");
                    _isDownloading = true;
                }
                catch (Exception) { throw new Exception("Error"); }

                while (_isDownloading) { }
                AddToDetails("Waiting for updates to finish installing.");

                bool _isUpdating = true;
                Process.Start(Globals.Downloads + "\\Update.exe");

                while (_isUpdating) if (Process.GetProcessesByName("Update").Length == 0) _isUpdating = false;

                File.Delete(Globals.Downloads + "\\Update.exe");
                return true;
            }
            return true;
        }

        private bool NeedToUpdate()
        {
            if (_LocalUpdateCollection != null)
            {
                _UpdateCollection = new UpdateCollection();

                foreach (UpdateObject UO in _RemoteUpdateCollection)
                {
                    UpdateObject tUO = _LocalUpdateCollection[UO.name];
                    if (tUO == null) _UpdateCollection.AddObject(UO);
                    else if (tUO.version != UO.version) _UpdateCollection.AddObject(UO);
                    else if (tUO.localpath != UO.localpath) MoveFile(tUO.name, tUO.localpath, UO.localpath);

                }
            }

            _UpdateCollection = (_UpdateCollection != null) ? _UpdateCollection : _RemoteUpdateCollection;

            if (_UpdateCollection.Count > 0) return true;
            else return false;
        }

        private void DownloadUpdates()
        {

            for (int i = 0; i < _UpdateCollection.Count; i++)
            {
                UpdateObject tUO = _UpdateCollection[i];
                AddToDetails("Downloading " + tUO.name + "....");

                if (tUO.name == "H2Launcher") _LauncherUpdated = true;
                WebClient Client = new WebClient();
                bool _isDownloading = false;
                Client.DownloadFileCompleted += (s, e) =>
                {
                    UpdateProgress(100);
                    //AddToDetails("Download Complete.");
                    Client.Dispose();
                    Client = null;
                    _isDownloading = false;
                };

                Client.DownloadProgressChanged += (s, e) => { UpdateProgress(e.ProgressPercentage); };

                try
                {
                    Client.DownloadFileAsync(new Uri(tUO.remotepath), tUO.localpath);
                    _isDownloading = true;
                }
                catch (Exception) { throw new Exception("Error"); }

                DownloadFile(tUO.remotepath, tUO.localpath);
                while (_isDownloading) { }
            }
        }

        public void Finished()
        {
            if (File.Exists(Globals.Files + "LocaleUpdate.xml")) File.Delete(Globals.Files + "LocaleUpdate.xml");
            File.Move(Globals.Files + "RemoteUpdate.xml", Globals.Files + "LocaleUpdate.xml");

            if (_LauncherUpdated)
            {
                AddToDetails("Restarting Launcher to complete update");
                Task.Delay(5000);
                ProcessStartInfo p = new ProcessStartInfo();
                p.UseShellExecute = false;
                p.Arguments = "/C ping 127.0.0.1 -n 1 -w 5000 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\" & ping 127.0.0.1 -n 1 -w 2000 > Nul & rename H2Launcher_temp.exe H2Launcher.exe & ping 127.0.0.1 -n 1 -w 20000 > Nul & start H2Launcher.exe";
                p.WindowStyle = ProcessWindowStyle.Hidden;
                p.CreateNoWindow = true;
                p.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                p.FileName = "cmd.exe";
                Process.Start(p);
                Process.GetCurrentProcess().Kill();
            }
            else UpdaterFinished();
        }

        private void MoveFile(string Name, string Source, string Destination)
        {
            using (Stream SourceStream = File.Open(Source, FileMode.Open))
            {
                using (Stream DestinationStream = File.Create(Destination))
                {
                    AddToDetails("Moving " + Name + " \r\n\tFrom " + Source + " \r\n\tTo " + Destination);
                    UpdateProgress(0);
                    byte[] buffer = new byte[SourceStream.Length / 1024];
                    int read;
                    while ((read = SourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        DestinationStream.Write(buffer, 0, buffer.Length);
                        int progress = (read / buffer.Length) * 100;
                        UpdateProgress(progress);
                    }
                    UpdateProgress(100);
                    AddToDetails("Installation complete.");
                }
            }
            Task.Delay(500);
            File.Delete(Source);
        }

        private void DownloadFile(String remoteFilename, String localFilename)
        {
            bool _isDownloading = false;
            WebClient Client = new WebClient();
            Client.DownloadFileCompleted += (s, e) =>
            {
                UpdateProgress(100);
                //AddToDetails("Downloading " + remoteFilename + " complete.");
                Client = null;
                _isDownloading = false;
            };
            Client.DownloadProgressChanged += (s, e) => { UpdateProgress(e.ProgressPercentage); };
            try
            {
                UpdateProgress(0);
                Client.Dispose();
                Client.DownloadFileAsync(new Uri(remoteFilename), localFilename);
                _isDownloading = true;
                while (_isDownloading) { }
            }
            catch (Exception) { throw new Exception("Error"); }
        }
        #endregion

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
            exlog.WriteLine("\r\n");

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
                    usTextBox.Clear();
                    CheckUpdates();
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

            if (Directory.Exists(Globals.GameDirectory))
            {
                if (psIntroMovies.IsChecked == false)
                {
                    try
                    {
                        if (File.Exists(Intro) && File.Exists(IntroLow) && !File.Exists(IntroBak) && !File.Exists(IntroLowBak))
                        {
                            File.Move(Intro, IntroBak);
                            File.Move(IntroLow, IntroLowBak);
                            File.Create(Intro).Close();
                            File.Create(IntroLow).Close();
                        }
                    }
                    catch (Exception Ex) { ExLogFile(Ex.ToString()); }
                }
                if (psIntroMovies.IsChecked == true)
                {
                    if (File.Exists(Intro) && File.Exists(IntroLow) && File.Exists(IntroBak) && File.Exists(IntroLowBak))
                    {
                        try
                        {
                            File.Delete(Intro);
                            File.Delete(IntroLow);
                            File.Move(IntroBak, Intro);
                            File.Move(IntroLowBak, IntroLow);
                        }
                        catch (Exception Ex) { ExLogFile(Ex.ToString()); }
                    }
                }

                LauncherSettings.SaveSettings();
                ProjectSettings.SaveSettings();
                LogFile("Settings saved");
            }
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

        private void psForceUpdate_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Globals.Files + "LocaleUpdate.xml")) File.Delete(Globals.Files + "LocaleUpdate.xml");
            Task.Delay(5000);
            ProcessStartInfo p = new ProcessStartInfo();
            p.UseShellExecute = false;
            p.Arguments = "/C ping 127.0.0.1 -n 1 -w 5000 > Nul & start H2Launcher.exe";
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.CreateNoWindow = true;
            p.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            p.FileName = "cmd.exe";
            Process.Start(p);
            Process.GetCurrentProcess().Kill();

            psForceUpdate.IsChecked = false;
        }

        private void psForceUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            psForceUpdate.IsChecked = false;
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
            if (!File.Exists(Globals.Files + "LocaleUpdate.xml") && File.Exists(Globals.Files + "RemoteUpdate.xml")) File.Move(Globals.Files + "RemoteUpdate.xml", Globals.Files + "LocaleUpdate.xml");
        }
    }
}