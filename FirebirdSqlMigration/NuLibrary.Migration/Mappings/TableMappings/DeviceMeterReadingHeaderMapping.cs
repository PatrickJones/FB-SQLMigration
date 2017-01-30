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

                        var dev = new PatientDevice
                        {
                            UserId = userId,
                            MeterIndex = (row["NUMEDICSMETERINDEX"] is DBNull) ? 0 : (Int32)row["NUMEDICSMETERINDEX"],
                            Manufacturer = (row["MANUFACTURER"] is DBNull) ? String.Empty : row["MANUFACTURER"].ToString(),
                            DeviceModel = (row["METERMODEL"] is DBNull) ? String.Empty : row["METERMODEL"].ToString(),
                            DeviceName = (row["METERNAME"] is DBNull) ? String.Empty : row["METERNAME"].ToString(),
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
                            ReviewedOn = (row["REVIEWEDON"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["REVIEWEDON"].ToString())
                        };

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
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = FbTableName,
                                    ObjectType = typeof(PatientDevice),
                                    JsonSerializedObject = JsonConvert.SerializeObject(dev),
                                    FailedReason = "Unable to add Patient Device to database."
                                });

                            FailedCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Patient Device (MeterReadingHeader) mapping.", e);
            }
        }

        public void AddToContext()
        {
            TransactionManager.DatabaseContext.PatientDevices.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PatientDevice entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PatientDevice entity", e);
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
