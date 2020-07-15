using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenerateOrdersScheduler.Models
{
    public class VaccineOrderModel
    {

        public string ProvId { get; set; }

        public string NdcCode { get; set; }

        public string DosesOrdered { get; set; }

        public string DAPed { get; set; }

        public string DAAdult { get; set; }

    }
}
