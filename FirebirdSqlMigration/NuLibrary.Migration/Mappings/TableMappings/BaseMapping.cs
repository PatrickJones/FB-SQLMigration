using NuLibrary.Migration.FBDatabase.FBTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class BaseMapping
    {
        public TableAgent TableAgent { get; set; }
        public string FbTableName { get; set; }

        public BaseMapping(string tableName)
        {
            FbTableName = tableName;
            SetTableAgent();
        }

        private void SetTableAgent()
        {
            if (TableAgentCollection.TableAgents.ContainsKey(FbTableName))
            {
                TableAgent = TableAgentCollection.TableAgents[FbTableName];
            }
        }

    }
}
