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
            foreach (var ins in aHelper.GetAllCorporationInfo())
            {
                var inst = new Institution
                {
                    InstitutionId = Guid.NewGuid(),
                    Name = ins.Site_Name,
                    LegacySiteId = (ins.SiteId.HasValue) ? ins.SiteId.Value : 0
                };

                TransactionManager.DatabaseContext.Institutions.Add(inst);
            }
        }
    }
}
