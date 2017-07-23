/*
 * Language.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

namespace Verloka.HelperLib.Localization
{
    /// <summary>
    /// Class to store information about the language
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Locale code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Name of language
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new default instance of the Language class
        /// </summary>
        public Language()
        {
            Code = "";
            Name = "";
        }
        /// <summary>
        /// Initializes a new instance of the Language class
        /// </summary>
        /// <param name="Code">Locale code</param>
        /// <param name="Name">Name of language</param>
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
