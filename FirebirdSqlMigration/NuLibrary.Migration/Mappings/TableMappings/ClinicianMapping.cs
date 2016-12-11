using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class ClinicianMapping
    {
        AspnetDbHelpers aHelper = new AspnetDbHelpers();
        NumedicsGlobalHelpers nHelper = new NumedicsGlobalHelpers();

        public void CreateClinicianMapping()
        {
            foreach (var adUser in aHelper.GetAllAdmins())
            {
                aspnet_Membership member;
                aspnet_Users aspUser;
                UserAuthentication uAuth = null;
                Guid appId = nHelper.GetApplicationId("Diabetes Partner");

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
                        break;
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
                    UserType = (int)UserType.Clinician,
                    CreationDate = member.CreateDate
                };

                var clin = new Clinician
                {
                    UserId = adUser.UserId,
                    Firstname = String.Empty,
                    Lastname = String.Empty,
                    StateLicenseNumber = String.Empty,
                    InstitutionId = nHelper.GetInstitutionId(adUser.CPSiteId)
                };

                user.UserAuthentications.Add(uAuth);
                user.Clinician = clin;

                TransactionManager.DatabaseContext.Users.Add(user);
            }
        }
    }
}
