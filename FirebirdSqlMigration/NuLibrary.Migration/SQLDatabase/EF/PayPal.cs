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
    
    public partial class PayPal
    {
        public int PayPalId { get; set; }
        public string txn_id { get; set; }
        public string parent_txn_id { get; set; }
        public System.DateTime payment_date { get; set; }
        public string payment_status { get; set; }
        public string pending_reason { get; set; }
        public string reason_code { get; set; }
        public decimal mc_gross { get; set; }
        public decimal mc_fee { get; set; }
        public string PayPal_post_vars { get; set; }
        public string SourceIP { get; set; }
        public int PaymentId { get; set; }
    
        public virtual Payment Payment { get; set; }
    }
}
