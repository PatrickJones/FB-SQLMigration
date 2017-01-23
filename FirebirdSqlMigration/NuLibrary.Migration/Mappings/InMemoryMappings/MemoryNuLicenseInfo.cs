using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.InMemoryMappings
{
    public static class MemoryNuLicenseInfo
    {
        // Item1 = site id
        // Item2 = number of licenses
        // Item3 = Expiration date
        public static List<Tuple<int, int, DateTime>> NuLicenses = new List<Tuple<int, int, DateTime>>();

    }
}
