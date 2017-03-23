﻿using Newtonsoft.Json;
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

        public static void LogFailedMapping(string tableName, Type classType, string serializedJson, string failedReason)
        {
            var fm = new FailedMappings();
            fm.Tablename = tableName;
            fm.ObjectType = classType;
            fm.JsonSerializedObject = serializedJson;
            fm.FailedReason = failedReason;
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

            File.WriteAllText(@"C:\Users\pjones\Documents\FbMigrationLog.json", sb.ToString());

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
