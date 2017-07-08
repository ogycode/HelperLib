using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Verloka.HelperLib.Update
{
    [DataContract]
    public class UpdateItem
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string ChangeNote { get; set; }
        [DataMember]
        public Version VersionNumber { get; set; }
        [DataMember]
        public List<string> Files { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        
        public UpdateItem()
        {
            Files = new List<string>();

            Title = "";
            ChangeNote = "";
            VersionNumber = new Version();
            Date = DateTime.Now;
        }
    }
}
