using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class AspnetDbHelpers : DatabaseContextDisposal
    {
        AspnetDbEntities db = new AspnetDbEntities();

        public AspnetDbHelpers(DbContext context)
        {
            db = (AspnetDbEntities)context;
        }

        public AspnetDbHelpers()
        {

        }

        public Guid GetUserIdFromPatientId(string patientId)
        {
            return db.clinipro_Users.Where(c => c.CliniProID == patientId).Select(s => s.UserId).FirstOrDefault();
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

        public ICollection<clinipro_Users> GetAllAdminsUsers()
        {
            return db.clinipro_Users.Where(w => w.CliniProID.ToLower() == "admin").ToList();
        }

        public ICollection<clinipro_Users> GetAllPatientUsers()
        {
            return db.clinipro_Users.Where(w => w.CliniProID.ToLower() != "admin").ToList();
        }

        public ICollection<clinipro_Users> GetAllUsers()
        {
            return db.clinipro_Users.ToList();
        }

        public SubscriptionHandler GetSubscriptionInfo(Guid userId)
        {
            var sh = new SubscriptionHandler(userId);

            sh.Adjustments = db.subs_Adjustments.Where(w => w.UserId == userId).ToList();
            sh.CheckPayments = db.subs_CheckPayments.Where(w => w.UserId == userId).ToList();
            sh.PayPalPayments = db.subs_PayPalPayments.Where(w => w.UserId == userId).ToList();
            sh.Trials = db.subs_Trials.Where(w => w.UserId == userId).ToList();
            sh.Subscriptions = db.subs_Subscriptions.Where(w => w.UserId == userId).ToList();

            return sh;
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
