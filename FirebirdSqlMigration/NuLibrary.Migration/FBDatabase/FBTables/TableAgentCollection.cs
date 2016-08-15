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
        public int SiteId { get; set; }
        public TableAgentCollection(int siteId)
        {
            SiteId = siteId;
            Populate();
        }

        private void Populate()
        {
            var fbAccess = new FBDataAccess(SiteId);

            //var mr = fbAccess.GetTableNames().Where(n => n == "METERREADING").FirstOrDefault();
            //this.AddOrUpdate(mr, new TableAgent(SiteId, mr), (k, v) => this[k] = v);

            Parallel.ForEach(fbAccess.GetTableNames().ToArray(), t =>
            {
                this.AddOrUpdate(t, new TableAgent(SiteId, t), (k, v) => this[k] = v);
            });
        }
    }
}
