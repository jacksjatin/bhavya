using AutoGenerateOrdersScheduler.Models;
using AutoGenerateOrdersScheduler.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoGenerateOrdersScheduler
{
    public class ProcessOrders
    {
        OrdersQueryService ordQry = new OrdersQueryService();
        public void ProccessVaccineOrders()
        {
            try
            {
                string inputFolder = ConfigurationManager.AppSettings["InputFolderPath"].ToString();

                string[] filepaths = Directory.GetFiles(inputFolder, "*.csv");

                foreach (var fpath in filepaths)
                {
                    if (File.Exists(fpath))
                    {
                        DataTable dt = CSVService.ConvertCSVtoDataTable(fpath);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                var vccOrd = new VaccineOrderModel
                                {
                                    ProvId = dr["ProvId"].ToString(),
                                    NdcCode = dr["NdcCode"].ToString(),
                                    DosesOrdered = dr["DosesOrdered"].ToString(),
                                    DAPed = dr["DAPed"].ToString(),
                                    DAAdult = dr["DAAdult"].ToString()
                                };

                                this.ProccessAllOrders(vccOrd);
                            }
                        }
                    }
                }               
                
            }
            catch (Exception ex)
            {

                // Log Error
                throw ex;
            }
        }


        public void ProccessAllOrders(VaccineOrderModel vccOrdMdl)
        {
            // Get CtrlNum

            string orderId = this.GetCtrlNum();

            // Create Order with Provider 
            this.CreateProviderOrder(orderId, vccOrdMdl.ProvId);

            // Get CategoryId and MfrCode

            var codes = this.GetCodes(vccOrdMdl.ProvId, vccOrdMdl.NdcCode);

            // Create Vaccine Orders
            this.CreateVaccineOrders(orderId, vccOrdMdl.NdcCode, codes.CategoryId, codes.MftrCode, vccOrdMdl.DosesOrdered, vccOrdMdl.DAPed, vccOrdMdl.DAAdult);

        }


        public int CreateProviderOrder(string orderId, string providerId)
        {
            return ordQry.CreateVaccineOrderWithProvider(orderId, providerId);
        }

        public int CreateVaccineOrders(string orderId, string ndcCode, string categoryId, string mfrCode, string dosesOrder, string daPed, string daAdult)
        {
            return ordQry.CreateOrderVaccines(orderId, ndcCode, categoryId, mfrCode, dosesOrder, daPed, daAdult);
        }


        public CategoryIdMftrCode GetCodes(string provId, string ndcCode)
        {
            DataSet ds = new DataSet();
            CategoryIdMftrCode codes = new CategoryIdMftrCode();
            ordQry.GetCatgoryMfrCode(provId, ndcCode, ref ds);


            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                codes.CategoryId = dr["Category_Id"].ToString();
                codes.MftrCode = dr["Mftr_Code"].ToString();

            }

            return codes;
        }


        public string GetCtrlNum()
        {
            string ctrl_Num = string.Empty;
            DataSet ordProvDs = new DataSet();

            // Get Ctrl Num 

            ordQry.GetVcOrderCtrlNum(ref ordProvDs);

            if (ordProvDs != null && ordProvDs.Tables.Count > 0 && ordProvDs.Tables[0].Rows.Count > 0)
            {
                ctrl_Num = ordProvDs.Tables[0].Rows[0][0].ToString();

                if (!string.IsNullOrEmpty(ctrl_Num))
                {
                    ordQry.UpdateVcOrderCtrlNum(int.Parse(ctrl_Num));
                }
            }

            return ctrl_Num;
        }


    }
}
