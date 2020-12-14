using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun.Controllers.ControllerObject
{
    public class ScoreListDto
    {
        public int StuId { get; set; }

        public string StuName { get; set; }

        public int? CourseId { get; set; }

        public string CourseName { get; set; }

        public int? Team { get; set; }

        public int? ScoreId { get; set; }

        public decimal NormalScore { get; set; }

        public decimal TermScore { get; set; }

        public decimal TotalScore { get; set; }

        public bool HasPermission { get; set; }

        public string stunum { get; set; }

        //为师大新增的人工成绩审核判断属性 1为通过 null或0为未通过
        public string ScoreOk { get; set; }
    }
}