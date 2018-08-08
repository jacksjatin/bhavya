using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMIFileGeneratorOutboundScheduler
{
    public class SeriLoggerClass
    {
        private static SeriLoggerClass mInstance;
        private static ILogger _Logger { get; set; }
        private static readonly object padlock = new object();
        private static string LoggerType { get; set; }
        private string UserCode { get; set; }
        private string ApplicationName { get; set; }
        private SeriLoggerClass()
        {

        }
        private SeriLoggerClass(string type, string arg2, string appName)
        {
            LoggerType = type;
            UserCode = arg2;
            ApplicationName = appName;
        }
        public static SeriLoggerClass Instance
        {
            get
            {
                if (mInstance == null)
                    throw new Exception("Object not created");
                IntializeLogger();
                return mInstance;
            }
        }
        public static void Create(string type, string userCode, string appName)
        {
            if (mInstance == null)
            {
                mInstance = new SeriLoggerClass(type, userCode, appName);
            }
        }
        static void IntializeLogger()
        {
            // var connStr = CustomConnection.GetDBConnectionString();
            var connStr = ConfigurationManager.ConnectionStrings["DBPath"].ConnectionString;
            if (_Logger == null)
            {
                if (LoggerType.ToUpper() == "DB")
                {
                    //_Logger = new LoggerConfiguration()
                    //    .WriteTo.MSSqlServer(connStr, "imiLog", columnOptions: GetsqlColumnOptions())
                    //    .CreateLogger();

                    _Logger = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connStr, "ImiReconcilation", columnOptions: GetImiReconcilationClmOptions())
                        .CreateLogger();
                }
                else
                {
                    string filePath = ConfigurationManager.AppSettings["LogFilePath"];
                    _Logger = new LoggerConfiguration().
                        WriteTo.File(filePath + "imiLog.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();
                }
            }
        }
        private static ColumnOptions GetsqlColumnOptions()
        {
            var colOptions = new ColumnOptions();
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            colOptions.AdditionalDataColumns = new Collection<DataColumn>
            {
                new DataColumn {DataType=typeof(string),ColumnName="UserCode" },
                new DataColumn{DataType=typeof(string),ColumnName="AppName" },
                 new DataColumn{DataType=typeof(string),ColumnName="FileName" },
                  new DataColumn{DataType=typeof(string),ColumnName="FunctionName" },
            };
            colOptions.Properties.ExcludeAdditionalProperties = true;
            return colOptions;
        }

        private static ColumnOptions GetImiReconcilationClmOptions()
        {
            var colOptions = new ColumnOptions();
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            colOptions.Store.Remove(StandardColumn.Message);
            colOptions.Store.Remove(StandardColumn.Properties);
            colOptions.Store.Remove(StandardColumn.Exception);
            colOptions.Store.Remove(StandardColumn.Level);
            colOptions.Store.Remove(StandardColumn.TimeStamp);


            colOptions.AdditionalDataColumns = new Collection<DataColumn>
            {
                new DataColumn {DataType=typeof(string),ColumnName="CreatedTimestamp" },
                new DataColumn{DataType=typeof(string),ColumnName="DPK" },
                 new DataColumn{DataType=typeof(string),ColumnName="FLD" },
                   new DataColumn{DataType=typeof(string),ColumnName="ImiFileName" },
                  new DataColumn{DataType=typeof(string),ColumnName="ImiGenerated" },
                   new DataColumn{DataType=typeof(string),ColumnName="AckReceived" },
                    new DataColumn{DataType=typeof(string),ColumnName="UpdatedTimestamp" },
            };
            colOptions.Properties.ExcludeAdditionalProperties = true;
            return colOptions;
        }
        public void Debug(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
                .ForContext("AppName", ApplicationName)
                .ForContext("FileName", fileName)
                .ForContext("FunctionName", functionName).Debug(message);
        }
        public void Information(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
               .ForContext("AppName", ApplicationName)
               .ForContext("FileName", fileName)
               .ForContext("FunctionName", functionName).Information(message);
        }

        public void imiInfo(string CreatedTimestamp, string DPK, string FLD, string ImiFileName,
            string ImiGenerated, string AckReceived, string UpdatedTimestamp)
        {
            _Logger.ForContext("CreatedTimestamp", CreatedTimestamp)
                .ForContext("DPK", DPK)
              .ForContext("FLD", FLD)
              .ForContext("ImiFileName", ImiFileName)
              .ForContext("ImiGenerated", ImiGenerated)
              .ForContext("AckReceived", AckReceived)
              .ForContext("UpdatedTimestamp", UpdatedTimestamp).Information("");

        }
        public void Warning(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
               .ForContext("AppName", ApplicationName)
               .ForContext("FileName", fileName)
               .ForContext("FunctionName", functionName).Warning(message);
        }
        public void Fatal(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
               .ForContext("AppName", ApplicationName)
               .ForContext("FileName", fileName)
               .ForContext("FunctionName", functionName).Fatal(message);
        }
        public void Verbose(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
               .ForContext("AppName", ApplicationName)
               .ForContext("FileName", fileName)
               .ForContext("FunctionName", functionName).Verbose(message);
        }
        public void Error(string fileName, string functionName, string message)
        {
            _Logger.ForContext("UserCode", UserCode)
               .ForContext("AppName", ApplicationName)
               .ForContext("FileName", fileName)
               .ForContext("FunctionName", functionName).Error(message);
        }
    }
}
