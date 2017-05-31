using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class MigrationHistoryHelpers : DatabaseContextDisposal
    {
        MigrationHistoryEntities db = new MigrationHistoryEntities();
        AspnetDbHelpers ahelper = new AspnetDbHelpers();

        public void LogMigration()
        {
            DateTime date = DateTime.Now;
            var dh = new DatabaseHistory {
                FbConnectionStringUsed = MigrationVariables.FbConnectionString,
                InstitutionName = ahelper.GetAllCorporationInfo().Where(c => c.SiteId == MigrationVariables.CurrentSiteId).Select(s => s.Site_Name).FirstOrDefault(),
                LastMigrationDate = date,
                MigrationLog = MappingStatistics.ExportToLog(),
                SiteId = MigrationVariables.CurrentSiteId
            };

            Array.ForEach(MigrationVariables.FirebirdTableNames.ToArray(), t => {
                if (dh.TableHistories.Any(a => string.Equals(a.TableName, t, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var th = dh.TableHistories.First(f => string.Equals(f.TableName, t, StringComparison.CurrentCultureIgnoreCase));
                    th.FirebirdRecordCount = +MappingStatistics.MappingStats.Where(w => string.Equals(w.FBTableName, t, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.FBRecordCount).FirstOrDefault();
                    th.MigratedRecordCount = +MappingStatistics.MappingStats.Where(w => string.Equals(w.FBTableName, t, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.CompletedMappingsCount).FirstOrDefault();
                }
                else
                {
                    dh.TableHistories.Add(new TableHistory
                    {
                        TableName = t,
                        LastMigrationDate = date,
                        FirebirdRecordCount = MappingStatistics.MappingStats.Where(w => string.Equals(w.FBTableName, t, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.FBRecordCount).FirstOrDefault(),
                        MigratedRecordCount = MappingStatistics.MappingStats.Where(w => string.Equals(w.FBTableName, t, StringComparison.CurrentCultureIgnoreCase)).Select(s => s.CompletedMappingsCount).FirstOrDefault()
                    });
                }
            });

            Array.ForEach(TransactionManager.DatabaseContext.Patients.ToArray(), p => {
                var pat = MemoryMappings.GetAllPatientInfo().FirstOrDefault(f => f.Item3 == p.UserId);
                if (pat != null)
                {
                    dh.PatientHistories.Add(new PatientHistory
                    {
                        BirthDate = p.DateofBirth,
                        FirebirdPatientId = pat.Item2,
                        Firstname = p.Firstname,
                        Lastname = p.Lastname,
                        SqlUserId = pat.Item3,
                        MigrationDate = date,
                    });
                }
            });

            Array.ForEach(TransactionManager.DatabaseContext.UserAuthentications.ToArray(), p =>
            {
                var userInfo = ahelper.GetAspUserInfo(p.Username);
                Guid legId = (userInfo == null) ? Guid.Empty : userInfo.UserId;

                dh.UserHistories.Add(new UserHistory
                {
                    SqlUserId = p.UserId,
                    MigrationDate = date,
                    Username = p.Username,
                    LegacyUserId = legId
                });
            });

            AddDatabaseMigration(dh);
        }

        public bool HasPreviousMigration => db.DatabaseHistories.Any(a => a.SiteId == MigrationVariables.CurrentSiteId);

        public bool HasUserMigrated(string username, Guid legacyUserid)
        {
            return db.UserHistories.Any(a => a.Username == username && a.LegacyUserId == legacyUserid);
        }

        public bool HasPatientMigrated(string fbPatientId)
        {
            return db.PatientHistories.Any(a => a.FirebirdPatientId == fbPatientId);
        }

        public ICollection<DatabaseHistory> GetMigrationHistory()
        {
            return db.DatabaseHistories.Include("TableHistories").Include("PatientHistories").ToList();
        }

        public int AddDatabaseMigration(DatabaseHistory dHistory)
        {
            //var dh = db.DatabaseHistories.FirstOrDefault(f => f.SiteId == dHistory.SiteId);
            //if (dh == null)
            //{
            //    db.DatabaseHistories.Add(dHistory);
            //}
            //else
            //{
            //    dh.FbConnectionStringUsed = dHistory.FbConnectionStringUsed;
            //    dh.InstitutionName = dHistory.InstitutionName;
            //    dh.LastMigrationDate = DateTime.Now;
            //    dh.MigrationLog = dHistory.MigrationLog;

            //    Array.ForEach(dHistory.PatientHistories.ToArray(), p => AddPatientMigration(p, dh.SiteId));
            //    Array.ForEach(dHistory.TableHistories.ToArray(), t => AddTableMigration(t, dh.SiteId));
            //}
            db.DatabaseHistories.Add(dHistory);
            return db.SaveChanges();
        }

        public int AddTableMigration(TableHistory tHistory, int siteId)
        {
            int result = 0;
            if (siteId != 0)
            {
                var dh = db.DatabaseHistories.FirstOrDefault(f => f.SiteId == siteId);
                if (dh != null)
                {
                    var tb = dh.TableHistories.FirstOrDefault(f => f.TableName == tHistory.TableName);
                    if (tb == null)
                    {
                        dh.TableHistories.Add(tHistory);
                    }
                    else
                    {
                        tb.FirebirdRecordCount = tHistory.FirebirdRecordCount;
                        tb.MigratedRecordCount = tHistory.MigratedRecordCount;
                        tb.LastMigrationDate = DateTime.Now;
                    }

                    result = db.SaveChanges();
                }
            }

            return result;
        }

        public int AddPatientMigration(PatientHistory pHistory, int siteId)
        {
            int result = 0;
            if (siteId != 0)
            {
                var dh = db.DatabaseHistories.FirstOrDefault(f => f.SiteId == siteId);
                if (dh != null)
                {
                    var pat = dh.PatientHistories.FirstOrDefault(f => f.FirebirdPatientId == pHistory.FirebirdPatientId);
                    if (pat == null)
                    {
                        // can only add newly migrated patients - should not be a need to updated existing patient
                        dh.PatientHistories.Add(pHistory);
                        result = db.SaveChanges();
                    }
                }
            }

            return result;
        }

        public ICollection<DatabaseHistory> GetDatabaseHistories(int siteId)
        {
            return db.DatabaseHistories.Where(f => f.SiteId == siteId).ToList();
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
