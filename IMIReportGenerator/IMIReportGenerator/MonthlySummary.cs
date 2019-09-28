using IMIReportGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator
{
    class MonthlySummary
    {

        public int GetMonthlyRecords(int month, string dpk, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            try
            {
                selectQuery = "SELECT SR.DPK,CONVERT(CHAR(10), SR.CTS, 126) AS CREATEDDATE, SUM(SR.TOTAL) AS TOTAL " +
                    "FROM (SELECT DPK, CONVERT(CHAR(10), CREATEDTIMESTAMP, 126) AS CTS, COUNT(*) AS TOTAL FROM [JATINDB].[DBO].[IMIREPORT]" +
                    "WHERE CREATEDTIMESTAMP > = CONVERT(CHAR(10), DATEADD(MM, DATEDIFF(MM, 0, GETDATE()) -" + month + ", 0), 126) AND CREATEDTIMESTAMP < CONVERT(CHAR(10), DATEADD(MM, DATEDIFF(MM, 0, GETDATE()), 0), 126)" +
                    "GROUP BY DPK, CREATEDTIMESTAMP) SR WHERE SR.DPK in (" + dpk + ") GROUP BY SR.DPK,SR.CTS";

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
