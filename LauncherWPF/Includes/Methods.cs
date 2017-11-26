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

        public void Error(string Error)
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
            catch (Exception e)
            {
#if DEBUG
                Error("Failed to set premissions for \"" + filename + "\"");
#endif
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
            ProcessStartInfo proc = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName)
            {
                Verb = "runas"
            };

            try
            {
                Process.Start(proc);
            }
            catch (Exception e) {};
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
			ProcessStartInfo Info = new ProcessStartInfo();
			Info.Arguments = Arguments;
			Info.WindowStyle = ProcessWindowStyle.Hidden;
			Info.CreateNoWindow = true;
			Info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Info.FileName = "cmd.exe";
			Process.Start(Info);
			Process.GetCurrentProcess().Kill();
		}

		public void WebServerCheck()
		{
			HttpWebRequest request = WebRequest.Create(Globals.LAUNCHER_CHECK) as HttpWebRequest;
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
