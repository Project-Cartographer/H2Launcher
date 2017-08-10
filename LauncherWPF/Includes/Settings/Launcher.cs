using Cartographer_Launcher.Includes.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes.Settings
{
	public class Launcher
	{
		private Dictionary<String, String> keyValues = new Dictionary<string, string>();

		public string GameDirectory
		{
			get { return keyValues["GameDirectory"]; }
			set { keyValues["GameDirectory"] = value; }
		}

		public string LauncherRunPath
		{
			get { return keyValues["LauncherRunPath"]; }
			set { keyValues["LauncherRunPath"] = value; }
		}

		public string PlayerTag
		{
			get { return keyValues["PlayerTag"]; }
			set { keyValues["PlayerTag"] = "" + value; }
		}

		public Globals.SettingsDisplayMode DisplayMode
		{
			get { return Enum.TryParse(keyValues["DisplayMode"], out ); }
			set { Enum.TryParse(keyValues["DisplayMode"], out ) = value; }
		}

		public int NoGameSound
		{
			get { return int.Parse(keyValues["GameSound"]); }
			set { keyValues["GameSound"] = "" + value; }
		}

		public int VerticalSync
		{
			get { return int.Parse(keyValues["VerticalSync"]); }
			set { keyValues["VerticalSync"] = "" + value; }
		}

		public int DefaultDisplay
		{
			get { return int.Parse(keyValues["DefaultDisplay"]); }
			set { keyValues["DefaultDisplay"] = "" + value; }
		}

		public int RememberMe
		{
			get { return int.Parse(keyValues["GameSound"]); }
			set { keyValues["RememberMe"] = "" + value; }
		}

		private int GetDefaultDisplay()
		{
			for (int i = 0; i < Screen.AllScreens.Length; i++) if (Screen.AllScreens[i].Primary) return i;
			return 0;
		}

		private void setDefaults()
		{
			GameDirectory = Globals.GameDirectory;
			LauncherRunPath = AppDomain.CurrentDomain.BaseDirectory;
			PlayerTag = "Player_1";
			DisplayMode = GameRegistrySettings.GetDisplayMode();
			NoGameSound = 0;
			VerticalSync = 1;
			DefaultDisplay = 0;
			RememberMe = 0;
		}


		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();

			foreach (KeyValuePair<string, string> entry in keyValues)
				SB.AppendLine(entry.Key + " = " + entry.Value);

			File.WriteAllText(Globals.GameDirectory + "xlive.ini", SB.ToString());

		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.Files + "Settings.ini"))
			{
				setDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader streamreader = new StreamReader(Globals.Files + "Settings.ini"))
				{
					string[] xliveLines = streamreader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string Line in xliveLines)
					{
						string[] Setting = Line.Split(new string[] { ":" }, StringSplitOptions.None);
						keyValues.Add(Setting[0], Setting[1]);
					}
					setDefaults();
				}
			}
		}
	}
}