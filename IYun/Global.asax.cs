using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IYun.Common;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace IYun
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MiniProfilerEF6.Initialize();
            PowerInit.InitPower();

            //var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IYunEntities"].ToString();
            ////启动数据库的数据缓存依赖功能    
            //SqlCacheDependencyAdmin.EnableNotifications(connectionString);
            ////启用数据表缓存
            //SqlCacheDependencyAdmin.EnableTableForNotifications(connectionString, "YD_Sys_Module");
            //SqlCacheDependencyAdmin.EnableTableForNotifications(connectionString, "YD_Sys_Power");
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)//这里是允许本地访问启动监控,可不写
            {
                MiniProfiler.Start();

            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }


        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    //在出现未处理的错误时运行的代码         
        //    Exception objError = Server.GetLastError().GetBaseException();

        //    if (objError.GetType().Name.Equals("HttpException"))
        //    {
        //        if (((HttpException)objError).GetHttpCode() == 404)
        //        {
        //            return;
        //        }
        //    }

        //    string errortime = string.Empty;
        //    string erroraddr = string.Empty;
        //    string errorinfo = string.Empty;
        //    string errorsource = string.Empty;
        //    string errortrace = string.Empty;
        //    string errormethodname = string.Empty;
        //    string errorclassname = string.Empty;

        //    errortime = "发生时间:" + System.DateTime.Now.ToString();
        //    erroraddr = "发生异常页: " + System.Web.HttpContext.Current.Request.Url.ToString();
        //    errorinfo = "异常信息: " + objError.Message;
        //    errorsource = "错误源:" + objError.Source;
        //    errortrace = "堆栈信息:" + objError.StackTrace;
        //    errorclassname = "发生错误的类名" + objError.TargetSite.DeclaringType.FullName;
        //    errormethodname = "发生错误的方法名：" + objError.TargetSite.Name;
        //    //清除当前异常 使之不返回到请求页面
        //    Server.ClearError();
        //    lock (this)
        //    {
        //        //文件不存在就创建,true表示追加
        //        // writer = new System.IO.StreamWriter(file.FullName, true); 
        //        string ip = "用户IP:" + Request.UserHostAddress;
        //        string log = errortime + "##" + erroraddr + "##" + ip + "##" + errorclassname + "##" + errormethodname + "##" + errorinfo + "##" + errorsource + "##" + errortrace.Replace("\r\n", "<br>");
        //        LogHelper.WriteErrorLog(GetType(), DateTime.Now.ToString("HH:mm:ss.fff") + log);
        //        //LogHelper.Error(new Exception(log), this.GetType().Name, "Global");
        //    }

        //    Response.Redirect("/SysAdmin/ErrorHtml");

        //}
    }
}