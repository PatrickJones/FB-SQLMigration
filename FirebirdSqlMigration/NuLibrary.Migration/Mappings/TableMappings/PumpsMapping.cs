﻿using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class PumpsMapping : BaseMapping, IContextHandler
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
        MappingUtilities mu = new MappingUtilities();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();

        public ICollection<Pump> CompletedMappings = new List<Pump>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreatePumpsMapping()
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

                    if (!mHelper.HasPatientMigrated(patId))
                    {
                        if (userId != Guid.Empty)
                        {
                            List<PumpSetting> settings = null;
                            List<PumpProgram> programs = null;

                            if (MemoryMappings.GetAllPumpSettings().ContainsKey(userId))
                            {
                                var set = MemoryMappings.GetAllPumpSettings().Where(k => k.Key == userId).Single();
                                settings = set.Value;
                            }

                            if (MemoryMappings.GetAllPumpPrograms().ContainsKey(userId))
                            {
                                var allprog = MemoryMappings.GetAllPumpPrograms().Count; //TESTING

                                var prog = MemoryMappings.GetAllPumpPrograms().Where(p => p.Key == userId).Single();
                                var tl = prog.Value;
                                programs = new List<PumpProgram>();
                                Array.ForEach(tl.ToArray(), a => {
                                    if (a.Item2.ProgramTimeSlots.Count > 0)
                                    {
                                        programs.Add(a.Item2);
                                    }
                                });
                            }

                            var pum = new Pump
                            {
                                UserId = userId,
                                PumpType = "Meter",
                                PumpName = (row["PUMPBRAND"] is DBNull) ? String.Empty : row["PUMPBRAND"].ToString(),
                                PumpStartDate = mu.ParseFirebirdDateTime(row["PUMPSTARTDATE"].ToString()),
                                PumpInfusionSet = (row["PUMPINFUSIONSET"] is DBNull) ? String.Empty : row["PUMPINFUSIONSET"].ToString(),
                                Cannula = mu.ParseDouble(row["CANNULA"].ToString()),
                                ReplacementDate = mu.ParseFirebirdDateTime(row["DATEREPLACED"].ToString()),
                                Notes = (row["NOTES"] is DBNull) ? String.Empty : row["NOTES"].ToString(),
                                PumpSettings = settings,
                                PumpPrograms = programs
                            };

                            MemoryMappings.AddPump(pum);

                            if (CanAddToContext(pum.UserId, pum.PumpName))
                            {
                                CompletedMappings.Add(pum);
                            }
                            else
                            {
                                MappingStatistics.LogFailedMapping("PATIENTPUMP", patId, "Pumps", typeof(Pump), JsonConvert.SerializeObject(pum), "Unable to add Pump to database.");
                                FailedCount++;
                            }
                        }
                    }

                }

                MappingStatistics.LogMappingStat("PATIENTPUMP", RecordCount, "Pumps", CompletedMappings.Count, FailedCount);
            }
            catch (Exception)
            {
                throw new Exception("Error creating Pump mapping.");
            }
        }

        public void SaveChanges()
        {
            throw new NotImplementedException("Implemeted in DeviceMeterReadingHeaderMapping class.");
        }

        private bool CanAddToContext(Guid userId, string pumpName)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return !ctx.Pumps.Any(a => a.UserId == userId && a.PumpName == pumpName);
            }
        }
    }
}
