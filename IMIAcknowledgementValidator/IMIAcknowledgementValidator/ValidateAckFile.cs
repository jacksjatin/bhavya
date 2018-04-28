using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMIAcknowledgementValidator
{
    public class ValidateAckFile
    {
        private HelperClasses.Helpers helpers;
        public void validateAcknowledgememt(FileInfo fi)
        {

            try
            {
                helpers = new HelperClasses.Helpers();
                string ackData = string.Empty;

                using (StreamReader sr = new StreamReader(fi.FullName))
                {
                    ackData = sr.ReadToEnd();
                }
                bool Success = false;
                string SourceLocation = ConfigurationManager.AppSettings["SourceLocation"].ToString();
                string TrackingLocation = ConfigurationManager.AppSettings["TrackingLocation"].ToString();
                string ArchiveLocation = ConfigurationManager.AppSettings["ArchiveLocation"].ToString();

                string ReconcileLocation = ConfigurationManager.AppSettings["ReconcileLocation"].ToString();
                string[] SuccessCode = ConfigurationManager.AppSettings["SuccessCodes"].Split(',');

                Success = ContainsAll(ackData, SuccessCode, StringComparison.OrdinalIgnoreCase);
                //for (int i = 0; i < SuccessCode.Length; i++)
                //{
                //    Success = false;
                //    if (ackData.Contains(SuccessCode[i]))
                //    {
                //        Success = true;
                //    }                                        

                //}
                bool isMail = false;

                Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);

                StringBuilder sb = new StringBuilder();
                string imifilename = fi.Name.Replace(".ack", ".imi");
                if (!Success)
                {
                    if (File.Exists(Path.Combine(TrackingLocation, imifilename)))
                    {
                        helpers.MoveFile(Path.Combine(TrackingLocation, imifilename), Path.Combine(ReconcileLocation, imifilename), true);
                        if (File.Exists(Path.Combine(SourceLocation, fi.Name)))
                            helpers.MoveFile(Path.Combine(SourceLocation, fi.Name), Path.Combine(ReconcileLocation, fi.Name), true);

                        if (isMail)
                        {
                            sb.Append("For more information please check below files" + "\n");
                            sb.Append("IMI Location: " + Path.Combine(ReconcileLocation, imifilename) + "\n");
                            sb.Append("ACK Location: " + Path.Combine(ReconcileLocation, fi.Name) + "\n");
                            string Subject = string.Format("Document Rejected: {0}", fi.Name);
                            string Msg = sb.ToString();
                            SendMail(Subject, Msg);
                        }
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(TrackingLocation, imifilename)))
                    {
                        helpers.MoveFile(Path.Combine(TrackingLocation, imifilename), Path.Combine(ArchiveLocation, imifilename), true);
                        if (File.Exists(Path.Combine(SourceLocation, fi.Name)))
                            helpers.MoveFile(Path.Combine(SourceLocation, fi.Name), Path.Combine(ArchiveLocation, fi.Name), true);
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public bool ContainsAll(string source, IEnumerable<string> values, StringComparison comp = StringComparison.CurrentCulture)
        {
            return values.All(value => ContainsStr(source, value, comp));
        }

        public bool ContainsStr(string source, string value, StringComparison comp)
        {
            return source.IndexOf(value, comp) > -1;
        }
        
        public void SendMail(string Subject,string Message)
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
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
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
