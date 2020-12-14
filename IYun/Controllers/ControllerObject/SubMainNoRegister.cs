using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Controllers.ControllerObject
{
    public class SubMainNoRegister
    {
        /// <summary>
        /// 入学年份
        /// </summary>
        public int y_inyear { get; set; }
      
        /// <summary>
        /// 函授站名称
        /// </summary>
        public string schoolname { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 已提交人数
        /// </summary>
        public int regsteryes { get; set; }

        /// <summary>
        /// 已审核人数
        /// </summary>
        public int regstercheck { get; set; }
        /// <summary>
        /// 未提交人数
        /// </summary>
        public int regsterno { get; set; }
    }
}