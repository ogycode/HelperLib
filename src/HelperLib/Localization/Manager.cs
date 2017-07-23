/*
 * Manager.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verloka.HelperLib.Localization
{
    /// <summary>
    /// Class fo work with localization of application
    /// </summary>
    public class Manager
    {
        public const string CODE = "code";
        public const string NULL_NAME = "null";
        public const string CURRENT_KEY = "current";
        public const string EMPTY_VALUE = "empty";

        /// <summary>
        /// Occurs when current Language was change
        /// </summary>
        public event Action<Manager> LanguageChanged;
        /// <summary>
        /// Occurs when can not load a file of locales
        /// </summary>
        public event Action<string> LoadError;

        /// <summary>
        /// List of available languages in locale file
        /// </summary>
        public List<Language> AvailableLanguages { get; private set; }
        /// <summary>
        /// Current Language
        /// </summary>
        public Language Current { get; private set; }
        /// <summary>
        /// Path to file with locales
        /// </summary>
        public string Path { get; private set; }
        public string this[string key]
        {
            get
            {
                if (Current.Name != NULL_NAME)
                    return file[Current.Name][key] != null ? file[Current.Name][key].ToString() : key;

                return key;
            }
        }

        INI.INIFile file;

        /// <summary>
        /// Initializes a new instance of the Manager class
        /// </summary>
        /// <param name="path">Path to file with locales</param>
        public Manager(string path)
        {
            Path = path;
            AvailableLanguages = new List<Language>();
            Current = new Language() { Name = NULL_NAME };
        }

        /// <summary>
        /// Loading locales from file
        /// </summary>
        public void Load()
        {
            if (!File.Exists(Path))
            {
                LoadError?.Invoke($"File {Path} not found!");
                return;
            }

            file = new INI.INIFile(Path, "=", ";");
            UpdateAvailableLanguages();
            SetCurrent(file.Read<string>(CURRENT_KEY));
        }
        /// <summary>
        /// Update List<Language> of available languages in locale file
        /// </summary>
        public void UpdateAvailableLanguages()
        {
            AvailableLanguages.Clear();
            foreach (var item in file.Sections)
                if (!item.IsRoot)
                {
                    Language l = new Language() { Name = item.Name, Code = item[CODE].ToString() };
                    AvailableLanguages.Add(l);
                }
        }
        /// <summary>
        /// Setup current Language by code
        /// </summary>
        /// <param name="code">Code of language</param>
        public void SetCurrent(string code)
        {
            Language lang = AvailableLanguages.SingleOrDefault(l => l.Code == code);

            if (lang == null)
                return;

            Current = lang;

            file.Write(CURRENT_KEY, Current.Code);

            LanguageChanged?.Invoke(this);
        }
        /// <summary>
        /// Return all keys in file
        /// </summary>
        /// <returns></returns>
        public List<string> Keys() => file.Sections.Count > 1 ? (List<string>)file.Sections[1].GetKeys() : new List<string>();
        /// <summary>
        /// Return value by Language name and key of the value
        /// </summary>
        /// <param name="lang">Language name</param>
        /// <param name="name">Key of value</param>
        /// <returns></returns>
        public string GetValueByLanguage(string lang, string name)
        {
            return file[lang][name]?.ToString();
        }
        /// <summary>
        /// Adding key in all Languages in AvailableLanguages
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True - key added</returns>
        public bool AddKey(string key)
        {
            if(IsValidKey(key))
            {
                foreach (var item in file.Sections)
                    if (!item.IsRoot)
                        item.Add(key, EMPTY_VALUE);

                return true;
            }

            return false;
        }
        /// <summary>
        /// Edit a value by Language name and key of value
        /// </summary>
        /// <param name="locale">Language name</param>
        /// <param name="key">Key of value</param>
        /// <param name="value">Value</param>
        public void EditKey(string locale, string key, string value)
        {
            file[locale][key] = value;
        }
        /// <summary>
        /// Add new Language by name and code
        /// </summary>
        /// <param name="name">Language name</param>
        /// <param name="code">Code of language</param>
        /// <returns>True - locale added</returns>
        public bool AddLocale(string name, string code)
        {
            var s1 = AvailableLanguages.SingleOrDefault(l => l.Name == name);
            var s2 = AvailableLanguages.SingleOrDefault(l => l.Code == code);

            if (s1 != null || s2 != null)
                return false;

            INI.Section sec = file[name];
            sec[CODE] = code;

            UpdateAvailableLanguages();

            if (file.Sections.Count == 2)
                return true;

            foreach (var item in file.Sections[1].GetKeys())
                if (item != CODE)
                    sec[item] = EMPTY_VALUE;
            
            return true;
        }
        /// <summary>
        /// Remove a key from all Languages in AvailableLanguages
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True - key removed</returns>
        public bool RemoveKey(string key)
        {
            bool res = false;

            foreach (var item in file.Sections)
                if (item.Remove(key))
                    res = true;

            return res;
        }
        /// <summary>
        /// Remove Language by name
        /// </summary>
        /// <param name="name">Language name</param>
        /// <returns>True - locale removed</returns>
        public bool RemoveLocale(string name)
        {
            bool res = file.RemoveSection(name);
            UpdateAvailableLanguages();
            return res;
        }
        /// <summary>
        /// Renane Language by name
        /// </summary>
        /// <param name="name">Old language name</param>
        /// <param name="newName">New language name</param>
        /// <returns>True - locale removed</returns>
        public bool RenameLocale(string name, string newName)
        {
            var sec = file.Sections.SingleOrDefault(l => l.Name == name);

            if (sec == null)
                return false;

            sec.SetName(newName);
            UpdateAvailableLanguages();

            return true;
        }
        /// <summary>
        /// Save locales data to file
        /// </summary>
        public void Save()
        {
            file.Save();
        }

        bool IsValidKey(string key)
        {
            switch (key)
            {
                case NULL_NAME:
                    return false;
                case CODE:
                    return false;
                default:
                    break;
            }

            return true;
        }
    }
}