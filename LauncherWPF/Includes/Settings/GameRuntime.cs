using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartographer_Launcher.Includes.Dependencies;

namespace Cartographer_Launcher.Includes.Settings
{
    public class GameRuntime
    {
        Launcher Launcher = new Launcher();
        ProjectCartographer Project = new ProjectCartographer();
        Runtime Runtime = new Runtime();

        public void RunGame()
        {
            ProcessStartInfo ProcInfo = new ProcessStartInfo();
            ProcInfo.WorkingDirectory = Globals.GameDirectory;
            ProcInfo.FileName = "halo2.exe";

            GameRegistrySettings.SetScreenResX(Launcher.ResolutionWidth);
            GameRegistrySettings.SetScreenResY(Launcher.ResolutionHeight);

            switch (Launcher.DisplayMode)
            {
                case SettingsDisplayMode.Windowed:
                    {
                        ProcInfo.Arguments += "-windowed ";
                        GameRegistrySettings.SetDisplayMode(false);
                        Runtime.AddCommand("SetWindowResolution", new object[] { Launcher.ResolutionWidth, Launcher.ResolutionHeight });
                        break;
                    }
                case SettingsDisplayMode.Fullscreen:
                    {
                        GameRegistrySettings.SetDisplayMode(true);
                        ProcInfo.Arguments += "-monitor:" + Launcher.DefaultDisplay.ToString() + " ";
                        break;
                    }
                case SettingsDisplayMode.Borderless:
                    {
                        ProcInfo.Arguments += "-windowed ";
                        GameRegistrySettings.SetDisplayMode(false);
                        Runtime.AddCommand("SetWindowBorderless");
                        Runtime.AddCommand("SetWindowResolution", new object[] { Launcher.ResolutionWidth, Launcher.ResolutionHeight });
                        break;
                    }

            }
            Project.SaveSettings();
            Launcher.SaveSettings();

            if (!Launcher.GameSound)
                ProcInfo.Arguments += "-nosound ";
            if (!Launcher.VerticalSync)
                ProcInfo.Arguments += "-novsync ";

            Process.Start(ProcInfo);
            Runtime.RunCommands();
        }

        public void KillGame()
        {
            foreach (Process P in Process.GetProcessesByName("halo2"))
                P.Kill();
        }
    }
}
