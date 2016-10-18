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

        public PumpSettingMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePumpSettingMapping()
        {
            MappingUtilities mu = new MappingUtilities();
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var PatientId = (String)row["PATIENTID"];
                var patientPump = mu.FindPatientPump(PatientId);
                var ppId = patientPump.PumpId;
                var ips = mu.CreatePumpSetting(row, ppId).ToList();
                foreach (var item in ips)
                {
                    TransactionManager.DatabaseContext.PumpSettings.Add(item);
                }

                for (int i = 1; i < 9; i++)
                {
                    DateTime ptstart = (DateTime)row[$"TARGETBGSTART_{i}"];
                    DateTime ptstop = (DateTime)row[$"TARGETBGSTOP_{i}"];
                    var pt = new PumpBGTarget
                    {
                        PumpId = ppId,
                        TargetBG = (int)row[$"TARGETBG_{i}"],
                        TargetBGCorrect = (int)row[$"TARGETBGCORRECT_{i}"],
                        TargetBGStart = new TimeSpan(ptstart.Ticks),
                        TargetBGStop = new TimeSpan(ptstop.Ticks)
                    };
                    
                    DateTime icstart = (DateTime)row[$"ICSTART_{i}"];
                    DateTime icstop = (DateTime)row[$"ICSTOP_{i}"];
                    var pic = new PumpInsulinCorrection
                    {
                        PumpId = ppId,
                        InsulinCorrectionStart = new TimeSpan(icstart.Ticks),
                        InsulinCorrectionStop = new TimeSpan(icstop.Ticks),
                        InsulinCorrectionValue = (int)row[$"ICVALUE_{i}"]
                    };

                    DateTime cfstart = (DateTime)row[$"CFSTART_{i}"];
                    DateTime cfstop = (DateTime)row[$"CFSTOP_{i}"];
                    var pcf = new PumpCorrectionFactor
                    {
                        PumpId = ppId,
                        CorrectionFactorStart = new TimeSpan(cfstart.Ticks),
                        CorrectionFactorStop = new TimeSpan(cfstop.Ticks),
                        CorrectionFactorValue = (int)row[$"CFVALUE_{i}"]
                    };

                    TransactionManager.DatabaseContext.PumpBGTargets.Add(pt);
                    TransactionManager.DatabaseContext.PumpInsulinCorrections.Add(pic);
                    TransactionManager.DatabaseContext.PumpCorrectionFactors.Add(pcf);
                }
            }
        }
    }
}
