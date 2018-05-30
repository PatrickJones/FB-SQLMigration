using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.Test.EF;
using System.Linq;
using NuLibrary.Migration.SQLDatabase.EF;
using Moq;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace NuLibrary.Migration.Test.SqlDatabaseTest
{
    /// <summary>
    /// Summary description for AspnetDbHelpersTest
    /// </summary>
    [TestClass]
    public class AspnetDbHelpersTest : DbContextDisposal
    {
        aspnerdbUnitTestEntities db;
        
        static Mock<AspnetDbEntities> nuContext = new Mock<AspnetDbEntities>();

        ICollection<SQLDatabase.EF.CorporationsView> cView = new List<SQLDatabase.EF.CorporationsView>();
        ICollection<SQLDatabase.EF.FirebirdConnection> fbConn = new List<SQLDatabase.EF.FirebirdConnection>();
        ICollection<SQLDatabase.EF.clinipro_Users> cpUsers = new List<SQLDatabase.EF.clinipro_Users>();

        public AspnetDbHelpersTest()
        {
            db = new aspnerdbUnitTestEntities();
            InitCollections();
        }


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private void InitCollections()
        {
            Array.ForEach(db.CorporationsViews.ToArray(), c => {
                var nView = new SQLDatabase.EF.CorporationsView();

                nView.Corp_Name = c.Corp_Name;
                nView.DatabaseLocation = c.DatabaseLocation;
                nView.DatasourceServer = c.DatasourceServer;
                nView.Location = c.Location;
                nView.Location_Name = c.Location_Name;
                nView.Port = c.Port;
                nView.SiteId = c.SiteId;
                nView.SiteName = c.SiteName;
                nView.Site_Name = c.Site_Name;

                cView.Add(nView);
            });

            Array.ForEach(db.FirebirdConnections.ToArray(), c => {
                var nFC = new SQLDatabase.EF.FirebirdConnection();

                nFC.ConnectionId = c.ConnectionId;
                nFC.DatabaseLocation = c.DatabaseLocation;
                nFC.DatasourceServer = c.DatasourceServer;
                nFC.Location = c.Location;
                nFC.Password = c.Password;
                nFC.Port = c.Port;
                nFC.SiteId = c.SiteId;
                nFC.SiteName = c.SiteName;
                nFC.Name = c.Name;
                nFC.User = c.User;
                nFC.Dialect = c.Dialect;
                nFC.Charset = c.Charset;
                nFC.Pooling = c.Pooling;
                nFC.MinPoolSize = c.MinPoolSize;
                nFC.MaxPoolSize = c.MaxPoolSize;
                nFC.PacketSize = c.PacketSize;
                nFC.ServerType = c.ServerType;
                nFC.Role = c.Role;
                nFC.ConnectionLifeTime = c.ConnectionLifeTime;

                fbConn.Add(nFC);
            });

            Array.ForEach(db.clinipro_Users.ToArray(), a => {
                var nCP = new SQLDatabase.EF.clinipro_Users();
                nCP.CliniProID = a.CliniProID;
                nCP.CPSiteId = a.CPSiteId;
                nCP.CreatedBy = a.CreatedBy;
                nCP.IsApproved = a.IsApproved;
                nCP.IsFreeUser = a.IsFreeUser;
                nCP.IsNotApprovedReason = a.IsNotApprovedReason;
                nCP.LoginCount = a.LoginCount;
                nCP.MustChangePW = a.MustChangePW;
                nCP.MVCEligible = a.MVCEligible;
                nCP.UserId = a.UserId;

                cpUsers.Add(nCP);
            });

        }

        [TestMethod]
        public void Get_UserId_From_Patient_Id_Test()
        {
            string good_patientId = TestUsers.TestPatient.Item1;
            string bad_patientId = TestUsers.FakePatient.Item1;

            Mock<DbSet<SQLDatabase.EF.clinipro_Users>> moq_clinipro_users = new Mock<DbSet<SQLDatabase.EF.clinipro_Users>>();
            moq_clinipro_users.SetupData().Object.Add(cpUsers.Where(w => w.CliniProID == good_patientId).FirstOrDefault());

            nuContext.Setup(c => c.clinipro_Users).Returns(moq_clinipro_users.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            Guid good_user = ah.GetUserIdFromPatientId(good_patientId);
            Guid bad_user = ah.GetUserIdFromPatientId(bad_patientId);

            // good patent user id (GUID) is not empty
            Assert.AreNotEqual(good_user, Guid.Empty);
            // bad patient user id (GUID) is empty
            Assert.AreEqual(bad_user, Guid.Empty);
        }

        [TestMethod]
        public void GetAllCorporationInfoTest()
        {
            Mock<DbSet<SQLDatabase.EF.CorporationsView>> moq_CorpViews = new Mock<DbSet<SQLDatabase.EF.CorporationsView>>();
            moq_CorpViews.SetupData(cView);

            nuContext.Setup(c => c.CorporationsViews).Returns(moq_CorpViews.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            Assert.AreEqual(db.CorporationsViews.Count(), ah.GetAllCorporationInfo().Count);
        }

        [TestMethod]
        public void CreateCliniProUserTest()
        {
            Mock<DbSet<SQLDatabase.EF.clinipro_Users>> moq_cpUsers = new Mock<DbSet<SQLDatabase.EF.clinipro_Users>>();
            moq_cpUsers.SetupData(new List<SQLDatabase.EF.clinipro_Users>());

            nuContext.Setup(c => c.clinipro_Users).Returns(moq_cpUsers.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            // verify user is not already in database
            Assert.IsFalse(moq_cpUsers.Object.Any(a => a.CliniProID == TestUsers.FakePatient.Item1));

            // add user to database
            ah.CreateCliniProUser(TestUsers.FakePatient.Item2, TestUsers.FakePatient.Item1);

            // verify user created in database
            Assert.IsTrue(moq_cpUsers.Object.Any(a => a.CliniProID == TestUsers.FakePatient.Item1 && a.UserId == TestUsers.FakePatient.Item2));
        }

        [TestMethod]
        public void GetCorporationNameTest()
        {
            Mock<DbSet<SQLDatabase.EF.CorporationsView>> moq_CorpViews = new Mock<DbSet<SQLDatabase.EF.CorporationsView>>();

            var view = cView.Where(w => w.SiteId == TestUsers.TestInsuletSite).FirstOrDefault();
            view.Corp_Name = String.Empty;

            moq_CorpViews.SetupData(cView);

            nuContext.Setup(c => c.CorporationsViews).Returns(moq_CorpViews.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            // verify corporate name is returned
            Assert.AreEqual("CliniProWeb", ah.GetCorporationName(TestUsers.TestCliniProSite));

            // verify corporation name not exist returns empty string
            Assert.IsTrue(String.IsNullOrEmpty(ah.GetCorporationName(TestUsers.TestInsuletSite)));

            // verify if site id not exist returns empty string
            Assert.IsTrue(String.IsNullOrEmpty(ah.GetCorporationName(TestUsers.FakeSite)));
        }

        [TestMethod]
        public void GetMembershipInfoTest()
        {
            Mock<DbSet<SQLDatabase.EF.aspnet_Membership>> moq_Membership = new Mock<DbSet<SQLDatabase.EF.aspnet_Membership>>();
            moq_Membership.SetupData(new List<SQLDatabase.EF.aspnet_Membership>());

            Array.ForEach(db.aspnet_Membership
                .ToArray(), a =>
                {
                    if (a.UserId == TestUsers.UserA || a.UserId == TestUsers.UserB || a.UserId == TestUsers.UserC)
                    {
                        var nMember = new SQLDatabase.EF.aspnet_Membership();

                        nMember.ApplicationId = a.ApplicationId;
                        nMember.Comment = a.Comment;
                        nMember.CreateDate = a.CreateDate;
                        nMember.Email = a.Email;
                        nMember.FailedPasswordAnswerAttemptCount = a.FailedPasswordAnswerAttemptCount;
                        nMember.FailedPasswordAnswerAttemptWindowStart = a.FailedPasswordAnswerAttemptWindowStart;
                        nMember.FailedPasswordAttemptCount = a.FailedPasswordAttemptCount;
                        nMember.FailedPasswordAttemptWindowStart = a.FailedPasswordAttemptWindowStart;
                        nMember.IsApproved = a.IsApproved;
                        nMember.IsLockedOut = a.IsLockedOut;
                        nMember.IsTemp = a.IsTemp;
                        nMember.LastLockoutDate = a.LastLockoutDate;
                        nMember.LastLoginDate = a.LastLoginDate;
                        nMember.LastPasswordChangedDate = a.LastPasswordChangedDate;
                        nMember.LoweredEmail = a.LoweredEmail;
                        nMember.MobilePIN = a.MobilePIN;
                        nMember.Password = a.Password;
                        nMember.PasswordAnswer = a.PasswordAnswer;
                        nMember.PasswordFormat = a.PasswordFormat;
                        nMember.PasswordQuestion = a.PasswordQuestion;
                        nMember.PasswordSalt = a.PasswordSalt;
                        nMember.UserId = a.UserId;

                        moq_Membership.Object.Add(nMember);
                    }
                });

            nuContext.Setup(c => c.aspnet_Membership).Returns(moq_Membership.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            // verify user is unique in membership table
            var cnt = nuContext.Object.aspnet_Membership.Where(a => a.UserId == TestUsers.UserA).ToList();
            Assert.AreEqual(1, cnt.Count);

            var expUser = db.aspnet_Membership.Where(t => t.UserId == TestUsers.UserC).FirstOrDefault();
            var retUser = ah.GetMembershipInfo(TestUsers.UserC);

            //verify is null for non-matching user
            Assert.IsNull(ah.GetMembershipInfo(TestUsers.UserFake));

            // verify properties match
            Assert.AreEqual(expUser.ApplicationId, retUser.ApplicationId);
            Assert.AreEqual(expUser.Comment, retUser.Comment);
            Assert.AreEqual(expUser.CreateDate, retUser.CreateDate);
            Assert.AreEqual(expUser.Email, retUser.Email);
            Assert.AreEqual(expUser.FailedPasswordAnswerAttemptCount, retUser.FailedPasswordAnswerAttemptCount);
            Assert.AreEqual(expUser.FailedPasswordAnswerAttemptWindowStart, retUser.FailedPasswordAnswerAttemptWindowStart);
            Assert.AreEqual(expUser.FailedPasswordAttemptCount, retUser.FailedPasswordAttemptCount);
            Assert.AreEqual(expUser.FailedPasswordAttemptWindowStart, retUser.FailedPasswordAttemptWindowStart);
            Assert.AreEqual(expUser.IsApproved, retUser.IsApproved);
            Assert.AreEqual(expUser.IsLockedOut, retUser.IsLockedOut);
            Assert.AreEqual(expUser.IsTemp, retUser.IsTemp);
            Assert.AreEqual(expUser.LastLockoutDate, retUser.LastLockoutDate);
            Assert.AreEqual(expUser.LastLoginDate, retUser.LastLoginDate);
            Assert.AreEqual(expUser.LastPasswordChangedDate, retUser.LastPasswordChangedDate);
            Assert.AreEqual(expUser.LoweredEmail, retUser.LoweredEmail);
            Assert.AreEqual(expUser.MobilePIN, retUser.MobilePIN);
            Assert.AreEqual(expUser.Password, retUser.Password);
            Assert.AreEqual(expUser.PasswordAnswer, retUser.PasswordAnswer);
            Assert.AreEqual(expUser.PasswordFormat, retUser.PasswordFormat);
            Assert.AreEqual(expUser.PasswordQuestion, retUser.PasswordQuestion);
            Assert.AreEqual(expUser.PasswordSalt, retUser.PasswordSalt);
            Assert.AreEqual(expUser.UserId, retUser.UserId);
        }

        [TestMethod]
        public void GetAspUserInfoTest()
        {
            Mock<DbSet<SQLDatabase.EF.aspnet_Users>> moq_aspnetUsers = new Mock<DbSet<SQLDatabase.EF.aspnet_Users>>();
            moq_aspnetUsers.SetupData(new List<SQLDatabase.EF.aspnet_Users>());

            Array.ForEach(db.aspnet_Users
                .ToArray(), a =>
                {
                    if (a.UserId == TestUsers.UserA || a.UserId == TestUsers.UserB || a.UserId == TestUsers.UserC)
                    {
                        var nAsp = new SQLDatabase.EF.aspnet_Users();

                        nAsp.ApplicationId = a.ApplicationId;
                        nAsp.UserName = a.UserName;
                        nAsp.LoweredUserName = a.LoweredUserName;
                        nAsp.MobileAlias = a.MobileAlias;
                        nAsp.IsAnonymous = a.IsAnonymous;
                        nAsp.LastActivityDate = a.LastActivityDate;
                        nAsp.EULA_Ver = a.EULA_Ver;
                        nAsp.EULA_Date = a.EULA_Date;
                        nAsp.UserId = a.UserId;

                        moq_aspnetUsers.Object.Add(nAsp);
                    }
                });

            nuContext.Setup(c => c.aspnet_Users).Returns(moq_aspnetUsers.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            // verify user is unique in aspnet_Users table
            var cnt = nuContext.Object.aspnet_Users.Where(a => a.UserId == TestUsers.UserA).ToList();
            Assert.AreEqual(1, cnt.Count);

            var expUser = db.aspnet_Users.Where(t => t.UserId == TestUsers.UserC).FirstOrDefault();
            var retUser = ah.GetAspUserInfo(TestUsers.UserC);

            //verify is null for non-matching user
            Assert.IsNull(ah.GetAspUserInfo(TestUsers.UserFake));

            // verify properties match
            Assert.AreEqual(expUser.ApplicationId, retUser.ApplicationId);
            Assert.AreEqual(expUser.UserName, retUser.UserName);
            Assert.AreEqual(expUser.LoweredUserName, retUser.LoweredUserName);
            Assert.AreEqual(expUser.MobileAlias, retUser.MobileAlias);
            Assert.AreEqual(expUser.IsAnonymous, retUser.IsAnonymous);
            Assert.AreEqual(expUser.LastActivityDate, retUser.LastActivityDate);
            Assert.AreEqual(expUser.EULA_Ver, retUser.EULA_Ver);
            Assert.AreEqual(expUser.EULA_Date, retUser.EULA_Date);
            Assert.AreEqual(expUser.UserId, retUser.UserId);
        }

        [TestMethod]
        public void GetAllFirebirdConnectionsTest()
        {
            Mock<DbSet<SQLDatabase.EF.FirebirdConnection>> moq_FireConn = new Mock<DbSet<SQLDatabase.EF.FirebirdConnection>>();
            moq_FireConn.SetupData(fbConn);

            nuContext.Setup(c => c.FirebirdConnections).Returns(moq_FireConn.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            Assert.AreEqual(db.FirebirdConnections.Count(), ah.GetAllFirebirdConnections().Count);
        }

        [TestMethod]
        public void GetAllAdminsTest()
        {
            Mock<DbSet<SQLDatabase.EF.clinipro_Users>> moq_cpusers = new Mock<DbSet<SQLDatabase.EF.clinipro_Users>>();
            moq_cpusers.SetupData(cpUsers);

            nuContext.Setup(c => c.clinipro_Users).Returns(moq_cpusers.Object);
            AspnetDbHelpers ah = new AspnetDbHelpers(nuContext.Object);

            int exp = db.clinipro_Users.Where(w => w.CliniProID.ToLower() == "admin").Count();
            int act = ah.GetAllAdminsUsers().Count;

            Assert.AreEqual(exp, act);
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
