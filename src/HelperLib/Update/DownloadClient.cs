/*
 * DownloadClient.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Class for download files (single and multiple)
    /// </summary>
    public class DownloadClient
    {
        /// <summary>
        /// Occurs when download progress was changed
        /// </summary>
        public event Action<string, int, double> DownloadProgress;
        /// <summary>
        /// Occurs when download all files is completed
        /// </summary>
        public event Action DownloadCompleted;
        /// <summary>
        /// Occurs when there are exceptions by WebClient
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the DownloadClient clas
        /// </summary>
        public DownloadClient()
        {
            sw = new Stopwatch();

            SavePath = @"C:\\";
            Files = new List<string>();
            FileCount = Files.Count;
        }
        /// <summary>
        /// Initializes a new instance of the DownloadClient clas
        /// </summary>
        /// <param name="Files">List with files will be downloaded, can be empty</param>
        /// <param name="SavePath">Destination location for save files</param>
        public DownloadClient(List<string> Files, string SavePath)
        {
            sw = new Stopwatch();

            this.SavePath = SavePath;
            this.Files = Files;
            FileCount = this.Files.Count;
        }

        /// <summary>
        /// Start download all files from list
        /// </summary>
        public void Start()
        {
            PrepareDownload();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
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
        /// <summary>
        /// Download single file
        /// </summary>
        /// <param name="urlAddress">Url's file</param>
        /// <param name="location">Destination location for save files</param>
        public void DownloadFile(string urlAddress, string location)
        {
            sw.Start();
            using (webClient = new WebClient())
            {
                HttpWebResponse response = null;
                var request = (HttpWebRequest)WebRequest.Create(urlAddress);
                request.Method = "HEAD";
                
                try
                {
                    response = (HttpWebResponse)request.GetResponse();

                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedSingle);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChangedSingle);
                    try
                    {
                        webClient.DownloadFileAsync(new Uri(urlAddress), location);
                    }
                    catch (WebException e)
                    {
                        WebException?.Invoke(e);
                    }
                }
                catch (WebException e) {  WebException?.Invoke(e); }
                finally
                {
                    if (response != null)
                        response.Close();
                }
            }
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

            download(CurrentUrl, CurrentPath);
            return false;
        }
        void download(string urlAddress, string location)
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

        private void ProgressChangedSingle(object sender, DownloadProgressChangedEventArgs e)
        {
            double speed = e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds;
            int perc = (e.ProgressPercentage + TotalPerc);
            DownloadProgress?.Invoke(CurrentName, perc, speed);
        }
        private void CompletedSingle(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();
            DownloadCompleted?.Invoke();
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
