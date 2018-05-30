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
    public class PaymentMethodValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod> DefaultPaymentMethods = new List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod>();
        public List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod> Missing = new List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod>();

        public PaymentMethodValidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
            Init();
        }

        public PaymentMethodValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "PayPal",
                "Check",
                "Invoice",
                "Adjustment"
            };

            Array.ForEach(typeArr, a => {
                DefaultPaymentMethods.Add(
                new NuLibrary.Migration.SQLDatabase.EF.PaymentMethod
                {
                    MethodName = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "PaymentMethods";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in DefaultPaymentMethods)
            {
                if (!db.PaymentMethods.Any(a => a.MethodName.ToLower() == ut.MethodName.ToLower()))
                {
                    Missing.Add(ut);
                }
            }

            return (Missing.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.PaymentMethods.AddRange(Missing);
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
