using FirebirdSql.Data.FirebirdClient;
using NuLibrary.Migration.DatabaseUtilities;
using NuLibrary.Migration.SQLDatabase.EF;
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
        public int SiteId { get; set; }

        public FBDataAccess(int siteId)
        {
            SiteId = siteId;
        }

        public override DbProviderFactory GetDbProvider()
        {
            return DbProviderFactories.GetFactory("FirebirdSql.Data.FirebirdClient");
        }
        public override IDbConnection GetConnnection()
        {
            FirebirdConnection connEntity;
            string connStr = String.Empty;
            using (var ctx = new AspnetDbEntities())
            {
                connEntity = ctx.FirebirdConnections.Where(s => s.SiteId == SiteId).FirstOrDefault();
            }

            if (connEntity != null)
            {
                var split = connEntity.DatabaseLocation.Split(':');
                var dbFile = String.Format("{0}:{1}", split[1], split[2]);
                connStr = String.Format("User={0};Password={1};Database={2};DataSource={3};Port={4};Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;", connEntity.User, connEntity.Password, dbFile, connEntity.DatasourceServer, connEntity.Port);
            }

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
                //Console.WriteLine("Connection object: {0}", cn.GetType().Name);
                cn.Open();

                var dp = GetDbProvider();

                DbCommand cmd = dp.CreateCommand();
                Console.WriteLine("Creating Command object");
                cmd.Connection = cn;
                cmd.CommandText = "Select * from Patients";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    //Console.WriteLine("Creating data reader object");
                    while (dr.Read())
                    {
                        //Console.WriteLine("Name: {0}", dr["FIRSTNAME"]);
                    }
                }
            }

            Console.ReadLine();
        }
        /// <summary>
        /// Gets the name of the tables within this database, excluding system tables ($).
        /// </summary>
        /// <returns>ICollection<string> - Collection of table names</returns>
        public ICollection<string> GetTableNames()
        {
            ICollection<string> results = new List<string>();
            using (FbConnection cn = (FbConnection)GetConnnection())
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                    var tableNames = cn.GetSchema("Tables");

                    foreach (System.Data.DataRow row in tableNames.Rows)
                    {
                        //Console.WriteLine("Table Name = {0}", row["TABLE_NAME"]);

                        if (!row["TABLE_NAME"].ToString().Contains("$"))
                        {
                            results.Add((string)row["TABLE_NAME"]);
                        }
                    }
                    cn.Close();
                }
            }
            //Console.ReadLine();
            return results;
        }
    }
}
