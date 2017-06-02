using Newtonsoft.Json;
using NuLibrary.Migration.GlobalVar;
using System;
using System.Collections.Concurrent;
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
        public static ICollection<MappingStats> MappingStats = new List<MappingStats>();
        public static ConcurrentBag<FailedMappings> FailedMappingCollection = new ConcurrentBag<FailedMappings>();

        public static void LogFailedMapping(string fbTableName, string fbPrimaryId, string sqlTableName, Type classType, string serializedJson, string failedReason)
        {
            var fm = new FailedMappings();
            fm.FBTableName = fbTableName;
            fm.FBPrimaryKey = fbPrimaryId;
            fm.SqlTablename = sqlTableName;
            fm.ObjectType = classType;
            fm.JsonSerializedObject = serializedJson;
            fm.FailedReason = failedReason;

            FailedMappingCollection.Add(fm);
        }

        public static void LogMappingStat(string fbTablename, int fbRecordCount, string sqlTablename, int completedMappings, int failedMappings)
        {
            var ms = new MappingStats();
            ms.FBTableName = fbTablename;
            ms.FBRecordCount = fbRecordCount;
            ms.SQLMappedTable = sqlTablename;
            ms.CompletedMappingsCount = completedMappings;
            ms.FailedMappingsCount = failedMappings;

            MappingStats.Add(ms);
        }

        public static string ExportToLog()
        {
            var ss = JsonConvert.SerializeObject(SqlTableStatistics, Formatting.Indented);
            var ms = JsonConvert.SerializeObject(MappingStats, Formatting.Indented);

            var sb = new StringBuilder();
            sb.AppendLine("Sql Table Statistics");
            sb.AppendLine(ss);
            sb.AppendLine("Mapping Statistics");
            sb.AppendLine(ms);

            if (File.Exists(MigrationVariables.LogFileLocation))
            {
                File.Delete(MigrationVariables.LogFileLocation);
            }

            File.WriteAllText(MigrationVariables.LogFileLocation, sb.ToString());

            return sb.ToString();
        }

        public static void ClearAll()
        {
            SqlTableStatistics.Clear();
            MappingStats.Clear();
            FailedMappingCollection = new ConcurrentBag<FailedMappings>();
        }
    }
}
