/*
 * RegSettings.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Verloka.HelperLib.Settings
{
    /// <summary>
    /// Class for working with app's settings
    /// </summary>
    public class RegSettings : IDisposable, IEnumerable
    {
        Dictionary<string, object> settings;
        RegistryKey Key;
        RegistryKey KeyCustom;

        /// <summary>
        /// Application name
        /// </summary>
        public string AppName { get; set; }
        public object this[string index]
        {
            set
            {
                SetValue(index, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the RegSettings class
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
        /// Remove parametr from settings and register
        /// </summary>
        /// <param name="name">Name of param</param>
        /// <returns>True - param removed</returns>
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
        /// Edit conatains setting or add new
        /// </summary>
        /// <typeparam name="T">Type of your param</typeparam>
        /// <param name="name">Name of param</param>
        /// <param name="value">Param for write</param>
        /// <returns>Param in converted to object</returns>
        public object SetValue<T>(string name, T value)
        {
            string obj;
            if (!Equals(value, default(T)))
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
                Key.SetValue(name, Equals(value, default(T)) ? new object() : value);

            return value;
        }
        /// <summary>
        /// Return setting by name
        /// </summary>
        /// <typeparam name="T">Type of your param</typeparam>
        /// <param name="name">Name of param</param>
        /// <returns>Param</returns>
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
        /// Return setting by name
        /// </summary>
        /// <typeparam name="T">Type of your param</typeparam>
        /// <param name="name">Name of param</param>
        /// <param name="defaultValue">Default value of param</param>
        /// <returns>Param</returns>
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
        /// Clear all settings
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

                Key?.Close();
                KeyCustom?.Close();

                disposedValue = true;
            }
        }
        ~RegSettings()
        {
            Dispose(false);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through a collection
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)settings).GetEnumerator();
        }
    }
}
