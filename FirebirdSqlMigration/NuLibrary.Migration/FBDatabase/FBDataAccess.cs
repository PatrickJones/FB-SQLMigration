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
                Console.WriteLine("Connection object: {0}", cn.GetType().Name);
                cn.Open();

                var dp = GetDbProvider();

                DbCommand cmd = dp.CreateCommand();
                Console.WriteLine("Creating Command object");
                cmd.Connection = cn;
                cmd.CommandText = "Select * from Patients";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    Console.WriteLine("Creating data reader object");
                    while (dr.Read())
                    {
                        Console.WriteLine("Name: {0}", dr["FIRSTNAME"]);
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

                var adt = new FbDataAdapter("Select * from Patients", cn);

                DataSet patients = new DataSet();
                adt.Fill(patients, "Patients");

                Console.WriteLine(patients.Tables["Patients"].Rows.Count);
                Console.ReadLine();
                return patients.Tables["Patients"];
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
