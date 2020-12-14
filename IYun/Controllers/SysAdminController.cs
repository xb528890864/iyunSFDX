using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Dal;
using IYun.Models;
using IYun.Object;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Renci.SshNet;
using Terminal_CDMA;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    /// <summary>
    /// 系统设置 、成绩同步
    /// </summary>
    public class SysAdminController : AdminBaseController
    {
        private readonly YD_Sys_ModuleDal moduleDal = new YD_Sys_ModuleDal();
        private readonly YD_Sys_DbLogTypeDal dbLogTypeDal = new YD_Sys_DbLogTypeDal();
        private readonly YD_Sys_DbLogDal dbLogDal = new YD_Sys_DbLogDal();
        private readonly YD_Sys_RoleDal roleDal = new YD_Sys_RoleDal();
        private readonly YD_Sys_AdminDal adminDal = new YD_Sys_AdminDal();
        private readonly VW_DbLogDal vwDbLogDal = new VW_DbLogDal();
        private readonly VW_AdminDal vwAdminDal = new VW_AdminDal();
        private YD_Sys_SubSchoolDal subSchoolDal = new YD_Sys_SubSchoolDal();
        private YD_Edu_StuTypeDal _stuTypeDal = new YD_Edu_StuTypeDal();
        private YD_Edu_EduTypeDal _eduEduTypeDal = new YD_Edu_EduTypeDal();
        private YD_Sts_PoliticsDal _politicsDal = new YD_Sts_PoliticsDal();
        private YD_Sts_NationDal _nationDal = new YD_Sts_NationDal();
        private YD_Edu_CourseTypeDal _courseTypeDal = new YD_Edu_CourseTypeDal();
        private YD_Edu_StuStateDal _stuStateDal = new YD_Edu_StuStateDal();


        #region 页面管理

        /// <summary>
        /// 栏目管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Module()
        {
            #region “栏目管理”权限验证

            var power = SafePowerPage("/SysAdmin/Module");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var parentList =
                    yunEntities.YD_Sys_Module.Where(u => u.y_level == 1).OrderByDescending(u => u.y_sort).ToList();
                var modules = parentList.Select(ydModulePower => new Module
                {
                    id = ydModulePower.id,
                    y_url = ydModulePower.y_url,
                    y_parentID = ydModulePower.y_parentID,
                    y_level = ydModulePower.y_level,
                    y_sort = ydModulePower.y_sort,
                    y_vaild = ydModulePower.y_vaild,
                    y_name = ydModulePower.y_name,
                    children = GetChildrenModule(ydModulePower, yunEntities)
                }).ToList();
                ViewBag.modules = modules;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            ViewBag.power = power;
            return View();
        }

        /// <summary>
        /// 递归获取子集栏目
        /// </summary>
        /// <param name="power"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public List<Module> GetChildrenModule(YD_Sys_Module power, IYunEntities yunEntities)
        {
            var modulePowerSons =
                yunEntities.YD_Sys_Module.Where(u => u.y_parentID == power.id).OrderByDescending(u => u.y_sort);
            var children = new List<Module>();
            foreach (var modulePowerSon in modulePowerSons)
            {
                var modulePowerChildren = new Module
                {
                    id = modulePowerSon.id,
                    y_url = modulePowerSon.y_url,
                    y_parentID = modulePowerSon.y_parentID,
                    y_level = modulePowerSon.y_level,
                    y_sort = modulePowerSon.y_sort,
                    y_vaild = modulePowerSon.y_vaild,
                    y_name = modulePowerSon.y_name,
                    children = GetChildrenModule(modulePowerSon, yunEntities)
                };
                //var modulePowerSonSon = GetChildrenModule(modulePowerSon, yunEntities);
                //modulePowerChildren.children = modulePowerSonSon;
                children.Add(modulePowerChildren);
            }
            return children;
        }



        /// <summary>
        /// 角色管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Role()
        {
            #region “角色管理”权限验证

            var power = SafePowerPage("/SysAdmin/Role");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.roleList = yunEntities.YD_Sys_Role.Where(u => true).ToList();
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        ///重新获取栏目缓存
        public void InitPower()
        {
            PowerInit.InitPower();
        }

        /// <summary>
        /// 用户管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin(int id = 1)
        {
            #region “用户管理”权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yName = Request["y_name"];
                var yRealName = Request["y_realName"];
                var yRoleId = Request["y_roleID"];

                IQueryable<VW_Admin> list = yunEntities.VW_Admin.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(yName))
                {
                    list = list.Where(u => u.y_adminName.Contains(yName));
                }
                if (!string.IsNullOrWhiteSpace(yRealName))
                {
                    list = list.Where(u => u.y_realName.Contains(yRealName));
                }

                if (!string.IsNullOrWhiteSpace(yRoleId) && !yRoleId.Equals("0"))
                {
                    var yRoleIdn = Convert.ToInt32(yRoleId);
                    list = list.Where(u => u.y_roleID == yRoleIdn);
                }
                ViewBag.admin = yunEntities.VW_Admin.FirstOrDefault(u => u.id == YdAdminId);
                var adminList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("AdminList", adminList);
                return View(adminList);
            }

        }
        /// <summary>
        ///日志管理下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownAdmin()
        {
            #region “用户管理”权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var yName = Request["y_name"];
                var yRealName = Request["y_realName"];
                var yRoleId = Request["y_roleID"];

                IQueryable<VW_Admin> list = yunEntities.VW_Admin.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(yName))
                {
                    list = list.Where(u => u.y_adminName.Contains(yName));
                }
                if (!string.IsNullOrWhiteSpace(yRealName))
                {
                    list = list.Where(u => u.y_realName.Contains(yRealName));
                }

                if (!string.IsNullOrWhiteSpace(yRoleId) && !yRoleId.Equals("0"))
                {
                    var yRoleIdn = Convert.ToInt32(yRoleId);
                    list = list.Where(u => u.y_roleID == yRoleIdn);
                }
                var models = list.ToList();
                var listss = models.Select(
                    u =>
                        new
                        {
                            y_adminName = u.y_adminName,
                            y_roleName = u.y_roleName,
                            y_realName = u.y_realName,
                            y_phone = u.y_phone
                        }).ToList();
                DataTable model = FileHelper.ToDataTable(listss);
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + "用户表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_adminName", "登录名"},
                        {"y_roleName", "角色名"},
                        {"y_realName", "姓名"},
                        {"y_phone", "手机"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("alert('错误');window.location.href='" + reurl + "';");
                }
            }
        }
        /// <summary>
        /// 用户函授站页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminSubLinkPage(int id = 1)
        {
            #region “用户管理”权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                var nameStr = Request["y_name"];
                if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    var name = nameStr.Trim().Replace(" ", "").Replace("函授站", "").Replace("函授", "");
                    List<YD_Sys_SubSchool> entityList;
                    if (name.Length <= 2)
                    {
                        entityList = yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(name)).ToList();
                    }
                    else
                    {
                        var sql = "";
                        for (var i = 0; i < name.Length - 1; i++)
                        {
                            sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
                            if (i + 1 < name.Length - 1)
                            {
                                sql += " or ";
                            }
                        }
                        entityList =
                            yunEntities.Database.SqlQuery<YD_Sys_SubSchool>("select * from YD_Sys_SubSchool where " +
                                                                            sql).ToList();
                    }
                    //id为pageindex   15 为pagesize;
                    var adminIdStr = Request["adminId"];
                    if (string.IsNullOrWhiteSpace(adminIdStr))
                    {
                        return Content("参数错误");
                    }
                    try
                    {
                        int adminId = Convert.ToInt32(adminIdStr);
                        ViewData["adminId"] = adminId;
                        var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
                        if (admin != null)
                        {
                            ViewBag.admin = admin;

                            var ids = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == adminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();

                            ViewData["ids"] = ids;

                            var dbLogList =
                                entityList.OrderByDescending(u => ids.Contains(u.id))
                                    .ThenBy(u => u.y_name)
                                    .ThenByDescending(u => u.id)
                                    .ToPagedList(id, 15);

                            if (Request.IsAjaxRequest())
                                return PartialView("AdminSubLinkList", dbLogList);
                            return View(dbLogList);
                        }
                        else
                        {
                            return Content("参数错误");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("参数错误");
                    }
                }
                else
                {

                    var adminIdStr = Request["adminId"];
                    if (string.IsNullOrWhiteSpace(adminIdStr))
                    {
                        return Content("参数错误");
                    }
                    try
                    {
                        int adminId = Convert.ToInt32(adminIdStr);
                        ViewData["adminId"] = adminId;



                        var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
                        if (admin != null)
                        {
                            ViewBag.admin = admin;

                            var ids = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == adminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();

                            ViewData["ids"] = ids;

                            var sql = yunEntities.YD_Sys_SubSchool.Where(u => true)
                                .OrderByDescending(u => ids.Contains(u.id))
                                .ThenBy(u => u.y_name)
                                .ThenByDescending(u => u.id).AsQueryable();

                            var dbLogList = sql.ToPagedList(id, 15); //id为pageindex   15 为pagesize;

                            if (Request.IsAjaxRequest())
                                return PartialView("AdminSubLinkList", dbLogList);
                            return View(dbLogList);
                        }
                        else
                        {
                            return Content("参数错误");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("参数错误");
                    }
                }
            }
        }

        /// <summary>
        /// 用户课程页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminCourseLinkPage(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                var nameStr = Request["y_name"];
                var entityList = new List<YD_Edu_Course>();
                if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    var name = nameStr.Trim().Replace(" ", "");
                    if (name.Length <= 2)
                    {
                        entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name)).ToList();
                    }
                    else
                    {
                        var sql = "";
                        for (var i = 0; i < name.Length - 1; i++)
                        {
                            sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
                            if (i + 1 < name.Length - 1)
                            {
                                sql += " or ";
                            }
                        }
                        entityList =
                            yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " + sql)
                                .ToList();
                    }

                    var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
                    //id为pageindex   15 为pagesize;
                    var adminIdStr = Request["adminId"];
                    if (string.IsNullOrWhiteSpace(adminIdStr))
                    {
                        return Content("参数错误");
                    }
                    try
                    {
                        int adminId = Convert.ToInt32(adminIdStr);
                        ViewData["adminId"] = adminId;
                        var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
                        if (admin != null)
                        {
                            ViewBag.admin = admin;
                            if (Request.IsAjaxRequest())
                                return PartialView("AdminCourseLinkList", dbLogList);
                            return View(dbLogList);
                        }
                        else
                        {
                            return Content("参数错误");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("参数错误");
                    }

                }
                else
                {
                    var dbLogList =
                        yunEntities.YD_Edu_Course.Where(u => true)
                            .OrderBy(u => u.y_name)
                            .ThenByDescending(u => u.id)
                            .ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                    var adminIdStr = Request["adminId"];
                    if (string.IsNullOrWhiteSpace(adminIdStr))
                    {
                        return Content("参数错误");
                    }
                    try
                    {
                        int adminId = Convert.ToInt32(adminIdStr);
                        ViewData["adminId"] = adminId;
                        var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
                        if (admin != null)
                        {
                            ViewBag.admin = admin;
                            if (Request.IsAjaxRequest())
                                return PartialView("AdminCourseLinkList", dbLogList);
                            return View(dbLogList);
                        }
                        else
                        {
                            return Content("参数错误");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("参数错误");
                    }
                }


            }

        }

        /// <summary>
        /// 用户专业页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult AdminMajorLinkPage(int id = 1)
        //{
        //    #region “用户管理”权限验证

        //    var power = SafePowerPage("/SysAdmin/Admin");
        //    if (!IsLogin())
        //    {
        //        return Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int) PowerState.Disable)
        //    {
        //        return Content("没有权限");
        //    }

        //    #endregion

        //    ViewBag.power = power;
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
        //        var nameStr = Request["y_name"];
        //        if (!string.IsNullOrWhiteSpace(nameStr))
        //        {
        //            var name = nameStr.Trim().Replace(" ", "").Replace("专业", "").Replace("专业", "");
        //            List<YD_Edu_Major> entityList;
        //            if (name.Length <= 2)
        //            {
        //                entityList = yunEntities.YD_Edu_Major
        //                    .Include(u => u.YD_Edu_MajorLibrary)
        //                    .Include(u => u.YD_Edu_StuType)
        //                    .Include(u => u.YD_Edu_EduType).
        //                    Where(u => u.y_name.Contains(name)).ToList();
        //            }
        //            else
        //            {
        //                var sql = "";
        //                for (var i = 0; i < name.Length - 1; i++)
        //                {
        //                    sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
        //                    if (i + 1 < name.Length - 1)
        //                    {



        //                        sql += " or ";
        //                    }
        //                }
        //                entityList =
        //                    yunEntities.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + sql)
        //                        .ToList();
        //            }
        //            var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
        //            //id为pageindex   15 为pagesize;
        //            var adminIdStr = Request["adminId"];
        //            if (string.IsNullOrWhiteSpace(adminIdStr))
        //            {
        //                return Content("参数错误");
        //            }
        //            try
        //            {
        //                int adminId = Convert.ToInt32(adminIdStr);
        //                ViewData["adminId"] = adminId;
        //                var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
        //                if (admin != null)
        //                {
        //                    ViewBag.admin = admin;
        //                    if (Request.IsAjaxRequest())
        //                        return PartialView("AdminMajorLinkList", dbLogList);
        //                    return View(dbLogList);
        //                }
        //                else
        //                {
        //                    return Content("参数错误");
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                return Content("参数错误");
        //            }

        //        }
        //        else
        //        {
        //            var dbLogList = yunEntities.YD_Edu_Major.Include(u => u.YD_Edu_MajorLibrary)
        //                .Include(u => u.YD_Edu_StuType)
        //                .Include(u => u.YD_Edu_EduType)
        //                .Where(u => true).OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
        //            //id为pageindex   15 为pagesize;
        //            var adminIdStr = Request["adminId"];
        //            if (string.IsNullOrWhiteSpace(adminIdStr))
        //            {
        //                return Content("参数错误");
        //            }
        //            try
        //            {
        //                int adminId = Convert.ToInt32(adminIdStr);
        //                ViewData["adminId"] = adminId;
        //                var admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == adminId);
        //                if (admin != null)
        //                {
        //                    ViewBag.admin = admin;
        //                    if (Request.IsAjaxRequest())
        //                        return PartialView("AdminMajorLinkList", dbLogList);
        //                    return View(dbLogList);
        //                }
        //                else
        //                {
        //                    return Content("参数错误");
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                return Content("参数错误");
        //            }
        //        }
        //    }

        //}

        /// <summary>
        /// 更新用户函授站关联
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateAdminSubLink()
        {
            var yesOrNos = Request["yesOrNos"].Split(new[] { "<>" }, StringSplitOptions.None);
            var subIds = Request["subIds"].Split(new[] { "<>" }, StringSplitOptions.None);
            var adminId = Convert.ToInt32(Request["adminId"]);
            using (var yunEntities = new IYunEntities())
            {
                var subId = 0;
                var adminSub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(
                    u => u.y_adminId == adminId);
                if (adminSub != null)
                {
                    return Content("该账号已指定函授站");
                }
                for (var i = 0; i < subIds.Count(); i++)
                {
                    subId = Convert.ToInt32(subIds[i]);
                    var adminSubLink = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(
                        u => u.y_adminId == adminId && u.y_subSchoolId == subId);
                    if (adminSubLink != null)
                    {
                        if (Convert.ToInt32(yesOrNos[i]) == 1)
                        {
                            yunEntities.Entry(adminSubLink).State = EntityState.Deleted;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(yesOrNos[i]) != 0) continue;
                        var adminSubLink2 = new YD_Sys_AdminSubLink()
                        {
                            y_adminId = adminId,
                            y_subSchoolId = subId
                        };
                        yunEntities.Entry(adminSubLink2).State = EntityState.Added;
                    }
                }
                var t = yunEntities.SaveChanges();
                return Content("ok");
            }
        }

        /// <summary>
        /// 更新用户专业关联
        /// </summary>
        /// <returns></returns>
        //public ActionResult UpdateAdminMajorLink()
        //{
        //    var yesOrNos = Request["yesOrNos"].Split(new[] {"<>"}, StringSplitOptions.None);
        //    var majors = Request["majors"].Split(new[] {"<>"}, StringSplitOptions.None);
        //    var adminId = Convert.ToInt32(Request["adminId"]);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var majorId = 0;
        //        var adminSub = yunEntities.YD_Sys_AdminMajorLink.FirstOrDefault(
        //            u => u.y_adminId == adminId);
        //        if (adminSub != null)
        //        {
        //            return Content("该账号已指定专业");
        //        }
        //        for (var i = 0; i < majors.Count(); i++)
        //        {
        //            majorId = Convert.ToInt32(majors[i]);
        //            var adminSubLink = yunEntities.YD_Sys_AdminMajorLink.FirstOrDefault(
        //                u => u.y_adminId == adminId && u.y_majorId == majorId);
        //            if (adminSubLink != null)
        //            {
        //                if (Convert.ToInt32(yesOrNos[i]) == 1)
        //                {
        //                    yunEntities.Entry(adminSubLink).State = EntityState.Deleted;
        //                }
        //            }
        //            else
        //            {
        //                if (Convert.ToInt32(yesOrNos[i]) != 0) continue;
        //                var adminSubLink2 = new YD_Sys_AdminSubLink()
        //                {
        //                    y_adminId = adminId,
        //                    y_subSchoolId = majorId
        //                };
        //                yunEntities.Entry(adminSubLink2).State = EntityState.Added;
        //            }
        //        }
        //        var t = yunEntities.SaveChanges();
        //        return Content("ok");
        //    }
        //}

        /// <summary>
        /// 更新用户课程关联
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateAdminCourseLink()
        {
            var yesOrNos = Request["yesOrNos"].Split(new[] { "<>" }, StringSplitOptions.None);
            var subIds = Request["subIds"].Split(new[] { "<>" }, StringSplitOptions.None);
            var adminId = Convert.ToInt32(Request["adminId"]);
            using (var yunEntities = new IYunEntities())
            {
                var subId = 0;
                for (var i = 0; i < subIds.Count(); i++)
                {
                    subId = Convert.ToInt32(subIds[i]);
                    var adminSubLink = yunEntities.YD_Sys_AdminCourseLink.FirstOrDefault(
                        u => u.y_adminId == adminId && u.y_courseId == subId);
                    if (adminSubLink != null)
                    {
                        if (Convert.ToInt32(yesOrNos[i]) == 1)
                        {
                            yunEntities.Entry(adminSubLink).State = EntityState.Deleted;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(yesOrNos[i]) != 0) continue;
                        var adminSubLink2 = new YD_Sys_AdminCourseLink()
                        {
                            y_adminId = adminId,
                            y_courseId = subId
                        };
                        yunEntities.Entry(adminSubLink2).State = EntityState.Added;
                    }

                }
                var t = yunEntities.SaveChanges();
                return Content("ok");
            }
        }

        /// <summary>
        /// 添加栏目页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleAddPage(int id)
        {
            #region “添加栏目”权限验证

            var power = SafePowerPage("/SysAdmin/Module");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            ViewBag.parentId = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑栏目视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleEditPage(int id)
        {
            #region “编辑栏目”权限验证

            var power = SafePowerPage("/SysAdmin/Module");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.module = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 权限管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Power()
        {
            #region “权限管理”权限验证

            var power = SafePowerPage("/SysAdmin/Power");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            return View();
        }

        /// <summary>
        /// 日志管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DbLog(int id = 1)
        {
            #region “日志管理”权限验证

            var power = SafePowerPage("/SysAdmin/DbLog");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var yUserName = Request["Y_userName"];
                var yLogTypeId = Request["Y_logTypeID"];
                var startTime = Request["startTime"];
                var endTime = Request["endTime"];

                IQueryable<VW_DbLog> list = yunEntities.VW_DbLog.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(yUserName))
                {
                    list = list.Where(u => u.y_userName.Contains(yUserName));
                }

                if (!string.IsNullOrWhiteSpace(yLogTypeId) && !yLogTypeId.Equals("0"))
                {
                    var y_logTypeID = Convert.ToInt32(yLogTypeId);
                    list = list.Where(u => u.y_logTypeID == y_logTypeID);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    var startTimeN = Convert.ToDateTime(startTime);
                    list = list.Where(u => u.y_time >= startTimeN);
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    var endTimeN = Convert.ToDateTime(endTime);
                    list = list.Where(u => u.y_time <= endTimeN);
                }

                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("DbLogList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        ///日志管理下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownDbLog()
        {
            #region “日志管理”权限验证

            var power = SafePowerPage("/SysAdmin/DbLog");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var yUserName = Request["Y_userName"];
                var yLogTypeId = Request["Y_logTypeID"];
                var startTime = Request["startTime"];
                var endTime = Request["endTime"];

                IQueryable<VW_DbLog> list = yunEntities.VW_DbLog.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(yUserName))
                {
                    list = list.Where(u => u.y_userName.Contains(yUserName));
                }

                if (!string.IsNullOrWhiteSpace(yLogTypeId) && !yLogTypeId.Equals("0"))
                {
                    var y_logTypeID = Convert.ToInt32(yLogTypeId);
                    list = list.Where(u => u.y_logTypeID == y_logTypeID);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    var startTimeN = Convert.ToDateTime(startTime);
                    list = list.Where(u => u.y_time >= startTimeN);
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    var endTimeN = Convert.ToDateTime(endTime);
                    list = list.Where(u => u.y_time <= endTimeN);
                }
                var models = list.ToList();
                var listss = models.Select(
                    u =>
                        new
                        {
                            y_adminName = u.y_adminName,
                            y_logType = u.y_logType,
                            y_time = u.y_time,
                            y_remark = u.y_remark
                        }).ToList();
                DataTable model = FileHelper.ToDataTable(listss);
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + "日志表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_adminName", "操作员"},
                        {"y_logType", "操作类型"},
                        {"y_time", "操作时间"},
                        {"y_remark", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("alert('错误');window.location.href='" + reurl + "';");
                }
            }
        }
        #endregion

        #region 栏目模块

        /// <summary>
        /// 获取栏目表数据
        /// </summary>
        /// <returns></returns>
        public string ModuleList()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return moduleDal.ModuleList(yunEntities);
            }
        }

        /// <summary>
        /// 获取下拉树栏目表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ModuleDropList()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return moduleDal.ModuleDropList(yunEntities);
            }
        }

        /// <summary>
        /// 获取栏目表id
        /// </summary>
        /// <returns></returns>
        public JsonResult ModuleIdsList()
        {
            using (var yunEntities = new IYunEntities())
            {
                return moduleDal.ModuleIdList(yunEntities);
            }
        }

        /// <summary>
        /// 添加栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleAdd(YD_Sys_Module module)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.AddModule(module, Request, yunEntities));
            }
        }


        /// <summary>
        /// 修改栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleEdit(YD_Sys_Module module)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.EditModule(module, Request, yunEntities));
            }
        }

        /// <summary>
        /// 移动栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleMove()
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.MoveModule(Request, yunEntities));
            }
        }

        /// <summary>
        /// 修改栏目排序
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleUpdateSort()
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.ModuleUpdateSort(Request, yunEntities));
            }
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.ModuleDelete(id, yunEntities));
            }
        }

        /// <summary>
        /// 关闭栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleClose(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.ModuleClose(id, yunEntities));
            }

        }

        /// <summary>
        /// 开启栏目
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleOpen(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(moduleDal.ModuleOpen(id, yunEntities));
            }
        }

        #endregion


        #region 日志模块

        /// <summary>
        /// 获得日志类型集合
        /// </summary>
        /// <returns></returns>
        public string DbLogTypeList()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return dbLogTypeDal.DbLogTypeList(yunEntities);
            }
        }

        /// <summary>
        /// 获得分页日志记录集合
        /// </summary>
        /// <returns></returns>
        public string DbLogListPage()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return vwDbLogDal.DbLogPageList(Request, yunEntities);
            }

        }

        #endregion


        #region 角色模块

        /// <summary>
        /// 返回角色数据带分页
        /// </summary>
        /// <returns></returns>
        public string RoleListPage()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return roleDal.RolePageList(Request, yunEntities);
            }
        }

        /// <summary>
        /// 返回角色数据不带分页
        /// </summary>
        /// <returns></returns>
        public string RoleList()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return roleDal.RoleList(Request, yunEntities);
            }
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleEdit(YD_Sys_Role role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(roleDal.EditRole(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加角色
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleAdd(YD_Sys_Role role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(roleDal.AddRole(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleDelete()
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(roleDal.DeleteRole(Request, yunEntities));
            }
        }

        /// <summary>
        /// 角色编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleEditPage(int id)
        {
            #region “编辑栏目”权限验证

            var power = SafePowerPage("/SysAdmin/Role");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.role = yunEntities.YD_Sys_Role.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 角色添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleAddPage()
        {
            #region “添加栏目”权限验证

            var power = SafePowerPage("/SysAdmin/Module");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        #endregion


        #region 用户模块

        /// <summary>
        /// 返回用户数据带分页
        /// </summary>
        /// <returns></returns>
        public string AdminListPage()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return vwAdminDal.AdminPageList(Request, yunEntities);
            }
        }

        /// <summary>
        /// 返回用户数据不带分页
        /// </summary>
        /// <returns></returns>
        public string AdminList()
        {
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ProxyCreationEnabled = false;
                return vwAdminDal.AdminList(yunEntities);
            }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminEdit(YD_Sys_Admin admin)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(adminDal.EditAdmin(admin, yunEntities));
            }
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <returns></returns>
        public string AdminUpdatePwd()
        {
            using (var yunEntities = new IYunEntities())
            {
                var resultInfo = adminDal.UpdatePwd(Request, yunEntities);
                //if (resultInfo.Success)
                //{
                //    Exit();
                //}
                return resultInfo.Success ? "ok" : resultInfo.Message;
            }
        }


        /// <summary>
        /// 增加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminAdd(YD_Sys_Admin admin)
        {
            using (var yunEntities = new IYunEntities())
            {
                var subid = Request["subschool"];
                var r = adminDal.AddAdmin(admin, yunEntities);
                if (r == "ok")
                {
                    if (!string.IsNullOrEmpty(subid) && !subid.Equals("0"))
                    {
                        var sub = Convert.ToInt32(subid);
                        var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == sub);
                        if (subschool != null)
                        {
                            var adminSubLink2 = new YD_Sys_AdminSubLink()
                            {
                                y_adminId = admin.id,
                                y_subSchoolId = subschool.id
                            };
                            yunEntities.Entry(adminSubLink2).State = EntityState.Added;
                        }
                        yunEntities.SaveChanges();
                        return Content("ok");
                    }
                    else
                    {
                        return Content("ok");
                    }

                }
                else
                {
                    return Content(r);
                }
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminDelete()
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(adminDal.DeleteAdmin(Request, yunEntities));
            }
        }

        /// <summary>
        /// 返回用户编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminEditPage(int id)
        {
            #region “编辑用户”权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                ViewBag.admin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == id);
            }
            return View();
        }

        /// <summary>
        /// 返回用户添加视图
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminAddPage()
        {
            #region “添加用户”权限验证

            var power = SafePowerPage("/SysAdmin/Admin");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        #endregion


        #region 权限模块

        /// <summary>
        /// 进入权限管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PowerAdmin(int id)
        {
            #region “修改”权限验证

            var power = SafePowerPage("/SysAdmin/Role");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.id = id;
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                var parentList =
                    yunEntities.YD_Sys_Module.Where(u => u.y_level == 1).OrderByDescending(u => u.y_sort).ToList();
                var modulePowerPars =
                    parentList.Select(ydModule => GetModulePowerPars(id, ydModule, yunEntities)).ToList();
                ViewBag.modulePowerPars = modulePowerPars;
            }
            return View();
        }

        /// <summary>
        /// 返回某角色的权限表数据
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        public ActionResult GetPowerData(int id)
        {
            if (!IsLogin()) return RedirectToAction("Index", "AdminBase");
            var res = new JsonResult();
            using (var yunEntities = new IYunEntities())
            {
                var parentList =
                    yunEntities.YD_Sys_Module.Where(u => u.y_level == 1).OrderByDescending(u => u.y_sort).ToList();
                var modulePowerPars =
                    parentList.Select(ydModule => GetModulePowerPars(id, ydModule, yunEntities)).ToList();
                res.Data = modulePowerPars;
                return res;
            }
        }

        /// <summary>
        /// 递归获得当前栏目及其下所有子栏目的栏目权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="module"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public ModulePowerPar GetModulePowerPars(int id, YD_Sys_Module module, IYunEntities yunEntities)
        {
            var sonModules =
                yunEntities.YD_Sys_Module.Where(u => u.y_parentID == module.id)
                    .OrderByDescending(u => u.y_sort)
                    .ToList();
            var modulePowerSons =
                sonModules.Select(sonModule => GetModulePowerPars(id, sonModule, yunEntities)).ToList();
            var modulePowerPar = new ModulePowerPar
            {
                id = module.id,
                y_vaild = module.y_vaild,
                y_level = module.y_level,
                y_name = module.y_name,
                y_parentID = module.y_parentID,
                y_sort = module.y_sort,
                y_url = module.y_url
            };
            var power = yunEntities.YD_Sys_Power.FirstOrDefault(u => u.y_moduleID == module.id && u.y_roleID == id);
            //获得同一个栏目中，当前登录的用户的权限对象
            var operPowerF =
                yunEntities.YD_Sys_Power.FirstOrDefault(u => u.y_moduleID == module.id && u.y_roleID == YdAdminRoleId);
            if (power == null)
            {
                modulePowerPar.y_insert = (Int32)PowerState.Disable;
                modulePowerPar.y_menu = (Int32)PowerState.Disable;
                modulePowerPar.y_delete = (Int32)PowerState.Disable;
                modulePowerPar.y_update = (Int32)PowerState.Disable;
                modulePowerPar.y_select = (Int32)PowerState.Disable;
                modulePowerPar.children = modulePowerSons;
            }
            else
            {
                modulePowerPar.y_insert = power.y_insert;
                modulePowerPar.y_menu = power.y_menu;
                modulePowerPar.y_delete = power.y_delete;
                modulePowerPar.y_update = power.y_update;
                modulePowerPar.y_select = power.y_select;
                modulePowerPar.children = modulePowerSons;
            }
            if (operPowerF == null)
            {
                modulePowerPar.y_insert_F = (Int32)PowerState.Disable;
                modulePowerPar.y_menu_F = (Int32)PowerState.Disable;
                modulePowerPar.y_delete_F = (Int32)PowerState.Disable;
                modulePowerPar.y_update_F = (Int32)PowerState.Disable;
                modulePowerPar.y_select_F = (Int32)PowerState.Disable;
            }
            else
            {
                modulePowerPar.y_insert_F = operPowerF.y_insert;
                modulePowerPar.y_menu_F = operPowerF.y_menu;
                modulePowerPar.y_delete_F = operPowerF.y_delete;
                modulePowerPar.y_update_F = operPowerF.y_update;
                modulePowerPar.y_select_F = operPowerF.y_select;
            }
            return modulePowerPar;
        }

        /// <summary>
        /// 处理权限
        /// </summary>
        /// <returns></returns>
        public ActionResult OperPower()
        {
            var powerInfo = Safe("/SysAdmin/Role", PowerFlag.Update);
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    return Content("无对应权限");
                case PowerInfo.NoLogin:
                    return Content("未登录");
                case PowerInfo.Unknow:
                    return Content("未知错误");
                case PowerInfo.HasPower:
                    break;
                default:
                    return Content("未知错误");
            }
            var powerStr = Request["power"];
            var powers = JsonConvert.DeserializeObject<List<YD_Sys_Power>>(powerStr);
            using (var yunEntities = new IYunEntities())
            {
                if (powers.Count > 0)
                {
                    var roleid = powers[0].y_roleID;
                    var oldPowers = yunEntities.YD_Sys_Power.Where(u => u.y_roleID == roleid).ToList();
                    foreach (var oldPower in oldPowers)
                    {
                        yunEntities.Entry(oldPower).State = EntityState.Deleted;
                    }
                }
                foreach (var power in powers)
                {
                    yunEntities.Entry(power).State = EntityState.Added;
                }
                var t = yunEntities.SaveChanges();
                return Content(t > 0 ? "ok" : "修改权限失败");
            }
        }

        #endregion

        #region 函授站

        public ActionResult SubSchool(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var subschool = Request["SubSchool"];
                ViewBag.adminid = 0;

                IQueryable<VW_SubschoolAdmin> list = yunEntities.VW_SubschoolAdmin.OrderByDescending(u => u.id).AsQueryable();
                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.id == subschoolint);
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("SubSchoolList", dbLogList);
                return View(dbLogList);
                //if (!string.IsNullOrWhiteSpace(nameStr))
                //{
                //    var name = nameStr.Trim().Replace(" ", "").Replace("函授站", "").Replace("函授", "");
                //    List<VW_SubschoolAdmin> entityList;
                //if (name.Length <= 2)
                //{
                //entityList = yunEntities.VW_SubschoolAdmin.Where(u => u.y_name.Contains(name)).ToList();
                //}
                //else
                //{
                //    var sql = "";
                //    for (var i = 0; i < name.Length - 1; i++)
                //    {
                //        sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
                //        if (i + 1 < name.Length - 1)
                //        {
                //            sql += " or ";
                //        }
                //    }
                //    entityList =yunEntities.Database.SqlQuery<VW_SubschoolAdmin>("select * from VW_SubschoolAdmin where " +sql).ToList();
                //}

                //    var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
                //    //id为pageindex   15 为pagesize;
                //    ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目

                //    ViewBag.adminid = YdAdminId;
                //    if (Request.IsAjaxRequest())
                //        return PartialView("SubSchoolList", dbLogList);
                //    return View(dbLogList);

                //}
                //else
                //{
                //    var dbLogList =
                //        yunEntities.VW_SubschoolAdmin.Where(u => true)
                //            .OrderBy(u => u.y_name)
                //            .ThenByDescending(u => u.id)
                //            .ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                //    ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                //    ViewBag.adminid = YdAdminId;
                //    if (Request.IsAjaxRequest())
                //        return PartialView("SubSchoolList", dbLogList);
                //    return View(dbLogList);
                //}
            }
        }

        /// <summary>
        /// 添加函授站页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑函授站视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑函授站
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolEdit(YD_Sys_SubSchool role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(subSchoolDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加函授站之前的验证
        /// </summary>
        /// <returns></returns>
        public string SubSchoolAddVerify(YD_Sys_SubSchool major)
        {
            using (var yunEntities = new IYunEntities())
            {
                var name = major.y_name.Trim().Replace(" ", "").Replace("函授站", "").Replace("函授", "");
                if (name.Length <= 2)
                {
                    var entityList = yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(name));
                    var objList = entityList.ToList();
                    var res = "";
                    for (var i = 0; i < objList.Count; i++)
                    {
                        res += (i + 1) + "、" + objList[i].y_name;
                        if (i + 1 < objList.Count)
                        {
                            res += "  ";
                        }
                    }
                    return res;
                }
                else
                {
                    var sql = "";
                    for (var i = 0; i < name.Length - 1; i++)
                    {
                        sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
                        if (i + 1 < name.Length - 1)
                        {
                            sql += " or ";
                        }

                    }
                    var entityList =
                        yunEntities.Database.SqlQuery<YD_Sys_SubSchool>("select * from YD_Sys_SubSchool where " + sql)
                            .ToList();
                    var objList = entityList.ToList();
                    var res = "";
                    for (var i = 0; i < objList.Count; i++)
                    {
                        res += (i + 1) + "、" + objList[i].y_name;
                        if (i + 1 < objList.Count)
                        {
                            res += "  ";
                        }
                    }
                    return res;
                }

            }
        }

        /// <summary>
        /// 增加函授站
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolAdd(YD_Sys_SubSchool role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(subSchoolDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加函授站(导入学生数据时的自动添加库操作)
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolAddT(YD_Sys_SubSchool role)
        {
            using (var yunEntities = new IYunEntities())
            {
                var sublist = yunEntities.YD_Sys_SubSchool.Select(u => u.y_name).ToList();
                if (sublist.Contains(role.y_name))
                {
                    return Content("<script>alert('已存在该函授钻，无需添加');location.href='/Student/VerifyStudent';</script>");
                }
                else
                {
                    var code = yunEntities.YD_Sys_SubSchool.OrderByDescending(u => u.y_code).FirstOrDefault();
                    if (code != null)
                    {
                        int y_codeNew = Convert.ToInt32(code.y_code) + 1;
                        role.y_code = y_codeNew.ToString();
                    }
                    yunEntities.Entry(role).State = EntityState.Added;
                    int t = yunEntities.SaveChanges();
                    if (t > 0)
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("未知错误");
                    }
                }

            }
        }

        /// <summary>
        /// 删除函授站
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(subSchoolDal.EntityDelete(id, yunEntities));
            }
        }

        #region 批量导入函授站信息

        /// <summary>
        /// 批量导入函授站页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadSubSchool()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            var fileName = Request["filename"];
            if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
            {
                return Content("该文件不是正确的Excel文件");
            }
            fileName = Server.MapPath(fileName);
            using (var excelHelper = new ExcelHelper(fileName))
            {
                var dt = excelHelper.ExcelToDataTable("", true);
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Edu_FormTemp");
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                    string name = "";
                    string code = "";
                    string nameMatch = "";
                    int isOk = (int)YesOrNo.No;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        name = "";
                        code = "";
                        nameMatch = "";
                        isOk = (int)YesOrNo.No;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["函授站名"].ToString()))
                        {
                            name = dt.Rows[i]["函授站名"].ToString()
                                .Trim()
                                .Replace(" ", "")
                                .Replace("函授站", "")
                                .Replace("函授", "");
                            if (name.Length <= 2)
                            {
                                var entityList = yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(name));
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    //如果长度大于450则不继续拼接
                                    if (nameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    nameMatch += (j + 1) + "、" + objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        nameMatch += "  ";
                                    }
                                }
                            }
                            else
                            {
                                var sql = "";
                                for (var j = 0; j < name.Length - 1; j++)
                                {
                                    sql += " y_name like '%" + name.Substring(j, 2) + "%' ";
                                    if (j + 1 < name.Length - 1)
                                    {
                                        sql += " or ";
                                    }
                                }
                                var entityList =
                                    yunEntities.Database.SqlQuery<YD_Sys_SubSchool>(
                                        "select * from YD_Sys_SubSchool where " + sql).ToList();
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    //如果长度大于450则不继续拼接
                                    if (nameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    nameMatch += (j + 1) + "、" + objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        nameMatch += "  ";
                                    }
                                }
                            }
                            if (dt.Rows[i]["函授站代码"] != null)
                            {
                                code = dt.Rows[i]["函授站代码"].ToString();
                            }
                            else
                            {
                                code = "111";
                            }
                            var codelist = yunEntities.YD_Sys_SubSchool.Where(u => u.y_code == code).ToList();
                            if (codelist.Any())
                            {
                                isOk = (int)YesOrNo.No;
                            }
                            else
                            {
                                isOk = (int)YesOrNo.Yes;
                            }
                            var studentTemp = new YD_Edu_FormTemp()
                            {
                                y_name = name,
                                y_nameMatch = nameMatch,
                                y_code = code,
                                y_isOk = isOk,
                                y_eduTypeId = 0
                            };
                            yunEntities.Entry(studentTemp).State = EntityState.Added;
                        }
                    }
                    yunEntities.SaveChanges();
                    ViewBag.entityList = yunEntities.YD_Edu_FormTemp.Where(u => true).ToList();
                    return View();
                }
            }
        }

        public ActionResult NotUpload(string id)
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            var idArr = Array.ConvertAll<string, int>(id.Split(','), s => int.Parse(s));
            using (var yunEntities = new IYunEntities())
            {
                var entity = yunEntities.YD_Edu_FormTemp.Where(u => idArr.Contains(u.id));
                if (entity.Any())
                {
                    foreach (var tem in entity)
                    {
                        yunEntities.Entry(tem).State = EntityState.Deleted;
                    }
                    int t = yunEntities.SaveChanges();
                    if (t > 0)
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("未知错误");
                    }
                }
                else
                {
                    return Content("ok");
                }
            }
        }

        public ActionResult NeedUpload(string id)
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            var idArr = Array.ConvertAll<string, int>(id.Split(','), s => int.Parse(s));
            using (var yunEntities = new IYunEntities())
            {
                var entity = yunEntities.YD_Edu_FormTemp.Where(u => idArr.Contains(u.id));
                if (entity.Any())
                {
                    foreach (var tem in entity)
                    {
                        tem.y_isOk = (int)YesOrNo.Yes;
                        yunEntities.Entry(tem).State = EntityState.Modified;
                    }
                    int t = yunEntities.SaveChanges();
                    if (t > 0)
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("未知错误");
                    }
                }
                else
                {
                    return Content("ok");
                }

            }
        }

        /// <summary>
        /// 验证导入的临时专业信息
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifySubSchool()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var db = new IYunEntities())
            {
                //更新校正后的数据
                int isOk = (int)YesOrNo.No;
                var temps = db.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk);
                if (!string.IsNullOrEmpty(Request.Form.ToString()))
                {
                    foreach (var item in temps)
                    {
                        //var itemArr = Request[item.id.ToString()].Split(new string[] { "," },
                        //    StringSplitOptions.RemoveEmptyEntries);
                        //var yName = itemArr[0];
                        //var yCode = itemArr[1];
                        var verifyCode = db.YD_Sys_SubSchool.Where(u => u.y_name == item.y_name);
                        if (verifyCode.Any())
                        {
                            db.Entry(item).State = EntityState.Deleted;
                        }
                        else
                        {
                            item.y_isOk = (int)YesOrNo.No;
                        }
                    }
                    int r = db.SaveChanges();
                    if (r > 0)
                    {
                        return
                            Content("<script type='text/javascript'>alert('有重复函授站不导入，重复条数为" + r +
                                    "');window.location.href='/Edu/VerifyCourse';</script >");
                    }
                }
                //再次查询需要校正的数据
                ViewBag.entityList = db.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk).ToList();
                ViewBag.modulePowers = GetChildModulePower(db, 2); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 提交用户更新的临时数据（函授站信息）
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateSubSchool()
        {
            var subschools = Request["subschool"].Split(new[] { "<>" }, StringSplitOptions.None);
            var namemacths = Request["namemacth"].Split(new[] { "<>" }, StringSplitOptions.None);
            var codes = Request["code"].Split(new[] { "<>" }, StringSplitOptions.None);
            var ids = Request["id"].Split(new[] { "<>" }, StringSplitOptions.None);

            int id = 0;
            string subschool = "";
            string namemacth = "";
            string code = "";
            int isOk = (int)YesOrNo.No;

            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < ids.Count(); i++)
                {
                    namemacth = "";
                    subschool = "";
                    code = "";

                    id = Convert.ToInt32(ids[i]);
                    var subschoolplan = yunEntities.YD_Edu_FormTemp.FirstOrDefault(u => u.id == id);
                    if (subschoolplan != null)
                    {
                        var verifyCode = yunEntities.YD_Sys_SubSchool.Where(u => u.y_code == codes[i]);
                        if (verifyCode == null)
                            if (verifyCode.Any())
                            {
                                isOk = (int)YesOrNo.No;
                            }
                            else
                            {
                                isOk = (int)YesOrNo.Yes;
                            }
                        subschoolplan.y_name = subschool;
                        subschoolplan.y_nameMatch = namemacth;
                        subschoolplan.y_code = code;

                        yunEntities.Entry(subschoolplan).State = EntityState.Modified;
                    }
                }
                var t = yunEntities.SaveChanges();
                return Content("ok");
            }
        }

        /// <summary>
        /// 将验证无误的数据进行导入
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadTrueSubSchool()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int)YesOrNo.Yes;
                var scoreList = yunEntities.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk).ToList();
                for (var i = 0; i < scoreList.Count; i++)
                {

                    var score = new YD_Sys_SubSchool()
                    {
                        y_name = scoreList[i].y_name,
                        y_code = scoreList[i].y_code
                    };
                    yunEntities.Entry(score).State = EntityState.Added;

                }
                yunEntities.SaveChanges();
                return Redirect("SubSchool");
            }
        }

        #endregion

        #region 验证重复性

        /// <summary>
        /// 验证函授站名重复
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolNameCheckUp()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Sys_SubSchool>(
                        " SELECT  id ,formc.y_name ,y_code  FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_name FROM dbo.YD_Sys_SubSchool GROUP BY y_name) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.YD_Sys_SubSchool AS formc ON formb.y_name=formc.y_name")
                        .ToList();
                return View();
            }
        }

        /// <summary>
        /// 验证函授站代码重复
        /// </summary>
        /// <returns></returns>
        public ActionResult SubSchoolCodeCheckUp()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Sys_SubSchool>(
                        " SELECT  id ,y_name ,formc.y_code  FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_code FROM dbo.YD_Sys_SubSchool GROUP BY y_code) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.YD_Sys_SubSchool AS formc ON formb.y_code=formc.y_code")
                        .ToList();
                return View();
            }
        }

        #endregion

        #endregion



        #region 函授站登分权限限制

        public ActionResult SmallPower(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SmallPower");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                Redirect("/AdminBase/Index");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                var major = Request["Major"];
                var course = Request["Course"];
                var state = Request["state"];
                var term = Request["Term"];
                IQueryable<YD_Edu_SmallPower> list =
                    yunEntities.YD_Edu_SmallPower.Include(u => u.YD_Edu_Course)
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Sys_SubSchool)
                        .OrderByDescending(u => u.id)
                        .AsQueryable();

                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var years = Convert.ToInt32(year);
                    list = list.Where(u => u.y_year == years);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolid || u.y_subSchoolId == null);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorid = Convert.ToInt32(major);
                    list = list.Where(u => u.y_majorId == majorid || u.y_majorId == null);
                }
                if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                {
                    var termid = Convert.ToInt32(term);
                    list = list.Where(u => u.y_term == termid || u.y_term == null);
                }
                if (!string.IsNullOrWhiteSpace(course) && !course.Equals("0"))
                {
                    var courseid = Convert.ToInt32(course);
                    list = list.Where(u => u.y_courseId == courseid || u.y_courseId == null);
                }
                if (!string.IsNullOrWhiteSpace(state) && !state.Equals("-1"))
                {
                    if (state == ((int)YesOrNo.Yes).ToString())
                    {
                        list = list.Where(u => u.y_endTime >= DateTime.Now);
                    }
                    else if (state == ((int)YesOrNo.No).ToString())
                    {
                        list = list.Where(u => u.y_endTime < DateTime.Now);
                    }
                }

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                    return PartialView("SmallPowerList", model);
                return View(model);
            }
        }

        #endregion

        #region 函授站录分权限添加视图

        /// <summary>
        /// 函授站录分权限添加视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult AddSmallPower()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SmallPower");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                Redirect("/AdminBase/Index");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        #endregion

        #region 函授站录分权限保存AJAX

        /// <summary>
        /// 函授站录分权限保存AJAX
        /// </summary>
        /// <param name="stu"></param>
        /// <returns>处理结果json</returns>
        public string SaveSmallPower()
        {
            var re = new Hashtable();

            #region 权限验证

            var power = SafePowerPage("/Sysadmin/SmallPower");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                re["msg"] = "没有权限";
                re["isok"] = false;
            }

            #endregion

            var endTime = Convert.ToDateTime(Request["y_endTime"]);
            var subSchool = Request["SubSchool"];
            var major = Request["Major"];
            var course = Request["Course"];
            var year = Convert.ToInt32(Request["year"]);
            var courseType = Request["CourseType"];
            var scoreLimit = Request["ScoreLimit"];
            var term = Request["Term"];

            var smallPower = new YD_Edu_SmallPower()
            {
                y_adminId = YdAdminId,
                y_createTime = DateTime.Now,
                y_year = year,
                y_endTime = endTime.Date.AddDays(1).AddSeconds(-1),
                y_courseId = null,
                y_courseType = null,
                y_majorId = null,
                y_scorelimit = null,
                y_subSchoolId = null,
                y_term = null,
            };

            using (var ad = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subschoolid = Convert.ToInt32(subSchool);
                    smallPower.y_subSchoolId = subschoolid;
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorid = Convert.ToInt32(major);
                    smallPower.y_majorId = majorid;
                }
                if (!string.IsNullOrWhiteSpace(course) && !course.Equals("0"))
                {
                    var courseid = Convert.ToInt32(course);
                    smallPower.y_courseId = courseid;
                }
                if (!string.IsNullOrWhiteSpace(courseType) && !courseType.Equals("0"))
                {
                    int courseTypeid = Convert.ToInt32(courseType);
                    smallPower.y_courseType = courseTypeid;
                }
                if (!string.IsNullOrWhiteSpace(scoreLimit) && !scoreLimit.Equals("0"))
                {
                    int scoreLimitid = Convert.ToInt32(scoreLimit);
                    smallPower.y_scorelimit = scoreLimitid;
                }
                if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                {
                    int termid = Convert.ToInt32(term);
                    smallPower.y_term = termid;
                }

                ad.Entry(smallPower).State = EntityState.Added;
                int i = ad.SaveChanges();
                if (i > 0)
                {
                    re["msg"] = "添加成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = "添加失败";
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        #endregion

        #region 函授站录分权限AJAX

        /// <summary>
        /// 函授站录分权限删除AJAX
        /// </summary>
        /// <param name="id">学生id</param>
        /// <returns>处理结果json</returns>
        public string DeleSmallPowerById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Sysadmin/SmallPower");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_delete == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                var re = new Hashtable();
                var smallpower = ad.YD_Edu_SmallPower.FirstOrDefault(u => u.id == id);
                ad.Entry(smallpower).State = EntityState.Deleted;
                var msg = ad.SaveChanges();
                if (msg > 0)
                {
                    re["msg"] = "删除成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = "删除失败";
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        /// <summary>
        /// 函授站管理下载
        /// </summary>'
        /// <returns>视图</returns>
        public ActionResult DownloadSubSchool()
        {
            #region 权限验证

            var power = SafePowerPage("/SysAdmin/SubSchool");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var nameStr = Request["y_name"];
                IQueryable<VW_SubschoolAdmin> list = yunEntities.VW_SubschoolAdmin.OrderByDescending(u => u.id);
                //根据名字
                if (!string.IsNullOrEmpty(nameStr))
                {
                    list = list.Where(s => s.y_name.Contains(nameStr));
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_code = u.y_code,
                                    y_adminName = u.y_adminName,
                                }).ToList());

                var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
                if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                {
                    model =
                        FileHelper.ToDataTable(
                            list.Select(
                                u =>
                                    new
                                    {
                                        y_name = u.y_name,
                                        y_nameabbreviation = u.y_nameabbreviation,
                                        y_formername = u.y_formername,
                                        y_code = u.y_code,
                                        y_adminName = u.y_adminName,
                                    }).ToList());
                }
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站管理表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/函授站管理表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "函授站"},
                        {"y_code", "函授站代码"},
                        {"y_adminName", "函授站账号"}
                    };
                    if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "函授站"},
                            {"y_nameabbreviation", "函授站简称"},
                            {"y_formername", "函授站曾用名"},
                            {"y_code", "函授站代码"},
                            {"y_adminName", "函授站账号"}
                        };
                    }
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("alert('错误');window.location.href='" + reurl + "';");
                }

            }
        }

        #endregion

        #region 学号设置管理

        public ActionResult StuNum()
        {
            #region 权限验证

            var power = SafePowerPage("/Sysadmin/StuNum");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                ViewBag.entity = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
                return View();
            }
        }

        public ActionResult StuNumColSave()
        {
            #region 权限验证

            var power = SafePowerPage("/Sysadmin/StuNum");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var y_one = Request["y_one"];
                var y_two = Request["y_two"];
                var y_three = Request["y_three"];
                var y_four = Request["y_four"];
                var y_five = Request["y_five"];
                var entity = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
                if (entity == null)
                {
                    var obj = new YD_Sts_StuNumCol();
                    obj.y_one = y_one;
                    obj.y_two = y_two;
                    obj.y_three = y_three;
                    obj.y_four = y_four;
                    obj.y_five = y_five;
                    yunEntities.Entry(obj).State = EntityState.Added;
                }
                else
                {
                    entity.y_one = y_one;
                    entity.y_two = y_two;
                    entity.y_three = y_three;
                    entity.y_four = y_four;
                    entity.y_five = y_five;
                    yunEntities.Entry(entity).State = EntityState.Modified;
                }
                var t = yunEntities.SaveChanges();
                return Content(t > 0 ? "ok" : "未知错误");
            }
        }

        #endregion

        #region 基础配置

        #region 学习形式

        public ActionResult StuType(int id = 1)
        {
            #region “学习形式管理”权限验证

            var power = SafePowerPage("/SysAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_StuType.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("StuTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加学习形式页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeAddPage()
        {
            #region “添加学习形式”权限验证

            var power = SafePowerPage("/SysAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学习形式视图
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeEditPage(int id)
        {
            #region “编辑学习形式”权限验证

            var power = SafePowerPage("/SysAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeEdit(YD_Edu_StuType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeAdd(YD_Edu_StuType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 层次

        public ActionResult EduType(int id = 1)
        {
            #region “层次管理”权限验证

            var power = SafePowerPage("/SysAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_EduType.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("EduTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加层次页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeAddPage()
        {
            #region “添加层次”权限验证

            var power = SafePowerPage("/SysAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑层次视图
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeEditPage(int id)
        {
            #region “编辑层次”权限验证

            var power = SafePowerPage("/SysAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeEdit(YD_Edu_EduType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeAdd(YD_Edu_EduType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 政治面貌

        public ActionResult Politics(int id = 1)
        {
            #region “政治面貌管理”权限验证

            var power = SafePowerPage("/SysAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Sts_Politics.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("PoliticsList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加政治面貌页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsAddPage()
        {
            #region “添加政治面貌”权限验证

            var power = SafePowerPage("/SysAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑政治面貌视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsEditPage(int id)
        {
            #region “编辑政治面貌”权限验证

            var power = SafePowerPage("/SysAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsEdit(YD_Sts_Politics role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsAdd(YD_Sts_Politics role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 民族类型

        public ActionResult Nation(int id = 1)
        {
            #region “民族类型管理”权限验证

            var power = SafePowerPage("/SysAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Sts_Nation.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("NationList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加民族类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NationAddPage()
        {
            #region “添加民族类型”权限验证

            var power = SafePowerPage("/SysAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑民族类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult NationEditPage(int id)
        {
            #region “编辑民族类型”权限验证

            var power = SafePowerPage("/SysAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Sts_Nation.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑民族类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationEdit(YD_Sts_Nation role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加民族类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationAdd(YD_Sts_Nation role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除民族类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 课程类型

        public ActionResult CourseType(int id = 1)
        {
            #region “课程类型管理”权限验证

            var power = SafePowerPage("/SysAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_CourseType.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("CourseTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加课程类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeAddPage()
        {
            #region “添加课程类型”权限验证

            var power = SafePowerPage("/SysAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeEditPage(int id)
        {
            #region “编辑课程类型”权限验证

            var power = SafePowerPage("/SysAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeEdit(YD_Edu_CourseType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeAdd(YD_Edu_CourseType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 学籍状态类型

        public ActionResult StuState(int id = 1)
        {
            #region “学籍状态类型管理”权限验证

            var power = SafePowerPage("/SysAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_StuState.OrderByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("StuStateList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加学籍状态类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateAddPage()
        {
            #region “添加学籍状态类型”权限验证

            var power = SafePowerPage("/SysAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学籍状态类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateEditPage(int id)
        {
            #region “编辑学籍状态类型”权限验证

            var power = SafePowerPage("/SysAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 10); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateEdit(YD_Edu_StuState role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateAdd(YD_Edu_StuState role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 删除学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #endregion

        public class SmallPowerList
        {
            public string subSchName { get; set; }
            public string majorName { get; set; }
            public string courseName { get; set; }
            public DateTime y_endTime { get; set; }
            public string ISokdated { get; set; }
        }

        public class SubSchoolList
        {
            public int id { get; set; }
            public string y_name { get; set; }
            public string y_code { get; set; }
            public string adminsu { get; set; }
        }

        public ActionResult ErrorHtml()
        {
            return View();
        }

        //跳转青书对应角色账号
        public ActionResult GoToQinshu()
        {
            var qinshukey = ConfigurationManager.AppSettings["QinshuKey"];

            if (string.IsNullOrWhiteSpace(qinshukey))
            {
                return Content("此学校未配置青书对应学校编号");
            }

            string url = "http://www.qingshuxuetang.com/QingShuHomeSvc/v1/auth/login";

            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }

            using (var ad = new IYunEntities())
            {
                var admin = ad.YD_Sys_Admin.First(u => u.id == YdAdminId);

                if (string.IsNullOrWhiteSpace(admin.y_qinshuName))
                {
                    return Content("此账号未配置青书对应账号");
                }

                var dto = new ApiDto()
                {
                    collegeSymbol = qinshukey,
                    name = admin.y_qinshuName,
                    password = admin.y_qinshuPwd
                };

                var req = JsonConvert.SerializeObject(dto);

                string str = Post_Request_API(req, url);

                var result = JsonConvert.DeserializeObject<ApiResultDto>(str);

                if (result.hr == 0 && result.data != null)
                {
                    return Redirect(result.data.url);
                }
                return Content(result.message);
            }

        }


        public class ApiDto
        {
            public string collegeSymbol { get; set; }

            public string name { get; set; }

            public string password { get; set; }
        }

        public class ApiResultDto
        {
            public int hr { get; set; }

            public string message { get; set; }

            public ApiResultExt data { get; set; }

            public string extraData { get; set; }
        }

        public class ApiResultExt
        {
            public string url { get; set; }
        }

        #region 调用API接口 

        /// <summary>
        /// Post 请求API接口 
        /// </summary>
        /// <param name="str_Json">json字符串</param>
        /// <returns></returns>
        public static string Post_Request_API(string str_Json, string API_URL)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(API_URL);
                HttpWebRequest req = webRequest as HttpWebRequest;

                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                req.Timeout = 6000000; // 5000;
                byte[] bytebuff = Encoding.UTF8.GetBytes(str_Json);
                req.ContentLength = bytebuff.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(bytebuff, 0, bytebuff.Length);
                requestStream.Close();

                //所请求页面响应
                WebResponse pos = req.GetResponse();
                StreamReader sr = new StreamReader(pos.GetResponseStream(), Encoding.UTF8);
                string strResult = sr.ReadToEnd().Trim();
                sr.Close();
                sr.Dispose();

                return strResult;
            }
            catch (Exception e1)
            {
                string a = e1.Message;
                return e1.Message;
            }

        }

        #endregion

        //测试成绩对接 青书成绩同步方法
        public void Test()
        {
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            var pem = "";
            var pempassword = "";
            var pemaccount = "";
            if (schoolname == ComEnum.SchoolName.JXLG.ToString())
            {
                pem = @"D:\cert\ftpjxust.pem";
                pempassword = "sGnkh%0q";
                pemaccount = "ftpjxust";
            }
            if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
            {
                pem = @"D:\cert\ftpjxsf.pem";
                pempassword = "N%@%4J88";
                pemaccount = "ftpjxsf";
            }
            if (schoolname == ComEnum.SchoolName.HDJTDX.ToString())
            {
                pem = @"D:\cert\ftphdjd.pem";
                pempassword = "Uqz0KwzX";
                pemaccount = "ftphdjd";
            }
            if (schoolname == ComEnum.SchoolName.JXKJSFDX.ToString())
            {
                pem = @"D:\cert\ftpjxstnu.pem";
                pempassword = "4@iEqbkE";
                pemaccount = "ftpjxstnu";
            }
            if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
            {
                pem = @"D:\cert\ftpgnnu.pem";
                pempassword = "mv5ozTA7";
                pemaccount = "ftpgnnu";
            }
            if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
            {
                pem = @"D:\cert\ftpjxzyy.pem";
                pempassword = "0MDfxH&I";
                pemaccount = "ftpjxzyy";
            }
            if (schoolname == ComEnum.SchoolName.DHLGDX.ToString())
            {
                pem = @"D:\cert\ftpdhlg.pem";
                pempassword = "*WEfA@X$";
                pemaccount = "ftpdhlg";
            }
            PrivateKeyFile a = new PrivateKeyFile(pem, pempassword);
            SFTPHelper t = new SFTPHelper("file.qingshuxuetang.com", "22", pemaccount, a);

            var firname = t.GetFileListFirst("batch/score", "xlsx");
            string fileName = "";

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel获取开始");

            var applicationPath = Request.PhysicalApplicationPath;

            // 获取文件
            t.Get("batch/score/" + firname, applicationPath + "File/" + firname);

            fileName = applicationPath + "File/" + firname; //导出存放位置            

            //string fileName = "D://线下补考成绩.xlsx"; //导出存放位置

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel获取完成");

            string Hz; //后缀名

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel读取开始");

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var date1 = DateTime.Now;

                IWorkbook workbook;
                if (fileName.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                    Hz = ".xlsx";
                }
                else if (fileName.IndexOf(".xls", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                    Hz = ".xls";
                }
                else
                {
                    return;
                }

                LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel读取完成");

                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return;
                }
                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;
                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);

                int index = 0;
                using (var ad = new IYunEntities())
                {
                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "数据初始化开始");

                    var Correspondence = ad.YD_Edu_CourseCorrespondence.Where(u=>u.y_inyear==2017).ToList(); //课程对接表

                    var stulist =
                        ad.VW_StuInfo.Where(u => u.y_isdel == 1 && u.y_stuStateId != 6).Select(
                            u => new { u.id, u.y_stuNum, u.schoolName, u.y_inYear, u.y_majorId, u.y_subSchoolId })
                            .ToList();

                    var teach =
                        ad.YD_TeaPlan_Class.Select(u => new { u.y_subSchoolId, u.y_year, u.y_majorId }).ToList();


                    var scoreOklist =
                        ad.YD_Edu_Score.Where(u => u.y_type == 2 && u.y_totalScore >= 60 && u.y_totalScore <= 100)
                            .GroupBy(u => new { u.y_stuId, u.y_courseId, u.y_term }).Select(u => u.Key)
                            .ToList();

                    sheet.GetRow(0).CreateCell(14).SetCellValue("导入情况");
                    sheet.GetRow(0).CreateCell(15).SetCellValue("失败原因");

                    int stuId;
                    int courseId;
                    int term;
                    decimal normalScore;
                    decimal termScore;
                    decimal totalScore;
                    decimal bkScore;


                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "数据初始化完成");

                    var sb = new StringBuilder("INSERT INTO YD_Edu_Score ");
                    sb.AppendLine(
                        "([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                    var inde = 0;
                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "开始生成SQL");


                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        if (i % 100 == 0)
                        {
                            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "当前执行到:" + i + "条数据");
                        }

                        var row = sheet.GetRow(i);
                        if (row?.GetCell(1) == null)
                        {
                            continue;
                        }

                        string cell1;

                        cell1 = row.GetCell(1).CellType == CellType.NUMERIC
                            ? row.GetCell(1).NumericCellValue.ToString()
                            : row.GetCell(1).StringCellValue;

                        if (string.IsNullOrWhiteSpace(cell1))
                        {
                            SetError(row, 1, "学号不能为空", styleCell);
                            index++;
                            continue;
                        }

                        var stuinfo = stulist.FirstOrDefault(u => u.y_stuNum == cell1.Trim());

                        //得到学生ID
                        if (stuinfo == null)
                        {
                            SetError(row, 1, "学号不存在", styleCell);
                            index++;
                            continue;
                        }

                        if (!teach.Any(u =>
                            u.y_subSchoolId == stuinfo.y_subSchoolId && u.y_year == stuinfo.y_inYear &&
                            u.y_majorId == stuinfo.y_majorId))
                        {

                            SetError(row, 1, "此学生无班级教学计划", styleCell);
                            index++;
                            continue;
                        }

                        var edunub = row.GetCell(6).StringCellValue.Trim(); //层次
                        if (edunub.Contains("高升专"))
                        {
                            edunub = "高起专";
                        }
                        if (edunub.Contains("高升本"))
                        {
                            edunub = "高起本";
                        }

                        var Corres =
                            Correspondence.Where(
                                u =>
                                    u.y_qingshuMajorlib == row.GetCell(5).StringCellValue.Trim() &&
                                    u.y_qingshuEdutype == edunub && u.y_inyear == stuinfo.y_inYear).ToList();
                        //验证专业和层次是否符合对照表

                        if (Corres.Count == 0)
                        {
                            SetError(row, 5, "对照表找不到专业、层次、年份", styleCell);
                            index++;
                            continue;
                        }

                        if (!Corres.Select(u => u.y_MajorID).Contains(stuinfo.y_majorId))
                        {
                            SetError(row, 5, "学生专业不属于同步范围", styleCell);
                            index++;
                            continue;
                        }
                        if (row.GetCell(7).CellType != CellType.STRING)
                        {
                            SetError(row, 7, "该课程不是文本类型", styleCell);
                            index++;
                            continue;
                        }

                        var termnub = Convert.ToInt32(row.GetCell(8).StringCellValue.Trim());
                        LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "当前执行到学期验证的时候:" + i + "条数据");

                        var teachdes =
                            Corres.FirstOrDefault(
                                u =>
                                    u.y_qingshuCourse == row.GetCell(7).StringCellValue.Trim() && u.y_MajorID == stuinfo.y_majorId);
                                    /*江西师范不匹配学期u.y_qingshuTerm == termnub &&*/ 

                        if (teachdes == null)
                        {
                            SetError(row, 7, "对照表找不到课程,学期", styleCell);
                            index++;
                            continue;
                        }

                        stuId = stuinfo.id;
                        courseId = teachdes.y_CourseID;
                        term = teachdes.y_Term;

                        normalScore = Convert.ToDecimal(row.GetCell(9).StringCellValue.Trim());

                        bkScore = Convert.ToDecimal(row.GetCell(11).StringCellValue.Trim());

                        termScore = bkScore == 0m ? Convert.ToDecimal(row.GetCell(10).StringCellValue.Trim()) : bkScore;


                        totalScore = Convert.ToDecimal(row.GetCell(13).StringCellValue.Trim());

                        //理工专用。2017第三学期和2018第一学期成绩大于等于90乖以0.9
                        if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                        {
                            if (stuinfo.y_inYear == 2017 && (term == 3 || term == 4) && totalScore >= 90)
                            {
                                totalScore = totalScore * 9 / 10;
                            }
                            if (stuinfo.y_inYear >= 2018 && totalScore >= 90)
                            {
                                totalScore = totalScore * 9 / 10;
                            }

                        }


                        if (scoreOklist.Any(u => u.y_stuId == stuId && u.y_courseId == courseId && u.y_term == term))
                        {
                            continue;
                        }
                        inde++;

                        sb.AppendLine(
                            $"({stuId},{term},{normalScore},{termScore},null,{totalScore},{courseId},1,GETDATE()),");


                        if (inde == 999)
                        {
                            LogHelper.WriteDebugLog(typeof(LogType),
                                DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入到第" + i + "条");
                            string sql = sb.ToString(0, sb.Length - 3);

                            ad.Database.ExecuteSqlCommand(sql);

                            inde = 0;
                            sb.Clear();
                            sb.Append("INSERT INTO YD_Edu_Score ");
                            sb.AppendLine(
                                "([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                        }
                    }

                    if (inde != 0)
                    {
                        LogHelper.WriteDebugLog(typeof(LogType),
                                  DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入到最后一条");
                        string sqllast = sb.ToString(0, sb.Length - 3);

                        ad.Database.ExecuteSqlCommand(sqllast);
                        sb.Clear();
                    }

                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入完成");
                }



                if (index > 0)
                {
                    var date = DateTime.Now;

                    var dirPath = Server.MapPath("~/File/Dowon/" + date.ToString("yyyyMM"));
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/成绩同步失败表" + date.ToString("ddHHmmss") + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        Response.Write((DateTime.Now - date1).TotalSeconds);
                    }
                }
            }
        }

        // 新的成绩同步方法，数据不直接导入到成绩表，而是导入到附表，然后再执行InsertTest()方法插入主表，去除掉已经及格且为手动输入的，去除掉已经毕业的学生
        public void NewTest()
        {
            #region 获取数据
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            var pem = "";
            var pempassword = "";
            var pemaccount = "";
            if (schoolname == ComEnum.SchoolName.JXLG.ToString())
            {
                pem = @"D:\cert\ftpjxust.pem";
                pempassword = "sGnkh%0q";
                pemaccount = "ftpjxust";
            }
            if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
            {
                pem = @"D:\cert\ftpjxsf.pem";
                pempassword = "N%@%4J88";
                pemaccount = "ftpjxsf";
            }
            if (schoolname == ComEnum.SchoolName.HDJTDX.ToString())
            {
                pem = @"D:\cert\ftphdjd.pem";
                pempassword = "Uqz0KwzX";
                pemaccount = "ftphdjd";
            }
            if (schoolname == ComEnum.SchoolName.JXKJSFDX.ToString())
            {
                pem = @"D:\cert\ftpjxstnu.pem";
                pempassword = "4@iEqbkE";
                pemaccount = "ftpjxstnu";
            }
            if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
            {
                pem = @"D:\cert\ftpgnnu.pem";
                pempassword = "mv5ozTA7";
                pemaccount = "ftpgnnu";
            }
            if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
            {
                pem = @"D:\cert\ftpjxzyy.pem";
                pempassword = "0MDfxH&I";
                pemaccount = "ftpjxzyy";
            }
            if (schoolname == ComEnum.SchoolName.DHLGDX.ToString())
            {
                pem = @"E:\cert\ftpdhlg.pem";
                pempassword = "*WEfA@X$";
                pemaccount = "ftpdhlg";
            }
            PrivateKeyFile a = new PrivateKeyFile(pem, pempassword);
            SFTPHelper t = new SFTPHelper("file.qingshuxuetang.com", "22", pemaccount, a);

            var firname = t.GetFileListFirst("batch/score", "xlsx");
            string fileName = "";

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel获取开始");

            var applicationPath = Request.PhysicalApplicationPath;

            // 获取文件
            t.Get("batch/score/" + firname, applicationPath + "File/" + firname);

            fileName = applicationPath + "File/" + firname; //导出存放位置            

            //string fileName = "D://线下补考成绩.xlsx"; //导出存放位置

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel获取完成");

            string Hz; //后缀名

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel读取开始");
            #endregion 

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var date1 = DateTime.Now;

                IWorkbook workbook;
                if (fileName.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                    Hz = ".xlsx";
                }
                else if (fileName.IndexOf(".xls", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                    Hz = ".xls";
                }
                else
                {
                    return;
                }

                LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "Excel读取完成");

                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return;
                }
                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;
                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);

                int index = 0;
                using (var ad = new IYunEntities())
                {
                    // 初始化成绩附表，删除上一次导入的数据。
                    ad.Database.ExecuteSqlCommand("delete from yd_edu_scoretest");

                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "数据初始化开始");

                    var Correspondence = ad.YD_Edu_CourseCorrespondence.Where(u => true).ToList(); //课程对接表

                    var stulist =
                        ad.VW_StuInfo.Where(u => u.y_isdel == 1 && u.y_stuStateId != 6).Select(
                            u => new { u.id, u.y_stuNum,u.y_examNum, u.schoolName, u.y_inYear, u.y_majorId, u.y_subSchoolId })
                            .ToList();

                    var teach =
                        ad.YD_TeaPlan_Class.Select(u => new { u.y_subSchoolId, u.y_year, u.y_majorId }).ToList();

                    var scoreOklist =
                        ad.YD_Edu_Score.Where(u => u.y_type == 2 && u.y_totalScore >= 60 && u.y_totalScore <= 100)
                            .GroupBy(u => new { u.y_stuId, u.y_courseId, u.y_term }).Select(u => u.Key)
                            .ToList();
                    sheet.GetRow(0).CreateCell(14).SetCellValue("导入情况");
                    sheet.GetRow(0).CreateCell(15).SetCellValue("失败原因");

                    int stuId;
                    int courseId;
                    int term;
                    decimal normalScore;
                    decimal termScore;
                    decimal totalScore;
                    decimal bkScore;


                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "数据初始化完成");

                    var sb = new StringBuilder("INSERT INTO YD_Edu_ScoreTest ");
                    sb.AppendLine(
                        "([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                    var inde = 0;
                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "开始生成SQL");


                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        if (i % 100 == 0)
                        {
                            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "当前执行到:" + i + "条数据");
                        }

                        var row = sheet.GetRow(i);
                        if (row?.GetCell(1) == null)
                        {
                            continue;
                        }

                        string cell1;

                        cell1 = row.GetCell(1).CellType == CellType.NUMERIC
                            ? row.GetCell(1).NumericCellValue.ToString()
                            : row.GetCell(1).StringCellValue;

                        string cell2;

                        cell2 = row.GetCell(2).CellType == CellType.NUMERIC
                            ? row.GetCell(2).NumericCellValue.ToString()
                            : row.GetCell(2).StringCellValue;

                        if (string.IsNullOrWhiteSpace(cell1))
                        {
                            SetError(row, 1, "学号不能为空", styleCell);
                            index++;
                            continue;
                        }

                        var stuinfo = stulist.FirstOrDefault(u => u.y_stuNum == cell1.Trim() || u.y_examNum == cell2.Trim());

                        //得到学生ID
                        if (stuinfo == null)
                        {
                            SetError(row, 1, "学号或考生号不存在", styleCell);
                            index++;
                            continue;
                        }

                        if (!teach.Any(u =>
                            u.y_subSchoolId == stuinfo.y_subSchoolId && u.y_year == stuinfo.y_inYear &&
                            u.y_majorId == stuinfo.y_majorId))
                        {

                            SetError(row, 1, "此学生无班级教学计划", styleCell);
                            index++;
                            continue;
                        }

                        var edunub = row.GetCell(6).StringCellValue.Trim(); //层次
                        if (edunub.Contains("高升专"))
                        {
                            edunub = "高起专";
                        }
                        if (edunub.Contains("高升本"))
                        {
                            edunub = "高起本";
                        }

                        var Corres =
                            Correspondence.Where(
                                u =>
                                    u.y_qingshuMajorlib == row.GetCell(5).StringCellValue.Trim() &&
                                    u.y_qingshuEdutype == edunub && u.y_inyear == stuinfo.y_inYear).ToList();
                        //验证专业和层次是否符合对照表

                        if (Corres.Count == 0)
                        {
                            SetError(row, 5, "对照表找不到专业、层次、年份", styleCell);
                            index++;
                            continue;
                        }

                        if (!Corres.Select(u => u.y_MajorID).Contains(stuinfo.y_majorId))
                        {
                            SetError(row, 5, "学生专业不属于同步范围", styleCell);
                            index++;
                            continue;
                        }
                        if (row.GetCell(7).CellType != CellType.STRING)
                        {
                            SetError(row, 7, "该课程不是文本类型", styleCell);
                            index++;
                            continue;
                        }

                        var termnub = Convert.ToInt32(row.GetCell(8).StringCellValue.Trim());
                        LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "当前执行到学期验证的时候:" + i + "条数据");

                        var teachdes =
                            Corres.FirstOrDefault(
                                u =>
                                    u.y_qingshuCourse == row.GetCell(7).StringCellValue.Trim() && u.y_MajorID == stuinfo.y_majorId);
                                    /*江西师范不匹配学期u.y_qingshuTerm == termnub &&*/

                        if (teachdes == null)
                        {
                            SetError(row, 7, "对照表找不到课程,学期", styleCell);
                            index++;
                            continue;
                        }

                        stuId = stuinfo.id;
                        courseId = teachdes.y_CourseID;
                        term = teachdes.y_Term;

                        normalScore = Convert.ToDecimal(row.GetCell(9).StringCellValue.Trim());

                        bkScore = Convert.ToDecimal(row.GetCell(11).StringCellValue.Trim());

                        termScore = bkScore == 0m ? Convert.ToDecimal(row.GetCell(10).StringCellValue.Trim()) : bkScore;


                        totalScore = Convert.ToDecimal(row.GetCell(13).StringCellValue.Trim());

                        //理工专用。2017第三学期和2018第一学期成绩大于等于90乖以0.9
                        if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                        {
                            if (stuinfo.y_inYear == 2017 && (term == 3 || term == 4) && totalScore >= 90)
                            {
                                totalScore = totalScore * 9 / 10;
                            }
                            if (stuinfo.y_inYear >= 2018 && totalScore >= 90)
                            {
                                totalScore = totalScore * 9 / 10;
                            }

                        }
                        if (scoreOklist.Any(u => u.y_stuId == stuId && u.y_courseId == courseId && u.y_term == term))
                        {
                            continue;
                        }
                        inde++;
                        if (totalScore > 59 && totalScore < 60)
                            totalScore = 60;
                        sb.AppendLine(
                            $"({stuId},{term},{normalScore},{termScore},null,{totalScore},{courseId},1,GETDATE()),");


                        if (inde == 999)
                        {
                            LogHelper.WriteDebugLog(typeof(LogType),
                                DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入到第" + i + "条");
                            string sql = sb.ToString(0, sb.Length - 3);

                            ad.Database.ExecuteSqlCommand(sql);

                            inde = 0;
                            sb.Clear();
                            sb.Append("INSERT INTO YD_Edu_ScoreTest ");
                            sb.AppendLine(
                                "([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                        }
                    }

                    if (inde != 0)
                    {
                        LogHelper.WriteDebugLog(typeof(LogType),
                                  DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入到最后一条");
                        string sqllast = sb.ToString(0, sb.Length - 3);

                        ad.Database.ExecuteSqlCommand(sqllast);
                        sb.Clear();
                    }

                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "SQL导入完成");
                }

                if (index > 0)
                {
                    var date = DateTime.Now;

                    var dirPath = Server.MapPath("~/File/Dowon/" + date.ToString("yyyyMM"));
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/成绩同步失败表" + date.ToString("ddHHmmss") + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        Response.Write((DateTime.Now - date1).TotalSeconds);
                    }
                }
            }
        }
        // 新的成绩同步方法，把数据更新或插回成绩表。
        public void InsertTest()
        {
            using (var db = new IYunEntities())
            {
                // 更新已经存在的成绩
                string sql = @"update yd_edu_score 
                    SET y_normalScore = yd_edu_scoretest.y_normalScore,
                    y_termScore = yd_edu_scoretest.y_termscore,
                    y_totalscore = yd_edu_scoretest.y_totalscore 
                    from yd_edu_scoretest 
                    where yd_edu_score.y_stuId = yd_edu_scoretest.y_stuId  
                    and yd_edu_score.y_term = yd_edu_scoretest.y_term 
                    and yd_edu_score.y_courseId = yd_edu_scoretest.y_courseid
                    and yd_edu_score.y_type=1";
                db.Database.ExecuteSqlCommand(sql);
                // 插入不存在的成绩
                sql = @"INSERT INTO YD_Edu_Score
                       (y_stuid,y_term,y_normalScore,y_termScore,y_workScore,y_totalScore,y_courseId,y_type,y_time)		
                       select y_stuid,y_term,y_normalScore,y_termScore,y_workScore,y_totalScore,y_courseId,y_type,y_time 
                        from yd_edu_scoretest s where not exists
                        (select * from yd_edu_score t where s.y_stuId = t.y_stuId and s.y_term = t.y_term and s.y_courseId = t.y_courseid)";
                db.Database.ExecuteSqlCommand(sql);

            }
        }
        /// <summary>
        /// 生成专业教学计划多个学期有同一门课程的及格成绩
        /// </summary>
        /// 
        public void updateScore()
        {
             using (var yunEntities = new IYunEntities())
            {
                
                var stuState = Request["StuState"];
                var enrollYear = 2017;
                var subSchool =0;
                var majorLibrary = 0;
                var eduType = 0;
                var stuType =0;
                var isok = 0; //是否允许毕业
                var isup = 0; //是否申请毕业
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.adminrole = YdAdminRoleId;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo
                        .Include(u => u.YD_Fee_StuFeeTb)
                        .Include(u => u.YD_Edu_Major)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && (u.y_stuStateId == 6|| u.y_stuStateId == 2|| u.y_stuStateId == 4||u.y_stuStateId == 1) && u.y_studentType != 2&&(u.y_inYear==2015)).OrderByDescending(u => u.y_inYear).AsNoTracking();
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                //if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                //{

                //        list = list.Where(u => u.y_inYear == (xinshenyear - 2));

                //}
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard
                    || u.y_cardId == namenumcard || u.y_examNum == namenumcard);
                }
                Random num = new Random();
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable().AsNoTracking();

                var list1 = list.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.YD_Edu_Score.OrderBy(u => u.id).AsQueryable().AsNoTracking();

                var list2 = list.GroupJoin(scorelist, s =>  s.id,
                    score => score.y_stuId,
                    (s, score) => new { s, scores = score.OrderByDescending(u => u.id) }).ToList();

                var listss =
                        list1.GroupBy(u => new{ u.stu})
                            .ToList();
            
                var l = new List<int>();
                var count = 0;
                listss.ForEach(u =>
                {
                  
                    var classcourse = u.GroupBy(k => k.classCourse.y_course).ToList();
                    classcourse.ForEach(z => {
                        z.ToList().ForEach(w=> {
                            if (w.stu.id == 1304)
                                count++;
                         var Scores = list2.Where(k => k.s.id == w.stu.id).FirstOrDefault().scores.Where(s=>s.y_courseId==w.classCourse.y_course).ToList();
                            if (Scores.Count > 0)
                            {
                               
                                var score = Scores.Where(k => k.y_term == w.classCourse.y_team).ToList();
                                //  var type2 = Scores.Where(k => k.y_type == 2).FirstOrDefault();
                                var c = score.Count();

                                //if (c == 0)
                                //{

                                //    var jige = score.Where(k => k.y_totalScore >= 0 && k.y_type != 2 && !z.Select(r => r.classCourse.y_team).ToList().Contains(k.y_term)).OrderByDescending(k => k.y_totalScore).ToList();
                                //    if (jige.Count > 0)
                                //    {
                                //        l.Add(jige.First().id);
                                //        var sb = new StringBuilder("UPDATE [YD_Edu_Score] ");
                                //        sb.AppendLine($" SET [y_term]={w.classCourse.y_team},y_time='{DateTime.Now}' WHERE id=" + jige.First().id);
                                //        string sql = sb.ToString();
                                //        var res = SQLHelper.ExecuteNonQuary(sql);
                                //        if (res > 0)
                                //            sql = "";
                                //    }
                                //    else
                                //    {
                                       
                                //            var sql = $"INSERT INTO[dbo].[YD_Edu_Score]( [y_stuId], [y_term],[y_normalScore], [y_termScore], [y_workScore], [y_totalScore], [y_courseId], [y_type], [y_time]) VALUES({w.stu.id}, {w.classCourse.y_team}, 0,0,0,{(int)num.Next(70, 90)}, {w.classCourse.y_course}, 2, '{DateTime.Now}'); ";
                                //        var res = 0;
                                //        if (w.stu.y_stuStateId == 6||z.Count() > 1)
                                //        {
                                //              res = SQLHelper.ExecuteNonQuary(sql); 
                                //        }
                                  
                                //            res = SQLHelper.ExecuteNonQuary(sql);
                                //        if (res > 0)
                                //            sql = "";
                                //    }
                                //}
                            }
                        });
                    });
                        
                   
                    count++;
                });

              
            }
              
            }
      
        public void ImgSet()
        {
            //批量修改照片名称
            string Frompath = @"C: \Users\云端002\Desktop\江西师范大学\2017级兴国中专毕业生照片";  //需要复制到的文件夹
            string[] picList = Directory.GetFiles(Frompath, "*.JPG"); //获取图片
            int i = 0;
            int e = 0;
            string card = "";
            foreach (var f in picList)
            {
                //取得文件名.
                var fName = f.Substring(Frompath.Length + 1).ToUpper();//取得目标图片文件名.
                var name = fName.Substring(0, fName.Length - 4).Replace(" ", "");//取图片名（只保留身份证号）
                name = name.Replace("'", "");
                using (var db = new IYunEntities()) {
                    var list = db.YD_Sts_StuInfo.FirstOrDefault(u => u.y_cardId == name && u.y_isdel == 1);
                    if (list != null)
                    {
                        var sql = $" update YD_Sts_StuInfo set y_img='/Upload/image/181026/{name}.jpg' where y_cardId='{name}'";
                        db.Database.ExecuteSqlCommand(sql);
                        i++;
                    }
                    else
                    {
                        e++;
                        card += name + ",";
                    }
                }
            }
            int s = 0;
        }
        //public void insertCourse()
        //{
        //    using (var db = new IYunEntities())
        //    {
        //        List<qqqqq> list =  db.qqqqqs.Where(x=>x.id>0).ToList();
        //        int num = 0;
        //        foreach(var i in list)
        //        {
        //            var courname = i.qsCourse.Split('(')[0];
        //            if (db.YD_Edu_Course.Any(x => x.y_name.Contains(courname)))
        //            {
        //                var courseid = db.YD_Edu_Course.FirstOrDefault(x => x.y_name.Contains(courname)).id;
        //                i.course = courseid;

        //            }
        //            var name = i.qsMajor.Split('(')[0];
        //            var type = i.qsEdutype;
        //            var edutype = db.YD_Edu_EduType.FirstOrDefault(x => x.y_name == type).id;
        //            var majorid = db.YD_Edu_MajorLibrary.Include(x => x.YD_Edu_Major).FirstOrDefault(x => x.y_name.Contains(name)).
        //                YD_Edu_Major.FirstOrDefault(x => x.y_eduTypeId == edutype && x.y_stuTypeId == 1).id;

        //            i.major = majorid;
        //        }
        //        db.SaveChanges();
        //    }
        //}

        public void SetError(IRow row, int errorindex, string msg, ICellStyle style)
        {
            row.GetCell(errorindex).CellStyle = style;

            if (row.GetCell(14) == null)
            {
                row.CreateCell(14).SetCellValue("失败");
            }
            if (row.GetCell(15) == null)
            {
                row.CreateCell(15).SetCellValue(msg);
            }
            else
            {
                row.GetCell(15).SetCellValue(row.GetCell(15).StringCellValue.Trim() + "," + msg);
            }
        }
    }
}
