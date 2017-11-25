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
			get { return int.Parse(keyValues[SKIP_INTRO_TOGGLE]); }
			set { keyValues[SKIP_INTRO_TOGGLE] = "" + value; }
		}

		public int DisableKeyboard
		{
			get { return int.Parse(keyValues[DISABLE_KEYBOARD_TOGGLE]); }
			set { keyValues[DISABLE_KEYBOARD_TOGGLE] = "" + value; }
		}

		public string DediServerName
		{
			get { return keyValues[SERVER_NAME]; }
			set { keyValues[SERVER_NAME] = "" + value; }
		}

		public string DediServerPlaylist
		{
			get { return keyValues[SERVER_PLAYLIST]; }
			set { keyValues[SERVER_PLAYLIST] = "" + value; }
		}

		public VirtualKeyValues.VirtualKeyStates HotKeyHelp
		{
			get
			{
				VirtualKeyValues.VirtualKeyStates HotKeyHelpValue;
				Enum.TryParse(keyValues[HELP_HOTKEY], out HotKeyHelpValue);
				return HotKeyHelpValue;
			}
			set { keyValues[HELP_HOTKEY] = "" + value.ToString(); }
		}

		public string HotKeyDebug
		{
			get { return keyValues[DEBUG_TOGGLE_HOTKEY]; }
			set { keyValues[DEBUG_TOGGLE_HOTKEY] = "" + value; }
		}

		public string HotKeyWindowAlign
		{
			get { return keyValues[WINDOW_ALIGN_HOTKEY]; }
			set { keyValues[WINDOW_ALIGN_HOTKEY] = "" + value; }
		}

		public string HotKeyBorderless
		{
			get { return keyValues[BORDERLESS_WINDOW_TOGGLE_HOTKEY]; }
			set { keyValues[BORDERLESS_WINDOW_TOGGLE_HOTKEY] = "" + value; }
		}

		public string HotKeyInGameTextChat
		{
			get { return keyValues[HIDE_TEXT_CHAT_TOGGLE_HOTKEY]; }
			set { keyValues[HIDE_TEXT_CHAT_TOGGLE_HOTKEY] = "" + value; }
		}

		private void SetDefaults()
		{

			LanguageSelect = Globals.SettingsLanguageSelect.Default;
			SkipIntro = 1;
			DisableKeyboard = 0;
			DediServerName = "";
			DediServerPlaylist = "";
			System.Windows.MessageBox.Show(HotKeyHelp.ToString());
			HotKeyHelp = VirtualKeyValues.VirtualKeyStates.VK_F3;
			HotKeyDebug = "113";
			HotKeyWindowAlign = "118";
			HotKeyBorderless = "119";
			HotKeyInGameTextChat = "120";
		}
		

		public void SaveSettings()
		{
			StringBuilder SB = new StringBuilder();
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
						string[] Setting = Line.Split(new string[] { " = " }, StringSplitOptions.None);
						if (!Line.Contains("#") && !keyValues.ContainsKey(Setting[0])) keyValues.Add(Setting[0], Setting[1]);
					}
				}
			}
		}
	}
}
