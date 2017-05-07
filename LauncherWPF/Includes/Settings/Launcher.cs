using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Cartographer_Launcher.Includes.Dependencies;
using H2Shield.Includes;

namespace Cartographer_Launcher.Includes.Settings
{
    public class Launcher
    {
        Encryption Encrypt = new Encryption();
        private string _GameDirectory = Globals.GameDirectory;
        private string _PlayerTag = "Player_1";
        private string _PlayerLoginToken = "";
        private SettingsDisplayMode _DisplayMode = GameRegistrySettings.GetDisplayMode();
        private int _GameSound = 1;
        private int _VerticalSync = 1;
        private int _DefaultDisplay = 0;
        private int _StartupCredits = 1;
        private int _RememberMe = 0;

        public SettingsDisplayMode DisplayMode
        {
            get { return _DisplayMode; }
            set { _DisplayMode = value; }
        }

        public int GameSound
        {
            get { return _GameSound; }
            set { _GameSound = value; }
        }

        public int VerticalSync
        {
            get { return _VerticalSync; }
            set { _VerticalSync = value; }
        }

        public int DefaultDisplay
        {
            get { return _DefaultDisplay; }
            set { _DefaultDisplay = value; }
        }

        public int StartupCredits
        {
            get { return _StartupCredits; }
            set { _StartupCredits = value; }
        }

        public int RememberMe
        {
            get { return _RememberMe; }
            set { _RememberMe = value; }
        }

        public string PlayerLoginToken
        {
            get { return _PlayerLoginToken; }
            set { _PlayerLoginToken = value; }
        }

        public string GameDirectory
        {
            get { return _GameDirectory; }
            set { _GameDirectory = value; }
        }

        public string PlayerTag
        {
            get { return _PlayerTag; }
            set { _PlayerTag = value; }
        }

        private int GetDefaultDisplay()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
                if (Screen.AllScreens[i].Primary)
                    return i;
            return 0;
        }

        public void LoadSettings()
        {
            if (!File.Exists(Globals.Files + "Settings.ini")) SaveSettings();
            else
            {
                StreamReader SR = new StreamReader(Globals.Files + "Settings.ini");
                string[] Lines = SR.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                SR.Close();
                SR.Dispose();
                foreach (string Line in Lines)
                {
                    string[] Setting = Line.Split(':');
                    switch (Setting[0])
                    {
                        case "PlayerTag":
                            {
                                PlayerTag = Setting[1];
                                break;
                            }
                        case "GameDirectory":
                            {
                                GameDirectory = Setting[1];
                                break;
                            }
                        case "DisplayMode":
                            {
                                DisplayMode = (SettingsDisplayMode)Enum.Parse(typeof(SettingsDisplayMode), Setting[1]);
                                break;
                            }
                        case "GameSound":
                            {
                                GameSound = int.Parse(Setting[1]);
                                break;
                            }
                        case "VerticalSync":
                            {
                                VerticalSync = int.Parse(Setting[1]);
                                break;
                            }
                        case "DefaultDisplay":
                            {
                                DefaultDisplay = int.Parse(Setting[1]);
                                break;
                            }
                        case "StartupCredits":
                            {
                                StartupCredits = int.Parse(Setting[1]);
                                break;
                            }
                        case "RememberMe":
                            {
                                RememberMe = int.Parse(Setting[1]);
                                break;
                            }
                        case "PlayerLoginToken":
                            {
                                PlayerLoginToken = Encrypt.DecryptStringAES(Setting[1]);
                                break;
                            }

                    }
                }
            }
        }

        public void SaveSettings()
        {
            StreamWriter SW = new StreamWriter(Globals.Files + "Settings.ini");
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("GameDirectory:" + Globals.GameDirectory);
            SB.AppendLine("LauncherRunPath:" + AppDomain.CurrentDomain.BaseDirectory);
            SB.AppendLine("PlayerTag:" + PlayerTag);
            SB.AppendLine("DisplayMode:" + DisplayMode.ToString());
            SB.AppendLine("GameSound:" + GameSound.ToString());
            SB.AppendLine("VerticalSync:" + VerticalSync.ToString());
            SB.AppendLine("DefaultDisplay:" + DefaultDisplay.ToString());
            SB.AppendLine("StartupCredits:" + StartupCredits.ToString());
            SB.AppendLine("RememberMe:" + RememberMe.ToString());
            SB.AppendLine("PlayerLoginToken:" + Encrypt.EncryptStringAES(PlayerLoginToken));
            SW.Write(SB.ToString());
            SW.Flush();
            SW.Close();
            SW.Dispose();

        }
    }
}


