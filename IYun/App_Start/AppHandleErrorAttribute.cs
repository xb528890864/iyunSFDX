using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using log4net;

namespace IYun
{
    public class AppHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //使用log4net记录错误消息
            Exception Error = filterContext.Exception;
            string Message = Error.Message;//错误信息
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(Message);
            }
            else if (logger.IsErrorEnabled)
            {
                logger.Error(Message);
            }
            else if (logger.IsWarnEnabled)
            {
                logger.Warn(Message);
            }
            else if (logger.IsDebugEnabled)
            {
                logger.Debug(Message);
            }
            else if (logger.IsInfoEnabled)
            {
                logger.Info(Message);
            }
        }
    }
}