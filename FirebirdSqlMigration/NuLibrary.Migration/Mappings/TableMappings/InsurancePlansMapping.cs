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
    public class InsurancePlansMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public InsurancePlansMapping() : base("INSURANCEPLANS2")
        {

        }

        public InsurancePlansMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();


        public void CreateInsurancePlansMapping()
        {
            //MappingUtilities mu = new MappingUtilities();
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = (String)row["PATIENTID"];
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var insp = new InsurancePlan
                    {
                        UserId = userId,
                        PlanType = (String)row["INSTYPEID"], // is an int in firebird, may need to convert
                        PlanIdentifier = (String)row["PLANIDENTIFIER"],
                        PolicyNumber = (String)row["POLICYNUMBER"],
                        GroupName = (String)row["GROUPNAME"],
                        GroupIdentifier = (String)row["GROUPNUMBER"],
                        CoPay = (Decimal)row["COPAY"], // is a double precision in firebird, may need to convert
                        Purchaser = (String)row["PURCHASER"],
                        IsActive = (bool)row["ISACTIVE"], // is a char(1) in firebird, may need to convert
                        InActiveDate = (DateTime)row["INACTIVEDATE"],
                        EffectiveDate = (DateTime)row["EFFECTIVEDATE"],
                        CompanyId = (int)row["INSCOID"] //is a double precision in firebird, may need to convert
                    };

                    TransactionManager.DatabaseContext.InsurancePlans.Add(insp);
                }
            }
        }
    }
}
