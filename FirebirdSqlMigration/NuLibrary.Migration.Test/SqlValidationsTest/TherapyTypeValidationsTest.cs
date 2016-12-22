using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SqlValidations;
using Moq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for TherapyTypeValidationsTest
    /// </summary>
    [TestClass]
    public class TherapyTypeValidationsTest
    {
        TherapyTypeValidation tv;
        static List<TherapyType> defTherapyTypes = new List<TherapyType>();
        static List<TherapyType> missing = new List<TherapyType>();

        static Mock<DbSet<TherapyType>> moqStatus = new Mock<DbSet<TherapyType>>();
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
        public static void TherapyTypeValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void TherapyTypeValidationTestInitialize()
        {
            tv = new TherapyTypeValidation();
            defTherapyTypes = tv.DefaultTherapyTypes;
        }

        [TestCleanup()]
        public void TherapyTypeValidationTestCleanup() { defTherapyTypes.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("TherapyTypes", tv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            tv = new TherapyTypeValidation(nuContext.Object);

            moqStatus.SetupData();
            var count = await moqStatus.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defTherapyTypes.ToArray(), a => {
                moqStatus.Object.Add(a);
            });

            Assert.AreEqual(moqStatus.Object.Count(), defTherapyTypes.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqStatus.SetupData();
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defTherapyTypes.Count}");
            Array.ForEach(defTherapyTypes.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.TypeName.ToLower() == d.TypeName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 2);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqStatus.SetupData(defTherapyTypes);
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defTherapyTypes.Count}");
            Array.ForEach(defTherapyTypes.ToArray(), d => {
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
