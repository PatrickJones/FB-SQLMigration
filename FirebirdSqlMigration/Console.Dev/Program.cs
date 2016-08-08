using NuLibrary.Migration.FBDatabase;
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
            var cls = new FBDataAccess(21002);
            //cls.GetTableSchema();
            //cls.GetData();
            var table = cls.GetDataTable();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                var col = table.Columns[i];
                System.Console.WriteLine(String.Format("Column Name: {0} - Column Type: {1}", col.ColumnName, col.DataType.Name));
            }
            System.Console.ReadLine();
        }
    }
}
