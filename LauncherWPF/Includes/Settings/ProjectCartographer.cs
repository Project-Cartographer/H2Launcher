using System;
using System.IO;
using System.Text;

namespace Cartographer_Launcher.Includes.Settings
{
	public class ProjectCartographer
	{
		private string _LoginToken = "";
		private int _DebugLog = 0;
		private int _Ports = 1000;
		private int _GunGame = 0;
		private int _FPSCap = 1;
		private int _FPSLimit = 60;
		private int _VoiceChat = 0;
		private int _MapDownload = 1;

		public int DebugLog
		{
			get { return _DebugLog; }
			set { _DebugLog = value; }
		}
		public string LoginToken
		{
			get { return _LoginToken; }
			set { _LoginToken = value; }
		}
		public int Ports
		{
			get { return _Ports; }
			set { _Ports = value; }
		}
		public int GunGame
		{
			get { return _GunGame; }
			set { _GunGame = value; }
		}
		public int FPSCap
		{
			get { return _FPSCap; }
			set { _FPSCap = value; }
		}
		public int FPSLimit
		{
			get { return _FPSLimit; }
			set { _FPSLimit = value; }
		}
		public int VoiceChat
		{
			get { return _VoiceChat; }
			set { _VoiceChat = value; }
		}
		public int MapDownload
		{
			get { return _MapDownload; }
			set { _MapDownload = value; }
		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.GameDirectory + "xlive.ini")) SaveSettings();
			else
			{
				StreamReader SR = new StreamReader(Globals.GameDirectory + "xlive.ini");
				string[] Lines = SR.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				SR.Close();
				SR.Dispose();
				foreach (string Line in Lines)
				{
					string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
					switch (Setting[0])
					{
						case "login_token":
							{
								LoginToken = Setting[1];
								break;
							}
						case "debug_log":
							{
								DebugLog = int.Parse(Setting[1]);
								break;
							}
						case "port":
							{
								Ports = int.Parse(Setting[1]);
								break;
							}
						case "fps_enable":
							{
								FPSCap = int.Parse(Setting[1]);
								break;
							}
						case "fps_limit":
							{
								FPSLimit = int.Parse(Setting[1]);
								break;
							}
						case "voice_chat":
							{
								VoiceChat = int.Parse(Setting[1]);
								break;
							}
						case "map_downloading_enable":
							{
								MapDownload = int.Parse(Setting[1]);
								break;
							}
					}
				}
			}
		}

		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();
			SB.AppendLine("login_token = " + LoginToken);
			SB.AppendLine("debug_log = " + DebugLog);
			SB.AppendLine("port = " + Ports);
			SB.AppendLine("gungame = " + GunGame);
			SB.AppendLine("fps_enable = " + FPSCap);
			SB.AppendLine("fps_limit = " + FPSLimit);
			SB.AppendLine("voice_chat = " + VoiceChat);
			SB.AppendLine("map_downloading_enable = " + MapDownload);
			File.WriteAllText(Globals.GameDirectory + "xlive.ini", SB.ToString());
		}
	}
}