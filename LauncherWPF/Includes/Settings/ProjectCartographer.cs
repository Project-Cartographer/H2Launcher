using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cartographer_Launcher.Includes.Settings
{
	public class ProjectCartographer
	{
		private Dictionary<String, String> keyValues = new Dictionary<string, string>();

		public int DebugLog
		{
			get { return int.Parse(keyValues["debug_log"]); }
			set { keyValues["base_port"] = "" + value; }
		}

		public string LoginToken
		{
			get { return keyValues["login_token"]; }
			set { keyValues["login_token"] = value; }
		}

		public int Ports
		{
			get { return int.Parse(keyValues["base_port"]); }
			set { keyValues["base_port"] = "" + value; }
		}

		public string LANIP
		{
			get { return keyValues["LANIP"]; }
			set { keyValues["LANIP"] = "" + value; }
		}

		public string WANIP
		{
			get { return keyValues["WANIP"]; }
			set { keyValues["WANIP"] = "" + value; }
		}

		public int GunGame
		{
			get { return int.Parse(keyValues["gun_game"]); }
			set { keyValues["gun_game"] = "" + value; }
		}

		public int FPSCap
		{
			get { return int.Parse(keyValues["fps_enable"]); }
			set { keyValues["fps_enable"] = "" + value; }
		}

		public int FPSLimit
		{
			get { return int.Parse(keyValues["fps_limit"]); }
			set { keyValues["fps_limit"] = "" + value; }
		}

		public int VoiceChat
		{
			get { return int.Parse(keyValues["voice_chat"]); }
			set { keyValues["voice_chat"] = "" + value; }
		}

		public int MapDownload
		{
			get { return int.Parse(keyValues["map_downloading_enable"]); }
			set { keyValues["map_downloading_enable"] = "" + value; }
		}

		public int FOV
		{
			get { return int.Parse(keyValues["field_of_view"]); }
			set { keyValues["field_of_view"] = "" + value; }
		}

		public string Reticle
		{
			get { return keyValues["crosshair_offset"]; }
			set { keyValues["crosshair_offset"] = "" + value; }
		}

		private void setDefaults()
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
				setDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader streamreader = new StreamReader(Globals.GameDirectory + "xlive.ini"))
				{
					string[] xliveLines = streamreader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string Line in xliveLines)
					{
						string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
						keyValues.Add(Setting[0], Setting[1]);
					}
					setDefaults();
				}
			}
		}
	}
}
