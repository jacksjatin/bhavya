using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    public class SynergerticsUpdate
    {

        public void ProcessSynRecords()
        {
            SyngEntityHelper synhelp = new SyngEntityHelper();
            SynergerticModel sModel = new SynergerticModel();
            DataSet ds = new DataSet();
            int timeIntrval = 0;
            try
            {
                //Get Records based on configured time interval from Synergertic Table
                int.TryParse(ConfigurationManager.AppSettings["TimeInveterval"], out timeIntrval);
                synhelp.GetRecordsbyInterval(timeIntrval, ref ds);
                
                #region Loop through filtered synergertics records

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    sModel.Batch_name = item["Batch_name"].ToString();
                    sModel.Batch_status = item["Batch_status"].ToString();
                    sModel.Creation_date = item["Creation_date"].ToString();
                    sModel.Last_modified = item["Last_modified"].ToString();
                    sModel.Batch_Class = item["Batch_Class"].ToString();
                    sModel.unqid = synhelp.GetUniqueID(sModel.Batch_name);

                    //Check Unique ID exists in IMI Recon Table
                    if (synhelp.CheckUniqueIDExistsInIMIRecon(sModel.unqid, ref ds) == 1)
                    {
                        synhelp.CreatorUpdSynergerticRecord(sModel.unqid, sModel.Batch_name, sModel.Batch_status, sModel.Creation_date, sModel.Last_modified, sModel.Batch_Class, "TRUE", true);
                        continue;
                    }

                    //Create New Record in IMI Recon Table in case of email etc
                    synhelp.CreatorUpdSynergerticRecord(sModel.unqid, sModel.Batch_name, sModel.Batch_status, sModel.Creation_date, sModel.Last_modified, sModel.Batch_Class, "TRUE", false);

                }
                #endregion
            }
            catch (Exception ex)
            {
                // Log Error
            }

        }
    }

    public class SynergerticModel
    {
        public string unqid { get; set; }
        public string Batch_name { get; set; }
        public string Batch_status { get; set; }
        public string Creation_date { get; set; }
        public string Last_modified { get; set; }
        public string Batch_Class { get; set; }
    }
}
