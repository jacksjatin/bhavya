using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SMPTUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Hello World");
            logger.Debug("debug me");

            logger.Trace("This is a trace message");

            logger.Debug("This is a debug message");

            logger.Info("This is an informational message");

            logger.Warn("This is a warning message");

            logger.Error("This is an error message");

            logger.Fatal("This is a fatal message");

            Console.ReadKey();

            //string smtpAddress = "smtp.gmail.com";
            //int portNumber = 587;
            //bool enableSSL = true;
            //string emailFrom = "jatin.023@gmail.com";
            //string password = "Mylord@14";
            //string emailTo = "bhavya.mallela@gmail.com";
            //string subject = "Hello!";
            //string body = "This is Test Email from Jatin";
            //MailMessage mail = new MailMessage();
            //mail.From = new MailAddress(emailFrom);
            //mail.To.Add(emailTo);
            //mail.Subject = subject;
            //mail.Body = body;
            //mail.IsBodyHtml = true;
            //using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
            //{
            //    try
            //    {
            //        smtp.Credentials = new NetworkCredential(emailFrom, password);
            //        smtp.EnableSsl = enableSSL;
            //        smtp.Send(mail);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
        }
    }
}
