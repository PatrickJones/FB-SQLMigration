using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.FBDatabase.FBTables;
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
            //var cls = new TableAgent(999); //localhost


            //cls.GetTableSchema();
            //cls.GetData();
            //var table = cls.GetDataTable();

            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //    var col = table.Columns[i];
            //    System.Console.WriteLine(String.Format("Column Name: {0} - Column Type: {1}", col.ColumnName, col.DataType.Name));
            //}

            var tac = new TableAgentCollection(999);

            System.Console.ReadLine();
        }
    }
}
