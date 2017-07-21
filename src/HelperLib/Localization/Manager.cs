using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verloka.HelperLib.Localization
{
    public class Manager
    {
        public const string CODE = "code";
        public const string NULL_NAME = "null";
        public const string CURRENT_KEY = "current";
        public const string EMPTY_VALUE = "empty";

        public event Action<Manager> LanguageChanged;
        public event Action<string> LoadError;

        public List<Language> AvailableLanguages { get; private set; }
        public Language Current { get; private set; }
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

        public Manager(string path)
        {
            Path = path;
            AvailableLanguages = new List<Language>();
            Current = new Language() { Name = NULL_NAME };
        }

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
        public void SetCurrent(string code)
        {
            Language lang = AvailableLanguages.SingleOrDefault(l => l.Code == code);

            if (lang == null)
                return;

            Current = lang;

            file.Write(CURRENT_KEY, Current.Code);

            LanguageChanged?.Invoke(this);
        }
        public List<string> Keys() => file.Sections.Count > 1 ? (List<string>)file.Sections[1].GetKeys() : new List<string>();
        public string GetValueByLanguage(string lang, string name)
        {
            return file[lang][name]?.ToString();
        }
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
        public void EditKey(string locale, string key, string value)
        {
            file[locale][key] = value;
        }
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
        public bool RemoveKey(string key)
        {
            bool res = false;

            foreach (var item in file.Sections)
                if (item.Remove(key))
                    res = true;

            return res;
        }
        public bool RemoveLocale(string name)
        {
            bool res = file.RemoveSection(name);
            UpdateAvailableLanguages();
            return res;
        }
        public bool RenameLocale(string name, string newName)
        {
            var sec = file.Sections.SingleOrDefault(l => l.Name == name);

            if (sec == null)
                return false;

            sec.SetName(newName);
            UpdateAvailableLanguages();

            return true;
        }
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