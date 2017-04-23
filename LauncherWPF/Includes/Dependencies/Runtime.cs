using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes.Dependencies
{
    public class Runtime
    {
        Settings.Launcher Launcher = new Settings.Launcher();
        private Dictionary<string, object[]> PostCommands = new Dictionary<string, object[]>();

        #region DLL Imports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        #endregion

        public void AddCommand(string Command, object[] Params)
        {
            PostCommands.Add(Command, Params);
        }

        public void AddCommand(string Command)
        {
            PostCommands.Add(Command, new object[] { });
        }

        public async void RunCommands()
        {
            while (Process.GetProcessesByName("halo2").Length == 0) { }
            //Hangs the process till XLive is loaded into the game.
            bool t = false;
            do
            {
                try
                {
                    ProcessModuleCollection pm = Process.GetProcessesByName("halo2")[0].Modules;
                    var query = from ProcessModule m in pm where m.FileName.EndsWith("ntdll.dll").ToString() == "True" select m;
                    if (query.ToArray().Length > 0)
                        t = true;
                }
                catch (Exception) { }
            } while (!t);
            await Task.Delay(6000);
            foreach (string Command in PostCommands.Keys)
            {

                switch (Command)
                {
                    case "SetWindowResolution":
                        {
                            SetWindowResolution((int)PostCommands[Command][0], (int)PostCommands[Command][1]);
                            break;
                        }
                    case "SetWindowBorderless":
                        {
                            SetWindowBorderless();
                            break;
                        }
                }
            }
            this.PostCommands = new Dictionary<string, object[]>();
        }

        private void SetWindowResolution(int Width, int Height)
        {
            int ScreenWidthMid = ((Screen.AllScreens[Launcher.DefaultDisplay].Bounds.Width / 2));
            int ScreenHeightMid = (Screen.AllScreens[Launcher.DefaultDisplay].Bounds.Height / 2);
            int ResolutionWidthMid = (Launcher.ResolutionWidth / 2);
            int ResolutionHeightMid = (Launcher.ResolutionHeight / 2);
            int ScreenWidthOffset = Screen.AllScreens[Launcher.DefaultDisplay].Bounds.X;
            //SetWindowPos(FindWindow(null, "Halo 2"), IntPtr.Zero, (((Screen.PrimaryScreen.Bounds.Width / 2) - (H2Launcher.LauncherSettings.ResolutionWidth) / 2)), (((Screen.PrimaryScreen.Bounds.Height / 2) - (H2Launcher.LauncherSettings.ResolutionHeight / 2))), H2Launcher.LauncherSettings.ResolutionWidth, H2Launcher.LauncherSettings.ResolutionHeight, 0);

            SetWindowPos(FindWindow(null, "Halo 2"), 
                IntPtr.Zero, 
                (ScreenWidthOffset + (ScreenWidthMid - ResolutionWidthMid)), 
                ((ScreenHeightMid - ResolutionHeightMid)), 
                Launcher.ResolutionWidth, 
                Launcher.ResolutionHeight, 0);
        }

        private void SetWindowBorderless()
        {
            SetWindowLong(FindWindow(null, "Halo 2"), -16, (((GetWindowLong(FindWindow(null, "Halo 2"), -16)) & ~(0x00040000 | 0x00800000 | (0x00800000 | 0x00400000)))));
        }
    }
}
