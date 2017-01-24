using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class SubscriptionTypeVaidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public List<NuLibrary.Migration.SQLDatabase.EF.SubscriptionType> DefaultSubscriptionTypes = new List<NuLibrary.Migration.SQLDatabase.EF.SubscriptionType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.SubscriptionType> Missing = new List<NuLibrary.Migration.SQLDatabase.EF.SubscriptionType>();

        public SubscriptionTypeVaidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
            Init();
        }

        public SubscriptionTypeVaidation()
        {
            Init();
        }

        private void Init()
        {
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers();

            DefaultSubscriptionTypes.Add(new SubscriptionType {
                ApplicationId = nh.GetApplicationId("diabetes partner"),
                SubscriptionLengthDays = 30,
                Label = "30 dsys",
                Description = "30 Day Subscription",
                Price = 6.50M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("diabetes partner"),
                SubscriptionLengthDays = 90,
                Label = "3 months",
                Description = "3 Month Subscription",
                Price = 5.50M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("diabetes partner"),
                SubscriptionLengthDays = 180,
                Label = "6 months",
                Description = "6 Month Subscription",
                Price = 4.00M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("diabetes partner"),
                SubscriptionLengthDays = 365,
                Label = "1 year",
                Description = "1 Year Subscription",
                Price = 3.00M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("clinipro-web"),
                SubscriptionLengthDays = 30,
                Label = "30 dsys",
                Description = "30 Day Subscription Plus",
                Price = 999.99M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("clinipro-web"),
                SubscriptionLengthDays = 30,
                Label = "30 dsys",
                Description = "30 Day Subscription",
                Price = 45.95M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("administration"),
                SubscriptionLengthDays = 0,
                Label = "Adjustment",
                Description = "Adjustment",
                Price = 0M
            });

            DefaultSubscriptionTypes.Add(new SubscriptionType
            {
                ApplicationId = nh.GetApplicationId("administration"),
                SubscriptionLengthDays = 0,
                Label = "Free",
                Description = "Free Download",
                Price = 0M
            });
        }

        public string TableName
        {
            get
            {
                return "SubscriptionType";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in DefaultSubscriptionTypes)
            {
                if (!db.SubscriptionTypes.Any(a => a.Description.ToLower() == ut.Description.ToLower()))
                {
                    Missing.Add(ut);
                }
            }

            if (Missing.Count == DefaultSubscriptionTypes.Count)
            {
                SyncTable();
                return true;
            }
            else
            {
                return (Missing.Count == 0) ? true : false;
            }
        }

        public void SyncTable()
        {
            db.SubscriptionTypes.AddRange(Missing);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
