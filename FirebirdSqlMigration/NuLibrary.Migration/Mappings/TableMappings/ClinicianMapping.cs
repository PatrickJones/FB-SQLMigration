using Newtonsoft.Json;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class ClinicianMapping : IContextHandler
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

        public ICollection<Clinician> CompletedMappings = new List<Clinician>();

        public int RecordCount = 0;
        public int FailedCount = 0;


        public void CreateClinicianMapping()
        {
            try
            {
                var dataSet = aHelper.GetAllAdminsUsers();
                RecordCount = dataSet.Count;

                foreach (var adUser in aHelper.GetAllAdminsUsers())
                {
                    var instId = nHelper.GetInstitutionId(adUser.CPSiteId);

                    var clin = new Clinician
                    {
                        UserId = adUser.UserId,
                        Firstname = "No Name",
                        Lastname = "No Name",
                        StateLicenseNumber = "No License Number",
                        InstitutionId = instId
                    };

                    if (CanAddToContext(clin.UserId) && instId != Guid.Empty)
                    {
                        //TransactionManager.DatabaseContext.Clinicians.Add(clin);
                        CompletedMappings.Add(clin);
                    }
                    else
                    {
                        TransactionManager.FailedMappingCollection
                            .Add(new FailedMappings
                            {
                                Tablename = "Clinicians",
                                ObjectType = typeof(Clinician),
                                JsonSerializedObject = JsonConvert.SerializeObject(clin),
                                FailedReason = (instId == Guid.Empty) ? "Clinician is not linked to institution." : "Clinician already exist in database."
                                
                            });

                        FailedCount++;
                    }
                }

                //TransactionManager.DatabaseContext.SaveChanges();

            }
            catch (Exception e)
            {
                throw new Exception("Error creating Clinician mapping.", e);
            }
            
        }

        public void AddToContext()
        {
            TransactionManager.DatabaseContext.Clinicians.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating Clinician entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Clinician entity", e);
            }
        }


        private bool CanAddToContext(Guid userId)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.Clinicians.Any(c => c.UserId == userId)) ? false : true;
            }
        }

    }
}
