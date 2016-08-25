using NuLibrary.Migration.FBDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.GlobalVar
{
    public static class MigrationVariables
    {
        public static int SiteId { get; set; }
        public static ICollection<string> FirebirdTableNames {get; set;}

        static MigrationVariables()
        {
            var fba = new FBDataAccess();
            FirebirdTableNames = fba.GetTableNames();
        }
    }
}
