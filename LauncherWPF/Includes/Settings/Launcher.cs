﻿using Cartographer_Launcher.Includes.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes.Settings
{
	public class Launcher
	{
		Methods Methods = LauncherRuntime.Methods;
		private Dictionary<String, String> keyValues = new Dictionary<string, string>();

		private const string GAME_DIRECTORY = "GameDirectory";
		private const string LAUNCHER_RUN_PATH = "LauncherRunPath";
		private const string PLAYER_TAG = "PlayerTag";
		private const string DISPLAY_MODE = "DisplayMode";
		private const string NO_GAME_SOUND = "NoGameSound";
		private const string VERTICAL_SYNC = "VerticalSync";
		private const string DEFAULT_DISPLAY = "DefaultDisplay";
		private const string REMEMBER_ME = "RememberMe";

		public string GameDirectory
		{
			get
			{
				if (!keyValues.ContainsKey(GAME_DIRECTORY)) return Globals.GAME_DIRECTORY;
				else return keyValues[GAME_DIRECTORY];
			}
			set { keyValues[GAME_DIRECTORY] = "" + value; }
		}

		public string LauncherRunPath
		{
			get
			{
				if (!keyValues.ContainsKey(LAUNCHER_RUN_PATH)) return AppDomain.CurrentDomain.BaseDirectory;
				else return keyValues[LAUNCHER_RUN_PATH];
			}
			set { keyValues[LAUNCHER_RUN_PATH] = "" + value; }
		}

		public string PlayerTag
		{
			get
			{
				if (!keyValues.ContainsKey(PLAYER_TAG)) return "Player_1";
				else return keyValues[PLAYER_TAG];
			}
			set { keyValues[PLAYER_TAG] = "" + value; }
		}

		public Globals.SettingsDisplayMode DisplayMode
		{
			get
			{
				Globals.SettingsDisplayMode DisplayValue;
				if (!keyValues.ContainsKey(DISPLAY_MODE)) return Globals.SettingsDisplayMode.Fullscreen;
				else
				{
					Enum.TryParse(keyValues[DISPLAY_MODE], out DisplayValue);
					return DisplayValue;
				}
			}
			set { keyValues[DISPLAY_MODE] = "" + value.ToString(); }
		}

		public int NoGameSound
		{
			get
			{
				if (!keyValues.ContainsKey(NO_GAME_SOUND)) return 0;
				else return int.Parse(keyValues[NO_GAME_SOUND]);
			}
			set { keyValues[NO_GAME_SOUND] = "" + value; }
		}

		public int VerticalSync
		{
			get
			{
				if (!keyValues.ContainsKey(VERTICAL_SYNC)) return 1;
				else return int.Parse(keyValues[VERTICAL_SYNC]);
			}
			set { keyValues[VERTICAL_SYNC] = "" + value; }
		}

		public int DefaultDisplay
		{
			get
			{
				if (!keyValues.ContainsKey(DEFAULT_DISPLAY)) return 0;
				else return int.Parse(keyValues[DEFAULT_DISPLAY]);
			}
			set { keyValues[DEFAULT_DISPLAY] = "" + value; }
		}

		public int RememberMe
		{
			get
			{
				if (!keyValues.ContainsKey(REMEMBER_ME)) return 0;
				else return int.Parse(keyValues[REMEMBER_ME]);
			}
			set { keyValues[REMEMBER_ME] = "" + value; }
		}

		private int GetDefaultDisplay()
		{
			for (int i = 0; i < Screen.AllScreens.Length; i++) if (Screen.AllScreens[i].Primary) return i;
			return 0;
		}

		private void SetDefaults()
		{
			GameDirectory = Globals.GAME_DIRECTORY;
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
			foreach (KeyValuePair<string, string> entry in keyValues) SB.AppendLine(entry.Key + " = " + entry.Value);
			try { File.WriteAllText(Globals.FILES_DIRECTORY + "Settings.ini", SB.ToString()); }
			catch (Exception Ex)
			{
				Methods.ExLogFile(Ex.ToString());
				Methods.DebugAbort("The launcher failed to save settings to the specified file: Settings.ini");
			}

		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.FILES_DIRECTORY + "Settings.ini"))
			{
				SetDefaults();
				SaveSettings();
			}
			else
			{
				using (StreamReader SR = new StreamReader(Globals.FILES_DIRECTORY + "Settings.ini"))
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