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

        public void Purge()
        {
            PurgeUsers();
        }

        /// <summary>
        /// purge users that have UserId with no corresponding patient
        /// </summary>
        private void PurgeUsers()
        {
            var remove = db.Users.Include("AssignedUserTypes").Include("UserAuthentications").Where(w => w.Clinician == null && w.Patient == null);
            Array.ForEach(remove.ToArray(), r => db.UserAuthentications.RemoveRange(r.UserAuthentications));
            db.Users.RemoveRange(remove);

            Save();
        }

        private int Save()
        {
            try
            {
                return db.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }
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
