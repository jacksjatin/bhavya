using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{
    class IMIAckTable
    {       

        string updateQuery = "UPDATE ImiReconcilation SET AckReceived=@AckReceived,UpdatedTimestamp=@UpdatedTimestamp where ImiFileName=@ImiFileName";

        public int CheckAndUpdateAckStatus(string _ImiFileName, string _AckReceived,string _UpdatedTimeStamp, ref DataSet ds)
        {
            string imiFileQuery = "SELECT * from ImiReconcilation where ImiFileName=" + "'" + _ImiFileName + "'";
            int res = 0;
            ds = DBHelpers.ExecuteDS(imiFileQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                CommandType type = CommandType.Text;
                SqlParameter[] parameterList = { new SqlParameter("@AckReceived",_AckReceived),
                                                 new SqlParameter("@ImiFileName",_ImiFileName),
                                                 new SqlParameter("@UpdatedTimestamp",_UpdatedTimeStamp)
                };
                res = DBHelpers.ExecuteNonQuery(updateQuery, type, parameterList);
            }
            return res;
        }

    }
}
