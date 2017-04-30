using System.Diagnostics;
using Cartographer_Launcher.Includes.Dependencies;

namespace Cartographer_Launcher.Includes.Settings
{
    public class GameRuntime
    {
        Launcher LauncherSettings = new Launcher();
        ProjectCartographer ProjectSettings = new ProjectCartographer();
        Runtime Runtime = new Runtime();

        public void RunGame()
        {
            LauncherSettings.LoadSettings();
            ProjectSettings.LoadSettings();
            ProcessStartInfo ProcInfo = new ProcessStartInfo();
            ProcInfo.WorkingDirectory = Globals.GameDirectory;
            ProcInfo.FileName = "halo2.exe";

            GameRegistrySettings.SetScreenResX(LauncherSettings.ResolutionWidth);
            GameRegistrySettings.SetScreenResY(LauncherSettings.ResolutionHeight);

            switch (LauncherSettings.DisplayMode)
            {
                case SettingsDisplayMode.Windowed:
                    {
                        ProcInfo.Arguments += "-windowed ";
                        GameRegistrySettings.SetDisplayMode(false);
                        Runtime.AddCommand("SetWindowResolution", new object[] { LauncherSettings.ResolutionWidth, LauncherSettings.ResolutionHeight });
                        break;
                    }
                case SettingsDisplayMode.Fullscreen:
                    {
                        GameRegistrySettings.SetDisplayMode(true);
                        ProcInfo.Arguments += "-monitor:" + LauncherSettings.DefaultDisplay.ToString() + " ";
                        break;
                    }
                case SettingsDisplayMode.Borderless:
                    {
                        ProcInfo.Arguments += "-windowed ";
                        GameRegistrySettings.SetDisplayMode(false);
                        Runtime.AddCommand("SetWindowBorderless");
                        Runtime.AddCommand("SetWindowResolution", new object[] { LauncherSettings.ResolutionWidth, LauncherSettings.ResolutionHeight });
                        break;
                    }

            }
            ProjectSettings.SaveSettings();
            LauncherSettings.SaveSettings();

            if (LauncherSettings.GameSound == 0) ProcInfo.Arguments += "-nosound ";
            if (LauncherSettings.VerticalSync == 0) ProcInfo.Arguments += "-novsync ";

            Process.Start(ProcInfo);
            Runtime.RunCommands();
        }

        public void KillGame()
        {
            foreach (Process P in Process.GetProcessesByName("halo2")) P.Kill();
        }
    }
}
