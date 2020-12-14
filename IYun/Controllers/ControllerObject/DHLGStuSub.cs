using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class DHLGStuSub
    {
        public DHLGStuSub(DateTime date)
        {
            y_birthday = date.ToString("yyyyMMdd");
        }
        public string y_name { get; set; }
        public string y_sex { get; set; }
        public string y_stuNum { get; set; }
        public string y_examNum { get; set; }
        public string majorLibraryName { get; set; }
        public string schoolCode { get; set; }
        public string majorLibraryCode { get; set; }
        public string eduTypeName { get; set; }
        public string stuTypeName { get; set; }
        public string schoolName { get; set; }
        public string y_cardId { get; set; }
        public string y_birthday { get; set; }
        public string nationName { get; set; }
        public string politicsName { get; set; }
        public string stuStateName { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public int y_inYear { get; set; }
        public string y_graduateNumber { get; set; }

        public string y_ClassNum { get; set; }

        public int y_realYear { get; set; }

    }
}