using NuLibrary.Migration.FBDatabase.FBTables;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.GlobalVar;
using System.Xml.Linq;


namespace NuLibrary.Migration.Test.FBDatabaseTest
{
    /// <summary>
    /// Summary description for TableAgentTest
    /// </summary>
    [TestClass]
    public class TableAgentTest
    {
        TableAgent ta;
        public TableAgentTest()
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

        [TestInitialize]
        public void TableAgnetTestInitialize()
        {
            // use PATIENTS table for test
            ta = new TableAgent("PATIENTS");
        }

        [TestMethod()]
        public void GetDataTableTest()
        {
            var table = ta.GetDataTable();

            // verify returned value is not null
            Assert.IsNotNull(table);
            // DataTable name matches table agent name 
            Assert.AreEqual(ta.TableName, table.TableName);
        }

        [TestMethod()]
        public void GetTableSchemaTest()
        {
            var schema = ta.GetTableSchema();

            // verify returned xDcoument is not null
            Assert.IsNotNull(schema);
            // verify schema contains text
            Assert.IsFalse(String.IsNullOrEmpty(schema.Document.ToString()));
        }

        [TestMethod()]
        public void InitTest()
        {
            var dSet = ta.DataSet;

            // verify table name 
            Assert.AreEqual("PATIENTS", ta.TableName);
            // verify dataset is not null
            Assert.IsNotNull(dSet);
            // verify row count is grater than 0
            Assert.IsTrue(ta.RowCount > 0);
            // verify dataset tables collection contains only one table
            Assert.AreEqual(1, dSet.Tables.Count);
        }
    }
}
