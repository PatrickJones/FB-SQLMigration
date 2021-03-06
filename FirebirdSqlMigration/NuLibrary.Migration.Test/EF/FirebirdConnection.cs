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
    
    public partial class FirebirdConnection
    {
        public int ConnectionId { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DatabaseLocation { get; set; }
        public string DatasourceServer { get; set; }
        public int Port { get; set; }
        public Nullable<int> SiteId { get; set; }
        public string SiteName { get; set; }
        public string Location { get; set; }
        public Nullable<int> Dialect { get; set; }
        public string Charset { get; set; }
        public Nullable<bool> Pooling { get; set; }
        public Nullable<int> MinPoolSize { get; set; }
        public Nullable<int> MaxPoolSize { get; set; }
        public Nullable<int> PacketSize { get; set; }
        public Nullable<int> ServerType { get; set; }
        public string Role { get; set; }
        public Nullable<int> ConnectionLifeTime { get; set; }
    }
}
