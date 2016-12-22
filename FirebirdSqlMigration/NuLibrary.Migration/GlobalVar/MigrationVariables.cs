using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.GlobalVar
{
    public static class MigrationVariables
    {
        static int currSiteId;
        public static int CurrentSiteId
        {
            get
            {
                return currSiteId;
            }
            set
            {
                currSiteId = value;
                ReloadTableNames();
            }
        }

        public static ICollection<int> SiteIds = new List<int>();
        public static ICollection<string> FirebirdTableNames = new List<string>();

        public static void Init()
        {
            if (SiteIds.Count == 0)
            {
                AspnetDbHelpers help = new AspnetDbHelpers();
                SiteIds = help.GetAllFirebirdConnections().Where(w => w.SiteId.HasValue).Select(s => s.SiteId.Value).ToList();
            }
        }

        public static void ReloadTableNames()
        {
            if (CurrentSiteId != 0)
            {
                FBDataAccess fba = new FBDataAccess();
                FirebirdTableNames = fba.GetTableNames();
            }
            else
            {
                FirebirdTableNames.Clear();
            }
        }
    }
}
