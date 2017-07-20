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
        private double date;
        private string guid;
        
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
        public double GetDate()
        {
            return date;
        }
        public void SetDate(double value)
        {
            date = value;
        }
        public string GetGUID()
        {
            return guid;
        }
        public void SetGUID()
        {
            SetGUID(Guid.NewGuid().ToString());
        }
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
