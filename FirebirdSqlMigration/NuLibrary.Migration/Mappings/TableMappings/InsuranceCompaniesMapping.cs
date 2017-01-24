using Newtonsoft.Json;
using NuLibrary.Migration.FBDatabase.FBTables;
using NuLibrary.Migration.Interfaces;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
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
    public class InsuranceCompaniesMapping : BaseMapping, IContextHandler
    {
        /// <summary>
        /// Default constructor that passes Firebird Table name to base class
        /// </summary>
        public InsuranceCompaniesMapping() : base("INSURANCECOS")
        {

        }

        public InsuranceCompaniesMapping(string tableName) :base(tableName)
        {

        }

        MappingUtilities map = new MappingUtilities();

        public ICollection<InsuranceProvider> CompletedMappings = new List<InsuranceProvider>();

        public int RecordCount = 0;
        public int FailedCount = 0;

        public void CreateInsuranceCompanyMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;
                RecordCount = TableAgent.RowCount;

                foreach (DataRow row in dataSet)
                {
                    if (!String.IsNullOrEmpty(row["NAME"].ToString()))
                    {
                        var name = row["NAME"].ToString();
                        var kId = row["KEYID"].ToString();

                        MemoryMappings.AddCompnay(kId, name);

                        var ips = new InsuranceProvider
                        {
                            Name = name,
                            IsActive = map.ParseFirebirdBoolean(row["ISACTIVE"].ToString()),
                            InActiveDate = map.ParseFirebirdDateTime(row["INACTIVEDATE"].ToString())
                        };

                        var adr = new InsuranceAddress
                        {
                            Street1 = (row["STREET1"] is DBNull) ? String.Empty : row["STREET1"].ToString(),
                            Street2 = (row["STREET2"] is DBNull) ? String.Empty : row["STREET2"].ToString(),
                            Street3 = (row["STREET3"] is DBNull) ? String.Empty : row["STREET3"].ToString(),
                            City = (row["CITY"] is DBNull) ? String.Empty : row["CITY"].ToString(),
                            //County = (row["COUNTY"] is DBNull) ? String.Empty : row["COUNTY"].ToString(),
                            State = (row["STATE"] is DBNull) ? String.Empty : row["STATE"].ToString(),
                            Zip = (row["ZIP"] is DBNull) ? String.Empty : row["ZIP"].ToString(),
                            Country = (row["COUNTRY"] is DBNull) ? String.Empty : row["COUNTRY"].ToString()
                        };

                        var cont = new InsuranceContact
                        {
                            FullName = (row["CONTACTNAME"] is DBNull) ? String.Empty : row["CONTACTNAME"].ToString(),
                            Email = (row["EMAIL"] is DBNull) ? String.Empty : row["EMAIL"].ToString()
                        };

                        ips.InsuranceAddresses.Add(adr);
                        ips.InsuranceContacts.Add(cont);

                        if (CanAddToContext(ips.Name))
                        {
                            //TransactionManager.DatabaseContext.InsuranceProviders.Add(ips);
                            CompletedMappings.Add(ips);
                        }
                        else
                        {
                            TransactionManager.FailedMappingCollection
                                .Add(new FailedMappings
                                {
                                    Tablename = FbTableName,
                                    ObjectType = typeof(InsuranceProvider),
                                    JsonSerializedObject = JsonConvert.SerializeObject(ips),
                                    FailedReason = "Insurance Provider already exist in database."
                                });

                            FailedCount++;
                        }
                    }
                }
                //TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error creating InsuranceProvider mapping.", e);
            }
        }

        public void AddToContext()
        {
            TransactionManager.DatabaseContext.InsuranceProviders.AddRange(CompletedMappings);
        }

        public void SaveChanges()
        {
            try
            {
               TransactionManager.DatabaseContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Error validating InsuranceProvider entity", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error saving InsuranceProvider entity", e);
            }
        }


        private bool CanAddToContext(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                return false;
            }

            using (var ctx = new NuMedicsGlobalEntities())
            {
                return (ctx.InsuranceProviders.Any(a => a.Name == providerName)) ? false : true;
            }
        }

    }
}
