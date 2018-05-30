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
    /// Summary description for ApplicationValidationTest
    /// </summary>
    [TestClass]
    public class ApplicationValidationTest
    {
        ApplicationValidation av;
        static List<Application> defaultApps = new List<Application>();
        static List<Application> missing = new List<Application>();

        static Mock<DbSet<Application>> moqApps = new Mock<DbSet<Application>>();
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
        public static void ApplicationValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void ApplicationValidationTestInitialize()
        {
            av = new ApplicationValidation();
            defaultApps = av.DefaultApps;
        }

        [TestCleanup()]
        public void ApplicationValidationTestCleanup() { defaultApps.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("Applications", av.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            av = new ApplicationValidation(nuContext.Object);

            moqApps.SetupData();
            var count = await moqApps.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defaultApps.ToArray(), a => {
                moqApps.Object.Add(a);
            });

            Assert.AreEqual(moqApps.Object.Count(), defaultApps.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqApps.SetupData();
            Trace.WriteLine($"Moq apps count : {moqApps.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defaultApps.Count}");
            Array.ForEach(defaultApps.ToArray(), d => {
                if (!moqApps.Object.Any(a => a.ApplicationId == d.ApplicationId && a.ApplicationName.ToLower() == d.ApplicationName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 4);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqApps.SetupData(defaultApps);
            Trace.WriteLine($"Moq apps count : {moqApps.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defaultApps.Count}");
            Array.ForEach(defaultApps.ToArray(), d => {
                if (!moqApps.Object.Any(a => a.ApplicationId == d.ApplicationId && a.ApplicationName.ToLower() == d.ApplicationName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }
    }
}
