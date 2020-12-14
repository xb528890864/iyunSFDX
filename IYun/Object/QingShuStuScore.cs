using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public class QingShuStuScore
    {
        public int studentId { get; set; }
        public string studentNumber { get; set; }
        public int courseId { get; set; }
        public int term { get; set; }
        public string courseName { get; set; }
        public int teachingPlanCourseId { get; set; }
        public decimal loginScore { get; set; }
        public decimal courseStudyScore { get; set; }
        public decimal quizScore { get; set; }
        public decimal forumScore { get; set; }
        public decimal usualScore { get; set; }
        public decimal examScore { get; set; }
        public decimal finalScore { get; set; }
        public string dispalyName { get; set; }
    }
}