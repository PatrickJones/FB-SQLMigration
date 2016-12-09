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
    public class PaymentMethodValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod> defPaymentMethods = new List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod>();
        public List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod> Failures = new List<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod>();

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
                defPaymentMethods.Add(
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
            foreach (var ut in defPaymentMethods)
            {
                if (!db.PaymentMethods.Any(a => a.MethodName.ToLower() == ut.MethodName.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.PaymentMethods.AddRange(Failures);
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
