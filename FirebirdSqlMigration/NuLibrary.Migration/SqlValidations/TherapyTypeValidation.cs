using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class TherapyTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<NuLibrary.Migration.SQLDatabase.EF.TherapyType> defTherapyTypes = new List<NuLibrary.Migration.SQLDatabase.EF.TherapyType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.TherapyType> Failures = new List<NuLibrary.Migration.SQLDatabase.EF.TherapyType>();

        public TherapyTypeValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "Scheduled",
                "OnDemand"
            };

            Array.ForEach(typeArr, a => {
                defTherapyTypes.Add(
                new NuLibrary.Migration.SQLDatabase.EF.TherapyType
                {
                    TypeName = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "TherapyTypes";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in defTherapyTypes)
            {
                if (!db.TherapyTypes.Any(a => a.TypeName.ToLower() == ut.TypeName.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.TherapyTypes.AddRange(Failures);
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
