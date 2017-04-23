using System;
using System.IO;
using System.Text;

namespace Cartographer_Launcher.Includes.Settings
{
    public class ProjectCartographer
    {
        private int _DebugLog = 0;
        private int _Ports = 1000;
        private int _GunGame = 0;
        private int _FPSCap = 1;
        private int _FPSLimit = 60;
        private int _VoiceChat = 0;
        private int _MapDownload = 1;

        public int DebugLog
        {
            get { return this._DebugLog; }
            set { this._DebugLog = value; }
        }
        public int Ports
        {
            get { return this._Ports; }
            set { this._Ports = value; }
        }
        public int GunGame
        {
            get { return this._GunGame; }
            set { this._GunGame = value; }
        }
        public int FPSCap
        {
            get { return this._FPSCap; }
            set { this._FPSCap = value; }
        }
        public int FPSLimit
        {
            get { return this._FPSLimit; }
            set { this._FPSLimit = value; }
        }
        public int VoiceChat
        {
            get { return this._VoiceChat; }
            set { this._VoiceChat = value; }
        }
        public int MapDownload
        {
            get { return this._MapDownload; }
            set { this._MapDownload = value; }
        }

        public void LoadSettings()
        {
            if (!File.Exists(Globals.GameDirectory + "xlive.ini")) SaveSettings();
            else
            {
                StreamReader SR = new StreamReader(Globals.GameDirectory + "xlive.ini");
                string[] Lines = SR.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                SR.Close();
                SR.Dispose();
                foreach (string Line in Lines)
                {
                    string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
                    switch (Setting[0])
                    {
                        case "debug_log":
                            {
                                this.DebugLog = int.Parse(Setting[1]);
                                break;
                            }
                        case "port":
                            {
                                this.Ports = int.Parse(Setting[1]);
                                break;
                            }
                        case "fps_enable":
                            {
                                this.FPSCap = int.Parse(Setting[1]);
                                break;
                            }
                        case "fps_limit":
                            {
                                this.FPSLimit = int.Parse(Setting[1]);
                                break;
                            }
                        case "voice_chat":
                            {
                                this.VoiceChat = int.Parse(Setting[1]);
                                break;
                            }
                        case "map_downloading_enable":
                            {
                                this.MapDownload = int.Parse(Setting[1]);
                                break;
                            }
                    }
                }
            }
        }

        public void SaveSettings()
        {
            //ADD SETTINGS CONTROLS FOR THE SETTINGS THAT NEED IT.
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("debug_log = " + this.DebugLog);
            SB.AppendLine("port = " + this.Ports);
            SB.AppendLine("gungame = " + this.GunGame);
            SB.AppendLine("fps_enable = " + this.FPSCap);
            SB.AppendLine("fps_limit = " + this.FPSLimit);
            SB.AppendLine("voice_chat = " + this.VoiceChat);
            SB.AppendLine("map_downloading_enable = " + this.MapDownload);
            File.WriteAllText(Globals.GameDirectory + "xlive.ini", SB.ToString());
        }
    }
}
