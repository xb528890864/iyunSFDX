using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    /// <summary>
    /// 普通方法返回信息集对象
    /// </summary>
    public class ResultInfo
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 权限信息
        /// </summary>
        public PowerInfo Info { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public String Message { get; set; }
    }
}