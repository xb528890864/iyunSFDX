using System.Collections.Generic;

namespace IYun.Controllers.ControllerObject
{
    public class ScoreAjaxModel
    {
        public int NomalBili { get; set; }

        public int ExamBili { get; set; }

        public List<ScoreAjaxDto> ScoreList { get; set; }
    }
}