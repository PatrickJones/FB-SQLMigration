using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings.InMemoryMappings;
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

            try
            {
                foreach (DataRow row in TableAgent.DataSet.Tables[FbTableName].Rows)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTID"].ToString();
                    var userId = MemoryPatientInfo.GetUserId(MigrationVariables.CurrentSiteId, patId);

                    if (userId != Guid.Empty)
                    {
                        var dm = new DiabetesManagementData
                        {
                            UserId = userId,
                            LowBGLevel = (row["LOWBGLEVEL"] is DBNull) ? 19 : (Int16)row["LOWBGLEVEL"],
                            HighBGLevel = (row["HIGHBGLEVEL"] is DBNull) ? 201 : (Int16)row["HIGHBGLEVEL"],
                            PremealTarget = (row["PREMEALTARGET"] is DBNull) ? 50 : (Int32)row["PREMEALTARGET"],
                            PostmealTarget = (row["POSTMEALTARGET"] is DBNull) ? 50 : (Int32)row["POSTMEALTARGET"],
                            ModifiedDate = (row["LASTMODIFIEDDATE"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["LASTMODIFIEDDATE"].ToString()),
                            ModifiedUserId = (row["LASTMODIFIEDBYUSER"] is DBNull) ? Guid.Empty : mu.ParseGUID(row["LASTMODIFIEDBYUSER"].ToString())
                        };

                        // add to temp collection for addition with "PatientDevicesMapping"
                        MemoryDiabetesManagementData.DMDataCollection.Add(dm);

                        var ibId = mu.FindInsulinBrandId(row["INSULINBRAND"].ToString());
                        var imId = mu.FindInsulinMethodId(row["INSULINMETHOD"].ToString());
                        var typeId = mu.FindDMTypeId(row["DMTYPE"].ToString());

                        CareSetting careset = new CareSetting();
                        careset.UserId = userId;
                        careset.HyperglycemicLevel = (row["HYPERGLYCEMICLEVEL"] is DBNull) ? 0 : (Int16)row["HYPERGLYCEMICLEVEL"];
                        careset.HypoglycemicLevel = (row["HYPOGLYCEMICLEVEL"] is DBNull) ? 0 : (Int16)row["HYPOGLYCEMICLEVEL"];
                        careset.InsulinMethod = imId;
                        careset.InsulinBrand = ibId;
                        careset.DiabetesManagementType = typeId;
                        careset.DateModified = (row["LASTMODIFIEDDATE"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["LASTMODIFIEDDATE"].ToString());

                        var ct = (row["DMCONTROLTYPE"] is DBNull) ? mu.ParseDMControlTypes(0) : mu.ParseDMControlTypes((Int32)row["DMCONTROLTYPE"]);
                        foreach (var item in ct)
                        {
                            DiabetesControlType dct = new DiabetesControlType();
                            dct.ControlName = item.Key;
                            //dct.CareSettingsId = careset.CareSettingsId;
                            dct.DMDataId = dm.DMDataId;
                            dct.IsEnabled = (item.Value) ? true : false;

                            careset.DiabetesControlTypes.Add(dct);
                            //TransactionManager.DatabaseContext.DiabetesControlTypes.Add(dct);
                        }

                        if (CanAddToContext(careset.UserId, careset.HyperglycemicLevel, careset.HypoglycemicLevel))
                        {
                            TransactionManager.DatabaseContext.CareSettings.Add(careset);
                        }
                        else
                        {
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = FbTableName,
                                    ObjectType = typeof(CareSetting),
                                    JsonSerializedObject = JsonConvert.SerializeObject(careset),
                                    FailedReason = "Unable to add Care Setting to database."
                                });
                        }

                        //if (CanAddToContext(dm.UserId, dm.LowBGLevel, dm.HighBGLevel, dm.PostmealTarget, dm.PremealTarget))
                        //{
                        //    TransactionManager.DatabaseContext.DiabetesManagementDatas.Add(dm);
                        //}
                        //else
                        //{
                        //    TransactionManager.FailedMappingCollection
                        //        .Add(new FailedMappings
                        //        {
                        //            Tablename = FbTableName,
                        //            ObjectType = typeof(DiabetesManagementData),
                        //            JsonSerializedObject = JsonConvert.SerializeObject(dm),
                        //            FailedReason = "Diabetes Management Data already exist in database."
                        //        });
                        //}
                    }
                }

                TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating CareSetting mapping.", e);
            }
        }

        //private bool CanAddToContext(Guid userId, int lowBGLevel, int highBGLevel, int postmealTarget, int premealTarget)
        //{
        //    using (var ctx = new NuMedicsGlobalEntities())
        //    {
        //        return (ctx.DiabetesManagementDatas.Any(a => a.UserId == userId && a.LowBGLevel == lowBGLevel && a.HighBGLevel == highBGLevel && a.PostmealTarget == postmealTarget && a.PremealTarget == premealTarget)) ? false : true;
        //    }
        //}

        private bool CanAddToContext(Guid userId, int hyperglycemicLevel, int hypoglycemicLevel)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.CareSettings.Any(a => a.UserId == userId && a.HyperglycemicLevel == hyperglycemicLevel && a.HypoglycemicLevel == hypoglycemicLevel)) ? false : true;
            }
        }
    }
}
