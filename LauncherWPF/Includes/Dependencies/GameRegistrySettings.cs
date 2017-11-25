using Microsoft.Win32;
using System;

namespace Cartographer_Launcher.Includes.Dependencies
{
	public static class GameRegistrySettings
	{
		public static Globals.SettingsDisplayMode GetDisplayMode()
		{
			try
			{
				return (((int)GetVideoSetting("DisplayMode") == 1) ? Globals.SettingsDisplayMode.Windowed : Globals.SettingsDisplayMode.Fullscreen);
			}
			catch (Exception)
			{
				return Globals.SettingsDisplayMode.Windowed;
			}
		}

		public static void SetDisplayMode(bool FullScreen)
		{
			SetVideoSetting("DisplayMode", (FullScreen) ? 0 : 1);
		}

		private static object GetVideoSetting(string Setting)
		{
			return Registry.GetValue(Globals.REGISTRY_BASE + "Video Settings", Setting, null);
		}

		private static void SetVideoSetting(string Setting, object Value)
		{
			Registry.SetValue(Globals.REGISTRY_BASE + "Video Settings", Setting, Value);
		}
	}
}