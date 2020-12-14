using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun.Controllers.ControllerObject
{
    public class StudentsScoreDto
    {
        public VW_StuInfo Stu { get; set; }
        public VW_Score Score { get; set; }
        public YD_TeaPlan_ClassCourseDes Classdes { get; set; }
       
    }
}