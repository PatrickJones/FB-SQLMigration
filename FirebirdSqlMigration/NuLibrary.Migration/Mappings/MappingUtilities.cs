using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class MappingUtilities : ClientDatabaseBase
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();
        
        public Patient FindPatient(Guid userId)
        {
            return db.Patients.Where(x => x.UserId == userId).FirstOrDefault();
            
        }

        public InsuranceProvider FindInsuranceCo(int insCoId)
        {
            return db.InsuranceProviders.Where(x => x.CompanyId == insCoId).FirstOrDefault();
        }

        public CareSetting FindPatientCareSetting(Guid userId)
        {
            return db.CareSettings.Where(x => x.UserId == userId).FirstOrDefault();
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

        public Pump FindPatientPump(Guid userId)
        {
            var pump = db.Pumps.Where(x => x.UserId == userId).FirstOrDefault();
            return pump;
        }
        public PumpProgram FindPumpProgram(string name, int pkey)
        {
            var ppId = db.PumpPrograms.Where(x => x.ProgramName == name && x.ProgramKey == pkey).FirstOrDefault();
            return ppId;
        }

        public ICollection<PumpSetting> CreatePumpSetting(DataRow record, long id)
        {
            ICollection<PumpSetting> ips = new List<PumpSetting>();
            ips.Add(new PumpSetting { SettingValue = (String)record["BOLUSCALCS"], SettingName = "BOLUSCALCS", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["MINBGFORCALCS"], SettingName = "MINBGFORCALCS", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["REVERSECORRECTION"], SettingName = "REVERSECORRECTION", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["INSULINACTION"], SettingName = "INSULINACTION", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["LOWERTHRESHOLDFORBG"], SettingName = "LOWERTHRESHOLDFORBG", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["UPPERTHRESHOLDFORBG"], SettingName = "UPPERTHRESHOLDFORBG", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["INSULINLEFTALERT"], SettingName = "INSULINLEFTALERT", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["BGREMINDERS"], SettingName = "BGREMINDERS", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["PODEXPIRATIONALERT"], SettingName = "PODEXPIRATIONALERT", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["AUTOOFFALARMTIME"], SettingName = "AUTOOFFALARMTIME", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["ENABLEDBOLUSREMINDER"], SettingName = "ENABLEDBOLUSREMINDER", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["ENABLEDREMINDERALERT"], SettingName = "ENABLEDREMINDERALERT", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["ENABLEDCONFIDENCEALERT"], SettingName = "ENABLEDCONFIDENCEALERT", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["DISPLAYBG"], SettingName = "DISPLAYBG", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["MAXBASALRATE"], SettingName = "MAXBASALRATE", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["MAXBOLUSVOLUME"], SettingName = "MAXBOLUSVOLUME", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["EXTENDEDBOLUS"], SettingName = "EXTENDEDBOLUS", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["SOUNDBG"], SettingName = "SOUNDBG", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["INCREMENTBOLUS"], SettingName = "INCREMENTBOLUS", PumpId = id });
            ips.Add(new PumpSetting { SettingValue = (String)record["TEMPBASALDELIVERY"], SettingName = "TEMPBASALDELIVERY", PumpId = id });

            return ips;
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
