using IMIReportGenerator.Helpers;
using System;
using System.Collections.Generic;
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
            List<string> requiredDpks = new List<string>();
            int daysback = 0;
            bool isMail = false;
            try
            {
                requiredDpks = ConfigurationManager.AppSettings["Dpks"].Split(',').ToList();
                string dpks = helper.FormateDPKString(requiredDpks);
                int.TryParse(ConfigurationManager.AppSettings["DailyIntervalInDays"], out daysback);
                string date = DateTime.Today.AddDays(-daysback).ToString("yyyy-MM-dd");
                dailySumry.GetDailyRecords(date, dpks, ref ds);

                Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);
                StringBuilder sb = new StringBuilder();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    requiredDpks.ForEach(dk =>
                    {
                        DataRow dr = ds.Tables[0].AsEnumerable().Where(x => x["DPK"].ToString() == dk).SingleOrDefault();
                        string dpk = dk;
                        string count = "0";
                        if (dr != null)
                        {
                            count = dr["TOTAL"].ToString();
                        }
                        sb.Append(dpk + " Daily batch count : " + count + " ");
                    });

                    if (isMail)
                    {
                        string Subject = sb.ToString();
                        string Msg = sb.ToString();
                        mailer.SendMail(Subject, Msg);
                    }
                }
                else
                {
                    // No data present
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}