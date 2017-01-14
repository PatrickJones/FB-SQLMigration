using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with - 
    /// </summary>
    public class PatientsMapping : BaseMapping
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

        public void CreatePatientMapping()
        {
            try
            {
                foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
                {
                    //aspnet_Membership member;
                    //aspnet_Users aspUser;
                    //UserAuthentication uAuth = null;
                    User user = new User();
                    Guid instId = nHelper.GetInstitutionId(MigrationVariables.CurrentSiteId);

                    // get userid from old aspnetdb matching on patientid #####.#####
                    // if no userid then create new one for this patient
                    var patId = row["KEYID"].ToString();
                    var uid = aHelper.GetUserIdFromPatientId(patId);
                    var userId = (uid != Guid.Empty) ? uid : Guid.NewGuid();
                    // must create clinipro user to store new userid for future usage
                    if (uid == Guid.Empty)
                    {
                        aHelper.CreateCliniProUser(userId, patId);

                        user.UserId = userId;
                        user.UserType = (int)UserType.Patient;
                        user.CreationDate = DateTime.Now;

                        TransactionManager.DatabaseContext.Users.Add(user);
                        TransactionManager.DatabaseContext.SaveChanges();
                    }
                    //else
                    //{
                    //    //only create authentication record if user has current login
                    //    var appId = nHelper.GetApplicationId("Diabetes Partner");
                    //    member = aHelper.GetMembershipInfo(uid);
                    //    aspUser = aHelper.GetAspUserInfo(uid);
                    //    if (member != null)
                    //    {
                    //        uAuth = new UserAuthentication {
                    //            ApplicationId = appId,
                    //            UserId = uid,
                    //            Username = aspUser.UserName,
                    //            Password = member.Password,
                    //            PasswordQuestion = member.PasswordQuestion,
                    //            PasswordAnswer = member.PasswordAnswer,
                    //            PasswordAnswerFailureCount = member.FailedPasswordAnswerAttemptCount,
                    //            PasswordFailureCount = member.FailedPasswordAttemptCount,
                    //            LastActivityDate = aspUser.LastActivityDate,
                    //            LastLockOutDate = member.LastLockoutDate,
                    //            IsApproved = member.IsApproved,
                    //            IsLockedOut = member.IsLockedOut,
                    //            IsTempPassword = member.IsTemp,
                    //            IsloggedIn = false
                    //        };
                    //    }
                    //}

                    //var user = new User {
                    //    UserId = userId,
                    //    UserType = (int)UserType.Patient,
                    //    CreationDate = DateTime.Now
                    //};

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
                        InstitutionId = instId
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
                        Country = (row["COUNTRY"] is DBNull) ? String.Empty : row["COUNTRY"].ToString()
                    };

                    //if (uAuth != null)
                    //{
                    //    user.UserAuthentications.Add(uAuth);
                    //}

                    pat.PatientAddresses.Add(adr);
                    //user.Patient = pat;

                    if (CanAddToContext(user.UserId))
                    {
                        TransactionManager.DatabaseContext.Patients.Add(pat);
                    }
                    else
                    {
                        TransactionManager.FailedMappingCollection
                            .Add(new FailedMappings
                            {
                                Tablename = "Patients",
                                ObjectType = typeof(Patient),
                                JsonSerializedObject = JsonConvert.SerializeObject(user),
                                FailedReason = "Patient already exist in database."
                            });
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Patient mapping.", e);
            }
        }

        private bool CanAddToContext(Guid userId)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.Patients.Any(a => a.UserId == userId)) ? false : true;
            }
        }
    }
}
