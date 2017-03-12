using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public static class MappingStatistics
    {
        public static ICollection<SqlTableStats> SqlTableStatistics = new List<SqlTableStats>();
        public static void ExportToLog()
        {
            File.WriteAllText(@"C:\Users\pjones\Documents\FbMigrationLog.json", JsonConvert.SerializeObject(SqlTableStatistics, Formatting.Indented));
        }
    }
}
