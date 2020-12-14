using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Object
{
    public enum ApprovaState
    {
        /// <summary>
        /// 待审批状态
        /// </summary>
        WaitApprova = 1,
        /// <summary>
        /// 已审批状态
        /// </summary>
        HadApprova = 2,
        /// <summary>
        /// 已退回
        /// </summary>
        Return = 3,
       
    }
}