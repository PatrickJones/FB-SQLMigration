using NuLibrary.Migration.GlobalVar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.FBDatabase.FBTables
{
    /// <summary>
    /// Collection of TableAgents
    /// </summary>
    public static class TableAgentCollection
    {
        /// <summary>
        /// Collection of TableAgents
        /// </summary>
        public static ConcurrentDictionary<string, TableAgent> TableAgents = new ConcurrentDictionary<string, TableAgent>();

        /// <summary>
        /// Populates a collection of TableAgents based on all Firebird Table names
        /// </summary>
        public static void Populate()
        {
            Parallel.ForEach(MigrationVariables.FirebirdTableNames.ToArray(), t =>
            {
                TableAgents.AddOrUpdate(t, new TableAgent(t), (k, v) => TableAgents[k] = v);
            });
        }

        /// <summary>
        /// Populates a collection of TableAgents based on select Firebird Table names
        /// </summary>
        /// <param name="tableNames"></param>
        public static void Populate(ICollection<string> tableNames)
        {
            var temp = from tn in tableNames
                       from ft in MigrationVariables.FirebirdTableNames
                       where tn == ft
                       select ft;

            Parallel.ForEach(temp.ToArray(), t =>
            {
                TableAgents.AddOrUpdate(t, new TableAgent(t), (k, v) => TableAgents[k] = v);
            });
        }
    }
}
