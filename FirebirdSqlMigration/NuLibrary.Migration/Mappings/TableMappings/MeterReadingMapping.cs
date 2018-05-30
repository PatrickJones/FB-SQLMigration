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
    public class MeterReadingMapping : BaseMapping, IContextHandler
    {
        public MeterReadingMapping() : base("METERREADING")
        {

        }

        public MeterReadingMapping(string tableName) : base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();
        MappingUtilities mu = new MappingUtilities();
        MeterReadingHandler handler;

        public List<BloodGlucoseReading> CompletedBGMappings = new List<BloodGlucoseReading>();
        public List<NutritionReading> CompletedNutritionMappings = new List<NutritionReading>();
        public List<ReadingEvent> CompletedReadingEventMappings = new List<ReadingEvent>();
        public List<DeviceSetting> CompletedDeviceSettingsMappings = new List<DeviceSetting>();
        public List<BolusDelivery> CompletedBolusMappings = new List<BolusDelivery>();
        public List<BasalDelivery> CompletedBasalMappings = new List<BasalDelivery>();
        public List<TotalDailyInsulinDelivery> CompletedTDDMappings = new List<TotalDailyInsulinDelivery>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public bool BgExtractionComplete = false;
        public bool NutritionExtractionComplete = false;
        public bool PumpDeliveryExtractionComplete = false;
        public bool PumpEventsExtractionComplete = false;
        public bool UserSettingsExtractionComplete = false;

        public bool AllExtractionsComplete
        {
            get
            {
                return (BgExtractionComplete && NutritionExtractionComplete && PumpDeliveryExtractionComplete && PumpEventsExtractionComplete && UserSettingsExtractionComplete) ? true : false;
            }
        }

        public void CreateDeviceMeterReadingMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                 RecordCount = TableAgent.RowCount;

                DataRow[] rowArray = new DataRow[dataSet.Count];
                dataSet.CopyTo(rowArray, 0);

                Array.ForEach(rowArray, row => {
                    var patId = row["PATIENTKEYID"].ToString();
                    if (mHelper.HasPatientMigrated(patId))
                    {
                        dataSet.Remove(row);
                    }
                });

                handler = new MeterReadingHandler(dataSet);

                handler.BGExtractionEvent += BGExtractionEventHandler;
                handler.NutritionExtractionEvent += NutritionExtractionEventHandler;
                handler.PumpDeliveryExtractionEvent += PumpDeliveryExtractionEventHandler;
                handler.PumpEventsExtractionEvent += PumpEventsExtractionEventHandler;
                handler.UserSettingsExtractionEvent += UserSettingsExtractionEventHandler;
            }
            catch (Exception e)
            {
                throw new Exception("Error creating MeterReading mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "BloodGlucoseReadings",
                    PreSaveCount = CompletedBGMappings.Count
                };

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedBGMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm; 

                TransactionManager.DatabaseContext.BloodGlucoseReadings.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<BloodGlucoseReading>()
                    .Where(w => w.State == System.Data.Entity.EntityState.Added)
                    .Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveTotalDailyInsulinDeliveries();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Blood Glucose Reading mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Blood Glucose Reading mapped entity", e);
            }
        }

        private void SaveTotalDailyInsulinDeliveries()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "TotalDailyInsulinDeliveries",
                    PreSaveCount = CompletedTDDMappings.Count
                };

                                var q = from cm in CompletedTDDMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.TotalDailyInsulinDeliveries.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries <TotalDailyInsulinDelivery>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveBasalDeliveries();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Total Daily Insulin Deliveries entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Total Daily Insulin Deliveries mapped entity", e);
            }
        }

        private void SaveBasalDeliveries()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "BasalDeliveries",
                    PreSaveCount = CompletedBasalMappings.Count
                };

                var q = from cm in CompletedBasalMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.BasalDeliveries.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<BasalDelivery>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveDeviceSettings();

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Basal Deliveries mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Basal Deliveries mapped entity", e);
            }
        }

        private void SaveDeviceSettings()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "DeviceSettings",
                    PreSaveCount = CompletedDeviceSettingsMappings.Count
                };

                var q = from cm in CompletedDeviceSettingsMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.DeviceSettings.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<DeviceSetting>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveReadingEvents();

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Device Settings mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Device Settings mapped entity", e);
            }
        }

        private void SaveReadingEvents()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "ReadingEvents",
                    PreSaveCount = CompletedReadingEventMappings.Count
                };

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedReadingEventMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.ReadingEvents.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<ReadingEvent>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveNutritionReadings();

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Reading Events mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Reading Events mapped entity", e);
            }
        }

        private void SaveNutritionReadings()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "NutritionReadings",
                    PreSaveCount = CompletedNutritionMappings.Count
                };

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedNutritionMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.NutritionReadings.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<NutritionReading>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
                SaveBolusDeliveries();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Nutrition Reading mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Nutrition Reading mapped entity", e);
            }
        } 

        private void SaveBolusDeliveries()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "BolusDeliveries",
                    PreSaveCount = CompletedBolusMappings.Count
                };

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedBolusMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.BolusDeliveries.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<BolusDelivery>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                var saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = (saved > stats.PreSaveCount) ? stats.PreSaveCount : saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating Bolus Delivery mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Bolus Delivery mapped entity", e);
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

        private void UserSettingsExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedDeviceSettingsMappings.AddRange(handler.DeviceSettings);
                var fc = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(DeviceSetting)).Count();
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "DeviceSettings", CompletedDeviceSettingsMappings.Count, fc);

                UserSettingsExtractionComplete = true;
            }
            else
            {
                UserSettingsExtractionComplete = true;
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void PumpEventsExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedReadingEventMappings.AddRange(handler.ReadingEvents);
                var fc = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(ReadingEvent)).Count();
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "ReadingEvents", CompletedReadingEventMappings.Count, fc);

                PumpEventsExtractionComplete = true;
            }
            else
            {
                PumpEventsExtractionComplete = true;
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void PumpDeliveryExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedBolusMappings.AddRange(handler.BolusDeliveries);
                CompletedBasalMappings.AddRange(handler.BasalDeliveries);
                CompletedTDDMappings.AddRange(handler.TotalDailyInsulinDeliveries);

                var fcBol = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(BolusDelivery)).Count();
                var fcBas = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(BasalDelivery)).Count();
                var fcTdd = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(TotalDailyInsulinDelivery)).Count();

                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BolusDelivery", CompletedBolusMappings.Count, fcBol);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BasalDelivery", CompletedBasalMappings.Count, fcBas);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "TotalDailyInsulinDeliveries", CompletedTDDMappings.Count, fcTdd);

                PumpDeliveryExtractionComplete = true;
            }
            else
            {
                PumpDeliveryExtractionComplete = true;
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void NutritionExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedNutritionMappings.AddRange(handler.NutritionReadings);
                var fc = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(NutritionReading)).Count();
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "NutritionReadings", CompletedNutritionMappings.Count, fc);

                NutritionExtractionComplete = true;
            }
            else
            {
                NutritionExtractionComplete = true;
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void BGExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedBGMappings.AddRange(handler.BloodGlucoseReadings);
                var fc = MappingStatistics.FailedMappingCollection.Where(w => w.ObjectType == typeof(BloodGlucoseReading)).Count();
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BloodGlucoseReadings", CompletedBGMappings.Count, fc);

                BgExtractionComplete = true;
            }
            else
            {
                BgExtractionComplete = true;
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }
    }
}
