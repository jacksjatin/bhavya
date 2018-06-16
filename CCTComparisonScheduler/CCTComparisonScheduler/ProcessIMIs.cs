using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CCTComparisonScheduler
{
    public class ProcessIMIs
    {
        private Hashtable htAppConfig;
        private Helpers helpers;
        private CCTProcessor objcct;
        private List<csv> lstRec;
        public int totalClaims = 0;
        public int totalClaimsUpdated = 0;
        public int totalClaimsMatched = 0;
        public int totalMembersMatched = 0;
        public int totalUnknownContracts = 0;
        public int totalUnknownClaim = 0;
        public ProcessIMIs()
        {
            helpers = new Helpers();
        }

        public void getfiles()
        {
            ArrayList movedFiles = new ArrayList();
            ArrayList filesToProcess = new ArrayList();
            lstRec = new List<csv>();
            RetrieveConfigInformation();
            objcct = new CCTProcessor();
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
                fileNames = helpers.GetFiles(htAppConfig["SourceLocation"].ToString(), totalCount, string.Format("*.txt"));
                int sleepTime = intervalTime;
                if (null == fileNames || fileNames.Count == 0) return;

                //Moves the files to Inprocess
                helpers.MoveFiles(fileNames, htAppConfig["SourceLocation"].ToString(), htAppConfig["InProcessLocation"].ToString());


                for (int iIndex = 0; iIndex < batchCount; iIndex++)
                {
                    #region "Inner batch size loop..."
                    for (int jIndex = 0; jIndex < batchSize; jIndex++)
                    {
                        try
                        {

                            NameValueCollection nvcFile = new NameValueCollection();
                            string fileName = string.Empty;
                            FileInfo fileInfo = new FileInfo(Path.Combine(htAppConfig["InProcessLocation"].ToString(), fileNames[fileCount]));
                            fileName = fileNames[fileCount].ToString();
                            objcct.CompareContract(fileInfo, ref lstRec, ref totalClaims, ref totalClaimsUpdated, ref totalClaimsMatched, ref
             totalMembersMatched, ref totalUnknownContracts, ref totalUnknownClaim);
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

                WriteCSV(lstRec,Path.Combine(htAppConfig["reportcsv"].ToString(),"Output_"+DateTime.Now.ToString("mmddyyyy") +".csv"));
                // loggers
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
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
