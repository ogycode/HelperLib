using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Client for downloading update files
    /// </summary>
    public class DownloadClient : IDisposable
    {
        /// <summary>
        /// Event to display the change download files
        /// </summary>
        public event Action<string, int, double> DownloadProgress;
        /// <summary>
        /// Event when downloading is end
        /// </summary>
        public event Action DownloadCompleted;

        WebClient webClient;
        Stopwatch sw;

        string CurrentUrl;
        string CurrentPath;
        string CurrentName;

        string SavePath;
        List<string> Files;
        int TotalPerc = -100;
        int FileCount = 0;

        /// <summary>
        /// Initializes an object of DownloadClient
        /// </summary>
        /// <param name="Files">List of files to download</param>
        /// <param name="SavePath">Path when files need save (~temp folder as example)</param>
        public DownloadClient(List<string> Files, string SavePath)
        {
            sw = new Stopwatch();

            this.SavePath = SavePath;
            this.Files = Files;
            FileCount = this.Files.Count;
        }

        /// <summary>
        /// Start downloading files
        /// </summary>
        public void Start()
        {
            PrepareDownload();
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

        /// <summary>
        /// Download single file
        /// </summary>
        /// <param name="urlAddress">Url address</param>
        /// <param name="location">Path to save</param>
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
            int perc = (e.ProgressPercentage + TotalPerc) / FileCount;
            DownloadProgress?.Invoke(CurrentName, perc, speed);
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();
            if (PrepareDownload())
                DownloadCompleted?.Invoke();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
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
                }

                sw.Stop();
                sw = null;

                disposedValue = true;
            }
        }

        ~DownloadClient()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
