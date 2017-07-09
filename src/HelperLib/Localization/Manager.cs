using System;
using System.Collections.Generic;
using System.Linq;

namespace Verloka.HelperLib.Localization
{
    public class Manager
    {
        public event Action<Manager> LanguageChanged;
        public event Action LoadError;

        public List<Language> AvailableLanguages { get; private set; }
        public Language Current { get; private set; }
        public string this[string key]
        {
            get { return ""; }
        }

        string path;

        public Manager(string folderPath)
        {
            path = folderPath;
        }

        public void Load(Language lang)
        {
            Current = AvailableLanguages.Single(l => l.Code == lang.Code);
            if (Current.Load())
                LanguageChanged?.Invoke(this);
            else
                LoadError?.Invoke();
        }
    }
}
