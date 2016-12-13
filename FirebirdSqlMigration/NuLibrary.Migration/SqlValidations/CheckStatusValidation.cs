using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class CheckStatusValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<CheckStatu> defCheckStatus = new List<CheckStatu>();
        public List<CheckStatu> Failures = new List<CheckStatu>();

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
                defCheckStatus.Add(
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
            foreach (var ut in defCheckStatus)
            {
                if (!db.CheckStatus.Any(a => a.Status.ToLower() == ut.Status.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.CheckStatus.AddRange(Failures);
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
