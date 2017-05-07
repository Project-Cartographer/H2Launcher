using System.Diagnostics;
using Cartographer_Launcher.Includes.Dependencies;

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
                case SettingsDisplayMode.Windowed:
                    {
                        ProcInfo.Arguments += "-windowed ";
                        GameRegistrySettings.SetDisplayMode(false);
                        break;
                    }
                case SettingsDisplayMode.Fullscreen:
                    {
                        GameRegistrySettings.SetDisplayMode(true);
                        ProcInfo.Arguments += "-monitor:" + LauncherSettings.DefaultDisplay.ToString() + " ";
                        break;
                    }
            }
            ProjectSettings.SaveSettings();
            LauncherSettings.SaveSettings();

            if (LauncherSettings.GameSound == 0) ProcInfo.Arguments += "-nosound ";
            if (LauncherSettings.VerticalSync == 0) ProcInfo.Arguments += "-novsync ";

            Process.Start(ProcInfo);
        }

        public void KillGame()
        {
            foreach (Process P in Process.GetProcessesByName("halo2")) P.Kill();
        }
    }
}
