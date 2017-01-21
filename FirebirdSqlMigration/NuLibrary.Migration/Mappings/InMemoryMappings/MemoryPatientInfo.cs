using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.InMemoryMappings
{
    public static class MemoryPatientInfo
    {
        // item1 = site id
        // item2 = patient id
        // item3 = user id
        private static List<Tuple<int, string, Guid>> patientInfo = new List<Tuple<int, string, Guid>>();

        public static Guid GetUserId(int siteId, string patientId)
        {
            return patientInfo.Where(w => w.Item1 == siteId && w.Item2 == patientId).Select(s => s.Item3).FirstOrDefault();
        }

        public static void AddPatientInfo(int siteId, string patientId, Guid userId)
        {
            if (siteId != 0 && !String.IsNullOrEmpty(patientId) && userId != Guid.Empty)
            {
                var tup = new Tuple<int, string, Guid>(siteId, patientId, userId);
                if (!patientInfo.Contains(tup))
                {
                    patientInfo.Add(tup);
                }
            }
        }

        public static int Count()
        {
            return patientInfo.Count;
        }
    }
}
