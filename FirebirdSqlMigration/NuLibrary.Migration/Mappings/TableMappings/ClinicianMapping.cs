using Newtonsoft.Json;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class ClinicianMapping
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

        public void CreateClinicianMapping()
        {
            try
            {
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
                        TransactionManager.DatabaseContext.Clinicians.Add(clin);
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
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();

            }
            catch (Exception e)
            {
                throw;
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
