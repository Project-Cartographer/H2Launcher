using Cartographer_Launcher.Includes.Settings;
using System.Diagnostics;

namespace Cartographer_Launcher.Includes.Dependencies
{
	public class GameLaunch
	{
		Launcher LauncherSettings = LauncherRuntime.LauncherSettings;
		ProjectCartographer ProjectSettings = LauncherRuntime.ProjectSettings;

		public void RunGame()
		{
			LauncherSettings.LoadSettings();
			ProjectSettings.LoadSettings();

			ProcessStartInfo ProcInfo = new ProcessStartInfo();
			ProcInfo.WorkingDirectory = Globals.GameDirectory;
			ProcInfo.FileName = "halo2.exe";

			switch (LauncherSettings.DisplayMode)
			{
				case Globals.SettingsDisplayMode.Fullscreen:
					{
						ProcInfo.Arguments += " -monitor:" + LauncherSettings.DefaultDisplay.ToString();
						break;
					}
				case Globals.SettingsDisplayMode.Windowed:
					{
						ProcInfo.Arguments += " -windowed";
						ProcInfo.Arguments += " -monitor:" + LauncherSettings.DefaultDisplay.ToString();
						break;
					}
			}

			if (LauncherSettings.NoGameSound == 1) ProcInfo.Arguments += " -nosound";
			if (LauncherSettings.VerticalSync == 0) ProcInfo.Arguments += " -novsync";

			Process.Start(ProcInfo);
		}
	}
}