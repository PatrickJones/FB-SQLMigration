using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class NumedicsGlobalHelpers : DatabaseContextDisposal
    {
        NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.UserType> GetAllUserTypes()
        {
            return db.UserTypes.ToList();
        }

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.TherapyType> GetAllTherapyTypes()
        {
            return db.TherapyTypes.ToList();
        }

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.ReadingEventType> GetAllReadingEventTypes()
        {
            return db.ReadingEventTypes.ToList();
        }

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.PaymentMethod> GetAllPaymentMethods()
        {
            return db.PaymentMethods.ToList();
        }

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.CheckStatu> GetAllCheckStatusTypes()
        {
            return db.CheckStatus.ToList();
        }

        public ICollection<NuLibrary.Migration.SQLDatabase.EF.InsulinType> GetAllInsulinTypes()
        {
            return db.InsulinTypes.ToList();
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
