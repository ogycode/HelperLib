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
        public event Action<WebException> WebException;

        WebClient webClient;
        Stopwatch sw;

        string CurrentUrl;
        string CurrentPath;
        string CurrentName;

        string SavePath;
        List<string> Files;
        int TotalPerc = -100;
        int FileCount = 0;
        
        public DownloadClient(List<string> Files, string SavePath)
        {
            sw = new Stopwatch();

            this.SavePath = SavePath;
            this.Files = Files;
            FileCount = this.Files.Count;
        }
        
        public void Start()
        {
            PrepareDownload();
        }
        public void Close()
        {
            DownloadCompleted = null;
            DownloadProgress = null;

            CurrentUrl = null;
            CurrentPath = null;
            CurrentName = null;

            SavePath = null;
            Files.Clear();
            Files = null;

            TotalPerc = 0;
            FileCount = 0;

            sw.Stop();
            sw = null;

            GC.SuppressFinalize(this);
        }

        bool PrepareDownload()
        {
            if (Files.Count == 0)
                return true;

            TotalPerc = TotalPerc + 100;
            CurrentUrl = Files.First();
            Files.Remove(CurrentUrl);
            CurrentName = System.IO.Path.GetFileName(new Uri(CurrentUrl).LocalPath);
            CurrentPath = $"{SavePath}\\{CurrentName}";

            DownloadFile(CurrentUrl, CurrentPath);
            return false;
        }
        
        public void DownloadFile(string urlAddress, string location)
        {
            sw.Start();
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                try
                {
                    webClient.DownloadFileAsync(new Uri(urlAddress), location);
                }
                catch (WebException e)
                {
                    WebException?.Invoke(e);
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double speed = e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds;
            int perc = (e.ProgressPercentage + TotalPerc) / FileCount;
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
