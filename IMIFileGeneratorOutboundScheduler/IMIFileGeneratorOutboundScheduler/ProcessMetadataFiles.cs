using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
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
        public ILogger loggers = null;



        private ProcessDirectories ProcessObj;
        public ProcessMetadataFiles()
        {
            helpers = new HelperClasses.Helpers();

        }


        public void GetMDFiles()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBPath"].ConnectionString;
            string WriteTo = ConfigurationManager.AppSettings["WriteTo"];
            loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connStr, "ImiReconcilation", columnOptions: GetImiReconcilationClmOptions())
                        .CreateLogger();
            imiInfo("2018-07-12 08:27:02.416", "LAW121", "3399310", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
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


        public void imiInfo(string CreatedTimestamp, string DPK, string FLD, string ImiFileName,
           string ImiGenerated, string AckReceived, string UpdatedTimestamp)
        {
            loggers.ForContext("CreatedTimestamp", CreatedTimestamp)
                .ForContext("DPK", DPK)
              .ForContext("FLD", FLD)
              .ForContext("ImiFileName", ImiFileName)
              .ForContext("ImiGenerated", ImiGenerated)
              .ForContext("AckReceived", AckReceived)
              .ForContext("UpdatedTimestamp", UpdatedTimestamp).Information("");

        }
        private ColumnOptions GetImiReconcilationClmOptions()
        {
            var colOptions = new ColumnOptions();
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            colOptions.Store.Remove(StandardColumn.Message);
            colOptions.Store.Remove(StandardColumn.Properties);
            colOptions.Store.Remove(StandardColumn.Exception);
            colOptions.Store.Remove(StandardColumn.Level);
            colOptions.Store.Remove(StandardColumn.TimeStamp);


            colOptions.AdditionalDataColumns = new Collection<DataColumn>
            {
                new DataColumn {DataType=typeof(string),ColumnName="CreatedTimestamp" },
                new DataColumn{DataType=typeof(string),ColumnName="DPK" },
                 new DataColumn{DataType=typeof(string),ColumnName="FLD" },
                   new DataColumn{DataType=typeof(string),ColumnName="ImiFileName" },
                  new DataColumn{DataType=typeof(string),ColumnName="ImiGenerated" },
                   new DataColumn{DataType=typeof(string),ColumnName="AckReceived" },
                    new DataColumn{DataType=typeof(string),ColumnName="UpdatedTimestamp" },
            };
            colOptions.Properties.ExcludeAdditionalProperties = true;
            return colOptions;
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
