using Ionic.Zip;
using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Dal;
using IYun.Models;
using IYun.Object;
using log4net;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;


namespace IYun.Controllers
{
    /// <summary>
    /// 学籍管理
    /// </summary>
    public class StudentController : AdminBaseController
    {
        // GET: /Student/

        private readonly YD_Sts_StuInfoDal _stuInfoDal = new YD_Sts_StuInfoDal();
        private readonly YD_Sts_StuStrangeDal _stuStrangeDal = new YD_Sts_StuStrangeDal();
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #region 学籍信息模块         

        #region 新生信息模块


        /// <summary>
        /// 新生信息视图控制
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewlyStudentInfo(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var name = Request["name"];
                var stuState = Request["StuState"];
                var card = Request["card"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var tel = Request["tel"];
                var namenumcard = Request["namenumcard"];
                var nosub = Request["nosub"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolct = Convert.ToInt32(Request["schoolct"]);
                IQueryable<VW_StuInfo> list =
                   yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                       .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.DHLGDX.ToString())
                {
                    list = list.Where(x => x.y_stuStateId == 7||x.y_stuStateId == 8);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(nosub) && !nosub.Equals("0"))
                {
                    list = list.Where(u => u.y_subSchoolId == null);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard) ||
                                u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(stunum))
                {
                    list = list.Where(u => u.y_stuNum == stunum);
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(card))
                {
                    list = list.Where(u => u.y_cardId.Contains(card));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(tel))
                {
                    list = list.Where(u => u.y_tel.Contains(tel));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                if (schoolct != 0)
                {
                    var stuids =
                        yunEntities.YD_Sts_SubSchoolStuInfo.Where(k => k.y_isdel == 1)
                            .GroupBy(u => u.y_cardId)
                            .Where(u => u.Count() >= 2)
                            .Select(u => u.Key)
                            .ToList();

                    if (schoolct == 1)
                    {
                        list = list.Where(u => u.y_subSchoolId == null && stuids.Contains(u.y_cardId));
                    }
                    else
                    {
                        list = list.Where(u => u.y_subSchoolId != null || !stuids.Contains(u.y_cardId));
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.OrderBy(x => x.y_subSchoolId).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                model.ForEach(u =>
                {
                    //暂时用此字段保存是否有函授站冲突
                    if (u.y_subSchoolId == null && yunEntities.YD_Sts_SubSchoolStuInfo.Count(k => k.y_cardId == u.y_cardId && k.y_isdel == 1) >= 2)
                    {
                        u.y_isPay = 1;
                    }
                });

                if (Request.IsAjaxRequest())
                    return PartialView("NewlyStudentList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 学生信息查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult NewlyStudentDes(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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

            if (!id.HasValue)
            {
                return RedirectToAction(" NewlyStudentInfo");
            }
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                //获取异动信息
                var strange =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status)
                        .FirstOrDefault(u => u.y_stuId == id.Value);
                ViewBag.strange = strange;
                if (student == null)
                {
                    return RedirectToAction(" NewlyStudentInfo");
                }
                ViewData["student"] = student;
            }
            return View();
        }

        /// <summary>
        /// 学生信息编辑视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult NewlyEditStudent(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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

            if (!id.HasValue)
            {
                return RedirectToAction("NewlyStudentInfo");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                if (student == null)
                {
                    return RedirectToAction("NewlyStudentInfo");
                }
                ViewData["student"] = student;
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        /// <summary>
        /// 全校新生录取名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndMajorStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                ViewBag.majorliid = 0;
                ViewBag.edutypeid = 0;
                ViewBag.stutypeid = 0;
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    ViewBag.majorliid = majorliid.id;
                    ViewBag.majorli = majorliid.y_name;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    ViewBag.edutypeid = edutype.id;
                    ViewBag.edutype = edutype.y_name;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    ViewBag.stutypeid = stutypetype.id;
                    ViewBag.stutypetype = stutypetype.y_name;
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) || stuType.Equals("0") && string.IsNullOrWhiteSpace(eduType) ||
                    eduType.Equals("0") && string.IsNullOrWhiteSpace(majorLibrary) && majorLibrary.Equals("0"))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                return View(model);
            }
        }

        /// <summary>
        /// 函授站新生录取名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndSubSchoolStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var subSchool = Request["SubSchool"];
                ViewBag.majorliid = 0;
                ViewBag.edutypeid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.subschoolid = 0;
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subSchoolname = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subSchoolname != null)
                    {
                        ViewBag.subschoolid = subSchoolname.id;
                        ViewBag.subSchoolname = subSchoolname.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if ((string.IsNullOrWhiteSpace(stuType) || stuType.Equals("0")) &&
                    (string.IsNullOrWhiteSpace(eduType) ||
                     eduType.Equals("0")) && (string.IsNullOrWhiteSpace(majorLibrary) || majorLibrary.Equals("0")))
                {
                    model = null;
                    ViewBag.count = 0;
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var subadmin = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (subadmin != null)
                {
                    var sub = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subadmin.y_subSchoolId);
                    if (sub != null)
                        ViewBag.admin = sub.y_name;
                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校新生录取名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNewAndMajor()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new xinshengAllStudent
                                {
                                    eduTypeName = u.eduTypeName,
                                    majorLibraryName = u.majorLibraryName,
                                    stuTypeName = u.stuTypeName,
                                    count = count,
                                    y_examNum = u.y_examNum,
                                    y_name = u.y_name,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.y_cardId,
                                    y_postalcode = u.y_postalcode,
                                    y_tel = u.y_tel,
                                    y_address = u.y_address,
                                    schoolName = u.schoolName

                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/新生录取名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/新生录取名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "录取人数"},
                        {"y_examNum", "考生号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_postalcode", "邮政编码"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "所属函授站"}

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
        /// 全校新生录取名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult
            DownloadNewAndSubSchool()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new xinshengSubStudent
                                {
                                    eduTypeName = u.eduTypeName,
                                    majorLibraryName = u.majorLibraryName,
                                    stuTypeName = u.stuTypeName,
                                    count = count,
                                    y_examNum = u.y_examNum,
                                    y_name = u.y_name,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.y_cardId,
                                    y_postalcode = u.y_postalcode,
                                    y_tel = u.y_tel,
                                    y_address = u.y_address

                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站新生录取名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站新生录取名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "录取人数"},
                        {"y_examNum", "考生号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_postalcode", "邮政编码"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"}

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
        /// 学生数据下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult NewDownloadStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyStudentInfo");
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
                var sex = Request["sex"];
                var stuState = Request["StuState"];
                var card = Request["card"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard) ||
                                u.y_stuNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(sex))
                {
                    var sexint = Convert.ToInt32(sex);
                    list = list.Where(u => u.y_sex == sexint);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(card))
                {
                    list = list.Where(u => u.y_cardId.Contains(card));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                var lists = new List<StuList>();

                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    list.OrderByDescending(u => u.y_subSchoolId).ThenByDescending(e => e.y_majorId).ThenByDescending(e => e.y_stuNum).ThenByDescending(e => e.y_graduateNumber).ToList().ForEach(u =>
                    {
                        lists.Add(new StuList(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = yunEntities.YD_Edu_Major.FirstOrDefault(x => x.y_code == u.majorCode).y_StandardCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                }
                else
                {
                    list.OrderByDescending(u => u.id).ToList().ForEach(u =>
                    {
                        lists.Add(new StuList(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.majorLibraryCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                }
                //lists = lists.OrderBy(u =>u.id).ToList();
                var model = FileHelper.ToDataTable(lists);
                var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
                if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                {
                    var lists2 = new List<JXLGStuSub>();
                    list.OrderByDescending(u => u.id).ToList().ForEach(u =>
                    {
                        lists2.Add(new JXLGStuSub(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_nameabbreviation = u.y_nameabbreviation,
                            y_formername = u.y_formername,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.majorLibraryCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                    model = FileHelper.ToDataTable(lists2);
                }
                else if (schoolName == ComEnum.SchoolName.DHLGDX.ToString())
                {
                    var lists3 = new List<DHLGStuSub>();
                    list.OrderByDescending(u => u.id).ToList().ForEach(u =>
                    {
                        lists3.Add(new DHLGStuSub(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.y_StandardCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear,
                            y_ClassNum = u.y_classNum,
                            y_realYear = u.y_realYear.Value
                        });
                    });
                    model = FileHelper.ToDataTable(lists3);
                }
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/新生信息表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/新生信息表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorLibraryName", "专业名"},
                        {"majorLibraryCode", "专业代码"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"schoolCode", "函授站代码"},
                        {"y_cardId", "身份证"},
                        {"y_birthday", "出生日期"},
                        {"nationName", "民族"},
                        {"politicsName", "政治面貌"},
                        {"stuStateName", "学籍状态"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_inYear", "入学年份"}
                    };
                    if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "姓名"},
                            {"y_nameabbreviation", "函授站简称"},
                            {"y_formername", "函授站曾用名"},
                            {"y_sex", "性别"},
                            {"y_stuNum", "学号"},
                            {"y_examNum", "考生号"},
                            {"majorLibraryName", "专业名"},
                            {"majorLibraryCode", "专业代码"},
                            {"eduTypeName", "层次"},
                            {"stuTypeName", "学习形式"},
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_cardId", "身份证"},
                            {"y_birthday", "出生日期"},
                            {"nationName", "民族"},
                            {"politicsName", "政治面貌"},
                            {"stuStateName", "学籍状态"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_inYear", "入学年份"}
                        };
                    }
                    if (schoolName == ComEnum.SchoolName.DHLGDX.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "姓名"},
                            {"y_sex", "性别"},
                            {"y_stuNum", "学号"},
                            {"y_examNum", "考生号"},
                            {"majorLibraryName", "专业名"},
                            {"majorLibraryCode", "专业代码"},
                            {"y_realYear", "毕业年份" },
                            {"y_ClassNum", "班级号" },
                            {"eduTypeName", "层次"},
                            {"stuTypeName", "学习形式"},
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_cardId", "身份证"},
                            {"y_birthday", "出生日期"},
                            {"nationName", "民族"},
                            {"politicsName", "政治面貌"},
                            {"stuStateName", "学籍状态"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_inYear", "入学年份"},
                            {"y_graduateNumber", "毕业证号"},
                        };
                    }
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

        /// <summary>
        /// 新生导入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewlyUploadStudent(int id = 1)
        {
            #region 权限验证      

            var power = SafePowerPage("/Student/NewlyUploadStudent");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 缴费注册
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewlyFeeManage(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var inYear = Request["inYear"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var term = Request["term"];
                var isplanOK = Request["isplanOK"];
                var namenumcard = Request["namenumcard"];

                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.xinshengyear = xinshenyear;
                //var inyearint = Convert.ToInt32(inYear);
                var feeyearint = Convert.ToInt32(term);

                IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo
                    .Include(u => u.YD_Fee_StuFeeTb)
                    .Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                    .OrderByDescending(u => u.y_inYear)
                    .ThenByDescending(u => u.y_subSchoolId)
                    .ThenByDescending(u => u.id)
                    .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != null
                     && u.y_stuStateId != 4 && u.y_stuStateId != 6 && u.y_studentType != 2);

                //根据入学年份查询
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == yInYear);
                }
                var subSchool = Request["SubSchool"];
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                list =
                    list.Where(
                        u =>
                            !u.YD_Fee_StuFeeTb.Any(K => K.y_feeYear == 1) ||
                            u.YD_Fee_StuFeeTb.FirstOrDefault(K => K.y_feeYear == 1).y_isCheckFee ==
                            (int)YesOrNo.No);
                IEnumerable<YD_Sts_StuInfo> result = list;

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    result = result.Where(u => u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard)
                                           || u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                //if (inyearint != 0)
                //{
                //    list = list.Where(u => u.y_inYear == inyearint);
                //}
                //else
                //{
                //    list = list.Where(u => u.y_inYear == xinshenyear);
                //}
                ViewBag.admin = YdAdminRoleId;
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    result = result.Where(s => s.y_inYear == xinshenyear);
                }
               
                
                if (!string.IsNullOrWhiteSpace(isplanOK) && isplanOK != "0")
                {
                    if (isplanOK == "1") //已处理  
                    {
                        result =
                            result.Where(
                                u =>
                                    u.YD_Fee_StuFeeTb.Any(
                                        a =>
                                            (a.y_planconductOK == (int)YesOrNo.No ||
                                             a.y_planconductOK == (int)YesOrNo.Yes) && a.y_feeYear == feeyearint));
                    }
                    else if (isplanOK == "2") //未处理
                    {
                        result =
                            result.Where(
                                u =>
                                    !u.YD_Fee_StuFeeTb.Any(
                                        a =>
                                        //    a.y_planconductOK != (int) YesOrNo.No &&
                                        //    a.y_planconductOK != (int) YesOrNo.Yes 
                                        a.y_feeYear == feeyearint &&
                                        a.y_stuId == u.id));//2018-5-14焦赞
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    result = result.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var model = result.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                ViewBag.admin = YdAdminRoleId;
                ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];

                if (Request.IsAjaxRequest())
                    return PartialView("NewlyFeeManageList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 学生缴费审核下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult NewlyDownloadFeeManage()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var isplanOK = Request["isplanOK"];
                var term = Request["term"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                var feeyearint = Convert.ToInt32(term);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo
                      .Include(u => u.YD_Fee_StuFeeTb)
                      .Include(u => u.YD_Sys_SubSchool)
                      .Include(u => u.YD_Edu_Major)
                      .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                      .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                      .OrderByDescending(u => u.y_inYear)
                      .ThenByDescending(u => u.y_subSchoolId)
                      .ThenByDescending(u => u.id)
                      .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != null
                       && u.y_stuStateId != 4 && u.y_stuStateId != 6);

                list =
                    list.Where(
                        u =>
                            !u.YD_Fee_StuFeeTb.Any(K => K.y_feeYear == 1) ||
                            u.YD_Fee_StuFeeTb.FirstOrDefault(K => K.y_feeYear == 1).y_isCheckFee ==
                            (int)YesOrNo.No);
                //根据入学年份查询
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == yInYear);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard)
                                          || u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                var schoolName = "";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var schoolfirst = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    schoolName = schoolfirst != null ? schoolfirst.y_name : "";
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(isplanOK) && isplanOK != "0")
                {
                    if (isplanOK == "1") //已处理  
                    {
                        list = list.Where(u => u.YD_Fee_StuFeeTb.Any(a => (a.y_planconductOK == (int)YesOrNo.No ||
                          a.y_planconductOK == (int)YesOrNo.Yes) && a.y_feeYear == feeyearint));
                    }
                    else if (isplanOK == "2") //未处理
                    {
                        list = list.Where(u => u.YD_Fee_StuFeeTb.Any(a => a.y_planconductOK != (int)YesOrNo.No &&
                                              a.y_planconductOK != (int)YesOrNo.Yes && a.y_feeYear == feeyearint));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                DataTable model = new DataTable();
                if (ConfigurationManager.AppSettings["SchoolName"] == "JXSFDX")
                {
                    model = FileHelper.ToDataTable(
                   list.Select(
                       u =>
                           new FeeCheckList
                           {
                               y_stuNum = u.y_stuNum,
                               y_examNum = u.y_examNum,
                               y_name = u.y_name,
                               y_inYear = u.y_inYear,
                               schoolName = u.YD_Sys_SubSchool.y_name,
                               schoolCode = u.YD_Sys_SubSchool.y_code,
                               y_majorlibrary = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                               majorLibraryCode = u.YD_Edu_Major.y_StandardCode,
                               y_stuType = u.YD_Edu_Major.YD_Edu_StuType.y_name,
                               y_eduType = u.YD_Edu_Major.YD_Edu_EduType.y_name,
                               y_tel = u.y_tel,
                               y_address = u.y_address,
                               y_cardId = u.y_cardId,
                               y_stuYear = u.YD_Edu_Major.y_stuYear,
                               y_feeYear = feeyearint,
                               y_isUp = u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isUp == 0 ? "已申请" : "未申请",
                               y_isCheckFee =
                                   u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isCheckFee == 0 ? "已通过" : "待审核"
                           }).ToList());
                }
                else
                {
                    model = FileHelper.ToDataTable(
                    list.Select(
                        u =>
                            new FeeCheckList
                            {
                                y_stuNum = u.y_stuNum,
                                y_examNum = u.y_examNum,
                                y_name = u.y_name,
                                y_inYear = u.y_inYear,
                                schoolName = u.YD_Sys_SubSchool.y_name,
                                schoolCode = u.YD_Sys_SubSchool.y_code,
                                y_majorlibrary = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                majorLibraryCode = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_code,
                                y_stuType = u.YD_Edu_Major.YD_Edu_StuType.y_name,
                                y_eduType = u.YD_Edu_Major.YD_Edu_EduType.y_name,
                                y_tel = u.y_tel,
                                y_address = u.y_address,
                                y_cardId = u.y_cardId,
                                y_stuYear = u.YD_Edu_Major.y_stuYear,
                                y_feeYear = feeyearint,
                                y_isUp = u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isUp == 0 ? "已申请" : "未申请",
                                y_isCheckFee =
                                    u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isCheckFee == 0 ? "已通过" : "待审核"
                            }).ToList());
                }

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + schoolName + "新生缴费审核表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变
                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
                        {"schoolName", "函授站"},
                        {"schoolCode", "函授站代码"},
                        {"y_majorlibrary", "专业"},
                        {"majorLibraryCode", "专业代码"},
                        {"y_stuType", "学习形式"},
                        {"y_eduType", "层次"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_cardId", "身份证号"},
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
        /// 审核学生是否完善信息情况
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
            var feeYearint = Request["feeYear"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                if (stu != null)
                {
                    var fee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                    if (fee != null)
                    {
                        if (fee.y_isUp == (int)YesOrNo.Yes)
                        {
                            return "已经提交不允许修改";
                        }
                        else
                        {
                            fee.y_planconductOK = 1;
                            fee.y_NoplanconductReason = "";
                            yunEntities.Entry(fee).State = EntityState.Modified;

                            LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",审核学生是否完善信息情况,修改缴费记录,ID:" + fee.id + ",方法:NewlyPlanCheck");
                        }
                    }
                    else
                    {
                        //添加缴费记录                                      
                        var stuFee = new YD_Fee_StuFeeTb();
                        stuFee.y_feeYear = 1;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_stuId = oid;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFee.y_planconductOK = 1;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_NoplanconductReason = "";
                        yunEntities.Entry(stuFee).State = EntityState.Added;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + "审核学生是否完善信息情况,添加缴费记录,学生id为:" + stuFee.y_stuId + ",时间：" + stuFee.y_createtime + ",方法:NewlyPlanCheck");
                    }
                }
                else
                {
                    return "不存在该学生信息";
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
        /// 取消学生注册资格并填写原因
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheckNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
            var feeYearint = Request["feeYear"];
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            var PlanReason = Request["PlanReason"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(PlanReason))
            {
                return "原因必填";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var fee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                if (fee != null)
                {
                    if (fee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    else
                    {
                        fee.y_planconductOK = 0;
                        fee.y_NoplanconductReason = PlanReason;
                        yunEntities.Entry(fee).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",取消学生注册资格,修改缴费记录,ID:" + fee.id + ",方法:NewlyPlanCheckNo");

                    }

                }
                else
                {
                    //添加缴费记录                                      
                    var stuFee = new YD_Fee_StuFeeTb();
                    stuFee.y_feeYear = 1;
                    stuFee.y_isUp = (int)YesOrNo.No;
                    stuFee.y_isCheckFee = (int)YesOrNo.No;
                    stuFee.y_stuId = oid;
                    stuFee.y_needFee = 0;
                    stuFee.y_needUpFee = 0;
                    stuFee.y_planconductOK = 0;
                    stuFee.y_registerYear = DateTime.Now.Year;
                    stuFee.y_createtime = DateTime.Now;
                    stuFee.y_NoplanconductReason = PlanReason;
                    yunEntities.Entry(stuFee).State = EntityState.Added;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + "，取消学生注册资格,添加缴费记录,学生id为:" + stuFee.y_stuId + ",时间：" + stuFee.y_createtime + ",方法:NewlyPlanCheck");
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
        /// 在线完善学生信息
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheckwanshan()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
            var feeYearint = Request["feeYear"];
            var tel = Request["tel"];
            var adress = Request["adress"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(tel))
            {
                return "电话必填";
            }
            if (string.IsNullOrWhiteSpace(adress))
            {
                return "地址必填";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var fee =
                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                        .FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                if (fee != null)
                {
                    if (fee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    else
                    {
                        fee.y_planconductOK = 1;
                        fee.y_NoplanconductReason = "";
                        fee.YD_Sts_StuInfo.y_tel = tel;
                        fee.YD_Sts_StuInfo.y_address = adress;
                        //yunEntities.Entry(fee.YD_Sts_StuInfo).State = EntityState.Modified;
                        yunEntities.Entry(fee).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",在线完善学生信息,修改,ID:" + fee.id + ",方法:NewlyPlanCheckwanshan");
                    }
                }
                else //todo:要判断STU是否存在
                {
                    //添加缴费记录                                      
                    var stuFee = new YD_Fee_StuFeeTb();
                    stuFee.y_feeYear = 1;
                    stuFee.y_isUp = (int)YesOrNo.No;
                    stuFee.y_isCheckFee = (int)YesOrNo.No;
                    stuFee.y_stuId = oid;
                    stuFee.y_needFee = 0;
                    stuFee.y_needUpFee = 0;
                    stuFee.y_planconductOK = 1;
                    stuFee.y_registerYear = DateTime.Now.Year;
                    stuFee.y_createtime = DateTime.Now;
                    stuFee.y_NoplanconductReason = "";
                    yunEntities.Entry(stuFee).State = EntityState.Added;

                    var stu = yunEntities.YD_Sts_StuInfo.Find(oid);
                    stu.y_tel = tel;
                    stu.y_address = adress;

                    yunEntities.Entry(stu).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",在线完善学生信息,添加缴费记录,学生id为:" + stuFee.y_stuId + ",时间：" + stuFee.y_createtime + ",方法:NewlyPlanCheckwanshan");
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
        /// 撤销学生注册操作
        /// </summary>
        /// <returns></returns>
        public string RevocationPlanconduct()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
            var feeyear = Request["feeYear"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeyear))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(id);
                var feeint = Convert.ToInt32(feeyear);
                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == feeint);
                if (stuFee != null)
                {
                    if (stuFee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    stuFee.y_planconductOK = null;
                    stuFee.y_NoplanconductReason = "";
                    yunEntities.Entry(stuFee).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",撤销学生注册操作,修改,ID:" + stuFee.id + ",方法:RevocationPlanconduct");
                }
                yunEntities.SaveChanges();
                return "ok";
            }
        }

        /// <summary>
        /// 批量处理该站点学生为预注册
        /// </summary>
        /// <returns></returns>
        public string CheckboxtruePlanCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);
                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
                          u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId
                          && u.YD_Sts_StuInfo.y_inYear == inyearint
                              && u.y_feeYear == 1 //学年不用管，三条条件目标是把大部分的学生给注册,留下零零散散的让函授站自己点
                                                  //&& u.y_isUp == (int)YesOrNo.No //干掉的原因：不论有没有审核过我都不要你了
                                                  //&& u.y_planconductOK == null //已经进入过一次表的都不进入批量处理
                              ).ToList();//List是已经注册的学生
                                         //找出符合下列条件的学生
                                         //1.函授站ID 2.注册年份
                    var stuList = list.Select(e => e.y_stuId);
                    int year = Convert.ToInt32(inyear);
                    var registerStus = yunEntities.YD_Sts_StuInfo.Where(e => e.y_subSchoolId == sub.y_subSchoolId && e.y_inYear == year).ToList();
                    foreach (var stu in stuList)//循环去掉已经预注册的学生
                    {
                        registerStus = registerStus.Where(e => e.id != stu).ToList();
                    }
                    int loopTime = 0;
                    if (stuList.Any())//被取消预注册的和撤销的在“批量处理”后也全部预注册
                    {
                        using (var db = new IYunEntities())
                        {
                            foreach (var stu in stuList)
                            {
                                var stuEntity = db.YD_Fee_StuFeeTb.SingleOrDefault(e => e.y_stuId == stu &&
                                e.y_feeYear == 1);
                                if (stuEntity.y_planconductOK != 0)
                                {
                                    stuEntity.y_planconductOK = 1;
                                    stuEntity.y_NoplanconductReason = "";
                                }
                            }
                            loopTime += db.SaveChanges();
                        }
                    }
                    if (registerStus.Any())
                    {
                        using (var Db = new IYunEntities())
                        {

                        }
                        //var stuList = list.Select(e => e.id);
                        //var registerStus = yunEntities.YD_Sts_StuInfo.ToList();
                        //foreach(var stu in stuList)//循环去掉已经预注册的学生
                        //{
                        //    registerStus = registerStus.Where(e => e.id != stu).ToList();
                        //}
                        //开始预注册
                        using (var Db = new IYunEntities())
                        {
                            foreach (var registerStu in registerStus)
                            {
                                var stuFee = new YD_Fee_StuFeeTb();
                                stuFee.y_feeYear = 1;
                                stuFee.y_isUp = (int)YesOrNo.No;
                                stuFee.y_isCheckFee = (int)YesOrNo.No;
                                stuFee.y_stuId = registerStu.id;
                                stuFee.y_needFee = 0;
                                stuFee.y_needUpFee = 0;
                                stuFee.y_planconductOK = 1;
                                stuFee.y_registerYear = DateTime.Now.Year;
                                stuFee.y_createtime = DateTime.Now;
                                stuFee.y_NoplanconductReason = "";
                                Db.YD_Fee_StuFeeTb.Add(stuFee);

                                LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "批量操作预注册：用户ID:" + Session[KeyValue.Admin_id] +
                                    ",用户名：" + Session[KeyValue.Admin_Name]
                                    + "审核学生是否完善信息情况,添加缴费记录,学生id为:"
                                    + stuFee.y_stuId + ",时间：" + stuFee.y_createtime
                                    + ",方法:NewlyPlanCheck");
                            }
                            loopTime += Db.SaveChanges();
                        }
                        //list.ForEach(u =>
                        //{
                        //    u.y_planconductOK = 1;
                        //    u.y_NoplanconductReason = "";
                        //    yunEntities.Entry(u).State = EntityState.Modified;
                        //    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量处理该站点学生为预注册,修改缴费记录,ID:" + u.id + ",方法:CheckboxtruePlanCheck");
                        //});
                        yunEntities.SaveChanges();
                        if (loopTime > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "设置失败";
                        }
                    }
                    else
                    {
                        if (loopTime > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "没有学生可以设置";
                        }
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
            }
        }
        /// <summary>
        /// 批量取消该站点预注册学生
        /// </summary>
        /// <returns></returns>
        public string CheckboxfalsePlanCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);
                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
                          u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId
                          && u.YD_Sts_StuInfo.y_inYear == inyearint && u.y_feeYear == feeyearint
                              && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK == 1).ToList();
                    if (list.Any())
                    {
                        list.ForEach(u =>
                        {
                            u.y_planconductOK = null;
                            u.y_NoplanconductReason = "";
                            yunEntities.Entry(u).State = EntityState.Modified;

                            LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量取消该站点预注册学生,修改缴费记录,ID:" + u.id + ",方法:CheckboxfalsePlanCheck");
                        });
                        int w = yunEntities.SaveChanges();
                        if (w > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "设置失败";
                        }
                    }
                    else
                    {
                        return "没有学生可以设置";
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
            }
        }


        /// <summary>
        /// 批量处理该站点老生预注册
        /// </summary>
        /// <returns></returns>
        public string CheckboxtruePlanCheckOld()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);
                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
                          u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId
                          && u.YD_Sts_StuInfo.y_inYear == inyearint
                              && u.y_feeYear == feeyearint && u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear>= feeyearint
                              //学制必须大于或等于缴费学年，不能存在如3年制的学生有第4，5学年的缴费记录
                              //学年不用管，三条条件目标是把大部分的学生给注册,留下零零散散的让函授站自己点
                              //&& u.y_isUp == (int)YesOrNo.No //干掉的原因：不论有没有审核过我都不要你了
                              //&& u.y_planconductOK == null //已经进入过一次表的都不进入批量处理
                              ).ToList();//List是已经注册的学生
                                         //找出符合下列条件的学生
                                         //1.函授站ID 2.注册年份
                    var stuList = list.Select(e => e.y_stuId);
                    int year = Convert.ToInt32(inyear);
                    var registerStus = yunEntities.YD_Sts_StuInfo.Where(e => e.y_subSchoolId == sub.y_subSchoolId && e.y_inYear == year).ToList();
                    foreach (var stu in stuList)//循环去掉已经预注册的学生
                    {
                        registerStus = registerStus.Where(e => e.id != stu).ToList();
                    }
                    int loopTime = 0;
                    if (stuList.Any())//被取消预注册的和撤销的在“批量处理”后也全部预注册
                    {
                        using (var db = new IYunEntities())
                        {
                            foreach (var stu in stuList)
                            {
                                var stuEntity = db.YD_Fee_StuFeeTb.SingleOrDefault(e => e.y_stuId == stu &&
                                e.y_feeYear == feeyearint);
                                if (stuEntity.y_planconductOK != 0)
                                {
                                    stuEntity.y_planconductOK = 1;
                                    stuEntity.y_NoplanconductReason = "";
                                }
                            }
                            loopTime += db.SaveChanges();
                        }
                    }
                    if (registerStus.Any())
                    {
                        using (var Db = new IYunEntities())
                        {

                        }
                        //var stuList = list.Select(e => e.id);
                        //var registerStus = yunEntities.YD_Sts_StuInfo.ToList();
                        //foreach(var stu in stuList)//循环去掉已经预注册的学生
                        //{
                        //    registerStus = registerStus.Where(e => e.id != stu).ToList();
                        //}
                        //开始预注册
                        using (var Db = new IYunEntities())
                        {
                            foreach (var registerStu in registerStus)
                            {
                                var stuFee = new YD_Fee_StuFeeTb();
                                stuFee.y_feeYear = Convert.ToInt32(feeyear);
                                stuFee.y_isUp = (int)YesOrNo.No;
                                stuFee.y_isCheckFee = (int)YesOrNo.No;
                                stuFee.y_stuId = registerStu.id;
                                stuFee.y_needFee = 0;
                                stuFee.y_needUpFee = 0;
                                stuFee.y_planconductOK = 1;
                                stuFee.y_registerYear = DateTime.Now.Year;
                                stuFee.y_createtime = DateTime.Now;
                                stuFee.y_NoplanconductReason = "";
                                Db.YD_Fee_StuFeeTb.Add(stuFee);

                                LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "批量操作预注册：用户ID:" + Session[KeyValue.Admin_id] +
                                    ",用户名：" + Session[KeyValue.Admin_Name]
                                    + "审核学生是否完善信息情况,添加缴费记录,学生id为:"
                                    + stuFee.y_stuId + ",时间：" + stuFee.y_createtime
                                    + ",方法:NewlyPlanCheck");
                            }
                            loopTime += Db.SaveChanges();
                        }
                        //list.ForEach(u =>
                        //{
                        //    u.y_planconductOK = 1;
                        //    u.y_NoplanconductReason = "";
                        //    yunEntities.Entry(u).State = EntityState.Modified;
                        //    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量处理该站点学生为预注册,修改缴费记录,ID:" + u.id + ",方法:CheckboxtruePlanCheck");
                        //});
                        yunEntities.SaveChanges();
                        if (loopTime > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "设置失败";
                        }
                    }
                    else
                    {
                        if (loopTime > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "没有学生可以设置";
                        }
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
            }
        }
        /// <summary>
        /// 批量取消该站点预注册老生
        /// </summary>
        /// <returns></returns>
        public string CheckboxfalsePlanCheckOld()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);
                    var list =
                        yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
                          u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId
                          && u.YD_Sts_StuInfo.y_inYear == inyearint && u.y_feeYear == feeyearint
                              && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK == 1).ToList();
                    if (list.Any())
                    {
                        list.ForEach(u =>
                        {
                            u.y_planconductOK = null;
                            u.y_NoplanconductReason = "";
                            yunEntities.Entry(u).State = EntityState.Modified;

                            LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量取消该站点预注册学生,修改缴费记录,ID:" + u.id + ",方法:CheckboxfalsePlanCheck");
                        });
                        int w = yunEntities.SaveChanges();
                        if (w > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "设置失败";
                        }
                    }
                    else
                    {
                        return "没有学生可以设置";
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
            }
        }
        /// <summary>
        /// 选择性提交学生缴费情况
        /// </summary>
        /// <returns></returns>
        public string PlanconductCheckSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyFeeManage");
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
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);

                    var ishave =
                        yunEntities.YD_Sts_StuInfo.Where(
                            u => sub.Contains(u.y_subSchoolId.Value) && u.y_inYear == inyearint && u.y_isdel == 1 && u.y_stuStateId != 4).ToList()
                            .Any(
                                u =>
                                    u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint) == false ||
                                    u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint && k.y_planconductOK == null));

                    if (!ishave)
                    {
                        IQueryable<YD_Fee_StuFeeTb> list =
                            yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                            .OrderByDescending(u => u.id)
                            .Where(u => sub.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value) &&//函授站ID
                                        u.YD_Sts_StuInfo.y_inYear == inyearint &&//学年相同
                                        u.y_feeYear == feeyearint &&//学年相同
                                        u.y_isUp == (int)YesOrNo.No &&//
                                        u.y_planconductOK == 1);

                        if (!list.Any())
                        {
                            return "没有能提交的学生";
                        }
                        var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => sub.Contains(u.id));
                        var stuidlist = string.Join(",", list.Select(u => u.y_stuId).ToArray()); //所有stuid根据逗号相加成一个字符串
                        if (!stuidlist.EndsWith(","))
                        {
                            stuidlist += ",";
                        }
                        var entitylist = list;//.ToList();
                        var obj = new YD_Fee_StuRegistrBatch();
                        if (school != null)
                        {
                            obj.y_subSchoolId = school.id;
                            obj.schoolCode = school.y_code;
                            obj.schoolName = school.y_name;
                            obj.y_feeyear = feeyearint;
                            obj.y_inyear = inyearint;
                            if(schoolname == ComEnum.SchoolName.DHLGDX.ToString())
                            {
                                var governorName = Request["governorName"];
                                if (string.IsNullOrWhiteSpace(governorName))
                                {
                                    return "请输入负责人姓名";
                                }
                                obj.governorName = governorName;
                            }
                        }
                        else
                        {
                            return "函授站信息有误！";
                        }
                        obj.totalcount = entitylist.Count();
                        obj.y_time = DateTime.Now;
                        obj.y_check = 0;
                        obj.y_stuid = stuidlist;
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            #region  计算学费总金额和缴费总金额以及学生的个人金额
                            var idlist = stuidlist;
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
                                 ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                                 "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                                 "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                                 "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                                 "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                                 "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                                 "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                                 "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                            var lists = yunEntities.Database.SqlQuery<BiliDto>(sqls).ToList();
                            decimal needUpFee = 0;
                            decimal needFee = 0;
                            entitylist.ToList().ForEach(u =>
                            {
                                var y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                                var y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                                var sb = new StringBuilder("UPDATE [YD_Fee_StuFeeTb] ");
                                sb.AppendLine(" SET [y_needFee]=" + y_needFee + ",y_needUpFee=" + y_needUpFee + " WHERE id=" + u.id);
                                string sql = sb.ToString();
                                yunEntities.Database.ExecuteSqlCommand(sql);
                                needFee += y_needFee;
                                needUpFee += y_needUpFee;
                                //u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                                //u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                                //yunEntities.Entry(u).State = EntityState.Modified;
                            });
                            obj.needtotal = needUpFee;
                            obj.tuitiontotal = needFee;
                            //obj.needtotal = list.Where(u => u.y_feeYear == feeyearint).ToList().Sum(u => u.y_needUpFee);
                            //obj.tuitiontotal = list.Where(u => u.y_feeYear == feeyearint).ToList().Sum(u => u.y_needFee);
                            #endregion
                        }
                        else
                        {
                            obj.tuitiontotal = 0;
                            obj.needtotal = 0;
                        }
                        yunEntities.Entry(obj).State = EntityState.Added;
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            LogHelper.DbLog(YdAdminId, YdAdminRelName,
                           (int)LogType.Update, "时间:" + DateTime.Now + "申请注册,申请总学费为" + obj.tuitiontotal + ",缴费总金额为" + obj.needtotal);
                        }
                        var statename = "注册待审核";
                        var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
                        if (state == null)
                        {
                            return "找不到注册待审核状态枚举";
                        }
                        //entitylist.ForEach(u =>
                        //{
                        //    u.YD_Sts_StuInfo.y_stuStateId = state.id;
                        //    u.y_isCheckFee = (int) YesOrNo.No;
                        //    u.y_isUp = (int) YesOrNo.Yes;

                        //    yunEntities.Entry(u.YD_Sts_StuInfo).State = EntityState.Modified;
                        //    yunEntities.Entry(u).State = EntityState.Modified;
                        //});
                        foreach (var entity in entitylist)
                        {
                            entity.YD_Sts_StuInfo.y_stuStateId = state.id;
                            entity.y_isCheckFee = (int)YesOrNo.No;
                            entity.y_isUp = (int)YesOrNo.Yes;
                        }
                        int w = yunEntities.SaveChanges();
                        if (w > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "提交失败";
                        }
                    }
                    else
                    {
                        return "没有处理完同年级同一缴费学年学生不能提交";
                    }
                }
                else
                {
                    return "该账号没有指定函授站";
                }
            }
        }

        /// <summary>
        /// 新生注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewlyRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 全校新生已注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                ViewBag.majorliid = 0;
                ViewBag.edutypeid = 0;
                ViewBag.stutypeid = 0;
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.y_majorId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null
                            && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.Yes);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) && string.IsNullOrWhiteSpace(eduType)
                    && (string.IsNullOrWhiteSpace(majorLibrary)))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校新生已注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNewRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.y_majorId)
                    .
                    Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_subSchoolId != null
                               && u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_feeYear == 1 &&
                               u.y_isCheckFee == (int)YesOrNo.Yes);
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
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Include(u => u.YD_Sts_StuInfo).Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name

                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/新生已注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/新生已注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "所属函授站"}

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
        /// 全校新生未注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewNoRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear
                               && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) && string.IsNullOrWhiteSpace(eduType)
                    && (string.IsNullOrWhiteSpace(majorLibrary)))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校新生未注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNewNoRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear
                               && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.No);
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
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name
                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/新生未注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/新生未注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "所属函授站"}

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
        /// 函授站新生已注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndSubschoolRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var subSchool = Request["SubSchool"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.subschoolid = 0;
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null
                            && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.Yes);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null)
                    {
                        ViewBag.subschoolid = subschool.id;
                        ViewBag.subschool = subschool.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) && string.IsNullOrWhiteSpace(eduType)
                    && (string.IsNullOrWhiteSpace(majorLibrary) && string.IsNullOrWhiteSpace(subSchool)))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                ViewBag.admin = YdAdminRoleId;
                return View(model);
            }
        }

        /// <summary>
        /// 函授站新生已注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNewAndSubschoolRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_feeYear == 1 &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null
                            && u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes);
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
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name

                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站已注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站已注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "站点"}
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
        /// 函授站新生未注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndSubschoolNoRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var subSchool = Request["SubSchool"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.subschoolid = 0;
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null
                            && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null)
                    {
                        ViewBag.subschoolid = subschool.id;
                        ViewBag.subschool = subschool.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) && string.IsNullOrWhiteSpace(eduType)
                    && string.IsNullOrWhiteSpace(majorLibrary))
                {
                    model = null;
                    ViewBag.count = 0;
                }
                return View(model);
            }
        }

        /// <summary>
        /// 函授站新生未注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNewAndSubschoolNoRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.y_inYear == xinshenyear &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null
                            && u.y_feeYear == 1 && u.y_isCheckFee == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
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
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName =
                                        u.YD_Sts_StuInfo.YD_Sys_SubSchool == null
                                            ? ""
                                            : u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name
                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站未注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站未注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "站点"}
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
        /// 新生注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NewlysRegistertatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlysRegistertatistics");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 全校新生已注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NewRegisterStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return RedirectToAction("Index", "AdminBase");
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var edutypeid = 0;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                string sql;
                //if (edutypeid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1 "+
                //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_eduTypeId = " + edutypeid + " and y_isCheckFee="+(int)YesOrNo.Yes+ "and y_feeYear=1 " +
                //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.y_feeYear == 1);

                if (edutypeid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改
                ViewBag.edutypeid = edutypeid;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;

                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NewRegisterStuStatisticsPost(int id)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var ad = new IYunEntities())
            {
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var edutypeid = id;
                //string sql;
                //if (edutypeid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //       where y_inyear = " + xinshen + " and y_isCheckFee=" + (int)YesOrNo.Yes+ "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //       where y_inyear = " + xinshen + " and y_eduTypeId = " + edutypeid+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                ViewBag.edutypeid = edutypeid;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.y_feeYear == 1);

                if (edutypeid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        ///  全校新生已注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownNewRegisterStuStatistics(int id)
        {
            if (!IsLogin())
            {
                var reurl = Request.UrlReferrer.ToString();
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            string sql;
            using (var ad = new IYunEntities())
            {
                //if (id == 0)
                //{
                //        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.Yes+ "and y_feeYear=1" +
                //           " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_eduTypeId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}

                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.y_feeYear == 1);

                if (id != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改

                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/全校新生已注册统计表" + ".xls"; //todo:改变
                var fileName = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/全校新生已注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }


        }

        /// <summary>
        /// 全校新生未注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NewNoRegisterStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return RedirectToAction("Index", "AdminBase");
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var edutypeid = 0;
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                var xinshenint = Convert.ToInt32(xinshen);
                //string sql;
                //if (edutypeid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No+ "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";

                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_eduTypeId = " + edutypeid+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                ViewBag.edutypeid = edutypeid;

                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshenint && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear == 1);

                if (edutypeid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改

                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NewNoRegisterStuStatisticsPost(int id)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var ad = new IYunEntities())
            {
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var edutypeid = id;
                //string sql;
                //if (edutypeid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_eduTypeId = " + edutypeid+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                ViewBag.edutypeid = edutypeid;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear == 1);

                if (edutypeid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改

                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        ///  全校新生未注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownNewNoRegisterStuStatistics(int id)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            string sql;
            using (var ad = new IYunEntities())
            {
                //if (id == 0)
                //{

                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_eduTypeId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}

                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();



                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear == 1);

                if (id != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                }
                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改

                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });
                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/全校新生未注册统计表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/全校新生未注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }


        }

        /// <summary>
        /// 函授站新生已注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndSubschoolStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return RedirectToAction("Index", "AdminBase");
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var schoolid = 0;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                string sql;
                //if (schoolid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";                   
                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                //}
                ViewData["schoolid"] = schoolid;

                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.y_feeYear == 1);

                if (schoolid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                }

                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NewAndSubschoolStuStatisticsPost(int id)
        {
            using (var ad = new IYunEntities())
            {
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolid = id;
                if (!IsLogin())
                {
                    return Redirect("/AdminBase/Index");
                }
                string sql;
                var list = new List<NewStuStatistics>();
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == xinshen && u.YD_Sts_StuInfo.y_subSchoolId == schoolid &&
                                u.y_isCheckFee == (int)YesOrNo.No && u.y_feeYear == 1);
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                    //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                    //    " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == 1);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewData["schoolid"] = schoolid;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();

                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        /// 函授站新生已注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownNewAndSubschoolStuStatistics(int id)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            string sql;
            using (var ad = new IYunEntities())
            {
                var list = new List<NewStuStatistics>();

                if (YdAdminRoleId == 4)
                {
                    id = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                if (id == 0)
                {
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u => u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.Yes
                                 && u.y_feeYear == 1);
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改

                    //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + xinshen + " and y_subSchoolId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                    //     " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }
                else
                {
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u => u.YD_Sts_StuInfo.y_inYear == xinshen && u.YD_Sts_StuInfo.y_subSchoolId == id
                                 && u.y_isCheckFee == (int)YesOrNo.Yes && u.y_feeYear == 1);
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                    //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.Yes + "and y_feeYear=1" +
                    //    " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }

                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();

                //int i = 1;
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站新生已注册统计表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站新生已注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }
        }

        /// <summary>
        /// 函授站新生未注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAndSubschoolNoStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {

                return RedirectToAction("Index", "AdminBase");

            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目

                var schoolid = 0;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                string sql;
                //if (schoolid == 0)
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";

                //}
                //else
                //{
                //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                //          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                //           where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";

                //}

                ViewData["schoolid"] = schoolid;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var tblist =
                    ad.YD_Fee_StuFeeTb.Where(
                        u =>
                            u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear == 1);

                if (schoolid != 0)
                {
                    tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                }
                var list =
                    tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Select(
                            u =>
                                new NewStuStatistics
                                {
                                    cc = u.Key.YD_Edu_EduType.y_name,
                                    xs = u.Key.YD_Edu_StuType.y_name,
                                    zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                    counts = u.Count()
                                }).ToList(); //todo:测试修改
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NewAndSubschoolNoStuStatisticsPost(int id)
        {
            using (var ad = new IYunEntities())
            {
                var list = new List<NewStuStatistics>();
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolid = id;
                if (!IsLogin())
                {
                    return Redirect("/AdminBase/Index");
                }
                string sql;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == xinshen && u.YD_Sts_StuInfo.y_subSchoolId == schoolid &&
                                u.y_isCheckFee == (int)YesOrNo.No && u.y_feeYear == 1);
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                    //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                    //    " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == 1);

                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewData["schoolid"] = schoolid;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        /// 函授站新生未注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownNewAndSubschoolNoStuStatistics(int id)
        {
            var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            string sql;
            using (var ad = new IYunEntities())
            {
                var list = new List<NewStuStatistics>();
                if (id == 0)
                {
                    if (!IsLogin())
                    {
                        //var reurl = Request.UrlReferrer.ToString();
                        var reurl = "/AdminBase/Index";
                        return @"alert('错误');window.location.href='" + reurl + "';";
                    }

                    if (YdAdminRoleId == 4)
                    {
                        id = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                        var tblist =
                            ad.YD_Fee_StuFeeTb.Where(
                                u =>
                                    u.YD_Sts_StuInfo.y_inYear == xinshen && u.YD_Sts_StuInfo.y_subSchoolId == id &&
                                    u.y_isCheckFee == (int)YesOrNo.No && u.y_feeYear == 1);
                        list =
                            tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                                .Select(
                                    u =>
                                        new NewStuStatistics
                                        {
                                            cc = u.Key.YD_Edu_EduType.y_name,
                                            xs = u.Key.YD_Edu_StuType.y_name,
                                            zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                            counts = u.Count()
                                        }).ToList(); //todo:测试修改
                        //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                        //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                        //   where y_inyear = " + xinshen + " and y_subSchoolId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                        //     " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    }
                    else
                    {
                        //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                        //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                        //   where y_inyear = " + xinshen+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                        //    " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                        var tblist =
                            ad.YD_Fee_StuFeeTb.Where(
                                u =>
                                    u.YD_Sts_StuInfo.y_inYear == xinshen && u.y_isCheckFee == (int)YesOrNo.No &&
                                    u.y_feeYear == 1);
                        list =
                            tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                                .Select(
                                    u =>
                                        new NewStuStatistics
                                        {
                                            cc = u.Key.YD_Edu_EduType.y_name,
                                            xs = u.Key.YD_Edu_StuType.y_name,
                                            zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                            counts = u.Count()
                                        }).ToList(); //todo:测试修改
                    }
                }
                else
                {
                    //sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + xinshen + " and y_subSchoolId = " + id+ " and y_isCheckFee=" + (int)YesOrNo.No + "and y_feeYear=1" +
                    //    " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == xinshen && u.YD_Sts_StuInfo.y_subSchoolId == id &&
                                u.y_isCheckFee == (int)YesOrNo.No && u.y_feeYear == 1);
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }

                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();

                //int i = 1;
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站新生未注册统计表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站新生未注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }


        }

        /// <summary>
        /// 打印新生录取通知书
        /// </summary>
        /// <returns></returns>
        public ActionResult NewlyPrintNotice(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyPrintNotice");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname) ||
                                u.y_examNum == cardandname);
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null) ViewBag.subschoolid = subschool.id;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorli = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorli != null) ViewBag.majorliid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var majorli = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (majorli != null) ViewBag.edutypeid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var majorli = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (majorli != null) ViewBag.stutypeid = majorli.id;
                }
                var model = list.OrderByDescending(x=>x.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("NewlyPrintNoticeList", model);
                return View(model);
            }
        }
        public ActionResult NewlyPrintNoticeTwo(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyPrintNoticeTwo");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname) ||
                                u.y_examNum == cardandname);
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null) ViewBag.subschoolid = subschool.id;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorli = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorli != null) ViewBag.majorliid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var majorli = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (majorli != null) ViewBag.edutypeid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var majorli = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (majorli != null) ViewBag.stutypeid = majorli.id;
                }
                var model = list.OrderByDescending(x => x.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("NewlyPrintNoticeTwoList", model);
                return View(model);
            }
        }
        public ActionResult NewlyPrintNoticeThree(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyPrintNoticeThree");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname) ||
                                u.y_examNum == cardandname);
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null) ViewBag.subschoolid = subschool.id;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorli = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorli != null) ViewBag.majorliid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var majorli = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (majorli != null) ViewBag.edutypeid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var majorli = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (majorli != null) ViewBag.stutypeid = majorli.id;
                }
                var model = list.OrderByDescending(x => x.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("NewlyPrintNoticeThreeList", model);
                return View(model);
            }
        }
        public ActionResult NewlyPrintNoticeFour(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyPrintNoticeFour");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname) ||
                                u.y_examNum == cardandname);
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null) ViewBag.subschoolid = subschool.id;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorli = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorli != null) ViewBag.majorliid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var majorli = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (majorli != null) ViewBag.edutypeid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var majorli = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (majorli != null) ViewBag.stutypeid = majorli.id;
                }
                var model = list.OrderByDescending(x => x.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("NewlyPrintNoticeFourList", model);
                return View(model);
            }
        }
        public ActionResult NewlyPrintNoticeFive(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyPrintNoticeFive");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stunum = Request["stuNum"];
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname) ||
                                u.y_examNum == cardandname);
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null) ViewBag.subschoolid = subschool.id;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                    var majorli = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorli != null) ViewBag.majorliid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                    var majorli = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (majorli != null) ViewBag.edutypeid = majorli.id;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                    var majorli = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (majorli != null) ViewBag.stutypeid = majorli.id;
                }
                var model = list.OrderByDescending(x => x.id).ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("NewlyPrintNoticeFiveList", model);
                return View(model);
            }
        }
        public static int StrToInt(string str)
        {
            return int.Parse(str);
        }
        /// <summary>
        /// 新生入学通知书打印
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult NewPrintNotice()
        {
            using (var yunEntities = new IYunEntities())
            {
                string stuid = string.Empty;
                IQueryable<VW_StuInfo> list = null;
                int[] ids = null;
                //if (Request["ids"] != null && Request["ids"]!="")
                //{
                //    stuid = Request["ids"];
                //    stuid = stuid.TrimEnd(',');
                //    string[] aa = stuid.Split(',');

                //    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                //}

                if (Request["stuid"] != null && Request["stuid"] != "")
                {
                    stuid = Request["stuid"];
                    stuid = stuid.TrimEnd(',');
                    string[] aa = stuid.Split(',');

                    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                }

                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    list = list.Where(u => ids.Contains(u.id));
                  
                    //List<VW_StuInfo> stuinfoList = new List<VW_StuInfo>();
                    //foreach(var item in ids)
                    //{
                    //    stuinfoList.Add(list.FirstOrDefault(x => x.id == item));
                    //}
                    //ViewData["list"] = stuinfoList;
                    //return View();
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }

                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }

                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.OrderByDescending(x=>x.id).ToList();
                return View();

            }
        }

        public ActionResult NewPrintNoticeTwo()
        {
            using (var yunEntities = new IYunEntities())
            {
                string stuid = string.Empty;
                IQueryable<VW_StuInfo> list = null;
                int[] ids = null;
                if (Request["stuid"] != null && Request["stuid"] != "")
                {
                    stuid = Request["stuid"];
                    stuid = stuid.TrimEnd(',');
                    string[] aa = stuid.Split(',');

                    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                }

                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    list = list.Where(u => ids.Contains(u.id));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }

                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }

                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.OrderByDescending(x => x.id).ToList();
                return View();

            }
        }
        public ActionResult NewPrintNoticeThree() 
        {
            using (var yunEntities = new IYunEntities())
            {
                string stuid = string.Empty;
                IQueryable<VW_StuInfo> list = null;
                int[] ids = null;
                if (Request["stuid"] != null && Request["stuid"] != "")
                {
                    stuid = Request["stuid"];
                    stuid = stuid.TrimEnd(',');
                    string[] aa = stuid.Split(',');

                    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                }

                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    list = list.Where(u => ids.Contains(u.id));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }

                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }

                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.OrderByDescending(x => x.id).ToList();
                return View();

            }
        }
        public ActionResult NewPrintNoticeFour()
        {
            using (var yunEntities = new IYunEntities())
            {
                string stuid = string.Empty;
                IQueryable<VW_StuInfo> list = null;
                int[] ids = null;
                if (Request["stuid"] != null && Request["stuid"] != "")
                {
                    stuid = Request["stuid"];
                    stuid = stuid.TrimEnd(',');
                    string[] aa = stuid.Split(',');

                    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                }

                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    list = list.Where(u => ids.Contains(u.id));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }

                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }

                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.OrderByDescending(x => x.id).ToList();
                return View();

            }
        }
        public ActionResult NewPrintNoticeFive()
        {
            using (var yunEntities = new IYunEntities())
            {
                string stuid = string.Empty;
                IQueryable<VW_StuInfo> list = null;
                int[] ids = null;
                if (Request["stuid"] != null && Request["stuid"] != "")
                {
                    stuid = Request["stuid"];
                    stuid = stuid.TrimEnd(',');
                    string[] aa = stuid.Split(',');

                    ids = Array.ConvertAll(aa, new Converter<string, int>(StrToInt));
                }

                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_inYear == xinshenyear);


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    list = list.Where(u => ids.Contains(u.id));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }

                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }

                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.OrderByDescending(x => x.id).ToList();
                return View();

            }
        }
        #endregion

        #region 旁听生信息视图

        /// <summary>
        /// 旁听生信息视图控制
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult AuditorInfo(int id = 1)
        {
            #region 权限验证        

            var power = SafePowerPage("/Student/AuditorInfo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                const int isauditor = (int)Object.StudentType.旁听生;
                IQueryable<VW_StuInfo> list = yunEntities.VW_StuInfo.OrderByDescending(u => u.y_inYear).
                    Where(u => u.y_isdel == isnotdel && u.y_studentType == isauditor);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard || u.y_cardId == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
               
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                {
                    return PartialView("AuditorInfoList", model);
                }
                return View(model);
            }
        }

        /// <summary>
        /// 旁听生信息添加视图控制
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult AddAuditor()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/AuditorInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
            }
            ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
            return View();
        }

        /// <summary>
        /// 旁听生信息保存AJAX
        /// </summary>
        /// <param name="stu">学生信息扩展对象</param>
        /// <returns>处理结果json</returns>
        public string SaveAuditorInfo(YD_Sts_StuInfo stu)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/AuditorInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var subSchool = Request["SubSchool"];
            var realyear = Convert.ToInt32(Request["y_realinYear"]);
            using (var ad = new IYunEntities())
            {
                stu.y_examNum = stu.y_examNum ?? "";
                var re = new Hashtable();
                //var birthday = DateTime.MaxValue;
                //DateTime.TryParse(
                //                   stu.y_cardId.Substring(6, 4) + "-" + stu.y_cardId.Substring(10, 2) + "-" +
                //                   stu.y_cardId.Substring(12, 2), out birthday);
                //stu.y_birthday = birthday;

                if (stu.y_birthday.Equals(DateTime.MinValue))
                {
                    re["msg"] = "出生年月错误，请仔细核对后提交";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (ad.VW_StuInfo.Any(u => u.y_cardId == stu.y_cardId))
                {
                    re["msg"] = "该身份证号已经存在";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    stu.y_subSchoolId = subSchoolid;
                }
                stu.y_isMoneyOk = (int)YesOrNo.No;
                stu.y_realYear = realyear; //实际入学年份
                stu.y_studentType = (int)Object.StudentType.旁听生;
                var msg = _stuInfoDal.AddStuInfoExtended(stu, Request, ad);
                if (msg == "ok")
                {
                    re["msg"] = "添加成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        /// <summary>
        /// 学生信息编辑视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult EditAuditor(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/AuditorInfo");
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

            if (!id.HasValue)
            {
                return RedirectToAction("AuditorInfo");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                if (student == null)
                {
                    return RedirectToAction("AuditorInfo");
                }
                ViewData["student"] = student;
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        /// <summary>
        /// 学生信息编辑AJAX
        /// </summary>
        /// <param name="stu">学生信息对象</param>
        /// <returns>处理结果json</returns>
        public string EditAuditorInfo(YD_Sts_StuInfo stu)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/AuditorInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var subSchool = Request["SubSchool"];
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                var re = new Hashtable();
                if (stu.y_subSchoolId == 0)
                {
                    var stus = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == stu.id);
                    if (stus != null)
                        stu.y_subSchoolId = stus.y_subSchoolId;
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    stu.y_subSchoolId = subSchoolid;
                }
                const int isauditor = (int)Object.StudentType.旁听生;
                stu.y_realYear = stu.y_realYear;
                stu.y_studentType = isauditor;
                stu.y_examNum = stu.y_examNum ?? "";
                if (stu.y_birthday.Equals(DateTime.MinValue))
                {
                    re["msg"] = "出生年月错误，请仔细核对后提交";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                var majorli = Request["MajorLibrary"];
                int majorliid = 0;
                string ms = "";
                if (majorli == "" && stu.y_majorId == 0)
                {
                    var majorlib = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == stu.id);
                    if (majorlib != null)
                    {
                        var major = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == majorlib.y_majorId);
                        if (major != null)
                            majorliid = major.y_majorLibId;
                    }
                }
                else
                {
                    majorliid = Convert.ToInt32(majorli);
                }
                if (string.IsNullOrWhiteSpace(stu.y_registerState))
                {
                    var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

                    var majorId = GetMajorIds(majorliid, Convert.ToInt32(Request.Params["EduType"]),
                        Convert.ToInt32(Request.Params["StuType"]));
                    if (majorId == 0)
                    {
                        ms = "学生修改失败：该层次，学习形式的专业不存在";
                        return ms;
                    }
                    stu.y_majorId = majorId;
                }
                if (stu.y_nationId == 0)
                {
                    stu.y_nationId = null;
                }
                if (stu.y_politicsId == 0)
                {
                    stu.y_politicsId = null;
                }
                yunEntities.Entry(stu).State = EntityState.Modified;

                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    re["msg"] = "修改成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = ms;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);

            }
        }

        /// <summary>
        /// 旁听生数据下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadAuditor()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/AuditorInfo");
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
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                const int isauditor = (int)Object.StudentType.旁听生;
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_studentType == isauditor);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard) ||
                                u.y_stuNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                var lists = new List<StuList>();
                list.ToList().ForEach(u =>
                {
                    lists.Add(new StuList(u.y_birthday)
                    {
                        y_name = u.y_name,
                        y_sex = u.y_sex == 0 ? "男" : "女",
                        y_stuNum = u.y_stuNum,
                        y_examNum = u.y_examNum,
                        majorLibraryName = u.majorLibraryName,
                        majorLibraryCode = u.majorLibraryCode,
                        eduTypeName = u.eduTypeName,
                        stuTypeName = u.stuTypeName,
                        schoolName = u.schoolName,
                        schoolCode = u.schoolCode,
                        y_cardId = u.y_cardId,
                        y_tel = u.y_tel,
                        nationName = u.nationName,
                        politicsName = u.politicsName,
                        stuStateName = u.stuStateName,
                        y_address = u.y_address,
                        y_inYear = u.y_inYear
                    });
                });
                var model = FileHelper.ToDataTable(lists);
                var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
                if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                {
                    var lists2 = new List<JXLGStuSub>();
                    list.ToList().ForEach(u =>
                    {
                        lists2.Add(new JXLGStuSub(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.majorLibraryCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_nameabbreviation = u.y_nameabbreviation,
                            y_formername = u.y_formername,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                    model = FileHelper.ToDataTable(lists2);
                }
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/旁听生表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorLibraryName", "专业名"},
                        {"majorLibraryCode", "专业代码"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"schoolCode", "函授站代码"},
                        {"y_cardId", "身份证"},
                        {"y_birthday", "出生日期"},
                        {"nationName", "民族"},
                        {"politicsName", "政治面貌"},
                        {"stuStateName", "学籍状态"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_inYear", "入学年份"}
                    };
                    if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "姓名"},
                            {"y_nameabbreviation", "函授站简称"},
                            {"y_formername", "函授站曾用名"},
                            {"y_sex", "性别"},
                            {"y_stuNum", "学号"},
                            {"y_examNum", "考生号"},
                            {"majorLibraryName", "专业名"},
                            {"majorLibraryCode", "专业代码"},
                            {"eduTypeName", "层次"},
                            {"stuTypeName", "学习形式"},
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_cardId", "身份证"},
                            {"y_birthday", "出生日期"},
                            {"nationName", "民族"},
                            {"politicsName", "政治面貌"},
                            {"stuStateName", "学籍状态"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_inYear", "入学年份"}
                        };
                    }
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

        #endregion

        #region 学籍信息视图

        /// <summary>
        /// 学籍信息视图控制
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StudentInfo(int id = 1)
        {
            #region 权限验证        

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stuState = Request["StuState"]; //学籍状态
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"]; //层次
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                var graduateYear = Request["graduateYear"];
                var SubSchoolName = Request["SubSchoolName"];
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];

                ViewBag.schoolname = schoolname;
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_StuState)
                    .OrderByDescending(u => u.y_inYear).Where(u => u.y_isdel == isnotdel && u.y_stuNum != "" && u.y_studentType != 2);
                //不显示未注册和注册待审核学生
                var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "未注册");
                var statecheck = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "注册待审核");
                if (state != null && schoolname != "JXLG")
                {
                    list = list.Where(u => u.y_stuStateId != state.id);
                }
                //if (statecheck != null && schoolname != "JXLG")
                //{
                //    list = list.Where(u => u.y_stuStateId != statecheck.id);
                //}
                var ste = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "在读");
                if (ste != null)
                {
                    ViewBag.ste = ste.id;
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));

                    ViewBag.subschoolid = subSchoolIds.FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard || u.y_cardId == namenumcard || u.y_examNum == namenumcard);

                    ViewBag.namenumcard = namenumcard;
                }

                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);

                    ViewBag.inyear = enrollYearint;
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    ViewBag.subschoolid = subSchoolint;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    ViewBag.majorliid = majorLibraryint;
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                    ViewBag.stuStateint = stuStateint;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    ViewBag.edutype = eduTypeint;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(graduateYear) && !graduateYear.Equals("0"))
                {
                    list = list.Where(u => u.y_graduateNumber.Substring(6, 4) == graduateYear);
                }
                if (!string.IsNullOrWhiteSpace(SubSchoolName) && !SubSchoolName.Equals("0"))
                {
                    list = list.Where(u => u.YD_Sys_SubSchool.y_name == SubSchoolName);
                }
                var isJige = Request["isJige"];
                if (!string.IsNullOrWhiteSpace(isJige) && !isJige.Equals("0"))
                {
                    if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
                    {
                        var stulist = list.Select(u => u.id);

                        //var scorelist = yunEntities.YD_Edu_Score.Where(u=>stulist.Contains(u.y_stuId))
                        //    .GroupBy(score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId })
                        //    .Select(score => score.OrderByDescending(u => u.id).FirstOrDefault());

                        //var listss =
                        //     scorelist
                        //        .GroupBy(u => u.y_stuId)
                        //         .Where(u => u.All(k => k != null && k.y_totalScore >= 60))
                        //         .Select(u => u.Key).ToList();


                        var sql = "select y_stuId from YD_Edu_Score where id in( select MAX(id) from YD_Edu_Score where y_totalScore<60 group by y_stuId,y_term,y_courseId)";

                        var stuidlist = yunEntities.Database.SqlQuery<int>(sql).ToList();
                        var stul = list.ToList();

                        if (isJige == "1") //及格
                        {
                            list = stul.Where(u => !stuidlist.Contains(u.id)).AsQueryable();
                        }
                        else
                        {
                            list = stul.Where(u => stuidlist.Contains(u.id)).AsQueryable();
                        }
                    }
                }
                ViewBag.admin = YdAdminRoleId;

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                {
                    return PartialView("StudentList", model);
                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校所有在校生注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 全校所有在校生已注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var inyear = Request["EnrollYearOld"];
                var term = Request["term"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                ViewBag.majorliid = 0;
                ViewBag.edutypeid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.year = 0;
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(inyear) && !inyear.Equals("0"))
                {
                    var inyearint = Convert.ToInt32(inyear);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyearint);
                    ViewBag.year = inyearint;
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if (string.IsNullOrWhiteSpace(stuType) && (string.IsNullOrWhiteSpace(eduType))
                    && (string.IsNullOrWhiteSpace(majorLibrary)))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校所有在校生已注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStudentRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var inyear = Request["EnrollYearOld"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var term = Request["term"];

                const int isnotdel = (int)YesOrNo.No;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null && u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (!string.IsNullOrWhiteSpace(inyear) && !inyear.Equals("0"))
                {
                    var inyearint = Convert.ToInt32(inyear);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyearint);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
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
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name

                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/全校所有在校生已注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/全校所有在校生已注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "所属函授站"}
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
        /// 全校所有在校生未注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult NoRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var year = Request["EnrollYearOld"];
                var term = Request["term"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.year = 0;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                    ViewBag.year = inyear;
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if ((string.IsNullOrWhiteSpace(stuType)) && (string.IsNullOrWhiteSpace(eduType))
                    && (string.IsNullOrWhiteSpace(majorLibrary)))
                {
                    model = null;
                    ViewBag.count = 0;
                }
                return View(model);
            }
        }

        /// <summary>
        /// 全校所有在校生未注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadNoRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var year = Request["EnrollYearOld"];
                var term = Request["term"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
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
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address,
                                    schoolName =
                                        u.YD_Sts_StuInfo.YD_Sys_SubSchool == null
                                            ? ""
                                            : u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_name

                                }).ToList());


                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/全校所有在校生未注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/全校所有在校生未注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"},
                        {"schoolName", "所属函授站"}

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
        /// 函授站所有在校生已注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubschoolRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var year = Request["EnrollYearOld"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.subschoolid = 0;
                ViewBag.year = 0;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                    ViewBag.year = inyear;
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
                    ViewBag.term = feeyear;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null)
                    {
                        ViewBag.subschoolid = subschool.id;
                        ViewBag.subschool = subschool.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if ((string.IsNullOrWhiteSpace(stuType)) && (string.IsNullOrWhiteSpace(eduType)
                                                             && (string.IsNullOrWhiteSpace(majorLibrary)
                                                                 && string.IsNullOrWhiteSpace(subSchool))))
                {
                    model = null;
                    ViewBag.count = 0;

                }
                return View(model);
            }
        }

        /// <summary>
        /// 函授站所有在校生已注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadSubschoolRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var year = Request["EnrollYearOld"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.Yes &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null && u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
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
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address

                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站所有在校生已注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站所有在校生已注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"}
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
        /// 函授站所有在校生未注册名单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubschoolNoRegisterStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var year = Request["EnrollYearOld"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.year = 0;
                ViewBag.edutypeid = 0;
                ViewBag.majorliyid = 0;
                ViewBag.stutypeid = 0;
                ViewBag.subschoolid = 0;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear != 1
                            && u.YD_Sts_StuInfo.y_subSchoolId != null && u.YD_Sts_StuInfo.y_inYear != xinshen);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                    ViewBag.year = inyear;
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
                    ViewBag.term = feeyear;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    var majorliid = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == majorLibraryint);
                    if (majorliid != null)
                    {
                        ViewBag.majorliyid = majorliid.id;
                        ViewBag.majorli = majorliid.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    var edutype = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == eduTypeint);
                    if (edutype != null)
                    {
                        ViewBag.edutypeid = edutype.id;
                        ViewBag.edutype = edutype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_stuTypeId == stuTypeint);
                    var stutypetype = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == stuTypeint);
                    if (stutypetype != null)
                    {
                        ViewBag.stutypeid = stutypetype.id;
                        ViewBag.stutypetype = stutypetype.y_name;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    if (subschool != null)
                    {
                        ViewBag.subschoolid = subschool.id;
                        ViewBag.subschool = subschool.y_name;
                    }
                }
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToList();
                ViewBag.count = model.Count;
                if ((string.IsNullOrWhiteSpace(stuType)) && (string.IsNullOrWhiteSpace(eduType))
                    && (string.IsNullOrWhiteSpace(majorLibrary)))
                {
                    model = null;
                    ViewBag.count = 0;
                }
                return View(model);
            }
        }

        /// <summary>
        /// 函授站所有在校生未注册名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloaSubschoolNoRegister()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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
                var year = Request["EnrollYearOld"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId)
                    .
                    Where(
                        u =>
                            u.YD_Sts_StuInfo.y_isdel == isnotdel && u.y_isCheckFee == (int)YesOrNo.No &&
                            u.y_feeYear != 1 &&
                            u.YD_Sts_StuInfo.y_subSchoolId != null && u.YD_Sts_StuInfo.y_inYear != xinshen);
                {
                    var inyear = Convert.ToInt32(year);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == inyear);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var feeyear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feeyear);
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
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == subSchoolint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var count = list.ToList().Count;
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.YD_Sts_StuInfo.y_inYear,
                                    eduTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorLibraryName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Sts_StuInfo.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    count = count,
                                    y_examNum = u.YD_Sts_StuInfo.y_examNum,
                                    y_stuNum = u.YD_Sts_StuInfo.y_stuNum,
                                    y_name = u.YD_Sts_StuInfo.y_name,
                                    y_sex = u.YD_Sts_StuInfo.y_sex == 0 ? "男" : "女",
                                    y_cardId = u.YD_Sts_StuInfo.y_cardId,
                                    y_tel = u.YD_Sts_StuInfo.y_tel,
                                    y_address = u.YD_Sts_StuInfo.y_address
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站所有在校生未注册名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站所有在校生未注册名单表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "年级"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"count", "注册人数"},
                        {"y_examNum", "考生号"},
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证"},
                        {"y_tel", "联系电话"},
                        {"y_address", "联系地址"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("alert('错误');window.location.href='" + reurl + "';");
                }
            }
        }

        /// <summary>
        /// 所有在校生注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult Registertatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/Registertatistics");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 全校所有在校生已注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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

            using (var ad = new IYunEntities())
            {
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var year = 0;
                var edutypeid = 0;
                var term = 0;
                string sql;
                var list = new List<NewStuStatistics>();
                if (year == 0)
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";

                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_eduTypeId = " + edutypeid + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and  y_eduTypeId = " + edutypeid + " and y_isCheckFee=" +
                    //          (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewBag.edutypeid = edutypeid;
                ViewBag.year = year;
                ViewBag.term = term;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult RegisterStuStatisticsPost(int id, int year, int term)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var edutypeid = id;
                var EnrollYear = year;
                var feeyear = term; //学年
                string sql;
                var list = new List<NewStuStatistics>();
                if (EnrollYear == 0)
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + feeyear +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_eduTypeId = " + edutypeid + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + feeyear +
                    //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + EnrollYear + " and  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + feeyear +
                    //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + EnrollYear + " and y_eduTypeId = " + edutypeid + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + feeyear +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == EnrollYear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewBag.edutypeid = edutypeid;
                ViewBag.year = year;
                ViewBag.term = term;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        ///  全校 所有在校生已注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownRegisterStuStatistics(int id, int year, int term)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            string sql;

            using (var ad = new IYunEntities())
            {
                var list = new List<NewStuStatistics>();
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                if (year == 0)
                {
                    //if (id == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                    //       where  y_eduTypeId = " + id + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);
                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改

                }
                else
                {
                    //if (id == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_eduTypeId = " + id + " and y_isCheckFee=" + (int)YesOrNo.Yes
                    //      + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist = ad.YD_Fee_StuFeeTb.Where(
                        u => u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.Yes &&
                             u.y_feeYear == term);

                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();


                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var fileName1 = "/全校所有在校生已注册统计表" + ".xls"; //todo:改变
                var fileName = dirPath + fileName1; //todo:改变

                //var filename1 = "File/Dowon/全校所有在校生已注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + fileName1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }
        }

        /// <summary>
        /// 全校 所有在校生未注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult NoRegisterStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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

            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var year = 0;
                var edutypeid = 0;
                var term = 0;
                string sql;
                var list = new List<NewStuStatistics>();
                if (year == 0)
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_eduTypeId = " + edutypeid + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_eduTypeId = " + edutypeid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改

                }

                ViewBag.edutypeid = edutypeid;
                ViewBag.year = year;
                ViewBag.term = term;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NoRegisterStuStatisticsPost(int id, int year, int term)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var enrollyear = year;
                var edutypeid = id;
                string sql;
                var list = new List<NewStuStatistics>();
                if (enrollyear == 0)
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_eduTypeId = " + edutypeid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}

                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (edutypeid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + enrollyear + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + enrollyear + " and y_eduTypeId = " + edutypeid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (edutypeid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewBag.edutypeid = edutypeid;
                ViewBag.year = enrollyear;
                ViewBag.term = term;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        ///  全校所有在校生未注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownNoRegisterStuStatistics(int id, int year, int term)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            string sql;
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var list = new List<NewStuStatistics>();
                if (year == 0)
                {
                    //if (id == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_eduTypeId = " + id + " and y_isCheckFee=" +
                    //          (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (id == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_eduTypeId = " + id + " and y_isCheckFee=" +
                    //          (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);

                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == id);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var fileName1 = "/全校所有在校生未注册统计表" + ".xls"; //todo:改变
                var fileName = dirPath + fileName1; //todo:改变

                //var filename1 = "File/Dowon/全校所有在校生未注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + fileName1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }
        }

        /// <summary>
        /// 函授站在校生已注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SubschoolStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
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

            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var enrollyear = 0;
                var schoolid = 0;
                var term = 0;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                string sql;
                var list = new List<NewStuStatistics>();
                if (enrollyear == 0)
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + enrollyear + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + enrollyear + " and y_subSchoolId = " + schoolid + " and y_isCheckFee=" +
                    //          (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == enrollyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);

                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewData["schoolid"] = schoolid;
                ViewBag.year = enrollyear;
                ViewBag.term = term;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult SubschoolStuStatisticsPost(int id, int year, int term)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var enllyear = year;
                var schoolid = id;
                string sql;
                var list = new List<NewStuStatistics>();
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                if (enllyear == 0)
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + enllyear + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + enllyear + " and y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.Yes
                    //   + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == enllyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);

                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }

                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }

                ViewData["schoolid"] = schoolid;
                ViewBag.year = enllyear;
                ViewBag.term = term;
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        /// 函授站在校生已注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownSubschoolStuStatistics(int id, int year, int term)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            string sql;
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var list = new List<NewStuStatistics>();
                if (YdAdminRoleId == 4)
                {
                    id = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                if (year == 0)
                {
                    //if (id == 0)
                    //{    
                    //        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //              " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_subSchoolId = " + id + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);
                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (id == 0)
                    //{
                    //        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.Yes + " and y_feeYear=" + term +
                    //            " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";

                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_subSchoolId = " + id + " and y_isCheckFee=" + (int)YesOrNo.Yes
                    //      + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.Yes &&
                                u.y_feeYear == term);
                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改

                }
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });
                var model = FileHelper.ToDataTable(lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var fileName1 = "/函授站在校生已注册统计表" + ".xls"; //todo:改变
                var fileName = dirPath + fileName1;
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + fileName1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }
        }

        /// <summary>
        /// 函授站在校生未注册统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SubschoolNoStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewlyRegister");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return RedirectToAction("Index", "AdminBase");
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                var year = 0;
                var schoolid = 0;
                var term = 0;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                string sql;
                var list = new List<NewStuStatistics>();
                if (year == 0)
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + year + " and y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewData["schoolid"] = schoolid;
                ViewBag.year = year;
                ViewBag.term = term;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult SubschoolNoStuStatisticsPost(int id, int year, int term)
        {
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var enrollyear = year;
                var schoolid = id;
                if (!IsLogin())
                {
                    return Redirect("/AdminBase/Index");
                }
                string sql;
                var list = new List<NewStuStatistics>();
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                if (enrollyear == 0)
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where  y_subSchoolId = " + schoolid + " and y_isCheckFee=" +
                    //          (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    // }
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (schoolid == 0)
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + enrollyear + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //  , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //   where y_inyear = " + enrollyear + " and y_subSchoolId = " + schoolid + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == enrollyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (schoolid != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == schoolid);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                ViewData["schoolid"] = schoolid;
                ViewBag.year = enrollyear;
                ViewBag.term = term;
                //var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                //统计总人数
                int count = 0;
                list.ForEach(u =>
                {
                    count += u.counts;
                });
                ViewBag.count = count;
                return PartialView(list);
            }
        }

        /// <summary>
        /// 函授站在校生未注册统计下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DownSubschoolNoStuStatistics(int id, int year, int term)
        {
            if (!IsLogin())
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return @"alert('错误');window.location.href='" + reurl + "';";
            }
            string sql;
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                var list = new List<NewStuStatistics>();
                if (YdAdminRoleId == 4)
                {
                    id = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                if (year == 0)
                {
                    //if (id == 0)
                    //{

                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_isCheckFee=" + (int) YesOrNo.No + " and y_inyear != " + xinshenyear +
                    //          " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where  y_subSchoolId = " + id + " and y_isCheckFee=" +
                    //          (int) YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear != xinshenyear && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                else
                {
                    //if (id == 0)
                    //{
                    //        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //            " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    //else
                    //{
                    //    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                    //      , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[YD_Fee_StuFeeTb]
                    //       where y_inyear = " + year + " and y_subSchoolId = " + id + " and y_isCheckFee=" + (int)YesOrNo.No + " and y_inyear != " + xinshenyear + " and y_feeYear=" + term +
                    //        " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    //}
                    var tblist =
                        ad.YD_Fee_StuFeeTb.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_inYear == year && u.y_isCheckFee == (int)YesOrNo.No &&
                                u.y_feeYear == term);
                    if (id != 0)
                    {
                        tblist = tblist.Where(u => u.YD_Sts_StuInfo.y_subSchoolId == id);
                    }
                    list =
                        tblist.GroupBy(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                            .Select(
                                u =>
                                    new NewStuStatistics
                                    {
                                        cc = u.Key.YD_Edu_EduType.y_name,
                                        xs = u.Key.YD_Edu_StuType.y_name,
                                        zy = u.Key.YD_Edu_MajorLibrary.y_name,
                                        counts = u.Count()
                                    }).ToList(); //todo:测试修改
                }
                // var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });
                var model = FileHelper.ToDataTable(lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var fileName1 = "/函授站在校生未注册统计表" + ".xls"; //todo:改变
                var fileName = dirPath + fileName1; //todo:改变

                //var filename1 = "File/Dowon/函授站在校生未注册统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + fileName1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }
        }

        /// <summary>
        /// 学生信息缴费情况
        /// </summary>
        /// <returns></returns>
        public ActionResult StuFeeDes(int? id)
        {
            #region 权限验证

            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--start action!");

            var power = SafePowerPage("/Student/StudentInfo");


            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--power End!");

            if (!IsLogin())
            {

                LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--IsLogin End!");

                return Redirect("/AdminBase/Index");
            }


            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--IsLogin End!");

            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }


            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "--power End!");

            #endregion

            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfo");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                //查询出该学生的相关缴费记录
                var list =
                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                        .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .OrderByDescending(u => u.id)
                        .Where(u => u.y_stuId == id)
                        .ToList();
                ViewBag.admin = YdAdminRoleId;
                if (!list.Any())
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewData["stuFee"] = list;
                return View();
            }
        }

        /// <summary>
        /// 函授站管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubSchoolStuInfo(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");

            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }

            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目


                var name = Request["name"];
                var examNum = Request["examNum"];
                var card = Request["card"];
                var subSchool = Request["subSchool"];
                var namenumcard = Request["namenumcard"];
                var inYear = Request["EnrollYear"];
                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_SubSchoolStuInfo> list =
                    yunEntities.VW_SubSchoolStuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != null && u.y_hide == 1);
                var xinshen = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"].ToString());
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId));
                }
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    list = list.Where(u => u.y_year == xinshen + 1);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_examNum == namenumcard ||
                                u.y_cardId.Contains(namenumcard));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(examNum))
                {
                    list = list.Where(u => u.y_examNum.Contains(examNum));
                }
                if (!string.IsNullOrWhiteSpace(card))
                {
                    list = list.Where(u => u.y_cardId.Contains(card));
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var inyear = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_year == inyear);
                }
                ViewBag.adminrole = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                    return PartialView("SubSchoolStuList", model);
                return View(model);
            }
        }

        #endregion

        /// <summary>
        /// 验证学号重复
        /// </summary>
        /// <returns></returns>
        public ActionResult
            StuNumCheckUp()
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<VW_StuInfo>(
                        " SELECT id,y_name,formc.y_stuNum,y_examNum,y_sex,y_inYear,y_cardId,y_majorId,y_subSchoolId,y_stuStateId,y_politicsId,y_nationId,y_img,y_tel,y_mail,y_address,y_registerState,y_isChangePlan,y_changePlanId,y_loginName,y_password,y_stuStrange,y_isdel,schoolCode,schoolName,stuStateName,y_stuStateCode,nationName,politicsName,y_majorLibId,y_stuTypeId,y_eduTypeId,majorName,majorCode,y_stuYear,stuTypeName,y_stuTypeCode,eduTypeName,y_eduTypeCode,majorLibraryName,majorLibraryCode,subSchoolCode,y_birthday,y_isMoneyOk,y_applyOK,y_ischeck,y_degreeOK,y_graduationdata,y_graduationschool,y_registrationNum,y_postalcode,y_foreignLanguageId,y_recruitTypeId,y_professionTypeId,y_cultureExtentId,y_examFeatureId,y_foreignLanguageCode,y_foreignLanguageName,y_cultureExtentCode,y_cultureExtentName,y_professionTypeCode,y_professionTypeName,y_recruitTypeCode,y_recruitTypeName,y_ZJTD,y_examFeatureCode,y_examFeatureName,y_TZF FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_stuNum FROM dbo.VW_StuInfo GROUP BY y_stuNum) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.VW_StuInfo AS formc ON formb.y_stuNum=formc.y_stuNum ORDER BY formc.y_stuNum")
                        .ToList();
                return View();
            }
        }

        /// <summary>
        /// 验证身份证重复
        /// </summary>
        /// <returns></returns>
        public ActionResult CardIdCheckUp()
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<VW_StuInfo>(
                        " SELECT id,y_name,y_stuNum,y_examNum,y_sex,y_inYear,formc.y_cardId,y_majorId,y_subSchoolId,y_stuStateId,y_politicsId,y_nationId,y_img,y_tel,y_mail,y_address,y_registerState,y_isChangePlan,y_changePlanId,y_loginName,y_password,y_stuStrange,y_isdel,schoolCode,schoolName,stuStateName,y_stuStateCode,nationName,politicsName,y_majorLibId,y_stuTypeId,y_eduTypeId,majorName,majorCode,y_stuYear,stuTypeName,y_stuTypeCode,eduTypeName,y_eduTypeCode,majorLibraryName,majorLibraryCode,subSchoolCode,y_birthday,y_isMoneyOk,y_applyOK,y_ischeck,y_degreeOK,y_graduationdata,y_graduationschool,y_registrationNum,y_postalcode,y_foreignLanguageId,y_recruitTypeId,y_professionTypeId,y_cultureExtentId,y_examFeatureId,y_foreignLanguageCode,y_foreignLanguageName,y_cultureExtentCode,y_cultureExtentName,y_professionTypeCode,y_professionTypeName,y_recruitTypeCode,y_recruitTypeName,y_ZJTD,y_examFeatureCode,y_examFeatureName,y_TZF FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_cardId FROM dbo.VW_StuInfo GROUP BY y_cardId) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.VW_StuInfo AS formc ON formb.y_cardId=formc.y_cardId ORDER BY formc.y_cardId")
                        .ToList();
                return View();
            }
        }

        public class Course
        {
            //课程名称
            public string Name { get; set; }

            //课程类型
            public string Type { get; set; }

            //学分
            public string Credit { get; set; }

            //成绩
            public string Score { get; set; }

        }

        public class CourseList
        {
            public string Name { get; set; }
            public List<Course> Courses { get; set; }
        }

        public static string CourseScore(int id, int courseId, int term)
        {

            using (var db = new IYunEntities())
            {
                var score = db.YD_Edu_Score.Where(u => u.y_stuId == id && u.y_courseId == courseId && u.y_term == term).OrderByDescending(u => u.id).FirstOrDefault();

                if (score == null)
                {
                    return "";
                }

                return score.y_totalScore.ToString();
            }
        }
        static void SetHead(HSSFWorkbook hssfworkbook, ISheet sheet, string value)
        {
            IRow row = sheet.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(value);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            style.Alignment = HorizontalAlignment.CENTER;
            //新建一个字体样式对象
            IFont font = hssfworkbook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 24;
            //设置字体加粗样式
            font.Boldweight = (short)FontBoldWeight.BOLD;
            //使用SetFont方法将字体样式添加到单元格样式中 
            style.SetFont(font);
            //将新的样式赋给单元格
            cell.CellStyle = style;
            //合并单元格
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 15));
        }

        static void SetDesc(ISheet sheet, string[] value)
        {
            IRow row = sheet.CreateRow(1);

            //for (int i = 0; i < value.Length; i++)
            //{
            //    ICell cell = row.CreateCell(i * 2);
            //    cell.SetCellValue(value[i]);
            //    sheet.AutoSizeColumn(i * 2);
            //}
            ICell cell = row.CreateCell(0);
            cell.SetCellValue($"{value[0]}        {value[1]}        {value[2]}");
            ICell cell2 = row.CreateCell(7);
            cell2.SetCellValue(value[3]);
            ICell cell3 = row.CreateCell(10);
            cell3.SetCellValue(value[4]);
            ICell cell4 = row.CreateCell(13);
            cell4.SetCellValue(value[5]);
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 9));
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 10, 12));
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 13, 15));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <param name="sheet"></param>
        /// <param name="title">上半年或下半年</param>
        /// <param name="courses">课程集合</param>
        static void SetBody(HSSFWorkbook hssfworkbook, ISheet sheet, string title, List<Course> courses)
        {
            int rowCount = sheet.LastRowNum + 1;
            IRow row = sheet.CreateRow(rowCount);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(title);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            sheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 15));
            style.Alignment = HorizontalAlignment.CENTER;
            style.BorderTop = BorderStyle.THIN;
            style.BorderBottom = BorderStyle.THIN;
            style.BorderLeft = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            cell.CellStyle = style;
            for (int i = 1; i < 16; i++)
            {
                ICell rowSheet = row.CreateCell(i);
                rowSheet.CellStyle = style;
            }
            IRow rowTitlte = sheet.CreateRow(rowCount + 1);
            string[] titleStrs = { "课程名称", "课程类型", "学分", "考试成绩" };
            for (int i = 0; i < 16; i++)
            {
                ICell cellTitle = rowTitlte.CreateCell(i);
                cellTitle.SetCellValue(titleStrs[i % 4]);
                cellTitle.CellStyle = style;
            }
            //课程为空
            if (courses == null)
            {
                IRow rowBody = sheet.CreateRow(rowCount + 3);
            }
            else
            {
                for (int k = 0; k <= courses.Count / 4; k++)
                {
                    IRow rowBody = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int i = 0; i < 4; i++)
                    {
                        ICell cellTitle1 = rowBody.CreateCell(i * 4);
                        ICell cellTitle2 = rowBody.CreateCell(i * 4 + 1);
                        ICell cellTitle3 = rowBody.CreateCell(i * 4 + 2);
                        ICell cellTitle4 = rowBody.CreateCell(i * 4 + 3);
                        if (i + k * 4 < courses.Count)
                        {
                            cellTitle1.SetCellValue(courses[i + k * 4].Name);
                            cellTitle2.SetCellValue(courses[i + k * 4].Type);
                            cellTitle3.SetCellValue(courses[i + k * 4].Credit);
                            cellTitle4.SetCellValue(courses[i + k * 4].Score);
                        }
                        cellTitle1.CellStyle = style;
                        cellTitle2.CellStyle = style;
                        cellTitle3.CellStyle = style;
                        cellTitle4.CellStyle = style;
                    }
                }
            }
        }

        public class StudentScore
        {
            //姓名
            public string Name { get; set; }

            //学号
            public string StudentNo { get; set; }

            //学院名
            public string College { get; set; }

            //专业名1
            public string ProfessionalName { get; set; }

            //专业名2
            public string ProfessionalYear { get; set; }

            //各个学年
            public List<CourseList> CourseList { get; set; }
        }

        public string Print(StudentScore studentScore)
        {
            //导出Excel
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet();

            string[] headName =
            {
                studentScore.College,
                studentScore.ProfessionalName,
                studentScore.ProfessionalYear,
                "姓名：" + studentScore.Name,
                "学号：" +studentScore.StudentNo,
                $"打印日期：{DateTime.Now.ToString("yyyy/MM/dd")}"
            };
            SetHead(hssfworkbook, sheet, "华东交通大学学生成绩表");
            SetDesc(sheet, headName);
            foreach (var item in studentScore.CourseList)
            {
                SetBody(hssfworkbook, sheet, item.Name, item.Courses);
            }

            sheet.SetColumnWidth(0, 4000);
            sheet.SetColumnWidth(4, 4000);
            sheet.SetColumnWidth(8, 4000);
            sheet.SetColumnWidth(12, 4000);

            //保存


            var Path = Server.MapPath("~/File/Dowon/");

            if (!System.IO.Directory.Exists(Path))
                System.IO.Directory.CreateDirectory(Path);
            string fileName = studentScore.Name + studentScore.StudentNo + ".xls";
            using (FileStream file = new FileStream(Path + "\\" + fileName, FileMode.Create))
            {
                hssfworkbook.Write(file);　　//创建test.xls文件。
                file.Close();
            }
            string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon/" + fileName;
            return url;
        }


        public void Empty(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        /// <summary>
        /// 华东交大成绩单Execl导出
        /// </summary>
        /// <returns></returns>
        public JsonResult HdjdScore()
        {

            var stuids = Request["id"];

            List<string> ids = stuids.Split(',').ToList();

            if (ids == null || ids.Count==0)
            {
                Json(new { msg = "导出失败,请刷新后重试" });

            }

            string url = Server.MapPath("~/File/Dowon/");
            Empty(new DirectoryInfo(url));


            foreach (var stuid in ids)
            {
                if (!string.IsNullOrEmpty(stuid))
                {
                    var id = Convert.ToInt32(stuid);

                    using (var db = new IYunEntities())
                    {
                        var stu = db.VW_StuInfo.FirstOrDefault(u => u.id == id);
                        if (stu == null)
                        {
                            Json(new { msg = "导出失败,请刷新后重试" });
                        }


                        var lists = db.YD_TeaPlan_ClassCourseDes.Where(u => u.YD_TeaPlan_Class.y_majorId == stu.y_majorId && u.YD_TeaPlan_Class.y_year == stu.y_inYear && u.YD_TeaPlan_Class.y_subSchoolId == stu.y_subSchoolId).ToList();



                        var list = lists.Select(u => new
                        {
                            Name = u.YD_Edu_Course.y_name,
                            Type = CourseType2.CourseTypeName(u.y_courseType),
                            Credit = u.y_score,
                            Score = CourseScore(stu.id, u.YD_Edu_Course.id, u.y_team),
                            team = u.y_team
                        }).ToList();


                        if (list.Count() <= 0)
                        {
                            Json(new { msg = "此学生没有教学计划" });

                        }

                        var maxteam = list.Max(u => u.team);


                        var CoList = new List<CourseList>();

                        for (var i = 1; i <= maxteam; i++)
                        {
                            List<Course> courses = new List<Course>();

                            var li = list.Where(u => u.team == i).ToList();

                            if (li.Count() > 0)
                            {
                                var name = "";//某年上半年或者下半年

                                for (var y = 0; y < li.Count(); y++)
                                {
                                    Course course6 = new Course()
                                    {
                                        Name = li[y].Name,
                                        Type = li[y].Type,
                                        Credit = string.Format("{0:0.#}", li[y].Credit),
                                        Score = li[y].Score
                                    };
                                    if (course6.Name.Contains("毕业论文") || course6.Name.Contains("毕业设计"))
                                    {
                                        int scoreNum;
                                        if (!string.IsNullOrEmpty(course6.Score))
                                        {
                                            scoreNum = Convert.ToInt32(course6.Score);
                                        }
                                        else
                                        {
                                            scoreNum = 0;
                                        }
                                        string scoreString = "";
                                        if (scoreNum >= 90)
                                        {
                                            scoreString = "优";
                                        }
                                        else if (scoreNum >= 80)
                                        {
                                            scoreString = "良好";
                                        }
                                        else if (scoreNum >= 70)
                                        {
                                            scoreString = "中等";
                                        }
                                        else if (scoreNum >= 60)
                                        {
                                            scoreString = "及格";
                                        }
                                        else
                                        {
                                            scoreString = "不及格";
                                        }
                                        course6.Score = scoreString;
                                    }
                                    courses.Add(course6);


                                }


                                if (i == 1) { name = $"{stu.y_inYear}年上半年"; }
                                if (i == 2) { name = $"{stu.y_inYear}年下半年"; }
                                if (i == 3) { name = $"{stu.y_inYear + 1}年上半年"; }
                                if (i == 4) { name = $"{stu.y_inYear + 1}年下半年"; }
                                if (i == 5) { name = $"{stu.y_inYear + 2}年上半年"; }
                                if (i == 6) { name = $"{stu.y_inYear + 2}年下半年"; }
                                if (i == 7) { name = $"{stu.y_inYear + 3}年上半年"; }
                                if (i == 8) { name = $"{stu.y_inYear + 3}年下半年"; }
                                if (i == 9) { name = $"{stu.y_inYear + 4}年上半年"; }
                                if (i == 10) { name = $"{stu.y_inYear + 4}年下半年"; }

                                CoList.Add(new CourseList { Name = name, Courses = courses });
                            }
                        }
                        StudentScore studentScore = new StudentScore();
                        if (stu.y_subSchoolId != null && stu.y_subSchoolId != 0)
                        {
                            var subint = Convert.ToInt32(stu.y_subSchoolId);
                            var subname = db.YD_Sys_SubSchool.FirstOrDefault(x => x.id == subint).y_name;
                            studentScore.College = $"{subname}";
                        }
                        else
                        {
                            studentScore.College = "";
                        }
                        studentScore.ProfessionalName = $"{stu.majorName} 专业";
                        studentScore.ProfessionalYear = $"{stu.y_inYear}-2";
                        studentScore.Name = stu.y_name;
                        studentScore.StudentNo = $"{stu.y_stuNum}";


                        studentScore.CourseList = CoList;

                        Print(studentScore);

                    }
                }
            }
            
            string source = Server.MapPath("~/File/Dowon/");
            using (var archive = new ZipFile(Encoding.Default))
            {
                DirectoryInfo dic = new DirectoryInfo(source);
                    foreach (var file in dic.GetFiles())
                    {
                        archive.AddEntry(file.Name, file.OpenRead());
                    }
                string fileName = DateTime.Now.ToString("yyyyMMdd");
                string zipFile = Server.MapPath("~/File/Dowon/" + fileName + ".zip");
                FileStream fs_scratchPath = new FileStream(zipFile, FileMode.OpenOrCreate, FileAccess.Write);
                archive.Save(fs_scratchPath);
                fs_scratchPath.Close();
                string zipurl = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon/" + fileName + ".zip";
                return Json(new { msg = "ok", url = zipurl });
            }
            
        }

        /// <summary>
        /// 学生数据下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
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
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var stuState = Request["StuState"];
                var namenumcard = Request["namenumcard"];
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                var graduateYear = Request["graduateYear"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_stuNum != "" && !string.IsNullOrEmpty(u.y_examNum));
                //不显示未注册和注册待审核学生
                var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "未注册");
                var statecheck = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "注册待审核");
                if (state != null && schoolname != "JXLG")
                {
                    list = list.Where(u => u.y_stuStateId != state.id);
                }
                if (statecheck != null && schoolname != "JXLG")
                {
                    list = list.Where(u => u.y_stuStateId != statecheck.id);
                }
                var ste = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "在读");
                if (ste != null)
                {
                    ViewBag.ste = ste.id;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var inyearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == inyearint);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard) ||
                                 u.y_stuNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(graduateYear) && !graduateYear.Equals("0"))
                {
                    list = list.Where(u => u.y_graduateNumber.Substring(6, 4) == graduateYear);
                }
                var isJige = Request["isJige"];
                if (!string.IsNullOrWhiteSpace(isJige) && !isJige.Equals("0"))
                {
                    if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
                    {

                        var sql = "select y_stuId from YD_Edu_Score where id in( select MAX(id) from YD_Edu_Score where y_totalScore<60 group by y_stuId,y_term,y_courseId)";

                        var stuidlist = yunEntities.Database.SqlQuery<int>(sql).ToList();

                        var stul = list.ToList();

                        if (isJige == "1") //及格
                        {
                            list = stul.Where(u => !stuidlist.Contains(u.id)).AsQueryable();
                        }
                        else
                        {
                            list = stul.Where(u => stuidlist.Contains(u.id)).AsQueryable();
                        }

                    }
                }
                var lists = new List<StuList>();
                var lists2 = new List<JXLGStuSub>();
                var lists3 = new List<DHLGStuSub>();
                var model = FileHelper.ToDataTable(lists);
                var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
                if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                {
                    list.OrderByDescending(u => u.y_subSchoolId).ThenByDescending(e => e.y_majorId).ThenByDescending(e => e.y_stuNum).ThenByDescending(e => e.y_graduateNumber).ToList().ForEach(u =>
                    {
                        lists2.Add(new JXLGStuSub(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_nameabbreviation = u.y_nameabbreviation,
                            y_formername = u.y_formername,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.majorLibraryCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                    model = FileHelper.ToDataTable(lists2);
                }
                else if (schoolName == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    list.OrderByDescending(u => u.y_subSchoolId).ThenByDescending(e => e.y_majorId).ThenByDescending(e => e.y_stuNum).ThenByDescending(e => e.y_graduateNumber).ToList().ForEach(u =>
                    {
                        lists.Add(new StuList(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = yunEntities.YD_Edu_Major.FirstOrDefault(x => x.id == u.y_majorId).y_StandardCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear,
                            y_graduateNumber = u.y_graduateNumber
                        });
                    });
                    model = FileHelper.ToDataTable(lists);
                }
                else if(schoolName == ComEnum.SchoolName.DHLGDX.ToString())
                {
                    list.ToList().ForEach(u =>
                    {
                        lists3.Add(new DHLGStuSub(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.y_StandardCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear,
                            y_ClassNum = u.y_classNum,
                            y_realYear = u.y_realYear.Value
                        });
                    });
                    model = FileHelper.ToDataTable(lists3);
                }
                else
                {
                    list.ToList().ForEach(u =>
                    {
                        lists.Add(new StuList(u.y_birthday)
                        {
                            y_name = u.y_name,
                            y_sex = u.y_sex == 0 ? "男" : "女",
                            y_stuNum = u.y_stuNum,
                            y_examNum = u.y_examNum,
                            majorLibraryName = u.majorLibraryName,
                            majorLibraryCode = u.majorLibraryCode,
                            eduTypeName = u.eduTypeName,
                            stuTypeName = u.stuTypeName,
                            schoolName = u.schoolName,
                            schoolCode = u.schoolCode,
                            y_cardId = u.y_cardId,
                            y_tel = u.y_tel,
                            nationName = u.nationName,
                            politicsName = u.politicsName,
                            stuStateName = u.stuStateName,
                            y_address = u.y_address,
                            y_inYear = u.y_inYear
                        });
                    });
                    model = FileHelper.ToDataTable(lists);
                }
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学生信息表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/学生信息表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable();
                    if (schoolName == ComEnum.SchoolName.JXLG.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "姓名"},
                            {"y_nameabbreviation", "函授站简称"},
                            {"y_formername", "函授站曾用名"},
                            {"y_sex", "性别"},
                            {"y_stuNum", "学号"},
                            {"y_examNum", "考生号"},
                            {"majorLibraryName", "专业名"},
                            {"majorLibraryCode", "专业代码"},
                            {"eduTypeName", "层次"},
                            {"stuTypeName", "学习形式"},
                            {"schoolName", "函授站"},
                            {"schoolCode", "函授站代码"},
                            {"y_cardId", "身份证"},
                            {"y_birthday", "出生日期"},
                            {"nationName", "民族"},
                            {"politicsName", "政治面貌"},
                            {"stuStateName", "学籍状态"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_inYear", "入学年份"},
                            {"y_graduateNumber", "毕业证号" }
                        };
                    }
                    else if(schoolName == ComEnum.SchoolName.DHLGDX.ToString())
                    {
                        ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorLibraryName", "专业名"},
                        {"majorLibraryCode", "专业代码"},
                        {"y_realYear", "毕业年份" },
                        {"y_ClassNum", "班级号" },
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"schoolCode", "函授站代码"},
                        {"y_cardId", "身份证"},
                        {"y_birthday", "出生日期"},
                        {"nationName", "民族"},
                        {"politicsName", "政治面貌"},
                        {"stuStateName", "学籍状态"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_inYear", "入学年份"},
                        {"y_graduateNumber", "毕业证号"},
                    };
                    }else
                    {
                        ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorLibraryName", "专业名"},
                        {"majorLibraryCode", "专业代码"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"schoolCode", "函授站代码"},
                        {"y_cardId", "身份证"},
                        {"y_birthday", "出生日期"},
                        {"nationName", "民族"},
                        {"politicsName", "政治面貌"},
                        {"stuStateName", "学籍状态"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"y_inYear", "入学年份"},
                        {"y_graduateNumber", "毕业证号" }
                    };
                    }
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

        /// <summary>
        /// 入学通知书打印
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult PrintNotice()
        {
            using (var yunEntities = new IYunEntities())
            {
                var name = Request["name"];
                var stuState = Request["StuState"];
                var card = Request["card"];
                var birthday = Request["birthday"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(card))
                {
                    list = list.Where(u => u.y_cardId.Contains(card));
                }
                if (!string.IsNullOrWhiteSpace(birthday))
                {
                    var birthdaydate = Convert.ToDateTime(birthday);
                    list = list.Where(u => u.y_birthday == birthdaydate);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                ViewData["list"] = list.ToList();

                return View();
            }
        }

        /// <summary>
        /// 函授站下的学生数据下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadSubSchoolStu()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");
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
                var namenumcard = Request["namenumcard"];
                var inYear = Request["EnrollYear"];

                const int isnotdel = (int)YesOrNo.No;
                IQueryable<VW_SubSchoolStuInfo> list =
                    yunEntities.VW_SubSchoolStuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_cardId == namenumcard ||
                                u.y_examNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var inyear = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_year == inyear);
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_examNum = u.y_examNum,
                                    schoolName = u.schoolName,
                                    y_cardId = u.y_cardId,
                                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/函授站学生信息表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/函授站学生信息表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_examNum", "考生号"},
                        {"schoolName", "函授站"},
                        {"y_cardId", "身份证"}
                    };
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

        #region 学生信息添加视图

        /// <summary>
        /// 学生信息添加视图控制
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult AddStudent()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
            }
            ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
            return View();
        }

        #endregion

        #region 学生信息保存AJAX

        /// <summary>
        /// 学生信息保存AJAX
        /// </summary>
        /// <param name="stu">学生信息扩展对象</param>
        /// <returns>处理结果json</returns>
        public string SaveStudentInfo(YD_Sts_StuInfo stu)
        {
            #region 权限验证SaveStudentInfo

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var subSchool = Request["SubSchool"];
            using (var ad = new IYunEntities())
            {
                var re = new Hashtable();
                //var birthday = DateTime.MaxValue;
                //DateTime.TryParse(
                //                   stu.y_cardId.Substring(6, 4) + "-" + stu.y_cardId.Substring(10, 2) + "-" +
                //                   stu.y_cardId.Substring(12, 2), out birthday);
                //stu.y_birthday = birthday;
                if (stu.y_birthday.Equals(DateTime.MinValue))
                {
                    re["msg"] = "出生年月错误，请仔细核对后提交";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (ad.VW_StuInfo.Any(u => u.y_cardId == stu.y_cardId && u.y_isdel == 1))
                {
                    re["msg"] = "已存在身份证号";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    stu.y_subSchoolId = subSchoolid;
                }
                stu.y_isMoneyOk = (int)YesOrNo.No;
                if (string.IsNullOrWhiteSpace(stu.y_examNum))
                {
                    stu.y_examNum = "";
                }
                var msg = _stuInfoDal.AddStuInfoExtended(stu, Request, ad);
                if (msg == "ok")
                {
                    re["msg"] = "添加成功";
                    re["isok"] = true;
                    //var feeTblist = new List<YD_Fee_StuFeeTb>();
                    //var oneYearStu = ad.VW_StuInfo.FirstOrDefault(u => u.id == stu.id);
                    //if (oneYearStu != null)
                    //{
                    //    var stuFee = new YD_Fee_StuFeeTb();
                    //    stuFee.y_feeYear = 1;
                    //    stuFee.y_isUp = (int) YesOrNo.No;
                    //    stuFee.y_isCheckFee = (int) YesOrNo.No;
                    //    stuFee.y_stuId = oneYearStu.id;
                    //    stuFee.y_registerYear = DateTime.Now.Year;
                    //    stuFee.y_createtime = DateTime.Now;
                    //    stuFee.y_needFee = 0;
                    //    stuFee.y_needUpFee = 0;
                    //    ad.Entry(stuFee).State = EntityState.Added;
                    //    ad.SaveChanges();
                    //}
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        #endregion

        #region 学生信息编辑视图 

        /// <summary>
        /// 学生信息编辑视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult EditStudent(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
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

            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfo");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                if (student == null)
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewData["student"] = student;
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        public ActionResult TermRegisterEditStu(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StuRegistra");
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

            if (!id.HasValue)
            {
                return RedirectToAction("StuRegistra");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                if (student == null)
                {
                    return RedirectToAction("StuRegistra");
                }
                ViewData["student"] = student;
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        #endregion

        #region 学生信息查看视图

        /// <summary>
        /// 学生信息查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StudentDes(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
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

            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfo");
            }

            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                //获取异动信息
                var strange =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status)
                        .FirstOrDefault(u => u.y_stuId == id.Value);
                ViewBag.strange = strange;
                if (student == null)
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewData["student"] = student;
            }
            return View();
        }

        #endregion

        #region 学生信息编辑AJAX

        /// <summary>
        /// 学生信息编辑AJAX
        /// </summary>
        /// <param name="stu">学生信息对象</param>
        /// <returns>处理结果json</returns>
        public string EditStudentInfo(YD_Sts_StuInfo stu)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var subSchool = Request["y_subSchoolId"];

            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                var stus = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stu.id);

                var re = new Hashtable();
                if (stu.y_subSchoolId == 0)
                {
                    stus.y_subSchoolId = stus.y_subSchoolId;
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolid = Convert.ToInt32(subSchool);
                    stus.y_subSchoolId = subSchoolid;
                }
                var majorli = Request["MajorLibrary"];
                int majorliid = 0;
                string ms = "";
                if (majorli == "" && stu.y_majorId == 0)
                {
                    var major = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == stus.y_majorId);
                    if (major != null)
                        majorliid = major.y_majorLibId;
                }
                else
                {
                    majorliid = Convert.ToInt32(majorli);
                }

                var majorId = GetMajorIds(majorliid, Convert.ToInt32(Request.Params["EduType"]),
                    Convert.ToInt32(Request.Params["StuType"]));
                if (majorId == 0)
                {
                    ms = "学生修改失败：专业生成参数错误";
                    return ms;
                }
                stus.y_majorId = majorId;

                if (stu.y_nationId == 0)
                {
                    stus.y_nationId = null;
                }
                else
                {
                    stus.y_nationId = stu.y_nationId;
                }
                if (stu.y_politicsId == 0)
                {
                    stus.y_politicsId = null;
                }
                else
                {
                    stus.y_politicsId = stu.y_politicsId;
                }
                //var msg = _stuInfoDal.EditStuInfoExtended(stu, Request, yunEntities);
                stus.y_name = stu.y_name;
                stus.y_sex = stu.y_sex;
                stus.y_cardId = stu.y_cardId;
                stus.y_birthday = stu.y_birthday;
                stus.y_tel = stu.y_tel;
                stus.y_mail = stu.y_mail;
                stus.y_address = stu.y_address;
                stus.y_img = stu.y_img;
                stus.y_stuNum = stu.y_stuNum;
                stus.y_examNum = stu.y_examNum == null ? "" : stu.y_examNum;
                stus.y_inYear = stu.y_inYear;
                stus.y_stuStateId = stu.y_stuStateId;
                stus.y_inYear = stu.y_inYear;
                yunEntities.Entry(stus).State = EntityState.Modified;

                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    LogHelper.DbLog(Convert.ToInt32(Session[KeyValue.Admin_id]),
                   Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update,
                    "修改学生，" + ",id：" + stu.id + "姓名，入学年份，身份证号，出生日期，学号，考生号，学籍状态" + stu.y_name + "," + stu.y_inYear + "," + stu.y_cardId + "," + stu.y_birthday + "," + stu.y_stuNum + "," + stu.y_examNum + "," + stu.y_stuStateId);
                    re["msg"] = "修改成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = ms;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);

            }
        }

        #endregion

        #region 学籍信息保存学生头像到数据库
        public string SaveStudentImg()
        {
            var img = Request["img"];
            var stuid = Request["stuid"];

            if (string.IsNullOrWhiteSpace(img))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(stuid))
            {
                return "未知错误";
            }
            var _img = Server.MapPath(@"/Upload/Web/image/20190325/" + img.Substring(img.LastIndexOf("/") + 1, img.Length - img.LastIndexOf("/") - 1));

            using (FileStream fs = new FileStream(_img, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                int width = image.Width;
                int height = image.Height;
                if (width != 100 && height != 150)
                {
                    return "图片尺寸不符合规格，请上传宽度100px，高度150px的照片";
                }
            }

            using (var yunEntities = new IYunEntities())
            {

                var ostuid = Convert.ToInt32(stuid);
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == ostuid);

                stu.y_img = img;

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

        #endregion

        #region 学生信息软删除AJAX

        /// <summary>
        /// 学生信息软删除AJAX
        /// </summary>
        /// <param name="id">学生id</param>
        /// <returns>处理结果json</returns>
        public string DeleStudentById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfo");
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
                var msg = _stuInfoDal.DeleStudentById(id, ad);
                if (msg == "ok")
                {
                    re["msg"] = "删除成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        /// <summary>
        /// 考生信息软删除AJAX
        /// </summary>
        /// <param name="id">考生id</param>
        /// <returns>处理结果json</returns>
        public string DeleSubStudentById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");
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
                var stu = ad.YD_Sts_SubSchoolStuInfo.Find(id);
                if (stu == null)
                {
                    return "删除学生失败！编号无效";
                }
                stu.y_isdel = (int)YesOrNo.Yes;
                ad.Entry(stu).State = EntityState.Modified;
                int t = ad.SaveChanges();
                if (t > 0)
                {
                    re["msg"] = "删除成功";
                    re["isok"] = true;
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "删除考生，学生为" + stu.y_name + ",考生号：" + stu.y_examNum);
                }
                else
                {
                    re["msg"] = "删除失败";
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        #endregion

        #region 批量导入学生信息

        /// <summary>
        /// 批量导入学生页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadStudentTerm(string fileName)
        {
            var result = new Hashtable
            {
                ["Message"] = "Excel格式不正确",
                ["IsOk"] = false
            };
            //var fileName = Request["filename"];
            //if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
            //{
            //    return "该文件不是正确的Excel文件";
            //}
            //fileName = Server.MapPath(fileName);
            fileName = @"F:\成人教务系统\江西师范大学\新\trunk\师大208级学籍第四次.xlsx"; //@"C:\Users\云端002\Desktop\江西师范大学\江西师范大学2.xlsx";
            string Hz; //后缀名

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
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
                    return Content("<script type='text/javascript'>alert('Excel格式不正确')</script >");
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Content("<script type='text/javascript'>alert('Excel格式不正确')</script >");
                }
                var errorCount = 0;

                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"].ToString();
                List<YD_Sts_StuInfoTemp> list = null; 
                if (schoolname == "JXLG")
                {
                    list = CoreFunction.StudentTempletValidateJXLG(ref errorCount, sheet, styleCell);
                }
                else
                {
                    list = CoreFunction.StudentTempletValidate(ref errorCount, sheet, styleCell);
                }
                //验证表格的错误情况，并返回错误数量，详情参方法体内
                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/学生导入信息表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return
                            Content(
                                "<script type='text/javascript'>alert('Excel验证失败，点击确认获取Excel错误提示！');window.location.href='" +
                                url + "';</script >");
                        //result["IsOk"] = true;
                        //result["Message"] = url;
                        //return JsonConvert.SerializeObject(result);
                    }
                }
                else //否则直接导入数据
                {
                    CoreFunction.UploadTrueStu(list);
                    result["Message"] = list.Count.ToString();
                    //return JsonConvert.SerializeObject(result);
                    return
                        Content("<script type='text/javascript'>alert('导入成功,导入" + list.Count +
                                "条数据');window.location.href='/Student/NewlyStudentInfo';</script >");
                }
            }
        }

        /// <summary>
        /// 批量导入考生页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadSubStudentTerm(string fileName)
        {
            var result = new Hashtable
            {
                ["Message"] = "Excel格式不正确",
                ["IsOk"] = false
            };
            fileName = Server.MapPath(fileName);
            string Hz; //后缀名

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {

                IWorkbook workbook;
                var errorCount = 0;

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
                    return Content("<script type='text/javascript'>alert('Excel格式不正确')</script >");
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Content("<script type='text/javascript'>alert('Excel格式不正确')</script >");
                }


                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);
                int subSchoolId = 1;
                using (var ad = new IYunEntities())
                {
                    if(ad.YD_Sys_AdminSubLink.Any(x => x.y_adminId == YdAdminId))
                    {
                        subSchoolId = ad.YD_Sys_AdminSubLink.FirstOrDefault(x => x.y_adminId == YdAdminId).y_subSchoolId;
                    }
                }
                   

                //using (var ad = new IYunEntities())
                //{
                //if (YdAdminRoleId == 4)
                //{
                //    var subadmin = ad.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                //    if (subadmin != null)
                //    {
                //        var entity =
                //            ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subadmin.y_subSchoolId);
                //        if (subadmin != null)
                //        {
                //            subSchoolId = entity.id;
                //        }
                //        else
                //        {
                //            return
                //                Content(
                //                    "<script type='text/javascript'>alert('请联系管理员到用户管理为本账号指定函授站');window.location.href='/Student/SubSchoolStuInfo';</script >");
                //        }
                //    }
                //    else
                //    {
                //        return
                //            Content(
                //                "<script type='text/javascript'>alert('请联系管理员到用户管理为本账号指定函授站');window.location.href='/Student/SubSchoolStuInfo';</script >");
                //    }
                //}
                //else
                //{
                //    return Content("只用于函授站账号使用");
                //}
                //}

                var list = CoreFunction.SubStudentTempletValidate(ref errorCount, sheet, styleCell, subSchoolId);

                //验证表格的错误情况，并返回错误数量，详情参方法体内
                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/考生导入信息表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return
                            Content(
                                "<script type='text/javascript'>alert('Excel验证失败，点击确认获取Excel错误提示！');window.location.href='" +
                                url + "';</script >");
                        //result["IsOk"] = true;
                        //result["Message"] = url;
                        //return JsonConvert.SerializeObject(result);
                    }
                }
                else //否则直接导入数据
                {
                    CoreFunction.UploadSubStu(list);
                    result["Message"] = list.Count.ToString();
                    //return JsonConvert.SerializeObject(result);
                    return
                        Content("<script type='text/javascript'>alert('导入成功,导入" + list.Count +
                                "条数据');window.location.href='/Student/SubSchoolStuInfo';</script >");
                }

            }
        }
        /// <summary>
        /// 批量导入学生页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult UploadStudent(int id = 1)
        //{
        //    #region 权限验证
        //    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "权限 start!");
        //    var power = SafePowerPage("/Student/StudentInfo");
        //    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "权限 end!");
        //    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "登录 start!");
        //    if (!IsLogin())
        //    {
        //        return Redirect("/AdminBase/Index");
        //    }
        //    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "登录 end!");
        //    if (power == null || power.y_menu == (int)PowerState.Disable)
        //    {
        //        //var reurl = Request.UrlReferrer.ToString();
        //        var reurl = "/AdminBase/Index";
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }

        //    #endregion
        //    var fileName = Request["filename"];
        //    if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
        //    {
        //        return Content("该文件不是正确的Excel文件");
        //    }
        //    fileName = Server.MapPath(fileName);
        //    using (var excelHelper = new ExcelHelper(fileName))
        //    {

        //        var dt = excelHelper.ExcelToDataTable("", true);

        //        int colCount = 0;
        //        var schoolname = ConfigurationManager.AppSettings["SchoolName"].ToString();

        //        if (KeyValue.NationAndPolitics.Contains(schoolname) && KeyValue.SubschoolAndSchool.Contains(schoolname))
        //        {
        //            colCount = 24;
        //        }
        //        //如果有导入政治面貌和民族学校导入字段
        //        else if (KeyValue.NationAndPolitics.Contains(schoolname))
        //        {
        //            colCount = 25;
        //        }
        //        else if (KeyValue.SubschoolAndSchool.Contains(schoolname))
        //        {
        //            colCount = 13;
        //        }
        //        else
        //        {
        //            colCount = 14;
        //        }
        //        if (dt.Columns.Count != colCount)
        //        {
        //            return Content("该文件不合法，请确认是否多列或少列！");
        //        }
        //        //验证excel文件列信息是否正确
        //        string column = string.Empty;

        //        //如果有导入政治面貌和民族学校以及匹配函授站导入字段
        //        if (KeyValue.NationAndPolitics.Contains(schoolname) && KeyValue.SubschoolAndSchool.Contains(schoolname))
        //        {
        //            column = "姓名,性别,学号,考生号,准考证号,毕业日期,毕业学校,外语语种,招生类别,职业类别,文化程度,考生特征,入学年份, 学习形式,层次,专业名,学籍状态,电话,地址,邮政编码,身份证号,出生日期,民族,政治面貌";
        //        }
        //        else if (KeyValue.NationAndPolitics.Contains(schoolname))
        //        {
        //            column = "姓名,性别,学号,考生号,准考证号,毕业日期,毕业学校,外语语种,招生类别,职业类别,文化程度,考生特征,入学年份, 学习形式,层次,专业名,函授站,学籍状态,电话,地址,邮政编码,身份证号,出生日期,民族,政治面貌";
        //        }
        //        else if (KeyValue.SubschoolAndSchool.Contains(schoolname))
        //        {
        //            column = "姓名,性别,学号,考生号,入学年份,学习形式,层次,专业名,学籍状态,电话,地址,身份证号,出生日期";
        //        }
        //        else
        //        {
        //            column = "姓名,性别,学号,考生号,入学年份,学习形式,层次,专业名,函授站,学籍状态,电话,地址,身份证号,出生日期";
        //        }

        //        foreach (var item in dt.Columns)
        //        {
        //            if (column.Contains(item.ToString()))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                return Content("该文件有错误的列，请仔细阅读上传附件时的说明。");
        //            }
        //        }

        //        using (var yunEntities = new IYunEntities())
        //        {
        //            yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Sts_StuInfoTemp");

        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

        //            string name = "";
        //            int sex = -1;
        //            string sexName = "";
        //            int year = -1;
        //            string cardId = "";
        //            int majorId = -1;
        //            int majorLibId = -1;
        //            string majorLibName = "";
        //            string majorNameMatch = "";
        //            int subSchoolId = -1;
        //            string subSchoolName = "";
        //            string subNameMatch = "";
        //            int stuTypeId = -1;
        //            string stuTypeName = "";
        //            int eduTypeId = -1;
        //            string eduTypeName = "";
        //            string stuNum = "";
        //            string examNum = "";
        //            string tel = "";
        //            string address = "";
        //            int stuStateId = -1;
        //            string stuStateName = "";
        //            string birthdate = "";
        //            DateTime birthday = DateTime.MaxValue;
        //            int nationId = -1;
        //            string nationName = "";
        //            int politicsId = -1;
        //            string politicsName = "";

        //            //科技师范新添加的字段
        //            string gradudate = "";
        //            DateTime graduationdata = DateTime.MaxValue; //毕业日期
        //            string graduationschool = "";//毕业学校
        //            string registrationNum = "";//准考证号
        //            string postalcode = "";//邮政编码
        //            int foreignLanguageId = -1;//外语语种
        //            string foreignLanguageName = "";
        //            int recruitTypeId = -1;// 招生类别
        //            string recruitTypeName = "";
        //            int professionTypeId = -1;//职业类别
        //            string professionTypeName = "";
        //            int cultureExtentId = -1;//文化程度
        //            string cultureExtentName = "";
        //            int examFeatureId = -1;//考生特征
        //            string examFeatureName = "";

        //            int isOk = (int)YesOrNo.No;
        //            var studentTempList = new List<YD_Sts_StuInfoTemp>();
        //            LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "循环内容 state!");
        //            for (var i = 0; i < dt.Rows.Count; i++)
        //            {
        //                name = "";
        //                sex = -1;
        //                sexName = "";
        //                year = -1;
        //                cardId = "";
        //                majorId = -1;
        //                majorLibId = -1;
        //                majorLibName = "";
        //                majorNameMatch = "";
        //                if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    nationId = -1;
        //                    nationName = "";
        //                    politicsId = -1;
        //                    politicsName = "";
        //                }
        //                subSchoolId = -1;
        //                subSchoolName = "";
        //                subNameMatch = "";
        //                stuTypeId = -1;
        //                stuTypeName = "";
        //                eduTypeId = -1;
        //                eduTypeName = "";
        //                stuNum = "";
        //                examNum = "";
        //                tel = "";
        //                address = "";
        //                stuStateId = -1;
        //                stuStateName = "";
        //                birthday = DateTime.MaxValue;

        //                //科技师范新添加的字段
        //                graduationdata = DateTime.MaxValue; //毕业日期
        //                graduationschool = "";//毕业学校
        //                registrationNum = "";//准考证号
        //                postalcode = "";//邮政编码
        //                foreignLanguageId = -1;//外语语种
        //                foreignLanguageName = "";
        //                recruitTypeId = -1;// 招生类别
        //                recruitTypeName = "";
        //                professionTypeId = -1;//职业类别
        //                professionTypeName = "";
        //                cultureExtentId = -1;//文化程度
        //                cultureExtentName = "";
        //                examFeatureId = -1;//考生特征
        //                examFeatureName = "";
        //                isOk = (int)YesOrNo.No;

        //                //科技师范大学新添加字段
        //                if (KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    #region 科技师范新添加的字段
        //                    if (dt.Rows[i]["毕业日期"] != null)
        //                    {
        //                        gradudate = dt.Rows[i]["毕业日期"].ToString();
        //                        if (gradudate != "")
        //                        {
        //                            DateTime.TryParse(
        //                                gradudate.Substring(0, 4) + "-" + gradudate.Substring(4, 2) + "-" +
        //                                gradudate.Substring(6, 2), out graduationdata);
        //                        }
        //                        else
        //                        {
        //                            graduationdata = DateTime.MaxValue;
        //                        }
        //                    }
        //                    if (dt.Rows[i]["毕业学校"] != null)
        //                    {
        //                        graduationschool = dt.Rows[i]["毕业学校"].ToString();
        //                    }
        //                    if (dt.Rows[i]["准考证号"] != null)
        //                    {
        //                        registrationNum = dt.Rows[i]["准考证号"].ToString();
        //                    }
        //                    if (dt.Rows[i]["邮政编码"] != null)
        //                    {
        //                        postalcode = dt.Rows[i]["邮政编码"].ToString();
        //                    }
        //                    #region 获取外语语种
        //                    if (dt.Rows[i]["外语语种"] != null)
        //                    {
        //                        foreignLanguageName = dt.Rows[i]["外语语种"].ToString();
        //                        var entity = yunEntities.YD_Edu_ForeignLanguage.FirstOrDefault(u => u.y_name == foreignLanguageName);
        //                        if (entity != null)
        //                            foreignLanguageId = entity.id;
        //                    }
        //                    #endregion
        //                    #region 获取招生类别
        //                    if (dt.Rows[i]["招生类别"] != null)
        //                    {
        //                        recruitTypeName = dt.Rows[i]["招生类别"].ToString();
        //                        var entity = yunEntities.YD_Edu_RecruitType.FirstOrDefault(u => u.y_name == recruitTypeName);
        //                        if (entity != null)
        //                            recruitTypeId = entity.id;
        //                    }
        //                    #endregion
        //                    #region 获取职业类别
        //                    if (dt.Rows[i]["职业类别"] != null)
        //                    {
        //                        professionTypeName = dt.Rows[i]["职业类别"].ToString();
        //                        var entity = yunEntities.YD_Edu_ProfessionType.FirstOrDefault(u => u.y_name == professionTypeName);
        //                        if (entity != null)
        //                            professionTypeId = entity.id;
        //                    }
        //                    #endregion
        //                    #region 获取文化程度
        //                    if (dt.Rows[i]["文化程度"] != null)
        //                    {
        //                        cultureExtentName = dt.Rows[i]["文化程度"].ToString();
        //                        var entity = yunEntities.YD_Edu_CultureExtent.FirstOrDefault(u => u.y_name == cultureExtentName);
        //                        if (entity != null)
        //                            cultureExtentId = entity.id;
        //                    }
        //                    #endregion
        //                    #endregion
        //                }
        //                #region 性别id

        //                if (dt.Rows[i]["性别"] != null)
        //                {
        //                    sexName = dt.Rows[i]["性别"].ToString().Trim();
        //                    switch (sexName)
        //                    {
        //                        case "男":
        //                            sex = 0;
        //                            break;
        //                        case "女":
        //                            sex = 1;
        //                            break;
        //                        default:
        //                            sex = -1;
        //                            break;
        //                    }
        //                }

        //                #endregion

        //                #region 获取入学年份

        //                if (dt.Rows[i]["入学年份"] != null)
        //                {
        //                    int.TryParse(dt.Rows[i]["入学年份"].ToString().Trim(), out year);
        //                }

        //                #endregion

        //                #region 获取学习形式id

        //                if (dt.Rows[i]["学习形式"] != null)
        //                {
        //                    stuTypeName = dt.Rows[i]["学习形式"].ToString();
        //                    var entity = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.y_name == stuTypeName);
        //                    if (entity != null)
        //                        stuTypeId = entity.id;
        //                }

        //                #endregion

        //                #region 获取层次id

        //                if (dt.Rows[i]["层次"] != null)
        //                {
        //                    eduTypeName = dt.Rows[i]["层次"].ToString().Trim();
        //                    var entity = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.y_name == eduTypeName);
        //                    if (entity != null)
        //                        eduTypeId = entity.id;
        //                }

        //                #endregion

        //                //判断是哪个学校需要政治面貌和民族
        //                if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    #region   获取民族id

        //                    if (dt.Rows[i]["民族"] != null)
        //                    {
        //                        nationName = dt.Rows[i]["民族"].ToString().Trim();
        //                        var entity = yunEntities.YD_Sts_Nation.FirstOrDefault(u => u.y_name == nationName);
        //                        if (entity != null)
        //                            nationId = entity.id;
        //                    }

        //                    #endregion
        //                    #region   获取政治面貌id

        //                    if (dt.Rows[i]["政治面貌"] != null)
        //                    {
        //                        politicsName = dt.Rows[i]["政治面貌"].ToString().Trim();
        //                        var entity = yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.y_name == politicsName);
        //                        if (entity != null)
        //                            politicsId = entity.id;
        //                    }

        //                    #endregion
        //                }
        //                #region 获取专业库id

        //                if (dt.Rows[i]["专业名"] != null)
        //                {
        //                    majorLibName = dt.Rows[i]["专业名"].ToString().Replace(" ", "");
        //                    var entity =
        //                        yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(
        //                            u => u.y_name == majorLibName);
        //                    if (entity == null)
        //                    {
        //                        var myname = majorLibName.Replace("专业", "");
        //                        if (myname.Length <= 2)
        //                        {
        //                            var entityList =
        //                                yunEntities.YD_Edu_MajorLibrary.Where(u => u.y_name.Contains(myname));
        //                            var objList = entityList.ToList();
        //                            for (var j = 0; j < objList.Count; j++)
        //                            {
        //                                majorNameMatch += objList[j].y_name;
        //                                if (j + 1 < objList.Count)
        //                                {
        //                                    majorNameMatch += ",";
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var sql = "";
        //                            for (var j = 0; j < myname.Length - 1; j++)
        //                            {
        //                                sql += " y_name like '%" + myname.Substring(j, 2) + "%' ";
        //                                if (j + 1 < myname.Length - 1)
        //                                {
        //                                    sql += " or ";
        //                                }
        //                            }
        //                            var entityList =
        //                                yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
        //                                    "select * from YD_Edu_MajorLibrary where " + sql).ToList();
        //                            var objList = entityList.ToList();
        //                            for (var j = 0; j < objList.Count; j++)
        //                            {
        //                                majorNameMatch += objList[j].y_name;
        //                                if (j + 1 < objList.Count)
        //                                {
        //                                    majorNameMatch += ",";
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        majorLibId = entity.id;
        //                    }

        //                }

        //                #endregion

        //                #region 获取专业id

        //                if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0)
        //                {
        //                    majorId = GetMajorIds(majorLibId, eduTypeId, stuTypeId);
        //                }

        //                #endregion

        //                //如果是科技师范大学则不需要导入此函授站，其他学校目前待定
        //                if (!KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {

        //                    #region 获取函授站id

        //                    if (dt.Rows[i]["函授站"] != null)
        //                    {
        //                        subSchoolName = dt.Rows[i]["函授站"].ToString().Replace(" ", "");
        //                        var entity = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.y_name == subSchoolName);
        //                        if (entity == null)
        //                        {
        //                            var myname = subSchoolName.Replace("函授站", "").Replace("函授", "");

        //                            if (myname.Length <= 2)
        //                            {
        //                                var entityList =
        //                                    yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(myname));
        //                                var objList = entityList.ToList();
        //                                for (var j = 0; j < objList.Count; j++)
        //                                {
        //                                    subNameMatch += objList[j].y_name;
        //                                    if (j + 1 < objList.Count)
        //                                    {
        //                                        subNameMatch += ",";
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var sql = "";
        //                                for (var j = 0; j < myname.Length - 1; j++)
        //                                {
        //                                    sql += " y_name like '%" + myname.Substring(j, 2) + "%' ";
        //                                    if (j + 1 < myname.Length - 1)
        //                                    {
        //                                        sql += " or ";
        //                                    }
        //                                }
        //                                var entityList =
        //                                    yunEntities.Database.SqlQuery<YD_Sys_SubSchool>(
        //                                        "select * from YD_Sys_SubSchool where " + sql).ToList();
        //                                var objList = entityList.ToList();
        //                                for (var j = 0; j < objList.Count; j++)
        //                                {
        //                                    subNameMatch += objList[j].y_name;
        //                                    if (j + 1 < objList.Count)
        //                                    {
        //                                        subNameMatch += ",";
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            subSchoolId = entity.id;
        //                        }
        //                    }
        //                    #endregion
        //                }

        //                #region 获取学籍状态id

        //                if (dt.Rows[i]["学籍状态"] != null)
        //                {
        //                    stuStateName = dt.Rows[i]["学籍状态"].ToString();
        //                    var entity = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == stuStateName);
        //                    if (entity != null)
        //                        stuStateId = entity.id;
        //                }

        //                #endregion

        //                if (dt.Rows[i]["学号"] != null)
        //                {
        //                    stuNum = dt.Rows[i]["学号"].ToString();
        //                }
        //                if (dt.Rows[i]["电话"] != null)
        //                {
        //                    tel = dt.Rows[i]["电话"].ToString();
        //                }
        //                if (dt.Rows[i]["考生号"] != null)
        //                {
        //                    examNum = dt.Rows[i]["考生号"].ToString().Trim();
        //                    var temp = yunEntities.YD_Sts_StuInfoTemp.FirstOrDefault(u => u.y_examNum == examNum);
        //                    var substu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == examNum && u.y_isdel == 1);
        //                    if (temp != null || substu != null)
        //                    {
        //                        examNum = "";
        //                    }
        //                }
        //                if (dt.Rows[i]["身份证号"] != null)
        //                {
        //                    cardId = dt.Rows[i]["身份证号"].ToString().Trim();
        //                    var temp = yunEntities.YD_Sts_StuInfoTemp.FirstOrDefault(u => u.y_cardId == cardId);
        //                    var substu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_cardId == cardId && u.y_isdel == 1);
        //                    if (temp != null || substu != null)
        //                    {
        //                        cardId = "";
        //                    }
        //                }
        //                if (dt.Rows[i]["出生日期"] != null && (Regex.IsMatch(dt.Rows[i]["出生日期"].ToString(),
        //                    @"^[12]\d{3}(0\d|1[0-2])([0-2]\d|3[01])$",
        //                        RegexOptions.IgnoreCase)))
        //                {
        //                    birthdate = dt.Rows[i]["出生日期"].ToString();
        //                    /*\b\d{4}((?:0[13578]|1[02])(?:0[1-9]|[12]\d|3[01])|02(?:0[1-9]|[12]\d)|(?:0[469]|11)(?:0[1-9]|[12]\d|30))\b*/
        //                    if (birthdate != "")
        //                    {
        //                        DateTime.TryParse(
        //                            birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" +
        //                            birthdate.Substring(6, 2), out birthday);
        //                    }
        //                    else
        //                    {
        //                        birthday = DateTime.MaxValue;
        //                    }
        //                }
        //                if (dt.Rows[i]["地址"] != null)
        //                {
        //                    address = dt.Rows[i]["地址"].ToString();
        //                }
        //                if (dt.Rows[i]["姓名"] != null)
        //                {
        //                    name = dt.Rows[i]["姓名"].ToString().Trim();
        //                }
        //                if (KeyValue.NationAndPolitics.Contains(schoolname) && KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                       birthday != DateTime.MaxValue &&
        //                       year != -1 && year != -1 && stuStateId != -1 && name != "" && nationId != -1 && politicsId != -1
        //                       && registrationNum != "" && foreignLanguageId != -1 && recruitTypeId != -1 && professionTypeId != -1 && cultureExtentId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "" && nationId != -1 && politicsId != -1 && subSchoolId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else if (KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "")
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "" && subSchoolId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                var studentTemp = new YD_Sts_StuInfoTemp();
        //                if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    studentTemp = new YD_Sts_StuInfoTemp
        //                    {
        //                        y_name = name,
        //                        y_sex = sex,
        //                        y_sexName = sexName,
        //                        y_inYear = year,
        //                        y_cardId = cardId,
        //                        y_majorId = majorId,
        //                        y_majorLibId = majorLibId,
        //                        y_majorLibName = majorLibName,
        //                        y_subSchoolId = subSchoolId,
        //                        y_subSchoolName = subSchoolName,
        //                        y_stuTypeId = stuTypeId,
        //                        y_stuTypeName = stuTypeName,
        //                        y_eduTypeId = eduTypeId,
        //                        y_eduTypeName = eduTypeName,
        //                        y_stuNum = stuNum,
        //                        y_examNum = examNum,
        //                        y_tel = tel,
        //                        y_address = address,
        //                        y_stuStateId = stuStateId,
        //                        y_stuStateName = stuStateName,
        //                        y_birthday = birthday,
        //                        y_isOk = isOk,
        //                        y_majorNameMatch = majorNameMatch,
        //                        y_subNameMatch = subNameMatch,
        //                        y_nationId = nationId,
        //                        y_politicsId = politicsId,
        //                        y_nationName = nationName,
        //                        y_politicsName = politicsName,
        //                        y_graduationdata = graduationdata,
        //                        y_graduationschool = graduationschool,
        //                        y_registrationNum = registrationNum,
        //                        y_postalcode = postalcode,
        //                        y_foreignLanguageId = foreignLanguageId,
        //                        y_foreignLanguageName = foreignLanguageName,
        //                        y_recruitTypeId = recruitTypeId,
        //                        y_recruitTypeName = recruitTypeName,
        //                        y_professionTypeId = professionTypeId,
        //                        y_professionTypeName = professionTypeName,
        //                        y_cultureExtentId = cultureExtentId,
        //                        y_cultureExtentName = cultureExtentName,
        //                        y_examFeatureId = examFeatureId,
        //                        y_examFeatureName = examFeatureName
        //                    };
        //                }
        //                else
        //                {
        //                    studentTemp = new YD_Sts_StuInfoTemp
        //                    {
        //                        y_name = name,
        //                        y_sex = sex,
        //                        y_sexName = sexName,
        //                        y_inYear = year,
        //                        y_cardId = cardId,
        //                        y_majorId = majorId,
        //                        y_majorLibId = majorLibId,
        //                        y_majorLibName = majorLibName,
        //                        y_subSchoolId = subSchoolId,
        //                        y_subSchoolName = subSchoolName,
        //                        y_stuTypeId = stuTypeId,
        //                        y_stuTypeName = stuTypeName,
        //                        y_eduTypeId = eduTypeId,
        //                        y_eduTypeName = eduTypeName,
        //                        y_stuNum = stuNum,
        //                        y_examNum = examNum,
        //                        y_tel = tel,
        //                        y_address = address,
        //                        y_stuStateId = stuStateId,
        //                        y_stuStateName = stuStateName,
        //                        y_birthday = birthday,
        //                        y_isOk = isOk,
        //                        y_majorNameMatch = majorNameMatch,
        //                        y_subNameMatch = subNameMatch
        //                    };
        //                }
        //                studentTempList.Add(studentTemp);
        //            }

        //            yunEntities.Configuration.AutoDetectChangesEnabled = false;
        //            yunEntities.Configuration.ValidateOnSaveEnabled = false;
        //            yunEntities.Set<YD_Sts_StuInfoTemp>().AddRange(studentTempList);
        //            yunEntities.SaveChanges();
        //            //yunEntities.BulkInsert(studentTempList);
        //            //yunEntities.BulkSaveChanges();

        //            var list = yunEntities.YD_Sts_StuInfoTemp.Where(u => true).ToList();
        //            var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
        //            const int iisOk = (int)YesOrNo.No;
        //            ViewBag.teachPlanList = yunEntities.YD_Sts_StuInfoTemp.Where(u => u.y_isOk == iisOk).Take(15).ToList();
        //            ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
        //            if (Request.IsAjaxRequest())
        //                return PartialView("StuInfoUpdateList", dbLogList);
        //            return View(dbLogList);
        //        }
        //    }
        //}
        /// <summary>
        /// 批量导入考生页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadSubSchoolStu(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");
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

            var fileName = Request["filename"];
            if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
            {
                return Content("该文件不是正确的Excel文件");
            }
            fileName = Server.MapPath(fileName);
            using (var excelHelper = new ExcelHelper(fileName))
            {
                var dt = excelHelper.ExcelToDataTable("", true);
                int colCount = 4;
                if (dt.Columns.Count != colCount)
                {
                    return Content("该文件不合法，请确认是否多列或少列！");
                }
                //验证excel文件列信息是否正确
                string column = string.Empty;
                if (KeyValue.NationAndPolitics.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    column = "姓名，年份，考生号，身份证号";
                }
                else
                {
                    column = "姓名，年份，考生号，身份证号";
                }
                foreach (var item in dt.Columns)
                {
                    if (column.Contains(item.ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        return Content("该文件有错误的列，请仔细阅读上传附件时的说明。");
                    }
                }
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Sts_SubStuTemp");

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量导入考生页面,清空函授站学生信息临时表数据" + ",方法:UploadSubSchoolStu");

                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                    string name = "";
                    int year = 0;
                    int subSchoolId = -1;
                    string subSchoolName = "";
                    string y_examNum = "";
                    string examNumtemp = "";
                    string y_cardId = "";
                    string cardidtemp = "";
                    int isOk = (int)YesOrNo.Yes;
                    var studentTempList = new List<YD_Sts_SubStuTemp>();
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        name = "";
                        subSchoolId = -1;
                        subSchoolName = "";
                        y_examNum = "";
                        examNumtemp = "";
                        y_cardId = "";
                        cardidtemp = "";
                        isOk = (int)YesOrNo.No;

                        #region 获取函授站id

                        if (YdAdminRoleId == 4)
                        {
                            var subadmin = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                            if (subadmin != null)
                            {
                                var entity =
                                    yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subadmin.y_subSchoolId);
                                if (subadmin != null)
                                {
                                    subSchoolId = entity.id;
                                }
                                else
                                {
                                    return
                                        Content(
                                            "<script type='text/javascript'>alert('请联系管理员到用户管理为本账号指定函授站');window.location.href='/Student/SubSchoolStuInfo';</script >");
                                }
                            }
                            else
                            {
                                return
                                    Content(
                                        "<script type='text/javascript'>alert('请联系管理员到用户管理为本账号指定函授站');window.location.href='/Student/SubSchoolStuInfo';</script >");
                            }
                        }
                        else
                        {
                            return Content("只用于函授站账号使用");
                        }

                        #endregion

                        if (dt.Rows[i]["姓名"] != null)
                        {
                            name = dt.Rows[i]["姓名"].ToString().Trim();
                        }
                        if (dt.Rows[i]["年份"] != null)
                        {
                            year = Convert.ToInt32(dt.Rows[i]["年份"]);
                        }
                        if (dt.Rows[i]["考生号"] != null)
                        {
                            y_examNum = dt.Rows[i]["考生号"].ToString().Trim();
                        }
                        if (dt.Rows[i]["身份证号"] != null)
                        {
                            y_cardId = dt.Rows[i]["身份证号"].ToString().Trim();
                            //cardidtemp = y_cardId;
                            var temp = yunEntities.YD_Sts_SubStuTemp.FirstOrDefault(u => u.y_cardId == y_cardId);
                            var substu =
                                yunEntities.YD_Sts_SubSchoolStuInfo.FirstOrDefault(
                                    u => u.y_cardId == y_cardId && u.y_isdel == 1);
                            if (temp != null || substu != null)
                            {
                                cardidtemp = "-1";
                            }
                        }
                        if (subSchoolId != -1 && y_cardId != "" && cardidtemp != "-1")
                        {
                            isOk = (int)YesOrNo.Yes;
                        }
                        else
                        {
                            isOk = (int)YesOrNo.No;
                        }
                        var studentTemp = new YD_Sts_SubStuTemp
                        {
                            y_name = name,
                            y_year = year,
                            y_subSchoolId = subSchoolId,
                            y_examNum = y_examNum,
                            y_cardId = y_cardId,
                            y_isOk = isOk,
                            y_cardIdTemp = cardidtemp

                        };
                        studentTempList.Add(studentTemp);

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量导入考生页面,添加函授站学生信息临时表数据,ID:" + studentTemp.id + ",身份证:" + studentTemp.y_cardId + ",方法:UploadSubSchoolStu");
                    }
                    yunEntities.Configuration.AutoDetectChangesEnabled = false;
                    yunEntities.Configuration.ValidateOnSaveEnabled = false;
                    yunEntities.Set<YD_Sts_SubStuTemp>().AddRange(studentTempList);
                    yunEntities.SaveChanges();
                    var list = yunEntities.YD_Sts_SubStuTemp.Where(u => true).ToList();
                    var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                    const int iisOk = (int)YesOrNo.No;
                    ViewBag.teachPlanList =
                        yunEntities.YD_Sts_SubStuTemp.Where(u => u.y_isOk == iisOk).Take(15).ToList();
                    ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
                    if (Request.IsAjaxRequest())
                        return PartialView("SubStuUpdateList", dbLogList);
                    return View(dbLogList);
                }
            }
        }

        /// <summary>
        /// 验证导入的临时学生信息
        /// </summary>
        /// <returns></returns>
        //public ActionResult VerifyStudent(int id = 1)
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/StudentInfo");
        //    if (!IsLogin())
        //    {
        //        return Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_menu == (int)PowerState.Disable)
        //    {
        //        //var reurl = Request.UrlReferrer.ToString();
        //        var reurl = "/AdminBase/Index";
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }

        //    #endregion

        //    using (var yunEntities = new IYunEntities())
        //    {
        //        const int isOk = (int)YesOrNo.No;
        //        //如果数据库存在导入的教学计划则不导入
        //        var alikesubstu = yunEntities.YD_Sts_StuInfoTemp.ToList();
        //        var lib = yunEntities.VW_StuInfo.AsQueryable();
        //        if (alikesubstu.Any())
        //        {
        //            foreach (var item in alikesubstu)
        //            {
        //                if (lib.Any(a => a.y_majorId == item.y_majorId && a.y_cardId == item.y_cardId && a.y_subSchoolId == item.y_subSchoolId && a.y_examNum == item.y_examNum && a.y_inYear == item.y_inYear))
        //                {
        //                    yunEntities.YD_Sts_StuInfoTemp.Remove(item);
        //                }
        //            }
        //            int r = yunEntities.SaveChanges();
        //            if (r > 0)
        //            {
        //                return Content("<script type='text/javascript'>alert('有重复学生数据不导入，重复条数为" + r + "');window.location.href='/Student/VerifyStudent';</script >");
        //            }
        //        }
        //        var list = yunEntities.YD_Sts_StuInfoTemp.Where(u => u.y_isOk == isOk).ToList();
        //        //var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;

        //        ViewBag.teachPlanList = yunEntities.YD_Sts_StuInfoTemp.Where(u => u.y_isOk == isOk).ToList();

        //        ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
        //        ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
        //        //if (Request.IsAjaxRequest())
        //        //    return PartialView("VerifyStudentList", dbLogList);
        //        return View(list);
        //    }
        //}
        /// <summary>
        /// 验证导入的临时函授站学生信息
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifySubStudent(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");
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
                const int isOk = (int)YesOrNo.No;
                //如果数据库存在导入的教学计划则不导入
                var alikesubstu = yunEntities.YD_Sts_SubStuTemp.ToList();
                var lib = yunEntities.VW_SubSchoolStuInfo.Where(u => u.y_isdel == 1).AsQueryable();
                if (alikesubstu.Any())
                {
                    foreach (var item in alikesubstu)
                    {
                        if (
                            lib.Any(
                                a =>
                                    a.y_cardId == item.y_cardId && a.y_subSchoolId == item.y_subSchoolId &&
                                    a.y_examNum == item.y_examNum))
                        {
                            yunEntities.YD_Sts_SubStuTemp.Remove(item);
                        }
                    }
                    int r = yunEntities.SaveChanges();
                    if (r > 0)
                    {
                        return
                            Content("<script type='text/javascript'>alert('有重复学生数据不导入，重复条数为" + r +
                                    "');window.location.href='/Student/VerifySubStudent';</script >");
                    }
                }
                var list = yunEntities.YD_Sts_SubStuTemp.Where(u => u.y_isOk == isOk).ToList();
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;

                ViewBag.teachPlanList = yunEntities.YD_Sts_SubStuTemp.Where(u => u.y_isOk == isOk).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"].ToString();
                if (Request.IsAjaxRequest())
                    return PartialView("VerifyStuStuList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 提交用户更新的临时数据
        /// </summary>
        /// <returns></returns>
        //public ActionResult UpdateVerify()
        //{
        //    var names = Request["name"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var stuNums = Request["stuNum"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var examNums = Request["examNum"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var sexs = Request["sex"].Split(new[] { "<>" }, StringSplitOptions.None);

        //    var majorLibs = Request["majorLib"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var stuTypeNames = Request["stuTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var eduTypeNames = Request["eduTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var years = Request["year"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var cardIds = Request["cardId"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var birthdays = Request["birthday"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var subSchools = Request["subSchool"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var tels = Request["tel"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var addresss = Request["address"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var stuStates = Request["stuState"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var ids = Request["id"].Split(new[] { "<>" }, StringSplitOptions.None);

        //    var graduationschools = Request["graduationschool"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var graduationdatas = Request["graduationdata"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var registrationNums = Request["registrationNum"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var postalcodes = Request["postalcode"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var foreignLanguageNames = Request["foreignLanguageName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var recruitTypeNames = Request["recruitTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var professionTypeNames = Request["professionTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var cultureExtentNames = Request["cultureExtentName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var examFeatureNames = Request["examFeatureName"].Split(new[] { "<>" }, StringSplitOptions.None);


        //    var nationNames = new string[] { };
        //    var politicsNames = new string[] { };
        //    var schoolname = ConfigurationManager.AppSettings["SchoolName"].ToString();
        //    if (KeyValue.NationAndPolitics.Contains(schoolname))
        //    {
        //        nationNames = Request["nationName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //        politicsNames = Request["politicsName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    }
        //    // 科技师范新添加的字段
        //    string graduationschool = "";
        //    DateTime? graduationdata = DateTime.MaxValue;
        //    string registrationNum = "";
        //    string postalcode = "";
        //    int foreignLanguageId = -1;
        //    string foreignLanguageName = "";
        //    int recruitTypeId = -1;
        //    string recruitTypeName = "";
        //    int professionTypeId = -1;
        //    string professionTypeName = "";
        //    int cultureExtentId = -1;
        //    string cultureExtentName = "";
        //    int examFeatureId = -1;
        //    string examFeatureName = "";
        //    string name = "";
        //    int sex = -1;
        //    string sexName = "";
        //    int year = -1;
        //    string cardId = "";
        //    int majorId = -1;
        //    int majorLibId = -1;
        //    string majorLibName = "";
        //    string majorNameMatch = "";
        //    int subSchoolId = -1;
        //    string subSchoolName = "";
        //    string subNameMatch = "";
        //    int stuTypeId = -1;
        //    string stuTypeName = "";
        //    int eduTypeId = -1;
        //    string eduTypeName = "";
        //    string stuNum = "";
        //    string examNum = "";
        //    string tel = "";
        //    string address = "";
        //    int stuStateId = -1;
        //    string stuStateName = "";

        //    int nationId = -1;
        //    string nationName = "";
        //    int politicsId = -1;
        //    string politicsName = "";

        //    int isOk = (int)YesOrNo.No;
        //    DateTime birthday = DateTime.MaxValue;
        //    int id = 0;
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (var i = 0; i < ids.Count(); i++)
        //        {
        //            name = "";
        //            sex = -1;
        //            sexName = "";
        //            year = -1;
        //            cardId = "";
        //            majorId = -1;
        //            majorLibId = -1;
        //            majorNameMatch = "";
        //            subSchoolId = -1;
        //            subNameMatch = "";
        //            stuTypeId = -1;
        //            eduTypeId = -1;
        //            stuStateId = -1;
        //            birthday = DateTime.MaxValue;

        //            nationId = -1;
        //            nationName = "";
        //            politicsId = -1;
        //            politicsName = "";
        //            graduationschool = "";
        //            graduationdata = DateTime.MaxValue;
        //            registrationNum = "";
        //            postalcode = "";
        //            foreignLanguageId = -1;
        //            foreignLanguageName = "";
        //            recruitTypeId = -1;
        //            recruitTypeName = "";
        //            professionTypeId = -1;
        //            professionTypeName = "";
        //            cultureExtentId = -1;
        //            cultureExtentName = "";
        //            examFeatureId = -1;
        //            examFeatureName = "";




        //            id = (int)Convert.ToInt64(ids[i]);
        //            name = names[i];
        //            var teachPlan = yunEntities.YD_Sts_StuInfoTemp.FirstOrDefault(u => u.id == id);
        //            if (teachPlan != null)
        //            {
        //                if (sexs[i] == "男")
        //                {
        //                    sex = 0;
        //                }
        //                else if (sexs[i] == "女")
        //                {
        //                    sex = 1;

        //                }
        //                else
        //                {
        //                    sex = -1;
        //                }
        //                sexName = sexs[i];
        //                if (!int.TryParse(years[i], out year))
        //                {
        //                    year = -1;
        //                }
        //                cardId = cardIds[i];
        //                if (!string.IsNullOrWhiteSpace(birthdays[i]))
        //                {
        //                    birthday = Convert.ToDateTime(birthdays[i]);
        //                }
        //                //if ((Regex.IsMatch(birthdays[i], @"^\d{4}-(0[1-9]|1[1-2])-(0[1-9]|2[0-9]|3[0-1])$")))
        //                //{
        //                //    birthday =Convert.ToDateTime(birthdays[i]);
        //                //}
        //                //else
        //                //{
        //                //    DateTime.TryParse(
        //                //             cardId.Substring(6, 4) + "-" + cardId.Substring(10, 2) + "-" +
        //                //             cardId.Substring(12, 2), out birthday);
        //                //}

        //                #region 获取学习形式id

        //                stuTypeName = stuTypeNames[i];
        //                var entity1 = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.y_name == stuTypeName);
        //                if (entity1 != null)
        //                    stuTypeId = entity1.id;

        //                #endregion

        //                //科技师范新添加的字段
        //                graduationschool = graduationschools[i];
        //                if (!string.IsNullOrWhiteSpace(graduationdatas[i]))
        //                {
        //                    graduationdata = Convert.ToDateTime(graduationdatas[i]);
        //                }
        //                registrationNum = registrationNums[i];
        //                postalcode = postalcodes[i];

        //                #region 获取外语语种id

        //                foreignLanguageName = foreignLanguageNames[i];
        //                var entity8 =
        //                    yunEntities.YD_Edu_ForeignLanguage.FirstOrDefault(u => u.y_name == foreignLanguageName);
        //                if (entity8 != null)
        //                    foreignLanguageId = entity8.id;

        //                #endregion

        //                #region 获取招生类别id

        //                recruitTypeName = recruitTypeNames[i];
        //                var entity9 = yunEntities.YD_Edu_RecruitType.FirstOrDefault(u => u.y_name == recruitTypeName);
        //                if (entity9 != null)
        //                    recruitTypeId = entity9.id;

        //                #endregion

        //                #region 获取职业类别id

        //                professionTypeName = professionTypeNames[i];
        //                var entity =
        //                    yunEntities.YD_Edu_ProfessionType.FirstOrDefault(u => u.y_name == professionTypeName);
        //                if (entity != null)
        //                    professionTypeId = entity.id;

        //                #endregion

        //                #region 获取文化程度id

        //                cultureExtentName = cultureExtentNames[i];
        //                var entity6 = yunEntities.YD_Edu_CultureExtent.FirstOrDefault(u => u.y_name == cultureExtentName);
        //                if (entity6 != null)
        //                    cultureExtentId = entity6.id;

        //                #endregion

        //                #region 获取考生特征id

        //                examFeatureName = examFeatureNames[i];
        //                var entity4 = yunEntities.YD_Edu_ExamFeature.FirstOrDefault(u => u.y_name == examFeatureName);
        //                if (entity4 != null)
        //                    examFeatureId = entity4.id;

        //                #endregion

        //                #region 获取层次id

        //                eduTypeName = eduTypeNames[i];
        //                var entity2 = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.y_name == eduTypeName);
        //                if (entity2 != null)
        //                    eduTypeId = entity2.id;

        //                #endregion

        //                if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    #region  获取民族层次ID

        //                    nationName = nationNames[i];
        //                    var entity11 = yunEntities.YD_Sts_Nation.FirstOrDefault(u => u.y_name == nationName);
        //                    if (entity11 != null)
        //                        nationId = entity11.id;

        //                    #endregion

        //                    #region  获取政治面貌ID

        //                    politicsName = politicsNames[i];
        //                    var entity7 = yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.y_name == politicsName);
        //                    if (entity7 != null)
        //                        politicsId = entity7.id;

        //                    #endregion
        //                }

        //                #region 获取专业库id

        //                majorLibName = majorLibs[i].Trim();
        //                var entity3 = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == majorLibName);
        //                if (entity3 == null)
        //                {
        //                    var myname = majorLibName.Replace("专业", "");
        //                    if (myname.Length <= 2)
        //                    {
        //                        var entityList = yunEntities.YD_Edu_MajorLibrary.Where(u => u.y_name.Contains(myname));
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            majorNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                majorNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var sql = "";
        //                        for (var j = 0; j < myname.Length - 1; j++)
        //                        {
        //                            sql += " y_name like '%" + myname.Substring(j, 2) + "%' ";
        //                            if (j + 1 < myname.Length - 1)
        //                            {
        //                                sql += " or ";
        //                            }
        //                        }
        //                        var entityList =
        //                            yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
        //                                "select * from YD_Edu_MajorLibrary where " + sql).ToList();
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            majorNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                majorNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    majorLibId = entity3.id;
        //                }


        //                #endregion

        //                #region 获取函授站id

        //                subSchoolName = subSchools[i].Trim().Replace(" ", "");
        //                var entity5 = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.y_name == subSchoolName);
        //                if (entity5 == null)
        //                {
        //                    var myname = subSchoolName.Replace("函授站", "").Replace("函授", "");
        //                    if (myname.Length <= 2)
        //                    {
        //                        var myname1 = myname;
        //                        var entityList = yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(myname1));
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            subNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                subNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var sql = "";
        //                        for (var j = 0; j < myname.Length - 1; j++)
        //                        {
        //                            sql += " y_name like '%" + myname.Substring(j, 2) + "%' ";
        //                            if (j + 1 < myname.Length - 1)
        //                            {
        //                                sql += " or ";
        //                            }
        //                        }
        //                        var entityList =
        //                            yunEntities.Database.SqlQuery<YD_Sys_SubSchool>("select * from YD_Sys_SubSchool where " + sql).ToList();
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            subNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                subNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    subSchoolId = entity5.id;
        //                }
        //                #endregion



        //                #region 获取专业id

        //                if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0)
        //                {
        //                    majorId = GetMajorIds(majorLibId, eduTypeId, stuTypeId);
        //                }
        //                else
        //                {
        //                    majorId = -1;
        //                }

        //                #endregion

        //                stuNum = stuNums[i];
        //                examNum = examNums[i];
        //                tel = tels[i];
        //                address = addresss[i];

        //                #region 获取学籍状态id

        //                stuStateName = stuStates[i];
        //                var entity12 = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == stuStateName);
        //                if (entity12 != null)
        //                    stuStateId = entity12.id;

        //                #endregion

        //                if (KeyValue.NationAndPolitics.Contains(schoolname) && KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "" && nationId != -1 &&
        //                        politicsId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 && birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "" && nationId != -1 && politicsId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else if (KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "")
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                else
        //                {
        //                    if (stuTypeId != -1 && eduTypeId != -1 && majorLibId != -1 && majorId != -1 && sex != -1 &&
        //                        birthday != DateTime.MaxValue &&
        //                        year != -1 && year != -1 && stuStateId != -1 && name != "" && subSchoolId != -1)
        //                    {
        //                        isOk = (int)YesOrNo.Yes;
        //                    }
        //                    else
        //                    {
        //                        isOk = (int)YesOrNo.No;
        //                    }
        //                }
        //                teachPlan.y_name = name;
        //                teachPlan.y_sex = sex;
        //                teachPlan.y_sexName = sexName;
        //                teachPlan.y_inYear = year;
        //                teachPlan.y_majorId = majorId;
        //                teachPlan.y_cardId = cardId;
        //                teachPlan.y_majorLibId = majorLibId;
        //                teachPlan.y_majorLibName = majorLibName;
        //                teachPlan.y_majorId = majorId;
        //                teachPlan.y_stuTypeId = stuTypeId;
        //                teachPlan.y_stuTypeName = stuTypeName;
        //                teachPlan.y_eduTypeId = eduTypeId;
        //                teachPlan.y_eduTypeName = eduTypeName;
        //                teachPlan.y_stuNum = stuNum;
        //                teachPlan.y_examNum = examNum;
        //                teachPlan.y_tel = tel;
        //                teachPlan.y_address = address;
        //                teachPlan.y_stuStateId = stuStateId;
        //                teachPlan.y_stuStateName = stuStateName;
        //                teachPlan.y_isOk = isOk;
        //                teachPlan.y_birthday = birthday;
        //                teachPlan.y_majorNameMatch = majorNameMatch;

        //                teachPlan.y_graduationschool = graduationschool;
        //                teachPlan.y_graduationdata = graduationdata;
        //                teachPlan.y_registrationNum = registrationNum;
        //                teachPlan.y_postalcode = postalcode;
        //                teachPlan.y_foreignLanguageName = foreignLanguageName;
        //                teachPlan.y_recruitTypeName = recruitTypeName;
        //                teachPlan.y_professionTypeName = professionTypeName;
        //                teachPlan.y_cultureExtentName = cultureExtentName;
        //                teachPlan.y_examFeatureName = examFeatureName;

        //                teachPlan.y_foreignLanguageId = foreignLanguageId;
        //                teachPlan.y_recruitTypeId = recruitTypeId;
        //                teachPlan.y_professionTypeId = professionTypeId;
        //                teachPlan.y_cultureExtentId = cultureExtentId;
        //                teachPlan.y_examFeatureId = examFeatureId;
        //                if (!KeyValue.SubschoolAndSchool.Contains(schoolname))
        //                {
        //                    teachPlan.y_subSchoolId = subSchoolId;
        //                    teachPlan.y_subSchoolName = subSchoolName;
        //                    teachPlan.y_subNameMatch = subNameMatch;
        //                }
        //                if (KeyValue.NationAndPolitics.Contains(schoolname))
        //                {
        //                    teachPlan.y_nationId = nationId;
        //                    teachPlan.y_politicsId = politicsId;
        //                    teachPlan.y_nationName = nationName;
        //                    teachPlan.y_politicsName = politicsName;
        //                }
        //                yunEntities.Entry(teachPlan).State = EntityState.Modified;
        //            }
        //        }
        //        var t = yunEntities.SaveChanges();
        //        return Content("ok");

        //    }
        //}

        /// <summary>
        /// 提交函授站更新的学生临时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateSubVerify()
        {
            var names = Request["name"].Split(new[] { "<>" }, StringSplitOptions.None);
            var cardIds = Request["cardId"].Split(new[] { "<>" }, StringSplitOptions.None);
            var examNums = Request["examNum"].Split(new[] { "<>" }, StringSplitOptions.None);
            var ids = Request["id"].Split(new[] { "<>" }, StringSplitOptions.None);
            string name = "";
            string cardId = "";
            string cardidtemp = "";
            string examNum = "";
            int isOk = (int)YesOrNo.No;
            int id = 0;
            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < ids.Count(); i++)
                {
                    name = "";
                    cardId = "";
                    cardidtemp = "";
                    examNum = "";

                    id = Convert.ToInt32(ids[i]);
                    var teachPlan = yunEntities.YD_Sts_SubStuTemp.FirstOrDefault(u => u.id == id);
                    if (teachPlan != null)
                    {
                        name = names[i];
                        examNum = examNums[i];

                        #region  身份证号验证是否重复

                        cardId = cardIds[i];
                        var temp = yunEntities.YD_Sts_SubStuTemp.FirstOrDefault(u => u.y_cardId == cardId);
                        var substu =
                            yunEntities.YD_Sts_SubSchoolStuInfo.FirstOrDefault(
                                u => u.y_cardId == cardId && u.y_isdel == 1);
                        if (temp != null || substu != null)
                        {
                            cardidtemp = "-1";
                        }

                        #endregion

                        if (cardId != "" && cardidtemp != "-1")
                        {
                            isOk = (int)YesOrNo.Yes;
                        }
                        else
                        {
                            isOk = (int)YesOrNo.No;
                        }
                        teachPlan.y_name = name;
                        teachPlan.y_examNum = examNum;
                        teachPlan.y_cardId = cardId;
                        teachPlan.y_cardIdTemp = cardidtemp;
                        teachPlan.y_isOk = isOk;
                        yunEntities.Entry(teachPlan).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",提交函授站更新的学生临时数据,修改函授站学生临时数据,ID:" + teachPlan.id + ",方法:UpdateSubVerify");
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
        public ActionResult UploadTrueSubSchoolStu()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/SubSchoolStuInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int)YesOrNo.Yes;
                var scoreList = yunEntities.YD_Sts_SubStuTemp.Where(u => u.y_isOk == isOk).ToList();
                var scoreListTemp = new List<YD_Sts_SubSchoolStuInfo>();
                var score = new YD_Sts_SubSchoolStuInfo();
                for (var i = 0; i < scoreList.Count; i++)
                {
                    score = new YD_Sts_SubSchoolStuInfo
                    {
                        y_name = scoreList[i].y_name,
                        y_year = scoreList[i].y_year,
                        y_examNum = scoreList[i].y_examNum,
                        y_cardId = scoreList[i].y_cardId,
                        y_subSchoolId = Convert.ToInt32(scoreList[i].y_subSchoolId),
                        y_isdel = (int)YesOrNo.No,
                        y_hide = 1
                    };
                    scoreListTemp.Add(score);

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",将验证无误的数据进行导入,添加考生管理信息,身份证号:" + score.y_cardId + ",方法:UploadTrueSubSchoolStu");
                }
                yunEntities.Configuration.AutoDetectChangesEnabled = false;
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                yunEntities.Set<YD_Sts_SubSchoolStuInfo>().AddRange(scoreListTemp);

                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    var stulist = yunEntities.YD_Sts_StuInfo.Where(u => u.y_isdel == 1).AsQueryable();
                    for (var i = 0; i < scoreList.Count; i++)
                    {
                        var cardId = scoreList[i].y_cardId;
                        var subschoolId = scoreList[i].y_subSchoolId;

                        var substu = yunEntities.YD_Sts_SubSchoolStuInfo.FirstOrDefault(u => u.y_cardId == cardId);
                        var stulistone = stulist.FirstOrDefault(u => u.y_cardId == cardId); //学校导入表
                        if (stulistone != null && (substu != null && stulistone.y_subSchoolId != null))
                        {
                            var subname = scoreList[i].y_name;
                            var subexem = scoreList[i].y_examNum;
                            var sub =
                                yunEntities.YD_Sts_SubSchoolStuInfo.FirstOrDefault(
                                    u => u.y_cardId == cardId && u.y_name == subname && u.y_examNum == subexem);
                            if (sub != null)
                            {
                                sub.y_hide = 0;
                            }
                            yunEntities.Entry(sub).State = EntityState.Modified;
                        }
                        else if (stulistone != null && stulistone.y_subSchoolId == -1)
                        {
                            stulistone.y_subSchoolId = subschoolId;
                            if (substu != null)
                            {
                                substu.y_hide = 0;
                                yunEntities.Entry(stulistone).State = EntityState.Modified;
                                yunEntities.Entry(substu).State = EntityState.Modified;
                            }
                        }
                    }
                    yunEntities.SaveChanges();
                    return
                        Content("<script type='text/javascript'>alert('导入成功,导入" + r +
                                "条数据');window.location.href='/Student/SubSchoolStuInfo';</script >");
                }
                else
                {
                    return Content("导入失败");
                }
                //return Redirect("SubSchoolStuInfo/?id=1");
            }
        }

        #region 学籍异动模块

        #region 学籍异动申请视图

        /// <summary>
        /// 学籍异动申请视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuInfoChangeApply(int? id)
        {
            using (var yunEntities = new IYunEntities())
            {
                if (!id.HasValue)
                {
                    return RedirectToAction("StudentInfo");
                }
                var student =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Sts_Nation)
                        .Include(u => u.YD_Edu_Major)
                        .First(u => u.id == id);
                if (student == null)
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewData["student"] = student;
            }
            return View();
        }

        #endregion

        #region 学籍异动申请AJAX

        /// <summary>
        /// 学籍异动申请AJAX
        /// </summary>
        /// <param name="stra">学籍异动对象</param>
        /// <returns>处理结果json</returns>
        public string StuStrangeApply(YD_Sts_StuStrange stra)
        {
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = _stuStrangeDal.AddExtended(stra, yunEntities);
                if (msg == "ok")
                {
                    re["msg"] = "申请成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        #endregion

        #region 学籍异动批量待处理视图

        public ActionResult StudentInfoChangeAll(int id = 1)
        {
            if (!IsLogin())
            {
                return View("../AdminBase/Login");
            }
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.WaitApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No)
                        .OrderByDescending(u => u.id);
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var strangeTypeName = Request["StrangeType"];
                var namenumcard = Request["namenumcard"];


                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuNum.Contains(namenumcard) || u.y_cardId.Contains(namenumcard)
                                           || u.studentName.Contains(namenumcard) || u.y_examNum == namenumcard);
                }

                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("AllStrangeList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 学籍异动待处理视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StudentInfoChange(int id = 1)
        {
            if (!IsLogin())
            {
                return View("../AdminBase/Login");
            }
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.WaitApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).OrderByDescending(u => u.id);
                var name = Request["name"];
                var stuState = Request["StuState"];
                var card = Request["card"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.studentName.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(card))
                {
                    list = list.Where(u => u.y_cardId.Contains(card));
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("StrangeList", model);
                return View(model);
            }
        }

        #endregion

        /// <summary>
        /// 学籍异动待处理下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStudentInfoChange()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfoChangeAll");
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

            var StuState = Request["StuState"];
            var y_inYear = Request["EnrollYear"];
            var schoolName = Request["SubSchool2"];
            var majorName = Request["MajorLibrary2"];
            var stuTypeName = Request["StuType"];
            var strangeTypeName = Request["StrangeType"];
            var namenumcard = Request["namenumcard"];
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.WaitApprova;
                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.OrderByDescending(u => u.id)
                        .Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.studentName.Contains(namenumcard) || u.y_cardId.Contains(namenumcard) ||
                                u.y_stuNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(StuState) && !StuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(StuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(y_inYear) && !y_inYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(y_inYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(schoolName) && !schoolName.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(schoolName);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorName) && !majorName.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorName);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuTypeName) && !stuTypeName.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuTypeName);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }

                var li = list.ToList();
                List<StudentInfoChangeB> stlist = new List<StudentInfoChangeB>();

                for (var i = 0; i < li.Count; i++)
                {
                    var st = new StudentInfoChangeB();


                    st.StudentName = li[i].studentName;
                    st.y_sex = li[i].y_sex == 0 ? "男" : "女";
                    st.y_cardId = li[i].y_cardId;
                    st.y_stuNum = li[i].y_stuNum;
                    st.y_exNum = li[i].y_examNum;
                    st.schoolName = li[i].schoolName;
                    st.y_inYear = li[i].y_inYear;

                    st.majorName = li[i].majorLibraryName;//当前专业
                    st.y_eduTypeName = li[i].eduTypeName;
                    st.stuTypeName = li[i].stuTypeName;

                    st.y_tel = li[i].y_tel;
                    st.y_address = li[i].y_address;
                    st.strangeTypeName = li[i].strangeTypeName;
                    st.y_applyTime = li[i].y_applyTime;
                    if (li[i].y_contentAName == null || li[i].y_contentAName == "")
                    {

                    }
                    else
                    {
                        var a = "";

                        if (li[i].y_contentAName.Contains("专升本"))
                        {
                            a = li[i].y_contentAName.Replace("专升本", "");
                            st.y_contentAName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后



                        }
                        if (li[i].y_contentAName.Contains("高起专"))
                        {
                            a = li[i].y_contentAName.Replace("高起专", "");
                            st.y_contentAName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }
                        if (li[i].y_contentAName.Contains("高起本"))
                        {
                            a = li[i].y_contentAName.Replace("高起本", "");
                            st.y_contentAName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }











                        //st.y_contentAName = li[i].y_contentAName.Split(' ').Count() > 0 ? li[i].y_contentAName.Split(' ')[0] : li[i].y_contentAName;//专出后
                        //st.y_contentAName2 = li[i].y_contentAName.Split(' ').Count() > 1 ? li[i].y_contentAName.Split(' ')[1] : li[i].y_contentAName;
                        //st.y_contentAName3 = li[i].y_contentAName.Split(' ').Count() > 2 ? li[i].y_contentAName.Split(' ')[2] : li[i].y_contentAName;
                    }

                    if (li[i].y_contentBName == null || li[i].y_contentBName == "")
                    {

                    }
                    else
                    {

                        var a = "";

                        if (li[i].y_contentBName.Contains("专升本"))
                        {
                            a = li[i].y_contentBName.Replace("专升本", "");
                            st.y_contentBName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后



                        }
                        if (li[i].y_contentBName.Contains("高起专"))
                        {
                            a = li[i].y_contentBName.Replace("高起专", "");
                            st.y_contentBName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后

                        }
                        if (li[i].y_contentBName.Contains("高起本"))
                        {
                            a = li[i].y_contentBName.Replace("高起本", "");
                            st.y_contentBName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后
                        }





                        //    st.y_contentBName = li[i].y_contentBName.Split(' ').Count() > 0 ? li[i].y_contentBName.Split(' ')[0] : li[i].y_contentBName;//转出前
                        //st.y_contentBName2 = li[i].y_contentBName.Split(' ').Count() > 1 ? li[i].y_contentBName.Split(' ')[1] : li[i].y_contentBName;
                        //st.y_contentBName3 = li[i].y_contentBName.Split(' ').Count() > 2 ? li[i].y_contentBName.Split(' ')[2] : li[i].y_contentBName;
                    }




                    stlist.Add(st);




                };


                var model =
                    FileHelper.ToDataTable(
                        stlist);
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学籍异动待审批表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/学籍异动待审批表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"StudentName", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证号"},
                        {"y_stuNum", "学号"},
                        {"y_exNum", "考生号"},
                        {"schoolName", "函授站"},
                        {"y_inYear", "入学年份"},
                        {"majorName", "专业"},
                          {"y_eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"strangeTypeName", "异动类型"},
                        {"y_applyTime", "申请时间"},
                        {"y_approvalStatus", "状态"},
                        {"y_contentBName", "转出前"},
                         {"y_contentBName2", "转出前层次"},
                          {"y_contentBName3", "转出前形式"},
                        {"y_contentAName", "转出后"},
                          {"y_contentAName2", "转出后层次"},
                           {"y_contentAName3", "转出后形式"}

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

        #region 学籍异动已通过视图

        /// <summary>
        /// 学籍异动已通过视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StudentInfoChangeB(int id = 1)
        {
            if (!IsLogin())
            {
                return View("../AdminBase/Login");
            }
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No)
                        .OrderByDescending(u => u.y_approvalTime);
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var strangeTypeName = Request["StrangeType"];
                var namenumcard = Request["namenumcard"];
                var startTime = Request["startTime"];
                var endTime = Request["endTime"];
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuNum.Contains(namenumcard) || u.y_cardId.Contains(namenumcard)
                                           || u.studentName.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    var startTimeN = Convert.ToDateTime(startTime);
                    list = list.Where(u => u.y_approvalTime >= startTimeN);
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    var endTimeN = Convert.ToDateTime(endTime);
                    list = list.Where(u => u.y_approvalTime <= endTimeN);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15);
                if (Request.IsAjaxRequest())
                    return PartialView("StrangeList", model);
                return View(model);
            }
        }

        #endregion

        /// <summary>
        /// 学籍异动已通过下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStudentInfoChangeB()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfoChangeAll");
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

            var StuState = Request["StuState"];
            var y_inYear = Request["EnrollYear"];
            var schoolName = Request["SubSchool2"];
            var majorName = Request["MajorLibrary2"];
            var stuTypeName = Request["StuType"];
            var strangeTypeName = Request["StrangeType"];
            var namenumcard = Request["namenumcard"];

            var startTime = Request["startTime"];
            var endTime = Request["endTime"];

            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;
                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.OrderByDescending(u => u.y_approvalTime)
                        .Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_cardId.Contains(namenumcard) || u.y_stuNum == namenumcard ||
                                u.studentName.Contains(namenumcard));
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    var startTimeN = Convert.ToDateTime(startTime);
                    list = list.Where(u => u.y_approvalTime >= startTimeN);
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    var endTimeN = Convert.ToDateTime(endTime);
                    list = list.Where(u => u.y_approvalTime <= endTimeN);
                }
                if (!string.IsNullOrWhiteSpace(StuState) && !StuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(StuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(y_inYear) && !y_inYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(y_inYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(schoolName) && !schoolName.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(schoolName);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorName) && !majorName.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorName);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuTypeName) && !stuTypeName.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuTypeName);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }



                var li = list.ToList();
                List<StudentInfoChangeB> stlist = new List<StudentInfoChangeB>();

                for (var i = 0; i < li.Count; i++)
                {
                    var st = new StudentInfoChangeB();


                    st.StudentName = li[i].studentName;
                    st.y_sex = li[i].y_sex == 0 ? "男" : "女";
                    st.y_cardId = li[i].y_cardId;
                    st.y_stuNum = li[i].y_stuNum;
                    st.y_exNum = li[i].y_examNum;
                    st.schoolName = li[i].schoolName;
                    st.y_inYear = li[i].y_inYear;

                    st.majorName = li[i].majorLibraryName;//当前专业
                    st.y_eduTypeName = li[i].eduTypeName;
                    st.stuTypeName = li[i].stuTypeName;

                    st.y_tel = li[i].y_tel;
                    st.y_address = li[i].y_address;
                    st.strangeTypeName = li[i].strangeTypeName;
                    st.y_applyTime = li[i].y_applyTime;
                    if (li[i].y_contentAName == null || li[i].y_contentAName == "")
                    {

                    }
                    else
                    {
                        var a = "";

                        if (li[i].y_contentAName.Contains("专升本"))
                        {
                            a = li[i].y_contentAName.Replace("专升本", "");
                            st.y_contentAName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后



                        }
                        if (li[i].y_contentAName.Contains("高起专"))
                        {
                            a = li[i].y_contentAName.Replace("高起专", "");
                            st.y_contentAName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }
                        if (li[i].y_contentAName.Contains("高起本"))
                        {
                            a = li[i].y_contentAName.Replace("高起本", "");
                            st.y_contentAName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }











                        //st.y_contentAName = li[i].y_contentAName.Split(' ').Count() > 0 ? li[i].y_contentAName.Split(' ')[0] : li[i].y_contentAName;//专出后
                        //st.y_contentAName2 = li[i].y_contentAName.Split(' ').Count() > 1 ? li[i].y_contentAName.Split(' ')[1] : li[i].y_contentAName;
                        //st.y_contentAName3 = li[i].y_contentAName.Split(' ').Count() > 2 ? li[i].y_contentAName.Split(' ')[2] : li[i].y_contentAName;
                    }

                    if (li[i].y_contentBName == null || li[i].y_contentBName == "")
                    {

                    }
                    else
                    {

                        var a = "";

                        if (li[i].y_contentBName.Contains("专升本"))
                        {
                            a = li[i].y_contentBName.Replace("专升本", "");
                            st.y_contentBName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后



                        }
                        if (li[i].y_contentBName.Contains("高起专"))
                        {
                            a = li[i].y_contentBName.Replace("高起专", "");
                            st.y_contentBName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后

                        }
                        if (li[i].y_contentBName.Contains("高起本"))
                        {
                            a = li[i].y_contentBName.Replace("高起本", "");
                            st.y_contentBName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后
                        }





                        //    st.y_contentBName = li[i].y_contentBName.Split(' ').Count() > 0 ? li[i].y_contentBName.Split(' ')[0] : li[i].y_contentBName;//转出前
                        //st.y_contentBName2 = li[i].y_contentBName.Split(' ').Count() > 1 ? li[i].y_contentBName.Split(' ')[1] : li[i].y_contentBName;
                        //st.y_contentBName3 = li[i].y_contentBName.Split(' ').Count() > 2 ? li[i].y_contentBName.Split(' ')[2] : li[i].y_contentBName;
                    }




                    stlist.Add(st);




                };


                var model =
                    FileHelper.ToDataTable(
                        stlist);

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学籍异动已通过表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/学籍异动已通过表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"StudentName", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证号"},
                        {"y_stuNum", "学号"},
                        {"y_exNum", "考生号"},
                        {"schoolName", "函授站"},
                        {"y_inYear", "入学年份"},
                        {"majorName", "专业"},
                         {"y_eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"strangeTypeName", "异动类型"},
                        {"y_applyTime", "申请时间"},
                        {"y_approvalTime", "审核时间"},
                        {"y_approvalStatus", "状态"},
                         {"y_contentBName", "转出前"},
                         {"y_contentBName2", "转出前层次"},
                          {"y_contentBName3", "转出前形式"},
                        {"y_contentAName", "转出后"},
                          {"y_contentAName2", "转出后层次"},
                           {"y_contentAName3", "转出后形式"}

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

        #region 学籍异动已退回视图

        /// <summary>
        /// 学籍异动已退回视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StudentInfoChangeC(int id = 1)
        {
            if (!IsLogin())
            {
                return View("../AdminBase/Login");
            }

            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.Return;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No)
                        .OrderByDescending(u => u.id);
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var strangeTypeName = Request["StrangeType"];
                var namenumcard = Request["namenumcard"];
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_cardId.Contains(namenumcard) || u.studentName.Contains(namenumcard)
                                           || u.y_stuNum.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 15);

                if (Request.IsAjaxRequest())
                    return PartialView("StrangeList", model);
                return View(model);
            }
        }

        #endregion

        /// <summary>
        /// 学籍异动已拒绝下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStudentInfoChangeC()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StudentInfoChangeAll");
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

            var StuState = Request["StuState"];
            var y_inYear = Request["EnrollYear"];
            var schoolName = Request["SubSchool2"];
            var majorName = Request["MajorLibrary2"];
            var stuTypeName = Request["StuType"];
            var strangeTypeName = Request["StrangeType"];
            var y_tel = Request["tel"];
            var namenumcard = Request["namenumcard"];
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.Return;
                IQueryable<VW_Strange> list =
                    yunEntities.VW_Strange.OrderByDescending(u => u.id)
                        .Where(u => u.y_approvalStatus == status && u.y_straisdel == (int)YesOrNo.No);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_cardId.Contains(namenumcard) || u.y_stuNum == namenumcard ||
                                u.studentName.Contains(namenumcard));
                }
                if (!string.IsNullOrWhiteSpace(StuState) && !StuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(StuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                if (!string.IsNullOrWhiteSpace(y_inYear) && !y_inYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(y_inYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(schoolName) && !schoolName.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(schoolName);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorName) && !majorName.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorName);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(strangeTypeName) && !strangeTypeName.Equals("0"))
                {
                    var strangeTypeint = Convert.ToInt32(strangeTypeName);
                    list = list.Where(u => u.y_strangeType == strangeTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuTypeName) && !stuTypeName.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuTypeName);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                var li = list.ToList();
                List<StudentInfoChangeB> stlist = new List<StudentInfoChangeB>();

                for (var i = 0; i < li.Count; i++)
                {
                    var st = new StudentInfoChangeB();


                    st.StudentName = li[i].studentName;
                    st.y_sex = li[i].y_sex == 0 ? "男" : "女";
                    st.y_cardId = li[i].y_cardId;
                    st.y_stuNum = li[i].y_stuNum;
                    st.y_exNum = li[i].y_examNum;
                    st.schoolName = li[i].schoolName;
                    st.y_inYear = li[i].y_inYear;

                    st.majorName = li[i].majorLibraryName;//当前专业
                    st.y_eduTypeName = li[i].eduTypeName;
                    st.stuTypeName = li[i].stuTypeName;

                    st.y_tel = li[i].y_tel;
                    st.y_address = li[i].y_address;
                    st.strangeTypeName = li[i].strangeTypeName;
                    st.y_applyTime = li[i].y_applyTime;
                    if (li[i].y_contentAName == null || li[i].y_contentAName == "")
                    {

                    }
                    else
                    {
                        var a = "";

                        if (li[i].y_contentAName.Contains("专升本"))
                        {
                            a = li[i].y_contentAName.Replace("专升本", "");
                            st.y_contentAName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后



                        }
                        if (li[i].y_contentAName.Contains("高起专"))
                        {
                            a = li[i].y_contentAName.Replace("高起专", "");
                            st.y_contentAName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }
                        if (li[i].y_contentAName.Contains("高起本"))
                        {
                            a = li[i].y_contentAName.Replace("高起本", "");
                            st.y_contentAName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentAName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentAName3 = "业余";
                            }

                            st.y_contentAName = a;//专出后
                        }











                        //st.y_contentAName = li[i].y_contentAName.Split(' ').Count() > 0 ? li[i].y_contentAName.Split(' ')[0] : li[i].y_contentAName;//专出后
                        //st.y_contentAName2 = li[i].y_contentAName.Split(' ').Count() > 1 ? li[i].y_contentAName.Split(' ')[1] : li[i].y_contentAName;
                        //st.y_contentAName3 = li[i].y_contentAName.Split(' ').Count() > 2 ? li[i].y_contentAName.Split(' ')[2] : li[i].y_contentAName;
                    }

                    if (li[i].y_contentBName == null || li[i].y_contentBName == "")
                    {

                    }
                    else
                    {

                        var a = "";

                        if (li[i].y_contentBName.Contains("专升本"))
                        {
                            a = li[i].y_contentBName.Replace("专升本", "");
                            st.y_contentBName2 = "专升本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后



                        }
                        if (li[i].y_contentBName.Contains("高起专"))
                        {
                            a = li[i].y_contentBName.Replace("高起专", "");
                            st.y_contentBName2 = "高起专";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后

                        }
                        if (li[i].y_contentBName.Contains("高起本"))
                        {
                            a = li[i].y_contentBName.Replace("高起本", "");
                            st.y_contentBName2 = "高起本";

                            if (a.Contains("函授"))
                            {
                                a = a.Replace("函授", "");
                                st.y_contentBName3 = "函授";
                            }
                            if (a.Contains("业余"))
                            {
                                a = a.Replace("业余", "");
                                st.y_contentBName3 = "业余";
                            }

                            st.y_contentBName = a;//专出后
                        }





                        //    st.y_contentBName = li[i].y_contentBName.Split(' ').Count() > 0 ? li[i].y_contentBName.Split(' ')[0] : li[i].y_contentBName;//转出前
                        //st.y_contentBName2 = li[i].y_contentBName.Split(' ').Count() > 1 ? li[i].y_contentBName.Split(' ')[1] : li[i].y_contentBName;
                        //st.y_contentBName3 = li[i].y_contentBName.Split(' ').Count() > 2 ? li[i].y_contentBName.Split(' ')[2] : li[i].y_contentBName;
                    }




                    stlist.Add(st);




                };


                var model =
                    FileHelper.ToDataTable(
                        stlist);

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学籍异动已拒绝表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/学籍异动已j拒绝表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"StudentName", "姓名"},
                        {"y_sex", "性别"},
                        {"y_cardId", "身份证号"},
                        {"y_stuNum", "学号"},
                        {"y_exNum", "考生号"},
                        {"schoolName", "函授站"},
                        {"y_inYear", "入学年份"},
                        {"majorName", "专业"},
                          {"y_eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"y_tel", "电话"},
                        {"y_address", "地址"},
                        {"strangeTypeName", "异动类型"},
                        {"y_applyTime", "申请时间"},
                        {"y_approvalTime", "审核时间"},
                        {"y_approvalStatus", "状态"},
                         {"y_contentBName", "转出前"},
                         {"y_contentBName2", "转出前层次"},
                          {"y_contentBName3", "转出前形式"},
                        {"y_contentAName", "转出后"},
                          {"y_contentAName2", "转出后层次"},
                           {"y_contentAName3", "转出后形式"}

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

        #region 学籍异动详情视图

        /// <summary>
        /// 学籍异动申请视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuInfoChangeDetail(int? id)
        {
            if (!IsLogin())
            {
                return View("../AdminBase/Login");
            }
            using (var yunEntities = new IYunEntities())
            {
                if (!id.HasValue)
                {
                    return RedirectToAction("StudentInfoChangeAll");
                }
                var strange = yunEntities.VW_Strange.SingleOrDefault(u => u.id == id.Value);
                if (strange == null)
                {
                    return RedirectToAction("StudentInfoChangeAll");
                }
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewData["strange"] = strange;
                ViewBag.adminrole = YdAdminRoleId;
                if (strange.y_approvalStatus == (int)ApprovaState.WaitApprova)
                {
                    return View("StuInfoChangeDetail");
                }
                else if (strange.y_approvalStatus == (int)ApprovaState.HadApprova)
                {
                    return View("StuInfoChangeDetB");
                }
                else if (strange.y_approvalStatus == (int)ApprovaState.Return)
                {
                    return View("StuInfoChangeDetC");
                }
            }
            return RedirectToAction("StudentInfoChangeAll");
        }


        /// <summary>
        /// 打印--基本信息修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrintStuStrange(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfoChangeAll");
            }
            using (var yunEntities = new IYunEntities())
            {
                var strange = yunEntities.VW_Strange.SingleOrDefault(u => u.id == id.Value);
                var applyAdmin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == strange.y_applyAdmin);
                ViewBag.applyAdmin = "";
                if (applyAdmin != null)
                {
                    ViewBag.applyAdmin = applyAdmin.y_realName;
                }
                ViewData["strange"] = strange;
                return View();
            }
        }

        /// <summary>
        /// 打印--退学，休学
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrintStrangeStop(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfoChangeAll");
            }
            using (var yunEntities = new IYunEntities())
            {
                var strange = yunEntities.VW_Strange.SingleOrDefault(u => u.id == id.Value);
                var applyAdmin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == strange.y_applyAdmin);
                ViewBag.applyAdmin = "";
                if (applyAdmin != null)
                {
                    ViewBag.applyAdmin = applyAdmin.y_realName;
                }
                ViewData["strange"] = strange;
                return View();
            }
        }

        /// <summary>
        /// 打印--转专业，转函授站
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrintStrangeMajororSub(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("StudentInfoChangeAll");
            }
            using (var yunEntities = new IYunEntities())
            {
                var strange = yunEntities.VW_Strange.SingleOrDefault(u => u.id == id.Value);
                var applyAdmin = yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.id == strange.y_applyAdmin);
                ViewBag.applyAdmin = "";
                if (applyAdmin != null)
                {
                    ViewBag.applyAdmin = applyAdmin.y_realName;
                }
                ViewData["strange"] = strange;
                return View();
            }
        }

        #endregion

        #region 学籍异动审批AJAX

        /// <summary>
        /// 学籍异动审批AJAX
        /// </summary>
        /// <param name="stra">学籍异动对象</param>
        /// <returns>处理结果json</returns>
        public string StuStrangeApprova(YD_Sts_StuStrange stra)
        {
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = _stuStrangeDal.ApprovaStra(stra, yunEntities);
                if (msg == "ok")
                {
                    re["msg"] = "审批成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        /// <summary>
        /// 学籍异动批量审批AJAX
        /// </summary>
        /// <param name="stra">学籍异动对象</param>
        /// <returns>处理结果json</returns>
        public string StuStrangeApprovaAll()
        {
            var id = Request["ids"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = "";
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stustrange = yunEntities.YD_Sts_StuStrange.FirstOrDefault(u => u.id == oid);
                    if (stustrange != null)
                    {
                        stustrange.y_approvalStatus = 2;
                        msg = _stuStrangeDal.ApprovaStra(stustrange, yunEntities);
                    }
                }
                if (msg == "ok")
                {
                    re["msg"] = "审批成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        /// <summary>
        /// 学籍异动批量审批AJAX--拒绝
        /// </summary>
        /// <param name="stra">学籍异动对象</param>
        /// <returns>处理结果json</returns>
        public string NoStuStrangeApprovaAll()
        {
            var id = Request["ids"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = "";
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stustrange = yunEntities.YD_Sts_StuStrange.FirstOrDefault(u => u.id == oid);
                    if (stustrange != null)
                    {
                        stustrange.y_approvalStatus = 3;
                        msg = _stuStrangeDal.ApprovaStra(stustrange, yunEntities);
                    }
                }
                if (msg == "ok")
                {
                    re["msg"] = "审批成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }

        }

        /// <summary>
        /// 学籍异动删除AJAX
        /// </summary>
        /// <param name="id">学籍异动对象id</param>
        /// <returns>处理结果json</returns>
        public string DeleteChange()
        {
            using (var yunEntities = new IYunEntities())
            {
                var msg = _stuStrangeDal.DeleteStrange(Request, yunEntities);

                return msg;
            }
        }

        /// <summary>
        /// 学籍异动删除AJAX
        /// </summary>
        /// <param name="id">学籍异动对象id</param>
        /// <returns>处理结果json</returns>
        public string BatchDeleteChange()
        {
            var id = Request["ids"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = "";
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stustrange = yunEntities.YD_Sts_StuStrange.FirstOrDefault(u => u.id == oid);
                    if (stustrange != null)
                    {
                        stustrange.y_isdel = (int)YesOrNo.Yes;
                        yunEntities.Entry(stustrange).State = EntityState.Modified;
                        yunEntities.SaveChanges();
                        LogHelper.DbLog(Convert.ToInt32(HttpContext.Session[KeyValue.Admin_id]),
                            HttpContext.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete,
                            "删除学籍异动,id：" + stustrange.id);
                        msg = "ok";
                    }
                }
                if (msg == "ok")
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
        /// 学籍异动审核错误驳回AJAX
        /// </summary>
        /// <param name="id">学籍异动对象id</param>
        /// <returns>处理结果json</returns>
        public string TurnChange()
        {
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var ids = Request["ids"];
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return "数据为空";
                }
                var msg = _stuStrangeDal.TurnApprovaStra(Request, yunEntities);
                if (msg == "ok")
                {
                    re["msg"] = "驳回成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        #endregion

        #endregion

        #region 学期注册模块

        #region 学期注册视图

        /// <summary>
        /// 缴费注册
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FeeManage(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/FeeManage");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var inYear = Request["inYear"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var term = Request["term"];
                var isFee = Request["isFee"];
                var isplanOK = Request["isplanOK"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                //var inyearint = Convert.ToInt32(inYear);
                var feeyearint = Convert.ToInt32(term);

                IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Fee_StuFeeTb).
                    Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Edu_Major.YD_Edu_EduType).OrderByDescending(u => u.y_subSchoolId).OrderByDescending(u => u.y_inYear)
                    .ThenByDescending(u => u.y_subSchoolId)
                    .ThenByDescending(u => u.id).
                    Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != null && u.y_inYear != xinshenyear
                    && u.y_stuStateId != 4 && u.y_stuStateId != 6 && u.y_studentType != 2 && u.YD_Edu_Major.y_stuYear>= feeyearint);

                var ss = list.ToList();
                //根据入学年份查询
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == (xinshenyear - 1));
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == yInYear);
                }

                var subSchool = Request["SubSchool"];
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
           

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard)
                                           || u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                var sss = list.ToList();
                ViewBag.admin = YdAdminRoleId;
          
                if (term == null)
                {
                    int feeyear = 2;
                    if(ConfigurationManager.AppSettings["SchoolName"] == "JXLG")
                    {
                        feeyear = 1;//江西理工希望能够在"在校生注册"页面注册第一学年
                    }
                    ViewBag.feeyear = feeyear;
                    list =
                        list.Where(
                            u =>
                                !u.YD_Fee_StuFeeTb.Any(K => K.y_feeYear == feeyear) ||
                                u.YD_Fee_StuFeeTb.FirstOrDefault(K => K.y_feeYear == feeyear).y_isCheckFee ==
                                (int)YesOrNo.No);
                }
                else
                {
                    ViewBag.feeyear = Convert.ToInt32(term);
                    list = list.Where(
                        u =>
                            !u.YD_Fee_StuFeeTb.Any(K => K.y_feeYear == feeyearint) ||
                            u.YD_Fee_StuFeeTb.FirstOrDefault(K => K.y_feeYear == feeyearint).y_isCheckFee ==
                            (int)YesOrNo.No);
                }
               


                if (!string.IsNullOrWhiteSpace(isplanOK) && isplanOK != "0")
                {
                    if (isplanOK == "1") //已处理  
                    {
                        list =
                            list.Where(
                                u =>
                                    u.YD_Fee_StuFeeTb.Any(
                                        a => a.y_planconductOK == (int)YesOrNo.No && a.y_feeYear == feeyearint) ||
                                    u.YD_Fee_StuFeeTb.Any(
                                        a => a.y_planconductOK == (int)YesOrNo.Yes && a.y_feeYear == feeyearint));
                    }
                    else if (isplanOK == "2") //未处理
                    {
                        list =
                            list.Where(
                                u =>
                                    !u.YD_Fee_StuFeeTb.Any(
                                        a =>
                                            //a.y_planconductOK != (int) YesOrNo.No &&
                                            //a.y_planconductOK != (int) YesOrNo.Yes && a.y_feeYear == feeyearint)
                                            a.y_feeYear == feeyearint &&
                                        a.y_stuId == u.id)
                                            );
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                ViewBag.admin = YdAdminRoleId;
                ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];
                //var newcount = list.Where(u => u.y_isUp == (int)YesOrNo.No).Count();
                //ViewBag.newcount = newcount;
                if (Request.IsAjaxRequest())
                    return PartialView("FeeManageList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 审核学生是否完善信息情况
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheckOld()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/FeeManage");
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
            var feeYearint = Request["feeYear"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                if (stu != null)
                {
                    var stuFee =
                        yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                    if (stuFee != null)
                    {
                        if (stuFee.y_planconductOK == 1 && stuFee.y_isUp == (int)YesOrNo.Yes)
                        {
                            return "已经提交不允许修改";
                        }
                        else
                        {
                            stuFee.y_planconductOK = 1;
                            stuFee.y_NoplanconductReason = "";
                            yunEntities.Entry(stuFee).State = EntityState.Modified;

                            LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",审核学生是否完善信息情况,修改缴费记录,ID:" + stuFee.id + ",方法:NewlyPlanCheckOld");
                        }
                    }
                    else
                    {
                        //添加缴费记录                                      
                        var Fee = new YD_Fee_StuFeeTb();
                        Fee.y_feeYear = yearint;
                        Fee.y_isUp = (int)YesOrNo.No;
                        Fee.y_isCheckFee = (int)YesOrNo.No;
                        Fee.y_stuId = oid;
                        Fee.y_needFee = 0;
                        Fee.y_needUpFee = 0;
                        Fee.y_planconductOK = 1;
                        Fee.y_registerYear = DateTime.Now.Year;
                        Fee.y_createtime = DateTime.Now;
                        Fee.y_NoplanconductReason = "";
                        yunEntities.Entry(Fee).State = EntityState.Added;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",审核学生是否完善信息情况,添加缴费记录,学生ID:" + Fee.y_stuId + ",缴费学年:" + Fee.y_feeYear + ",方法:NewlyPlanCheckOld");
                    }
                }
                else
                {
                    return "不存在该学生信息";
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
        /// 取消学生注册资格并填写原因
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheckOldNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/FeeManage");
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
            var feeYearint = Request["feeYear"];
            var PlanReason = Request["PlanReason"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(PlanReason))
            {
                return "原因必填";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                if (stuFee != null)
                {
                    if (stuFee.y_planconductOK == 1 && stuFee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    else
                    {
                        stuFee.y_planconductOK = 0;
                        stuFee.y_NoplanconductReason = PlanReason;
                        yunEntities.Entry(stuFee).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",取消学生注册资格并填写原因,修改缴费记录,ID:" + stuFee.id + ",方法:NewlyPlanCheckOldNo");
                    }
                }
                else //todo:要判断STU是否存在
                {
                    //添加缴费记录                                      
                    var Fee = new YD_Fee_StuFeeTb();
                    Fee.y_feeYear = yearint;
                    Fee.y_isUp = (int)YesOrNo.No;
                    Fee.y_isCheckFee = (int)YesOrNo.No;
                    Fee.y_stuId = oid;
                    Fee.y_needFee = 0;
                    Fee.y_needUpFee = 0;
                    Fee.y_planconductOK = 0;
                    Fee.y_registerYear = DateTime.Now.Year;
                    Fee.y_createtime = DateTime.Now;
                    Fee.y_NoplanconductReason = PlanReason;
                    yunEntities.Entry(Fee).State = EntityState.Added;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",取消学生注册资格并填写原因,添加缴费记录,学生ID:" + Fee.y_stuId + ",缴费学年:" + Fee.y_feeYear + ",方法:NewlyPlanCheckOldNo");
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
        /// 在线完善学生信息
        /// </summary>
        /// <returns></returns>
        public string NewlyPlanCheckwanshanOld()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/FeeManage");
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
            var feeYearint = Request["feeYear"];
            var tel = Request["tel"];
            var adress = Request["adress"];
            if (string.IsNullOrWhiteSpace(stuId))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYearint))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(tel))
            {
                return "电话必填";
            }
            if (string.IsNullOrWhiteSpace(adress))
            {
                return "地址必填";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(stuId);
                var yearint = Convert.ToInt32(feeYearint);
                var stuFee =
                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo)
                        .FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == yearint);
                if (stuFee != null)
                {
                    if (stuFee.y_planconductOK == 1 && stuFee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    else
                    {
                        stuFee.y_planconductOK = 1;
                        stuFee.YD_Sts_StuInfo.y_tel = tel;
                        stuFee.YD_Sts_StuInfo.y_address = adress;
                        stuFee.y_NoplanconductReason = "";
                        yunEntities.Entry(stuFee).State = EntityState.Modified;
                        yunEntities.Entry(stuFee.YD_Sts_StuInfo).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",在线完善学生信息,修改缴费记录,ID:" + stuFee.id + ",修改学生信息，ID:" + stuFee.YD_Sts_StuInfo.id + ",方法:NewlyPlanCheckwanshanOld");
                    }
                }
                else //todo:要判断STU是否存在
                {
                    //添加缴费记录                                      
                    var Fee = new YD_Fee_StuFeeTb();
                    Fee.y_feeYear = yearint;
                    Fee.y_isUp = (int)YesOrNo.No;
                    Fee.y_isCheckFee = (int)YesOrNo.No;
                    Fee.y_stuId = oid;
                    Fee.y_needFee = 0;
                    Fee.y_needUpFee = 0;
                    Fee.y_planconductOK = 1;
                    Fee.y_registerYear = DateTime.Now.Year;
                    Fee.y_createtime = DateTime.Now;
                    Fee.y_NoplanconductReason = "";
                    yunEntities.Entry(Fee).State = EntityState.Added;

                    var stu = yunEntities.YD_Sts_StuInfo.Find(oid);
                    stu.y_tel = tel;
                    stu.y_address = adress;

                    yunEntities.Entry(stu).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",在线完善学生信息,添加缴费记录,学生ID:" + Fee.y_stuId + ",缴费学年:" + Fee.y_feeYear + ",修改学生信息，ID:" + stuFee.YD_Sts_StuInfo.id + ",方法:NewlyPlanCheckwanshanOld");
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
        /// 撤销学生注册操作
        /// </summary>
        /// <returns></returns>
        public string RevocationPlanconductOld()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/FeeManage");
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
            var feeYear = Request["feeYear"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(feeYear))
            {
                return "未知错误";
            }
            using (var yunEntities = new IYunEntities())
            {
                var oid = Convert.ToInt32(id);
                var feeyearint = Convert.ToInt32(feeYear);
                var stuFee =
                    yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == oid && u.y_feeYear == feeyearint);
                if (stuFee != null)
                {
                    if (stuFee.y_isUp == (int)YesOrNo.Yes)
                    {
                        return "已经提交不允许修改";
                    }
                    stuFee.y_planconductOK = null;
                    stuFee.y_NoplanconductReason = "";
                    yunEntities.Entry(stuFee).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",撤销学生注册操作,修改缴费记录,ID:" + stuFee.id + ",方法:NewlyPlanCheckwanshanOld");
                }
                yunEntities.SaveChanges();
                return "ok";
            }
        }

        ///// <summary>
        ///// 批量处理该站点学生为预注册
        ///// </summary>
        ///// <returns></returns>
        //public string CheckboxtruePlanCheckOld()
        //{
        //     #region 权限验证
        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int) PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    var inyear = Request["inYear"];
        //    var feeyear = Request["term"];
        //    if (string.IsNullOrWhiteSpace(inyear))
        //    {
        //        return "未知错误";
        //    }
        //    if (string.IsNullOrWhiteSpace(feeyear))
        //    {
        //        return "未知错误";
        //    }
        //    using (var yunEntities = new IYunEntities())
        //    {
        //         var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
        //        if (sub != null)
        //        {
        //             var inyearint = Convert.ToInt32(inyear);
        //            var feeyearint = Convert.ToInt32(feeyear);
        //                var list =
        //                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
        //                      u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId &&u.YD_Sts_StuInfo.y_isdel!=0
        //                      && u.YD_Sts_StuInfo.y_inYear == inyearint && u.y_feeYear == feeyearint
        //                          && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK == null);
        //            if (list.Any())
        //            {
        //                foreach(var stu in list)
        //                {
        //                    stu.y_planconductOK = 1;
        //                    stu.y_NoplanconductReason = "";

        //                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量处理该站点学生为预注册,修改缴费记录,ID:" + stu.id + ",方法:CheckboxtruePlanCheckOld");
        //                }
        //                int w = yunEntities.SaveChanges();
        //                if (w > 0)
        //                {
        //                    return "ok";
        //                }
        //                else
        //                {
        //                    return "设置失败";
        //                }
        //            }
        //            else
        //            {
        //                return "没有学生可以设置";
        //            }    
        //        }
        //        else
        //        {
        //            return "该账号没有指定函授站";
        //        }
        //    }
        //}
        ///// <summary>
        ///// 批量取消该站点预注册学生
        ///// </summary>
        ///// <returns></returns>
        //public string CheckboxfalsePlanCheckOld()
        //{
        //     #region 权限验证
        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int) PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    var inyear = Request["inYear"];
        //    var feeyear = Request["term"];
        //    if (string.IsNullOrWhiteSpace(inyear))
        //    {
        //        return "未知错误";
        //    }
        //    if (string.IsNullOrWhiteSpace(feeyear))
        //    {
        //        return "未知错误";
        //    }
        //    using (var yunEntities = new IYunEntities())
        //    {
        //         var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
        //        if (sub != null)
        //        {
        //             var inyearint = Convert.ToInt32(inyear);
        //            var feeyearint = Convert.ToInt32(feeyear);
        //                var list =
        //                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
        //                      u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId 
        //                      && u.YD_Sts_StuInfo.y_inYear == inyearint && u.y_feeYear == feeyearint
        //                          && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK ==1).ToList();
        //            if (list.Any())
        //            {
        //                list.ForEach(u =>
        //                {
        //                    u.y_planconductOK =null;
        //                    u.y_NoplanconductReason = "";
        //                    yunEntities.Entry(u).State = EntityState.Modified;

        //                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",批量取消该站点预注册学生,修改缴费记录,ID:" + u.id + ",方法:CheckboxfalsePlanCheckOld");
        //                });
        //                int w = yunEntities.SaveChanges();
        //                if (w > 0)
        //                {
        //                    return "ok";
        //                }
        //                else
        //                {
        //                    return "设置失败";
        //                }
        //            }
        //            else
        //            {
        //                return "没有学生可以设置";
        //            }    
        //        }
        //        else
        //        {
        //            return "该账号没有指定函授站";
        //        }
        //    }
        //}



        /// <summary>
        /// 选择性提交学生缴费情况
        /// </summary>
        /// <returns></returns>
        public string PlanconductCheckSomeOld()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/FeeManage");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion

            //Thread.Sleep(10000);
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

                #region 函授站全部处理完学生后自动提交注册名单
                var sub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(u => u.y_adminId == YdAdminId);
                if (sub != null)
                {
                    var inyearint = Convert.ToInt32(inyear);
                    var feeyearint = Convert.ToInt32(feeyear);
                    var schoolname = ConfigurationManager.AppSettings["SchoolName"].ToString();
                    //得到相同函授站下同年级，缴费年份的学生数据
                    //var list1 = yunEntities.YD_Sts_StuInfo.Include(x => x.YD_Fee_StuFeeTb).Where(
                    //         u => u.y_subSchoolId == sub.y_subSchoolId && (u.y_stuStateId != 2 && u.y_stuStateId != 4 && u.y_stuStateId != 6) && u.y_inYear == inyearint && u.y_isdel == 1 && u.YD_Fee_StuFeeTb.Any(x => x.y_feeYear == feeyearint))
                    //         .Where(
                    //             u =>
                    //                 u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint) == false ||
                    //                 u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint && k.y_planconductOK == null)).ToList();
                    var ishave =
                         yunEntities.YD_Sts_StuInfo.Include(x => x.YD_Fee_StuFeeTb).Where(
                             u => u.y_subSchoolId == sub.y_subSchoolId && (u.y_stuStateId != 2 && u.y_stuStateId != 4 && u.y_stuStateId != 6) && u.y_inYear == inyearint && u.y_isdel == 1 && u.YD_Edu_Major.y_stuYear >= feeyearint && u.YD_Fee_StuFeeTb.Any(x => x.y_feeYear == feeyearint))
                             .Any(
                                 u =>
                                     u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint) == false ||
                                     u.YD_Fee_StuFeeTb.Any(k => k.y_feeYear == feeyearint && k.y_planconductOK == null));
                    
                    if (!ishave)
                    {
                        IQueryable<YD_Fee_StuFeeTb> list =
                            yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).OrderByDescending(u => u.id).Where(u =>
                              u.YD_Sts_StuInfo.y_subSchoolId == sub.y_subSchoolId && u.YD_Sts_StuInfo.y_isdel == 1 &&
                              (u.YD_Sts_StuInfo.y_stuStateId != 2 && u.YD_Sts_StuInfo.y_stuStateId != 4 && u.YD_Sts_StuInfo.y_stuStateId != 6)
                              && u.YD_Sts_StuInfo.y_inYear == inyearint && u.y_feeYear == feeyearint
                                  && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK == 1 && u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear >= feeyearint);
                        var data = list.Include(e => e.YD_Sts_StuInfo).Include(e => e.YD_Sts_StuInfo.YD_Edu_Major).Where(u => feeyearint > u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear).ToList();
                        if (!list.Any())
                        {
                            return "没有能提交的学生";
                        }
                        var BadStu = list.Where(u => feeyearint > u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear).ToList();
                        var test = list.ToList();
                        if (list.Any(u => feeyearint > u.YD_Sts_StuInfo.YD_Edu_Major.y_stuYear))
                        {
                            return "没有该学年注册学生";
                        }
                        var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == sub.y_subSchoolId);
                        var stuidlist = string.Join(",", list.Select(u => u.y_stuId).ToArray());  //所有stuid根据逗号相加成一个字符串
                        if (!stuidlist.EndsWith(","))
                        {
                            stuidlist += ",";
                        }
                        var entitylist = list.ToList();
                        var obj = new YD_Fee_StuRegistrBatch();
                        if (school != null)
                        {
                            obj.y_subSchoolId = school.id;
                            obj.schoolCode = school.y_code;
                            obj.schoolName = school.y_name;
                        }
                        else
                        {
                            return "函授站信息有误";
                        }
                        obj.y_feeyear = feeyearint;
                        obj.y_inyear = inyearint;
                        obj.totalcount = entitylist.Count();
                        obj.y_stuid += stuidlist;
                        if (schoolname == ComEnum.SchoolName.DHLGDX.ToString())
                        {
                            var governorName = Request["governorName"];
                            if (string.IsNullOrWhiteSpace(governorName))
                            {
                                return "请输入负责人姓名";
                            }
                            obj.governorName = governorName;
                        }

                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            #region  计算学费总金额和缴费总金额以及学生的个人金额
                            var idlist = stuidlist;
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
                                 ")as c on m.majorid = c.y_majorId  and m.schoolid = c.y_subSchoolId   join( select d.*,case when s.y_bili is not null then  " +
                                 "s.y_bili when a.y_bili is not null then a.y_bili  else 100 end as bili from YD_Fee_SubFeeBili as s " +
                                 "right join(select YD_Sys_SubSchool.y_name as schoolName, " +
                                 "YD_Edu_EduType.y_name as EduName, YD_Sys_SubSchool.id as schoolId, YD_Edu_EduType.id as EduId " +
                                 "from YD_Sys_SubSchool full join YD_Edu_EduType on 1 = 1 ) as d " +
                                 "on d.schoolId = s.y_subSchoolId and d.EduId = s.y_eduTypeId and y_Visible = 1 " +
                                 "left join (select * from dbo.YD_Fee_AllBili where y_Visible = 1) as a on a.y_eduTypeId = d.EduId ) " +
                                 "as p on p.schoolId = c.y_subSchoolId and p.EduId = c.y_eduTypeId";

                            var lists = yunEntities.Database.SqlQuery<BiliDto>(sqls).ToList();
                            decimal needUpFee = 0;
                            decimal needFee = 0;
                            entitylist.ForEach(u =>
                            {
                                var y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                                var y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                                var sb = new StringBuilder("UPDATE [YD_Fee_StuFeeTb] ");
                                sb.AppendLine(" SET [y_needFee]=" + y_needFee + ",y_needUpFee=" + y_needUpFee + " WHERE id=" + u.id);
                                string sql = sb.ToString();
                                yunEntities.Database.ExecuteSqlCommand(sql);
                                needFee += y_needFee;
                                needUpFee += y_needUpFee;
                                //u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                                //u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                                //yunEntities.Entry(u).State = EntityState.Modified;
                            });
                            obj.needtotal = needUpFee;
                            obj.tuitiontotal = needFee;
                            //obj.needtotal = list.Where(u => u.y_feeYear == feeyearint).ToList().Sum(u => u.y_needUpFee);
                            //obj.tuitiontotal = list.Where(u => u.y_feeYear == feeyearint).ToList().Sum(u => u.y_needFee);
                            #endregion
                        }
                        else
                        {
                            obj.tuitiontotal = 0;
                            obj.needtotal = 0;
                        }
                        obj.y_time = DateTime.Now;
                        obj.y_check = 0;
                        yunEntities.Entry(obj).State = EntityState.Added;
                        //var statename = "注册待审核";
                        //var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
                        //if (state == null)
                        //{
                        //    return "找不到注册待审核状态枚举";
                        //}
                        entitylist.ForEach(u =>
                        {
                            //u.YD_Sts_StuInfo.y_stuStateId = state.id;
                            u.YD_Sts_StuInfo.y_ischeck = 0;
                            u.y_isCheckFee = (int)YesOrNo.No;
                            u.y_isUp = (int)YesOrNo.Yes;

                            yunEntities.Entry(u.YD_Sts_StuInfo).State = EntityState.Modified;
                            yunEntities.Entry(u).State = EntityState.Modified;
                        });
                        int w = yunEntities.SaveChanges();
                        if (w > 0)
                        {
                            return "ok";
                        }
                        else
                        {
                            return "提交失败";
                        }
                    }
                    else
                    {
                        return "没有处理完同年级同一缴费学年学生不能提交";
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
        /// 选择性勾选学生情况
        /// </summary>
        /// <returns></returns>
        //public string StuFeeCheck()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion

        //    var id = Request["ids"];

        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                stuFee.isCheckForSchool = true;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "设置失败";
        //        }
        //    }
        //}
        /// <summary>
        /// 选择性取消勾选学生情况
        /// </summary>
        /// <returns></returns>
        //public string StuFeeCheckno()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion

        //    var id = Request["ids"];

        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                stuFee.isCheckForSchool = false;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "设置失败";
        //        }
        //    }
        //}

        /// <summary>
        /// 批量提交函授站学生名单
        /// </summary>
        /// <returns></returns>
        //public string StuFeebatchSome()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    var id = Request["ids"];

        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    //将string数组转换成int数组
        //    int[] ids = System.Array.ConvertAll(id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);

        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuFees = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => ids.Contains(u.id) && u.y_isUp == 0);
        //        if (stuFees != null)
        //        {
        //            return "不能选择已经申请的学生";
        //        }
        //        if (stuFees.y_planconductOK != 1)
        //        {
        //            return "不能选择不是预处理学生";
        //        }
        //        if (YdAdminRoleId == 4)
        //        {
        //            var adminsub = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(t => t.y_adminId == YdAdminId);
        //            if (adminsub == null)
        //            {
        //                return "请为该账号指定函授站";
        //            }
        //            IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id).
        //                Where(u => u.isCheckForSchool == true && u.y_subSchoolId != -1 && u.y_subSchoolId == adminsub.y_subSchoolId && u.y_isUp == (int)YesOrNo.No && u.y_planconductOK==1);
        //            var Fee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_subSchoolId == adminsub.y_subSchoolId);
        //            var entitylist = list.ToList();
        //            var obj = new YD_Fee_StuRegistrBatch();
        //            obj.y_subSchoolId = Fee.y_subSchoolId;
        //            obj.schoolCode = Fee.schoolCode;
        //            obj.schoolName = Fee.schoolName;
        //            obj.totalcount = entitylist.Count();
        //            obj.y_time = DateTime.Now;
        //            obj.y_check = 0;
        //            for (int i = 0; i < entitylist.Count(); i++)
        //            {
        //                var oid = Convert.ToInt32(entitylist[i].id);
        //                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                if (stuFee != null)
        //                {
        //                    obj.y_stuid += stuFee.y_stuId + ",";
        //                    obj.needtotal += stuFee.y_needUpFee;
        //                }
        //            }
        //            yunEntities.Entry(obj).State = EntityState.Added;
        //            int r = yunEntities.SaveChanges();
        //            if (r > 0)
        //            {
        //                #region 修改缴费表
        //                for (int i = 0; i < entitylist.Count(); i++)
        //                {
        //                    var oid = Convert.ToInt32(entitylist[i].id);
        //                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                    var statename = "注册待审核";
        //                    int stateid = 0;
        //                    var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                    if (state != null)
        //                        stateid = state.id;
        //                    if (stuFee != null)
        //                    {
        //                        //查询出对应学生信息
        //                        var objstu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
        //                        objstu.y_ischeck = 0;
        //                        //obj.y_registerState = obj.y_registerState.Replace(stuterm, "");
        //                        objstu.y_stuStateId = stateid;
        //                        yunEntities.Entry(objstu).State = EntityState.Modified;
        //                    }
        //                    stuFee.y_stuStateId = stateid;
        //                    stuFee.y_isCheckFee = (int)YesOrNo.No;
        //                    stuFee.y_upFee = 0;
        //                    stuFee.y_isUp = (int)YesOrNo.Yes;
        //                    yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                }
        //                yunEntities.SaveChanges();
        //                #endregion
        //                return "ok";
        //            }
        //            else
        //            {
        //                return "提交失败";
        //            }
        //        }
        //        else
        //        {
        //            return "只限于函授站账号使用";
        //        }

        //    }
        //}

        /// <summary>
        /// 选择性提交学生缴费情况
        /// </summary>
        /// <returns></returns>
        //public string StuFeeSome()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    var id = Request["ids"];
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    var term = Request["term"]; //学年
        //    var stuterm = ""; //学期
        //    switch (term)
        //    {
        //        case "1":
        //            stuterm = "[1][2]";
        //            break;
        //        case "2":
        //            stuterm = "[3][4]";
        //            break;
        //        case "3":
        //            stuterm = "[5][6]";
        //            break;
        //        case "4":
        //            stuterm = "[7][8]";
        //            break;
        //        case "5":
        //            stuterm = "[9][10]";
        //            break;
        //    }
        //    //将string数组转换成int数组
        //    int[] idsint = System.Array.ConvertAll(id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuFees = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => idsint.Contains(u.id) && u.y_isUp == 0);
        //        if (stuFees != null)
        //        {
        //            return "不能选择已经申请的学生";
        //        }
        //        List<int> subschoolids = new List<int>();
        //        for (int j = 0; j < ids.Count(); j++)
        //        {
        //            var jid = Convert.ToInt32(ids[j]);
        //            var stu = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == jid);
        //            if (stu != null)
        //            {
        //                if (subschoolids.Count == 0)
        //                {
        //                    subschoolids.Add(stu.y_subSchoolId);
        //                }
        //                if (!subschoolids.Contains(stu.y_subSchoolId))
        //                {
        //                    return "请选择相同函授站的学生";
        //                }
        //            }
        //        }
        //        var ob = new YD_Fee_StuRegistrBatch();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                if (stuFee.y_subSchoolId == -1)
        //                {
        //                    return "你选择的学生中不存在函授站信息，不能进行普通注册，请先注册到其他函授站";
        //                }
        //                if (stuFee.y_planconductOK != 1)
        //                {
        //                    return "不能选择不是预处理学生";
        //                }
        //                stuFee.y_isCheckFee = (int)YesOrNo.No;
        //                stuFee.y_upFee = 0;
        //                stuFee.y_isUp = (int)YesOrNo.Yes;
        //                stuFee.isCheckForSchool = false;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                ob.y_subSchoolId = stuFee.y_subSchoolId;
        //                ob.schoolCode = stuFee.schoolCode;
        //                ob.schoolName = stuFee.schoolName;
        //                ob.y_stuid += stuFee.y_stuId + ",";
        //                ob.needtotal += stuFee.y_needUpFee;
        //            }
        //        }
        //        ob.totalcount = ids.Count();
        //        ob.y_time = DateTime.Now;
        //        ob.y_check = 0;
        //        yunEntities.Entry(ob).State = EntityState.Added;
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            #region  只有函授站账号进入这里

        //            if (YdAdminRoleId == 4 || YdAdminRoleId == 1 || YdAdminRoleId == 6) //如果是函授站登录则表示未审核不能注册
        //            {
        //                for (int i = 0; i < ids.Count(); i++)
        //                {
        //                    var oid = Convert.ToInt32(ids[i]);
        //                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                    var statename = "注册待审核";
        //                    int stateid = 0;
        //                    var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                    if (state != null)
        //                        stateid = state.id;
        //                    if (stuFee != null)
        //                    {
        //                        //查询出对应学生信息
        //                        var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
        //                        obj.y_ischeck = 0;
        //                        //obj.y_registerState = obj.y_registerState.Replace(stuterm, "");
        //                        obj.y_stuStateId = stateid;
        //                        yunEntities.Entry(obj).State = EntityState.Modified;
        //                    }
        //                    yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                }
        //                yunEntities.SaveChanges();
        //            }

        //            #endregion

        //            return "ok";
        //        }
        //        else
        //        {
        //            return "设置失败";
        //        }
        //    }
        //}
        /// <summary>
        /// 学生缴费审核下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadFeeManage()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/FeeManage");
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
                var term = Request["term"];
                var isFee = Request["isFee"];
                var isplanOK = Request["isplanOK"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;

                var feeyearint = Convert.ToInt32(term);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Fee_StuFeeTb).
                    Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Edu_Major.YD_Edu_EduType).OrderByDescending(u => u.y_subSchoolId).
                    Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != null && u.y_inYear != xinshenyear
                    && u.y_stuStateId != 4 && u.y_stuStateId != 6 && u.YD_Edu_Major.y_stuYear >= feeyearint);

                //根据入学年份查询
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == (xinshenyear - 1));
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == yInYear);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard)
                                           || u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                var schoolName = "";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    var schoolfirst = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    schoolName = schoolfirst != null ? schoolfirst.y_name : "";
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(term))
                {
                    list = list.Where(u => !u.YD_Fee_StuFeeTb.Any(K => K.y_feeYear == feeyearint) ||
                            u.YD_Fee_StuFeeTb.FirstOrDefault(K => K.y_feeYear == feeyearint).y_isCheckFee == (int)YesOrNo.No);
                }
                if (!string.IsNullOrWhiteSpace(isplanOK) && isplanOK != "0")
                {
                    if (isplanOK == "1") //已处理  
                    {
                        list = list.Where(u => u.YD_Fee_StuFeeTb.Any(a => a.y_planconductOK == (int)YesOrNo.No && a.y_feeYear == feeyearint)
                             || u.YD_Fee_StuFeeTb.Any(a => a.y_planconductOK == (int)YesOrNo.Yes && a.y_feeYear == feeyearint));
                    }
                    else if (isplanOK == "2") //未处理
                    {
                        list = list.Where(u => u.YD_Fee_StuFeeTb.Any(a => a.y_planconductOK != (int)YesOrNo.No &&
                                              a.y_planconductOK != (int)YesOrNo.Yes && a.y_feeYear == feeyearint));
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                DataTable model = new DataTable();
                if (ConfigurationManager.AppSettings["SchoolName"] == "JXSFDX")
                {
                    var terms = Convert.ToInt32(Request["term"]);
                    model = FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new FeeCheckList
                                {
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    y_name = u.y_name,
                                    y_inYear = u.y_inYear,
                                    schoolName = u.YD_Sys_SubSchool.y_name,
                                    schoolCode = u.YD_Sys_SubSchool.y_code,
                                    y_majorlibrary = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    majorLibraryCode = u.YD_Edu_Major.y_StandardCode,
                                    y_stuType = u.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    y_eduType = u.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    y_tel = u.y_tel,
                                    y_address = u.y_address,
                                    y_cardId = u.y_cardId,
                                    y_stuYear = u.YD_Edu_Major.y_stuYear,
                                    y_feeYear = feeyearint,
                                    y_isUp = u.YD_Fee_StuFeeTb.FirstOrDefault(a => a.y_feeYear == terms).y_isUp == 0 ? "已申请" : "未申请",
                                    y_isCheckFee = u.YD_Fee_StuFeeTb.FirstOrDefault(a => a.y_feeYear == terms).y_isCheckFee == 0 ? "已通过" : "待审核"
                                }).ToList());
                }
                else
                {
                    model = FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new FeeCheckList
                                {
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    y_name = u.y_name,
                                    y_inYear = u.y_inYear,
                                    schoolName = u.YD_Sys_SubSchool.y_name,
                                    schoolCode = u.YD_Sys_SubSchool.y_code,
                                    y_majorlibrary = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    majorLibraryCode = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_code,
                                    y_stuType = u.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    y_eduType = u.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    y_tel = u.y_tel,
                                    y_address = u.y_address,
                                    y_cardId = u.y_cardId,
                                    y_stuYear = u.YD_Edu_Major.y_stuYear,
                                    y_feeYear = feeyearint,
                                    y_isUp = u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isUp == 0 ? "已申请" : "未申请",
                                    y_isCheckFee = u.YD_Fee_StuFeeTb.FirstOrDefault(a => true).y_isCheckFee == 0 ? "已通过" : "待审核"
                                }).ToList());
                }

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/" + schoolName + "在校生缴费审核表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
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
        /// 缴费审核 --弃用
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult FeeCheck(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Student/FeeCheck");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var inYear = Request["inYear"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];


                var isFee = Request["isFee"];
                int isfeeNew = 1;
                var isCheck = Request["isCheck"];

                var name = Request["name"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Fee_StuFeeTb> list = yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).
                    OrderByDescending(u => u.id).Where(u => u.y_isCheckFee == (int)YesOrNo.No);
                ViewBag.admin = YdAdminRoleId;

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
                if (string.IsNullOrWhiteSpace(isFee))
                {
                    //默认选中已申请注册
                    if (isfeeNew == 1)
                    {
                        list = list.Where(u => (u.y_isUp == (int)YesOrNo.Yes));
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
                if (!string.IsNullOrWhiteSpace(name))
                {

                    list = list.Where(u => u.YD_Sts_StuInfo.y_name == name);
                }
                if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                {
                    var feelyeear = Convert.ToInt32(term);
                    list = list.Where(u => u.y_feeYear == feelyeear);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.YD_Sts_StuInfo.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sts_StuInfo.YD_Sys_SubSchool.id));
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                ViewData["SchoolName"] = ConfigurationManager.AppSettings["SchoolName"];
                if (Request.IsAjaxRequest())
                    return PartialView("FeeCheckList", model);
                return View(model);
            }
        }
        /// <summary>
        /// 自动生成缴费名单 --弃用
        /// </summary>
        /// <returns></returns>
        public string CreateStuFee()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/FeeCheck");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion
            using (var yunEntities = new IYunEntities())
            {
                string feeYearStr = Request["feeYear"];
                if (string.IsNullOrWhiteSpace(feeYearStr))
                {
                    return "参数错误";
                }
                var feeYear = Convert.ToInt32(feeYearStr);
                //var feeallSys = yunEntities.YD_Fee_AllFeeSys.Where(u => u.y_inYear == feeYear).ToList();
                //if (feeallSys.Count == 0)
                //{
                //    return "请先设置好整体的收费标准";
                //}
                //生成第一缴费学年的学生名单
                var oneYearStus = yunEntities.VW_StuInfo.Where(u => u.y_inYear == feeYear).ToList();
                //生成第二缴费学年的学生名单
                var twoYearStus = yunEntities.VW_StuInfo.Where(u => u.y_inYear == (feeYear - 1)).ToList();
                //生成第三缴费学年的学生名单
                var threeYearStus = yunEntities.VW_StuInfo.Where(u => u.y_inYear == (feeYear - 2)).ToList();
                //生成第四缴费学年的学生名单
                var fourYearStus =
                    yunEntities.VW_StuInfo.Where(u => u.y_inYear == (feeYear - 3) && u.y_eduTypeId == 1).ToList();
                var fourYearStus2 = new List<VW_StuInfo>();
                //如果是中医药大学则需要生成高起专层次的第四缴费学年的名单
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    fourYearStus2 = yunEntities.VW_StuInfo.Where(u => u.y_inYear == (feeYear - 3) && u.y_eduTypeId == 2).ToList();
                }
                //生成第五缴费学年的学生名单
                var fiveYearStus = yunEntities.VW_StuInfo.Where(u => u.y_inYear == (feeYear - 4) && u.y_eduTypeId == 1).ToList();
                var stuFeeTbsOne = new List<YD_Fee_StuFeeTb>();
                var stuFeeTbsTwo = new List<YD_Fee_StuFeeTb>();
                var stuFeeTbsThree = new List<YD_Fee_StuFeeTb>();
                var stuFeeTbsFour = new List<YD_Fee_StuFeeTb>();
                var stuFeeTbsFour2 = new List<YD_Fee_StuFeeTb>();
                var stuFeeTbsFive = new List<YD_Fee_StuFeeTb>();
                #region 收集指定年份需上缴第一个学年的学生
                foreach (var oneYearStu in oneYearStus)
                {
                    //存在记录不用添加
                    if (!yunEntities.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 1))
                    {
                        var stuFee = new YD_Fee_StuFeeTb();
                        stuFee.y_feeYear = 1;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;

                        stuFeeTbsOne.Add(stuFee);
                    }
                }
                #endregion
                #region 收集指定年份需上缴第二个学年的学生

                foreach (var oneYearStu in twoYearStus)
                {
                    //存在记录不用添加
                    if (!yunEntities.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 2))
                    {
                        var stuFee = new YD_Fee_StuFeeTb();
                        stuFee.y_feeYear = 2;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFeeTbsTwo.Add(stuFee);
                    }
                }
                #endregion
                #region 收集指定年份需上缴第三个学年的学生

                foreach (var oneYearStu in threeYearStus)
                {
                    //存在记录不用添加
                    if (!yunEntities.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 3))
                    {

                        var stuFee = new YD_Fee_StuFeeTb();

                        stuFee.y_feeYear = 3;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFeeTbsThree.Add(stuFee);
                    }
                }

                #endregion
                #region 收集指定年份需上缴第四个学年的学生

                foreach (var oneYearStu in fourYearStus)
                {
                    //存在记录不用添加
                    if (!yunEntities.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 4))
                    {
                        var stuFee = new YD_Fee_StuFeeTb();

                        stuFee.y_feeYear = 4;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFeeTbsFour.Add(stuFee);
                    }
                }

                #endregion
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    #region 收集中医药大学高起专层次指定年份需上缴第四个学年的学生

                    foreach (var oneYearStu in fourYearStus2)
                    {
                        var stuFee = new YD_Fee_StuFeeTb();
                        stuFee.y_feeYear = 4;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFeeTbsFour.Add(stuFee);
                    }
                    #endregion
                }
                #region 收集指定年份需上缴第五个学年的学生

                foreach (var oneYearStu in fiveYearStus)
                {
                    //存在记录不用添加
                    if (!yunEntities.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 5))
                    {
                        var stuFee = new YD_Fee_StuFeeTb();
                        stuFee.y_feeYear = 5;
                        stuFee.y_isUp = (int)YesOrNo.No;
                        stuFee.y_isCheckFee = (int)YesOrNo.No;
                        stuFee.y_registerYear = DateTime.Now.Year;
                        stuFee.y_createtime = DateTime.Now;
                        stuFee.y_stuId = oneYearStu.id;
                        stuFee.y_needFee = 0;
                        stuFee.y_needUpFee = 0;
                        stuFeeTbsFive.Add(stuFee);
                    }
                }
                #endregion
                yunEntities.Configuration.AutoDetectChangesEnabled = false;
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsOne);
                yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsTwo);
                yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsThree);
                yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFour);
                if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFour2);
                }
                yunEntities.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFive);
                int t = yunEntities.SaveChanges();
                if (t > 0)
                {
                    return "ok";
                }
                else
                {
                    return "没有需要学生需要生成名单";
                }
            }
        }

        /// <summary> 
        /// 学期注册视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuRegistra(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuRegistra");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var year = Request["inYear"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var subSchool = Request["subSchool"];
                var major = Request["MajorLibrary"];
                var stuState = Request["stuState"];
                var name = Request["name"];
                var nosub = Request["nosub"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_StuState).Include(u => u.YD_Sys_SubSchool).OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name) || u.y_examNum == name);
                }
                if (!string.IsNullOrWhiteSpace(nosub) && !nosub.Equals("0"))
                {
                    list = list.Where(u => u.y_subSchoolId == null);
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                }
                //if (!string.IsNullOrWhiteSpace(registerState) && !registerState.Equals("0"))
                //{
                //    if (registerState == "1") //已注册
                //    {
                //        // || u.y_registerState.Contains(term) ||
                //        list = list.Where(u => u.y_stuStateId == 1);

                //    }
                //    else if (registerState == "2") //未注册
                //    {
                //        //u.y_registerState == null || u.y_registerState == "" ||!u.y_registerState.Contains(stuterm) ||
                //        list = list.Where(u =>u.y_stuStateId == 7);
                //    }
                //    else if (registerState == "3") //只有函数站管理员注册才需要的注册待审核
                //    {
                //        //u.y_ischeck == 0 ||
                //        list = list.Where(u => u.y_stuStateId == 8);
                //    }
                //}
                ViewBag.adminroleid = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                var a = Request["a"];
                var page = Request["num"];
                int num = Convert.ToInt32(page);
                if (num != 0)
                {
                    model = list.ToPagedList(id, num); //id为pageindex   15 为pagesize
                    if (Request.IsAjaxRequest())
                        return PartialView("StuRegistraList", model);
                    return View(model);
                }
                if (Request.IsAjaxRequest())
                    return PartialView("StuRegistraList", model);
                return View(model);

            }
        }
        /// <summary>
        /// 学生注册管理下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuRegistra()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StuRegistra");
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
                var inYear = Request["inYear"];
                var stuType = Request["stuType"];
                var eduType = Request["eduType"];
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var term = Request["term"];
                var stuState = Request["stuState"];
                var money = Request["money"];
                var registerState = Request["registerState"];
                var name = Request["name"];
                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                //根据入学年份查询
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(inYear);
                    list = list.Where(s => s.y_inYear == yInYear);
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(s => s.y_name == name);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(registerState) && !registerState.Equals("0"))
                {
                    if (registerState == "1") //已注册
                    {
                        list = list.Where(u => u.y_registerState.Contains(term) || u.y_stuStateId == 1);

                    }
                    else if (registerState == "2") //未注册
                    {
                        list = list.Where(u => u.y_registerState == null || u.y_registerState == "" || !u.y_registerState.Contains(term) || u.y_stuStateId == 7);
                    }
                }
                if (!string.IsNullOrWhiteSpace(money))
                {
                    if (money == "0") //已缴清
                    {
                        list = list.Where(u => u.y_isMoneyOk == 0);
                    }
                    else if (money == "1") //未缴清
                    {
                        list = list.Where(u => u.y_isMoneyOk == 1);
                    }
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new StuRegistra()
                                {
                                    inYear = u.y_inYear,
                                    stuName = u.y_name,
                                    stuTypeName = u.stuTypeName,
                                    eduTypeName = u.eduTypeName,
                                    majorName = u.majorName,
                                    subSchoolName = u.schoolName,
                                    stuStateName = u.stuStateName,
                                    Money = u.y_isMoneyOk == 1 ? "注册" : "未注册",
                                    registerState = u.y_registerState
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/学生注册管理表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/Dowon/学生注册管理表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"inYear", "入学年份"},
                        {"stuName","姓名"},
                        {"stuTypeName", "学习形式"},
                        {"eduTypeName", "层次"},
                        {"majorName", "专业"},
                        {"subSchoolName", "站点"},
                         {"stuStateName", "注册状态"},
                        {"Money", "是否注册"},
                        {"registerState", "学期"}
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
        /// 选择性注册学生
        /// </summary>
        /// <returns></returns>
        //public string StuRegisterSome()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StuRegistra");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }
        //    #endregion
        //    var id = Request["ids"];
        //    var term = Request["term"];
        //    var stuterm = "";
        //    switch (term)
        //    {
        //        case "1":
        //            stuterm = "[1][2]";
        //            break;
        //        case "2":
        //            stuterm = "[3][4]";
        //            break;
        //        case "3":
        //            stuterm = "[5][6]";
        //            break;
        //        case "4":
        //            stuterm = "[7][8]";
        //            break;
        //        case "5":
        //            stuterm = "[9][10]";
        //            break;
        //    }
        //    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(term))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
        //            if (obj != null && obj.y_subSchoolId == -1)
        //            {
        //                return "你选择的学生中不存在函授站信息，不能进行普通注册，请先注册到其他函授站";
        //            }
        //            if (obj != null && (string.IsNullOrWhiteSpace(obj.y_registerState) || !obj.y_registerState.Contains(stuterm)))
        //            {
        //                obj.y_registerState = obj.y_registerState + stuterm;
        //                var statename = "在读";
        //                var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                if (state != null)
        //                    obj.y_stuStateId = state.id;
        //                obj.y_ischeck = 1;
        //                #region 暂时不用的功能（只用于学校管理员补注册功能）
        //                //if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
        //                //{
        //                //    if (YdAdminRoleId == 4 )
        //                //    {
        //                //        obj.y_ischeck = 0; //如果是函授站登录则表示未审核不能注册
        //                //        obj.y_registerState = obj.y_registerState.Replace(stuterm, "");
        //                //        statename = "注册待审核";
        //                //        state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                //        if (state != null)
        //                //        {
        //                //            obj.y_stuStateId = state.id;
        //                //        }
        //                //}
        //                #endregion
        //                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == obj.id);
        //                //如果注册成功之后修改缴费表的相关学生信息
        //                if (stuFee != null)
        //                {
        //                    stuFee.stuStateName = statename;
        //                    if (state != null)
        //                    {
        //                        stuFee.y_stuStateId = state.id;
        //                        stuFee.y_stuStateCode = state.y_stuStateCode;
        //                    }
        //                    var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == obj.y_subSchoolId);
        //                    stuFee.y_subSchoolId = obj.y_subSchoolId;
        //                    if (subschool != null)
        //                    {
        //                        stuFee.schoolCode = subschool.y_code;
        //                        stuFee.subSchoolCode = subschool.y_code;
        //                    }
        //                    yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                    //}
        //                }
        //                yunEntities.Entry(obj).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "注册失败";
        //        }
        //    }
        //}

        /// <summary>
        /// 学校管理员审核函授站提交的注册名单
        /// </summary>
        /// <returns></returns>
        //public string StuRegisterCheck()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/StuRegistra");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion

        //    var id = Request["ids"];
        //    var term = Request["term"];
        //    var stuterm = "";
        //    switch (term)
        //    {
        //        case "1":
        //            stuterm = "[1][2]";
        //            break;
        //        case "2":
        //            stuterm = "[3][4]";
        //            break;
        //        case "3":
        //            stuterm = "[5][6]";
        //            break;
        //        case "4":
        //            stuterm = "[7][8]";
        //            break;
        //        case "5":
        //            stuterm = "[9][10]";
        //            break;
        //    }
        //    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(term))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
        //            if (obj.y_subSchoolId == -1)
        //            {
        //                return "你选择的学生中不存在函授站信息，不能进行普通注册，请先注册到其他函授站";
        //            }
        //            if (string.IsNullOrWhiteSpace(obj.y_registerState) || !obj.y_registerState.Contains(term))
        //            {
        //                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
        //                {
        //                    if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 6)
        //                    {
        //                        if (obj.y_ischeck != 0)
        //                        {
        //                            return "只用于审核函授站申请的注册名单";
        //                        }
        //                        obj.y_ischeck = 1; //如果是学校管理员登录则审核成功       
        //                        obj.y_registerState = obj.y_registerState + stuterm;
        //                        var statename = "在读";
        //                        var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                        if (state != null)
        //                            obj.y_stuStateId = state.id;
        //                    }
        //                }
        //                yunEntities.Entry(obj).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "注册审核失败";
        //        }
        //    }
        //}

        /// <summary>
        /// 将学生注册到指定的函授站
        /// </summary>
        /// <returns></returns>
        //public string StuRegisterSub()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StuRegistra");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }
        //    #endregion
        //    var id = Request["ids"];
        //    var subSchool = Request["subSchoolId"];
        //    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(subSchool))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
        //            if (obj.y_subSchoolId != -1)
        //            {
        //                return "你选择的学生中已经存在函授站信息，不能注册到其他函授站";
        //            }
        //            //将函授站关联到注册到的函授站，将第一学年注册好，且将学籍状态改为在读。
        //            var subschoolid = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.y_name == subSchool);
        //            obj.y_subSchoolId = subschoolid.id;
        //            obj.y_stuStateId = 1;
        //            obj.y_registerState = "[1][2]";
        //            yunEntities.Entry(obj).State = EntityState.Modified;

        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "注册失败";
        //        }

        //    }
        //}


        /// <summary>
        /// 根据条件注册所以学生
        /// </summary>
        /// <returns></returns>
        //public string Register()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StuRegistra");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }
        //    #endregion
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var year = Request["year"];
        //        var stuType = Request["stuType"];
        //        var eduType = Request["eduType"];
        //        var subSchool = Request["SubSchool"];
        //        var major = Request["MajorLibrary"];
        //        var term = Request["term"];
        //        var stuState = Request["stuState"];
        //        var money = Request["money"];
        //        if (string.IsNullOrWhiteSpace(term))
        //        {
        //            term = "1";
        //        }
        //        var stuterm = "";
        //        switch (term)
        //        {
        //            case "1":
        //                stuterm = "[1][2]";
        //                break;
        //            case "2":
        //                stuterm = "[3][4]";
        //                break;
        //            case "3":
        //                stuterm = "[5][6]";
        //                break;
        //            case "4":
        //                stuterm = "[7][8]";
        //                break;
        //            case "5":
        //                stuterm = "[9][10]";
        //                break;
        //        }
        //        const int isnotdel = (int)YesOrNo.No;
        //        IQueryable<VW_StuInfo> list =
        //            yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel && u.y_subSchoolId != -1);
        //        if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
        //        {
        //            var enrollYearint = Convert.ToInt32(year);
        //            list = list.Where(u => u.y_inYear == enrollYearint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
        //        {
        //            var subSchoolint = Convert.ToInt32(subSchool);
        //            list = list.Where(u => u.y_subSchoolId == subSchoolint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
        //        {
        //            var majorLibraryint = Convert.ToInt32(major);
        //            list = list.Where(u => u.y_majorLibId == majorLibraryint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
        //        {
        //            var eduTypeint = Convert.ToInt32(eduType);
        //            list = list.Where(u => u.y_eduTypeId == eduTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
        //        {
        //            var stuTypeint = Convert.ToInt32(stuType);
        //            list = list.Where(u => u.y_stuTypeId == stuTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
        //        {
        //            var stuStateint = Convert.ToInt32(stuState);
        //            list = list.Where(u => u.y_stuStateId == stuStateint);
        //        }
        //        var entitylist = list.Where(u => u.y_registerState == null || u.y_registerState == "" || !u.y_registerState.Contains(stuterm)).ToList();//未注册的学生
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < entitylist.Count(); i++)
        //        {
        //            int id = entitylist[i].id;
        //            var entity = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
        //            entity.y_registerState = entity.y_registerState + stuterm;
        //            var statename = "在读";
        //            var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //            if (state != null)
        //                entity.y_stuStateId = state.id;
        //            #region 如果是函授站登录则表示未审核不能注册
        //            if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
        //            {
        //                if (YdAdminRoleId == 4 || YdAdminRoleId == 1 || YdAdminRoleId == 6)
        //                {
        //                    entity.y_ischeck = 0; //如果是函授站登录则表示未审核不能注册
        //                    entity.y_registerState = entity.y_registerState.Replace(stuterm, "");
        //                    statename = "注册待审核";
        //                    state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                    if (state != null)
        //                        entity.y_stuStateId = state.id;
        //                }
        //            }
        //            #endregion
        //            yunEntities.Entry(entity).State = EntityState.Modified;
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            return "注册成功";
        //        }
        //        else
        //        {
        //            return "注册失败";
        //        }
        //    }
        //}
        /// <summary>
        /// 选择性提交学生未缴费情况
        /// </summary>
        /// <returns></returns>
        //public string StuFeeSomeno()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeManage");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion
        //    var id = Request["ids"];
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                stuFee.y_isCheckFee = (int)YesOrNo.No;
        //                stuFee.y_upFee = 1;
        //                stuFee.y_isUp = (int)YesOrNo.No;
        //                stuFee.isCheckForSchool = false;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            #region  只有函授站账号进入这里

        //            if (YdAdminRoleId == 4 || YdAdminRoleId == 1 || YdAdminRoleId == 6) //如果是函授站登录则表示未审核不能注册
        //            {
        //                for (int i = 0; i < ids.Count(); i++)
        //                {
        //                    var oid = Convert.ToInt32(ids[i]);
        //                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                    if (stuFee != null)
        //                    {
        //                        //查询出对应学生信息
        //                        var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
        //                        if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
        //                        {
        //                            var statename = "未注册";
        //                            var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                            if (state != null)
        //                                obj.y_stuStateId = state.id;
        //                        }
        //                        yunEntities.Entry(obj).State = EntityState.Modified;
        //                    }
        //                }
        //                yunEntities.SaveChanges();
        //            }

        //            #endregion
        //            return "ok";

        //        }
        //        else
        //        {
        //            return "设置失败";
        //        }

        //    }
        //}
        /// <summary>
        /// 选择性取消注册学生
        /// </summary>
        /// <returns></returns>
        //public string StuRegisterSomeNo()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StuRegistra");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }
        //    #endregion
        //    var id = Request["ids"];
        //    var term = Request["term"];
        //    var stuterm = "";
        //    switch (term)
        //    {
        //        case "1":
        //            stuterm = "[1][2]";
        //            break;
        //        case "2":
        //            stuterm = "[3][4]";
        //            break;
        //        case "3":
        //            stuterm = "[5][6]";
        //            break;
        //        case "4":
        //            stuterm = "[7][8]";
        //            break;
        //        case "5":
        //            stuterm = "[9][10]";
        //            break;
        //    }
        //    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(term))
        //    {
        //        return "未知错误";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
        //            if (obj != null && (!string.IsNullOrWhiteSpace(obj.y_registerState) && obj.y_registerState.Contains(stuterm)))
        //            {
        //                obj.y_registerState = obj.y_registerState.Replace(stuterm, "");
        //                var statename = "未注册";
        //                var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                if (state != null)
        //                {
        //                    obj.y_stuStateId = state.id;
        //                }
        //                var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.y_stuId == obj.id);
        //                //如果取消注册之后修改缴费表的相关学生信息
        //                if (stuFee != null)
        //                {
        //                    stuFee.stuStateName = statename;
        //                    if (state != null)
        //                    {
        //                        stuFee.y_stuStateId = state.id;
        //                        stuFee.y_stuStateCode = state.y_stuStateCode;
        //                    }
        //                    yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                }
        //            }
        //            yunEntities.Entry(obj).State = EntityState.Modified;
        //        }
        //        int t = yunEntities.SaveChanges();
        //        if (t > 0)
        //        {
        //            return "ok";

        //        }
        //        else
        //        {
        //            return "未知错误";
        //        }
        //    }
        //}
        /// <summary>
        /// 选择性审核学生缴费情况
        /// </summary>
        /// <returns></returns>
        //public string StuFeeCheckSome()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeCheck");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion

        //    var id = Request["ids"];
        //    var term = Request["term"]; //不需要用的检索条件的学年
        //    var stuterm = ""; //学期
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    if (string.IsNullOrWhiteSpace(term))
        //    {
        //        return "学年没有";
        //    }
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                stuFee.y_isCheckFee = (int)YesOrNo.Yes;
        //                stuFee.y_isUp = (int)YesOrNo.Yes;
        //                stuFee.y_upFee = stuFee.y_needFee;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            #region  只有学校管理员进入这里

        //            if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 6) //如果是函授站登录则表示未审核不能注册
        //            {
        //                for (int i = 0; i < ids.Count(); i++)
        //                {
        //                    var oid = Convert.ToInt32(ids[i]);
        //                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                    if (stuFee != null)
        //                    {
        //                        var feeyear = stuFee.y_feeYear;
        //                        //得到该缴费学年
        //                        switch (feeyear)
        //                        {
        //                            case 1:
        //                                stuterm = "[1][2]";
        //                                break;
        //                            case 2:
        //                                stuterm = "[3][4]";
        //                                break;
        //                            case 3:
        //                                stuterm = "[5][6]";
        //                                break;
        //                            case 4:
        //                                stuterm = "[7][8]";
        //                                break;
        //                            case 5:
        //                                stuterm = "[9][10]";
        //                                break;
        //                        }
        //                        //查询出对应学生信息
        //                        var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
        //                        if (obj.y_subSchoolId == -1)
        //                        {
        //                            return "你选择的学生中不存在函授站信息，不能进行普通注册，请先注册到其他函授站";
        //                        }
        //                        if (string.IsNullOrWhiteSpace(obj.y_registerState) || !obj.y_registerState.Contains(stuterm))
        //                        {
        //                            if (obj.y_ischeck != 0)
        //                            {
        //                                return "只用于审核函授站申请的注册名单";
        //                            }
        //                            obj.y_ischeck = 1; //如果是学校管理员登录则审核成功       
        //                            obj.y_registerState = obj.y_registerState + stuterm;
        //                            var statename = "在读";
        //                            var state =
        //                                yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                            if (state != null)
        //                            {
        //                                obj.y_stuStateId = state.id;
        //                            }
        //                            //如果注册成功之后修改缴费表的相关学生信息
        //                            stuFee.stuStateName = statename;
        //                            if (state != null)
        //                            {
        //                                stuFee.y_stuStateId = state.id;
        //                                stuFee.y_registerState = stuFee.y_registerState + stuterm;
        //                                stuFee.y_stuStateCode = state.y_stuStateCode;
        //                            }
        //                            var subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == obj.y_subSchoolId);
        //                            stuFee.y_subSchoolId = obj.y_subSchoolId;
        //                            if (subschool != null)
        //                            {
        //                                stuFee.schoolCode = subschool.y_code;
        //                                stuFee.subSchoolCode = subschool.y_code;
        //                            }
        //                            yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                        }
        //                        yunEntities.Entry(obj).State = EntityState.Modified;
        //                    }
        //                }
        //                yunEntities.SaveChanges();
        //            }

        //            #endregion
        //            return r.ToString();
        //        }
        //        else
        //        {
        //            return "审核失败";
        //        }
        //    }
        //}


        /// <summary>
        /// 选择性审核学生缴费情况--拒绝
        /// </summary>
        /// <returns></returns>
        //public string StuFeeCheckSomeNo()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Student/FeeCheck");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return "没有权限";
        //    }

        //    #endregion

        //    var id = Request["ids"];

        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return "未知错误";
        //    }
        //    var term = Request["term"]; //学年
        //    var stuterm = ""; //学期           
        //    string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var stuList = new List<YD_Sts_StuInfo>();
        //        for (int i = 0; i < ids.Count(); i++)
        //        {
        //            var oid = Convert.ToInt32(ids[i]);
        //            var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //            if (stuFee != null)
        //            {
        //                stuFee.y_isCheckFee = (int)YesOrNo.No;
        //                stuFee.y_upFee = 0;
        //                stuFee.y_isUp = (int)YesOrNo.No;
        //                yunEntities.Entry(stuFee).State = EntityState.Modified;
        //            }
        //        }
        //        int r = yunEntities.SaveChanges();
        //        if (r > 0)
        //        {
        //            #region  只有学校账号进入这里

        //            if (YdAdminRoleId == 3 || YdAdminRoleId == 1 || YdAdminRoleId == 6)
        //            {
        //                for (int i = 0; i < ids.Count(); i++)
        //                {
        //                    var oid = Convert.ToInt32(ids[i]);
        //                    var stuFee = yunEntities.YD_Fee_StuFeeTb.FirstOrDefault(u => u.id == oid);
        //                    if (stuFee != null)
        //                    {
        //                        var feeyear = stuFee.y_feeYear;
        //                        switch (feeyear)
        //                        {
        //                            case 1:
        //                                stuterm = "[1][2]";
        //                                break;
        //                            case 2:
        //                                stuterm = "[3][4]";
        //                                break;
        //                            case 3:
        //                                stuterm = "[5][6]";
        //                                break;
        //                            case 4:
        //                                stuterm = "[7][8]";
        //                                break;
        //                            case 5:
        //                                stuterm = "[9][10]";
        //                                break;
        //                        }
        //                        //查询出对应学生信息
        //                        var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuFee.y_stuId);
        //                        if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.JXSFDX.ToString())
        //                        {
        //                            //obj.y_registerState = obj.y_registerState.Replace(stuterm, "");
        //                            var statename = "未注册";
        //                            var state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == statename);
        //                            if (state != null)
        //                            {
        //                                obj.y_stuStateId = state.id;
        //                            }
        //                            //如果取消注册之后修改缴费表的相关学生信息
        //                            stuFee.stuStateName = statename;
        //                            if (state != null)
        //                            {
        //                                stuFee.y_stuStateId = state.id;
        //                                stuFee.y_stuStateCode = state.y_stuStateCode;
        //                            }
        //                            yunEntities.Entry(stuFee).State = EntityState.Modified;
        //                        }
        //                        yunEntities.Entry(obj).State = EntityState.Modified;
        //                    }
        //                }
        //                yunEntities.SaveChanges();
        //            }
        //            #endregion
        //            return "ok";
        //        }
        //        else
        //        {
        //            return "审批失败";
        //        }

        //    }
        //}
        #endregion

        #endregion

        #region 学号设置管理

        public ActionResult StuNum()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuNum");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewBag.entity = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
                return View();
            }
        }

        public ActionResult StuNumColSave()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuNum");
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


        #region 学号组成设置

        public ActionResult StuNumSys(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuNumSys");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var name = Request["name"];
                var year = Request["year"];
                var stuType = Request["StuType"];
                var eduType = Request["EduType"];
                var subSchool = Request["SubSchool"];
                var majorLibId = Request["major"];
                var stuNumState = Request["stuNumState"];
                var sex = Request["sex"];
                var rests = Request["rests"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (!string.IsNullOrWhiteSpace(rests) && !rests.Equals("0"))
                {
                    list = list.Where(u => u.y_name.Contains(rests) || u.y_cardId.Contains(rests) || u.y_examNum.Contains(rests) || u.y_address.Contains(rests));
                }
                if (!string.IsNullOrWhiteSpace(sex))
                {
                    var sexint = Convert.ToInt32(sex);
                    if (sexint == 1) //男
                    {
                        list = list.Where(u => u.y_sex == 0);
                    }
                    else if (sexint == 2) //女
                    {
                        list = list.Where(u => u.y_sex == 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibId) && !majorLibId.Equals("0"))
                {
                    var majorLibIdint = Convert.ToInt32(majorLibId);
                    list = list.Where(u => u.y_majorLibId == majorLibIdint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuNumState) && !stuNumState.Equals("0"))
                {
                    var stuNumStateint = Convert.ToInt32(stuNumState);
                    if (stuNumStateint == 1) //有学号
                    {
                        list = list.Where(u => u.y_stuNum != "");

                    }
                    else if (stuNumStateint == 2) //无学号
                    {
                        list = list.Where(u => u.y_stuNum == "" || u.y_stuNum == null);
                    }
                }

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                    return PartialView("StuNumSysList", model);
                return View(model);
            }
        }
        /// <summary>
        /// 学号管理下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuNumSys()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/StuNumSys");
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
                var major = Request["MajorLibrary"];
                var subSchool = Request["SubSchool"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var stuNumState = Request["stuNumState"];
                var name = Request["name"];
                var sex = Request["sex"];
                var rests = Request["rests"];

                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(sex))
                {
                    int sexint = Convert.ToInt32(sex);
                    if (sexint == 1) //男
                    {
                        list = list.Where(u => u.y_sex == 0);

                    }
                    else if (sexint == 2) //女
                    {
                        list = list.Where(u => u.y_sex == 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(rests) && !rests.Equals("0"))
                {
                    list.Where(u => u.y_cardId.Contains(rests) || u.y_examNum.Contains(rests) || u.y_name.Contains(rests) || u.y_address.Contains(rests));
                }
                //根据入学年份查询
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var yInYear = Convert.ToInt16(year);
                    list = list.Where(s => s.y_inYear == yInYear);
                }

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(major);
                    list = list.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuNumState) && !stuNumState.Equals("0"))
                {
                    var stuNumStateint = Convert.ToInt32(stuNumState);
                    if (stuNumStateint == 1) //有学号
                    {
                        list = list.Where(u => u.y_stuNum != "");

                    }
                    else if (stuNumStateint == 2) //无学号
                    {
                        list = list.Where(u => u.y_stuNum == "" || u.y_stuNum == null);
                    }
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_inYear = u.y_inYear,
                                    y_name = u.y_name,
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    y_cardId = u.y_cardId,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_address = u.y_address,
                                    eduTypeName = u.eduTypeName,
                                    stuTypeName = u.stuTypeName,
                                    majorLibraryName = u.majorLibraryName,
                                    schoolName = u.schoolName,
                                    stuStateName = u.stuStateName
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/学号管理表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/学号管理表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inYear", "入学年份"},
                        {"y_name", "姓名"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"y_cardId", "身份证号"},
                        {"y_sex", "性别"},
                        {"y_address", "地址"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"majorLibraryName", "专业"},
                        {"schoolName","函授站"},
                        {"stuStateName", "状态"}
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
        /// 针对2018级学号生成错误的江西师大的学号生成方法。
        /// </summary>
        /// <returns></returns>
        //public ActionResult CreateStuNumJXSD()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StuNumSys");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_update == (int)PowerState.Disable)
        //    {
        //        return Content("没有权限");
        //    }
        //    #endregion
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var year = Request["year"];
        //        var stuType = Request["stuType"];
        //        var eduType = Request["EduType"];
        //        var subSchool = Request["SubSchool"];
        //        var majorlib = Request["major"];
        //        var stuNumState = Request["stuNumState"];
        //        var sex = Request["sex"];
        //        const int isnotdel = (int)YesOrNo.No;
        //        //获取学号组成规则
        //        var stuNumCol = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
        //        //学号随机数长度
        //        int randomNumLength = Convert.ToInt16(ConfigurationManager.AppSettings["StuNumLength"]);
        //        var stuNumCols = new List<string>
        //        {
        //            string.IsNullOrWhiteSpace(stuNumCol.y_one) ? "空" : stuNumCol.y_one,
        //            string.IsNullOrWhiteSpace(stuNumCol.y_two) ? "空" : stuNumCol.y_two,
        //            string.IsNullOrWhiteSpace(stuNumCol.y_three) ? "空" : stuNumCol.y_three,
        //            string.IsNullOrWhiteSpace(stuNumCol.y_four) ? "空" : stuNumCol.y_four,
        //            string.IsNullOrWhiteSpace(stuNumCol.y_five) ? "空" : stuNumCol.y_five
        //        };
        //        if (!stuNumCols.Contains("专业"))
        //        {
        //            return Content("学号组成设置不正确！学号中必须带有专业编号！"); ;
        //        }
        //        IQueryable<VW_StuInfo> list = yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);

        //        if (subSchool.Equals("0"))
        //        {
        //            return Content("请选择函授站");
        //        }
        //        if (year.Equals("0"))
        //        {
        //            return Content("请选择年份");
        //        }
        //        if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
        //        {
        //            var enrollYearint = Convert.ToInt32(year);
        //            list = list.Where(u => u.y_inYear == enrollYearint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
        //        {
        //            var subSchoolint = Convert.ToInt32(subSchool);
        //            list = list.Where(u => u.y_subSchoolId == subSchoolint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(majorlib) && !majorlib.Equals("0"))
        //        {
        //            var majorlibint = Convert.ToInt32(majorlib);
        //            list = list.Where(u => u.y_majorLibId == majorlibint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
        //        {
        //            var eduTypeint = Convert.ToInt32(eduType);
        //            list = list.Where(u => u.y_eduTypeId == eduTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
        //        {
        //            var stuTypeint = Convert.ToInt32(stuType);
        //            list = list.Where(u => u.y_stuTypeId == stuTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(sex) && !sex.Equals("0"))
        //        {
        //            var sexint = Convert.ToInt32(sex);
        //            if (sexint == 1)
        //            {
        //                list = list.Where(u => u.y_sex == 0);
        //            }
        //            if (sexint == 2)
        //            {
        //                list = list.Where(u => u.y_sex == 1);
        //            }
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuNumState) && !stuNumState.Equals("0"))
        //        {
        //            var stumlist = list.Where(u => u.y_stuNum == "").ToList();
        //            //if (stumlist.Count > 900)
        //            //{
        //            //    return Content("请选择层次");
        //            //}
        //        }
        //        var entityList = list.ToList();

        //        //是对哪些学生生成学号
        //        var stuNumStateint = Convert.ToInt32(stuNumState);

        //    }
        //}

        /// <summary>
        /// 生成学号
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateStuNum()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuNumSys");
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
                var year = Request["year"];
                var stuType = Request["stuType"];
                var eduType = Request["EduType"];
                var subSchool = Request["SubSchool"];
                var majorlib = Request["major"];
                var stuNumState = Request["stuNumState"];
                var sex = Request["sex"];
                const int isnotdel = (int)YesOrNo.No;
                //获取学号组成规则
                var stuNumCol = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
                //学号随机数长度
                int randomNumLength = Convert.ToInt16(ConfigurationManager.AppSettings["StuNumLength"]);
                var stuNumCols = new List<string>
                {
                    string.IsNullOrWhiteSpace(stuNumCol.y_one) ? "空" : stuNumCol.y_one,
                    string.IsNullOrWhiteSpace(stuNumCol.y_two) ? "空" : stuNumCol.y_two,
                    string.IsNullOrWhiteSpace(stuNumCol.y_three) ? "空" : stuNumCol.y_three,
                    string.IsNullOrWhiteSpace(stuNumCol.y_four) ? "空" : stuNumCol.y_four,
                    string.IsNullOrWhiteSpace(stuNumCol.y_five) ? "空" : stuNumCol.y_five
                };
                if (!stuNumCols.Contains("专业"))
                {
                    return Content("学号组成设置不正确！学号中必须带有专业编号！"); ;
                }
                IQueryable<VW_StuInfo> list = yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);

                if (subSchool.Equals("0"))
                {
                    return Content("请选择函授站");
                }
                if (year.Equals("0"))
                {
                    return Content("请选择年份");
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorlib) && !majorlib.Equals("0"))
                {
                    var majorlibint = Convert.ToInt32(majorlib);
                    list = list.Where(u => u.y_majorLibId == majorlibint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(sex) && !sex.Equals("0"))
                {
                    var sexint = Convert.ToInt32(sex);
                    if (sexint == 1)
                    {
                        list = list.Where(u => u.y_sex == 0);
                    }
                    if (sexint == 2)
                    {
                        list = list.Where(u => u.y_sex == 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuNumState) && !stuNumState.Equals("0"))
                {
                    var stumlist = list.Where(u => u.y_stuNum == "").ToList();
                    //if (stumlist.Count > 900)
                    //{
                    //    return Content("请选择层次");
                    //}
                }
                var entityList = list.ToList();

                //是对哪些学生生成学号
                int num = 1;//递增流水号初始值
                var stuNumStateint = Convert.ToInt32(stuNumState);
                if (stuNumStateint == 2) //仅对无学号进行生成学号
                {
                    #region
                    //如果选择了某个专业就对这个专业进行编学号
                    if (!string.IsNullOrWhiteSpace(majorlib) && !majorlib.Equals("0"))
                    {
                        var majorint = Convert.ToInt32(majorlib);
                        var stuList = new List<YD_Sts_StuInfo>();
                        list = list.Where(u => u.y_majorLibId == majorint && u.y_subSchoolId != null);
                        var stuListHasN = list.Where(u => u.y_stuNum != "" && u.y_stuNum != null).ToList();//找出有学号的学生
                        var stuListNoN = list.Where(u => u.y_stuNum == "" || u.y_stuNum == null).ToList();//找出没有学号的学生
                        if (stuListHasN.Any())//存在有学号的学生，对没有学号的学生进行递增学号
                        {   //找出最大学号
                            var stuNumList = new List<long>();
                            for (var i = 0; i < stuListHasN.Count(); i++)
                            {
                                stuNumList.Add(long.Parse(stuListHasN[i].y_stuNum));
                            }
                            var maxStuNum = stuNumList.Max();
                            //给没有学号的学生进行递增学号流水号
                            for (var i = 0; i < stuListNoN.Count; i++)
                            {
                                int id = stuListNoN[i].id;
                                var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                                if (nowStu != null)
                                {
                                    nowStu.y_stuNum = (maxStuNum + i + 1).ToString();
                                    stuList.Add(nowStu);
                                }
                            }
                        }
                        else//不存在有学号的学生，那么就重新生成学号
                        {
                            //给没有学号的学生进行递增学号流水号
                            var hs = new Hashtable();
                            for (var i = 0; i < stuListNoN.Count; i++)
                            {
                                hs["年份"] = stuListNoN[i].y_inYear.ToString().Remove(0, 2);
                                hs["函授站"] = stuListNoN[i].subSchoolCode;
                                hs["学习形式"] = stuListNoN[i].y_stuTypeCode;
                                hs["专业"] = stuListNoN[i].majorLibraryCode;
                                hs["层次"] = stuListNoN[i].y_eduTypeCode;
                                hs["空"] = "";
                                int id = stuListNoN[i].id;
                                var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                                if (nowStu != null)
                                {
                                    nowStu.y_stuNum = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] + hs[stuNumCols[3]] + hs[stuNumCols[4]] + num.ToString().PadLeft(randomNumLength, '0');
                                    //得到最新自增数
                                    var repetition = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == nowStu.y_stuNum);
                                    //相同的专业，函授站，年份组成的学号顺序
                                    var str = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] + hs[stuNumCols[3]] + hs[stuNumCols[4]];
                                    if (repetition != null)
                                    {
                                        string sql = @"select y_stuNum from YD_Sts_StuInfo where y_stuNum like '" + str + "%'";
                                        var sqllist = yunEntities.Database.SqlQuery<string>(sql).ToList();
                                        if (sqllist.Count > 0)
                                        {
                                            //找出最大学号
                                            var stuNumList = new List<long>();
                                            for (var j = 0; j < sqllist.Count(); j++)
                                            {
                                                stuNumList.Add(long.Parse(sqllist[j]));
                                            }
                                            var maxStuNum = stuNumList.Max();
                                            nowStu.y_stuNum = (maxStuNum + 1).ToString();
                                            yunEntities.Entry(nowStu).State = EntityState.Modified;
                                            yunEntities.SaveChanges();
                                        }
                                    }
                                    num++;
                                    yunEntities.Entry(nowStu).State = EntityState.Modified;
                                }
                            }
                        }
                        int r = yunEntities.SaveChanges();
                        if (r > 0)
                        {
                            return Content("ok");
                        }
                        else
                        {
                            return Content("注册失败");
                        }

                    }
                    else//如果没有选择某个专业就对所有专业进行编学号
                    {
                        var majorList = yunEntities.YD_Edu_Major.Where(u => true).ToList();
                        var stuList = new List<YD_Sts_StuInfo>();
                        for (var i = 0; i < majorList.Count; i++)//根据专业循环学生
                        {
                            var majorId = majorList[i].id;
                            var listNow = list.Where(u => u.y_majorId == majorId && u.y_subSchoolId != null);
                            var stuListHasN = listNow.Where(u => u.y_stuNum != "" && u.y_stuNum != null).ToList();//找出有学号的学生
                            var stuListNoN = listNow.Where(u => u.y_stuNum == "" || u.y_stuNum == null).ToList();//找出没有学号的学生
                            if (stuListHasN.Any())//存在有学号的学生，对没有学号的学生进行递增学号
                            {   //找出最大学号
                                var stuNumList = new List<long>();
                                for (var j = 0; j < stuListHasN.Count(); j++)
                                {
                                    stuNumList.Add(long.Parse(stuListHasN[j].y_stuNum));
                                }
                                var maxStuNum = stuNumList.Max();
                                //给没有学号的学生进行递增学号流水号
                                for (var j = 0; j < stuListNoN.Count; j++)
                                {
                                    var id = stuListNoN[j].id;
                                    var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                                    if (nowStu == null) continue;
                                    nowStu.y_stuNum = (maxStuNum + j + 1).ToString();
                                    stuList.Add(nowStu);
                                }
                            }
                            else//不存在有学号的学生，那么就重新生成学号
                            {
                                //给没有学号的学生进行递增学号流水号
                                var hs = new Hashtable();
                                for (var j = 0; j < stuListNoN.Count; j++)
                                {
                                    hs["年份"] = stuListNoN[j].y_inYear.ToString().Remove(0, 2);
                                    hs["函授站"] = stuListNoN[j].subSchoolCode;
                                    hs["学习形式"] = stuListNoN[j].y_stuTypeCode;
                                    hs["专业"] = stuListNoN[j].majorLibraryCode;
                                    hs["层次"] = stuListNoN[j].y_eduTypeCode;
                                    hs["空"] = "";
                                    int id = stuListNoN[j].id;
                                    var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                                    if (nowStu != null)
                                    {
                                        //var fixnum = randomNumLength -
                                        //    (hs[stuNumCols[0]].ToString().Length +
                                        //    hs[stuNumCols[1]].ToString().Length +
                                        //    hs[stuNumCols[2]].ToString().Length +
                                        //    hs[stuNumCols[3]].ToString().Length +
                                        //    hs[stuNumCols[4]].ToString().Length);
                                        var strnum = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] + hs[stuNumCols[3]] + hs[stuNumCols[4]] + num.ToString().PadLeft(randomNumLength, '0');

                                        nowStu.y_stuNum = strnum;
                                        //得到最新自增数
                                        var repetition =
                                            yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == nowStu.y_stuNum);
                                        //相同的专业，函授站，年份组成的学号顺序
                                        var str = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] +
                                                  hs[stuNumCols[3]] + hs[stuNumCols[4]];
                                        if (repetition != null)
                                        {
                                            string sql = @"select y_stuNum from YD_Sts_StuInfo where y_stuNum like '" + str + "%'";
                                            var sqllist = yunEntities.Database.SqlQuery<string>(sql).ToList();
                                            if (sqllist.Count > 0)
                                            {
                                                //找出最大学号
                                                var stuNumList = new List<long>();
                                                for (var p = 0; p < sqllist.Count(); p++)
                                                {
                                                    stuNumList.Add(long.Parse(sqllist[p]));
                                                }
                                                var maxStuNum = stuNumList.Max();
                                                nowStu.y_stuNum = (maxStuNum + 1).ToString();

                                                yunEntities.Entry(nowStu).State = EntityState.Modified;
                                                yunEntities.SaveChanges();
                                            }
                                        }
                                        num++;
                                        yunEntities.Entry(nowStu).State = EntityState.Modified;

                                    }
                                }
                            }
                        }
                        int r = yunEntities.SaveChanges();
                        if (r > 0)
                        {
                            return Content("ok");
                        }
                        else
                        {
                            return Content("注册失败");
                        }

                    }
                    #endregion
                }
                else
                {
                    return Content("只对无学号进行生成学号");
                }
                //#region 全部重新生成
                ////记录需要处理的学生数量
                //var stuCount = list.Count();
                ////如果选择了某个专业就对这个专业进行编学号
                //if (!string.IsNullOrWhiteSpace(majorlib) && !majorlib.Equals("0"))
                //{
                //    var majorint = Convert.ToInt32(majorlib);
                //    var stuList = new List<YD_Sts_StuInfo>();
                //    list = list.Where(u => u.y_majorLibId == majorint && u.y_subSchoolId != null);
                //    var stuListNoN = list.ToList();
                //    var hs = new Hashtable();
                //    for (var i = 0; i < stuListNoN.Count; i++)
                //    {
                //        hs["年份"] = stuListNoN[i].y_inYear.ToString().Remove(0, 2);
                //        hs["函授站"] = stuListNoN[i].subSchoolCode;
                //        hs["学习形式"] = stuListNoN[i].y_stuTypeCode;
                //        hs["专业"] = stuListNoN[i].majorLibraryCode;
                //        hs["层次"] = stuListNoN[i].y_eduTypeCode;

                //        hs["空"] = "";
                //        int id = stuListNoN[i].id;
                //        var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                //        if (nowStu != null)
                //        {
                //            nowStu.y_stuNum = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] + hs[stuNumCols[3]] + hs[stuNumCols[4]] + num.ToString().PadLeft(randomNumLength, '0');
                //            //得到最新自增数
                //            var repetition = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == nowStu.y_stuNum);
                //            //相同的专业，函授站，年份组成的学号顺序
                //            var str = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] +
                //                      hs[stuNumCols[3]] + hs[stuNumCols[4]];
                //            if (repetition != null)
                //            {
                //                string sql = @"select y_stuNum from YD_Sts_StuInfo where y_stuNum like '" + str + "%'";
                //                var sqllist = yunEntities.Database.SqlQuery<string>(sql).ToList();
                //                if (sqllist.Count > 0)
                //                {
                //                    //找出最大学号
                //                    var stuNumList = new List<long>();
                //                    for (var x = 0; x < sqllist.Count(); x++)
                //                    {
                //                        stuNumList.Add(long.Parse(sqllist[x]));
                //                    }
                //                    var maxStuNum = stuNumList.Max();
                //                    nowStu.y_stuNum = (maxStuNum + 1).ToString();


                //                    yunEntities.Entry(nowStu).State = EntityState.Modified;

                //                    yunEntities.SaveChanges();
                //                }
                //            }
                //            num++;
                //            yunEntities.Entry(nowStu).State = EntityState.Modified;

                //        }
                //        stuList.Add(nowStu);
                //    }
                //    int r = yunEntities.SaveChanges();
                //    if (r > 0)
                //    {
                //        return Content("ok");
                //    }
                //    else
                //    {
                //        return Content("注册失败");
                //    }
                //}
                //else//如果没有选择某个专业就对所有专业进行编学号
                //{

                //    var majorList = yunEntities.YD_Edu_Major.Where(u => true).ToList();
                //    var majorId = 0;
                //    var stuListNoN = new List<VW_StuInfo>();
                //    var stuList = new List<YD_Sts_StuInfo>();
                //    var hs = new Hashtable();
                //    for (var i = 0; i < majorList.Count; i++)//根据专业循环学生
                //    {
                //        majorId = majorList[i].id;
                //        //list = list.Where(u => u.y_majorId == majorId);
                //        //stuListNoN = list.ToList();
                //        stuListNoN = list.Where(u => u.y_majorId == majorId && u.y_subSchoolId != null).ToList();
                //        for (var j = 0; j < stuListNoN.Count; j++)
                //        {
                //            hs["年份"] = stuListNoN[j].y_inYear.ToString().Remove(0, 2);
                //            hs["函授站"] = stuListNoN[j].subSchoolCode;
                //            hs["学习形式"] = stuListNoN[j].y_stuTypeCode;
                //            hs["专业"] = stuListNoN[j].majorLibraryCode;
                //            hs["层次"] = stuListNoN[j].y_eduTypeCode;

                //            hs["空"] = "";
                //            int id = stuListNoN[j].id;
                //            var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                //            if (nowStu != null)
                //            {
                //                nowStu.y_stuNum = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] +
                //                                  hs[stuNumCols[3]] + hs[stuNumCols[4]] +
                //                                  num.ToString().PadLeft(randomNumLength, '0');
                //            }
                //            //得到最新自增数
                //            var repetition = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == nowStu.y_stuNum);
                //            //相同的专业，函授站，年份组成的学号顺序
                //            var str = hs[stuNumCols[0]] + hs[stuNumCols[1]].ToString() + hs[stuNumCols[2]] + hs[stuNumCols[3]] + hs[stuNumCols[4]];
                //            if (repetition != null)
                //            {
                //                string sql = @"select y_stuNum from YD_Sts_StuInfo where y_stuNum like '" + str + "%'";
                //                var sqllist = yunEntities.Database.SqlQuery<string>(sql).ToList();
                //                if (sqllist.Count > 0)
                //                {
                //                    //找出最大学号
                //                    var stuNumList = new List<long>();
                //                    for (var t = 0; t < sqllist.Count(); t++)
                //                    {
                //                        stuNumList.Add(long.Parse(sqllist[t]));
                //                    }
                //                    var maxStuNum = stuNumList.Max();
                //                    nowStu.y_stuNum = (maxStuNum + 1).ToString();


                //                    yunEntities.Entry(nowStu).State = EntityState.Modified;

                //                    yunEntities.SaveChanges();
                //                }
                //            }
                //            num++;
                //            yunEntities.Entry(nowStu).State = EntityState.Modified;

                //        }
                //        Dispose();
                //    }
                //    int r = yunEntities.SaveChanges();
                //    if (r > 0)
                //    {
                //        return Content("ok");
                //    }
                //    else
                //    {
                //        if (stuCount > 0)
                //        {
                //            return Content("注册失败");
                //        }
                //        else
                //        {
                //            return Content("检索结果中没有需要生成学号的学生");
                //        }
                //    }
                //}
                //#endregion
            }
        }


        /// <summary>
        /// 生成学号
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateStuNum1()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StuNumSys");
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
                var year = Request["year"];
                var stuType = Request["stuType"];
                var eduType = Request["EduType"];
                var subSchool = Request["SubSchool"];
                var majorlib = Request["major"];
                var stuNumState = Request["stuNumState"];
                var sex = Request["sex"];
                const int isnotdel = (int)YesOrNo.No;
                //获取学号组成规则
                var stuNumCol = yunEntities.YD_Sts_StuNumCol.FirstOrDefault(u => true);
                //学号随机数长度
                int randomNumLength = Convert.ToInt16(ConfigurationManager.AppSettings["StuNumLength"]);
                var stuNumCols = new List<string>
                {
                    string.IsNullOrWhiteSpace(stuNumCol.y_one) ? "空" : stuNumCol.y_one,
                    string.IsNullOrWhiteSpace(stuNumCol.y_two) ? "空" : stuNumCol.y_two,
                    string.IsNullOrWhiteSpace(stuNumCol.y_three) ? "空" : stuNumCol.y_three,
                    string.IsNullOrWhiteSpace(stuNumCol.y_four) ? "空" : stuNumCol.y_four,
                    string.IsNullOrWhiteSpace(stuNumCol.y_five) ? "空" : stuNumCol.y_five
                };
                if (!stuNumCols.Contains("专业"))
                {
                    return Content("学号组成设置不正确！学号中必须带有专业编号！"); ;
                }
                IQueryable<VW_StuInfo> list = yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);

                ////if (subSchool.Equals("0"))
                ////{
                ////    return Content("请选择函授站");
                ////}
                if (year.Equals("0"))
                {
                    return Content("请选择年份");
                }
                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorlib) && !majorlib.Equals("0"))
                {
                    var majorlibint = Convert.ToInt32(majorlib);
                    list = list.Where(u => u.y_majorLibId == majorlibint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.y_stuTypeId == stuTypeint);
                }
                if (!string.IsNullOrWhiteSpace(sex) && !sex.Equals("0"))
                {
                    var sexint = Convert.ToInt32(sex);
                    if (sexint == 1)
                    {
                        list = list.Where(u => u.y_sex == 0);
                    }
                    if (sexint == 2)
                    {
                        list = list.Where(u => u.y_sex == 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(stuNumState) && !stuNumState.Equals("0"))
                {
                    var stumlist = list.Where(u => u.y_stuNum == "").ToList();
                    //if (stumlist.Count > 900)
                    //{
                    //    return Content("请选择层次");
                    //}
                }
                var entityList = list.ToList();

                //是对哪些学生生成学号

                var stuNumStateint = Convert.ToInt32(stuNumState);
                if (stuNumStateint == 2) //仅对无学号进行生成学号
                {
                    #region
                    var firstStuNum = 15802182440778;
                    var stuList = entityList.Where(u => u.y_stuNum != "" && u.y_stuNum != null && u.y_inYear == 2015 && u.y_stuNum.Length>14).ToList();//找出有学号的学生
 
                  
                    //给没有学号的学生进行递增学号流水号
                    var hs = new Hashtable();
                    for (var j = 0; j < stuList.Count; j++)
                    {

                        int id = stuList[j].id;
                        var nowStu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                        if (nowStu != null)
                        {
                            //var fixnum = randomNumLength -
                            //    (hs[stuNumCols[0]].ToString().Length +
                            //    hs[stuNumCols[1]].ToString().Length +
                            //    hs[stuNumCols[2]].ToString().Length +
                            //    hs[stuNumCols[3]].ToString().Length +
                            //    hs[stuNumCols[4]].ToString().Length);
                            firstStuNum++;
                            nowStu.y_stuNum = firstStuNum.ToString();
                            yunEntities.Entry(nowStu).State = EntityState.Modified;
                            yunEntities.SaveChanges();
                        }
                    }

                    int r = yunEntities.SaveChanges();
                    if (r > 0)
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("注册失败");
                    }


                    #endregion
                }
            }
            return Content("55");
        }
        #endregion
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdatePassword()
        {
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 毕业条件设置
        /// </summary>
        /// <returns></returns>
        public ActionResult GradCondition()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/GradCondition");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                ViewBag.entity = yunEntities.YD_Edu_GradCondition.Where(u => true).ToList();
                return View();
            }
        }
        /// <summary>
        /// 设置毕业条件
        /// </summary>
        /// <returns></returns>
        public ActionResult SetCodition()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/GradCondition");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var entity = yunEntities.YD_Edu_GradCondition.Where(u => true).ToList();
                for (int i = 0; i < entity.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(Request[entity[i].id.ToString()]))
                    {
                        entity[i].y_isUse = (int)YesOrNo.No;
                    }
                    else
                    {
                        entity[i].y_isUse = (int)YesOrNo.Yes;
                    }

                    yunEntities.Entry(entity[i]).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",设置毕业条件,修改毕业条件,ID:" + entity[i].id + ",y_isUse:" + entity[i].y_isUse + ",方法:SetCodition");
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return Content("<script>alert('设置成功');window.location.href='/Student/GradCondition'</script>");
                }
                else
                {
                    return Content("设置失败");
                }

            }
        }

        #region 毕业信息视图

        /// <summary>
        /// 生成毕业生方法
        /// </summary>
        /// <returns></returns>
        public string StartGrad()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/StudentGradInfo");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目


                const int yes = (int)YesOrNo.Yes;
                //获取毕业条件
                var entity = yunEntities.YD_Edu_GradCondition.Where(u => u.y_isUse == yes).ToList();

                IQueryable<VW_StuInfo> list =
                   yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel != yes);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var nowList = list.ToList();
                foreach (var ydEduGradCondition in entity)
                {
                    if (ydEduGradCondition.id == 1)//达到毕业年份,入学年份+学制=当前年份，且当前日期大于等于设置的毕业日期
                    {
                        var year = DateTime.Now.Year;
                        var five = year - 5;//五年学制
                        var three = year - 3;//三年学制
                        nowList = nowList.Where(u => (u.y_eduTypeId == 1 && u.y_inYear == five) || (u.y_eduTypeId != 1 && u.y_inYear == three)).ToList();
                    }
                    else if (ydEduGradCondition.id == 2)//各个学期已经注册，3年学制应注册123456学期，五年制应注册12345678910学期
                    {
                        nowList = nowList.Where(u => (u.y_eduTypeId == 1 && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[1]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[2]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[3]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[4]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[5]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[6]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[7]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[8]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[9]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[10]"))) || (u.y_eduTypeId != 1 && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[1]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[2]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[3]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[4]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[5]")) && (!string.IsNullOrWhiteSpace(u.y_registerState) && u.y_registerState.Contains("[6]")))).ToList();
                    }
                    else if (ydEduGradCondition.id == 3)//成绩合格，获取教学计划，有未考试的不予审核通过，全部考试了但存在不及格现象不予通过
                    {
                        //获取学生的专业教学计划
                        for (var i = 0; i < nowList.Count; i++)
                        {
                            var majorId = nowList[i].y_majorId;
                            var year = nowList[i].y_inYear;
                            var sid = nowList[i].id;
                            var teachPlans =
                                yunEntities.YD_Edu_MajorTeachPlan.Where(
                                    u => u.y_majorId == majorId && u.y_teaPlanType == 2 && u.y_year == year);
                            var teachplanDeses = new List<YD_Edu_TeachplanDes>();
                            foreach (var ydEduMajorTeachPlan in teachPlans)
                            {
                                teachplanDeses.AddRange(yunEntities.YD_Edu_TeachplanDes.Where(u => u.y_teaPlanId == ydEduMajorTeachPlan.id).ToList());
                            }
                            var scores = yunEntities.VW_Score.Where(u => u.y_stuId == sid);
                            if (scores.Count() == teachPlans.Count())
                            {
                                if (scores.Any(u => u.y_totalScore < 60))//存在不合格科目
                                {
                                    nowList.Remove(nowList[i]);
                                }
                            }
                            else//存在未考科目
                            {
                                nowList.Remove(nowList[i]);
                            }
                        }
                    }
                }
                var scoreListT = new List<YD_Sts_StuInfo>();
                for (int i = 0; i < nowList.Count; i++)
                {
                    int id = nowList[i].id;
                    var a = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                    if (a != null)
                    {
                        a.y_stuStateId = 6;
                        yunEntities.Entry(a).State = EntityState.Modified;

                        LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",生成毕业生方法,修改学生信息为已毕业,学生ID:" + id + ",方法:StartGrad");
                    }
                }
                int t = yunEntities.SaveChanges();
                if (t > 0)
                {
                    return "ok";
                }
                else
                {
                    return "生成失败";
                }


            }
        }
        /// <summary>
        /// 毕业信息视图控制
        /// </summary>
        /// <returns>视图</returns>
        //public ActionResult StudentGradInfo(int id = 1)
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Student/StudentGradInfo");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_select == (int)PowerState.Disable)
        //    {
        //        //var reurl = Request.UrlReferrer.ToString();
        //        var reurl = "/AdminBase/Index";
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }
        //    #endregion

        //    using (var yunEntities = new IYunEntities())
        //    {
        //        ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目

        //        var name = Request["name"];
        //        var sex = Request["sex"];
        //        var stuState = Request["StuState"];
        //        var card = Request["card"];
        //        var enrollYear = Request["EnrollYear"];
        //        var subSchool = Request["SubSchool"];
        //        var majorLibrary = Request["MajorLibrary"];
        //        var eduType = Request["EduType"];
        //        var stuType = Request["StuType"];
        //        var tel = Request["tel"];
        //        const int isnotdel = (int)YesOrNo.No;

        //        IQueryable<VW_StuInfo> list =
        //           yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel && u.y_stuStateId == 6);
        //        if (YdAdminRoleId == 1 || YdAdminRoleId == 3 || YdAdminRoleId == 6)
        //        {

        //        }
        //        else
        //        {

        //            List<YD_Sys_AdminSubLink> subLinks =
        //                yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).ToList();
        //            var subschoolids = new List<int>();
        //            for (var i = 0; i < subLinks.Count; i++)
        //            {
        //                subschoolids.Add(subLinks[i].y_subSchoolId);
        //            }
        //            list = list.Where(u => subschoolids.Contains(u.y_subSchoolId));
        //        }



        //        if (!string.IsNullOrWhiteSpace(name))
        //        {
        //            list = list.Where(u => u.y_name.Contains(name));
        //        }
        //        if (!string.IsNullOrWhiteSpace(sex))
        //        {
        //            var sexint = Convert.ToInt32(sex);
        //            list = list.Where(u => u.y_sex == sexint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
        //        {
        //            var stuStateint = Convert.ToInt32(stuState);
        //            list = list.Where(u => u.y_stuStateId == stuStateint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(card))
        //        {
        //            list = list.Where(u => u.y_cardId.Contains(card));
        //        }
        //        if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
        //        {
        //            var enrollYearint = Convert.ToInt32(enrollYear);
        //            list = list.Where(u => u.y_inYear == enrollYearint);
        //        }

        //        if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
        //        {
        //            var subSchoolint = Convert.ToInt32(subSchool);
        //            list = list.Where(u => u.y_subSchoolId == subSchoolint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(tel))
        //        {
        //            list = list.Where(u => u.y_tel.Contains(tel));
        //        }
        //        if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
        //        {
        //            var majorLibraryint = Convert.ToInt32(majorLibrary);
        //            list = list.Where(u => u.y_majorLibId == majorLibraryint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
        //        {
        //            var eduTypeint = Convert.ToInt32(eduType);
        //            list = list.Where(u => u.y_eduTypeId == eduTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
        //        {
        //            var stuTypeint = Convert.ToInt32(stuType);
        //            list = list.Where(u => u.y_stuTypeId == stuTypeint);
        //        }


        //        var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

        //        if (Request.IsAjaxRequest())
        //            return PartialView("StudentGradInfoList", model);
        //        return View(model);
        //    }
        //}

        #endregion

        #region 学籍成绩表打印
        /// <summary>
        /// 学生学籍成绩表 --师大学籍表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuStatusScore(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                //获取异动信息
                var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).Where(u => u.y_stuId == id).ToList();
                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.Where(u => u.y_stuId == id).OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.stu.id ,s.classCourse.y_team},
                    score => new { y_course = score.y_courseId, id = score.y_stuId, y_team = score.y_term, },
                    (s, score) => new { s, score = score.OrderByDescending(u=>u.y_type).ThenByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists.GroupBy(u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore > 99 ? 85 : u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();

                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                ViewBag.GradeYear=ConfigurationManager.AppSettings["xinsheng"];
                return View(stuScore);
            }
        }


        public ActionResult StuStatusScore4(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                //获取异动信息
                var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).FirstOrDefault(u => u.y_stuId == id);
                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.Where(u => u.y_stuId == id).OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                return View(stuScore);
            }
        }
        /// <summary>
        /// 东华理工大学籍表Excel版批量导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string StuStatusScoreExcel()
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                List<int> stuIdList = yunEntities.YD_Sts_StuInfo.Where(u => u.y_inYear == 2016).Select(x => x.id).ToList();
                if (stuIdList.Count <= 0)
                {
                    return "err";
                }
                foreach (int id in stuIdList)
                {
                    var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(x => x.id == id); ;

                    const int status = (int)ApprovaState.HadApprova;
                    //获取异动信息

                    var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).FirstOrDefault(u => u.y_stuId == id);
                    //获取学生成绩
                    //获取学籍教学计划

                    if (!yunEntities.YD_Edu_Score.Any(x => x.y_stuId == id))
                    {
                        continue;
                    }


                    var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                    var stu =
                        yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                    var list = stu.GroupJoin(classCourse,
                        s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                        c =>
                            new
                            {
                                c.YD_TeaPlan_Class.y_majorId,
                                y_inYear = c.YD_TeaPlan_Class.y_year,
                                c.YD_TeaPlan_Class.y_subSchoolId,
                            }, (s, c) => new { s, c })
                        .SelectMany(
                            xy => xy.c.DefaultIfEmpty(),
                            (x, y) => new { stu = x.s, classCourse = y });

                    var scorelist = yunEntities.VW_Score.Where(u => u.y_stuId == id).OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                    List<ScoreStatistics_Course> stuScore = null;
                    if (lists.All(u => u.s.classCourse != null))
                    {
                        stuScore = lists
                            .GroupBy(
                                u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                            .Select(
                                u =>
                                    new ScoreStatistics_Course
                                    {
                                        CourseName = u.Key.YD_Edu_Course.y_name,
                                        CourseId = u.Key.YD_Edu_Course.id,
                                        Term = u.Key.y_team,
                                        TotalScore = u.FirstOrDefault().score.y_totalScore,
                                        SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                        TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                        y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                        y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                    })
                            .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                            .ToList();
                    }

                    if (stuScore.Any(x => x.TotalScore < 60))
                    {
                        continue;
                    }
                    string fileTempName = Server.MapPath("~/File/mould/学生学籍表模板.xlsx");
                    XSSFWorkbook wk = null;
                    using (FileStream fs = new FileStream(fileTempName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //把xls文件读入workbook变量里，之后就可以关闭了
                        wk = new XSSFWorkbook(fs);
                        fs.Close();
                    }
                    int year = 2016;

                    ISheet ws = wk.GetSheet("学籍表");
                    XSSFSheet sheet = (XSSFSheet)wk.GetSheetAt(0);
                    sheet.GetRow(3).GetCell(3).SetCellValue(stuInfo.y_stuNum);
                    sheet.GetRow(3).GetCell(17).SetCellValue(stuInfo.y_name);
                    sheet.GetRow(4).GetCell(3).SetCellValue(stuInfo.y_sex == 0 ? "男" : "女");
                    string nation = "";
                    if (stuInfo.y_nationId.HasValue && stuInfo.y_nationId > 0)
                    {
                        nation = yunEntities.YD_Sts_Nation.FirstOrDefault(x => x.id == stuInfo.y_nationId).y_name;
                    }
                    sheet.GetRow(4).GetCell(12).SetCellValue(nation);
                    sheet.GetRow(4).GetCell(17).SetCellValue(stuInfo.y_birthday.ToString("yyyy-MM-dd"));
                    sheet.GetRow(5).GetCell(3).SetCellValue(stuInfo.y_cardId);
                    sheet.GetRow(5).GetCell(17).SetCellValue(stuInfo.y_tel);
                    sheet.GetRow(6).GetCell(3).SetCellValue(stuInfo.y_address);
                    string eduType = "";
                    if (stuInfo.y_eduTypeId.HasValue && stuInfo.y_eduTypeId > 0)
                    {
                        eduType = yunEntities.YD_Edu_EduType.FirstOrDefault(x => x.id == stuInfo.y_eduTypeId).y_name;
                    }
                    sheet.GetRow(7).GetCell(3).SetCellValue(eduType);
                    sheet.GetRow(7).GetCell(17).SetCellValue(stuInfo.y_stuYear + "年");
                    sheet.GetRow(8).GetCell(3).SetCellValue(stuInfo.majorLibraryName);
                    string subschool = "";
                    if (stuInfo.y_subSchoolId.HasValue && stuInfo.y_subSchoolId > 0)
                    {
                        subschool = yunEntities.YD_Sys_SubSchool.FirstOrDefault(x => x.id == stuInfo.y_subSchoolId).y_name;
                    }
                    sheet.GetRow(8).GetCell(17).SetCellValue(subschool);

                    if (stuScore != null && stuScore.Count() > 0)
                    {

                        for (int i = 1; i <= 5; i++)
                        {
                            int j = 0, k = 0;
                            foreach (var item in stuScore)
                            {
                                if (item.Term / i == 2 && item.Term % 2 == 0)
                                {
                                    if (item.Term == 2)
                                    {
                                        sheet.GetRow(21 + j).GetCell(1).SetCellValue(item.CourseName.Split('(')[0]);
                                    }
                                    else
                                    {
                                        sheet.GetRow(21 + j).GetCell(3 + (i - 1) % 5 * 5).SetCellValue(item.CourseName.Contains('(') ? item.CourseName.Split('(')[0] : item.CourseName);
                                    }
                                    sheet.GetRow(21 + j).GetCell(5 + (i - 1) % 5 * 5).SetCellValue(Math.Round((double)item.TotalScore, 0, MidpointRounding.AwayFromZero));
                                    j++;
                                }
                                else if ((item.Term + 1) / i == 2 && (item.Term + 1) % i == 0)
                                {
                                    if (item.Term == 1)
                                    {
                                        sheet.GetRow(13 + k).GetCell(1).SetCellValue(item.CourseName.Split('(')[0]);
                                    }
                                    else
                                    {
                                        sheet.GetRow(13 + k).GetCell(3 + (i - 1) % 5 * 5).SetCellValue(item.CourseName.Contains('(') ? item.CourseName.Split('(')[0] : item.CourseName);
                                    }
                                    sheet.GetRow(13 + k).GetCell(5 + (i - 1) % 5 * 5).SetCellValue(Math.Round((double)item.TotalScore, 0, MidpointRounding.AwayFromZero));
                                    k++;
                                }


                            }
                        }
                    }
                    // 打包目录
                    string filePath = Server.MapPath($"~/File/Dowon/{year}");
                    if (!System.IO.Directory.Exists(filePath))
                        System.IO.Directory.CreateDirectory(filePath);
                    // 教学点目录
                    string filePath2 = Server.MapPath($"~/File/Dowon/{year}/{subschool}");
                    if (!System.IO.Directory.Exists(filePath2))
                        System.IO.Directory.CreateDirectory(filePath2);
                    string fileName = $"{ stuInfo.y_stuNum}_{stuInfo.y_name}.xlsx";
                    using (FileStream file = new FileStream(filePath2 + "\\" + fileName, FileMode.Create))
                    {
                        wk.Write(file);  //创建test.xls文件。
                        file.Close();
                    }
                }
                CompressZip(2016);
                return "ok";
            }
        }
        /// <summary>
        /// 江西师范批量打印学籍表
        /// </summary>
        /// <returns></returns>
        public string JXSFStustatusScoreExcel()
        {

            using (var yunEntities = new IYunEntities())
            {
                //打印的学生入学年份
                int year = 2016;
                //获取学生对象,满足条件：入学年份相等，状态为已毕业，有毕业证号，有毕业照片（毕业照片放在181026文件夹，所以要要有这个关键词）
                List<int> stuIdList = yunEntities.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_isgraduate == true && u.y_graduateNumber != null && u.y_img.Contains("181026")).Select(x => x.id).ToList();
                //List<int> stuIdList = new List<int>();//测试用打印对象
                //stuIdList.Add(18207);
                if (stuIdList.Count <= 0)
                {
                    return "no student can print";
                }
                var stuInfos = yunEntities.VW_StuInfo.Where(e => stuIdList.Contains(e.id)).ToList();

                var AllStranges = yunEntities.VW_Strange.Where(e => stuIdList.Contains(e.y_stuId)).ToList();
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.ToList();
                var allStuScores = yunEntities.VW_Score.Where(u => stuIdList.Contains(u.y_stuId)).ToList();
                var allSubSchool = yunEntities.YD_Sys_SubSchool.ToList();
                var allMajor = yunEntities.YD_Edu_Major.ToList();
                foreach (int id in stuIdList)
                {
                    var stuInfo = stuInfos.FirstOrDefault(x => x.id == id); ;

                    const int status = (int)ApprovaState.HadApprova;
                    //获取异动信息


                    //获取学生成绩
                    //获取学籍教学计划


                    var stu = stuInfos.Where(u => u.id == id);

                    var list = stu.GroupJoin(classCourse,
                        s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                        c =>
                            new
                            {
                                c.YD_TeaPlan_Class.y_majorId,
                                y_inYear = c.YD_TeaPlan_Class.y_year,
                                c.YD_TeaPlan_Class.y_subSchoolId,
                            }, (s, c) => new { s, c })
                        .SelectMany(
                            xy => xy.c.DefaultIfEmpty(),
                            (x, y) => new { stu = x.s, classCourse = y });

                    var scorelist = allStuScores.Where(u => u.y_stuId == id).OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                    List<ScoreStatistics_Course> stuScore = null;
                    if (lists.All(u => u.s.classCourse != null))
                    {
                        stuScore = lists
                            .GroupBy(
                                u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                            .Select(
                                u =>
                                    new ScoreStatistics_Course
                                    {
                                        CourseName = u.Key.YD_Edu_Course.y_name,
                                        CourseId = u.Key.YD_Edu_Course.id,
                                        Term = u.Key.y_team,
                                        TotalScore = u.FirstOrDefault().score.y_totalScore,
                                        SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                        TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                        y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                        y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                    })
                            .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                            .ToList();
                    }

                    if (stuScore.Any(x => x.TotalScore < 60))
                    {
                        continue;
                    }
                    string fileTempName = Server.MapPath("~/File/mould/SD学生学籍表模板.xls");
                    HSSFWorkbook wk = null;
                    using (FileStream fs = new FileStream(fileTempName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //把xls文件读入workbook变量里，之后就可以关闭了
                        wk = new HSSFWorkbook(fs);
                        fs.Close();
                    }

                    //ISheet ws = wk.GetSheet("学籍表");
                    HSSFSheet sheet = (HSSFSheet)wk.GetSheetAt(0);
                    sheet.GetRow(1).GetCell(2).SetCellValue($"站点:{stuInfo.schoolName}  年级:{stuInfo.y_inYear}  专业:{stuInfo.majorLibraryName}");
                    sheet.GetRow(2).GetCell(2).SetCellValue($"层次:{stuInfo.eduTypeName}  学习形式:{stuInfo.stuTypeName}  学制:{stuInfo.y_stuYear}");
                    sheet.GetRow(3).GetCell(2).SetCellValue($"姓名:{stuInfo.y_name}  学号:{stuInfo.y_stuNum}  入学成绩:{stuInfo.y_examScore}");
                    //姓名
                    sheet.GetRow(4).GetCell(4).SetCellValue(stuInfo.y_name);
                    //性别
                    sheet.GetRow(4).GetCell(7).SetCellValue(stuInfo.y_sex == 0 ? "男" : "女");
                    //民族
                    sheet.GetRow(4).GetCell(9).SetCellValue(stuInfo.nationName);
                    //身份证号
                    sheet.GetRow(6).GetCell(4).SetCellValue(stuInfo.y_cardId);
                    //政治面貌
                    sheet.GetRow(6).GetCell(8).SetCellValue(stuInfo.politicsName);
                    //地址
                    sheet.GetRow(8).GetCell(4).SetCellValue(stuInfo.y_address);

                    var strange = AllStranges.Where(u => u.y_approvalStatus == status && u.y_stuId == id);
                    StringBuilder strangeStr = new StringBuilder();
                    if (strange.Any(e => e.y_strangeType == 1))
                    {
                        var strange1 = strange.FirstOrDefault(e => e.y_strangeType == 1);
                        var formMajor = allMajor.FirstOrDefault(e => e.id == Convert.ToInt32(strange1.y_contentB));
                        var toMajor = allMajor.FirstOrDefault(e => e.id == Convert.ToInt32(strange1.y_contentA));
                        if (formMajor != null)
                        {
                            strangeStr.Append("转出专业:");
                            strangeStr.Append(formMajor.y_name);
                            strangeStr.Append("  转入专业:");
                            strangeStr.Append(toMajor.y_name);
                        }
                    }
                    if (strange.Any(e => e.y_strangeType == 2))
                    {
                        var strange2 = strange.FirstOrDefault(e => e.y_strangeType == 2);
                        var formSchool = allSubSchool.FirstOrDefault(e => e.id == Convert.ToInt32(strange2.y_contentB));
                        var toSchool = allSubSchool.FirstOrDefault(e => e.id == Convert.ToInt32(strange2.y_contentA));
                        if (formSchool != null)
                        {
                            strangeStr.Append("转出函授站:");
                            strangeStr.Append(formSchool.y_name);
                            strangeStr.Append("  转入函授站:");
                            strangeStr.Append(toSchool.y_name);
                        }
                    }
                    if (strange.Any(e => e.y_strangeType == 4))
                    {
                        strangeStr.Append("延期毕业");
                    }
                    //异动
                    sheet.GetRow(10).GetCell(4).SetCellValue(strangeStr.ToString());

                    //毕业证
                    sheet.GetRow(12).GetCell(4).SetCellValue(stuInfo.y_graduateNumber);

                    //插入毕业照片
                    if (stuInfo.y_img != null)
                    {
                        byte[] imgBytes = null;
                        using (FileStream fs = new FileStream(Server.MapPath(stuInfo.y_img), FileMode.Open))
                        {
                            imgBytes = new byte[fs.Length];
                            fs.Read(imgBytes, 0, imgBytes.Length);
                            fs.Close();
                        }
                        int pictureIdx = wk.AddPicture(imgBytes, PictureType.JPEG);
                        IDrawing patriarch = sheet.CreateDrawingPatriarch();
                        IClientAnchor anchor = new HSSFClientAnchor(-1000, -1000, 1000, 1000, 0, 4, 3, 14);
                        //把图片插到相应的位置
                        var pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    }

                    //成绩
                    if (stuScore != null && stuScore.Count() > 0)
                    {
                        int[,] keyValues = new int[11, 2] { { 0, 0 }, { 17, 0 }, { 17, 5 }, { 27, 0 }, { 27, 5 }, { 3, 11 }, { 3, 16 }, { 11, 11 }, { 11, 16 }, { 19, 11 }, { 19, 16 } };

                        for (int i = 1; i <= stuScore.Max(e => e.Term); i++)
                        {
                            for (int j = 0; j < stuScore.Count(e => e.Term == i); j++)
                            {
                                var termScore = stuScore.Where(e => e.Term == i).OrderBy(e => e.CourseId).ToArray();
                                string courseName = termScore[j].CourseName;
                                if (termScore[j].y_isMain)
                                {
                                    courseName += "*";
                                }
                                if (termScore[j].y_sampleexam == true)
                                {
                                    courseName += "*";
                                }
                                sheet.GetRow(keyValues[i, 0] + j).GetCell(keyValues[i, 1]).SetCellValue(courseName);
                                sheet.GetRow(keyValues[i, 0] + j).GetCell(keyValues[i, 1] + 3).SetCellValue(termScore[j].SelfPeriod + termScore[j].TeaPeriod);
                                sheet.GetRow(keyValues[i, 0] + j).GetCell(keyValues[i, 1] + 4).SetCellValue(Math.Round(termScore[j].TotalScore.HasValue ? (double)termScore[j].TotalScore.Value : 0.0d, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                    }
                    // 打包目录
                    string filePath = Server.MapPath($"~/File/Dowon/{year}");
                    if (!System.IO.Directory.Exists(filePath))
                        System.IO.Directory.CreateDirectory(filePath);
                    // 教学点目录
                    string filePath2 = Server.MapPath($"~/File/Dowon/{year}/{stuInfo.schoolName}");
                    if (!System.IO.Directory.Exists(filePath2))
                        System.IO.Directory.CreateDirectory(filePath2);
                    string fileName = $"{ stuInfo.y_stuNum}_{stuInfo.y_name}.xlsx";
                    using (FileStream file = new FileStream(filePath2 + "\\" + fileName, FileMode.Create))
                    {
                        wk.Write(file);  //创建test.xls文件。
                        file.Close();
                    }
                }
                return CompressZip(2016);
            }
        }

        /// <summary>
        /// zip压缩
        /// </summary>
        /// <param name="sourceDir"></param>
        public string CompressZip(int year)
        {
            string source = Server.MapPath("~/File/Dowon/" + year + "/");
            using (var archive = new ZipFile(Encoding.Default))
            {
                DirectoryInfo dic = new DirectoryInfo(source);
                DirectoryInfo[] dics = dic.GetDirectories();
                foreach (var item in dics)
                {
                    foreach (var file in item.GetFiles())
                    {
                        archive.AddEntry(item.Name + '/' + file.Name, file.OpenRead());
                    }
                }
                string fileName = DateTime.Now.ToString("yyyyMMdd");
                string zipFile = Server.MapPath("~/File/Dowon/" + fileName + ".zip");
                FileStream fs_scratchPath = new FileStream(zipFile, FileMode.OpenOrCreate, FileAccess.Write);
                archive.Save(fs_scratchPath);
                return fileName;
            }
        }

        /// <summary>
        ///科技师范学籍表--批量打印
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuStatusScore2()
        {
            using (var yunEntities = new IYunEntities())
            {
                var namenumcard = Request["namenumcard"];
                var subschoolid = Request["subschoolid"];
                var majorliid = Request["majorliid"];
                var year = Request["year"];
                var state = Request["ste"];
                var edutype = Request["edutype"];
                const int isnotdel = (int)YesOrNo.No;

                var stuid = Request["stuid"];
                int[] ids = null;
                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                }
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                    .Where(u => u.y_isdel == isnotdel).AsQueryable();

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    stuInfo = stuInfo.Where(u => ids.Contains(u.id));
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);

                    stuInfo = stuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    stuInfo = stuInfo.Where(u => u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard));
                }

                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    stuInfo = stuInfo.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subschoolid) && !subschoolid.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subschoolid);
                    stuInfo = stuInfo.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorliid) && !majorliid.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorliid);
                    stuInfo = stuInfo.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(edutype);
                    stuInfo = stuInfo.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(state) && !state.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(state);
                    stuInfo = stuInfo.Where(u => u.y_stuStateId == stuTypeint);
                }
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                ////获取异动信息
                var strange = yunEntities.VW_Strange.OrderByDescending(u => u.y_majorLibId)
                    .Where(u => u.y_approvalStatus == status);


                ////获取学生成绩
                ////获取学籍教学计划

                List<StuScoreStatistics_CourseList> StuScoreList = new List<StuScoreStatistics_CourseList>();
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();
                foreach (var stulist in stuInfo)
                {
                    var StuScores = new StuScoreStatistics_CourseList();
                    List<ScoreStatistics_Course> stuScore = new List<ScoreStatistics_Course>();
                    var stu = yunEntities.VW_StuInfo.Where(u => u.id == stulist.id).AsQueryable();
                    #region  单个学生教学计划和成绩
                    var list = stu.GroupJoin(classCourse,
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

                    var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                    if (lists.All(u => u.s.classCourse != null))
                    {
                        stuScore = lists
                            .GroupBy(
                                u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                            .Select(
                                u =>
                                    new ScoreStatistics_Course
                                    {
                                        CourseName = u.Key.YD_Edu_Course.y_name,
                                        CourseId = u.Key.YD_Edu_Course.id,
                                        Term = u.Key.y_team,
                                        TotalScore = u.FirstOrDefault().score.y_totalScore
                                    })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();

                        //批量添加学生对应成绩和教学计划
                        StuScores.stulist = stulist;
                        StuScores.ScoreList = stuScore;
                        StuScoreList.Add(StuScores);
                    }
                    #endregion
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;

                ViewData["list"] = stuInfo.ToList();
                ViewData["strange"] = strange.ToList();

                return View(StuScoreList);
            }
        }

        /// <summary>
        /// 学生学籍成绩表 --科技师范学籍表单个打印
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuStatusScoreOne(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                //获取异动信息
                var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).FirstOrDefault(u => u.y_stuId == id);
                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                return View(stuScore);
            }
        }

        /// <summary>
        /// 学生学籍成绩表 --赣南师范学籍表批量
        /// </summary>
        /// <returns></returns>
        public ActionResult GNSFDXStuStatusScore()
        {

            using (var yunEntities = new IYunEntities())
            {
                var namenumcard = Request["namenumcard"];
                var subschoolid = Request["subschoolid"];
                var majorliid = Request["majorliid"];
                var year = Request["year"];
                var state = Request["ste"];
                var edutype = Request["edutype"];
                const int isnotdel = (int)YesOrNo.No;
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                var stuid = Request["stuid"];
                int[] ids = null;
                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                }

                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId).Where(u => u.y_isdel == isnotdel).AsQueryable();

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    stuInfo = stuInfo.Where(u => ids.Contains(u.id));
                }

                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);

                    stuInfo = stuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    stuInfo = stuInfo.Where(u => u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard));
                }

                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    stuInfo = stuInfo.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subschoolid) && !subschoolid.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subschoolid);
                    stuInfo = stuInfo.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorliid) && !majorliid.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorliid);
                    stuInfo = stuInfo.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(edutype);
                    stuInfo = stuInfo.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(state) && !state.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(state);
                    stuInfo = stuInfo.Where(u => u.y_stuStateId == stuTypeint);
                }
                var isJige = Request["isJige"];
                if (!string.IsNullOrWhiteSpace(isJige) && !isJige.Equals("0"))
                {
                    if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
                    {
                        //var stulist = stuInfo.Select(u => u.id);

                        //var scorelist = yunEntities.YD_Edu_Score.Where(u => stulist.Contains(u.y_stuId))
                        //    .GroupBy(score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId })
                        //    .Select(score => score.OrderByDescending(u => u.id).FirstOrDefault());

                        //var listss =
                        //     scorelist
                        //        .GroupBy(u => u.y_stuId)
                        //         .Where(u => u.All(k => k != null && k.y_totalScore >= 60))
                        //         .Select(u => u.Key).ToList();

                        var sql = "select y_stuId from YD_Edu_Score where id in( select MAX(id) from YD_Edu_Score where y_totalScore<60 group by y_stuId,y_term,y_courseId)";

                        var stuidlist = yunEntities.Database.SqlQuery<int>(sql).ToList();

                        var stul = stuInfo.ToList();

                        if (isJige == "1") //及格
                        {
                            stuInfo = stul.Where(u => !stuidlist.Contains(u.id)).AsQueryable();
                        }
                        else
                        {
                            stuInfo = stul.Where(u => stuidlist.Contains(u.id)).AsQueryable();
                        }

                    }
                }
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('没有找到学生');window.location.href='" + reurl + "'</script>");
                }

                ////获取学生成绩
                ////获取学籍教学计划
                List<StuScoreStatistics_CourseList> StuScoreList = new List<StuScoreStatistics_CourseList>();
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();
                foreach (var stulist in stuInfo)
                {
                    var StuScores = new StuScoreStatistics_CourseList();
                    List<ScoreStatistics_Course> stuScore = new List<ScoreStatistics_Course>();
                    var stu = yunEntities.VW_StuInfo.Where(u => u.id == stulist.id).AsQueryable();
                    #region  单个学生教学计划和成绩
                    var list = stu.GroupJoin(classCourse,
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

                    var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                    if (lists.All(u => u.s.classCourse != null))
                    {
                        stuScore = lists
                            .GroupBy(
                                u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                            .Select(
                                u =>
                                    new ScoreStatistics_Course
                                    {
                                        CourseName = u.Key.YD_Edu_Course.y_name,
                                        CourseId = u.Key.YD_Edu_Course.id,
                                        Term = u.Key.y_team,
                                        TotalScore = u.FirstOrDefault().score.y_totalScore
                                    })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();

                        //批量添加学生对应成绩和教学计划
                        StuScores.stulist = stulist;
                        StuScores.ScoreList = stuScore;

                        StuScoreList.Add(StuScores);

                    }
                    #endregion
                }

                //ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;

                ViewData["list"] = stuInfo.ToList();
                //ViewData["strange"] = strange.ToList();

                return View(StuScoreList);
            }

        }

        /// <summary>
        /// 学生学籍成绩表 --赣南师范学籍表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuStatusScore3(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;

                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }

                //ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                return View(stuScore);
            }
        }

        /// <summary>
        /// 中医药学生学籍成绩表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ZYYStuScore(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                //获取异动信息
                var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).FirstOrDefault(u => u.y_stuId == id);
                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                return View(stuScore);
            }
        }

        /// <summary>
        /// 华东交大学籍表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuStatusScoreOne2(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                //获取异动信息
                var strange = yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status).FirstOrDefault(u => u.y_stuId == id);
                //获取学生成绩
                //获取学籍教学计划

                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

                var list = stu.GroupJoin(classCourse,
                    s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
                    c =>
                        new
                        {
                            c.YD_TeaPlan_Class.y_majorId,
                            y_inYear = c.YD_TeaPlan_Class.y_year,
                            c.YD_TeaPlan_Class.y_subSchoolId,
                        }, (s, c) => new { s, c })
                    .SelectMany(
                        xy => xy.c.DefaultIfEmpty(),
                        (x, y) => new { stu = x.s, classCourse = y });

                var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                List<ScoreStatistics_Course> stuScore = null;
                if (lists.All(u => u.s.classCourse != null))
                {
                    stuScore = lists
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    TotalScore = u.FirstOrDefault().score.y_totalScore == 100 ? 92 : u.FirstOrDefault().score.y_totalScore,
                                    SelfPeriod = u.FirstOrDefault().s.classCourse.y_selfPeriod ?? 0,
                                    TeaPeriod = u.FirstOrDefault().s.classCourse.y_teaPeriod ?? 0,
                                    y_isMain = u.FirstOrDefault().s.classCourse.y_isMain,
                                    y_sampleexam = u.FirstOrDefault().s.classCourse.y_sampleexam
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;
                ViewBag.stuScore = stuScore;
                return View();
            }
        }

        public ActionResult PrintStuStatusScores()
        {
            using (var yunEntities = new IYunEntities())
            {
                var namenumcard = Request["namenumcard"];
                var subschoolid = Request["subschoolid"];
                var majorliid = Request["majorliid"];
                var year = Request["year"];
                var state = Request["ste"];
                var edutype = Request["edutype"];
                const int isnotdel = (int)YesOrNo.No;

                var stuid = Request["stuid"];

                int[] ids = null;
                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                }

                //获取学生对象
                var stuInfo = yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                    .Where(u => u.y_isdel == isnotdel).AsQueryable();

                if (!string.IsNullOrWhiteSpace(stuid))
                {
                    stuInfo = stuInfo.Where(u => ids.Contains(u.id));
                }

                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);

                    stuInfo = stuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    stuInfo = stuInfo.Where(u => u.y_name.Contains(namenumcard) || u.y_cardId.Contains(namenumcard));
                }

                if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(year);
                    stuInfo = stuInfo.Where(u => u.y_inYear == enrollYearint);
                }

                if (!string.IsNullOrWhiteSpace(subschoolid) && !subschoolid.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subschoolid);
                    stuInfo = stuInfo.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(majorliid) && !majorliid.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorliid);
                    stuInfo = stuInfo.Where(u => u.y_majorLibId == majorLibraryint);
                }

                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(edutype);
                    stuInfo = stuInfo.Where(u => u.y_eduTypeId == eduTypeint);
                }
                if (!string.IsNullOrWhiteSpace(state) && !state.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(state);
                    stuInfo = stuInfo.Where(u => u.y_stuStateId == stuTypeint);
                }
                if (stuInfo == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                const int status = (int)ApprovaState.HadApprova;
                ////获取异动信息
                var strange = yunEntities.VW_Strange.OrderByDescending(u => u.y_majorLibId)
                    .Where(u => u.y_approvalStatus == status);


                ////获取学生成绩
                ////获取学籍教学计划

                List<StuScoreStatistics_CourseList> StuScoreList = new List<StuScoreStatistics_CourseList>();
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();
                foreach (var stulist in stuInfo)
                {
                    var StuScores = new StuScoreStatistics_CourseList();
                    List<ScoreStatistics_Course> stuScore = new List<ScoreStatistics_Course>();
                    var stu = yunEntities.VW_StuInfo.Where(u => u.id == stulist.id).AsQueryable();
                    #region  单个学生教学计划和成绩
                    var list = stu.GroupJoin(classCourse,
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

                    var scorelist = yunEntities.VW_Score.OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                    if (lists.All(u => u.s.classCourse != null))
                    {
                        stuScore = lists
                            .GroupBy(
                                u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                            .Select(
                                u =>
                                    new ScoreStatistics_Course
                                    {
                                        CourseName = u.Key.YD_Edu_Course.y_name,
                                        CourseId = u.Key.YD_Edu_Course.id,
                                        Term = u.Key.y_team,
                                        TotalScore = u.FirstOrDefault().score.y_totalScore
                                    })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();

                        //批量添加学生对应成绩和教学计划
                        StuScores.stulist = stulist;
                        StuScores.ScoreList = stuScore;
                        StuScoreList.Add(StuScores);
                    }
                    #endregion
                }

                ViewBag.strange = strange;
                ViewBag.stuInfo = stuInfo;

                ViewData["list"] = stuInfo.ToList();
                ViewData["strange"] = strange.ToList();

                return View(StuScoreList);
            }
        }
        #endregion

        public bool StudentScorellist(VW_StuInfo stuInfo, Dictionary<Dictionary<int, List<VW_TeachPlanDes>>, VW_StuInfo> userprolists, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms1, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms2, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms3, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms4, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms5, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms6, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms7, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms8, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms9, Dictionary<Dictionary<int, VW_Score>, VW_StuInfo> terms10, IYunEntities yunEntities)
        {
            #region
            //获取学生成绩
            //获取单个学籍教学计划
            var teachPlanList = yunEntities.YD_Edu_MajorTeachPlan.Where(
                u => u.y_majorId == stuInfo.y_majorId && u.y_year == stuInfo.y_inYear && u.y_teaPlanType == 2)
                .ToList();
            var teachPlanDess = new Dictionary<int, List<VW_TeachPlanDes>>(); //各个学期的教学计划详细
            for (var i = 0; i < teachPlanList.Count; i++)
            {
                var teachPlanId = teachPlanList[i].y_teachPlanId;
                var teachPlan = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == teachPlanId);
                if (teachPlan != null && !teachPlanDess.ContainsKey(teachPlan.y_term))
                {
                    teachPlanDess[teachPlan.y_term] =
                        yunEntities.VW_TeachPlanDes.Where(u => u.y_teaPlanId == teachPlanId).ToList();
                }
            }
            userprolists.Add(teachPlanDess, stuInfo);
            var term1 = new Dictionary<int, VW_Score>();
            var term2 = new Dictionary<int, VW_Score>();
            var term3 = new Dictionary<int, VW_Score>();
            var term4 = new Dictionary<int, VW_Score>();
            var term5 = new Dictionary<int, VW_Score>();
            var term6 = new Dictionary<int, VW_Score>();
            var term7 = new Dictionary<int, VW_Score>();
            var term8 = new Dictionary<int, VW_Score>();
            var term9 = new Dictionary<int, VW_Score>();
            var term10 = new Dictionary<int, VW_Score>();
            if (teachPlanDess.ContainsKey(1))
            {
                for (var i = 0; i < teachPlanDess[1].Count; i++) //第一学期成绩
                {
                    var courseId = teachPlanDess[1][i].y_courseId;
                    term1[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第一学期成绩
            terms1.Add(term1, stuInfo);
            if (teachPlanDess.ContainsKey(2))
            {
                for (var i = 0; i < teachPlanDess[2].Count; i++) //第二学期成绩
                {
                    var courseId = teachPlanDess[2][i].y_courseId;
                    term2[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第二学期成绩
            terms2.Add(term2, stuInfo);
            if (teachPlanDess.ContainsKey(3))
            {
                for (var i = 0; i < teachPlanDess[3].Count; i++) //第三学期成绩
                {
                    var courseId = teachPlanDess[3][i].y_courseId;
                    term3[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第三学期成绩
            terms3.Add(term3, stuInfo);
            if (teachPlanDess.ContainsKey(4))
            {
                for (var i = 0; i < teachPlanDess[4].Count; i++) //第四学期成绩
                {
                    var courseId = teachPlanDess[4][i].y_courseId;
                    term4[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第四学期成绩
            terms4.Add(term4, stuInfo);
            if (teachPlanDess.ContainsKey(5))
            {
                for (var i = 0; i < teachPlanDess[5].Count; i++) //第五学期成绩
                {
                    var courseId = teachPlanDess[5][i].y_courseId;
                    term5[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第五学期成绩
            terms5.Add(term5, stuInfo);
            if (teachPlanDess.ContainsKey(6))
            {
                for (var i = 0; i < teachPlanDess[6].Count; i++) //第六学期成绩
                {
                    var courseId = teachPlanDess[6][i].y_courseId;
                    term6[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第六学期成绩
            terms6.Add(term6, stuInfo);
            if (teachPlanDess.ContainsKey(7))
            {
                for (var i = 0; i < teachPlanDess[7].Count; i++) //第七学期成绩
                {
                    var courseId = teachPlanDess[7][i].y_courseId;
                    term7[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第七学期成绩
            terms7.Add(term7, stuInfo);
            if (teachPlanDess.ContainsKey(8))
            {
                for (var i = 0; i < teachPlanDess[8].Count; i++) //第八学期成绩
                {
                    var courseId = teachPlanDess[8][i].y_courseId;
                    term8[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第八学期成绩
            terms8.Add(term8, stuInfo);
            if (teachPlanDess.ContainsKey(9))
            {
                for (var i = 0; i < teachPlanDess[9].Count; i++) //第九学期成绩
                {
                    var courseId = teachPlanDess[9][i].y_courseId;
                    term9[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第九学期成绩
            terms9.Add(term9, stuInfo);
            if (teachPlanDess.ContainsKey(10))
            {
                for (var i = 0; i < teachPlanDess[10].Count; i++) //第十学期成绩
                {
                    var courseId = teachPlanDess[10][i].y_courseId;
                    term10[courseId] =
                        yunEntities.VW_Score.FirstOrDefault(
                            u => u.y_stuId == stuInfo.id && u.y_courseId == courseId);
                }
            }
            //得到单个学生第十学期成绩
            terms10.Add(term10, stuInfo);
            #endregion
            return true;
        }
        #endregion     

        #region 新生统计

        public ActionResult NewStuStatist()
        {
            #region 权限验证
            var power = SafePowerPage("/Student/NewStuStatist");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
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
                ViewBag.adminroleid = YdAdminRoleId;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
            }

            return View();
        }

        public ActionResult NewStuStatistics()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/NewStuStatist");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return RedirectToAction("Index", "AdminBase");
            }
            #endregion

            var xinshen = ConfigurationManager.AppSettings["xinsheng"];
            string sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
            using (var ad = new IYunEntities())
            {
                var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                return View(list);
            }
        }

        #endregion

        #region 函授站新生统计

        public ActionResult NewStuStatistics2()
        {

            #region 权限验证

            var power = SafePowerPage("/Student/NewStuStatist");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
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

            using (var ad = new IYunEntities())
            {
                var schoolid = 0;
                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;
                }
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                string sql;

                if (schoolid == 0)
                {
                    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen +
                          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid +
                          " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                }
                ViewData["schoolid"] = schoolid;
                var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult NewStuStatistics2Post(int id)
        {
            using (var ad = new IYunEntities())
            {
                var xinshen = ConfigurationManager.AppSettings["xinsheng"];
                var schoolid = id;
                if (!IsLogin())
                {
                    return Redirect("/AdminBase/Index");
                }
                string sql;

                if (YdAdminRoleId == 4)
                {
                    schoolid = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;

                    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid +
                         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }
                else
                {
                    if (schoolid == 0)
                    {
                        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen +
                             " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    }
                    else
                    {
                        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " and y_subSchoolId = " + schoolid +
                             " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    }
                }

                ViewData["schoolid"] = schoolid;
                var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();
                return PartialView(list);
            }
        }

        [HttpPost]
        public string DownNewStuStatistics()
        {
            var xinshen = ConfigurationManager.AppSettings["xinsheng"];
            string sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
            using (var ad = new IYunEntities())
            {
                var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();

                //int i = 1;
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/全校新生统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变
                //var filename1 = "File/Dowon/全校新生统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }


        }
        [HttpPost]
        public string DownNewStuStatistics2(int id)
        {
            var xinshen = ConfigurationManager.AppSettings["xinsheng"];
            string sql;


            using (var ad = new IYunEntities())
            {
                if (id == 0)
                {
                    if (!IsLogin())
                    {
                        //var reurl = Request.UrlReferrer.ToString();
                        var reurl = "/AdminBase/Index";
                        return @"alert('错误');window.location.href='" + reurl + "';";
                    }

                    if (YdAdminRoleId == 4)
                    {
                        id = ad.YD_Sys_AdminSubLink.First(u => u.y_adminId == YdAdminId).y_subSchoolId;

                        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " and y_subSchoolId = " + id +
                              " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    }
                    else
                    {
                        sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen +
                             " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                    }
                }
                else
                {
                    sql = @"select y_eduTypeCode, majorLibraryCode, y_stuTypeCode, COUNT(*) as counts
                          , max([eduTypeName]) as cc,max([majorLibraryName]) as zy,max([stuTypeName]) as xs  from[VW_StuInfo]
                           where y_inyear = " + xinshen + " and y_subSchoolId = " + id +
                         " group by y_eduTypeCode,majorLibraryCode,y_stuTypeCode order by y_eduTypeCode, majorLibraryCode, y_stuTypeCode";
                }

                var list = ad.Database.SqlQuery<NewStuStatistics>(sql).ToList();

                //int i = 1;
                var lists = list.Select(u => new { u.cc, u.zy, u.xs, u.counts, bz = "" });

                var model =
                    FileHelper.ToDataTable(lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var fileName1 = "/新生统计表" + ".xls";      //todo:改变
                var fileName = dirPath + fileName1;                               //todo:改变
                using (var excelHelper = new ExcelHelper(fileName))
                {
                    var ht = new Hashtable
                    {
                        {"cc", "层次"},
                        {"zy", "专业"},
                        {"xs", "学习形式"},
                        {"counts", "录取人数"},
                        {"bz", "备注"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + fileName1;  //todo:改变
                        return url;
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return @"alert('错误');window.location.href='" + reurl + "';";
                }
            }

        }

        #endregion

        public ActionResult HandSubSchool(int id)
        {
            #region 权限验证
            var power = SafePowerPage("/Student/NewlyStudentInfo");
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

            using (var ad = new IYunEntities())
            {
                var student = ad.YD_Sts_StuInfo.First(u => u.id == id);
                var schools = ad.YD_Sts_SubSchoolStuInfo.Where(u => u.y_cardId == student.y_cardId && u.y_isdel == 1).Include(u => u.YD_Sys_SubSchool).ToList();
                ViewData["schools"] = schools;
                ViewData["student"] = student;
                ViewBag.modulePowers = GetChildModulePower(ad, 1); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        [HttpPost]
        public string SetSchool(int schoolid, int stuid)
        {
            using (var ad = new IYunEntities())
            {
                var student = ad.YD_Sts_StuInfo.First(u => u.id == stuid);
                student.y_subSchoolId = schoolid;
                ad.Entry(student).State = EntityState.Modified;
                ad.SaveChanges();

                LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",更新学生所属函授站,修改学生信息表,ID:" + student.id + ",函授站id:" + student.y_subSchoolId + ",方法:SetSchool");
            }

            return "true";
        }

        #region  电子照片管理


        public ActionResult PhotoManager(int id = 1)
        {
            #region 权限验证        

            var power = SafePowerPage("/Student/PhotoManager");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var stuState = Request["StuState"]; //学籍状态
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"]; //层次
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];


                ViewBag.schoolname = schoolname;
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo
                    .Include(u => u.YD_Edu_Major)
                    .Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Edu_StuState)
                    .OrderByDescending(u => u.y_inYear).Where(u => u.y_isdel == isnotdel && u.y_stuNum != "");
                //不显示未注册和注册待审核学生
                YD_Edu_StuState state = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "未注册");
                var statecheck = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "注册待审核");
                if (state != null)
                {
                    list = list.Where(u => u.y_stuStateId != state.id);
                }
                if (statecheck != null)
                {
                    list = list.Where(u => u.y_stuStateId != statecheck.id);
                }
                var ste = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "在读");
                if (ste != null)
                {
                    ViewBag.ste = ste.id;
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));

                    ViewBag.subschoolid = subSchoolIds.FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard || u.y_cardId == namenumcard || u.y_examNum == namenumcard);

                    ViewBag.namenumcard = namenumcard;
                }

                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);

                    ViewBag.inyear = enrollYearint;
                }
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                    ViewBag.subschoolid = subSchoolint;
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
                    ViewBag.majorliid = majorLibraryint;
                }
                if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
                {
                    var stuStateint = Convert.ToInt32(stuState);
                    list = list.Where(u => u.y_stuStateId == stuStateint);
                    ViewBag.stuStateint = stuStateint;
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var eduTypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
                    ViewBag.edutype = eduTypeint;
                }
                if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
                {
                    var stuTypeint = Convert.ToInt32(stuType);
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
                }
                var isJige = Request["isJige"];
                if (!string.IsNullOrWhiteSpace(isJige) && !isJige.Equals("0"))
                {
                    if (schoolname == ComEnum.SchoolName.GNSFDX.ToString())
                    {
                        var stulist = list.Select(u => u.id);

                        //var scorelist = yunEntities.YD_Edu_Score.Where(u=>stulist.Contains(u.y_stuId))
                        //    .GroupBy(score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId })
                        //    .Select(score => score.OrderByDescending(u => u.id).FirstOrDefault());

                        //var listss =
                        //     scorelist
                        //        .GroupBy(u => u.y_stuId)
                        //         .Where(u => u.All(k => k != null && k.y_totalScore >= 60))
                        //         .Select(u => u.Key).ToList();


                        var sql = "select y_stuId from YD_Edu_Score where id in( select MAX(id) from YD_Edu_Score where y_totalScore<60 group by y_stuId,y_term,y_courseId)";

                        var stuidlist = yunEntities.Database.SqlQuery<int>(sql).ToList();
                        var stul = list.ToList();

                        if (isJige == "1") //及格
                        {
                            list = stul.Where(u => !stuidlist.Contains(u.id)).AsQueryable();
                        }
                        else
                        {
                            list = stul.Where(u => stuidlist.Contains(u.id)).AsQueryable();
                        }
                    }
                }
                ViewBag.admin = YdAdminRoleId;

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                {
                    return PartialView("PhotoList", model);
                }
                return View(model);
            }
        }



        //电子照片上传片页
        public ActionResult StudentDes2(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/photoManager");
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

            if (!id.HasValue)
            {
                return RedirectToAction("photoManager");
            }

            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 1); //根据父栏目ID获取兄弟栏目
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == id.Value);
                //获取异动信息
                var strange =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status)
                        .FirstOrDefault(u => u.y_stuId == id.Value);
                ViewBag.strange = strange;
                if (student == null)
                {
                    return RedirectToAction("photoManager");
                }
                ViewData["student"] = student;
            }
            return View();
        }

        //单个审核
        public JsonResult Shenhe(int stuId, int y_ImgIsok)
        {
            #region 权限验证

            var power = SafePowerPage("/Student/photoManager");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {


                return Json(new { Isok = false, msg = "没有权限" });
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                var stu = ad.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stuId);

                stu.y_ImgIsok = y_ImgIsok;

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { Isok = true, msg = "审核成功！" }); }

                else { return Json(new { Isok = false, msg = "审核失败！" }); }
            }




        }

        //批量审核通过
        public JsonResult ShenhequanYes()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/photoManager");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {


                return Json(new { Isok = false, msg = "没有权限" });
            }

            #endregion

            var check = Request["check"];

            log.Info($"{this.GetType()}{check}");
            if (check == null)
            {
                return Json(new { Isok = false, msg = "未选择审核项" });
            }



            var ids = check.Split(',');

            using (var ad = new IYunEntities())
            {
                foreach (var id in ids)
                {

                    var Id = Convert.ToInt32(id);
                    var stu = ad.YD_Sts_StuInfo.FirstOrDefault(u => u.id == Id);

                    stu.y_ImgIsok = 1;

                }

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { Isok = true, msg = "审核成功！" }); }

                else { return Json(new { Isok = false, msg = "审核失败！" }); }
            }




        }


        //批量审核不通过
        public JsonResult ShenhequanNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Student/photoManager");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {


                return Json(new { Isok = false, msg = "没有权限" });
            }

            #endregion

            var check = Request["check"];

            log.Info($"{this.GetType()}{check}");
            if (check == null)
            {
                return Json(new { Isok = false, msg = "未选择审核项" });
            }



            var ids = check.Split(',');

            using (var ad = new IYunEntities())
            {
                foreach (var id in ids)
                {

                    var Id = Convert.ToInt32(id);
                    var stu = ad.YD_Sts_StuInfo.FirstOrDefault(u => u.id == Id);

                    stu.y_ImgIsok = 0;

                }

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { Isok = true, msg = "审核成功！" }); }

                else { return Json(new { Isok = false, msg = "审核失败！" }); }
            }




        }

        #endregion

        /// <summary>
        /// 学年页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SchoolYear()
        {
            //GetSchoolYear
            using (IYunEntities db = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(db, 1); //根据父栏目ID获取兄弟栏目
                var settings = db.YT_RegisterSettings.ToList();
                return View(settings);
            }
        }

        public ActionResult InsertOrUpdateSchoolYear(string id)
        {
            using (var db = new IYunEntities())
            {
                if (id != "" && id != null)
                {
                    var Id = Convert.ToInt32(id);
                    ViewBag.YT_RegisterSettings = db.YT_RegisterSettings.FirstOrDefault(u => u.id == Id);
                }
            }
            return View();
        }


        /// <summary>
        /// 新增或增加学年
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertOrUpdateSchoolYear(int? Id, int schoolYear, DateTime startTime, DateTime endTime, string describe)
        {
            using (IYunEntities db = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(db, 1); //根据父栏目ID获取兄弟栏目

                if (Id.HasValue && Id != 0)
                {
                    var Settings = db.YT_RegisterSettings.FirstOrDefault(u => u.id == Id.Value);
                    Settings.y_inyear = schoolYear;
                    Settings.y_starttime = startTime;
                    Settings.y_endtime = endTime;
                    Settings.y_Remarks = describe;

                }
                else
                {
                    var Settings = new YT_RegisterSettings();
                    Settings.y_inyear = schoolYear;
                    Settings.y_starttime = startTime;
                    Settings.y_endtime = endTime;
                    Settings.y_Remarks = describe;


                    db.Entry(Settings).State = EntityState.Added;
                }
                var isOk = db.SaveChanges();
                if (isOk > 0)
                {
                    return Json(new { status = "ok", msg = "InsertOrUpdateOk" });
                }
                else
                {
                    return Json(new { status = "fail", msg = "Error" });
                }
            }
        }

        public string DeleteSchoolYear(int schoolYearId)
        {
            using (IYunEntities db = new IYunEntities())
            {
                var entity = db.YT_RegisterSettings.SingleOrDefault(e => e.id == schoolYearId);
                db.YT_RegisterSettings.Remove(entity);

                var j = db.SaveChanges();

                if (j > 0) { return "ok"; }
                else { return "删除失败！"; }
            }
        }
        //修改或新增考生页面
        public ActionResult InsertOrUpdateSubSchoolStuView(int? id)
        {
            if (!id.HasValue)
            {
                return View(new YD_Sts_SubSchoolStuInfo { id = 0 });
            }
            using (IYunEntities db = new IYunEntities())
            {
                var stu = db.YD_Sts_SubSchoolStuInfo.FirstOrDefault(e => e.id == id.Value);
                return View(stu);
            }
        }

        public ActionResult InsertOrUpdateSubSchoolStu(YD_Sts_SubSchoolStuInfo stu)
        {
            using (IYunEntities db = new IYunEntities())
            {
                int result = 0;
                if (stu.id == 0)
                {
                    if (db.YD_Sts_SubSchoolStuInfo.Any(e => e.y_cardId == stu.y_cardId && e.y_hide == 1 && e.y_isdel == 1 ))
                    {
                        return Json(new { status = "fail", msg = "已经存在相同身份证" });
                    }
                    db.YD_Sts_SubSchoolStuInfo.Add(stu);
                    result = db.SaveChanges();
                }
                else
                {
                    var stuInfo = db.YD_Sts_SubSchoolStuInfo.FirstOrDefault(e => e.id == stu.id);
                    stuInfo.y_cardId = stu.y_cardId;
                    stuInfo.y_examNum = stu.y_examNum;
                    stuInfo.y_name = stu.y_name;
                    stuInfo.y_subSchoolId = stu.y_subSchoolId;
                    stuInfo.y_year = stu.y_year;
                    result = db.SaveChanges();
                }
                if (result == 1)
                {
                    return Json(new { status = "ok" });
                }
                else
                {
                    return Json(new { status = "fail", msg = "保存失败" });
                }
            }
        }

        public ActionResult SubStuStatist()
        {
            #region 权限验证
            //var power = SafePowerPage("/Student/StuNumSys");
            //if (!IsLogin())
            //{
            //    Redirect("/AdminBase/Index");
            //}
            //if (power == null || power.y_select == (int)PowerState.Disable)
            //{
            //    var reurl = Request.UrlReferrer.ToString();
            //    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            //}
            #endregion
            using (var db = new IYunEntities())
            {
               List<SubStuStatistView> result = db.YD_Sts_SubSchoolStuInfo.Include(x => x.YD_Sys_SubSchool).
                    Where(x => x.id>0).Select(x=>new {
                        schoolname = x.YD_Sys_SubSchool.y_name,
                        stuName = x.y_name
                    }).GroupBy(x=>new { x.schoolname}).Select(x=>new SubStuStatistView {
                        schoolName = x.Key.schoolname,
                        count = x.Count()
                    }).ToList();
                return View(result);
            }

        }
    }

    public class SubStuStatistView
    {
        public string schoolName { get; set; }
        public string majorName { get; set; }
        public int count { get; set; }
    }

    public class xinshengAllStudent
    {
        public string eduTypeName { get; set; }
        public string majorLibraryName { get; set; }
        public string stuTypeName { get; set; }
        public int count { get; set; }
        public string y_examNum { get; set; }
        public string y_name { get; set; }
        public string y_sex { get; set; }
        public string y_cardId { get; set; }
        public string y_postalcode { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public string schoolName { get; set; }
    }
    public class xinshengSubStudent
    {
        public string eduTypeName { get; set; }
        public string majorLibraryName { get; set; }
        public string stuTypeName { get; set; }
        public int count { get; set; }
        public string y_examNum { get; set; }
        public string y_name { get; set; }
        public string y_sex { get; set; }
        public string y_cardId { get; set; }
        public string y_postalcode { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
    }
    public class StuList
    {
        public string y_name { get; set; }
        public string y_sex { get; set; }
        public string y_stuNum { get; set; }
        public string y_examNum { get; set; }
        public string majorLibraryName { get; set; }
        public string schoolCode { get; set; }
        public string majorLibraryCode { get; set; }
        public string eduTypeName { get; set; }
        public string stuTypeName { get; set; }
        public string schoolName { get; set; }
        public string y_cardId { get; set; }
        public string y_birthday { get; set; }
        public string nationName { get; set; }
        public string politicsName { get; set; }
        public string stuStateName { get; set; }
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public int y_inYear { get; set; }
        public string y_graduateNumber { get; set; }
        public StuList(DateTime date)
        {
            y_birthday = date.ToString("yyyyMMdd");
        }
    }

    public class InvoiceeStuFee
    {
        public string y_stuNum { get; set; }
        public string y_name { get; set; }
        public int y_inYear { get; set; }
        public string schoolName { get; set; }
        public int y_feeYear { get; set; }
        public int y_needUpFee { get; set; }
        public int? y_invoiceMoney { get; set; }
        public string Invoiceename { get; set; }
    }
    public class StuRegistra
    {

        public int inYear { get; set; }
        public string stuName { get; set; }
        public string stuTypeName { get; set; }
        public string eduTypeName { get; set; }
        public string majorName { get; set; }
        public string subSchoolName { get; set; }
        public string stuStateName { get; set; }
        public string registerState { get; set; }
        public string Money { get; set; }

    }

    public class StudentInfoChangeB
    {
        public string StudentName { get; set; }
        public string y_sex { get; set; }
        public string y_cardId { get; set; }
        public string y_stuNum { get; set; }
        public string y_exNum { get; set; }
        public string schoolName { get; set; }
        public int y_inYear { get; set; }
        public string majorName { get; set; }//当前专业

        public string y_eduTypeName { get; set; }//当前层次名

        public string stuTypeName { get; set; }//当前学习形式名
        public string y_tel { get; set; }
        public string y_address { get; set; }
        public string strangeTypeName { get; set; }
        public Nullable<DateTime> y_applyTime { get; set; }
        public Nullable<DateTime> y_approvalTime { get; set; }
        public string y_contentAName { get; set; }//转出后专业

        public string y_contentAName2 { get; set; } //转出后学习形式
        public string y_contentAName3 { get; set; }//转出后层次
        public string y_contentBName { get; set; }//转出前专业

        public string y_contentBName2 { get; set; }
        public string y_contentBName3 { get; set; }

    }

    public class NewStuStatistics
    {
        public int index { get; set; }
        public string y_eduTypeCode { get; set; }
        public string majorLibraryCode { get; set; }
        public string y_stuTypeCode { get; set; }
        public int counts { get; set; }
        public string cc { get; set; }
        public string zy { get; set; }
        public string xs { get; set; }
    }
}