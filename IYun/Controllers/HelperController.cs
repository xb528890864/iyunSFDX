using IYun.Models;
using IYun.Object;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace IYun.Controllers
{

    /// <summary>
    /// 公用工具类
    /// </summary>
    public class HelperController : AdminBaseController
    {
        //
        // GET: /Helper/

        //函授站
        public ActionResult SubSchool(HelperParameterM para)
        {
            ViewData["para"] = para;

            if (string.IsNullOrWhiteSpace(para.whereSql) && CurrentAdmin.y_roleID != 5 && CurrentAdmin.y_roleID != 4)
            {
                if (HttpRuntime.Cache["SubSchool"] == null)   //如果缓存不存在了
                {
                    using (var yunEntity = new IYunEntities())
                    {
                        var sqlWhere = "SELECT * FROM YD_Sys_SubSchool order by y_name";
                        var list = yunEntity.Database.SqlQuery<YD_Sys_SubSchool>(sqlWhere).ToList();
                        ViewData["data"] = list;
                        HttpRuntime.Cache.Insert("SubSchool", list, null, DateTime.Now.AddHours(1)
                            , Cache.NoSlidingExpiration);
                    }
                }
                else
                {
                    ViewData["data"] = HttpRuntime.Cache["SubSchool"] as List<YD_Sys_SubSchool>;
                }
            }
            else   //如果有SQL，就跳过缓存直接读取
            {
                using (var yunEntity = new IYunEntities())
                {
                    var sqlWhere = "SELECT * FROM YD_Sys_SubSchool WHERE 1 = 1 " + para.whereSql;
                    if (CurrentAdmin.y_roleID == 5 || CurrentAdmin.y_roleID == 4)
                    {
                        var subIds =
                            yunEntity.YD_Sys_AdminSubLink.Where(a => a.y_adminId == CurrentAdmin.id)
                                .Select(a => a.y_subSchoolId)
                                .ToArray();
                        if (subIds.Length > 0)
                        {
                            sqlWhere += " AND id in (" + string.Join(",", subIds) + ") order by y_name ";
                        }
                    }
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sys_SubSchool>(sqlWhere).ToList();
                }
            }

            return PartialView();
        }
        //函授站转函授站操作
        public ActionResult SubSchool2(HelperParameterM para)
        {
            ViewData["para"] = para;

            if (string.IsNullOrWhiteSpace(para.whereSql) && CurrentAdmin.y_roleID != 5 && CurrentAdmin.y_roleID != 4)
            {
                if (HttpRuntime.Cache["SubSchool"] == null)   //如果缓存不存在了
                {
                    using (var yunEntity = new IYunEntities())
                    {
                        var sqlWhere = "SELECT * FROM YD_Sys_SubSchool order by y_name ";

                        var list = yunEntity.Database.SqlQuery<YD_Sys_SubSchool>(sqlWhere).ToList();


                        ViewData["data"] = list;

                        HttpRuntime.Cache.Insert("SubSchool", list, null, DateTime.Now.AddHours(1)
                            , Cache.NoSlidingExpiration);
                    }

                }
                else
                {
                    ViewData["data"] = HttpRuntime.Cache["SubSchool"] as List<YD_Sys_SubSchool>;
                }
            }
            else   //如果有SQL，就跳过缓存直接读取
            {
                using (var yunEntity = new IYunEntities())
                {
                    var sqlWhere = "SELECT * FROM YD_Sys_SubSchool WHERE 1 = 1 " + para.whereSql;
                    if (CurrentAdmin.y_roleID == 5 || CurrentAdmin.y_roleID == 4)
                    {
                        var subIds =
                            yunEntity.YD_Sys_AdminSubLink.Where(a => a.y_adminId == CurrentAdmin.id)
                                .Select(a => a.y_subSchoolId)
                                .ToArray();
                        if (subIds.Length > 0)
                        {
                            sqlWhere += " AND id in (" + string.Join(",", subIds) + ") order by y_name";
                        }
                    }
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sys_SubSchool>(sqlWhere).ToList();
                }
            }
            return PartialView();
        }

        //只用于异动转函授站
        public ActionResult StrangeSubschool(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (string.IsNullOrWhiteSpace(para.whereSql))
                {
                    var list = yunEntity.YD_Sys_SubSchool.OrderBy(u => u.y_name).ToList();
                    ViewData["data"] = list;
                }
                else
                {
                    var list = yunEntity.Database.SqlQuery<YD_Sys_SubSchool>(para.whereSql).ToList();
                    ViewData["data"] = list;
                }
            }
            return PartialView();
        }

        //入学年份
        public ActionResult EnrollYear(HelperParameterM para)
        {
            ViewData["para"] = para;
            int yearnow = DateTime.Now.Year;
            var yearlist = new List<int>();
            for (int i = 2009; i < yearnow + 6; i++)
            {
                yearlist.Add(i);
            }
            ViewData["data"] = yearlist;

            return PartialView();
        }
        //入学年份--在校生入学年份
        public ActionResult EnrollYearOld(HelperParameterM para)
        {
            ViewData["para"] = para;
            int yearnow = DateTime.Now.Year;
            var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            var yearlist = new List<int>();
            for (int i = 2012; i <= xinshenyear; i++)
            {
                yearlist.Add(i);
            }
            ViewData["data"] = yearlist;

            return PartialView();
        }
        //入学年份--在校生缴费年度
        public ActionResult EnrollYear2(HelperParameterM para)
        {
            ViewData["para"] = para;
            int yearnow = DateTime.Now.Year;

            var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            var yearlist = new List<int>();
            for (int i = 2009; i < xinshenyear + 1; i++)
            {
                yearlist.Add(i);
            }
            ViewData["data"] = yearlist;

            return PartialView();
        }
        //专业库
        public ActionResult MajorLibrary(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (string.IsNullOrWhiteSpace(para.whereSql))
                {
                    var list = yunEntity.YD_Edu_MajorLibrary.OrderBy(u => u.y_name).ToList();
                    ViewData["data"] = list;
                }
                else
                {
                    var list = yunEntity.Database.SqlQuery<YD_Edu_MajorLibrary>(para.whereSql).ToList();
                    ViewData["data"] = list;
                }
            }
            return PartialView();
        }
        //专业库
        public ActionResult MajorLibrary2(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (string.IsNullOrWhiteSpace(para.whereSql))
                {
                    var list = yunEntity.YD_Edu_MajorLibrary.OrderBy(u => u.y_name).ToList();
                    ViewData["data"] = list;
                    //if (HttpRuntime.Cache["MajorLibrary"] == null) //如果缓冲不存在
                    //{
                    //    var list = yunEntity.YD_Edu_MajorLibrary.ToList();
                    //    ViewData["data"] = list;
                    //    HttpRuntime.Cache.Insert("MajorLibrary", list, null, DateTime.Now.AddHours(1)
                    //        , Cache.NoSlidingExpiration);
                    //}
                    //else
                    //{
                    //    ViewData["data"] = HttpRuntime.Cache["MajorLibrary"] as List<YD_Edu_MajorLibrary>;
                    //}
                }
                else
                {
                    var list = yunEntity.Database.SqlQuery<YD_Edu_MajorLibrary>(para.whereSql).ToList();
                    ViewData["data"] = list;
                }
            }

            return PartialView();
        }
        //层次
        public ActionResult EduType(HelperParameterM para)
        {
            ViewData["para"] = para;
            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_EduType>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_EduType.ToList();
                }
            }

            return PartialView();
        }

        //学习形式
        public ActionResult StuType(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_StuType>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_StuType.ToList();
                }
            }

            return PartialView();
        }

        //学籍状态
        public ActionResult StuState(HelperParameterM para)
        {
            ViewData["para"] = para;
            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_StuState>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_StuState.ToList();
                }
            }
            return PartialView();
        }
        //角色
        public ActionResult Role(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sys_Role>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Sys_Role.ToList();
                }
            }

            return PartialView();
        }
        //操作类型
        public ActionResult LogType(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sys_DbLogType>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Sys_DbLogType.ToList();
                }
            }

            return PartialView();
        }

        //民族
        public ActionResult Nation(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sts_Nation>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Sts_Nation.ToList();
                }
            }

            return PartialView();
        }

        //政治面貌
        public ActionResult Politics(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sts_Politics>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Sts_Politics.ToList();
                }
            }

            return PartialView();
        }
        //课程类型
        public ActionResult CourseType(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_CourseType>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_CourseType.OrderBy(u => u.y_name).ToList();
                }
            }

            return PartialView();
        }
        //课程库
        public ActionResult Course(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_Course>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_Course.OrderBy(u => u.y_name).ToList();
                }
            }

            return PartialView();
        }
        //异动类型
        public ActionResult StrangeType(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Sts_StrangeType>(para.whereSql).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Sts_StrangeType.ToList();
                }
            }
            return PartialView();
        }

        //专业综合
        public ActionResult Major(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {





                ViewData["data"] = yunEntity.YD_Edu_Major.OrderBy(u => u.y_name).ThenBy(u => u.y_eduTypeId).ToList();
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_Major>(para.whereSql).OrderBy(u => u.y_majorLibId).ThenBy(u => u.y_eduTypeId).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_Major.OrderBy(u => u.y_name).ThenBy(u => u.y_eduTypeId).ToList();
                }
            }

            return PartialView();
        }
        //专业综合
        public ActionResult Major2(HelperParameterM para)
        {
            ViewData["para"] = para;

            using (var yunEntity = new IYunEntities())
            {
                ViewData["data"] = yunEntity.YD_Edu_Major.OrderBy(u => u.y_name).ThenBy(u => u.y_eduTypeId).ToList();
                if (!string.IsNullOrWhiteSpace(para.whereSql))
                {
                    ViewData["data"] = yunEntity.Database.SqlQuery<YD_Edu_Major>(para.whereSql).OrderBy(u => u.y_majorLibId).ThenBy(u => u.y_eduTypeId).ToList();
                }
                else
                {
                    ViewData["data"] = yunEntity.YD_Edu_Major.OrderBy(u => u.y_name).ThenBy(u => u.y_eduTypeId).ToList();
                }
            }

            return PartialView();
        }

        //通过函授站获取专业
        public string GetMajor()
        {
            using (var db = new IYunEntities())
            {
                var sb = new StringBuilder();
                var strSubId = Request["subId"];
                var subId = Convert.ToInt32(strSubId);
                var list = db.VW_StuInfo.Where(u => u.y_subSchoolId == subId).OrderBy(u => u.majorName).DistinctBy(u => new { u.majorName, u.y_majorId, u.y_subSchoolId }).ToList();
                sb.AppendFormat("<option value='{0}'>{1}</option>", "0", "请选择");
                foreach (var major in list)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", major.y_majorId, major.majorName);
                }
                return sb.ToString();
            }
        }


        //通过专业获取函授站--新生
        public string GetSubSchool()
        {
            using (var db = new IYunEntities())
            {
                var strMajId = Request["majorId"];
                var majId = Convert.ToInt32(strMajId);
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list;
                if (majId == 0)
                {
                    list = db.VW_StuInfo.Where(u => u.y_subSchoolId != null && u.y_inYear == xinshenyear).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_majorLibId == majId && u.y_subSchoolId != null && u.y_inYear == xinshenyear).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();

                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_subSchoolId, u.schoolName });
                return JsonConvert.SerializeObject(lists);

            }
        }
        //通过专业获取函授站
        public string GetSubSchoolOld()
        {
            using (var db = new IYunEntities())
            {
                var strMajId = Request["majorId"];
                var majId = Convert.ToInt32(strMajId);
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list;
                if (majId == 0)
                {
                    list = db.VW_StuInfo.Where(u => u.y_subSchoolId != null && u.y_inYear != xinshenyear).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_majorLibId == majId && u.y_subSchoolId != null && u.y_inYear != xinshenyear).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();

                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_subSchoolId, u.schoolName });
                return JsonConvert.SerializeObject(lists);

            }
        }
        //通过专业获取函授站--只用于新生预注册
        public string GetSubNewRegister()
        {
            using (var db = new IYunEntities())
            {
                var strMajId = Request["majorId"];
                var majId = Convert.ToInt32(strMajId);
                IQueryable<VW_StuInfo> list;
                if (majId == 0)
                {
                    list = db.VW_StuInfo.Where(u => u.y_subSchoolId != null).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_majorLibId == majId && u.y_subSchoolId != null).OrderBy(u => u.schoolName).DistinctBy(u => new { u.y_subSchoolId }).AsQueryable();
                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_subSchoolId, u.schoolName });
                return JsonConvert.SerializeObject(lists);

            }
        }
        //通过层次获取专业--只用于新生预注册
        public string GetMajorLibNewRegister()
        {
            using (var db = new IYunEntities())
            {
                var edutypeId = Request["eduId"];
                var eduId = Convert.ToInt32(edutypeId);
                IQueryable<VW_StuInfo> list;
                if (eduId == 0)
                {
                    list = db.VW_StuInfo.OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_eduTypeId == eduId).OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();
                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_majorLibId, u.majorLibraryName });
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过层次获取专业--新生
        public string GetMajorLibrary()
        {
            using (var db = new IYunEntities())
            {
                var edutypeId = Request["eduId"];
                var eduId = Convert.ToInt32(edutypeId);
                var xinshen = ConfigurationManager.AppSettings["xinsheng"].ToString();
                var xinshenyear = Convert.ToInt32(xinshen);
                IQueryable<VW_StuInfo> list;
                if (eduId == 0)
                {
                    list = db.VW_StuInfo.Where(u => u.y_inYear == xinshenyear).OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_eduTypeId == eduId && u.y_inYear == xinshenyear).OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();

                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_majorLibId, u.majorLibraryName });
                return JsonConvert.SerializeObject(lists);
            }
        }
        //通过层次获取专业
        public string GetMajorLibraryOld()
        {
            using (var db = new IYunEntities())
            {
                var edutypeId = Request["eduId"];
                var eduId = Convert.ToInt32(edutypeId);
                IQueryable<VW_StuInfo> list;
                if (eduId == 0)
                {
                    list = db.VW_StuInfo.OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();
                }
                else
                {
                    list = db.VW_StuInfo.Where(u => u.y_eduTypeId == eduId).OrderBy(u => u.majorLibraryName).DistinctBy(u => new { u.y_majorLibId }).AsQueryable();
                }
                var lists = list.OrderBy(u => u.y_majorLibId).Select(u => new { u.y_majorLibId, u.majorLibraryName });
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站，层次获取班级教学计划中的专业
        public string GetMajorLibrary_Teaplan(int schoolid, int eduId, int year, int stuTypeId)
        {
            using (var db = new IYunEntities())
            {
                var list =
                    db.YD_TeaPlan_Class.Where(
                        u =>
                            u.y_subSchoolId == schoolid && u.YD_Edu_Major.y_eduTypeId == eduId && u.y_year == year &&
                            u.YD_Edu_Major.y_stuTypeId == stuTypeId)
                        .AsQueryable();
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站获取班级教学计划中的专业
        public string GetMajor_Teaplan(int schoolid, int year)
        {
            using (var db = new IYunEntities())
            {
                var list =
                    db.YD_TeaPlan_Class.AsQueryable();

                if (year != 0)
                {
                    list = list.Where(u => u.y_year == year);
                }
                if (schoolid != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolid);
                }

                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.id,
                                majorName = u.YD_Edu_Major.y_name
                            }).DistinctBy(u => new { u.id })
                        .OrderBy(u => u.majorName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过专业，函授站获取课程
        public string GetCourse(int majorId, int schoolId, int year, int eduId, int team, int stuTypeId)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_TeaPlan_ClassCourseDes.Include(u => u.YD_Edu_Course).Include(u => u.YD_TeaPlan_Class).AsQueryable();

                list = list.Where(u => u.YD_TeaPlan_Class.y_subSchoolId == schoolId && u.YD_TeaPlan_Class.y_year == year);  //函授站年份筛选
                list =
                    list.Where(
                        u =>
                            u.YD_TeaPlan_Class.YD_Edu_Major.y_majorLibId == majorId &&
                            u.YD_TeaPlan_Class.YD_Edu_Major.y_eduTypeId == eduId && u.YD_TeaPlan_Class.YD_Edu_Major.y_stuTypeId == stuTypeId); //专业层次筛选

                var teamindex = team;

                list = list.Where(u => u.y_team == teamindex);



                var lists =
                    list.Select(u => new { u.YD_Edu_Course.id, u.YD_Edu_Course.y_name }).DistinctBy(u => new { u.id })
                        .OrderBy(u => u.y_name)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过专业，函授站获取课程
        public string GetCourse_S(int majorId, int schoolId, int year, int term)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_TeaPlan_ClassCourseDes.Include(u => u.YD_Edu_Course).Include(u => u.YD_TeaPlan_Class).AsQueryable();

                if (year != 0)
                {
                    list = list.Where(u => u.YD_TeaPlan_Class.y_year == year);
                }
                if (schoolId != 0)
                {
                    list = list.Where(u => u.YD_TeaPlan_Class.y_subSchoolId == schoolId);
                }
                if (majorId != 0)
                {
                    list = list.Where(u => u.YD_TeaPlan_Class.YD_Edu_Major.id == majorId);
                }
                if (term != 0)
                {
                    list = list.Where(u => u.y_team == term);
                }

                var lists =
                    list.Select(u => new { u.YD_Edu_Course.id, u.YD_Edu_Course.y_name }).DistinctBy(u => new { u.id })
                        .OrderBy(u => u.y_name)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过学生年份获取函授站
        public string GetSchoolByYear_Stu(int year)
        {
            using (var db = new IYunEntities())
            {
                var list = db.VW_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1).AsQueryable();

                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }


                var lists =
                    list.Select(u => new { u.y_subSchoolId, u.schoolName }).DistinctBy(u => new { u.y_subSchoolId })
                        .OrderBy(u => u.schoolName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站，层次获取学生中的专业
        public string GetMajorLibrary_Stu(int schoolid, int eduId, int year, int stuTypeId)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1);
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }
                if (schoolid != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolid);
                }
                if (eduId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == eduId);
                }
                if (stuTypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_stuTypeId == stuTypeId);
                }

                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站获取学生中的专业
        public string GetMajor_Stu(int year, int schoolId)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1);
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                id = u.YD_Edu_Major.id,
                                majorName = u.YD_Edu_Major.y_name
                            }).DistinctBy(u => new { u.id })
                        .OrderBy(u => u.majorName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过函授站，层次获取学生的专业--用于新生信息
        public string GetNewMajorLibrary_Stu(int schoolId, int edutypeId)
        {
            using (var db = new IYunEntities())
            {
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1 && u.y_inYear == xinshenyear);
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                if (edutypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeId);
                }
                if (schoolId == 0 && YdAdminRoleId == 4)
                {
                    var subSchoolIds = db.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }
        //通过入学年份，函授站获取学生的专业 --用于班级教学计划
        public string GetSubMajorLibrary_Stu(int schoolId, int year, int edutypeId)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1);
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                if (edutypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeId);
                }
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }

                if (schoolId == 0 && YdAdminRoleId == 4)
                {
                    var subSchoolIds = db.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站，层次获取学生的专业--用于新生注册
        public string GetNewFeeMajorLibrary_Stu(int schoolId, int edutypeId, int year)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1);
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                if (edutypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeId);
                }
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }
                if (schoolId == 0 && YdAdminRoleId == 4)
                {
                    var subSchoolIds = db.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站，层次获取学生的专业--用于在校生注册
        public string GetFeeMajorLibrary_Stu(int schoolId, int edutypeId, int year)
        {
            using (var db = new IYunEntities())
            {
                var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1 && u.y_inYear != xinshenyear);
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                if (edutypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeId);
                }
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }
                if (schoolId == 0 && YdAdminRoleId == 4)
                {
                    var subSchoolIds = db.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过入学年份，函授站，层次获取学生的专业--用于学籍信息
        public string GetStuMajorLibrary_Stu(int schoolId, int edutypeId, int year)
        {
            using (var db = new IYunEntities())
            {
                var list = db.YD_Sts_StuInfo.Where(u => u.y_subSchoolId != null && u.y_isdel == 1 && u.y_stuNum != "");
                //不显示未注册和注册待审核学生
                var state = db.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "未注册");
                var statecheck = db.YD_Edu_StuState.FirstOrDefault(u => u.y_name == "注册待审核");
                if (state != null)
                {
                    list = list.Where(u => u.y_stuStateId != state.id);
                }
                if (statecheck != null)
                {
                    list = list.Where(u => u.y_stuStateId != statecheck.id);
                }
                if (schoolId != 0)
                {
                    list = list.Where(u => u.y_subSchoolId == schoolId);
                }
                if (edutypeId != 0)
                {
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeId);
                }
                if (year != 0)
                {
                    list = list.Where(u => u.y_inYear == year);
                }
                if (schoolId == 0 && YdAdminRoleId == 4)
                {
                    var subSchoolIds = db.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId.Value));
                }
                var lists =
                    list.Select(
                        u =>
                            new
                            {
                                u.YD_Edu_Major.y_majorLibId,
                                majorLibraryName = u.YD_Edu_Major.YD_Edu_MajorLibrary.y_name
                            }).DistinctBy(u => new { u.y_majorLibId })
                        .OrderBy(u => u.majorLibraryName)
                        .ToList();
                return JsonConvert.SerializeObject(lists);
            }
        }

        //通过学生层次形式获取对应专业
        public string GetMajorByEdu_Stu(int eduid, int stutypeid)
        {
            using (var db = new IYunEntities())
            {
                //------------原-------------------

                //var list = db.VW_StuInfo.Where(u=>u.y_isdel == 1).AsQueryable();

                //if (eduid != 0)
                //{
                //    list = list.Where(u => u.y_eduTypeId == eduid);
                //}
                // if (stutypeid != 0)
                //{
                //    list = list.Where(u => u.y_stuTypeId == stutypeid);
                //}


                //var lists =
                //    list.Select(u => new { u.y_majorId, u.majorName }).DistinctBy(u => new { u.y_majorId })
                //        .OrderBy(u => u.majorName)
                //        .ToList();
                //-------------------------------------------------------------------------------

                var list = db.YD_Edu_Major.Where(u => true);

                if (eduid != 0)
                {
                    list = list.Where(u => u.y_eduTypeId == eduid);
                }

                //if (stutypeid != 0)
                //{
                //    list = list.Where(u => u.y_stuTypeId == stutypeid);
                //}


                var lists = list.Select(u => new { y_majorId = u.id, majorName = u.y_name }).DistinctBy(u => new { u.y_majorId })
                        .OrderBy(u => u.majorName)
                        .ToList();

                return JsonConvert.SerializeObject(lists);
            }
        }
    }
}
