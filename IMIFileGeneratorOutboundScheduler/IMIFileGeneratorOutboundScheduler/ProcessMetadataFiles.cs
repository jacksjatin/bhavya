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
                bool iSexitFlag = false;
                bool bSleep = false;
                int batchCount;
                int batchSize;
                int fileCount = 0;
                int intervalTime;
                Int32.TryParse(htAppConfig["BatchCount"].ToString(), out batchCount);
                Int32.TryParse(htAppConfig["BatchSize"].ToString(), out batchSize);
                Int32.TryParse(htAppConfig["SleepInterval"].ToString(), out intervalTime);
                bSleep = Convert.ToBoolean(htAppConfig["Sleep"].ToString());
                int totalCount = batchCount * batchSize;
                List<string> fileNames = null;
                fileNames = helpers.GetFiles(htAppConfig["OutboundSourceLocation"].ToString(), totalCount, string.Format("*.idx"));
                int sleepTime = intervalTime;
                if (null == fileNames || fileNames.Count == 0) return;

                //Moves the files to Inprocess
                helpers.MoveFiles(fileNames, htAppConfig["OutboundSourceLocation"].ToString(), htAppConfig["OutboundInProcessLocation"].ToString());

                helpers.MoveImageFiles(fileNames, htAppConfig["OutboundSourceLocation"].ToString(), htAppConfig["OutboundInProcessLocation"].ToString());
                //for (int i = 0; i < fileNames.Count; i++)
                //{
                //    ProcessObj.ProcessFolders(Path.Combine(htAppConfig["InputRootFolder"].ToString(), fileNames[i]));
                //}

                for (int iIndex = 0; iIndex < batchCount; iIndex++)
                {
                    #region "Inner batch size loop..."
                    for (int jIndex = 0; jIndex < batchSize; jIndex++)
                    {
                        try
                        {
                            //if (Convert.ToBoolean(htAppConfig["OutboundLogIncoming"]))
                            //    helpers.CopyFile(Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), fileNames[fileCount]),
                            //                                    Path.Combine(htAppConfig["OutboundArchiveLocation"].ToString(), fileNames[fileCount]), true);

                            NameValueCollection nvcFile = new NameValueCollection();
                            string fileName = string.Empty;
                            FileInfo fileInfo = new FileInfo(Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), fileNames[fileCount]));
                            fileName = fileNames[fileCount].ToString();
                           // MoveToDealerDirectory(fileInfo);
                            ProcessObj.ProcessFolders(fileInfo);
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
