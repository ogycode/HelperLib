using System.Collections.Generic;

namespace Verloka.HelperLib.Localization
{
    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
