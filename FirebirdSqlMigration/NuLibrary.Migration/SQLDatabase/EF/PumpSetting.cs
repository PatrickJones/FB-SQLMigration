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
    
    public partial class PumpSetting
    {
        public int SettingId { get; set; }
        public int SettingType { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingValueType { get; set; }
        public System.Guid PumpKeyId { get; set; }
    
        public virtual Pump Pump { get; set; }
    }
}
