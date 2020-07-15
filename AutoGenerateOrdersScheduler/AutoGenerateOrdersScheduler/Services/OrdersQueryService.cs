using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenerateOrdersScheduler.Services
{
    public class OrdersQueryService
    {

        public int GetVcOrderCtrlNum(ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("Select Ctrl_Num = (Cast(Ctrl_Num as int) + 1 ) from Vtrcks_Order_CtrlNum ");
            sb.Append("Where VtrcksOrderId = 'OrderID'; ");

            selectQuery = sb.ToString();
            ds = DBHelperService.ExecuteDS(selectQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {

                res = 1;
            }
            return res;
        }

        public int GetCatgoryMfrCode(string ProvId, string NdcCode, ref DataSet ds)
        {
            string selectQuery = string.Empty;
            int res = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("Select Category_Id,Mftr_Code from Vtrcks_ProvVaccines Where ");
            sb.Append("Prov_Id=" + "'" + ProvId + "' AND ");
            sb.Append("NDC_Code=" + "'" + NdcCode + "'");

            selectQuery = sb.ToString();
            ds = DBHelperService.ExecuteDS(selectQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {

                res = 1;
            }
            return res;

        }


        public int UpdateVcOrderCtrlNum(int ctrlNum)
        {
            string selectQuery = string.Empty;
            int res = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("Update Vtrcks_Order_CtrlNum ");
            sb.Append("Set Ctrl_Num = @ctrlNum ");
            sb.Append("Where VtrcksOrderId = 'OrderID'; ");

            selectQuery = sb.ToString();
            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@ctrlNum", ctrlNum) };

            res = DBHelperService.ExecuteNonQuery(selectQuery, type, parameterList);

            return res;

        }

        public int CreateVaccineOrderWithProvider(string orderId, string providerId)
        {
            string selectQuery = string.Empty;
            int res = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append("Insert into Vtrcks_Vaccine_Order (OrderId,ProvID,OrderDate,Status_Code,FrigTempReported,NDDOH_Comment) ");
            sb.Append("Values (cast(@OrderId as nchar(10)),cast(@ProvID as varchar(5)),GetDate(),@StatusCode,@FridTempReported,@NDDOH_Comment) ");

            selectQuery = sb.ToString();
            CommandType type = CommandType.Text;

            SqlParameter[] parameterList = { new SqlParameter("@OrderId", orderId),
            new SqlParameter("@ProvID", providerId),
            new SqlParameter("@StatusCode", 6),
            new SqlParameter("@FridTempReported", 'Y'),
            new SqlParameter("@NDDOH_Comment","")};

            res = DBHelperService.ExecuteNonQuery(selectQuery, type, parameterList);

            return res;

        }


        public int CreateOrderVaccines(string orderId, string ndcCode, string categoryId, string mfrCode, string dosesOrdered, string DAPed, string DAAdult)
        {

            string selectQuery = string.Empty;
            int res = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append("Insert into Vtrcks_OrderId_Vaccine ");
            sb.Append("(OrderID, NDC_Code, Category_Id, Mftr_Code, NDIIS_Inventory, Inven_On_Hand, Doses_Administered, ");
            sb.Append("Sugg_Order_Min, Sugg_Order_Max, Doses_Ordered, Reason_For_OverSugg, ");
            sb.Append("DAPed, DAAdult, DSHFunding317, DSHFundingVFC, DSHFundingState, DSHStatePO, HBig_Comment) ");
            sb.Append("Values (cast(@OrderID as nchar(10)), @NDC_Code, @Category_Id, @Mftr_Code, @NDIIS_Inventory, @Inven_On_Hand, @Doses_Administered, ");
            sb.Append("@Sugg_Order_Min, @Sugg_Order_Max, @Doses_Ordered, @Reason_For_OverSugg, ");
            sb.Append("@DAPed, @DAAdult, @DSHFunding317, @DSHFundingVFC, @DSHFundingState, @DSHStatePO, @HBig_Comment)");

            selectQuery = sb.ToString();

            CommandType type = CommandType.Text;
            SqlParameter[] parameterList = { new SqlParameter("@OrderID",orderId),
                                                 new SqlParameter("@NDC_Code",ndcCode),
                                                 new SqlParameter("@Category_Id",categoryId), // 3
                                                 new SqlParameter("@Mftr_Code",mfrCode), // PMC
                                                 new SqlParameter("@NDIIS_Inventory","0"),
                                                 new SqlParameter("@Inven_On_Hand","0"),
                                                 new SqlParameter("@Doses_Administered","0"),
                                                 new SqlParameter("@Sugg_Order_Min","0"),
                                                 new SqlParameter("@Sugg_Order_Max","0"),
                                                 new SqlParameter("@Doses_Ordered",dosesOrdered), // 20
                                                 new SqlParameter("@Reason_For_OverSugg","Allocation"),
                                                 new SqlParameter("@DAPed",DAPed),
                                                 new SqlParameter("@DAAdult",DAAdult),
                                                 new SqlParameter("@DSHFunding317",""),
                                                 new SqlParameter("@DSHFundingVFC",""),
                                                 new SqlParameter("@DSHFundingState",""),
                                                 new SqlParameter("@DSHStatePO",""),
                                                 new SqlParameter("@HBig_Comment",""),
                };

            res = DBHelperService.ExecuteNonQuery(selectQuery, type, parameterList);

            return res;


        }


    }
}
