using Newtonsoft.Json;
using NuLibrary.Migration.Interfaces;
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

        public void CreateInstitutionMapping()
        {
            try
            {
                var dataSet = aHelper.GetAllPatientUsers();
                RecordCount = dataSet.Count;

                foreach (var pat in dataSet)
                {
                    var sh = aHelper.GetSubscriptionInfo(pat.UserId);

                    foreach (var sub in sh.GetMappedSubscriptions())
                    {
                        if (CanAddToContext(sub.UserId, sub.SubscriptionType, sub.ExpirationDate))
                        {
                            CompletedMappings.Add(sub);
                        }
                        else
                        {
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = "Institutions",
                                    ObjectType = typeof(Subscription),
                                    JsonSerializedObject = JsonConvert.SerializeObject(sub),
                                    FailedReason = "Subscription already exist in database."
                                });

                            FailedCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Institution mapping.", e);
            }
        }


        public void AddToContext()
        {
            TransactionManager.DatabaseContext.Subscriptions.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating Institution entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Institution entity", e);
            }
        }

        private bool CanAddToContext(Guid userid, int subscriptionType, DateTime expiration)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.Subscriptions.Any(a => a.UserId == userid && a.SubscriptionType == subscriptionType && a.ExpirationDate == expiration)) ? false : true;
            }
        }

    }
}
