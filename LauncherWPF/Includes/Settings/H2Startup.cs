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
		private int _LanguageCode = -1;
		private int _SkipIntro = 0;
		private int _DisableKeyboard = 0;
		private string _ServerName = "";
		private string _HotKeyDebug = "114 #0x72 - VK_F3";
		private string _HotKeyAlign = "118 #0x76 - VK_F7";
		private string _HotKeyBorderless = "119 #0x77 - VK_F8";
		private string _HotKeyHideChat = "120 #0x78 - VK_F9";

		public int LanguageCode
		{
			get { return _LanguageCode; }
			set { _LanguageCode = value; }
		}

		public int SkipIntro
		{
			get { return _SkipIntro; }
			set { _SkipIntro = value; }
		}

		public int DisableKeyboard
		{
			get { return _DisableKeyboard; }
			set { _DisableKeyboard = value; }
		}

		private string GetLine(string fileName, int line)
		{
			using (var sr = new StreamReader(Globals.GameDirectory + "h2startup1.ini"))
			{
				for (int i = 1; i < line; i++)
					sr.ReadLine();
				return sr.ReadLine();
			}
		}

		public void LoadSettings()
		{
			if (!File.Exists(Globals.GameDirectory = "h2startup1.ini")) SaveSettings();

		}

		public void SaveSettings()
		{
			StreamWriter sw = new StreamWriter(Globals.Files + "Settings.ini");
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("GameDirectory:" + Globals.GameDirectory);
			sw.Write(sb.ToString());
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}
}
