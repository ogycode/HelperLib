namespace Verloka.HelperLib.Update
{
    public class Version
    {
        public const char SEPARATOR = '.';
        private int major;
        private int minor;
        private int build;
        private int revision;

        public Version()
        {
            SetMajor(0);
            SetMinor(0);
            SetRevision(0);
            SetBuild(0);
        }
        public Version(int Major, int Minor, int Build, int Revision)
        {
            this.SetMajor(Major);
            this.SetMinor(Minor);
            this.SetBuild(Build);
            this.SetRevision(Revision);
        }
        public Version(string Major, string Minor, string Build, string Revision)
        {

            int.TryParse(Major, out int i);
            this.SetMajor(i);

            int.TryParse(Minor, out i);
            this.SetMinor(i);

            int.TryParse(Build, out i);
            this.SetBuild(i);

            int.TryParse(Revision, out i);
            this.SetRevision(i);
        }
        public Version(Version copy)
        {
            SetMajor(copy.GetMajor());
            SetMinor(copy.GetMinor());
            SetBuild(copy.GetBuild());
            SetRevision(copy.GetRevision());
        }

        public int GetMajor()
        {
            return major;
        }
        public void SetMajor(int value)
        {
            major = value;
        }
        public int GetMinor()
        {
            return minor;
        }
        public void SetMinor(int value)
        {
            minor = value;
        }
        public int GetBuild()
        {
            return build;
        }
        public void SetBuild(int value)
        {
            build = value;
        }
        public int GetRevision()
        {
            return revision;
        }
        public void SetRevision(int value)
        {
            revision = value;
        }

        public override string ToString()
        {
            return $"{GetMajor()}{SEPARATOR}{GetMinor()}{SEPARATOR}{GetBuild()}{SEPARATOR}{GetRevision()}";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return (obj as Version).GetMajor() == GetMajor() && (obj as Version).GetMinor() == GetMinor() && 
                (obj as Version).GetRevision() == GetRevision() && (obj as Version).GetBuild() == GetBuild();
        }
        public override int GetHashCode()
        {
            return GetMajor() ^ GetMinor() ^ GetBuild() ^ GetRevision();
        }

        public static bool operator ==(Version a, Version b)
        {
            if (a == null || b == null)
                return false;
                        //Major                 Minor                   Build                   Revision
            return (a.GetMajor() == b.GetMajor()) && (a.GetMinor() == b.GetMinor()) && (a.GetBuild() == b.GetBuild()) && (a.GetRevision() == b.GetRevision());
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }
        public static bool operator >(Version a, Version b)
        {
            //Major
            if (a.GetMajor() > b.GetMajor())
                return true;

            if (a.GetMajor() < b.GetMajor())
                return false;

            //Minor
            if (a.GetMinor() > b.GetMinor())
                return true;

            if (a.GetMinor() < b.GetMinor())
                return false;

            //Build
            if (a.GetBuild() > b.GetBuild())
                return true;

            if (a.GetBuild() < b.GetBuild())
                return false;

            //Revision
            if (a.GetRevision() > b.GetRevision())
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
            if (a.GetMajor() >= b.GetMajor())
                return true;
            else if (a.GetMinor() >= b.GetMinor())
                return true;
            else if (a.GetBuild() >= b.GetBuild())
                return true;
            else if (a.GetRevision() >= b.GetRevision())
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
