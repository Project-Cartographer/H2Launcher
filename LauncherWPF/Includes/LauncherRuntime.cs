using System.Threading.Tasks;
using H2Shield.Includes;
using Cartographer_Launcher.Includes.Dependencies;
using Cartographer_Launcher.Includes.Settings;
using LauncherWPF;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes
{
    public static class LauncherRuntime
    {
        private static Launcher _LauncherSettings;
        private static GameRuntime _GameRuntime;
        private static WebHandler _WebControl;
        private static ProjectCartographer _ProjectSettings;
        private static MainWindow _MainWindow;

        public static Launcher LauncherSettings
        {
            get
            {
                if (_LauncherSettings == null) _LauncherSettings = new Launcher();
                return _LauncherSettings;
            }
        }

        public static ProjectCartographer ProjectSettings
        {
            get
            {
                if (_ProjectSettings == null) _ProjectSettings = new ProjectCartographer();
                return _ProjectSettings;
            }
        }

        public static MainWindow MainWindow
        {
            get
            {
                if (_MainWindow == null) _MainWindow = new MainWindow();
                return _MainWindow;
            }
        }
        public static GameRuntime GameRuntime
        {
            get
            {
                if (_GameRuntime == null) _GameRuntime = new GameRuntime();
                return _GameRuntime;
            }
        }

        public static WebHandler WebControl
        {
            get
            {
                if (_WebControl == null) _WebControl = new WebHandler();
                return _WebControl;
            }
        }

        public static async void StartHalo(string Gamertag, string LoginToken, MainWindow MainForm)
        {
            MainForm.Hide();
            int RunningTicks = 0;

			LauncherSettings.PlayerTag = Gamertag;
            ProjectSettings.LoginToken = LoginToken;

			LauncherSettings.SaveSettings();
			ProjectSettings.SaveSettings();

			await Task.Delay(500).ContinueWith(_ => { MainForm.Dispatcher.Invoke(() => { GameRuntime.RunGame(); }); });

            while (Process.GetProcessesByName("halo2").Length == 1)
            {
				#region TickLogic
				if (RunningTicks == 16) RunningTicks = 0;
				else RunningTicks++;
				await Task.Delay(1000);
				#endregion

				if (RunningTicks == 15) 
                {
                    var banResult = WebControl.CheckBan(Gamertag, LoginToken);

                    if (banResult == CheckBanResult.Banned)
                    {
                        GameRuntime.KillGame();
                        MainForm.Topmost = true;
                        MainForm.Focus();
                        MainForm.Topmost = false;
                        if (MessageBox.Show("You have been banned, please visit the forum to appeal your ban." + Environment.NewLine + "Would you like us to open the forums for you?.",
                            Kantanomo.PauseIdiomGenerator, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            Process.Start(@"http://www.halo2vista.com/forums/viewforum.php?f=45");
                    }
                }

            }
            MainForm.Show();
        }
    }
}
