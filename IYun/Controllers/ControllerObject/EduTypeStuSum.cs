using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class EduTypeStuSum
    {
        public int sumcount { get; set; } //总人数
        public int y_inyear { get; set; }
         /// <summary>
        /// 高起专(普通/3)
        /// </summary>
        public int specomthree { get; set; } 
        /// <summary>
        /// 高起专(艺术/3)
        /// </summary>
        public int speartthree { get; set; } 
        /// <summary>
        /// 高起专(普通/4)
        /// </summary>
        public int specomfour { get; set; } 
        /// <summary>
        /// 专升本(普通/3)
        /// </summary>
        public int speupgracomthree { get; set; }

        /// <summary>
        /// 专升本(艺术/3)
        /// </summary>
        public int speupgraartthree { get; set; }
        /// <summary>
        /// 高起本(普通/5)
        /// </summary>
        public int thiscomfive { get; set; }
    }
}