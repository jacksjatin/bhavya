using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogger
{
    public class IMIRecon
    {

        string insertReconQuery = "INSERT INTO ImiReconcilation (CreatedTimestamp,DPK,FLD,ImiFileName,ImiGenerated,AckReceived,UpdatedTimestamp) " +
            "VALUES (@CreatedTimestamp,@DPK,@FLD, @ImiFileName,@ImiGenerated,@AckReceived,@UpdatedTimestamp)";


        private int ImiReconLogger(ImiReconModel imiRec, string insertReconQuery)
        {
            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@CreatedTimestamp",imiRec.CreatedTimestamp),
                                                 new SqlParameter("@DPK",imiRec.DPK),
                                                 new SqlParameter("@FLD",imiRec.FLD),
                                                 new SqlParameter("@ImiFileName",imiRec.ImiFileName),
                                                 new SqlParameter("@ImiGenerated",imiRec.ImiGenerated),
                                                 new SqlParameter("@AckReceived",imiRec.AckReceived),
                                                 new SqlParameter("@UpdatedTimestamp",imiRec.UpdatedTimestamp)
                };
            int res = DBHelpers.ExecuteNonQuery(insertReconQuery, type, parameterList);
            return res;
        }

        public int imiReconLog(string CreatedTimestamp, string DPK, string FLD, string ImiFileName, string ImiGenerated, string AckReceived, string UpdatedTimestamp)
        {

            ImiReconModel imiRec = new ImiReconModel();
            imiRec.CreatedTimestamp = CreatedTimestamp;
            imiRec.DPK = DPK;
            imiRec.FLD = FLD;
            imiRec.ImiFileName = ImiFileName;
            imiRec.ImiGenerated = ImiGenerated;
            imiRec.AckReceived = AckReceived;
            imiRec.UpdatedTimestamp = UpdatedTimestamp;

            int res = ImiReconLogger(imiRec, insertReconQuery);
            return res;
        }

    }

    public class ImiReconModel
    {
        public string CreatedTimestamp { get; set; }
        public string DPK { get; set; }
        public string FLD { get; set; }
        public string ImiFileName { get; set; }
        public string ImiGenerated { get; set; }
        public string AckReceived { get; set; }
        public string UpdatedTimestamp { get; set; }
    }
}
