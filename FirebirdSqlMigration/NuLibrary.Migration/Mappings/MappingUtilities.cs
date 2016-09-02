using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class MappingUtilities : ClientDatabaseBase
    {
        private ClientDatabaseTemplateEntities db = new ClientDatabaseTemplateEntities();
        
        public Patient FindPatient(String patientId)
        {
            return db.Patients.Where(x => x.PatientId == patientId).FirstOrDefault();
            
        }

        public InsuranceProvider FindInsuranceCo(int insCoId)
        {
            return db.InsuranceProviders.Where(x => x.CompanyId == insCoId).FirstOrDefault();
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
