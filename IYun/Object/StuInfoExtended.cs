using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun.Object
{
    public class StuInfoExtended:YD_Sts_StuInfo
    {
        public int MajorLibrary { get; set; }
        public int EduType { get; set; }
        public int StuType { get; set; }
    }
}