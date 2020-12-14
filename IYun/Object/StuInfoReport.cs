using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public class StuInfoReport
    {
        public int totalcount { get; set; }
        public string schoolName { get; set; }
        public string majorLibraryName { get; set; }
        public string eduTypeName { get; set; }
        public string stuTypeName { get; set; }
        public string stuStateName { get; set; } 
        public  int y_inyear { get; set; }
    }

    public class SubStuReport
    {
        public string schoolName { get; set; }
        public int y_inyear { get; set; }
        public int zaidu { get; set; }
        public int tuixue { get; set; }
        public int weizhuce { get; set; }
        public int zhuedaishenhe { get; set; }
        public int xiuxue { get; set; }
      }

    public class SubStuSupStuReport
    {
        public string schoolName { get; set; }

        public int y_inyear { get; set; }

        //支持
        public int isSuped { get; set; }

        //不支持
        public int notSuped { get; set; }
    }
}