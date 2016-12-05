using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SqlValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            //var cls = new FBDataAccess(21002);
            //cls.GetTableNames();
            //var cls = new TableAgent(999); //localhost
            //cls.GetTableSchema();
            //cls.GetData();
            //var table = cls.GetDataTable();
            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //    var col = table.Columns[i];
            //    System.Console.WriteLine(String.Format("Column Name: {0} - Column Type: {1}", col.ColumnName, col.DataType.Name));
            //}
            //var tac = new TableAgentCollection(999); Local



            //MigrationVariables.SiteId = 21002;
            //var t = TableAgentCollection.TableAgents;
            //System.Console.ReadLine();


            //TestTransaction();
            TestValidation();
        }

        private static void TestValidation()
        {
            var val = new InsulinTypeValidation();
            var valid = val.ValidateTable();
            
            if (!valid)
            {
                val.SyncTable();
            }


            //var valid = val.ValidateAll();

            //bool failed = valid.Any(a => a.Value == false);

            //System.Console.WriteLine($"Has any failed: {failed}");
            System.Console.WriteLine(valid);
            System.Console.ReadLine();
        }

        static void TestTransaction()
        {
            var pat = new Patient
            {
                MRID = "12345",
                Firstname = "Mighty",
                Lastname = "Lion",
                Gender = 1,
                DateofBirth = new DateTime(2000, 8, 1)
            };

            var di = new DatabaseInfo { SiteId = 555 };
            TransactionManager.DatabaseContext.Patients.Add(pat);
            TransactionManager.DatabaseContext.DatabaseInfoes.Add(di);
            TransactionManager.ExecuteTransaction();
            //var pCount = TransactionManager.DatabaseContext.Patients.Count();
            //var dCount = TransactionManager.DatabaseContext.DatabaseInfoes.Count();
            //System.Console.WriteLine(pCount);
            //System.Console.WriteLine(dCount);

            //System.Console.ReadLine();
        }
    }
}
