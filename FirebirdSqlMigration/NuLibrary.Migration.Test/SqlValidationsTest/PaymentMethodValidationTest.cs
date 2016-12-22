using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.SqlValidations;
using NuLibrary.Migration.SQLDatabase.EF;
using System.Data.Entity;
using Moq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for PaymentMethodValidationTest
    /// </summary>
    [TestClass]
    public class PaymentMethodValidationTest
    {
        PaymentMethodValidation pv;
        static List<PaymentMethod> defPaymentMethods = new List<PaymentMethod>();
        static List<PaymentMethod> missing = new List<PaymentMethod>();

        static Mock<DbSet<PaymentMethod>> moqStatus = new Mock<DbSet<PaymentMethod>>();
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
        public static void PaymentMethodValidationTestClassCleanup() { nuContext.Object.Dispose(); }

        [TestInitialize()]
        public void PaymentMethodValidationTestInitialize()
        {
            pv = new PaymentMethodValidation();
            defPaymentMethods = pv.DefaultPaymentMethods;
        }

        [TestCleanup()]
        public void CheckStatusValidationTestCleanup() { defPaymentMethods.Clear(); missing.Clear(); }

        [TestMethod]
        public void Verify_Table_Name()
        {
            Assert.AreEqual("PaymentMethods", pv.TableName);
        }

        [TestMethod]
        public async Task Sync_Records_If_Missing()
        {
            pv = new PaymentMethodValidation(nuContext.Object);

            moqStatus.SetupData();
            var count = await moqStatus.Object.CountAsync();

            Assert.IsTrue(count == 0);

            Array.ForEach(defPaymentMethods.ToArray(), a => {
                moqStatus.Object.Add(a);
            });

            Assert.AreEqual(moqStatus.Object.Count(), defPaymentMethods.Count);
        }

        [TestMethod]
        public void Validation_Check_For_Missing_Records()
        {
            moqStatus.SetupData();
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defPaymentMethods.Count}");
            Array.ForEach(defPaymentMethods.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.MethodName.ToLower() == d.MethodName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Assert.IsTrue(missing.Count == 4);
        }

        [TestMethod]
        public void Validation_Check_For_No_Missing_Records()
        {
            moqStatus.SetupData(defPaymentMethods);
            Trace.WriteLine($"Moq apps count : {moqStatus.Object.Count()}");
            Trace.WriteLine($"Default apps count : {defPaymentMethods.Count}");
            Array.ForEach(defPaymentMethods.ToArray(), d => {
                if (!moqStatus.Object.Any(a => a.MethodName.ToLower() == d.MethodName.ToLower()))
                {
                    missing.Add(d);
                }
            });

            Trace.WriteLine($"Missing: {missing.Count}");
            Assert.IsTrue(missing.Count == 0);
        }
    }
}
