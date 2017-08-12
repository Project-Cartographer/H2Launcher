using Cartographer_Launcher.Includes;
using Cartographer_Launcher.Includes.Dependencies;
using Cartographer_Launcher.Includes.Settings;
using H2Shield.Includes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Linq;

namespace LauncherWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

	public partial class MainWindow : Window
	{
		#region Global Declares
		BitmapImage TitleLogo = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/h2logo.png"));
		SolidColorBrush MenuItemSelect = new SolidColorBrush(Color.FromRgb(178, 211, 246));
		SolidColorBrush MenuItem = new SolidColorBrush(Color.FromRgb(94, 109, 139));
		Launcher LauncherSettings = LauncherRuntime.LauncherSettings;
		ProjectCartographer ProjectSettings = LauncherRuntime.ProjectSettings;
		H2Startup H2StartupSettings = new H2Startup();

		private const string RegisterURL = @"https://www.cartographer.online/";
		private const string AppealURL = @"https://www.halo2.online/forums/the-drama-spot.31/";
		private string DateTimeStamp = DateTime.Now.ToString("M/dd/yyyy (HH:mm)");
		private string DisplayMode;
		public bool PlayCheck;
		private bool ApplicationShutdownCheck,
			SaveSettingsCheck,
			LoginPanelCheck,
			SettingsPanelCheck,
			UpdatePanelCheck,
			MessageBoxPanelCheck,
			NoGameSound,
			Vsync,
			DebugLog,
			VoiceChat,
			MapDownloading,
			fpsEnable,
			RememberMe;
		private static bool NumbericInputOnly(string NumericText) { Regex r = new Regex("[^0-9.]+"); return !r.IsMatch(NumericText); }

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
			try { InitializeComponent(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to load components.");
			}
		}

		#region Main Methods
		public void LogFile(string Message)
		{
			StreamWriter log;
			if (!File.Exists(Globals.LogFile)) log = new StreamWriter(Globals.LogFile);
			else log = File.AppendText(Globals.LogFile);

			log.WriteLine("Date: " + DateTimeStamp + " - " + Message);

			log.Flush();
			log.Dispose();
			log.Close();
		}

		public void ExLogFile(string Message)
		{
			StreamWriter exlog;
			if (!File.Exists(Globals.ExLogFile)) exlog = new StreamWriter(Globals.ExLogFile);
			else exlog = File.AppendText(Globals.ExLogFile);

			exlog.WriteLine("Date: " + DateTimeStamp);
			exlog.WriteLine(Message);
			exlog.WriteLine(Environment.NewLine);

			exlog.Flush();
			exlog.Dispose();
			exlog.Close();
		}

		private void MessageBoxPanelContent(string Title, string Content)
		{
			mbTitle.Content = Title;
			mbMessage.Text = Content;

		}

		public void Debug(string Error)
		{
			MessageBoxResult mr = MessageBox.Show(Error, Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Error);
			switch (mr)
			{
				case MessageBoxResult.OK:
					Process.Start(Globals.ExLogFile);
					Application.Current.Shutdown();
					break;

				case MessageBoxResult.None:
					Application.Current.Shutdown();
					break;
			}
		}


		private void LauncherDelete(string Arguments)
		{
			Task.Delay(1000);
			ProcessStartInfo Info = new ProcessStartInfo();
			Info.Arguments = Arguments;
			Info.WindowStyle = ProcessWindowStyle.Hidden;
			Info.CreateNoWindow = true;
			Info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Info.FileName = "cmd.exe";
			Process.Start(Info);
			Process.GetCurrentProcess().Kill();
		}

		private void WebServerCheck()
		{
			HttpWebRequest request = WebRequest.Create(Globals.LauncherCheck) as HttpWebRequest;
			request.Method = "HEAD";
			HttpWebResponse response;
			try { response = request.GetResponse() as HttpWebResponse; }
			catch (WebException wex)
			{
				response = wex.Response as HttpWebResponse;
				MessageBox.Show("Cannot connect to Cartographer webserver." + Environment.NewLine + "Please check your internet connection and try again." + Environment.NewLine + Environment.NewLine + "The launcher will now be deleted.", "CONNECTION ERROR");
				LauncherDelete("/C ping 127.0.0.1 -n 1 -w 100 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"");
			}
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				MessageBox.Show("Cannot connect to Cartographer webserver." + Environment.NewLine + "Please check your internet connection and try again." + Environment.NewLine + Environment.NewLine + "The launcher will now be deleted.", "CONNECTION ERROR");
				LauncherDelete("/C ping 127.0.0.1 -n 1 -w 100 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"");
			}
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
					else Application.Current.Shutdown();
				}
			}
			else
			{
				if (!Directory.Exists(Globals.GameDirectory))
				{
					MessageBox.Show("Halo 2 game directory was not found, please relocate the executable.", Kantanomo.GoIdioms, MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
					Globals.GameDirectory = "";
					CheckInstallPath();
				}
			}
		}

		private async void LoginVerification()
		{
			await Task.Delay(500).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					if (LoginPanel.Margin.Top == 0)
					{
						PanelAnimation("sbHideLoginMenu", LoginPanel);
						LoginPanelCheck = false;
					}
				});
			});
			StatusButton.Content = "Currently verifying login...";
			await Task.Delay(1000).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);

					switch (loginResult.LoginResultEnum)
					{
						case LoginResultEnum.Successfull:
							{
								StatusButton.Content = Globals.VersionNumber;
								LauncherSettings.PlayerTag = lsUser.Text;
								ProjectSettings.LoginToken = loginResult.LoginToken;
								if (PlayCheck)
								{
									LauncherRuntime.StartHalo(lsUser.Text, loginResult.LoginToken, this);
									LogFile("Login successful, game starting...");
									PlayCheck = false;
								}
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.InvalidLoginToken:
							{
								StatusButton.Content = Globals.VersionNumber;
								MessageBox.Show(this, "This login token is no longer valid." + Environment.NewLine + "Please enter your login credentials and try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								ProjectSettings.LoginToken = "";
								lsPass.Password = "";
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Login token invalid");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.InvalidUsernameOrPassword:
							{
								StatusButton.Content = Globals.VersionNumber;
								MessageBox.Show(this, "The playertag or password entered is invalid." + Environment.NewLine + "Please try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								lsUser.Text = "";
								lsPass.Password = "";
								ProjectSettings.LoginToken = "";
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Player credentials invalid");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.Banned:
							{
								StatusButton.Content = Globals.VersionNumber;
								if (MessageBox.Show(this, "You have been banned, please visit the forum to appeal your ban." + Environment.NewLine + "Would you like us to open the forums for you?.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
									Process.Start(AppealURL);
								ProjectSettings.LoginToken = "";
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Machine is banned");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.GenericFailure:
							{
								if (LoginPanel.Margin.Top == -140)
								{
									PanelAnimation("sbShowLoginMenu", LoginPanel);
									LoginPanelCheck = true;
								}
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: General login failure");
								PlayCheck = false;
								break;
							}
					}
				});
			});
		}
		#endregion

		#region Form Event Methods
		private void MainForm_Initialized(object sender, EventArgs e)
		{
			if (File.Exists(Globals.LogFile)) File.Delete(Globals.LogFile);
			if (File.Exists(Globals.ExLogFile)) File.Delete(Globals.ExLogFile);

			MessageBoxPanelContent("", "");
			MessageBoxPanel.Visibility = Visibility.Hidden;
			mbMessage.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

			LogFile("Log file initialized.");
			LogFile("Game install directory: " + Globals.GameDirectory);
			LogFile("Launcher file directory: " + Globals.H2vHubDirectory);

			try { WebServerCheck(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to check launcher.");
			}

			try { CheckInstallPath(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to open Windows Explorer.");
			}

			try { LoadSettings(); }
			catch (Exception Ex)
			{
				if (File.Exists(Globals.Files + "Settings.ini")) File.Delete(Globals.Files + "Settings.ini");
				if (File.Exists(Globals.GameDirectory + "xlive.ini")) File.Delete(Globals.GameDirectory + "xlive.ini");
				ExLogFile(Ex.ToString());
				Debug("Failed to load setting files.");
			}

			try { LoginVerification(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to verify login token.");
			}

			try { CheckUpdates(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to begin update process");
			}

			usProgressLabel.Tag = "{0}/100";
			usProgressLabel.Content = "--/100";
		}

		private void MainForm_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed) DragMove();
			MainGrid.Focus();
		}

		private void MainForm_Closed(object sender, EventArgs e)
		{
			ApplicationShutdownCheck = true;
			try { SaveSettings(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to save settings");
			}
		}

		private void LogoImage_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
			logo.UriSource = new Uri("pack://application:,,,/Resources/Images/h2logo.png");
			logo.DecodePixelWidth = 800;
			logo.EndInit();
			logo.Freeze();

			var logo_image = sender as Image;
			logo_image.Source = logo;
		}

		private void BackgroundImage_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage bg = new BitmapImage();
			bg.BeginInit();
			bg.UriSource = new Uri("pack://application:,,,/Resources/Images/launcher_background.png");
			bg.DecodePixelWidth = 800;
			bg.EndInit();
			bg.Freeze();

			var bg_image = sender as Image;
			bg_image.Source = bg;
		}

		private void PanelAnimation(string Storyboard, StackPanel sp)
		{
			Storyboard sb = Resources[Storyboard] as Storyboard;
			sb.Begin(sp);
		}

		private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !NumbericInputOnly(e.Text);
		}

		private void NumericPasteCheck(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(String)))
			{
				String numeric_text = (String)e.DataObject.GetData(typeof(String));
				if (!NumbericInputOnly(numeric_text)) e.CancelCommand();
			}
			else e.CancelCommand();
		}

		private void ControlMinimize_Click(object sender, RoutedEventArgs e)
		{
			MainForm.WindowState = WindowState.Minimized;
		}

		private void ControlClose_Click(object sender, RoutedEventArgs e)
		{
			MainForm.Visibility = Visibility.Hidden;
			ApplicationShutdownCheck = true;
			try { SaveSettings(); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to save settings.");
			}
		}

		private void StatusButton_Click(object sender, RoutedEventArgs e)
		{
			PanelAnimation("sbShowMessageBox", MessageBoxPanel);
			MessageBoxPanel.Visibility = Visibility.Visible;
			MessageBoxPanelContent(Kantanomo.GoIdioms, "-Project Devs" + Environment.NewLine + Environment.NewLine + "Permanull" + Environment.NewLine + "Rude Yoshi" + Environment.NewLine + "Glitchy Scripts" + Environment.NewLine + "Himanshu" + Environment.NewLine + "supersniper" + Environment.NewLine + "Hootspa (left)" + Environment.NewLine + "Kantanomo (left)" + Environment.NewLine + "FishPHD (left)");
			if (StatusButton.IsChecked == true) StatusButton.IsChecked = false;
		}

		private void mbpOK_Click(object sender, RoutedEventArgs e)
		{
			PanelAnimation("sbHideMessageBox", MessageBoxPanel);
			MessageBoxPanel.Visibility = Visibility.Hidden;
			MessageBoxPanelCheck = false;
		}

		private void PanelClose_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
			{
				if (MessageBoxPanelCheck)
				{
					PanelAnimation("sbHideMessageBox", MessageBoxPanel);
					MessageBoxPanel.Visibility = Visibility.Hidden;
					MessageBoxPanelCheck = false;
				}
				if (SettingsPanelCheck)
				{
					if (SettingPanel.Margin.Right == 0)
					{
						PanelAnimation("sbHideSettingsMenu", SettingPanel);
						SettingsPanelCheck = false;
						SaveSettingsCheck = true;

						try { SaveSettings(); }
						catch (Exception Ex)
						{
							ExLogFile(Ex.ToString());
							Debug("Failed to save settings.");
						}
					}
				}
				if (UpdatePanelCheck)
				{
					if (UpdatePanel.Margin.Bottom == 0)
					{
						PanelAnimation("sbHideUpdateMenu", UpdatePanel);
						UpdatePanelCheck = false;
					}
				}
				if (LoginPanelCheck)
				{
					if (LoginPanel.Margin.Top == 0)
					{
						if (lsPass.Password != "") PlayButton.Content = "PLAY";
						PanelAnimation("sbHideLoginMenu", LoginPanel);
						LoginPanelCheck = false;
						SaveSettingsCheck = true;

						try { LoginVerification(); }
						catch (Exception Ex)
						{
							ExLogFile(Ex.ToString());
							Debug("Failed to verify login token.");
						}

						try { SaveSettings(); }
						catch (Exception Ex)
						{
							ExLogFile(Ex.ToString());
							Debug("Failed to save settings.");
						}
					}
				}
			}
		}
		#endregion

		#region Menu Buttons
		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
			{
				if (!MessageBoxPanelCheck)
				{
					if (!PlayCheck)
					{
						if (lsPass.Password == "")
						{
							if (!SettingsPanelCheck && !UpdatePanelCheck)
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
									SaveSettingsCheck = true;

									try { SaveSettings(); }
									catch (Exception Ex)
									{
										ExLogFile(Ex.ToString());
										Debug("Failed to save settings.");
									}
								}
							}
						}
						if (SettingPanel.Margin.Right == 0)
						{
							PanelAnimation("sbHideSettingsMenu", SettingPanel);
							SettingsPanelCheck = false;
							SaveSettingsCheck = true;

							try { SaveSettings(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to save settings.");
							}
						}
						if (PlayButton.Content.ToString() != "PLAY" && LoginPanel.Margin.Top == -140)
						{
							PanelAnimation("sbShowLoginMenu", LoginPanel);
							LoginPanelCheck = true;
						}
						if (UpdatePanelCheck)
						{
							if (UpdatePanel.Margin.Bottom == 0)
							{
								PanelAnimation("sbHideUpdateMenu", UpdatePanel);
								UpdatePanelCheck = false;
							}
							if (PlayButton.Content.ToString() != "PLAY" && LoginPanel.Margin.Top == -140)
							{
								PanelAnimation("sbShowLoginMenu", LoginPanel);
								LoginPanelCheck = true;
							}
						}
						if (LoginPanelCheck)
						{
							if (PlayButton.Content.ToString() != "PLAY" && LoginPanel.Margin.Top == 0)
							{
								PanelAnimation("sbHideLoginMenu", LoginPanel);
								LoginPanelCheck = false;
								SaveSettingsCheck = true;

								try { SaveSettings(); }
								catch (Exception Ex)
								{
									ExLogFile(Ex.ToString());
									Debug("Failed to save settings.");
								}
							}
						}
						if (!SettingsPanelCheck && !UpdatePanelCheck && !LoginPanelCheck && PlayButton.Content.ToString() != "LOGIN" || PlayButton.Content.ToString() == "PLAY")
						{
							PlayCheck = true;

							try { LoginVerification(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to get login token.");
							}

						}
					}
				}
			}
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
			{
				if (!MessageBoxPanelCheck)
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
							SaveSettingsCheck = true;

							try { SaveSettings(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to save settings.");
							}
						}
					}
					if (LoginPanelCheck)
					{
						if (LoginPanel.Margin.Top == 0)
						{
							PanelAnimation("sbHideLoginMenu", LoginPanel);
							LoginPanelCheck = false;
							SaveSettingsCheck = true;

							try { SaveSettings(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to save settings.");
							}
						}
						if (SettingPanel.Margin.Right == -220)
						{
							PanelAnimation("sbShowSettingsMenu", SettingPanel);
							SettingsPanelCheck = true;
						}
					}
					if (UpdatePanelCheck)
					{
						if (UpdatePanel.Margin.Bottom == 0)
						{
							PanelAnimation("sbHideUpdateMenu", UpdatePanel);
							UpdatePanelCheck = false;
						}
						if (SettingPanel.Margin.Right == -220)
						{
							PanelAnimation("sbShowSettingsMenu", SettingPanel);
							SettingsPanelCheck = true;
						}
					}
				}
			}
		}

		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
			{
				if (!MessageBoxPanelCheck)
				{
					if (!LoginPanelCheck && !SettingsPanelCheck)
					{
						if (UpdatePanel.Margin.Bottom == -250)
						{
							usTextBox.Clear();

							try { CheckUpdates(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to begin update process.");
							}
							PanelAnimation("sbShowUpdateMenu", UpdatePanel);
							UpdatePanelCheck = true;
						}
						else if (UpdatePanel.Margin.Bottom == 0)
						{
							PanelAnimation("sbHideUpdateMenu", UpdatePanel);
							UpdatePanelCheck = false;
						}
					}
					if (LoginPanelCheck)
					{
						if (LoginPanel.Margin.Top == 0)
						{
							PanelAnimation("sbHideLoginMenu", LoginPanel);
							LoginPanelCheck = false;
							SaveSettingsCheck = true;

							try { SaveSettings(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to save settings.");
							}
						}
						if (UpdatePanel.Margin.Bottom == -250)
						{
							usTextBox.Clear();

							try { CheckUpdates(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to begin update process.");
							}
							PanelAnimation("sbShowUpdateMenu", UpdatePanel);
							UpdatePanelCheck = true;
						}
					}
					if (SettingsPanelCheck)
					{
						if (SettingPanel.Margin.Right == 0)
						{
							PanelAnimation("sbHideSettingsMenu", SettingPanel);
							SettingsPanelCheck = false;
							SaveSettingsCheck = true;

							try { SaveSettings(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to save settings.");
							}
						}
						if (UpdatePanel.Margin.Bottom == -250)
						{
							usTextBox.Clear();

							try { CheckUpdates(); }
							catch (Exception Ex)
							{
								ExLogFile(Ex.ToString());
								Debug("Failed to begin update process.");
							}
							PanelAnimation("sbShowUpdateMenu", UpdatePanel);
							UpdatePanelCheck = true;
						}
					}
				}
			}
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(RegisterURL);
		}
		#endregion

		#region Update Global Definitions
		private bool UpdateGameToLatest()
		{
			string DateStamp = "[" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "]" + Environment.NewLine;
			string CurrentHalo2Version = FileVersionInfo.GetVersionInfo(Globals.GameDirectory + "halo2.exe").FileVersion;

			try { AddToDetails(string.Format(DateStamp + "Halo 2 Current Version: {0}" + Environment.NewLine + "Halo 2 Expected Version: {1}", CurrentHalo2Version, _Halo2Version + Environment.NewLine)); }
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Failed to get current version of Halo 2");
			}

			if (_Halo2Version != CurrentHalo2Version)
			{
				AddToDetails("Updating Halo 2 to the latest version...");

				WebClient Client = new WebClient();
				bool _isDownloading = false;
				Client.DownloadFileCompleted += (s, e) =>
				{
					UpdateProgress(100);
					AddToDetails("Halo 2 update patch download completed.");
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
				AddToDetails("Waiting for updates to finish installing." + Environment.NewLine);

				bool _isUpdating = true;
				Process.Start(Globals.Downloads + "\\Update.exe");

				while (_isUpdating) if (Process.GetProcessesByName("Update").Length == 0) _isUpdating = false;

				File.Delete(Globals.Downloads + "\\Update.exe");
				return true;
			}
			return true;
		}

		public bool LoadLocalUpdateCollection()
		{
			try
			{
				if (File.Exists(Globals.Files + "LocalUpdate.xml"))
				{
					XDocument RemoteXML = XDocument.Load(Globals.Files + "LocalUpdate.xml");
					UpdateCollection tUpdateColleciton = new UpdateCollection();
					foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
										   select
										   new UpdateObject
										   {
											   localpath = (string)XmlRoot.Element("localpath"),
											   remotepath = (string)XmlRoot.Element("remotepath"),
											   version = (string)XmlRoot.Element("version"),
											   name = (string)XmlRoot.Element("name")
										   }
										))
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
					AddToDetails("Remote update XML file download complete." + Environment.NewLine);

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
				foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
									   select
									   new UpdateObject
									   {
										   localpath = (string)XmlRoot.Element("localpath"),
										   remotepath = (string)XmlRoot.Element("remotepath"),
										   version = (string)XmlRoot.Element("version"),
										   name = (string)XmlRoot.Element("name")
									   }
									))
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
		#endregion

		#region Update Methods
		public void AddToDetails(string Message)
		{
			if (usTextBox.Dispatcher.CheckAccess())
			{
				AddToDetailsCallback Update = new AddToDetailsCallback(AddToDetails);
				Dispatcher.Invoke(Update, new object[] { Message });
			}
			else Dispatcher.Invoke(() => { usTextBox.Text += Message + Environment.NewLine; });
		}

		private void MoveFile(string Name, string Source, string Destination)
		{
			using (Stream SourceStream = File.Open(Source, FileMode.Open))
			{
				using (Stream DestinationStream = File.Create(Destination))
				{
					AddToDetails("Moving " + Name + Environment.NewLine + " \tfrom " + Source + Environment.NewLine + " \tto " + Destination);
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
					AddToDetails("Installation complete." + Environment.NewLine);
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
				AddToDetails("Downloading complete.");
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
					AddToDetails("File downloaded." + Environment.NewLine);
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
			string CurrentName = Assembly.GetExecutingAssembly().Location.ToString();
			if (File.Exists(Globals.Files + "LocalUpdate.xml")) File.Delete(Globals.Files + "LocalUpdate.xml");
			File.Move(Globals.Files + "RemoteUpdate.xml", Globals.Files + "LocalUpdate.xml");

			if (_LauncherUpdated)
			{
				AddToDetails("The launcher needs to restart to complete the update.");
				Task.Delay(4000);
				LauncherDelete("/c ping 127.0.0.1 -n 3 -w 2000 > Nul & Del " + "\"" + CurrentName + "\"" + "& ping 127.0.0.1 -n 1 -w 2000 > Nul & rename H2Launcher_temp.exe H2Launcher.exe & ping 127.0.0.1 -n 1 -w 1000 > Nul & start H2Launcher.exe");
			}
			else UpdaterFinished();
		}

		public void UpdaterFinished()
		{
			if (Dispatcher.CheckAccess())
			{
				UpdaterFinishedCallback Update = new UpdaterFinishedCallback(UpdaterFinished);
				Dispatcher.Invoke(Update);
			}
			Dispatcher.Invoke(() =>
			{
				if (UpdatePanel.Margin.Bottom == 0)
				{
					PanelAnimation("sbHideUpdateMenu", UpdatePanel);
					UpdatePanelCheck = false;
				}
			});
		}

		public async void CheckUpdates()
		{
			await Task.Delay(1000).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					if (!LoginPanelCheck && !SettingsPanelCheck)
					{
						if (UpdatePanel.Margin.Bottom == -250)
						{
							PanelAnimation("sbShowUpdateMenu", UpdatePanel);
							UpdatePanelCheck = true;
						}
					}
				});
			});
			await Task.Delay(1200).ContinueWith(_ =>
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
								AddToDetails("Project Cartographer up to date.");
								Task.Delay(1000);
								UpdaterFinished();
							}
						}
						else
						{
							AddToDetails("Failed to load the remote update file.");
							Task.Delay(1000);
							Finished();
						}
					}
					else
					{
						AddToDetails("Failed to load the local update file.");
						Task.Delay(1000);
						Finished();
					}
				}
				else
				{
					AddToDetails("Failed to update the game to the latest version.");
					Task.Delay(1000);
					Finished();
				}
			});
		}
		#endregion

		#region Setting Events
		private void lsUser_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (lsUser.Text != LauncherSettings.PlayerTag)
			{
				PlayButton.Content = "LOGIN";
				lsPass.Password = "";
				ProjectSettings.LoginToken = "";
			}
		}

		private void lsPass_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (lsUser.Text != "" && lsPass.Password != "") PlayButton.Content = "PLAY";
			else PlayButton.Content = "LOGIN";
		}

		private void psForceUpdate_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				File.Delete(Globals.Files + "LocalUpdate.xml");
				LauncherDelete("/C ping 127.0.0.1 -n 1 -w 2000 > Nul & start H2Launcher.exe");
			}
			catch (Exception Ex)
			{
				ExLogFile(Ex.ToString());
				Debug("Local update file not found.");
			}
			psForceUpdate.IsChecked = false;
		}

		private void psForceUpdate_Unchecked(object sender, RoutedEventArgs e)
		{
			psForceUpdate.IsChecked = false;
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

		private void psGameDirectory_Checked(object sender, RoutedEventArgs e)
		{
			Process.Start(Globals.GameDirectory);
			psGameDirectory.IsChecked = false;
		}

		private void psGameDirectory_Unchecked(object sender, RoutedEventArgs e)
		{
			psGameDirectory.IsChecked = false;
		}
		#endregion

		#region Launcher Settings
		private void psWindow_Checked(object sender, RoutedEventArgs e)
		{
			psWindow.IsChecked = true;
			DisplayMode = "Windowed";
			GameRegistrySettings.SetDisplayMode(false);
			LogFile("Display Mode: Window mode enabled.");
		}

		private void psWindow_Unchecked(object sender, RoutedEventArgs e)
		{
			psWindow.IsChecked = false;
			DisplayMode = "Fullscreen";
			GameRegistrySettings.SetDisplayMode(true);
			LogFile("Display Mode: Window mode disabled.");
		}

		private void psNoSound_Checked(object sender, RoutedEventArgs e)
		{
			NoGameSound = true;
			LogFile("Halo 2 Launch Parameter: -nosound added to game launch.");
		}

		private void psNoSound_Unchecked(object sender, RoutedEventArgs e)
		{
			NoGameSound = false;
			LogFile("Halo 2 Launch Parameter: -nosound removed from game launch.");
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

		private void psMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LogFile("Halo 2 Launch Parameter: -monitor:" + psMonitorSelect.SelectedIndex.ToString());
		}

		private void psMonitor_Unchecked(object sender, RoutedEventArgs e)
		{
			psMonitor.IsChecked = true;
		}

		private void lsRememberMe_Checked(object sender, RoutedEventArgs e)
		{
			RememberMe = true;
			LogFile("H2Launcher Setting: Autologin enabled.");
		}

		private void lsRememberMe_Unchecked(object sender, RoutedEventArgs e)
		{
			RememberMe = false;
			LogFile("H2Launcher Setting: Autologin disabled.");
		}
		#endregion

		#region Xlive Settings
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

		private void psPorts_Unchecked(object sender, RoutedEventArgs e)
		{
			psPorts.IsChecked = true;
		}

		private void psPortNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			LogFile("Project Cartographer: Game base port set to " + psPortNumber.Text.ToString());
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

		private void psFPSLimit_TextChanged(object sender, TextChangedEventArgs e)
		{
			LogFile("Project Cartographer: Maximum frames set to " + psFPSLimit.Text.ToString());
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

		private void psMaps_Checked(object sender, RoutedEventArgs e)
		{
			MapDownloading = true;
			LogFile("Project Cartographer: Custom map downloading enabled.");
		}

		private void psMaps_Unchecked(object sender, RoutedEventArgs e)
		{
			MapDownloading = false;
			LogFile("Project Cartographer: Custom map downloading disabled");
		}

		private void psFOV_Unchecked(object sender, RoutedEventArgs e)
		{
			psFOV.IsChecked = true;
		}

		private void psFOVSetting_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (psFOVSetting.Text == "" || int.Parse(psFOVSetting.Text) <= 0 || int.Parse(psFOVSetting.Text) >= 115) psFOVSetting.Text = "";
			LogFile("Project Cartographer: Game FOV changed to " + psFOVSetting.Text);
		}

		private void psCrosshair_Unchecked(object sender, RoutedEventArgs e)
		{
			psCrosshair.IsChecked = true;
		}

		private void psCrosshairSetting_TextChanged(object sender, TextChangedEventArgs e)
		{
			LogFile("Project Cartographer: Game reticle position changed to " + psCrosshairSetting.Text);
		}
		#endregion

		#region Settings: Load/Save
		private void LoadSettings()
		{
			LauncherSettings.LoadSettings();
			ProjectSettings.LoadSettings();

			//Playertag
			if (lsUser.Text != "") lsUsername.IsChecked = true;
			if (lsPass.Password != "") lsPassword.Foreground = MenuItemSelect;

			//Display Mode
			switch (LauncherSettings.DisplayMode)
			{
				case Globals.SettingsDisplayMode.Fullscreen:
					{
						psWindow.IsChecked = false;
						DisplayMode = "Fullscreen";
						break;
					}
				case Globals.SettingsDisplayMode.Windowed:
					{
						psWindow.IsChecked = true;

						DisplayMode = "Windowed";
						break;
					}
			}

			//Game Sound
			if (LauncherSettings.NoGameSound == 1) psNoSound.IsChecked = true;
			else psNoSound.IsChecked = false;

			//Vertical Sync
			if (LauncherSettings.VerticalSync == 1) psVsync.IsChecked = true;
			else psVsync.IsChecked = false;

			//Default Display
			psMonitor.IsChecked = true;
			for (int s = 0; s < System.Windows.Forms.Screen.AllScreens.Length; s++)
			{
				psMonitorSelect.Items.Add((s + 1).ToString() + ((System.Windows.Forms.Screen.AllScreens[s].Primary) ? "*" : ""));
				if (s == LauncherSettings.DefaultDisplay) psMonitorSelect.SelectedIndex = s;
			}

			//Remember Me
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

			//Login Token
			if (ProjectSettings.LoginToken != "")
			{
				if (LauncherSettings.PlayerTag != "") lsPass.Password = "PASSWORDHOLDER";
				else ProjectSettings.LoginToken = "";
			}

			//Debug Log
			if (ProjectSettings.DebugLog == 1) psDebug.IsChecked = true;
			else psDebug.IsChecked = false;

			//Ports
			psPorts.IsChecked = true;
			psPortNumber.Foreground = MenuItemSelect;
			psPortNumber.Text = ProjectSettings.Ports.ToString();

			//FPS
			if (ProjectSettings.FPSCap == 1) psFPS.IsChecked = true;
			psFPSLimit.Text = ProjectSettings.FPSLimit.ToString();

			//Voice Chat
			if (ProjectSettings.VoiceChat == 1) psVoice.IsChecked = true;
			else psVoice.IsChecked = false;

			//Map Downloading
			if (ProjectSettings.MapDownload == 1) psMaps.IsChecked = true;
			else psMaps.IsChecked = false;

			//FOV
			psFOV.IsChecked = true;
			psFOVSetting.Foreground = MenuItemSelect;
			psFOVSetting.Text = ProjectSettings.FOV.ToString();

			//Crosshair
			psCrosshair.IsChecked = true;
			psCrosshairSetting.Foreground = MenuItemSelect;
			psCrosshairSetting.Text = ProjectSettings.Reticle;
		}

		public async void SaveSettings()
		{
			StatusButton.Content = "Currently saving configuration files...";
			await Task.Delay(1000);

			//Remember Me
			if (lsUser.Text == "") LauncherSettings.RememberMe = 0;
			else LauncherSettings.RememberMe = (RememberMe) ? 1 : 0;

			//Playertag
			LauncherSettings.PlayerTag = lsUser.Text;

			//DisplayMode
			LauncherSettings.DisplayMode = (Globals.SettingsDisplayMode)Enum.Parse(typeof(Globals.SettingsDisplayMode), DisplayMode.ToString());

			//Game Sound
			LauncherSettings.NoGameSound = (NoGameSound) ? 1 : 0;

			// Vertical Sync
			LauncherSettings.VerticalSync = (Vsync) ? 1 : 0;

			// Default Display
			LauncherSettings.DefaultDisplay = psMonitorSelect.SelectedIndex;

			// Debug Log
			ProjectSettings.DebugLog = (DebugLog) ? 1 : 0;

			// Ports
			if (psPortNumber.Text == "") ProjectSettings.Ports = int.Parse("1000");
			else ProjectSettings.Ports = int.Parse(psPortNumber.Text);

			// FPS Enable
			ProjectSettings.FPSCap = (fpsEnable) ? 1 : 0;
			if (psFPSLimit.Text == "") ProjectSettings.FPSLimit = int.Parse("60");
			else ProjectSettings.FPSLimit = int.Parse(psFPSLimit.Text);

			// Voice Chat
			ProjectSettings.VoiceChat = (VoiceChat) ? 1 : 0;

			// Map Downloading
			LauncherRuntime.ProjectSettings.MapDownload = (MapDownloading) ? 1 : 0;

			// Field of View
			if (psFOVSetting.Text == "") ProjectSettings.FOV = int.Parse("57");
			else ProjectSettings.FOV = int.Parse(psFOVSetting.Text);

			// Reticle
			if (psCrosshairSetting.Text == "") ProjectSettings.Reticle = "0.165";
			else ProjectSettings.Reticle = psCrosshairSetting.Text;

			LauncherSettings.SaveSettings();
			ProjectSettings.SaveSettings();

			LogFile("Settings saved");
			SaveSettingsCheck = false;
			StatusButton.Content = Globals.VersionNumber;
			if (ApplicationShutdownCheck) Application.Current.Shutdown();
		}
		#endregion
	}
}