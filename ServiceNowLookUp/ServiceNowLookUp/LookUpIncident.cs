using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNowLookUp
{
    class LookUpIncident
    {
        public HttpWebRequest request = null;
        public HttpWebResponse response = null;
        public Stream respStream = null;
        public StreamReader reader = null;

        public Exception FindIncident(string sysID,ref string results)
        {
            string strMessage = string.Empty;
            string strReturnJson = string.Empty;
            Exception excep = null;
            try
            {
                string PartnerEndpointUrl = "http://localhost:49276/api/incident/getIncident?sysID="+ sysID;
                request = (HttpWebRequest)WebRequest.Create(PartnerEndpointUrl);
                request.Method = "GET";
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (reader = new StreamReader(response.GetResponseStream()))
                    {
                        strReturnJson = reader.ReadToEnd();
                        results = strReturnJson;
                        int StatusCode = (int)response.StatusCode;
                        if (StatusCode < 200 || StatusCode > 299)
                        {
                            return new Exception("Invalid response from : " + PartnerEndpointUrl + " Response : " + strReturnJson + " Status Code " + StatusCode.ToString());
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                strMessage = "FindIncident method: Web Exception:" + webEx.Message;
                string ErrorMessage = string.Empty;
                excep = webEx;
            }
            catch (Exception ex)
            {
                excep = ex;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (respStream != null) respStream.Close();
                if (response != null) response.Close();
            }
            return excep;
        }
    }
}
