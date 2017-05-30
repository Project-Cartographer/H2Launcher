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
		public static string GameDirectory
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

		public static string GameExecutable
		{
			get { return GameDirectory + "\\halo2.exe"; }
		}

		public static string GameStartupExecutable
		{
			get { return GameDirectory + "\\startup.exe"; }
		}

		public static string LocalAppData
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

		public static string H2vHubDirectory
		{
			get { return LocalAppData; }
		}

		public static string LogFile
		{
			get { return LocalAppData + "\\H2Launcher.log"; }
		}

		public static string ExLogFile
		{
			get { return LocalAppData + "\\Error.log"; }
		}

		public static string Downloads
		{
			get { return LocalAppData + "\\Download\\"; }
		}

		public static string Files
		{
			get { return LocalAppData + "\\Files\\"; }
		}

		public static string Content
		{
			get { return LocalAppData + "\\Files\\Content\\"; }
		}

		public static string H2RegistryBase
		{
			get { return @"HKEY_CURRENT_USER\Software\Microsoft\Halo 2\"; }
		}

		public static string WebHost
		{
			get { return @"http://www.cartographer.online/H2Cartographer/"; }
		}

		public static string LANIP
		{
			get
			{
				var localhost = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var lanip in localhost.AddressList)
				{
					if (lanip.AddressFamily == AddressFamily.InterNetwork)
					{
						return lanip.ToString();
					}
				}
				throw new Exception("Local IP Address Not Found!");
			}
		}
		

		public static string WANIP
		{
			get
			{
				try
				{
					WebClient wc = new WebClient();
					string url = @"http://www.cartographer.online/wanip.php";
					byte[] response = wc.DownloadData(url);
					UTF8Encoding utf = new UTF8Encoding();
					string wanip = utf.GetString(response);
					return wanip;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}
		public static string LauncherCheck
		{
			get { return WebHost + "v2.txt"; }
		}

		public static string RemoteUpdate
		{
			get { return WebHost + "update/"; }
		}

		public static string RemoteUpdateXML
		{
			get { return WebHost + "update3.xml"; }
		}

		public static string VersionNumber
		{
			get { return "Version: 2.0.5"; }
		}
	}
}