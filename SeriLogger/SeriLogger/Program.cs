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
                loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "0010" + i.ToString(), "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
            }


            loggers.imiInfo("2018-07-12 08:27:02.416", "LAW121", "30010", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");

            string _ImiFileName = "wsdfghjhg55d";
            string imiFileQuery = "SELECT * from ImiReconcilation where ImiFileName=" + "'" + _ImiFileName + "'";
            string _AckReceived = "TRUE";
            string updateQuery = "UPDATE ImiReconcilation SET AckReceived=@AckReceived where ImiFileName=@ImiFileName";
            DataSet ds = null;
            CheckAndUpdateAckStatus(_ImiFileName, imiFileQuery, _AckReceived, updateQuery, ref ds);

        }

        private void CheckAndUpdateAckStatus(string _ImiFileName, string imiFileQuery, string _AckReceived,string  updateQuery,ref DataSet ds)
        {
            ds = DBHelpers.ExecuteDS(imiFileQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                CommandType type = CommandType.Text;
                SqlParameter[] parameterList = { new SqlParameter("@AckReceived",_AckReceived),
                                                 new SqlParameter("@ImiFileName",_ImiFileName)
                };
                int res = DBHelpers.ExecuteNonQuery(updateQuery, type, parameterList);
            }
        }

         

    }
}
