using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Verloka.HelperLib.Settings
{
    /// <summary>
    /// General class fro working with settings
    /// </summary>
    public class RegSettings : IDisposable, IEnumerable
    {
        Dictionary<string, object> settings;
        RegistryKey Key;
        RegistryKey KeyCustom;

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">name of setting</param>
        /// <returns>setting in object type</returns>
        public object this[string index]
        {
            set
            {
                SetValue(index, value);
            }
        }
        /// <summary>
        /// Name of application
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="AppName">Name of application</param>
        public RegSettings(string AppName)
        {
            settings = new Dictionary<string, object>();
            this.AppName = AppName;
            Key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings");
            KeyCustom = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{this.AppName}\\Settings\\ISettingStruct");
            Load();
        }

        /// <summary>
        /// Delete value from register
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <returns>True - successful; False - faild</returns>
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
        /// <summary>
        /// Edit or add value
        /// </summary>
        /// <typeparam name="T">Any system type or ISettingStruct</typeparam>
        /// <param name="name">Name of setting</param>
        /// <param name="value">Value for adding or editing</param>
        /// <returns>Value which added or edited</returns>
        public object SetValue<T>(string name, T value)
        {
            string obj;
            if (value != null)
                obj = value is ISettingStruct ? (value as ISettingStruct).GetValue() : value.ToString();
            else
                obj = "";

            if (settings.ContainsKey(name))
                settings[name] = obj;
            else
                settings.Add(name, obj);

            if (value is ISettingStruct)
                KeyCustom.SetValue(name, (value as ISettingStruct).GetValue());
            else
                Key.SetValue(name, (value == null) ? new object() : value);

            return value;
        }
        /// <summary>
        /// Get value by name
        /// </summary>
        /// <typeparam name="T">Any system type or ISettingStruct</typeparam>
        /// <param name="name">Name of value</param>
        /// <returns>Value by name</returns>
        public T GetValue<T>(string name)
        {
            if (settings.ContainsKey(name))
            {
                T instance;

                try { instance = (T)Activator.CreateInstance(typeof(T)); }
                catch { instance = default(T); }

                if (instance is ISettingStruct)
                {
                    return GetCustom<T>(name);
                }
                else
                {
                    return GetStandart<T>(name);
                }
            }
            else
            {
                T instance;

                try { instance = (T)Activator.CreateInstance(typeof(T)); }
                catch { instance = default(T); }

                return (T)SetValue(name, instance);
            }
        }
        /// <summary>
        /// Get value by name
        /// </summary>
        /// <typeparam name="T">Any system type or ISettingStruct</typeparam>
        /// <param name="name">Name of value</param>
        /// <param name="defaultValue">Вefault value if the value is not there</param>
        /// <returns>Value by name</returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            if (settings.ContainsKey(name))
            {
                T instance;

                try { instance = (T)Activator.CreateInstance(typeof(T)); }
                catch (Exception) { instance = default(T); }

                if (instance is ISettingStruct)
                {
                    return GetCustom<T>(name);
                }
                else
                {
                    return GetStandart<T>(name);
                }
            }
            else
            {
                return (T)SetValue(name, defaultValue);
            }
        }
        /// <summary>
        /// Delete all settings from register
        /// </summary>
        public void Clear()
        {
            var s = Key.GetValueNames();
            var c = KeyCustom.GetValueNames();

            foreach (var item in s)
                Key.DeleteValue(item);

            foreach (var item in c)
                KeyCustom.DeleteValue(item);

            settings.Clear();
        }

        void Load()
        {
            foreach (var item in Key.GetValueNames())
                settings.Add(item, Key.GetValue(item));

            foreach (var item in KeyCustom.GetValueNames())
                settings.Add(item, KeyCustom.GetValue(item));
        }
        T GetStandart<T>(string name)
        {
            object obj = Key.GetValue(name);
            try { return (T)Convert.ChangeType(obj, typeof(T)); }
            catch { return default(T); }
        }
        T GetCustom<T>(string name)
        {
            object obj = KeyCustom.GetValue(name);
            var a = Activator.CreateInstance(typeof(T));
            (a as ISettingStruct).SetValue(obj.ToString());
            return (T)a;
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
