using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator
{
    class MainEntry : IDisposable
    {
        public enum ValidateArgs : int
        {
            Success = 0,
            NoArgument = 1,
            InvalidParameter = 2,
            ExcessArguments = 4
        }

        private bool disposed = false;

        static void Main(string[] args)
        {
            bool IsSuccess = true;
            string errMsg = string.Empty;

            DailySummaryReporter daily = null;
            MonthlySummaryReporter monthly = null;

            // Basic validations
            ValidateArgs chkArgsEnum = ValidateArgs.Success;
            chkArgsEnum = (ValidateArgs)ValidateParameters(args);

            switch (chkArgsEnum)
            {
                case ValidateArgs.NoArgument:
                    IsSuccess = false;
                    errMsg = "";
                    break;

                case ValidateArgs.InvalidParameter:
                    IsSuccess = false;
                    errMsg = " ";
                    break;

                case ValidateArgs.ExcessArguments:
                    IsSuccess = false;
                    errMsg = " ";
                    break;

                case ValidateArgs.Success:
                    break;

            }

            if (!IsSuccess)
            {
                return;
            }

            if (args[0].Equals("DAILY", StringComparison.CurrentCultureIgnoreCase))
            {
                //Generate Daily Records Summary
                daily = new DailySummaryReporter();
                daily.processDailyRecords();
                return;
            }

            if (args[0].Equals("MONTHLY", StringComparison.CurrentCultureIgnoreCase))
            {
                //Generate Monthly Records Summary
                monthly = new MonthlySummaryReporter();
                monthly.processMonthlyRecords();
                return;
            }

        }

        private static ValidateArgs ValidateParameters(string[] args)
        {
            ValidateArgs argsEnum = ValidateArgs.Success;

            if (args.Length <= 0)
            {
                argsEnum = ValidateArgs.NoArgument;
                return argsEnum;
            }

            if (args.Length > 2)
            {
                argsEnum = ValidateArgs.ExcessArguments;
                return argsEnum;
            }
            if (!((args[0].Trim().Equals("DAILY", StringComparison.CurrentCultureIgnoreCase)) ||
                   (args[0].Trim().Equals("MONTHLY", StringComparison.CurrentCultureIgnoreCase))))
            {
                argsEnum = ValidateArgs.InvalidParameter;
                return argsEnum;
            }

            return argsEnum;

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
