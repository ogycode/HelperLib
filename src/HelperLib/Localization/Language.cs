namespace Verloka.HelperLib.Localization
{
    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Language()
        {
            Code = "";
            Name = "";
        }
        public Language(string Code, string Name)
        {
            this.Code = Code;
            this.Name = Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
