using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes.Settings
{
    public class Launcher
    {
        private int _ResolutionWidth = Dependencies.GameRegistrySettings.GetScreenResX();
        private int _ResolutionHeight = Dependencies.GameRegistrySettings.GetScreenResY();
        private Dependencies.SettingsDisplayMode _DisplayMode = Dependencies.GameRegistrySettings.GetDisplayMode();
        private bool _GameSound = true;
        private bool _VerticalSync = true;
        private int _DefaultDisplay = 0;
        private bool _StartupCredits = !Directory.Exists(Globals.GameDirectory + "\\movie.bak");
        private Dependencies.GameLaunchEnvironment _GameEnvironment = Dependencies.GameLaunchEnvironment.Cartographer;
        private bool _RememberAccount = false;
        private string _RememberToken = "";
        private string _RememberUsername = "";
        private string _LoginTokin = "";
        private string _PlayerTag = "";
        private string _GameDirectory = Globals.GameDirectory;
        private byte[] _salt = Encoding.ASCII.GetBytes("GIVEITTOMEDICK");

        public int ResolutionWidth
        {
            get { return this._ResolutionWidth; }
            set { this._ResolutionWidth = value; }
        }

        public int ResolutionHeight
        {
            get { return this._ResolutionHeight; }
            set { this._ResolutionHeight = value; }
        }

        public Dependencies.SettingsDisplayMode DisplayMode
        {
            get { return this._DisplayMode; }
            set { this._DisplayMode = value; }
        }

        public bool GameSound
        {
            get { return this._GameSound; }
            set { this._GameSound = value; }
        }

        public bool VerticalSync
        {
            get { return this._VerticalSync; }
            set { this._VerticalSync = value; }
        }

        public int DefaultDisplay
        {
            get { return this._DefaultDisplay; }
            set { this._DefaultDisplay = value; }
        }

        public bool StartupCredits
        {
            get { return this._StartupCredits; }
            set { this._StartupCredits = value; }
        }

        public Dependencies.GameLaunchEnvironment GameEnvironment
        {
            get { return this._GameEnvironment; }
            set { this._GameEnvironment = value; }
        }

        public bool RememberAccount
        {
            get { return this._RememberAccount; }
            set { this._RememberAccount = value; }
        }

        public string RememberToken
        {
            get { return this._RememberToken; }
            set { this._RememberToken = value; }
        }

        public string LoginToken
        {
            get { return this._LoginTokin; }
            set { this._LoginTokin = value; }
        }

        public string GameDirectory
        {
            get { return this._GameDirectory; }
            set { this._GameDirectory = value; }
        }

        public string PlayerTag
        {
            get { return this._PlayerTag; }
            set { this._PlayerTag = value; }
        }

        public string RememberUsername
        {
            get { return this._RememberUsername; }
            set { this._RememberUsername = value; }
        }

        private int GetDefaultDisplay()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
                if (Screen.AllScreens[i].Primary)
                    return i;
            return 0;
        }

        #region Encrypt
        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new SystemException("Stream did not contain properly formatted byte array");

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException("Did not read byte array properly");

            return buffer;
        }

        private string EncryptStringAES(string plainText)
        {
            string Secret = "";
            ManagementClass Class = new ManagementClass("win32_processor");
            ManagementObjectCollection Collec = Class.GetInstances();
            foreach (ManagementObject Obj in Collec)
            {
                if (Secret == "")
                {
                    Secret = Obj.Properties["processorID"].Value.ToString().Substring(0, 8);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(plainText) & !string.IsNullOrEmpty(Secret))
            {
                string outStr = null;
                RijndaelManaged aesAlg = null;
                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Secret, _salt);
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) swEncrypt.Write(plainText);
                        outStr = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
                finally
                {
                    if (aesAlg != null)
                        aesAlg.Clear();
                }
                return outStr;
            }
            return "";
        }

        private string DecryptStringAES(string cipherText)
        {
            string Secret = "";
            ManagementClass Class = new ManagementClass("win32_processor");
            ManagementObjectCollection Collec = Class.GetInstances();
            foreach (ManagementObject Obj in Collec)
            {
                if (Secret == "")
                {
                    Secret = Obj.Properties["processorID"].Value.ToString().Substring(0, 8);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(cipherText) & !string.IsNullOrEmpty(Secret))
            {
                RijndaelManaged aesAlg = null;
                string plaintext = null;
                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Secret, _salt);
                    byte[] bytes = Convert.FromBase64String(cipherText);
                    using (MemoryStream msDecrypt = new MemoryStream(bytes))
                    {
                        aesAlg = new RijndaelManaged();
                        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                        aesAlg.IV = ReadByteArray(msDecrypt);
                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
                finally
                {
                    if (aesAlg != null)
                        aesAlg.Clear();
                }
                return plaintext;
            }
            return "";
        }
        #endregion

        public void LoadSettings()
        {
            if (!File.Exists(Globals.Files + "Settings.ini")) SaveSettings();
            else
            {
                StreamReader SR = new StreamReader(Globals.Files + "Settings.ini");
                string[] Lines = SR.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                SR.Close();
                SR.Dispose();
                foreach (string Line in Lines)
                {
                    string[] Setting = Line.Split(':');
                    switch (Setting[0])
                    {
                        case "PlayerTag":
                            {
                                this.PlayerTag = Setting[1];
                                break;
                            }
                        case "GameDirectory":
                            {
                                this.GameDirectory = Setting[1];
                                break;
                            }
                        case "ResolutionHeight":
                            {
                                this.ResolutionHeight = int.Parse(Setting[1]);
                                break;
                            }
                        case "ResolutionWidth":
                            {
                                this.ResolutionWidth = int.Parse(Setting[1]);
                                break;
                            }
                        case "DisplayMode":
                            {
                                this.DisplayMode = (Dependencies.SettingsDisplayMode)Enum.Parse(typeof(Dependencies.SettingsDisplayMode), Setting[1]);
                                break;
                            }
                        case "GameSound":
                            {
                                this.GameSound = bool.Parse(Setting[1]);
                                break;
                            }
                        case "VerticalSync":
                            {
                                this.VerticalSync = bool.Parse(Setting[1]);
                                break;
                            }
                        case "DefaultDisplay":
                            {
                                this.DefaultDisplay = int.Parse(Setting[1]);
                                break;
                            }
                        case "StartupCredits":
                            {
                                this.StartupCredits = bool.Parse(Setting[1]);
                                break;
                            }
                        case "GameEnvironment":
                            {
                                this.GameEnvironment = (Dependencies.GameLaunchEnvironment)Enum.Parse(typeof(Dependencies.GameLaunchEnvironment), Setting[1]);
                                break;
                            }
                        case "RememberAccount":
                            {
                                this.RememberAccount = bool.Parse(Setting[1]);
                                break;
                            }
                        case "RememberToken":
                            {
                                this.RememberToken = DecryptStringAES(Setting[1]);
                                break;
                            }
                        case "LoginToken":
                            {
                                this.LoginToken = Setting[1];
                                break;
                            }
                        case "RememberUsername":
                            {
                                this.RememberUsername = Setting[1];
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
            SB.AppendLine("GameDirectory:" + this.GameDirectory);
            SB.AppendLine("LauncherRunPath:" + AppDomain.CurrentDomain.BaseDirectory);
            SB.AppendLine("PlayerTag:" + this.LoginToken);
            SB.AppendLine("LoginToken:" + this.LoginToken);
            SB.AppendLine("ResolutionHeight:" + this.ResolutionHeight.ToString());
            SB.AppendLine("ResolutionWidth:" + this.ResolutionWidth.ToString());
            SB.AppendLine("DisplayMode:" + DisplayMode.ToString());
            SB.AppendLine("GameSound:" + GameSound.ToString());
            SB.AppendLine("VerticalSync:" + VerticalSync.ToString());
            SB.AppendLine("DefaultDisplay:" + DefaultDisplay.ToString());
            SB.AppendLine("StartupCredits:" + StartupCredits.ToString());
            SB.AppendLine("GameEnvironment:" + GameEnvironment.ToString());
            SB.AppendLine("RememberAccount:" + RememberAccount.ToString());
            SB.AppendLine("RememberToken:" + EncryptStringAES(RememberToken));
            SB.AppendLine("RememberUsername:" + RememberUsername);
            SW.Write(SB.ToString());
            SW.Flush();
            SW.Close();
            SW.Dispose();

        }
    }
}


