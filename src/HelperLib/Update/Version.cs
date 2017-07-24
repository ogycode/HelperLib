/*
 * Version.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Class to store information about the update version number
    /// </summary>
    public class Version
    {
        public const char SEPARATOR = '.';

        int major;
        int minor;
        int build;
        int revision;

        /// <summary>
        /// Initializes a new instance of the Version class
        /// </summary>
        public Version()
        {
            SetMajor(0);
            SetMinor(0);
            SetRevision(0);
            SetBuild(0);
        }
        /// <summary>
        /// Initializes a new instance of the Version class
        /// </summary>
        /// <param name="Major">Major number</param>
        /// <param name="Minor">Minor number</param>
        /// <param name="Build">Build number</param>
        /// <param name="Revision">Revision number</param>
        public Version(int Major, int Minor, int Build, int Revision)
        {
            this.SetMajor(Major);
            this.SetMinor(Minor);
            this.SetBuild(Build);
            this.SetRevision(Revision);
        }
        /// <summary>
        /// Initializes a new instance of the Version class
        /// </summary>
        /// <param name="Major">Major number</param>
        /// <param name="Minor">Minor number</param>
        /// <param name="Build">Build number</param>
        /// <param name="Revision">Revision number</param>
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
        /// <summary>
        /// Initializes a new instance of the Version class
        /// </summary>
        /// <param name="copy">Copy of <see cref="Version"/></param>
        public Version(Version copy)
        {
            SetMajor(copy.GetMajor());
            SetMinor(copy.GetMinor());
            SetBuild(copy.GetBuild());
            SetRevision(copy.GetRevision());
        }

        /// <summary>
        /// Get Major number
        /// </summary>
        /// <returns>Major number</returns>
        public int GetMajor()
        {
            return major;
        }
        /// <summary>
        /// Set Major number
        /// </summary>
        /// <param name="value">Major number</param>
        public void SetMajor(int value)
        {
            major = value < 0 ? 0 : value;
        }
        /// <summary>
        /// Get Minor number
        /// </summary>
        /// <returns>Minor number</returns>
        public int GetMinor()
        {
            return minor;
        }
        /// <summary>
        /// Set Minor number
        /// </summary>
        /// <param name="value">Minor number</param>
        public void SetMinor(int value)
        {
            minor = value < 0 ? 0 : value;
        }
        /// <summary>
        /// Get Build number
        /// </summary>
        /// <returns>Build number</returns>
        public int GetBuild()
        {
            return build;
        }
        /// <summary>
        /// Set Build number
        /// </summary>
        /// <param name="value">Build number</param>
        public void SetBuild(int value)
        {
            build = value < 0 ? 0 : value;
        }
        /// <summary>
        /// Get Revision number
        /// </summary>
        /// <returns>Revision number</returns>
        public int GetRevision()
        {
            return revision;
        }
        /// <summary>
        /// Set Revision number
        /// </summary>
        /// <param name="value">Revision number</param>
        public void SetRevision(int value)
        {
            revision = value < 0 ? 0 : value;
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
                        //Major                 Minor                   Build                   Revision
            return (a?.GetMajor() == b?.GetMajor()) && (a?.GetMinor() == b?.GetMinor()) && (a?.GetBuild() == b?.GetBuild()) && (a?.GetRevision() == b?.GetRevision());
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }
        public static bool operator >(Version a, Version b)
        {
            //Major
            if (a?.GetMajor() > b?.GetMajor())
                return true;

            if (a?.GetMajor() < b?.GetMajor())
                return false;

            //Minor
            if (a?.GetMinor() > b?.GetMinor())
                return true;

            if (a?.GetMinor() < b?.GetMinor())
                return false;

            //Build
            if (a?.GetBuild() > b?.GetBuild())
                return true;

            if (a?.GetBuild() < b?.GetBuild())
                return false;

            //Revision
            if (a?.GetRevision() > b?.GetRevision())
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
