using IYun.Models;

namespace IYun.Controllers.ControllerObject
{
    public class ScoreInputDto
    {
        public int stuId { get; set; }

        public string stuName { get; set; }

        public int scoreid { get; set; }

        public int term { get; set; }

        public int courseId { get; set; }

        public decimal normalScore { get; set; }

        public decimal termScore { get; set; }

        public decimal totalScore { get; set; }

        public string courseName { get; set; }

        public bool hasPermission { get; set; }
    }
}