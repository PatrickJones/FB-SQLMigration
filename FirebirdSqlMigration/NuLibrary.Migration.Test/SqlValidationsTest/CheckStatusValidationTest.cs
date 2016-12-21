using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.SQLDatabase.EF;
using Moq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for CheckStatusValidationTest
    /// </summary>
    [TestClass]
    public class CheckStatusValidationTest
    {
        CheckStatusValidation cv;
        static List<CheckStatu> defCheckStatus = new List<CheckStatu>();
        static List<CheckStatu> missing = new List<CheckStatu>();

        static Mock<DbSet<CheckStatu>> moqApps = new Mock<DbSet<CheckStatu>>();
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
        public static void CheckStatusValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void CheckStatusValidationTestInitialize()
        {
            cv = new CheckStatusValidation();
            defCheckStatus = cv.DefaultCheckStatus;
        }

        [TestCleanup()]
        public void CheckStatusValidationTestCleanup() { defCheckStatus.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("CheckStatus", cv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            cv = new CheckStatusValidation(nuContext.Object);

            moqApps.SetupData();
            var count = await moqApps.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defCheckStatus.ToArray(), a => {
                moqApps.Object.Add(a);
            });

            Assert.AreEqual(moqApps.Object.Count(), defCheckStatus.Count);
        }

        public void Validation_Check_For_Missing_Records()
        {
            moqApps.SetupData();
            Trace.WriteLine($"Moq apps count : {moqApps.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defCheckStatus.Count}");
            Array.ForEach(defCheckStatus.ToArray(), d => {
                if (!moqApps.Object.Any(a => a.Status.ToLower() == d.Status.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 4);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqApps.SetupData(defCheckStatus);
            Trace.WriteLine($"Moq apps count : {moqApps.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defCheckStatus.Count}");
            Array.ForEach(defCheckStatus.ToArray(), d => {
                if (!moqApps.Object.Any(a => a.Status.ToLower() == d.Status.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }

        private void SetDefaults()
        {
            string[] typeArr = {
                "Completed",
                "Cancelled",
                "Pending",
                "RejectedByBank"
            };

            Array.ForEach(typeArr, a => {
                defCheckStatus.Add(
                new CheckStatu
                {
                    Status = a
                });
            });
        }
    }
}
