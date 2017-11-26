﻿using Cartographer_Launcher.Includes;
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
		H2Startup GSSettings = LauncherRuntime.GSSettings;
		Methods Methods = LauncherRuntime.Methods;

		private const string REGISTER_URL = @"https://www.cartographer.online/";
		private const string APPEAL_URL = @"https://www.halo2.online/forums/the-drama-spot.31/";
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
			RememberMe,
			SkipIntro,
			RawMouseInput,
			DiscordRichPresence;
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
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to load components.");
			}
		}

		#region Main Methods
		private void MessageBoxPanelContent(string Title, string Content)
		{
			mbTitle.Content = Title;
			mbMessage.Text = Content;

		}

		private async void LoginVerification()
		{
			await Task.Delay(1000).ContinueWith(_ =>
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
			await Task.Delay(1200).ContinueWith(_ =>
			{
				Dispatcher.Invoke(() =>
				{
					var loginResult = LauncherRuntime.WebControl.Login(lsUser.Text, lsPass.Password, ProjectSettings.LoginToken);

					switch (loginResult.LoginResultEnum)
					{
						case LoginResultEnum.Successfull:
							{
								StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
								LauncherSettings.PlayerTag = lsUser.Text;
								ProjectSettings.LoginToken = loginResult.LoginToken;
								SaveSettings();

								if (PlayCheck)
								{
									LauncherRuntime.StartHalo(lsUser.Text, loginResult.LoginToken, this);
									Methods.LogFile("Login successful, game starting...");
									PlayCheck = false;
								}
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.InvalidLoginToken:
							{
								StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
								MessageBox.Show(this, "This login token is no longer valid." + Environment.NewLine + "Please enter your login credentials and try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								ProjectSettings.LoginToken = "";
								lsPass.Password = "";
								PlayButton.Content = "LOGIN";
								Methods.LogFile("Project Cartographer: Login token invalid");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.InvalidUsernameOrPassword:
							{
								StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
								MessageBox.Show(this, "The playertag or password entered is invalid." + Environment.NewLine + "Please try again.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning);
								lsUser.Text = "";
								lsPass.Password = "";
								ProjectSettings.LoginToken = "";
								PlayButton.Content = "LOGIN";
								Methods.LogFile("Project Cartographer: Player credentials invalid");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.Banned:
							{
								StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
								if (MessageBox.Show(this, "You have been banned, please visit the forum to appeal your ban." + Environment.NewLine + "Would you like us to open the forums for you?.", Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
									Process.Start(APPEAL_URL);
								ProjectSettings.LoginToken = "";
								PlayButton.Content = "LOGIN";
								Methods.LogFile("Project Cartographer: Machine is banned");
								PlayCheck = false;
								break;
							}
						case LoginResultEnum.GenericFailure:
							{
								StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
								if (PlayCheck && LoginPanel.Margin.Top == -140)
								{
									PanelAnimation("sbShowLoginMenu", LoginPanel);
									LoginPanelCheck = true;
								}
								PlayButton.Content = "LOGIN";
								Methods.LogFile("Project Cartographer: General login failure");
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
			if (File.Exists(Globals.LAUNCHER_LOG_FILE)) File.Delete(Globals.LAUNCHER_LOG_FILE);
			if (File.Exists(Globals.LAUNCHER_EXCEPTION_LOG_FILE)) File.Delete(Globals.LAUNCHER_EXCEPTION_LOG_FILE);

			MessageBoxPanelContent("", "");
			MessageBoxPanel.Visibility = Visibility.Hidden;
			mbMessage.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

			Methods.LogFile("Log file initialized.");
			Methods.LogFile("Game install directory: " + Globals.GAME_DIRECTORY);
			Methods.LogFile("Launcher file directory: " + Globals.H2V_HUB_DIRECTORY);

			try { Methods.WebServerCheck(); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to check launcher.");
			}

			try { Methods.CheckInstallPath(); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to open Windows Explorer.");
			}

			try { LoadSettings(); }
			catch (Exception Ex)
			{
				if (File.Exists(Globals.FILES_DIRECTORY + "Settings.ini")) File.Delete(Globals.FILES_DIRECTORY + "Settings.ini");
				if (File.Exists(Globals.GAME_DIRECTORY + "xlive.ini")) File.Delete(Globals.GAME_DIRECTORY + "xlive.ini");
				if (File.Exists(Globals.GAME_DIRECTORY + "h2startup1.ini")) File.Delete(Globals.GAME_DIRECTORY + "h2startup1.ini");
				Methods.ExLogFile(Ex.ToString());
				Methods.Error("Failed to load setting files.");
			}

			try { LoginVerification(); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to verify login token.");
			}

			try { CheckUpdates(); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to begin update process");
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
			SaveSettings();
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
			SaveSettings();
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

						try { LoginVerification(); }
						catch (Exception Ex)
						{
							Methods.ExLogFile(Ex.ToString());
							Methods.DebugAbort("Failed to verify login token.");
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
						if (!SettingsPanelCheck && !UpdatePanelCheck && !LoginPanelCheck && PlayButton.Content.ToString() != "LOGIN" || PlayButton.Content.ToString() == "PLAY")
						{
							PlayCheck = true;

							try { LoginVerification(); }
							catch (Exception Ex)
							{
								Methods.ExLogFile(Ex.ToString());
								Methods.DebugAbort("Failed to get login token.");
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
								Methods.ExLogFile(Ex.ToString());
								Methods.DebugAbort("Failed to begin update process.");
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
							SaveSettings();
						}
						if (UpdatePanel.Margin.Bottom == -250)
						{
							usTextBox.Clear();

							try { CheckUpdates(); }
							catch (Exception Ex)
							{
								Methods.ExLogFile(Ex.ToString());
								Methods.DebugAbort("Failed to begin update process.");
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
							SaveSettings();
						}
						if (UpdatePanel.Margin.Bottom == -250)
						{
							usTextBox.Clear();

							try { CheckUpdates(); }
							catch (Exception Ex)
							{
								Methods.ExLogFile(Ex.ToString());
								Methods.DebugAbort("Failed to begin update process.");
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
			Process.Start(REGISTER_URL);
		}
		#endregion

		#region Update Global Definitions
		private bool UpdateGameToLatest()
		{
			string DateStamp = "[" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + "]" + Environment.NewLine;
			string CurrentHalo2Version = FileVersionInfo.GetVersionInfo(Globals.GAME_DIRECTORY + "halo2.exe").FileVersion;

			try { AddToDetails(string.Format(DateStamp + "Halo 2 Current Version: {0}" + Environment.NewLine + "Halo 2 Expected Version: {1}", CurrentHalo2Version, _Halo2Version + Environment.NewLine)); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Failed to get current version of Halo 2");
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
					Client.DownloadFileAsync(new Uri(Globals.REMOTE_UPDATE_DIRECTORY + "halo2/Update.exe"), Globals.DOWNLOADS_DIRECTORY + "\\Update.exe");
					_isDownloading = true;
				}
				catch (Exception) { throw new Exception("Error"); }

				while (_isDownloading) { }
				AddToDetails("Waiting for updates to finish installing." + Environment.NewLine);

				bool _isUpdating = true;
				Process.Start(Globals.DOWNLOADS_DIRECTORY + "\\Update.exe");

				while (_isUpdating) if (Process.GetProcessesByName("Update").Length == 0) _isUpdating = false;

				File.Delete(Globals.DOWNLOADS_DIRECTORY + "\\Update.exe");
				return true;
			}
			return true;
		}

		public bool LoadLocalUpdateCollection()
		{
			try
			{
				if (File.Exists(Globals.FILES_DIRECTORY + "LocalUpdate.xml"))
				{
					XDocument RemoteXML = XDocument.Load(Globals.FILES_DIRECTORY + "LocalUpdate.xml");
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
				if (File.Exists(Globals.FILES_DIRECTORY + "RemoteUpdate.xml")) File.Delete(Globals.FILES_DIRECTORY + "RemoteUpdate.xml");

				Client.DownloadFileCompleted += (s, e) =>
				{
					UpdateProgress(100);
					AddToDetails("Remote update XML file download complete." + Environment.NewLine);

					Client.Dispose();
					Client = null;
					_isDownloading = false;
				};

				Client.DownloadProgressChanged += (s, e) => { UpdateProgress(e.ProgressPercentage); };
				Client.DownloadFileAsync(new Uri(Globals.REMOTE_UPDATE_XML_FILE), Globals.FILES_DIRECTORY + "RemoteUpdate.xml");
				_isDownloading = true;
				while (_isDownloading) { }
				XDocument RemoteXML = XDocument.Load(Globals.FILES_DIRECTORY + "RemoteUpdate.xml");
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
			if (File.Exists(Globals.FILES_DIRECTORY + "LocalUpdate.xml")) File.Delete(Globals.FILES_DIRECTORY + "LocalUpdate.xml");
			File.Move(Globals.FILES_DIRECTORY + "RemoteUpdate.xml", Globals.FILES_DIRECTORY + "LocalUpdate.xml");

			if (_LauncherUpdated)
			{
				AddToDetails("The launcher needs to restart to complete the update.");
				Task.Delay(4000);
				Methods.LauncherDelete("/c ping 127.0.0.1 -n 3 -w 2000 > Nul & Del " + "\"" + CurrentName + "\"" + "& ping 127.0.0.1 -n 1 -w 2000 > Nul & rename H2Launcher_temp.exe H2Launcher.exe & ping 127.0.0.1 -n 1 -w 1000 > Nul & start H2Launcher.exe");
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

		#region Settings: Misc. Events
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
				File.Delete(Globals.FILES_DIRECTORY + "LocalUpdate.xml");
				Methods.LauncherDelete("/C ping 127.0.0.1 -n 1 -w 2000 > Nul & start H2Launcher.exe");
			}
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("Local update file not found.");
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
			Process.Start(Globals.GAME_DIRECTORY);
			psGameDirectory.IsChecked = false;
		}

		private void psGameDirectory_Unchecked(object sender, RoutedEventArgs e)
		{
			psGameDirectory.IsChecked = false;
		}
		#endregion

		#region Settings: Launcher
		private void psWindow_Checked(object sender, RoutedEventArgs e)
		{
			psWindow.IsChecked = true;
			DisplayMode = "Windowed";
			GameRegistrySettings.SetDisplayMode(false);
			Methods.LogFile("Display Mode: Window mode enabled.");
		}

		private void psWindow_Unchecked(object sender, RoutedEventArgs e)
		{
			psWindow.IsChecked = false;
			DisplayMode = "Fullscreen";
			GameRegistrySettings.SetDisplayMode(true);
			Methods.LogFile("Display Mode: Window mode disabled.");
		}

		private void psNoSound_Checked(object sender, RoutedEventArgs e)
		{
			NoGameSound = true;
			Methods.LogFile("Halo 2 Launch Parameter: -nosound added to game launch.");
		}

		private void psNoSound_Unchecked(object sender, RoutedEventArgs e)
		{
			NoGameSound = false;
			Methods.LogFile("Halo 2 Launch Parameter: -nosound removed from game launch.");
		}

		private void psVsync_Checked(object sender, RoutedEventArgs e)
		{
			Vsync = true;
			Methods.LogFile("Halo 2 Launch Parameter: -novsync removed from game launch.");
		}

		private void psVsync_Unchecked(object sender, RoutedEventArgs e)
		{
			Vsync = false;
			Methods.LogFile("Halo 2 Launch Parameter: -novsync added to game launch.");
		}

		private void psMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Methods.LogFile("Halo 2 Launch Parameter: -monitor:" + psMonitorSelect.SelectedIndex.ToString());
		}

		private void psMonitor_Unchecked(object sender, RoutedEventArgs e)
		{
			psMonitor.IsChecked = true;
		}

		private void lsRememberMe_Checked(object sender, RoutedEventArgs e)
		{
			RememberMe = true;
			Methods.LogFile("H2Launcher Setting: Autologin enabled.");
		}

		private void lsRememberMe_Unchecked(object sender, RoutedEventArgs e)
		{
			RememberMe = false;
			Methods.LogFile("H2Launcher Setting: Autologin disabled.");
		}
		#endregion

		#region Settings: Project Cartographer
		private void psDebug_Checked(object sender, RoutedEventArgs e)
		{
			DebugLog = true;
			Methods.LogFile("Project Cartographer: Trace logs enabled.");
		}

		private void psDebug_Unchecked(object sender, RoutedEventArgs e)
		{
			DebugLog = false;
			Methods.LogFile("Project Cartographer: Trace logs disabled.");
		}

		private void psPorts_Unchecked(object sender, RoutedEventArgs e)
		{
			psPorts.IsChecked = true;
		}

		private void psPortNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			Methods.LogFile("Project Cartographer: Game base port set to " + psPortNumber.Text.ToString());
		}

		private void psFPS_Checked(object sender, RoutedEventArgs e)
		{
			fpsEnable = true;
			psFPSLimit.IsEnabled = true;
			psFPSLimit.Foreground = MenuItemSelect;
			Methods.LogFile("Project Cartographer: FPS limiter enabled.");
		}

		private void psFPS_Unchecked(object sender, RoutedEventArgs e)
		{
			fpsEnable = false;
			psFPSLimit.IsEnabled = false;
			Methods.LogFile("Project Cartographer: FPS limiter disabled.");
		}

		private void psRawMouseInput_Checked(object sender, RoutedEventArgs e)
		{
			RawMouseInput = true;
			Methods.LogFile("Project Cartographer: Raw mouse input enabled.");
		}

		private void psRawMouseInput_Unchecked(object sender, RoutedEventArgs e)
		{
			RawMouseInput = false;
			Methods.LogFile("Project Cartographer: Raw mouse input disabled.");
		}

		private void psDiscordRichPresence_Checked(object sender, RoutedEventArgs e)
		{
			DiscordRichPresence = true;
			Methods.LogFile("Project Cartographer: Discord rich presence is enabled.");
		}

		private void psDiscordRichPresence_Unchecked(object sender, RoutedEventArgs e)
		{
			DiscordRichPresence = false;
			Methods.LogFile("Project Cartographer: Discord rich presence is disabled.");
		}

		private void psFPSLimit_TextChanged(object sender, TextChangedEventArgs e)
		{
			Methods.LogFile("Project Cartographer: Maximum frames set to " + psFPSLimit.Text.ToString());
		}

		private void psVoice_Checked(object sender, RoutedEventArgs e)
		{
			VoiceChat = true;
			Methods.LogFile("Project Cartographer: Voice chat enabled.");
		}

		private void psVoice_Unchecked(object sender, RoutedEventArgs e)
		{
			VoiceChat = false;
			Methods.LogFile("Project Cartographer: Voice chat disabled.");
		}

		private void psMaps_Checked(object sender, RoutedEventArgs e)
		{
			MapDownloading = true;
			Methods.LogFile("Project Cartographer: Custom map downloading enabled.");
		}

		private void psMaps_Unchecked(object sender, RoutedEventArgs e)
		{
			MapDownloading = false;
			Methods.LogFile("Project Cartographer: Custom map downloading disabled");
		}

		private void psFOV_Unchecked(object sender, RoutedEventArgs e)
		{
			psFOV.IsChecked = true;
		}

		private void psFOVSetting_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (psFOVSetting.Text == "" || int.Parse(psFOVSetting.Text) <= 0 || int.Parse(psFOVSetting.Text) >= 115) psFOVSetting.Text = "";
			Methods.LogFile("Project Cartographer: Game FOV changed to " + psFOVSetting.Text);
		}

		private void psCrosshair_Unchecked(object sender, RoutedEventArgs e)
		{
			psCrosshair.IsChecked = true;
		}

		private void psCrosshairSetting_TextChanged(object sender, TextChangedEventArgs e)
		{
			Methods.LogFile("Project Cartographer: Game reticle position changed to " + psCrosshairSetting.Text);
		}
		#endregion

		#region Settings: H2Startup
		private void psSkipIntro_Checked(object sender, RoutedEventArgs e)
		{
			SkipIntro = true;
			Methods.LogFile("Project Cartographer: Startup credits enabled.");
		}

		private void psSkipIntro_Unchecked(object sender, RoutedEventArgs e)
		{
			SkipIntro = false;
			Methods.LogFile("Project Cartographer: Startup credits disabled.");
		}

		private void psLanguage_Unchecked(object sender, RoutedEventArgs e)
		{
			psLanguage.IsChecked = true;
		}

		private void psLanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Methods.LogFile("Project Cartographer: Game language changed to " + psLanguageSelect.SelectedIndex.ToString());
		}
		#endregion

		#region Settings: Load/Save
		private void LoadSettings()
		{
			LauncherSettings.LoadSettings();
			ProjectSettings.LoadSettings();
			GSSettings.LoadSettings();

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

			//Startup Credit Videos
			if (GSSettings.SkipIntro == 1) psSkipIntro.IsChecked = true;
			else psSkipIntro.IsChecked = false;

			//Raw Mouse Input
			if (ProjectSettings.RawMouseInput == 1) psRawMouseInput.IsChecked = true;
			else psRawMouseInput.IsChecked = false;

			//Discord Rich Presence
			if (ProjectSettings.DiscordRichPresence == 1) psDiscordRichPresence.IsChecked = true;
			else psDiscordRichPresence.IsChecked = false;

			//Language
			psLanguage.IsChecked = true;
			switch (GSSettings.LanguageSelect)
			{
				case -1:
					{
						psLanguageSelect.SelectedIndex = 0;
						break;
					}
				case 0:
					{
						psLanguageSelect.SelectedIndex = 1;
						break;
					}
				case 1:
					{
						psLanguageSelect.SelectedIndex = 2;
						break;
					}
				case 2:
					{
						psLanguageSelect.SelectedIndex = 3;
						break;
					}
				case 3:
					{
						psLanguageSelect.SelectedIndex = 4;
						break;
					}
				case 4:
					{
						psLanguageSelect.SelectedIndex = 5;
						break;
					}
				case 5:
					{
						psLanguageSelect.SelectedIndex = 6;
						break;
					}
				case 6:
					{
						psLanguageSelect.SelectedIndex = 7;
						break;
					}
				case 7:
					{
						psLanguageSelect.SelectedIndex = 8;
						break;
					}
			}
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

			//Vertical Sync
			LauncherSettings.VerticalSync = (Vsync) ? 1 : 0;

			//Default Display
			LauncherSettings.DefaultDisplay = psMonitorSelect.SelectedIndex;

			//Debug Log
			ProjectSettings.DebugLog = (DebugLog) ? 1 : 0;

			//Ports
			if (psPortNumber.Text == "") ProjectSettings.Ports = int.Parse("1000");
			else ProjectSettings.Ports = int.Parse(psPortNumber.Text);

			//FPS Enable
			ProjectSettings.FPSCap = (fpsEnable) ? 1 : 0;
			if (psFPSLimit.Text == "") ProjectSettings.FPSLimit = int.Parse("60");
			else ProjectSettings.FPSLimit = int.Parse(psFPSLimit.Text);

			//Voice Chat
			ProjectSettings.VoiceChat = (VoiceChat) ? 1 : 0;

			//Map Downloading
			ProjectSettings.MapDownload = (MapDownloading) ? 1 : 0;

			//Field of View
			if (psFOVSetting.Text == "") ProjectSettings.FOV = int.Parse("57");
			else ProjectSettings.FOV = int.Parse(psFOVSetting.Text);

			// Reticle
			if (psCrosshairSetting.Text == "") ProjectSettings.Reticle = "0.165";
			else ProjectSettings.Reticle = psCrosshairSetting.Text;

			//Skip Startup Credit Videos
			GSSettings.SkipIntro = (SkipIntro) ? 1 : 0;

			//Raw Mouse Input
			ProjectSettings.RawMouseInput = (RawMouseInput) ? 1 : 0;

			//Discord Rich Presence
			ProjectSettings.DiscordRichPresence = (DiscordRichPresence) ? 1 : 0;

			//Language
			GSSettings.LanguageSelect = psLanguageSelect.SelectedIndex -1;

			LauncherSettings.SaveSettings();
			ProjectSettings.SaveSettings();
			GSSettings.SaveSettings();

			Methods.LogFile("Settings saved");
			SaveSettingsCheck = false;
			StatusButton.Content = Globals.LAUNCHER_RELEASE_VERSION;
			if (ApplicationShutdownCheck) Application.Current.Shutdown();
		}
		#endregion
	}
}
