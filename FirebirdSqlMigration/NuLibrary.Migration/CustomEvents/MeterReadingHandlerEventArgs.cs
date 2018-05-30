using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.CustomEvents
{
    public class MeterReadingHandlerEventArgs : EventArgs
    {
        public MeterReadingHandlerEventArgs(string extractionName, bool extractionSuccessful)
        {
            ExtractionName = extractionName;
            ExtractionSuccessful = extractionSuccessful;
        }

        public MeterReadingHandlerEventArgs(bool extractionSuccessful)
        {
            ExtractionSuccessful = extractionSuccessful;
        }

        public string ExtractionName { get; set; }
        public bool ExtractionSuccessful { get; set; }
    }
}
