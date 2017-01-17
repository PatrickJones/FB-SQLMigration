using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
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
            try
            {
                var rows = TableAgent.DataSet.Tables[FbTableName].Rows;

                foreach (DataRow row in rows)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PARENTID"].ToString();
                    var userId = MemoryPatientInfo.GetUserId(MigrationVariables.CurrentSiteId, patId);

                    var patNum = new PatientPhoneNumber
                    {
                        UserId = userId,
                        Number = (row["NUMBER"] is DBNull) ? String.Empty : row["NUMBER"].ToString(),
                        Extension = (row["EXTENSION"] is DBNull) ? String.Empty : row["EXTENSION"].ToString(),
                        Type = (row["ATYPE"] is DBNull) ? 0 : map.ParseFirebirdPhoneTypes(row["ATYPE"].ToString()),
                        IsPrimary = (row["ISPRIMARY"] is DBNull) ? false : map.ParseFirebirdBoolean(row["ISPRIMARY"].ToString()),
                        RecieveText = (row["RECEIVETEXT"] is DBNull) ? false : map.ParseFirebirdBoolean(row["RECEIVETEXT"].ToString())
                    };

                    if (userId != Guid.Empty && CanAddToContext(patNum.Number))
                    {
                        TransactionManager.DatabaseContext.PatientPhoneNumbers.Add(patNum);
                    }
                    else
                    {
                        TransactionManager.FailedMappingCollection
                            .Add(new FailedMappings
                            {
                                Tablename = "PatientPhoneNumbers",
                                ObjectType = typeof(PatientPhoneNumber),
                                JsonSerializedObject = JsonConvert.SerializeObject(patNum),
                                FailedReason = "Patient phone number already exist in database."
                            });
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating PatientPhonenumber mapping.", e);
            }
        }

        private bool CanAddToContext(string phoneNumber)
        {
            if (String.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.PatientPhoneNumbers.Any(a => a.Number == phoneNumber)) ? false : true;
            }
        }
    }
}
