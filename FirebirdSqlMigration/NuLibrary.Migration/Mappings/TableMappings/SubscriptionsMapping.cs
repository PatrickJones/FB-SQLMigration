using Newtonsoft.Json;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class SubscriptionsMapping : IContextHandler
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();
        MappingUtilities mu = new MappingUtilities();

        public ICollection<Subscription> CompletedMappings = new List<Subscription>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreateSubscriptionMapping()
        {
            try
            {
                var dataSet = MemoryMappings.GetAllUserIdsFromPatientInfo();
                RecordCount = dataSet.Count;

                foreach (var g in dataSet)
                {
                    var sh = aHelper.GetSubscriptionInfo(g);

                    foreach (var sub in sh.GetMappedSubscriptions())
                    {
                        if (CanAddToContext(sub.UserId, sub.SubscriptionType, sub.ExpirationDate, sub.InstitutionId))
                        {
                            CompletedMappings.Add(sub);
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("Subscriptions", typeof(Subscription), JsonConvert.SerializeObject(sub), "Subscription already exist in database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("None", 0, "Subscriptions", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Subscription mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "Subscriptions",
                    PreSaveCount = CompletedMappings.Count()
                };


                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;
                // Ensure subscription has a matching institution id associated
                var r = from m in q
                        from i in TransactionManager.DatabaseContext.Institutions
                        where m.InstitutionId == i.InstitutionId
                        select m;

                TransactionManager.DatabaseContext.Subscriptions.AddRange(r);
                TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = TransactionManager.DatabaseContext.Subscriptions.Count();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating Subscription entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Subscription entity", e);
            }
        }

        private bool CanAddToContext(Guid userid, int subscriptionType, DateTime expiration, Guid institutionId)
        {
            if (institutionId == Guid.Empty)
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                if (MemoryMappings.GetAllInstitutions().Any(a => a.InstitutionId == institutionId))
                {
                    return !ctx.Subscriptions.Any(a => a.UserId == userid && a.SubscriptionType == subscriptionType && a.ExpirationDate == expiration);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
