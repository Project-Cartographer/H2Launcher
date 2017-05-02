using Cartographer_Launcher.Includes.Dependencies;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using LauncherWPF;
using System.Reflection;

namespace Cartographer_Launcher.Includes
{


    public class UpdateController
    {
        //private UpdateCollection _RemoteUpdateCollection;
        //private UpdateCollection _LocalUpdateCollection;
        //private UpdateCollection _UpdateCollection;
        //private volatile bool _LauncherUpdated = false; //Flags the launcher to need a restart, to replace the current version with H2Launcher_temp.exe
        //private volatile string _Halo2Version = "1.00.00.11122";

        //    public bool LoadRemoteUpdateCollection()
        //    {
        //        try
        //        {
        //            mw.AddToDetails("Downloading remote update XML file....");
        //            if (File.Exists(Globals.Files + "RemoteUpdate.xml")) File.Delete(Globals.Files + "RemoteUpdate.xml");
        //            WebClient Client = new WebClient();
        //            bool _isDownloading = false;

        //            Client.DownloadFileCompleted += (s, e) =>
        //            {
        //                mw.UpdateProgress(100);
        //                mw.AddToDetails("Download Complete.");

        //                Client.Dispose();
        //                Client = null;
        //                _isDownloading = false;
        //            };
        //            Client.DownloadProgressChanged += (s, e) =>
        //            {
        //                mw.UpdateProgress(e.ProgressPercentage);
        //            };
        //            try
        //            {
        //                Client.DownloadFileAsync(new Uri(Globals.RemoteUpdateXML), Globals.Files + "RemoteUpdate.xml");
        //                _isDownloading = true;
        //            }
        //            catch (Exception) { throw new Exception("Error"); }
        //            while (_isDownloading) { }
        //            XDocument RemoteXML = XDocument.Load(Globals.Files + "RemoteUpdate.xml");
        //            UpdateCollection tUpdateColleciton = new UpdateCollection();
        //            //replaceoriginal = (XmlRoot.Element("localpath").HasAttributes) ? ((XmlRoot.Element("localpath").Attribute("replaceoriginal") != null) ? true : false) : false
        //            foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
        //                                   select
        //                                       new UpdateObject
        //                                       {
        //                                           localpath = (string)XmlRoot.Element("localpath"),
        //                                           remotepath = (string)XmlRoot.Element("remotepath"),
        //                                           version = (string)XmlRoot.Element("version"),
        //                                           name = (string)XmlRoot.Element("name")
        //                                       }
        //                                )
        //            )
        //            {
        //                tUpdateColleciton.AddObject((UpdateObject)UO);
        //            }
        //            _RemoteUpdateCollection = tUpdateColleciton;
        //            return true;

        //        }
        //        catch (Exception)
        //        {
        //            mw.AddToDetails("There was an issue loading the remote updates, try restarting. You can play the game normally still.");
        //            return false;
        //        }
        //    }

        //    public bool LoadLocalUpdateCollection()
        //    {
        //        try
        //        {
        //            if (File.Exists(Globals.Files + "LocalUpdate.xml"))
        //            {
        //                //await Task.Delay(0);
        //                XDocument RemoteXML = XDocument.Load(Globals.Files + "LocalUpdate.xml");
        //                UpdateCollection tUpdateColleciton = new UpdateCollection();
        //                foreach (object UO in (from XmlRoot in RemoteXML.Element("update").Elements("file")
        //                                       select
        //                                           new UpdateObject
        //                                           {
        //                                               localpath = (string)XmlRoot.Element("localpath"),
        //                                               remotepath = (string)XmlRoot.Element("remotepath"),
        //                                               version = (string)XmlRoot.Element("version"),
        //                                               name = (string)XmlRoot.Element("name")
        //                                               //replaceoriginal = (bool)XmlRoot.Element("localpath").Attribute("replaceoriginal")
        //                                           }
        //                                    )
        //                )
        //                {
        //                    if (File.Exists(((UpdateObject)UO).localpath.Replace("_temp", "")))
        //                        tUpdateColleciton.AddObject((UpdateObject)UO);
        //                }
        //                _LocalUpdateCollection = tUpdateColleciton;
        //                return true;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            mw.AddToDetails("There was an issue loading the local updates, try restarting. You can play the game normally still.");
        //            return false;
        //        }
        //    }

        //    public async void CheckUpdates()
        //    {
        //        await Task.Run(() =>
        //        {
        //            if (UpdateGameToLatest())
        //            {
        //                if (LoadLocalUpdateCollection())
        //                {
        //                    if (LoadRemoteUpdateCollection())
        //                    {
        //                        if (NeedToUpdate())
        //                        {
        //                            DownloadUpdates();
        //                            mw.AddToDetails("Updates Complete");
        //                            Task.Delay(1000);
        //                            Finished();
        //                        }
        //                        else
        //                        {
        //                            mw.AddToDetails("No Updates found.");
        //                            Task.Delay(1000);
        //                            mw.UpdaterFinished();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        mw.AddToDetails("Update Failed");
        //                        Task.Delay(1000);
        //                        Finished();
        //                    }
        //                }
        //                else
        //                {
        //                    mw.AddToDetails("Update Failed");
        //                    Task.Delay(1000);
        //                    Finished();
        //                }
        //            }
        //            else
        //            {
        //                mw.AddToDetails("Update Failed");
        //                Task.Delay(1000);
        //                Finished();
        //            }
        //        });
        //    }

        //    private bool UpdateGameToLatest()
        //    {
        //        string CurrentHalo2Version = FileVersionInfo.GetVersionInfo(Globals.GameDirectory + "halo2.exe").FileVersion;
        //        mw.AddToDetails(string.Format("Halo 2 Version Current Version: {0} Expected Version {1}", CurrentHalo2Version, _Halo2Version));
        //        if (_Halo2Version != CurrentHalo2Version)
        //        {
        //            mw.AddToDetails("Updating Halo 2 to the latest version");
        //            WebClient Client = new WebClient();
        //            bool _isDownloading = false;
        //            Client.DownloadFileCompleted += (s, e) =>
        //            {
        //                mw.UpdateProgress(100);
        //                mw.AddToDetails("Download Complete.");
        //                Client.Dispose();
        //                Client = null;
        //                _isDownloading = false;
        //            };
        //            Client.DownloadProgressChanged += (s, e) =>
        //            {
        //                mw.UpdateProgress(e.ProgressPercentage);
        //            };
        //            try
        //            {
        //                Client.DownloadFileAsync(new Uri(Globals.RemoteUpdate + "halo2/Update.exe"), Globals.Downloads + "\\Update.exe");
        //                _isDownloading = true;
        //            }
        //            catch (Exception) { throw new Exception("Error"); }
        //            while (_isDownloading) { }
        //            mw.AddToDetails("Waiting for update to finish installing");
        //            bool _isUpdating = true;
        //            Process.Start(Globals.Downloads + "\\Update.exe");
        //            while (_isUpdating)
        //            {
        //                if (Process.GetProcessesByName("Update").Length == 0)
        //                    _isUpdating = false;
        //            }
        //            File.Delete(Globals.Downloads + "\\Update.exe");
        //            return true;
        //        }
        //        return true;
        //    }

        //    private bool NeedToUpdate()
        //    {
        //        if (_LocalUpdateCollection != null)
        //        {
        //            _UpdateCollection = new UpdateCollection();
        //            foreach (UpdateObject UO in _RemoteUpdateCollection)
        //            {
        //                UpdateObject tUO = _LocalUpdateCollection[UO.name];
        //                if (tUO == null)
        //                    _UpdateCollection.AddObject(UO);
        //                else if (tUO.version != UO.version)
        //                    _UpdateCollection.AddObject(UO);
        //                else if (tUO.localpath != UO.localpath)
        //                    MoveFile(tUO.name, tUO.localpath, UO.localpath);

        //            }
        //        }
        //        _UpdateCollection = (_UpdateCollection != null) ? _UpdateCollection : _RemoteUpdateCollection;
        //        if (_UpdateCollection.Count > 0)
        //            return true;
        //        else
        //            return false;
        //    }

        //    private void DownloadUpdates()
        //    {

        //        for (int i = 0; i < _UpdateCollection.Count; i++)
        //        {
        //            UpdateObject tUO = _UpdateCollection[i];
        //            mw.AddToDetails("Downloading " + tUO.name + "....");
        //            if (tUO.name == "Halo_2_Launcher")
        //                _LauncherUpdated = true;
        //            WebClient Client = new WebClient();
        //            bool _isDownloading = false;
        //            Client.DownloadFileCompleted += (s, e) =>
        //            {
        //                mw.UpdateProgress(100);
        //                mw.AddToDetails("Download Complete.");
        //                Client.Dispose();
        //                Client = null;
        //                _isDownloading = false;
        //            };
        //            Client.DownloadProgressChanged += (s, e) =>
        //            {
        //                mw.UpdateProgress(e.ProgressPercentage);
        //            };
        //            try
        //            {
        //                Client.DownloadFileAsync(new Uri(tUO.remotepath), tUO.localpath);
        //                _isDownloading = true;
        //            }
        //            catch (Exception) { throw new Exception("Error"); }
        //            DownloadFile(tUO.remotepath, tUO.localpath);
        //            while (_isDownloading) { }
        //        }
        //    }
        //    //public void 

        //    public void Finished()
        //    {
        //        //public void 
        //        //await Task.Run(() => { while (!_Finished) { } 
        //        File.Delete(Globals.Files + "LocalUpdate.xml"); File.Copy(Globals.Files + "RemoteUpdate.xml", Globals.Files + "LocalUpdate.xml");
        //        //});
        //        if (_LauncherUpdated)
        //        {
        //            mw.AddToDetails("Restarting Launcher to complete update");
        //            Task.Delay(5000);
        //            ProcessStartInfo Info = new ProcessStartInfo();
        //            Info.Arguments = "/C ping 127.0.0.1 -n 1 -w 5000 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\" & ping 127.0.0.1 -n 1 -w 2000 > Nul & rename Halo_2_Launcher_temp.exe Halo_2_Launcher.exe & ping 127.0.0.1 -n 1 -w 20000 > Nul & start Halo_2_Launcher.exe";
        //            Info.WindowStyle = ProcessWindowStyle.Hidden;
        //            Info.CreateNoWindow = true;
        //            Info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //            Info.FileName = "cmd.exe";
        //            Process.Start(Info);
        //            Process.GetCurrentProcess().Kill();
        //        }
        //        else
        //        {
        //            mw.UpdaterFinished();
        //        }
        //    }
        //    //public void 
        //    private void MoveFile(string Name, string Source, string Destination)
        //    {
        //        using (Stream SourceStream = File.Open(Source, FileMode.Open))
        //        {
        //            using (Stream DestinationStream = File.Create(Destination))
        //            {
        //                mw.AddToDetails("Moving " + Name + " \r\n\tFrom " + Source + " \r\n\tTo " + Destination);
        //                mw.UpdateProgress(0);
        //                byte[] buffer = new byte[SourceStream.Length / 1024];
        //                int read;
        //                while ((read = SourceStream.Read(buffer, 0, buffer.Length)) > 0)
        //                {
        //                    DestinationStream.Write(buffer, 0, buffer.Length);
        //                    int progress = (read / buffer.Length) * 100;
        //                    mw.UpdateProgress(progress);
        //                }
        //                mw.UpdateProgress(100);
        //                mw.AddToDetails("Moving Complete");
        //            }
        //        }
        //        Task.Delay(500);
        //        File.Delete(Source);
        //    }

        //    private void DownloadFile(String remoteFilename, String localFilename)
        //    {
        //        bool _isDownloading = false;
        //        WebClient Client = new WebClient();
        //        Client.DownloadFileCompleted += (s, e) =>
        //        {
        //            mw.UpdateProgress(100);
        //            mw.AddToDetails("Download Complete.");
        //            Client.Dispose();
        //            Client = null;
        //            _isDownloading = false;
        //        };
        //        Client.DownloadProgressChanged += (s, e) =>
        //        {
        //            mw.UpdateProgress(e.ProgressPercentage);
        //        };
        //        try
        //        {
        //            mw.UpdateProgress(0);
        //            Client.DownloadFileAsync(new Uri(remoteFilename), localFilename);
        //            _isDownloading = true;
        //            while (_isDownloading) { }
        //        }
        //        catch (Exception) { throw new Exception("Error"); }
        //    }
        //}
    }
}
