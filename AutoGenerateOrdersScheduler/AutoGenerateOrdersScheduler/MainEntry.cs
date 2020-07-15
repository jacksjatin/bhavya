using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenerateOrdersScheduler
{
    class MainEntry : IDisposable
    {
        private bool disposed = false;
        static void Main(string[] args)
        {
            ProcessOrders obj = new ProcessOrders();
            obj.ProccessVaccineOrders();

        }

        #region "Dispose methods"
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // disposing has been done.
                disposed = true;

            }
        }
        #endregion
    }
}
