using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNowLookUp
{
    class CheckIncidents
    {

        private SqlConnection Conn;
        private void CreateConnection()
        {
            string ConnStr = ConfigurationManager.AppSettings["DBString"];
            Conn = new SqlConnection(ConnStr);
            Conn.Open();
        }


        public void CheckAndDeleteIncidents()
        {

            string SqlString = "SELECT * FROM imiServiceNow";
            try
            {
                CreateConnection();
                DataSet dt = ExecuteDS(SqlString);

                if ((dt.Tables != null && dt.Tables[0].Rows.Count <= 0 ? true : false))
                    return;

                foreach (DataRow dr in dt.Tables[0].Rows)
                {
                    string inc = dr["IncidentNumber"].ToString();
                    string sysId = dr["SysID"].ToString();
                    #region FindIncident
                    LookUpIncident lkup = new LookUpIncident();
                    Exception excep = null;
                    string response = string.Empty;
                    excep = lkup.FindIncident(sysId, ref response);
                    JObject jobj = JObject.Parse(response);
                    string status = jobj["result"]["state"].ToString();
                    if (status != "1")
                        DeleteIncident(inc);
                    #endregion
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Conn != null)
                {
                    Conn.Close();
                }
            }
        }

        public int DeleteIncident(string IncidentNumber)
        {

            string deleteQuery = "DELETE FROM imiServiceNow where IncidentNumber=" + "'" + IncidentNumber + "' ";
            string imiCheckIncQuery = "SELECT * from imiServiceNow where IncidentNumber=" + "'" + IncidentNumber + "' ";
            int res = 0;
            DataSet ds = ExecuteDS(imiCheckIncQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                CommandType type = CommandType.Text;
                res = ExecuteNonQuery(deleteQuery, type);
            }
            return res;
        }

        public DataSet ExecuteDS(string strSQL)
        {
            DataSet ds = null;
            Exception rethrow = null;
            try
            {
                SqlCommand command = new SqlCommand(strSQL, Conn);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                ds = new DataSet();
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                string strMessage = "ExecuteDS(..) SqlException!\n " +
                    "\n\nSQL: " + strSQL;
                rethrow = new Exception(strMessage);
            }
            catch (Exception e)
            {
                string strMessage = "ExecuteDS(..) failed!\n "
                    + e.Message
                    + "\nSQL: " + strSQL;
                rethrow = new Exception(strMessage);
            }


            if (rethrow != null)
            {
                throw rethrow;
            }

            return ds;
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] commandParameters)
        {
            int affectedRows = 0;
            Exception rethrow = null;

            try
            {
                using (var command = new SqlCommand(commandText, Conn))
                {
                    command.CommandType = commandType;
                    command.Parameters.AddRange(commandParameters);
                    affectedRows = command.ExecuteNonQuery();
                }

            }
            catch (SqlException e)
            {
                string strMessage = "DBHelpers.ExecuteNonQuery(..) SqlException!\n " +
                    "\n\nSQL: " + commandText;
                rethrow = new Exception(strMessage);
            }
            catch (Exception e)
            {
                string strMessage = "DBHelpers.ExecuteNonQuery(..) failed!\n "
                    + e.Message
                    + "\nSQL: " + commandText;
                rethrow = new Exception(strMessage);
            }

            if (rethrow != null)
            {
                throw rethrow;
            }
            return affectedRows;
        }


    }
}
