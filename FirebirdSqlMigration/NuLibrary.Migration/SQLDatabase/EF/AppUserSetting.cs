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
    
    public partial class AppUserSetting
    {
        public int AppUserSettingId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public System.Guid AppicationId { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid LastUpdatedByUser { get; set; }
    
        public virtual Application Application { get; set; }
    }
}
