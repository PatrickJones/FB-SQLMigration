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
    
    public partial class ReadingEvent
    {
        public int Eventid { get; set; }
        public int EventType { get; set; }
        public string EventValue { get; set; }
        public System.DateTime EventTime { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime ResumeTime { get; set; }
        public System.DateTime StopTime { get; set; }
        public System.Guid ReadingKeyId { get; set; }
        public System.Guid UserId { get; set; }
    
        public virtual ReadingHeader ReadingHeader { get; set; }
    }
}
