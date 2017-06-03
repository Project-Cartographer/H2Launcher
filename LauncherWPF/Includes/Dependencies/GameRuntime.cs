using Cartographer_Launcher.Includes.Settings;
using System.Diagnostics;

namespace Cartographer_Launcher.Includes.Dependencies
{
	public class GameLaunch
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
				case Globals.SettingsDisplayMode.Fullscreen:
					{
						ProcInfo.Arguments += " -monitor:" + LauncherSettings.DefaultDisplay.ToString();
						break;
					}
				case Globals.SettingsDisplayMode.Windowed:
					{
						ProcInfo.Arguments += " -windowed";
						break;
					}
			}

			if (LauncherSettings.NoGameSound == 1) ProcInfo.Arguments += " -nosound";
			if (LauncherSettings.VerticalSync == 0) ProcInfo.Arguments += " -novsync";

			Process.Start(ProcInfo);
		}
	}
}