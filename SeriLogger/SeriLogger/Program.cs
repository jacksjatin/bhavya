﻿using SeriLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

            #region ServiceNow
            DataSet ds = null;
            int res;
            ServiceNowIncidents sc = new ServiceNowIncidents();

            //Create Incident with Rejection Code
            sc.CreateNewIncident("122SID", "ENC", "F000", "INC122", "", "", DateTime.Now.ToString());

            //Create Incident with Segments
            sc.CreateNewIncident("124SID", "ENC", "", "INC124", "P01", "", DateTime.Now.ToString());

            //Create Incident with Exception
            sc.CreateNewIncident("124SID", "", "", "INC124", "", "ACK NOT RECEIVED", DateTime.Now.ToString());

            //Find Incident By RejectCode
            res = sc.IncidentExistByRejectCode("ENC", "F000", ref ds);

            //Find Incident By Segments
            res = sc.IncidentExistsBySegment("ENC", "P01", ref ds);

            //Find Incident By Exception
            res = sc.IncidentExistsByException("ACK NOT RECEIVED", ref ds);

            //Delete Incident
            res = sc.DeleteIncident("INC122");

            #endregion

            #region OldCode
            CreateIncident id = new CreateIncident();
            IncidentRequest ic = new IncidentRequest(); // incident model object request
            ic.u_affectedci = "";
            ic.u_caller_email = "";
            ic.u_customer_reference_no_ = "";
            id.creatInc(ic);


            Console.WriteLine(((object)10).Equals(10));
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
            #endregion
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
            ack.CheckAndUpdateAckStatus("ImiFIleName", "TRUE", DateTime.Now.ToString(), ref ds);

            //This is General Log we can you use any where
            IMITable log = new IMITable();
            log.imiLogEntry("UserCode", "AppName", "FileName", "Logged Event", "", LogType.Warning, "", "", "");


            // This is in IMI Generation Method
            IMIRecon recLog = new IMIRecon();
            recLog.imiReconLog("2018-07-12 08:27:02.410", "DPK", "FLD", "ImiFIleName", "TRUE", "FLASE", "2018-07-12 08:27:02.410");

        }

    }



}

