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
    public class DeviceMeterReadingHeaderMapping : BaseMapping
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

        public void CreateDeviceMeterReadingHeaderMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = (String)row["PATIENTID"];
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var dev = new PatientDevice
                    {
                        UserId = userId,
                        MeterIndex = (int)row["NUMEDICSMETERINDEX"],
                        Manufacturer = (String)row["MANUFACTURER"],
                        DeviceModel = (String)row["METERMODEL"],
                        DeviceName = (String)row["METERNAME"],
                        SerialNumber = (String)row["SERIALNUMBER"],
                        SoftwareVersion = (String)row["SOFTWAREVERSION"],
                        HardwareVersion = (String)row["HARDWAREVERSION"]
                    };

                    var mrh = new ReadingHeader
                    {
                        UserId = userId,
                        DownloadKeyId = (int)row["DOWNLOADKEYID"],
                        DeviceId = (int)row[dev.DeviceId],
                        ServerDateTime = (DateTime)row["SERVERDATETIME"],
                        MeterDateTime = (DateTime)row["METERDATETIME"],
                        Readings = (int)row["READINGS"],
                        SiteSource = (String)row["SOURCE"],
                        ReviewedOn = (DateTime)row["REVIEWEDON"]
                    };



                    var pat = mu.FindPatient(userId);
                    pat.PatientDevices.Add(dev);

                    TransactionManager.DatabaseContext.PatientDevices.Add(dev);
                    TransactionManager.DatabaseContext.ReadingHeaders.Add(mrh);
                    TransactionManager.DatabaseContext.Patients.Add(pat);

                }
            }
        }
    }
}
