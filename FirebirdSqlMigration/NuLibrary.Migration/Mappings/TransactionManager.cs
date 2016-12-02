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
                    DatabaseContext.SaveChanges();
                    trans.Commit();
                    DatabaseContext.Dispose();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    DatabaseContext.Dispose();
                }
            }
            return result;
        }
    }
}
