using NuLibrary.Migration.FBDatabase.FBTables;
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
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();


        public void CreatePatientPhoneNumbersMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = row["PATIENTID"].ToString();
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var patNum = new PatientPhoneNumber
                    {
                        UserId = userId,
                        Number = (row["NUMBER"] is DBNull) ? String.Empty : row["NUMBER"].ToString(),
                        Extension = (row["EXTENSION"] is DBNull) ? String.Empty : row["EXTENSION"].ToString(),
                        Type = (row["ATYPE"] is DBNull) ? 0 : map.ParseFirebirdPhoneTypes(row["ATYPE"].ToString()),
                        IsPrimary = (row["ISPRIMARY"] is DBNull) ? false : map.ParseFirebirdBoolean(row["ISPRIMARY"].ToString()),
                        RecieveText = (row["RECEIVETEXT"] is DBNull) ? false : map.ParseFirebirdBoolean(row["RECEIVETEXT"].ToString())
                    };

                    TransactionManager.DatabaseContext.PatientPhoneNumbers.Add(patNum);
                }
                
            }
        }

    }
}
