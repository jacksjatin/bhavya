﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMIAcknowledgementValidator.HelperClasses
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

        public void MoveFile(string Source, string Destination, bool OverWrite)
        {
            try
            {
                File.Copy(Source, Destination, OverWrite);

                File.Delete(Source);

            }
            catch (Exception ex)
            {
                throw new Exception("Error in MoveFile method. " + ex.Message);
            }
        }

        public void MoveFiles(List<string> fileNames, string sourceLoc, string DestLoc)
        {
            //This method may still requires some modifications.
            //Need to revisit later
            try
            {
                string errorFileMessages = string.Empty;
                foreach (string file in fileNames)
                {
                    try
                    {
                        File.Move(Path.Combine(sourceLoc, file), Path.Combine(DestLoc, file));
                    }
                    catch (Exception fileEx)
                    {
                        errorFileMessages = errorFileMessages + fileEx + " " + file;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(FormatErrorMessage(ex.Message));
            }
        }

        public bool CopyFile(string strFileName, string strDestPath, bool bOverwrite)
        {
            //Copies the file from one folder to another folder.
            try
            {
                if (bOverwrite)
                {
                    File.Copy(strFileName, strDestPath, bOverwrite);
                }
                else
                {
                    DeleteFile(strDestPath);
                    File.Copy(strFileName, strDestPath);
                }

                return true;

            }
            catch (IOException ioex)
            {
                throw new Exception("Error in CopyFile method of Helpers class ." + ioex.Message);
            }
            catch (ArgumentException arex)
            {
                throw new Exception("Error in CopyFile method of Helpers class ." + arex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CopyFile method of Helpers class ." + ex.Message);
            }
        }

        public static bool DeleteFile(string strFileName)
        {
            //Deletes the file from the specified path
            try
            {
                File.Delete(strFileName);
                return true;
            }
            catch (IOException ioex)
            {
                throw new Exception("Error in DeleteFile method of Helpers class ." + ioex.Message);
            }
            catch (ArgumentException arex)
            {
                throw new Exception("Error in DeleteFile method of Helpers class ." + arex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteFile method of Helpers class ." + ex.Message);
            }
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
                throw new Exception("Error in SortFiles method of Helpers class ." + iopex.Message);
            }
            catch (DriveNotFoundException diex)
            {
                throw new Exception("Error in SortFiles method of Helpers class ." + diex.Message);
            }
            catch (ArgumentNullException arnullex)
            {
                throw new Exception("Error in SortFiles method of Helpers class ." + arnullex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SortFiles method of Helpers class ." + ex.Message);
            }
        }

        public void SendMail(string Subject, string Message)
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString();
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString();
                string password = ConfigurationManager.AppSettings["password"].ToString();
                string emailTo = ConfigurationManager.AppSettings["emailTo"].ToString();
                int portNumber = 587;
                bool enableSSL = true;
                string subject = Subject;
                string body = Message;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    try
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
