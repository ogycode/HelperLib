/*
 * Manager.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Class fo work with update of application
    /// </summary>
    public class Manager
    {
        public const string KEY_TITLE = "title";
        public const string KEY_CHANGENOTE = "change";
        public const string KEY_EXE = "exe";
        public const string KEY_ZIP = "zip";
        public const string KEY_DATE = "date";
        public const string KEY_VERSION = "version";
        public const string KEY_CHANENOTE_SEPARTOR = "-";

        /// <summary>
        /// Occurs when file can not load
        /// </summary>
        public event Action<string> LoadError;
        /// <summary>
        /// Occurs when any exception by WebClient
        /// </summary>
        public event Action<WebException> WebException;
        /// <summary>
        /// Occurs when data about update information loaded
        /// </summary>
        public event Action<bool> DataLoaded;

        /// <summary>
        /// <see cref="List{UpdateElement}"/> with all versions of app
        /// </summary>
        public List<UpdateElement> Elements { get; private set; }
        /// <summary>
        /// Newest update
        /// </summary>
        public UpdateElement Last { get => Elements.Aggregate((i, j) => i.GetVersionNumber() > j.GetVersionNumber() ? i : j); }
        /// <summary>
        /// Url to *.ini file with update info
        /// </summary>
        public string Url { get; private set; }

        INI.INIFile file;
        bool isLocale = false;

        /// <summary>
        /// Initializes a new instance of the Manager class
        /// </summary>
        /// <param name="Url">Url to *.ini file with update info</param>
        public Manager(string Url)
        {
            Elements = new List<UpdateElement>();
            this.Url = Url;
        }

        /// <summary>
        /// Load information about versions from web
        /// </summary>
        public async Task LoadFromWeb()
        {
            bool result = false;
            await Task.Run(async () =>
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Revalidate);
                        string resp = await client.DownloadStringTaskAsync(Url);
                        Read(resp);
                        result = true;
                    }
                    catch (WebException e)
                    {
                        WebException?.Invoke(e);
                    }
                }
            });
            DataLoaded?.Invoke(result);
        }
        /// <summary>
        /// Load information about versions from path, Edit mode is enabled
        /// </summary>
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
        /// <summary>
        /// Compares your version with the newest
        /// </summary>
        /// <param name="ver">Installed app version</param>
        /// <returns>True - new version is available</returns>
        public bool IsAvailable(Version ver)
        {
            return CheckVersion(ver);
        }
        /// <summary>
        /// Compares your version with the newest
        /// </summary>
        /// <param name="ver">Installed app version</param>
        /// <returns>True - new version is available</returns>
        public bool IsAvailable(System.Version ver)
        {
            return CheckVersion(new Version(ver.Major, ver.Minor, ver.Build, ver.Revision));
        }
        /// <summary>
        /// Compares your version with the newest
        /// </summary>
        /// <param name="Major">Major number of installed app version</param>
        /// <param name="Minor">Minor number of installed app version</param>
        /// <param name="Build">Build number of installed app version</param>
        /// <param name="Revision">Revision number of installed app version</param>
        /// <returns>True - new version is available</returns>
        public bool IsAvailable(int Major, int Minor, int Build, int Revision)
        {
            return CheckVersion(new Version(Major, Minor, Build, Revision));
        }
        /// <summary>
        /// Add new update to *.ini file
        /// </summary>
        /// <param name="elem"></param>
        /// <returns>True - add</returns>
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
        /// <summary>
        /// Remove update from *.ini file
        /// </summary>
        /// <param name="element">Concrete update for removing</param>
        /// <returns>True - remove</returns>
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
        /// <summary>
        /// Remove update from *.ini file
        /// </summary>
        /// <param name="version">Version of update what need remove</param>
        /// <returns>True - remove</returns>
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
        /// <summary>
        /// Remove update from *.ini file
        /// </summary>
        /// <param name="guid">GUID of update what need remove</param>
        /// <returns>True - remove</returns>
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
        /// <summary>
        /// Save changed to *.ini file
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (!isLocale)
                return false;

            file.Save();
            return true;
        }
        /// <summary>
        /// Close object of <see cref="Manager"/>
        /// </summary>
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
