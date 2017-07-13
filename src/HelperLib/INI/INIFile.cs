using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Verloka.HelperLib.INI
{
    public class INIFile
    {
        public event Action<string> IsNotFound;

        public Encoding Encoding { get; private set; }
        public string Separator { get; private set; }
        public string Comment { get; private set; }
        Content content;
        public string Path { get; private set; }
        public List<Section> Sections { get { return content?.Sections; } }
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

        public INIFile(string path)
        {
            Separator = "=";
            Comment = ";";
            Path = path;
            Encoding = Encoding.UTF8;
            LeftBracket = "[";
            RightBracket = "]";

            content = read(path, Separator, Comment, LeftBracket, RightBracket, Encoding);
        }
        public INIFile(string path, string separ, string comm)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            Encoding = Encoding.UTF8;
            LeftBracket = "[";
            RightBracket = "]";

            content = read(path, Separator, Comment, LeftBracket, RightBracket, Encoding);
        }
        public INIFile(string path, string separ, string comm, Encoding edc)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            Encoding = edc;
            LeftBracket = "[";
            RightBracket = "]";

            content = read(path, Separator, Comment, LeftBracket, RightBracket, Encoding);
        }
        public INIFile(string path, string separ, string comm, string lb, string rb, Encoding edc)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            Encoding = edc;
            LeftBracket = lb;
            RightBracket = rb;

            content = read(path, Separator, Comment, LeftBracket, RightBracket, Encoding);
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
            save(content, Path, Separator, Comment, LeftBracket, RightBracket, Encoding);
        }

        public static IDictionary<string, object> GetDictionary(string path)
        {
            return read(path, "=", "#", "[", "]", Encoding.UTF8).ToDictionary();
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm)
        {
            return read(path, separ, comm, "[", "]", Encoding.UTF8).ToDictionary();
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm, Encoding edc)
        {
            return read(path, separ, comm, "[", "]", edc).ToDictionary();
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm, string lb, string rb, Encoding edc)
        {
            return read(path, separ, comm, lb, rb, edc).ToDictionary();
        }

        static Content read(string path, string separator, string comment, string lb, string rb, Encoding edc)
        {
            Content con = new Content();
            Section sec = con.Root;

            using (StreamReader sr = new StreamReader(File.Open(path, FileMode.Open), edc))
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
        static void save(Content cont, string path, string separator, string comment, string lb, string rb, Encoding edc, bool repeat = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(path, FileMode.CreateNew)))
                {
                    //copyright
                    sw.WriteLine($"{comment}{new string('-', 50)}");
                    sw.WriteLine($"{comment}INI file created by Verloka.HelperLib.INI");
                    sw.WriteLine($"{comment}Verloka.HelperLib developed by Verloka Vadim");
                    sw.WriteLine($"{comment}Verloka Vadim - https://verloka.github.io");
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
            catch (IOException)
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (!repeat)
                    save(cont, path, separator, comment, lb, rb, edc, true);
            }
        }
    }
}
