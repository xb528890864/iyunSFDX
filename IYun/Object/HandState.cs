using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    /// <summary>
    /// 处理/评价/回复状态
    /// </summary>
    public enum HandState
    {
        /// <summary>
        /// 未处理/评价/回复状态
        /// </summary>
        Handing = 0,
        /// <summary>
        /// 已处理/评价/回复状态
        /// </summary>
        Handed = 1
    }
}