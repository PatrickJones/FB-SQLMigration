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
    
    public partial class PatientHistory
    {
        public int PatientHistoryId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string FirebirdPatientId { get; set; }
        public System.Guid SqlUserId { get; set; }
        public System.DateTime MigrationDate { get; set; }
        public int MigrationId { get; set; }
    
        public virtual DatabaseHistory DatabaseHistory { get; set; }
    }
}
