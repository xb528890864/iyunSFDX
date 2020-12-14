using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class ZYYFeeCheckStu
    {
        public string y_stuNum { get; set; }
        public string y_name { get; set; }
        public int y_inYear { get; set; }
        public string schoolName { get; set; }
        public string MajorLibName { get; set; }
        public string EduTypeName { get; set; }
        public string StuTypeName { get; set; }
        public string y_cardId { get; set; }
        public int y_feeYear { get; set; }
        public string y_isUp { get; set; }
        public string y_isCheckFee { get; set; }
        public int y_needFee { get; set; }
    }
}