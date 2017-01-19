using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HelperLib.Settings
{
    public class RegSettings : IDisposable, IEnumerable
    {
        Dictionary<string, object> settings;
        RegistryKey Key;
        RegistryKey KeyCustom;

        public object this[string index]
        {
            get
            {
                return settings.ContainsKey(index) ? settings[index] : null;
            }
            set
            {
                SetValue(index, value);
            }
        }
        public string AppName { get; set; }

        public RegSettings(string AppName)
        {
            settings = new Dictionary<string, object>();
            this.AppName = AppName;
            Key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings");
            KeyCustom = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings\\ISettingStruct");
            Load();
        }

        public bool DeleteValue(string name)
        {
            if (settings.ContainsKey(name))
            {
                settings.Remove(name);
                if (KeyCustom.GetValue(name) != null)
                    KeyCustom.DeleteValue(name);
                else
                    Key.DeleteValue(name);
            }
            return false;
        }
        public object SetValue<T>(string name, T value)
        {
            string obj = value is ISettingStruct ? (value as ISettingStruct).GetValue() : value.ToString();

            if (settings.ContainsKey(name))
                settings[name] = obj;
            else
                settings.Add(name, obj);

            if (value is ISettingStruct)
                KeyCustom.SetValue(name, (value as ISettingStruct).GetValue());
            else
                Key.SetValue(name, value);

            return value;
        }
        public T GetValue<T>(string name)
        {
            object obj = null;
            if (settings.ContainsKey(name))
                obj = Key.GetValue(name);
            else
                return default(T);

            if (obj is T)
                return (T)obj;
            else
            {
                try { return (T)Convert.ChangeType(obj, typeof(T)); }
                catch { return default(T); }
            }
        }
        public T GetValue<T>(string name, T defaultValue)
        {
            object obj = null;
            if (settings.ContainsKey(name))
                obj = Key.GetValue(name);
            else
                return defaultValue;

            if (obj is T)
                return (T)obj;
            else
            {
                try { return (T)Convert.ChangeType(obj, typeof(T)); }
                catch { return defaultValue; }
            }
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

            foreach (var item in KeyCustom.GetValueNames())
                settings.Add(item, KeyCustom.GetValue(item));
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
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)settings).GetEnumerator();
        }
    }
}
