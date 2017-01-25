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
    
    public partial class PumpProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PumpProgram()
        {
            this.BasalProgramTimeSlots = new HashSet<BasalProgramTimeSlot>();
            this.BolusProgramTimeSlots = new HashSet<BolusProgramTimeSlot>();
        }
    
        public int PumpProgramId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Source { get; set; }
        public string ProgramName { get; set; }
        public int ProgramKey { get; set; }
        public bool Valid { get; set; }
        public int NumOfSegments { get; set; }
        public System.Guid PumpKeyId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BasalProgramTimeSlot> BasalProgramTimeSlots { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BolusProgramTimeSlot> BolusProgramTimeSlots { get; set; }
        public virtual Pump Pump { get; set; }
    }
}
