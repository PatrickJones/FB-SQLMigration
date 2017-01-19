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
            try
            {
                foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = (String)row["PATIENTID"].ToString();
                    var userId = MemoryPatientInfo.GetUserId(MigrationVariables.CurrentSiteId, patId);

                    if (userId != Guid.Empty)
                    {
                        var insp = new InsurancePlan
                        {
                            UserId = userId,
                            PlanName = String.Empty,
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

                        if (CanAddToContext(insp.UserId, insp.PlanType, insp.PolicyNumber))
                        {
                            TransactionManager.DatabaseContext.InsurancePlans.Add(insp);
                        }
                        else
                        {
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = "InsurancePlans",
                                    ObjectType = typeof(InsurancePlan),
                                    JsonSerializedObject = JsonConvert.SerializeObject(insp),
                                    FailedReason = "Insurance Plan already exist in database."
                                });
                        }
                        
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating InsurancePlan mapping.", e);
            }
        }

        private bool CanAddToContext(Guid userId, string planType, string policyNumber)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.InsurancePlans.Any(a => a.UserId == userId && a.PlanType == planType && a.PolicyNumber == policyNumber)) ? false : true;
            }
        }
    }
}
