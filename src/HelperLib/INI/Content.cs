using System.Collections.Generic;

namespace Verloka.HelperLib.INI
{
    public class Content
    {
        public List<Section> Sections { get; private set; }
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

        public Content()
        {
            Sections = new List<Section>
            {
                new Section()
            };
        }

        public bool Write<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key) || Equals(value, default(T)))
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
        public bool RemoveNode(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            string[] part = key.Split('.');

            if (part.Length == 1)
                return Root.Remove(key);

            return GetSection(part[0]).Remove(part[1]);
        }
        public bool RemoveSection(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key == "Root")
                return false;

            var sec = Sections.Find(s => s.Name == key);

            if (sec == null)
                return false;

            Sections.Remove(sec);

            return true;
        }
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
        public bool ContainsKey(string key)
        {
            foreach (var item in Sections)
                if (item.GetContent().ContainsKey(key))
                    return true;
            return false;
        }
        public Section GetSection(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return Root;

            foreach (var item in Sections)
                if (item.Name == Name)
                    return item;

            return null;
        }
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
