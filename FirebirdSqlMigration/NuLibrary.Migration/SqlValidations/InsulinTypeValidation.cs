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
    public class InsulinTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public List<NuLibrary.Migration.SQLDatabase.EF.InsulinType> DefaultInsulinTypes = new List<NuLibrary.Migration.SQLDatabase.EF.InsulinType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.InsulinType> Missing = new List<NuLibrary.Migration.SQLDatabase.EF.InsulinType>();

        public InsulinTypeValidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
            Init();
        }
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
                DefaultInsulinTypes.Add(
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
            foreach (var ut in DefaultInsulinTypes)
            {
                if (!db.InsulinTypes.Any(a => a.Type.ToLower() == ut.Type.ToLower()))
                {
                    Missing.Add(ut);
                }
            }

            return (Missing.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.InsulinTypes.AddRange(Missing);
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
