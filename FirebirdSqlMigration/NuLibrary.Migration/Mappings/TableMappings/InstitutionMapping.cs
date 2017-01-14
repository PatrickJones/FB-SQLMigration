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
    public class InstitutionMapping
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();
        MappingUtilities mu = new MappingUtilities();

        public void CreateInstitutionMapping()
        {
            try
            {
                foreach (var ins in aHelper.GetAllCorporationInfo())
                {
                    var inst = new Institution
                    {
                        InstitutionId = Guid.NewGuid(),
                        Name = ins.Site_Name,
                        LegacySiteId = (ins.SiteId.HasValue) ? ins.SiteId.Value : 0
                    };

                    if (CanAddToContext(inst.Name, inst.LegacySiteId))
                    {
                        TransactionManager.DatabaseContext.Institutions.Add(inst);
                    }
                    else
                    {
                        TransactionManager.FailedMappingCollection
                            .Add(new FailedMappings {
                                Tablename = "Institutions",
                                ObjectType = typeof(Institution),
                                JsonSerializedObject = JsonConvert.SerializeObject(inst),
                                FailedReason = "Instition already exist in database."
                            });
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CanAddToContext(string name, int legacySiteId)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.Institutions.Any(a => a.Name == name && a.LegacySiteId == legacySiteId)) ? false : true;
            }
        }
    }
}
