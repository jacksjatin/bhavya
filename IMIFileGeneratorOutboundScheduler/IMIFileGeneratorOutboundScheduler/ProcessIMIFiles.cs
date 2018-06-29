using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIFileGeneratorOutboundScheduler
{
    public class ProcessIMIFiles
    {

        private Hashtable htAppConfig;
        private HelperClasses.Helpers helpers;
        private UploadToSFTP objuploadToSFTP;
              
        public ProcessIMIFiles()
        {
            helpers = new HelperClasses.Helpers();

        }

        public void GetIMIFiles()
        {
            ArrayList movedFiles = new ArrayList();
            ArrayList filesToProcess = new ArrayList();
            RetrieveConfigInformation();
            objuploadToSFTP = new UploadToSFTP();
            try
            {
                bool iSexitFlag = false;
                bool bSleep = false;
                int batchCount;
                int batchSize;
                int fileCount = 0;
                int intervalTime;
                Int32.TryParse(htAppConfig["SFTPBatchCount"].ToString(), out batchCount);
                Int32.TryParse(htAppConfig["SFTPBatchSize"].ToString(), out batchSize);
                Int32.TryParse(htAppConfig["SFTPSleepInterval"].ToString(), out intervalTime);
                bSleep = Convert.ToBoolean(htAppConfig["SFTPSleep"].ToString());
                int totalCount = batchCount * batchSize;
                List<string> fileNames = null;
                fileNames = helpers.GetFiles(htAppConfig["SFTPSourceLocation"].ToString(), totalCount, string.Format("*.imi"));
                int sleepTime = intervalTime;
                if (null == fileNames || fileNames.Count == 0) return;

                //Moves the files to Inprocess
                helpers.MoveFiles(fileNames, htAppConfig["SFTPSourceLocation"].ToString(), htAppConfig["SFTPInProcessLocation"].ToString());

            
                for (int iIndex = 0; iIndex < batchCount; iIndex++)
                {
                    #region "Inner batch size loop..."
                    for (int jIndex = 0; jIndex < batchSize; jIndex++)
                    {
                        try
                        {
                           
                            NameValueCollection nvcFile = new NameValueCollection();
                            string fileName = string.Empty;
                            FileInfo fileInfo = new FileInfo(Path.Combine(htAppConfig["SFTPInProcessLocation"].ToString(), fileNames[fileCount]));
                            fileName = fileNames[fileCount].ToString();
                            objuploadToSFTP.uploadFiletoSFTP(fileInfo);
                            fileCount++;
                            if (fileCount == fileNames.Count)
                            {
                                iSexitFlag = true;
                                break;
                            }
                            else continue;
                        }
                        catch (Exception ex)
                        {
                            //continue
                        }

                      
                    }

                    #endregion

                    if (iSexitFlag) break;

                    //Check to Sleep or Not
                    if (bSleep) System.Threading.Thread.Sleep(sleepTime * 1000);
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
