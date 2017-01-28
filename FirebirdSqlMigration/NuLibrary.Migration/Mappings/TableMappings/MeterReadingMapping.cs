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

                MeterReadingHandler handler = new MeterReadingHandler(dataSet);

                CompletedBGMappings.AddRange(handler.BloodGlucoseReadings);
                CompletedNutritionMappings.AddRange(handler.NutritionReadings);
                CompletedReadingEventMappings.AddRange(handler.ReadingEvents);
                CompletedDeviceSettingsMappings.AddRange(handler.DeviceSettings);
                CompletedBolusMappings.AddRange(handler.BolusDeliveries);
                CompletedBasalMappings.AddRange(handler.BasalDeliveries);
                CompletedTDDMappings.AddRange(handler.TotalDailyInsulinDeliveries);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating MeterReading mapping.", e);
            }
        }

        public void AddToContext()
        {
            TransactionManager.DatabaseContext.BolusDeliveries.AddRange(CompletedBolusMappings);
            TransactionManager.DatabaseContext.BloodGlucoseReadings.AddRange(CompletedBGMappings);
            TransactionManager.DatabaseContext.NutritionReadings.AddRange(CompletedNutritionMappings);
            TransactionManager.DatabaseContext.ReadingEvents.AddRange(CompletedReadingEventMappings);
            TransactionManager.DatabaseContext.DeviceSettings.AddRange(CompletedDeviceSettingsMappings);
            TransactionManager.DatabaseContext.BasalDeliveries.AddRange(CompletedBasalMappings);
            TransactionManager.DatabaseContext.TotalDailyInsulinDeliveries.AddRange(CompletedTDDMappings);
        }
        

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception("Error validating meter reading mapped entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving meter reading mapped entity", e);
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
