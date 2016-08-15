using FirebirdSql.Data.FirebirdClient;
using NuLibrary.Migration.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuLibrary.Migration.FBDatabase.FBTables
{
    public class TableAgent : FBDataAccess, ITableData
    {
        public string TableName { get; set; }
        public DataSet DataSet { get; set; }
        public Int32 RowCount { get; set; }
        public TableAgent(int siteId) : base(siteId)
        {
            TableName = this.GetType().Name.ToUpper();
            Init();
        }

        public TableAgent(int siteId, string tableName) : base(siteId)
        {
            TableName = tableName;
            Init();
        }

        public void Init()
        {
            Console.WriteLine($"Creating TableAgent for: {TableName}");
            DataSet = new DataSet();
            using (FbConnection cn = (FbConnection)GetConnnection())
            {
                //Console.WriteLine("Connection object: {0}", cn.GetType().Name);
                cn.Open();
                var dp = GetDbProvider();

                DbCommand cmd = dp.CreateCommand();
                //Console.WriteLine("Creating Command object");
                cmd.Connection = cn;
                cmd.CommandText = $"Select COUNT(*) from {TableName}";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    //Console.WriteLine("Creating data reader object");
                    while (dr.Read())
                    {
                        RowCount = (Int32)dr["COUNT"];
                    }
                }


                string queryStr = $"Select * from {TableName}";
                var adt = new FbDataAdapter(queryStr, cn);

                adt.Fill(DataSet, TableName);
            }
        }

        public virtual DataTable GetDataTable()
        {
            return DataSet.Tables[TableName];
        }

        public virtual XDocument GetTableSchema()
        {
            var table = this.GetDataTable();
            var schema = String.Empty;

            using (var ms = new MemoryStream())
            {
                table.WriteXmlSchema(ms);
                ms.Position = 0;

                var sr = new StreamReader(ms);
                schema = sr.ReadToEnd();

                //Console.WriteLine(schema);
                //Console.ReadLine();
            }

            return XDocument.Parse(schema);
        }

    }
}
