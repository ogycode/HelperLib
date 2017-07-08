using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verloka.HelperLib.INI
{
    public class INIFile
    {
        public string Separator { get; private set; }
        public string Comment { get; private set; }
        Dictionary<string, string> content;

        public INIFile(string path)
        {
            Separator = "=";
            Comment = ";";

            content = read(path, Separator, Comment);
        }
        public INIFile(string path, string separ, string comm)
        {
            Separator = separ;
            Comment = comm;

            content = read(path, Separator, Comment);
        }

        public void Write(string key, string value)
        {

        }
        public void Remove(string key)
        {

        }
        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>(content);
        }

        public static IDictionary<string, string> GetDictionary(string path)
        {
            return read(path, "=", "#");
        }
        public static IDictionary<string, string> GetDictionary(string path, string separ, string comm)
        {
            return read(path, separ, comm);
        }

        static Dictionary<string, string> read(string path, string separator, string comment)
        {
            return null;
        }
    }
}
