using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Verloka.HelperLib.Update
{
    public class UpdateClient
    {
        public event Action<WebException> WebException;

        public List<UpdateElement> Elements { get; private set; }
        public UpdateElement Last { get; private set; }
        public string Url { get; private set; }
        
        public UpdateClient(string Url)
        {
            this.Url = Url;
        }
        
        public async void LoadData()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string resp = await client.DownloadStringTaskAsync(Url);
                    Read(resp);
                }
                catch (WebException e)
                {
                    WebException?.Invoke(e);
                }
            }
        }
        public bool IsAvailable(Version ver)
        {
            return CheckVersion(ver);
        }
        public bool IsAvailable(System.Version ver)
        {
            return CheckVersion(new Version(ver.Major, ver.Minor, ver.Build, ver.Revision));
        }
        public bool IsAvailable(int Major, int Minor, int Build, int Revision)
        {
            return CheckVersion(new Version(Major, Minor, Build, Revision));
        }
        public void Close()
        {
            Elements.Clear();
            Elements = null;
            Last = null;
            Url = null;

            GC.SuppressFinalize(this);
        }

        bool CheckVersion(Version ver)
        {
            return Last.VersionNumber > ver;
        }
        void Read(string resp)
        {
            INI.INIFile file = new INI.INIFile(resp);
        }
    }
}
