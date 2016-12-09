using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
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
        public PumpsMapping() : base("PATIENTPUMP")
        {

        }

        public PumpsMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();

        public void CreatePumpsMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = (String)row["PATIENTID"];
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var pum = new Pump
                    {
                        UserId = userId,
                        PumpName = (String)row["PUMPBRAND"],
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
}
