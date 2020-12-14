using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public class QingShuStuMessage
    {
        public int hr { get; set; }
        public string message { get; set; }
        public List<QingStuInfo> data { get; set; }
        public string extraData { get; set; }
    }
}