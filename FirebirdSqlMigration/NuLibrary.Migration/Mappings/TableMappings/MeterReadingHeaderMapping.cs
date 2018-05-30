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
    public class MeterReadingHeaderMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public MeterReadingHeaderMapping() : base("METERREADINGHEADER")
        {

        }

        public MeterReadingHeaderMapping(string tableName) :base(tableName)
        {

        }

        public void CreateMeterReadingHeaderMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var mrh = new MeterReadingHeader
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

                TransactionManager.DatabaseContext.MeterReadingHeaders.Add(mrh);
            }
        }
    }
}
