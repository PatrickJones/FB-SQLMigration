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
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                var pd = new PatientDevice
                {
                    PatientId = (String)row["PATIENTID"],
                    DeviceModel = (String)row["METERMODEL1"]
                };

                var dm = new DiabetesManagementData
                {
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
                
                //pd.DiabetesManagementData.Add(dm);  //ERROR: DOES NOT CONTAIN A DEFINITION FOR ADD

                TransactionManager.DatabaseContext.PatientDevices.Add(pd);
            }
        }
    }
}
