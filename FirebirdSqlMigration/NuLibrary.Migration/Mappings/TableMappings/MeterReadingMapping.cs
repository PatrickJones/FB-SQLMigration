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

        public void CreateDeviceMeterReadingMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                handler = new MeterReadingHandler(dataSet);

                //while (!handler.ExtractionComplete) { }

                handler.BGExtractionEvent += BGExtractionEventHandler;
                handler.NutritionExtractionEvent += NutritionExtractionEventHandler;
                handler.PumpDeliveryExtractionEvent += PumpDeliveryExtractionEventHandler;
                handler.PumpEventsExtractionEvent += PumpEventsExtractionEventHandler;
                handler.UserSettingsExtractionEvent += UserSettingsExtractionEventHandler;

                //CompletedBGMappings.AddRange(handler.BloodGlucoseReadings);
                //CompletedNutritionMappings.AddRange(handler.NutritionReadings);
                //CompletedReadingEventMappings.AddRange(handler.ReadingEvents);
                //CompletedDeviceSettingsMappings.AddRange(handler.DeviceSettings);
                //CompletedBolusMappings.AddRange(handler.BolusDeliveries);
                //CompletedBasalMappings.AddRange(handler.BasalDeliveries);
                //CompletedTDDMappings.AddRange(handler.TotalDailyInsulinDeliveries);


                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BloodGlucoseReadings", 0, CompletedBGMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "NutritionReadings", 0, CompletedNutritionMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "ReadingEvents", 0, CompletedReadingEventMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "DeviceSettings", 0, CompletedDeviceSettingsMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BolusDelivery", 0, CompletedBolusMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BasalDelivery", 0, CompletedBasalMappings.Count, FailedCount);
                //MappingStatistics.LogMappingStat("METERREADING", RecordCount, "TotalDailyInsulinDeliveries", 0, CompletedTDDMappings.Count, FailedCount);
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
                    PreSaveCount = CompletedBGMappings.Count()
                };

                TransactionManager.DatabaseContext.BloodGlucoseReadings.AddRange(CompletedBGMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedTDDMappings.Count()
                };

                TransactionManager.DatabaseContext.TotalDailyInsulinDeliveries.AddRange(CompletedTDDMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedBasalMappings.Count()
                };

                TransactionManager.DatabaseContext.BasalDeliveries.AddRange(CompletedBasalMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedDeviceSettingsMappings.Count()
                };

                TransactionManager.DatabaseContext.DeviceSettings.AddRange(CompletedDeviceSettingsMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedReadingEventMappings.Count()
                };

                TransactionManager.DatabaseContext.ReadingEvents.AddRange(CompletedReadingEventMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedNutritionMappings.Count()
                };

                TransactionManager.DatabaseContext.NutritionReadings.AddRange(CompletedNutritionMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                    PreSaveCount = CompletedBolusMappings.Count()
                };

                TransactionManager.DatabaseContext.BolusDeliveries.AddRange(CompletedBolusMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

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
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "DeviceSettings", 0, CompletedDeviceSettingsMappings.Count, FailedCount);
            }
            else
            {
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void PumpEventsExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedReadingEventMappings.AddRange(handler.ReadingEvents);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "ReadingEvents", 0, CompletedReadingEventMappings.Count, FailedCount);
            }
            else
            {
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

                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BolusDelivery", 0, CompletedBolusMappings.Count, FailedCount);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BasalDelivery", 0, CompletedBasalMappings.Count, FailedCount);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "TotalDailyInsulinDeliveries", 0, CompletedTDDMappings.Count, FailedCount);
            }
            else
            {
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void NutritionExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedNutritionMappings.AddRange(handler.NutritionReadings);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "NutritionReadings", 0, CompletedNutritionMappings.Count, FailedCount);
            }
            else
            {
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

        private void BGExtractionEventHandler(object sender, CustomEvents.MeterReadingHandlerEventArgs e)
        {
            if (handler != null && e.ExtractionSuccessful)
            {
                CompletedBGMappings.AddRange(handler.BloodGlucoseReadings);
                MappingStatistics.LogMappingStat("METERREADING", RecordCount, "BloodGlucoseReadings", 0, CompletedBGMappings.Count, FailedCount);
            }
            else
            {
                throw new Exception($"Extraction incomplete for {e.ExtractionName}");
            }
        }

    }
}
