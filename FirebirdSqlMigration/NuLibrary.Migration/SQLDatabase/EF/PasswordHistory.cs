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
    
    public partial class PasswordHistory
    {
        public string Password { get; set; }
        public System.Guid UserId { get; set; }
        public System.DateTime LastDateUsed { get; set; }
        public int AuthenticationId { get; set; }
        public int HistoryId { get; set; }
    
        public virtual UserAuthentication UserAuthentication { get; set; }
    }
}
