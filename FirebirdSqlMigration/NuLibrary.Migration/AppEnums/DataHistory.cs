using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.AppEnums
{
    public static class DataHistory
    {
        public enum HistoryRange
        {
            All = 1,
            Month30 = 30,
            Month60 = 60,
            Month90 = 90,
            Month180 = 180,
            Year1 = 365,
            Year2 = 730,
            Year3 = 1095
        }
    }
}
