using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Mappings
{
    public class ClientDatabaseBase : IDisposable
    {

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose();
            }
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
            //this.Dispose();
        }
    }
}
