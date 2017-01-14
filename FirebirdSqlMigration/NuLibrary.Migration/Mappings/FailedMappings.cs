using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class FailedMappings
    {
        public string Tablename { get; set; }
        public Type ObjectType { get; set; }
        public string JsonSerializedObject { get; set; }
        public string FailedReason { get; set; }
    }
}
