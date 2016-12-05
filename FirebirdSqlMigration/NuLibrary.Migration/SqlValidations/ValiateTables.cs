using NuLibrary.Migration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class ValiateTables
    {
        List<ITableValidate> valTables = new List<ITableValidate>();
        Dictionary<string, bool> tableValidations = new Dictionary<string, bool>();

        public ValiateTables()
        {
            Intit();
        }

        private void Intit()
        {
            valTables.Add(new ApplicationValidation());
            valTables.Add(new CheckStatusValidation());
            valTables.Add(new PaymentMethodValidation());
            valTables.Add(new ReadingEventTypeValidation());
            valTables.Add(new TherapyTypeValidation());
            valTables.Add(new UserTypeValidation());
            valTables.Add(new InsulinTypeValidation());
        }

        public Dictionary<string, bool> ValidateAll()
        {
            foreach (var tb in valTables)
            {
                tableValidations.Add(tb.TableName, tb.ValidateTable());
            }

            return tableValidations;
        }
    }
}
