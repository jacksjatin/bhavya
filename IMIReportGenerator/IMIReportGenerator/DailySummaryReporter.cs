﻿using IMIReportGenerator.Helpers;
using IMIReportGenerator.Models;
using Newtonsoft.Json;
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
                int.TryParse(ConfigurationManager.AppSettings["DailyIntervalInDays"], out daysback);
                string date = DateTime.Today.AddDays(-daysback).ToString("yyyy-MM-dd");

                RootObject obj = JsonConvert.DeserializeObject<RootObject>(mailer.GetSubscriberConfig());

                Boolean.TryParse(ConfigurationManager.AppSettings["isMail"], out isMail);
                string subject = string.Empty;

                requiredDpks.ForEach(dpk =>
                {
                    dailySumry.GetDailyRecords(date, String.Format("'{0}'", dpk), ref ds);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {                     
                        string count = ds.Tables[0].Rows[0]["TOTAL"].ToString();
                        subject = dpk + " Daily batch count : " + count + " ";
                    }
                    else
                    {
                        subject = "No Records Found for "+ dpk;
                        // No data available
                    }

                    DpkSubscriber dpkconfig = obj.DpkSubscribers.Find(x => x.Dpk == dpk);
                    if (dpkconfig != null && dpkconfig.Subscribers.Count > 0 && isMail)
                    {
                        mailer.SendMail(subject, subject, dpkconfig.Subscribers);
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
    }
}