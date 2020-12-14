using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class StuScoreDto
    {
        public int StuId { get; set; }

        public int Term { get; set; }

        public int CourseId { get; set; }

        public decimal AvgNum { get; set; }

        public decimal EndNum { get; set; }

        public decimal TotalNum { get; set; }

    }
}