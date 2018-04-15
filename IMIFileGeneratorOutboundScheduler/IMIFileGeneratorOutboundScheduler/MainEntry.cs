﻿using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            bool IsSuccess = true;
            string errMsg = string.Empty;

            // Basic validations
            ValidateArgs chkArgsEnum = ValidateArgs.Success;
            chkArgsEnum = (ValidateArgs)ValidateParameters(args);

            ProcessDirectories request = null;

            ProcessMetadataFiles objdir = null;

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

            objdir = new ProcessMetadataFiles();
            objdir.GetMDFiles();

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

            if (args.Length > 2)
            {
                argsEnum = ValidateArgs.ExcessArguments;               
                return argsEnum;
            }
            if (args[0].ToUpper()!="BATCH")
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