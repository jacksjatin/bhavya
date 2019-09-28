using IMIReportGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator
{
    class DailySummary
    {
        public int GetDailyRecords(string date, string dpk, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            try
            {
                selectQuery = "SELECT DPK, COUNT(*) AS TOTAL FROM IMIREPORT WHERE " +
                    "CAST(DATEDIFF(DAY, 0, CREATEDTIMESTAMP) AS DATETIME) ='" + date + "' AND DPK in (" + dpk + ")  GROUP BY DPK";

                ds = DBHelper.ExecuteDS(selectQuery);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return res;
        }
    }
}
