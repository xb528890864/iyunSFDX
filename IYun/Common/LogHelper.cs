using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using IYun.Models;
using log4net;

namespace IYun.Common
{
    public class LogHelper
    {
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void WriteLog(Type t, Exception ex)
        {
            var log = LogManager.GetLogger(t);
            log.Error(ex);
        }

        /// <summary>
        /// 输出Fatal日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteFatalLog(Type t, string msg)
        {
            var log = LogManager.GetLogger(t);
            
            log.Fatal(msg);
        }
        /// <summary>
        /// 输出Error日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteErrorLog(Type t, string msg)
        {
            var log = LogManager.GetLogger(t);
            log.Error(msg);
        }
        /// <summary>
        /// 输出Warn日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteWarnLog(Type t, string msg)
        {
            var log = LogManager.GetLogger(t);
            log.Warn(msg);
        }
        /// <summary>
        /// 输出Debug日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteDebugLog(Type t, string msg)
        {
            var log = LogManager.GetLogger(t);
            log.Debug(msg);
        }
        /// <summary>
        /// 输出Info日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteInfoLog(Type t, string msg)
        {
            var log = LogManager.GetLogger(t);
            log.Info(msg);
        }
        /// <summary>
        /// 记录数据库日志
        /// </summary>
        /// <param name="userId">操作员id</param>
        /// <param name="userName">操作员当前名</param>
        /// <param name="logTypeId">操作类型</param>
        /// <param name="remark">备注</param>
        public static void DbLog(int userId,string userName,int logTypeId,string remark)
        {
            var yunEntities = new IYunEntities();
            var dbLog = new YD_Sys_DbLog()
            {
                y_userID = userId,
                y_remark = remark,
                y_time = DateTime.Now,
                Y_userName = userName,
                Y_logTypeID = logTypeId,
            };
            yunEntities.Entry(dbLog).State = EntityState.Added;
            yunEntities.SaveChanges();
            yunEntities.Dispose();
        }
    }
}