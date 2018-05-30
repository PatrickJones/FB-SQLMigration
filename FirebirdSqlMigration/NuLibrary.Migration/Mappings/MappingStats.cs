using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class MappingStats
    {
        public string FBTableName { get; set; }
        public int FBRecordCount { get; set; }
        public string SQLMappedTable { get; set; }
        public int CompletedMappingsCount { get; set; }
        public int FailedMappingsCount { get; set; }
    }
}
