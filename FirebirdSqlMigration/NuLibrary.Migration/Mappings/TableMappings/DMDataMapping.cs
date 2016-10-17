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

        public DMDataMapping(string tableName) : base(tableName)
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
                    PremealTarget = (Int32)row["PremealTarget"],
                    PostmealTarget = (Int32)row["PostmealTarget"],
                    ModifiedDate = (DateTime)row["ModifiedDate"],
                    ModifiedUserId = (Guid)row["ModifiedUserId"]
                };


                var ibId = mu.FindInsulinBrandId((String)row["INSULINBRAND"]);
                var imId = mu.FindInsulinMethodId((String)row["INSULINMETHOD"]);
                var typeId = mu.FindDMTypeId((String)row["DMTYPE"]);

                CareSetting careset = new CareSetting();
                careset.PatientId = (String)row["PATIENTID"];
                careset.HyperglycemicLevel = (Int32)row["HyperglycemicLevel"];
                careset.HypoglycemicLevel = (Int32)row["HypoglycemicLevel"];
                careset.InsulinMethod = imId;
                careset.InsulinBrand = ibId;
                careset.DiabetesManagementType = typeId;
                careset.DateModified = (DateTime)row["LASTMODIFIEDDATE"];
                //pat.CareSettings.Add(careset);

                var ct = mu.ParseDMControlTypes((int)row["DMCONTROLTYPE"]);
                DiabetesControlType dct = new DiabetesControlType();

                foreach (var item in ct)
                {
                    dct.ControlName = item.Key;
                    dct.CareSettingsId = careset.CareSettingsId;
                    dct.DMDataId = dm.DMDataId;
                    if (item.Value)
                    {
                        dct.IsEnabled = true;
                    }
                    else
                    {
                        dct.IsEnabled = false;
                    }
                    TransactionManager.DatabaseContext.DiabetesControlTypes.Add(dct);
                }

                TransactionManager.DatabaseContext.DiabetesManagementDatas.Add(dm);
                TransactionManager.DatabaseContext.PatientDevices.Add(pd1);
                TransactionManager.DatabaseContext.PatientDevices.Add(pd2);
                TransactionManager.DatabaseContext.CareSettings.Add(careset);
            }
        }
    }
}
