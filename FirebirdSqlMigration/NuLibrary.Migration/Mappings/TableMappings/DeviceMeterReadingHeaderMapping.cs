using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class DeviceMeterReadingHeaderMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public DeviceMeterReadingHeaderMapping() : base("METERREADINGHEADER")
        {

        }

        public DeviceMeterReadingHeaderMapping(string tableName) : base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        MappingUtilities mu = new MappingUtilities();

        public ICollection<PatientDevice> CompletedMappings = new List<PatientDevice>();
        public ICollection<PumpSetting> CompletedPumpSettingMappings = new List<PumpSetting>();
        public ICollection<PumpProgram> CompletedPumpProgramMappings = new List<PumpProgram>();
        public ICollection<BasalProgramTimeSlot> CompletedBasalProgramTimeSlots = new List<BasalProgramTimeSlot>();
        public ICollection<BolusProgramTimeSlot> CompletedBolusProgramTimeSlots = new List<BolusProgramTimeSlot>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreateDeviceMeterReadingHeaderMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTKEYID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

                    if (userId != Guid.Empty)
                    {
                        var dmData = MemoryMappings.GetAllDiabetesManagementData().Where(w => w.UserId == userId).FirstOrDefault();
                        var meterName = (row["METERNAME"] is DBNull) ? String.Empty : row["METERNAME"].ToString();

                        var dev = new PatientDevice
                        {
                            UserId = userId,
                            MeterIndex = (row["NUMEDICSMETERINDEX"] is DBNull) ? 0 : (Int32)row["NUMEDICSMETERINDEX"],
                            Manufacturer = (row["MANUFACTURER"] is DBNull) ? String.Empty : row["MANUFACTURER"].ToString(),
                            DeviceModel = (row["METERMODEL"] is DBNull) ? String.Empty : row["METERMODEL"].ToString(),
                            DeviceName = meterName,
                            SerialNumber = (row["SERIALNUMBER"] is DBNull) ? string.Empty : row["SERIALNUMBER"].ToString(),
                            SoftwareVersion = (row["SOFTWAREVERSION"] is DBNull) ? String.Empty : row["SOFTWAREVERSION"].ToString(),
                            HardwareVersion = (row["HARDWAREVERSION"] is DBNull) ? String.Empty : row["HARDWAREVERSION"].ToString()
                        };

                        if (dmData != null)
                        {
                            dev.DiabetesManagementData = dmData;
                        }

                        var mrh = new ReadingHeader
                        {
                            ReadingKeyId = Guid.NewGuid(),
                            UserId = userId,
                            LegacyDownloadKeyId = (row["DOWNLOADKEYID"] is DBNull) ? String.Empty : row["DOWNLOADKEYID"].ToString(),
                            ServerDateTime = (row["SERVERDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["SERVERDATETIME"].ToString()),
                            MeterDateTime = (row["METERDATETIME"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["METERDATETIME"].ToString()),
                            Readings = (row["READINGS"] is DBNull) ? 0 : (Int32)row["READINGS"],
                            SiteSource = (row["SOURCE"] is DBNull) ? String.Empty : row["SOURCE"].ToString(),
                            ReviewedOn = (row["REVIEWEDON"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["REVIEWEDON"].ToString()),
                            //Pump = (meterName.ToLower().Contains("omnipod")) ? MemoryMappings.GetAllPump().Where(w => w.UserId == userId).FirstOrDefault() : null
                        };

                        if (meterName.ToLower().Contains("omnipod"))
                        {
                            var ePump = MemoryMappings.GetAllPump().Where(w => w.UserId == userId).FirstOrDefault();

                            if (ePump != null)
                            {
                                mrh.Pump = new Pump();

                                mrh.Pump.PumpKeyId = mrh.ReadingKeyId;


                                if (ePump.PumpPrograms != null)
                                {
                                    mrh.Pump.PumpPrograms = new List<PumpProgram>();
                                    Array.ForEach(ePump.PumpPrograms.ToArray(), p =>
                                    {
                                        var prog = new PumpProgram
                                        {
                                            CreationDate = p.CreationDate,
                                            NumOfSegments = p.NumOfSegments,
                                            ProgramKey = p.ProgramKey,
                                            ProgramName = p.ProgramName,
                                            Source = p.Source,
                                            Valid = p.Valid,
                                            PumpKeyId = mrh.Pump.PumpKeyId
                                        };

                                        CompletedPumpProgramMappings.Add(prog);

                                        if (p.BasalProgramTimeSlots != null && p.BasalProgramTimeSlots.Count != 0)
                                        {
                                            Array.ForEach(p.BasalProgramTimeSlots.ToArray(), a => {
                                                prog.BasalProgramTimeSlots.Add(a);
                                                //CompletedBasalProgramTimeSlots.Add(a);
                                            });
                                        }

                                        if (p.BolusProgramTimeSlots != null && p.BolusProgramTimeSlots.Count != 0)
                                        {
                                            Array.ForEach(p.BolusProgramTimeSlots.ToArray(), r => {
                                                prog.BolusProgramTimeSlots.Add(r);
                                                //CompletedBolusProgramTimeSlots.Add(r);
                                            });
                                        }
                                    });
                                }

                                if (ePump.PumpSettings != null)
                                {
                                    mrh.Pump.PumpSettings = new List<PumpSetting>();
                                    Array.ForEach(ePump.PumpSettings.ToArray(), s =>
                                    {
                                        var ps = new PumpSetting
                                        {
                                            Date = s.Date,
                                            Description = s.Description,
                                            SettingName = s.SettingName,
                                            SettingValue = s.SettingValue,
                                            PumpKeyId = mrh.Pump.PumpKeyId
                                        };

                                        //mrh.Pump.PumpSettings.Add(ps);
                                        CompletedPumpSettingMappings.Add(ps);
                                    });
                                }

                                mrh.Pump.ActiveProgramId = ePump.ActiveProgramId;
                                mrh.Pump.Cannula = ePump.Cannula;
                                mrh.Pump.Notes = ePump.Notes;
                                mrh.Pump.PumpInfusionSet = ePump.PumpInfusionSet;
                                mrh.Pump.PumpName = ePump.PumpName;
                                mrh.Pump.PumpStartDate = ePump.PumpStartDate;
                                mrh.Pump.PumpType = ePump.PumpType;
                                mrh.Pump.ReplacementDate = ePump.ReplacementDate;
                                mrh.Pump.UserId = ePump.UserId;
                            }
                        }

                        if (CompletedMappings.Any(a => a.SerialNumber == dev.SerialNumber))
                        {
                            var device = CompletedMappings.Where(w => w.SerialNumber == dev.SerialNumber).FirstOrDefault();
                            device.ReadingHeaders.Add(mrh);
                        }
                        else
                        {
                            dev.ReadingHeaders.Add(mrh);
                        }

                        MemoryMappings.AddReadingHeaderkeyId(mrh.LegacyDownloadKeyId.Trim(), mrh.ReadingKeyId);

                        if (CanAddToContext(dev.UserId, dev.SerialNumber))
                        {
                            if (!CompletedMappings.Any(a => a.SerialNumber == dev.SerialNumber))
                            {
                                CompletedMappings.Add(dev);
                            }
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("PatientDevices", typeof(PatientDevice), JsonConvert.SerializeObject(dev), "Unable to add Patient Device to database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("METERREADINGHEADER", RecordCount, "PatientDevices", 0, CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Patient Device (MeterReadingHeader) mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "PatientDevices",
                    PreSaveCount = CompletedMappings.Count()
                };

                TransactionManager.DatabaseContext.PatientDevices.AddRange(CompletedMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PatientDevice entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PatientDevice entity", e);
            }

            SaveCompletedPumpSettingMappings();
        }

        private void SaveCompletedPumpSettingMappings()
        {
            try
            {
                using (var ctx = new NuMedicsGlobalEntities())
                {
                    var stats = new SqlTableStats
                    {
                        Tablename = "PumpSettings",
                        PreSaveCount = CompletedPumpSettingMappings.Count()
                    };

                    ctx.PumpSettings.AddRange(CompletedPumpSettingMappings);
                    int saved = ctx.SaveChanges();
                    stats.PostSaveCount = saved;

                    MappingStatistics.SqlTableStatistics.Add(stats);
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PumpSetting entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PumpSetting entity", e);
            }

            SaveCompletedPumpProgramMappings();
        }

        private void SaveCompletedPumpProgramMappings()
        {
            try
            {
                using (var ctx = new NuMedicsGlobalEntities())
                {
                    var stats = new SqlTableStats
                    {
                        Tablename = "PumpPrograms",
                        PreSaveCount = CompletedPumpProgramMappings.Count
                    };

                    ctx.PumpPrograms.AddRange(CompletedPumpProgramMappings);
                    int saved = ctx.SaveChanges();
                    stats.PostSaveCount = saved;

                    MappingStatistics.SqlTableStatistics.Add(stats);
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PumpProgram entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PumpProgram entity", e);
            }
        }

        private bool CanAddToContext(Guid userId, string serialNumber)
        {
            if (String.IsNullOrEmpty(serialNumber))
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.PatientDevices.Any(a => a.UserId == userId && a.SerialNumber == serialNumber)) ? false : true;
            }
        }
    }
}
