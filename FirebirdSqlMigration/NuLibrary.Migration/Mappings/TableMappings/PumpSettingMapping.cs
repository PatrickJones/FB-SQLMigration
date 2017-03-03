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
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PumpSettingMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PumpSettingMapping() : base("INSULETPUMPSETTINGS")
        {

        }

        public PumpSettingMapping(string tableName) : base(tableName)
        {

        }

        MappingUtilities mu = new MappingUtilities();
        AspnetDbHelpers aHelper = new AspnetDbHelpers();

        public ICollection<PumpSetting> CompletedMappings = new List<PumpSetting>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreatePumpSettingMapping()
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
                        var tempList = new List<PumpSetting>();

                        // iterate through table columns and only get columns that are NOT time slots
                        // as these will be settings
                        for (int i = 0; i < row.Table.Columns.Count; i++)
                        {
                            var column = row.Table.Columns[i].ColumnName.Trim();

                            // don't want columns that start with these characters - these are timeslot values
                            if ((column.ToLower().Substring(0,2) != "IC") || (column.ToLower().Substring(0, 2) != "CF") || (column.ToLower().Substring(0, 6) != "TARGET"))
                            {
                                PumpSetting ps = new PumpSetting();
                                ps.SettingName = column;
                                ps.SettingValue = (row[column] is DBNull) ? String.Empty : row[column].ToString();
                                ps.Date = new DateTime(1800, 1, 1);

                                tempList.Add(ps);

                                if (CanAddToContext(ps.SettingValue))
                                {
                                    //TransactionManager.DatabaseContext.Patients.Add(pat);
                                    CompletedMappings.Add(ps);
                                }
                                else
                                {
                                    TransactionManager.FailedMappingCollection
                                        .Add(new FailedMappings
                                        {
                                            Tablename = "PumpSettings",
                                            ObjectType = typeof(PumpSetting),
                                            JsonSerializedObject = JsonConvert.SerializeObject(ps),
                                            FailedReason = "Pump Setting has no value."
                                        });

                                    FailedCount++;
                                }
                            }
                        }

                        // add to Memory Mappings so that Pump object and retieve
                        // a single user should only have a single collections of PumpSettings in the FB database
                        MemoryMappings.AddPumpSetting(userId, tempList);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating PumpSetting mapping.", e);
            }
        }

        public void AddToContext()
        {
            //TransactionManager.DatabaseContext.PumpSettings.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.PumpSettings.AddRange(CompletedMappings);
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PumpSetting entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PumpSetting entity", e);
            }
        }

        private bool CanAddToContext(String value)
        {
            // don't add empty settings to database (context)
            return (String.IsNullOrEmpty(value)) ? false : true;
        }
    }
}
