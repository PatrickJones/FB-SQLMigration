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
    
    public partial class BasalProgramTimeSlot
    {
        public int BasalSlotId { get; set; }
        public double BasalValue { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan StopTime { get; set; }
        public int PumpProgramId { get; set; }
        public System.DateTime DateSet { get; set; }
    
        public virtual PumpProgram PumpProgram { get; set; }
    }
}
