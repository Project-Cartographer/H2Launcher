using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cartographer_Launcher.Includes.Settings
{
	public class ProjectCartographer
	{
		private Dictionary<String, String> keyValues = new Dictionary<string, string>();

		private const string DEBUG_LOG = "debug_log";
		private const string LOGIN_TOKEN = "login_token";
		private const string BASE_PORT = "base_port";
		private const string LAN_IP = "LANIP";
		private const string WAN_IP = "WANIP";
		private const string GUN_GAME = "gun_game";
		private const string FPS_ENABLE = "fps_enable";
		private const string FPS_LIMIT = "fps_limit";
		private const string VOICE_CHAT = "voice_chat";
		private const string MAP_DOWNLOADING_ENABLE = "map_downloading_enable";
		private const string FIELD_OF_VIEW = "field_of_view";
		private const string CROSSHAIR_OFFSET = "crosshair_offset";



		public int DebugLog
		{
			get { return int.Parse(keyValues[DEBUG_LOG]); }
			set { keyValues[DEBUG_LOG] = "" + value; }
		}

		public string LoginToken
		{
			get { return keyValues[LOGIN_TOKEN]; }
			set { keyValues[LOGIN_TOKEN] = "" + value; }
		}

		public int Ports
		{
			get { return int.Parse(keyValues[BASE_PORT]); }
			set { keyValues[BASE_PORT] = "" + value; }
		}

		public string LANIP
		{
			get { return keyValues[LAN_IP]; }
			set { keyValues[LAN_IP] = "" + value; }
		}

		public string WANIP
		{
			get { return keyValues[WAN_IP]; }
			set { keyValues[WAN_IP] = "" + value; }
		}

		public int GunGame
		{
			get { return int.Parse(keyValues[GUN_GAME]); }
			set { keyValues[GUN_GAME] = "" + value; }
		}

		public int FPSCap
		{
			get { return int.Parse(keyValues[FPS_ENABLE]); }
			set { keyValues[FPS_ENABLE] = "" + value; }
		}

		public int FPSLimit
		{
			get { return int.Parse(keyValues[FPS_LIMIT]); }
			set { keyValues[FPS_LIMIT]] = "" + value; }
		}

		public int VoiceChat
		{
			get { return int.Parse(keyValues[VOICE_CHAT]); }
			set { keyValues[VOICE_CHAT] = "" + value; }
		}

		public int MapDownload
		{
			get { return int.Parse(keyValues[MAP_DOWNLOADING_ENABLE]); }
			set { keyValues[MAP_DOWNLOADING_ENABLE] = "" + value; }
		}

		public int FOV
		{
			get { return int.Parse(keyValues[FIELD_OF_VIEW]); }
			set { keyValues[FIELD_OF_VIEW] = "" + value; }
		}

		public string Reticle
		{
			get { return keyValues[CROSSHAIR_OFFSET]; }
			set { keyValues[CROSSHAIR_OFFSET] = "" + value; }
		}

		private void SetDefaults()
		{
			LoginToken = "";
			DebugLog = 0;
			Ports = 1000;
			LANIP = "";
			WANIP = "";
			GunGame = 0;
			FPSCap = 1;
			FPSLimit = 60;
			VoiceChat = 0;
			MapDownload = 0;
			FOV = 57;
			Reticle = "0.165";
		}

		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();
			foreach (KeyValuePair<string, string> entry in keyValues)
			{
				SB.AppendLine(entry.Key + " = " + entry.Value);
			}
			File.WriteAllText(Globals.GameDirectory + "xlive.ini", SB.ToString());
		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.GameDirectory + "xlive.ini"))
			{
				SetDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader SR = new StreamReader(Globals.GameDirectory + "xlive.ini"))
				{
					string[] SettingLines = SR.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string Line in SettingLines)
					{
						string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
						keyValues.Add(Setting[0], Setting[1]);
					}
					SetDefaults();
				}
			}
		}
	}
}
