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

        public CareSetting FindPatientCareSetting(String patientId)
        {
            return db.CareSettings.Where(x => x.PatientId == patientId).FirstOrDefault();
        }

        public int FindInsulinBrandId(String insbrand)
        {
            var ib =  db.InsulinBrands.Where(x => x.BrandName.ToLower() == insbrand.ToLower()).FirstOrDefault();
            return ib.InsulinBrandId;
        }

        public int FindInsulinMethodId(String insmethod)
        {
            var im = db.InsulinMethods.Where(x => x.Method.ToLower() == insmethod.ToLower()).FirstOrDefault();
            return im.InsulinMethodId;
        }

        public int FindDMTypeId(String name)
        {
            var dmType = db.DiabetesManagementTypes.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            return dmType.TypeId;
        }

        public Dictionary<string, bool> ParseDMControlTypes(int typeValue)
        {
            var dict = new Dictionary<string, bool>();
            if (typeValue != 0)
            {
                //bool Diet = (typeValue / 8) % 2 == 1; // e.g., "13" or "1101 base 2" gives us a "1" or "true"
                //bool Exercise = (typeValue / 4) % 2 == 1; // // e.g., "13" or "1101 base 2" gives us a "1" or "true"
                //bool Insulin = (typeValue / 2) % 2 == 1;// e.g., "13" or "1101 base 2" gives us a "0" or "false"
                //bool Medication = typeValue % 2 == 1;// e.g., "13" or "1101 base 2" gives us a "1" or "true"

                dict.Add("Diet", ((typeValue / 8) % 2 == 1)); // e.g., "13" or "1101 base 2" gives us a "1" or "true"
                dict.Add("Exercise", ((typeValue / 4) % 2 == 1)); // // e.g., "13" or "1101 base 2" gives us a "1" or "true"
                dict.Add("Insulin", ((typeValue / 2) % 2 == 1));// e.g., "13" or "1101 base 2" gives us a "0" or "false"
                dict.Add("Medication", (typeValue % 2 == 1));// e.g., "13" or "1101 base 2" gives us a "1" or "true"
            }

            return dict;
        }
        //public List<MeterReadingHeader> FindPatientMeterReadingHeader(String patientId)
        //{
        //    return db.MeterReadingHeaders.Where(x => x.PatientId == patientId).ToList();
        //}

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
