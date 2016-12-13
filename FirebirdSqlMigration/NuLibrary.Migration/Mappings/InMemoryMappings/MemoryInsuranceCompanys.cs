using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.InMemoryMappings
{
    public static class MemoryInsuranceCompanys
    {
        // key = firebird keyid for company
        // value = company name
        public static Dictionary<string, string> Companies = new Dictionary<string, string>();
    }
}
