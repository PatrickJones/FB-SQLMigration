using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuLibrary.Migration.Test.SqlDatabaseTest
{
    /// <summary>
    /// Summary description for NumedicsGlobalHelpersTest
    /// </summary>
    [TestClass]
    public class NumedicsGlobalHelpersTest
    {
        public NumedicsGlobalHelpersTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllTherapyTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllReadingEventTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllPaymentMethodsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllCheckStatusTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllInsulinTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetApplicationIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetInstitutionIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetInsuranceCompanyIdTest()
        {
            Assert.Fail();
        }
    }
}
