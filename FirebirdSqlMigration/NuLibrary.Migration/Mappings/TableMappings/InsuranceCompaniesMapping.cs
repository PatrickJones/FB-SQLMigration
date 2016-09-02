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
    public class InsuranceCompaniesMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public InsuranceCompaniesMapping() : base("INSURANCECOMPANIES")
        {

        }

        public InsuranceCompaniesMapping(string tableName) :base(tableName)
        {

        }

        public void CreatePatientMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var ips = new InsuranceProvider
                {
                    CompanyId = (Int32)row["KEYID"],
                    Name = (String)row["NAME"],
                    IsActive = (Boolean)row["ISACTIVE"],
                    InActiveDate = (DateTime)row["INACTIVEDATE"]
                };

                var adr = new InsuranceAddress
                {
                    Street1 = (String)row["STREET1"],
                    Street2 = (String)row["STREET2"],
                    Street3 = (String)row["STREET3"],
                    City = (String)row["CITY"],
                    County = (String)row["COUNTY"],
                    State = (String)row["STATE"],
                    Zip = (String)row["ZIP"],
                    Country = (String)row["COUNTRY"]
                };

                var cont = new InsuranceContact
                {
                    FullName = (String)row["CONTACTNAME"],
                    Email = (String)row["EMAIL"]
                };

                ips.InsuranceAddresses.Add(adr);
                ips.InsuranceContacts.Add(cont);

                TransactionManager.DatabaseContext.InsuranceProviders.Add(ips);
            }
        }
    }
}
