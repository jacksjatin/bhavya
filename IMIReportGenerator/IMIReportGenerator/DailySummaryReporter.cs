using IMIReportGenerator.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace IMIReportGenerator
{
    public class DailySummaryReporter
    {
        public void processDailyRecords()
        {
            DailySummary dailySumry = new DailySummary();
            DBHelper helper = new DBHelper();
            Mailer mailer = new Mailer();
            DataSet ds = new DataSet();
            string dpks = string.Empty;
            int daysback = 0;
            bool isMail = false;
            try
            {
                dpks = helper.FormateDPKString(ConfigurationManager.AppSettings["Dpks"]);
                int.TryParse(ConfigurationManager.AppSettings["DailyIntervalInDays"], out daysback);
                string date = DateTime.Today.AddDays(-daysback).ToString("yyyy-MM-dd");
                dailySumry.GetDailyRecords(date, dpks, ref ds);

                Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);
                StringBuilder sb = new StringBuilder();

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    string dpk = item["DPK"].ToString();
                    string count = item["TOTAL"].ToString();
                    sb.Append(dpk + " Daily batch count : " + count + " ");
                }

                if (isMail)
                {
                    string Subject = sb.ToString();
                    string Msg = sb.ToString();
                    mailer.SendMail(Subject, Msg);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}