using System.Runtime.Serialization;

namespace Verloka.HelperLib.Update
{
    [DataContract]
    public class Version
    {
        [DataMember]
        public int Major { get; set; }
        [DataMember]
        public int Minor { get; set; }
        [DataMember]
        public int Revision { get; set; }
        [DataMember]
        public int Build { get; set; }
        
        public Version()
        {
            Major = 1;
            Minor = 0;
            Revision = 0;
            Build = 0;
        }
        public Version(int Major, int Minor, int Revision, int Build)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Revision = Revision;
            this.Build = Build;
        }
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
            return $"{Major}.{Minor}.{Revision}.{Build}";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return (obj as Version).Major == Major && (obj as Version).Minor == Minor && 
                (obj as Version).Revision == Revision && (obj as Version).Build == Build;
        }
        public override int GetHashCode()
        {
            return Major ^ Minor ^ Revision ^ Build;
        }

        public static bool operator ==(Version a, Version b)
        {
            if (a == null || b == null)
                return false;

            return a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision && a.Build == b.Build;
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }

        public static bool operator >(Version a, Version b)
        {
            //Major
            if (a.Major > b.Major)
                return true;

            if (a.Major < b.Major)
                return false;

            //Minor
            if (a.Minor > b.Minor)
                return true;

            if (a.Minor < b.Minor)
                return false;

            //Build
            if (a.Build > b.Build)
                return true;

            if (a.Build < b.Build)
                return false;

            //Revision
            if (a.Revision > b.Revision)
                return true;

            if (a.Revision < b.Revision)
                return false;

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
            else if (a.Build >= b.Build)
                return true;
            else if (a.Revision >= b.Revision)
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
