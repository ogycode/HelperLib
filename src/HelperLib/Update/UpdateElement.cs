/*
 * UpdateElement.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Class for store information about update
    /// </summary>
    public class UpdateElement
    {
        string title;
        string changeNote;
        Version versionNumber;
        string exe;
        string zip;
        private double date;
        private string guid;

        /// <summary>
        /// Initializes a new instance of the UpdateElement class
        /// </summary>
        public UpdateElement()
        {
            SetTitle("");
            SetChangeNote("");
            SetVersionNumber(new Version());
            SetEXE("");
            SetZIP("");
            SetDate(0);
            SetGUID(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Get title of update
        /// </summary>
        /// <returns>Title</returns>
        public string GetTitle()
        {
            return title;
        }
        /// <summary>
        /// Set title of update
        /// </summary>
        /// <param name="value">Title</param>
        public void SetTitle(string value)
        {
            title = value;
        }
        /// <summary>
        /// Get changenote of update
        /// </summary>
        /// <returns>Changenote</returns>
        public string GetChangeNote()
        {
            return changeNote;
        }
        /// <summary>
        /// Set changenote of update
        /// </summary>
        /// <param name="value">Changenote</param>
        public void SetChangeNote(string value)
        {
            changeNote = value;
        }
        /// <summary>
        /// Get version number of update
        /// </summary>
        /// <returns>Version number</returns>
        public Version GetVersionNumber()
        {
            return versionNumber;
        }
        /// <summary>
        /// Set version number of update
        /// </summary>
        /// <param name="value">Version number</param>
        public void SetVersionNumber(Version value)
        {
            versionNumber = value;
        }
        /// <summary>
        /// Get url to setup.exe
        /// </summary>
        /// <returns>Url</returns>
        public string GetEXE()
        {
            return exe;
        }
        /// <summary>
        /// Set url to setup.exe
        /// </summary>
        /// <param name="value">Url</param>
        public void SetEXE(string value)
        {
            exe = value;
        }
        /// <summary>
        /// Get url to zip archive with files of new version
        /// </summary>
        /// <returns>Url</returns>
        public string GetZIP()
        {
            return zip;
        }
        /// <summary>
        /// Set url to zip with archive with files of new version
        /// </summary>
        /// <param name="value">Url</param>
        public void SetZIP(string value)
        {
            zip = value;
        }
        /// <summary>
        /// Get date when update created
        /// </summary>
        /// <returns>Date</returns>
        public double GetDate()
        {
            return date;
        }
        /// <summary>
        /// Set date when update created
        /// </summary>
        /// <param name="value">Date</param>
        public void SetDate(double value)
        {
            date = value;
        }
        /// <summary>
        /// Get GUID of update
        /// </summary>
        /// <returns>GUID</returns>
        public string GetGUID()
        {
            return guid;
        }
        /// <summary>
        /// Set new GUID of update
        /// </summary>
        public void SetGUID()
        {
            SetGUID(Guid.NewGuid().ToString());
        }
        /// <summary>
        /// Set GUID of update
        /// </summary>
        /// <param name="value">GUID</param>
        public void SetGUID(string value)
        {
            guid = value;
        }

        public override string ToString()
        {
            return $"{GetVersionNumber()} / {DateTime.FromOADate(GetDate())}";
        }
    }
}
