using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.SQLDatabase.EF;
using System.Data.Entity;
using Moq;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for UserTypeValidationTest
    /// </summary>
    [TestClass]
    public class UserTypeValidationTest
    {
        UserTypeValidation uv;
        static List<UserType> defUserType = new List<UserType>();
        static List<UserType> missing = new List<UserType>();

        static Mock<DbSet<UserType>> moqStatus = new Mock<DbSet<UserType>>();
        static Mock<NuMedicsGlobalEntities> nuContext = new Mock<NuMedicsGlobalEntities>();

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

        [ClassCleanup()]
        public static void UserTypeValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void UserTypeValidationTestInitialize()
        {
            uv = new UserTypeValidation();
            defUserType = uv.DefaultUserTypes;
        }

        [TestCleanup()]
        public void UserTypeValidationTestCleanup() { defUserType.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("UserTypes", uv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            uv = new UserTypeValidation(nuContext.Object);

            moqStatus.SetupData();
            var count = await moqStatus.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defUserType.ToArray(), a => {
                moqStatus.Object.Add(a);
            });

            Assert.AreEqual(moqStatus.Object.Count(), defUserType.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqStatus.SetupData();
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defUserType.Count}");
            Array.ForEach(defUserType.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.TypeName.ToLower() == d.TypeName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 3);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqStatus.SetupData(defUserType);
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defUserType.Count}");
            Array.ForEach(defUserType.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.TypeName.ToLower() == d.TypeName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }
    }
}
