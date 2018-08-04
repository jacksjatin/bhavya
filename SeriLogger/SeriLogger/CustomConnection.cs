using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SeriLogger
{
    class CustomConnection
    {
        public static string GetDBConnectionString()
        {
            try
            {
                //Decryptor decryptor = new Decryptor();
                //string NMICRegKey = ConfigurationManager.AppSettings["NMICRegKey"];
                //string FeeScheduleDBkey = ConfigurationManager.AppSettings["FeeScheduleDbSubkey"];
                //var connString = decryptor.GetConnString(NMICRegKey, FeeScheduleDBkey);
                //return connString.ToString();
                return "";
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
