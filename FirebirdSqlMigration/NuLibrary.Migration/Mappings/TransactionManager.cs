using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Concurrent;
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
            DatabaseContext.Dispose();
            return true;

            //bool result = false;
            //using (var trans = DatabaseContext.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        trans.Commit();
            //    }
            //    catch (Exception e)
            //    {
            //        var msg = e.Message;
            //        trans.Rollback();
            //    }
            //    finally
            //    {
            //        DatabaseContext.Dispose();
            //    }
            //}
            //return result;
        }
    }
}
