using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public class QingShuCourseMessage
    {
        public int hr { get; set; }
        public string message { get; set; }
        public List<QingShuStuCourse> data { get; set; }
        public string extraData { get; set; }
    }
}