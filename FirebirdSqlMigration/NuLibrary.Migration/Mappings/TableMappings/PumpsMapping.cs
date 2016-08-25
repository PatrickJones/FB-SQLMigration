using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class PumpsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpsMapping() : base("Pumps")
        {

        }

        public PumpsMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePumpsMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var pum = new Pump
                {
                    PumpStartDate = (DateTime)row["PUMPSTARTDATE"],
                    PumpInfusionSet = (String)row["PUMPINFUSIONSET"],
                    Cannula = (Double)row["CANNULA"],
                    ReplacementDate = (DateTime)row["DATEREPLACED"],
                    Notes = (String)row["NOTES"]
                };

                TransactionManager.DatabaseContext.Pumps.Add(pum);
            }
        }
    }
}
