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
    
    public partial class TensReading
    {
        public int ReadingId { get; set; }
        public System.DateTime ReadingDate { get; set; }
        public string StartTime { get; set; }
        public int TherapyType { get; set; }
        public int DurationScheduled { get; set; }
        public int DurationCompleted { get; set; }
        public int Aplitude { get; set; }
        public int PulseWidth { get; set; }
        public int Frequency { get; set; }
        public System.Guid ReadingKeyId { get; set; }
        public System.Guid UserId { get; set; }
    
        public virtual ReadingHeader ReadingHeader { get; set; }
    }
}
