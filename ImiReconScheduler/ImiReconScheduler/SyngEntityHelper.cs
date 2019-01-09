using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    public class SyngEntityHelper
    {
        public int GetRecordsbyInterval(int minutes, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "select * from Synergertic where Creation_date >=  DATEADD(minute,-" + minutes + ", GETDATE())";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }


        public string GetUniqueID(string batchname)
        {
            try
            {
                string[] nameArr = batchname.Split('-');
                return nameArr[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int CheckUniqueIDExistsInIMIRecon(string unqID, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            selectQuery = "SELECT * from IMIRecon where Right_UniqueID='" + unqID + "'";
            ds = DBHelpers.ExecuteDS(selectQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                res = 1;
            }
            return res;
        }

        public int CreatorUpdSynergerticRecord(string unqID, string batchName, string batchStatus, string creationDate, string lastModified, string batchClass, string synStatus, bool updateSynrec = false)
        {
            string query = string.Empty;
            query = "INSERT INTO IMIRecon (Right_UniqueID,Synergetics_batchName,Synergetics_CreationTime,Synergetics_batchStatus,Synergetics_LastModified,Syner_BatchClass,Syner_Flag) " +
 "VALUES (@Right_UniqueID,@Synergetics_batchName,@Synergetics_CreationTime, @Synergetics_batchStatus,@Synergetics_LastModified,@Syner_BatchClass,@Syner_Flag)";

            if (updateSynrec)
            {
                query = "UPDATE IMIRecon SET Synergetics_batchName=@Synergetics_batchName,Synergetics_batchStatus=@Synergetics_batchStatus," +
                    "Synergetics_CreationTime=@Synergetics_CreationTime,Synergetics_LastModified=@Synergetics_LastModified," +
                    "Syner_BatchClass=@Syner_BatchClass,Syner_Flag=@Syner_Flag where Right_UniqueID=@Right_UniqueID";
            }

            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@Right_UniqueID",unqID),
                                                 new SqlParameter("@Synergetics_batchName",batchName),
                                                  new SqlParameter("@Synergetics_batchStatus",batchStatus),
                                                 new SqlParameter("@Synergetics_CreationTime",creationDate),
                                                 new SqlParameter("@Synergetics_LastModified",lastModified),
                                                 new SqlParameter("@Syner_BatchClass",batchClass),
                                                 new SqlParameter("@Syner_Flag",synStatus),
                };
            int res = DBHelpers.ExecuteNonQuery(query, type, parameterList);
            return res;
        }
    }


}
