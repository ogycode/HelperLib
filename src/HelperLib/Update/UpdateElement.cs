using System;

namespace Verloka.HelperLib.Update
{
    public class UpdateElement
    {
        string title;
        string changeNote;
        Version versionNumber;
        string exe;
        string zip;
        private DateTime date;
        private string gUID;
        
        public UpdateElement()
        {
            SetTitle("");
            SetChangeNote("");
            SetVersionNumber(new Version());
            SetEXE("");
            SetZIP("");
            SetDate(DateTime.Now);
            SetGUID(Guid.NewGuid().ToString());
        }

        public string GetTitle()
        {
            return title;
        }
        public void SetTitle(string value)
        {
            title = value;
        }
        public string GetChangeNote()
        {
            return changeNote;
        }
        public void SetChangeNote(string value)
        {
            changeNote = value;
        }
        public Version GetVersionNumber()
        {
            return versionNumber;
        }
        public void SetVersionNumber(Version value)
        {
            versionNumber = value;
        }
        public string GetEXE()
        {
            return exe;
        }
        public void SetEXE(string value)
        {
            exe = value;
        }
        public string GetZIP()
        {
            return zip;
        }
        public void SetZIP(string value)
        {
            zip = value;
        }
        public DateTime GetDate()
        {
            return date;
        }
        public void SetDate(DateTime value)
        {
            date = value;
        }
        public string GetGUID()
        {
            return gUID;
        }
        public void SetGUID(string value)
        {
            gUID = value;
        }
    }
}
