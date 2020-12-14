using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class ExamPlanDto
    {
        public string majorName { get; set; }

        public int year { get; set; }

        public List<ExamPlan> ExamPlanList;
    }

    public class ExamPlan
    {
        public int term { get; set; }

        public int id { get; set; }

        public string courseName { get; set; }

        public DateTime time { get; set; }

        public string subSchoolName { get; set; }
    }

}