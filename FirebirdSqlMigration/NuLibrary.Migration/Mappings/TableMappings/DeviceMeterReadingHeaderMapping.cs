﻿using Newtonsoft.Json;
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
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();

        public ICollection<PatientDevice> CompletedMappings = new List<PatientDevice>();
        public ICollection<PumpSetting> CompletedPumpSettingMappings = new List<PumpSetting>();
        public ICollection<PumpProgram> CompletedPumpProgramMappings = new List<PumpProgram>();
        public ICollection<ProgramTimeSlot> CompletedBasalProgramTimeSlots = new List<ProgramTimeSlot>();
        public ICollection<ProgramTimeSlot> CompletedBolusProgramTimeSlots = new List<ProgramTimeSlot>();

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

                    if (!mHelper.HasPatientMigrated(patId))
                    {
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
                                                PumpKeyId = mrh.Pump.PumpKeyId,
                                                IsEnabled = p.IsEnabled,
                                                ProgramTypeId = p.ProgramTypeId
                                            };

                                            if (p.ProgramTimeSlots != null && p.ProgramTimeSlots.Count != 0)
                                            {
                                                Array.ForEach(p.ProgramTimeSlots.ToArray(), a =>
                                                {
                                                    prog.ProgramTimeSlots.Add(a);
                                                });
                                            }

                                            CompletedPumpProgramMappings.Add(prog);

                                            //if (p.BolusProgramTimeSlots != null && p.BolusProgramTimeSlots.Count != 0)
                                            //{
                                            //    Array.ForEach(p.BolusProgramTimeSlots.ToArray(), r =>
                                            //    {
                                            //        prog.BolusProgramTimeSlots.Add(r);
                                            //    });
                                            //}
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

                            bool alreadyMapped = false;

                            if (CompletedMappings.Any(a => a.SerialNumber == dev.SerialNumber))
                            {
                                alreadyMapped = true;

                                var device = CompletedMappings.Where(w => w.SerialNumber == dev.SerialNumber).FirstOrDefault();
                                device.ReadingHeaders.Add(mrh);
                            }
                            else
                            {
                                dev.ReadingHeaders.Add(mrh);
                            }

                            if (CanAddToContext(dev.UserId, dev.SerialNumber) && !alreadyMapped)
                            {
                                if (!CompletedMappings.Any(a => a.SerialNumber == dev.SerialNumber))
                                {
                                    CompletedMappings.Add(dev);
                                }

                                MemoryMappings.AddReadingHeaderkeyId(mrh.LegacyDownloadKeyId.Trim(), mrh.ReadingKeyId);
                            }
                            else
                            {
                                if (alreadyMapped && !String.IsNullOrEmpty(dev.SerialNumber))
                                {
                                    MemoryMappings.AddReadingHeaderkeyId(mrh.LegacyDownloadKeyId.Trim(), mrh.ReadingKeyId);
                                }

                                var fr = (dev.UserId == Guid.Empty) ? "Device has no corresponding user." : (String.IsNullOrEmpty(dev.SerialNumber)) ? "Device has no serial number recorded." : "Device already assigned to user.";

                                MappingStatistics.LogFailedMapping("METERREADERHEADER", row["DOWNLOADKEYID"].ToString(), "PatientDevices", typeof(PatientDevice), JsonConvert.SerializeObject(dev), fr);
                                FailedCount++;
                            }
                        }

                    }

                }

                MappingStatistics.LogMappingStat("METERREADINGHEADER", RecordCount, "PatientDevices", CompletedMappings.Count, FailedCount);
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
                var stats = new SqlTableStats("PatientDevices");
                var rhStats = new SqlTableStats("ReadingHeaders");
                var dmStats = new SqlTableStats("DiabetesManagementData");
                var pmpstats = new SqlTableStats("Pumps");

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.PatientDevices.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<PatientDevice>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                rhStats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<ReadingHeader>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                dmStats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<DiabetesManagementData>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                pmpstats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<Pump>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();

                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;
                rhStats.PostSaveCount = (saved > rhStats.PreSaveCount) ? rhStats.PreSaveCount : saved;
                dmStats.PostSaveCount = (saved > dmStats.PreSaveCount) ? dmStats.PreSaveCount : saved;
                pmpstats.PostSaveCount = (saved > pmpstats.PreSaveCount) ? pmpstats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                MappingStatistics.SqlTableStatistics.Add(rhStats);
                MappingStatistics.SqlTableStatistics.Add(dmStats);
                MappingStatistics.SqlTableStatistics.Add(pmpstats);
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
                    var stats = new SqlTableStats("PumpSettings");

                    ctx.PumpSettings.AddRange(CompletedPumpSettingMappings);
                    stats.PreSaveCount = ctx.ChangeTracker.Entries<PumpSetting>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();

                    var saved = ctx.SaveChanges();
                    stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

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
                    var stats = new SqlTableStats("PumpPrograms");
                    var basalPrgStats = new SqlTableStats("BasalProgramTimeSlots");
                    var bolusPrgStats = new SqlTableStats("BolusProgamTimeSlots");

                    ctx.PumpPrograms.AddRange(CompletedPumpProgramMappings);
                    stats.PreSaveCount = ctx.ChangeTracker.Entries<PumpProgram>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();

                    var pt = ctx.ChangeTracker.Entries<ProgramTimeSlot>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();

                    basalPrgStats.PreSaveCount = pt;
                    bolusPrgStats.PreSaveCount = pt;

                    var saved = ctx.SaveChanges();
                    stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;
                    basalPrgStats.PostSaveCount = (saved > basalPrgStats.PreSaveCount) ? basalPrgStats.PreSaveCount : saved;
                    bolusPrgStats.PostSaveCount = (saved > bolusPrgStats.PreSaveCount) ? bolusPrgStats.PreSaveCount : saved;

                    MappingStatistics.SqlTableStatistics.Add(stats);
                    MappingStatistics.SqlTableStatistics.Add(basalPrgStats);
                    MappingStatistics.SqlTableStatistics.Add(bolusPrgStats);
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
