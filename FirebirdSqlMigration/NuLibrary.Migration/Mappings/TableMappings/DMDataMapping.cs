using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    /// <summary>
    /// Note: Has relationship with -
    /// </summary>
    public class DMDataMapping : BaseMapping, IContextHandler
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

        public ICollection<CareSetting> CompletedMappings = new List<CareSetting>();

        public int RecordCount = 0;
        public int FailedCount = 0;


        public void CreateDMDataMapping()
        {

            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    // get userid from old aspnetdb matching on patientid #####.#####
                    var patId = row["PATIENTID"].ToString();
                    var userId = MemoryMappings.GetUserIdFromPatientInfo(MigrationVariables.CurrentSiteId, patId);

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
                        MemoryMappings.AddDiabetesManagementData(dm);

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
                            dct.DMDataId = dm.DMDataId;
                            dct.IsEnabled = (item.Value) ? true : false;

                            careset.DiabetesControlTypes.Add(dct);
                        }

                        if (CanAddToContext(careset.UserId, careset.HyperglycemicLevel, careset.HypoglycemicLevel))
                        {
                            CompletedMappings.Add(careset);
                        }
                        else
                        {
                            MappingStatistics.LogFailedMapping("CareSettings", typeof(CareSetting), JsonConvert.SerializeObject(careset), "Unable to add Care Setting to database.");
                            FailedCount++;
                        }
                    }
                }

                MappingStatistics.LogMappingStat("DMDATA", RecordCount, "CareSettings", 0, CompletedMappings.Count, FailedCount);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating CareSetting mapping.", e);
            }
        }

        public void SaveChanges()
        {
            try
            {
                var stats = new SqlTableStats
                {
                    Tablename = "CareSettings",
                    PreSaveCount = CompletedMappings.Count()
                };

                //Array.ForEach(CompletedMappings.ToArray(), c => {
                //    var exist = TransactionManager.DatabaseContext.Patients.Any(a => a.UserId == c.UserId);
                //    if (!exist)
                //    {
                //        CompletedMappings.Remove(c);
                //    }
                //});

                var q = from cm in CompletedMappings
                        from ps in mu.GetPatients()
                        where cm.UserId == ps.UserId
                        select cm;

                TransactionManager.DatabaseContext.CareSettings.AddRange(q);
                TransactionManager.DatabaseContext.SaveChanges();
                stats.PostSaveCount = TransactionManager.DatabaseContext.CareSettings.Count();

                MappingStatistics.SqlTableStatistics.Add(stats);
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating CareSetting entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving CareSetting entity", e);
            }
        }

        private bool CanAddToContext(Guid userId, int hyperglycemicLevel, int hypoglycemicLevel)
        {
            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.CareSettings.Any(a => a.UserId == userId && a.HyperglycemicLevel == hyperglycemicLevel && a.HypoglycemicLevel == hypoglycemicLevel)) ? false : true;
            }
        }
    }
}
