using IMIReportGenerator.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;

namespace IMIReportGenerator
{
    public class MonthlySummaryReporter
    {
        public void processMonthlyRecords()
        {
            MonthlySummary monthlySumry = new MonthlySummary();
            DBHelper helper = new DBHelper();
            Mailer mailer = new Mailer();
            DataSet ds = new DataSet();
            string dpks = string.Empty;
            int monthsback = 0;
            bool isMail = false;
            try
            {
                dpks = helper.FormateDPKString(ConfigurationManager.AppSettings["Dpks"]);
                int.TryParse(ConfigurationManager.AppSettings["MonthlyIntervalInMonths"], out monthsback);
                monthlySumry.GetMonthlyRecords(monthsback, dpks, ref ds);

                Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);
                StringBuilder sb = new StringBuilder();

                sb.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.AddMonths(-monthsback).Month)} Report");

                if (isMail)
                {
                    string Subject = sb.ToString();
                    string Msg = RenderDataTableToHtml(ds.Tables[0]);
                    mailer.SendMail(Subject, Msg);
                }

            }
            catch (Exception ex)
            {

            }
        }

        private string RenderDataTableToHtml(DataTable dtInfo)
        {
            StringBuilder tableStr = new StringBuilder();

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
                        tableStr.Append("</TD>");
                    }
                    tableStr.Append("</TR>");
                }

                tableStr.Append("</TABLE>");
            }

            return tableStr.ToString();

        }
    }
}