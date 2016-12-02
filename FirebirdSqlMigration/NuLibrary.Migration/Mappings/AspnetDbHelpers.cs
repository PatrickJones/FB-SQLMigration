using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class AspnetDbHelpers : ClientDatabaseBase
    {
        AspnetDbEntities db = new AspnetDbEntities();

        public Guid GetUserIdFromPatientId(string patientId)
        {
            var user = db.clinipro_Users.Where(c => c.CliniProID == patientId).FirstOrDefault();
            return (user != null) ? user.UserId : Guid.Empty;
        }
        internal void CreateCliniProUser(Guid userId, string patientId)
        {
            var cpUser = new clinipro_Users { UserId = userId, CliniProID = patientId };
            db.clinipro_Users.Add(cpUser);
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
