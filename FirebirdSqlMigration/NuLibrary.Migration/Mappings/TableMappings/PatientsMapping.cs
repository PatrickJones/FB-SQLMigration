using NuLibrary.Migration.FBDatabase.FBTables;
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

        public void CreatePatientMapping()
        {
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                aspnet_Membership member;
                aspnet_Users aspUser;
                UserAuthentication uAuth = null;

                // get userid from old aspnetdb matching on patientid #####.#####
                // if no userid then create new one for this patient
                var patId = (String)row["KEYID"];
                var uid = aHelper.GetUserIdFromPatientId(patId);
                var userId = (uid != Guid.Empty) ? uid : new Guid();
                // must create clinipro user to store new userid for future usage
                if (uid == Guid.Empty)
                {
                    aHelper.CreateCliniProUser(userId, patId);
                }
                else
                {
                    var appId = nHelper.GetApplicationId("Diabetes Partner");
                    member = aHelper.GetMembershipInfo(uid);
                    aspUser = aHelper.GetAspUserInfo(uid);
                    if (member != null)
                    {
                        uAuth = new UserAuthentication {
                            ApplicationId = appId,
                            UserId = uid,
                            Username = aspUser.UserName,
                            Password = member.Password,
                            PasswordQuestion = member.PasswordQuestion,
                            PasswordAnswer = member.PasswordAnswer,
                            PasswordAnswerFailureCount = member.FailedPasswordAnswerAttemptCount,
                            PasswordFailureCount = member.FailedPasswordAttemptCount,
                            LastActivityDate = aspUser.LastActivityDate,
                            LastLockOutDate = member.LastLockoutDate,
                            IsApproved = member.IsApproved,
                            IsLockedOut = member.IsLockedOut,
                            IsTempPassword = member.IsTemp,
                            IsloggedIn = false
                        };

                    }
                }



                var user = new User {
                    UserId = userId,
                    UserType = (int)UserType.Patient,
                    CreationDate = DateTime.Now
                };


                var pat = new Patient
                {
                    UserId = userId,
                    MRID = (String)row["MEDICALRECORDIDENTIFIER"],
                    Firstname = (String)row["FIRSTNAME"],
                    Lastname = (String)row["LASTNAME"],
                    Middlename = (String)row["MIDDLENAME"],
                    Gender = (Int32)row["GENDER"],
                    DateofBirth = (DateTime)row["DOB"],
                    Email = (String)row["EMAIL"]
                };

                var adr = new PatientAddress {
                    Street1 = (String)row["STREET1"],
                    Street2 = (String)row["STREET2"],
                    Street3 = (String)row["STREET3"],
                    City = (String)row["CITY"],
                    County = (String)row["COUNTY"],
                    State = (String)row["STATE"],
                    Zip = (String)row["ZIP"],
                    Country = (String)row["COUNTRY"]
                };


                if (uAuth != null)
                {
                    user.UserAuthentications.Add(uAuth);
                }

                pat.PatientAddresses.Add(adr);
                user.Patient = pat;

                TransactionManager.DatabaseContext.Users.Add(user);
            }
        }
    }
}
