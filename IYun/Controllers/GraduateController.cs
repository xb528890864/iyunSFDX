using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Models;
using IYun.Object;
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
using System.Text;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using WebGrease.Css.Extensions;

namespace IYun.Controllers
{
    /// <summary>
    /// 毕业管理
    /// </summary>
    public class GraduateController : AdminBaseController
    {

        //
        // GET: /Graduate      
        /// <summary>
        /// 毕业申报视图控制
        /// </summary>
        /// <returns>视图</returns>
        //public ActionResult StudentGradInfo(int id = 1)
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Graduate/StudentGradInfo");
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
        //        if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
        //        }
        //        else
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
        //        }
        //        var stuState = Request["StuState"];
        //        var enrollYear = Request["EnrollYear"];
        //        var subSchool = Request["SubSchool"];
        //        var majorLibrary = Request["MajorLibrary"];
        //        var eduType = Request["EduType"];
        //        var stuType = Request["StuType"];
        //        var isok = Request["isok"]; //是否允许毕业
        //        var isup = Request["isup"]; //是否申请毕业
        //        var namenumcard = Request["namenumcard"];
        //        const int isnotdel = (int)YesOrNo.No;
        //        ViewBag.adminrole = YdAdminRoleId;
        //        IQueryable<YD_Sts_StuInfo> list =
        //            yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
        //                .Include(u => u.YD_Fee_StuFeeTb)
        //                .Include(u => u.YD_Edu_Major)
        //                .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 1 && u.y_studentType != 2).AsNoTracking();
        //        var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
        //        var schoolname = ConfigurationManager.AppSettings["SchoolName"];
        //        //if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
        //        //{

        //        //        list = list.Where(u => u.y_inYear == (xinshenyear - 2));

        //        //}
        //        if (!string.IsNullOrWhiteSpace(namenumcard))
        //        {
        //            list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard
        //            || u.y_cardId == namenumcard || u.y_examNum == namenumcard);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuState) && !stuState.Equals("0"))
        //        {
        //            var stuStateint = Convert.ToInt32(stuState);
        //            list = list.Where(u => u.y_stuStateId == stuStateint);
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
        //        if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
        //        {
        //            var majorLibraryint = Convert.ToInt32(majorLibrary);
        //            list = list.Where(u => u.YD_Edu_Major.y_majorLibId == majorLibraryint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
        //        {
        //            var eduTypeint = Convert.ToInt32(eduType);
        //            list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduTypeint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(stuType) && !stuType.Equals("0"))
        //        {
        //            var stuTypeint = Convert.ToInt32(stuType);
        //            list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeint);
        //        }

        //        var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable().AsNoTracking();

        //        var list1 = list.GroupJoin(classCourse,
        //            s => new { s.y_majorId, s.y_inYear, y_subSchoolId = s.y_subSchoolId.Value },
        //            c =>
        //                new
        //                {
        //                    c.YD_TeaPlan_Class.y_majorId,
        //                    y_inYear = c.YD_TeaPlan_Class.y_year,
        //                    c.YD_TeaPlan_Class.y_subSchoolId
        //                }, (s, c) => new { s, c })
        //            .SelectMany(
        //                xy => xy.c.DefaultIfEmpty(),
        //                (x, y) => new { stu = x.s, classCourse = y });

        //        var scorelist = yunEntities.YD_Edu_Score.OrderBy(u => u.id).AsQueryable().AsNoTracking();

        //        var lists = list1.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
        //            score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
        //            (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

        //        list = list.Where(
        //            u =>
        //                (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
        //                xinshenyear);

        //        if (!string.IsNullOrWhiteSpace(isok) && !isok.Equals("0"))
        //        {
        //            var listss =
        //                lists.GroupBy(u => u.s.stu)
        //                    .Where(u => u.All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100))
        //                    .Select(u => u.Key.id)
        //                    .ToList();
        //            if (isok == "1") //允许毕业
        //            {
        //                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
        //                {
        //                    list =
        //                        list.Where(
        //                            u =>
        //                                (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
        //                                xinshenyear &&
        //                                u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
        //                                u.YD_Edu_Major.y_stuYear && listss.Contains(u.id));
        //                }
        //                else
        //                {
        //                    list =
        //                       list.Where(
        //                           u =>
        //                               (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
        //                               xinshenyear &&
        //                               u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
        //                               u.YD_Edu_Major.y_stuYear && listss.Contains(u.id) && u.y_img != null);
        //                }

        //            }
        //            else if (isok == "2") //不允许毕业
        //            {
        //                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
        //                {
        //                    list =
        //                    list.Where(
        //                        u =>
        //                            !((u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
        //                              xinshenyear) ||
        //                            u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
        //                            u.YD_Edu_Major.y_stuYear || !listss.Contains(u.id));
        //                }
        //                else
        //                {
        //                    list =
        //                      list.Where(
        //                          u =>
        //                              (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
        //                              xinshenyear &&
        //                              u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
        //                              u.YD_Edu_Major.y_stuYear && listss.Contains(u.id) && u.y_img == null);
        //                }
        //            }
        //        }

        //        if (!string.IsNullOrWhiteSpace(isup) && !isup.Equals("0"))
        //        {
        //            if (isup == "1") //已申请毕业
        //            {
        //                list = list.Where(u => u.y_isgraduate == true);
        //            }
        //            else if (isup == "2") //未申请毕业
        //            {
        //                list = list.Where(u => u.y_isgraduate == null);
        //            }
        //        }

        //        if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
        //        {
        //            var subSchoolIds =
        //                yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).AsNoTracking()
        //                    .Select(u => u.y_subSchoolId)
        //                    .ToList();
        //            list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
        //        }
        //        ViewBag.admin = YdAdminRoleId;
        //        var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize



        //        list.ForEach(u =>
        //        {
        //            var graduateYear = u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1;
        //            //毕业年份
        //            var iscc = u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
        //                   u.YD_Edu_Major.y_stuYear; //各学年是否都注册
        //            var isimg = u.y_img != null;
        //            var isScore = lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100);
        //            if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
        //            {
        //                isScore = lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 59 && k.score.y_totalScore <= 100);
        //            }

        //            var isMain = false;
        //            //所有科目是否有成绩以及成绩是否都合格
        //            if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
        //            {
        //                isMain = graduateYear <= xinshenyear && iscc && isScore && isimg; //江西师范不需要判断是否有照片
        //            }
        //            else
        //            {
        //                isMain = graduateYear <= xinshenyear && iscc && isScore;
        //            }
        //            if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
        //            {
        //                u.y_isdel = isMain ? 1 : 0; //是否满足毕业所有条件
        //            }
        //            u.y_degreeOK = graduateYear <= xinshenyear ? 1 : 0; //是否达到毕业年份
        //            u.y_applyOK = iscc ? 1 : 0; //是否各个学年已经注册
        //            u.y_isChangePlan = isScore ? 1 : 0; //是否所有成绩都有并且合格

        //        });

        //        if (Request.IsAjaxRequest())
        //            return PartialView("StudentGradInfoList", model);
        //        return View(model);
        //    }
        //}

        public ActionResult StudentGradInfo(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var stuState = Request["StuState"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var isok = Request["isok"]; //是否允许毕业
                var isup = Request["isup"]; //是否申请毕业
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.adminrole = YdAdminRoleId;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Include(u => u.YD_Fee_StuFeeTb)
                        .Include(u => u.YD_Edu_Major)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 1 && u.y_studentType != 2).AsNoTracking();
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

                var lists = list1.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                list = list.Where(
                    u =>
                        (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                        xinshenyear);

                if (!string.IsNullOrWhiteSpace(isok) && !isok.Equals("0"))
                {
                    var listss =
                        lists.GroupBy(u => u.s.stu)
                            .Where(u => u.All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100))
                            .Select(u => u.Key.id)
                            .ToList();
                    if (isok == "1") //允许毕业
                    {
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            list =
                                list.Where(
                                    u =>
                                        (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                                        xinshenyear &&
                                        u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                        u.YD_Edu_Major.y_stuYear && listss.Contains(u.id));
                        }
                        else
                        {
                            list =
                               list.Where(
                                   u =>
                                       (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                                       xinshenyear &&
                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                       u.YD_Edu_Major.y_stuYear && listss.Contains(u.id) && u.y_img != null);
                        }

                    }
                    else if (isok == "2") //不允许毕业
                    {
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            list =
                            list.Where(
                                u =>
                                    !((u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                                      xinshenyear) ||
                                    u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
                                    u.YD_Edu_Major.y_stuYear || !listss.Contains(u.id));
                        }
                        else
                        {
                            list =
                              list.Where(
                                  u =>
                                      (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                                      xinshenyear &&
                                      u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                      u.YD_Edu_Major.y_stuYear && listss.Contains(u.id) && u.y_img == null);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(isup) && !isup.Equals("0"))
                {
                    if (isup == "1") //已申请毕业
                    {
                        list = list.Where(u => u.y_isgraduate == true);
                    }
                    else if (isup == "2") //未申请毕业
                    {
                        list = list.Where(u => u.y_isgraduate == null);
                    }
                }

                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).AsNoTracking()
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                ViewBag.admin = YdAdminRoleId;
                var model = list.ToPagedList(id, 10); //id为pageindex   15 为pagesize



                model.ForEach(u =>
                {
                    var graduateYear = u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1;
                    //毕业年份
                   // var iscc = true;
                    var iscc = u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                         u.YD_Edu_Major.y_stuYear; //各学年是否都注册
                     var isimg = u.y_img != null;
                    var isScore = lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100);
                    if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
                    {
                        isScore = lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 59 && k.score.y_totalScore <= 100);
                    }

                    var isMain = false;
                    //所有科目是否有成绩以及成绩是否都合格
                    if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                    {
                        isMain = graduateYear <= xinshenyear && iscc && isScore && isimg; //江西师范不需要判断是否有照片
                    }
                    else
                    {
                        isMain = graduateYear <= xinshenyear && iscc && isScore;
                    }
                    if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                    {
                        u.y_isdel = isMain ? 1 : 0; //是否满足毕业所有条件
                    }
                    u.y_degreeOK = graduateYear <= xinshenyear ? 1 : 0; //是否达到毕业年份
                    u.y_applyOK = iscc ? 1 : 0; //是否各个学年已经注册
                    u.y_isChangePlan = isScore ? 1 : 0; //是否所有成绩都有并且合格

                });

                if (Request.IsAjaxRequest())
                    return PartialView("StudentGradInfoList", model);
                return View(model);
            }
        }

        public ActionResult GraduateReport()
        {
            #region 权限验证

            var power = SafePowerPage("/report/SubStuReport");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion
            var subSchool = Request["SubSchool"];
            var inYear = Request["inYear"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 36); //根据父栏目ID获取兄弟栏目
                var list = yunEntities.YD_Sts_StuInfo.Where(x => x.y_stuStateId == 6);
                if (string.IsNullOrWhiteSpace(subSchool))
                {
                    int subschoolint = Convert.ToInt32(subSchool);
                    list = list.Where(x => x.y_subSchoolId == subschoolint);

                }
                if (string.IsNullOrWhiteSpace(inYear))
                {
                    int inyearint = Convert.ToInt32(inYear);
                    list = list.Where(x => x.y_inYear == inyearint);
                }
                return View(list);
            }
        }

        /// <summary>
        /// 毕业生申请导出下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuGradInfo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var isok = Request["isok"]; //是否允许毕业
                var isup = Request["isup"]; //是否申请毕业
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.adminrole = YdAdminRoleId;

                int inYear = Convert.ToInt32(enrollYear);
                #region 得到学生所有教学计划和成绩
                ///File/Dowon
                string mapPath = Server.MapPath($"~/File/Dowon/{inYear}毕业生申请表.xls");
                FileInfo fi = new FileInfo(mapPath);
                if (fi.LastWriteTime > DateTime.Now.AddMinutes(-60))
                {
                    return Content(Request.Url.Scheme + "://" + Request.Url.Authority + $"/File/Dowon/{inYear}毕业生申请表.xls");
                }
                ///课程IQ
                //int[] stateList = { 1, 7, 8 };
                var stu =
                    yunEntities.VW_StuInfo
                    .Where(u => u.y_stuStateId == 1 && u.y_subSchoolId.HasValue && u.y_inYear == inYear && u.y_isdel == 1)
                        .ToList(); //函授站不为-1，学籍状态为在读，未注册等
                var stuIds = stu.Select(e => e.id);
                var majorId = stu.Select(e => e.y_majorId);
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.Where(e => majorId.Contains(e.YD_TeaPlan_Class.y_majorId)).ToList();

                var list1 = stu.GroupJoin(classCourse,
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

                var scorelist = yunEntities.YD_Edu_Score.Where(e => stuIds.Contains(e.y_stuId)).OrderBy(u => u.id).ToList();

                var lists = list1.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() }).ToList();
                ///课程IQ

                #endregion

                IEnumerable<YD_Sts_StuInfo> list =
                yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                    .Include(u => u.YD_Fee_StuFeeTb)
                    .Include(u => u.YD_Edu_Major)
                    .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 1).ToList();

                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    if (string.IsNullOrWhiteSpace(enrollYear))
                    {
                        list = list.Where(u => u.y_inYear == (xinshenyear - 2));
                    }
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
                if (!string.IsNullOrWhiteSpace(isok) && !isok.Equals("0"))
                {
                    if (isok == "1") //允许毕业
                    {
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            list = list.Where(u => u.y_realYear.HasValue);
                            if (lists.Any())
                            {
                                list = list.Where(u => u.y_realYear <= xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100));
                            }
                            else
                            {
                                list = list.Where(u => (u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <= xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100));
                            }
                        }
                        else
                        {
                            list = list.Where(u => u.y_realYear.HasValue);
                            if (lists.Any())
                            {
                                list = list.Where(u => u.y_realYear <= xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100)
                                                           && u.y_img != null
                                                           );
                            }
                            else
                            {
                                list = list.Where(u => (u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <= xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60
                                                           && k.score.y_totalScore < 100) && u.y_img != null);
                            }
                        }
                    }
                    else if (isok == "2") //不允许毕业
                    {
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            list = list.Where(u => u.y_realYear.HasValue);
                            if (lists.Any())
                            {
                                list = list.Where(u => u.y_realYear > xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       !lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100));
                            }
                            else
                            {
                                list = list.Where(u => (u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) > xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       !lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100));
                            }
                        }
                        else
                        {
                            list = list.Where(u => u.y_realYear.HasValue);
                            if (lists.Any())
                            {
                                list = list.Where(u => u.y_realYear > xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       !lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100) && u.y_img == null);
                            }
                            else
                            {
                                list = list.Where(u => (u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) > xinshenyear &&
                                                       u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) !=
                                                       u.YD_Edu_Major.y_stuYear
                                                       &&
                                                       !lists.Where(k => k.s.stu.id == u.id)
                                                           .All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100) && u.y_img == null);
                            }
                        }

                    }
                }
                if (!string.IsNullOrWhiteSpace(isup) && !isup.Equals("0"))
                {
                    if (isup == "1") //已申请毕业
                    {
                        list = list.Where(u => u.y_isgraduate == true);
                    }
                    else if (isup == "2") //未申请毕业
                    {
                        list = list.Where(u => u.y_isgraduate == false);
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }

                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_inYear = u.y_inYear,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    majorname = u.YD_Edu_Major.y_name,
                                    schoolName = u.YD_Sys_SubSchool.y_name,
                                    y_cardId = u.y_cardId,
                                    stuStateName = u.YD_Edu_StuState.y_name,
                                    y_address = u.y_address,
                                    y_isgraduate = u.y_isgraduate == true ? "已申请" : "未申请"
                                }).ToList());
                if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_inYear = u.y_inYear,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    majorname = u.YD_Edu_Major.y_name,
                                    schoolName = u.YD_Sys_SubSchool.y_name,
                                    y_cardId = u.y_cardId,
                                    stuStateName = u.YD_Edu_StuState.y_name,
                                    y_address = u.y_address,
                                    y_degreeOK = (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <= xinshenyear ? "是" : "否",
                                    y_applyOK = (u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                       u.YD_Edu_Major.y_stuYear) ? "是" : "否",
                                    y_isChangePlan = (lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 59 && k.score.y_totalScore <= 100)) ? "是" : "否",
                                    y_img = u.y_img != null ? "是" : "否",
                                    y_isdel = ((u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <= xinshenyear) && (u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                       u.YD_Edu_Major.y_stuYear) && (lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 59 && k.score.y_totalScore <= 100)) && u.y_img != null ? "是" : "否",
                                    y_isgraduate = u.y_isgraduate == true ? "已申请" : "未申请"
                                }).Where(e => e.y_degreeOK == "是").ToList());
                }
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = $"/{inYear}毕业生申请表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorname", "专业名"},
                        {"schoolName", "函授站"},
                        {"y_cardId", "身份证"},
                        {"stuStateName", "学籍状态"},
                        {"y_address", "地址"},
                        {"y_isgraduate", "是否申请毕业"}
                    };
                    if (schoolname == ComEnum.SchoolName.ZYYDX.ToString())
                    {
                        ht = new Hashtable
                        {
                            {"y_name", "姓名"},
                            {"y_inYear", "入学年份"},
                            {"y_sex", "性别"},
                            {"y_stuNum", "学号"},
                            {"y_examNum", "考生号"},
                            {"majorname", "专业名"},
                            {"schoolName", "函授站"},
                            {"y_cardId", "身份证"},
                            {"stuStateName", "学籍状态"},
                            {"y_address", "地址"},
                            {"y_degreeOK", "是否达到毕业年份"},
                            {"y_applyOK", "是否学年都注册"},
                            {"y_isChangePlan", "是否成绩合格"},
                            {"y_img", "是否有照片"},
                            {"y_isdel", "是否允许毕业"},
                            {"y_isgraduate", "是否申请毕业"}
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
        /// 选择性勾选学生是否申请毕业情况
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoCheckOk()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.isCheckForSchool = true;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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
        /// 选择性取消勾选学生是否申请毕业情况
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoCheckno()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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

            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.isCheckForSchool = false;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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
        /// 选择所有能毕业学生
        /// </summary>
        /// <returns></returns>
        public string AllStuGradCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var year = Request["EnrollYear"];
                //if (string.IsNullOrWhiteSpace(year))
                //{
                //    return "必须选择年份";
                //}              
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                //var enrollYearint = Convert.ToInt32(year);
                #region 得到学生所有教学计划和成绩

                ///课程IQ
                int[] stateList = { 1, 7, 8 };
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                        .AsQueryable(); //函授站不为-1，学籍状态为在读，未注册等

                var list1 = stu.GroupJoin(classCourse,
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

                var scorelist = yunEntities.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list1.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                ///课程IQ

                #endregion

                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    //只获取该站点没有申请的学生
                    IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Where(u =>
                            u.y_isdel == isnotdel && u.y_subSchoolId.HasValue &&
                            sub.Contains(u.y_subSchoolId.Value) && u.y_stuStateId == 1 &&
                            u.y_isgraduate != true).AsQueryable();
                    if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                    {
                        var enrollYearint = Convert.ToInt32(year);
                        list = list.Where(u => u.y_inYear == enrollYearint);
                    }
                    if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXSFDX.ToString())
                    {
                        list = list.Where(e => e.y_scoreOk == 1 && e.y_img.Contains("181026"));
                    }
                    List<int> stuIds = new List<int>();
                    list.ToList().ForEach(u =>
                    {
                        var graduateYear = u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1; //毕业年份
                        var iscc = u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) == u.YD_Edu_Major.y_stuYear; //各学年是否都注册
                        var isimg = u.y_img != null; //是否有照片
                        var isScore = lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100);

                        //所有科目是否有成绩以及成绩是否都合格
                        var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                        var isMain = false;
                        //所有科目是否有成绩以及成绩是否都合格
                        if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            isMain = graduateYear <= xinshenyear && iscc && isScore && isimg; //江西师范不需要暂时不需要毕业判断是否有照片
                        }
                        else
                        {
                            isMain = graduateYear <= xinshenyear && iscc && u.y_scoreOk == 1 && u.y_img != null && u.y_img.Contains("181026"); //isScore;//师大改为人工审核
                        }


                        u.y_isMoneyOk = isMain ? 1 : 0; //是否满足毕业所有条件
                        if (u.y_isMoneyOk == 1)
                        {
                            stuIds.Add(u.id);
                        }
                    });
                    if (stuIds.Count > 0)
                    {
                        var isokStus = yunEntities.YD_Sts_StuInfo.Where(u => stuIds.Contains(u.id)).ToList();
                        //得到保存到的满足毕业条件的学生
                        if (isokStus.Any())
                        {
                            isokStus.ForEach(k =>
                            {
                                var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                                sb.AppendLine(" SET [y_isMoneyOk]=1,isCheckForSchool=1 WHERE id=" + k.id);
                                string sql = sb.ToString();
                                yunEntities.Database.ExecuteSqlCommand(sql);


                                //k.y_isMoneyOk = 1;
                                //k.isCheckForSchool = true;
                                //yunEntities.Entry(k).State = EntityState.Modified;
                            });
                        }
                        return "设置成功";
                        //int r = yunEntities.SaveChanges();
                        //if (r > 0)
                        //{
                        //    return "设置成功";
                        //}
                        //else
                        //{
                        //    return "没有满足毕业条件的学生";
                        //}
                    }
                    else
                    {
                        return "没有满足毕业条件的学生";
                    }
                }
                else
                {
                    return "账号没有关联站点";
                }
            }
        }

        /// <summary>
        /// 取消选择的所有能毕业学生
        /// </summary>
        /// <returns></returns>
        public string AllStuGradCheckNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var year = Request["EnrollYear"];
                //if (string.IsNullOrWhiteSpace(year))
                //{
                //    return "必须选择年份";
                //}
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                //var enrollYearint = Convert.ToInt32(year);

                #region 得到学生所有教学计划和成绩

                ///课程IQ
                int[] stateList = { 1, 7, 8 };
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                        .AsQueryable(); //函授站不为-1，学籍状态为在读，未注册等

                var list1 = stu.GroupJoin(classCourse,
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

                var scorelist = yunEntities.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list1.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                ///课程IQ

                #endregion

                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    //只获取该站点没有申请的学生
                    var list = yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Where(u =>
                            u.y_isdel == isnotdel && u.y_subSchoolId.HasValue &&
                            sub.Contains(u.y_subSchoolId.Value) && u.y_stuStateId == 1
                             && u.y_isgraduate != true);
                    if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                    {
                        var enrollYearint = Convert.ToInt32(year);
                        list = list.Where(u => u.y_inYear == enrollYearint);
                    }
                    List<int> stuIds = new List<int>();
                    list.ToList().ForEach(u =>
                    {
                        var graduateYear = u.y_realYear.HasValue
                            ? u.y_realYear
                            : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1; //毕业年份
                        var iscc = u.YD_Fee_StuFeeTb.Count(k => k.y_isCheckFee == (int)YesOrNo.Yes) ==
                                   u.YD_Edu_Major.y_stuYear; //各学年是否都注册
                        var isimg = u.y_img != null; //是否有照片
                        var isScore =
                            lists.Where(k => k.s.stu.id == u.id).All(k => k.score != null && k.score.y_totalScore >= 60 && k.score.y_totalScore < 100);
                        //所有科目是否有成绩以及成绩是否都合格
                        var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                        var isMain = false;
                        //所有科目是否有成绩以及成绩是否都合格
                        if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            isMain = graduateYear <= xinshenyear && iscc && isScore && isimg;//江西师范不需要暂时不需要毕业判断是否有照片
                        }
                        else
                        {
                            isMain = graduateYear <= xinshenyear && iscc && isScore;
                        }

                        u.y_isMoneyOk = isMain ? 1 : 0; //是否满足毕业所有条件
                        if (u.y_isMoneyOk == 1)
                        {
                            stuIds.Add(u.id);
                        }
                    });
                    if (stuIds.Count > 0)
                    {
                        var isokStus = yunEntities.YD_Sts_StuInfo.Where(u => stuIds.Contains(u.id)).ToList();
                        //得到保存到的满足毕业条件的学生
                        if (isokStus.Any())
                        {
                            isokStus.ForEach(k =>
                            {
                                var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                                sb.AppendLine(" SET [y_isMoneyOk]=1,isCheckForSchool=0 WHERE id=" + k.id);
                                string sql = sb.ToString();
                                yunEntities.Database.ExecuteSqlCommand(sql);

                                //k.y_isMoneyOk = 1;
                                //k.isCheckForSchool = false;
                                //yunEntities.Entry(k).State = EntityState.Modified;
                            });
                        }
                        return "设置成功";
                        //int r = yunEntities.SaveChanges();
                        //if (r > 0)
                        //{
                        //    return "设置成功";
                        //}
                        //else
                        //{
                        //    return "没有满足毕业条件的学生";
                        //}
                    }
                    else
                    {
                        return "没有满足毕业条件的学生";
                    }
                }
                else
                {
                    return "账号没有关联站点";
                }
            }
        }

        /// <summary>
        /// 毕业学生注册情况
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuGradInfoRester(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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

            if (id == null)
            {
                return RedirectToAction("StudentGradInfo");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                IQueryable<YD_Fee_StuFeeTb> list =
                    yunEntities.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                        .Where(u => u.y_stuId == id)
                        .Include(u => u.YD_Sts_StuInfo)
                        .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                        .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool);
                if (!list.Any())
                {
                    return RedirectToAction("StudentGradInfo");
                }
                ViewBag.admin = YdAdminRoleId;
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var schoolids =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list =
                        list.Where(
                            u =>
                                u.YD_Sts_StuInfo.y_subSchoolId.HasValue &&
                                schoolids.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                }
                var model = list.ToList();
                return View(model);
            }
        }

        /// <summary>
        /// 学生的毕业成绩单页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuScoreList(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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

            ViewBag.adminrole = YdAdminRoleId;
            if (id == null)
            {
                return RedirectToAction("StudentGradInfo");
            }
            using (var ad = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 53); //根据父栏目ID获取兄弟栏目
                }

                var stuI =
                    ad.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Sys_SubSchool)
                        .FirstOrDefault(u => u.id == id);

                ViewBag.stu = stuI;

                int[] stateList = { 1, 7, 8 };

                var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    ad.VW_StuInfo.Where(
                        u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue && u.id == id)
                        .AsQueryable(); //函授站不为-1，学籍状态为在读，未注册等

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

                var scorelist = ad.VW_Score.OrderBy(u => u.id).AsQueryable();
                if (!list.Any())
                {
                    return RedirectToAction("StudentGradInfo");
                }
                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                if (!lists.Any())
                {
                    return RedirectToAction("StudentGradInfo");
                }

                if (lists.Any(u => u.s.classCourse == null))
                {
                    return View(new List<ScoreStatistics_Course>());
                }

                var stuScore = lists
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
                if (!stuScore.Any())
                {
                    return RedirectToAction("StudentGradInfo");
                }
                return View(stuScore);
            }
        }

        /// <summary>
        /// 函授站选择性提交学生毕业名单
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
            //if (string.IsNullOrWhiteSpace(inyear) && inyear.Equals("0"))
            //{
            //    return "请选择年份";
            //}
            using (var yunEntities = new IYunEntities())
            {
                //var inyearint = Convert.ToInt32(inyear);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => sub.Contains(u.id));
                    var obj = new YD_Graduate_StudentApply();
                    if (school != null)
                    {
                        obj.y_SubSchoolId = school.id;
                        obj.SchoolName = school.y_name;
                        //得到该站点在读允许毕业未申请的学生名单
                        IQueryable<YD_Sts_StuInfo> list =
                            yunEntities.YD_Sts_StuInfo.Where(
                                u => u.y_isdel == 1 && u.y_stuStateId == 1
                                && u.y_subSchoolId == school.id && u.isCheckForSchool == true &&
                                     u.y_isgraduate != true).AsQueryable();
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                        {
                            list.Where(e => e.y_scoreOk == 1 && e.y_img.Contains("181026"));
                        }
                        if (!string.IsNullOrWhiteSpace(inyear) && !inyear.Equals("0"))
                        {
                            var enrollYearint = Convert.ToInt32(inyear);
                            list = list.Where(u => u.y_inYear == enrollYearint);
                        }
                        if (list.ToList().Any())
                        {
                            var stuidlist = string.Join(",", list.Select(u => u.id).ToArray()); //所有stuid根据逗号相加成一个字符串

                            obj.totalcount = list.Count();
                            obj.y_time = DateTime.Now;
                            obj.y_check = 0;
                            obj.y_stuid = stuidlist;
                            if (schoolname != ComEnum.SchoolName.JXSFDX.ToString())
                            {
                                obj.y_inyear = xinshenyear; //毕业年份
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(inyear) && !inyear.Equals("0"))
                                {
                                    var enrollYearint = Convert.ToInt32(inyear);
                                    obj.y_inyear = enrollYearint; //入学年份
                                }
                                else
                                {
                                    obj.y_inyear = xinshenyear - 2; //默认入学年份
                                }

                            }

                            yunEntities.Entry(obj).State = EntityState.Added;
                            list.ForEach(u =>
                            {
                                var isgraduate = true;
                                var isCheckForSchool = true;
                                var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                                sb.AppendLine(" SET [y_isgraduate]=1,isCheckForSchool=1 WHERE id=" + u.id);
                                string sql = sb.ToString();
                                yunEntities.Database.ExecuteSqlCommand(sql);

                                //u.y_isgraduate = true;
                                //u.isCheckForSchool = false;
                                //yunEntities.Entry(u).State = EntityState.Modified;
                            });
                        }
                    }
                    else
                    {
                        return "函授站信息有误！";
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
        /// 学校审核毕业学生名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuGradInfoCheck(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoCheck");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var subSchool = Request["SubSchool"];
                var inYear = Request["inYear"];
                var ischeck = Request["isCheck"];
                var year = Request["year"];
                var intyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Graduate_StudentApply> list =
                    yunEntities.YD_Graduate_StudentApply.OrderByDescending(u => u.id).Where(u => true);
                ViewBag.admin = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_SubSchoolId == subSchoolint);
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
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                ViewBag.admin = YdAdminRoleId;

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize    

                if (Request.IsAjaxRequest())
                    return PartialView("StuGradInfoCheckList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 毕业生统计
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintGradInfoTonji()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoCheck");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                   yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                       .Include(u => u.YD_Fee_StuFeeTb)
                       .Include(u => u.YD_Edu_Major)
                       .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_inYear == 2015);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                //达到毕业年份
                list = list.Where(
                     u =>
                         (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <=
                         xinshenyear);
                var model = list.ToList().GroupBy(u => u.YD_Edu_Major.y_eduTypeId).Select(u => new GridStuTonji { key = u.Key, count = u.Count() }).ToList();    //得到各层次达到毕业年份人数

                ViewBag.model = model;

                return View(model);
            }
        }

        public class GridStuTonji
        {
            public int key { get; set; }
            public int count { get; set; }
        }
        /// <summary>
        ///函授站提交毕业名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownApplyGradute()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoCheck");
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
                var year = Request["year"];
                IQueryable<YD_Graduate_StudentApply> list =
                    yunEntities.YD_Graduate_StudentApply.OrderByDescending(u => u.id).Where(u => true);
                ViewBag.admin = YdAdminRoleId;
                var schoolName = "";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    var schoolfirst = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    schoolName = schoolfirst != null ? schoolfirst.y_name : "";
                    list = list.Where(u => u.y_SubSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yearint = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_inyear == yearint);
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
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                var models = list.ToList();
                var listss = models.Select(
                    u =>
                        new
                        {
                            schoolName = u.SchoolName,
                            y_inyear = u.y_inyear,
                            totalcount = u.totalcount,
                            y_time = u.y_time,
                            y_check = u.y_check == 1 ? "已审核" : u.y_check == 2 ? "审核不通过" : "待审核"
                        }).ToList();
                DataTable model = FileHelper.ToDataTable(listss);
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + schoolName + "审核毕业名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/缴费审核表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"y_inyear", "入学年份"},
                        {"totalcount", "提交人数"},
                        {"y_time", "提交时间"},
                        {"y_check", "状态"}
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
        /// 函授站提交毕业名单全部人数查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuGradCheckDes(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoCheck");
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

            if (id == null)
            {
                return RedirectToAction("StuGradInfoCheck");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var ydStuGraduate = yunEntities.YD_Graduate_StudentApply.FirstOrDefault(u => u.id == id);
                if (ydStuGraduate != null)
                {
                    string stuid = ydStuGraduate.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<VW_StuInfo> list =
                        yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id));
                    if (!list.Any())
                    {
                        return RedirectToAction("StuGradInfoCheck");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    //根据名字搜索
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        list = list.Where(u => u.y_name.Contains(name));
                    }
                    //根据入学年份查询
                    if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                    {
                        var yInYear = Convert.ToInt16(inYear);
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
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId);

                        list = list.Where(u => u.y_subSchoolId.HasValue && schoolids.Contains(u.y_subSchoolId.Value));
                    }
                    var model = list.ToList();
                    return View(model);
                }
                else
                {
                    var model = new List<VW_StuInfo>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 函授站提交毕业名单全部人数下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuGradCheckDes()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoCheck");
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
                var graduteid = Request["graduteid"];
                if (graduteid == null)
                {
                    return Content("未知错误");
                }
                var id = Convert.ToInt32(graduteid);

                var ydStuGraduate = yunEntities.YD_Graduate_StudentApply.FirstOrDefault(u => u.id == id);
                if (ydStuGraduate != null)
                {
                    string stuid = ydStuGraduate.y_stuid;
                    int ischeck = ydStuGraduate.y_check; //审核状态
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<VW_StuInfo> list =
                        yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id));
                    var schoolName = ydStuGraduate.SchoolName;
                    var inyear = ydStuGraduate.y_inyear;
                    var models = list.ToList();
                    DataTable model = null;
                    var listss = models.Select(
                                u =>
                                    new
                                    {
                                        schoolName = u.schoolName,
                                        y_stuNum = u.y_stuNum,
                                        y_name = u.y_name,
                                        y_inYear = u.y_inYear,
                                        majorName = u.majorName,
                                        y_tel = u.y_tel,
                                        y_address = u.y_address,
                                        y_cardId = u.y_cardId,
                                        y_stuYear = u.y_stuYear,
                                        y_isUp = u.y_isgraduate == true ? "已申请" : "未申请",
                                        y_isCheckFee = ischeck == 1 ? "已通过" : ischeck == 2 ? "已拒绝" : "待审核",
                                    }).OrderByDescending(u => u.y_stuNum)
                                .ToList();
                    model = FileHelper.ToDataTable(listss);

                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/" + schoolName + inyear + "毕业学生名单" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变

                    using (var excelHelper = new ExcelHelper(fileName3))
                    {
                        var ht = new Hashtable
                        {
                            {"schoolName", "函授站"},
                            {"y_stuNum", "学号"},
                            {"y_name", "姓名"},
                            {"y_inYear", "入学年份"},
                            {"majorName", "专业"},
                            {"y_tel", "电话"},
                            {"y_address", "地址"},
                            {"y_cardId", "身份证号"},
                            {"y_stuYear", "学制"},
                            {"y_isUp", "申请毕业状态"},
                            {"y_isCheckFee", "审核毕业状态"}
                        };
                        if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                        {
                            ht = new Hashtable
                            {
                                {"schoolName", "函授站"},
                                {"y_stuNum", "学号"},
                                {"y_name", "姓名"},
                                {"y_inYear", "入学年份"},
                                {"majorLibraryName", "专业名"},
                                {"eduTypeName", "层次"},
                                {"stuTypeName", "学制"},
                                {"y_tel", "电话"},
                                {"y_address", "地址"},
                                {"y_cardId", "身份证号"},
                                {"y_stuYear", "学制"},
                                {"y_isUp", "申请毕业状态"},
                                {"y_isCheckFee", "审核毕业状态"}
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
        /// 取消某个学生的状态并且将他的毕业证号撤销
        /// </summary>
        /// <returns></returns>
        public string CancelStudentGradStatus(int userId)
        {
            using (var yunEntities = new IYunEntities())
            {
                var statename = "在读";
                var state = yunEntities.YD_Edu_StuState.First(u => u.y_name == statename);
                if (state == null)
                {
                    return "学籍状态缺少在读状态";
                }
                string sql = $"UPDATE [YD_Sts_StuInfo] SET y_stuStateId='{state.id}',y_graduateNumber=null,y_isgraduate=null WHERE id='{userId}';";
                yunEntities.Database.ExecuteSqlCommand(sql);
                return "ok";
            }
        }

        /// <summary>
        /// 学校审核学生毕业--通过
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoCheckSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var ydStuGraduate = yunEntities.YD_Graduate_StudentApply.FirstOrDefault(u => u.id == intid);
                if (ydStuGraduate != null)
                {
                    ydStuGraduate.y_check = 1; //审核通过
                    yunEntities.Entry(ydStuGraduate).State = EntityState.Modified;

                    var statename = "已毕业";
                    var state = yunEntities.YD_Edu_StuState.First(u => u.y_name == statename);
                    if (state == null)
                    {
                        return "学籍状态缺少已毕业状态";
                    }
                    string stuid = ydStuGraduate.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    list.ForEach(u =>
                    {
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_stuStateId]=" + state.id + " WHERE id=" + u.id);
                        string sql = sb.ToString();
                       var res= yunEntities.Database.ExecuteSqlCommand(sql);
                       

                        //u.y_stuStateId = state.id;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "审核为通过,学生学籍状态为已毕业");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }

        /// <summary>
        /// 选择性审核学生毕业--拒绝
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoCheckSomeNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var ydStuGraduate = yunEntities.YD_Graduate_StudentApply.FirstOrDefault(u => u.id == intid);
                if (ydStuGraduate != null)
                {
                    ydStuGraduate.y_check = 2; //审核不通过
                    yunEntities.Entry(ydStuGraduate).State = EntityState.Modified;
                    string stuid = ydStuGraduate.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    list.ForEach(u =>
                    {
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_isgraduate]=0" + "  WHERE id=" + u.id);
                        string sql = sb.ToString();
                        yunEntities.Database.ExecuteSqlCommand(sql);
                        //u.y_isgraduate = false;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "审核为不通过,函授站需重新提交毕业名单");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }


        /// <summary>
        /// 选择性审核学生毕业--撤销审核失误操作
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoRevocationCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGradInfo");
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
                var ydStuGraduate = yunEntities.YD_Graduate_StudentApply.FirstOrDefault(u => u.id == intid);
                if (ydStuGraduate != null)
                {
                    string stuid = ydStuGraduate.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    if (ydStuGraduate.y_check == 2 && list.Any(u => u.y_isgraduate == true))
                    {
                        return "不能撤销";
                    }
                    ydStuGraduate.y_check = 0; //改为待审核状态
                    yunEntities.Entry(ydStuGraduate).State = EntityState.Modified;

                    var statename = "在读";
                    var state = yunEntities.YD_Edu_StuState.First(u => u.y_name == statename);
                    if (state == null)
                    {
                        return "学籍状态缺少在读状态";
                    }

                    list.ForEach(u =>
                    {
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_stuStateId]=" + state.id + ",y_isgraduate=1" + " WHERE id=" + u.id);
                        string sql = sb.ToString();
                        yunEntities.Database.ExecuteSqlCommand(sql);

                        //u.y_stuStateId = state.id;
                        //u.y_isgraduate = true;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "撤销之前审核操作失误，还原到初始状态");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }

        /// <summary>
        /// 毕业生管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuGradInfoNum(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoNum");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var isgradunum = Request["isgradunum"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.y_subSchoolId)
                        .Include(u => u.YD_Sys_SubSchool)
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Edu_StuState)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);

                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    var gradYear = Request["gradYear"];
                    if (gradYear != "null")
                    {
                        list = list.Where(e => e.y_graduateNumber.Contains("104145" + gradYear));
                    }
                }
                //if (string.IsNullOrWhiteSpace(enrollYear))
                //{
                //    list = list.Where(u => u.y_inYear == (xinshenyear - 2));
                //}

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard
                     || u.y_cardId == namenumcard || u.y_examNum == namenumcard);
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
                if (!string.IsNullOrWhiteSpace(isgradunum) && !isgradunum.Equals("0"))
                {
                    if (isgradunum == "1") //有毕业证号
                    {
                        list = list.Where(u => u.y_graduateNumber != null);
                    }
                    else if (isgradunum == "2") //没有毕业证号
                    {
                        list = list.Where(u => u.y_graduateNumber == null);
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                ViewBag.adminrole = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("StuGradInfoNumList", model);
                return View(model);
            }
        }

        /// <summary>
        ///毕业生证号导入页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuGradInfoUpload(int id = 1)
        {
            #region 权限验证      

            var power = SafePowerPage("/Graduate/StuGradInfoUpload");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
            }
            return View();
        }

        /// <summary>
        /// 根据考生号导入匹配的毕业证号
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult UploadGradInfoNum(string fileName)
        {
            fileName = Server.MapPath(fileName);
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
                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);

                int index = 0;
                using (var ad = new IYunEntities())
                {
                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0); //考生号
                        var cell1 = row.GetCell(1);
                        //var cell2 = row.GetCell(2); //入学年份
                        var examnub = "";
                        var inyear = 0;
                        if (cell.CellType != CellType.STRING) //如果不是string 类型
                        {
                            cell.CellStyle = styleCell;
                            index++;
                        }
                        else if (cell1.CellType != CellType.STRING) //如果不是string 类型
                        {
                            cell1.CellStyle = styleCell;
                            index++;
                        }
                        //else if (cell2.CellType != CellType.NUMERIC) //如果不是string 类型
                        //{
                        //    cell2.CellStyle = styleCell;
                        //    index++;
                        //}
                        else
                        {
                            examnub = cell.StringCellValue.Trim(); //考生号
                            //inyear = Convert.ToInt32(cell2.NumericCellValue); //入学年份
                            var list = ad.YD_Sts_StuInfo.Where(u => u.y_isdel == 1).ToList();
                            var stuinfo = list.FirstOrDefault(u => u.y_examNum == examnub);
                            if (stuinfo == null)
                            {
                                cell.CellStyle = styleCell;
                                index++;
                                continue;
                            }
                            if (index == 0)
                            {
                                stuinfo.y_graduateNumber = cell1.StringCellValue.Trim(); //毕业证号

                                //ad.Entry(stuinfo).State = EntityState.Modified;

                                var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                                sb.AppendLine(" SET [y_graduateNumber]=" + cell1.StringCellValue.Trim() + " WHERE id=" + stuinfo.id);
                                string sql = sb.ToString();
                                ad.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }
                    if (index > 0)
                    {
                        var dirPath = Server.MapPath("~/File/Dowon");
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var filename1 = "/考生号错误表" + Hz;
                        var fileName3 = dirPath + filename1;

                        //将工作簿写入文件
                        using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                        {
                            workbook.Write(fs2);
                            string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                            return
                                Content(
                                    "<script type='text/javascript'>alert('Excel验证失败，点击确认获取Excel错误提示！'); window.location.href='" +
                                    url + "';</script >");
                        }
                    }
                    else
                    {
                        return Content("ok");
                        //int r = ad.SaveChanges();
                        //if (r > 0)
                        //{
                        //    return
                        //        Content("<script type='text/javascript'>alert('导入成功,导入" + r +
                        //                "条数据');window.location.href='/Graduate/StuGradInfoNum';</script >");
                        //}
                        //else
                        //{
                        //    return Content("<script type='text/javascript'>alert('导入成功')</script >");
                        //}
                    }
                }
            }
        }


        /// <summary>
        /// 毕业生管理导出下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuGradInfoNum()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGradInfoNum");
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
                var isgradunum = Request["isgradunum"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Include(u => u.YD_Sys_SubSchool)
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Edu_StuState)
                        .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                        .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                        .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6);
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    var gradYear = Request["gradYear"];
                    if (gradYear != "null")
                    {
                        list = list.Where(e => e.y_graduateNumber.Contains("104145" + gradYear));
                    }
                }

                if (string.IsNullOrWhiteSpace(enrollYear))
                {
                    list = list.Where(u => u.y_inYear == (xinshenyear - 2));
                }

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard || u.y_cardId == namenumcard);
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
                if (!string.IsNullOrWhiteSpace(isgradunum) && !isgradunum.Equals("0"))
                {
                    if (isgradunum == "1") //有毕业证号
                    {
                        list = list.Where(u => u.y_graduateNumber != null);
                    }
                    else if (isgradunum == "2") //没有毕业证号
                    {
                        list = list.Where(u => u.y_graduateNumber == null);
                    }
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                var model =
                    FileHelper.ToDataTable(
                        list.OrderBy(u => u.y_subSchoolId)
                        .ThenBy(e => e.YD_Edu_Major.y_eduTypeId)
                        .ThenBy(e => e.y_majorId)
                        .ThenBy(e => e.YD_Edu_Major.y_stuTypeId)
                        .ThenBy(e => e.y_stuNum).Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_inYear = u.y_inYear,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_graduateNumber = u.y_graduateNumber,
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    eduTypeName = u.YD_Edu_Major.YD_Edu_EduType.y_name,
                                    majorname = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name,
                                    stuTypeName = u.YD_Edu_Major.YD_Edu_StuType.y_name,
                                    schoolName = u.YD_Sys_SubSchool.y_name,
                                    y_cardId = u.y_cardId,
                                    stuStateName = u.YD_Edu_StuState.y_name,
                                    y_address = u.y_address
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/毕业生信息表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
                        {"y_sex", "性别"},
                        {"y_graduateNumber", "毕业证号"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"eduTypeName", "层次" },
                        {"majorname", "专业名"},
                        {"stuTypeName", "学习形式" },
                        {"schoolName", "函授站"},
                        {"y_cardId", "身份证"},
                        {"stuStateName", "学籍状态"},
                        {"y_address", "地址"}
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
        //补历届毕业学生名单
        public ActionResult ForGraduation()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/ForGraduation");
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
            using (var ad = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 53); //根据父栏目ID获取兄弟栏目
                }
                const int isnotdel = (int)YesOrNo.No;
                var year = Request["EnrollYear"];
                var inyear = 0;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]); ; //获取今年年份
                if (string.IsNullOrWhiteSpace(year) || year.Equals("0"))
                {
                    inyear = xinshenyear - 1; //默认毕业年份是去年
                }
                else
                {
                    inyear = Convert.ToInt32(year);
                }
                var gridstuid = ad.YD_Graduate_StudentApply.Where(u => u.y_time.Year == xinshenyear).Select(u => u.y_stuid); //获取今年申请毕业的名单
                                                                                                                             //将string数组转换成int数组
                int[] ids = new int[] { };
                foreach (var stuid in gridstuid)
                {
                    ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                }

                var list = ad.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                    Include(u => u.YD_Edu_Major.YD_Edu_EduType).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Sys_SubSchool).Where(u => u.y_isdel == isnotdel && ids.Contains(u.id) && u.y_stuStateId == 6
                      && u.y_inYear == inyear + 1 - u.YD_Edu_Major.y_stuYear); //补指定毕业年份的学生名单

                list = list.Where(
                  u =>
                      (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <
                      xinshenyear);
                var model = list.ToList();

                ViewBag.year = inyear;
                return View(model);
            }
        }

        //补历届毕业学生名单导出
        public ActionResult DownForGraduation()
        {

            using (var ad = new IYunEntities())
            {
                const int isnotdel = (int)YesOrNo.No;
                var year = Request["EnrollYear"];
                var inyear = 0;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]); ; //获取今年年份
                if (string.IsNullOrWhiteSpace(year) || year.Equals("0"))
                {
                    inyear = xinshenyear - 1; //默认毕业年份是去年
                }
                else
                {
                    inyear = Convert.ToInt32(year);
                }
                var gridstuid = ad.YD_Graduate_StudentApply.Where(u => u.y_time.Year == xinshenyear).Select(u => u.y_stuid); //获取今年申请毕业的名单
                                                                                                                             //将string数组转换成int数组
                int[] ids = new int[] { };
                foreach (var stuid in gridstuid)
                {
                    ids = Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                }

                var list = ad.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                    Include(u => u.YD_Edu_Major.YD_Edu_EduType).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Sys_SubSchool).Where(u => u.y_isdel == isnotdel && ids.Contains(u.id) && u.y_stuStateId == 6
                      && u.y_inYear == inyear + 1 - u.YD_Edu_Major.y_stuYear); //补指定毕业年份的学生名单

                list = list.Where(
                  u =>
                      (u.y_realYear.HasValue ? u.y_realYear : u.y_inYear + u.YD_Edu_Major.y_stuYear - 1) <
                      xinshenyear);
                var model = list.ToList();
                if (model.Any())
                {
                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    //设置单元格内容
                    cell.SetCellValue("成人高等教育补" + inyear + "年学历电子注册学生名单");

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style;

                    //设置单元格的高度
                    row.Height = 30 * 20;
                    //设置单元格的宽度
                    sheet.SetColumnWidth(2, 30 * 256);
                    var styleCell = workbook.CreateCellStyle();
                    styleCell.Alignment = HorizontalAlignment.CENTER;
                    var font2 = workbook.CreateFont();
                    styleCell.SetFont(font2);


                    var row1 = sheet.CreateRow(1);
                    row1.CreateCell(0).SetCellValue("序号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 0));
                    row1.CreateCell(1).SetCellValue("学号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 1));
                    row1.CreateCell(2).SetCellValue("考生号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 2, 2));
                    row1.CreateCell(3).SetCellValue("姓名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 3, 3));
                    row1.CreateCell(4).SetCellValue("性别");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 4, 4));
                    row1.CreateCell(5).SetCellValue("专业");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 5, 5));
                    row1.CreateCell(6).SetCellValue("站点");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 6, 6));
                    row1.CreateCell(7).SetCellValue("学制");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 7));
                    row1.CreateCell(8).SetCellValue("年级");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 8));
                    row1.CreateCell(9).SetCellValue("学习形式");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 9, 9));
                    row1.CreateCell(10).SetCellValue("层次");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 10, 10));
                    row1.CreateCell(11).SetCellValue("身份证号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 11, 11));
                    row1.CreateCell(12).SetCellValue("毕业编号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 12, 12));
                    row1.CreateCell(13).SetCellValue("情况说明");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));
                    row1.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    for (int i = 0; i < model.Count; i++)
                    {
                        var rowNumber = sheet.PhysicalNumberOfRows;
                        var rowCourse = sheet.CreateRow(rowNumber);
                        rowCourse.CreateCell(0).SetCellValue(i + 1); //序号
                        rowCourse.CreateCell(1).SetCellValue(model[i].y_stuNum);
                        rowCourse.CreateCell(2).SetCellValue(model[i].y_examNum);
                        rowCourse.CreateCell(3).SetCellValue(model[i].y_name);
                        rowCourse.CreateCell(4).SetCellValue(model[i].y_sex == 0 ? "男" : "女");
                        rowCourse.CreateCell(5).SetCellValue(model[i].YD_Edu_Major.YD_Edu_MajorLibrary.y_name);
                        rowCourse.CreateCell(6).SetCellValue(model[i].YD_Sys_SubSchool.y_name);
                        rowCourse.CreateCell(7).SetCellValue(model[i].YD_Edu_Major.y_stuYear);
                        rowCourse.CreateCell(8).SetCellValue(model[i].y_inYear);
                        rowCourse.CreateCell(9).SetCellValue(model[i].YD_Edu_Major.YD_Edu_StuType.y_name);
                        rowCourse.CreateCell(10).SetCellValue(model[i].YD_Edu_Major.YD_Edu_EduType.y_name);
                        rowCourse.CreateCell(11).SetCellValue(model[i].y_cardId);
                        rowCourse.CreateCell(12).SetCellValue("");
                        rowCourse.CreateCell(13).SetCellValue("");

                        rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    }
                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/补" + inyear + "年毕业学生名单导出" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变
                    //将工作簿写入文件
                    using (FileStream fs = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                }
                else
                {
                    return Content("没有学生名单");

                }
            }
        }


        /// 学位外语申报页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentGegreeEnglish(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }

                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                var isok = Request["isok"]; //是否允许申报
                var isup = Request["isup"]; //是否已申请
                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_stuStateId == 6
                                    && u.eduTypeName != "高起专" && u.y_bachelordegreeCheck != true);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                ViewBag.adminrole = YdAdminRoleId;
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard
                     || u.y_cardId == namenumcard || u.y_examNum == namenumcard);
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

                if (!string.IsNullOrWhiteSpace(isok) && !isok.Equals("0"))
                {
                    var engisthscore =
                        yunEntities.YD_Graduate_StudentScore.Where(s => s.y_isdel == 1 && s.y_verdict == 1)
                            .GroupBy(s => s.y_stuId)
                            .Select(s => s.Key)
                            .ToList();

                    if (isok == "1") //允许申报
                    {
                        list = list.Where(u => engisthscore.Contains(u.id));

                    }
                    else if (isok == "2") //不允许申报
                    {
                        list = list.Where(u => !engisthscore.Contains(u.id));
                    }
                }

                if (!string.IsNullOrWhiteSpace(isup) && !isup.Equals("0"))
                {
                    if (isup == "1") //已申请
                    {
                        list = list.Where(u => u.y_bachelordegree == true);
                    }
                    else if (isup == "2") //未申请
                    {
                        list = list.Where(u => u.y_bachelordegree == false);
                    }
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                model.ForEach(u =>
                {
                    var engisthscore =
                        yunEntities.YD_Graduate_StudentScore.Where(s => s.y_isdel == 1)
                            .GroupBy(s => new { s.y_stuId, s.y_verdict })
                            .Select(s => s.Key)
                            .ToList();
                    var isverdict = engisthscore.FirstOrDefault(k => k.y_stuId == u.id);
                    u.y_isdel = isverdict != null && isverdict.y_verdict == 1 ? 1 : 0;
                });

                if (Request.IsAjaxRequest())
                    return PartialView("StudentGegreeEnglishList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 学生学位外语成绩情况
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StDegreeScore(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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

            if (id == null)
            {
                return RedirectToAction("StudentGegreeEnglish");
            }
            ViewBag.id = id;
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                IQueryable<YD_Graduate_StudentScore> list =
                    yunEntities.YD_Graduate_StudentScore.Include(u => u.YD_Sts_StuInfo).Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                    .OrderByDescending(u => u.id)
                        .Where(u => u.y_stuId == id);
                if (!list.Any())
                {
                    return RedirectToAction("StudentGegreeEnglish");
                }
                ViewBag.admin = YdAdminRoleId;
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var schoolids =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => schoolids.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                }
                var model = list.ToList();
                return View(model);
            }
        }

        /// <summary>
        /// 选择性勾选学生是否申请学士学位情况
        /// </summary>
        /// <returns></returns>
        public string StuDegreeCheckOk()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.isbachelorForcheck = true;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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
        /// 选择性取消勾选学生是否申请学位情况
        /// </summary>
        /// <returns></returns>
        public string StDegreeCCheckno()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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

            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.isbachelorForcheck = false;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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
        /// 选择所有能申请学位学生
        /// </summary>
        /// <returns></returns>
        public string AllDegreeCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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
                var year = Request["EnrollYear"];
                if (string.IsNullOrWhiteSpace(year))
                {
                    return "必须选择年份";
                }
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var enrollYearint = Convert.ToInt32(year);

                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => sub.Contains(u.id));
                    if (school != null)
                    {
                        //只获取该站点没有申请的学生
                        var list =
                            yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major.YD_Edu_EduType).Where(
                                u =>
                                    u.y_isdel == isnotdel && u.y_stuStateId == 6 &&
                                    u.YD_Edu_Major.YD_Edu_EduType.y_name != "高起专" && u.y_inYear == enrollYearint
                                    && u.y_subSchoolId == school.id &&
                                    u.y_bachelordegree != true).ToList();
                        List<int> stuIds = new List<int>();
                        list.ToList().ForEach(u =>
                        {
                            var engisthscore =
                                yunEntities.YD_Graduate_StudentScore.Where(s => s.y_isdel == 1)
                                    .GroupBy(s => new { s.y_stuId, s.y_verdict })
                                    .Select(s => s.Key)
                                    .ToList();
                            var isverdict = engisthscore.FirstOrDefault(k => k.y_stuId == u.id);
                            u.y_isMoneyOk = isverdict != null && isverdict.y_verdict == 1 ? 1 : 0;
                            if (u.y_isMoneyOk == 1)
                            {
                                stuIds.Add(u.id);
                            }
                        });
                        if (stuIds.Count > 0)
                        {
                            var isokStus = yunEntities.YD_Sts_StuInfo.Where(u => stuIds.Contains(u.id)).ToList();
                            //得到保存到的满足申请学位条件的学生
                            if (isokStus.Any())
                            {
                                isokStus.ForEach(k =>
                                {
                                    k.y_isMoneyOk = 1;
                                    k.isbachelorForcheck = true;
                                    yunEntities.Entry(k).State = EntityState.Modified;
                                });
                            }
                        }
                        else
                        {
                            return "没有满足毕业条件的学生";
                        }
                        int r = yunEntities.SaveChanges();
                        if (r > 0)
                        {
                            return "设置成功";
                        }
                        else
                        {
                            return "函授站信息有误！";
                        }
                    }
                    else
                    {
                        return "没有满足毕业条件的学生";
                    }
                }
                else
                {
                    return "账号没有关联站点";
                }
            }
        }

        /// <summary>
        /// 取消选择的所有能申请学位学生
        /// </summary>
        /// <returns></returns>
        public string AllDegreeCheckNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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
                var year = Request["EnrollYear"];
                if (string.IsNullOrWhiteSpace(year))
                {
                    return "必须选择年份";
                }
                const int isnotdel = (int)YesOrNo.No;

                var enrollYearint = Convert.ToInt32(year);

                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => sub.Contains(u.id));
                    if (school != null)
                    {
                        //只获取该站点没申请的学生
                        var list =
                            yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major.YD_Edu_EduType).Where(
                                u =>
                                    u.y_isdel == isnotdel && u.y_stuStateId == 6 &&
                                    u.YD_Edu_Major.YD_Edu_EduType.y_name != "高起专" && u.y_inYear == enrollYearint
                                    && u.y_subSchoolId == school.id && u.isbachelorForcheck == true &&
                                    u.y_bachelordegree != true).ToList();
                        List<int> stuIds = new List<int>();
                        list.ToList().ForEach(u =>
                        {
                            var engisthscore =
                                yunEntities.YD_Graduate_StudentScore.Where(s => s.y_isdel == 1)
                                    .GroupBy(s => new { s.y_stuId, s.y_verdict })
                                    .Select(s => s.Key)
                                    .ToList();
                            var isverdict = engisthscore.FirstOrDefault(k => k.y_stuId == u.id);
                            u.y_isMoneyOk = isverdict != null && isverdict.y_verdict == 1 ? 1 : 0;
                            if (u.y_isMoneyOk == 1)
                            {
                                stuIds.Add(u.id);
                            }
                        });
                        if (stuIds.Count > 0)
                        {
                            var isokStus = yunEntities.YD_Sts_StuInfo.Where(u => stuIds.Contains(u.id)).ToList();
                            //得到保存到的满足申请学位条件的学生
                            if (isokStus.Any())
                            {
                                isokStus.ForEach(k =>
                                {
                                    k.y_isMoneyOk = 1;
                                    k.isbachelorForcheck = false;
                                    yunEntities.Entry(k).State = EntityState.Modified;
                                });
                            }
                        }
                        else
                        {
                            return "没有满足毕业条件的学生";
                        }
                        int r = yunEntities.SaveChanges();
                        if (r > 0)
                        {
                            return "设置成功";
                        }
                        else
                        {
                            return "函授站信息有误！";
                        }
                    }
                    else
                    {
                        return "没有满足毕业条件的学生";
                    }
                }
                else
                {
                    return "账号没有关联站点";
                }
            }
        }

        /// <summary>
        /// 函授站选择性提交学生申请学位名单
        /// </summary>
        /// <returns></returns>
        public string EnterDegree()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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
            if (string.IsNullOrWhiteSpace(inyear) && inyear.Equals("0"))
            {
                return "请选择年份";
            }
            using (var yunEntities = new IYunEntities())
            {
                var inyearint = Convert.ToInt32(inyear);
                var sub =
                    yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                        .Select(u => u.y_subSchoolId)
                        .ToList();
                if (sub.Any())
                {
                    var school = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => sub.Contains(u.id));
                    var obj = new YD_Graduate_Bachelor();
                    if (school != null)
                    {
                        obj.y_SubSchoolId = school.id;
                        obj.SchoolName = school.y_name;
                        //得到该站点毕业允许申请学位但是未申请的学生名单
                        var list =
                            yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major.YD_Edu_EduType).Where(
                                u =>
                                    u.y_isdel == 1 && u.y_stuStateId == 6 &&
                                    u.YD_Edu_Major.YD_Edu_EduType.y_name != "高起专" && u.y_inYear == inyearint
                                    && u.y_subSchoolId == school.id && u.isbachelorForcheck == true &&
                                    u.y_bachelordegree != true).ToList();
                        if (list.Any())
                        {
                            var stuidlist = string.Join(",", list.Select(u => u.id).ToArray()); //所有stuid根据逗号相加成一个字符串

                            obj.totalcount = list.Count();
                            obj.y_time = DateTime.Now;
                            obj.y_check = 0;
                            obj.y_stuid = stuidlist;
                            obj.y_inyear = inyearint;
                            yunEntities.Entry(obj).State = EntityState.Added;
                            list.ForEach(u =>
                            {
                                u.y_bachelordegree = true;
                                u.isbachelorForcheck = false;

                                yunEntities.Entry(u).State = EntityState.Modified;
                            });
                        }
                    }
                    else
                    {
                        return "函授站信息有误！";
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
        /// 学校审核申请学士学位学生名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuDegreeCheck(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var subSchool = Request["SubSchool"];
                var inYear = Request["inYear"];
                var ischeck = Request["isCheck"];
                var year = Request["year"];
                var intyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                IQueryable<YD_Graduate_Bachelor> list =
                    yunEntities.YD_Graduate_Bachelor.OrderByDescending(u => u.id).Where(u => true);

                ViewBag.admin = YdAdminRoleId;

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_SubSchoolId == subSchoolint);
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
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                ViewBag.admin = YdAdminRoleId;

                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize    

                if (Request.IsAjaxRequest())
                    return PartialView("StuDegreeCheckList", model);
                return View(model);
            }
        }

        /// <summary>
        ///函授站提交申请学士学位名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownApplyDegree()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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
                var year = Request["year"];
                IQueryable<YD_Graduate_Bachelor> list =
                    yunEntities.YD_Graduate_Bachelor.OrderByDescending(u => u.id).Where(u => true);
                ViewBag.admin = YdAdminRoleId;
                var schoolName = "";
                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    var schoolfirst = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subSchoolint);
                    schoolName = schoolfirst != null ? schoolfirst.y_name : "";
                    list = list.Where(u => u.y_SubSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var yearint = Convert.ToInt32(inYear);
                    list = list.Where(u => u.y_inyear == yearint);
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
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sys_SubSchool.id));
                }
                var models = list.ToList();
                var listss = models.Select(
                    u =>
                        new
                        {
                            schoolName = u.SchoolName,
                            y_inyear = u.y_inyear,
                            totalcount = u.totalcount,
                            y_time = u.y_time,
                            y_check = u.y_check == 1 ? "已审核" : u.y_check == 2 ? "审核不通过" : "待审核"
                        }).ToList();
                DataTable model = FileHelper.ToDataTable(listss);
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/" + schoolName + "审核申请学士学位名单表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"y_inyear", "入学年份"},
                        {"totalcount", "提交人数"},
                        {"y_time", "提交时间"},
                        {"y_check", "状态"}
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
        /// 函授站提交申请学士学位名单全部人数查看视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult StuDegreeCheckDes(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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

            if (id == null)
            {
                return RedirectToAction("StuDegreeCheck");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var ydBachelor = yunEntities.YD_Graduate_Bachelor.FirstOrDefault(u => u.id == id);
                if (ydBachelor != null)
                {
                    string stuid = ydBachelor.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);
                    IQueryable<VW_StuInfo> list =
                        yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id));
                    if (!list.Any())
                    {
                        return RedirectToAction("StuDegreeCheck");
                    }
                    ViewBag.admin = YdAdminRoleId;
                    //根据名字搜索
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        list = list.Where(u => u.y_name.Contains(name));
                    }
                    //根据入学年份查询
                    if (!string.IsNullOrEmpty(inYear) && !inYear.Equals("0"))
                    {
                        var yInYear = Convert.ToInt16(inYear);
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
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)

                    {
                        var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId);

                        list = list.Where(u => u.y_subSchoolId.HasValue && schoolids.Contains(u.y_subSchoolId.Value));
                    }
                    var model = list.ToList();
                    return View(model);
                }
                else
                {
                    var model = new List<VW_StuInfo>();
                    return View(model);
                }
            }
        }

        /// <summary>
        /// 学士学位申请表
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintStuDegree(int? id)
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
                //获取学生外语成绩
                var degreescore =
                    yunEntities.YD_Graduate_StudentScore.OrderByDescending(u => u.y_sumsore)
                        .FirstOrDefault(u => u.y_stuId == id);
                if (degreescore != null) ViewBag.degreescore = degreescore.y_sumsore;
                //获取学生成绩
                //获取对应主干课程教学计划
                var classCourse = yunEntities.YD_TeaPlan_ClassCourseDes.Where(u => u.y_isMain == true).AsQueryable();

                var stu =
                    yunEntities.VW_StuInfo.Where(u => u.id == id).AsQueryable();

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
                                    TotalScore = u.FirstOrDefault().score.y_totalScore
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();
                }
                ViewBag.stuInfo = stuInfo;
                return View(stuScore);
            }
        }
        //申请学士学位汇总表--科技师范
        public ActionResult StuGridApplyDegree()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/StuGridApplyDegree");
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
                #region
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                #endregion
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请学位的毕业学生名单
                var stu =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType).
                    Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                    OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                    && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                u.y_isgraduate == true).ToList();
                var stuidlist = stu.Select(u => u.id).ToList(); // 获取已申请学位学生的ID集合
                var degree = yunEntities.YD_Graduate_StudentScore.Where(u => stuidlist.Contains((int)u.y_stuId)).ToList(); //得到已申请学生的外语成绩
                ViewBag.degree = degree;
                return View(stu);
            }
        }
        //申请学士学位汇总表--江西理工
        public ActionResult StuGridApplyDegreetoo(string examYear)
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/StuGridApplyDegreetoo");
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
                #region
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                #endregion
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请学位的毕业学生名单
                var stu =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                    .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                    .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                    .Include(u => u.YD_Sys_SubSchool)
                    .Include(u => u.YD_Graduate_StudentScore)
                    .OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                    && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                u.y_isgraduate == true).AsQueryable();
                IQueryable<YD_Graduate_StudentScore> degreestu = yunEntities.YD_Graduate_StudentScore;
                if (!string.IsNullOrWhiteSpace(examYear))
                {
                    degreestu = yunEntities.YD_Graduate_StudentScore.Where(e => e.y_examtime == examYear);
                }
                var degreestuid = degreestu.Select(u => u.y_stuId).ToList(); //得到已申请学生的外语成绩
                stu = stu.Where(u => degreestuid.Contains(u.id));
                var degree = yunEntities.YD_Graduate_StudentScore.Where(u => u.y_isdel == 1).ToList(); //得到已申请学生的外语成绩
                ViewBag.degree = degree;
                ViewBag.examYear = examYear;
                return View(stu.ToList());
            }
        }

        //建议授予学士学位名单导出
        public ActionResult DownStuGridApplyDegree()
        {
            using (var yunEntities = new IYunEntities())
            {
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolname = @ConfigurationManager.AppSettings["SchoolTable"];
                //得到已申请学位的毕业学生名单
                var list =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType).
                    Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                    OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                    && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                u.y_isgraduate == true).ToList();
                var stuidlist = list.Select(u => u.id).ToList(); // 获取已申请学位学生的ID集合
                var degree = yunEntities.YD_Graduate_StudentScore.Where(u => stuidlist.Contains((int)u.y_stuId)).ToList(); //得到已申请学生的外语成绩

                if (list.Any())
                {
                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    //设置单元格内容
                    cell.SetCellValue(schoolname + xinshenyear + "年个人学位申请表汇总表");

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style;

                    //设置单元格的高度
                    row.Height = 30 * 20;
                    //设置单元格的宽度
                    sheet.SetColumnWidth(2, 30 * 256);
                    var styleCell = workbook.CreateCellStyle();
                    styleCell.Alignment = HorizontalAlignment.CENTER;
                    var font2 = workbook.CreateFont();
                    styleCell.SetFont(font2);


                    var row1 = sheet.CreateRow(1);
                    var row2 = sheet.CreateRow(2);
                    row1.CreateCell(0).SetCellValue("序号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                    row1.CreateCell(1).SetCellValue("学校全名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 1, 1));
                    row1.CreateCell(2).SetCellValue("学生姓名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 2));
                    row1.CreateCell(3).SetCellValue("性别");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 3, 3));
                    row1.CreateCell(4).SetCellValue("身份证号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 4, 4));
                    row1.CreateCell(5).SetCellValue("入学时间");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 5, 5));
                    row1.CreateCell(6).SetCellValue("毕业时间");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 6, 6));
                    row1.CreateCell(7).SetCellValue("学位外语考试成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 8));
                    row2.CreateCell(7).SetCellValue("总分");
                    row2.CreateCell(8).SetCellValue("主观分");
                    row1.CreateCell(9).SetCellValue("学位外语准考证号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 9, 9));
                    row1.CreateCell(10).SetCellValue("专业名称");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 10, 10));
                    row1.CreateCell(11).SetCellValue("申请授予学位门类");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));
                    row1.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    for (int i = 0; i < list.Count; i++)
                    {
                        var rowNumber = sheet.PhysicalNumberOfRows;
                        var rowCourse = sheet.CreateRow(rowNumber);
                        rowCourse.CreateCell(0).SetCellValue(i + 1); //序号
                        rowCourse.CreateCell(1).SetCellValue(schoolname); //学校全名
                        rowCourse.CreateCell(2).SetCellValue(list[i].y_name);
                        rowCourse.CreateCell(3).SetCellValue(list[i].y_sex == 0 ? "男" : "女");
                        rowCourse.CreateCell(4).SetCellValue(list[i].y_cardId);
                        rowCourse.CreateCell(5).SetCellValue(list[i].y_inYear);
                        rowCourse.CreateCell(6).SetCellValue("");
                        var score = degree.OrderByDescending(u => u.y_sumsore).FirstOrDefault(u => u.y_stuId == list[i].id);
                        if (score != null)
                        {
                            rowCourse.CreateCell(7).SetCellValue((double)score.y_sumsore);
                            rowCourse.CreateCell(8).SetCellValue((double)score.y_subjectivitysore);
                            rowCourse.CreateCell(9).SetCellValue(score.y_admissionNum);
                        }
                        else
                        {
                            rowCourse.CreateCell(7).SetCellValue("");
                            rowCourse.CreateCell(8).SetCellValue("");
                            rowCourse.CreateCell(9).SetCellValue("");
                        }
                        rowCourse.CreateCell(10).SetCellValue(list[i].YD_Edu_Major.YD_Edu_MajorLibrary.y_name);
                        rowCourse.CreateCell(11).SetCellValue(list[i].YD_Edu_Major.y_GridType);

                        rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    }
                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/建议授予学士学位名单导出" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变
                    //将工作簿写入文件
                    using (FileStream fs = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                }
                else
                {
                    return Content("没有学生名单");

                }
            }
        }

        //申请学士学位名单导出 --理工
        public ActionResult DownStuGridApplyDegreetoo(string examYear)
        {
            using (var yunEntities = new IYunEntities())
            {
                const int isnotdel = (int)YesOrNo.No;
                var schoolname = @ConfigurationManager.AppSettings["SchoolTable"];
                //得到已申请学位的毕业学生名单
                var stu =
                     yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                     Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                     .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                     .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                     .Include(u => u.YD_Sys_SubSchool)
                     .Include(u => u.YD_Graduate_StudentScore)
                     .OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                     && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                 u.y_isgraduate == true).AsQueryable().ToList();
                IQueryable<YD_Graduate_StudentScore> degreestu = yunEntities.YD_Graduate_StudentScore;
                if (!string.IsNullOrWhiteSpace(examYear))
                {
                    degreestu = yunEntities.YD_Graduate_StudentScore.Where(e => e.y_examtime == examYear);
                }
                var degreestuid = degreestu.Select(u => u.y_stuId).ToList(); //得到已申请学生的外语成绩
                var list = stu.Where(u => degreestuid.Contains(u.id)).ToList();
                var degree = yunEntities.YD_Graduate_StudentScore.Where(u => u.y_isdel == 1).ToList(); //得到已申请学生的外语成绩

                if (list.Any())
                {
                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    //设置单元格内容
                    cell.SetCellValue(schoolname + "成人（自学考试）本科毕业生申请学士学位人员名单汇总表");

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style;

                    //设置单元格的高度
                    row.Height = 30 * 20;
                    //设置单元格的宽度
                    sheet.SetColumnWidth(2, 30 * 256);
                    var styleCell = workbook.CreateCellStyle();
                    styleCell.Alignment = HorizontalAlignment.CENTER;
                    var font2 = workbook.CreateFont();
                    styleCell.SetFont(font2);


                    var row1 = sheet.CreateRow(1);
                    var row2 = sheet.CreateRow(2);
                    row1.CreateCell(0).SetCellValue("序号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                    row1.CreateCell(1).SetCellValue("姓名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 1, 1));
                    row1.CreateCell(2).SetCellValue("姓名拼音");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 2));
                    row1.CreateCell(3).SetCellValue("学号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 3, 3));
                    row1.CreateCell(4).SetCellValue("学历层次");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 4, 4));
                    row1.CreateCell(5).SetCellValue("学习形式");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 5, 5));
                    row1.CreateCell(6).SetCellValue("性别");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 6, 6));
                    row1.CreateCell(7).SetCellValue("身份证号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 7, 7));
                    row1.CreateCell(8).SetCellValue("录取时间");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 8, 8));
                    row1.CreateCell(9).SetCellValue("总成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 9, 9));
                    row1.CreateCell(10).SetCellValue("主观题成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 10, 10));
                    row1.CreateCell(11).SetCellValue("学位外语准考证号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 11, 11));
                    row1.CreateCell(12).SetCellValue("通过时间");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 12, 12));
                    row1.CreateCell(13).SetCellValue("专业");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 13, 13));
                    row1.CreateCell(14).SetCellValue("教学站点");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 14, 14));
                    row1.CreateCell(15).SetCellValue("班主任");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 15, 15));

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));
                    row1.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    for (int i = 0; i < list.Count; i++)
                    {
                        var score = degree.OrderByDescending(u => u.y_sumsore).FirstOrDefault(u => u.y_stuId == list[i].id);

                        var rowNumber = sheet.PhysicalNumberOfRows;
                        var rowCourse = sheet.CreateRow(rowNumber);
                        rowCourse.CreateCell(0).SetCellValue(i + 1); //序号
                        rowCourse.CreateCell(1).SetCellValue(list[i].y_name);
                        if (score != null)
                        {
                            rowCourse.CreateCell(2).SetCellValue(score.y_namePinyin); //姓名拼音
                        }
                        else
                        {
                            rowCourse.CreateCell(2).SetCellValue(""); //姓名拼音
                        }

                        rowCourse.CreateCell(3).SetCellValue(list[i].y_stuNum); //学号
                        rowCourse.CreateCell(4).SetCellValue(list[i].YD_Edu_Major.YD_Edu_EduType.y_name); //学历层次
                        rowCourse.CreateCell(5).SetCellValue(list[i].YD_Edu_Major.YD_Edu_StuType.y_name); //学习形式
                        rowCourse.CreateCell(6).SetCellValue(list[i].y_sex == 0 ? "男" : "女");
                        rowCourse.CreateCell(7).SetCellValue(list[i].y_cardId);
                        rowCourse.CreateCell(8).SetCellValue(list[i].y_inYear);
                        if (score != null)
                        {
                            rowCourse.CreateCell(9).SetCellValue((double)score.y_sumsore);
                            rowCourse.CreateCell(10).SetCellValue((double)score.y_subjectivitysore);
                            rowCourse.CreateCell(11).SetCellValue(score.y_admissionNum);
                            rowCourse.CreateCell(12).SetCellValue(score.y_adopttime);
                        }
                        else
                        {
                            rowCourse.CreateCell(9).SetCellValue("");
                            rowCourse.CreateCell(10).SetCellValue("");
                            rowCourse.CreateCell(11).SetCellValue("");
                            rowCourse.CreateCell(12).SetCellValue("");
                        }
                        rowCourse.CreateCell(13).SetCellValue(list[i].YD_Edu_Major.YD_Edu_MajorLibrary.y_name);
                        rowCourse.CreateCell(14).SetCellValue(list[i].YD_Sys_SubSchool.y_name);
                        if (score != null)
                        {
                            rowCourse.CreateCell(15).SetCellValue(score.y_subschoolname);
                        }
                        else
                        {
                            rowCourse.CreateCell(15).SetCellValue("");
                        }


                        rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    }
                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/申请学士学位人员名单汇总表导出" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变
                    //将工作簿写入文件
                    using (FileStream fs = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                }
                else
                {
                    return Content("没有学生名单");

                }
            }
        }

        /// <summary>
        /// 授予学士学位名单
        /// </summary>
        /// <returns></returns>
        public ActionResult StuGridAwardDegree()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridAwardDegree");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请学位的毕业学生名单
                var stu =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                    Include(u => u.YD_Edu_Major.YD_Edu_StuType).
                    Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                    OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                    && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                u.y_isgraduate == true).ToList();

                var stuidlist = stu.Select(u => u.id).ToList(); // 获取已申请学位学生的ID集合
                var degree = yunEntities.YD_Graduate_StudentScore.Where(u => stuidlist.Contains((int)u.y_stuId)).ToList(); //得到已申请学生的外语成绩
                ViewBag.degree = degree;

                return View(stu);
            }
        }
        //建议授予学士学位名单导出
        public ActionResult DownGridAwardDegree()
        {
            using (var yunEntities = new IYunEntities())
            {
                const int isnotdel = (int)YesOrNo.No;
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var list =
                   yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Edu_Major).
                   Include(u => u.YD_Edu_Major.YD_Edu_StuType).
                   Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary).
                   OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel
                   && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                               u.y_isgraduate == true).ToList();

                if (list.Any())
                {
                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    //设置单元格内容
                    cell.SetCellValue(xinshenyear + "年成人本科毕业学生申请学士学位人员名单");

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style;

                    //设置单元格的高度
                    row.Height = 30 * 20;
                    //设置单元格的宽度
                    sheet.SetColumnWidth(2, 30 * 256);
                    var styleCell = workbook.CreateCellStyle();
                    styleCell.Alignment = HorizontalAlignment.CENTER;
                    var font2 = workbook.CreateFont();
                    styleCell.SetFont(font2);


                    var row1 = sheet.CreateRow(1);
                    row1.CreateCell(0).SetCellValue("序号");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 0));
                    row1.CreateCell(1).SetCellValue("姓名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 1));
                    row1.CreateCell(2).SetCellValue("专业");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 2, 2));
                    row1.CreateCell(3).SetCellValue("学习形式");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 3, 3));
                    row1.CreateCell(4).SetCellValue("毕业时间");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 4, 4));
                    row1.CreateCell(5).SetCellValue("课程成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 5, 5));
                    row1.CreateCell(6).SetCellValue("毕业设计（论文）成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 6, 6));
                    row1.CreateCell(7).SetCellValue("学位英语成绩");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 7));
                    row1.CreateCell(8).SetCellValue("审核意见");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));
                    row1.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    for (int i = 0; i < list.Count; i++)
                    {
                        var rowNumber = sheet.PhysicalNumberOfRows;
                        var rowCourse = sheet.CreateRow(rowNumber);
                        rowCourse.CreateCell(0).SetCellValue(i + 1); //序号
                        rowCourse.CreateCell(1).SetCellValue(list[i].y_name);
                        rowCourse.CreateCell(2).SetCellValue(list[i].YD_Edu_Major.YD_Edu_MajorLibrary.y_name);
                        rowCourse.CreateCell(3).SetCellValue(list[i].YD_Edu_Major.YD_Edu_StuType.y_name);
                        rowCourse.CreateCell(4).SetCellValue("");
                        rowCourse.CreateCell(5).SetCellValue("合格");
                        rowCourse.CreateCell(6).SetCellValue("合格");
                        rowCourse.CreateCell(7).SetCellValue("合格");
                        rowCourse.CreateCell(8).SetCellValue("建议授予" + list[i].YD_Edu_Major.y_GridType + "学士学位");

                        rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    }
                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/建议授予学士学位名单导出" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变
                    //将工作簿写入文件
                    using (FileStream fs = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Content(url);
                    }
                }
                else
                {
                    return Content("没有学生名单");

                }
            }
        }


        /// <summary>
        /// 学校审核学生学士学位申请--通过
        /// </summary>
        /// <returns></returns>
        public string StuDegreeCheckSome()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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
                var ydBachelor = yunEntities.YD_Graduate_Bachelor.FirstOrDefault(u => u.id == intid);
                if (ydBachelor != null)
                {
                    ydBachelor.y_check = 1; //审核通过
                    yunEntities.Entry(ydBachelor).State = EntityState.Modified;
                    string stuid = ydBachelor.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    list.ForEach(u =>
                    {
                        var bachelordegreeCheck = true;
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_bachelordegreeCheck]=1  WHERE id=" + u.id);
                        string sql = sb.ToString();
                        yunEntities.Database.ExecuteSqlCommand(sql);
                        //u.y_bachelordegreeCheck = true;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "审核为通过,学生学籍状态为已毕业");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }

        /// <summary>
        /// 选择性审核学生学士学位申请--拒绝
        /// </summary>
        /// <returns></returns>
        public string StuDegreeCheckSomeNo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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
                var ydBachelor = yunEntities.YD_Graduate_Bachelor.FirstOrDefault(u => u.id == intid);
                if (ydBachelor != null)
                {
                    ydBachelor.y_check = 2; //审核不通过
                    yunEntities.Entry(ydBachelor).State = EntityState.Modified;
                    string stuid = ydBachelor.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    list.ForEach(u =>
                    {
                        var bachelordegree = false;
                        var bachelordegreeCheck = false;
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_bachelordegree]=0,y_bachelordegreeCheck=0  WHERE id=" + u.id);
                        string sql = sb.ToString();
                        yunEntities.Database.ExecuteSqlCommand(sql);
                        //u.y_bachelordegree = false;
                        //u.y_bachelordegreeCheck = false;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "审核为不通过,函授站需重新提交毕业名单");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }

        /// <summary>
        /// 选择性审核学生毕业--撤销审核失误操作
        /// </summary>
        /// <returns></returns>
        public string StuDegreeRevocationCheck()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuDegreeCheck");
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
                var ydSBachelor = yunEntities.YD_Graduate_Bachelor.FirstOrDefault(u => u.id == intid);
                if (ydSBachelor != null)
                {
                    ydSBachelor.y_check = 0; //改为待审核状态
                    yunEntities.Entry(ydSBachelor).State = EntityState.Modified;
                    string stuid = ydSBachelor.y_stuid;
                    //将string数组转换成int数组
                    int[] ids = Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        Convert.ToInt32);
                    List<YD_Sts_StuInfo> list =
                        yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id).Where(u => ids.Contains(u.id)).ToList();
                    list.ForEach(u =>
                    {
                        var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                        sb.AppendLine(" SET [y_bachelordegree]=1,isCheckForSchool=null  WHERE id=" + u.id);
                        string sql = sb.ToString();
                        yunEntities.Database.ExecuteSqlCommand(sql);

                        //u.y_bachelordegree = true;
                        //u.y_bachelordegreeCheck = null;
                        //yunEntities.Entry(u).State = EntityState.Modified;
                    });
                    yunEntities.SaveChanges();
                    LogHelper.DbLog(YdAdminId, YdAdminRelName, (int)LogType.Update,
                        "时间:" + DateTime.Now + "撤销之前审核操作失误，还原到初始状态");
                    return "ok";
                }
                else
                {
                    return "审核失败!";
                }
            }
        }

        /// <summary>
        /// 学位外语报考名单下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStuGegree()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StudentGegreeEnglish");
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

                var birthday = Request["birthday"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var namenumcard = Request["namenumcard"];
                var isok = Request["isok"]; //是否允许申报
                var isup = Request["isup"]; //是否已申请

                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(u => u.y_isdel == isnotdel && u.y_stuStateId == 6
                                    && u.eduTypeName != "高起专" && u.y_bachelordegreeCheck != true);
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_name.Contains(namenumcard) || u.y_stuNum == namenumcard || u.y_cardId == namenumcard);
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
                if (!string.IsNullOrWhiteSpace(isup) && !isup.Equals("0"))
                {
                    if (isup == "1") //已申请
                    {
                        list = list.Where(u => u.y_bachelordegree == true);
                    }
                    else if (isup == "2") //未申请
                    {
                        list = list.Where(u => u.y_bachelordegree == false);
                    }
                }
                if (!string.IsNullOrWhiteSpace(isok) && !isok.Equals("0"))
                {
                    var engisthscore =
                        yunEntities.YD_Graduate_StudentScore.Where(s => s.y_isdel == 1 && s.y_verdict == 1)
                            .GroupBy(s => s.y_stuId)
                            .Select(s => s.Key)
                            .ToList();

                    if (isok == "1") //允许申报
                    {
                        list = list.Where(u => engisthscore.Contains(u.id));

                    }
                    else if (isok == "2") //不允许申报
                    {
                        list = list.Where(u => !engisthscore.Contains(u.id));
                    }
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_name = u.y_name,
                                    y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_stuNum = u.y_stuNum,
                                    y_examNum = u.y_examNum,
                                    majorLibraryName = u.majorLibraryName,
                                    eduTypeName = u.eduTypeName,
                                    stuTypeName = u.stuTypeName,
                                    schoolName = u.schoolName,
                                    y_cardId = u.y_cardId,
                                    y_birthday = u.y_birthday,
                                    nationName = u.nationName,
                                    politicsName = u.politicsName,
                                    stuStateName = u.stuStateName,
                                    y_address = u.y_address,
                                    y_inYear = u.y_inYear,
                                    y_bachelordegree = u.y_bachelordegree == true ? "已申报" : "未申报"
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学生报考学位英语表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/学生报考学位英语表" + Guid.NewGuid() + ".xls";
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
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"y_cardId", "身份证"},
                        {"y_birthday", "出生日期"},
                        {"nationName", "民族"},
                        {"politicsName", "政治面貌"},
                        {"stuStateName", "学籍状态"},
                        {"y_address", "地址"},
                        {"y_inYear", "入学年份"},
                        {"y_degreeOK", "是否报考学位英语"}
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

        /// <summary>
        /// 学位外语成绩查询页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuGridEnglishScore(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridEnglishScore");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion
            var namenumcard = Request["namenumcard"];
            var enrollYear = Request["EnrollYear"];
            var subSchool = Request["SubSchool"];
            var eduType = Request["EduType"];
            var majorLibrary = Request["MajorLibrary"];
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                const int isnotdel = (int)YesOrNo.No;
                ViewBag.adminrole = YdAdminRoleId;
                IQueryable<YD_Graduate_StudentScore> list =
                    yunEntities.YD_Graduate_StudentScore.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major)
                    .Include(u => u.YD_Sts_StuInfo.YD_Sys_SubSchool)
                    .OrderByDescending(u => u.id).Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel);

                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuname.Contains(namenumcard) || u.y_cardId == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.YD_Sts_StuInfo.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var edutypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                if (Request.IsAjaxRequest())
                    return PartialView("StuGridEnglishScoreList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 学位外语成绩导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult UploadNewStuGridEnglishScore(string fileName)
        {
            fileName = Server.MapPath(fileName);
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
                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);
                int index = 0;
                var list = CoreFunction.StuGridEnglishScoreTemplet(ref index, sheet, styleCell);
                if (index > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/学位外语成绩导入表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return
                            Content(
                                "<script type='text/javascript'>alert('Excel验证失败，点击确认获取Excel错误提示！'); window.location.href='" +
                                url + "';</script >");
                    }
                }
                else
                {
                    CoreFunction.UploadTrueGridEnglishScore(list);
                    using (var ad = new IYunEntities())
                    {
                        //var schoolname = @ConfigurationManager.AppSettings["SchoolName"];
                        //var sb = new StringBuilder("INSERT INTO YD_Graduate_StudentScore ");
                        //sb.AppendLine("([y_stuId],[y_examtime],[y_admissionNum],[y_stuname],[y_sex],[y_verdict],[y_inYear],[y_cardId],[y_subjectivitysore],[y_sumsore],[y_explain],[y_remark],[y_adopttime],[y_isdel],[y_namePinyin],[y_subschoolname],[y_createtime]) VALUES");
                        //var inde = 0;

                        //for (int i = 0; i < list.Count; i++)
                        //{
                        //    var item = list[i];

                        //    if (ad.YD_Graduate_StudentScore.Any(u => u.y_stuId == item.y_stuId && u.y_isdel == 1))
                        //    {
                        //        continue;
                        //    }
                        //    inde++;
                        //    if (schoolname == Common.ComEnum.SchoolName.JXLG.ToString())
                        //    {
                        //        sb.AppendLine($"({item.y_stuId},'{item.y_examtime}','{item.y_admissionNum}','{item.y_stuname}',null,{item.y_verdict},{item.y_inYear},'{item.y_cardId}',{item.y_subjectivitysore},{item.y_sumsore},'{item.y_explain}','{item.y_remark}',{item.y_adopttime},1,'{item.y_namePinyin}','{item.y_subschoolname}','{item.y_createtime}',)");
                        //    }
                        //    else
                        //    {
                        //        sb.AppendLine($"({item.y_stuId},'{item.y_examtime}','{item.y_admissionNum}','{item.y_stuname}',{item.y_sex},{item.y_verdict},{item.y_inYear},'{item.y_cardId}',{item.y_subjectivitysore},{item.y_sumsore},'{item.y_explain}','{item.y_remark}',{item.y_adopttime},1,'{item.y_namePinyin}','{item.y_subschoolname}','{item.y_createtime}',)");
                        //    }

                        //    if (inde == 900 || i == list.Count - 1)
                        //    {
                        //        string sql = sb.ToString(0, sb.Length - 3);

                        //        ad.Database.ExecuteSqlCommand(sql);

                        //        inde = 0;

                        //        sb = new StringBuilder("INSERT INTO YD_Graduate_StudentScore ");
                        //        sb.AppendLine("([y_stuId],[y_examtime],[y_admissionNum],[y_stuname],[y_sex],[y_verdict],[y_inYear],[y_cardId],[y_subjectivitysore],[y_sumsore],[y_explain],[y_remark],[y_adopttime],[y_isdel],[y_namePinyin],[y_subschoolname],[y_createtime]) VALUES");
                        //    }
                        //}
                        return
                   Content("<script type='text/javascript'>alert('导入成功,导入" + list.Count +
                           "条数据');window.location.href='/Graduate/StuGridEnglishScore';</script >");
                    }
                }
            }
        }

        #region 没用代码

        /// <summary>
        /// 批量导入学生学生学位外语成绩
        /// </summary>
        /// <returns></returns>
        //public ActionResult UploadStuGridEnglishScore(int id = 1)
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Graduate/StuGridEnglishScore");
        //    if (!IsLogin())
        //    {
        //        return Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_menu == (int)PowerState.Disable)
        //    {
        //        var reurl = Request.UrlReferrer.ToString();
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
        //        int colCount = 10;
        //        if (dt.Columns.Count != colCount)
        //        {
        //            return Content("该文件不合法，请确认是否多列或少列！");
        //        }
        //        //验证excel文件列信息是否正确
        //        string column = "考试时间，入学年份，姓名，性别,身份证号，专业名，准考证号，总分，主观分，结论";
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
        //            yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Graduate_EnglishScoreTemp");
        //            if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //            {
        //                ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
        //            }
        //            else if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //            {
        //                ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
        //            }
        //            else
        //            {
        //                ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
        //            }
        //            string examdate = "";
        //            DateTime y_examtime = DateTime.MaxValue;
        //            string y_admissionNum = "";
        //            int y_subjectivitysore = -1;
        //            string name = "";
        //            int sex = -1;
        //            string sexName = "";
        //            string stunameMatch = ""; //用来备注是否存在数据库
        //            int year = -1;
        //            string cardId = "";
        //            string cardIdMatch = "";//用来备注是否存在数据库
        //            int majorLibId = -1;
        //            string majorLibName = "";
        //            string majorNameMatch = "";
        //            int y_subSchoolId = -1;
        //            string subSchoolName = "";
        //            int y_sumsore = -1;
        //            string y_explain = "";
        //            int y_verdict = -1;
        //            string y_verdictname = "";
        //            string y_remark = "";
        //            int isOk = (int)YesOrNo.No;
        //            var studentTempList = new List<YD_Graduate_EnglishScoreTemp>();
        //            for (var i = 0; i < dt.Rows.Count; i++)
        //            {
        //                y_examtime = DateTime.MaxValue;
        //                name = "";
        //                sex = -1;
        //                sexName = "";
        //                stunameMatch = "";//用来备注是否存在数据库
        //                cardIdMatch = "";//用来备注是否存在数据库
        //                year = -1;
        //                cardId = "";
        //                majorLibId = -1;
        //                majorLibName = "";
        //                majorNameMatch = "";
        //                y_subSchoolId = -1;
        //                subSchoolName = "";
        //                y_admissionNum = "";
        //                y_subjectivitysore = -1;
        //                y_sumsore = -1;
        //                y_explain = "";
        //                y_verdict = -1;
        //                y_verdictname = "";
        //                y_remark = "";
        //                isOk = (int)YesOrNo.No;
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

        //                if (dt.Rows[i]["考试时间"] != null)
        //                {
        //                    examdate = dt.Rows[i]["考试时间"].ToString();
        //                    if (examdate != "")
        //                    {
        //                        DateTime.TryParse(examdate.Substring(0, 4) + "-" + examdate.Substring(4, 2) + "-" +
        //                            examdate.Substring(6, 2), out y_examtime);
        //                    }
        //                    else
        //                    {
        //                        y_examtime = DateTime.MaxValue;
        //                    }
        //                }
        //                #region 获取专业库id
        //                if (dt.Rows[i]["专业名"] != null)
        //                {
        //                    name = dt.Rows[i]["姓名"].ToString().Trim();
        //                    majorLibName = dt.Rows[i]["专业名"].ToString();
        //                    var entity = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == majorLibName);
        //                    var stu = yunEntities.VW_StuInfo.FirstOrDefault(u => u.y_name == name && u.majorLibraryName == majorLibName);
        //                    if (entity != null && stu == null)
        //                        majorLibId = entity.id;
        //                    else
        //                    {
        //                        majorLibId = -1;
        //                    }

        //                }
        //                #endregion
        //                if (dt.Rows[i]["准考证号"] != null)
        //                {
        //                    y_admissionNum = dt.Rows[i]["准考证号"].ToString();
        //                }
        //                if (dt.Rows[i]["身份证号"] != null)
        //                {
        //                    name = dt.Rows[i]["姓名"].ToString().Trim();
        //                    cardId = dt.Rows[i]["身份证号"].ToString();
        //                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_cardId == cardId && u.y_name == name);
        //                    if (stu != null)
        //                    {
        //                        var sub = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == stu.y_subSchoolId);
        //                        y_subSchoolId = sub.id;
        //                        subSchoolName = sub.y_name;
        //                    }
        //                    else
        //                    {
        //                        cardIdMatch = "-1";
        //                    }
        //                }
        //                if (dt.Rows[i]["姓名"] != null)
        //                {
        //                    name = dt.Rows[i]["姓名"].ToString().Trim();
        //                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_name == name);
        //                    if (stu == null)
        //                    {
        //                        stunameMatch = "-1";
        //                    }
        //                }
        //                if (dt.Rows[i]["主观分"] != null)
        //                {
        //                    int.TryParse(dt.Rows[i]["主观分"].ToString().Trim(), out y_subjectivitysore);
        //                }
        //                if (dt.Rows[i]["总分"] != null)
        //                {
        //                    int.TryParse(dt.Rows[i]["总分"].ToString().Trim(), out y_sumsore);
        //                }
        //                //if (dt.Rows[i]["说明"] != null)
        //                //{
        //                //    y_explain = dt.Rows[i]["说明"].ToString().Trim();
        //                //}
        //                #region 结论id

        //                if (dt.Rows[i]["结论"] != null)
        //                {
        //                    y_verdictname = dt.Rows[i]["结论"].ToString().Trim();
        //                    switch (y_verdictname)
        //                    {
        //                        case "不通过":
        //                            y_verdict = 0;
        //                            break;
        //                        case "通过":
        //                            y_verdict = 1;
        //                            break;
        //                        default:
        //                            y_verdict = -1;
        //                            break;
        //                    }
        //                }

        //                #endregion
        //                //if (dt.Rows[i]["备注"] != null)
        //                //{
        //                //    y_remark = dt.Rows[i]["备注"].ToString().Trim();
        //                //}
        //                if (y_verdict != -1 && sex != -1 && y_examtime != DateTime.MaxValue && stunameMatch != "-1" && cardIdMatch != "-1" && y_admissionNum != "" && majorLibId != -1 && year != -1 && name != "")
        //                {
        //                    isOk = (int)YesOrNo.Yes;
        //                }
        //                else
        //                {
        //                    isOk = (int)YesOrNo.No;
        //                }
        //                var studentTemp = new YD_Graduate_EnglishScoreTemp
        //                {
        //                    y_examtime = y_examtime,
        //                    y_admissionNum = y_admissionNum,
        //                    y_stuname = name,
        //                    y_sex = sex,
        //                    y_sexName = sexName,
        //                    y_stunameMatch = stunameMatch,
        //                    y_cardIdMatch = cardIdMatch,
        //                    y_inYear = year,
        //                    y_cardId = cardId,
        //                    y_majorLibId = majorLibId,
        //                    y_majorLibName = majorLibName,
        //                    y_majorNameMatch = majorNameMatch,
        //                    y_subSchoolId = y_subSchoolId,
        //                    y_subSchoolName = subSchoolName,
        //                    y_subjectivitysore = y_subjectivitysore,
        //                    y_sumsore = y_sumsore,
        //                    y_explain = y_explain,
        //                    y_verdict = y_verdict,
        //                    y_verdictname = y_verdictname,
        //                    y_remark = y_remark,
        //                    y_isOk = isOk,
        //                };
        //                studentTempList.Add(studentTemp);
        //            }
        //            yunEntities.Configuration.AutoDetectChangesEnabled = false;
        //            yunEntities.Configuration.ValidateOnSaveEnabled = false;
        //            yunEntities.Set<YD_Graduate_EnglishScoreTemp>().AddRange(studentTempList);
        //            yunEntities.SaveChanges();
        //            var list = yunEntities.YD_Graduate_EnglishScoreTemp.Where(u => true).ToList();
        //            var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
        //            const int iisOk = (int)YesOrNo.No;
        //            ViewBag.teachPlanList = yunEntities.YD_Graduate_EnglishScoreTemp.Where(u => u.y_isOk == iisOk).Take(15).ToList();
        //            if (Request.IsAjaxRequest())
        //                return PartialView("StudEnglishUpdateList", dbLogList);
        //            return View(dbLogList);
        //        }
        //    }
        //}



        /// <summary>
        /// 验证导入的临时学生信息
        /// </summary>
        /// <returns></returns>
        //public ActionResult VerifyEnglishScore(int id = 1)
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Graduate/StuGridEnglishScore");
        //    if (!IsLogin())
        //    {
        //        return Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_menu == (int)PowerState.Disable)
        //    {
        //        var reurl = Request.UrlReferrer.ToString();
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }
        //    #endregion
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        //如果数据库存在导入的教学计划则不导入
        //        var alikesubstu = yunEntities.YD_Graduate_EnglishScoreTemp.ToList();
        //        var lib = yunEntities.VW_GraduateStuScore.Where(u => u.y_isdel == (int)YesOrNo.No).AsQueryable();
        //        if (alikesubstu.Any())
        //        {
        //            foreach (var item in alikesubstu)
        //            {
        //                if (lib.Any(a => a.y_stuname == item.y_stuname && a.y_majorLibId == item.y_majorLibId && a.y_cardId == item.y_cardId && a.y_subSchoolId == item.y_subSchoolId && a.y_admissionNum == item.y_admissionNum && a.y_inYear == item.y_inYear))
        //                {
        //                    yunEntities.YD_Graduate_EnglishScoreTemp.Remove(item);
        //                }
        //            }
        //            int r = yunEntities.SaveChanges();
        //            if (r > 0)
        //            {
        //                return Content("<script type='text/javascript'>alert('有重复学位外语成绩不导入，重复条数为" + r + "');window.location.href='/Graduate/VerifyEnglishScore';</script >");
        //            }
        //        }
        //        const int isOk = (int)YesOrNo.No;
        //        var list = yunEntities.YD_Graduate_EnglishScoreTemp.Where(u => u.y_isOk == isOk).ToList();
        //        var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
        //        ViewBag.teachPlanList = yunEntities.YD_Graduate_EnglishScoreTemp.Where(u => u.y_isOk == isOk).ToList();

        //        if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
        //        }
        //        else if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
        //        }
        //        else
        //        {
        //            ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
        //        }
        //        if (Request.IsAjaxRequest())
        //            return PartialView("erifyEnglishScoreList", dbLogList);
        //        return View(dbLogList);
        //    }
        //}
        /// <summary>
        /// 提交用户更新的临时数据
        /// </summary>
        /// <returns></returns>
        //public ActionResult UpdateEnglishVerify()
        //{
        //    var examtimes = Request["examtime"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var names = Request["name"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var examNums = Request["examNum"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var sexs = Request["sex"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var majorLibs = Request["majorLib"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var years = Request["year"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var cardIds = Request["cardId"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var subjectivitysores = Request["subjectivitysore"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var sumsores = Request["sumsore"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var verdicts = Request["verdict"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var subSchools = Request["subSchool"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var ids = Request["id"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    DateTime examtime = DateTime.MaxValue;
        //    string name = "";
        //    string stunameMatch = "";
        //    int sex = -1;
        //    string sexName = "";
        //    int year = -1;
        //    string cardId = "";
        //    string cardIdMatch = "";
        //    int majorId = -1;
        //    int majorLibId = -1;
        //    string majorLibName = "";
        //    string majorNameMatch = "";
        //    int subSchoolId = -1;
        //    string subSchoolName = "";
        //    string examNum = "";
        //    int subjectivitysore = 0;
        //    int sumsore = 0;
        //    int verdict = -1;
        //    string verdictname = "";

        //    int isOk = (int)YesOrNo.No;
        //    int id = 0;
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (var i = 0; i < ids.Count(); i++)
        //        {
        //            examtime = DateTime.MaxValue;
        //            name = "";
        //            stunameMatch = "";
        //            examNum = "";
        //            sex = -1;
        //            sexName = "";
        //            year = -1;
        //            cardId = "";
        //            cardIdMatch = "";
        //            majorId = -1;
        //            majorLibId = -1;
        //            majorLibName = "";
        //            majorNameMatch = "";
        //            subSchoolId = -1;
        //            subjectivitysore = 0;
        //            sumsore = 0;
        //            verdict = -1;
        //            verdictname = "";
        //            id = Convert.ToInt32(ids[i]);
        //            #region  名字
        //            name = names[i];
        //            var entity = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_name == name);
        //            if (entity == null)
        //                stunameMatch = "-1";
        //            #endregion
        //            examNum = examNums[i];
        //            subjectivitysore = Convert.ToInt32(subjectivitysores[i]);
        //            sumsore = Convert.ToInt32(sumsores[i]);
        //            var teachPlan = yunEntities.YD_Graduate_EnglishScoreTemp.FirstOrDefault(u => u.id == id);
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
        //                if (verdicts[i] == "不通过")
        //                {
        //                    verdict = 0;
        //                }
        //                else if (verdicts[i] == "通过")
        //                {
        //                    verdict = 1;
        //                }
        //                else
        //                {
        //                    verdict = -1;
        //                }
        //                verdictname = verdicts[i];
        //                sexName = sexs[i];
        //                if (!int.TryParse(years[i], out year))
        //                {
        //                    year = -1;
        //                }
        //                if (!string.IsNullOrEmpty(examtimes[i]))
        //                {
        //                    examtime = Convert.ToDateTime(examtimes[i]);
        //                }
        //                #region  身份证号
        //                cardId = cardIds[i];
        //                var card = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_cardId == cardId && u.y_name == name);
        //                if (card == null)
        //                    cardIdMatch = "-1";
        //                #endregion
        //                #region  专业
        //                majorLibName = majorLibs[i].Trim();

        //                var MajorLi = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == majorLibName);
        //                var stu = yunEntities.VW_StuInfo.FirstOrDefault(u => u.majorLibraryName == majorLibName && u.y_name == name);
        //                if (MajorLi != null && stu != null)
        //                    majorLibId = MajorLi.id;
        //                else
        //                {
        //                    majorLibId = -1;
        //                }
        //                #endregion

        //                #region 获取函授站id

        //                subSchoolName = subSchools[i].Trim().Replace(" ", "");
        //                var entity5 = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.y_name == subSchoolName);
        //                if (entity5 != null)
        //                {
        //                    subSchoolId = entity5.id;
        //                }
        //                #endregion
        //                if (verdict != -1 && stunameMatch != "-1" && cardIdMatch != "-1" && majorLibId != -1 && sex != -1 && year != -1 && year != -1 && name != "")
        //                {
        //                    isOk = (int)YesOrNo.Yes;
        //                }
        //                else
        //                {
        //                    isOk = (int)YesOrNo.No;
        //                }
        //                teachPlan.y_stuname = name;
        //                teachPlan.y_sex = sex;
        //                teachPlan.y_sexName = sexName;
        //                teachPlan.y_inYear = year;
        //                teachPlan.y_cardId = cardId;
        //                teachPlan.y_cardIdMatch = cardIdMatch;
        //                teachPlan.y_stunameMatch = stunameMatch;
        //                teachPlan.y_majorLibId = majorLibId;
        //                teachPlan.y_majorLibName = majorLibName;
        //                teachPlan.y_admissionNum = examNum;
        //                teachPlan.y_subSchoolId = subSchoolId;
        //                teachPlan.y_examtime = examtime;
        //                teachPlan.y_verdict = verdict;
        //                teachPlan.y_verdictname = verdictname;
        //                teachPlan.y_subjectivitysore = subjectivitysore;
        //                teachPlan.y_sumsore = sumsore;
        //                teachPlan.y_isOk = isOk;
        //                teachPlan.y_majorNameMatch = majorNameMatch;
        //                yunEntities.Entry(teachPlan).State = EntityState.Modified;
        //            }
        //        }
        //        var t = yunEntities.SaveChanges();
        //        return Content("ok");
        //    }
        //}
        /// <summary>
        /// 将验证无误的数据进行导入
        /// </summary>
        /// <returns></returns>
        //public ActionResult UploadTrueEnglishScore()
        //{
        //    #region 权限验证

        //    var power = SafePowerPage("/Graduate/StuGridEnglishScore");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_insert == (int)PowerState.Disable)
        //    {
        //        var reurl = Request.UrlReferrer.ToString();
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }

        //    #endregion
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        const int isOk = (int)YesOrNo.Yes;
        //        var scoreList = yunEntities.YD_Graduate_EnglishScoreTemp.Where(u => u.y_isOk == isOk).ToList();
        //        //如果数据库存在导入的学位外语成绩则不导入
        //        var lib = yunEntities.VW_GraduateStuScore.Where(u => u.y_isdel == (int)YesOrNo.Yes).ToList();

        //        var scoreListTemp = new List<YD_Graduate_StudentScore>();
        //        for (var i = 0; i < scoreList.Count; i++)
        //        {

        //            var name = scoreList[i].y_stuname;
        //            var card = scoreList[i].y_cardId;
        //            var majorLibId = scoreList[i].y_majorLibId;
        //            var examNum = scoreList[i].y_admissionNum;
        //            var year = scoreList[i].y_inYear;
        //            if (lib.Any(a => a.y_stuname == name && a.y_majorLibId == majorLibId
        //           && a.y_cardId == card && a.y_admissionNum == examNum && a.y_inYear == year))
        //            {
        //                yunEntities.Entry(scoreList[i]).State = EntityState.Deleted;
        //            }
        //            int r = yunEntities.SaveChanges();
        //            if (r > 0)
        //            {
        //                return Content("<script type='text/javascript'>alert('有重复学位外语成绩不导入，重复条数为" + r + "');window.location.href='/Graduate/VerifyEnglishScore';</script >");
        //            }
        //            var subschool = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_name == name && u.y_cardId == card);
        //            var score = new YD_Graduate_StudentScore();
        //            if (subschool != null && subschool.y_subSchoolId != -1)
        //            {
        //                score = new YD_Graduate_StudentScore
        //                {
        //                    y_examtime = scoreList[i].y_examtime,
        //                    y_admissionNum = scoreList[i].y_admissionNum,
        //                    y_stuname = scoreList[i].y_stuname,
        //                    y_sex = scoreList[i].y_sex,
        //                    y_verdict = Convert.ToInt32(scoreList[i].y_verdict),
        //                    y_inYear = Convert.ToInt32(scoreList[i].y_inYear),
        //                    y_majorLibId = Convert.ToInt32(scoreList[i].y_majorLibId),
        //                    y_subSchoolId = (int)subschool.y_subSchoolId,
        //                    y_cardId = scoreList[i].y_cardId,
        //                    y_subjectivitysore = Convert.ToInt32(scoreList[i].y_subjectivitysore),
        //                    y_sumsore = Convert.ToInt32(scoreList[i].y_sumsore),
        //                    y_explain = scoreList[i].y_explain,
        //                    y_remark = scoreList[i].y_remark,
        //                    y_isdel = (int)YesOrNo.No
        //                };
        //            }
        //            else
        //            {
        //                score = new YD_Graduate_StudentScore
        //                {
        //                    y_examtime = scoreList[i].y_examtime,
        //                    y_admissionNum = scoreList[i].y_admissionNum,
        //                    y_stuname = scoreList[i].y_stuname,
        //                    y_verdict = Convert.ToInt32(scoreList[i].y_verdict),
        //                    y_inYear = Convert.ToInt32(scoreList[i].y_inYear),
        //                    y_majorLibId = Convert.ToInt32(scoreList[i].y_majorLibId),
        //                    y_subSchoolId = Convert.ToInt32(scoreList[i].y_subSchoolId),
        //                    y_cardId = scoreList[i].y_cardId,
        //                    y_subjectivitysore = Convert.ToInt32(scoreList[i].y_subjectivitysore),
        //                    y_sumsore = Convert.ToInt32(scoreList[i].y_sumsore),
        //                    y_explain = scoreList[i].y_explain,
        //                    y_remark = scoreList[i].y_remark,
        //                    y_isdel = (int)YesOrNo.No
        //                };
        //            }
        //            scoreListTemp.Add(score);
        //        }
        //        yunEntities.Configuration.AutoDetectChangesEnabled = false;
        //        yunEntities.Configuration.ValidateOnSaveEnabled = false;
        //        yunEntities.Set<YD_Graduate_StudentScore>().AddRange(scoreListTemp);
        //        yunEntities.SaveChanges();
        //        return Redirect("StuGridEnglishScore/?id=1");
        //    }
        //}

        #endregion

        /// <summary>
        /// 学生外语成绩数据下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadEnglishScore()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridEnglishScore");
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
                var namenumcard = Request["namenumcard"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var eduType = Request["EduType"];
                var majorLibrary = Request["MajorLibrary"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Graduate_StudentScore> list =
                    yunEntities.YD_Graduate_StudentScore.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_stuname.Contains(namenumcard) || u.y_cardId == namenumcard);
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                }
                if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
                {
                    var edutypeint = Convert.ToInt32(eduType);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_eduTypeId == edutypeint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Sts_StuInfo.YD_Edu_Major.y_majorLibId == majorLibraryint);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => subSchoolIds.Contains(u.YD_Sts_StuInfo.y_subSchoolId.Value));
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    //y_examtime = u.y_examtime,
                                    y_adopttime = u.y_adopttime,
                                    y_admissionNum = u.y_admissionNum,
                                    y_stuname = u.y_stuname,
                                    y_namePinyin = u.y_namePinyin,
                                    //y_sex = u.y_sex == 0 ? "男" : "女",
                                    y_inYear = u.y_inYear,
                                    majorLibName = u.YD_Sts_StuInfo.YD_Edu_Major.y_name,
                                    subschoolname = u.YD_Sts_StuInfo.YD_Sys_SubSchool.y_nameabbreviation,
                                    y_cardId = u.y_cardId,
                                    y_subjectivitysore = u.y_subjectivitysore,
                                    y_sumsore = u.y_sumsore,
                                    y_verdict = u.y_verdict == 0 ? "不通过" : "通过"
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学位外语成绩表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/学位外语成绩表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        //{"y_examtime", "考试时间"},
                        {"y_adopttime","通过时间"},
                        {"y_admissionNum", "准考证号"},
                        {"y_stuname", "姓名"},
                        {"y_namePinyin","拼音"},
                        //{"y_sex", "性别"},
                        {"y_inYear", "年级"},
                        {"majorLibName", "专业"},
                        {"subschoolname","站点（班主任" },
                        {"y_cardId", "身份证号"},
                        {"y_subjectivitysore", "主观分"},
                        {"y_sumsore", "总分"},
                        {"y_verdict", "结论"}
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

        /// <summary>
        /// 学生外语成绩编辑视图
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult EditEnglishScore(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridEnglishScore");
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
                return RedirectToAction("StuGridEnglishScore");
            }
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var student = yunEntities.YD_Graduate_StudentScore.Include(u => u.YD_Sts_StuInfo)
                    .Include(u => u.YD_Sts_StuInfo.YD_Edu_Major).SingleOrDefault(u => u.id == id.Value);
                if (student == null)
                {
                    return RedirectToAction("StuGridEnglishScore");
                }
                ViewData["student"] = student;
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        /// <summary>
        /// 学生外语成绩修改信息编辑AJAX
        /// </summary>
        /// <param name="stu">学生信息对象</param>
        /// <returns>处理结果json</returns>
        public string EditEnglishScoreInfo()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridEnglishScore");
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
            var examtime = Request["examtime"];
            var admissionNum = Request["admissionNum"];
            var sumsore = Request["sumsore"];
            var subjectivitysore = Request["subjectivitysore"];
            var verdict = Request["verdict"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(examtime))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(admissionNum))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(sumsore))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(subjectivitysore))
            {
                return "未知错误";
            }
            if (string.IsNullOrWhiteSpace(verdict))
            {
                return "未知错误";
            }
            using (var ad = new IYunEntities())
            {
                var ids = Convert.ToInt32(id);
                var engisth = ad.YD_Graduate_StudentScore.FirstOrDefault(u => u.id == ids);
                if (engisth != null)
                {
                    engisth.y_examtime = examtime/*Convert.ToDateTime(examtime)*/;
                    engisth.y_admissionNum = admissionNum;
                    engisth.y_sumsore = Convert.ToInt32(sumsore);
                    engisth.y_subjectivitysore = Convert.ToInt32(subjectivitysore);
                    engisth.y_verdict = Convert.ToInt32(verdict);

                    ad.Entry(engisth).State = EntityState.Modified;
                }
                int t = ad.SaveChanges();
                if (t > 0)
                {
                    return "ok";
                }
                else
                {
                    return "保存失败";
                }

            }
        }

        /// <summary>
        /// 学生信息软删除AJAX
        /// </summary>
        /// <param name="id">学生id</param>
        /// <returns>处理结果json</returns>
        public string DeleStuEnglishScoreById(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/StuGridEnglishScore");
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
                var stu = ad.YD_Graduate_StudentScore.Find(id);
                if (stu == null)
                {
                    return "删除学生学位外语成绩失败！编号无效";
                }
                stu.y_isdel = (int)YesOrNo.Yes;
                ad.Entry(stu).State = EntityState.Modified;
                LogHelper.DbLog(Convert.ToInt32(System.Web.HttpContext.Current.Session[KeyValue.Admin_id]),
                    System.Web.HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete,
                    "删除学生学位外语成绩,id：" + stu.id);
                var j = ad.SaveChanges();
                var msg = j > 0 ? "ok" : "删除学生学位外语成绩失败";
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


        //抽考成绩录入页面
        public ActionResult GridSampleEExamScore()
        {
            #region 权限验证

            var power = SafePowerPage("/Graduate/GridSampleEExamScore");
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
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var yearStr = Request.Form["EnrollYear2"];
                int year;
                if (string.IsNullOrWhiteSpace(yearStr))
                {
                    year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                }
                else
                {
                    year = Convert.ToInt32(yearStr);
                }

                var schoolStr = Request.Form["SubSchool"];
                int schoolid = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);
                var majorLIStr = Request.Form["MajorLibrary"];
                int majorlibid = string.IsNullOrWhiteSpace(majorLIStr) ? 0 : Convert.ToInt32(majorLIStr);

                var eduStr = Request["EduType"];
                int eduid = string.IsNullOrWhiteSpace(eduStr) ? 0 : Convert.ToInt32(eduStr);
                var stuStr = Request["StuType"];
                int stuTypeid = string.IsNullOrWhiteSpace(stuStr) ? 0 : Convert.ToInt32(stuStr);

                int islf = Convert.ToInt32(Request["islf"]);
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请学位的毕业学生名单
                IQueryable<YD_Sts_StuInfo> stu =
                    yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Where(
                            u =>
                                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                                u.y_isgraduate == true);
                stu =
                    stu.Where(
                        u =>
                            u.y_inYear == year && u.y_subSchoolId == schoolid &&
                            u.YD_Edu_Major.y_eduTypeId == eduid && u.YD_Edu_Major.y_stuTypeId == stuTypeid &&
                            u.YD_Edu_Major.y_majorLibId == majorlibid);

                var clist =
                    yunEntities.YD_TeaPlan_ClassCourseDes.Include(u => u.YD_Edu_Course)
                        .Include(u => u.YD_TeaPlan_Class)
                        .AsQueryable();
                clist =
                    clist.Where(
                        u =>
                            u.YD_TeaPlan_Class.y_subSchoolId == schoolid && u.YD_TeaPlan_Class.y_year == year &&
                            u.y_sampleexam == true);
                //函授站年份筛选
                clist =
                    clist.Where(
                        u =>
                            u.YD_TeaPlan_Class.YD_Edu_Major.y_majorLibId == majorlibid &&
                            u.YD_TeaPlan_Class.YD_Edu_Major.y_eduTypeId == eduid &&
                            u.YD_TeaPlan_Class.YD_Edu_Major.y_stuTypeId == stuTypeid); //专业层次筛选


                var list = stu.GroupJoin(clist,
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

                var scorelist = yunEntities.YD_Graduate_SampleExamScore.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.stu.id },
                    score => new { y_course = score.y_courseId, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                int? nullint = null;
                var model =
                    lists.Select(
                        u => new GridSampleExam
                        {
                            StuId = u.s.stu.id,
                            StuName = u.s.stu.y_name,
                            CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course,
                            CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name,
                            ScoreId = u.score == null ? nullint : u.score.id,
                            TotalScore = u.score == null ? 0M : u.score.y_totalScore
                        }).ToList();

                ViewData["year"] = year;
                ViewData["school"] = schoolid;
                ViewData["edutype"] = eduid;
                ViewData["stutype"] = stuTypeid;
                ViewData["majorlib"] = majorlibid;
                return View(model);
            }
        }

        //抽考成绩录入
        [HttpPost]
        public void SaveGridSamPleExamScore(List<GridSampleExam> ScoreList)
        {
            var scorelist = ScoreList;
            using (var ad = new IYunEntities())
            {
                var updatelist = scorelist.Where(u => u.ScoreId != 0).ToList();
                var insertlist = scorelist.Where(u => u.ScoreId == 0).ToList();
                insertlist.ForEach(u =>
                {
                    var score = new YD_Graduate_SampleExamScore
                    {
                        y_courseId = (int)u.CourseId,
                        y_stuId = u.StuId,
                        y_totalScore = u.TotalScore,
                        id = 0
                    };
                    ad.YD_Graduate_SampleExamScore.Add(score);
                });

                updatelist.ForEach(u =>
                {
                    var score = new YD_Graduate_SampleExamScore
                    {
                        y_courseId = (int)u.CourseId,
                        y_stuId = u.StuId,
                        y_totalScore = u.TotalScore,
                        id = (int)u.ScoreId
                    };
                    ad.Entry(score).State = EntityState.Modified;
                });
                ad.SaveChanges();
            }
        }


        /// <summary>
        /// 学生学位成绩单-抽考，主干,学位外语
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassGridSampleStu()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/ClassGridSampleStu");
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

            var yearStr = Request.Form["EnrollYear2"];
            int year;
            if (string.IsNullOrWhiteSpace(yearStr))
            {
                year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            }
            else
            {
                year = Convert.ToInt32(yearStr);
            }

            var schoolStr = Request.Form["SubSchool"];
            int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);


            var majorLIStr = Request.Form["MajorLibrary"];
            int majorlibid = string.IsNullOrWhiteSpace(majorLIStr) ? 0 : Convert.ToInt32(majorLIStr);

            var eduStr = Request["EduType"];
            int eduid = string.IsNullOrWhiteSpace(eduStr) ? 0 : Convert.ToInt32(eduStr);
            var stuStr = Request["StuType"];
            int stuTypeid = string.IsNullOrWhiteSpace(stuStr) ? 0 : Convert.ToInt32(stuStr);

            using (var ad = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(ad, 53); //根据父栏目ID获取兄弟栏目
                }
                var model = new List<GridSampleScoreListDto>();

                if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    int[] stateList = { 1, 6, 7, 8, 9 };

                    var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                    var stu =
                        ad.YD_Sts_StuInfo.Where(
                            u =>
                                stateList.Contains(u.y_stuStateId) &&
                                u.y_subSchoolId == school && u.YD_Edu_Major.y_eduTypeId == eduid &&
                                u.YD_Edu_Major.y_stuTypeId == stuTypeid
                                && u.YD_Edu_Major.y_majorLibId == majorlibid && u.y_inYear == year && u.y_isgraduate == true)
                            .AsQueryable();

                    int? nullint = null;

                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {

                        var subSchoolIds =
                            ad.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId)
                                .ToList();
                        stu = stu.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                    }
                    //主干课程成绩
                    var list = stu.GroupJoin(classCourse.Where(u => u.y_isMain == true),
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
                    var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                    lists.ToList().ForEach(u =>
                    {
                        var grid = new GridSampleScoreListDto();
                        grid.StuId = u.s.stu.id;
                        grid.StuName = u.s.stu.y_name;
                        grid.CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course;
                        grid.CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name;
                        grid.Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team;
                        grid.ScoreId = u.score == null ? nullint : u.score.id;
                        grid.TotalScore = u.score == null ? 0M : u.score.y_totalScore;
                        grid.hasPermission = 1; //主干
                        model.Add(grid);
                    });

                    //抽考课程成绩
                    var list2 = stu.GroupJoin(classCourse.Where(u => u.y_sampleexam == true),
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
                    var scorelist2 = ad.YD_Graduate_SampleExamScore.OrderBy(u => u.id).AsQueryable();

                    var lists3 = list2.GroupJoin(scorelist2,
                        s => new { s.classCourse.y_course, s.stu.id },
                        score => new { y_course = score.y_courseId, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                    lists3.ToList().ForEach(u =>
                    {
                        var grid = new GridSampleScoreListDto();
                        grid.StuId = u.s.stu.id;
                        grid.StuName = u.s.stu.y_name;
                        grid.CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course;
                        grid.CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name;
                        grid.Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team;
                        grid.ScoreId = u.score == null ? nullint : u.score.id;
                        grid.TotalScore = u.score == null ? 0M : u.score.y_totalScore;
                        grid.hasPermission = 2; //抽考
                        model.Add(grid);
                    });
                    //学位英语
                    List<int> stuids = stu.Select(u => u.id).ToList();
                    var graduate = ad.YD_Graduate_StudentScore.Where(u => stuids.Contains((int)u.y_stuId)).ToList();

                    ViewBag.graduate = graduate;
                }

                ViewData["year"] = year;
                ViewData["school"] = school;
                ViewData["edutype"] = eduid;
                ViewData["stutype"] = stuTypeid;
                ViewData["majorlib"] = majorlibid;
                return View(model);
            }

        }


        ///学生学士成绩单导出
        public string ClassGridSampleStuDown()
        {
            var yearStr = Request.Form["EnrollYear2"];
            int year;
            if (string.IsNullOrWhiteSpace(yearStr))
            {
                year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            }
            else
            {
                year = Convert.ToInt32(yearStr);
            }

            var schoolStr = Request.Form["SubSchool"];
            int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);


            var majorLIStr = Request.Form["MajorLibrary"];
            int majorlibid = string.IsNullOrWhiteSpace(majorLIStr) ? 0 : Convert.ToInt32(majorLIStr);

            var eduStr = Request["EduType"];
            int eduid = string.IsNullOrWhiteSpace(eduStr) ? 0 : Convert.ToInt32(eduStr);
            var stuStr = Request["StuType"];
            int stuTypeid = string.IsNullOrWhiteSpace(stuStr) ? 0 : Convert.ToInt32(stuStr);

            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };
            var subschoolName = "";
            using (var ad = new IYunEntities())
            {
                #region

                var model = new List<GridSampleScoreListDto>();
                int[] stateList = { 1, 6, 7, 8, 9 };

                var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    ad.YD_Sts_StuInfo.Where(
                        u =>
                            stateList.Contains(u.y_stuStateId) &&
                            u.y_subSchoolId == school && u.YD_Edu_Major.y_eduTypeId == eduid &&
                            u.YD_Edu_Major.y_stuTypeId == stuTypeid
                            && u.YD_Edu_Major.y_majorLibId == majorlibid && u.y_inYear == year && u.y_isgraduate == true)
                        .AsQueryable();
                if (!stu.Any())
                {
                    return "未找到符合条件的学生";
                }
                int? nullint = null;

                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {

                    var subSchoolIds =
                        ad.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    stu = stu.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                //主干课程成绩
                var list = stu.GroupJoin(classCourse.Where(u => u.y_isMain == true),
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
                var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                lists.ToList().ForEach(u =>
                {
                    var grid = new GridSampleScoreListDto();
                    grid.StuId = u.s.stu.id;
                    grid.StuName = u.s.stu.y_name;
                    grid.CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course;
                    grid.CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name;
                    grid.Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team;
                    grid.ScoreId = u.score == null ? nullint : u.score.id;
                    grid.TotalScore = u.score == null ? 0M : u.score.y_totalScore;
                    grid.hasPermission = 1; //主干
                    model.Add(grid);
                });

                //抽考课程成绩
                var list2 = stu.GroupJoin(classCourse.Where(u => u.y_sampleexam == true),
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
                var scorelist2 = ad.YD_Graduate_SampleExamScore.OrderBy(u => u.id).AsQueryable();

                var lists3 = list2.GroupJoin(scorelist2,
                    s => new { s.classCourse.y_course, s.stu.id },
                    score => new { y_course = score.y_courseId, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });
                lists3.ToList().ForEach(u =>
                {
                    var grid = new GridSampleScoreListDto();
                    grid.StuId = u.s.stu.id;
                    grid.StuName = u.s.stu.y_name;
                    grid.CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course;
                    grid.CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name;
                    grid.Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team;
                    grid.ScoreId = u.score == null ? nullint : u.score.id;
                    grid.TotalScore = u.score == null ? 0M : u.score.y_totalScore;
                    grid.hasPermission = 2; //抽考
                    model.Add(grid);
                });
                //学位英语
                List<int> stuids = stu.Select(u => u.id).ToList();
                var graduate = ad.YD_Graduate_StudentScore.Where(u => stuids.Contains((int)u.y_stuId)).OrderByDescending(u => u.y_sumsore).ToList();
                #endregion

                if (model.Any())
                {
                    //主干课程
                    var termlist = model.Where(u => u.hasPermission == 1).GroupBy(u => new
                    {
                        u.CourseId,
                        u.CourseName
                    }).Select(u => u.Key).OrderBy(u => u.CourseId).ToList();

                    //抽考课程
                    var samplelist = model.Where(u => u.hasPermission == 2).GroupBy(u => new
                    {
                        u.CourseId,
                        u.CourseName
                    }).Select(u => u.Key).OrderBy(u => u.CourseId).ToList();

                    var stulist = model.OrderBy(u => u.StuId).GroupBy(u => new
                    {
                        u.StuId,
                        u.StuName
                    }).ToList();
                    int count = termlist.Count(); //主干数
                    int samcount = samplelist.Count(); //抽考数
                    if (school != 0)
                    {
                        var ydSysSubSchool = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school);
                        if (ydSysSubSchool != null)
                            subschoolName = ydSysSubSchool.y_name;
                    }

                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    //设置单元格内容
                    cell.SetCellValue(subschoolName + "学生学位成绩单");

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style;
                    //设置单元格的高度
                    row.Height = 30 * 20;
                    //设置单元格的宽度
                    sheet.SetColumnWidth(0, 10 * 256);
                    sheet.SetColumnWidth(1, 30 * 256);
                    sheet.SetColumnWidth(2, 30 * 256);
                    sheet.SetColumnWidth(3, 10 * 256);
                    sheet.SetColumnWidth(4, 10 * 256);
                    sheet.SetColumnWidth(5, 10 * 256);
                    sheet.SetColumnWidth(6, 60 * 256);

                    var styleCell = workbook.CreateCellStyle();
                    styleCell.Alignment = HorizontalAlignment.CENTER;
                    styleCell.VerticalAlignment = VerticalAlignment.CENTER;

                    var font2 = workbook.CreateFont();

                    styleCell.SetFont(font2);


                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                    var row1 = sheet.CreateRow(1);
                    var row2 = sheet.CreateRow(2);


                    row1.CreateCell(0).SetCellValue("姓名");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                    row1.CreateCell(1).SetCellValue("主干课程");
                    for (int i = 2; i <= count; i++)
                    {
                        row1.CreateCell(i);
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, count));
                    row1.CreateCell(count + 1).SetCellValue("抽考课程");
                    for (int i = count + 2; i <= count + samcount; i++)
                    {
                        row1.CreateCell(i);
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, count + 1, count + samcount));
                    row1.CreateCell(count + samcount + 1).SetCellValue("学位英语");
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, count + samcount + 1, count + samcount + 1));
                    //设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                    //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1)); //标题

                    //主干课程
                    for (int i = 0; i < termlist.Count(); i++)
                    {
                        row2.CreateCell(i + 1).SetCellValue(termlist[i].CourseName);
                    }
                    // 抽考课程
                    for (int i = 0; i < samplelist.Count(); i++)
                    {
                        row2.CreateCell(count + i + 1).SetCellValue(samplelist[i].CourseName);
                    }

                    for (int i = 0; i < stulist.Count(); i++)
                    {
                        var rowNumber = sheet.PhysicalNumberOfRows;
                        var rowCourse = sheet.CreateRow(rowNumber);

                        rowCourse.CreateCell(0).SetCellValue(stulist[i].Key.StuName); //姓名

                        //主干成绩
                        for (int z = 0; z < termlist.Count(); z++)
                        {
                            var score = stulist[i].OrderByDescending(u => u.TotalScore).First(u => u.CourseId == termlist[z].CourseId);
                            if (score.ScoreId != null)
                            {
                                rowCourse.CreateCell(z + 1).SetCellValue((double)score.TotalScore);
                            }
                            else
                            {
                                rowCourse.CreateCell(z + 1).SetCellValue((double)score.TotalScore);
                            }
                        }
                        // 抽考成绩
                        for (int s = 0; s < samplelist.Count(); s++)
                        {
                            var score = stulist[i].OrderByDescending(u => u.TotalScore).First(u => u.CourseId == samplelist[s].CourseId);
                            if (score.ScoreId != null)
                            {
                                rowCourse.CreateCell(count + s + 1).SetCellValue((double)score.TotalScore);
                            }
                            else
                            {
                                rowCourse.CreateCell(count + s + 1).SetCellValue(0);
                            }
                        }
                        //学位外语
                        var num = 0;
                        foreach (var grad in graduate)
                        {
                            var sco = stulist[i].FirstOrDefault(u => u.StuId == grad.y_stuId);
                            if (sco != null)
                            {
                                rowCourse.CreateCell(count + samcount + 1).SetCellValue((double)grad.y_sumsore);
                                num++;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (num == 0)
                        {
                            rowCourse.CreateCell(count + samcount + 1).SetCellValue(0);
                        }

                        rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式
                    }
                    row1.Cells.ForEach(u => u.CellStyle = styleCell);
                    row2.Cells.ForEach(u => u.CellStyle = styleCell);

                    var rowFoot = sheet.CreateRow(sheet.PhysicalNumberOfRows);
                    rowFoot.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式


                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/学位成绩单" + ".xls"; //todo:改变
                    var fileName3 = dirPath + filename1; //todo:改变
                                                         //将工作簿写入文件
                    using (FileStream fs = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        result.IsOk = true;
                        result.Message = url;
                        return JsonConvert.SerializeObject(result);
                    }
                }
                return JsonConvert.SerializeObject(result);
            }
        }


        //打印学士学位页面
        public ActionResult PrintGridDegree(int id = 1)
        {
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请且审核通过学位的毕业学生名单
                //IQueryable<VW_StuInfo> list =
                //    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                //        .Where(
                //            u =>
                //                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                //                u.y_isgraduate == true && u.y_bachelordegreeCheck == true);          
                
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(
                            u =>
                                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue 
                                );
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
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                    ViewBag.yearint = enrollYearint;
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
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("PrintGridDegreeList", model);
                return View(model);
            }
        }


        /// <summary>
        /// 打印学士学位打印
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult PrintGridDegreeShow()
        {
            using (var yunEntities = new IYunEntities())
            {
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请且审核通过学位的毕业学生名单
                //IQueryable<VW_StuInfo> list =
                //    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                //        .Where(
                //            u =>
                //                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                //                u.y_isgraduate == true && u.y_bachelordegreeCheck == true);    
            
            IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(
                            u =>
                                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue );
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
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


        //打印毕业证页面
        public ActionResult PrintGrid(int id = 1)
        {
            using (var yunEntities = new IYunEntities())
            {
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请且审核通过学位的毕业学生名单
                //IQueryable<VW_StuInfo> list =
                //    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                //        .Where(
                //            u =>
                //                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                //                u.y_isgraduate == true && u.y_bachelordegreeCheck == true);   
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(
                            u =>
                                //u.y_isdel == isnotdel && u.y_subSchoolId.HasValue 
                                //&& u.y_stuStateId == 6 &&
                                u.y_graduateNumber!=null
                                //&&u.y_isgraduate == true 
                                //&& u.y_bachelordegreeCheck == true
                                );
                var count = list.Count();
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
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
                    ViewBag.cardandname = cardandname;
                }
                if (!string.IsNullOrWhiteSpace(enrollYear) && !enrollYear.Equals("0"))
                {
                    var enrollYearint = Convert.ToInt32(enrollYear);
                    list = list.Where(u => u.y_inYear == enrollYearint);
                    ViewBag.yearint = enrollYearint;
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
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    var gradYear = Request["gradYear"];
                    if (gradYear != "null")
                    {
                        list = list.Where(e => e.y_graduateNumber.Contains("104145" + gradYear));
                    }
                }
                var model = list.ToPagedList(id, 50); //id为pageindex   15 为pagesize     
                if (Request.IsAjaxRequest())
                    return PartialView("PrintGridList2", model);
                return View(model);
            }
        }
        //打印毕业证打印
        public ActionResult PrintGridShow()
        {
            using (var yunEntities = new IYunEntities())
            {
                var enrollYear = Request["EnrollYear1"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var cardandname = Request["cardandname"];
                var stuids = Request["stuidl"];
                const int isnotdel = (int)YesOrNo.No;
                //得到已申请且审核通过学位的毕业学生名单
                //IQueryable<VW_StuInfo> list =
                //    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                //        .Where(
                //            u =>
                //                u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6 &&
                //                u.y_isgraduate == true && u.y_bachelordegreeCheck == true);   
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id)
                        .Where(
                            u =>
                                //u.y_isdel == isnotdel && u.y_subSchoolId.HasValue 
                                //&& u.y_stuStateId == 6 
                                //&&
                                u.y_graduateNumber != null
                                //&& u.y_bachelordegreeCheck == true
                                );
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                if (!string.IsNullOrWhiteSpace(cardandname))
                {
                    list = list.Where(u => u.y_name.Contains(cardandname) || u.y_cardId.Contains(cardandname));
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
                if (!string.IsNullOrWhiteSpace(stuids) && !stuids.Equals("0"))
                {
                    var stuIdArray = stuids.TrimEnd(',').Split(',').Select(e=>Convert.ToInt32(e));
                    list = list.Where(u => stuIdArray.Contains(u.id));

                }
                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    var gradYear = Request["gradYear1"];
                    if (gradYear != "null")
                    {
                        list = list.Where(e => e.y_graduateNumber.Contains("104145" + gradYear));
                    }
                }
                ViewData["list"] = list.ToList();
                return View();

            }
        }
        /// <summary>
        /// 毕业证号生成页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GraduateNumber(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/GraduateNumber");
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
                #region
                if (KeyValue.StuGradutateModel.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 59); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel2.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 55); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel3.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 80); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel4.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 81); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel5.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 92); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel6.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 57); //根据父栏目ID获取兄弟栏目
                }
                else if (KeyValue.StuGradutateModel7.Contains(ConfigurationManager.AppSettings["SchoolName"].ToString()))
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 77); //根据父栏目ID获取兄弟栏目
                }
                else
                {
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 53); //根据父栏目ID获取兄弟栏目
                }
                #endregion
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Sts_StuInfo> list =
                    yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                        .Include(u => u.YD_Sys_SubSchool)
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Edu_StuState)
                        .Where(u => u.y_isdel == isnotdel && u.y_subSchoolId.HasValue && u.y_stuStateId == 6
                        && (u.y_graduateNumber == null||u.y_graduateNumber==""));
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                if (string.IsNullOrWhiteSpace(enrollYear))
                {
                    list = list.Where(u => u.y_inYear == (xinshenyear - 2));
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum.Contains(namenumcard)
                     || u.y_cardId.Contains(namenumcard) || u.y_examNum.Contains(namenumcard));
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

                ViewBag.adminrole = YdAdminRoleId;
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("GraduateNumberList", model);
                return View(model);
            }
        }

        /// <summary>
        /// 选择所有需要生成毕业证号毕业生
        /// </summary>
        /// <returns></returns>
        public string AllGradNumCheck()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/GraduateNumber");
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
                var year = Request["inYear"];
                if (string.IsNullOrWhiteSpace(year))
                {
                    return "必须选择年份";
                }
                var subSchool = Request["SubSchool"];
                var edutype = Request["edutype"];
                var majorLibrary = Request["majorLibrary"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;
                var enrollYearint = Convert.ToInt32(year);
                //只获取没有生成毕业证号的毕业生
                IQueryable<YD_Sts_StuInfo> list = yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                    .Where(u =>
                        u.y_isdel == isnotdel && u.y_stuStateId == 6 && u.y_inYear == enrollYearint &&
                       u.y_graduateNumber == null && u.Y_generateWhether != true).AsQueryable();

                if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subSchoolint = Convert.ToInt32(subSchool);
                    list = list.Where(u => u.y_subSchoolId == subSchoolint);
                }
                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var edutypeint = Convert.ToInt32(edutype);
                    list = list.Where(u => u.YD_Edu_Major.YD_Edu_EduType.id == edutypeint);
                }
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Edu_Major.YD_Edu_MajorLibrary.id == majorLibraryint);
                }
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list =
                        list.Where(
                            u =>
                                u.y_stuNum == namenumcard || u.y_name.Contains(namenumcard) ||
                                u.y_cardId.Contains(namenumcard) || u.y_examNum == namenumcard);
                }
                if (list.ToList().Count > 0)
                {
                    list.ToList().ForEach(u =>
                    {
                        u.Y_generateWhether = true;
                        yunEntities.Entry(u).State = EntityState.Modified;
                    });
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
                else
                {
                    return "没有需要生成毕业证号的学生";
                }
            }
        }
        /// <summary>
        /// 取消所有需要生成毕业证号毕业生
        /// </summary>
        /// <returns></returns>
        public string AllGradNumCheckNo()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/GraduateNumber");
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
                var year = Request["inYear"];
                if (string.IsNullOrWhiteSpace(year))
                {
                    return "必须选择年份";
                }
                const int isnotdel = (int)YesOrNo.No;
                var enrollYearint = Convert.ToInt32(year);
                //只获取没有生成毕业证号的毕业生
                var list = yunEntities.YD_Sts_StuInfo.OrderByDescending(u => u.id)
                    .Where(u =>
                        u.y_isdel == isnotdel && u.y_stuStateId == 6 && u.y_inYear == enrollYearint &&
                       u.y_graduateNumber == null && u.Y_generateWhether == true).ToList();
                if (list.Any())
                {
                    list.ToList().ForEach(u =>
                    {
                        u.Y_generateWhether = false;
                        yunEntities.Entry(u).State = EntityState.Modified;
                    });
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
                else
                {
                    return "没有需要生成毕业证号的学生";
                }
            }
        }
        /// <summary>
        /// 选择性勾选学生是否生成毕业证号情况
        /// </summary>
        /// <returns></returns>
        public string GradNumCheckOk()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/GraduateNumber");
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.Y_generateWhether = true;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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
        /// 选择性取消学生是否生成毕业证号情况
        /// </summary>
        /// <returns></returns>
        public string GradNumCheckOkNo()
        {
            #region 权限验证
            var power = SafePowerPage("/Graduate/GraduateNumber");
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return "未知错误";
            }
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (var yunEntities = new IYunEntities())
            {
                for (int i = 0; i < ids.Count(); i++)
                {
                    var oid = Convert.ToInt32(ids[i]);
                    var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == oid);
                    if (stu != null)
                    {
                        stu.Y_generateWhether = false;
                        yunEntities.Entry(stu).State = EntityState.Modified;
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

        //毕业证号编排
        public string GraduateNumberSetting()
        {
            var inyear = Request["inYear"];
            if (inyear != null && (string.IsNullOrWhiteSpace(inyear) && inyear.Equals("0")))
            {
                return "请选择年份";
            }
            //int[] stuids = new int[] { 1156, 1263, 1317, 1384 };
            using (var ad = new IYunEntities())
            {
                var inyearint = Convert.ToInt32(inyear);
                //得到该年份未生成毕业证号毕业学生名单
                var list =
                    ad.YD_Sts_StuInfo.Where(
                        u => u.y_isdel == 1 && u.y_stuStateId == 6 && u.y_inYear == inyearint
                        && (u.y_graduateNumber == null|| u.y_graduateNumber=="") && u.Y_generateWhether == true).ToList();
                if (list.Any())
                {
                    var stuidlist = string.Join(",", list.Select(u => u.id).ToArray()); //所有stuid根据逗号相加成一个字符串
                    //将string数组转换成int数组
                    int[] stuids =
                        Array.ConvertAll(stuidlist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                    var zk = ad.YD_Edu_EduType.First(u => u.y_name == "高起专").id;


                    var stulist = ad.YD_Sts_StuInfo.Where(u => stuids.Contains(u.id)).ToList();

                    var stugroup = stulist.GroupBy(u => u.YD_Edu_Major.y_eduTypeId).ToList();
                    var xin = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"].ToString());

                    //var xinshen = (xin + 1).ToString();

                    //临时用的
                    //var xinshen = (xin).ToString();
                    var xinshen = "2021";
                    var a =
                        ad.YD_Sts_StuInfo.OrderByDescending(u => u.y_graduateNumber)
                            .FirstOrDefault(
                                u => u.y_graduateNumber.Length == 18 && u.y_graduateNumber.Contains("104145" + xinshen + "05"));
                    var b =
                        ad.YD_Sts_StuInfo.OrderByDescending(u => u.y_graduateNumber)
                            .FirstOrDefault(
                                u => u.y_graduateNumber.Length == 18 && u.y_graduateNumber.Contains("104145" + xinshen + "06"));
                    var max = a == null ? 1 : Convert.ToInt32(a.y_graduateNumber.Substring(12, 6)) + 1;
                    var max2 = b == null ? 1 : Convert.ToInt32(b.y_graduateNumber.Substring(12, 6)) + 1;

                    foreach (var item in stugroup)
                    {
                        if (item.Key == zk)
                        {
                            foreach (var stu in item.ToList())
                            {
                                if (stu.y_graduationdata==null)
                                {
                                    stu.y_graduateNumber = "104145" + xinshen + "06" + DDD(max2, 6);
                                    ad.Entry(stu).State = EntityState.Modified;
                                    max2++;
                                }
                                stu.y_graduateNumber = "104145" + xinshen + "06" + DDD(max2, 6);
                                ad.Entry(stu).State = EntityState.Modified;
                                max2++;
                            }
                        }
                        else
                        {
                            foreach (var stu in item.ToList())
                            {
                                stu.y_graduateNumber = "104145" + xinshen + "05" + DDD(max, 6);
                                ad.Entry(stu).State = EntityState.Modified;
                                max++;
                            }
                        }
                    }
                    ad.SaveChanges();
                    return "ok";
                }
                else
                {
                    return "没有学生需要生成毕业编号";
                }

            }
        }

        //撤销毕业证号
        public string CancelGraduateNumberSetting()
        {
            var inyear = Request["inYear"];
            if (inyear != null && (string.IsNullOrWhiteSpace(inyear) && inyear.Equals("0")))
            {
                return "请选择年份";
            }
            //int[] stuids = new int[] { 1156, 1263, 1317, 1384 };
            using (var ad = new IYunEntities())
            {
                var inyearint = Convert.ToInt32(inyear);
                //得到该年份未生成毕业证号毕业学生名单
                var list =
                    ad.YD_Sts_StuInfo.Where(
                        u => u.y_isdel == 1 && u.y_stuStateId == 6 && u.y_inYear == inyearint
                        && u.y_graduateNumber != null && u.Y_generateWhether == true).ToList();
                if (list.Any())
                {
                    var stuidlist = string.Join(",", list.Select(u => u.id).ToArray()); //所有stuid根据逗号相加成一个字符串
                    //将string数组转换成int数组
                    int[] stuids =
                        Array.ConvertAll(stuidlist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                    var stulist = ad.YD_Sts_StuInfo.Where(u => stuids.Contains(u.id)).ToList();

                    var stugroup = stulist.GroupBy(u => u.YD_Edu_Major.y_eduTypeId).ToList();

                    foreach (var item in stugroup)
                    {
                        foreach (var stu in item.ToList())
                        {
                            stu.y_graduateNumber = null;
                            ad.Entry(stu).State = EntityState.Modified;
                        }
                    }
                    ad.SaveChanges();
                    return "ok";
                }
                else
                {
                    return "没有学生需要生成毕业编号";
                }

            }
        }


        /// 补全数字
        public string DDD(int index, int length)
        {
            string indexs = index.ToString();

            var lengths = length - indexs.Length;

            for (int i = 0; i < lengths; i++)
            {
                indexs = "0" + indexs;
            }
            return indexs;
        }
    }
}