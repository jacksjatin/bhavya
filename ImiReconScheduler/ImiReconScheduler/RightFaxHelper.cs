using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    class RightFaxHelper
    {

        public int GetRecordsbyLastIndex(int rowindex, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                selectQuery = "SELECT * FROM(SELECT ROW_NUMBER() OVER(ORDER BY(select 0)) as RowNo, * FROM Rightfax) as t WHERE RowNo>" + rowindex;
                sb.AppendLine("Query GetRecordsbyLastIndex: " + selectQuery);
                ds = DBHelpers.ExecuteDS(selectQuery);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sb.AppendLine("Table Row Count: " + ds.Tables[0].Rows.Count);
                    res = 1;
                }
                else
                {
                    sb.AppendLine("Table Row Count: 0 or  null");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            loggertofile(sb.ToString());
            return res;
        }

        public int getRowNumberbyUniqueID(string uniqueId, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * FROM(SELECT ROW_NUMBER() OVER(ORDER BY(select 0)) as RowNo, UniqueID FROM Rightfax) as t WHERE UniqueID=" + uniqueId;
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {

                res = 1;
            }
            return res;
        }

        public int getCurrentDayRecords(ref DataSet ds)
        {
            int res = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                string selectQuery = string.Empty;                             
                selectQuery = "SELECT * FROM(SELECT ROW_NUMBER() OVER(ORDER BY(select 0)) as RowNo, * FROM Rightfax) as t WHERE CreationTime >= CONVERT(DateTime, DATEDIFF(DAY, 0, GETDATE()))";
                sb.AppendLine("Query getCurrentDayRecords: " + selectQuery);
                ds = DBHelpers.ExecuteDS(selectQuery);                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sb.AppendLine("Table Row Count: " + ds.Tables[0].Rows.Count);
                    res = 1;
                }
                else
                {
                    sb.AppendLine("Table Row Count: 0 or  null" );
                }
            }           
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            loggertofile(sb.ToString());
            return res;

        }

        public int CreatRightFacRecord(string unqID, string CreationTime, string CompletionTime, int TermStat)
        {
            string query = string.Empty;
            query = "INSERT INTO IMIRecon (Right_UniqueID,Right_CreationTime,Right_CompletionTime,Right_TermStat) " +
 "VALUES (@Right_UniqueID,@Right_CreationTime,@Right_CompletionTime, @Right_TermStat)";

            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@Right_UniqueID",unqID),
                                                 new SqlParameter("@Right_CreationTime",CreationTime),
                                                  new SqlParameter("@Right_CompletionTime",CompletionTime),
                                                 new SqlParameter("@Right_TermStat",TermStat)

                };
            int res = DBHelpers.ExecuteNonQuery(query, type, parameterList);
            return res;
        }


        public void loggertofile(string text)
        {
            File.AppendAllText(ConfigurationManager.AppSettings["logger"], text);
        }
    }
}
