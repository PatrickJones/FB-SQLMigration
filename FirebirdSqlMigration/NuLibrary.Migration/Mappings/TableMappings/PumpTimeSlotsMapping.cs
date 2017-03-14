using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
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

                    // get the PumpProgram instance

                    // temp collecions
                    var tempBasal = new List<BasalProgramTimeSlot>();
                    var tempBolus = new List<BolusProgramTimeSlot>();

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
                            BasalProgramTimeSlot bats = new BasalProgramTimeSlot
                            {
                                BasalValue = mu.ParseInt(row[$"BASAL{i}VAL"].ToString()),
                                StartTime = bastart.TimeOfDay,
                                StopTime = bastop.TimeOfDay
                            };

                            if (createDate != DateTime.MinValue)
                            {
                                tempBasal.Add(bats);
                            }
                            else
                            {
                                //TransactionManager.FailedMappingCollection
                                //.Add(new FailedMappings
                                //{
                                //    Tablename = FbTableName,
                                //    ObjectType = typeof(BasalProgramTimeSlot),
                                //    JsonSerializedObject = JsonConvert.SerializeObject(bats),
                                //    FailedReason = "Unable to add BasalProgramTimeSlot to database because creation date was null."
                                //});

                                MappingStatistics.LogFailedMapping("BasalProgramTimeSlots", typeof(BasalProgramTimeSlot), JsonConvert.SerializeObject(bats), "Unable to add BasalProgramTimeSlot to database because creation date was null.");
                                FailedCount++;
                            }
                        }

                        if (i < 13)
                        {
                            DateTime botime = (row[$"BOLUS{i}TIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BOLUS{i}TIME"].ToString());

                            if (botime != DateTime.MinValue)
                            {
                                BolusProgramTimeSlot bots = new BolusProgramTimeSlot
                                {
                                    BolusValue = mu.ParseInt(row[$"BOLUS{i}VAL"].ToString()),
                                    StartTime = botime.TimeOfDay
                                };

                                if (createDate != DateTime.MinValue)
                                {
                                    tempBolus.Add(bots);
                                }
                                else
                                {
                                    //TransactionManager.FailedMappingCollection
                                    //.Add(new FailedMappings
                                    //{
                                    //    Tablename = FbTableName,
                                    //    ObjectType = typeof(BolusProgramTimeSlot),
                                    //    JsonSerializedObject = JsonConvert.SerializeObject(bots),
                                    //    FailedReason = "Unable to add BolusProgramTimeSlot to database because creation date was null."
                                    //});

                                    MappingStatistics.LogFailedMapping("BolusProgramTimeSlots", typeof(BolusProgramTimeSlot), JsonConvert.SerializeObject(bots), "Unable to add BolusProgramTimeSlot to database because creation date was null.");
                                    FailedCount++;
                                }
                            }
                        }
                    }

                    if (createDate == DateTime.MinValue)
                    {
                        FailedCount++;
                    }

                    Array.ForEach(tempBasal.ToArray(), a => {
                        MemoryMappings.AddBasalPrgTimeSlot(userId, createDate, a);
                    });

                    Array.ForEach(tempBolus.ToArray(), a => {
                        MemoryMappings.AddBolusPrgTimeSlot(userId, createDate, a);
                    });
                }

                MappingStatistics.LogMappingStat("PUMPTIMESLOTS", RecordCount, "BasalProgramTimeSlots", 0, MemoryMappings.GetAllBasalPrgTimeSlots().Count, FailedCount);
                MappingStatistics.LogMappingStat("PUMPTIMESLOTS", RecordCount, "BolusProgramTimeSlots", 0, MemoryMappings.GetAllBolusPrgTimeSlots().Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Pump Program Time Slot mapping.", e);
            }
        }

        public void AddToContext()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
