using System.Collections.Generic;

namespace Verloka.HelperLib.INI
{
    public class Section
    {
        public string Name { get; private set; }
        public bool IsRoot { get; private set; }
        Dictionary<string, object> content { get; set; }

        public Section()
        {
            IsRoot = true;
            Name = "";
            content = new Dictionary<string, object>();
        }
        public Section(string Name)
        {
            IsRoot = false;
            this.Name = Name;
            content = new Dictionary<string, object>();
        }

        public T Get<T>(string key)
        {
            if (content.ContainsKey(key))
                return (T)content[key];

            return default(T);
        }
        public bool Add(string key, object value)
        {
            if(content.ContainsKey(key))
            {
                content[key] = value;
                return false;
            }
            else
            {
                content.Add(key, value);
                return true;
            }
        }
        public bool Remove(string key)
        {
            if (content.ContainsKey(key))
            {
                content.Remove(key);
                return true;
            }
            else
                return false;
        }

        public Dictionary<string, object> GetContent()
        {
            if (IsRoot)
                return content;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var item in content)
                dic.Add($"{Name}.{item.Key}", item.Value);

            return dic;
        }
        public Dictionary<string, object> GetPureContent()
        {
            if (IsRoot)
                return content;

            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var item in content)
                dic.Add(item.Key, item.Value);

            return dic;
        }
    }
}
