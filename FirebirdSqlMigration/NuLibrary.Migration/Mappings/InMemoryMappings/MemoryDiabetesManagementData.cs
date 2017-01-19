using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.InMemoryMappings
{
    public class MemoryDiabetesManagementData
    {
        public static ICollection<DiabetesManagementData> DMDataCollection = new List<DiabetesManagementData>();
    }
}
