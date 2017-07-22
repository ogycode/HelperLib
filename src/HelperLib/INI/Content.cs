/*
 * Content.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System.Collections.Generic;

namespace Verloka.HelperLib.INI
{
    /// <summary>
    /// Class for storing sections of *.ini file
    /// </summary>
    public class Content
    {
        /// <summary>
        /// List with all section of file and with root section
        /// </summary>
        public List<Section> Sections { get; private set; }
        /// <summary>
        /// Root section, section without name
        /// </summary>
        public Section Root
        {
            get
            {
                if (Sections.Count != 0)
                    return Sections[0];
                return null;
            }
        }
        public Section this[string name]
        {
            get
            {
                Section sec = GetSection(name);
                if (sec == null)
                {
                    sec = new Section(name);
                    Sections.Add(sec);
                    return sec;
                }
                else
                    return sec;
            }
            set
            {
                if (GetSection(name) == null)
                    Sections.Add(new Section(name));
                else
                    Sections[Sections.IndexOf(GetSection(name))] = new Section(name);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Content class
        /// Default: root section create auto
        /// </summary>
        public Content()
        {
            Sections = new List<Section>
            {
                new Section()
            };
        }

        /// <summary>
        /// Write to file parametr
        /// Key mast have the form - SectionName.KeyName, if not value will be writed in root
        /// </summary>
        /// <param name="key">Key of value</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public bool Write(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) || Equals(value, null))
                return false;

            string[] part = key.Split('.');

            if (part.Length == 1)
                return Root.Add(key, value);

            Section sec = GetSection(part[0]);

            if (sec != null)
                return sec.Add(part[1], value);

            sec = new Section(part[0]);
            Sections.Add(sec);

            return sec.Add(part[1], value);
        }
        /// <summary>
        /// Remove key from section
        /// Key mast have the form - SectionName.KeyName, if not key will be removed from root
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            string[] part = key.Split('.');

            if (part.Length == 1)
                return Root.Remove(key);

            return GetSection(part[0]).Remove(part[1]);
        }
        /// <summary>
        /// Remove sectoin from file
        /// Root section can not be removed
        /// </summary>
        /// <param name="name">Name of section</param>
        /// <returns>True - section was removed, False - section can not be removed</returns>
        public bool RemoveSection(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name == "Root")
                return false;

            var sec = Sections.Find(s => s.Name == name);

            if (sec == null)
                return false;

            Sections.Remove(sec);

            return true;
        }
        /// <summary>
        /// Read value by key from file
        /// Key mast have the form - SectionName.KeyName, if not value will be readed from root
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key of value</param>
        /// <returns>If value not found then returned default value of type</returns>
        public T Read<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);

            string[] part = key.Split('.');

            if (part.Length == 1)
            {
                if (Root.GetContent().ContainsKey(key))
                    return Root.Read<T>(part[0]);
                else
                    return default(T);
            }

            return GetSection(part[0]).Read<T>(part[1]);
        }
        /// <summary>
        /// Checking if key contains in file
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True - key contains in one or few sections, False - key not found in file</returns>
        public bool ContainsKeyFile(string key)
        {
            foreach (var item in Sections)
                if (item.GetContent().ContainsKey(key))
                    return true;
            return false;
        }
        /// <summary>
        /// Checking if key contains in concrete section
        /// </summary>
        /// <param name="name">Name of section</param>
        /// <param name="key">Key</param>
        /// <returns>True - the section contains key, False - key not found in section</returns>
        public bool ContainsKeySection(string name, string key)
        {
            if (GetSection(name).GetContent().ContainsKey(key))
                return true;
            return false;
        }
        /// <summary>
        /// Return section by name
        /// </summary>
        /// <param name="Name">Name of section what need find</param>
        /// <returns>Section - find, Null - do not find</returns>
        public Section GetSection(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return Root;

            foreach (var item in Sections)
                if (item.Name == Name)
                    return item;

            return null;
        }
        /// <summary>
        /// Convert file to dictionary when key have the form - SectionName.KeyName
        /// Root section have not name
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var sec in Sections)
            {
                string namePrefix = sec.IsRoot ? "" : $"{sec.Name}.";
                foreach (var item in sec.GetContent())
                    dic.Add(item.Key, item.Value);
            }

            return dic;
        }
    }
}
