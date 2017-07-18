namespace Verloka.HelperLib.Update
{
    public class Version
    {
        public const char SEPARATOR = '.';
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }
        public int Revision { get; private set; }
        
        public Version()
        {
            Major = 0;
            Minor = 0;
            Revision = 0;
            Build = 0;
        }
        public Version(int Major, int Minor, int Build, int Revision)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Build = Build;
            this.Revision = Revision;
        }
        public Version(string Major, string Minor, string Build, string Revision)
        {

            int.TryParse(Major, out int i);
            this.Major = i;

            int.TryParse(Minor, out i);
            this.Minor = i;

            int.TryParse(Build, out i);
            this.Build = i;

            int.TryParse(Revision, out i);
            this.Revision = i;
        }
        public Version(Version copy)
        {
            SetMajor(copy.Major);
            SetMinor(copy.Minor);
            SetBuild(copy.Build);
            SetRevision(copy.Revision);
        }
        
        public void SetMajor(int Major)
        {
            this.Major = Major;
        }
        public void SetMinor(int Minor)
        {
            this.Minor = Minor;
        }
        public void SetBuild(int Build)
        {
            this.Build = Build;
        }
        public void SetRevision(int Revision)
        {
            this.Revision = Revision;
        }

        public override string ToString()
        {
            return $"{Major}{SEPARATOR}{Minor}{SEPARATOR}{Build}{SEPARATOR}{Revision}";
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
            return Major ^ Minor ^ Build ^ Revision;
        }

        public static bool operator ==(Version a, Version b)
        {
            if (a == null || b == null)
                return false;
                        //Major                 Minor                   Build                   Revision
            return (a.Major == b.Major) && (a.Minor == b.Minor) && (a.Build == b.Build) && (a.Revision == b.Revision);
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

        public static implicit operator Version(string str)
        {
            string[] part = str.Split(SEPARATOR);
            if (part.Length < 4)
                return new Version();

            return new Version(part[0], part[1], part[2], part[3]);
        }
    }
}
