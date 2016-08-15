using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.DatabaseUtilities
{
    public abstract class DatabaseAccessADO
    {
        /// <summary>
        /// Gets the database provider.
        /// </summary>
        /// <returns></returns>
        public virtual DbProviderFactory GetDbProvider()
        {
            // Default Provider
            return DbProviderFactories.GetFactory("System.Data.SqlClient");
        }
        /// <summary>
        /// Gets the database connnection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection GetConnnection()
        {
            // TODO: Should come from database or config file. Not Hardcoded.
            string connStr = @"Data Source=PRIMARYPC\PRIMARY2; Initial Catalog=LionDatabase; Integrated Security=True";

            var dbConn = GetDbProvider();
            DbConnection conn = dbConn.CreateConnection();
            conn.ConnectionString = connStr;

            return conn;
        }
        /// <summary>
        /// Gets the data adapter for the database.
        /// </summary>
        /// <returns></returns>
        public virtual IDbDataAdapter GetDataAdapter()
        {
            // Default Data Adapter
            return new SqlDataAdapter();
        }
        /// <summary>
        /// Gets the database command.
        /// </summary>
        /// <returns></returns>
        public virtual IDbCommand GetCommand()
        {
            // Default Command Object
            return new SqlCommand();
        }
        /// <summary>
        /// Gets the database parameter.
        /// </summary>
        /// <returns></returns>
        public virtual IDbDataParameter GetDbParameter()
        {
            // Default Paramter object
            return new SqlParameter();
        }
    }
}
