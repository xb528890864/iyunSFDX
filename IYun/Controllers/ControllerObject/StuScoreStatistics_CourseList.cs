using IYun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class StuScoreStatistics_CourseList
    {
        public VW_StuInfo stulist { get; set; }

        public List<ScoreStatistics_Course> ScoreList { get;set;}
    }
}