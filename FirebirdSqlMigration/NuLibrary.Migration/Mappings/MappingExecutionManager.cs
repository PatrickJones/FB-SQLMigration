using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.Mappings.TableMappings;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.SQLDatabase.EF;

namespace NuLibrary.Migration.Mappings
{
    /// <summary>
    /// Manages the execution of the migration.
    /// </summary>
    public class MappingExecutionManager
    {
        /// <summary>
        /// Stores collection of mapping instances
        /// </summary>
        public Dictionary<int, KeyValuePair<Type, IContextHandler>> mapInstances = new Dictionary<int, KeyValuePair<Type, IContextHandler>>();

        /// <summary>
        /// Constructor that initializes mapping instances.
        /// </summary>
        public MappingExecutionManager()
        {
            InitializeMappings();
        }

        /// <summary>
        /// Initializes mapping instances and adds them to the dictionary
        /// </summary>
        private void InitializeMappings()
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
            mapInstances.Add(9, new KeyValuePair<Type, IContextHandler>(typeof(DeviceMeterReadingHeaderMapping), new DeviceMeterReadingHeaderMapping()));
            mapInstances.Add(10, new KeyValuePair<Type, IContextHandler>(typeof(SubscriptionsMapping), new SubscriptionsMapping()));
        }

        /// <summary>
        /// Validates all lookup tables.
        /// Begins execution of mapping sequence
        /// </summary>
        public void BeginExecution()
        {
            var vt = new ValidateTables();
            var validDict = vt.ValidateAll();

            CreateMappings();
        }

        /// <summary>
        /// Creates firebird to sql mappings in paralell
        /// </summary>
        private void CreateMappings()
        {
            // this set of task must execute first to populate in-memory objects used by subsequent task list.
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
                }),
                Task.Run(() =>
                {
                    var instance = new NuLicenseMapping();
                    instance.CreateNuLicenseMapping();
                })
            };


            

            Task.WhenAll(taskSetA).ContinueWith(doneA => {
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

                Task.WhenAll(taskSetB).ContinueWith(doneB => {
                    var taskSetC = new List<Task> {
                        Task.Run(() =>
                        {
                            var instance = (DeviceMeterReadingHeaderMapping)mapInstances[9].Value;
                            instance.CreateDeviceMeterReadingHeaderMapping();
                        }),
                        Task.Run(() =>
                        {
                            var instance = (SubscriptionsMapping)mapInstances[10].Value;
                            instance.CreateSubscriptionMapping();
                        })
                    };

                    Task.WhenAll(taskSetC).ContinueWith(doneC => {
                        UpdateContext();
                    });
                });
            });
        }

        /// <summary>
        /// Updates the database context with in-memory entities and saves the changes.
        /// </summary>
        public void UpdateContext()
        {
            for (int i = 0; i < mapInstances.Count; i++)
            {
                mapInstances[i].Value.AddToContext();
                mapInstances[i].Value.SaveChanges();
            }

            //AddDmDataSet();
            CommitExecution();
        }

        //private void AddDmDataSet()
        //{
        //    using (var ctx = new NuMedicsGlobalEntities())
        //    {
        //        Array.ForEach(MemoryDiabetesManagementData.DMDataCollection.ToArray(), d => {
        //            var pd = ctx.PatientDevices.Where(w => w.UserId == d.UserId).FirstOrDefault();

        //            if (pd != null)
        //            {
        //                d.PatientDevice = pd;
        //                ctx.DiabetesManagementDatas.Add(d);
        //            }
        //        });

        //        ctx.SaveChanges();
        //    }
        //}


        /// <summary>
        /// Commits the transaction.
        /// </summary>
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
