using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Verloka.HelperLib.Update
{
    public class DownloadClient
    {
        public event Action<string, int, double> DownloadProgress;
        public event Action DownloadCompleted;

        WebClient webClient;              
        Stopwatch sw;

        string CurrentUrl;
        string CurrentPath;
        string CurrentName;

        string SavePath;
        List<string> Files;
        int TotlaPercent;
        
        public void Start()
        {
            PrepareDownload();
        }
        public DownloadClient(List<string>Files, string SavePath)
        {
            sw = new Stopwatch();

            this.SavePath = SavePath;
            this.Files = Files;

            TotlaPercent = 100 * this.Files.Count;
        }

        bool PrepareDownload()
        {
            if (Files.Count == 0)
                return true;

            CurrentUrl = Files.First();
            Files.Remove(CurrentUrl);
            CurrentName = System.IO.Path.GetFileName(new Uri(CurrentUrl).LocalPath);
            CurrentPath = $"{SavePath}\\{CurrentName}";

            DownloadFile(CurrentUrl, CurrentPath);
            return false;
        }

        public void DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                sw.Start();
                try
                {
                    webClient.DownloadFileAsync(new Uri(urlAddress), location);
                }
                catch { }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double speed = e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds;
            int perc = e.ProgressPercentage;
            DownloadProgress?.Invoke(CurrentName, perc, speed);
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();
            if (PrepareDownload())
                DownloadCompleted?.Invoke();
        }
    }
}
