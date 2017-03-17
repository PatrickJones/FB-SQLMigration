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
        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the Firebird table.
        /// </value>
        public string TableName { get; set; }
        /// <summary>
        /// Gets or sets the DataSet.
        /// </summary>
        /// <value>
        /// The DataSet.
        /// </value>
        public DataSet DataSet { get; set; }
        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>
        /// The Firebird table row count
        /// </value>
        public Int32 RowCount { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAgent"/> class.
        /// </summary>
        /// <param name="siteId">The site id of the client.</param>
        public TableAgent()
        {
            TableName = this.GetType().Name.ToUpper();
            Init();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAgent"/> class.
        /// </summary>
        /// <param name="siteId">The site id of the client.</param>
        /// <param name="tableName">Name of the table.</param>
        public TableAgent(string tableName)
        {
            TableName = tableName;
            Init();
        }
        /// <summary>
        /// Initializes this instance.
        /// </summary>
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
                var yearBack = DateTime.Now.Subtract(new TimeSpan(365, 0, 0, 0));

                if (TableName == "METERREADING")
                {
                    queryStr = $"Select * from {TableName} where READINGDATETIME > '{yearBack.Month}-{yearBack.Day}-{yearBack.Year}'";
                }

                //if(TableName == "METERREADINGHEADER")
                //{
                //    queryStr = $"Select * from {TableName} where METERDATETIME > '{yearBack.Month}-{yearBack.Day}-{yearBack.Year}'";
                //}

                var adt = new FbDataAdapter(queryStr, cn);

                adt.Fill(DataSet, TableName);
            }

            Console.WriteLine(RowCount);
        }
        /// <summary>
        /// Gets the DataTable.
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetDataTable()
        {
            return DataSet.Tables[TableName];
        }
        /// <summary>
        /// Gets the table schema in xml.
        /// </summary>
        /// <returns></returns>
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
