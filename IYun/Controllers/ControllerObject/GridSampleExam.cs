using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun.Controllers.ControllerObject
{
    public class GridSampleExam
    {
        public int StuId { get; set; }

        public string StuName { get; set; }

        public int? CourseId { get; set; }

        public string CourseName { get; set; }

        public int? ScoreId { get; set; }

        public decimal TotalScore { get; set; }
    }
}