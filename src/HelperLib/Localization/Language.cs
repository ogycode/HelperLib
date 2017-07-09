using System.Collections.Generic;

namespace Verloka.HelperLib.Localization
{
    public class Language
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public Dictionary<string, string> Content { get; private set; }

        public Language(string path)
        {
            //open and read name and code
            //from name file. example:
            //Русский_ru-ru.ini
            //name    code
        }

        public bool Load()
        {
            //INI file reading
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
