using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
                UpdateVariables();
            }
        }

        private static void UpdateVariables()
        {
            MigrationHistoryHelpers mig = new MigrationHistoryHelpers();
            var dh = mig.GetDatabaseHistories(currSiteId);

            AspnetDbHelpers ah = new AspnetDbHelpers();
            var corp = ah.GetAllCorporationInfo().FirstOrDefault(f => f.SiteId == currSiteId);

            Institution = corp?.Site_Name;
            InitialMigration = (dh.Count == 0) ? "No Migration" : dh.OrderBy(o => o.LastMigrationDate).Select(s => s.LastMigrationDate.ToString()).First();
            LastMigration = (dh.Count == 0) ? "No Migration" : dh.OrderBy(o => o.LastMigrationDate).Select(s => s.LastMigrationDate.ToString()).Last();
        }
        public static string Institution { get; set; }
        public static string InitialMigration { get; set; }
        public static string LastMigration { get; set; }


        static int dataHistory = 90;
        public static int DataHistoryRange
        {
            get
            {
                return dataHistory;
            }
            set
            {
                dataHistory = value;
            }
        }


        public static ICollection<int> SiteIds = new List<int>();
        public static ICollection<string> FirebirdTableNames = new List<string>();
        public static string FbConnectionString = String.Empty;
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
                var dbConn = fba.GetConnnection();

                FbConnectionString = dbConn.ConnectionString;
                FirebirdTableNames = fba.GetTableNames();
            }
            else
            {
                FirebirdTableNames.Clear();
            }
        }

        public static ICollection<string> GetRangeDates()
        {
            return new List<string> { "All", "1 Month", "2 Months", "3 Months", "6 Months", "1 Year", "2 Years", "3 Years" };
        }
    }
}
