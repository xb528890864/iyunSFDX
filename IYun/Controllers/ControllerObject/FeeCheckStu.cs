using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class FeeCheckStu
    {
        public string y_stuNum { get; set; }
        public string y_name { get; set; }
        public int y_inYear { get; set; }
        public string schoolName { get; set; }
        public string schoolCode { get; set; }
        public string majorName { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public string y_cardId { get; set; }
        public int y_stuYear { get; set; }   //学制
        public int y_feeYear { get; set; }
        public string y_isUp { get; set; }
        public string y_isCheckFee { get; set; }
        public int y_needFee { get; set; }
        public int y_needUpFee { get; set; }

    }
}