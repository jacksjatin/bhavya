using SeriLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{
    class Program
    {
        public readonly SeriLoggerClass loggers;
        //CustomConnection con = new CustomConnection();
        public Program()
        {
            string WriteTo = ConfigurationManager.AppSettings["WriteTo"];

            SeriLoggerClass.Create("db", "imiLog", "IMI");
            loggers = SeriLoggerClass.Instance;
        }
        public static void Main(string[] args)
        {
            Program pr = new Program();
            pr.WriteToDB();
            Console.WriteLine("Logs added");
            Console.Read();
            //loggers.Information("");
        }
        public void WriteToDB()
        {
            //loggers.Information(typeof(Program).Name, "", "Logged");
            //loggers.Information(typeof(Program).Name, "test", "Logged");
            //loggers.Information(typeof(Program).Name, "", "Logged");

            for (int i = 0; i < 10; i++)
            {
                loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "0010"+i.ToString(), "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
            }


            loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "30010", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
        }
    }
}
