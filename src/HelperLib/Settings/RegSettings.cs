using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HelperLib.Settings
{
    public class RegSettings : IDisposable, IEnumerable
    {
        Dictionary<string, string> settings;
        RegistryKey Key;
        RegistryKey KeyCustom;

        public string this[string index]
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
            settings = new Dictionary<string, string>();
            this.AppName = AppName;
            Key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings");
            KeyCustom = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings\\ISettingStruct");
            Load();
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
                try
                {
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
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
                settings.Add(item, Key.GetValue(item).ToString());

            foreach (var item in KeyCustom.GetValueNames())
                settings.Add(item, KeyCustom.GetValue(item).ToString());
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
