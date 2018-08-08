using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{

    public enum LogType
    {
        Debug,
        Information,
        Warning,
        Error,
        Verbose,
        Fatal
    }
    public class IMITable
    {
        string insertQuery = "INSERT INTO imiLog (UserCode,AppName,FileName,FunctionName,Message,Level,TimeStamp,Exception,Properties) " +
              "VALUES (@UserCode,@AppName,@FileName, @FunctionName,@Message,@Level,@TimeStamp,@Exception,@Properties)";
        
        private int ImiLogger(imiModel imi, string insertQuery)
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

        public int imiLogEntry(string UserCode,string Appname,string FileName,string FunctionName,string Message, LogType Level,string TimeStamp,string Exception,string Properties)
        {
            imiModel imi = new imiModel();
            imi.UserCode = UserCode;
            imi.AppName = Appname;
            imi.FileName = FileName;
            imi.FunctionName = FunctionName;
            imi.Message = Message;
            imi.Level = Level.ToString();
            imi.TimeStamp = TimeStamp;
            imi.Exception = Exception;
            imi.Properties = Properties;
          int res =  ImiLogger(imi, insertQuery);

            return res;
        }

    }
    public class imiModel
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
}
