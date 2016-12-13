using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuLibrary.Migration.SqlValidations;

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
            throw new NotImplementedException();

            ExecuteClinicianMapping();
        }

        private void ExecuteClinicianMapping()
        {
            throw new NotImplementedException();

            ExecutePatientMapping();
        }

        private void ExecutePatientMapping()
        {
            throw new NotImplementedException();

            ExecutePatientPhoneMapping();
        }

        private void ExecutePatientPhoneMapping()
        {
            throw new NotImplementedException();

            ExecuteInsuranceCompanyMapping();
        }

        private void ExecuteInsuranceCompanyMapping()
        {
            throw new NotImplementedException();

            ExecuteInsurancePlanMapping();
        }

        private void ExecuteInsurancePlanMapping()
        {
            throw new NotImplementedException();
        }
    }
}
