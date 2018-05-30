using NuLibrary.Migration.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SqlValidations
{
    public class ValidateTables
    {
        List<ITableValidate> valTables = new List<ITableValidate>(7);
        Dictionary<string, bool> tableValidations = new Dictionary<string, bool>();

        public ValidateTables()
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
            valTables.Add(new SubscriptionTypeVaidation());
        }

        public IList<ITableValidate> GetReadOnlyValidations()
        {
            return valTables.AsReadOnly();
        }

        public IReadOnlyDictionary<string, bool> ValidateAll()
        {
            foreach (var tb in valTables)
            {
                tableValidations.Add(tb.TableName, tb.ValidateTable());
            }

            return tableValidations;
        }

        public IReadOnlyDictionary<string, bool> ValidateAndSync()
        {
            foreach (var tb in valTables)
            {
                if (!tb.ValidateTable())
                {
                    tb.SyncTable();
                }

                tableValidations.Add(tb.TableName, true);
            }

            return tableValidations;
        }
    }
}
