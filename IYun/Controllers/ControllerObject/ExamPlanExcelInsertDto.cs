using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class ExamPlanExcelInsertDto
    {
        // <summary>
        /// 专业名
        /// </summary>
        public string MajorName { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public string EduType { get; set; }

        /// <summary>
        /// 学习形式
        /// </summary>
        public string StuType { get; set; }

        /// <summary>
        /// 课程名
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// 考试时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public int Year { get; set; }
    }
}