//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuLibrary.Migration.SQLDatabase.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class BloodGlucoseReading
    {
        public int ReadingId { get; set; }
        public int DatetimeKey { get; set; }
        public System.DateTime ReadingDateTime { get; set; }
        public string Units { get; set; }
        public string Value { get; set; }
        public long DownloadKedyId { get; set; }
        public System.Guid UserId { get; set; }
        public bool Active { get; set; }
    
        public virtual ReadingHeader ReadingHeader { get; set; }
    }
}