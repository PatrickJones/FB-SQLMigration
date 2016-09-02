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
    public class PatientsMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PatientsMapping() : base("Patients")
        {

        }

        public PatientsMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePatientMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var pat = new Patient
                {
                    PatientId = (String)row["KEYID"],
                    MRID = (String)row["MEDICALRECORDIDENTIFIER"],
                    Firstname = (String)row["FIRSTNAME"],
                    Lastname = (String)row["LASTNAME"],
                    Middlename = (String)row["MIDDLENAME"],
                    Suffix = (String)row["SUFFIX"],
                    Gender = (Int32)row["GENDER"],
                    DateofBirth = (DateTime)row["DOB"]
                };

                var adr = new PatientAddress {
                    Street1 = (String)row["STREET1"],
                    Street2 = (String)row["STREET2"],
                    Street3 = (String)row["STREET3"],
                    City = (String)row["CITY"],
                    County = (String)row["COUNTY"],
                    State = (String)row["STATE"],
                    Zip = (String)row["ZIP"],
                    Country = (String)row["COUNTRY"]
                };

                var email = new PatientEmail {
                    Email = (String)row["EMAIL"],
                    LoweredEmail = row["EMAIL"].ToString().ToLower()
                };

                pat.PatientAddresses.Add(adr);
                pat.PatientEmails.Add(email);

                TransactionManager.DatabaseContext.Patients.Add(pat);
            }
        }
    }
}
