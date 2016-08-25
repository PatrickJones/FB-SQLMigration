using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class BaseMapping
    {
        public int SiteId { get; set; }
        public BaseMapping(int siteId)
        {
            SiteId = siteId;
        }
    }
}
