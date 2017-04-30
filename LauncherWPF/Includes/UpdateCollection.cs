using System;
using System.Collections;
using System.IO;

namespace Cartographer_Launcher.Includes
{
    public class UpdateCollection : CollectionBase, IEnumerable
    {
        public void AddObject(UpdateObject UO)
        {
            List.Add(UO);
        }

        public UpdateObject this[int i]
        {
            get { return (UpdateObject)List[i]; }
            set { List[i] = value; }
        }

        public UpdateObject this[string name]
        {
            get
            {
                for (int i = 0; i < List.Count; i++)
                    if (((UpdateObject)List[i]).name == name)
                        return ((UpdateObject)List[i]);
                return null;
            }
            set
            {
                for (int i = 0; i < List.Count; i++)
                    if (((UpdateObject)List[i]).name == name)
                        List[i] = value;
            }
        }
    }

    public class UpdateObject : object
    {
        public string name;
        public string version;
        private string _localpath;
        private string _remotepath;
        public bool replaceoriginal;
        public string remotepath
        {
            get { return FormatPath(_remotepath); }
            set { _remotepath = value; }
        }

        public string localpath
        {
            get
            {
                string LocalDirectoryPath = Path.GetDirectoryName(FormatPath(_localpath));
                if (!Directory.Exists(LocalDirectoryPath))
                    Directory.CreateDirectory(LocalDirectoryPath);
                return FormatPath(_localpath);
            }
            set { _localpath = value; }
        }

        private string FormatPath(string str)
        {
            string tString = str;
            tString = tString.Replace("{Version}", version);
            tString = tString.Replace("{InstallDir}", Globals.GameDirectory);
            tString = tString.Replace("{Files}", Globals.Files);
            tString = tString.Replace("{LauncherDir}", Globals.H2vHubDirectory);
            tString = tString.Replace("{RemoteDir}", Globals.RemoteUpdate);
            tString = tString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory);
            return tString;
        }

        public override bool Equals(object UO)
        {
            UpdateObject tUO = ((UpdateObject)UO);
            if (UO == null)
                return false;
            else
            {
                if (localpath == tUO.localpath && remotepath == tUO.remotepath && name == tUO.name && version == tUO.version)
                    return true;
                return false;
            }
        }

        public override int GetHashCode()
        {
            int tHash = 0;
            foreach (char c in name)
                tHash += c.GetHashCode();
            return tHash;
        }
    }
}
