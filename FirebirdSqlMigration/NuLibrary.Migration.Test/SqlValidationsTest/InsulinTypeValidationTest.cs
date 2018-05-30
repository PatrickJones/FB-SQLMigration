using NuLibrary.Migration.SqlValidations;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SQLDatabase.EF;
using Moq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for InsulinTypeValidationTest
    /// </summary>
    [TestClass]
    public class InsulinTypeValidationTest
    {
        InsulinTypeValidation iv;
        static List<InsulinType> defInsulinTypes = new List<InsulinType>();
        static List<InsulinType> missing = new List<InsulinType>();

        static Mock<DbSet<InsulinType>> moqStatus = new Mock<DbSet<InsulinType>>();
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
        public static void InsulinTypeValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void InsulinTypeValidationTestInitialize()
        {
            iv = new InsulinTypeValidation();
            defInsulinTypes = iv.DefaultInsulinTypes;
        }

        [TestCleanup()]
        public void InsulinTypeValidationTestCleanup() { defInsulinTypes.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("InsulinTypes", iv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            iv = new InsulinTypeValidation(nuContext.Object);

            moqStatus.SetupData();
            var count = await moqStatus.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defInsulinTypes.ToArray(), a => {
                moqStatus.Object.Add(a);
            });

            Assert.AreEqual(moqStatus.Object.Count(), defInsulinTypes.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqStatus.SetupData();
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defInsulinTypes.Count}");
            Array.ForEach(defInsulinTypes.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.Type.ToLower() == d.Type.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 5);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqStatus.SetupData(defInsulinTypes);
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defInsulinTypes.Count}");
            Array.ForEach(defInsulinTypes.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.Type.ToLower() == d.Type.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }

    }
}
