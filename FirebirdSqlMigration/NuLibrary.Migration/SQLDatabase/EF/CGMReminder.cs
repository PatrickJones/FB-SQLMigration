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
    
    public partial class CGMReminder
    {
        public int ReminderId { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public string Time { get; set; }
        public System.Guid ReadingKeyId { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual ReadingHeader ReadingHeader { get; set; }
    }
}
