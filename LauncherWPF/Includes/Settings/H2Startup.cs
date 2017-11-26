using Cartographer_Launcher.Includes.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartographer_Launcher.Includes.Settings
{
	public class H2Startup
	{
		private Dictionary<String, String> keyValues = new Dictionary<string, string>();

		private const string LANGUAGE_SELECT = "language_code";
		private const string SKIP_INTRO_TOGGLE = "skip_intro";
		private const string DISABLE_KEYBOARD_TOGGLE = "disable_ingame_keyboard";
		private const string SERVER_NAME = "server_name";
		private const string SERVER_PLAYLIST = "server_playlist";
		private const string HELP_HOTKEY = "hotkey_help";
		private const string DEBUG_TOGGLE_HOTKEY = "hotkey_toggle_debug";
		private const string WINDOW_ALIGN_HOTKEY = "hotkey_align_window";
		private const string BORDERLESS_WINDOW_TOGGLE_HOTKEY = "hotkey_window_mode";
		private const string HIDE_TEXT_CHAT_TOGGLE_HOTKEY = "hotkey_hide_ingame_chat";

		public Globals.SettingsLanguageSelect LanguageSelect
		{
			get
			{
				Globals.SettingsLanguageSelect LanguageValue;
				Enum.TryParse(keyValues[LANGUAGE_SELECT], out LanguageValue);
				return LanguageValue;
			}
			set { keyValues[LANGUAGE_SELECT] = "" + value.ToString(); }
		}

		public int SkipIntro
		{
			get
			{
				if (!keyValues.ContainsKey(SKIP_INTRO_TOGGLE)) return 1;
				else return int.Parse(keyValues[SKIP_INTRO_TOGGLE]);
			}
			set { keyValues[SKIP_INTRO_TOGGLE] = "" + value; }
		}

		public int DisableKeyboard
		{
			get
			{
				if (!keyValues.ContainsKey(DISABLE_KEYBOARD_TOGGLE)) return 0;
				else return int.Parse(keyValues[DISABLE_KEYBOARD_TOGGLE]);
			}
			set { keyValues[DISABLE_KEYBOARD_TOGGLE] = "" + value; }
		}

		public string DediServerName
		{
			get
			{
				if (!keyValues.ContainsKey(SERVER_NAME)) return "";
				else return keyValues[SERVER_NAME];
			}
			set { keyValues[SERVER_NAME] = "" + value; }
		}

		public string DediServerPlaylist
		{
			get
			{
				if (!keyValues.ContainsKey(SERVER_PLAYLIST)) return "";
				else return keyValues[SERVER_PLAYLIST];
			}
			set { keyValues[SERVER_PLAYLIST] = "" + value; }
		}

		public string HotKeyHelp
		{
			get
			{
				if (!keyValues.ContainsKey(HELP_HOTKEY)) return "114";
				else return keyValues[HELP_HOTKEY];
			}
			set { keyValues[HELP_HOTKEY] = "" + value.ToString(); }
		}

		public string HotKeyDebug
		{
			get
			{
				if (!keyValues.ContainsKey(DEBUG_TOGGLE_HOTKEY)) return "113";
				else return keyValues[DEBUG_TOGGLE_HOTKEY];
			}
			set { keyValues[DEBUG_TOGGLE_HOTKEY] = "" + value; }
		}

		public string HotKeyWindowAlign
		{
			get
			{
				if (!keyValues.ContainsKey(WINDOW_ALIGN_HOTKEY)) return "118";
				else return keyValues[WINDOW_ALIGN_HOTKEY];
			}
			set { keyValues[WINDOW_ALIGN_HOTKEY] = "" + value; }
		}

		public string HotKeyBorderless
		{
			get
			{
				if (!keyValues.ContainsKey(BORDERLESS_WINDOW_TOGGLE_HOTKEY)) return "119";
				else return keyValues[BORDERLESS_WINDOW_TOGGLE_HOTKEY];
			}
			set { keyValues[BORDERLESS_WINDOW_TOGGLE_HOTKEY] = "" + value; }
		}

		public string HotKeyInGameTextChat
		{
			get
			{
				if (!keyValues.ContainsKey(HIDE_TEXT_CHAT_TOGGLE_HOTKEY)) return "120";
				else return keyValues[HIDE_TEXT_CHAT_TOGGLE_HOTKEY];
			}
			set { keyValues[HIDE_TEXT_CHAT_TOGGLE_HOTKEY] = "" + value; }
		}

		private void SetDefaults()
		{

			LanguageSelect = Globals.SettingsLanguageSelect.Default;
			SkipIntro = 1;
			DisableKeyboard = 0;
			DediServerName = "";
			DediServerPlaylist = "";
			HotKeyHelp = "114";
			HotKeyDebug = "113";
			HotKeyWindowAlign = "118";
			HotKeyBorderless = "119";
			HotKeyInGameTextChat = "120";
		}

		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();
			SB.AppendLine("# language_code Options (Client):" + Environment.NewLine +
				"# -1 - System Default" + Environment.NewLine +
				"# 0  - Chinese" + Environment.NewLine +
				"# 1  - German" + Environment.NewLine +
				"# 2  - Spanish" + Environment.NewLine +
				"# 3  - French" + Environment.NewLine +
				"# 4  - Italian" + Environment.NewLine +
				"# 5  - Japanese" + Environment.NewLine +
				"# 6  - Korean" + Environment.NewLine +
				"# 7  - English" + Environment.NewLine + Environment.NewLine +
				"# skip_intro Options (Client):" + Environment.NewLine +
				"# 0 - Normal Intro" + Environment.NewLine +
				"# 1 - No Intro" + Environment.NewLine + Environment.NewLine +
				"# disable_ingame_keyboard Options (Client):" + Environment.NewLine +
				"# 0 - Normal Game Controls" + Environment.NewLine +
				"# 1 - Disables ONLY Keyboard when in-game & allows controllers when game is not in focus" + Environment.NewLine + Environment.NewLine +
				"# server_name Options (Server):" + Environment.NewLine +
				"# Sets the name of the server up to 31 characters long." + Environment.NewLine +
				"# Leave blank/empty for no effect." + Environment.NewLine + Environment.NewLine +
				"# server_playlist Options (Server):" + Environment.NewLine +
				"# Sets the playlist of the server up to 255 characters long." + Environment.NewLine +
				"# Leave blank/empty for no effect." + Environment.NewLine + Environment.NewLine +
				"# hotkey_... Options (Client):" + Environment.NewLine +
				"# The number used is the keyboard Virtual-Key (VK) Code in base-10 integer form." + Environment.NewLine +
				"# The codes in hexadecimal (base-16) form can be found here:" + Environment.NewLine +
				"# https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx" + Environment.NewLine + Environment.NewLine);
			foreach (KeyValuePair<string, string> entry in keyValues) SB.AppendLine(entry.Key + " = " + entry.Value);
			File.WriteAllText(Globals.GAME_DIRECTORY + "h2startup1.ini", SB.ToString());
		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.GAME_DIRECTORY + "h2startup1.ini"))
			{
				SetDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader SR = new StreamReader(Globals.GAME_DIRECTORY + "h2startup1.ini"))
				{
					string[] SettingLines = SR.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string Line in SettingLines)
					{
						if (!Line.StartsWith("#"))
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
