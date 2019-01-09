using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    class ConnectionManager
    {
        public static SqlConnection GetSqlConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SynDBPath"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
