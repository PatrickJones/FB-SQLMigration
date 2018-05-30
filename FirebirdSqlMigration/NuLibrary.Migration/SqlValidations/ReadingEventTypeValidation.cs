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
    public class ReadingEventTypeValidation : ClientDatabaseBase, ITableValidate
    {
        private NuMedicsGlobalEntities db = new NuMedicsGlobalEntities();

        public List<NuLibrary.Migration.SQLDatabase.EF.ReadingEventType> DefaultReadingEventTypes = new List<NuLibrary.Migration.SQLDatabase.EF.ReadingEventType>();
        public List<NuLibrary.Migration.SQLDatabase.EF.ReadingEventType> Misssing = new List<NuLibrary.Migration.SQLDatabase.EF.ReadingEventType>();

        public ReadingEventTypeValidation(DbContext context)
        {
            db = (NuMedicsGlobalEntities)context;
            Init();
        }

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
                "TimeChange",
                "Download",
                "AlarmHistory",
                "Occlusion",
                "PumpHazard",
                "PumpAdvise"
            };

            Array.ForEach(typeArr, a => {
                DefaultReadingEventTypes.Add(
                new NuLibrary.Migration.SQLDatabase.EF.ReadingEventType
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
            foreach (var ut in DefaultReadingEventTypes)
            {
                if (!db.ReadingEventTypes.Any(a => a.EventName.ToLower() == ut.EventName.ToLower()))
                {
                    Misssing.Add(ut);
                }
            }

            return (Misssing.Count == 0) ? true : false;
        }

        public void SyncTable()
        {
            db.ReadingEventTypes.AddRange(Misssing);
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
