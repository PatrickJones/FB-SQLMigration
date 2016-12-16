using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
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

        MappingUtilities mu = new MappingUtilities();
        AspnetDbHelpers aHelper = new AspnetDbHelpers();

        public void CreateDMDataMapping()
        {
            
            foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
            {
                // get userid from old aspnetdb matching on patientid #####.#####
                var patId = row["PATIENTID"].ToString();
                var userId = aHelper.GetUserIdFromPatientId(patId);

                if (userId != Guid.Empty)
                {
                    var dm = new DiabetesManagementData
                    {
                        UserId = userId,
                        LowBGLevel = (row["LOWBGLEVEL"] is DBNull) ? 19 : (Int32)row["LOWBGLEVEL"],
                        HighBGLevel = (row["HighBGLevel"] is DBNull) ? 201 : (Int32)row["HighBGLevel"],
                        PremealTarget = (row["PremealTarget"] is DBNull) ? 50 : (Int32)row["PremealTarget"],
                        PostmealTarget = (row["PostmealTarget"] is DBNull) ? 50 : (Int32)row["PostmealTarget"],
                        ModifiedDate = (row["ModifiedDate"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["ModifiedDate"].ToString()),
                        ModifiedUserId = (row["ModifiedUserId"] is DBNull) ? Guid.Empty : mu.ParseGUID(row["ModifiedUserId"].ToString())
                    };


                    var ibId = mu.FindInsulinBrandId((String)row["INSULINBRAND"]);
                    var imId = mu.FindInsulinMethodId((String)row["INSULINMETHOD"]);
                    var typeId = mu.FindDMTypeId((String)row["DMTYPE"]);

                    CareSetting careset = new CareSetting();
                    careset.UserId = userId;
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
                    TransactionManager.DatabaseContext.CareSettings.Add(careset);
                }
            }
        }
    }
}
