/*
 * Section.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;
using System.Collections.Generic;

namespace Verloka.HelperLib.INI
{
    /// <summary>
    /// Class for store section's info
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Name of section
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// If true then this section have not name
        /// </summary>
        public bool IsRoot { get; private set; }
        public object this [string key]
        {
            get => Read<object>(key);
            set => Add(key, value);
        }

        Dictionary<string, object> Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the Section class, default section - root
        /// </summary>
        public Section()
        {
            IsRoot = true;
            Name = "Root";
            Content = new Dictionary<string, object>();
        }
        /// <summary>
        /// Initializes a new instance of the Section class with concrete name
        /// </summary>
        /// <param name="Name"></param>
        public Section(string Name)
        {
            IsRoot = false;
            this.Name = Name;
            Content = new Dictionary<string, object>();
        }

        /// <summary>
        /// Read data from section's content
        /// </summary>
        /// <typeparam name="T">Type of value (ex. int, double etc.)</typeparam>
        /// <param name="key">Key of parametr</param>
        /// <returns>Value by key</returns>
        public T Read<T>(string key)
        {
            if (Content.ContainsKey(key))
                if (Content[key] is T)
                    return (T)Content[key];
                else
                    try { return (T)Convert.ChangeType(Content[key], typeof(T)); }
                    catch { }


            return default(T);
        }
        /// <summary>
        /// Add new value or edit if exist the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>True - value was added, False - value was edited</returns>
        public bool Add(string key, object value)
        {
            if(Content.ContainsKey(key))
            {
                Content[key] = value;
                return false;
            }
            else
            {
                Content.Add(key, value);
                return true;
            }
        }
        /// <summary>
        /// Remove value from section by key
        /// </summary>
        /// <param name="key">Key of value</param>
        /// <returns>True - value was removed, False - the key do not exist</returns>
        public bool Remove(string key)
        {
            if (Content.ContainsKey(key))
            {
                Content.Remove(key);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Setup section name
        /// </summary>
        /// <param name="name">Name of section</param>
        public void SetName(string name)
        {
            IsRoot = name == "Root" || string.IsNullOrWhiteSpace(name) ? true : false;

            Name = string.IsNullOrWhiteSpace(name) ? "Root" : name;
        }

        /// <summary>
        /// Keys what contains in section
        /// </summary>
        /// <returns>List with keys</returns>
        public IList<string> GetKeys()
        {
            List<string> keys = new List<string>();

            foreach (var item in Content.Keys)
                keys.Add(item);

            return keys;
        }
        /// <summary>
        /// Content of section in the form of dictionary.
        /// Key have the form like this - SectionName.KeyName
        /// Ex. SectionName.KeyName = Value
        /// </summary>
        /// <returns>Dictionary with content of section</returns>
        public IDictionary<string, object> GetContent()
        {
            if (IsRoot)
                return Content;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var item in Content)
                dic.Add($"{Name}.{item.Key}", item.Value);

            return dic;
        }
        /// <summary>
        /// Content of section in the form of dictionary.
        /// Key have the form like this = KeyName
        /// Ex. KeyName = Value
        /// </summary>
        /// <returns>Dictionary with content of section</returns>
        public IDictionary<string, object> GetPureContent()
        {
            if (IsRoot)
                return Content;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var item in Content)
                dic.Add(item.Key, item.Value);

            return dic;
        }
    }
}
