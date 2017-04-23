using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartographer_Launcher.Includes.Dependencies
{
    public static class Kantanomo
    {
        private static List<string> _Nouns = new List<string>()
        {
              "cheif", "man", "dude", "partner", "bro", "buddy" , "hombre"
        };
        private static List<string> _Pauses = new List<string>()
        {
              "minute", "sec", "moment"
        };
        private static List<string> _Fuck = new List<string>()
        {
            "Wait", "Stop", "Hold on", "Hang on", "Hold up"
        };
        private static Dictionary<string, List<List<string>>> _PauseIdioms = new Dictionary<string, List<List<string>>>()
        {
            { "{0} a {1} {2}!", new List<List<string>>(){_Fuck, _Pauses, _Nouns } },
            { "Hold on {0}!", new List<List<string>>(){ _Nouns } },
            { "Hold up {0}!", new List<List<string>>(){ _Nouns } },
            { "Stop right there {0}", new List<List<string>>(){ _Nouns } },
            { "Not today {0}", new List<List<string>>(){ _Nouns } }

        };
        private static Dictionary<string, List<List<string>>> _GoIdioms = new Dictionary<string, List<List<string>>>()
        {
            { "Good job {0}!", new List<List<string>>(){ _Nouns } },
            { "Congratulations {0}!", new List<List<string>>(){ _Nouns } },
            { "You got it {0}!", new List<List<string>>(){ _Nouns } }
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
