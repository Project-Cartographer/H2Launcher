using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Cartographer_Launcher.Includes
{
    public static class Globals
    {
        public static string GameDirectory
        {
            get
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
            get { return @"http://69.195.136.203/H2Cartographer/"; }
        }

        public static string RemoteAPI
        {
            get { return WebHost + "H2Cartographer.php"; }
        }

        public static string RemoteUpdate
        {
            get { return WebHost + "update/"; }
        }

        public static string RemoteUpdateXML
        {
            get { return WebHost + "update.xml"; }
            //get { return RemotePath + "dev_update.xml"; }
        }
    }
}
