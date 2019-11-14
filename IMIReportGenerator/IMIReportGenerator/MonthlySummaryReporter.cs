using IMIReportGenerator.Helpers;
using IMIReportGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IMIReportGenerator
{
    public class MonthlySummaryReporter
    {

        public MonthlySummaryReporter()
        {

        }
        public void processMonthlyRecords()
        {
            MonthlySummary monthlySumry = new MonthlySummary();
            DBHelper helper = new DBHelper();
            Mailer mailer = new Mailer();
            DataSet ds = new DataSet();
            List<string> requiredDpks = new List<string>();
            int monthsback = 0;
            bool isMail = false;
            try
            {
                requiredDpks = ConfigurationManager.AppSettings["Dpks"].Split(',').ToList();               
                int.TryParse(ConfigurationManager.AppSettings["MonthlyIntervalInMonths"], out monthsback);
                RootObject obj = JsonConvert.DeserializeObject<RootObject>(mailer.GetSubscriberConfig());
                requiredDpks.ForEach(dpk =>
                {
                    Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);
                    string Subject = string.Empty;
                    monthlySumry.GetMonthlyRecords(monthsback, String.Format("'{0}'", dpk), ref ds);
                    Subject = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.AddMonths(-monthsback).Month)} Report";
                    DpkSubscriber dpkconfig = obj.DpkSubscribers.Find(x => x.Dpk == dpk);

                    if (dpkconfig != null && dpkconfig.Subscribers.Count > 0 && isMail)
                    {
                        string Msg = ds.Tables.Count > 0 ? RenderDataTableToHtml(ds.Tables[0]) : "No Records found";
                        if (!string.IsNullOrEmpty(Msg)) { mailer.SendMail(Subject, Msg, dpkconfig.Subscribers); }
                    }
                    else
                    {
                        //Log Config Not found for DPK
                    }
                });

            }
            catch (Exception ex)
            {

            }
        }

        private string RenderDataTableToHtml(DataTable dtInfo)
        {
            StringBuilder tableStr = new StringBuilder();
            int totalCount = 0;
            if (dtInfo.Rows != null && dtInfo.Rows.Count > 0)
            {
                int columnsQty = dtInfo.Columns.Count;
                int rowsQty = dtInfo.Rows.Count;

                tableStr.Append("<TABLE BORDER=\"1\">");
                tableStr.Append("<TR>");
                for (int j = 0; j < columnsQty; j++)
                {
                    tableStr.Append("<TH>" + dtInfo.Columns[j].ColumnName + "</TH>");
                }
                tableStr.Append("</TR>");

                for (int i = 0; i < rowsQty; i++)
                {
                    tableStr.Append("<TR>");
                    for (int k = 0; k < columnsQty; k++)
                    {
                        tableStr.Append("<TD>");
                        tableStr.Append(dtInfo.Rows[i][k].ToString());
                        if (k == 2)
                        {
                            totalCount += (int)dtInfo.Rows[i][k];
                        }
                        tableStr.Append("</TD>");
                    }
                    tableStr.Append("</TR>");
                }

                tableStr.Append("<TR>" + "<TD>TOTAL</TD><TD></TD>" + "<TD>" + totalCount.ToString() + "</TD></TR>");


                tableStr.Append("</TABLE>");
            }

            return tableStr.ToString();

        }
    }
}