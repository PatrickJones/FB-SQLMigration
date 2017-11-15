using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
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
    public class PumpTimeSlotsMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpTimeSlotsMapping() : base("PUMPTIMESLOTS")
        {

        }

        public PumpTimeSlotsMapping(string tableName) :base(tableName)
        {

        }

        MappingUtilities mu = new MappingUtilities();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreatePumpTimeSlotsMapping()
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
                        // temp collecions
                        var tempBasal = new List<ProgramTimeSlot>();
                        var tempBolus = new List<ProgramTimeSlot>();

                        var keyId = mu.ParseInt(row["KEYID"].ToString());
                        var programKey = mu.ParseInt(row["PROGRAMNUMBER"].ToString());
                        var programName = (row["PROGRAMNAME"] is DBNull) ? "Name" : row["PROGRAMNAME"].ToString();
                        var createDate = (row["CREATEDATE"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row["CREATEDATE"].ToString());

                        for (int i = 1; i < 25; i++)
                        {
                            DateTime bastart = (row[$"BASAL{i}STARTTIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BASAL{i}STARTTIME"].ToString());
                            DateTime bastop = (row[$"BASAL{i}STOPTIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BASAL{i}STOPTIME"].ToString());

                            if (bastart != DateTime.MinValue && bastop != DateTime.MinValue)
                            {
                                ProgramTimeSlot bats = new ProgramTimeSlot
                                {
                                    Value = mu.ParseDouble(row[$"BASAL{i}VAL"].ToString()),
                                    StartTime = bastart.TimeOfDay,
                                    StopTime = bastop.TimeOfDay,
                                    DateSet = createDate
                                };

                                if (createDate != DateTime.MinValue && IsValid(bats))
                                {
                                    tempBasal.Add(bats);
                                }
                                else
                                {
                                    MappingStatistics.LogFailedMapping("PUMPTIMESLOTS", row["KEYID"].ToString(), "BasalProgramTimeSlots", typeof(ProgramTimeSlot), JsonConvert.SerializeObject(bats), "Unable to add BasalProgramTimeSlot to database because creation date was null.");
                                    FailedCount++;
                                }
                            }

                            if (i < 13)
                            {
                                DateTime botime = (row[$"BOLUS{i}TIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BOLUS{i}TIME"].ToString());

                                if (botime != DateTime.MinValue)
                                {
                                    ProgramTimeSlot bots = new ProgramTimeSlot
                                    {
                                        Value = mu.ParseDouble(row[$"BOLUS{i}VAL"].ToString()),
                                        StartTime = botime.TimeOfDay,
                                        DateSet = createDate
                                    };

                                    if (createDate != DateTime.MinValue && IsValid(bots))
                                    {
                                        tempBolus.Add(bots);
                                    }
                                    else
                                    {
                                        MappingStatistics.LogFailedMapping("PUMPTIMESLOTS", row["KEYID"].ToString(), "BolusProgramTimeSlots", typeof(ProgramTimeSlot), JsonConvert.SerializeObject(bots), "Unable to add BolusProgramTimeSlot to database because creation date was null.");
                                        FailedCount++;
                                    }
                                }
                            }
                        }

                        if (createDate == DateTime.MinValue)
                        {
                            FailedCount++;
                        }

                        Array.ForEach(tempBasal.ToArray(), a =>
                        {
                            MemoryMappings.AddBasalPrgTimeSlot(userId, createDate, a);
                        });

                        Array.ForEach(tempBolus.ToArray(), a =>
                        {
                            MemoryMappings.AddBolusPrgTimeSlot(userId, createDate, a);
                        });
                    }
                }

                MappingStatistics.LogMappingStat("PUMPTIMESLOTS", RecordCount, "BasalProgramTimeSlots", MemoryMappings.GetAllBasalPrgTimeSlots().Count, FailedCount);
                MappingStatistics.LogMappingStat("PUMPTIMESLOTS", RecordCount, "BolusProgramTimeSlots", MemoryMappings.GetAllBolusPrgTimeSlots().Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Pump Program Time Slot mapping.", e);
            }
        }

        private bool IsValid(ProgramTimeSlot slots)
        {
            var ts = new TimeSpan(0, 0, 0);

            if (slots.Value == 0.0 && slots.StartTime == ts && slots.StopTime == ts)
            {
                return false;
            }

            return true;
            //return slots.StartTime != ts && slots.StopTime != ts && slots.Value != 0.0;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
