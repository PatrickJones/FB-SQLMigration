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
    
    public partial class help_Modules
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public help_Modules()
        {
            this.help_ModuleOrder = new HashSet<help_ModuleOrder>();
        }
    
        public long ModuleId { get; set; }
        public string SiteVersion { get; set; }
        public string Page { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<help_ModuleOrder> help_ModuleOrder { get; set; }
    }
}
