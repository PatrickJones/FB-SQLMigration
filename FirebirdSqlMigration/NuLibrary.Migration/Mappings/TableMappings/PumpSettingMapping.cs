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
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();

        public ICollection<PumpSetting> CompletedMappings = new List<PumpSetting>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreatePumpSettingMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;
                var dateSetter = DateTime.Now;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

                    var icDict = new Dictionary<char, ProgramTimeSlot> {
                        { '1', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '2', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '3', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '4', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '5', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '6', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '7', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '8', new ProgramTimeSlot { DateSet = dateSetter } }
                    };

                    var bgDict = new Dictionary<char, ProgramTimeSlot> {
                        { '1', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '2', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '3', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '4', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '5', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '6', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '7', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '8', new ProgramTimeSlot { DateSet = dateSetter } }
                    };

                    var bcDict = new Dictionary<char, ProgramTimeSlot> {
                        { '1', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '2', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '3', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '4', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '5', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '6', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '7', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '8', new ProgramTimeSlot { DateSet = dateSetter } }
                    };

                    var cfDict = new Dictionary<char, ProgramTimeSlot> {
                        { '1', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '2', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '3', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '4', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '5', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '6', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '7', new ProgramTimeSlot { DateSet = dateSetter } },
                        { '8', new ProgramTimeSlot { DateSet = dateSetter } }
                    };

                    PumpProgram icProgram = DefaultPumpProgram();
                    PumpProgram bgProgram = DefaultPumpProgram();
                    PumpProgram bcProgram = DefaultPumpProgram();
                    PumpProgram cfProgram = DefaultPumpProgram();

                    icProgram.ProgramName = "PROG_IC";
                    bgProgram.ProgramName = "PROG_BG";
                    bcProgram.ProgramName = "PROG_BGC";
                    cfProgram.ProgramName = "PROG_CF";

                    icProgram.ProgramTypeId = 4;
                    bgProgram.ProgramTypeId = 5;
                    bcProgram.ProgramTypeId = 7;
                    cfProgram.ProgramTypeId = 3;

                    icProgram.ProgramTimeSlots = new List<ProgramTimeSlot>();
                    bgProgram.ProgramTimeSlots = new List<ProgramTimeSlot>();
                    bcProgram.ProgramTimeSlots = new List<ProgramTimeSlot>();
                    cfProgram.ProgramTimeSlots = new List<ProgramTimeSlot>();

                    if (!mHelper.HasPatientMigrated(patId))
                    {
                        if (userId != Guid.Empty)
                        {
                            var tempList = new List<PumpSetting>();

                            // iterate through table columns and only get columns that are NOT time slots
                            // as these will be settings
                            for (int i = 0; i < row.Table.Columns.Count; i++)
                            {
                                var column = row.Table.Columns[i].ColumnName.Trim();
                                // don't want columns that start with these characters - these are timeslot values which are handled in the 'else' statement
                                var exclude = new List<bool> {
                                    column.StartsWith("ic",StringComparison.OrdinalIgnoreCase),
                                    column.StartsWith("cf",StringComparison.OrdinalIgnoreCase),
                                    column.StartsWith("target",StringComparison.OrdinalIgnoreCase),
                                    column.StartsWith("patientid", StringComparison.OrdinalIgnoreCase)
                                };
                            
                                if (exclude.All(a => !a))
                                {
                                    PumpSetting ps = new PumpSetting();
                                    ps.SettingName = column;
                                    ps.SettingValue = (row[column] is DBNull) ? String.Empty : row[column].ToString();
                                    ps.Date = new DateTime(1800, 1, 1);

                                    tempList.Add(ps);

                                    if (CanAddToContext(ps.SettingValue))
                                    {
                                        CompletedMappings.Add(ps);
                                    }
                                    else
                                    {
                                        MappingStatistics.LogFailedMapping("INSULETPUMPSETTINGS", patId, "PumpSettings", typeof(PumpSetting), JsonConvert.SerializeObject(ps), "Pump Setting has no value.");
                                        FailedCount++;
                                    }
                                }
                                else
                                {
                                    if (!(row[column] is DBNull))
                                    {
                                        if (column.StartsWith("cf", StringComparison.OrdinalIgnoreCase) && cfDict.ContainsKey(column.Last()))
                                        {
                                            var cfd = cfDict[column.Last()];
                                            if (column.StartsWith("cfstart", StringComparison.OrdinalIgnoreCase))
                                            {
                                                cfd.StartTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("cfstop", StringComparison.OrdinalIgnoreCase))
                                            {
                                                cfd.StopTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("cfvalue", StringComparison.OrdinalIgnoreCase))
                                            {
                                                cfd.Value = mu.ParseDouble(row[column].ToString());
                                            }
                                        }

                                        if (column.StartsWith("ic", StringComparison.OrdinalIgnoreCase) && icDict.ContainsKey(column.Last()))
                                        {
                                            var icd = icDict[column.Last()];
                                            if (column.StartsWith("icstart", StringComparison.OrdinalIgnoreCase))
                                            {
                                                icd.StartTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("icstop", StringComparison.OrdinalIgnoreCase))
                                            {
                                                icd.StopTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("icvalue", StringComparison.OrdinalIgnoreCase))
                                            {
                                                icd.Value = mu.ParseDouble(row[column].ToString());
                                            }
                                        }

                                        if (column.StartsWith("target", StringComparison.OrdinalIgnoreCase) && bgDict.ContainsKey(column.Last()) && bcDict.ContainsKey(column.Last()))
                                        {
                                            var bgd = bgDict[column.Last()];
                                            var bcd = bcDict[column.Last()];

                                            if (column.StartsWith("targetbgstart", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bgd.StartTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                                bcd.StartTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("targetbgstop", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bgd.StopTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                                bcd.StopTime = mu.ParseFirebirdTimespan(row[column].ToString());
                                            }

                                            if (column.StartsWith("targetbg", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bgd.Value = mu.ParseDouble(row[column].ToString());
                                            }

                                            if (column.StartsWith("targetbgcorrect", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bcd.Value = mu.ParseDouble(row[column].ToString());
                                            }
                                        }
                                    }
                                }
                            }

                            // add to Memory Mappings so that Pump object and retieve
                            // a single user should only have a single collections of PumpSettings in the FB database
                            MemoryMappings.AddPumpSetting(userId, tempList);
                        }
                    }

                    //Purge dictionaries
                    PurgeDictionary(icDict);
                    PurgeDictionary(bgDict);
                    PurgeDictionary(bcDict);
                    PurgeDictionary(cfDict);

                    // add dictionary values (ProgramTimeSlots)
                    var icCollection = AddTimeSlots(icProgram.ProgramTimeSlots.ToList(), icDict.Values);
                    foreach (var item in icCollection)
                    {
                        icProgram.ProgramTimeSlots.Add(item);
                    }

                    var bgCollection = AddTimeSlots(bgProgram.ProgramTimeSlots.ToList(), bgDict.Values);
                    foreach (var item in bgCollection)
                    {
                        bgProgram.ProgramTimeSlots.Add(item);
                    }

                    var bcCollection = AddTimeSlots(bcProgram.ProgramTimeSlots.ToList(), bcDict.Values);
                    foreach (var item in bcCollection)
                    {
                        bcProgram.ProgramTimeSlots.Add(item);
                    }

                    var cfCollection = AddTimeSlots(cfProgram.ProgramTimeSlots.ToList(), cfDict.Values);
                    foreach (var item in cfCollection)
                    {
                        cfProgram.ProgramTimeSlots.Add(item);
                    }

                    // add pump programs to memory mappings
                    AddPumpProgram(userId, icProgram);
                    AddPumpProgram(userId, bgProgram);
                    AddPumpProgram(userId, bcProgram);
                    AddPumpProgram(userId, cfProgram);
                }

                MappingStatistics.LogMappingStat("INSULETPUMPSETTINGS", RecordCount, "PumpSettings", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating PumpSetting mapping.", e);
            }
        }

        private List<ProgramTimeSlot> AddTimeSlots(List<ProgramTimeSlot> programTimeSlots, Dictionary<char, ProgramTimeSlot>.ValueCollection values)
        {
            if (values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    var ele = values.ElementAt(i);

                    if (ele.Value != 0)
                    {
                        ele.PumpProgramId = i + 1;
                        programTimeSlots.Add(ele);
                    }
                }
            }

            return programTimeSlots;
        }

        private void AddPumpProgram(Guid userId, PumpProgram program)
        {
            if (program.ProgramTimeSlots != null || program.ProgramTimeSlots.Count > 0)
            {
                program.NumOfSegments = program.ProgramTimeSlots.Count;
                MemoryMappings.AddPumpProgram(userId, 0, program);
            }
        }

        private void PurgeDictionary(Dictionary<char, ProgramTimeSlot> programDict)
        {
            var removeSet = new List<char>();

            foreach (var kv in programDict)
            {
                var prg = kv.Value;

                if (prg.Value == 0)
                {
                    removeSet.Add(kv.Key);
                }
            }

            Array.ForEach(removeSet.ToArray(), r => programDict.Remove(r));
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "PumpSettings",
                    PreSaveCount = CompletedMappings.Count()
                };

                TransactionManager.DatabaseContext.PumpSettings.AddRange(CompletedMappings);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<PumpSetting>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PumpSettings entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PumpSettings entity", e);
            }
        }

        private bool CanAddToContext(String value)
        {
            // don't add empty settings to database (context)
            return (String.IsNullOrEmpty(value)) ? false : true;
        }

        private PumpProgram DefaultPumpProgram()
        {
            var p = new PumpProgram();
            p.CreationDate = DateTime.Now;
            p.Source = String.Empty;
            p.Valid = true;
            p.ProgramKey = 0;
            p.NumOfSegments = 8;
            p.ProgramName = String.Empty;
            p.IsEnabled = true;

            return p;
        }
    }
}
