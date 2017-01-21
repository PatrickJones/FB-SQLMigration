using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.Mappings.TableMappings;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.Interfaces;

namespace NuLibrary.Migration.Mappings
{
    public class MappingExecutionManager
    {
        public Dictionary<int, KeyValuePair<Type, IContextHandler>> mapInstances = new Dictionary<int, KeyValuePair<Type, IContextHandler>>();

        public MappingExecutionManager()
        {
            mapInstances.Add(0, new KeyValuePair<Type, IContextHandler>(typeof(InstitutionMapping), new InstitutionMapping()));
            mapInstances.Add(1, new KeyValuePair<Type, IContextHandler>(typeof(UserAuthenticationsMapping), new UserAuthenticationsMapping()));
            mapInstances.Add(2, new KeyValuePair<Type, IContextHandler>(typeof(ClinicianMapping), new ClinicianMapping()));
            mapInstances.Add(3, new KeyValuePair<Type, IContextHandler>(typeof(PatientsMapping), new PatientsMapping()));
            mapInstances.Add(4, new KeyValuePair<Type, IContextHandler>(typeof(PatientPhoneNumbersMapping), new PatientPhoneNumbersMapping()));
            mapInstances.Add(5, new KeyValuePair<Type, IContextHandler>(typeof(InsuranceCompaniesMapping), new InsuranceCompaniesMapping()));
            mapInstances.Add(6, new KeyValuePair<Type, IContextHandler>(typeof(InsurancePlansMapping), new InsurancePlansMapping()));
            mapInstances.Add(7, new KeyValuePair<Type, IContextHandler>(typeof(DMDataMapping), new DMDataMapping()));
            mapInstances.Add(8, new KeyValuePair<Type, IContextHandler>(typeof(TimeSlotsMapping), new TimeSlotsMapping()));
        }

        public void BeginExecution()
        {
            var vt = new ValidateTables();
            var validDict = vt.ValidateAll();

            CreateMappings();
        }

        private void CreateMappings()
        {
            var taskSetA = new List<Task> { 
                Task.Run(() =>
                {
                    var instance = (InstitutionMapping)mapInstances[0].Value;
                    instance.CreateInstitutionMapping();
                }),
                Task.Run(() =>
                {
                    var instance = (UserAuthenticationsMapping)mapInstances[1].Value;
                    instance.CreateUserAuthenticationMapping();
                }),
                Task.Run(() =>
                {
                    var instance = (ClinicianMapping)mapInstances[2].Value;
                    instance.CreateClinicianMapping();
                }),
                Task.Run(() =>
                {
                    var instance = (PatientsMapping)mapInstances[3].Value;
                    instance.CreatePatientMapping();
                })
            };

            Task.WhenAll(taskSetA).ContinueWith(done => {
                var taskSetB = new List<Task> {
                    Task.Run(() =>
                    {
                        var instance = (PatientPhoneNumbersMapping)mapInstances[4].Value;
                        instance.CreatePatientPhoneNumbersMapping();
                    }),
                    Task.Run(() =>
                    {
                        var instance = (InsuranceCompaniesMapping)mapInstances[5].Value;
                        instance.CreateInsuranceCompanyMapping();
                    }),
                    Task.Run(() =>
                    {
                        var instance = (InsurancePlansMapping)mapInstances[6].Value;
                        instance.CreateInsurancePlansMapping();
                    }),
                    Task.Run(() =>
                    {
                        var instance = (DMDataMapping)mapInstances[7].Value;
                        instance.CreateDMDataMapping();
                    }),
                    Task.Run(() =>
                    {
                        var instance = (TimeSlotsMapping)mapInstances[8].Value;
                        instance.CreateTimeSlotsMapping();
                    })
                };

                Task.WhenAll(taskSetB).ContinueWith(end => { UpdateContext(); });
            });
        }

        public void UpdateContext()
        {
            for (int i = 0; i < mapInstances.Count; i++)
            {
                mapInstances[i].Value.AddToContext();
                mapInstances[i].Value.SaveChanges();
            }

            CommitExecution();
        }



        private void CommitExecution()
        {
            //throw new NotImplementedException();
            try
            {


                TransactionManager.ExecuteTransaction(); //TESTING ONLY
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
