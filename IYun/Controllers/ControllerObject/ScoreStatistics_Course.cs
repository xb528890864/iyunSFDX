using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class ScoreStatistics_Course
    {
        public int HasCount { get; set; }

        public int TotalCount { get; set; }

        public string CourseName { get; set; }

        public int CourseId { get; set; }

        public int Term { get; set; }

        public decimal? TotalScore { get; set; }

        public int SelfPeriod { get; set; }

        public int TeaPeriod { get; set; }

        public bool y_isMain { get; set; }

          public bool? y_sampleexam { get; set; }
    }
}