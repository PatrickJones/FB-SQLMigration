using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class ApplicationValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<Application> defApps = new List<Application>();
        public List<Application> Missing = new List<Application>();

        public ApplicationValidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
        }

        public ApplicationValidation()
        {
            Init();
        }

        private void Init()
        {
            defApps.Add(
                new Application {
                    ApplicationId = Guid.Parse("DFE4EB52-401E-42DA-B7A0-D801749446A0"),
                    ApplicationName = "Administration",
                    Description = "Administration website"
                });

            defApps.Add(
                new Application
                {
                    ApplicationId = Guid.Parse("4AE4C176-C4A2-4C36-9866-FCDE16FF3AFA"),
                    ApplicationName = "CliniPro-Web",
                    Description = "CliniPro-Web site"
                });

            defApps.Add(
                new Application
                {
                    ApplicationId = Guid.Parse("5E1A0790-68AA-405D-908A-4AB578832EFE"),
                    ApplicationName = "Diabetes Partner",
                    Description = "Diabetes Partner site"
                });

            defApps.Add(
                new Application
                {
                    ApplicationId = Guid.Parse("05475E32-875A-4FED-B33F-34A1E6FE660F"),
                    ApplicationName = "OmniPod Partner",
                    Description = "OmniPod Partner site"
                });
        }

        public string TableName
        {
            get
            {
                return "Applications";
            }
        }


        public bool ValidateTable()
        {
            foreach (var app in defApps)
            {
                if (!db.Applications.Any(a => a.ApplicationId == app.ApplicationId && a.ApplicationName.ToLower() == app.ApplicationName.ToLower()))
                {
                    Missing.Add(app);
                }
            }

            return (Missing.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.Applications.AddRange(Missing);
            db.SaveChanges();
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
