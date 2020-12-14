using IYun.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IYun
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // 江西理工更改首页
            if(ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXLG.ToString() || ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
            {
                routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "report", action = "SubStuReport", id = UrlParameter.Optional }
            );
            }
            else
            {
                routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Edu", action = "Shouye", id = UrlParameter.Optional }
            );
            }
            
        }
    }
}