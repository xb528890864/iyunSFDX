using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// 未付款状态
        /// </summary>
        NoPay = 1,
        /// <summary>
        /// 已付款待发货状态
        /// </summary>
        HasPay = 2,
        /// <summary>
        /// 货到付款待发货状态
        /// </summary>
        CashOn = 3,
        /// <summary>
        /// 已发货状态
        /// </summary>
        SendOut = 4,
        /// <summary>
        /// 已收货状态
        /// </summary>
        TakeOver = 5,
        /// <summary>
        /// 订单取消状态
        /// </summary>
        OrderCancel = 6
    }
}