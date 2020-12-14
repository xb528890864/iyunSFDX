using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Models;
using IYun.Object;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    /// <summary>
    /// 统计报表
    /// </summary>
    public class ReportController : AdminBaseController
    {
        //
        // GET: /Report/

        #region 学生信息统计表

        public ActionResult StuInfoReport()
        {
            #region 权限验证

            var power = SafePowerPage("/report/stuinforeport");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            var major = Request["majorLibraryName"];
            var eduType = Request["eduTypeName"];
            var stuType = Request["stuTypeName"];
            var stuState = Request["stuStateName"];
            var subSchool = Request["schoolName"];
            var inyear = Request["y_inyear"];
            var majorFlag = false;
            var eduTypeFlag = false;
            var stuTypeFlag = false;
            var stuStateFlag = false;
            var subSchoolFlag = false;
            var inyearFlag = false;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 36); //根据父栏目ID获取兄弟栏目
                if (string.IsNullOrWhiteSpace(major) && string.IsNullOrWhiteSpace(eduType) &&
                    string.IsNullOrWhiteSpace(stuType)
                    && string.IsNullOrWhiteSpace(stuState) && string.IsNullOrWhiteSpace(subSchool) && string.IsNullOrWhiteSpace(inyear))
                {
                    ViewBag.hasStu = false;
                }
                else
                {
                    var str = "";
                    if (!string.IsNullOrWhiteSpace(subSchool))
                    {
                        str += subSchool + ",";
                        subSchoolFlag = true;
                    }
                    if (!string.IsNullOrWhiteSpace(inyear))
                    {
                        str += inyear + ",";
                        inyearFlag = true;
                    }
                    if (!string.IsNullOrWhiteSpace(major))
                    {
                        str += major + ",";
                        majorFlag = true;
                    }
                    if (!string.IsNullOrWhiteSpace(eduType))
                    {
                        str += eduType + ",";
                        eduTypeFlag = true;
                    }
                    if (!string.IsNullOrWhiteSpace(stuType))
                    {
                        str += stuType + ",";
                        stuTypeFlag = true;
                    }
                    if (!string.IsNullOrWhiteSpace(stuState))
                    {
                        str += stuState + ",";
                        stuStateFlag = true;
                    }
                    if (str.EndsWith(","))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    var sql =
                        "SELECT  COUNT(*) AS totalcount ," + str + " FROM dbo.VW_StuInfo GROUP BY " + str;
                    if (stuState != null)
                    {
                        sql = "SELECT  COUNT(*) AS totalcount ," + str + " FROM dbo.VW_StuInfo GROUP BY " + str + " order by stuStateName ";
                    }
                    ViewBag.hasStu = true;
                    ViewBag.stuList = yunEntities.Database.SqlQuery<StuInfoReport>(sql).ToList();
                }
                ViewBag.majorFlag = majorFlag;
                ViewBag.eduTypeFlag = eduTypeFlag;
                ViewBag.stuTypeFlag = stuTypeFlag;
                ViewBag.stuStateFlag = stuStateFlag;
                ViewBag.subSchoolFlag = subSchoolFlag;
                ViewBag.inyearFlag = inyearFlag;

            }
            return View();
        }

        /// <summary>
        /// 函授站学生统计
        /// </summary>
        /// <returns></returns>
        public ActionResult SubStuReport()
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
            ViewBag.YdAdminRoleId = YdAdminRoleId;
            var subSchool = Request["SubSchool"];
            var inYear = Request["inYear"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 36); //根据父栏目ID获取兄弟栏目
                var sql =
                    " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo GROUP BY schoolName,y_inyear  ORDER BY schoolName";

                var substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).OrderByDescending(x=>x.y_inyear).ToList();
                if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0") && !string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subschoolID = Convert.ToInt32(subSchool);
                    var year = Convert.ToInt32(inYear);
                    sql = " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo" +
                      " where y_subSchoolId='" +
                      subSchool + "' and y_inyear='" + inYear + "' GROUP BY schoolName,y_inyear  ORDER BY schoolName";
                    ViewBag.subschool = subschoolID;
                    ViewBag.year = year;
                    substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    var subschoolID = Convert.ToInt32(subSchool);
                    sql = " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo" +
                        " where y_subSchoolId='" +
                        subSchool + "' GROUP BY schoolName,y_inyear ORDER BY schoolName";
                    ViewBag.subschool = subschoolID;
                    substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    var year = Convert.ToInt32(inYear);
                    sql = " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo" +
                       " where y_inyear='" +
                       inYear + "' GROUP BY schoolName,y_inyear ORDER BY schoolName";
                    ViewBag.year = year;
                    substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).ToList();
                }
                if(ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString()&&YdAdminRoleId==4)
                {
                    var subid = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(x => x.y_adminId == YdAdminId).y_subSchoolId;
                    var nametrue = yunEntities.YD_Sys_SubSchool.FirstOrDefault(x => x.id == subid).y_name;
                    ViewBag.substulist = substulist.Where(x=>x.schoolName == nametrue).ToList();
                }
                else if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.ZYYDX.ToString() && YdAdminRoleId == 4)
                {
                    var subid = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(x => x.y_adminId == YdAdminId).y_subSchoolId;
                    var nametrue = yunEntities.YD_Sys_SubSchool.FirstOrDefault(x => x.id==subid).y_name;
                    ViewBag.substulist = substulist.Where(x => x.schoolName == nametrue).ToList();
                }
                else if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.DHLGDX.ToString() && YdAdminRoleId == 4)
                {
                    var subid = yunEntities.YD_Sys_AdminSubLink.FirstOrDefault(x => x.y_adminId == YdAdminId).y_subSchoolId;
                    var nametrue = yunEntities.YD_Sys_SubSchool.FirstOrDefault(x => x.id == subid).y_name;
                    ViewBag.substulist = substulist.Where(x => x.schoolName == nametrue).ToList();
                }
                else
                {
                    ViewBag.substulist = substulist;
                }

                return View();
            }
        }

        /// <summary>
        /// 全校各层次人数统计
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeNewStuStatist()
        {
            #region 权限验证
            var power = SafePowerPage("/report/EduTypeNewStuStatist");
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
                ViewBag.modulePowers = GetChildModulePower(ad, 36); //根据父栏目ID获取兄弟栏目
                var sql =
                    "select y_inyear, COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 1 then '高起专(普通/3)'  end) as specomthree," +
                    " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 2 then '高起专(艺术/3)'  end) as speartthree," +
                    " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 4 and majoredu.isys = 1 then '高起专(普通/4)'  end) as specomfour," +
                    " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 1 then '专升本(普通/3)'  end) as speupgracomthree," +
                    " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 2 then '专升本(艺术/3)'  end) as speupgraartthree," +
                    " COUNT(case when  eduTypeName = '高起本' and stu.y_stuYear = 5 and majoredu.isys = 1 then '高起本(普通/5)'  end) as thiscomfive" +
                    " , count(*) as sumcount " +
                    " from VW_StuInfo  as stu  " +
                    " left join(select y_eduTypeId, y_stuYear, isys, id from" +
                    " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                    " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    " where stu.y_isdel=1 and  stu.stuStateName!='退学' " +
                    " group by y_inYear order by y_inyear ";
                var list = ad.Database.SqlQuery<EduTypeStuSum>(sql).ToList();
                return View(list);
            }
        }
        /// <summary>
        /// 全校各层次人数统计下载
        /// </summary>
        /// <returns></returns>
        public string DowEduTypeNewStuStatist()
        {
            using (var ad = new IYunEntities())
            {
                var sql =
                   "select y_inyear, COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 1 then '高起专(普通/3)'  end) as specomthree," +
                   " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 3 and majoredu.isys = 2 then '高起专(艺术/3)'  end) as speartthree," +
                   " COUNT(case when  eduTypeName = '高起专' and stu.y_stuYear = 4 and majoredu.isys = 1 then '高起专(普通/4)'  end) as specomfour," +
                   " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 1 then '专升本(普通/3)'  end) as speupgracomthree," +
                   " COUNT(case when  eduTypeName = '专升本' and stu.y_stuYear = 3 and majoredu.isys = 2 then '专升本(艺术/3)'  end) as speupgraartthree," +
                   " COUNT(case when  eduTypeName = '高起本' and stu.y_stuYear = 5 and majoredu.isys = 1 then '高起本(普通/5)'  end) as thiscomfive" +
                   " , count(*) as sumcount " +
                   " from VW_StuInfo  as stu  " +
                   " left join(select y_eduTypeId, y_stuYear, isys, id from" +
                   " (select *,case  when y_majortype like '%艺术%' then 2 else 1  end as isys from YD_Edu_Major  )  as a" +
                   " group by y_eduTypeId,y_stuYear,isys,id)as majoredu on stu.y_majorId = majoredu.id" +
                    " where stu.y_isdel=1 and  stu.stuStateName!='退学' " +
                   " group by y_inYear order by y_inyear ";
                var list = ad.Database.SqlQuery<EduTypeStuSum>(sql).ToList();

                var lists = list.Select(u => new { u.y_inyear, u.specomthree, u.speartthree, u.specomfour, u.speupgracomthree, u.speupgraartthree, u.thiscomfive, u.sumcount });

                var model =
                    FileHelper.ToDataTable(
                        lists.ToList());
                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/全校各层次人数统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_inyear", "年级"},
                        {"specomthree", "高起专(普通/3)"},
                        {"speartthree","高起专(艺术/3)"},
                        {"specomfour", "高起专(普通/4)"},
                        {"speupgracomthree", "专升本(普通/3)"},
                        {"speupgraartthree", "专升本(艺术/3)"},
                        {"thiscomfive","高起本(普通/5)" },
                        {"sumcount","总人数" }
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
        /// 查询指定函授站学生信息统计
        /// </summary>
        /// <returns></returns>
        //public ActionResult SubStuReportShow()
        //{
        //    #region 权限验证

        //    //var power = SafePowerPage("/report/SubStuReport");
        //    //if (!IsLogin())
        //    //{
        //    //    return "/AdminBase/Index";
        //    //}
        //    //if (power == null || power.y_menu == (int)PowerState.Disable)
        //    //{
        //    //    return "没有权限";
        //    //}

        //    #endregion
        //    var subSchool = Request["subschool"];
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var sql =
        //           " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName, count (case when y_stuStateId = 1 then '在读' end) zaidu,count (case when y_stuStateId = 2 then '休学' end) xiuxue, count(case when y_stuStateId = 4 then '退学' end) tuixue, count(case when y_stuStateId = 7 then '未注册' end) weizhuce, count(case when y_stuStateId = 8 then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo GROUP BY schoolName ORDER BY schoolName";
        //        IQueryable<SubStuReport> substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).AsQueryable();
        //        if (!string.IsNullOrWhiteSpace(subSchool))
        //        {
        //            sql =
        //                " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName, count (case when y_stuStateId = 1 then '在读' end) zaidu,count (case when y_stuStateId = 2 then '休学' end) xiuxue, count(case when y_stuStateId = 4 then '退学' end) tuixue, count(case when y_stuStateId = 7 then '未注册' end) weizhuce, count(case when y_stuStateId = 8 then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo where schoolName='" +
        //                subSchool + "' GROUP BY schoolName ORDER BY schoolName";
        //            substulist = yunEntities.Database.SqlQuery<SubStuReport>(sql).AsQueryable();
        //        }
        //        int count = substulist.Count();
        //        if (substulist.Any())
        //        {
        //            var list = substulist.ToList();
        //            return PartialView("SubStuReportTable",list);
        //        }
        //        else
        //        {
        //            return Content("失败");
        //        }
        //    }
        //}
        #endregion


        /// <summary>
        /// 导出学生统计表
        /// </summary>
        /// <param name = "id" ></ param >
        /// < returns ></ returns >
        public ActionResult DownloadStuReport()
        {
            #region 权限验证

            var power = SafePowerPage("/report/stuinforeport");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }

            #endregion

            var major = Request["majorLibraryName"];
            var eduType = Request["eduTypeName"];
            var stuType = Request["stuTypeName"];
            var stuState = Request["stuStateName"];
            var subSchool = Request["schoolName"];
            var inyear = Request["y_inyear"];
            using (var yunEntities = new IYunEntities())
            {
                if (string.IsNullOrWhiteSpace(major) && string.IsNullOrWhiteSpace(eduType)
                    && string.IsNullOrWhiteSpace(stuType) && string.IsNullOrWhiteSpace(stuState) &&
                    string.IsNullOrWhiteSpace(subSchool) && string.IsNullOrWhiteSpace(inyear))
                {
                    ViewBag.hasStu = false;
                }
                else
                {
                    var str = "";
                    if (!string.IsNullOrWhiteSpace(major))
                    {
                        str += major + ",";
                    }
                    if (!string.IsNullOrWhiteSpace(eduType))
                    {
                        str += eduType + ",";
                    }
                    if (!string.IsNullOrWhiteSpace(stuType))
                    {
                        str += stuType + ",";
                    }

                    if (!string.IsNullOrWhiteSpace(stuState))
                    {
                        str += stuState + ",";
                    }
                    if (!string.IsNullOrWhiteSpace(subSchool))
                    {
                        str += subSchool + ",";
                    }
                    if (!string.IsNullOrWhiteSpace(inyear))
                    {
                        str += inyear;
                    }
                    if (str.EndsWith(","))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    var sql = "SELECT  COUNT(*) AS totalcount ," + str + " FROM dbo.VW_StuInfo GROUP BY " + str;
                    if (stuState != null)
                    {
                        sql = "SELECT  COUNT(*) AS totalcount ," + str + " FROM dbo.VW_StuInfo GROUP BY " + str + " order by stuStateName ";
                    }
                    ViewBag.hasStu = true;
                    var list = yunEntities.Database.SqlQuery<StuInfoReport>(sql);
                    var model =
                        FileHelper.ToDataTable(
                            list.Select(
                                u =>
                                    new
                                    {
                                        u.totalcount,
                                        u.majorLibraryName,
                                        u.eduTypeName,
                                        u.stuTypeName,
                                        u.stuStateName,
                                        u.schoolName,
                                        u.y_inyear
                                    }).ToList());

                    var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                    if (!Directory.Exists(dirPath))                       //todo:改变
                    {
                        Directory.CreateDirectory(dirPath);               //todo:改变
                    }
                    var filename1 = "/学生信息统计表" + ".xls";      //todo:改变
                    var fileName3 = dirPath + filename1;                               //todo:改变

                    //var filename1 = "File/Dowon/学生信息统计表" + Guid.NewGuid() + ".xls";
                    //var fileName2 = "~/" + filename1;
                    //var fileName3 = Server.MapPath(fileName2);
                    using (var excelHelper = new ExcelHelper(fileName3))
                    {
                        var ht = new Hashtable
                        {
                            {"totalcount", "人数"},
                            {"majorLibraryName", "专业名称"},
                            {"eduTypeName", "层次"},
                            {"stuTypeName", "学习形式"},
                            {"stuStateName", "学籍状态"},
                            {"schoolName", "函授站"},
                            {"y_inyear","入学年份"}
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
        }


        /// <summary>
        /// 导出函授站学生统计表
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadSubStuReport()
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
                var sql =
                    " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo GROUP BY schoolName,y_inyear  ORDER BY schoolName";

                if (!string.IsNullOrWhiteSpace(inYear) && !string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    sql =
                    " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo " +
                    " where y_subSchoolId=" +
                      subSchool + " and y_inyear='" + inYear + "' GROUP BY schoolName,y_inyear  ORDER BY schoolName";
                }
                else if (!string.IsNullOrWhiteSpace(subSchool) && !subSchool.Equals("0"))
                {
                    sql =
                    " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                    " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                    "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo " +
                        " where y_subSchoolId=" +
                        subSchool + "  GROUP BY schoolName,y_inyear ORDER BY schoolName";
                }
                else if (!string.IsNullOrWhiteSpace(inYear) && !inYear.Equals("0"))
                {
                    sql =
                   " SELECT case  when schoolName is null  then '未注册函授站' else schoolName end as schoolName,y_inyear, count (case when stuStateName ='在读' then '在读' end) zaidu," +
                   " count (case when stuStateName ='休学' then '休学' end) xiuxue, count(case when stuStateName ='退学' then '退学' end) tuixue, count(case when stuStateName ='未注册' then '未注册' end)" +
                   "  weizhuce, count(case when stuStateName ='注册待审核' then '注册待审核' end) zhuedaishenhe  FROM VW_StuInfo GROUP BY schoolName,y_inyear ORDER BY schoolName";
                }
                ViewBag.hasStu = true;
                var list = yunEntities.Database.SqlQuery<SubStuReport>(sql);
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    u.schoolName,
                                    u.y_inyear,
                                    u.zaidu,
                                    u.tuixue,
                                    u.weizhuce,
                                    u.zhuedaishenhe,
                                    u.xiuxue
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/函授站学生信息统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变


                //var filename1 = "File/Dowon/函授站学生信息统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"y_inyear","入学年份"},
                        {"zaidu", "在读"},
                        {"tuixue", "退学"},
                        {"weizhuce", "未注册"},
                        {"zhuedaishenhe", "注册待审核"},
                        {"xiuxue", "休学"}
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
        /// 年龄统计表
        /// </summary>
        /// <returns></returns>
        public ActionResult AgeReport()
        {
            #region 权限验证
            var power = SafePowerPage("/report/AgeReport");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 36); //根据父栏目ID获取兄弟栏目

                ViewBag.stuInfoAges = yunEntities.Database.SqlQuery<StuInfoAge>(
                     "SELECT COUNT(*)AS totolcount,ageform.y_eduTypeId,ageform.age,ageform.y_sex FROM (SELECT DATEDIFF(YEAR,dbo.VW_StuInfo.y_birthday,GETDATE())AS age,* FROM dbo.VW_StuInfo where y_stuStateId=1 and y_isdel=1) AS ageform GROUP BY ageform.y_eduTypeId,ageform.age,ageform.y_sex").ToList();
            }
            return View();
        }
        /// <summary>
        /// 学籍异动统计表
        /// </summary>
        /// <returns></returns>
        public ActionResult StrangeReport(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/report/StrangeReport");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 36); //根据父栏目ID获取兄弟栏目
                var year = Request["year"];
                var subSchool = Request["SubSchool"];
                IQueryable<VW_StrangeReportTotal> list = yunEntities.VW_StrangeReportTotal.OrderByDescending(u => u.y_subSchoolId).Where(u => true);
                //根据要查询的缴费年份找出相应要缴费的几届学生
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
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds =
                        yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                            .Select(u => u.y_subSchoolId)
                            .ToList();
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("StrangeTotalList", model);
                return View(model);
            }
        }
        /// <summary>
        ///函授站异动统计下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadStrangeTotal()
        {
            #region 权限验证

            var power = SafePowerPage("/report/StrangeReport");
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
                IQueryable<VW_StrangeReportTotal> list =
                    yunEntities.VW_StrangeReportTotal.OrderByDescending(u => u.y_subSchoolId).Where(u => true);
                //根据要查询的缴费年份找出相应要缴费的几届学生
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
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    schoolName = u.schoolName,
                                    strangeTypeName = u.strangeTypeName,
                                    totalStuCount = u.totalStuCount,
                                    y_inYear = u.y_inYear,
                                    approvalstatus = u.approvalstatus
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/函授站异动统计表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/函授站异动统计表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"strangeTypeName", "异动类型"},
                        {"totalStuCount", "异动人数"},
                        {"y_inYear", "入学年份"},
                        {"approvalstatus", "审核情况"}
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

        public ActionResult SupTaxReport()
        {
            var subSchool = Request["SubSchool"];
            var inYear = Request["inYear"];
            using (IYunEntities ctx = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ctx, 36); //根据父栏目ID获取兄弟栏目
                ViewBag.adminrole = YdAdminRoleId;
                string subSchoolSql = "y_subSchoolId is not null";
                string inYearSql = "y_inYear is not null";
                if (!string.IsNullOrEmpty(subSchool) && subSchool != "0")
                {
                    ViewBag.subschool = Convert.ToInt32(subSchool);
                    subSchoolSql = $"y_subSchoolId = '{subSchool}'";
                }
                if (YdAdminRoleId == 4)
                {
                    var subSchoolIds = ctx.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    subSchoolSql = $"y_subSchoolId = '{subSchoolIds.FirstOrDefault()}'";
                    ViewBag.subschoolid = subSchoolIds.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(inYear) && inYear != "0")
                {
                    ViewBag.year = Convert.ToInt32(inYear);
                    inYearSql = $"y_inYear = '{inYear}'";
                }
                string fullSql = $"SELECT schoolName,y_inYear,count(case when y_IsWorking is null then 1 end) as notSuped,count(case when y_IsWorking is not null then 1 end) as isSuped FROM VW_StuInfo WHERE ({subSchoolSql}) AND ({inYearSql}) AND (y_stuStateId != 6) AND y_isdel = 1 GROUP BY schoolName,y_inyear order by schoolName;";
                var Model = ctx.Database.SqlQuery<SubStuSupStuReport>(fullSql).ToList();
                ViewBag.substulist = Model;
                return View(Model);
            }
        }

        public ActionResult DownloadSupTax()
        {
            var subSchool = Request["SubSchool"];
            var inYear = Request["inYear"];
            using (IYunEntities ctx = new IYunEntities())
            {
                string subSchoolSql = "y_subSchoolId is not null";
                string inYearSql = "y_inYear is not null";
                if (!string.IsNullOrEmpty(subSchool) && subSchool != "0")
                {
                    ViewBag.subschool = Convert.ToInt32(subSchool);
                    subSchoolSql = $"y_subSchoolId = '{subSchool}'";
                }
                if (YdAdminRoleId == 4)
                {
                    var subSchoolIds = ctx.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    subSchoolSql = $"y_subSchoolId = '{subSchoolIds.FirstOrDefault()}'";
                    ViewBag.subschoolid = subSchoolIds.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(inYear) && inYear != "0")
                {
                    ViewBag.year = Convert.ToInt32(inYear);
                    inYearSql = $"y_inYear = '{inYear}'";
                }
                string fullSql = $"SELECT schoolName,y_inYear,count(case when y_IsWorking is null then 1 end) as notSuped,count(case when y_IsWorking is not null then 1 end) as isSuped FROM VW_StuInfo WHERE ({subSchoolSql}) AND ({inYearSql}) AND y_isdel = 1 AND (y_stuStateId != 6) GROUP BY schoolName,y_inyear order by schoolName;";
                var Model = ctx.Database.SqlQuery<SubStuSupStuReport>(fullSql).ToList();
                var model = FileHelper.ToDataTable(Model.Select(e=>new {
                    e.schoolName,
                    e.isSuped,
                    e.notSuped,
                    e.y_inyear
                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/补录个税专项信息统计" + ".xls";
                var fileName3 = dirPath + filename1;
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"schoolName", "函授站"},
                        {"isSuped", "已补录人数"},
                        {"notSuped", "未补录人数"},
                        {"y_inyear", "入学年份"}
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

        public ActionResult DownloadSupTaxSummary()
        {
            var subSchool = Request["SubSchool"];
            var inYear = Request["inYear"];
            using (IYunEntities ctx = new IYunEntities())
            {
                string subSchoolSql = "y_subSchoolId is not null";
                string inYearSql = "y_inYear is not null";
                if (!string.IsNullOrEmpty(subSchool) && subSchool != "0")
                {
                    ViewBag.subschool = Convert.ToInt32(subSchool);
                    subSchoolSql = $"y_subSchoolId = '{subSchool}'";
                }
                if (YdAdminRoleId == 4)
                {
                    var subSchoolIds = ctx.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    subSchoolSql = $"y_subSchoolId = '{subSchoolIds.FirstOrDefault()}'";
                    ViewBag.subschoolid = subSchoolIds.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(inYear) && inYear != "0")
                {
                    ViewBag.year = Convert.ToInt32(inYear);
                    inYearSql = $"y_inYear = '{inYear}'";
                }
                string fullSql = $"SELECT * FROM VW_StuInfo where y_IsWorking is not null AND ({inYearSql}) AND ({subSchoolSql}) AND (y_stuStateId != 6) AND y_isdel = 1;";
                var Model = ctx.Database.SqlQuery<VW_StuInfo>(fullSql).ToList();
                var model = FileHelper.ToDataTable(Model.Select(e => new
                {
                    e.y_stuNum,
                    e.y_name,
                    e.y_cardType,
                    e.y_cardId,
                    y_IsWorking = e.y_IsWorking == 1 ?"在职":"非在职",
                    e.y_inYear,
                    stuStateName = ConvertStateName(e.stuStateName),
                    e.y_parentName1,
                    e.y_parentCard1,
                    e.y_parentCardType1,
                    e.y_parentName2,
                    e.y_parentCard2,
                    e.y_parentCardType2,
                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/补录个税专项信息汇总" + ".xls";
                var fileName3 = dirPath + filename1;
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuNum", "学号"},
                        {"y_name", "学生姓名"},
                        {"y_cardType", "学生身份证件类型"},
                        {"y_cardId", "学生身份证件号码"},
                        {"y_IsWorking", "学生是否在职"},
                        {"y_inYear", "入学年份"},
                        {"stuStateName", "学籍状态"},
                        {"y_parentName1", "父母或监护人1姓名"},
                        {"y_parentCard1", "父母或监护人1身份证件类型"},
                        {"y_parentCardType1", "父母或监护人1身份证件号码"},
                        {"y_parentName2", "父母或监护人2姓名"},
                        {"y_parentCard2", "父母或监护人2身份证件类型"},
                        {"y_parentCardType2", "父母或监护人2身份证件号码"}
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

        private string ConvertStateName(string stustateName)
        {
            switch (stustateName)
            {
                case "在读":
                    return "注册学籍";
                case "休学":
                    return "休学";
                case "未注册":
                    return "暂缓注册";
                case "注册待审核":
                    return "暂缓注册";
                default:
                    return stustateName;
            }
        }

        public ActionResult SupTaxStuDetail(string year,string subschool,int isSuped)
        {
            using (IYunEntities ctx = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ctx, 36); //根据父栏目ID获取兄弟栏目
                ViewBag.adminrole = YdAdminRoleId;
                string supedSql = isSuped == 1 ? "y_IsWorking is not null" : "y_IsWorking is null";
                string fullSql = $"SELECT * FROM [dbo].[VW_StuInfo] where (schoolName = '{subschool}') and (y_inYear = '{year}') and (y_stuStateId != 6) and (y_isdel = 1) and ({supedSql});";
                var Model = ctx.Database.SqlQuery<VW_StuInfo>(fullSql).ToList();
                ViewBag.substulist = Model;
                ViewBag.isSuped = isSuped;
                ViewBag.year = year;
                ViewBag.subschool = subschool;
                return View(Model);
            }
        }

        public ActionResult DownloadSupTaxDetail(string year, string subschool, int isSuped)
        {
            using (IYunEntities ctx = new IYunEntities())
            {
                string supedSql = isSuped == 1 ? "y_IsWorking is not null" : "y_IsWorking is null";
                string fullSql = $"SELECT * FROM [dbo].[VW_StuInfo] where (schoolName = '{subschool}') and (y_inYear = '{year}') and (y_stuStateId != 6) and (y_isdel = 1) and ({supedSql});";
                var Model = ctx.Database.SqlQuery<VW_StuInfo>(fullSql).ToList();
                var model = FileHelper.ToDataTable(Model.Select(e => new {
                    e.y_stuNum,
                    e.y_name,
                    e.y_inYear,
                    e.schoolName,
                    e.y_cardId
                }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/补录个税专项信息详情" + ".xls";
                var fileName3 = dirPath + filename1;
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuNum", "学号"},
                        {"y_name", "姓名"},
                        {"y_inYear", "入学年份"},
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

                    return Content("alert('错误');window.location.href='" + reurl + "';");
                }
            }
        }

        public ActionResult ECharts()
        {
            return View();
        }
    }
}
