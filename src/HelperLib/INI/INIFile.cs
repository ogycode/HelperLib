/*
 * INIFile.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Verloka.HelperLib.INI
{
    /// <summary>
    /// Class for work with *.ini files
    /// </summary>
    public class INIFile
    {
        /// <summary>
        /// Occurs when file by path not found or key not found
        /// </summary>
        public event Action<string> IsNotFound;
        /// <summary>
        /// Occurs when class can not write to file
        /// </summary>
        public event Action<Exception> SaveException;
        
        /// <summary>
        /// Char of separator
        /// </summary>
        public string Separator { get; private set; }
        /// <summary>
        /// Char of comment
        /// </summary>
        public string Comment { get; private set; }
        /// <summary>
        /// Paht to *.ini file
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// All sections of file
        /// </summary>
        public List<Section> Sections => content?.Sections;
        /// <summary>
        /// Char of left bracket
        /// </summary>
        public string LeftBracket { get; private set; }
        /// <summary>
        /// Char of right bracket
        /// </summary>
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

        Content content;

        /// <summary>
        /// Initializes a new instance of the INIFile class
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="text">True - path is text of file</param>
        public INIFile(string path, bool text = false)
        {
            Separator = "=";
            Comment = ";";
            Path = path;
            LeftBracket = "[";
            RightBracket = "]";

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }
        /// <summary>
        /// Initializes a new instance of the INIFile class
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <param name="text">True - path is text of file</param>
        public INIFile(string path, string separ, string comm, bool text = false)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            LeftBracket = "[";
            RightBracket = "]";

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }
        /// <summary>
        /// Initializes a new instance of the INIFile class
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <param name="lb">Char of left bracket</param>
        /// <param name="rb">Char of right bracket</param>
        /// <param name="text">True - path is text of file</param>
        public INIFile(string path, string separ, string comm, string lb, string rb, bool text = false)
        {
            Separator = separ;
            Comment = comm;
            Path = path;
            LeftBracket = lb;
            RightBracket = rb;

            content = text ? ReadFromText(path, Separator, Comment, LeftBracket, RightBracket) : ReadFromPath(path, Separator, Comment, LeftBracket, RightBracket);
        }

        /// <summary>
        /// Read value by Key
        /// Key mast have the form - SectionName.KeyName
        /// If key has only KeyName then this key will be search in root section
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>If key exist - value else default value of type</returns>
        public T Read<T>(string key)
        {
            if (content.ContainsKeyFile(key))
                return content.Read<T>(key);

            IsNotFound?.Invoke($"Key \'{key}\' is not found!");

            return default(T);
        }
        /// <summary>
        /// Write value to file
        /// If key has only KeyName then value will be written to root
        /// </summary>
        /// <param name="key">Key of value</param>
        /// <param name="value">Value</param>
        /// <returns>True - value was added, False - value was edited</returns>
        public bool Write(string key, object value)
        {
            return content.Write(key, value);
        }
        /// <summary>
        /// Remove key from file
        /// If key has only KeyName then key will be removed from root
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (content.RemoveKey(key))
                return true;
            else
            {
                IsNotFound?.Invoke($"Key \'{key}\' is not found!");
                return false;
            }
        }
        /// <summary>
        /// Remove section by name
        /// </summary>
        /// <param name="name">Name of section</param>
        /// <returns>True - section was removed, False - section did not remove</returns>
        public bool RemoveSection(string name)
        {
            if (content.RemoveSection(name))
                return true;
            else
            {
                IsNotFound?.Invoke($"Section \'{name}\' is not found!");
                return false;
            }
        }
        /// <summary>
        /// Convert *.ini file to dictionary
        /// Key have the form like this - SectionName.KeyName
        /// Ex. SectionName.KeyName = Value
        /// </summary>
        /// <returns>Dictionary with content of section</returns>
        public IDictionary<string, object> ToDictionary()
        {
            return content.ToDictionary();
        }
        /// <summary>
        /// Save all changes to file
        /// </summary>
        public void Save()
        {
            SaveToFile(content, Path, Separator, Comment, LeftBracket, RightBracket);
        }

        /// <summary>
        /// Get converted *.ini file to dictionary
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Dictionary with content of section</returns>
        public static IDictionary<string, object> GetDictionary(string path)
        {
            return ReadFromPath(path, "=", "#", "[", "]").ToDictionary();
        }
        /// <summary>
        /// Get converted *.ini file to dictionary
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <returns>Dictionary with content of section</returns>
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm)
        {
            return ReadFromPath(path, separ, comm, "[", "]").ToDictionary();
        }
        /// <summary>
        /// Get converted *.ini file to dictionary
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <param name="lb">Char of left bracket</param>
        /// <param name="rb">Char of right bracket</param>
        /// <returns>Dictionary with content of section</returns>
        public static IDictionary<string, object> GetDictionary(string path, string separ, string comm, string lb, string rb)
        {
            return ReadFromPath(path, separ, comm, lb, rb).ToDictionary();
        }
        /// <summary>
        /// Get converted *.ini text to dictionary
        /// </summary>
        /// <param name="text">Text of file</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetDictionaryText(string text)
        {
            return ReadFromText(text, "=", "#", "[", "]").ToDictionary();
        }
        /// <summary>
        /// Get converted *.ini text to dictionary
        /// </summary>
        /// <param name="text">Text to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetDictionaryText(string text, string separ, string comm)
        {
            return ReadFromText(text, separ, comm, "[", "]").ToDictionary();
        }
        /// <summary>
        /// Get converted *.ini text to dictionary
        /// </summary>
        /// <param name="text">Text to file</param>
        /// <param name="separ">Char of separator</param>
        /// <param name="comm">Char of comment</param>
        /// <param name="lb">Char of left bracket</param>
        /// <param name="rb">Char of right bracket</param>
        /// <returns></returns>
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