//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuLibrary.Migration.Test.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class users_PasswordConfirmations
    {
        public long PasswordConfirmationId { get; set; }
        public System.Guid UserId { get; set; }
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> ConfirmedOn { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
