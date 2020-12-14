using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class GridSampleScoreListDto
    {
        public int StuId { get; set; }

        public string StuName { get; set; }

        public int? CourseId { get; set; }

        public string CourseName { get; set; }

        public int? Team { get; set; }

        public int? ScoreId { get; set; }  

        public decimal TotalScore { get; set; }

       //1:主干 2 ：抽考 
        public int  hasPermission { get; set; }
    }
}