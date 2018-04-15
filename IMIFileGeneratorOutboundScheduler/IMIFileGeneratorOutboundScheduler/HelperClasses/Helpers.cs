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

namespace IMIFileGeneratorOutboundScheduler.HelperClasses
{
    public class Helpers
    {

        public Hashtable GetappConfigData()
        {
            try
            {
                Hashtable htappConfigData = new System.Collections.Hashtable();
                NameValueCollection nvc = ConfigurationManager.AppSettings;
                foreach (string sKey in nvc.AllKeys)
                {
                    htappConfigData.Add(sKey, nvc[sKey]);
                }
                return htappConfigData;
            }
            catch (ConfigurationException ex)
            {
                throw new Exception("Error in GetappConfigData method. The configuration file is missing: " + ex.Message);
            }
        }
        public string FormatErrorMessage(string pMessage)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            StringBuilder sb = new StringBuilder();
            sb.Append("Error in ").Append(sf.GetMethod().Name).Append(" method of ").Append(this.GetType().Name);
            sb.Append(" class ");
            sb.Append(string.Empty).Append(pMessage);
            return sb.ToString();
        }

        public List<string> GetFiles(string strFilePath, int totalFilesCount, string strPattern)
        {
            try
            {

                List<string> fileNameList = new List<string>();
                int counter = 0;
                System.IO.DirectoryInfo objDirInfo = new DirectoryInfo(strFilePath);
                System.IO.FileInfo[] objGetFileInfo = objDirInfo.GetFiles(strPattern);
                for (int idx = 0; idx < objGetFileInfo.Length; idx++)
                {
                    FileInfo fileInfo = objGetFileInfo[idx];
                    fileNameList.Add(fileInfo.Name);
                    counter++;
                    if (counter == totalFilesCount)
                    {
                        break;
                    }
                }
                return fileNameList;

            }

            catch (InvalidOperationException iopex)
            {
                throw new Exception("Error in SortFiles method of FileSystemUtilty class of GenericUtility." + iopex.Message);
            }
            catch (DriveNotFoundException diex)
            {
                throw new Exception("Error in SortFiles method of FileSystemUtilty class of GenericUtility." + diex.Message);
            }
            catch (ArgumentNullException arnullex)
            {
                throw new Exception("Error in SortFiles method of FileSystemUtilty class of GenericUtility." + arnullex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SortFiles method of FileSystemUtilty class of GenericUtility." + ex.Message);
            }
        }
    }
}
