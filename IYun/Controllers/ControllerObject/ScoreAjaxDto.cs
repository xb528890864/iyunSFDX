namespace IYun.Controllers.ControllerObject
{
    public class ScoreAjaxDto
    {
        public int ScoreId { get; set; }

        public int CourseId { get; set; }

        public int StuId { get; set; }

        public int Term { get; set; }

        public decimal NormalScore { get; set; }

        public decimal TermScore { get; set; }
    }
}