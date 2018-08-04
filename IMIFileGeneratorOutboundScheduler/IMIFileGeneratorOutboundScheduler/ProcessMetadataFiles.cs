using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

                List<string> list = null;
                fileNames = new List<string>();
                list = new List<string>();
                // ProcessFoldersFirst 
                string[] folderArr = Directory.GetDirectories(htAppConfig["OutboundSourceLocation"].ToString());
                ProcessFolders(folderArr, ref fileNames);

                list = helpers.GetFiles(htAppConfig["OutboundSourceLocation"].ToString(), totalCount, string.Format("*.idx"));
               
                int sleepTime = intervalTime;
                if ((null == list || list.Count == 0) && (null == fileNames || fileNames.Count == 0))  return;

                //Moves the files to Inprocess
                helpers.MoveFiles(list, htAppConfig["OutboundSourceLocation"].ToString(), htAppConfig["OutboundInProcessLocation"].ToString());

                helpers.MoveImageFiles(list, htAppConfig["OutboundSourceLocation"].ToString(), htAppConfig["OutboundInProcessLocation"].ToString());
                //for (int i = 0; i < fileNames.Count; i++)
                //{
                //    ProcessObj.ProcessFolders(Path.Combine(htAppConfig["InputRootFolder"].ToString(), fileNames[i]));
                //}

                foreach (var item in fileNames)
                {
                    list.Add(item);
                }
            


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
                            FileInfo fileInfo = new FileInfo(Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), list[fileCount]));
                            fileName = list[fileCount].ToString();
                            // MoveToDealerDirectory(fileInfo);
                            if (File.Exists(fileInfo.FullName))
                            {
                                ProcessObj.ProcessFolders(fileInfo);
                            }                            
                            //Console.ReadLine();
                            fileCount++;
                            if (fileCount == list.Count)
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

        private void ProcessFolders(string[] folderArr, ref List<string> fileNames)
        {
            for (int i = 0; i < folderArr.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(folderArr[i]);
                string imiName = di.Name + ".idx";
                string zipName = di.Name + ".zip";

                if (File.Exists(Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), imiName)))
                {
                    string filePath = Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), imiName);
                    string[] splitedline = File.ReadAllText(filePath).Split('|');
                    string dpkvalue = splitedline[28];
                    string imgPath = splitedline.Last();
                    imgPath = imgPath.Replace("\r\n", "");
                    FileInfo fli = new FileInfo(imgPath);

                    if (File.Exists(Path.Combine(Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), di.Name), fli.Name)))
                    {

                        helpers.MoveFile(Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), imiName),
                            Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), imiName), true);
                        fileNames.Add(imiName);
                        if (Directory.Exists(Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), di.Name)))
                        {
                            Directory.Move(Path.Combine(htAppConfig["OutboundSourceLocation"].ToString(), di.Name),
                                Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), di.Name));
                            if (Directory.Exists(di.FullName))
                            {
                                Directory.Delete(di.FullName, true);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                // ZipFile.CreateFromDirectory(di.FullName, Path.Combine(htAppConfig["OutboundInProcessLocation"].ToString(), zipName));

               
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
