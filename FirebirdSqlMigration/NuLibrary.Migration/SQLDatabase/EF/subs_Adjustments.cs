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
    
    public partial class subs_Adjustments
    {
        public long AdjustmentId { get; set; }
        public string Notes { get; set; }
        public string SubscriptionTimeType { get; set; }
        public int SubscriptionTimeAmount { get; set; }
        public System.Guid UserId { get; set; }
        public System.DateTime Date { get; set; }
        public bool SubscriptionTimeApplied { get; set; }
        public System.Guid AddedBy { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
