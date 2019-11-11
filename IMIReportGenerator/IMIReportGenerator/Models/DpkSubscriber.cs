using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMIReportGenerator.Models
{
    public class DpkSubscriber
    {
        public string Dpk { get; set; }
        public List<string> Subscribers { get; set; }
    }

    public class RootObject
    {
        public List<DpkSubscriber> DpkSubscribers { get; set; }
    }
}
