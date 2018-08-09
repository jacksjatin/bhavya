using SeriLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{
    class Program
    {
        public static SeriLoggerClass loggers;
        //CustomConnection con = new CustomConnection();
        //public Program()
        //{

        //}
        public static void Main(string[] args)
        {
            //string WriteTo = ConfigurationManager.AppSettings["WriteTo"];
            //SeriLoggerClass.Create("db", "imiLog", "IMI");
            //loggers = SeriLoggerClass.Instance;
            Program pr = new Program();
            pr.WriteToDB();
            //Nested n = new Nested();
            //n.log();
            Console.WriteLine("Logs added");
            Console.Read();
            //loggers.Information("");
        }

        
        public void WriteToDB()
        {
            #region
            //loggers.Information(typeof(Program).Name, "", "Logged");
            //loggers.Information(typeof(Program).Name, "test", "Logged");
            //loggers.Information(typeof(Program).Name, "", "Logged");

            //for (int i = 0; i < 10; i++)
            //{
            //    loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "0010" + i.ToString(), "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
            //}


            //loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "30010", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");

            //string _ImiFileName = "wsdfghjhg55d";

            //string _AckReceived = "TRUE";


            //CheckAndUpdateAckStatus(_ImiFileName, imiFileQuery, _AckReceived, updateQuery, ref ds);
            #endregion

            // Ack Recieved Update
            IMIAckTable ack = new IMIAckTable();
            DataSet ds = null;
            ack.CheckAndUpdateAckStatus("ImiFIleName", "TRUE", DateTime.Now.ToString(),ref ds);

            //This is General Log we can you use any where
            IMITable log = new IMITable();
            log.imiLogEntry("UserCode", "AppName", "FileName", "Logged Event", "", LogType.Warning , "","","");


            // This is in IMI Generation Method
            IMIRecon recLog = new IMIRecon();
            recLog.imiReconLog("2018-07-12 08:27:02.410", "DPK", "FLD", "ImiFIleName", "TRUE", "FLASE", "2018-07-12 08:27:02.410");
              
        }

       
        
    }
   

   
}

