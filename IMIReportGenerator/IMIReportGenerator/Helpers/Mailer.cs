using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator.Helpers
{
    public class Mailer
    {
        public void SendMail(string Subject, string Message, List<string> mailto)
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString();
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString();
                string password = ConfigurationManager.AppSettings["password"].ToString();
                int portNumber = 587;
                bool enableSSL = true;
                string subject = Subject;
                string body = Message;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mailto.ForEach(email => mail.To.Add(email));
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

        public string GetSubscriberConfig()
        {
            string path = ConfigurationManager.AppSettings["SubscirberConfigPath"].ToString();
            return File.ReadAllText(path);
        }
    }
}
