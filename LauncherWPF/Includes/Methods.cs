using Cartographer_Launcher.Includes.Dependencies;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Cartographer_Launcher.Includes
{
	public class Methods
	{
		public string DateTimeStamp { get; private set; }

		public void LogFile(string Message)
		{
			StreamWriter log;
			if (!File.Exists(Globals.LAUNCHER_LOG_FILE)) log = new StreamWriter(Globals.LAUNCHER_LOG_FILE);
			else log = File.AppendText(Globals.LAUNCHER_LOG_FILE);

			log.WriteLine("Date: " + DateTimeStamp + " - " + Message);

			log.Flush();
			log.Dispose();
			log.Close();
		}

		public void ExLogFile(string Message)
		{
			StreamWriter exlog;
			if (!File.Exists(Globals.LAUNCHER_EXCEPTION_LOG_FILE)) exlog = new StreamWriter(Globals.LAUNCHER_EXCEPTION_LOG_FILE);
			else exlog = File.AppendText(Globals.LAUNCHER_EXCEPTION_LOG_FILE);

			exlog.WriteLine("Date: " + DateTimeStamp);
			exlog.WriteLine(Message);
			exlog.WriteLine(Environment.NewLine);

			exlog.Flush();
			exlog.Dispose();
			exlog.Close();
		}

		public void ErrorMessage(string Error)
		{
			MessageBox.Show(Error, Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public void AllowReadWrite(string filename)
		{
			try
			{
				FileSecurity sec = File.GetAccessControl(filename);

				SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
				sec.AddAccessRule(new FileSystemAccessRule(
					everyone,
					FileSystemRights.Write | FileSystemRights.ReadAndExecute,
					AccessControlType.Allow));

				File.SetAccessControl(filename, sec);
			}
			catch (Exception Ex)
			{
#if DEBUG
				Error("Failed to set premissions for \"" + filename + "\"");
#endif
				ExLogFile(Ex.ToString());
			}
		}

		public static bool IsAdministrator()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		public void RelaunchAsAdmin()
		{
			ProcessStartInfo proc = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName) { Verb = "runas" };
			try { Process.Start(proc); }
			catch (Exception Ex)
			{
				DebugAbort("Process failed to launch.  Please manually re-launch the process");
				ExLogFile(Ex.ToString());
			}
			Environment.Exit(0);
		}

		public void DebugAbort(string Error)
		{
			MessageBoxResult mr = MessageBox.Show(Error, Kantanomo.PauseIdiomGenerator, MessageBoxButton.OK, MessageBoxImage.Error);
			switch (mr)
			{
				case MessageBoxResult.OK:
					Process.Start(Globals.LAUNCHER_EXCEPTION_LOG_FILE);
					Application.Current.Shutdown();
					break;

				case MessageBoxResult.None:
					Application.Current.Shutdown();
					break;
			}
		}

		public void LauncherDelete(string Arguments)
		{
			Task.Delay(1000);
			ProcessStartInfo cmd = new ProcessStartInfo
			{
				Arguments = Arguments,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
				FileName = "cmd.exe"
			};
			Process.Start(cmd);
			Process.GetCurrentProcess().Kill();
		}

		private bool WebServerConnectionCheck()
		{
			HttpWebRequest requestInternetConnection = WebRequest.Create("https://www.cartographer.online/") as HttpWebRequest;
			requestInternetConnection.Method = "HEAD";
			HttpWebResponse responseInternetConnection;

			try
			{
				responseInternetConnection = requestInternetConnection.GetResponse() as HttpWebResponse;
				return true;
			}
			catch (WebException wex)
			{
				responseInternetConnection = wex.Response as HttpWebResponse;
				MessageBoxResult mr = MessageBox.Show("Cannot connect to Project Cartographer webserver." + Environment.NewLine + "Please check your internet connection and try again." + Environment.NewLine + Environment.NewLine + "The launcher will now close.", "CONNECTION ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
				switch (mr)
				{
					case MessageBoxResult.OK:
						Application.Current.Shutdown();
						break;
					case MessageBoxResult.None:
						Application.Current.Shutdown();
						break;
				}
				return false;
			}
		}

		public void SelfDestructButton()
		{
			if (WebServerConnectionCheck())
			{
				HttpWebRequest requestLauncherCheck = WebRequest.Create(Globals.LAUNCHER_CHECK) as HttpWebRequest;
				requestLauncherCheck.Method = "HEAD";
				HttpWebResponse responseLauncherCheck;

				try { responseLauncherCheck = requestLauncherCheck.GetResponse() as HttpWebResponse; }
				catch (WebException wex)
				{
					responseLauncherCheck = wex.Response as HttpWebResponse;
					if (responseLauncherCheck.StatusCode == HttpStatusCode.NotFound)
					{
						responseLauncherCheck.Close();
						MessageBoxResult mr2 = MessageBox.Show("The Launcher to Project Cartographer is depreciated." + Environment.NewLine + "Please run the game from the executable." + Environment.NewLine + Environment.NewLine + "This launcher will now be deleted.", "LAUNCHER ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
						switch (mr2)
						{
							case MessageBoxResult.OK:
								LauncherDelete("/C ping 127.0.0.1 -n 1 -w 100 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"");
								break;
							case MessageBoxResult.None:
								LauncherDelete("/C ping 127.0.0.1 -n 1 -w 100 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"");
								break;
						}
					}
				}
			}
		}

		public void CheckInstallPath()
		{
			if (Globals.GAME_DIRECTORY == "")
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
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) Globals.GAME_DIRECTORY = ofd.FileName.Replace(ofd.SafeFileName, "");
					else Application.Current.Shutdown();
				}
			}
			else
			{
				if (!Directory.Exists(Globals.GAME_DIRECTORY))
				{
					MessageBox.Show("Halo 2 game directory was not found, please relocate the executable.", Kantanomo.GoIdioms, MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK);
					Globals.GAME_DIRECTORY = "";
					CheckInstallPath();
				}
			}
		}
	}
}
