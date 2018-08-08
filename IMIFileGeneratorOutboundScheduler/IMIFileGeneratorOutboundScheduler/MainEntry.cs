using IMIFileGeneratorOutboundScheduler.HelperClasses;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIFileGeneratorOutboundScheduler
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
        public static void Main(string[] args)
        {
            

            bool IsSuccess = true;
            string errMsg = string.Empty;

            // Basic validations
            ValidateArgs chkArgsEnum = ValidateArgs.Success;
            chkArgsEnum = (ValidateArgs)ValidateParameters(args);

            ProcessMetadataFiles objdir = null;
            ProcessIMIFiles objImiFiles = null;


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


            if (args[0].Equals("BATCH", StringComparison.CurrentCultureIgnoreCase))
            {
                //Generate IMI Files
                objdir = new ProcessMetadataFiles();
                objdir.GetMDFiles();
                return;
            }
            else if (args[0].Equals("UPLOAD", StringComparison.CurrentCultureIgnoreCase))
            {
                //Upload IMI files to SFTP
                objImiFiles = new ProcessIMIFiles();
                objImiFiles.GetIMIFiles();
                return;
            }



            //request = new ProcessDirectories();           
            //request.ProcessFolders();


        }       

        private static ValidateArgs ValidateParameters(string[] args)
        {
            ValidateArgs argsEnum = ValidateArgs.Success;

            if (args.Length <= 0)
            {
                argsEnum = ValidateArgs.NoArgument;
                return argsEnum;
            }

            if (args.Length > 1)
            {
                argsEnum = ValidateArgs.ExcessArguments;
                return argsEnum;
            }
            if (!((args[0].Trim().Equals("BATCH", StringComparison.CurrentCultureIgnoreCase)) ||
                   (args[0].Trim().Equals("UPLOAD", StringComparison.CurrentCultureIgnoreCase))
               ))
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
