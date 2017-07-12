using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verloka.HelperLib.Localization
{
    public class Manager
    {
        public event Action<Manager> LanguageChanged;
        public event Action<string> LoadError;

        public List<Language> AvailableLanguages { get; private set; }
        public Language Current { get; private set; }
        public string Path { get; private set; }
        public string this[string key]
        {
            get
            {
                if (Current.Name != "null")
                    return file[Current.Name][key] != null ? file[Current.Name][key].ToString() : key;

                return key;
            }
        }

        INI.INIFile file;

        public Manager(string path)
        {
            Path = path;
            AvailableLanguages = new List<Language>();
            Current = new Language() { Name = "null" };
        }

        public void Load()
        {
            if (!File.Exists(Path))
            {
                LoadError?.Invoke($"File {Path} not found!");
                return;
            }

            file = new INI.INIFile(Path, "=", ";", System.Text.Encoding.UTF8);
            UpdateAvailableLanguages();
            SetCurrent(file.Read<string>("current"));
        }
        public void UpdateAvailableLanguages()
        {
            AvailableLanguages.Clear();
            foreach (var item in file.Sections)
                if (!item.IsRoot)
                {
                    Language l = new Language() { Name = item.Name, Code = item["code"].ToString() };
                    AvailableLanguages.Add(l);
                }
        }
        public void SetCurrent(string code)
        {
            Current = AvailableLanguages.SingleOrDefault(l => l.Code == code);
            LanguageChanged?.Invoke(this);
        }
        public List<string> Keys() => file.Sections.Count > 1 ? (List<string>)file.Sections[1].GetKeys() : new List<string>();
        public string GetValueByLanguage(string lang, string name)
        {
            return file[lang][name].ToString();
        }
        public void AddNode(string key)
        {
            foreach (var item in file.Sections)
                if (!item.IsRoot)
                    item.Add(key, "");
        }
        public bool RemoveKey(string key)
        {
            return true;
        }
        public bool RemoveLocale(string name)
        {
            bool b = file.RemoveSection(name);
            UpdateAvailableLanguages();
            return b;
        }
    }
}