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
    
    public partial class stats_PageRequests
    {
        public long PageReqId { get; set; }
        public System.Guid UserId { get; set; }
        public System.DateTime DateTime { get; set; }
        public string URL { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}