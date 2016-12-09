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
    public class InsulinTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<NuLibrary.Migration.SQLDatabase.EF.InsulinType> defInsulinTypes = new List<NuLibrary.Migration.SQLDatabase.EF.InsulinType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.InsulinType> Failures = new List<NuLibrary.Migration.SQLDatabase.EF.InsulinType>();

        public InsulinTypeValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "Rapid Acting",
                "Short Acting",
                "Intermediate Acting",
                "Long Acting",
                "PreMixed"
            };

            Array.ForEach(typeArr, a => {
                defInsulinTypes.Add(
                new NuLibrary.Migration.SQLDatabase.EF.InsulinType
                {
                    Type = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "InsulinTypes";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in defInsulinTypes)
            {
                if (!db.InsulinTypes.Any(a => a.Type.ToLower() == ut.Type.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.InsulinTypes.AddRange(Failures);
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
