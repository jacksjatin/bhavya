﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    class DBHelpers
    {    

        private static string GetSQLExceptionText(SqlException e)
        {
            string strReturn = "";

            try
            {
                strReturn += "Class: " + e.Class + "\n";
                strReturn += "Error # " + e.Number + ": " + e.Number + " on line " + e.LineNumber + "." + "\n";
                strReturn += "Error reported by " + e.Source + " while connected to " + e.Server + "\n";

                //loop through any errors in the collect
                if (e.Errors.Count > 0)
                {
                    strReturn += "Errors collection contains:\n";
                    for (int i = 0; i < e.Errors.Count; i++)
                    {
                        strReturn += "Class: " + e.Errors[i].Class + "\n";
                        strReturn += "Error #" + e.Errors[i].Number + ": " + e.Errors[i].Message + " on line " + e.Errors[i].LineNumber + ".\n";
                        strReturn += "Error reported by " + e.Errors[i].Source + " while connected to " + e.Errors[i].Server + "\n";
                    }
                }
            }
            catch (Exception e1)
            {
                strReturn = "Failed to parse SQL Exception! (" + e1.Message;
            }

            return strReturn;
        }

        public static DataSet ExecuteDS(string strSQL)
        {
            SqlConnection connection = null;
            DataSet ds = null;
            Exception rethrow = null;

            try
            {

                connection = ConnectionManager.GetSqlConnection();
                SqlCommand command = new SqlCommand(strSQL, connection);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                ds = new DataSet();
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                string strMessage = "DBHelpers.ExecuteDS(..) SqlException!\n " +
                    GetSQLExceptionText(e) + "\n\nSQL: " + strSQL;
                WriteToEventLog(strMessage, EventLogEntryType.Error);
            }
            catch (Exception e)
            {
                string strMessage = "DBHelpers.ExecuteDS(..) failed!\n "
                    + e.Message
                    + "\nSQL: " + strSQL;
                WriteToEventLog(strMessage, EventLogEntryType.Error);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            if (rethrow != null)
            {
                WriteToEventLog(rethrow.Message, EventLogEntryType.Error);
            }

            return ds;
        }

        public static int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] commandParameters)
        {
            int affectedRows = 0;
            Exception rethrow = null;
            SqlConnection connection = null;
            try
            {
                using (connection = ConnectionManager.GetSqlConnection())
                {
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        command.CommandType = commandType;
                        command.Parameters.AddRange(commandParameters);
                        affectedRows = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                string strMessage = "DBHelpers.ExecuteNonQuery(..) SqlException!\n " +
                    GetSQLExceptionText(e) + "\n\nSQL: " + commandText;
                WriteToEventLog(strMessage, EventLogEntryType.Error);
            }
            catch (Exception e)
            {
                string strMessage = "DBHelpers.ExecuteNonQuery(..) failed!\n "
                    + e.Message
                    + "\nSQL: " + commandText;
                WriteToEventLog(strMessage, EventLogEntryType.Error);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            if (rethrow != null)
            {
                WriteToEventLog(rethrow.Message, EventLogEntryType.Error);
            }
            return affectedRows;
        }


        public static void WriteToEventLog(string message, EventLogEntryType level)
        {
            string source = "ImiReconScheduler";
            string log = "Application";
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }
            EventLog.WriteEntry(source, message,
                level);
        }
    }
}
