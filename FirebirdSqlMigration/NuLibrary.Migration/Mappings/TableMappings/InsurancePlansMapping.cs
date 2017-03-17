using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class InsurancePlansMapping : BaseMapping, IContextHandler
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

        public ICollection<InsurancePlan> CompletedMappings = new List<InsurancePlan>();
        private ICollection<Tuple<string, InsurancePlan>> tempCompanyId = new List<Tuple<string, InsurancePlan>>();

        public int RecordCount = 0;
        public int FailedCount = 0;


        public void CreateInsurancePlansMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = (String)row["PATIENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

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
                            //CompanyId = nHelper.GetInsuranceCompanyId(row["INSCOID"].ToString()) //(int)row["INSCOID"] //is a double precision in firebird, may need to convert
                        };

                        if (CanAddToContext(insp.UserId, insp.PlanType, insp.PolicyNumber))
                        {
                            tempCompanyId.Add(new Tuple<string, InsurancePlan>(row["INSCOID"].ToString(), insp));
                            CompletedMappings.Add(insp);
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("InsurancePlans", typeof(InsurancePlan), JsonConvert.SerializeObject(insp), "Insurance Plan already exist in database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("INSURANCEPLANS2", RecordCount, "InsurancePlans", 0, CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating InsurancePlan mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                Array.ForEach(tempCompanyId.ToArray(), a =>
                {
                    a.Item2.CompanyId = nHelper.GetInsuranceCompanyId(a.Item1);
                });

                var stats = new SqlTableStats
                {
                    Tablename = "InsurancePlans",
                    PreSaveCount = CompletedMappings.Count()
                };

                Parallel.ForEach(CompletedMappings, c => {
                    var patient = TransactionManager.DatabaseContext.Patients.FirstOrDefault(f => f.UserId == c.UserId);
                    c.Patients.Add(patient);
                });

                TransactionManager.DatabaseContext.InsurancePlans.AddRange(CompletedMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating InsurancePlan entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving InsurancePlan entity", e);
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
