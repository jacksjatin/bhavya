using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIFileGeneratorOutboundScheduler
{
    public class ProcessMetadataFiles
    {
        private Hashtable htAppConfig;
        private HelperClasses.Helpers helpers;

        private ProcessDirectories ProcessObj;
        public ProcessMetadataFiles()
        {
            helpers = new HelperClasses.Helpers();
            
        }


        public void GetMDFiles()
        {
            ArrayList movedFiles = new ArrayList();
            ArrayList filesToProcess = new ArrayList();
            RetrieveConfigInformation();
            ProcessObj = new ProcessDirectories();
            try
            {
                int totalCount = 0;
                List<string> fileNames = null;
                fileNames = helpers.GetFiles(htAppConfig["InputRootFolder"].ToString(), totalCount, string.Format("*"));
                if (null == fileNames || fileNames.Count == 0) return;
                for (int i = 0; i < fileNames.Count; i++)
                {
                    ProcessObj.ProcessFolders(Path.Combine(htAppConfig["InputRootFolder"].ToString(), fileNames[i]));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void RetrieveConfigInformation()
        {
            try
            {
                htAppConfig = helpers.GetappConfigData();
            }
            catch (Exception ex)
            {
                throw new Exception(helpers.FormatErrorMessage(ex.Message));
            }
        }

    }
}
