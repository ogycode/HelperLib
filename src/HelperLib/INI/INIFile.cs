using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Verloka.HelperLib.INI
{
    public class INIFile
    {
        public event Action<string> IsNotFound;
        public event Action<Exception> SaveException;
        
        public string Separator { get; private set; }
        public string Comment { get; private set; }
        Content content;
        public string Path { get; private set; }
        public List<Section> Sections => content?.Sections;
        public string LeftBracket { get; private set; }
        public string RightBracket { get; private set; }
        public Section this[string name]
        {
            get
            {
                return content[name];
            }
            set
            {
                content[name] = value;
            }
        }

        public INIFile(string path, bool text = false)
        {
            Separator = "=";
            Comment = ";";
            Path = path;
            LeftBracket = "[";
            RightBracket = "]";

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }
        public INIFile(string path, string separ, string comm, bool text = false)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            LeftBracket = "[";
            RightBracket = "]";

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }
        public INIFile(string path, string separ, string comm, string lb, string rb, bool text = false)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            LeftBracket = lb;
            RightBracket = rb;

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }

        public T Read<T>(string key)
        {
            if (content.ContainsKey(key))
                return content.Read<T>(key);

            IsNotFound?.Invoke($"{key} is not found!");

            return default(T);
        }
        public bool Write<T>(string key, T value)
        {
            return content.Write(key, value);
        }
        public bool Remove(string key)
        {
            return content.RemoveNode(key);
        }
        public bool RemoveSection(string name)
        {
            return content.RemoveSection(name);
        }
        public IDictionary<string, object> ToDictionary()
        {
            return content.ToDictionary();
        }
        public void Save()
        {
            SaveToFile(content, Path, Separator, Comment, LeftBracket, RightBracket);
        }

        public static IDictionary<string, object> GetDictionary(string path)
        {
            return ReadFromPath(path, "=", "#", "[", "]").ToDictionary();
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm)
        {
            return ReadFromPath(path, separ, comm, "[", "]").ToDictionary();
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm, string lb, string rb)
        {
            return ReadFromPath(path, separ, comm, lb, rb).ToDictionary();
        }
        public static IDictionary<string, object> GetDictionaryText(string text)
        {
            return ReadFromText(text, "=", "#", "[", "]").ToDictionary();
        }
        public static IDictionary<string, object> GetDictionaryText(string text, string separ, string comm)
        {
            return ReadFromText(text, separ, comm, "[", "]").ToDictionary();
        }
        public static IDictionary<string, object> GetDictionaryText(string text, string separ, string comm, string lb, string rb)
        {
            return ReadFromText(text, separ, comm, lb, rb).ToDictionary();
        }

        static Content ReadFromPath(string path, string separator, string comment, string lb, string rb)
        {
            Content con = new Content();
            Section sec = con.Root;

            using (StreamReader sr = File.OpenText(path))
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    //remove comment
                    int index = line.IndexOf(comment);
                    if (index > 0)
                        line = line.Substring(0, index);

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] part = line.Split(separator.ToCharArray());

                    string section = Regex.Match(line, $@"\{lb}([^)]*)\{rb}").Groups[1].Value;

                    if (string.IsNullOrWhiteSpace(section))
                    {
                        if (part.Length != 2)
                            continue;

                        sec[part[0]] = part[1];
                    }
                    else
                        sec = con[section];
                }
            return con;
        }
        static Content ReadFromText(string text, string separator, string comment, string lb, string rb)
        {
            Content con = new Content();
            Section sec = con.Root;

            string[] str = text.Split('\n');
            int i = 0;

            while (i <= str.Length - 1)
            {
                string line = str[i];

                //remove comment
                int index = line.IndexOf(comment);
                if (index > 0)
                    line = line.Substring(0, index);

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] part = line.Split(separator.ToCharArray());

                string section = Regex.Match(line, $@"\{lb}([^)]*)\{rb}").Groups[1].Value;

                if (string.IsNullOrWhiteSpace(section))
                {
                    if (part.Length != 2)
                        continue;

                    sec[part[0]] = part[1];
                }
                else
                    sec = con[section];

                i++;
            }

            return con;
        }
        void SaveToFile(Content cont, string path, string separator, string comment, string lb, string rb, bool repeat = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    //copyright
                    sw.WriteLine($"{comment}{new string('-', 50)}");
                    sw.WriteLine($"{comment}INI file created by Verloka.HelperLib.INI");
                    sw.WriteLine($"{comment}Verloka.HelperLib developed by Verloka Vadim");
                    sw.WriteLine($"{comment}Verloka Vadim - https://verloka.github.io");
                    sw.WriteLine($"{comment}");
                    sw.WriteLine($"{comment}Last modified - {DateTime.Now.ToString()}");
                    sw.WriteLine($"{comment}{new string('-', 50)}\n\n");

                    foreach (var root in cont.Root.GetPureContent())
                        sw.WriteLine($"{root.Key}{separator}{root.Value}");
                    sw.WriteLine("");

                    foreach (var item in cont.Sections)
                    {
                        if (!item.IsRoot)
                        {
                            sw.WriteLine($"{lb}{item.Name}{rb}");
                            foreach (var sec in item.GetPureContent())
                                sw.WriteLine($"{sec.Key}{separator}{sec.Value}");
                            sw.WriteLine("");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!repeat)
                    SaveToFile(cont, path, separator, comment, lb, rb, true);
                else
                    SaveException?.Invoke(e);
            }
        }
    }
}
