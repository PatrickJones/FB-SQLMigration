using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.FBDatabase;
using NuLibrary.Migration.GlobalVar;
using System.Linq;

namespace NuLibrary.Migration.Test.FBDatabaseTest
{
    /// <summary>
    /// Summary description for FBDataAccessTest
    /// </summary>
    [TestClass]
    public class FBDataAccessTest
    {
        FBDataAccess da;

        public FBDataAccessTest()
        {
            MigrationVariables.CurrentSiteId = 355;
            MigrationVariables.Init();

            da = new FBDataAccess();
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

        [TestMethod]
        public void Verify_DbProvider()
        {
            Assert.AreEqual("FirebirdSql.Data.FirebirdClient", da.DatabaseProvider);
        }

        [TestMethod]
        public void Verify_ConnectionString()
        {
            var conn = da.GetConnnection();

            Assert.IsTrue(conn.ConnectionString.Contains("sysdba"));
            Assert.IsTrue(conn.ConnectionString.Contains("masterkey"));
            Assert.IsTrue(conn.ConnectionString.Contains("UnitTest"));
        }

        [TestMethod]
        public void Table_Names()
        {
            var tNames = da.GetTableNames();

            Assert.IsTrue(tNames.Count > 0);
            Assert.IsTrue(tNames.Any(a => a == "PATIENTS"));
            Assert.IsTrue(tNames.Any(a => a == "METERREADINGHEADER"));
            Assert.IsTrue(tNames.Any(a => a == "METERREADING"));
            Assert.IsTrue(tNames.Any(a => a == "DMDATA"));
            Assert.IsTrue(tNames.Any(a => a == "INSURANCECOS"));
            Assert.IsTrue(tNames.Any(a => a == "INSURANCEPLANS2"));
            Assert.IsTrue(tNames.Any(a => a == "PHONENUMBERS"));
            Assert.IsTrue(tNames.Any(a => a == "PATIENTPUMPPROGRAM"));
            Assert.IsTrue(tNames.Any(a => a == "INSULETPUMPSETTINGS"));
            Assert.IsTrue(tNames.Any(a => a == "PATIENTPUMP"));
            Assert.IsTrue(tNames.Any(a => a == "PUMPTIMESLOTS"));
            Assert.IsTrue(tNames.Any(a => a == "TIMESLOT"));
        }
    }
}
