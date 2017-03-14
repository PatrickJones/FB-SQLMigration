using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class MappingStats
    {
        public string FBTablename { get; set; }
        public int FBRecordCount { get; set; }
        public string SQLTablename { get; set; }
        public int SQLRecordCount { get; set; }
        public int CompletedMappingsCount { get; set; }
        public int FailedMappingsCount { get; set; }

        public override string ToString()
        {
            //var json = JsonConvert.SerializeObject(this);
            //json = json.Replace('{', ' ');
            //json = json.Replace('}', ' ');

            //return json;

            return $"FBTablename={FBTablename} FBRecordCount={FBRecordCount} SQLTablename={SQLTablename} SQLRecordCount={SQLRecordCount} CompletedMappingsCount={CompletedMappingsCount} FailedMappingsCount={FailedMappingsCount}";
        }
    }
}
