using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class SubFeeStuTjShow
    {
        public int schoolId { get; set; }
        public string schoolname { get; set; }
        public int inyear { get; set; }
        public int edutypeId { get; set; }
        public string edutypename { get; set; }
        public int majorlibId { get; set; }
        public string majorlibname { get; set; }
        public int stuyear { get; set; }
        public int rs { get; set; }
        public int needfee { get; set; }
        public int bili { get; set; }
        public decimal RealNeedfee { get; set; }
    }
}