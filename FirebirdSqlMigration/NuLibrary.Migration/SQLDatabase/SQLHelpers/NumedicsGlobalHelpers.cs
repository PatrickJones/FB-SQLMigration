using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class NumedicsGlobalHelpers : DatabaseContextDisposal
    {
        NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public NumedicsGlobalHelpers()
        {

        }

        public NumedicsGlobalHelpers(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
        }

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

        public Guid GetApplicationId(string applicationName)
        {
            var app = db.Applications.Where(w => w.ApplicationName.ToLower() == applicationName.ToLower()).FirstOrDefault();
            return (app == null) ? Guid.Empty : app.ApplicationId;
        }

        public Guid GetInstitutionId(int? cPSiteId)
        {
            return (cPSiteId.HasValue) ? db.Institutions.Where(w => w.LegacySiteId == cPSiteId.Value).Select(s => s.InstitutionId).FirstOrDefault() : Guid.Empty;
        }

        public int GetInsuranceCompanyId(string companyKeyId)
        {
            var kv = MemoryMappings.GetAllCompanies().Where(n => n.Key == companyKeyId).FirstOrDefault();
            var cName = db.InsuranceProviders.Where(n => n.Name == kv.Value).FirstOrDefault();
            return (cName == null) ? 0 : cName.CompanyId;
        }

        public bool UserIdExist(Guid userId)
        {
            return db.Users.Any(a => a.UserId == userId);
        }

        public User GetUser(Guid userId)
        {
            return db.Users.Where(w => w.UserId == userId).FirstOrDefault();
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
