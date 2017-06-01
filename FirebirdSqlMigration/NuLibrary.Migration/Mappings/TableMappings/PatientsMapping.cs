using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PatientsMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PatientsMapping() : base("PATIENTS")
        {

        }

        public PatientsMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();
        MappingUtilities mu = new MappingUtilities();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();

        public ICollection<Patient> CompletedMappings = new List<Patient>();
        private ICollection<User> newUsers = new List<User>();

        public int RecordCount = 0;
        public int FailedCount = 0;
        
        public void CreatePatientMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    User user = new User();
                    Guid instId = nHelper.GetInstitutionId(MigrationVariables.CurrentSiteId);

                    // get userid from old aspnetdb matching on patientid #####.#####
                    // if no userid then create new one for this patient
                    var patId = row["KEYID"].ToString();
                    var uid = aHelper.GetUserIdFromPatientId(patId);
                    var userId = (uid != Guid.Empty) ? uid : Guid.NewGuid();
                    userId = nHelper.ValidGuid(userId);

                    if (mHelper.HasPatientMigrated(patId))
                    {
                        MappingStatistics.LogFailedMapping("PATIENTS", patId, "Patients", typeof(Patient), String.Empty, "Patient previously migrated.");
                        FailedCount++;
                    }
                    else
                    {
                        var pat = new Patient
                        {
                            UserId = userId,
                            MRID = (row["MEDICALRECORDIDENTIFIER"] is DBNull) ? String.Empty : row["MEDICALRECORDIDENTIFIER"].ToString(),
                            Firstname = (row["FIRSTNAME"] is DBNull) ? String.Empty : row["FIRSTNAME"].ToString(),
                            Lastname = (row["LASTNAME"] is DBNull) ? String.Empty : row["LASTNAME"].ToString(),
                            Middlename = (row["MIDDLENAME"] is DBNull) ? String.Empty : row["MIDDLENAME"].ToString(),
                            Gender = (row["GENDER"] is DBNull) ? 1 : (row["GENDER"].ToString().ToLower().StartsWith("m", StringComparison.CurrentCulture)) ? 2 : 3, //From the GlobalStandards database, 'Gender' table
                            DateofBirth = (row["DOB"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["DOB"].ToString()),
                            Email = (row["EMAIL"] is DBNull) ? String.Empty : row["EMAIL"].ToString(),
                            InstitutionId = instId,
                            LastUpdatedByUser = userId
                        };

                        var adr = new PatientAddress
                        {
                            Street1 = (row["STREET1"] is DBNull) ? String.Empty : row["STREET1"].ToString(),
                            Street2 = (row["STREET2"] is DBNull) ? String.Empty : row["STREET2"].ToString(),
                            Street3 = (row["STREET3"] is DBNull) ? String.Empty : row["STREET3"].ToString(),
                            City = (row["CITY"] is DBNull) ? String.Empty : row["CITY"].ToString(),
                            County = (row["COUNTY"] is DBNull) ? String.Empty : row["COUNTY"].ToString(),
                            State = (row["STATE"] is DBNull) ? String.Empty : row["STATE"].ToString(),
                            Zip = (row["ZIP"] is DBNull) ? String.Empty : row["ZIP"].ToString(),
                            Country = (row["COUNTRY"] is DBNull) ? String.Empty : row["COUNTRY"].ToString(),
                            LastUpdatedByUser = userId
                        };

                        pat.PatientAddresses.Add(adr);

                        // must create clinipro user to store new userid for future usage
                        if (uid == Guid.Empty || uid != userId)
                        {
                            aHelper.CreateCliniProUser(userId, patId);

                            user.UserId = userId;
                            user.UserType = (int)UserType.Patient;
                            user.CreationDate = DateTime.Now;

                            pat.User = user;
                        }

                        // add patient info to in-memery collection for use throughout application
                        MemoryMappings.AddPatientInfo(MigrationVariables.CurrentSiteId, patId, pat.UserId);

                        if (CanAddToContext(user.UserId))
                        {
                            CompletedMappings.Add(pat);
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("PATIENTS", patId, "Patients", typeof(Patient), JsonConvert.SerializeObject(user), "Patient already exist in database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("PATIENTS", RecordCount, "Patients", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Patient mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "Patients",
                    PreSaveCount = CompletedMappings.Count()
                };

                ////Set instition id for each patient
                var institution = TransactionManager.DatabaseContext.Institutions.FirstOrDefault(f => f.LegacySiteId == MigrationVariables.CurrentSiteId);
                Parallel.ForEach(CompletedMappings, c => c.Institutions.Add(institution));

                TransactionManager.DatabaseContext.Patients.AddRange(CompletedMappings);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<Patient>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                stats.PostSaveCount = TransactionManager.DatabaseContext.SaveChanges();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating Patients entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving Patients entity", e);
            }
        }

        private bool CanAddToContext(Guid userId)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return !ctx.Patients.Any(a => a.UserId == userId);
            }
        }
    }
}
