using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public static class CourseType2
    {

        public static string CourseTypeName(int? id)
        {




            if (id.Value == 1) { return "公共基础课"; }
            if (id.Value == 2) { return "专业基础课"; }
            if (id.Value == 3) { return "专业课"; }
            if (id.Value == 4) { return "实践环节"; }
            if (id.Value == 5) { return "选修课"; }
            return "";

        }
    }
}