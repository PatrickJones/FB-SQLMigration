using NuLibrary.Migration.GlobalVar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.FBDatabase.FBTables
{
    public class TableAgentCollection : ConcurrentDictionary<string, TableAgent>
    {
        public TableAgentCollection()
        {
            Populate();
        }

        private void Populate()
        {
            var fbAccess = new FBDataAccess();

            //var mr = fbAccess.GetTableNames().Where(n => n == "METERREADING").FirstOrDefault();
            //this.AddOrUpdate(mr, new TableAgent(SiteId, mr), (k, v) => this[k] = v);

            Parallel.ForEach(fbAccess.GetTableNames().ToArray(), t =>
            {
                this.AddOrUpdate(t, new TableAgent(t), (k, v) => this[k] = v);
            });
        }
    }
}
