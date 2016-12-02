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
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PumpSettingMapping : BaseMapping
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

        public void CreatePumpSettingMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = (String)row["PATIENTID"];
                var userId = aHelper.GetUserIdFromPatientId(patId);


                if (userId != Guid.Empty)
                {
                    //var PatientId = (String)row["PATIENTID"];
                    var patientPump = mu.FindPatientPump(userId);
                    var ppId = patientPump.PumpId;
                    var ips = mu.CreatePumpSetting(row, ppId).ToList();
                    foreach (var item in ips)
                    {
                        TransactionManager.DatabaseContext.PumpSettings.Add(item);
                    }

                    //***************************************Notees********************************//
                    // commented out this section because data will be taken directly from bolus reading in MeterReadingTable
                    //***************************************End Notees********************************//
                    //for (int i = 1; i < 9; i++)
                    //{
                    //    DateTime ptstart = (DateTime)row[$"TARGETBGSTART_{i}"];
                    //    DateTime ptstop = (DateTime)row[$"TARGETBGSTOP_{i}"];
                    //    var pt = new BGTarget
                    //    {
                    //        PumpId = ppId,
                    //        TargetBG = (int)row[$"TARGETBG_{i}"],
                    //        TargetBGCorrect = (int)row[$"TARGETBGCORRECT_{i}"],
                    //        TargetBGStart = new TimeSpan(ptstart.Ticks),
                    //        TargetBGStop = new TimeSpan(ptstop.Ticks)
                    //    };

                    //    DateTime icstart = (DateTime)row[$"ICSTART_{i}"];
                    //    DateTime icstop = (DateTime)row[$"ICSTOP_{i}"];
                    //    var pic = new InsulinCorrection
                    //    {
                    //        PumpId = ppId,
                    //        InsulinCorrectionStart = new TimeSpan(icstart.Ticks),
                    //        InsulinCorrectionStop = new TimeSpan(icstop.Ticks),
                    //        InsulinCorrectionValue = (int)row[$"ICVALUE_{i}"]
                    //    };

                    //    DateTime cfstart = (DateTime)row[$"CFSTART_{i}"];
                    //    DateTime cfstop = (DateTime)row[$"CFSTOP_{i}"];
                    //    var pcf = new CorrectionFactor
                    //    {
                    //        PumpId = ppId,
                    //        CorrectionFactorStart = new TimeSpan(cfstart.Ticks),
                    //        CorrectionFactorStop = new TimeSpan(cfstop.Ticks),
                    //        CorrectionFactorValue = (int)row[$"CFVALUE_{i}"]
                    //    };

                    //    TransactionManager.DatabaseContext.BGTargets.Add(pt);
                    //    TransactionManager.DatabaseContext.InsulinCorrections.Add(pic);
                    //    TransactionManager.DatabaseContext.CorrectionFactors.Add(pcf);

                    //}
                }
            }
        }
    }
}
