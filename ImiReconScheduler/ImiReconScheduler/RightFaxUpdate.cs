using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImiReconScheduler
{
    class RightFaxUpdate
    {

        RightFaxHelper rfHelpObj = new RightFaxHelper();

        RightFaxModel rytFxModel = new RightFaxModel();
        public void processRightfaxRecords()
        {


            int curntIdx = getRowIndexFromStore();
            DataSet ds = new DataSet();
            rfHelpObj.loggertofile("Start Time: " + DateTime.Now.ToString());

            try
            {
                if (curntIdx != 0)
                {

                    rfHelpObj.loggertofile("Current Index: " + curntIdx);
                    rfHelpObj.GetRecordsbyLastIndex(curntIdx, ref ds);
                }
                else
                {
                    rfHelpObj.getCurrentDayRecords(ref ds);

                    //Insert All Records
                }
                StringBuilder sb = new StringBuilder();
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    int rowNum = Convert.ToInt32(item["RowNo"]);
                    sb.Append(" " + rowNum);
                    rytFxModel.unqid = item["UniqueID"].ToString();
                    rytFxModel.CreationTime = item["CreationTime"].ToString();
                    rytFxModel.TermStat = Convert.ToInt32(item["TermStat"]);
                    rytFxModel.CompletionTime = item["CompletionTime"].ToString();

                    rfHelpObj.CreatRightFacRecord(rytFxModel.unqid, rytFxModel.CreationTime, rytFxModel.CompletionTime, rytFxModel.TermStat);
                    if (item == ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1])
                    {

                        sb.Append("Last row value" + rowNum);
                        sb.AppendLine("In ForEach Row Count: " + ds.Tables[0].Rows.Count);
                        File.WriteAllText(ConfigurationManager.AppSettings["IndexStorePath"], $"RowIndex:{rowNum}");
                    }
                }

                rfHelpObj.loggertofile("Full Trace: " + sb.ToString());

                rfHelpObj.loggertofile("End Time: " + DateTime.Now.ToString());
            }
            catch(Exception ex)
            {

            }


        }

        public int getRowIndexFromStore()
        {
            string[] lines = File.ReadAllLines(ConfigurationManager.AppSettings["IndexStorePath"]);
            var dict = lines.Select(l => l.Split(':')).ToDictionary(a => a[0], a => a[1]);

            return Convert.ToInt32(dict["RowIndex"]);
        }
    }

    public class RightFaxModel
    {
        public string unqid { get; set; }
        public string CreationTime { get; set; }
        public string CompletionTime { get; set; }
        public int TermStat { get; set; }
    }


}
