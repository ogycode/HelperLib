using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Update information including: Version, Title, Changelog and Date when update was created
    /// </summary>
    [DataContract]
    public class UpdateItem
    {
        /// <summary>
        /// Update title
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// Update changelog
        /// </summary>
        [DataMember]
        public string ChangeNote { get; set; }
        /// <summary>
        /// Update version
        /// </summary>
        [DataMember]
        public Version VersionNumber { get; set; }
        /// <summary>
        /// Update files
        /// </summary>
        [DataMember]
        public List<string> Files { get; set; }
        /// <summary>
        /// Update date
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Initializes an object of UpdateItem
        /// </summary>
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
