using System.Threading.Tasks;
using H2Launcher.Includes;
using Cartographer_Launcher.Includes.Dependencies;
using Cartographer_Launcher.Includes.Settings;
using LauncherWPF;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes
{
    public static class LauncherRuntime
    {
        private static Launcher _LauncherSettings;
        private static GameRuntime _GameRuntime;
        private static Runtime _Runtime;
        private static WebHandler _WebControl;
        private static ProjectCartographer _ProjectSettings;

        public static Launcher LauncherSettings
        {
            get
            {
                if (_LauncherSettings == null)
                    _LauncherSettings = new Launcher();
                return _LauncherSettings;
            }
        }

        public static ProjectCartographer ProjectSettings
        {
            get
            {
                if (_ProjectSettings == null)
                    _ProjectSettings = new ProjectCartographer();
                return _ProjectSettings;
            }
        }

        public static GameRuntime GameRuntime
        {
            get
            {
                if (_GameRuntime == null)
                    _GameRuntime = new GameRuntime();
                return _GameRuntime;
            }
        }

        public static Runtime Runtime
        {
            get
            {
                if (_Runtime == null)
                    _Runtime = new Runtime();
                return _Runtime;
            }
        }

        public static WebHandler WebControl
        {
            get
            {
                if (_WebControl == null)
                    _WebControl = new WebHandler();
                return _WebControl;
            }
        }

        public static async void StartHalo(string Gamertag, string LoginToken, MainWindow MainForm)
        {
            MainForm.Hide();
            int RunningTicks = 0;

            LauncherSettings.PlayerTag = Gamertag;
            LauncherSettings.LoginToken = LoginToken;
            LauncherSettings.SaveSettings();

            LauncherSettings.SaveSettings();
            await Task.Delay(1);
            //File.WriteAllLines(Paths.InstallPath + "token.ini", new string[] { "token=" + LoginToken, "username=" + Gamertag });
            GameRuntime.RunGame();

            /*
             * Game Running thread ticks every 1 second with a maximum of 15 ticks till reset.
             * 
             * */
            while (Process.GetProcessesByName("halo2").Length == 1)//DURING HALO RUNNING THREAD
            {
                if (RunningTicks == 15) //Check Ban Status every 15 ticks
                {
                    var banResult = WebControl.CheckBan(Gamertag, LoginToken);

                    if (banResult == CheckBanResult.Banned)
                    {
                        GameRuntime.KillGame();
                        MainForm.Topmost = true;
                        MainForm.Focus();
                        MainForm.Topmost = false;
                        if (MessageBox.Show("You have been banned, please visit the forum to appeal your ban.\r\nWould you like us to open the forums for you?.",
                            Kantanomo.PauseIdiomGenerator, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            Process.Start(@"http://www.halo2vista.com/forums/viewforum.php?f=45");
                    }
                }

                #region TickLogic
                if (RunningTicks == 16)
                    RunningTicks = 0;
                else
                    RunningTicks++;
                await Task.Delay(1000);
                #endregion
            }
            MainForm.Show();
        }
    }
}
