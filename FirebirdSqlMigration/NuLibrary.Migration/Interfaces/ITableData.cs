using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuLibrary.Migration.Interfaces
{
    public interface ITableData
    {
        string TableName { get; set; }
        DataTable GetDataTable();
        XDocument GetTableSchema();
    }
}
