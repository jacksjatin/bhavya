using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIAcknowledgementValidator
{
    public class PurgeAckFIles
    {
        private HelperClasses.Helpers helpers;
        string  SourceLocation = string.Empty;
        public void PurgingAcks()
        {
            try
            {
                helpers = new HelperClasses.Helpers();
                string TrackingLocation = ConfigurationManager.AppSettings["TrackingLocation"].ToString();
                string ReconcileLocation = ConfigurationManager.AppSettings["ReconcileLocation"].ToString();
                SourceLocation = ConfigurationManager.AppSettings["SourceLocation"].ToString();
                bool iSexitFlag = false;
                bool bSleep = false;
                int batchCount;
                int batchSize;
                int intervalTime;
                int fileCount = 0;
                List<string> fileNames = null;
                Int32.TryParse(ConfigurationManager.AppSettings["BatchCount"].ToString(), out batchCount);
                Int32.TryParse(ConfigurationManager.AppSettings["BatchSize"].ToString(), out batchSize);
                Int32.TryParse(ConfigurationManager.AppSettings["SleepInterval"].ToString(), out intervalTime);
                int totalCount = batchCount * batchSize;
                fileNames = helpers.GetFiles(TrackingLocation, totalCount, string.Format("*.imi"));
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
                            FileInfo fileInfo = new FileInfo(Path.Combine(TrackingLocation.ToString(), fileNames[fileCount]));
                            fileName = fileNames[fileCount].ToString();
                            isPurge(TrackingLocation, ReconcileLocation, fileName);
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
            catch(Exception ex)
            {

            }
        }

        public void isPurge(string TrackingLocation, string ReconcileLocation, string fileName)
        {
            try
            {
                string[] nameArr = fileName.Split('.');
                string iString = nameArr[1];

                iString = iString.Substring(0, 4) + "-" + iString.Substring(4, 2) + "-" + iString.Substring(6, 2) + " " +
                    iString.Substring(8, 2) + ":" + iString.Substring(10, 2) + ":" + iString.Substring(12, 2) + "."
                    + iString.Substring(14, 3);

                int HoursExpires = 0;
                int.TryParse(ConfigurationManager.AppSettings["HoursToExpire"], out HoursExpires);

                string testValu = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                //   20180423175120543
                DateTime oDate = DateTime.ParseExact(iString, "yyyy-MM-dd HH:mm:ss.fff", null);
                var hours = (DateTime.Now - oDate).TotalHours;
                if (hours >= HoursExpires)
                {
                    string ackFileName = fileName.Replace(".imi", ".ack");
                    if (File.Exists(Path.Combine(SourceLocation, ackFileName)))
                    {
                        return;
                    }
                    else
                    {
                        helpers.MoveFile(Path.Combine(TrackingLocation, fileName), Path.Combine(ReconcileLocation, fileName), true);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }



    }
}
