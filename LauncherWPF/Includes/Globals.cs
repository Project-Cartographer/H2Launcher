using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cartographer_Launcher.Includes
{
	public static class Globals
	{
		public static string GAME_DIRECTORY
		{
			get
			{
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\halo2.exe"))
				{
					if (Environment.Is64BitOperatingSystem)
					{
						if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0\") == null)
						{
							Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0\");
							return "";
						}
						return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0", "GameInstallDir", null);

					}
					else
					{
						if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0\") == null)
						{
							Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0\");
							return "";
						}
						return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0", "GameInstallDir", null);
					}
				}
				else return AppDomain.CurrentDomain.BaseDirectory;
			}
			set
			{
				if (Environment.Is64BitOperatingSystem)
				{
					if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0\") == null)
						Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0\");
					Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo 2\1.0", "GameInstallDir", value);
				}
				else
				{
					if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0\") == null)
						Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0\");
					Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft Games\Halo 2\1.0", "GameInstallDir", value);
				}
			}
		}

		public static string GAME_EXECUTABLE
		{
			get { return GAME_DIRECTORY + "\\halo2.exe"; }
		}

		public static string GAME_STARTUP_EXECUTABLE
		{
			get { return GAME_DIRECTORY + "\\startup.exe"; }
		}

		public static string LOCAL_APPDATA
		{
			get
			{
				if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\"))
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\");
				if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Download"))
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Download");
				if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Files"))
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Files");
				if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Files\\Content"))
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\Files\\Content");
				return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\H2vHub\\Cartographer\\";
			}
		}

		public static string H2V_HUB_DIRECTORY
		{
			get { return LOCAL_APPDATA; }
		}

		public static string LAUNCHER_LOG_FILE
		{
			get { return LOCAL_APPDATA + "\\H2Launcher.log"; }
		}

		public static string LAUNCHER_EXCEPTION_LOG_FILE
		{
			get { return LOCAL_APPDATA + "\\Error.log"; }
		}

		public static string DOWNLOADS_DIRECTORY
		{
			get { return LOCAL_APPDATA + "\\Download\\"; }
		}

		public static string FILES_DIRECTORY
		{
			get { return LOCAL_APPDATA + "\\Files\\"; }
		}

		public static string CONTENT_DIRECTORY
		{
			get { return LOCAL_APPDATA + "\\Files\\Content\\"; }
		}

		public static string REGISTRY_BASE
		{
			get { return @"HKEY_CURRENT_USER\Software\Microsoft\Halo 2\"; }
		}

		public static string WEBHOST
		{
			get { return @"https://www.cartographer.online/H2Cartographer/"; }
		}

		public enum SettingsDisplayMode : int
		{
			Fullscreen = 0,
			Windowed = 1,
		}

		public static string LAN_IP
		{
			get
			{
				var localhost = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var lanip in localhost.AddressList) if (lanip.AddressFamily == AddressFamily.InterNetwork) return lanip.ToString();
				throw new Exception("Local IP Address Not Found!");
			}
		}
		
		public static string WAN_IP
		{
			get
			{
				try
				{
					WebClient wc = new WebClient();
					string url = @"https://www.cartographer.online/wanip.php";
					byte[] response = wc.DownloadData(url);
					UTF8Encoding utf = new UTF8Encoding();
					string wanip = utf.GetString(response);
					return wanip;
				}
				catch (Exception) { return ""; }
			}
		}

		public static string LAUNCHER_CHECK
		{
			get { return WEBHOST + "v2.txt"; }
		}

		public static string REMOTE_UPDATE_DIRECTORY
		{
			get { return WEBHOST + "update/"; }
		}

		public static string REMOTE_UPDATE_XML_FILE
		{
			get { return WEBHOST + "update3.xml"; }
		}

		public static string LAUNCHER_RELEASE_VERSION
		{
			get { return "Version: 2.1.2"; }
		}
	}
}