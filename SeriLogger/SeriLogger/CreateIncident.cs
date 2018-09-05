using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SeriLogger
{
    class CreateIncident
    {

        public HttpWebRequest request = null;
        public HttpWebResponse response = null;
        public Stream respStream = null;
        public StreamReader reader = null;


        public Exception creatInc(IncidentRequest ic)
        {
            string strMessage = string.Empty;
            string strReturnJson = string.Empty;
            string PartnerEndpointUrl = string.Empty;

            var json = new JavaScriptSerializer().Serialize(ic);
            Exception excep = null;
            try
            {

                PartnerEndpointUrl = "http://localhost:49276/api/incident/getInst";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PartnerEndpointUrl);
                request.Method = "POST";               
                request.ContentType = "application/json";
                var encoder = new UTF8Encoding();
                var data = encoder.GetBytes(json);
                request.ContentLength = data.Length;
                var reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                reqStream = null;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (reader = new StreamReader(response.GetResponseStream()))
                    {
                        strReturnJson = reader.ReadToEnd();
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
                strMessage = "CreateIncident method: Web Exception:" + webEx.Message;
                try
                {
                    response = (HttpWebResponse)webEx.Response;
                    respStream = response.GetResponseStream();
                    reader = new StreamReader(respStream, System.Text.Encoding.ASCII);
                    string FailureResponse = reader.ReadToEnd();
                   excep = new Exception(strMessage);
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} {1}", webEx.Message, ex.Message);
                    excep = new Exception(message);
                }
            }
            catch (Exception ex)
            {
                return new Exception(ex.Message);
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

    public class IncidentRequest
    {
        public string u_affectedci { get; set; }
        public string u_title { get; set; }
        public string u_type { get; set; }

        public string u_description { get; set; }

        public string u_customer_reference_no_ { get; set; }

        public string u_past_updates { get; set; }

        public string u_caller_email { get; set; }

        public string u_phone { get; set; }
    }

}
