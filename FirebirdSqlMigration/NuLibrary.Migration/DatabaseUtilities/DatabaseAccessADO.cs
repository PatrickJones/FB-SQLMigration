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
        public virtual DbProviderFactory GetDbProvider()
        {
            // Default Provider
            return DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        public virtual IDbConnection GetConnnection()
        {
            // TODO: Should come from database or config file. Not Hardcoded.
            string connStr = @"Data Source=PRIMARYPC\PRIMARY2; Initial Catalog=LionDatabase; Integrated Security=True";

            var dbConn = GetDbProvider();
            DbConnection conn = dbConn.CreateConnection();
            conn.ConnectionString = connStr;

            return conn;
        }

        public virtual IDbDataAdapter GetDataAdapter()
        {
            // Default Data Adapter
            return new SqlDataAdapter();
        }

        public virtual IDbCommand GetCommand()
        {
            // Default Command Object
            return new SqlCommand();
        }

        public virtual IDbDataParameter GetDbParameter()
        {
            // Default Paramter object
            return new SqlParameter();
        }
    }
}
