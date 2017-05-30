using Cartographer_Launcher.Includes.Dependencies;
using System.Diagnostics;

namespace Cartographer_Launcher.Includes.Settings
{
	public class GameRuntime
	{
		Launcher LauncherSettings = new Launcher();
		ProjectCartographer ProjectSettings = new ProjectCartographer();

		public void RunGame()
		{
			LauncherSettings.LoadSettings();
			ProjectSettings.LoadSettings();
			ProcessStartInfo ProcInfo = new ProcessStartInfo();
			ProcInfo.WorkingDirectory = Globals.GameDirectory;
			ProcInfo.FileName = "halo2.exe";

			switch (LauncherSettings.DisplayMode)
			{
				case SettingsDisplayMode.Fullscreen:
					{
						ProcInfo.Arguments += " -monitor:" + LauncherSettings.DefaultDisplay.ToString();
						break;
					}
				case SettingsDisplayMode.Windowed:
					{
						ProcInfo.Arguments += " -windowed";
						break;
					}
			}

			if (LauncherSettings.GameSound == 1) ProcInfo.Arguments += " -nosound";
			if (LauncherSettings.VerticalSync == 0) ProcInfo.Arguments += " -novsync";

			Process.Start(ProcInfo);
		}

		public void KillGame()
		{
			foreach (Process P in Process.GetProcessesByName("halo2")) P.Kill();
		}
	}
}