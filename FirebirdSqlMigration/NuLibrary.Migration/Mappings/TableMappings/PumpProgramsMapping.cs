using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings.InMemoryMappings;
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
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PumpProgramsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpProgramsMapping() : base("PATIENTPUMPPROGRAM")
        {

        }

        public PumpProgramsMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        MappingUtilities mu = new MappingUtilities();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreatePumpProgramssMapping()
        {

            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

                    if (userId != Guid.Empty)
                    {
                        var CreationDate = mu.ParseFirebirdDateTime(row["CREATEDATE"].ToString());
                        var Source = (row["SOURCE"] is DBNull) ? String.Empty : row["SOURCE"].ToString();
                        var Valid = mu.ParseFirebirdBoolean(row["ACTIVEPROGRAM"].ToString());

                        for (int i = 1; i < 8; i++)
                        {
                            var pKey = mu.ParseInt(row[$"PROG{i}KEYID"].ToString());

                            if (pKey != 0)
                            {
                                PumpProgram p = new PumpProgram();

                                p.CreationDate = CreationDate;
                                p.Source = Source;
                                p.Valid = Valid;
                                p.ProgramKey = pKey;
                                p.NumOfSegments = 7;

                                MemoryMappings.AddPumpProgram(userId, pKey, p);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error creating PumpProgram mapping.", e);
            }
        }
    }
}
