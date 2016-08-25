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
    public class PatientsMapping : BaseMapping
    {
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
                    MRID = (String)row["KEYID"],
                    Firstname = (String)row["FIRSTNAME"]
                };

                var adr = new PatientAddress {
                    Street1 = (String)row["STREET"],
                    Street2 = (String)row["STREET2"]
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
