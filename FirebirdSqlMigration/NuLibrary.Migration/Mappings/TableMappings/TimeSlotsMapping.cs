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
    public class TimeSlotsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public TimeSlotsMapping() : base("TIMESLOT")
        {

        }

        public TimeSlotsMapping(string tableName) :base(tableName)
        {

        }

        public void CreateTimeSlotsMapping()
        {
            MappingUtilities mu = new MappingUtilities();
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var pId = (String)row["PATIENTID"];
                var careset = mu.FindPatientCareSetting(pId);
                var caresetId = careset.CareSettingsId;
                for (int i = 1; i < 9; i++)
                {
                    DailyTimeSlot d = new DailyTimeSlot();

                    d.TimeSlotDescription = (String)row[$"SLOT{i}DESC"];
                    if (i < 8)
                    {
                        d.TImeSlotBoundary = (TimeSpan)row[$"SLOT{i}END"];
                    } 
                    d.CareSettingsId = caresetId;

                    TransactionManager.DatabaseContext.DailyTimeSlots.Add(d);
                }
            }
        }
    }
}
