using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class DHLGBatchCheck
    {
        public string schoolName { get; set; }
        public int y_inyear { get; set; }
        public int totalcount { get; set; }
        public decimal tuitiontotal { get; set; }
        public decimal needtotal { get; set; }
        public DateTime y_time { get; set; }
        public string y_check { get; set; }

        public string governorName { get; set; }
    }
}