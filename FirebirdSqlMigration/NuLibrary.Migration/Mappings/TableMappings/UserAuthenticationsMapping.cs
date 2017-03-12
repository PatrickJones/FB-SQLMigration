using Newtonsoft.Json;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class UserAuthenticationsMapping : IContextHandler
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

        public ICollection<User> CompletedMappings = new List<User>();

        public int RecordCount = 0;
        public int FailedCount = 0;


        public void CreateUserAuthenticationMapping()
        {
            try
            {
                var dataSet = aHelper.GetAllUsers().Where(w => w.CPSiteId.Value == MigrationVariables.CurrentSiteId).ToList();
                RecordCount = dataSet.Count;

                foreach (var adUser in dataSet)
                {
                    aspnet_Membership member;
                    aspnet_Users aspUser;
                    UserAuthentication uAuth = null;
                    Guid appId = nHelper.GetApplicationId("Diabetes Partner");
                    bool isAdmin = (adUser.CliniProID.ToLower() == "admin") ? true : false;
                    bool isAdminSiteUser = false;

                    if (isAdmin)
                    {
                        string corp = aHelper.GetCorporationName(adUser.CPSiteId);

                        switch (corp)
                        {
                            case "Insulet":
                                appId = nHelper.GetApplicationId("OmniPod Partner");
                                break;
                            case "CliniProWeb":
                                appId = nHelper.GetApplicationId("CliniPro-Web");
                                break;
                            case "NuMedics":
                            default:
                                appId = nHelper.GetApplicationId("Administration");
                                isAdminSiteUser = true;
                                break;
                        } 
                    }

                    member = aHelper.GetMembershipInfo(adUser.UserId);
                    aspUser = aHelper.GetAspUserInfo(adUser.UserId);

                    uAuth = new UserAuthentication
                    {
                        ApplicationId = appId,
                        UserId = adUser.UserId,
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

                    var user = new User
                    {
                        UserId = adUser.UserId,
                        UserType = (isAdmin) ? (int)UserType.Clinician : (int)UserType.Patient,
                        CreationDate = member.CreateDate
                    };

                    if (isAdminSiteUser)
                    {
                        user.UserType = (int)UserType.Admin;
                    }

                    user.UserAuthentications.Add(uAuth);

                    // add user info to in-memery collection for use throughout application
                    MemoryMappings.AddPatientInfo(adUser.CPSiteId.Value, adUser.CliniProID, user.UserId);

                    if (CanAddToContext(user.UserId))
                    {
                        CompletedMappings.Add(user);
                    }
                    else
                    {
                        TransactionManager.FailedMappingCollection
                            .Add(new FailedMappings
                            {
                                Tablename = "Users",
                                ObjectType = typeof(User),
                                JsonSerializedObject = JsonConvert.SerializeObject(user),
                                FailedReason = "User already exist in database."
                            });
                        FailedCount++;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating User mapping.", e);
            }

        }
        public void AddToContext()
        {
            //TransactionManager.DatabaseContext.Users.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "User",
                    PreSaveCount = CompletedMappings.Count()
                };

                stats.StartTimer();
                TransactionManager.DatabaseContext.Users.AddRange(CompletedMappings);
                int saved = TransactionManager.DatabaseContext.SaveChanges();
                stats.StopTimer();
                stats.PostSaveCount = saved;

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating User entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving User entity", e);
            }
        }

        private bool CanAddToContext(Guid userId)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.Users.Any(a => a.UserId == userId)) ? false : true;
            }
        }
    }
}
