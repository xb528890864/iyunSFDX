using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IYun.Models;
using IYun.Object;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.SS.Formula.Eval;
using Webdiyer.WebControls.Mvc;
using IYun.Common;
using System.IO;
using IYun.Controllers.ControllerObject;
using System.Data.SqlClient;

namespace IYun.Controllers
{

    /// <summary>
    /// 经费管理
    /// </summary>
    public class ExpenseController : AdminBaseController
    {
        /// 整体，函授站收费标准和分配比例，艺术生收费标准设置
        public ActionResult EduFeeSyBiliArt()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// 专业收费标准
        public ActionResult MajorFeeSys(int id = 1)
        {
            #region “专业库管理”权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var nameStr = Request["y_name"];
                var edutype = Request["EduType"];
                var entityList = new List<VW_Major>();
                if (!string.IsNullOrWhiteSpace(nameStr) && !string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var name = nameStr.Trim();
                    var edutypeId = Convert.ToInt32(edutype);
                    var sql = " y_name like '" + name + "%'" + " and y_eduTypeId=" + edutypeId;
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    var dbLogList = entityList.OrderByDescending(u => u.id).ToPagedList(id, 15);
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorFeeSysList", dbLogList);
                    return View(dbLogList);
                }
                else if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    var name = nameStr.Trim();
                    var sql = " y_name like '" + name + "%'";
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    var dbLogList = entityList.OrderByDescending(u => u.id).ToPagedList(id, 15);
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorFeeSysList", dbLogList);
                    return View(dbLogList);
                }
                else if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var edutypeId = Convert.ToInt32(edutype);
                    var sql = " y_eduTypeId=" + edutypeId;
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    var dbLogList = entityList.OrderByDescending(u => u.id).ToPagedList(id, 15);
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorFeeSysList", dbLogList);
                    return View(dbLogList);
                }
                else
                {
                    var dbLogList = yunEntities.VW_Major.Where(u => true).OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorFeeSysList", dbLogList);
                    return View(dbLogList);
                }
            }
        }

        /// 专业缴费标准下载
        public ActionResult DownloadMajorLib()
        {
            #region “专业库管理”权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var nameStr = Request["y_name"];
                var edutype = Request["EduType"];
                var entityList = new List<VW_Major>();
                if (!string.IsNullOrWhiteSpace(nameStr) && !string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var name = nameStr.Trim();
                    var edutypeId = Convert.ToInt32(edutype);
                    var sql = " y_name like '" + name + "%'" + " and y_eduTypeId=" + edutypeId;
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    var name = nameStr.Trim();
                    var sql = " y_name like '" + name + "%'";
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var edutypeId = Convert.ToInt32(edutype);
                    var sql = " y_eduTypeId=" + edutypeId;
                    entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                }
                else
                {
                    entityList = yunEntities.VW_Major.Where(u => true).OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToList();
                }
                var model =
                    FileHelper.ToDataTable(
                        entityList.Select(u => new { majorLibraryName = u.y_name, eduTypeName = u.eduTypeName, stuTypeName = u.stuTypeName, y_needFee = u.y_needFee })
                            .ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/专业缴费标准表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable { { "majorLibraryName", "专业" }, { "eduTypeName", "层次" }, { "stuTypeName", "学习形式" }, { "y_needFee", "缴费金额" } };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('错误');window.location.href='" + reurl + "'</script>");
                }

            }
        }

        /// 函授站整体经费设置
        public ActionResult SubEduFeeSys(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var eduTypeId = Request["EduType"];
                var y_subSchoolId = Request["SubSchool"];
                IQueryable<YD_Fee_SubFeeSys> list = yunEntities.YD_Fee_SubFeeSys.OrderByDescending(u => u.id)
                    .Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                    .Include(u => u.YD_Edu_EduType).Where(u => true);
                if (!string.IsNullOrWhiteSpace(eduTypeId) && !eduTypeId.Equals("0"))
                {
                    int edutype = Convert.ToInt32(eduTypeId);
                    list = list.Where(u => u.y_eduTypeId == edutype);
                    ViewBag.edutype = edutype;
                }
                if (!string.IsNullOrWhiteSpace(y_subSchoolId) && !y_subSchoolId.Equals("0"))
                {
                    int subSchoolId = Convert.ToInt32(y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId == subSchoolId);
                    ViewBag.subschool = subSchoolId;
                }
                var dbLogList = list.Where(u => true).OrderByDescending(u => u.id).ThenByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("SubEduFeeSysList", dbLogList);
                return View(dbLogList);
            }
        }
        /// <summary>
        /// 函授站已通过注册统计人数学生查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult SubFeeStuTonjiDes(int? schoolId, int? majorlibId, int? edutypeId, int? inyear)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/SubFeeStuTonji");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            if (schoolId == null || majorlibId == null || edutypeId == null || inyear == null)
            {
                return RedirectToAction("SubFeeStuTonji");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major).OrderByDescending(u => u.id)
                        .Where(u => u.YD_Sts_StuInfo.y_inYear == inyear && u.YD_Sts_StuInfo.y_subSchoolId == schoolId && u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorlibId && u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeId && u.y_isCheckFee == 0);
                if (!list.Any())
                {
                    return RedirectToAction("SubFeeStuTonji");
                }
                ViewBag.admin = YdAdminRoleId;
                var idlist = list.Select(u => u.y_stuId);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var schoolids = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => schoolids.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var model = list.ToList();
                ViewBag.schoolId = schoolId;
                ViewBag.edutypeId = edutypeId;
                ViewBag.majorlibId = majorlibId;
                ViewBag.inyear = inyear;
                return View(model);
            }
        }

        /// <summary>
        /// 学生已注册统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadSubFeeStuTonjiDes()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/SubFeeStuTonji");
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
                var school = Request["schoolId"];
                var majorlib = Request["majorlibId"];
                var inyear = Request["inyearint"];
                var edutype = Request["edutypeId"];
                if (school == null || majorlib == null || edutype == null || inyear == null)
                {
                    return Content("未知错误");
                }
                var schoolId = Convert.ToInt32(school);
                var majorlibId = Convert.ToInt32(majorlib);
                var inyearint = Convert.ToInt32(inyear);
                var edutypeId = Convert.ToInt32(edutype);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id)
                                .Where(u => u.YD_Sts_StuInfo.y_inYear == inyearint && u.YD_Sts_StuInfo.y_subSchoolId == schoolId && u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorlibId && u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeId && u.y_isCheckFee == 0);
                if (!list.Any())
                {
                    return RedirectToAction("SubFeeStuTonji");
                }
                ViewBag.admin = YdAdminRoleId;
                var models = list.ToList();
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var listss = models.Select(
                    u =>
                        new FeeCheckStu
                        {
                            schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                            schoolCode = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_code,
                            y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                            y_name = u.YD_Sts_StuInfo.y_name,
                            y_inYear = u.YD_Sts_StuInfo.y_inYear,
                            majorName = u.YD_Sts_StuInfo.YD_Edu_Major.y_name,
                            y_tel = u.YD_Sts_StuInfo.y_tel,
                            y_address = u.YD_Sts_StuInfo.y_address,
                            y_cardId = u.YD_Sts_StuInfo.y_cardId,
                            y_stuYear = (int)u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear,
                            y_feeYear = u.y_feeYear,
                            y_needFee = u.y_needFee,
                            y_needUpFee = u.y_needUpFee,
                            y_isUp = u.y_isUp == 0 ? "已申请" : "未申请",
                            y_isCheckFee = u.y_isCheckFee == 0 ? "已通过" : "待审核"
                        }).OrderByDescending(u => u.y_stuNum)
                    .ToList();
                DataTable model = FileHelper.ToDataTable(listss);

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + "已注册学生名单" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                        {
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_stuNum", "学号"},
                            {"y_name", "姓名"},
                            {"y_inYear", "入学年份"},
                            {"majorName", "专业"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_cardId", "身份证号"},
                            {"y_stuYear", "学制"},
                            {"y_feeYear", "缴费学年"},
                            {"y_needFee", "学费"},
                            {"y_needUpFee", "实缴费用"},
                            {"y_isUp", "缴费状态"},
                            {"y_isCheckFee", "审核状态"}
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
        ///函授站已提交注册统计
        public ActionResult SubFeeStuTonji(int id = 1)
        {
            #region “函授站整体经费比例”权限验证
            var power = SafePowerPage("/Expense/SubFeeStuTonji");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            var inYear = Request["inYear"];
            var edutype = Request["y_eduTypeId"];
            var subschool = Request["subSchool"];
            var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                try
                {
                    var sql =
                        "select s.y_name as schoolname,s.id as schoolId,ISnull(stu.y_inYear,0) as inyear,edu.y_name as edutypename,edu.id as edutypeId,majorlib.y_name as majorlibname,majorlib.id as majorlibId,ma.y_stuYear as stuyear ,stu.rs,case when subfee.y_StuFee is not null then subfee.y_StuFee " +
                        "  else ma.y_needFee end as needfee,case when subli.y_bili is not null then subli.y_bili when ali.y_bili is not null then ali.y_bili " +
                        "  else 100 end as bili,cast(ISNULL(subfee.y_StuFee, ma.y_needFee) * ISNULL(isnull(subli.y_bili, ali.y_bili), 100) / 100 as decimal(18, 2)) as RealNeedfee " +
                        "  from YD_Sys_SubSchool as s left join (select stu.y_subSchoolId, stu.y_majorId from YD_Sts_StuInfo as stu " +
                        "  group by stu.y_subSchoolId, stu.y_majorId) as sub on sub.y_subSchoolId = s.id " +
                        "  left join (select m.y_name, m.id, m.y_eduTypeId, m.y_majorLibId, m.y_stuYear, m.y_needFee from YD_Edu_Major as m " +
                        "  group by m.id, m.y_name, m.y_eduTypeId, m.y_majorLibId, m.y_stuYear, m.y_needFee)as ma on ma.id = sub.y_majorId " +
                        "  left join (select e.id, e.y_name from YD_Edu_EduType as e group by e.id, e.y_name) as edu on edu.id = ma.y_eduTypeId " +
                        "  left join (select mali.id, mali.y_name from YD_Edu_MajorLibrary as mali group by mali.id, mali.y_name) as majorlib on majorlib.id = ma.y_majorLibId " +
                        "  left join(select stu.y_inYear, COUNT(*) as rs, stu.y_subSchoolId, stu.y_majorId from YD_Sts_StuInfo as stu " +
                        "  join YD_Fee_StuFeeTb as tb on tb.y_stuId = stu.id and tb.y_ischeckfee = 0  " +
                        "  group by stu.y_inYear, stu.y_subSchoolId, stu.y_majorId) as stu on stu.y_majorId = ma.id and stu.y_subSchoolId = s.id " +
                        "  left join(select subfee.y_subSchoolId, subfee.y_majorid, subfee.y_eduTypeId, subfee.y_StuFee  from YD_Fee_SubFeeSys as subfee " +
                        "  group by subfee.y_subSchoolId, subfee.y_majorid, subfee.y_eduTypeId, subfee.y_StuFee) as subfee on subfee.y_subSchoolId = s.id and subfee.y_eduTypeId = edu.id " +
                        "  left join(select subbi.y_subSchoolId, subbi.y_eduTypeId, subbi.y_bili from YD_Fee_SubFeeBili  as subbi " +
                        "  where subbi.y_Visible = 1 " +
                        "  group by subbi.y_subSchoolId, subbi.y_eduTypeId, subbi.y_bili)as subli on subli.y_subSchoolId = s.id and subli.y_eduTypeId = edu.id " +
                        "  left join(select ali.y_eduTypeId, ali.y_bili from YD_Fee_AllBili as ali where ali.y_Visible = 1 group by ali.y_eduTypeId, ali.y_bili) " +
                        "  as ali on ali.y_eduTypeId = edu.id  where stu.y_inYear!=0 " +
                        " order by s.y_name,edu.y_name";
                    var list = yunEntities.Database.SqlQuery<SubFeeStuTjShow>(sql).ToList();
                    if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                    {
                        var year = Convert.ToInt32(inYear);
                        list = list.Where(u => u.inyear == year).ToList();
                    }
                    else
                    {
                        list = list.Where(u => u.inyear == xinshenyear).ToList();

                    }
                    if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                    {
                        var edutypeId = Convert.ToInt32(edutype);
                        list = list.Where(u => u.edutypeId == edutypeId).ToList();
                        ViewBag.edutype = edutypeId;
                    }
                    if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                    {
                        var subschoolId = Convert.ToInt32(subschool);
                        list = list.Where(u => u.schoolId == subschoolId).ToList();
                        ViewBag.subschool = subschoolId;
                    }
                    return View(list);
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

            }
        }

        /// 函授站已通过注册统计下载
        public ActionResult DowonSubFeeStuTonji()
        {
            #region “函授站整体经费比例”权限验证
            var power = SafePowerPage("/Expense/SubFeeStuTonji");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            var inYear = Request["inYear"];
            var edutype = Request["y_eduTypeId"];
            var subschool = Request["subSchool"];
            var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            using (var yunEntities = new IYunEntities())
            {
                var sql =
                    "select s.y_name as schoolname,s.id as schoolId,ISnull(stu.y_inYear,0) as inyear,edu.y_name as edutypename,edu.id as edutypeId,majorlib.y_name as majorlibname,majorlib.id as majorlibId,ma.y_stuYear  as stuyear,stu.rs,case when subfee.y_StuFee is not null then subfee.y_StuFee " +
                    "  else ma.y_needFee end as needfee,case when subli.y_bili is not null then subli.y_bili when ali.y_bili is not null then ali.y_bili " +
                    "  else 100 end as bili,cast(ISNULL(subfee.y_StuFee, ma.y_needFee) * ISNULL(isnull(subli.y_bili, ali.y_bili), 100) / 100 as decimal(18, 2)) as RealNeedfee " +
                    "  from YD_Sys_SubSchool as s left join (select stu.y_subSchoolId, stu.y_majorId from YD_Sts_StuInfo as stu " +
                    "  group by stu.y_subSchoolId, stu.y_majorId) as sub on sub.y_subSchoolId = s.id " +
                    "  left join (select m.y_name, m.id, m.y_eduTypeId, m.y_majorLibId, m.y_stuYear, m.y_needFee from YD_Edu_Major as m " +
                    "  group by m.id, m.y_name, m.y_eduTypeId, m.y_majorLibId, m.y_stuYear, m.y_needFee)as ma on ma.id = sub.y_majorId " +
                    "  left join (select e.id, e.y_name from YD_Edu_EduType as e group by e.id, e.y_name) as edu on edu.id = ma.y_eduTypeId " +
                    "  left join (select mali.id, mali.y_name from YD_Edu_MajorLibrary as mali group by mali.id, mali.y_name) as majorlib on majorlib.id = ma.y_majorLibId " +
                    "  left join(select stu.y_inYear, COUNT(*) as rs, stu.y_subSchoolId, stu.y_majorId from YD_Sts_StuInfo as stu " +
                    "  join YD_Fee_StuFeeTb as tb on tb.y_stuId = stu.id and tb.y_ischeckfee = 0  " +
                    "  group by stu.y_inYear, stu.y_subSchoolId, stu.y_majorId) as stu on stu.y_majorId = ma.id and stu.y_subSchoolId = s.id " +
                    "  left join(select subfee.y_subSchoolId, subfee.y_majorid, subfee.y_eduTypeId, subfee.y_StuFee  from YD_Fee_SubFeeSys as subfee " +
                    "  group by subfee.y_subSchoolId, subfee.y_majorid, subfee.y_eduTypeId, subfee.y_StuFee) as subfee on subfee.y_subSchoolId = s.id and subfee.y_eduTypeId = edu.id " +
                    "  left join(select subbi.y_subSchoolId, subbi.y_eduTypeId, subbi.y_bili from YD_Fee_SubFeeBili  as subbi " +
                    "  where subbi.y_Visible = 1 " +
                    "  group by subbi.y_subSchoolId, subbi.y_eduTypeId, subbi.y_bili)as subli on subli.y_subSchoolId = s.id and subli.y_eduTypeId = edu.id " +
                    "  left join(select ali.y_eduTypeId, ali.y_bili from YD_Fee_AllBili as ali where ali.y_Visible = 1 group by ali.y_eduTypeId, ali.y_bili) " +
                    "  as ali on ali.y_eduTypeId = edu.id  where stu.y_inYear!=0 " +
                    " order by s.y_name,edu.y_name";
                var list = yunEntities.Database.SqlQuery<SubFeeStuTj>(sql).ToList();
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var year = Convert.ToInt32(inYear);
                    list = list.Where(u => u.inyear == year).ToList();
                }
                else
                {
                    list = list.Where(u => u.inyear == xinshenyear).ToList();
                }
                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var edutypeId = Convert.ToInt32(edutype);
                    var edu = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == edutypeId);
                    if (edu != null)
                        list = list.Where(u => u.edutypename == edu.y_name).ToList();
                }
                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    var subschoolId = Convert.ToInt32(subschool);
                    var sub = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subschoolId);
                    if (sub != null)
                        list = list.Where(u => u.schoolname == sub.y_name).ToList();
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new SubFeeStuTj
                                {
                                    schoolname = u.schoolname,
                                    inyear = u.inyear,
                                    edutypename = u.edutypename,
                                    majorlibname = u.majorlibname,
                                    stuyear = u.stuyear,
                                    rs = u.rs,
                                    needfee = u.needfee,
                                    bili = u.bili,
                                    RealNeedfee = u.RealNeedfee
                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站注册通过统计表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolname", "函授站"},
                        {"inyear", "年度"},
                        {"edutypename", "层次"},
                        {"majorlibname", "专业"},
                        {"stuyear", "学制"},
                        {"rs", "人数"},
                        {"needfee", "学费"},
                        {"bili", "比例"},
                        {"RealNeedfee","应缴费用"}
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

        /// 站点不同层次提交注册统计下载
        public ActionResult DownStuFeeBatMajor()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var batchid = Request["batchid"];
                if (batchid == null)
                {
                    return Content("未知错误");
                }
                var id = Convert.ToInt32(batchid);

                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }
                    var sql =
                         "select COUNT(case when  eduTypeName = '高起专' and tb.y_stuYear = 3 and majoredu.isys = 1 then '高起专(普通/3)'  end) as specomthree," +
                         " COUNT(case when  eduTypeName = '高起专' and tb.y_stuYear = 3 and majoredu.isys = 2 then '高起专(艺术/3)'  end) as speartthree," +
                         " COUNT(case when  eduTypeName = '高起专' and tb.y_stuYear = 4 and majoredu.isys = 1 then '高起专(普通/4)'  end) as specomfour," +
                         " COUNT(case when  eduTypeName = '专升本' and tb.y_stuYear = 3 and majoredu.isys = 1 then '专升本(普通/3)'  end) as speupgracomthree," +
                         " COUNT(case when  eduTypeName = '专升本' and tb.y_stuYear = 3 and majoredu.isys = 2 then '专升本(艺术/3)'  end) as speupgraartthree," +
                         " COUNT(case when  eduTypeName = '高起本' and tb.y_stuYear = 5 and majoredu.isys = 1 then '高起本(普通/5)'  end) as thiscomfive" +
                         " from VW_StuInfo as tb  left join(select y_eduTypeId, y_stuYear, isys, id from" +
                         " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                         " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on tb.y_majorId = majoredu.id" +
                         " left join YD_Fee_StuFeeTb as  fee on tb.id=fee.y_stuId" +
                         " where tb.id in (" + idlist + ") " +
                         " and fee.y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear;

                    var list = yunEntities.Database.SqlQuery<MajorEduStu>(sql).ToList();
                    var models = list.ToList();
                    var schoolName = ydFeeStuRegistrBatch.schoolName;
                    var inyear = ydFeeStuRegistrBatch.y_inyear;
                    var listss = models.Select(
                        u =>
                            new
                            {
                                schoolName = schoolName,
                                specomthree = u.specomthree,
                                speartthree = u.speartthree,
                                specomfour = u.specomfour,
                                speupgracomthree = u.speupgracomthree,
                                speupgraartthree = u.speupgraartthree,
                                thiscomfive = u.thiscomfive,
                                total = ydFeeStuRegistrBatch.totalcount

                            }).ToList();
                    DataTable model = FileHelper.ToDataTable(listss);

                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/" + schoolName + inyear + "站点层次注册统计" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变

                    //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                    //var fileName2 = "~/" + filename1;
                    //var fileName3 = Server.MapPath(fileName2);
                    using (var excelHelper = new ExcelHelper(fileName3))
                    {
                        var ht = new Hashtable
                        {
                            {"schoolName", "函授站"},
                            {"specomthree", "高起专(普通/3年)"},
                            {"speartthree", "高起专(艺术/3年)"},
                            {"specomfour", "高起专(普通/4年)"},
                            {"speupgracomthree", "专升本(普通/3年)"},
                            {"speupgraartthree", "专升本(艺术/3年)"},
                            {"thiscomfive", "高起本(普通/5年)"},
                            {"total", "总人数"}
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
                else
                {
                    return Content("找不到学生名单");
                }
            }
        }

        /// 函授站经费添加视图
        public ActionResult AddSubFee()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var major = yunEntities.YD_Edu_Major
                    .Include(u => u.YD_Edu_MajorLibrary)
                    .Include(u => u.YD_Edu_EduType).OrderByDescending(u => u.y_majorLibId).ToList();
                ViewBag.major = major;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// 整体学费保存AJAX
        public string SaveSubFee(YD_Fee_SubFeeSys stu)
        {
            var re = new Hashtable();
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
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
            using (var yunEntities = new IYunEntities())
            {
                if (yunEntities.YD_Fee_SubFeeSys.Any(
                    u => u.y_eduTypeId == stu.y_eduTypeId && u.y_subSchoolId == stu.y_subSchoolId && u.y_majorid == stu.y_majorid))
                {
                    re["msg"] = "已存在该函授站年度缴费记录";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (stu.y_majorid == 0)
                {
                    stu.y_majorid = null;
                }
                yunEntities.Entry(stu).State = EntityState.Added;
                int i = yunEntities.SaveChanges();
                if (i > 0)
                {
                    LogHelper.WriteInfoLog(typeof(LogType), "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",整体学费保存,添加函授站学费设置表,ID:" + stu.id + ",方法:SaveSubFee");

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

        /// 整体学费删除AJAX
        public string DeleSubFeeById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
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
                var smallpower = ad.YD_Fee_SubFeeSys.FirstOrDefault(u => u.id == id);
                ad.Entry(smallpower).State = EntityState.Deleted;
                var msg = ad.SaveChanges();
                if (msg > 0)
                {
                    LogHelper.WriteInfoLog(typeof(LogType), "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",整体学费删除,删除函授站学费设置表信息,ID:" + id + ",方法:DeleSubFeeById");

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

        /// 整体比例
        public ActionResult AllBili(int id = 1)
        {
            #region “专业库管理”权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var eduTypeId = Request["y_eduTypeId"];
                //IQueryable<YD_Fee_AllBili> list =
                //    yunEntities.YD_Fee_AllBili.Include(u => u.YD_Edu_EduType).OrderByDescending(u => u.id).Where(u => u.y_Visible == true);
                IQueryable<YD_Fee_AllBili> list =
                    yunEntities.YD_Fee_AllBili.Include(u => u.YD_Edu_EduType)
                    .OrderByDescending(u => u.id).Where(u => u.y_Visible == true);
                if (!string.IsNullOrWhiteSpace(eduTypeId) && !eduTypeId.Equals("0"))
                {
                    int edutype = Convert.ToInt32(eduTypeId);
                    list = list.Where(u => u.y_eduTypeId == edutype);
                    ViewBag.edutype = edutype;
                }
                var dbLogList =
                    list.Where(u => true).OrderByDescending(u => u.id).ThenByDescending(u => u.id).ToPagedList(id, 15);
                //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("AllBiliList", dbLogList);
                return View(dbLogList);
            }
        }

        /// 整体比例添加视图
        public ActionResult AddAllBili()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
            }

            return View();
        }

        /// 整体比例保存AJAX
        public string SaveAllBili(YD_Fee_AllBili stu)
        {
            var re = new Hashtable();
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
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
            using (var yunEntities = new IYunEntities())
            {
                stu.y_feeYear = 1;
                stu.y_time = DateTime.Now;
                stu.y_Visible = true;
                var allbili =
                    yunEntities.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stu.y_eduTypeId && u.y_feeYear == stu.y_feeYear);
                if (allbili != null && allbili.y_Visible)
                {
                    allbili.y_Visible = false;
                    yunEntities.Entry(allbili).State = EntityState.Modified;

                    yunEntities.Entry(stu).State = EntityState.Added;
                    re["msg"] = "添加成功";
                    re["isok"] = true;
                    //re["msg"] = "已存在该缴费比例记录";
                    //re["isok"] = false;
                    //return JsonConvert.SerializeObject(re);
                }
                else
                {
                    yunEntities.Entry(stu).State = EntityState.Added;
                }
                int i = yunEntities.SaveChanges();
                if (i > 0)
                {
                    #region 更新了整体的缴费比例后，将未审核的学生缴费情况进行重新计算应上缴的费用  --没用
                    //查询出需要重新计算应上缴费用的学生  已审核缴费的学生不进行重新计算
                    //var stuList = yunEntities.YD_Fee_StuFeeTb.Where(
                    //    u =>
                    //        u.y_feeYear == stu.y_feeYear  &&
                    //        u.y_eduTypeId == stu.y_eduTypeId && u.y_inYear == stu.y_inYear &&
                    //        u.y_isCheckFee == (int)YesOrNo.No);
                    //var subBiliList = yunEntities.YD_Fee_SubFeeBili.Where(u => true);
                    //stuList = Enumerable.Aggregate(subBiliList, stuList,
                    //    (current, ydFeeSubFeeBili) =>
                    //        current.Where(
                    //            u =>
                    //                u.y_subSchoolId != ydFeeSubFeeBili.y_subSchoolId &&
                    //                u.y_eduTypeId != ydFeeSubFeeBili.y_eduTypeId &&
                    //                u.y_stuTypeId != ydFeeSubFeeBili.y_stuTypeId &&
                    //                u.y_feeYear != ydFeeSubFeeBili.y_feeYear && u.y_inYear != ydFeeSubFeeBili.y_inYear));
                    //var stuLists = stuList.ToList();
                    //for (int j = 0; j < stuLists.Count(); j++)
                    //{
                    //    stuLists[j].y_needUpFee = stuLists[j].y_needFee * stu.y_bili / 100;
                    //    yunEntities.Entry(stuLists[j]).State = EntityState.Modified;
                    //}
                    //int t = yunEntities.SaveChanges();
                    //if (t > 0)
                    //{
                    //    re["msg"] = "添加成功";
                    //    re["isok"] = true;
                    //}
                    //else
                    //{
                    //    re["msg"] = "添加比例成功，但是修改学生的上缴费用失败";
                    //    re["isok"] = true;
                    //}
                    #endregion

                    LogHelper.WriteInfoLog(typeof(LogType), "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",整体比例保存,添加整体学费比例,ID:" + stu.id + ",方法:SaveAllBili");

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

        /// 整体比例删除AJAX
        public string DeleAllBiliById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
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
                var smallpower = ad.YD_Fee_AllBili.FirstOrDefault(u => u.id == id);
                ad.Entry(smallpower).State = EntityState.Deleted;
                var msg = ad.SaveChanges();
                if (msg > 0)
                {
                    LogHelper.WriteInfoLog(typeof(LogType), "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",删除整体学费比例,ID:" + smallpower.id + ",方法:DeleAllBiliById");

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

        #region 函授站分成比例设置
        public class StuSubFeeBili
        {
            public int schoolId { get; set; }
            public int y_eduTypeId { get; set; }
            public string schoolName { get; set; }
            public string EduName { get; set; }
            public int? y_bili { get; set; }
        }
        public class StuSubAllBili
        {
            public int id { get; set; }
            public int? y_bili { get; set; }
        }
        /// <summary>
        /// 函授站分成比例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubFeeBili(int id = 1)
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var subSchool = Request["SubSchool"];
                var sql = "";
                var sublist = new List<StuSubFeeBili>();
                var sub = yunEntities.YD_Sys_SubSchool.Where(u => true).ToList();
                if (sub.Any())
                {
                    sql = "select d.*,s.y_bili from YD_Fee_SubFeeBili as s " +
                        "right join ( select YD_Sys_SubSchool.y_name as schoolName," +
                        "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId," +
                        "YD_Edu_EduType.id as EduId from YD_Sys_SubSchool  full join  YD_Edu_EduType" +
                        " on 1=1 ) as d on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId " +
                        "  and y_Visible=1" +
                        " order by d.schoolId,d.EduId";
                    sublist = yunEntities.Database.SqlQuery<StuSubFeeBili>(sql).ToList();
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subschoolId = Convert.ToInt32(subSchool);
                    sql = "select d.*,s.y_bili from YD_Fee_SubFeeBili as s " +
                          "right join ( select YD_Sys_SubSchool.y_name as schoolName," +
                          "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId," +
                          "YD_Edu_EduType.id as EduId from YD_Sys_SubSchool  full join  YD_Edu_EduType" +
                          " on 1=1 ) as d on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId " +
                          " and y_Visible=1" + "  where schoolId=" + subschoolId +
                          " order by d.schoolId,d.EduId";
                    sublist = yunEntities.Database.SqlQuery<StuSubFeeBili>(sql).ToList();
                    ViewBag.subschoolId = subschoolId;
                }
                ViewBag.sublist = sublist;

                var edutypelist = yunEntities.YD_Edu_EduType.ToList();
                ViewBag.edutypelist = edutypelist;

                //整体缴费比例
                var allbilisql =
                    "select e.id,c.y_bili from YD_Edu_EduType as e " +
                    "left join (select a.*from [YD_Fee_AllBili]  as a " +
                    "join (select MAX(y_time) as y_time,y_eduTypeId from [YD_Fee_AllBili] group by y_eduTypeId) as b " +
                    "on b.y_eduTypeId = a.y_eduTypeId and a.y_time = b.y_time  ) as c on c.y_eduTypeId = e.id order by e.id";
                var allbililist = yunEntities.Database.SqlQuery<StuSubAllBili>(allbilisql).ToList();
                ViewBag.allbililist = allbililist;

                //var dbLogList = sublist.ToPagedList(id, 15);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                //if (Request.IsAjaxRequest())
                //    return PartialView("SubFeeBiliList", dbLogList);
                return View();
            }
        }

        /// <summary>
        /// 更新学生缴费金额
        /// </summary>
        /// <returns></returns>
        public string UpdateStuSubFeeBili()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var tallthisStr = Request["tallthis"].Split(new[] { "<>" }, StringSplitOptions.None);  //高起本
            var expertthisStr = Request["expertthis"].Split(new[] { "<>" }, StringSplitOptions.None); //高起专
            var experttallStr = Request["experttall"].Split(new[] { "<>" }, StringSplitOptions.None);  //专升本
            var subIdStr = Request["subId"].Split(new[] { "<>" }, StringSplitOptions.None);
            using (var yunEntities = new IYunEntities())
            {
                var edutype = yunEntities.YD_Edu_EduType.Where(u => true).ToList();
                for (var i = 0; i < subIdStr.Count(); i++)
                {
                    var subId = Convert.ToInt32(subIdStr[i]);
                    if (tallthisStr[i] != "" && expertthisStr[i] != "" && experttallStr[i] != "")
                    {
                        var tallthis = Convert.ToInt32(tallthisStr[i]);
                        var expertthis = Convert.ToInt32(expertthisStr[i]);
                        var experttall = Convert.ToInt32(experttallStr[i]);

                        YD_Fee_SubFeeBili mys;
                        foreach (var edu in edutype)
                        {
                            var bili =
                                yunEntities.YD_Fee_SubFeeBili.Where(
                                    u => u.y_subSchoolId == subId && u.y_eduTypeId == edu.id && u.y_Visible);
                            if (bili.Any())
                            {
                                bili.ToList().ForEach(u =>
                                {
                                    u.y_Visible = false;
                                    yunEntities.Entry(u).State = EntityState.Modified;
                                }); //删除不显示
                            }
                            mys = new YD_Fee_SubFeeBili
                            {
                                y_Visible = true,
                                y_subSchoolId = subId,
                                y_eduTypeId = edu.id,
                                y_bili = tallthis,
                                y_time = DateTime.Now
                            };
                            if (edu.y_name.Equals("高起专"))
                            {
                                mys.y_bili = expertthis;
                            }
                            if (edu.y_name.Equals("专升本"))
                            {
                                mys.y_bili = experttall;
                            }
                            yunEntities.Entry(mys).State = EntityState.Added;
                        }
                    }
                }
                var t = yunEntities.SaveChanges();
                return t > 0 ? "ok" : "保存错误";
            }
        }
        /// <summary>
        /// 导出函授站学生统计表
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadSubStuBili()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            var subSchool = Request["SubSchool"];
            using (var yunEntities = new IYunEntities())
            {
                var sql = "select d.*,s.y_bili from YD_Fee_SubFeeBili as s " +
                          "right join ( select YD_Sys_SubSchool.y_name as schoolName," +
                          "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId," +
                          "YD_Edu_EduType.id as EduId from YD_Sys_SubSchool  full join  YD_Edu_EduType" +
                          " on 1=1 ) as d on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId " +
                          "  and y_Visible=1" +
                          " order by d.schoolId,d.EduId";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subschoolId = Convert.ToInt32(subSchool);
                    sql = "select d.*,s.y_bili from YD_Fee_SubFeeBili as s " +
                          "right join ( select YD_Sys_SubSchool.y_name as schoolName," +
                          "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId," +
                          "YD_Edu_EduType.id as EduId from YD_Sys_SubSchool  full join  YD_Edu_EduType" +
                          " on 1=1 ) as d on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId " +
                          "  and y_Visible=1" + "  where schoolId=" + subschoolId +
                          " order by d.schoolId,d.EduId";
                }
                ViewBag.hasStu = true;
                var list = yunEntities.Database.SqlQuery<StuSubFeeBili>(sql);
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    u.schoolName,
                                    u.EduName,
                                    u.y_bili
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/函授站缴费比例表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"EduName","层次"},
                        {"y_bili","比例"}
                    };
                    var t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        var url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                }
            }
            var reurl = Request.UrlReferrer.ToString();
            return Content("alert('错误');window.location.href='" + reurl + "';");
        }
        /// <summary>
        /// 函授站比例添加视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult AddFeeBili()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
            }

            return View();
        }

        #region 整体学费保存AJAX

        /// <summary>
        /// 整体学费保存AJAX
        /// </summary>
        /// <param name="stu"></param>
        /// <returns>处理结果json</returns>
        public string SaveFeeBili(YD_Fee_SubFeeBili stu)
        {
            var re = new Hashtable();

            #region 权限验证

            var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
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
            var subSchool = Request["y_subSchoolId"];

            using (var yunEntities = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    stu.y_subSchoolId = subSchoolid;
                    yunEntities.Entry(stu).State = EntityState.Modified;
                }
                if (yunEntities.YD_Fee_SubFeeBili.Any(
                    u =>
                        u.y_eduTypeId == stu.y_eduTypeId && u.y_inYear == stu.y_inYear &&
                        u.y_stuTypeId == stu.y_stuTypeId && u.y_subSchoolId == stu.y_subSchoolId &&
                        u.y_feeYear == stu.y_feeYear))
                {
                    re["msg"] = "已存在该函授站缴费比例记录";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                yunEntities.Entry(stu).State = EntityState.Added; yunEntities.Entry(stu).State = EntityState.Added;
                int i = yunEntities.SaveChanges();
                if (i > 0)
                {
                    #region 更新了函授站的缴费比例后，将未审核的学生缴费情况进行重新计算应上缴的费用

                    //查询出需要重新计算应上缴费用的学生  已审核缴费的学生不进行重新计算
                    var stuList = yunEntities.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.y_feeYear == stu.y_feeYear && u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stu.y_stuTypeId &&
                            u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == stu.y_eduTypeId &&
                            u.YD_Sts_StuInfo.y_inYear == stu.y_inYear &&
                            u.y_isCheckFee == (int)YesOrNo.No && u.YD_Sts_StuInfo.y_subSchoolId == stu.y_subSchoolId);
                    var stuLists = stuList.ToList();
                    for (int j = 0; j < stuLists.Count(); j++)
                    {
                        stuLists[j].y_needUpFee = stuLists[j].y_needFee * stu.y_bili / 100;
                        yunEntities.Entry(stuLists[j]).State = EntityState.Modified;
                    }
                    int t = yunEntities.SaveChanges();
                    //edit by dc 2016-03-02
                    //如果没有数据可以更新，也提示添加成功3
                    if (t > 0 || (stuList.Count() + t == 0))
                    {
                        re["msg"] = "添加成功";
                        re["isok"] = true;
                    }
                    else
                    {
                        re["msg"] = "添加分成比例成功，但是修改学生的上缴费用失败";
                        re["isok"] = true;
                    }

                    #endregion
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

        #region 函授站比例删除AJAX

        ///// <summary>
        ///// 整体学费删除AJAX
        ///// </summary>
        ///// <param name="id">学生id</param>
        ///// <returns>处理结果json</returns>
        //public string DeleFeeBiliById(int id)
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Expense/EduFeeSyBiliArt");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_delete == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    using (var ad = new IYunEntities())
        //    {


        //        var re = new Hashtable();
        //        var smallpower = ad.YD_Fee_SubFeeBili.FirstOrDefault(u => u.id == id);
        //        ad.Entry(smallpower).State = EntityState.Deleted;
        //        var msg = ad.SaveChanges();
        //        if (msg > 0)
        //        {
        //            re["msg"] = "删除成功";
        //            re["isok"] = true;
        //        }
        //        else
        //        {
        //            re["msg"] = "删除失败";
        //            re["isok"] = false;
        //        }
        //        return JsonConvert.SerializeObject(re);
        //    }
        //}

        #endregion

        #endregion
        /// 将学生注册到指定的函授站
        public string StuRegisterSub()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/FeeManage");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var id = Request["ids"];
            var subSchoolId = Request["SubSchool2"];
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(subSchoolId))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
                    if (stuFee != null)
                    {
                        var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
                        if (obj.y_subSchoolId != null)
                        {
                            return "你选择的学生中已经存在函授站信息，不能注册到其他函授站";
                        }
                        //将函授站关联到注册到的函授站，将第一学年注册好，且将学籍状态改为在读。
                        obj.y_subSchoolId = Convert.ToInt32(subSchoolId);
                        obj.y_stuStateId = 1;
                        obj.y_registerState = "[1][2]";
                        yunEntities.Entry(obj).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(typeof(LogType), "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",将学生注册到指定的函授站,修改学生信息,ID:" + obj.id + ",方法:StuRegisterSub");
                    }
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return "注册成功";
                }
                else
                {
                    return "注册失败";
                }

            }
        }

        /// <summary>
        /// 审核注册名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ApplyRegister(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var subSchool = Request["SubSchool"];
                var inYear = Request["inYear"];
                var ischeck = Request["isCheck"];
                var term = Request["term"];
                var year = Request["year"];
                var intyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                IQueryable<YD_Fee_StuRegistrBatch> list =
                   yunEntities.YD_Fee_StuRegistrBatch.OrderByDescending(u => u.id).Where(u => true);

                ViewBag.admin = YdAdminRoleId;

                var lists = new List<BiliDto>();
                if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                {
                    #region  注释掉的实时刷新
                    var ids = string.Join("", list.Select(u => u.y_stuid).ToArray());
                    if (ids.Any())
                    {
                        ids = ids.Substring(0, ids.Length - 1);
                        var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                                "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                                "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                                ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                                "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                                "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                                "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                                "from VW_StuInfo where id in (" + ids + ") " +
                                //"group by y_majorId,y_subSchoolId,y_eduTypeId" +
                                ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                                "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                                "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                                "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                                "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                                "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                                "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                                "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";



                        yunEntities.Database.CommandTimeout = 10000;

                        lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    }
                    #endregion
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    ViewBag.intyear = intyear;
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yearint = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_inyear == yearint);
                    ViewBag.intyear = yearint;
                }
                if (string.IsNullOrWhiteSpace(year))
                {
                    ViewBag.yearint = intyear;
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(inYear);
                    var yearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_feeyear == (yearint - inyear) + 1);
                    ViewBag.yearint = yearint;
                }
                //if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                //{
                //    var feelyeear = Convert.ToInt32(term);
                //    list = list.Where(u => u.y_feeyear == feelyeear);
                //}
                if (!string.IsNullOrWhiteSpace(ischeck) && ischeck != "0")
                {

                    if (ischeck == "2") //审核通过  
                    {
                        list = list.Where(u => (u.y_check == 1));
                    }
                    else if (ischeck == "3") //审核不通过
                    {
                        list = list.Where(u => (u.y_check == 2));
                    }
                    else
                    {
                        list = list.Where(u => (u.y_check == 0));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId));
                }
                ViewBag.admin = YdAdminRoleId;

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize    
                if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                {
                    #region 注释掉的算计总金额
                    model.ForEach(u =>
                {
                    if ((u.y_check == 0 || u.y_check == 2))
                    {
                        var idlist = Array.ConvertAll(u.y_stuid.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);

                        u.needtotal = lists.Where(k => idlist.Contains(k.id)).Sum(k => k.money);
                        u.tuitiontotal = lists.Where(k => idlist.Contains(k.id)).Sum(k => k.y_needFee);
                    }
                });
                    #endregion
                }


                if (Request.IsAjaxRequest())
                    return PartialView("PartialApplyRegisterList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 没有提交注册名单统计
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyNoRegister(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.adminroleid = YdAdminRoleId;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }


        /// <summary>
        /// 全校未提交注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var inyear = Request["inYear"];
                ViewBag.adminroleid = YdAdminRoleId;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                if (string.IsNullOrWhiteSpace(inyear) || inyear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inyear);
                }
                ViewBag.inyear = year;
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear).ToList();
                var feelist = new List<StuNoRegister>();
                if (list.Any())
                {
                    var data = new StuNoRegister();
                    data.y_inyear = year;
                    data.total = list.Count();
                    data.regsterno = list.Count(u => u.y_isUp == 1);
                    data.regsteryes = list.Count(u => u.y_isUp == 0);
                    data.regstercheck = list.Count(u => u.y_isCheckFee == 0);
                    feelist.Add(data);
                }
                return View(feelist);
            }
        }
        /// <summary>
        /// 全校未提交注册统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownStuNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var inYear = Request["inYear"];
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                if (string.IsNullOrWhiteSpace(inYear) || inYear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inYear);
                }
                ViewBag.inyear = year;
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear).ToList();
                var feelist = new List<StuNoRegister>();
                if (list.Any())
                {
                    var data = new StuNoRegister();
                    data.y_inyear = year;
                    data.total = list.Count();
                    data.regsterno = list.Count(u => u.y_isUp == 1);
                    data.regsteryes = list.Count(u => u.y_isUp == 0);
                    data.regstercheck = list.Count(u => u.y_isCheckFee == 0);
                    feelist.Add(data);
                }
                var models = feelist.ToList();
                DataTable model;
                model =
                FileHelper.ToDataTable(
                    models.Select(
                        u =>
                            new StuNoRegister
                            {
                                y_inyear = u.y_inyear,
                                total = u.total,
                                regsterno = u.regsterno,
                                regsteryes = u.regsteryes,
                                regstercheck = u.regstercheck

                            }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/" + "全校未提交统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变
                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inyear", "入学年份"},
                        {"total","总人数"},
                        {"regsterno","未提交人数"},
                        {"regsteryes","已提交人数"},
                        {"regstercheck","已审核人数"}
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
        ///本部未提交注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SubMainNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var inyear = Request["inYear"];
                ViewBag.adminroleid = YdAdminRoleId;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                if (string.IsNullOrWhiteSpace(inyear) || inyear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inyear);
                }
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                ViewBag.inyear = year;
                var sublist = yunEntities.YD_Sys_SubSchool.Where(u => u.y_schooltype == 1).ToList();
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear && u.YD_Sts_StuInfo.y_subSchoolId != null).ToList();

                var feelist = new List<SubMainNoRegister>();
                foreach (var sub in sublist)
                {
                    var data = new SubMainNoRegister();
                    var schoolstulist = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == sub.id).ToList();
                    if (schoolstulist.Any())
                    {

                        data.schoolname = sub.y_name;
                        data.y_inyear = year;
                        data.total = schoolstulist.Count();
                        data.regsterno = schoolstulist.Count(u => u.y_isUp == 1);
                        data.regsteryes = schoolstulist.Count(u => u.y_isUp == 0);
                        data.regstercheck = schoolstulist.Count(u => u.y_isCheckFee == 0);
                        feelist.Add(data);
                    }
                }
                return View(feelist);
            }
        }
        /// <summary>
        /// 本部未提交注册统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownSubMainNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var inYear = Request["inYear"];
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                if (string.IsNullOrWhiteSpace(inYear) || inYear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inYear);
                }
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                ViewBag.inyear = year;
                var sublist = yunEntities.YD_Sys_SubSchool.Where(u => u.y_schooltype == 1).ToList();
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear && u.YD_Sts_StuInfo.y_subSchoolId != null).ToList();

                var feelist = new List<SubMainNoRegister>();
                foreach (var sub in sublist)
                {
                    var data = new SubMainNoRegister();

                    data.schoolname = sub.y_name;
                    var schoolstulist = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == sub.id).ToList();
                    if (schoolstulist.Any())
                    {
                        data.y_inyear = year;
                        data.total = schoolstulist.Count();
                        data.regsterno = schoolstulist.Count(u => u.y_isUp == 1);
                        data.regsteryes = schoolstulist.Count(u => u.y_isUp == 0);
                        data.regstercheck = schoolstulist.Count(u => u.y_isCheckFee == 0);
                        feelist.Add(data);
                    }
                }
                var models = feelist.ToList();
                DataTable model;
                model =
                FileHelper.ToDataTable(
                    models.Select(
                        u =>
                            new SubMainNoRegister
                            {
                                y_inyear = u.y_inyear,
                                schoolname = u.schoolname,
                                total = u.total,
                                regsterno = u.regsterno,
                                regsteryes = u.regsteryes,
                                regstercheck = u.regstercheck

                            }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/" + "本部未提交统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变
                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inyear", "入学年份"},
                        {"schoolname","函授站"},
                        {"total","总人数"},
                        {"regsterno","未提交人数"},
                        {"regsteryes","已提交人数"},
                        {"regstercheck","已审核人数"}
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
        ///函授站未提交注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SubNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var inyear = Request["inYear"];
                ViewBag.adminroleid = YdAdminRoleId;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                if (string.IsNullOrWhiteSpace(inyear) || inyear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inyear);
                }
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                ViewBag.inyear = year;
                var sublist = yunEntities.YD_Sys_SubSchool.Where(u => u.y_schooltype != 1).ToList();
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear && u.YD_Sts_StuInfo.y_subSchoolId != null).ToList();

                var feelist = new List<SubMainNoRegister>();
                foreach (var sub in sublist)
                {
                    var data = new SubMainNoRegister();
                    var schoolstulist = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == sub.id).ToList();
                    if (schoolstulist.Any())
                    {

                        data.schoolname = sub.y_name;
                        data.y_inyear = year;
                        data.total = schoolstulist.Count();
                        data.regsterno = schoolstulist.Count(u => u.y_isUp == 1);
                        data.regsteryes = schoolstulist.Count(u => u.y_isUp == 0);
                        data.regstercheck = schoolstulist.Count(u => u.y_isCheckFee == 0);
                        feelist.Add(data);
                    }
                }
                return View(feelist);
            }
        }
        /// <summary>
        /// 函授站未提交注册统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownSubNoRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var inYear = Request["inYear"];
                int year = 0;
                int xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                if (string.IsNullOrWhiteSpace(inYear) || inYear.Equals("0"))
                {
                    year = xinshenyear;
                }
                else
                {
                    year = Convert.ToInt32(inYear);
                }
                int feeyear = (xinshenyear + 1) - year; //缴费学年
                ViewBag.inyear = year;
                var sublist = yunEntities.YD_Sys_SubSchool.Where(u => u.y_schooltype != 1).ToList();
                var list = yunEntities.YD_Fee_StuFeeTb.Where(u => u.YD_Sts_StuInfo.y_inYear == year && u.y_feeYear == feeyear && u.YD_Sts_StuInfo.y_subSchoolId != null).ToList();

                var feelist = new List<SubMainNoRegister>();
                foreach (var sub in sublist)
                {
                    var data = new SubMainNoRegister();

                    data.schoolname = sub.y_name;
                    var schoolstulist = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == sub.id).ToList();
                    if (schoolstulist.Any())
                    {
                        data.y_inyear = year;
                        data.total = schoolstulist.Count();
                        data.regsterno = schoolstulist.Count(u => u.y_isUp == 1);
                        data.regsteryes = schoolstulist.Count(u => u.y_isUp == 0);
                        data.regstercheck = schoolstulist.Count(u => u.y_isCheckFee == 0);
                        feelist.Add(data);
                    }
                }
                var models = feelist.ToList();
                DataTable model;
                model =
                FileHelper.ToDataTable(
                    models.Select(
                        u =>
                            new SubMainNoRegister
                            {
                                y_inyear = u.y_inyear,
                                schoolname = u.schoolname,
                                total = u.total,
                                regsterno = u.regsterno,
                                regsteryes = u.regsteryes,
                                regstercheck = u.regstercheck

                            }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/" + "函授站未提交统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变
                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inyear", "入学年份"},
                        {"schoolname","函授站" },
                        {"total","总人数"},
                        {"regsterno","未提交人数"},
                        {"regsteryes","已提交人数"},
                        {"regstercheck","已审核人数"}
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
        /// 学生缴费审核下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownApplyRegister()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var subSchool = Request["SubSchool"];
                var inYear = Request["inYear"];
                var ischeck = Request["isCheck"];
                var term = Request["term"];
                var year = Request["year"];
                IQueryable<YD_Fee_StuRegistrBatch> list =
                  yunEntities.YD_Fee_StuRegistrBatch.OrderByDescending(u => u.id).Where(u => true);
                ViewBag.admin = YdAdminRoleId;
                var ids = string.Join("", list.Select(u => u.y_stuid).ToArray());
                var lists = new List<BiliDto>();
                if (ids.Any())
                {
                    ids = ids.Substring(0, ids.Length - 1);
                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                                 "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                                 "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                                 ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                                 "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                                 "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                                 "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                                 "from VW_StuInfo where id in (" + ids + ") " +
                                 //"group by y_majorId,y_subSchoolId,y_eduTypeId" +
                                 ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                                 "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                                 "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                                 "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                                 "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                                 "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                                 "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                                 "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";
                    lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                }
                var schoolName = "";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    var schoolfirst = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    schoolName = schoolfirst != null ? schoolfirst.y_name : "";
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yearint = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_inyear == yearint);
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(inYear);
                    var yearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_feeyear == (yearint - inyear) + 1);
                }
                if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                {
                    var feelyeear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeyear == feelyeear);
                }
                if (!string.IsNullOrWhiteSpace(ischeck) && ischeck != "0")
                {
                    if (ischeck == "2") //审核通过  
                    {
                        list = list.Where(u => (u.y_check == 1));
                    }
                    else if (ischeck == "3") //审核不通过
                    {
                        list = list.Where(u => (u.y_check == 2));
                    }
                    else
                    {
                        list = list.Where(u => (u.y_check == 0));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId));
                }
                var models = list.ToList(); //id为pageindex   15 为pagesize    
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                models.ForEach(u =>
                {
                    if ((u.y_check == 0 || u.y_check == 2))
                    {
                        var idlist = Array.ConvertAll(u.y_stuid.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);

                        u.needtotal = lists.Where(k => idlist.Contains(k.id)).Sum(k => k.money);
                        u.tuitiontotal = lists.Where(k => idlist.Contains(k.id)).Sum(k => k.y_needFee);
                    }
                });
                DataTable model = new DataTable();
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.DHLGDX.ToString())
                {
                    var listss = models.Select(
                    u =>
                        new DHLGBatchCheck
                        {
                            schoolName = u.schoolName,
                            y_inyear = (int)u.y_inyear,
                            totalcount = u.totalcount,
                            tuitiontotal = (decimal)u.tuitiontotal,
                            needtotal = (decimal)u.needtotal,
                            y_time = u.y_time,
                            y_check = u.y_check == 1 ? "已审核" : u.y_check == 2 ? "审核不通过" : "待审核",
                            governorName = u.governorName
                        }).ToList();
                    var tuitiontotals = listss.Sum(u => u.tuitiontotal);
                    var needtotal = listss.Sum(u => u.tuitiontotal);
                    int count = listss.Sum(u => u.totalcount);
                    listss.Add(new DHLGBatchCheck
                    {
                        schoolName = "合计",
                        y_inyear = 0,
                        totalcount = count,
                        tuitiontotal = tuitiontotals,
                        needtotal = needtotal,
                        y_time = DateTime.Now,
                        y_check = "",
                        governorName = ""
                    });
                    model = FileHelper.ToDataTable(listss);
                }
                else
                {
                    var listss = models.Select(
                    u =>
                        new BatchCheck
                        {
                            schoolName = u.schoolName,
                            y_inyear = (int)u.y_inyear,
                            totalcount = u.totalcount,
                            tuitiontotal = (decimal)u.tuitiontotal,
                            needtotal = (decimal)u.needtotal,
                            y_time = u.y_time,
                            y_check = u.y_check == 1 ? "已审核" : u.y_check == 2 ? "审核不通过" : "待审核"
                        }).ToList();
                    var tuitiontotals = listss.Sum(u => u.tuitiontotal);
                    var needtotal = listss.Sum(u => u.tuitiontotal);
                    int count = listss.Sum(u => u.totalcount);
                    listss.Add(new BatchCheck
                    {
                        schoolName = "合计",
                        y_inyear = 0,
                        totalcount = count,
                        tuitiontotal = tuitiontotals,
                        needtotal = needtotal,
                        y_time = DateTime.Now,
                        y_check = ""
                    });
                    model = FileHelper.ToDataTable(listss);
                }
                var modelcout = model.Rows.Count - 1; //得到datatable最后一行
                model.Rows[modelcout]["y_inyear"] = DBNull.Value;
                model.Rows[modelcout]["y_time"] = DBNull.Value;

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/" + schoolName + "审核注册名单表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"y_inyear", "入学年份"},
                        {"totalcount","提交人数"},
                        {"tuitiontotal","学费总金额"},
                        {"needtotal","缴费总金额"},
                        {"y_time","提交时间"},
                        {"y_check","状态"}
                    };
                    if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.DHLGDX.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"schoolName", "函授站"},
                            {"y_inyear", "入学年份"},
                            {"totalcount","提交人数"},
                            {"tuitiontotal","学费总金额"},
                            {"needtotal","缴费总金额"},
                            {"y_time","提交时间"},
                            {"y_check","状态"},
                            {"governorName","负责人姓名" }
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
        /// <summary>
        /// 开票审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ApplyInvoice(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyInvoice");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目

                var subSchool = Request["SubSchool"];
                var ischeck = Request["isCheck"];

                IQueryable<YD_Fee_StuInvoicee> list =
                   yunEntities.YD_Fee_StuInvoicee.OrderByDescending(u => u.id).Where(u => true);
                ViewBag.admin = YdAdminRoleId;
                #region  实时更新提交名单缴费总金额
                var entilylist = list.ToList();
                foreach (var batch in entilylist)
                {
                    string stuid = batch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = System.Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                    IQueryable<YD_Fee_StuFeeTb> batchlist =
                        yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == batch.y_feeyear);

                    //第一学年已申请注册缴费之和
                    var applybatchtotal = batchlist.ToList().Sum(u => u.y_needUpFee);
                    batch.needtotal = applybatchtotal;
                    yunEntities.Entry(batch).State = EntityState.Modified;
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(ischeck) && ischeck != "0")
                {
                    if (ischeck == "2") //审核通过  
                    {
                        list = list.Where(u => (u.y_check == 1));
                    }
                    else if (ischeck == "3") //审核不通过
                    {
                        list = list.Where(u => (u.y_check == 2));
                    }
                    else
                    {
                        list = list.Where(u => (u.y_check == 0));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId);

                    list = list.Where(u => schoolids.Contains(u.y_subSchoolId));
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize    
                if (Request.IsAjaxRequest())
                    return PartialView("ApplyInvoiceList", model);
                return View(model);
            }
        }
        /// <summary>
        /// 函授站提交缴费学生全部人数查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatDes(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }

            #endregion
            if (id == null)
            {
                return RedirectToAction("ApplyRegister");
            }
            ViewBag.id = id;
            var name = Request["name"];
            var inYear = Request["inYear"];
            var stuType = Request["stuType"];
            var eduType = Request["eduType"];
            var major = Request["MajorLibrary"];
            var subSchool = Request["SubSchool"];
            var term = Request["term"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             //"group by y_majorId,y_subSchoolId,y_eduTypeId" +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();

                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                    IQueryable<YD_Fee_StuFeeTb> list =
                        yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                            .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear)
                            .Include(u => u.YD_Sts_StuInfo)
                            .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool);
                    if (!list.Any())
                    {
                        return RedirectToAction("ApplyRegister");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    //根据名字搜索
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        list = list.Where(u => u.YD_Sts_StuInfo.y_name.Contains(name));
                    }
                    //根据入学年份查询
                    if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                    {
                        var yInYear = Convert.ToInt16(inYear);
                        list = list.Where(s => s.YD_Sts_StuInfo.y_inYear == yInYear);
                    }
                    if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                    {
                        var subSchoolint = Convert.ToInt32(subSchool);
                        list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    }
                    if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                    {
                        var majorLibraryint = Convert.ToInt32(major);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    }
                    if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                    {
                        var eduTypeint = Convert.ToInt32(eduType);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    }
                    if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                    {
                        var stuTypeint = Convert.ToInt32(stuType);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    }
                    if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                    {
                        var feelyeear = Convert.ToInt32(term);
                        list = list.Where(u => u.y_feeYear == feelyeear);
                    }
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId);

                        list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId.HasValue && schoolids.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                    }
                    var model = list
                        .Include(e => e.YD_Sts_StuInfo.YD_Edu_Major)
                        .Include(e => e.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary)
                        .Include(e => e.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType)
                        .Include(e => e.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType).OrderBy(e => e.YD_Sts_StuInfo.YD_Edu_Major.id).ToList();
                    var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                    var xinshenyear = Convert.ToInt32(xinshen);
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }

                    //var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                    //ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];
                    //if (Request.IsAjaxRequest())
                    //    return PartialView("FeebatchCheckList", model);

                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }
        /// <summary>
        /// 函授站提交缴费学生各层次查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorDes(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }

            #endregion
            if (id == null)
            {
                return RedirectToAction("ApplyRegister");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目

                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql =
                        "select COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 1 then '高起专(普通/3)'  end) as specomthree," +
                        " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 2 then '高起专(艺术/3)'  end) as speartthree," +
                        " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 4 and majoredu.isys = 1 then '高起专(普通/4)'  end) as specomfour," +
                        " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 1 then '专升本(普通/3)'  end) as speupgracomthree," +
                        " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 2 then '专升本(艺术/3)'  end) as speupgraartthree," +
                        " COUNT(case when  eduTypeName = '高起本' and stu.y_stuYear = 5 and majoredu.isys = 1 then '高起本(普通/5)'  end) as thiscomfive ," +  
                        " COUNT(case when  eduTypeName = '高起本' and stu.y_stuYear = 5 and majoredu.isys = 2 then '高起本(艺术/5)'  end) as thiscomfive2 " +
                        " from YD_Fee_StuFeeTb as tb  left join VW_StuInfo as stu on stu.id = tb.y_stuId " +
                        " left join(select y_eduTypeId, y_stuYear, isys, id from" +
                        " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                        " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                        " where y_stuid in (" + idlist + ") " +
                        " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear;

                    var list = yunEntities.Database.SqlQuery<MajorEduStu>(sql).ToList();
                    ViewBag.schoolName = ydFeeStuRegistrBatch.schoolName;
                    ViewBag.total = ydFeeStuRegistrBatch.totalcount;
                    ViewBag.id = ydFeeStuRegistrBatch.id;

                    var edulist = yunEntities.YD_Edu_EduType.ToList();
                    #region 各层次总学费和总缴费
                    var sqls = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                         "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                         "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                         ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                         "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                         "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                         "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                         "from VW_StuInfo where id in (" + idlist + ") " +
                         ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                         "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                         "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                         "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                         "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                         "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                         "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                         "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";
                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sqls).ToList(); //计算名单的比例
                    #region 高起专普通/3年
                    var sqlszhuan =
                      "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                      " and eduTypeName='高起专' and stu.y_stuYear=3 and majoredu.isys=1";
                    var zhuanlist = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlszhuan).ToList(); //高起专普通/3年学生
                    int needFeeszhuan = 0;
                    int needUpFeeszhuan = 0;
                    if (zhuanlist.Count > 0)
                    {
                        zhuanlist.ForEach(u =>
                        {
                            needFeeszhuan += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeeszhuan += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeeszhuan = needFeeszhuan;
                    ViewBag.needUpFeeszhuan = needUpFeeszhuan;
                    #endregion
                    #region  高起专艺术/3年
                    var sqlszhuanart =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='高起专' and stu.y_stuYear=3 and majoredu.isys=2";
                    var zhuanartlist = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlszhuanart).ToList();
                    int needFeeszhuanArt = 0;
                    int needUpFeeszhuanArt = 0;
                    if (zhuanartlist.Count > 0)
                    {
                        zhuanartlist.ForEach(u =>
                        {
                            needFeeszhuanArt += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeeszhuanArt += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeeszhuanArt = needFeeszhuanArt;
                    ViewBag.needUpFeeszhuanArt = needUpFeeszhuanArt;
                    #endregion
                    #region 高起专普通/4年
                    var sqlszhuanpu =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='高起专' and stu.y_stuYear=4 and majoredu.isys=1";
                    var zhuanpulist = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlszhuanpu).ToList();
                    int needFeezhuanpu = 0;
                    int needUpFeeszhuanpu = 0;
                    if (zhuanpulist.Count > 0)
                    {
                        zhuanpulist.ForEach(u =>
                        {
                            needFeezhuanpu += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeeszhuanpu += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeezhuanpu = needFeezhuanpu;
                    ViewBag.needUpFeeszhuanpu = needUpFeeszhuanpu;
                    #endregion
                    #region  专升本普通/3年
                    var sqlszhuanben =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='专升本' and stu.y_stuYear=3 and majoredu.isys=1";
                    var sqlszhuanbenlist = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlszhuanben).ToList();
                    int needFeezhuanben = 0;
                    int needUpFeeszhuanben = 0;
                    if (sqlszhuanbenlist.Count > 0)
                    {
                        sqlszhuanbenlist.ForEach(u =>
                        {
                            needFeezhuanben += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeeszhuanben += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeezhuanben = needFeezhuanben;
                    ViewBag.needUpFeeszhuanben = needUpFeeszhuanben;
                    #endregion
                    #region 专升本艺术/3年
                    var sqlszhuanbenArt =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='专升本' and stu.y_stuYear=3 and majoredu.isys=2";
                    var listzhuanbenArt = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlszhuanbenArt).ToList();
                    int needFeezhuanbenArt = 0;
                    int needUpFeezhuanbenArt = 0;
                    if (listzhuanbenArt.Count > 0)
                    {
                        listzhuanbenArt.ForEach(u =>
                        {
                            needFeezhuanbenArt += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeezhuanbenArt += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeezhuanbenArt = needFeezhuanbenArt;
                    ViewBag.needUpFeezhuanbenArt = needUpFeezhuanbenArt;
                    #endregion
                    #region 高起本普通/5年
                    var sqlsben =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='高起本' and stu.y_stuYear=5 and majoredu.isys=1";
                    var benlist = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlsben).ToList();
                    int needFeeben = 0;
                    int needUpFeeben = 0;
                    if (benlist.Count > 0)
                    {
                        benlist.ForEach(u =>
                        {
                            needFeeben += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeeben += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeeben = needFeeben;
                    ViewBag.needUpFeeben = needUpFeeben;
                    #endregion

                    #region 高起本艺术/5年
                    var sqlsbeny =
                       "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                       " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                       " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                       " where y_stuid in (" + idlist + ") " +
                       " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                       " and eduTypeName='高起本' and stu.y_stuYear=5 and majoredu.isys=2";
                    var benlisty = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqlsbeny).ToList();
                    int needFeebeny = 0;
                    int needUpFeebeny = 0;
                    if (benlisty.Count > 0)
                    {
                        benlisty.ForEach(u =>
                        {
                            needFeebeny += lists.Find(k => k.id == u.y_stuId).y_needFee;
                            needUpFeebeny += Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    ViewBag.needFeebeny = needFeebeny;
                    ViewBag.needUpFeebeny = needUpFeebeny;
                    #endregion
                    #endregion

                    ViewBag.benbili = 100;
                    ViewBag.zhuanbili = 100;
                    ViewBag.zhuanbenbili = 100;
                    #region 求各层次的比例
                    foreach (var edu in edulist)
                    {
                        if (edu.y_name == "高起本")
                        {
                            var subbili = yunEntities.YD_Fee_SubFeeBili.FirstOrDefault(u => u.y_Visible == true && u.y_subSchoolId == ydFeeStuRegistrBatch.y_subSchoolId && u.y_eduTypeId == edu.id);
                            var bili = yunEntities.YD_Fee_AllBili.FirstOrDefault(u => u.y_Visible == true && u.y_eduTypeId == edu.id);
                            if (subbili != null)
                                ViewBag.benbili = subbili.y_bili;
                            else if (bili != null)
                                ViewBag.benbili = bili.y_bili;
                            else
                                ViewBag.benbili = 100;

                        }
                        if (edu.y_name == "高起专")
                        {
                            var subbili = yunEntities.YD_Fee_SubFeeBili.FirstOrDefault(u => u.y_Visible == true && u.y_subSchoolId == ydFeeStuRegistrBatch.y_subSchoolId && u.y_eduTypeId == edu.id);
                            var bili = yunEntities.YD_Fee_AllBili.FirstOrDefault(u => u.y_Visible == true && u.y_eduTypeId == edu.id);
                            if (subbili != null)
                                ViewBag.zhuanbili = subbili.y_bili;
                            else if (bili != null)
                                ViewBag.zhuanbili = bili.y_bili;
                            else
                                ViewBag.zhuanbili = 100;

                        }
                        if (edu.y_name == "专升本")
                        {
                            var subbili = yunEntities.YD_Fee_SubFeeBili.FirstOrDefault(u => u.y_Visible == true && u.y_subSchoolId == ydFeeStuRegistrBatch.y_subSchoolId && u.y_eduTypeId == edu.id);
                            var bili = yunEntities.YD_Fee_AllBili.FirstOrDefault(u => u.y_Visible == true && u.y_eduTypeId == edu.id);
                            if (subbili != null)
                                ViewBag.zhuanbenbili = subbili.y_bili;
                            else if (bili != null)
                                ViewBag.zhuanbenbili = bili.y_bili;
                            else
                                ViewBag.zhuanbenbili = 100;
                        }
                    }
                    #endregion
                    if (!list.Any())
                    {
                        return RedirectToAction("ApplyRegister");
                    }
                    ViewBag.admin = YdAdminRoleId;

                    return View(list);
                }
                else
                {
                    var model = new List<MajorEduStu>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交缴费学生高起专普通3年查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorzhuanComThree(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='高起专' and stu.y_stuYear=3 and majoredu.isys=1";
                    //  var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls).AsQueryable().Include(u=>u.YD_Sts_StuInfo).Include(u=>u.YD_Sts_StuInfo.YD_Sys_SubSchool).ToList();

                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "高起专" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 3 &&
                                (!u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术") || u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype == null))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);
                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;

                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交缴费学生高起专(艺术/3年)查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorzhuanArtThree(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='高起专' and stu.y_stuYear=3 and majoredu.isys=2";
                    //var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls).ToList();
                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "高起专" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 3 &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术"))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);

                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交缴费学生高起专(普通/4年)查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorzhuanComfour(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='高起专' and stu.y_stuYear=4 and majoredu.isys=1";
                    //var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls).ToList();
                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "高起专" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 4 &&
                                (!u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术") || u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype == null))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);

                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交缴费学生专升本(普通/3年)查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorzhuanbenComThree(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='专升本' and stu.y_stuYear=3 and majoredu.isys=1";
                    //var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls).ToList();

                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "专升本" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 3 &&
                                (!u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术") || u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype == null))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);

                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }
        /// <summary>
        /// 函授站提交缴费学生专升本(艺术/3年)查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorzhuanbenArtThree(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='专升本' and stu.y_stuYear=3 and majoredu.isys=2";
                    //var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls).ToList();

                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "专升本" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 3 &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术"))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);
                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交缴费学生高起本(普通/5年）查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuFeeBatMajorbenComfive(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }
            #endregion
            if (id == null)
            {
                return RedirectToAction("StuFeeBatMajorDes");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目-
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();
                    //var sqls =
                    //    "  select * from YD_Fee_StuFeeTb as tb left join VW_StuInfo as stu on stu.id = tb.y_stuId left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    //   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    //   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    //   " where y_stuid in (" + idlist + ") " +
                    //   " and y_feeYear=" + ydFeeStuRegistrBatch.y_feeyear +
                    //    " and eduTypeName='高起本' and stu.y_stuYear=5 and majoredu.isys=1";
                    //var list = yunEntities.Database.SqlQuery<YD_Fee_StuFeeTb>(sqls);

                    var ids = Array.ConvertAll(idlist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && ids.Contains(u.YD_Sts_StuInfo.id) &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name == "高起本" &&
                                u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear == 5 &&
                                (!u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype.Contains("艺术") || u.YD_Sts_StuInfo.YD_Edu_Major.y_majortype == null))
                                .Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary);
                    if (!list.Any())
                    {
                        return Content("<script>window.location.href='/Expense/StuFeeBatMajorDes?id=" + id + "'</script>");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    var model = list.ToList();
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        model.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    return View(model);
                }
                else
                {
                    var model = new List<YD_Fee_StuFeeTb>();
                    return View(model);
                }
            }
        }

        /// 函授站提交开票学生查看视图
        public ActionResult StuFeeInvoiceDes(int? id)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyInvoice");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                if (Request.UrlReferrer != null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
                }
                else
                {
                    return RedirectToAction("Index", "AdminBase");
                }
            }

            #endregion
            if (id == null)
            {
                return RedirectToAction("ApplyInvoice");
            }
            ViewBag.id = id;
            var name = Request["name"];
            var inYear = Request["inYear"];
            var stuType = Request["stuType"];
            var eduType = Request["eduType"];
            var major = Request["MajorLibrary"];
            var subSchool = Request["SubSchool"];
            var term = Request["term"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuInvoicee.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                       Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<YD_Fee_StuFeeTb> list =
                         yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                             .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear)
                             .Include(u => u.YD_Sts_StuInfo)
                             .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                             .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool);
                    if (!list.Any())
                    {
                        return RedirectToAction("ApplyInvoice");
                    }

                    #region 给函授站金额

                    ViewBag.money = 0;
                    var ydFeeStuFeeTb = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).FirstOrDefault(u => ids.Contains(u.y_stuId));
                    if (ydFeeStuFeeTb != null)
                    {
                        var year = ydFeeStuFeeTb.YD_Sts_StuInfo.y_inYear;
                        var inyearint = Convert.ToInt32(year);
                        IQueryable<YD_Fee_StuFeeTb> lists = yunEntities.YD_Fee_StuFeeTb.OrderByDescending(
                            u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == 1 &&
                                                  u.YD_Sts_StuInfo.y_subSchoolId != null &&
                                                  u.YD_Sts_StuInfo.y_subSchoolId == ydFeeStuRegistrBatch.y_subSchoolId && u.YD_Sts_StuInfo.y_inYear == inyearint &&
                                                  u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear && u.y_isCheckFee == 0);
                        var entitylist = lists.ToList();
                        //默认新生的注册总金额
                        var needmoneys = entitylist.Sum(u => u.y_needUpFee);
                        ViewBag.needmoneys = needmoneys;
                        //如果有开票金额则求差额
                        var invooklist = lists.Where(u => u.y_invoiceOK == 1 && u.y_invoiceUp == 1);
                        var entilyinvook = invooklist.ToList();
                        int? invookmoney = entilyinvook.Sum(u => u.y_invoiceMoney);
                        var money = needmoneys - invookmoney;
                        ViewBag.money = money;
                    }
                    #endregion
                    ViewBag.admin = YdAdminRoleId;
                    //根据名字搜索
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        list = list.Where(u => u.YD_Sts_StuInfo.y_name.Contains(name));
                    }
                    //根据入学年份查询
                    if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                    {
                        var yInYear = Convert.ToInt16(inYear);
                        list = list.Where(s => s.YD_Sts_StuInfo.y_inYear == yInYear);
                    }
                    if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                    {
                        var subSchoolint = Convert.ToInt32(subSchool);
                        list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    }
                    if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                    {
                        var majorLibraryint = Convert.ToInt32(major);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    }
                    if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                    {
                        var eduTypeint = Convert.ToInt32(eduType);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    }
                    if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                    {
                        var stuTypeint = Convert.ToInt32(stuType);
                        list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    }
                    if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                    {
                        var feelyeear = Convert.ToInt32(term);
                        list = list.Where(u => u.y_feeYear == feelyeear);
                    }
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId).ToList();

                        list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId.HasValue && schoolids.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                    }
                    var model = list.ToList();
                    //var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                    //ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];
                    //if (Request.IsAjaxRequest())
                    //    return PartialView("FeebatchCheckList", model);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ApplyInvoice");
                }
            }
        }

        ///学校审核函授站提交名单通过,2017-05-04 廖坤修改2018-05-03焦赞去掉了Tolst,注释掉了在循环内获取实体并修改跟踪状态
        public string StuFeeApplyCheckSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion
            var id = Request["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                int intid = Convert.ToInt32(id);
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == intid);
                if (ydFeeStuRegistrBatch.y_feeyear != null)
                {
                    ydFeeStuRegistrBatch.y_check = 1; //审核通过
                    yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;

                    var feeyear = ydFeeStuRegistrBatch.y_feeyear.Value;

                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                    IQueryable<YD_Fee_StuFeeTb> list =
                        yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                            .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == feeyear)
                            .Include(u => u.YD_Sts_StuInfo);

                    foreach (var stuFee in list)
                    {
                        //查询出对应学生信息
                        var stu = stuFee.YD_Sts_StuInfo;
                        if (stu.y_subSchoolId == null)
                        {
                            continue;
                        }

                        stu.y_ischeck = 1; //如果是学校管理员登录则审核成功    

                        var statename = "在读";
                        var state = yunEntities.YD_Edu_StuState.First(u => u.y_name == statename);
                        if (state != null && stu.y_stuStateId != state.id)
                        {
                            stu.y_stuStateId = state.id;
                        }
                        //yunEntities.Entry(stu).State = EntityState.Modified;

                        stuFee.y_isCheckFee = (int)YesOrNo.Yes;
                        stuFee.y_isUp = (int)YesOrNo.Yes;
                        //yunEntities.Entry(stuFee).State = EntityState.Modified;
                    }
                    var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                    if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                    {
                        #region 注释掉的计算学生个人金额和注册名单总金额
                        var idlist = ydFeeStuRegistrBatch.y_stuid;
                        if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                        {
                            idlist = idlist.Substring(0, idlist.Length - 1);
                        }

                        var sqls = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             //"group by y_majorId,y_subSchoolId,y_eduTypeId" +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                        var lists = yunEntities.Database.SqlQuery<BiliDto>(sqls).ToList();

                        //list.ForEach(u =>
                        //{
                        //    u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                        //    u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        //    yunEntities.Entry(u).State = EntityState.Modified;
                        //});
                        foreach(var u in list)
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        }
                        ydFeeStuRegistrBatch.needtotal = list.Where(u => u.y_feeYear == feeyear).ToList().Sum(u => u.y_needUpFee);
                        ydFeeStuRegistrBatch.tuitiontotal = list.Where(u => u.y_feeYear == feeyear).ToList().Sum(u => u.y_needFee);

                        yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;
                        #endregion
                    }
                    yunEntities.SaveChanges();

                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update, "时间:" + DateTime.Now + "审核为通过,审核总学费为" + ydFeeStuRegistrBatch.tuitiontotal + ",缴费总金额为" + ydFeeStuRegistrBatch.needtotal);

                    return "ok";
                }
                else
                {
                    return "审核失败!未找到缴费信息，或者缴费信息缺少缴费年份";
                }

            }
        }

        ///学校审核函授站提交名单不通过,2017-05-04 廖坤修改
        public string StuFeeApplyCheckSomeno()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var id = Request["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                int intid = Convert.ToInt32(id);
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == intid);
                if (ydFeeStuRegistrBatch?.y_feeyear != null)
                {
                    ydFeeStuRegistrBatch.y_check = 2; //审核不通过
                    yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;

                    var feeyear = ydFeeStuRegistrBatch.y_feeyear.Value;

                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                    IQueryable<YD_Fee_StuFeeTb> list =
                       yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                           .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == feeyear)
                           .Include(u => u.YD_Sts_StuInfo);
                           //.ToList();

                    foreach (var stuFee in list)
                    {
                        //查询出对应学生信息
                        var stu = stuFee.YD_Sts_StuInfo;
                        if (stu.y_subSchoolId == null)
                        {
                            continue;
                        }
                        stu.y_ischeck = 0;
                        if (ydFeeStuRegistrBatch.y_feeyear == 1)
                        {
                            var statename = "未注册";
                            var state = yunEntities.YD_Edu_StuState.First(u => u.y_name == statename);
                            stu.y_stuStateId = state.id;
                        }
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_isUp = (int)YesOrNo.No;

                        //yunEntities.Entry(stuFee).State = EntityState.Modified;
                        //yunEntities.Entry(stu).State = EntityState.Modified;

                    }
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update, "时间:" + DateTime.Now + "审核为不通过");
                    return "ok";

                }
                else
                {

                    return "审核失败!未找到缴费信息，或者缴费信息缺少缴费年份";
                }

            }

        }

        /// <summary>
        ///学校审核函授站提交名单通过撤销操作
        /// </summary>
        /// <returns></returns>
        public string StuFeeRevocationCheck()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var id = Request["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                int intid = Convert.ToInt32(id);
                if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 8) //如果是函授站登录则表示未审核不能注册
                {
                    var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == intid);
                    if (ydFeeStuRegistrBatch != null)
                    {

                        string stuid = ydFeeStuRegistrBatch.y_stuid;
                        //将string数组转换成int数组
                        int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                        var list =
                            yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Include(u => u.YD_Sts_StuInfo)
                                .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear)
                                .ToList();
                        if (ydFeeStuRegistrBatch.y_check == 2 && list.Any(u => u.y_isUp == 0))
                        {
                            return "不能撤销";
                        }
                        ydFeeStuRegistrBatch.y_check = 0; //返回待审核状态
                        yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;
                        for (int i = 0; i < list.Count(); i++)
                        {
                            var stuFee = list[i];
                            if (stuFee != null)
                            {
                                //查询出对应学生信息
                                var obj = stuFee.YD_Sts_StuInfo;
                                obj.y_ischeck = 0; //如果是学校管理员登录则审核成功       
                                if (ydFeeStuRegistrBatch.y_feeyear == 1)
                                {
                                    var statename = "注册待审核";
                                    var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
                                    if (state != null)
                                    {
                                        obj.y_stuStateId = state.id;
                                    }
                                }
                                yunEntities.Entry(obj).State = EntityState.Modified;
                                //如果注册成功之后修改缴费表的相关学生信息
                                stuFee.y_isCheckFee = (int)YesOrNo.No;
                                stuFee.y_isUp = (int)YesOrNo.Yes;
                                yunEntities.Entry(stuFee).State = EntityState.Modified;
                            }
                        }
                        //ydFeeStuRegistrBatch.needtotal = 0;
                        //ydFeeStuRegistrBatch.tuitiontotal = 0;
                        //yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;
                        yunEntities.SaveChanges();

                        LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                            "时间:" + DateTime.Now + "撤销操作  " + ydFeeStuRegistrBatch.id);

                        return "ok";
                    }
                    else
                    {
                        return "未找到注册记录";
                    }

                }
                else
                {
                    return "审核失败";
                }

            }
        }

        /// <summary>
        ///学校审核函授站提交开票名单通过
        /// </summary>
        /// <returns></returns>
        public string StuApplyCheckInvoiceSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/ApplyInvoice");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var id = Request["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                int intid = Convert.ToInt32(id);
                var batch = yunEntities.YD_Fee_StuInvoicee.FirstOrDefault(u => u.id == intid);
                if (batch != null)
                {
                    batch.y_check = 1;//审核通过
                    yunEntities.Entry(batch).State = EntityState.Modified;
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuInvoicee.FirstOrDefault(u => u.id == intid);
                    if (ydFeeStuRegistrBatch != null)
                    {
                        string stuid = ydFeeStuRegistrBatch.y_stuid;
                        //将string数组转换成int数组
                        int[] ids = System.Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                        IQueryable<YD_Fee_StuFeeTb> list =
                            yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear);
                        #region  只有学校管理员进入这里

                        var entitlylist = list.ToList();
                        if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 8) //如果是函授站登录则表示未审核不能注册
                        {
                            for (int i = 0; i < entitlylist.Count(); i++)
                            {
                                var oid = Convert.ToInt32(entitlylist[i].id);
                                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear);
                                if (stuFee != null)
                                {
                                    stuFee.y_invoiceCheck = 1;
                                    yunEntities.Entry(stuFee).State = EntityState.Modified;
                                }
                            }
                            yunEntities.SaveChanges();
                        }
                        #endregion
                    }
                    return "ok";
                }
                else
                {
                    return "审核失败";
                }
            }
        }
        /// <summary>
        ///学校审核函授站提交开票名单不通过
        /// </summary>
        /// <returns></returns>
        public string StuApplyCheckInvoiceSomeno()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyInvoice");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var id = Request["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                int intid = Convert.ToInt32(id);
                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuInvoicee.FirstOrDefault(u => u.id == intid);
                if (ydFeeStuRegistrBatch != null)
                {
                    ydFeeStuRegistrBatch.y_check = 2; //审核不通过
                    yunEntities.Entry(ydFeeStuRegistrBatch).State = EntityState.Modified;

                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        System.Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<YD_Fee_StuFeeTb> list =
                        yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear);


                    var entitlylist = list.ToList();
                    if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 8) //如果是函授站登录则表示未审核不能注册
                    {
                        for (int i = 0; i < entitlylist.Count(); i++)
                        {
                            var oid = Convert.ToInt32(entitlylist[i].id);
                            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear);
                            if (stuFee != null)
                            {
                                stuFee.y_invoiceCheck = 0;
                                stuFee.y_invoiceUp = 0;
                                yunEntities.Entry(stuFee).State = EntityState.Modified;
                            }
                        }
                    }
                    int r = yunEntities.SaveChanges();
                    if (r > 0)
                    {

                        return "ok";
                    }
                    else
                    {
                        return "审核失败";
                    }

                }
                return "ok";  //todo:仔细考虑返回什么
            }

        }
        /// <summary>
        /// 开票管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InvoiceeManage(int id = 1)
        {
            #region 权限验证
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            var power = SafePowerPage("/Expense/InvoiceeManage");
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var term = Request["term"];
                var inYear = Request["inYear"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var isFee = Request["isFee"];
                var isCheck = Request["isCheck"];
                var invoiceOk = Request["invoiceOk"];
                var name = Request["name"];
                var inyearint = Convert.ToInt32(inYear);
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.OrderByDescending(
                    u => u.YD_Sts_StuInfo.y_subSchoolId).
                    Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == 0)
                    .Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool);
                //根据名字搜索
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.YD_Sts_StuInfo.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feelyeear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feelyeear);
                }
                //根据入学年份查询
                if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.YD_Sts_StuInfo.y_inYear == yInYear);
                }
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list =
                        list.Where(
                            u =>
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint && u.y_feeYear == 1) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 4 && u.y_feeYear == 5));
                }
                var subSchool = Request["SubSchool"];
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(isFee) && isFee != "0")
                {
                    if (isFee == "1") //已缴费  
                    {
                        list = list.Where(u => (u.y_isUp == (int)YesOrNo.Yes));
                    }
                    else if (isFee == "2") //未缴费
                    {
                        list = list.Where(u => (u.y_isUp == (int)YesOrNo.No));
                    }
                }
                if (!string.IsNullOrWhiteSpace(isCheck) && isCheck != "0")
                {
                    if (isCheck == "1") //已审核  
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.Yes));
                    }
                    else if (isCheck == "2") //未审核
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.No));
                    }
                }
                if (!string.IsNullOrWhiteSpace(invoiceOk) && invoiceOk != "0")
                {
                    if (invoiceOk == "1") //开票
                    {
                        list = list.Where(u => (u.y_invoiceOK == (int)YesOrNo.No));
                    }
                    else if (invoiceOk == "2") //不开票
                    {
                        list = list.Where(u => (u.y_invoiceOK == (int)YesOrNo.Yes));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                //默认显示新生数据
                if (true)
                {
                    if (inyearint != 0)
                    {
                        list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyearint);
                        //默认新生的注册总金额
                        var entilylist = list.ToList();
                        var needmoneys = entilylist.Sum(u => u.y_needUpFee);
                        ViewBag.needmoneys = needmoneys;
                        //如果有开票金额则求差额
                        var invooklist = list.Where(u => u.y_invoiceOK == 1);
                        var entilyinvook = invooklist.ToList();
                        int? invookmoney = entilyinvook.Sum(u => u.y_invoiceMoney);
                        var money = needmoneys - invookmoney;
                        ViewBag.money = money;
                    }
                    else
                    {
                        list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_feeYear == 1);
                        //默认新生的注册总金额
                        var entilylist = list.ToList();
                        var needmoneys = entilylist.Sum(u => u.y_needUpFee);

                        ViewBag.needmoneys = needmoneys;
                        //如果有开票金额则求差额
                        var invooklist = list.Where(u => u.y_invoiceOK == 1);
                        var entilyinvook = invooklist.ToList();
                        int? invookmoney = entilyinvook.Sum(u => u.y_invoiceMoney);
                        var money = needmoneys - invookmoney;
                        ViewBag.money = money;

                    }
                }
                //如果学校审核则只显示未申请学生数据
                if (string.IsNullOrWhiteSpace(isFee))
                {
                    var checkok = list.Where(u => (u.y_invoiceCheck == 1) && u.y_invoiceUp == 1);

                    if (checkok.ToList().Count > 0)
                    {
                        //默认选中未申请
                        list = list.Where(u => u.y_invoiceUp == 0);
                    }
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                ViewBag.admin = YdAdminRoleId;
                ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];
                if (Request.IsAjaxRequest())
                    return PartialView("InvoiceeManageList", model);
                return View(model);
            }
        }
        /// <summary>
        /// 开发票并填写开票金额
        /// </summary>
        /// <returns></returns>
        public string InvoiceeCheck()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/InvoiceeManage");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion
            var stuId = Request["stuId"];
            var invoiceMoney = Request["invoiceMoney"];
            int money = Convert.ToInt32(invoiceMoney);
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (money < 0)
            {
                return "开票金额不能为负数";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid);
                if (stuFee != null)
                {
                    if (stuFee.y_invoiceOK == 1 && stuFee.y_invoiceUp == 1)
                    {
                        return "已经提交不允许修改";
                    }
                    if (stuFee.y_isCheckFee == 0)
                    {
                        if (money > stuFee.y_needFee)
                        {
                            return "开票金额不能大于缴费金额";
                        }
                        stuFee.y_invoiceUp = 0;
                        stuFee.y_invoiceOK = 1;
                        stuFee.y_invoiceMoney = money;
                        yunEntities.Entry(stuFee).State = EntityState.Modified;
                    }
                    else
                    {
                        return "该学生还未注册";
                    }
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return "ok";
                }
                else
                {
                    return "设置失败";
                }
            }
        }
        /// <summary>
        /// 取消学生开发票资格
        /// </summary>
        /// <returns></returns>
        public string InvoiceeCheckNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/InvoiceeManage");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion
            var stuId = Request["stuId"];

            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid);
                if (stuFee != null)
                {
                    if (stuFee.y_invoiceOK == 1 && stuFee.y_invoiceUp == 1)
                    {
                        return "已经提交不允许修改";
                    }
                    if (stuFee.y_isCheckFee == 0)
                    {
                        stuFee.y_invoiceOK = 0;
                        stuFee.y_invoiceMoney = 0;
                        yunEntities.Entry(stuFee).State = EntityState.Modified;
                    }
                    else
                    {
                        return "该学生还未注册";
                    }
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return "ok";
                }
                else
                {
                    return "设置失败";
                }
            }
        }
        /// <summary>
        /// 选择性提交学生缴费情况
        /// </summary>
        /// <returns></returns>
        public string InvoiceeCheckSome()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/InvoiceeManage");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion
            var inyear = Request["inYear"];
            var feeyear = Request["term"];
            if (string.IsNullOrWhiteSpace(inyear))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeyear))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                int needUpFees = new int();//学生缴费金额之和
                int? invoicees = 0;//学生开票金额
                #region 函授站全部处理完学生之后才允许提交判断函授站所开发票金额之和是否等于缴费金额之和
                //var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                //if (sub != null)
                //{
                //    var inyearint = Convert.ToInt32(inyear);
                //    var feeyearint = Convert.ToInt32(feeyear);
                //    var ishave =
                //        yunEntities.YD_Fee_StuFeeTb.Any(
                //            u =>
                //                u.y_subSchoolId == sub.y_subSchoolId && u.y_inYear == inyearint &&
                //                u.y_feeYear == feeyearint && u.y_invoiceOK!=0 && u.y_invoiceOK!= 1 && u.y_isCheckFee == 0);                
                //    if (!ishave)
                //    {
                //        IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.OrderByDescending(
                //            u => u.id).
                //            Where(
                //                u =>
                //                     u.y_subSchoolId != -1 &&
                //                    u.y_subSchoolId == sub.y_subSchoolId && u.y_inYear == inyearint &&
                //                    u.y_feeYear ==feeyearint&& u.y_invoiceOK!= null && u.y_isCheckFee == 0);
                //        var entitylist = list.ToList();
                //        for (int i = 0; i < entitylist.Count(); i++)
                //        {
                //            var oid = Convert.ToInt32(entitylist[i].id);
                //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
                //            if (stuFee != null)
                //            {
                //                needUpFees += stuFee.y_needUpFee;
                //                invoicees += stuFee.y_invoiceMoney;
                //            }
                //        }
                //        if (needUpFees == invoicees)
                //        {
                //            #region  缴费金额和开票金额一致提交
                //            var stulist = list.Where(u => u.y_invoiceOK == 1&& u.y_invoiceUp==0);
                //            var countlist = stulist.ToList();
                //            if (countlist.Count == 0)
                //            {
                //                return "没有能提交的学生";
                //            }
                //            var fee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_subSchoolId == sub.y_subSchoolId);
                //            if (fee != null)
                //            {
                //                var obj = new YD_Fee_StuInvoicee
                //                {
                //                    y_subSchoolId = fee.y_subSchoolId,
                //                    schoolCode = fee.schoolCode,
                //                    schoolName = fee.schoolName,
                //                    totalcount = countlist.Count(),
                //                    y_time = DateTime.Now,
                //                    y_check = 0
                //                };
                //                for (int i = 0; i < entitylist.Count(); i++)
                //                {
                //                    var oidnew = Convert.ToInt32(entitylist[i].id);
                //                    var stuFeenew = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oidnew);
                //                    if (stuFeenew != null)
                //                    {
                //                        obj.y_stuid += stuFeenew.y_stuId + ",";
                //                        obj.needtotal += stuFeenew.y_needUpFee;
                //                    }
                //                }
                //                yunEntities.Entry(obj).State = EntityState.Added;
                //            }
                //            int w = yunEntities.SaveChanges();
                //            if (w > 0)
                //            {
                //                #region 修改缴费表
                //                for (int i = 0; i < entitylist.Count(); i++)
                //                {
                //                    var oidnew2 = Convert.ToInt32(entitylist[i].id);
                //                    var stuFeenew2 = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oidnew2);
                //                    if (stuFeenew2 != null)
                //                    {
                //                        stuFeenew2.y_invoiceUp = 1;
                //                        yunEntities.Entry(stuFeenew2).State = EntityState.Modified;
                //                    }
                //                }
                //                yunEntities.SaveChanges();
                //                #endregion
                //                return "ok";
                //            }
                //            else
                //            {
                //                return "提交失败";
                //            }
                //            #endregion
                //        }
                //        else
                //        {
                //            if (needUpFees > invoicees)
                //            {
                //                var num = needUpFees - invoicees;
                //                return "开票金额小于缴费金额，差额" + num + "，不允许开票，请重新输入开票金额";
                //            }
                //            else
                //            {
                //                var num = invoicees - needUpFees;
                //                return "开票金额大于缴费金额，差额" + num + "，不允许开票，请重新输入开票金额";
                //            }
                //        }
                //    }
                //    else
                //    {
                //        return "没有处理完同年级同一缴费学年学生不能提交";
                //    }
                //}
                //else
                //{
                //    return "该账号没有指定函授站";
                //}
                #endregion
                #region 函授站提交判断函授站所开发票金额之和
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);

                    IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.OrderByDescending(
                        u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == 1 &&
                                 u.YD_Sts_StuInfo.y_subSchoolId != null &&
                                u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId && u.YD_Sts_StuInfo.y_inYear == inyearint &&
                                u.y_feeYear == feeyearint && u.y_isCheckFee == 0);
                    var entitylist = list.ToList();
                    //得到同年份同学年所有学生缴费金额之和
                    needUpFees = entitylist.Sum(u => u.y_needUpFee);
                    // 判断开票金额之和不能大于缴费金额之和
                    invoicees = list.Where(u => u.y_invoiceMoney != null && u.y_invoiceOK == 1)
                            .ToList()
                            .Sum(u => u.y_invoiceMoney);
                    if (invoicees <= needUpFees)
                    {
                        #region  缴费金额和开票金额一致提交

                        var stulist = list.Where(u => u.y_invoiceOK == 1 && u.y_invoiceUp == 0);
                        var countlist = stulist.ToList();
                        if (countlist.Count == 0)
                        {
                            return "没有能提交的学生";
                        }
                        var fee = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).FirstOrDefault(u => u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId);
                        if (fee != null && fee.YD_Sts_StuInfo.y_subSchoolId.HasValue)
                        {
                            var obj = new YD_Fee_StuInvoicee
                            {
                                y_subSchoolId = fee.YD_Sts_StuInfo.y_subSchoolId.Value,
                                schoolCode = fee.YD_Sts_StuInfo.YD_Sys_SubSchool.y_code,
                                schoolName = fee.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                                y_feeyear = feeyearint,
                                y_inyear = inyearint,
                                totalcount = countlist.Count(),
                                needtotal = needUpFees,
                                y_time = DateTime.Now,
                                y_check = 0
                            };
                            for (int i = 0; i < countlist.Count(); i++)
                            {
                                var oidnew = Convert.ToInt32(entitylist[i].id);
                                var stuFeenew =
                                    yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(
                                        u => u.id == oidnew && u.y_feeYear == feeyearint);
                                if (stuFeenew != null)
                                {
                                    obj.y_stuid += stuFeenew.y_stuId + ",";
                                }
                            }
                            yunEntities.Entry(obj).State = EntityState.Added;
                        }
                        int w = yunEntities.SaveChanges();
                        if (w > 0)
                        {
                            #region 修改缴费表

                            for (int i = 0; i < countlist.Count(); i++)
                            {
                                var oidnew2 = Convert.ToInt32(countlist[i].id);
                                var stuFeenew2 = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oidnew2);
                                if (stuFeenew2 != null)
                                {
                                    stuFeenew2.y_invoiceUp = 1;
                                    yunEntities.Entry(stuFeenew2).State = EntityState.Modified;
                                }
                            }
                            yunEntities.SaveChanges();

                            #endregion

                            return "ok";
                        }
                        else
                        {
                            return "提交失败";
                        }

                        #endregion
                    }
                    else
                    {
                        return "超过缴费总和，请减少开票金额";
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
                #endregion
            }
        }
        /// <summary>
        ///开票名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadInvoiceeManage()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/InvoiceeManage");
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
                var year = Request["year"];
                var inYear = Request["inYear"];
                var term = Request["term"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var isFee = Request["isFee"];
                var isCheck = Request["isCheck"];
                var invoiceOk = Request["invoiceOk"];
                const int isnotdel = (int)YesOrNo.No;

                IQueryable<YD_Fee_StuFeeTb> list =
                    yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == 0);
                //根据入学年份查询
                if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.YD_Sts_StuInfo.y_inYear == yInYear);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feelyeear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feelyeear);
                }
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list =
                        list.Where(
                            u =>
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint && u.y_feeYear == 1) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 4 && u.y_feeYear == 5))
                            .OrderByDescending(u => u.YD_Sts_StuInfo.y_stuNum);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }

                if (!string.IsNullOrWhiteSpace(isFee) && isFee != "0")
                {
                    if (isFee == "1") //已缴费  
                    {
                        list = list.Where(u => (u.y_isUp == (int)YesOrNo.Yes));
                    }
                    else if (isFee == "2") //未缴费
                    {
                        list = list.Where(u => (u.y_isUp == (int)YesOrNo.No));
                    }
                }
                if (!string.IsNullOrWhiteSpace(isCheck) && isCheck != "0")
                {
                    if (isCheck == "1") //已审核  
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.Yes));

                    }
                    else if (isCheck == "2") //未审核
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.No));
                    }
                }
                if (!string.IsNullOrWhiteSpace(invoiceOk) && invoiceOk != "0")
                {
                    if (invoiceOk == "1") //开票
                    {
                        list = list.Where(u => (u.y_invoiceOK == (int)YesOrNo.No) || u.y_invoiceOK == (int)YesOrNo.Yes);
                    }
                    else if (invoiceOk == "2") //不开票
                    {
                        list = list.Where(u => (u.y_invoiceOK != (int)YesOrNo.No) || u.y_invoiceOK != (int)YesOrNo.Yes);
                    }
                }

                var lists = new List<InvoiceeStuFee>();
                var fee = new InvoiceeStuFee();
                var invoname = "";
                list.Include(u => u.YD_Sts_StuInfo).ToList().ForEach(u =>
                  {
                      if (u.y_invoiceOK == 1)
                      {
                          invoname = "开票";
                      }
                      else if (u.y_invoiceOK == 0)
                      {
                          invoname = "不开票";
                      }
                      else
                      {
                          invoname = "暂定";
                      }

                      if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString())
                      {
                          fee = new InvoiceeStuFee
                          {
                              y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                              y_name = u.YD_Sts_StuInfo.y_name,
                              y_inYear = u.YD_Sts_StuInfo.y_inYear,
                              schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                              y_feeYear = u.y_feeYear + u.YD_Sts_StuInfo.y_inYear - 1,
                              y_needUpFee = u.y_needUpFee,
                              Invoiceename = invoname,
                              y_invoiceMoney = u.y_invoiceMoney
                          };
                      }
                      else
                      {
                          fee = new InvoiceeStuFee
                          {
                              y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                              y_name = u.YD_Sts_StuInfo.y_name,
                              y_inYear = u.YD_Sts_StuInfo.y_inYear,
                              schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                              y_feeYear = u.y_feeYear,
                              y_needUpFee = u.y_needUpFee,
                              Invoiceename = invoname,
                              y_invoiceMoney = u.y_invoiceMoney
                          };
                      }


                      lists.Add(fee);
                  });

                var model =
                    FileHelper.ToDataTable(lists);

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/开发票名单表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变


                //var filename1 = "File/Dowon/开发票名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
                        {"schoolName", "函授站"},
                        {"y_feeYear", "缴费学年"},
                        {"y_needUpFee", "缴费金额"},
                        {"Invoiceename", "是否开发票"},
                        {"y_invoiceMoney","开发票金额" }
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
        /// 学生缴费审核下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadFeeCheckStu()
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/ApplyRegister");
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
                var inYear = Request["inYear"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var batchid = Request["batchid"];
                if (batchid == null)
                {
                    return Content("未知错误");
                }
                var id = Convert.ToInt32(batchid);

                var ydFeeStuRegistrBatch = yunEntities.YD_Fee_StuRegistrBatch.FirstOrDefault(u => u.id == id);
                if (ydFeeStuRegistrBatch != null)
                {
                    var idlist = ydFeeStuRegistrBatch.y_stuid;
                    if (idlist.Substring(idlist.Length - 1, 1).Equals(","))
                    {
                        idlist = idlist.Substring(0, idlist.Length - 1);
                    }

                    var sql = "select c.*,m.y_needFee,p.bili,cast(1.*y_needFee*bili/100 as decimal(18,2)) as money " +
                             "from (select a.*,  case when b.y_stufee is not null then b.y_stufee   " +
                             "else c.y_needFee end as y_needFee from (select YD_Edu_Major.id as majorid,YD_Sys_SubSchool.id as schoolid" +
                             ",YD_Edu_Major.y_eduTypeId  from YD_Edu_Major full join YD_Sys_SubSchool on 1 = 1 ) as a " +
                             "left join YD_Fee_SubFeeSys as b on b.y_subSchoolId = a.schoolid and (b.y_majorid = a.majorid or " +
                             "(b.y_majorid is null and b.y_eduTypeId = a.y_eduTypeId)) left join YD_Edu_Major as c  on  c.id = a.majorid )  as m  " +
                             "join (select id,y_majorId,y_subSchoolId,y_eduTypeId " +
                             "from VW_StuInfo where id in (" + idlist + ") " +
                             //"group by y_majorId,y_subSchoolId,y_eduTypeId" +
                             ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                             "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                             "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                             "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                             "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                             "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                             "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                             "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                    var lists = yunEntities.Database.SqlQuery<BiliDto>(sql).ToList();

                    string stuid = ydFeeStuRegistrBatch.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<YD_Fee_StuFeeTb> list =
                        yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                            .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == ydFeeStuRegistrBatch.y_feeyear)
                            .Include(u => u.YD_Sts_StuInfo)
                            .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                            .AsQueryable();
                    var schoolName = ydFeeStuRegistrBatch.schoolName;
                    var inyear = ydFeeStuRegistrBatch.y_inyear;
                    var models = list.ToList();
                    var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                    var xinshenyear = Convert.ToInt32(xinshen);
                    if ((ydFeeStuRegistrBatch.y_check == 0 || ydFeeStuRegistrBatch.y_check == 2))
                    {
                        models.ForEach(u =>
                        {
                            u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                            u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        });
                    }
                    var listss = models.Select(
                        u =>
                            new FeeCheckStu
                            {
                                schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                                schoolCode = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_code,
                                y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                y_name = u.YD_Sts_StuInfo.y_name,
                                y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                majorName = u.YD_Sts_StuInfo.YD_Edu_Major.y_name,
                                y_tel = u.YD_Sts_StuInfo.y_tel,
                                y_address = u.YD_Sts_StuInfo.y_address,
                                y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                y_stuYear = (int)u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear,
                                y_feeYear = u.y_feeYear,
                                y_needFee = u.y_needFee,
                                y_needUpFee = u.y_needUpFee,
                                y_isUp = u.y_isUp == 0 ? "已申请" : "未申请",
                                y_isCheckFee = u.y_isCheckFee == 0 ? "已通过" : "待审核",
                            }).OrderByDescending(u => u.y_stuNum)
                        .ToList();
                    List<ZYYFeeCheckStu> zyyListss = null;
                    if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    {
                        zyyListss = models.Select(
                        u =>
                            new ZYYFeeCheckStu
                            {
                                schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name,
                                y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                y_name = u.YD_Sts_StuInfo.y_name,
                                y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                MajorLibName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                EduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                StuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                y_feeYear = u.y_feeYear,
                                y_needFee = u.y_needFee,
                                y_isUp = u.y_isUp == 0 ? "已申请" : "未申请",
                                y_isCheckFee = u.y_isCheckFee == 0 ? "已通过" : "待审核",

                            }).OrderByDescending(u => u.y_stuNum)
                        .ToList();
                    }
                    var needFeeTotal = listss.Sum(u => u.y_needFee);
                    var needUpFeeTotal = listss.Sum(u => u.y_needUpFee);
                    int yearint = Convert.ToInt32(ydFeeStuRegistrBatch.y_inyear);
                    int feeyear = Convert.ToInt32(ydFeeStuRegistrBatch.y_feeyear);
                    listss.Add(new FeeCheckStu
                    {
                        schoolName = "",
                        schoolCode = "",
                        y_stuNum = "合计",
                        y_name = "",
                        y_inYear = yearint,
                        y_cardId = "",
                        y_stuYear = 0,
                        y_feeYear = feeyear,
                        y_needFee = needFeeTotal,
                        y_isUp = "",
                        y_isCheckFee = "",
                    });
                    DataTable model = FileHelper.ToDataTable(listss);
                    var modelcout = model.Rows.Count - 1; //得到datatable最后一行
                    model.Rows[modelcout]["y_stuYear"] = DBNull.Value;
                    if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    {
                        model = FileHelper.ToDataTable(zyyListss);
                    }
                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/" + schoolName + inyear + "缴费审核学生名单" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变

                    //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                    //var fileName2 = "~/" + filename1;
                    //var fileName3 = Server.MapPath(fileName2);
                    using (var excelHelper = new ExcelHelper(fileName3))
                    {
                        var ht = new Hashtable
                        {
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_stuNum", "学号"},
                            {"y_name", "姓名"},
                            {"y_inYear", "入学年份"},
                            {"majorName", "专业"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_cardId", "身份证号"},
                            {"y_stuYear", "学制"},
                            {"y_feeYear", "缴费学年"},
                            {"y_needFee", "学费"},
                            {"y_needUpFee", "实缴费用"},
                            {"y_isUp", "缴费状态"},
                            {"y_isCheckFee", "审核状态"}
                        };
                        if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                        {
                            ht = new Hashtable
                            {
                                {"schoolName", "函授站"},
                                {"y_stuNum", "学号"},
                                {"y_name", "姓名"},
                                {"y_inYear", "入学年份"},
                                {"MajorLibName", "专业"},
                                {"EduTypeName", "层次"},
                                {"StuTypeName", "学习形式"},
                                {"y_cardId", "身份证号"},
                                {"y_stuYear", "学制"},
                                {"y_feeYear", "缴费学年"},
                                {"y_needFee", "学费"},
                                {"y_isUp", "缴费状态"},
                                {"y_isCheckFee", "审核状态"}
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
                else
                {
                    return Content("找不到学生名单");
                }
            }
        }


        /// <summary>
        /// 学生缴费统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuFeeTotal(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/StuFeeTotal");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                var isCheck = Request["isCheck"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Fee_StuFeeTb> list =
                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel);
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list =
                        list.Where(
                            u =>
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint && u.y_feeYear == 1) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 4 && u.y_feeYear == 5));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(isCheck) && isCheck != "0")
                {
                    if (isCheck == "1") //已审核  
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.Yes));
                    }
                    else if (isCheck == "2") //未审核
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.No));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                       yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                           .Select(u => u.y_subSchoolId)
                           .ToList();
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var allList = list.ToList();
                ViewBag.admin = YdAdminRoleId;
                ViewBag.totalStuCount = allList.Count;//总人数
                ViewBag.totalUpStuCount = allList.Count(u => u.y_isUp == (int)YesOrNo.Yes);//已上缴人数
                ViewBag.totalNoUpStuCount = allList.Count(u => u.y_isUp == (int)YesOrNo.No);//未上缴人数
                ViewBag.totalFee = allList.Sum(u => u.y_needFee);//总费用
                ViewBag.totalNeedUpFee = allList.Sum(u => u.y_needUpFee);//需上缴学校费用
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("StuFeeTotalList", model);
                return View(model);
            }
        }

        /// <summary>
        ///学生缴费统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuFeeTotal()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/StuFeeTotal");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                var isCheck = Request["isCheck"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Fee_StuFeeTb> list =
                    yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel);
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list =
                        list.Where(
                            u =>
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint && u.y_feeYear == 1) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) ||
                                (u.YD_Sts_StuInfo.y_inYear == enrollYearint - 4 && u.y_feeYear == 5));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(isCheck) && isCheck != "0")
                {
                    if (isCheck == "1") //已审核  
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.Yes));
                    }
                    else if (isCheck == "2") //未审核
                    {
                        list = list.Where(u => (u.y_isCheckFee == (int)YesOrNo.No));
                    }
                }

                DataTable model;

                if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString())
                {
                    model =
                    FileHelper.ToDataTable(
                        list.Include(u => u.YD_Sts_StuInfo).Select(
                            u =>
                                new FeeStuFeeTbList
                                {
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    y_needFee = u.y_needFee,
                                    y_feeYear = u.y_feeYear + u.YD_Sts_StuInfo.y_inYear - 1,
                                    y_isUp = u.y_isUp == 0 ? "已缴费" : "未缴费",
                                    y_isCheckFee = u.y_isCheckFee == 0 ? "已审核" : "未审核"
                                }).ToList());
                }
                else
                {
                    model =
                    FileHelper.ToDataTable(
                        list.Include(u => u.YD_Sts_StuInfo).Select(
                            u =>
                                new FeeStuFeeTbList
                                {
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    y_needFee = u.y_needFee,
                                    y_feeYear = u.y_feeYear,
                                    y_isUp = u.y_isUp == 0 ? "已缴费" : "未缴费",
                                    y_isCheckFee = u.y_isCheckFee == 0 ? "已审核" : "未审核"
                                }).ToList());
                }




                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/学生缴费统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/学生缴费统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuNum", "学号"},
                        {"y_examNum","考生号"},
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
                        {"schoolName", "函授站"},
                        {"schoolCode","函授站代码"},
                        {"y_majorlibrary", "专业"},
                        {"majorLibraryCode","专业代码"},
                        {"y_stuType", "学习形式"},
                        {"y_eduType", "层次"},
                         {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_cardId","身份证号"},
                        {"y_stuYear", "学制"},
                        {"y_feeYear", "缴费学年"},
                        {"y_isUp", "缴费状态"},
                        {"y_isCheckFee", "审核状态"}
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
        /// 函授站缴费统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubFeeTotal(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Expense/SubFeeTotal");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 43); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                IQueryable<VW_SubFeeTotal> list =
                    yunEntities.VW_SubFeeTotal.OrderByDescending(u => u.y_subSchoolId).Where(u => true);
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => (u.y_inYear == enrollYearint && u.y_feeYear == 1) || (u.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) || (u.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) || (u.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) || (u.y_inYear == enrollYearint - 4 && u.y_feeYear == 5));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                       yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                           .Select(u => u.y_subSchoolId)
                           .ToList();
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                    //list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("SubFeeTotalList", model);
                return View(model);
            }
        }

        /// <summary>
        ///函授站缴费统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadSubFeeTotal()
        {
            #region 权限验证

            var power = SafePowerPage("/Expense/SubFeeTotal");
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
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                IQueryable<VW_SubFeeTotal> list =
                    yunEntities.VW_SubFeeTotal.OrderByDescending(u => u.y_subSchoolId).Where(u => true);
                //根据要查询的缴费年份找出相应要缴费的几届学生
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => (u.y_inYear == enrollYearint && u.y_feeYear == 1) || (u.y_inYear == enrollYearint - 1 && u.y_feeYear == 2) || (u.y_inYear == enrollYearint - 2 && u.y_feeYear == 3) || (u.y_inYear == enrollYearint - 3 && u.y_feeYear == 4) || (u.y_inYear == enrollYearint - 4 && u.y_feeYear == 5));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    schoolName = u.schoolName,
                                    totalStuCount = u.totalStuCount,
                                    hasUpStuCount = u.hasUpStuCount,
                                    noUpStuCount = u.noUpStuCount,
                                    totalFee = u.totalFee,
                                    totalUpFee = u.totalUpFee,
                                    totalFeeChae = u.totalFeeChae,
                                    totalNeedUpFee = u.totalNeedUpFee,

                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/函授站缴费统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/函授站缴费统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"totalStuCount", "人数统计"},
                        {"hasUpStuCount", "已缴人数"},
                        {"noUpStuCount", "未缴人数"},
                        {"totalFee", "总费用"},
                        {"totalUpFee", "已收费用"},
                        {"totalFeeChae", "差额"},
                        {"totalNeedUpFee","需上缴校本费费用"}
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

    }


    // 学生缴费统计
    public class FeeStuFeeTbList
    {
        public string y_stuNum { get; set; }
        public string y_name { get; set; }
        public int y_inYear { get; set; }
        public int y_needFee { get; set; }
        public int y_upFee { get; set; }
        public int chae { get; set; }
        public int y_feeYear { get; set; }
        public string y_isUp { get; set; }
        public string y_isCheckFee { get; set; }

    }
    // 学生缴费审核
    public class FeeCheckList
    {
        public string y_stuNum { get; set; }
        public string y_examNum { get; set; }
        public string y_name { get; set; }
        public int y_inYear { get; set; }
        public string schoolName { get; set; }
        public string schoolCode { get; set; }
        public string y_majorlibrary { get; set; }
        public string majorLibraryCode { get; set; }
        public string y_stuType { get; set; }
        public string y_eduType { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public string y_cardId { get; set; }
        public int y_stuYear { get; set; }   //学制
        public int y_feeYear { get; set; }
        public string y_isUp { get; set; }
        public string y_isCheckFee { get; set; }
    }

}
#endregion