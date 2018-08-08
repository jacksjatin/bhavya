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

namespace SeriLogger
{
    class Nested
    {
        public ILogger loggers = null;
        public void log()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBPath"].ConnectionString;
            string WriteTo = ConfigurationManager.AppSettings["WriteTo"];
            loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connStr, "ImiReconcilation", columnOptions: GetImiReconcilationClmOptions())
                        .CreateLogger();
            imiInfo("2018-07-12 08:27:02.416", "LAW121", "3399310", "wsdfghjhg55d", "TRUE", "", "2018-07-12 08:27:02.410");
        }
        public void imiInfo(string CreatedTimestamp, string DPK, string FLD, string ImiFileName,
          string ImiGenerated, string AckReceived, string UpdatedTimestamp)
        {
            loggers.ForContext("CreatedTimestamp", CreatedTimestamp)
                .ForContext("DPK", DPK)
              .ForContext("FLD", FLD)
              .ForContext("ImiFileName", ImiFileName)
              .ForContext("ImiGenerated", ImiGenerated)
              .ForContext("AckReceived", AckReceived)
              .ForContext("UpdatedTimestamp", UpdatedTimestamp).Information("");

        }
        private ColumnOptions GetImiReconcilationClmOptions()
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
    }
}
