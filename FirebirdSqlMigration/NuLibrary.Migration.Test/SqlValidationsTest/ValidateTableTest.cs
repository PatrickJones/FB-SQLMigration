using NuLibrary.Migration.SqlValidations;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.Interfaces;

namespace NuLibrary.Migration.Test.SqlValidationsTest
{
    /// <summary>
    /// Summary description for ValidateTableTest
    /// </summary>
    [TestClass]
    public class ValidateTableTest
    {
        ValidateTables vt;
        IList<ITableValidate> list;
        IReadOnlyDictionary<string, bool> dict;

        public ValidateTableTest()
        {
            vt = new ValidateTables();
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

        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void ValidateTableTestInitialize()
        {
            dict = vt.ValidateAll();
            list = vt.GetReadonlyValidations();
        }

        [TestMethod]
        public void Verify_Readonly_Collection_Of_Instances()
        {
            List<Type> types = new List<Type> {
                typeof(ApplicationValidation),
                typeof(CheckStatusValidation),
                typeof(PaymentMethodValidation),
                typeof(ReadingEventTypeValidation),
                typeof(TherapyTypeValidation),
                typeof(UserTypeValidation),
                typeof(InsulinTypeValidation)
            };

            // verify generic list type
            Assert.AreEqual(list.GetType().GenericTypeArguments[0].Name, "ITableValidate");

            // verify returned collection is readonly and contains exact number of instances
            Assert.IsTrue(list.IsReadOnly);
            Assert.AreEqual(list.Count, 7);

            // verify collection contains one instance of each table validation class
            Assert.IsTrue(types.Contains(list[0].GetType()));
            Assert.IsTrue(types.Contains(list[1].GetType()));
            Assert.IsTrue(types.Contains(list[2].GetType()));
            Assert.IsTrue(types.Contains(list[3].GetType()));
            Assert.IsTrue(types.Contains(list[4].GetType()));
            Assert.IsTrue(types.Contains(list[5].GetType()));
            Assert.IsTrue(types.Contains(list[6].GetType()));
        }

        [TestMethod]
        public void Dictionary_Contains_Validations()
        {
            // verify dicationary is readonly
            Assert.AreEqual(list.GetType().Name, "ReadOnlyCollection`1");
            // verify dictionary key type
            Assert.AreEqual(dict.GetType().GenericTypeArguments[0].Name.ToLower(), "string");
            // verify dictionary value tpye
            Assert.AreEqual(dict.GetType().GenericTypeArguments[1].Name.ToLower(), "boolean");
            // verify dictionary count matches instance list count
            Assert.AreEqual(dict.Count, list.Count);
        }
    }
}
