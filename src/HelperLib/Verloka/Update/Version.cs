using System.Runtime.Serialization;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Verison information
    /// </summary>
    [DataContract]
    public class Version
    {
        /// <summary>
        /// Major number
        /// </summary>
        [DataMember]
        public int Major { get; set; }
        /// <summary>
        /// Minor number
        /// </summary>
        [DataMember]
        public int Minor { get; set; }
        /// <summary>
        /// Revision number
        /// </summary>
        [DataMember]
        public int Revision { get; set; }
        /// <summary>
        /// Build number
        /// </summary>
        [DataMember]
        public int Build { get; set; }

        /// <summary>
        /// Initializes an object of Version
        /// </summary>
        public Version()
        {
            Major = 1;
            Minor = 0;
            Revision = 0;
            Build = 0;
        }
        /// <summary>
        /// Initializes an object of Version
        /// </summary>
        /// <param name="Major">Major number</param>
        /// <param name="Minor">Minor number</param>
        /// <param name="Revision">Revision number</param>
        /// <param name="Build">Build number</param>
        public Version(int Major, int Minor, int Revision, int Build)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Revision = Revision;
            this.Build = Build;
        }
        /// <summary>
        /// Initializes an object of Version
        /// </summary>
        /// <param name="Major">Major number</param>
        /// <param name="Minor">Minor number</param>
        /// <param name="Revision">Revision number</param>
        /// <param name="Build">Build number</param>
        public Version(string Major, string Minor, string Revision, string Build)
        {
            int i = 0;
            int.TryParse(Major, out i);
            this.Major = i;

            int.TryParse(Minor, out i);
            this.Minor = i;

            int.TryParse(Revision, out i);
            this.Revision = i;

            int.TryParse(Build, out i);
            this.Build = i;
        }

        public override string ToString()
        {
            return $"{Major},{Minor},{Revision},{Build}";
        }
        public override bool Equals(object obj)
        {
            return (obj as Version).Major == Major && (obj as Version).Minor == Minor && 
                (obj as Version).Revision == Revision && (obj as Version).Build == Build;
        }
        public override int GetHashCode()
        {
            return Major ^ Minor ^ Revision ^ Build;
        }

        public static bool operator ==(Version a, Version b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision && a.Build == b.Build;
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }

        public static bool operator >(Version a, Version b)
        {
            if (a.Major > b.Major)
                return true;
            else if (a.Minor > b.Minor)
                return true;
            else if (a.Revision > b.Revision)
                return true;
            else if (a.Build > b.Build)
                return true;
            else
                return false;
        }
        public static bool operator <(Version a, Version b)
        {
            return !(a > b);
        }

        public static bool operator >=(Version a, Version b)
        {
            if (a.Major >= b.Major)
                return true;
            else if (a.Minor >= b.Minor)
                return true;
            else if (a.Revision >= b.Revision)
                return true;
            else if (a.Build >= b.Build)
                return true;
            else
                return false;
        }
        public static bool operator <=(Version a, Version b)
        {
            return !(a >= b);
        }
    }
}
