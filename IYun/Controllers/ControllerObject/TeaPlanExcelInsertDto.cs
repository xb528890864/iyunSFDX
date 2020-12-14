namespace IYun.Controllers.ControllerObject
{
    public class TeaPlanExcelInsertDto
    {
        /// <summary>
        /// 专业库名
        /// </summary>
        public string MajorName { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public int EduType { get; set; }

        /// <summary>
        /// 学习形式
        /// </summary>
        public int StuType { get; set; }

        /// <summary>
        /// 课程名
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public int Team { get; set; }

        /// <summary>
        /// 自学课时
        /// </summary>
        public int SelfPeriod { get; set; }

        /// <summary>
        /// 面授课时
        /// </summary>
        public int TeaPeriod { get; set; }

        /// <summary>
        /// 作业课时
        /// </summary>
        public int TaskPeriod { get; set; }

        /// <summary>
        /// 课程类型
        /// </summary>
        public int CourseType { get; set; }

        public bool IsMain { get; set; }

        /// <summary>
        /// 自学课时-备用
        /// </summary>
        public int SelfPeriod2 { get; set; }

        /// <summary>
        /// 面授课时-备用
        /// </summary>
        public int TeaPeriod2 { get; set; }
        
        /// <summary>
        /// 模板ID
        /// </summary>
        public int TempletId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 专业ID
        /// </summary>
        public int MajorId { get; set; }

        /// <summary>
        /// 专业库ID
        /// </summary>
        public int MajorLibId { get; set; }
    }
}