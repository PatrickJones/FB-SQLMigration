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
    
    public partial class EndUserLicenseAgreement
    {
        public int AgreementId { get; set; }
        public System.Guid UserId { get; set; }
        public System.DateTime AgreementDate { get; set; }
        public System.Guid ApplicationId { get; set; }
    
        public virtual Application Application { get; set; }
    }
}
