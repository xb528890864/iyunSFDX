using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    /// <summary>
    /// 权限验证返回值
    /// </summary>
    public enum PowerInfo
    {
        /// <summary>
        /// 无权限
        /// </summary>
        NoPower = 0,
        /// <summary>
        /// 未登录
        /// </summary>
        NoLogin = 1,
        /// <summary>
        /// 未知状况
        /// </summary>
        Unknow = 2,
        /// <summary>
        /// 拥有权限
        /// </summary>
        HasPower = 3
        
    }
}