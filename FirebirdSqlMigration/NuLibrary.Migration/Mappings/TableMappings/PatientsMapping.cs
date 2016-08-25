using NuLibrary.Migration.FBDatabase.FBTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings.TableMappings
{
    public class PatientsMapping : BaseMapping
    {
        public PatientsMapping() : base("Patients")
        {

        }

        public PatientsMapping(string tableName) :base(tableName)
        {

        }


    }
}
