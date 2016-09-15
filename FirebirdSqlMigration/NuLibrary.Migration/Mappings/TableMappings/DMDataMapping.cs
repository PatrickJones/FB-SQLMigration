using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with -
    /// </summary>
    public class DMDataMapping : BaseMapping
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>

        public DMDataMapping() : base("DMDATA")
        {

        }

        public DMDataMapping(string tableName) :base(tableName)
        {

        }

        public void CreateDMDataMapping()
        {
            MappingUtilities mu = new MappingUtilities();
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var pd1 = new PatientDevice
                {
                    PatientId = (String)row["PATIENTID"],
                    DeviceModel = (String)row["METERMODEL1"]
                };
                var pd2 = new PatientDevice
                {
                    PatientId = (String)row["PATIENTID"],
                    DeviceModel = (String)row["METERMODEL2"]
                };


                var dm = new DiabetesManagementData
                {
                    PatientId = (String)row["PATIENTID"],
                    LowBGLevel = (Int32)row["LOWBGLEVEL"],                                                                                
                    HighBGLevel = (Int32)row["HighBGLevel"],
                    //HyperglycemicLevel = (String)row["HyperglycemicLevel"],
                    //HypoglycemicLevel = (String)row["HypoglycemicLevel"],
                    //InsulinMethod = (String)row["InsulinMethod"],
                    PremealTarget = (Int32)row["PremealTarget"],
                    PostmealTarget = (Int32)row["PostmealTarget"],
                    ModifiedDate = (DateTime)row["ModifiedDate"],
                    ModifiedUserId = (Guid)row["ModifiedUserId"]
                };

                var pat = mu.FindPatient(pd1.PatientId);
                pat.PatientDevices.Add(pd1);
                pat.PatientDevices.Add(pd2);

                //pd.DiabetesManagementData.Add(dm);  //ERROR: DOES NOT CONTAIN A DEFINITION FOR ADD
                var careset = mu.FindPatientCareSetting(pd1.PatientId);
                careset.PatientId = (String)row["PATIENTID"];
                careset.HyperglycemicLevel = (Int32)row["HyperglycemicLevel"];
                careset.HypoglycemicLevel = (Int32)row["HypoglycemicLevel"];
                //careset.InsulinMethod = (String)row["INSULINMETHOD"];
                //careset.InsulinBrand = (String)row["INSULINBRAND"];
                //careset.DiabetesManagementType = (String)row["DMTYPE"];
                careset.DateModified = (DateTime)row["LASTMODIFIEDDATE"];

                

                TransactionManager.DatabaseContext.DiabetesManagementDatas.Add(dm);
            }
        }
    }
}
