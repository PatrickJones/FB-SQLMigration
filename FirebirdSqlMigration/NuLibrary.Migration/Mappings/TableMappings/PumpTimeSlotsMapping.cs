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
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PumpTimeSlotsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpTimeSlotsMapping() : base("PUMP TIMESLOTS")
        {

        }

        public PumpTimeSlotsMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePumpTimeSlotsMapping()
        {
            MappingUtilities mu = new MappingUtilities();
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var PatientId = (String)row["PATIENTID"];
                var programKey = (int)row["PROGRAMNUMBER"];
                var programName = (string)row["PROGRAMNAME"];
                var pp = mu.FindPumpProgram(programName, programKey);
                var ppId = pp.PumpProgramId;

                for (int i = 1; i < 24; i++)
                {
                    DateTime bastart = (DateTime)row[$"BASAL{i}STARTTIME"];
                    DateTime bastop = (DateTime)row[$"BASAL{i}STOPTIME"];
                    BasalProgramTimeSlot bats = new BasalProgramTimeSlot
                    {
                        PumpProgramId = ppId,
                        BasalValue = (int)row[$"BASAL{i}VAL"],
                        StartTime = new TimeSpan(bastart.Ticks),
                        StopTime = new TimeSpan(bastop.Ticks)
                    };
                    TransactionManager.DatabaseContext.BasalProgramTimeSlots.Add(bats);

                    if (i < 13)
                    {
                        DateTime botime = (DateTime)row[$"BOLUS{i}TIME"];
                        BolusProgramTimeSlot bots = new BolusProgramTimeSlot
                        {
                            PumpProgramId = ppId,
                            BolusValue = (int)row[$"BOLUS{i}VAL"],
                            StartTime = new TimeSpan(botime.Ticks)
                        };
                        TransactionManager.DatabaseContext.BolusProgramTimeSlots.Add(bots);
                    }
                }
            }
        }
    }
}
