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

        private enum LogType 
        {
            Debug,
            Information,
            Warning,
            Error,
            Verbose,
            Fatal
        }
        public void WriteToDB()
        {
            //loggers.Information(typeof(Program).Name, "", "Logged");
            //loggers.Information(typeof(Program).Name, "test", "Logged");
            //loggers.Information(typeof(Program).Name, "", "Logged");

            //for (int i = 0; i < 10; i++)
            //{
            //    loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "0010" + i.ToString(), "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
            //}


            //loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "30010", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");

            //string _ImiFileName = "wsdfghjhg55d";
            //string imiFileQuery = "SELECT * from ImiReconcilation where ImiFileName=" + "'" + _ImiFileName + "'";
            //string _AckReceived = "TRUE";

            //string updateQuery = "UPDATE ImiReconcilation SET AckReceived=@AckReceived where ImiFileName=@ImiFileName";
            //DataSet ds = null;
            //CheckAndUpdateAckStatus(_ImiFileName, imiFileQuery, _AckReceived, updateQuery, ref ds);

            //string insertQuery = "INSERT INTO imiLog (UserCode,AppName,FileName,FunctionName,Message,Level,TimeStamp,Exception,Properties) " +
            //    "VALUES (@UserCode,@AppName,@FileName, @FunctionName,@Message,@Level,@TimeStamp,@Exception,@Properties)";

            //imiTable imi = new imiTable();
            //imi.UserCode = "imiLog";
            //imi.AppName = "IMI";
            //imi.FileName = "db";
            //imi.FunctionName = "1234getdata989989";
            //imi.Message = "3456789iuhgvh";
            //imi.Level = LogType.Warning.ToString();
            //imi.TimeStamp = "2018-08-04 18:13:00.517";
            //imi.Exception = "";
            //imi.Properties = "";
            //ImiLogger(imi, insertQuery);

            string insertReconQuery = "INSERT INTO ImiReconcilation (CreatedTimestamp,DPK,FLD,ImiFileName,ImiGenerated,AckReceived,UpdatedTimestamp) " +
                "VALUES (@CreatedTimestamp,@DPK,@FLD, @ImiFileName,@ImiGenerated,@AckReceived,@UpdatedTimestamp)";

            ImiRecon imiRec = new ImiRecon();
            imiRec.CreatedTimestamp = "2018-07-12 08:27:02.417";
            imiRec.DPK = "LAW122";
            imiRec.FLD = "100104";
            imiRec.ImiFileName = "123wsdfghjhg55d";
            imiRec.ImiGenerated = "TRUE";
            imiRec.AckReceived = "";
            imiRec.UpdatedTimestamp = "2018-07-12 08:27:02.410";

            ImiReconLogger(imiRec, insertReconQuery);


        }

        private int CheckAndUpdateAckStatus(string _ImiFileName, string imiFileQuery, string _AckReceived, string updateQuery, ref DataSet ds)
        {
            int res = 0;
            ds = DBHelpers.ExecuteDS(imiFileQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                CommandType type = CommandType.Text;
                SqlParameter[] parameterList = { new SqlParameter("@AckReceived",_AckReceived),
                                                 new SqlParameter("@ImiFileName",_ImiFileName)
                };
                res = DBHelpers.ExecuteNonQuery(updateQuery, type, parameterList);
            }
            return res;
        }

        private int ImiLogger(imiTable imi,string insertQuery)
        {
            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@UserCode",imi.UserCode),
                                                 new SqlParameter("@AppName",imi.AppName),
                                                 new SqlParameter("@FileName",imi.FileName),
                                                 new SqlParameter("@FunctionName",imi.FunctionName),
                                                 new SqlParameter("@Message",imi.Message),
                                                 new SqlParameter("@Level",imi.Level),
                                                 new SqlParameter("@TimeStamp",imi.TimeStamp),
                                                  new SqlParameter("@Exception",imi.Exception),
                                                   new SqlParameter("@Properties",imi.Properties),
                };
            int res = DBHelpers.ExecuteNonQuery(insertQuery, type, parameterList);
            return res;
        }

        private int ImiReconLogger(ImiRecon imiRec , string insertReconQuery)
        {
            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@CreatedTimestamp",imiRec.CreatedTimestamp),
                                                 new SqlParameter("@DPK",imiRec.DPK),
                                                 new SqlParameter("@FLD",imiRec.FLD),
                                                 new SqlParameter("@ImiFileName",imiRec.ImiFileName),
                                                 new SqlParameter("@ImiGenerated",imiRec.ImiGenerated),
                                                 new SqlParameter("@AckReceived",imiRec.AckReceived),
                                                 new SqlParameter("@UpdatedTimestamp",imiRec.UpdatedTimestamp)                                               
                };
            int res = DBHelpers.ExecuteNonQuery(insertReconQuery, type, parameterList);
            return res;
        }
    }

    public class imiTable
    {
        public string UserCode { get; set; }
        public string AppName { get; set; }
        public string FileName { get; set; }
        public string FunctionName { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
    }

    public class ImiRecon
    {
        public string CreatedTimestamp { get; set; }
        public string DPK { get; set; }
        public string FLD { get; set; }
        public string ImiFileName { get; set; }
        public string ImiGenerated { get; set; }
        public string AckReceived { get; set; }
        public string UpdatedTimestamp { get; set; }
    }
}

