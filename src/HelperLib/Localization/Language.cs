﻿namespace Verloka.HelperLib.Localization
{
    public struct Language
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
