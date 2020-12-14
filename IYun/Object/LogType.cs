using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 登录
        /// </summary>
        LoginIn = 1,
        /// <summary>
        /// 登出
        /// </summary>
        LoginOut = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Update = 3,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 4, 
        /// <summary>
        /// 新增
        /// </summary>
        Insert = 5
    }
}