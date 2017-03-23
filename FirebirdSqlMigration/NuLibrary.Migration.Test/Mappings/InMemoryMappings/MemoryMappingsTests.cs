using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Test
{
    [TestClass()]
    public class MemoryMappingsTests
    {
        [TestMethod()]
        public void AddDiabetesManagementDataTest()
        {
            //Arrange
            var dm = Builder<DiabetesManagementData>.CreateNew().Build();

            //Act
            MemoryMappings.AddDiabetesManagementData(dm);
            var getDm = MemoryMappings.GetAllDiabetesManagementData().FirstOrDefault(f => f.DMDataId == dm.DMDataId);

            //Assert
            Assert.IsNotNull(getDm);
        }

        [TestMethod()]
        public void GetAllDiabetesManagementDataTest()
        {
            //Arrange
            var dm = Builder<DiabetesManagementData>.CreateListOfSize(10).Build();

            //Act
            foreach (var d in dm)
            {
                MemoryMappings.AddDiabetesManagementData(d);
            }
            
            //Assert
            Assert.AreEqual(10, dm.Count);
        }

        [TestMethod()]
        public void AddInstitutionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllInstitutionsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddCompnayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllCompaniesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddNuLicenseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllNuLicensesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserIdFromPatientInfoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddPatientInfoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllUserIdsFromPatientInfoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PatientCountTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NuLicenseCountTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CompaniesCountTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void InstitutionsCountTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DMDataCountTest()
        {
            Assert.Fail();
        }
    }
}