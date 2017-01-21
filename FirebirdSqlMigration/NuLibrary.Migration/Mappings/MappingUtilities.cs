using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.Mappings.InMemoryMappings;
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

        public ICollection<Patient> GetPatients()
        {
            return db.Patients.ToList();
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
            return db.InsulinBrands.Where(x => x.BrandName.ToLower() == insbrand.ToLower()).Select(s => s.InsulinBrandId).FirstOrDefault();
        }

        public int FindInsulinMethodId(String insmethod)
        {
            return db.InsulinMethods.Where(x => x.Method.ToLower() == insmethod.ToLower()).Select(s => s.InsulinMethodId).FirstOrDefault();
        }

        public int FindDMTypeId(String name)
        {
            return db.DiabetesManagementTypes.Where(x => x.Name.ToLower() == name.ToLower()).Select(s => s.TypeId).FirstOrDefault();
        }

        public string GetInsurancePlanType(string plan)
        {
            switch (plan)
            {
                case "1":
                    return "Assignment";
                case "2":
                    return "Cash";
                case "3":
                    return "FFS";
                case "4":
                    return "HMO";
                case "5":
                    return "Indemnity";
                case "6":
                    return "Medicaid";
                case "7":
                    return "Medicaid HMO";
                case "8":
                    return "PPO";
                case "9":
                    return "Unstated";
                case "12":
                    return "Medicare";
                case "13":
                    return "Medicare Supplement";
                case "14":
                    return "Private Insurance";
                case "15":
                    return "MediCal Fee for Service";
                case "16":
                    return "MediCal HMO";
                default:
                    return String.Empty;
            }
        }

        public Guid ParseGUID(string guid)
        {
            Guid g = Guid.Empty;
            var parse = Guid.TryParse(guid, out g);

            return g;
        }

        public Dictionary<string, bool> ParseDMControlTypes(int typeValue)
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Unable to parse DMControl Type.", e);
            }
        }


        public decimal ParseMoney(string money)
        {
            if (String.IsNullOrEmpty(money))
            {
                return 0;
            }

            decimal result = 0M;
            bool parse = Decimal.TryParse(money, out result);

            return result;
        }

        public Pump FindPatientPump(Guid userId)
        {
            return db.Pumps.Where(x => x.UserId == userId).FirstOrDefault();
        }
        public PumpProgram FindPumpProgram(string name, int pkey)
        {
            return db.PumpPrograms.Where(x => x.ProgramName == name && x.ProgramKey == pkey).FirstOrDefault();
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

        public DateTime ParseFirebirdDateTime(string datetime)
        {
            DateTime dt = new DateTime(1800, 1, 1);
            var parse = DateTime.TryParse(datetime, out dt);

            if (dt < new DateTime(1800,1,1) || dt > DateTime.Now)
            {
                dt = new DateTime(1800, 1, 1);
            }

            return dt;
        }

        public TimeSpan ParseFirebirdTimespan(string timespan)
        {
            DateTime dt;
            var parse = DateTime.TryParse(timespan, out dt);

            TimeSpan ts = new TimeSpan(dt.Hour, dt.Minute, dt.Second);

            if (parse)
            {
                return ts;
            }
            else
            {
                return new TimeSpan(12, 0, 0);
            }
        }

        public bool ParseFirebirdBoolean(string character)
        {
            if (String.IsNullOrEmpty(character))
            {
                return false;
            }

            switch (character.ToLower())
            {
                case "t":
                    return true;
                default:
                    return false;
            }
        }

        public int ParseFirebirdPhoneTypes(string pType)
        {
            if (String.IsNullOrEmpty(pType))
            {
                return 0;
            }

            switch (pType.ToLower())
            {
                case "cell":
                    return 3;
                case "fax":
                    return 4;
                case "home":
                    return 1;
                case "work":
                    return 2;
                default:
                    return 0;
            }
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
