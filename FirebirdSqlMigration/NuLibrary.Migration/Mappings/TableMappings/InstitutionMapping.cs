using Newtonsoft.Json;
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

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class InstitutionMapping : IContextHandler
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();
        MappingUtilities mu = new MappingUtilities();

        public ICollection<Institution> CompletedMappings = new List<Institution>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreateInstitutionMapping()
        {
            try
            {
                var dataSet = aHelper.GetAllCorporationInfo();
                RecordCount = dataSet.Count;

                foreach (var ins in dataSet)
                {
                    var inst = new Institution
                    {
                        InstitutionId = Guid.NewGuid(),
                        Name = ins.Site_Name,
                        LegacySiteId = (ins.SiteId.HasValue) ? ins.SiteId.Value : 0
                    };

                    MemoryMappings.AddInstitution(inst);

                    if (CanAddToContext(inst.Name, inst.LegacySiteId))
                    {
                        CompletedMappings.Add(inst);
                    }
                    else
                    {
                        MappingStatistics.LogFailedMapping("Institutions", typeof(Institution), JsonConvert.SerializeObject(inst), "Instition already exist in database.");
                        FailedCount++;
                    }
                }

                MappingStatistics.LogMappingStat("None", 0, "Institutions", RecordCount, CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Institution mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "Institutions",
                    PreSaveCount = CompletedMappings.Count()
                };

                TransactionManager.DatabaseContext.Institutions.AddRange(CompletedMappings);
                TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = TransactionManager.DatabaseContext.Institutions.Count();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating Institution entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Institution entity", e);
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
