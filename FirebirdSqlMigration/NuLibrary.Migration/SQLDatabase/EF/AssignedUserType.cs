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
    
    public partial class AssignedUserType
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public System.Guid UserId { get; set; }
    
        public virtual User User { get; set; }
    }
}