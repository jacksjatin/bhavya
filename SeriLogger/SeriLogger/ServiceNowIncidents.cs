using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{
    class ServiceNowIncidents
    {
        public int IncidentExistByRejectCode(string DPK, string RejectionCode, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * from imiServiceNow where DPK=" + "'" + DPK + "' and RejectionCode=" + "'" + RejectionCode + "'";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }

        public int IncidentExistsBySegment(string DPK, string Segement, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * from imiServiceNow where DPK=" + "'" + DPK + "' and Segments=" + "'" + Segement + "'";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }

        public int IncidentExistsByException(string Exception, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * from imiServiceNow where Exception=" + "'" + Exception + "' and (convert(date,Timestamp) = convert(date,GETDATE()))";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }

        public int IncidentExistsByExceptionAndDPK(string Exception,string DPK, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * from imiServiceNow where Exception=" + "'" + Exception + "'and DPK=" + "'" + DPK + "' and (convert(date,Timestamp) = convert(date,GETDATE()))";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }            

        public int GetTodayIncidentCount(ref int count )
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT count(*) as count FROM imiServiceNow WHERE (DAY(Timestamp) = DAY(GETDATE())) AND (MONTH(Timestamp) = MONTH(GETDATE())) AND (YEAR(Timestamp) = YEAR(GETDATE()))";
       DataSet ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                count = Convert.ToInt32(ds.Tables[0].Rows[0]["count"]);
                res = 1;
            }
            return res;
        }

        public int CreateNewIncident(string SysID, string DPK, string RejectionCode, string IncidentNumber, string Segments, string Exception, string TimeStamp)
        {
            string insertQuery = "INSERT INTO imiServiceNow (SysID,DPK,RejectionCode,IncidentNumber,Segments,Exception,TimeStamp) " +
     "VALUES (@SysID,@DPK,@RejectionCode, @IncidentNumber,@Segments,@Exception,@TimeStamp)";
            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@SysID",SysID),
                                                 new SqlParameter("@DPK",DPK),
                                                 new SqlParameter("@RejectionCode",RejectionCode),
                                                 new SqlParameter("@IncidentNumber",IncidentNumber),
                                                 new SqlParameter("@Segments",Segments),
                                                 new SqlParameter("@Exception",Exception),
                                                 new SqlParameter("@TimeStamp",TimeStamp),
                };
            int res = DBHelpers.ExecuteNonQuery(insertQuery, type, parameterList);
            return res;
        }

    }

    public class ImiIncidentModel
    {
        public string SysID { get; set; }
        public string DPK { get; set; }
        public string RejectionCode { get; set; }
        public string IncidentNumber { get; set; }
        public string Exception { get; set; }
        public string TimeStamp { get; set; }
    }
}
