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
    
    public partial class ReadingError
    {
        public int ErrorId { get; set; }
        public System.DateTime Time { get; set; }
        public string ErrorName { get; set; }
        public string ErrorText { get; set; }
        public bool IsActive { get; set; }
        public System.Guid ReadingKeyId { get; set; }
        public System.Guid UserId { get; set; }
    
        public virtual ReadingHeader ReadingHeader { get; set; }
    }
}
