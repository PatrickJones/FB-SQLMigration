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
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = "Subscriptions",
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
                throw new Exception("Error creating Subscription mapping.", e);
            }
        }


        public void AddToContext()
        {
            //Array.ForEach(CompletedMappings.ToArray(), c => {
            //    if (TransactionManager.DatabaseContext.Patients.Any(a => a.UserId == c.UserId))
            //    {
            //        TransactionManager.DatabaseContext.Subscriptions.Add(c);
            //    }
            //});
        }

        public void SaveChanges()
        {
            try
            {
                //do
                //{
                //    var range = CompletedMappings.Take(1000).ToList();

                //    if (range.Count != 0)
                //    {
                //        Array.ForEach(range.ToArray(), c =>
                //        {
                //            if (TransactionManager.DatabaseContext.Patients.Any(a => a.UserId == c.UserId))
                //            {
                //                TransactionManager.DatabaseContext.Subscriptions.Add(c);
                //                CompletedMappings.Remove(c);
                //            }
                //        });

                //        TransactionManager.DatabaseContext.SaveChanges();
                //        //TransactionManager.DatabaseContext.Subscriptions.RemoveRange(range); 
                //    }

                //} while (CompletedMappings.Count > 0);

                Array.ForEach(CompletedMappings.ToArray(), c =>
                {
                    if (TransactionManager.DatabaseContext.Patients.Any(a => a.UserId == c.UserId))
                    {
                        TransactionManager.DatabaseContext.Subscriptions.Add(c);
                    }
                });

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
                return (ctx.Subscriptions.Any(a => a.UserId == userid && a.SubscriptionType == subscriptionType && a.ExpirationDate == expiration)) ? false : true;
            }
        }

    }
}
