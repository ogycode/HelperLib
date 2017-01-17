using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HelperLib.Settings
{
    public class RegSettings : IDisposable, IEnumerable, IEnumerator
    {
        Dictionary<string, object> settings;
        RegistryKey Key;

        public object this[string index]
        {
            get
            {
                return settings.ContainsKey(index) ? settings[index] : null;
            }
            set
            {
                if (settings.ContainsKey(index))
                    settings[index] = value;
                else
                    SetValue(index, value);
            }
        }
        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                int numer = -1;
                foreach (var item in settings)
                {
                    if (index == numer)
                        return item;
                    numer++;
                }
                return default(KeyValuePair<string, object>);                  
            }
            set
            {
                int numer = -1;
                string name = string.Empty;

                foreach (var item in settings)
                {
                    if (index == numer)
                        name = item.Key;
                    numer++;
                }

                if(string.IsNullOrWhiteSpace(name))
                    SetValue(name, value);
                else
                    settings[name] = value;
            }
        }

        public string AppName { get; set; }

        public RegSettings(string AppName)
        {
            settings = new Dictionary<string, object>();
            this.AppName = AppName;
            Key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings");
            Load();
        }

        public object SetValue(string name, object value)
        {
            if (settings.ContainsKey(name))
                settings[name] = value;
            else
                settings.Add(name, value);

            Key.SetValue(name, value);
            return value;
        }
        public T GetValue<T>(string name)
        {
            if (settings.ContainsKey(name))
                return (T)settings[name];
            else
                return default(T);
        }
        public void Clear()
        {
            foreach (var item in settings)
                Key.DeleteValue(item.Key);
            settings.Clear();
        }

        void Load()
        {
            foreach (var item in Key.GetValueNames())
                settings.Add(item, Key.GetValue(item));
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    settings.Clear();
                    settings = null;
                }

                Key.Close();

                disposedValue = true;
            }
        }
        ~RegSettings()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IEnumerable, IEnumerator Support
        int index = -1;
        public IEnumerator GetEnumerator()
        {
            return this;
        }
        public bool MoveNext()
        {
            if (index >= settings.Count)
            {
                Reset();
                return false;
            }

            index++;
            return true;
        }
        public void Reset()
        {
            index = -1;
        }
        public object Current
        {
            get
            {
                return this[index];
            }
        }
        #endregion
    }
}
