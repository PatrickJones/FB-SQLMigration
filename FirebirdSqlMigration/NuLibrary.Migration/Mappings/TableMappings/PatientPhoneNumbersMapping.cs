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
    /// Note: Has relationship with Patients Table 1:M
    /// </summary>
    public class PatientPhoneNumbersMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PatientPhoneNumbersMapping() : base("PATIENTPHONENUMBERS")
        {

        }

        public PatientPhoneNumbersMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePatientPhoneNumbersMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var patNum = new PatientPhoneNumber
                {
                    PhoneId = (Int32)row["KEYID"],
                    Number = (String)row["NUMBER"],
                    Extension = (String)row["EXTENSION"],
                    Type = (Int32)row["ATYPE"],
                    IsPrimary = (Boolean)row["ISPRIMARY"],
                    RecieveText = (Boolean)row["RECEIVETEXT"]
                };

                TransactionManager.DatabaseContext.PatientPhoneNumbers.Add(patNum);
            }
        }

    }
}
