﻿using FirebirdSql.Data.FirebirdClient;
using NuLibrary.Migration.DatabaseUtilities;
using NuLibrary.Migration.GlobalVar;
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
        public string DatabaseProvider => "FirebirdSql.Data.FirebirdClient";

        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>
        /// The site identifier.
        /// </value>
        public int SiteId { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="FBDataAccess"/> class.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        public FBDataAccess()
        {
            SiteId = MigrationVariables.CurrentSiteId;
        }
        /// <summary>
        /// Gets the database provider.
        /// </summary>
        /// <returns></returns>
        public override DbProviderFactory GetDbProvider()
        {
            return DbProviderFactories.GetFactory(DatabaseProvider);
        }
        /// <summary>
        /// Gets the database connnection.
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnnection()
        {
            try
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

                System.Diagnostics.Debug.WriteLine($"Current Conncetionsting: {connStr}");

                return conn;
            }
            catch (Exception)
            {
                throw new FormatException("Unable to parse connection string.");
            }
            
        }

        /// <summary>
        /// Gets the data adapter for the database.
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter GetDataAdapter()
        {
            return new FbDataAdapter();
        }
        /// <summary>
        /// Gets the database command.
        /// </summary>
        /// <returns></returns>
        public override IDbCommand GetCommand()
        {
            return new FbCommand();
        }
        /// <summary>
        /// Gets the database parameter.
        /// </summary>
        /// <returns></returns>
        public override IDbDataParameter GetDbParameter()
        {
            return new FbParameter();
        }
        /// <summary>
        /// Gets the data.
        /// </summary>
        public DataTable GetData()
        {
            using (FbConnection cn = (FbConnection)GetConnnection())
            {
                //Console.WriteLine("Connection object: {0}", cn.GetType().Name);
                cn.Open();

                var dp = GetDbProvider();

                DbCommand cmd = dp.CreateCommand();
                Console.WriteLine("Creating Command object");
                cmd.Connection = cn;
                cmd.CommandText = "SELECT FIRST 1 a.METERSENT FROM METERREADING a Where a.READINGTYPE = 'Pump Delivery' and a.EVENTSUBTYPE_1 = 'BOLUS' order BY a.READINGDATETIME desc";

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    Console.WriteLine("Creating data reader object");
                    //while (dr.Read())
                    //{
                    //    Console.WriteLine("READINGNOTE: {0}", dr["READINGNOTE"]);
                    //}

                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    return dt;
                }
            }

            //Console.ReadLine();
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
                        if (!row["TABLE_NAME"].ToString().Contains("$"))
                        {
                            results.Add((string)row["TABLE_NAME"]);
                            System.Diagnostics.Debug.WriteLine($"Adding table: {row["TABLE_NAME"]}");
                        }
                    }
                    cn.Close();
                }
            }
            return results;
        }
    }
}
