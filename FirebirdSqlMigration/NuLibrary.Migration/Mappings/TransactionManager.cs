using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public static class TransactionManager
    {
        public static NuMedicsGlobalEntities DatabaseContext;
        public static ICollection<FailedMappings> FailedMappingCollection = new List<FailedMappings>();

        static TransactionManager()
        {
            DatabaseContext = new NuMedicsGlobalEntities();
        }
        public static bool ExecuteTransaction()
        {
            bool result = false;
            using (var trans = DatabaseContext.Database.BeginTransaction())
            {
                try
                {
                    trans.Commit();
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    trans.Rollback();
                }
                finally
                {
                    DatabaseContext.Dispose();
                }
            }
            return result;
        }
    }
}
