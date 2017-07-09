using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Verloka.HelperLib.INI
{
    public class INIFile
    {
        public event Action<string> IsNotFound;

        public Encoding Encoding { get; private set; }
        public string Separator { get; private set; }
        public string Comment { get; private set; }
        Dictionary<string, object> content;
        public string Path { get; private set; }

        public INIFile(string path)
        {
            Separator = "=";
            Comment = ";";
            this.Path = path;
            Encoding = Encoding.UTF8;

            content = read(path, Separator, Comment, Encoding);
        }
        public INIFile(string path, string separ, string comm)
        {
            Separator = separ;
            Comment = comm;
            this.Path = path;
            Encoding = Encoding.UTF8;

            content = read(path, Separator, Comment, Encoding);
        }
        public INIFile(string path, string separ, string comm, Encoding edc)
        {
            Separator = separ;
            Comment = comm;
            this.Path = path;
            Encoding = edc;

            content = read(path, Separator, Comment, Encoding);
        }

        public T Read<T>(string key)
        {
            if (content.ContainsKey(key))
                return (T)content[key];

            IsNotFound?.Invoke($"{key} is not found!");

            return default(T);
        }
        public bool Write<T>(string key, T value)
        {
            if (content.ContainsKey(key))
            {
                content[key] = value;
                save(content, Path, Separator, Encoding);
                return false;
            }
            else
            {
                content.Add(key, value);
                save(content, Path, Separator, Encoding);
                return true;
            }
        }
        public bool Remove(string key)
        {
            if (content.ContainsKey(key))
            {
                content.Remove(key);
                save(content, Path, Separator, Encoding);
                return true;
            }
            return false;            
        }
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(content);
        }

        public static IDictionary<string, object> GetDictionary(string path)
        {
            return read(path, "=", "#", Encoding.UTF8);
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm)
        {
            return read(path, "=", "#", Encoding.UTF8);
        }
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm, Encoding edc)
        {
            return read(path, "=", "#", edc);
        }

        static Dictionary<string, object> read(string path, string separator, string comment, Encoding edc)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            using (StreamReader sr = new StreamReader(File.Open(path, FileMode.Open), edc))
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    //remove comment
                    int index = line.IndexOf(comment);
                    if (index > 0)
                        line = line.Substring(0, index);

                    //read node
                    string[] kv = line.Split(separator.ToCharArray());
                    if (kv.Length != 2)
                        continue;

                    if (dic.ContainsKey(kv[0]))
                        dic[kv[0]] = kv[1];
                    else
                        dic.Add(kv[0], kv[1]);
                }

            return dic;
        }
        static void save(Dictionary<string, object> cont, string path, string separator, Encoding edc, bool repeat = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(path, FileMode.CreateNew)))
                    foreach (var item in cont)
                        sw.WriteLine($"{item.Key}{separator}{item.Value}");
            }
            catch (IOException)
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (!repeat)
                    save(cont, path, separator, edc, true);
            }
        }
    }
}
