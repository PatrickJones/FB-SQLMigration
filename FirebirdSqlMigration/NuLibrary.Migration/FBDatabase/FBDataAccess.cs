using FirebirdSql.Data.FirebirdClient;
using NuLibrary.Migration.DatabaseUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuLibrary.Migration.FBDatabase
{
    public class FBDataAccess : DatabaseAccessADO
    {
        public override DbProviderFactory GetDbProvider()
        {
            return DbProviderFactories.GetFactory("FirebirdSql.Data.FirebirdClient");
        }

        public override IDbConnection GetConnnection()
        {
            // Should come from database or config file. Not Hardcoded.
            string connStr = @"User=SYSDBA;Password=masterkey;Database=C:\Users\Patrick\Documents\FirebirdDatabases\Lion.gdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";

            var dbConn = GetDbProvider();
            DbConnection conn = dbConn.CreateConnection();
            conn.ConnectionString = connStr;

            return conn;
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new FbDataAdapter();
        }

        public override IDbCommand GetCommand()
        {
            return new FbCommand();
        }

        public override IDbDataParameter GetDbParameter()
        {
            return new FbParameter();
        }

        public void GetData()
        {
            using (FbConnection cn = (FbConnection)GetConnnection())
            {
                Console.WriteLine("Connection object: {0}", cn.GetType().Name);
                cn.Open();

                var dp = GetDbProvider();

                DbCommand cmd = dp.CreateCommand();
                Console.WriteLine("Creating Command object");
                cmd.Connection = cn;
                cmd.CommandText = "Select * from Lions";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    Console.WriteLine("Creating data reader object");
                    while (dr.Read())
                    {
                        Console.WriteLine("Name: {0}", dr["Name"]);
                    }
                }
            }

            Console.ReadLine();
        }

        public DataTable GetDataTable()
        {
            using (FbConnection cn = (FbConnection)GetConnnection())
            {
                Console.WriteLine("Connection object: {0}", cn.GetType().Name);

                var adt = new FbDataAdapter("Select * from Lions", cn);

                DataSet lions = new DataSet();
                adt.Fill(lions, "Lions");

                Console.WriteLine(lions.Tables["Lions"].Rows.Count);
                Console.ReadLine();
                return lions.Tables["Lions"];
            }
        }

        public XDocument GetTableSchema()
        {
            var table = this.GetDataTable();
            var schema = String.Empty;

            using (var ms = new MemoryStream())
            {
                table.WriteXmlSchema(ms);
                ms.Position = 0;

                var sr = new StreamReader(ms);
                schema = sr.ReadToEnd();

                Console.WriteLine(schema);
                Console.ReadLine();
            }

            return XDocument.Parse(schema);
        }
    }
}
