using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.SQLDatabase.EF;
using Moq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for ReadingEventTypeValidations
    /// </summary>
    [TestClass]
    public class ReadingEventTypeValidations
    {
        ReadingEventTypeValidation rv;
        static List<ReadingEventType> defReadingEvents = new List<ReadingEventType>();
        static List<ReadingEventType> missing = new List<ReadingEventType>();

        static Mock<DbSet<ReadingEventType>> moqStatus = new Mock<DbSet<ReadingEventType>>();
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
        public static void ReadingEventTypeValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void ReadingEventTypeValidationTestInitialize()
        {
            rv = new ReadingEventTypeValidation();
            defReadingEvents = rv.DefaultReadingEventTypes;
        }

        [TestCleanup()]
        public void CheckStatusValidationTestCleanup() { defReadingEvents.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("ReadingEventTypes", rv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            rv = new ReadingEventTypeValidation(nuContext.Object);

            moqStatus.SetupData();
            var count = await moqStatus.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defReadingEvents.ToArray(), a => {
                moqStatus.Object.Add(a);
            });

            Assert.AreEqual(moqStatus.Object.Count(), defReadingEvents.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqStatus.SetupData();
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defReadingEvents.Count}");
            Array.ForEach(defReadingEvents.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.EventName.ToLower() == d.EventName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 8);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqStatus.SetupData(defReadingEvents);
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defReadingEvents.Count}");
            Array.ForEach(defReadingEvents.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.EventName.ToLower() == d.EventName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }

    }
}
