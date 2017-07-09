using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Verloka.HelperLib.Update
{
    public class UpdateClient : IDisposable
    {
        public event Action<UpdateItem> NewVersion;
        public event Action<WebException> WebException;
        
        public string Url { get; set; }
        
        public UpdateClient(string Url)
        {
            this.Url = Url;
        }
        
        public void Check(Version v)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    string resp = webClient.DownloadString(Url);
                    UpdateItem upd = Deserialize<UpdateItem>(resp);

                    if (upd.VersionNumber > v)
                        NewVersion?.Invoke(upd);
                }
                catch (WebException e)
                {
                    WebException?.Invoke(e);
                }
            }
        }
        
        public static T Deserialize<T>(string json)
        {
            T obj = default(T);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                    settings.UseSimpleDictionaryFormat = true;

                    var ser = new DataContractJsonSerializer(typeof(T), settings);
                    obj = (T)ser.ReadObject(ms);
                }
            }
            catch
            {
                obj = (T)Activator.CreateInstance(typeof(T));
            }

            return obj;
        }
        public static string Serialize(object instance)
        {
            string json = string.Empty;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                    settings.UseSimpleDictionaryFormat = true;

                    var ser = new DataContractJsonSerializer(instance.GetType(), settings);
                    ser.WriteObject(ms, instance);
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms))
                    { json = sr.ReadToEnd(); }
                }
            }
            catch { }
            return json;
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    NewVersion = null;
                    WebException = null;

                    Url = null;
                }

                disposedValue = true;
            }
        }

        ~UpdateClient()
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
