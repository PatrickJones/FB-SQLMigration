using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Concurrent;
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

        public void CreatePumpProgramsMapping()
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
                        var CreationDate = (row["CREATEDATE"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row["CREATEDATE"].ToString());
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
                                p.ProgramName = $"Prog {pKey}";
                                p.BasalProgramTimeSlots = GetBasalPrgTimeSlots(userId, CreationDate);
                                p.BolusProgramTimeSlots = GetBolusPrgTimeSlots(userId, CreationDate);

                                if (CreationDate != DateTime.MinValue)
                                {
                                    MemoryMappings.AddPumpProgram(userId, pKey, p);
                                }
                                else
                                {
                                    MappingStatistics.LogFailedMapping("PumpPrograms", typeof(PumpProgram), JsonConvert.SerializeObject(p), "Unable to add PumpProgram to database because creation date was null.");
                                    FailedCount++;
                                }
                            }
                        }
                    }
                }

                MappingStatistics.LogMappingStat("PATIENTPUMPPROGRAM", RecordCount, "PumpPrograms", 0, MemoryMappings.GetAllPumpPrograms().Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating PumpProgram mapping.", e);
            }
        }

        private ICollection<BolusProgramTimeSlot> GetBolusPrgTimeSlots(Guid userId, DateTime creationDate)
        {
            var slots = MemoryMappings.GetAllBolusPrgTimeSlots();
            ConcurrentBag<BolusProgramTimeSlot> results = new ConcurrentBag<BolusProgramTimeSlot>();

            if (slots.ContainsKey(userId))
            {
                var set = slots[userId];
                Array.ForEach(set.ToArray(), s => {
                    if (s.Key == creationDate)
                    {
                        Parallel.ForEach(s.Value.ToArray(), v => {
                            results.Add(v);
                    });
                    }
                });
            }

            return results.ToList();
        }

        private ICollection<BasalProgramTimeSlot> GetBasalPrgTimeSlots(Guid userId, DateTime creationDate)
        {
            var slots = MemoryMappings.GetAllBasalPrgTimeSlots();
            ConcurrentBag<BasalProgramTimeSlot> results = new ConcurrentBag<BasalProgramTimeSlot>();

            if (slots.ContainsKey(userId))
            {
                var set = slots[userId];
                Array.ForEach(set.ToArray(), s => {
                    if (s.Key == creationDate)
                    {
                        Parallel.ForEach(s.Value.ToArray(), v => {
                            results.Add(v);
                        });
                    }
                });
            }

            return results.ToList();
        }
    }
}
