using NuLibrary.Migration.GlobalVar;
using NuLibrary.Migration.Mappings.InMemoryMappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class NuLicenseMapping : BaseMapping
    {
        public NuLicenseMapping() : base("NULICENSE")
        {

        }

        public NuLicenseMapping(string tableName) : base(tableName)
        {
            
        }

        MappingUtilities mu = new MappingUtilities();

        public void CreateNuLicenseMapping()
        {
            try
            {
                var dataSet = TableAgent.DataSet.Tables[FbTableName].Rows;

                foreach (DataRow row in dataSet)
                {
                    var numed = row["LICENSETYPE"].ToString();

                    if (numed == "numedics")
                    {
                        var licenseCnt = (row["USERS"] is DBNull) ? 0 : (Int16)row["USERS"];
                        var expires = (row["EXPIRES"] is DBNull) ? new DateTime(1800, 1, 1) : mu.ParseFirebirdDateTime(row["EXPIRES"].ToString());

                        MemoryNuLicenseInfo.NuLicenses.Add(new Tuple<int, int, DateTime>(MigrationVariables.CurrentSiteId, licenseCnt, expires));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating NuLicense mapping", e);
            }
        }
    }
}
