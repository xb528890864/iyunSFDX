using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IYun.Common;
using IYun.Dal;
using IYun.Models;
using IYun.Object;
using log4net;

namespace IYun.Controllers
{
    public class AdminBaseController : Controller
    {

        public int YdAdminRoleId
        {
            get
            {   
                if (Session[KeyValue.Admin_Role_id] == null)
                {
                    Session.RemoveAll();
                    Response.Redirect("/AdminBase/Index");
                }

                return Convert.ToInt32(Session[KeyValue.Admin_Role_id]);

            }
        }

        public int YdAdminId
        {
            get
            {
                if (Session[KeyValue.Admin_id] == null)
                {
                    Session.RemoveAll();
                    Response.Redirect("/AdminBase/Index");
                }

                return Convert.ToInt32(Session[KeyValue.Admin_id]);
            }
        }

        public string YdAdminRelName
        {
            get
            {
                if (Session[KeyValue.AdminRelName] == null)
                {
                    Session.RemoveAll();
                    Response.Redirect("/AdminBase/Index");
                }

                return Session[KeyValue.AdminRelName].ToString();
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult TestError()
        {
            throw new Exception("TestError");
           
        }

        protected override void OnException(ExceptionContext filterContext)
        {

            var logger = LogManager.GetLogger("Error");
            logger.Error($"请求URL:  {filterContext.HttpContext.Request.RawUrl}");
            logger.Error($"请求数据:  {filterContext.HttpContext.Request.QueryString}");
            logger.Error("发生异常", filterContext.Exception);
            //如果是GET请求则跳转页面
            if(filterContext.HttpContext.Request.RequestType.ToString().ToUpper() == "GET")
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = Redirect("Error");
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        /// <summary>
        /// 后台默认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Login");
        }

        /// <summary>
        /// 后台框架Right页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Right()
        {
            return View();
        }

        /// <summary>
        /// 后台框架面板页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Board()
        {
            return PartialView();
        }

        /// <summary>
        /// 后台顶部消息框
        /// </summary>
        /// <returns></returns>
        public ActionResult TopMessage()
        {
            return PartialView();
        }

        /// <summary>
        /// 后台框架Main页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Main()
        {
            return IsLogin() ? View() : View("Login");
        }

        /// <summary>
        /// 后台框架Top页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Top()
        {
            if (!IsLogin())
            {
                return RedirectToAction("Index");
            }
            ViewBag.adminName = YdAdminRelName;

            ViewBag.admin = YdAdminRoleId;

            using(var ad=new IYunEntities())
            {
                ViewBag.stranges = 0;
                const int status = (int)ApprovaState.WaitApprova;
                var list =ad.VW_Strange.Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No).ToList();
                if (list.Count > 0)
                    ViewBag.stranges = list.Count; 
            }
           
            const int menu = (int)PowerState.Able;

            var cacheName = "PowerList";
            var r = HttpRuntime.Cache[cacheName] as List<IGrouping<int, VW_Power>>;//获取缓存中的导航栏数据，没有就从数据库中取
            if (r != null)
            {
                var modulePowers =
                    r.Find(u => u.Key == YdAdminRoleId)
                    .Where(u => u.y_level == 1 && u.y_menu == menu)
                    .Select(ydModulePower => new ModulePower
                    {
                        id = ydModulePower.id,
                        y_url = ydModulePower.y_url,
                        y_sort = ydModulePower.y_sort,
                        y_parentID = ydModulePower.y_parentID,
                        y_level = ydModulePower.y_level,
                        y_vaild = ydModulePower.y_vaild,
                        y_roleID = ydModulePower.y_roleID,
                        y_moduleID = ydModulePower.y_moduleID,
                        y_menu = ydModulePower.y_menu,
                        y_insert = ydModulePower.y_insert,
                        y_delete = ydModulePower.y_delete,
                        y_update = ydModulePower.y_update,
                        y_select = ydModulePower.y_select,
                        y_roleName = ydModulePower.y_roleName,
                        y_moudleName = ydModulePower.y_moudleName
                    }).OrderByDescending(u => u.y_sort).ToList();
                ViewBag.modulePowers = modulePowers;
            }
            else
            {
                LogHelper.WriteWarnLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--在TOP视图，缓存失效!");

                r = PowerInit.InitPower();
                var modulePowers =
                    r.Find(u => u.Key == YdAdminRoleId)
                    .Where(u => u.y_level == 1 && u.y_menu == menu)
                    .Select(ydModulePower => new ModulePower
                    {
                        id = ydModulePower.id,
                        y_url = ydModulePower.y_url,
                        y_sort = ydModulePower.y_sort,
                        y_parentID = ydModulePower.y_parentID,
                        y_level = ydModulePower.y_level,
                        y_vaild = ydModulePower.y_vaild,
                        y_roleID = ydModulePower.y_roleID,
                        y_moduleID = ydModulePower.y_moduleID,
                        y_menu = ydModulePower.y_menu,
                        y_insert = ydModulePower.y_insert,
                        y_delete = ydModulePower.y_delete,
                        y_update = ydModulePower.y_update,
                        y_select = ydModulePower.y_select,
                        y_roleName = ydModulePower.y_roleName,
                        y_moudleName = ydModulePower.y_moudleName
                    }).OrderByDescending(u => u.y_sort).ToList();
                ViewBag.modulePowers = modulePowers;
            }

            return PartialView();
        }

        public ActionResult Foot()
        {
            return PartialView();
        }

        /// <summary>
        /// 后台修改个人密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdatePwd()
        {
            return View();
        }

        /// <summary>
        /// 后台框架Left页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Left()
        {
            const int vaild = (int)DataState.Able;
            const int menu = (int)PowerState.Able;
            //这里获取登录用户的菜单权限并分析
            if (!IsLogin()) return RedirectToAction("Index", "AdminBase");
            using (var yunEntities = new IYunEntities())
            {
                var parentList =
                    yunEntities.VW_Power.Where(
                        u => u.y_level == 1 && u.y_roleID == YdAdminRoleId && u.y_vaild == vaild && u.y_menu == menu)
                        .OrderByDescending(u => u.y_sort)
                        .ToList();
                var modulePowers = parentList.Select(ydModulePower => new ModulePower
                {
                    id = ydModulePower.id,
                    y_url = ydModulePower.y_url,
                    y_sort = ydModulePower.y_sort,
                    y_parentID = ydModulePower.y_parentID,
                    y_level = ydModulePower.y_level,
                    y_vaild = ydModulePower.y_vaild,
                    y_roleID = ydModulePower.y_roleID,
                    y_moduleID = ydModulePower.y_moduleID,
                    y_menu = ydModulePower.y_menu,
                    y_insert = ydModulePower.y_insert,
                    y_delete = ydModulePower.y_delete,
                    y_update = ydModulePower.y_update,
                    y_select = ydModulePower.y_select,
                    y_roleName = ydModulePower.y_roleName,
                    y_moudleName = ydModulePower.y_moudleName,
                    children = GetChildrenModulePower(ydModulePower.y_moduleID, yunEntities)
                }).ToList();
                ViewBag.modulePowers = modulePowers;
            }
            return View();
        }

        /// <summary>
        /// 获取左边栏目html
        /// </summary>
        /// <param name="modulePowers"></param>
        /// <returns></returns>
        public static MvcHtmlString GetModule(List<ModulePower> modulePowers)
        {
            var moduleBuilder = new StringBuilder();
            foreach (var modulePower in modulePowers)
            {
                moduleBuilder.Append("<li class=\"\">");
                moduleBuilder.AppendFormat("<a href=\"javascript:jumpRight('{0}','{1}')\" class=\"dropdown-toggle\">",
                    modulePower.y_url, modulePower.y_moudleName);
                moduleBuilder.Append("<i class=\"menu-icon fa fa-list\"></i>");
                moduleBuilder.AppendFormat("<span class=\"menu-text\">{0}</span>", modulePower.y_moudleName);
                moduleBuilder.Append("<b class=\"arrow fa fa-angle-down\"></b>");
                moduleBuilder.Append("</a>");
                moduleBuilder.Append("<b class=\"arrow\"></b>");
                moduleBuilder.Append("<ul class=\"submenu\">");
                moduleBuilder.Append(GetModuleChildren(modulePower.children));
                moduleBuilder.Append("</ul>");
                moduleBuilder.Append("</li>");
            }
            return MvcHtmlString.Create(moduleBuilder.ToString());
        }

        /// <summary>
        /// 获取左边子栏目html
        /// </summary>
        /// <param name="modulePowers"></param>
        /// <returns></returns>
        public static MvcHtmlString GetModuleChildren(List<ModulePower> modulePowers)
        {
            var moduleBuilder = new StringBuilder();
            foreach (var modulePower in modulePowers)
            {
                if (modulePower.children.Count != 0)
                {
                    moduleBuilder.Append("<li class=\"\">");
                    moduleBuilder.AppendFormat(
                        "<a href=\"javascript:jumpRight('{0}','{1}')\" target=\"main\" class=\"dropdown-toggle\">",
                        modulePower.y_url, modulePower.y_moudleName);
                    //moduleBuilder.Append("<i class=\"menu-icon fa fa-list\"></i>");
                    //moduleBuilder.AppendFormat("<span class=\"menu-text\">{0}</span>", modulePower.y_moudleName);

                    moduleBuilder.Append("<i class=\"menu-icon fa fa-caret-right\"></i>");
                    moduleBuilder.AppendFormat("{0}", modulePower.y_moudleName);

                    moduleBuilder.Append("<b class=\"arrow fa fa-angle-down\"></b>");
                    moduleBuilder.Append("</a>");
                    moduleBuilder.Append("<b class=\"arrow\"></b>");
                    moduleBuilder.Append("<ul class=\"submenu\">");
                    moduleBuilder.Append(GetModuleChildren(modulePower.children));
                    moduleBuilder.Append("</ul>");
                    moduleBuilder.Append("</li>");
                }
                else
                {
                    moduleBuilder.Append("<li class=\"\">");
                    moduleBuilder.AppendFormat("<a href=\"javascript:jumpRight('{0}','{1}')\" target=\"main\">",
                        modulePower.y_url, modulePower.y_moudleName);

                    moduleBuilder.Append("<i class=\"menu-icon fa fa-caret-right\"></i>");
                    moduleBuilder.AppendFormat("{0}", modulePower.y_moudleName);

                    moduleBuilder.Append("</a>");
                    moduleBuilder.Append("<b class=\"arrow\"></b>");
                    moduleBuilder.Append("</li>");
                }
            }
            return MvcHtmlString.Create(moduleBuilder.ToString());
        }

        /// <summary>
        /// 检测用户是否已经登录
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            if (Session[KeyValue.Admin_LoginFlag] == null) return false;
            if (Session[KeyValue.Admin_LoginFlag].ToString() != KeyValue.Admin_LoginOKFlag) return false;
            return true;
        }

        /// <summary>
        /// 退出后台登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "AdminBase");
        }

        /// <summary>
        /// 后台登录验证
        /// </summary>
        /// <returns></returns>
        public string LoginVerify()
        {
            var result = "非法请求";
            var code = Request["code"];
            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
            }
            else
            {
                result = "无效验证码";
                return result;
            }
            if (!IsRightCode(code))
            {
                result = "验证码错误";
                return result;
            }
            var adminname = Request["adminname"].Trim();
            if(string.IsNullOrWhiteSpace(adminname))
            {
                result = "账号不能为空";
                return result;
            }
            var password = PageValidate.GetMd5StrL(Request["password"].Trim());
            using (var yunEntities = new IYunEntities())
            {
                YD_Sys_Admin admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.y_name == adminname && u.y_password == password);
                if (admin == null)
                {
                    adminname = Request["adminname"].Trim();
                    password = Request["password"].Trim();
                    YD_Sts_StuInfo admin1;
                    //if(adminname.Split('_')[0] != ConfigurationManager.AppSettings["QinshuKey"].ToString().ToLower())
                    //{
                    //    result = "账号或密码错误";
                    //    return result;
                    //}
                    //var stunum = adminname.Split('_')[1];
                    var model = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_loginName == adminname && u.y_isdel == 1);
                    if(model == null)
                    {
                        result = "账号或密码错误";
                        return result;
                    }
                    admin1 = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_loginName == adminname && u.y_password == password);
                    if (admin1 == null)
                    {
                        result = "账号或密码错误";
                        return result;
                    }
                    System.Web.HttpContext.Current.Session[KeyValue.Admin_Name] = adminname;
                    System.Web.HttpContext.Current.Session[KeyValue.stu_id] = admin1.id;
                    //新增


                    System.Web.HttpContext.Current.Session[KeyValue.Admin_Role_id] = yunEntities.YD_Sys_Role.FirstOrDefault(x=>x.y_name == "学生").id;
                    System.Web.HttpContext.Current.Session[KeyValue.Admin_id] = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(x => x.y_subSchoolId == admin1.y_subSchoolId).y_adminId;
                    System.Web.HttpContext.Current.Session[KeyValue.Admin_LoginFlag] = KeyValue.Admin_LoginOKFlag;
                    System.Web.HttpContext.Current.Session[KeyValue.AdminRelName] = admin1.y_name;
                    result = "ok_student";
                    return result;
                }
               
                //保存不加密的密码
                var pass = Request["password"].Trim();
                admin.y_realpassword = pass;
                yunEntities.Entry(admin).State = EntityState.Modified;
                yunEntities.SaveChanges();
                System.Web.HttpContext.Current.Session[KeyValue.Admin_Name] = adminname;//保存一些角色信息
                System.Web.HttpContext.Current.Session[KeyValue.Admin_id] = admin.id;
                //新增
                System.Web.HttpContext.Current.Session[KeyValue.Admin_Role_id] = admin.y_roleID;

                System.Web.HttpContext.Current.Session[KeyValue.Admin_LoginFlag] = KeyValue.Admin_LoginOKFlag;
                System.Web.HttpContext.Current.Session[KeyValue.AdminRelName] = admin.y_realName;

                //HttpRuntime.Cache.Insert(KeyValue.AdminObj, admin,);

                LogHelper.DbLog(admin.id, admin.y_name, (int)LogType.LoginIn, "正常登录。");
            }
            result = "ok";
            return result;

        }

        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsRightCode(string code)
        {
            return (Session[KeyValue.All_Code] != null &&
                   String.Equals(Session[KeyValue.All_Code].ToString(), code, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <returns></returns>
        public FileContentResult Code()
        {
            string code;
            var imageByte = FileHelper.CreateImage(5, 80, 30, out code);
            Session[KeyValue.All_Code] = code;
            return File(imageByte, "image/gif");
        }

        /// <summary>
        /// 对栏目内具体方法的权限验证
        /// </summary>
        /// <returns></returns>
        public YD_Sys_Power SafePowerAction()
        {
            if (!IsLogin()) return null;
            if (Request.UrlReferrer == null) return null;
            var module = new YD_Sys_Module();
            var url = Request.UrlReferrer.AbsolutePath;
            using (var yunEntities = new IYunEntities())
            {
                var modules = yunEntities.YD_Sys_Module.Where(u => true).ToList();
                for (var i = 0; i < modules.Count(); i++)
                {
                    var moduleName = modules[i].y_url;
                    //带有参数的链接将参数去掉
                    if (moduleName.Contains("/?"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.IndexOf("/?"));
                    }
                    //带有/结尾的链接将/去掉
                    if (moduleName.EndsWith("/"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.Length - 1);
                    }
                    if (moduleName.ToLower() != url.ToLower()) continue;
                    module = modules[i];
                    break;
                }
                return module == null
                    ? null
                    : yunEntities.YD_Sys_Power.FirstOrDefault(
                        u => u.y_moduleID == module.id && u.y_roleID == YdAdminRoleId);
            }
        }

        /// <summary>
        /// 权限验证方法,返回权限对象
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public YD_Sys_Power SafePowerPage(string action)
        {
            if (!IsLogin()) return null;
            if (action == null) return null;
            var module = new YD_Sys_Module();
            //带有参数的链接将参数去掉
            if (action.Contains("?"))
            {
                action = action.Substring(0, action.IndexOf('?'));
            }
            action = action.EndsWith("/") ? action.Substring(0, action.Length - 1) : action;
            using (var yunEntities = new IYunEntities())
            {
                var modules = yunEntities.YD_Sys_Module.Where(u => true).ToList();
                for (var i = 0; i < modules.Count(); i++)
                {
                    var moduleName = modules[i].y_url;
                    //带有参数的链接将参数去掉
                    if (moduleName.Contains("/?"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.IndexOf("/?"));
                    }
                    //带有/结尾的链接将/去掉
                    if (moduleName.EndsWith("/"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.Length - 1);
                    }
                    if (moduleName.ToLower() != action.ToLower()) continue;
                    module = modules[i];
                    break;
                }
                return module == null
                    ? null
                    : yunEntities.YD_Sys_Power.FirstOrDefault(
                        u => u.y_moduleID == module.id && u.y_roleID == YdAdminRoleId);
            }
        }

        /// <summary>
        /// 权限验证方法,返回权限验证信息
        /// </summary>
        /// <param name="action">需要验证的栏目名称</param>
        /// <param name="flag">需要验证的栏目权限（枚举）</param>
        /// <returns>返回权限验证信息（枚举）</returns>
        public PowerInfo Safe(string action, PowerFlag flag)
        {
            return new BaseDal<YD_Sys_Admin>().Safe(action, flag);
        }

        /// <summary>
        /// 根据父栏目ID获取兄弟栏目
        /// </summary>
        /// <param name="yunEntities">上下文对象</param>
        /// <param name="yParentID">父栏目ID</param>
        /// <returns>父栏目下子栏目集合</returns>
        public List<ModulePower> GetChildModulePower(IYunEntities yunEntities, int yParentID)
        {
            const int menu = (int)PowerState.Able;
            var cacheName = "PowerList";
            var r = HttpRuntime.Cache[cacheName] as List<IGrouping<int, VW_Power>>;
            if (r != null)
            {
                var modulePowers = r.Find(u => u.Key == YdAdminRoleId)
                    .Where(u => u.y_menu == menu && u.y_parentID == yParentID && u.y_level == 2)
                    .Select(ydModulePower => new ModulePower
                    {
                        id = ydModulePower.id,
                        y_url = ydModulePower.y_url,
                        y_sort = ydModulePower.y_sort,
                        y_parentID = ydModulePower.y_parentID,
                        y_level = ydModulePower.y_level,
                        y_vaild = ydModulePower.y_vaild,
                        y_roleID = ydModulePower.y_roleID,
                        y_moduleID = ydModulePower.y_moduleID,
                        y_menu = ydModulePower.y_menu,
                        y_insert = ydModulePower.y_insert,
                        y_delete = ydModulePower.y_delete,
                        y_update = ydModulePower.y_update,
                        y_select = ydModulePower.y_select,
                        y_roleName = ydModulePower.y_roleName,
                        y_moudleName = ydModulePower.y_moudleName,
                        children = r.Find(u => u.Key == YdAdminRoleId)
                            .Where(u => u.y_menu == menu && u.y_parentID == ydModulePower.y_moduleID && u.y_level == 3)
                            .Select(k => new ModulePower
                            {
                                id = k.id,
                                y_url = k.y_url,
                                y_sort = k.y_sort,
                                y_parentID = k.y_parentID,
                                y_level = k.y_level,
                                y_vaild = k.y_vaild,
                                y_roleID = k.y_roleID,
                                y_moduleID = k.y_moduleID,
                                y_menu = k.y_menu,
                                y_insert = k.y_insert,
                                y_delete = k.y_delete,
                                y_update = k.y_update,
                                y_select = k.y_select,
                                y_roleName = k.y_roleName,
                                y_moudleName = k.y_moudleName,
                            }).OrderByDescending(u => u.y_sort).ToList()
                    }).OrderByDescending(u => u.y_sort).ToList();

                return modulePowers;
            }
            else
            {
                LogHelper.WriteWarnLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--在GetChildModulePower，缓存失效!");

                r = PowerInit.InitPower();

                var modulePowers = r.Find(u => u.Key == YdAdminRoleId)
                    .Where(u => u.y_menu == menu && u.y_parentID == yParentID && u.y_level == 2)
                    .Select(ydModulePower => new ModulePower
                    {
                        id = ydModulePower.id,
                        y_url = ydModulePower.y_url,
                        y_sort = ydModulePower.y_sort,
                        y_parentID = ydModulePower.y_parentID,
                        y_level = ydModulePower.y_level,
                        y_vaild = ydModulePower.y_vaild,
                        y_roleID = ydModulePower.y_roleID,
                        y_moduleID = ydModulePower.y_moduleID,
                        y_menu = ydModulePower.y_menu,
                        y_insert = ydModulePower.y_insert,
                        y_delete = ydModulePower.y_delete,
                        y_update = ydModulePower.y_update,
                        y_select = ydModulePower.y_select,
                        y_roleName = ydModulePower.y_roleName,
                        y_moudleName = ydModulePower.y_moudleName,
                        children = r.Find(u => u.Key == YdAdminRoleId)
                            .Where(u => u.y_menu == menu && u.y_parentID == ydModulePower.y_moduleID && u.y_level == 3)
                            .Select(k => new ModulePower
                            {
                                id = k.id,
                                y_url = k.y_url,
                                y_sort = k.y_sort,
                                y_parentID = k.y_parentID,
                                y_level = k.y_level,
                                y_vaild = k.y_vaild,
                                y_roleID = k.y_roleID,
                                y_moduleID = k.y_moduleID,
                                y_menu = k.y_menu,
                                y_insert = k.y_insert,
                                y_delete = k.y_delete,
                                y_update = k.y_update,
                                y_select = k.y_select,
                                y_roleName = k.y_roleName,
                                y_moudleName = k.y_moudleName,
                            }).OrderByDescending(u => u.y_sort).ToList()
                    }).OrderByDescending(u => u.y_sort).ToList();

                return modulePowers;
            }
        }

        /// <summary>
        /// 递归获取子集栏目权限
        /// </summary>
        /// <param name="powerModuleId"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public List<ModulePower> GetChildrenModulePower(int powerModuleId, IYunEntities yunEntities)
        {

            const int vaild = (int)DataState.Able;
            const int menu = (int)PowerState.Able;
            var modulePowerSons =
                yunEntities.VW_Power.Where(
                    u =>
                        u.y_parentID == powerModuleId && u.y_roleID == YdAdminRoleId && u.y_vaild == vaild &&
                        u.y_menu == menu).OrderByDescending(u => u.y_sort);
            var children = new List<ModulePower>();
            foreach (var modulePowerSon in modulePowerSons)
            {
                var modulePowerChildren = new ModulePower
                {
                    id = modulePowerSon.id,
                    y_url = modulePowerSon.y_url,
                    y_sort = modulePowerSon.y_sort,
                    y_parentID = modulePowerSon.y_parentID,
                    y_level = modulePowerSon.y_level,
                    y_vaild = modulePowerSon.y_vaild,
                    y_roleID = modulePowerSon.y_roleID,
                    y_moduleID = modulePowerSon.y_moduleID,
                    y_menu = modulePowerSon.y_menu,
                    y_insert = modulePowerSon.y_insert,
                    y_delete = modulePowerSon.y_delete,
                    y_update = modulePowerSon.y_update,
                    y_select = modulePowerSon.y_select,
                    y_roleName = modulePowerSon.y_roleName,
                    y_moudleName = modulePowerSon.y_moudleName,
                };
                children.Add(modulePowerChildren);
            }
            return children;
        }

        #region 根据输入专业库，专业层次，学习形式获取专业ID
        /// <summary>
        /// 根据输入专业库，专业层次，学习形式获取专业ID--只用于查询专业是否存在，不添加
        /// </summary>
        /// <param name="majorLib">专业库</param>
        /// <param name="eduType">专业层次</param>
        /// <param name="stuType">学习形式</param>
        /// <returns>专业ID</returns>
        public static int GetMajorIds(int majorLib, int eduType, int stuType)
        {
            using (var yunEntity = new IYunEntities())
            {
                if (!yunEntity.YD_Edu_MajorLibrary.Any(u => u.id == majorLib))
                {
                    return 0;
                }
                if (!yunEntity.YD_Edu_EduType.Any(u => u.id == eduType))
                {
                    return 0;
                }
                if (!yunEntity.YD_Edu_StuType.Any(u => u.id == stuType))
                {
                    return 0;
                }

                try
                {
                    var major = yunEntity.YD_Edu_Major.FirstOrDefault(
                                     u => u.y_eduTypeId == eduType && u.y_majorLibId == majorLib && u.y_stuTypeId == stuType);
                    if (major == null)
                    {
                        return 0; //调用添加方法
                    }
                    else
                    {
                        return major.id;
                    }
                }
                catch (Exception e)
                {
                    var cw = e.Message;
                    return 0;
                }

            }
        }
        /// <summary>
        /// 根据输入专业库，专业层次，学习形式获取专业ID
        /// </summary>
        /// <param name="majorLib">专业库</param>
        /// <param name="eduType">专业层次</param>
        /// <param name="stuType">学习形式</param>
        /// <returns>专业ID</returns>
        public static int GetMajorId(int majorLib, int eduType, int stuType)
        {
            using (var yunEntity = new IYunEntities())
            {
                if (!yunEntity.YD_Edu_MajorLibrary.Any(u => u.id == majorLib))
                {
                    return 0;
                }
                if (!yunEntity.YD_Edu_EduType.Any(u => u.id == eduType))
                {
                    return 0;
                }
                if (!yunEntity.YD_Edu_StuType.Any(u => u.id == stuType))
                {
                    return 0;
                }
                var major = yunEntity.YD_Edu_Major.FirstOrDefault(
                    u => u.y_eduTypeId == eduType && u.y_majorLibId == majorLib && u.y_stuTypeId == stuType);
                if (major == null)
                {
                    return AddMajor(majorLib, eduType, stuType); //调用添加方法
                }
                else
                {
                    return major.id;
                }
            }
        }

        #endregion

        #region 添加专业,调用此方法前确定三个参数都在数据库中存在

        /// <summary>
        /// 添加专业,调用此方法前确定三个参数都在数据库中存在
        /// </summary>
        /// <param name="majorLib">专业库</param>
        /// <param name="eduType">专业层次</param>
        /// <param name="stuType">学习形式</param>
        /// <returns>专业ID</returns>
        public static int AddMajor(int majorLib, int eduType, int stuType)
        {
            using (var yunEntity = new IYunEntities())
            {
                var major = new YD_Edu_Major();
                var edutypename = yunEntity.YD_Edu_EduType.FirstOrDefault(u => u.id == eduType);
                if (edutypename != null && edutypename.y_name == "高起专") //如果层次是高起专则学制为3
                {
                    major = new YD_Edu_Major
                    {
                        y_majorLibId = majorLib,
                        y_stuTypeId = stuType,
                        y_eduTypeId = eduType,
                        y_name = "",
                        y_code = "",
                        y_stuYear = 3
                    };
                }
                //如果是中医药大学则改变学制
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    if (edutypename != null && edutypename.y_name == "高起专") //如果层次是高起专则学制为4
                    {
                        major = new YD_Edu_Major
                        {
                            y_majorLibId = majorLib,
                            y_stuTypeId = stuType,
                            y_eduTypeId = eduType,
                            y_name = "",
                            y_code = "",
                            y_stuYear = 4
                        };
                    }
                }
                if (edutypename != null && edutypename.y_name == "高起本") //如果层次是高起本则学制为5
                {
                    major = new YD_Edu_Major
                    {
                        y_majorLibId = majorLib,
                        y_stuTypeId = stuType,
                        y_eduTypeId = eduType,
                        y_name = "",
                        y_code = "",
                        y_stuYear = 5
                    };
                }
                if (edutypename != null && edutypename.y_name == "专升本") //如果层次是专升本则学制为3
                {
                    major = new YD_Edu_Major
                    {
                        y_majorLibId = majorLib,
                        y_stuTypeId = stuType,
                        y_eduTypeId = eduType,
                        y_name = "",
                        y_code = "",
                        y_stuYear = 3
                    };
                }

                yunEntity.Entry(major).State = EntityState.Added;
                yunEntity.SaveChanges();
                major.y_code = major.id.ToString();
                var ydEduMajorLibrary = yunEntity.YD_Edu_MajorLibrary.Find(majorLib);
                if (ydEduMajorLibrary != null)
                {
                    var majorname = ydEduMajorLibrary.y_name;
                    //如果是中医药大学则改变学制
                    if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    {
                        //如果专业为医药营销则学制为3
                        if (majorname == "医药营销")
                        {
                            major.y_stuYear = 3;
                            yunEntity.Entry(major).State = EntityState.Modified;
                        }
                    }
                    var ydEduEduType = yunEntity.YD_Edu_EduType.Find(eduType);
                    if (ydEduEduType != null)
                    {
                        var eduTypename = ydEduEduType.y_name;
                        var ydEduStuType = yunEntity.YD_Edu_StuType.Find(stuType);
                        if (ydEduStuType != null)
                        {
                            var stuTypename = ydEduStuType.y_name;
                            major.y_name = majorname + " " + eduTypename + " " + stuTypename;
                        }
                    }
                }

                yunEntity.SaveChanges();
                return major.id;
            }
        }

        #endregion

        public static bool IsAdminSubLink(int adminId, int subSchId)
        {
            using (var yunEntities = new IYunEntities())
            {
                return yunEntities.YD_Sys_AdminSubLink.Any(u => u.y_adminId == adminId && u.y_subSchoolId == subSchId);
            }
        }
        //public static bool IsAdminMajorLink(int adminId, int majorId)
        //{
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        return yunEntities.YD_Sys_AdminMajorLink.Any(u => u.y_adminId == adminId && u.y_majorId == majorId);
        //    }
        //}
        public static bool IsAdminCourseLink(int adminId, int courseId)
        {
            using (var yunEntities = new IYunEntities())
            {
                return yunEntities.YD_Sys_AdminCourseLink.Any(u => u.y_adminId == adminId && u.y_courseId == courseId);
            }
        }

        /// <summary>
        /// 获取登录ID
        /// </summary>
        public YD_Sys_Admin CurrentAdmin
        {
            get
            {
                var sessionAdminIdStr = Session[KeyValue.Admin_id];
                int sessionAdminId = 0;
                if (sessionAdminIdStr != null)
                {
                    sessionAdminId = Convert.ToInt32(sessionAdminIdStr);
                }
                var db = new IYunEntities();
                var admin = db.YD_Sys_Admin.FirstOrDefault(a => a.id == sessionAdminId);
                db.Dispose();
                return admin;
            }
        }

        public ActionResult ModuleAction(int id)
        {
            IsLogin();

            using (var ad = new IYunEntities())
            {
                var modules =
                    ad.VW_Power
                    .Where(u => u.y_roleID == YdAdminRoleId
                && u.y_level == 2
                && u.y_menu == 1
                && u.y_vaild == 1
                && u.y_parentID == id)
                .OrderByDescending(u => u.y_sort).FirstOrDefault();

                if (modules != null)
                {
                    var child = ad.VW_Power.Where(
                        u => u.y_level == 3
                        && u.y_menu == 1
                        && u.y_roleID == YdAdminRoleId
                        && u.y_vaild == 1
                        && u.y_parentID == modules.y_moduleID)
                        .OrderByDescending(u => u.y_sort).FirstOrDefault();

                    if (child != null)
                    {
                        return Redirect(child.y_url);
                    }
                    else
                    {
                        return Redirect(modules.y_url);
                    }
                }
            }

            return Content("没有权限查看其栏目");
        }
    }
}
