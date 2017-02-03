using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
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
    public class PumpTimeSlotsMapping : BaseMapping
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
                    //var pp = mu.FindPumpProgram(programName, programKey);
                    //var ppId = pp.PumpProgramId;

                    for (int i = 1; i < 25; i++)
                    {
                        DateTime bastart = (row[$"BASAL{i}STARTTIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BASAL{i}STARTTIME"].ToString());
                        DateTime bastop = (row[$"BASAL{i}STOPTIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BASAL{i}STOPTIME"].ToString());

                        if (bastart != DateTime.MinValue && bastop != DateTime.MinValue)
                        {
                            BasalProgramTimeSlot bats = new BasalProgramTimeSlot
                            {
                                //PumpProgramId = ppId,
                                BasalValue = mu.ParseInt(row[$"BASAL{i}VAL"].ToString()),
                                StartTime = new TimeSpan(bastart.Ticks),
                                StopTime = new TimeSpan(bastop.Ticks)
                            };

                            tempBasal.Add(bats);
                        }

                        //TransactionManager.DatabaseContext.BasalProgramTimeSlots.Add(bats);

                        if (i < 13)
                        {
                            DateTime botime = (row[$"BOLUS{i}TIME"] is DBNull) ? DateTime.MinValue : mu.ParseFirebirdDateTime(row[$"BOLUS{i}TIME"].ToString());

                            if (botime != DateTime.MinValue)
                            {
                                BolusProgramTimeSlot bots = new BolusProgramTimeSlot
                                {
                                    //PumpProgramId = ppId,
                                    BolusValue = mu.ParseInt(row[$"BOLUS{i}VAL"].ToString()),
                                    StartTime = new TimeSpan(botime.Ticks)
                                };

                                tempBolus.Add(bots);
                            }

                            //TransactionManager.DatabaseContext.BolusProgramTimeSlots.Add(bots);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error creating Pump Program Time Slot mapping.", e);
            }
        }
    }
}
