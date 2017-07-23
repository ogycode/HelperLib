using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

namespace Verloka.HelperLib.Update
{
    public class Manager
    {
        public const string KEY_TITLE = "title";
        public const string KEY_CHANGENOTE = "change";
        public const string KEY_EXE = "exe";
        public const string KEY_ZIP = "zip";
        public const string KEY_DATE = "date";
        public const string KEY_VERSION = "version";
        public const string KEY_CHANENOTE_SEPARTOR = "-";

        public event Action<string> LoadError;

        public event Action<WebException> WebException;

        public List<UpdateElement> Elements { get; private set; }
        public UpdateElement Last { get => Elements.Aggregate((i, j) => i.GetVersionNumber() > j.GetVersionNumber() ? i : j); }
        public string Url { get; private set; }

        INI.INIFile file;
        bool isLocale = false;

        public Manager(string Url)
        {
            Elements = new List<UpdateElement>();
            this.Url = Url;
        }

        public async void LoadFromWeb()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string resp = await client.DownloadStringTaskAsync(Url);
                    Read(resp);
                }
                catch (WebException e)
                {
                    WebException?.Invoke(e);
                }
            }
        }
        public void LoadFromPath()
        {
            if (!File.Exists(Url))
            {
                LoadError?.Invoke($"File {Url} not found!");
                return;
            }

            isLocale = true;
            file = new INI.INIFile(Url, "=", ";");
            UpdateElements();
        }
        public bool IsAvailable(Version ver)
        {
            return CheckVersion(ver);
        }
        public bool IsAvailable(System.Version ver)
        {
            return CheckVersion(new Version(ver.Major, ver.Minor, ver.Build, ver.Revision));
        }
        public bool IsAvailable(int Major, int Minor, int Build, int Revision)
        {
            return CheckVersion(new Version(Major, Minor, Build, Revision));
        }
        public bool AddElement(UpdateElement elem)
        {
            if (!isLocale)
                return false;

            UpdateElement check = Elements.SingleOrDefault(e => e.GetVersionNumber() == elem.GetVersionNumber());
            if (check != null)
                return false;

            Add(elem);
            UpdateElements();
            return true;
        }
        public bool RemoveElement(UpdateElement element)
        {
            if (!isLocale)
                return false;

            UpdateElement check = Elements.SingleOrDefault(e => e.GetVersionNumber() == element.GetVersionNumber());
            if (check == null)
                return false;

            Remove(check.GetGUID());
            UpdateElements();
            return true;
        }
        public bool RemoveElement(Version version)
        {
            if (!isLocale)
                return false;

            UpdateElement check = Elements.SingleOrDefault(e => e.GetVersionNumber() == version);
            if (check == null)
                return false;

            Remove(check.GetGUID());
            UpdateElements();
            return true;
        }
        public bool RemoveElement(string guid)
        {
            if (!isLocale)
                return false;

            UpdateElement check = Elements.SingleOrDefault(e => e.GetGUID() == guid);
            if (check == null)
                return false;

            Remove(check.GetGUID());
            UpdateElements();
            return true;
        }
        public bool Save()
        {
            if (!isLocale)
                return false;

            file.Save();
            return true;
        }
        public void Close()
        {
            file = null;
            Url = null;

            GC.SuppressFinalize(this);
        }

        bool CheckVersion(Version ver)
        {
            return Last.GetVersionNumber() > ver;
        }
        void Read(string resp)
        {
            file = new INI.INIFile(resp, true);
            UpdateElements();
        }
        void UpdateElements()
        {
            Elements.Clear();

            if (file.Sections.Count < 2)
                return;

            foreach (var item in file.Sections)
                if (!item.IsRoot && !item.Name.EndsWith($"{KEY_CHANENOTE_SEPARTOR}{KEY_CHANGENOTE}"))
                {
                    //Other
                    UpdateElement element = new UpdateElement();
                    element.SetGUID(item.Name);
                    element.SetTitle(item[KEY_TITLE].ToString());
                    element.SetVersionNumber(item[KEY_VERSION].ToString());
                    element.SetEXE(item[KEY_EXE].ToString());
                    element.SetZIP(item[KEY_ZIP].ToString());
                    element.SetDate(item.Read<double>(KEY_DATE));

                    //Changenote
                    if (item.Read<int>(KEY_CHANGENOTE) != 0)
                    {
                        var dic = file[$"{element.GetGUID()}{KEY_CHANENOTE_SEPARTOR}{KEY_CHANGENOTE}"].GetPureContent();
                        string log = "";
                        foreach (var strings in dic)
                            log += $"{strings.Value}\n";
                        element.SetChangeNote(log);
                    }
                    else
                        element.SetChangeNote(string.Empty);

                    Elements.Add(element);
                }
        }
        void Add(UpdateElement elem)
        {
            //changenote
            string[] part = elem.GetChangeNote().Split('\n');
            for (int i = 0; i < part.Length; i++)
                file[$"{elem.GetGUID()}{KEY_CHANENOTE_SEPARTOR}{KEY_CHANGENOTE}"][$"line_{i}"] = part[i];
            file[elem.GetGUID()][KEY_CHANGENOTE] = part.Length;

            //other
            file[elem.GetGUID()][KEY_TITLE] = elem.GetTitle();
            file[elem.GetGUID()][KEY_VERSION] = elem.GetVersionNumber();
            file[elem.GetGUID()][KEY_EXE] = elem.GetEXE();
            file[elem.GetGUID()][KEY_ZIP] = elem.GetZIP();
            file[elem.GetGUID()][KEY_DATE] = elem.GetDate();
        }
        void Remove(string guid)
        {
            file.RemoveSection(guid);
        }
    }
}
