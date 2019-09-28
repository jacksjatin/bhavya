using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator.Helpers
{
    public class Mailer
    {
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
