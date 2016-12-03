using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Interfaces
{
    public interface ITableValidate
    {
        void SyncTable();
        bool ValidateTable();
        string TableName { get; }
    }
}
