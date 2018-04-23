using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIAcknowledgementValidator
{

    public class ProcessAckFiles
    {

        private Hashtable htAppConfig;
        private HelperClasses.Helpers helpers;
        private ValidateAckFile ObjvalAck;

        public ProcessAckFiles()
        {
            helpers = new HelperClasses.Helpers();
        }

        public void GetAckFiles()
        {
            ArrayList movedFiles = new ArrayList();
            ArrayList filesToProcess = new ArrayList();
            
            RetrieveConfigInformation();
            ObjvalAck = new ValidateAckFile();
            try
            {
                bool iSexitFlag = false;
                bool bSleep = false;
                int batchCount;
                int batchSize;
                int intervalTime;
                int fileCount = 0;
                List<string> fileNames = null;
                Int32.TryParse(htAppConfig["BatchCount"].ToString(), out batchCount);
                Int32.TryParse(htAppConfig["BatchSize"].ToString(), out batchSize);
                Int32.TryParse(htAppConfig["SleepInterval"].ToString(), out intervalTime);
                int totalCount = batchCount * batchSize;
                fileNames = helpers.GetFiles(htAppConfig["SourceLocation"].ToString(), totalCount, string.Format("*.ack"));
                int sleepTime = intervalTime;
                if (null == fileNames || fileNames.Count == 0) return;

                for (int iIndex = 0; iIndex < batchCount; iIndex++)
                {
                    #region "Inner batch size loop..."
                    for (int jIndex = 0; jIndex < batchSize; jIndex++)
                    {
                        try
                        {
                            NameValueCollection nvcFile = new NameValueCollection();
                            string fileName = string.Empty;
                            FileInfo fileInfo = new FileInfo(Path.Combine(htAppConfig["SourceLocation"].ToString(), fileNames[fileCount]));
                            fileName = fileNames[fileCount].ToString();
                            ObjvalAck.validateAcknowledgememt(fileInfo);
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
