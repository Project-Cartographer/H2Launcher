using System;
using System.Collections.Generic;
using System.Linq;

namespace Cartographer_Launcher.Includes.Dependencies
{
	public static class Kantanomo
	{
		private static List<string> _Nouns = new List<string>()
		{
			  "CHIEF", "MAN", "DUDE", "PARTNER", "BRO", "BUDDY" , "HOMBRE"
		};
		private static List<string> _Pauses = new List<string>()
		{
			  "MINUTE", "SEC", "MOMENT"
		};
		private static List<string> _Fuck = new List<string>()
		{
			"WAIT", "STOP", "HOLD ON", "HANG ON", "HOLD UP"
		};
		private static Dictionary<string, List<List<string>>> _PauseIdioms = new Dictionary<string, List<List<string>>>()
		{
			{ "{0} A {1} {2}", new List<List<string>>(){_Fuck, _Pauses, _Nouns } },
			{ "HOLD ON {0}", new List<List<string>>(){ _Nouns } },
			{ "HOLD UP {0}", new List<List<string>>(){ _Nouns } },
			{ "STOP RIGHT THERE {0}", new List<List<string>>(){ _Nouns } },
			{ "NOT TODAY {0}", new List<List<string>>(){ _Nouns } }

		};
		private static Dictionary<string, List<List<string>>> _GoIdioms = new Dictionary<string, List<List<string>>>()
		{
			{ "GOOD JOB {0}", new List<List<string>>(){ _Nouns } },
			{ "CONGRATULATIONS {0}", new List<List<string>>(){ _Nouns } },
			{ "YOU GOT IT {0}", new List<List<string>>(){ _Nouns } }
		};

		public static string PauseIdiomGenerator
		{
			get
			{
				Random rand = new Random();
				List<string> keys = Enumerable.ToList(_PauseIdioms.Keys);
				int size = _PauseIdioms.Count;
				string IdiomBase = keys[rand.Next(0, size)];
				string Idiom = IdiomBase;
				for (int i = 0; i < _PauseIdioms[IdiomBase].Count; i++)
				{
					List<string> Param = _PauseIdioms[IdiomBase][i];
					Idiom = Idiom.Replace("{" + i + "}", Param[rand.Next(0, Param.Count)]);
				}
				return Idiom;
			}
		}
		public static string GoIdioms
		{
			get
			{
				Random rand = new Random();
				List<string> keys = Enumerable.ToList(_GoIdioms.Keys);
				int size = _GoIdioms.Count;
				string IdiomBase = keys[rand.Next(0, size)];
				string Idiom = IdiomBase;
				for (int i = 0; i < _GoIdioms[IdiomBase].Count; i++)
				{
					List<string> Param = _GoIdioms[IdiomBase][i];
					Idiom = Idiom.Replace("{" + i + "}", Param[rand.Next(0, Param.Count)]);
				}
				return Idiom;
			}
		}
	}
}