using Newtonsoft.Json;
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
                var dataSet = aHelper.GetAllUsers();
                RecordCount = dataSet.Count;

                foreach (var adUser in aHelper.GetAllUsers())
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
                    MemoryPatientInfo.AddPatientInfo(adUser.CPSiteId.Value, adUser.CliniProID, user.UserId);

                    if (CanAddToContext(user.UserId))
                    {
                        //TransactionManager.DatabaseContext.Users.Add(user); 
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

                //TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating UserAuthentication mapping.", e);
            }

        }
        public void AddToContext()
        {
            TransactionManager.DatabaseContext.Users.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating UserAuthentication entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving UserAuthentication entity", e);
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
