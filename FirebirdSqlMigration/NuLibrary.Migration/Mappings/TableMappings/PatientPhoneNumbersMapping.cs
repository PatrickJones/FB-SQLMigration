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
        public PatientPhoneNumbersMapping() : base("PHONENUMBERS")
        {

        }

        public PatientPhoneNumbersMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        MappingUtilities map = new MappingUtilities();

        public void CreatePatientPhoneNumbersMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = (String)row["PATIENTID"];
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var patNum = new PatientPhoneNumber
                    {
                        PhoneId = (Int32)row["KEYID"],
                        UserId = userId,
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
}
