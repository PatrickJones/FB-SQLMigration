using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.Test.EF;
using NuLibrary.Migration.SQLDatabase.EF;
using Moq;
using System.Data.Entity;
using System.Linq;
using NuLibrary.Migration.Mappings.InMemoryMappings;

namespace NuLibrary.Migration.Test.SqlDatabaseTest
{
    /// <summary>
    /// Summary description for NumedicsGlobalHelpersTest
    /// </summary>
    [TestClass]
    public class NumedicsGlobalHelpersTest : DbContextDisposal
    {
        NuMedicsGlobalEntities db;

        static Mock<NuMedicsGlobalEntities> nuContext = new Mock<NuMedicsGlobalEntities>();

        public NumedicsGlobalHelpersTest()
        {
            db = new NuMedicsGlobalEntities();
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

        [TestMethod()]
        public void GetAllUserTypesTest()
        {
            Mock<DbSet<SQLDatabase.EF.UserType>> moq_UserTypes = new Mock<DbSet<UserType>>();
            moq_UserTypes.SetupData(new List<SQLDatabase.EF.UserType>());

            Array.ForEach(db.UserTypes.ToArray(), a => {
                var nUser = new SQLDatabase.EF.UserType();

                nUser.TypeId = a.TypeId;
                nUser.TypeName = a.TypeName;

                moq_UserTypes.Object.Add(nUser);
            });

            nuContext.Setup(c => c.UserTypes).Returns(moq_UserTypes.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(3, nh.GetAllUserTypes().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllUserTypes().Where(w => w.TypeName == "Clinician").Select(s => s.TypeId).Single());
            Assert.AreEqual(2, nh.GetAllUserTypes().Where(w => w.TypeName == "Patient").Select(s => s.TypeId).Single());
            Assert.AreEqual(3, nh.GetAllUserTypes().Where(w => w.TypeName == "Admin").Select(s => s.TypeId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllUserTypes().Any(a => a.TypeName == "Clinician"));
            Assert.IsTrue(nh.GetAllUserTypes().Any(a => a.TypeName == "Patient"));
            Assert.IsTrue(nh.GetAllUserTypes().Any(a => a.TypeName == "Admin"));

        }

        [TestMethod()]
        public void GetAllTherapyTypesTest()
        {
            Mock<DbSet<SQLDatabase.EF.TherapyType>> moq_TherapyTypes = new Mock<DbSet<TherapyType>>();
            moq_TherapyTypes.SetupData(new List<SQLDatabase.EF.TherapyType>());

            Array.ForEach(db.TherapyTypes.ToArray(), a => {
                var nTher = new SQLDatabase.EF.TherapyType();

                nTher.TypeId = a.TypeId;
                nTher.TypeName = a.TypeName;

                moq_TherapyTypes.Object.Add(nTher);
            });

            nuContext.Setup(c => c.TherapyTypes).Returns(moq_TherapyTypes.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(2, nh.GetAllTherapyTypes().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllTherapyTypes().Where(w => w.TypeName == "Scheduled").Select(s => s.TypeId).Single());
            Assert.AreEqual(2, nh.GetAllTherapyTypes().Where(w => w.TypeName == "OnDemand").Select(s => s.TypeId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllTherapyTypes().Any(a => a.TypeName == "Scheduled"));
            Assert.IsTrue(nh.GetAllTherapyTypes().Any(a => a.TypeName == "OnDemand"));

        }

        [TestMethod()]
        public void GetAllReadingEventTypesTest()
        {
            Mock<DbSet<SQLDatabase.EF.ReadingEventType>> moq_REType = new Mock<DbSet<ReadingEventType>>();
            moq_REType.SetupData(new List<SQLDatabase.EF.ReadingEventType>());

            Array.ForEach(db.ReadingEventTypes.ToArray(), a => {
                var nRET = new SQLDatabase.EF.ReadingEventType();

                nRET.EventId = a.EventId;
                nRET.EventName = a.EventName;

                moq_REType.Object.Add(nRET);
            });

            nuContext.Setup(c => c.ReadingEventTypes).Returns(moq_REType.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(8, nh.GetAllReadingEventTypes().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllReadingEventTypes().Where(w => w.EventName == "Alarm").Select(s => s.EventId).Single());
            Assert.AreEqual(2, nh.GetAllReadingEventTypes().Where(w => w.EventName == "DateChange").Select(s => s.EventId).Single());
            Assert.AreEqual(3, nh.GetAllReadingEventTypes().Where(w => w.EventName == "RemoteHazard").Select(s => s.EventId).Single());
            Assert.AreEqual(4, nh.GetAllReadingEventTypes().Where(w => w.EventName == "Activate").Select(s => s.EventId).Single());
            Assert.AreEqual(5, nh.GetAllReadingEventTypes().Where(w => w.EventName == "Deactivate").Select(s => s.EventId).Single());
            Assert.AreEqual(6, nh.GetAllReadingEventTypes().Where(w => w.EventName == "Suspend").Select(s => s.EventId).Single());
            Assert.AreEqual(7, nh.GetAllReadingEventTypes().Where(w => w.EventName == "Resume").Select(s => s.EventId).Single());
            Assert.AreEqual(8, nh.GetAllReadingEventTypes().Where(w => w.EventName == "TimeChange").Select(s => s.EventId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "Alarm"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "DateChange"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "RemoteHazard"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "Activate"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "Deactivate"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "Suspend"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "Resume"));
            Assert.IsTrue(nh.GetAllReadingEventTypes().Any(a => a.EventName == "TimeChange"));
        }

        [TestMethod()]
        public void GetAllPaymentMethodsTest()
        {
            Mock<DbSet<SQLDatabase.EF.PaymentMethod>> moq_PayMeth = new Mock<DbSet<PaymentMethod>>();
            moq_PayMeth.SetupData(new List<SQLDatabase.EF.PaymentMethod>());

            Array.ForEach(db.PaymentMethods.ToArray(), a => {
                var nPay = new SQLDatabase.EF.PaymentMethod();

                nPay.MethodId = a.MethodId;
                nPay.MethodName = a.MethodName;

                moq_PayMeth.Object.Add(nPay);
            });

            nuContext.Setup(c => c.PaymentMethods).Returns(moq_PayMeth.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(4, nh.GetAllPaymentMethods().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllPaymentMethods().Where(w => w.MethodName == "PayPal").Select(s => s.MethodId).Single());
            Assert.AreEqual(2, nh.GetAllPaymentMethods().Where(w => w.MethodName == "Check").Select(s => s.MethodId).Single());
            Assert.AreEqual(3, nh.GetAllPaymentMethods().Where(w => w.MethodName == "Invoice").Select(s => s.MethodId).Single());
            Assert.AreEqual(4, nh.GetAllPaymentMethods().Where(w => w.MethodName == "Adjustment").Select(s => s.MethodId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllPaymentMethods().Any(a => a.MethodName == "PayPal"));
            Assert.IsTrue(nh.GetAllPaymentMethods().Any(a => a.MethodName == "Check"));
            Assert.IsTrue(nh.GetAllPaymentMethods().Any(a => a.MethodName == "Invoice"));
            Assert.IsTrue(nh.GetAllPaymentMethods().Any(a => a.MethodName == "Adjustment"));
        }

        [TestMethod()]
        public void GetAllCheckStatusTypesTest()
        {
            Mock<DbSet<SQLDatabase.EF.CheckStatu>> moq_Chk = new Mock<DbSet<CheckStatu>>();
            moq_Chk.SetupData(new List<SQLDatabase.EF.CheckStatu>());

            Array.ForEach(db.CheckStatus.ToArray(), a => {
                var nChk = new SQLDatabase.EF.CheckStatu();

                nChk.StatusId = a.StatusId;
                nChk.Status = a.Status;

                moq_Chk.Object.Add(nChk);
            });

            nuContext.Setup(c => c.CheckStatus).Returns(moq_Chk.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(4, nh.GetAllCheckStatusTypes().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllCheckStatusTypes().Where(w => w.Status == "Completed").Select(s => s.StatusId).Single());
            Assert.AreEqual(2, nh.GetAllCheckStatusTypes().Where(w => w.Status == "Canceled").Select(s => s.StatusId).Single());
            Assert.AreEqual(3, nh.GetAllCheckStatusTypes().Where(w => w.Status == "Pending").Select(s => s.StatusId).Single());
            Assert.AreEqual(4, nh.GetAllCheckStatusTypes().Where(w => w.Status == "RejectedByBank").Select(s => s.StatusId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllCheckStatusTypes().Any(a => a.Status == "Completed"));
            Assert.IsTrue(nh.GetAllCheckStatusTypes().Any(a => a.Status == "Canceled"));
            Assert.IsTrue(nh.GetAllCheckStatusTypes().Any(a => a.Status == "Pending"));
            Assert.IsTrue(nh.GetAllCheckStatusTypes().Any(a => a.Status == "RejectedByBank"));
        }

        [TestMethod()]
        public void GetAllInsulinTypesTest()
        {
            Mock<DbSet<SQLDatabase.EF.InsulinType>> moq_Insulin = new Mock<DbSet<SQLDatabase.EF.InsulinType>>();
            moq_Insulin.SetupData(new List<SQLDatabase.EF.InsulinType>());

            Array.ForEach(db.InsulinTypes.ToArray(), a => {
                var nIns = new SQLDatabase.EF.InsulinType();

                nIns.InsulinTypeId = a.InsulinTypeId;
                nIns.Type = a.Type;

                moq_Insulin.Object.Add(nIns);
            });

            nuContext.Setup(c => c.InsulinTypes).Returns(moq_Insulin.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify only three types exist
            Assert.AreEqual(5, nh.GetAllInsulinTypes().Count);

            // verify type ids
            Assert.AreEqual(1, nh.GetAllInsulinTypes().Where(w => w.Type == "Rapid Acting").Select(s => s.InsulinTypeId).Single());
            Assert.AreEqual(2, nh.GetAllInsulinTypes().Where(w => w.Type == "Short Acting").Select(s => s.InsulinTypeId).Single());
            Assert.AreEqual(3, nh.GetAllInsulinTypes().Where(w => w.Type == "Intermediate Acting").Select(s => s.InsulinTypeId).Single());
            Assert.AreEqual(4, nh.GetAllInsulinTypes().Where(w => w.Type == "Long Acting").Select(s => s.InsulinTypeId).Single());
            Assert.AreEqual(5, nh.GetAllInsulinTypes().Where(w => w.Type == "PreMixed").Select(s => s.InsulinTypeId).Single());

            // verify types names
            Assert.IsTrue(nh.GetAllInsulinTypes().Any(a => a.Type == "Rapid Acting"));
            Assert.IsTrue(nh.GetAllInsulinTypes().Any(a => a.Type == "Short Acting"));
            Assert.IsTrue(nh.GetAllInsulinTypes().Any(a => a.Type == "Intermediate Acting"));
            Assert.IsTrue(nh.GetAllInsulinTypes().Any(a => a.Type == "Long Acting"));
            Assert.IsTrue(nh.GetAllInsulinTypes().Any(a => a.Type == "PreMixed"));
        }

        [TestMethod()]
        public void GetApplicationIdTest()
        {
            Mock<DbSet<SQLDatabase.EF.Application>> moq_App = new Mock<DbSet<Application>>();
            moq_App.SetupData(new List<SQLDatabase.EF.Application>());

            Array.ForEach(db.Applications.ToArray(), a => {
                var nApp = new SQLDatabase.EF.Application();

                nApp.ApplicationId = a.ApplicationId;
                nApp.ApplicationName = a.ApplicationName;
                nApp.BannerEnable = a.BannerEnable;
                nApp.BannerMessage = a.BannerMessage;
                nApp.Description = a.Description;

                moq_App.Object.Add(nApp);
            });

            nuContext.Setup(c => c.Applications).Returns(moq_App.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            Guid expId = Guid.Parse("DFE4EB52-401E-42DA-B7A0-D801749446A0");
            Guid actual = nh.GetApplicationId("Administration");

            // verify Id is returned
            Assert.AreEqual(expId, actual);

            // verify empty GUID is returned for applications not found
            Assert.IsTrue(nh.GetApplicationId("FakeApplication").Equals(Guid.Empty));
        }

        [TestMethod()]
        public void GetInstitutionIdTest()
        {
            Mock<DbSet<SQLDatabase.EF.Institution>> moq_Inst = new Mock<DbSet<Institution>>();
            moq_Inst.SetupData(new List<SQLDatabase.EF.Institution>());

            var nInst = new Institution {
                City = "Portland",
                ContactEmail = "someemail@email.com",
                ContactFirstname = "John",
                ContactLastname = "Doe",
                Country = "US",
                LegacySiteId = 12345,
                Name = "Some Institution",
                State = "Oregon",
                Street = "123 Street",
                Zip = "12345",
                InstitutionId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            moq_Inst.Object.Add(nInst);

            nuContext.Setup(c => c.Institutions).Returns(moq_Inst.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            //Verify institution Id is returned
            Assert.AreEqual(nInst.InstitutionId, nh.GetInstitutionId(nInst.LegacySiteId));
            // verify empty GUID is returned for site id's not found
            Assert.IsTrue(nh.GetInstitutionId(99999).Equals(Guid.Empty));
        }

        [TestMethod()]
        public void GetInsuranceCompanyIdTest()
        {
            MemoryInsuranceCompanys.Companies.Add("12345", "Acme Insurance");

            Mock<DbSet<SQLDatabase.EF.InsuranceProvider>> moq_Ins = new Mock<DbSet<InsuranceProvider>>();
            moq_Ins.SetupData(new List<SQLDatabase.EF.InsuranceProvider>());

            var nIns = new InsuranceProvider {
                Name = "Acme Insurance",
                IsActive = true,
                CompanyId = 12345
            };

            moq_Ins.Object.Add(nIns);

            nuContext.Setup(c => c.InsuranceProviders).Returns(moq_Ins.Object);
            NumedicsGlobalHelpers nh = new NumedicsGlobalHelpers(nuContext.Object);

            // verify insurance id is returned
            Assert.AreEqual(nIns.CompanyId, nh.GetInsuranceCompanyId(nIns.CompanyId.ToString()));
            // verify 0 is returned for insurance providers not found
            Assert.IsTrue(nh.GetInsuranceCompanyId("99999").Equals(0));
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
