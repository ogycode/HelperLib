using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace HelperLib.Settings
{
    public class RegSettings : IDisposable
    {
        public event Action<RegSettings, string> AlreadyExistSetting;

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
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~RegSettings() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
