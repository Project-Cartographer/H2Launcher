using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes.Dependencies
{
    public static class GameRegistrySettings
	{ 
        public static void SetDisplayMode(bool FullScreen)
        {
            SetVideoSetting("DisplayMode", (FullScreen) ? 0 : 1);
        }

        public static SettingsDisplayMode GetDisplayMode()
        {
            try { return (((int)GetVideoSetting("DisplayMode") == 1) ? SettingsDisplayMode.Windowed : SettingsDisplayMode.Fullscreen); }
            catch (Exception) { return SettingsDisplayMode.Windowed; }
        }

        private static void SetVideoSetting(string Setting, object Value)
        {
            Registry.SetValue(Globals.H2RegistryBase + "Video Settings", Setting, Value);
        }

        private static object GetVideoSetting(string Setting)
        {
            return Registry.GetValue(Globals.H2RegistryBase + "Video Settings", Setting, null);
        }
    }
}
