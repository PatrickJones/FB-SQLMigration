using NuLibrary.Migration.FBDatabase.FBTables;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.GlobalVar;

namespace NuLibrary.Migration.Test.FBDatabaseTest
{
    /// <summary>
    /// Summary description for TableAgentCollectionTest
    /// </summary>
    [TestClass]
    public class TableAgentCollectionTest
    {
        public TableAgentCollectionTest()
        {
            MigrationVariables.CurrentSiteId = 355;
            MigrationVariables.Init();
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

        [TestMethod]
        public void Populate_Collection()
        {
            // collection is empty prior to calling populate method
            Assert.IsTrue(TableAgentCollection.TableAgents.Count == 0);

            TableAgentCollection.Populate();

            // collecion is not empty after calling populate method
            Assert.IsTrue(TableAgentCollection.TableAgents.Count > 0);
        }

        [TestMethod]
        public void Populate_Collection_With_Table_Name()
        {
            // collection is empty prior to calling populate method
            Assert.IsTrue(TableAgentCollection.TableAgents.Count == 0);

            TableAgentCollection.Populate(new List<string> { "PATIENTS", "DMDATA" });

            // collecion is not empty after calling populate method
            Assert.IsTrue(TableAgentCollection.TableAgents.Count > 0);
        }

    }
}
