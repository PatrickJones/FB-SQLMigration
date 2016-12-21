using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class CheckStatusValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public List<CheckStatu> DefaultCheckStatus = new List<CheckStatu>();
        public List<CheckStatu> Missing = new List<CheckStatu>();

        public CheckStatusValidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
            Init();
        }

        public CheckStatusValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "Completed",
                "Cancelled",
                "Pending",
                "RejectedByBank"
            };

            Array.ForEach(typeArr, a => {
                DefaultCheckStatus.Add(
                new CheckStatu
                {
                    Status = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "CheckStatus";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in DefaultCheckStatus)
            {
                if (!db.CheckStatus.Any(a => a.Status.ToLower() == ut.Status.ToLower()))
                {
                    Missing.Add(ut);
                }
            }

            return (Missing.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.CheckStatus.AddRange(Missing);
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
