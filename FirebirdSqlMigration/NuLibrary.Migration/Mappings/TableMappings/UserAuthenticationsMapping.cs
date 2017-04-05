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
                    bool isAdmin = (string.Equals(adUser.CliniProID, "admin", StringComparison.CurrentCultureIgnoreCase)) ? true : false;
                    bool isAdminSiteUser = false;

                    if (isAdmin)
                    {
                        switch (aHelper.GetCorporationName(adUser.CPSiteId))
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
                    var userId = nHelper.ValidGuid(adUser.UserId);

                    uAuth = new UserAuthentication
                    {
                        ApplicationId = appId,
                        UserId = userId,
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
                        IsloggedIn = false,
                        LastUpdatedByUser = userId
                    };

                    var user = new User
                    {
                        UserId = userId,
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
                        MappingStatistics.LogFailedMapping("Users", typeof(User), JsonConvert.SerializeObject(user), "User already exist in database.");
                        FailedCount++;
                    }
                }

                MappingStatistics.LogMappingStat("None", 0, "Users", CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating User mapping.", e);
            }
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

                TransactionManager.DatabaseContext.Users.AddRange(CompletedMappings);
                TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = TransactionManager.DatabaseContext.Users.Count();

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
                return !ctx.Users.Any(a => a.UserId == userId);
            }
        }
    }
}
