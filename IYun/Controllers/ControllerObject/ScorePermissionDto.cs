using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class ScorePermissionDto
    {
        public int CourseId { get; set; }

        public int? Type { get; set; }

        public int? Term { get; set; }
    }
}