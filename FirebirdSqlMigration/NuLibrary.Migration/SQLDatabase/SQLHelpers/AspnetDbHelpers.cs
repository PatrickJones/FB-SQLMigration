using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class AspnetDbHelpers : DatabaseContextDisposal
    {
        AspnetDbEntities db = new AspnetDbEntities();

        public Guid GetUserIdFromPatientId(string patientId)
        {
            var user = db.clinipro_Users.Where(c => c.CliniProID == patientId).FirstOrDefault();
            return (user != null) ? user.UserId : Guid.Empty;
        }

        public ICollection<CorporationsView> GetAllCorporationInfo()
        {
            return db.CorporationsViews.ToList();
        }

        public void CreateCliniProUser(Guid userId, string patientId)
        {
            var cpUser = new clinipro_Users { UserId = userId, CliniProID = patientId };
            db.clinipro_Users.Add(cpUser);
        }

        public string GetCorporationName(int? cPSiteId)
        {
            return (cPSiteId.HasValue) ? db.CorporationsViews.Where(w => w.SiteId == cPSiteId.Value).Select(s => s.Corp_Name).FirstOrDefault() : String.Empty;
        }

        public aspnet_Membership GetMembershipInfo(Guid userid)
        {
            return db.aspnet_Membership.Where(w => w.UserId == userid).FirstOrDefault();
        }

        public aspnet_Users GetAspUserInfo(Guid userId)
        {
            return db.aspnet_Users.Where(w => w.UserId == userId).FirstOrDefault();
        }

        public ICollection<FirebirdConnection> GetAllFirebirdConnections()
        {
            return db.FirebirdConnections.ToList();
        }

        public ICollection<clinipro_Users> GetAllAdmins()
        {
            return db.clinipro_Users.Where(w => w.CliniProID.ToLower() == "admin").ToList();
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
