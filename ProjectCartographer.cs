using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cartographer_Launcher.Includes.Settings
{
	public class ProjectCartographer
	{
		Methods Methods = LauncherRuntime.Methods;
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
		private const string RAW_MOUSE_INPUT = "raw_input";
		private const string DISCORD_RICH_PRESENCE = "discord_enable";

		public int DebugLog
		{
			get
			{
				if (!keyValues.ContainsKey(DEBUG_LOG)) return 0;
				else return int.Parse(keyValues[DEBUG_LOG]);
			}
			set { keyValues[DEBUG_LOG] = "" + value; }
		}

		public string LoginToken
		{
			get
			{
				if (!keyValues.ContainsKey(LOGIN_TOKEN)) return "";
				else return keyValues[LOGIN_TOKEN];
			}
			set { keyValues[LOGIN_TOKEN] = "" + value; }
		}

		public int Ports
		{
			get
			{
				if (!keyValues.ContainsKey(BASE_PORT)) return 1000;
				else return int.Parse(keyValues[BASE_PORT]);
			}
			set { keyValues[BASE_PORT] = "" + value; }
		}

		public string LANIP
		{
			get
			{
				if (!keyValues.ContainsKey(LAN_IP)) return "";
				else return keyValues[LAN_IP];
			}
			set { keyValues[LAN_IP] = "" + Globals.LAN_IP; }
		}

		public string WANIP
		{
			get
			{
				if (!keyValues.ContainsKey(WAN_IP)) return "";
				else return keyValues[WAN_IP];
			}
			set { keyValues[WAN_IP] = "" + Globals.WAN_IP; }
		}

		public int GunGame
		{
			get
			{
				if (!keyValues.ContainsKey(GUN_GAME)) return 0;
				else return int.Parse(keyValues[GUN_GAME]);
			}
			set { keyValues[GUN_GAME] = "" + value; }
		}

		public int FPSCap
		{
			get
			{
				if (!keyValues.ContainsKey(FPS_ENABLE)) return 1;
				else return int.Parse(keyValues[FPS_ENABLE]);
			}
			set { keyValues[FPS_ENABLE] = "" + value; }
		}

		public int FPSLimit
		{
			get
			{
				if (!keyValues.ContainsKey(FPS_LIMIT)) return 60;
				else return int.Parse(keyValues[FPS_LIMIT]);
			}
			set { keyValues[FPS_LIMIT] = "" + value; }
		}

		public int VoiceChat
		{
			get
			{
				if (!keyValues.ContainsKey(VOICE_CHAT)) return 0;
				else return int.Parse(keyValues[VOICE_CHAT]);
			}
			set { keyValues[VOICE_CHAT] = "" + value; }
		}

		public int MapDownload
		{
			get
			{
				if (!keyValues.ContainsKey(MAP_DOWNLOADING)) return 0;
				else return int.Parse(keyValues[MAP_DOWNLOADING]);
			}
			set { keyValues[MAP_DOWNLOADING_ENABLE] = "" + value; }
		}

		public int FOV
		{
			get
			{
				if (!keyValues.ContainsKey(FIELD_OF_VIEW)) return 57;
				else return int.Parse(keyValues[FIELD_OF_VIEW]);
			}
			set { keyValues[FIELD_OF_VIEW] = "" + value; }
		}

		public string Reticle
		{
			get
			{
				if (!keyValues.ContainsKey(CROSSHAIR_OFFSET)) return "0.165";
				else return keyValues[CROSSHAIR_OFFSET];
			}
			set { keyValues[CROSSHAIR_OFFSET] = "" + value; }
		}

		public int RawMouseInput
		{
			get
			{
				if (!keyValues.ContainsKey(RAW_MOUSE_INPUT)) return 0;
				else return int.Parse(keyValues[RAW_MOUSE_INPUT]);
			}
			set { keyValues[RAW_MOUSE_INPUT] = "" + value; }
		}

		public int DiscordRichPresence
		{
			get
			{
				if (!keyValues.ContainsKey(DISCORD_RICH_PRESENCE)) return 1;
				else return int.Parse(keyValues[DISCORD_RICH_PRESENCE]);
			}
			set { keyValues[DISCORD_RICH_PRESENCE] = "" + value; }
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
			RawMouseInput = 0;
			DiscordRichPresence = 1;
		}

		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();
			foreach (KeyValuePair<string, string> entry in keyValues) SB.AppendLine(entry.Key + " = " + entry.Value);
			try
			{
                Methods.AllowReadWrite(Globals.GAME_DIRECTORY + "xlive.ini");
                File.WriteAllText(Globals.GAME_DIRECTORY + "xlive.ini", SB.ToString());
                Methods.AllowReadWrite(Globals.GAME_DIRECTORY + "xlive.ini");
            }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				if (!Methods.IsAdministrator())
				{
					Methods.ErrorMessage("The launcher failed to save settings to the specified file: xlive.ini, trying to relaunch as admin");
					Methods.RelaunchAsAdmin();
				}
				else Methods.DebugAbort("Can't access \"" + Globals.GAME_DIRECTORY + "xlive.ini" + "\" please fix the permissions for this file.");

            }
		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.GAME_DIRECTORY + "xlive.ini"))
			{
				SetDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader SR = new StreamReader(Globals.GAME_DIRECTORY + "xlive.ini"))
				{
					string[] SettingLines = SR.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string Line in SettingLines)
					{
						if (Line.Contains("="))
						{
							string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
							if (!keyValues.ContainsKey(Setting[0])) keyValues.Add(Setting[0], Setting[1]);
						}
					}
				}
			}
		}
	}
}
