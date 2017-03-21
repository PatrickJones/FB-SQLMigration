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
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class TimeSlotsMapping : BaseMapping, IContextHandler
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

        MappingUtilities mu = new MappingUtilities();
        AspnetDbHelpers aHelper = new AspnetDbHelpers();

        public ICollection<DailyTimeSlot> CompletedMappings = new List<DailyTimeSlot>();
        private ICollection<Tuple<Guid, DailyTimeSlot>> tempMappings = new List<Tuple<Guid, DailyTimeSlot>>();

        public int RecordCount = 0;
        public int FailedCount = 0;
                
        public void CreateTimeSlotsMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

                    if (userId != Guid.Empty)
                    {
                        for (int i = 1; i < 9; i++)
                        {
                            DailyTimeSlot d = new DailyTimeSlot();

                            d.TimeSlotDescription = (row[$"SLOT{i}DESC"] is DBNull) ? String.Empty : row[$"SLOT{i}DESC"].ToString();
                            if (i < 8)
                            {
                                d.TImeSlotBoundary = (row[$"SLOT{i}END"] is DBNull) ? new TimeSpan(12, 0, 0) : mu.ParseFirebirdTimespan(row[$"SLOT{i}END"].ToString());
                            }

                            tempMappings.Add(new Tuple<Guid, DailyTimeSlot>(userId, d));
                            CompletedMappings.Add(d);
                        }
                    }
                }

                MappingStatistics.LogMappingStat("TIMESLOT", RecordCount, "DailyTimeSlots", 0, CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error Creating TimeSlot mapping", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                Array.ForEach(tempMappings.ToArray(), a =>
                {
                    var careSetting = mu.FindPatientCareSetting(a.Item1);
                    if (careSetting != null)
                    {
                        var careId = careSetting.CareSettingsId;
                        a.Item2.CareSettingsId = careSetting.CareSettingsId;
                    }
                    else
                    {
                        tempMappings.Remove(a);
                    }

                    CompletedMappings = tempMappings.Select(s => s.Item2).ToList();
                });

                var stats = new SqlTableStats
                {
                    Tablename = "DailyTimeSlots",
                    PreSaveCount = CompletedMappings.Count()
                };

                TransactionManager.DatabaseContext.DailyTimeSlots.AddRange(CompletedMappings);
                TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = TransactionManager.DatabaseContext.DailyTimeSlots.Count();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating TimeSlot entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving TimeSlot entity", e);
            }
        }

        private bool CanAddToContext(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.InsuranceProviders.Any(a => a.Name == providerName)) ? false : true;
            }
        }
    }
}
