namespace IYun.Controllers.ControllerObject
{
    public class ScoreStatistics
    {
        public int SubSchoolId { get; set; }

        public int MajorId { get; set; }

        public string SchoolName { get; set; }

        public string MajorName { get; set; }

        public int TotalCount { get; set; }

        public int HasCount { get; set; }

        public bool IsSetTeaplan { get; set; }

        public int PassCount { get; set; }
    }
}