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
    /// Note: Has relationship with Patients Table 1:M
    /// </summary>
    public class PatientPhoneNumbersMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public PatientPhoneNumbersMapping() : base("PHONENUMBERS")
        {

        }

        public PatientPhoneNumbersMapping(string tableName) :base(tableName)
        {

        }

        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        MigrationHistoryHelpers mHelper = new MigrationHistoryHelpers();
        MappingUtilities map = new MappingUtilities();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

        public ICollection<PatientPhoneNumber> CompletedMappings = new List<PatientPhoneNumber>();

        public int RecordCount = 0;
        public int FailedCount = 0;
        
        public void CreatePatientPhoneNumbersMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PARENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

                    if (!mHelper.HasPatientMigrated(patId))
                    {
                        var patNum = new PatientPhoneNumber
                        {
                            UserId = userId,
                            Number = (row["NUMBER"] is DBNull) ? String.Empty : row["NUMBER"].ToString(),
                            Extension = (row["EXTENSION"] is DBNull) ? String.Empty : row["EXTENSION"].ToString(),
                            Type = (row["ATYPE"] is DBNull) ? 0 : map.ParseFirebirdPhoneTypes(row["ATYPE"].ToString()),
                            IsPrimary = (row["ISPRIMARY"] is DBNull) ? false : map.ParseFirebirdBoolean(row["ISPRIMARY"].ToString()),
                            RecieveText = (row["RECEIVETEXT"] is DBNull) ? false : map.ParseFirebirdBoolean(row["RECEIVETEXT"].ToString()),
                            LastUpdatedByUser = userId
                        };

                        if (userId != Guid.Empty && CanAddToContext(patNum.Number))
                        {
                            CompletedMappings.Add(patNum);
                        }
                        else
                        {
                            var fr = (userId == Guid.Empty) ? "Phone number has no corresponding patient." : "Patient phone number already exist in database.";

                            MappingStatistics.LogFailedMapping("PHONENUMBERS", row["KEYID"].ToString(), "PatientPhoneNumbers", typeof(PatientPhoneNumber), JsonConvert.SerializeObject(patNum), fr);
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("PHONENUMBERS", RecordCount, "PatientPhoneNumbers", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating PatientPhonenumber mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "PatientPhoneNumbers",
                    PreSaveCount = CompletedMappings.Count()
                };

                // Ensure pateint id exist (has been commited) before updating database
                var q = from cm in CompletedMappings
                        from ps in map.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.PatientPhoneNumbers.AddRange(q);
                stats.PreSaveCount = TransactionManager.DatabaseContext.ChangeTracker.Entries<PatientPhoneNumber>().Where(w => w.State == System.Data.Entity.EntityState.Added).Count();
                stats.PostSaveCount = TransactionManager.DatabaseContext.SaveChanges();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating PatientPhonenumber entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving PatientPhonenumber entity", e);
            }
        }

        private bool CanAddToContext(string phoneNumber)
        {
            if (String.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                return !ctx.PatientPhoneNumbers.Any(a => a.Number == phoneNumber);
            }
        }
    }
}
