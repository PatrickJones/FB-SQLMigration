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
    
    public partial class UserHistory
    {
        public int HistoryId { get; set; }
        public string Username { get; set; }
        public System.Guid SqlUserId { get; set; }
        public System.Guid LegacyUserId { get; set; }
        public int MigrationId { get; set; }
        public System.DateTime MigrationDate { get; set; }
    
        public virtual DatabaseHistory DatabaseHistory { get; set; }
    }
}
