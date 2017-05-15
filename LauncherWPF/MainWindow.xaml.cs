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
		Launcher LauncherSettings = new Launcher();
		ProjectCartographer ProjectSettings = new ProjectCartographer();

		private string RegisterURL = @"http://www.cartographer.h2pc.org/";
		private string AppealURL = @"http://www.halo2vista.com/forums/viewforum.php?f=45";
		private string DateTimeStamp = DateTime.Now.ToString("M/dd/yyyy (HH:mm)");
		private string LogFilePath = Globals.LogFile;
		private string ExLogFilePath = Globals.ExLogFile;
		private string DisplayMode;
		private bool ApplicationShutdownCheck,
			SaveSettingsCheck,
			LoginPanelCheck,
			SettingsPanelCheck,
			UpdatePanelCheck,
			GameSound,
			Vsync,
			DebugLog,
			VoiceChat,
			MapDownloading,
			fpsEnable,
			RememberMe;
		private static bool NoTextInput(string NumericText) { Regex r = new Regex("[^0-9.-]+"); return !r.IsMatch(NumericText); }

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
			InitializeComponent();

			LauncherCheck();
			CheckInstallPath();

			LogFile("Log file initialized.");
			LogFile("Game install directory: " + Globals.GameDirectory);
			LogFile("Launcher file directory: " + Globals.H2vHubDirectory);

			usProgressLabel.Tag = "{0}/100";
			usProgressLabel.Content = "100/100";
			StatusButton.Content = Globals.VersionNumber;

			try { LoadSettings(); }
			catch (Exception Ex) { ExLogFile(Ex.ToString()); }

			var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);
			if (loginResult.LoginResultEnum != LoginResultEnum.Successfull) PlayButton.Content = "LOGIN";
			else PlayButton.Content = "PLAY";

			CheckUpdates();
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
			exlog.WriteLine(Environment.NewLine);

			exlog.Flush();
			exlog.Dispose();
			exlog.Close();
		}

		private void LauncherCheck()
		{
			HttpWebRequest request = WebRequest.Create(Globals.LauncherCheck) as HttpWebRequest;
			request.Method = "HEAD";
			HttpWebResponse response;
			try { response = request.GetResponse() as HttpWebResponse; }
			catch (WebException wex) { response = wex.Response as HttpWebResponse; }

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				Task.Delay(1000);
				ProcessStartInfo Info = new ProcessStartInfo();
				Info.Arguments = "/C ping 127.0.0.1 -n 1 -w 100 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"";
				Info.WindowStyle = ProcessWindowStyle.Hidden;
				Info.CreateNoWindow = true;
				Info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Info.FileName = "cmd.exe";
				Process.Start(Info);
				Process.GetCurrentProcess().Kill();
			}
		}

		public void CheckInstallPath()
		{
			if (Globals.GameDirectory == "")
			{
				MessageBox.Show("The game directory was not found, please locate it to continue.", "", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
				string BaseFolder;

				if (Environment.Is64BitOperatingSystem) BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
				else BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

				using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
				{
					ofd.InitialDirectory = BaseFolder;
					ofd.Title = "Find Halo 2 Game Directory";
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
					MessageBox.Show("The game directory was not found, please locate it to continue.", "", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
					Globals.GameDirectory = "";
					CheckInstallPath();
				}
			}
		}

		private async void LoginTokenCheck()
		{
			StatusButton.Content = "Currently verifying login...";
			await Task.Delay(1000).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);
					if (loginResult.LoginResultEnum == LoginResultEnum.Successfull) ProjectSettings.LoginToken = loginResult.LoginToken;
				});
			});
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
			await Task.Delay(1000).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);
					if (loginResult.LoginResultEnum != LoginResultEnum.Successfull)
					{
						lsUser.Text = "";
						lsPass.Password = "";
						ProjectSettings.LoginToken = "";
						PlayButton.Content = "LOGIN";
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
								MessageBox.Show(this, "This login token is no longer valid." + Environment.NewLine + "Please re -enter your login information and try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								ProjectSettings.LoginToken = "";
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Login token invalid");
								break;
							}
						case LoginResultEnum.InvalidUsernameOrPassword:
							{
								MessageBox.Show(this, "The playertag or password entered is invalid." + Environment.NewLine + "Please try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Player credentials invalid");
								break;
							}
						case LoginResultEnum.Banned:
							{
								if (MessageBox.Show(this, "You have been banned, please visit the forum to appeal your ban." + Environment.NewLine + "Would you like us to open the forums for you?.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
									Process.Start(AppealURL);
								PlayButton.Content = "LOGIN";
								LogFile("Project Cartographer: Machine is banned");
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
								break;
							}
					}
				});
			});
		}

		private void MainForm_Initialized(object sender, EventArgs e)
		{
			if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
			if (File.Exists(ExLogFilePath)) File.Delete(ExLogFilePath);
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
			catch (Exception Ex) { ExLogFile(Ex.ToString()); }
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

		private void ControlMinimize_Click(object sender, RoutedEventArgs e)
		{
			MainForm.WindowState = WindowState.Minimized;
		}

		private void ControlClose_Click(object sender, RoutedEventArgs e)
		{
			MainForm.Visibility = Visibility.Hidden;
			ApplicationShutdownCheck = true;
			try { SaveSettings(); }
			catch (Exception Ex) { ExLogFile(Ex.ToString()); }
		}

		#region Menu Buttons
		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
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
							SaveSettings();
						}
					}
				}
				if (SettingPanel.Margin.Right == 0)
				{
					PanelAnimation("sbHideSettingsMenu", SettingPanel);
					SettingsPanelCheck = false;
					SaveSettingsCheck = true;
					SaveSettings();
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
						SaveSettings();
					}
				}
				if (!SettingsPanelCheck && !UpdatePanelCheck && !LoginPanelCheck && PlayButton.Content.ToString() != "LOGIN" || PlayButton.Content.ToString() == "PLAY") LoginVerification();
			}
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
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
						SaveSettings();
					}
				}
				if (LoginPanelCheck)
				{
					if (LoginPanel.Margin.Top == 0)
					{
						PanelAnimation("sbHideLoginMenu", LoginPanel);
						LoginPanelCheck = false;
						SaveSettingsCheck = true;
						SaveSettings();
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

		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
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
				if (LoginPanelCheck)
				{
					if (LoginPanel.Margin.Top == 0)
					{
						PanelAnimation("sbHideLoginMenu", LoginPanel);
						LoginPanelCheck = false;
						SaveSettingsCheck = true;
						SaveSettings();
					}
					if (UpdatePanel.Margin.Bottom == -250)
					{
						usTextBox.Clear();
						CheckUpdates();
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
						SaveSettings();
					}
					if (UpdatePanel.Margin.Bottom == -250)
					{
						usTextBox.Clear();
						CheckUpdates();
						PanelAnimation("sbShowUpdateMenu", UpdatePanel);
						UpdatePanelCheck = true;
					}
				}
			}
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(RegisterURL);
		}

		private void PanelClose_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveSettingsCheck)
			{
				if (SettingsPanelCheck)
				{
					if (SettingPanel.Margin.Right == 0)
					{
						PanelAnimation("sbHideSettingsMenu", SettingPanel);
						SettingsPanelCheck = false;
						SaveSettingsCheck = true;
						SaveSettings();
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
						LoginTokenCheck();
						SaveSettings();
					}
				}
			}
		}
		#endregion

		#region Updates
		public void AddToDetails(string Message)
		{
			if (usTextBox.Dispatcher.CheckAccess())
			{
				AddToDetailsCallback Update = new AddToDetailsCallback(AddToDetails);
				Dispatcher.Invoke(Update, new object[] { Message });
			}
			else Dispatcher.Invoke(() => { usTextBox.Text += Message + Environment.NewLine; });
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
			await Task.Delay(1000).ContinueWith(_ =>
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

		private bool UpdateGameToLatest()
		{
			string DateStamp = "[" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "]" + Environment.NewLine;
			string CurrentHalo2Version = FileVersionInfo.GetVersionInfo(Globals.GameDirectory + "halo2.exe").FileVersion;
			AddToDetails(string.Format(DateStamp + "Halo 2 Current Version: {0}" + Environment.NewLine + "Halo 2 Expected Version: {1}", CurrentHalo2Version, _Halo2Version + Environment.NewLine));

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
				Task.Delay(5000);
				ProcessStartInfo p = new ProcessStartInfo();
				p.CreateNoWindow = true;
				p.UseShellExecute = false;
				p.FileName = "cmd.exe";
				p.WindowStyle = ProcessWindowStyle.Hidden;
				p.Arguments = "/c ping 127.0.0.1 -n 3 -w 2000 > Nul & Del " + "\"" + CurrentName + "\"" + "& ping 127.0.0.1 -n 1 -w 2000 > Nul & rename H2Launcher_temp.exe H2Launcher.exe & ping 127.0.0.1 -n 1 -w 1000 > Nul & start H2Launcher.exe";
				p.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
		#endregion

		#region Settings Panel
		private void LoadSettings()
		{
			LauncherSettings.LoadSettings();
			ProjectSettings.LoadSettings();
			//
			//Playertag
			//
			if (lsUser.Text != "") lsUsername.IsChecked = true;
			if (lsPass.Password != "") lsPassword.Foreground = MenuItemSelect;
			//
			//Display Mode
			//
			switch (LauncherSettings.DisplayMode)
			{
				case SettingsDisplayMode.Fullscreen:
					{
						psFullScreen.IsChecked = true;
						DisplayMode = "Fullscreen";
						break;
					}
				case SettingsDisplayMode.Windowed:
					{
						psFullScreen.IsChecked = false;
						DisplayMode = "Windowed";
						break;
					}
			}
			//
			//Game Sound
			//
			if (LauncherSettings.GameSound == 1) psSound.IsChecked = true;
			else psSound.IsChecked = false;
			//
			//Vertical Sync
			//
			if (LauncherSettings.VerticalSync == 1) psVsync.IsChecked = true;
			else psVsync.IsChecked = false;
			//
			//Default Display
			//
			for (int s = 0; s < System.Windows.Forms.Screen.AllScreens.Length; s++)
			{
				psMonitorSelect.Items.Add((s + 1).ToString() + ((System.Windows.Forms.Screen.AllScreens[s].Primary) ? "*" : ""));
				if (s == LauncherSettings.DefaultDisplay) psMonitorSelect.SelectedIndex = s;
			}
			//
			//Remember Me
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
			psMonitor.IsChecked = true;
			//
			//Login Token
			//
			if (ProjectSettings.LoginToken != "")
			{
				if (LauncherSettings.PlayerTag != "") lsPass.Password = "PASSWORDHOLDER";
				else ProjectSettings.LoginToken = "";
			}
			//
			//Debug Log
			//
			if (ProjectSettings.DebugLog == 1) psDebug.IsChecked = true;
			else psDebug.IsChecked = false;
			//
			//Ports
			//
			psPorts.IsChecked = true;
			psPortNumber.Foreground = MenuItemSelect;
			psPortNumber.Text = ProjectSettings.Ports.ToString();
			//
			//FPS
			//
			if (ProjectSettings.FPSCap == 1) psFPS.IsChecked = true;
			psFPSLimit.Text = ProjectSettings.FPSLimit.ToString();
			//
			//Voice Chat
			//
			if (ProjectSettings.VoiceChat == 1) psVoice.IsChecked = true;
			else psVoice.IsChecked = false;
			//
			//Map Downloading
			//
			if (ProjectSettings.MapDownload == 1) psMaps.IsChecked = true;
			else psMaps.IsChecked = false;
		}

		public async void SaveSettings()
		{
			StatusButton.Content = "Currently saving configuration files...";
			await Task.Delay(1000);
			if (lsUser.Text == "") LauncherSettings.RememberMe = 0;
			else LauncherSettings.RememberMe = (RememberMe) ? 1 : 0;

			LauncherSettings.PlayerTag = lsUser.Text;
			LauncherSettings.DisplayMode = (SettingsDisplayMode)Enum.Parse(typeof(SettingsDisplayMode), DisplayMode.ToString());
			LauncherSettings.GameSound = (GameSound) ? 1 : 0;
			LauncherSettings.VerticalSync = (Vsync) ? 1 : 0;
			LauncherSettings.DefaultDisplay = psMonitorSelect.SelectedIndex;
			ProjectSettings.DebugLog = (DebugLog) ? 1 : 0;
			ProjectSettings.Ports = int.Parse(psPortNumber.Text);
			ProjectSettings.FPSCap = (fpsEnable) ? 1 : 0;
			ProjectSettings.FPSLimit = int.Parse(psFPSLimit.Text);
			ProjectSettings.VoiceChat = (VoiceChat) ? 1 : 0;
			ProjectSettings.MapDownload = (MapDownloading) ? 1 : 0;

			LauncherSettings.SaveSettings();
			ProjectSettings.SaveSettings();

			LogFile("Settings saved");
			SaveSettingsCheck = false;
			StatusButton.Content = Globals.VersionNumber;
			if (ApplicationShutdownCheck) Application.Current.Shutdown();
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

		private void StatusButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("-Core developers of this launcher-" + Environment.NewLine + Environment.NewLine + "Kantanomo : code base from previous launcher" + Environment.NewLine + "supersniper : current launcher", "The Props", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
			if (StatusButton.IsChecked == true) StatusButton.IsChecked = false;
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
			File.Delete(Globals.Files + "LocalUpdate.xml");
			Task.Delay(5000);
			ProcessStartInfo p = new ProcessStartInfo();
			p.UseShellExecute = false;
			p.Arguments = "/C ping 127.0.0.1 -n 1 -w 2000 > Nul & start H2Launcher.exe";
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
			psFullScreen.IsChecked = true;
			DisplayMode = "Fullscreen";
			LogFile("Display Mode: Full Screen enabled.");
		}

		private void psFullScreen_Unchecked(object sender, RoutedEventArgs e)
		{
			psFullScreen.IsChecked = false;
			DisplayMode = "Windowed";
			LogFile("Display Mode: Full Screen disabled.");
		}
		#endregion
	}
}