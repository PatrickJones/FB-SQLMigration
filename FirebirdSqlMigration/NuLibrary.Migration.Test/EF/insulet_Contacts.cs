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
    
    public partial class insulet_Contacts
    {
        public System.Guid ContactId { get; set; }
        public System.Guid Parent_Site { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsITManager { get; set; }
        public string Title { get; set; }
        public byte Order { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
