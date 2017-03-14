using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase
{
    public class SqlPurge : ClientDatabaseBase
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public SqlPurge()
        {
            PurgeUsers();
        }

        /// <summary>
        /// purge users that have UserId with no corresponding patient
        /// </summary>
        private void PurgeUsers()
        {
            db.Users.RemoveRange(db.Users.Where(w => w.Clinician == null && w.Patient == null));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
