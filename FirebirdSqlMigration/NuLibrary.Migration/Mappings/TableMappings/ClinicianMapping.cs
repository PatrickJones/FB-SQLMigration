using Newtonsoft.Json;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class ClinicianMapping : IContextHandler
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers(0;)

        public ICollection<Clinician> CompletedMappings = new List<Clinician>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreateClinicianMapping()
        {
            try
            {
                var dataSet = aHelper.GetAllAdminsUsers().Where(w => w.CPSiteId == MigrationVariables.CurrentSiteId).ToList();
                RecordCount = dataSet.Count;

                foreach (var adUser in dataSet)
                {
                    var userInfo = aHelper.GetAspUserInfo(adUser.UserId);
                    if (mHelper.HasUserMigrated((userInfo == null) ? String.Empty : userInfo.UserName, adUser.UserId))
                    {
                        MappingStatistics.LogFailedMapping("None", "None", "Clinicians", typeof(Clinician), String.Empty, "Clinician previously migrated.");
                        FailedCount++;
                    }
                    else
                    {
                        var clin = new Clinician
                        {
                            UserId = nHelper.ValidGuid(adUser.UserId),
                            Firstname = "No Name",
                            Lastname = "No Name",
                            StateLicenseNumber = "No License Number"
                        };

                        clin.LastUpdatedByUser = clin.UserId;

                        if (CanAddToContext(clin.UserId))
                        {
                            CompletedMappings.Add(clin);
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("None", "None", "Clinicians", typeof(Clinician), JsonConvert.SerializeObject(clin), "Clinician already exist in database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("None", 0, "Clinicians", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Clinician mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                // loop through mappings to assign clinicians to saved users
                Array.ForEach(CompletedMappings.ToArray(), c => {

                    User user = nHelper.GetUser(c.UserId);
                    var siteId = MemoryMappings.GetSiteIdFromPatientInfo(c.UserId);
                    var instId = nHelper.GetInstitutionId(siteId);

                    if (instId != Guid.Empty)
                    {
                        c.InstitutionId = instId;

                        var usr = TransactionManager.DatabaseContext.Users.Where(u => u.UserId == c.UserId).FirstOrDefault();
                        usr.Clinician = c;
                    }
                });

                var stats = new SqlTableStats
                {
                    Tablename = "Clinicians",
                    PreSaveCount = CompletedMappings.Count()
                };

                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
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
                return !ctx.Clinicians.Any(c => c.UserId == userId);
            }
        }
    }
}
