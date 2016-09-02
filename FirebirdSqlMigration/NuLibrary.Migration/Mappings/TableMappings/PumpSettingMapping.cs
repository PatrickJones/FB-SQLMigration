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
        public PumpSettingMapping() : base("PUMPSETTING")
        {

        }

        public PumpSettingMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePumpSettingMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var ps = new PumpSetting
                {
                    //PatientId = (String)row["KEYID"],
                    //MRID = (String)row["MEDICALRECORDIDENTIFIER"],
                    //Firstname = (String)row["FIRSTNAME"],
                    //Lastname = (String)row["LASTNAME"],
                    //Middlename = (String)row["MIDDLENAME"],
                    //Suffix = (String)row["SUFFIX"],
                    //Gender = (Int32)row["GENDER"],
                    //DateofBirth = (DateTime)row["DOB"]
                };

                

                TransactionManager.DatabaseContext.PumpSettings.Add(ps);
            }
        }
    }
}
