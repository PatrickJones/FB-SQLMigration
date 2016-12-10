﻿using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class UserTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<NuLibrary.Migration.SQLDatabase.EF.UserType> defUserTypes = new List<NuLibrary.Migration.SQLDatabase.EF.UserType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.UserType> Failures = new List<NuLibrary.Migration.SQLDatabase.EF.UserType>();

        public UserTypeValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "Clinician",
                "Patient",
                "Admin"
            };

            Array.ForEach(typeArr, a => {
                defUserTypes.Add(
                new NuLibrary.Migration.SQLDatabase.EF.UserType
                {
                    TypeName = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "UserTypes";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in defUserTypes)
            {
                if (!db.UserTypes.Any(a => a.TypeName.ToLower() == ut.TypeName.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.UserTypes.AddRange(Failures);
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