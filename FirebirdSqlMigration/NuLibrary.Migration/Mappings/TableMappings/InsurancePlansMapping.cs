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
        MappingUtilities map = new MappingUtilities();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

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
                        PlanType = (row["INSTYPEID"] is DBNull) ? String.Empty : map.GetInsurancePlanType(row["INSTYPEID"].ToString()),
                        PlanIdentifier = (row["PLANIDENTIFIER"] is DBNull) ? String.Empty : row["PLANIDENTIFIER"].ToString(),
                        PolicyNumber = (row["POLICYNUMBER"] is DBNull) ? String.Empty : row["POLICYNUMBER"].ToString(),
                        GroupName = (row["GROUPNAME"] is DBNull) ? String.Empty : row["GROUPNAME"].ToString(),
                        GroupIdentifier = (row["GROUPNUMBER"] is DBNull) ? String.Empty : row["GROUPNUMBER"].ToString(),
                        CoPay = (row["COPAY"] is DBNull) ? 0 : map.ParseMoney(row["COPAY"].ToString()),
                        Purchaser = (row["PURCHASER"] is DBNull) ? String.Empty : row["PURCHASER"].ToString(),
                        IsActive = (row["ISACTIVE"] is DBNull) ? false : map.ParseFirebirdBoolean(row["ISACTIVE"].ToString()),
                        InActiveDate = (row["INACTIVEDATE"] is DBNull) ? new DateTime(1800, 1, 1) : map.ParseFirebirdDateTime(row["INACTIVEDATE"].ToString()),
                        EffectiveDate = (row["EFFECTIVEDATE"] is DBNull) ? new DateTime(1800, 1, 1) : map.ParseFirebirdDateTime(row["EFFECTIVEDATE"].ToString()),
                        CompanyId = nHelper.GetInsuranceCompanyId(row["INSCOID"].ToString()) //(int)row["INSCOID"] //is a double precision in firebird, may need to convert
                    };

                    TransactionManager.DatabaseContext.InsurancePlans.Add(insp);
                }
            }
        }
    }
}
