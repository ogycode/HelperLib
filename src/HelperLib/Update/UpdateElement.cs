﻿using System;
using System.Collections.Generic;

namespace Verloka.HelperLib.Update
{
    public class UpdateElement
    {
        public string Title { get; private set; }
        public string ChangeNote { get; private set; }
        public Version VersionNumber { get; private set; }
        public List<UpdateFile> Files { get; private set; }
        public DateTime Date { get; private set; }
        public string GUID { get; private set; }

        public UpdateElement()
        {
            Files = new List<UpdateFile>();

            Title = "";
            ChangeNote = "";
            VersionNumber = new Version();
            Date = DateTime.Now;
            GUID = Guid.NewGuid().ToString();
        }
        public UpdateElement(string Title, string ChangeNote, string GUID, Version VersionNumber, DateTime Date)
        {
            Files = new List<UpdateFile>();

            this.Title = Title;
            this.ChangeNote = ChangeNote;
            this.GUID = GUID;
            this.VersionNumber = VersionNumber;
            this.Date = Date;
        }

        public void Add(UpdateFile f)
        {
            Files.Add(f);
        }
    }
}