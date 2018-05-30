using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class FailedMappings
    {
        public string FBTableName { get; set; }
        public string FBPrimaryKey { get; set; }
        public string SqlTablename { get; set; }
        public Type ObjectType { get; set; }
        public string JsonSerializedObject { get; set; }
        public string FailedReason { get; set; }

        public FailedMappings(string fbTableName, string fbPrimaryId, string sqlTableName, Type classType, string serializedJson, string failedReason)
        {
            FBTableName = fbTableName;
            FBPrimaryKey = fbPrimaryId;
            SqlTablename = sqlTableName;
            ObjectType = classType;
            JsonSerializedObject = serializedJson;
            FailedReason = failedReason;
        }

        public FailedMappings()
        {

        }
    }
}
