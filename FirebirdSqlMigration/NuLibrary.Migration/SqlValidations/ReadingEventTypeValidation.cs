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
    public class ReadingEventTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        List<ReadingEventType> defReadingEventTypes = new List<ReadingEventType>();
        public List<ReadingEventType> Failures = new List<ReadingEventType>();

        public ReadingEventTypeValidation()
        {
            Init();
        }

        private void Init()
        {
            string[] typeArr = {
                "Alarm",
                "DateChange",
                "RemoteHazard",
                "Activate",
                "Deactivate",
                "Suspend",
                "Resume",
                "TimeChange"
            };

            Array.ForEach(typeArr, a => {
                defReadingEventTypes.Add(
                new ReadingEventType
                {
                    EventName = a
                });
            });
        }

        public string TableName
        {
            get
            {
                return "ReadingEventTypes";
            }
        }


        public bool ValidateTable()
        {
            foreach (var ut in defReadingEventTypes)
            {
                if (!db.ReadingEventTypes.Any(a => a.EventName.ToLower() == ut.EventName.ToLower()))
                {
                    Failures.Add(ut);
                }
            }

            return (Failures.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.ReadingEventTypes.AddRange(Failures);
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
