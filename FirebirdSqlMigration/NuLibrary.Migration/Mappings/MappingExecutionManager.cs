using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.Mappings.TableMappings;

namespace NuLibrary.Migration.Mappings
{
    public class MappingExecutionManager
    {
        public MappingExecutionManager()
        {
            BeginExecution();
        }

        private void BeginExecution()
        {
            var vt = new ValidateTables();
            var validDict = vt.ValidateAll();

            ExecuteInstitutionMapping();
        }

        private void ExecuteInstitutionMapping()
        {
            InstitutionMapping map = new InstitutionMapping();
            map.CreateInstitutionMapping();

            ExecuteClinicianMapping();
        }

        private void ExecuteClinicianMapping()
        {
            ClinicianMapping map = new ClinicianMapping();
            map.CreateClinicianMapping();

            ExecutePatientMapping();
        }

        private void ExecutePatientMapping()
        {
            PatientsMapping map = new PatientsMapping();
            map.CreatePatientMapping();

            ExecutePatientPhoneMapping();
        }

        private void ExecutePatientPhoneMapping()
        {
            PatientPhoneNumbersMapping map = new PatientPhoneNumbersMapping();
            map.CreatePatientPhoneNumbersMapping();

            ExecuteInsuranceCompanyMapping();
        }

        private void ExecuteInsuranceCompanyMapping()
        {
            InsuranceCompaniesMapping map = new InsuranceCompaniesMapping();
            map.CreatePatientMapping();

            ExecuteInsurancePlanMapping();
        }

        private void ExecuteInsurancePlanMapping()
        {
            InsurancePlansMapping map = new InsurancePlansMapping();
            map.CreateInsurancePlansMapping();
        }
    }
}
