using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IYun.Common;
using IYun.Dal;
using IYun.Models;
using IYun.Object;
using Microsoft.Ajax.Utilities;
using Webdiyer.WebControls.Mvc;
using System.IO;
using System.Reflection;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;

namespace IYun.Controllers
{
    /// <summary>
    /// 教务管理 //仅用到新闻和专业库
    /// </summary>
    public class EduController : AdminBaseController
    {
        //
        // GET: /Edu/

        private YD_Edu_CourseDal _courseDal = new YD_Edu_CourseDal();
        private YD_Edu_MajorLibraryDal _majorLibraryDal = new YD_Edu_MajorLibraryDal();
        private YD_Edu_TeachPlanDal _teachPlanDal = new YD_Edu_TeachPlanDal();
        private YD_Edu_TeachPlanDesDesDal _teachPlanDesDal = new YD_Edu_TeachPlanDesDesDal();
        /// <summary>
        /// 首页新闻
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Shouye()
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                IQueryable<YD_Edu_News> list = yunEntities.YD_Edu_News.OrderByDescending(u => u.id);//查新闻，降序排列
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId==9)//YdAdminRole是继承自Adminbase的属性，获取Seesion中的权限Id
                {
                    var subSchoolIds =yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId).ToList();
                    list = list.Where(u => (u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value)) || u.y_subSchoolId == 0);             
                }//遍历判断只有有学校ID或者学校ID为0的才能放入List
                var newslist = list.Where(u => u.y_type == 2).Take(8).ToList();//然后是新闻类型分类
                var listedu = list.Where(u => u.y_type == 1).Take(8).ToList();
                var listtixi = list.Where(u => u.y_type == 3).Take(8).ToList();
                var listqita = list.Where(u => u.y_type == 4).Take(8).ToList();
                ViewBag.newslist = newslist;//放入ViewBag
                ViewBag.listedu = listedu;
                ViewBag.listtixi = listtixi;
                ViewBag.listqita = listqita;
                return View();
            }
        }

        /// <summary>
        /// 通知公告
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult News(int id = 1)
        {
            #region “通知公告”权限验证

            var power = SafePowerPage("/Edu/News");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion       
            var type = Request["type"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                IQueryable<VW_Sts_News> list = yunEntities.VW_Sts_News.OrderByDescending(u => u.id);
                if (!string.IsNullOrWhiteSpace(type) && !type.Equals("0"))
                {
                    var typeid = Convert.ToInt32(type);
                    list = list.Where(u => u.y_type == typeid);
                }
               if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId==9)
                {
                    var subSchoolIds =yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId).ToList();
                    list = list.Where(u => (u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value)) || u.y_subSchoolId == 0); 
                }
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize
                if (Request.IsAjaxRequest())
                    return PartialView("NewsList", model);

                ViewBag.admin = YdAdminRoleId;
                return View(model);
            }
        }

        /// <summary>
        /// 查看公告内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult NewDes(int? id)
        {
            #region “通知公告”权限验证

            var power = SafePowerPage("/Edu/News");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            if (!id.HasValue)
            {
                return RedirectToAction("News");
            }
            using (var yunEntities = new IYunEntities())
            {
                var newshits = yunEntities.YD_Edu_News.FirstOrDefault(u => u.id == id);
                if (newshits.y_hits == null)
                    newshits.y_hits = 0;
                newshits.y_hits += 1;
                yunEntities.Entry(newshits).State = EntityState.Modified;
                yunEntities.SaveChanges();
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                var news = yunEntities.YD_Edu_News.SingleOrDefault(u => u.id == id.Value);
                if (news == null)
                {
                    return RedirectToAction("News");
                }
                ViewData["news"] = news;
            }
            ViewBag.admin = YdAdminRoleId;
            return View();
        }

        /// <summary>
        /// 添加公告页面
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult NewAddPage()
        {
            #region “通知公告”权限验证

            var power = SafePowerPage("/Edu/News");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            IsLogin();
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewBag.admin = YdAdminRelName;
            }
            return View();
        } 
        /// <summary>
        /// 添加公告
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult NewAdd(string yContent)
        {
            var yType = Convert.ToInt32(Request["y_type"]);
            var subschool = Request["subschol"];
            var yTitle = Request["y_title"];
            var yAppyname = Request["y_appyname"];
            //var yContent = Request.Form["yContent"];
            var yAppytime = Convert.ToDateTime(Request["y_appytime"]);
            using (var yunEntities = new IYunEntities())
            {
                var subschoolId = Convert.ToInt32(subschool);
                var subschoolname = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subschoolId);
                if (subschoolname != null)
                {
                    var subschoolid = subschoolname.id;
                    var news = new YD_Edu_News
                    {
                        y_subSchoolId = subschoolid,
                        y_type = yType,
                        y_title = yTitle,
                        y_appyname = yAppyname,
                        y_content = yContent,
                        y_appytime = yAppytime,
                        y_hits = 0,
                        y_usable = 1
                    };
                    yunEntities.Entry(news).State = EntityState.Added;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",添加公告,标题是:" + news.y_title + ",方法:NewAdd");
                }
                else
                {
                    var news = new YD_Edu_News
                    {
                        y_subSchoolId = 0,
                        y_type = yType,
                        y_title = yTitle,
                        y_appyname = yAppyname,
                        y_content = yContent,
                        y_appytime = yAppytime,
                        y_hits = 0,
                        y_usable = 1
                    };
                    yunEntities.Entry(news).State = EntityState.Added;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",添加公告,标题是:" + news.y_title + ",方法:NewAdd");
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
        }

        /// <summary>
        /// 修改公告页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult NewEditPage(int id)
        {
            #region “通知公告”权限验证

            var power = SafePowerPage("/Edu/News");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_News.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult NewEdit(string yContent)
        {
            var id = Convert.ToInt32(Request["id"]);
            var subschool = Request["subschol"];
            var yType = Convert.ToInt32(Request["y_type"]);
            var yTitle = Request["y_title"];
            var yAppyname = Request["y_appyname"];
            //var yContent = Request["yContent"];
            var yAppytime = Convert.ToDateTime(Request["y_appytime"]);
            var hits = Convert.ToInt32(Request["hits"]);
            using (var yunEntities = new IYunEntities())
            {
                var subschoolId = Convert.ToInt32(subschool);
                var subschoolname = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subschoolId);
                if (subschoolname != null)
                {
                    var news = new YD_Edu_News
                    {
                        id = id,
                        y_subSchoolId = subschoolname.id,
                        y_type = yType,
                        y_title = yTitle,
                        y_appyname = yAppyname,
                        y_content = yContent,
                        y_appytime = yAppytime,
                        y_hits = hits,
                        y_usable = 1
                    };
                    yunEntities.Entry(news).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",修改公告,ID:" + news.id + ",方法:NewEdit");             
                }
                else
                {
                    var news = new YD_Edu_News
                    {
                        id = id,
                        y_subSchoolId = 0,
                        y_type = yType,
                        y_title = yTitle,
                        y_appyname = yAppyname,
                        y_content = yContent,
                        y_appytime = yAppytime,
                        y_hits = hits,
                        y_usable = 1
                    };
                    yunEntities.Entry(news).State = EntityState.Modified;

                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",修改公告,ID:" + news.id + ",方法:NewEdit");
                }
                int t = yunEntities.SaveChanges();
                if (t > 0)
                {
                    return Content("ok");
                }
                else
                {
                    return Content("修改失败");
                }
            }
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <returns></returns>
        public ActionResult NewDelete(int id)
        {
             #region “通知公告”权限验证

            var power = SafePowerPage("/Edu/News");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_delete == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                var news = yunEntities.YD_Edu_News.FirstOrDefault(u => u.id == id);
                yunEntities.Entry(news).State = EntityState.Deleted;
                int t = yunEntities.SaveChanges();
                if (t > 0)
                {
                    LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",删除公告,ID:" + news.id + ",方法:NewDelete");
                    return Content("ok");
                }
                else
                {
                    return Content("未知错误");
                }
            }
        }

        #region 专业库


        /// <summary>
        /// 导入标准专业代码
        /// </summary>
        public string ImportStandardCode()
        {
            string fileName = @"G:\专科.xlsx";
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var workbook = new XSSFWorkbook(fs);
            ISheet sheet = workbook.GetSheetAt(0);
            var firstRow =  sheet.GetRow(0);
            int cellCount = firstRow.LastCellNum;
            DataTable data = new DataTable();
            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
            {
                var cell = firstRow.GetCell(i);
                if (cell == null) continue;
                var cellValue = cell.StringCellValue;
                if (cellValue == null) continue;
                var column = new DataColumn(cellValue);
                data.Columns.Add(column);
            }
            var rowCount = sheet.LastRowNum;
            for (var i = 1; i <= rowCount; ++i)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue; //没有数据的行默认是null　　　　　　　

                var dataRow = data.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; ++j)
                {
                    dataRow[data.Columns[j]] = row.GetCell(j).ToString();
                }
                data.Rows.Add(dataRow);
            }
            using (var db = new IYunEntities())
            {
                var majorList = db.YD_Edu_Major.Include(x=>x.YD_Edu_MajorLibrary).ToList();
                majorList = majorList.Where(x => x.y_eduTypeId == 2).ToList();
                foreach (DataRow dr in data.Rows)
                {
                    var list = majorList.Where(x => x.YD_Edu_MajorLibrary.y_name == dr["ZYMC"].ToString()).ToList();
                    if(list.Count>0)
                    foreach (YD_Edu_Major item in list)
                    {
                        item.y_StandardCode = dr["ZYDM"].ToString();

                        db.Entry(item).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
            }
            return "ok";
        }

        public ActionResult MajorLib(int id = 1)
        {
            #region “专业库管理”权限验证
            var power = SafePowerPage("/Edu/MajorLib");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var nameStr = Request["y_name"];
                //var entityList = new List<VW_MajorLibrary>();
                var entityList = new List<VW_Major>();
                if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    #region 注释的是专业库的模糊查询
                    //toal:nameStr.Trim().Replace(" ", "").Replace("专业", "")
                    // toal:注释的是专业库的模糊查询
                    //if (name.Length <= 2)
                    //{
                    //    entityList = yunEntities.VW_MajorLibrary.Where(u => u.y_name.Contains(name)).ToList();
                    //}
                    //else
                    //{
                    //toal:.name.Substring(i, 2)
                    //for (var i = 0; i < name.Length; i++)
                    //{
                    //sql += " y_name like '" + name + "%'";
                    //if (i + 1 < name.Length - 1)
                    //{
                    //    sql += " or ";
                    //}
                    //}
                    //entityList =
                    //    yunEntities.Database.SqlQuery<VW_MajorLibrary>("select * from VW_MajorLibrary where " + sql)
                    //  
                    //} 
                    #endregion
                    var name = nameStr.Trim();
                    var sql = " y_name like '" + name + "%'";
                    entityList =yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    var dbLogList = entityList.OrderByDescending(u => u.y_eduTypeId).ToPagedList(id, 15);
                    //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorLibList", dbLogList);
                    return View(dbLogList);
                }
                else
                {
                    var dbLogList = yunEntities.VW_Major.Where(u => true)
                        .OrderBy(u => u.y_eduTypeId)
                        .ThenByDescending(u => u.id)
                        .ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorLibList", dbLogList);
                    return View(dbLogList);
                }
            }
        }

        public ActionResult DownloadAllMajorLibs()
        {
            using (IYunEntities ctx = new IYunEntities())
            {
                var list = ctx.YD_Edu_Major.Select(e => new {
                    e.YD_Edu_MajorLibrary.y_name,
                    e.y_StandardCode,
                    stuTypeName = e.YD_Edu_StuType.y_name,
                    eduTypeName = e.YD_Edu_EduType.y_name,
                    e.y_stuYear, e.y_needFee }).AsNoTracking().ToList();

                var model = FileHelper.ToDataTable(list);
                
                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/专业信息表" + Guid.NewGuid() + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/年度专业信息表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable { { "y_name", "专业名" }, { "y_StandardCode", "专业代码" }, { "stuTypeName", "学习形式" }, { "eduTypeName", "层次" }, { "y_stuYear", "学制" }, { "y_needFee", "缴费金额" } };
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
        /// 专业库下载
        /// </summary>
        /// <returns>视图</returns>
        //public ActionResult DownloadMajorLib()
        //{
        //    #region 权限验证
        //    var power = SafePowerPage("/Edu/MajorLib");
        //    if (!IsLogin())
        //    {
        //        Redirect("/AdminBase/Index");
        //    }
        //    if (power == null || power.y_select == (int) PowerState.Disable)
        //    {
        //        var reurl = Request.UrlReferrer.ToString();
        //        return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
        //    }
        //    #endregion
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        var major = Request["MajorLibrary"];
        //        var eduType = Request["EduType"];
        //        var code = Request["code"];
        //        IQueryable<VW_MajorLibrary> list =
        //            yunEntities.VW_MajorLibrary.OrderByDescending(u => u.id);
        //        if (!string.IsNullOrWhiteSpace(major) && !major.Equals("0"))
        //        {
        //            var majorLibraryint = Convert.ToInt32(major);
        //            list = list.Where(u => u.id == majorLibraryint);
        //        }
        //        if (!string.IsNullOrWhiteSpace(eduType) && !eduType.Equals("0"))
        //        {
        //            var eduTypeint = Convert.ToInt32(eduType);
        //            list = list.Where(u => u.y_eduTypeId == eduTypeint);
        //        }

        //        if (!string.IsNullOrWhiteSpace(code))
        //        {
        //            list = list.Where(u => u.y_code == code);
        //        }
        //        var model =
        //            FileHelper.ToDataTable(
        //                list.Select(
        //                    u => new {majorLibraryName = u.y_name, y_code = u.y_code, eduTypeName = u.y_eduTypeName})
        //                    .ToList());

        //        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
        //        if (!Directory.Exists(dirPath)) //todo:改变
        //        {
        //            Directory.CreateDirectory(dirPath); //todo:改变
        //        }
        //        var filename1 = "/年度专业信息表" + Guid.NewGuid() + ".xls"; //todo:改变
        //        var fileName3 = dirPath + filename1; //todo:改变

        //        //var filename1 = "File/Dowon/年度专业信息表" + Guid.NewGuid() + ".xls";
        //        //var fileName2 = "~/" + filename1;
        //        //var fileName3 = Server.MapPath(fileName2);
        //        using (var excelHelper = new ExcelHelper(fileName3))
        //        {
        //            var ht = new Hashtable {{"majorLibraryName", "专业"}, {"y_code", "专业代码"}, {"eduTypeName", "层次"}};
        //            int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
        //            if (t > 0)
        //            {
        //                string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
        //                return Redirect(url);
        //            }
        //            var reurl = Request.UrlReferrer.ToString();
        //            return Content("<script>alert('错误');window.location.href='" + reurl + "'</script>");
        //        }

        //    }
        //}



        #region 验证重复性

        /// <summary>
        /// 验证专业名重复
        /// </summary>
        /// <returns></returns>
        public ActionResult MajorLibNameCheckUp()
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Edu/MajorLib");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
                        " SELECT  id ,formc.y_name ,y_code,y_eduTypeId FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_name FROM YD_Edu_MajorLibrary GROUP BY y_name) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN YD_Edu_MajorLibrary AS formc ON formb.y_name=formc.y_name")
                        .ToList();
                return View();
            }
        }

        /// <summary>
        /// 验证专业代码重复
        /// </summary>
        /// <returns></returns>
        public ActionResult MajorLibCodeCheckUp()
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Edu/MajorLib");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
                        " SELECT  id ,y_name ,formc.y_code,y_eduTypeId  FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_code FROM dbo.YD_Edu_MajorLibrary  GROUP BY y_code) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.YD_Edu_MajorLibrary AS formc ON formb.y_code=formc.y_code")
                        .ToList();
                return View();
            }
        }

        #endregion


        #endregion

        #region 课程库  --不开放

        public ActionResult Course(int id = 1)
        {
            #region “课程库管理”权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
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
                        var sql = " y_name like '" + name + "%'";
                        //for (var i = 0; i < name.Length - 1; i++)
                        //{
                        //    sql += " y_name like '%" + name.Substring(i, 2) + "%' ";
                        //    if (i + 1 < name.Length - 1)
                        //    {
                        //        sql += " or ";
                        //    }
                        //}
                        entityList =
                            yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " + sql)
                                .ToList();
                    }

                    var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
                    //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("CourseList", dbLogList);
                    return View(dbLogList);

                }
                else
                {
                    var dbLogList =
                        yunEntities.YD_Edu_Course.Where(u => true)
                            .OrderBy(u => u.y_name)
                            .ThenByDescending(u => u.id)
                            .ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("CourseList", dbLogList);
                    return View(dbLogList);
                }


            }
        }


        /// <summary>
        /// 课程库下载
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadCourse()
        {
            #region 权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var Course = Request["Course"];
                var code = Request["code"];
                IQueryable<YD_Edu_Course> list =
                    yunEntities.YD_Edu_Course.OrderByDescending(u => u.id);
                if (!string.IsNullOrWhiteSpace(Course) && !Course.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(Course);
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    list = list.Where(u => u.y_code == code);
                }
                var model =
                    FileHelper.ToDataTable(list.Select(u => new {Course = u.y_name, y_code = u.y_code}).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/课程库下载表"  + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/课程库下载表" + Guid.NewGuid() + ".xls";
                //    var fileName2 = "~/" + filename1;
                //    var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable {{"CourseType", "课程名"}, {"y_code", "专业代码"}};
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Redirect(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('错误');window.location.href='" + reurl + "'</script>");
                }

            }
        }




        #region 批量导入课程信息

        /// <summary>
        /// 批量导入课程页面
        /// </summary>
        /// <returns></returns>
        public
            ActionResult UploadCourse()
        {
            #region 权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            var fileName = Request["filename"];
            if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('该文件不是正确的Excel文件');window.location.href='" + reurl + "'</script>");
            }
            fileName = Server.MapPath(fileName);
            using (var excelHelper = new ExcelHelper(fileName))
            {
                var dt = excelHelper.ExcelToDataTable("", true);
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Edu_FormTemp");
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    string name = "";
                    string code = "";
                    string nameMatch = "";
                    int isOk = (int) YesOrNo.No;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        name = "";
                        code = "";
                        nameMatch = "";
                        isOk = (int) YesOrNo.No;
                        if (dt.Rows[i]["课程名"] != null)
                        {
                            name = dt.Rows[i]["课程名"].ToString().Trim().Replace(" ", "");

                            if (name.Length <= 2)
                            {
                                var entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name));
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
                                    yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " +
                                                                                 sql).ToList();
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
                        }
                        if (dt.Rows[i]["课程代码"] != null)
                        {
                            code = dt.Rows[i]["课程代码"].ToString();
                        }
                        else
                        {
                            code = "111";
                        }
                        var studentTemp = new YD_Edu_FormTemp()
                        {
                            y_name = name,
                            y_nameMatch = nameMatch,
                            y_code = code,
                            y_isOk = isOk
                        };
                        yunEntities.Entry(studentTemp).State = EntityState.Added;
                    }
                    yunEntities.SaveChanges();
                    ViewBag.entityList = yunEntities.YD_Edu_FormTemp.Where(u => true).ToList();
                    return View();
                }
            }
        }

        public ActionResult NotUploadCourse(string id)
        {
            #region 权限验证

            var power = SafePowerPage("/edu/course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
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

        public ActionResult NeedUploadCourse(string id)
        {
            #region 权限验证

            var power = SafePowerPage("/edu/course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
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
                        tem.y_isOk = (int) YesOrNo.Yes;
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
        /// 验证导入的临时课程信息
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCourse()
        {
            #region 权限验证

            var power = SafePowerPage("/edu/course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var db = new IYunEntities())
            {
                const int isOk = (int) YesOrNo.No;
                var list = db.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk);
                string deleteCourse = "";
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.y_code) && !string.IsNullOrEmpty(item.y_name))
                    {
                        //var code = db.YD_Edu_Course.FirstOrDefault(u => u.y_code == item.y_code);
                        var course = db.YD_Edu_Course.FirstOrDefault(u => u.y_name == item.y_name);
                        if (course == null)
                        {
                            item.y_isOk = (int) YesOrNo.Yes;
                        }
                        else
                        {
                            deleteCourse += item.y_name + ",";
                            db.Entry(item).State = EntityState.Deleted;
                        }
                    }
                }
                ViewData["deleteCourse"] = deleteCourse.ToString();
                int r = db.SaveChanges();
                //if (r > 0)
                //{
                //    return Content("<script type='text/javascript'>alert('有重复课程不导入，重复条数为" + r + "');window.location.href='/Edu/VerifyCourse';</script >");
                //}
                ViewBag.entityList = db.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk).ToList();
                ViewBag.modulePowers = GetChildModulePower(db, 2); //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// <summary>
        /// 将验证无误的数据进行导入
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadTrueCourse()
        {
            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int) YesOrNo.Yes;
                var scoreList = yunEntities.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk).ToList();
                for (var i = 0; i < scoreList.Count; i++)
                {
                    var score = new YD_Edu_Course()
                    {
                        y_name = scoreList[i].y_name,
                        y_code = scoreList[i].y_code
                    };
                    yunEntities.Entry(score).State = EntityState.Added;
                }
                yunEntities.SaveChanges();
                return Redirect("Course");
            }
        }

        #endregion

        #region 验证重复性

        /// <summary>
        /// 验证专业名重复
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseNameCheckUp()
        {
            #region “专业库管理”权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Edu_Course>(
                        " SELECT  id ,formc.y_name ,y_code  FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_name FROM dbo.YD_Edu_Course GROUP BY y_name) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.YD_Edu_Course AS formc ON formb.y_name=formc.y_name")
                        .ToList();
                return View();
            }
        }

        /// <summary>
        /// 验证课程代码重复
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseCodeCheckUp()
        {
            #region “课程管理”权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.entityList =
                    yunEntities.Database.SqlQuery<YD_Edu_Course>(
                        " SELECT  id ,y_name ,formc.y_code  FROM (SELECT * FROM (SELECT COUNT(*) AS totalcount,y_code FROM dbo.YD_Edu_Course GROUP BY y_code) AS forma WHERE forma.totalcount>1) AS formb LEFT JOIN dbo.YD_Edu_Course AS formc ON formb.y_code=formc.y_code")
                        .ToList();
                return View();
            }
        }

        #endregion




        /// <summary>
        /// 添加课程库页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseAddPage()
        {
            #region “添加课程库”权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程库视图
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseEditPage(int id)
        {
            #region “编辑课程库”权限验证

            var power = SafePowerPage("/Edu/Course");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程库
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseEdit(YD_Edu_Course role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加课程之前的验证
        /// </summary>
        /// <returns></returns>
        public string CourseAddVerify(YD_Edu_Course major)
        {
            using (var yunEntities = new IYunEntities())
            {
                var name = major.y_name.Trim().Replace(" ", "");
                if (name.Length <= 2)
                {
                    var entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name));
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
                        yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " + sql)
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
        /// 增加课程库
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseAdd(YD_Edu_Course role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseDal.AddEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加课程库(导入教学计划时的自动添加库操作)
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseAddT(YD_Edu_Course role)
        {
            using (var yunEntities = new IYunEntities())
            {
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

        /// <summary>
        /// 删除课程库
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseDal.EntityDelete(id, yunEntities));
            }
        }

        #endregion

        #region 年度专业  --弃用

        /// <summary>
        /// 年度专业页面--弃用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Major(int id = 1)
        {
            #region “年度专业管理”权限验证

            var power = SafePowerPage("/Edu/Major");
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
                var year = Request["year"];
                var nameStr = Request["y_name"];
                var major = Request["MajorLibrary"];
                var majorteach = yunEntities.YD_Edu_MajorTeachPlan.Where(u => true).ToList();
                var majorteachs = new List<int>();
                for (var i = 0; i < majorteach.Count; i++)
                {
                    majorteachs.Add(majorteach[i].y_majorId);
                }
                //if (!string.IsNullOrWhiteSpace(nameStr))
                //{
                //    var name = nameStr.Trim();
                //    #region  注释掉的模糊查找
                //    //if (name.Length <= 2)
                //    //{
                //    //    entityList = yunEntities.VW_Major.Where(u => u.y_name.Contains(name)).ToList();
                //    //}
                //    //else
                //    //{
                //    //    var sql = "";
                //    //    for (var i = 0; i < name.Length - 1; i++)
                //    //    {
                //    //        sql += " y_name like '" + name.Substring(i, 2) + "%' ";
                //    //        if (i + 1 < name.Length - 1)
                //    //        {
                //    //            sql += " or ";
                //    //        }
                //    //    }
                //    #endregion
                if (!string.IsNullOrWhiteSpace(major))
                {
                    var majorId = Convert.ToInt32(major);

                    //var sql = " y_name like '" + name + "%'";
                    //if (ComEnum.SchoolName.JXKJSFDX.ToString() == ConfigurationManager.AppSettings["SchoolName"].ToString())
                    //{
                    //    sql = " y_name like '" + name + "%' and id in(select y_majorId from YD_Edu_MajorTeachPlan)";
                    //}
                    //entityList =yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    //}
                    var sql = " y_majorLibId=" + majorId + "  and id in(select y_majorId from YD_Edu_MajorTeachPlan)";

                    List<VW_Major> entityList = yunEntities.Database.SqlQuery<VW_Major>("select * from VW_Major where " + sql).ToList();
                    if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                    {
                        var inyear = Convert.ToInt32(year);
                        var majorte = yunEntities.VW_MajorTeachPlan.Where(u => u.y_year == inyear);
                        if (majorte == null)
                        {
                            return Content("<script>alert('没有该年的教学计划')</script>");
                        }
                    }
                    ViewData["year"] = !string.IsNullOrWhiteSpace(year) && !year.Equals("0")
                        ? year
                        : DateTime.Now.AddYears(-1).ToString("yyyy");
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 )
                    {
                        var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                        var stuInfos = yunEntities.YD_Sts_StuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value))
                            .DistinctBy(u => new { u.y_majorId, u.y_subSchoolId }).Select(u => u.y_majorId).ToList();
                        entityList = entityList.Where(u => stuInfos.Contains(u.id)).ToList();
                    }
                    var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
                    //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorList", dbLogList);
                    return View(dbLogList);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(year) && !year.Equals("0"))
                    {
                        var inyear = Convert.ToInt32(year);
                        var majorte = yunEntities.VW_MajorTeachPlan.FirstOrDefault(u => u.y_year == inyear);
                        if (majorte == null)
                        {
                            return Content("<script>alert('没有该年的教学计划')</script>");
                        }

                    }
                    ViewData["year"] = !string.IsNullOrWhiteSpace(year) && !year.Equals("0")
                        ? year
                        : DateTime.Now.AddYears(-1).ToString("yyyy");

                    IQueryable<VW_Major> list =
                        yunEntities.VW_Major.Where(u => majorteachs.Contains(u.id)).OrderByDescending(u => u.id);
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 )
                    {
                        var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                        var stuInfos =
                            yunEntities.VW_StuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value))
                                .DistinctBy(u => new { u.y_majorId, u.y_subSchoolId }).Select(u => u.y_majorId)
                                .ToList();
                        list = list.Where(u => stuInfos.Contains(u.id));
                    }
                    var dbLogList = list.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);
                    //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("MajorList", dbLogList);
                    return View(dbLogList);
                }
            }
        }

        #endregion

        /// <summary>
        /// 年度专业下载--弃用
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadMajor()
        {
            #region 权限验证

            var power = SafePowerPage("/Edu/Major");
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
                var major = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var code = Request["code"];
                IQueryable<VW_Major> list =
                    yunEntities.VW_Major.OrderByDescending(u => u.id);
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
                if (!string.IsNullOrWhiteSpace(code))
                {
                    list = list.Where(u => u.y_code == code);
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    majorLibraryName = u.majorLibraryName,
                                    eduTypeName = u.eduTypeName,
                                    stuTypeName = u.stuTypeName,
                                    y_code = u.y_code
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/年度专业信息表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/年度专业信息表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"majorLibraryName", "专业姓名"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"y_code", "专业代码"}
                    };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        return Redirect(url);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('错误');window.location.href='" + reurl + "'</script>");
                }

            }
        }

        #region 专业教学计划  --弃用

        /// <summary>
        /// 专业教学计划页面--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult MajorTeachPlan()
        {
            #region “教学计划管理”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
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
            //using (var yunEntities = new IYunEntities())
            //{
            //    var yearStr = Request["year"];
            //    if (string.IsNullOrEmpty(yearStr))
            //    {
            //        yearStr = DateTime.Now.Year.ToString();
            //    }
            //    var year = Convert.ToInt32(yearStr);
            //    var majorId = Convert.ToInt32(Request["majorId"]);
            //    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            //    ViewBag.teachPlanList =
            //        yunEntities.VW_MajorTeachPlan.Where(u => u.y_majorId == majorId && u.y_year == year).ToList();
            //    ViewBag.major = yunEntities.VW_Major.FirstOrDefault(u => u.id == majorId);
            //    ViewData["year"] = year;
            //    ViewData["majorId"] = majorId;
            //    return View();
            //}

            using (var ad = new IYunEntities())
            {

                ViewBag.modulePowers = GetChildModulePower(ad, 2); //根据父栏目ID获取兄弟栏目

                #region 用于年度专业进入

                var yearStr = Request["year"];
                if (string.IsNullOrEmpty(yearStr))
                {
                    return Content("<script>alert('请选择年份')</script>");
                }
                var year = Convert.ToInt32(yearStr);
                var majorId = Convert.ToInt32(Request["majorId"]);
                var tachlist =
                    ad.VW_MajorTeachPlan.Where(u => u.y_majorId == majorId && u.y_year == year);
                if (tachlist == null)
                {
                    return Content("<script>alert('没有该年的教学计划')</script>");
                }
                var id = tachlist.Select(u => u.y_teachPlanId).ToList();
                var ids = string.Join(",", id);

                #endregion

                #region  用于教学计划进入

                //var teachPlanIdStr = Request["teachPlanId"];
                //var teachPlanId = Convert.ToInt32(teachPlanIdStr);
                //var teachplan =
                //    ad.VW_TeachPlanDes.Where(u => u.y_teaPlanId == teachPlanId).Select(u => u.y_teaPlanId).ToList();
                //var ids = string.Join(",", teachplan);

                #endregion

                //var ids = "394,395,396,397,398,399,400,401";
                var sql =
                    "select SUM(y_stuTime) as stuTime,sum(y_faceTime1) as faceTime1,sum(y_faceTime2) as faceTime2,sum(y_task)as task,cid as courseId ,MAX(y_name) as name ,y_term as team from(select * from[YD_Edu_TeachplanDes] where y_teaPlanId in (" +
                    ids +
                    ")) as d full join(select * from(select id as cid, y_name  from YD_Edu_Course where id in (SELECT[y_courseId]FROM [YD_Edu_TeachplanDes] where[y_teaPlanId] in (" +
                    ids + ")))   as c  cross join(select id as tid, y_term from[YD_Edu_TeachPlan] where id in (" + ids +
                    ") )as s) as k on(k.cid = d.y_courseId and d.y_teaPlanId = k.tid)   group by cid,y_term order by cid, y_term";
                var list = ad.Database.SqlQuery<TeachPlanNew>(sql).ToList();
                if (list.Count == 0)
                {
                    return Content("未找到数据");
                }
                return View("MajorTeachPlanNew", list);
            }
        }

        #endregion

        #region 教学计划课程（教学计划详情）-- 弃用

        /// <summary>
        /// 教学计划课程页面----弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachplanDes()
        {
            #region “教学计划课程”权限验证

            var power = SafePowerPage("/Edu/Teachplan");
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
                var list = yunEntities.VW_TeachPlanDes.Where(u => true).ToList();
                var teachPlanIdStr = Request["teachPlanId"];
                if (!string.IsNullOrEmpty(teachPlanIdStr))
                {
                    var teachPlanId = Convert.ToInt32(teachPlanIdStr);
                    list = list.Where(u => u.y_teaPlanId == teachPlanId).ToList();
                }
                ViewBag.entityList = list;
                try
                {
                    var teachPlanId = Convert.ToInt32(teachPlanIdStr);
                    ViewBag.teachPlanId = teachPlanIdStr;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    var teachPlan = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == teachPlanId);
                    if (teachPlan == null)
                    {
                        //var reurl = Request.UrlReferrer.ToString();
                        var reurl = "/AdminBase/Index";
                        return Content("<script>alert('未知错误');window.location.href='" + reurl + "'</script>");
                    }
                    ViewBag.teachPlan = teachPlan;
                }
                catch (Exception)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('未知错误');window.location.href='" + reurl + "'</script>");
                }
            }
            return View();
        }

        /// <summary>
        /// 添加师大教学计划课程页面--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesAddPage(int id)
        {
            #region “添加教学计划”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                if (string.IsNullOrEmpty(yearStr))
                {
                    yearStr = DateTime.Now.Year.ToString();
                }
                var year = Convert.ToInt32(Request["year"]);
                var y_term = Convert.ToInt32(Request["y_term"]);
                var majorId = Convert.ToInt32(Request["majorlihidden2"]);
                ViewData["year"] = year;
                ViewData["majorId"] = majorId;
                ViewData["teachPlanId"] = id;
                var teachPlan = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == id);
                if (teachPlan == null)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }
                ViewBag.teachPlan = teachPlan;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

            }
            return View();
        }

        /// <summary>
        /// 添加师大教学计划课程页面--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesAddPageBYsd()
        {
            #region “添加教学计划”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                if (string.IsNullOrEmpty(yearStr))
                {
                    yearStr = DateTime.Now.Year.ToString();
                }
                var year = Convert.ToInt32(Request["year"]);
                var y_term = Convert.ToInt32(Request["y_term"]);
                var majorId = Convert.ToInt32(Request["majorlihidden2"]);
                var teachplantype = 2;
                var teachplanid =
                    yunEntities.VW_MajorTeachPlan.FirstOrDefault(
                        u =>
                            u.y_teaPlanType == teachplantype && u.y_majorId == majorId && u.y_term == y_term &&
                            u.y_year == year);

                ViewData["year"] = year;
                ViewData["majorId"] = majorId;
                ViewData["teachPlanId"] = teachplanid;
                if (teachplanid != null)
                {
                    var teachPlan = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == teachplanid.id);
                    if (teachPlan == null)
                    {
                        var reurl = Request.UrlReferrer.ToString();
                        return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                    }
                    ViewBag.teachPlan = teachPlan;
                }

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

            }
            return View();
        }

        /// <summary>
        /// 编辑教学计划课程图
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesEditPage(int id)
        {
            #region “编辑教学计划”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_TeachplanDes.FirstOrDefault(u => u.y_teaPlanId == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑教学计划课程
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesEdit(YD_Edu_TeachplanDes role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_teachPlanDesDal.EditEntity(Request, role, yunEntities));
            }
        }

        /// <summary>
        /// 增加教学计划课程
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesAdd(YD_Edu_TeachplanDes modules)
        {
            using (var yunEntities = new IYunEntities())
            {
                var course = Request["coursehidden"];
                if (!string.IsNullOrWhiteSpace(course) && !course.Equals("0"))
                {
                    var courseid = Convert.ToInt32(course);
                    modules.y_courseId = courseid;
                    yunEntities.Entry(modules).State = EntityState.Modified;
                }
                //根据学校判断是哪个方法
                return Content(_teachPlanDesDal.CreateTeachPlanCourse(Request, yunEntities, modules));
            }
        }

        /// <summary>
        /// 删除教学计划课程
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDesDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_teachPlanDesDal.EntityDelete(id, yunEntities));


            }
        }

        #endregion

        #region 教学计划 --弃用

     

        public ActionResult TeachPlan()
        {
            return View();
        }

        /// <summary>
        /// 教学计划列表页面--弃用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TeachPlanA(int id = 1)
        {
            #region “教学计划管理”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                var yMajorIdStr = Request["Major"];
                var yTermStr = Request["y_term"];
                //根据学校显示对应的教学计划页面
                IQueryable<VW_MajorTeachPlan> list = yunEntities.VW_MajorTeachPlan.OrderByDescending(u => u.id);

                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId==9)
                {
                     var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    var stuInfos =
                        yunEntities.VW_StuInfo.Where(u =>u.y_subSchoolId.HasValue&& subSchoolIds.Contains(u.y_subSchoolId.Value))
                            .DistinctBy(u => new {u.y_majorId, u.y_subSchoolId})
                            .ToList();
                    var stuInfoids = new List<int>();
                    for (var i = 0; i < stuInfos.Count; i++)
                    {
                        stuInfoids.Add(stuInfos[i].y_majorId);
                    }
                    list = list.Where(u => stuInfoids.Contains(u.y_majorId));
                }
                if (!string.IsNullOrWhiteSpace(yearStr))
                {
                    int year = Convert.ToInt32(yearStr);
                    list = list.Where(u => u.y_year == year);
                }

                if (!string.IsNullOrWhiteSpace(yTermStr) && !yTermStr.Equals("0"))
                {
                    var y_term = Convert.ToInt32(yTermStr);
                    list = list.Where(u => u.y_term == y_term);
                }

                if (!string.IsNullOrWhiteSpace(yMajorIdStr) && !yMajorIdStr.Equals("0"))
                {
                    var y_majorId = Convert.ToInt32(yMajorIdStr);
                    list = list.Where(u => u.y_majorId == y_majorId);
                }
                var adminList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("TeachPlanList", adminList);
                return View(adminList);
            }
        }

        /// <summary>
        /// 师范大学的教学计划--弃用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TeachPlanBYsd(int id = 1)
        {
            #region “教学计划管理”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                var y_majorIdStr = Request["Major"];
                var y_termStr = Request["y_term"];
                //根据学校显示对应的教学计划页面
                IQueryable<VW_MajorTeachPlanDes> list = yunEntities.VW_MajorTeachPlanDes.OrderByDescending(u => u.id);
                 if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId==9)
                {
                   var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    var stuInfos =
                        yunEntities.VW_StuInfo.Where(u => u.y_subSchoolId.HasValue&& subSchoolIds.Contains(u.y_subSchoolId.Value))
                            .DistinctBy(u => new {u.y_majorId, u.y_subSchoolId})
                            .ToList();
                    var stuInfoids = new List<int>();
                    for (var i = 0; i < stuInfos.Count; i++)
                    {
                        stuInfoids.Add(stuInfos[i].y_majorId);
                    }
                    list = list.Where(u => stuInfoids.Contains(u.y_majorId));
                }
                if (!string.IsNullOrWhiteSpace(yearStr) && !yearStr.Equals("0"))
                {
                    int year = Convert.ToInt32(yearStr);
                    list = list.Where(u => u.y_year == year);
                }
                if (!string.IsNullOrWhiteSpace(y_termStr) && !y_termStr.Equals("0"))
                {
                    var y_term = Convert.ToInt32(y_termStr);
                    list = list.Where(u => u.y_term == y_term);
                }
                if (!string.IsNullOrWhiteSpace(y_majorIdStr) && !y_majorIdStr.Equals("0"))
                {
                    var y_majorId = Convert.ToInt32(y_majorIdStr);
                    list = list.Where(u => u.y_majorId == y_majorId);
                }
                var adminList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("TeachPlanListBYsd", adminList);
                return View(adminList);
            }
        }

        /// <summary>
        /// 教学计划下载--弃用
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult DownloadTeachPlanBYsd()
        {
            //#region 权限验证

            //var power = SafePowerPage("/Edu/TeachPlan");
            //if (!IsLogin())
            //{
            //    Redirect("/AdminBase/Index");
            //}
            //if (power == null || power.y_select == (int) PowerState.Disable)
            //{
            //    var reurl = Request.UrlReferrer.ToString();
            //    return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            //}

            //#endregion

            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                var y_majorIdStr = Request["y_majorId"];
                var y_termStr = Request["y_term"];
                IQueryable<VW_MajorTeachPlanDes> list = yunEntities.VW_MajorTeachPlanDes.OrderByDescending(u => u.y_name);
                if (!string.IsNullOrWhiteSpace(yearStr))
                {
                    int year = Convert.ToInt32(yearStr);
                    list = list.Where(u => u.y_year == year);
                }
                if (!string.IsNullOrWhiteSpace(y_termStr) && !y_termStr.Equals("0"))
                {
                    var y_term = Convert.ToInt32(y_termStr);
                    list = list.Where(u => u.y_term == y_term);
                }

                if (!string.IsNullOrWhiteSpace(y_majorIdStr) && !y_majorIdStr.Equals("0"))
                {
                    var y_majorId = Convert.ToInt32(y_majorIdStr);
                    list = list.Where(u => u.y_majorId == y_majorId);
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    y_year=u.y_year,
                                    y_majorid=u.y_majorId,
                                    y_name=u.y_name,
                                    y_teaPlanId=u.y_teaPlanId,
                                    y_teaPlanName=u.y_teaPlanName,
                                    y_term=u.y_term,
                                    y_courseId=u.y_courseId,
                                    y_coursename=u.y_coursename,
                                    y_majorLibId=u.y_majorLibId,
                                    majorLibraryName=u.majorLibraryName,
                                    y_stuTypeId=u.y_stuTypeId,
                                    stuTypeName=u.stuTypeName,
                                    y_eduTypeId=u.y_eduTypeId,
                                    eduTypeName=u.eduTypeName,
                                    y_stuYear=u.y_stuYear
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/历届教学计划表" +".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/教学计划表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"CourseName", "课程"},
                        {"courseTypeName", "课程类型"},
                        {"y_term", "学期"},
                        {"y_stuTime", "学习时间"},
                        {"y_score", "及格分数"}
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
        /// 添加教学计划页面--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanAddPage()
        {
            #region “添加教学计划”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var yearStr = Request["year"];
                if (string.IsNullOrEmpty(yearStr))
                {
                    yearStr = DateTime.Now.Year.ToString();
                }
                try
                {
                    var year = Convert.ToInt32(yearStr);
                    var majorId = Convert.ToInt32(Request["majorId"]);
                    var major = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == majorId);
                    if (major == null)
                    {
                        var reurl = Request.UrlReferrer.ToString();
                        return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                    }
                    ViewData["year"] = year;
                    ViewData["majorId"] = majorId;
                    ViewData["major"] = major;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                }
                catch (Exception)
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误');window.location.href='" + reurl + "'</script>");
                }

            }
            return View();
        }

        /// <summary>
        /// 编辑教学计划视图--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanEditPage(int id)
        {
            #region “编辑教学计划”权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑教学计划--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanEdit(YD_Edu_TeachPlan role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_teachPlanDal.EditEntity(Request, role, yunEntities));
            }
        }

        /// <summary>
        /// 增加教学计划--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanAdd(YD_Edu_TeachPlan role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_teachPlanDal.AddEntity(Request, role, yunEntities));
            }
        }

        /// <summary>
        /// 删除教学计划--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult TeachPlanDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_teachPlanDal.EntityDelete(id, yunEntities));
            }
        }

        /// <summary>
        /// 批量导入页面--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadTeachPlan()
        {
            #region 权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            var fileName = Request["filename"];
            if (fileName.IndexOf(".xlsx") < 0 && fileName.IndexOf(".xls") < 0)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('该文件不是正确的Excel文件');window.location.href='" + reurl + "'</script>");
            }
            fileName = Server.MapPath(fileName);
            var teachPlanTempList = new List<YD_Edu_TeachPlanTemp>();
            var stuTypeHash = new Hashtable();
            var eduTypeHash = new Hashtable();
            var majorLibHash = new Hashtable();
            var majorHash = new Hashtable();
            var courseHash = new Hashtable();
            var courseTypeHash = new Hashtable();
            using (var excelHelper = new ExcelHelper(fileName))
            {
                var dt = excelHelper.ExcelToDataTable("", true);
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.Database.ExecuteSqlCommand("DELETE FROM YD_Edu_TeachPlanTemp");
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                    int year = 0;
                    int majorId = 0;
                    int majorLibId = 0;
                    string major = "";
                    string majorLib = "";
                    var majorNameMatch = new StringBuilder();
                    int stuTypeId = 0;
                    string stuTypeName = "";
                    int eduTypeId = 0;
                    string eduTypeName = "";
                    int term = 0;
                    int courseId = 0;
                    string courseName = "";
                    int courseTypeId = 0;
                    string courseType = "";
                    var courseNameMatch = new StringBuilder();
                    int stuTime = 0;
                    int teachPlanType = 0;
                    decimal score = 0;
                    int isOk = (int) YesOrNo.No;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        year = 0;
                        majorId = 0;
                        majorLibId = 0;
                        majorLib = "";
                        major = "";
                        //majorNameMatch = "";
                        stuTypeId = 0;
                        stuTypeName = "";
                        eduTypeId = 0;
                        eduTypeName = "";
                        term = 0;
                        courseId = 0;
                        courseName = "";
                        //courseNameMatch = "";
                        courseTypeId = 0;
                        courseType = "";
                        stuTime = 0;
                        teachPlanType = 0;
                        score = 0;
                        isOk = (int) YesOrNo.No;

                        #region 获取学习形式id

                        if (dt.Rows[i]["学习形式"] != null)
                        {
                            stuTypeName = dt.Rows[i]["学习形式"].ToString();
                            if (!stuTypeHash.ContainsKey(stuTypeName))
                            {
                                var entity = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.y_name == stuTypeName);
                                if (entity != null)
                                {
                                    stuTypeHash.Add(entity.y_name, entity.id);
                                    stuTypeId = entity.id;
                                }
                            }
                            else
                            {
                                stuTypeId = Convert.ToInt16(stuTypeHash[stuTypeName]);
                            }
                        }

                        #endregion

                        #region 获取层次id

                        if (dt.Rows[i]["层次"] != null)
                        {
                            eduTypeName = dt.Rows[i]["层次"].ToString();
                            if (!eduTypeHash.ContainsKey(eduTypeName))
                            {
                                var entity = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.y_name == eduTypeName);
                                if (entity != null)
                                {
                                    eduTypeHash.Add(eduTypeName, entity.id);
                                    eduTypeId = entity.id;
                                }
                            }
                            else
                            {
                                eduTypeId = Convert.ToInt16(eduTypeHash[eduTypeName]);
                            }
                        }

                        #endregion

                        #region 获取专业库id.

                        if (dt.Rows[i]["专业名"] != null)
                        {
                            majorLib = dt.Rows[i]["专业名"].ToString().Trim().Replace(" ", "");
                            if (!majorLibHash.ContainsKey(majorLib))
                            {
                                var entity =
                                    yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(
                                        u => u.y_name == majorLib);
                                if (entity == null)
                                {
                                    var name = majorLib.Replace("专业", "");
                                    if (name.Length <= 2)
                                    {
                                        var entityList =
                                            yunEntities.YD_Edu_MajorLibrary.Where(u => u.y_name.Contains(name));
                                        var objList = entityList.ToList();
                                        for (var j = 0; j < objList.Count; j++)
                                        {
                                            //如果长度大于450则不继续拼接
                                            if (majorNameMatch.Length > 450)
                                            {
                                                break;
                                            }
                                            majorNameMatch.Append(objList[j].y_name);
                                            if (j + 1 < objList.Count)
                                            {
                                                majorNameMatch.Append(",");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var sql = new StringBuilder();
                                        for (var j = 0; j < name.Length - 1; j++)
                                        {
                                            sql.Append(" y_name like '%").Append(name.Substring(j, 2) + "%' ");
                                            if (j + 1 < name.Length - 1)
                                            {
                                                sql.Append(" or ");
                                            }
                                        }
                                        var entityList =
                                            yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
                                                "select * from YD_Edu_MajorLibrary where " + sql.ToString()).ToList();
                                        var objList = entityList.ToList();
                                        for (var j = 0; j < objList.Count; j++)
                                        {
                                            //如果长度大于450则不继续拼接
                                            if (majorNameMatch.Length > 450)
                                            {
                                                break;
                                            }
                                            majorNameMatch.Append(objList[j].y_name);
                                            if (j + 1 < objList.Count)
                                            {
                                                majorNameMatch.Append(",");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    majorLibHash.Add(majorLib, entity.id);
                                    majorLibId = entity.id;
                                }
                            }
                            else
                            {
                                majorLibId = Convert.ToInt16(majorLibHash[majorLib]);
                            }

                        }

                        #endregion

                        #region 获取专业id

                        if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0)
                        {
                            var majorKey = stuTypeId + "," + eduTypeId + "," + majorLibId;
                            if (!majorHash.ContainsKey(majorKey))
                            {
                                majorId = GetMajorIds(majorLibId, eduTypeId, stuTypeId);
                                majorHash.Add(majorKey, majorId);
                            }
                            else
                            {
                                majorId = Convert.ToInt16(majorHash[majorKey]);
                            }
                        }

                        #endregion

                        #region 获取课程id

                        if (dt.Rows[i]["课程名"] != null)
                        {
                            courseName = dt.Rows[i]["课程名"].ToString().Trim().Replace(" ", "");
                            if (!courseHash.ContainsKey(courseName))
                            {
                                var entity = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == courseName);
                                if (entity == null)
                                {
                                    var name = courseName;
                                    if (name.Length <= 2)
                                    {
                                        var entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name));
                                        var objList = entityList.ToList();
                                        for (var j = 0; j < objList.Count; j++)
                                        {
                                            //如果长度大于450则不继续拼接
                                            if (courseNameMatch.Length > 450)
                                            {
                                                break;
                                            }
                                            courseNameMatch.Append(objList[j].y_name);
                                            if (j + 1 < objList.Count)
                                            {
                                                courseNameMatch.Append(",");
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
                                            yunEntities.Database.SqlQuery<YD_Edu_Course>(
                                                "select * from YD_Edu_Course where " + sql).ToList();
                                        var objList = entityList.ToList();
                                        for (var j = 0; j < objList.Count; j++)
                                        {
                                            //如果长度大于450则不继续拼接
                                            if (courseNameMatch.Length > 450)
                                            {
                                                break;
                                            }
                                            courseNameMatch.Append(objList[j].y_name);
                                            if (j + 1 < objList.Count)
                                            {
                                                courseNameMatch.Append(",");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    courseHash.Add(courseName, entity.id);
                                    courseId = entity.id;
                                }
                            }
                            else
                            {
                                courseId = Convert.ToInt16(courseHash[courseName]);
                            }
                        }

                        #endregion

                        #region 获取课程类型id

                        if (dt.Rows[i]["课程类型"] != null)
                        {
                            courseType = dt.Rows[i]["课程类型"].ToString();
                            if (!courseTypeHash.ContainsKey(courseType))
                            {
                                var entity = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.y_name == courseType);
                                if (entity != null)
                                {
                                    courseTypeHash.Add(courseType, entity.id);
                                }
                            }
                            else
                            {
                                courseTypeId = Convert.ToInt16(courseTypeHash[courseType]);
                            }
                        }

                        #endregion

                        #region 学期

                        if (dt.Rows[i]["学期"] != null)
                        {
                            int.TryParse(dt.Rows[i]["学期"].ToString(), out term);
                        }

                        #endregion

                        #region 及格分数

                        if (dt.Rows[i]["及格分数"] != null)
                        {
                            decimal.TryParse(dt.Rows[i]["及格分数"].ToString(), out score);
                        }

                        #endregion

                        #region 学时

                        if (dt.Rows[i]["学时"] != null)
                        {
                            int.TryParse(dt.Rows[i]["学时"].ToString(), out stuTime);
                        }

                        #endregion

                        #region 入学年份

                        if (dt.Rows[i]["入学年份"] != null)
                        {
                            int.TryParse(dt.Rows[i]["入学年份"].ToString(), out year);
                        }

                        #endregion

                        #region 教学计划类型

                        if (dt.Rows[i]["教学计划类型"] != null)
                        {
                            int.TryParse(dt.Rows[i]["教学计划类型"].ToString(), out teachPlanType);
                        }

                        #endregion

                        if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0 && majorId != 0 && courseId != 0 &&
                            courseTypeId != 0 && term != 0 && year != 0 && teachPlanType != 0)
                        {
                            isOk = (int) YesOrNo.Yes;
                        }
                        var teachPlanTemp = new YD_Edu_TeachPlanTemp()
                        {
                            y_year = year,
                            y_majorId = majorId,
                            y_majorLibId = majorLibId,
                            y_majorLib = majorLib,
                            y_major = major,
                            y_stuTypeId = stuTypeId,
                            y_stuTypeName = stuTypeName,
                            y_eduTypeId = eduTypeId,
                            y_eduTypeName = eduTypeName,
                            y_courseId = courseId,
                            y_term = term,
                            y_courseName = courseName,
                            y_courseTypeId = courseTypeId,
                            y_courseType = courseType,
                            y_stuTime = stuTime,
                            y_teachPlanType = teachPlanType,
                            y_score = score,
                            y_isok = isOk,
                            y_majorNameMatch = majorNameMatch.ToString(),
                            y_courseNameMatch = courseNameMatch.ToString()
                        };
                        yunEntities.Entry(teachPlanTemp).State = EntityState.Added;
                    }
                    yunEntities.Configuration.AutoDetectChangesEnabled = false;
                    yunEntities.Configuration.ValidateOnSaveEnabled = false;
                    yunEntities.Set<YD_Edu_TeachPlanTemp>().AddRange(teachPlanTempList);
                    yunEntities.SaveChanges();
                    //yunEntities.SaveChanges();
                    ViewBag.teachPlanList =
                        yunEntities.YD_Edu_TeachPlanTemp.Where(u => true).OrderBy(u => u.id).ToList();
                    return View();
                }
            }
        }

        /// <summary>
        /// 验证导入的临时教学计划--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyTeachPlan(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Edu/TeachPlan");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int) PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int) YesOrNo.No;
                //如果数据库存在导入的教学计划则不导入
                //var alikemajors = yunEntities.YD_Edu_TeachPlanTemp;
                var templist = yunEntities.YD_Edu_TeachPlanTemp.ToList();
                var list2 = yunEntities.VW_MajorTeachPlanDes.AsQueryable();

                foreach (var item in templist)
                {
                    if (list2.Any(u => u.y_majorLibId == item.y_majorLibId
                                       && u.stuTypeName == item.y_stuTypeName
                                       && u.eduTypeName == item.y_eduTypeName
                                       && u.y_coursename.Equals(item.y_courseName)
                                       && u.y_term == item.y_term
                                       && u.y_year == item.y_year))
                    {
                        yunEntities.YD_Edu_TeachPlanTemp.Remove(item);
                    }
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return
                        Content("<script type='text/javascript'>alert('有重复教学计划不导入，重复条数为" + r +
                                "');window.location.href='/Edu/VerifyTeachPlan';</script >");
                }
                var list = yunEntities.YD_Edu_TeachPlanTemp.Where(u => u.y_isok == isOk).ToList();
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.teachPlanList = yunEntities.YD_Edu_TeachPlanTemp.Where(u => u.y_isok == isOk).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("VerifyTeachPlanList", dbLogList);
                return View(dbLogList);
            }
        }


        /// <summary>
        /// 提交用户更新的临时数据（教学计划）--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateVerify()
        {
            var majorLibs = Request["majorLib"].Split(new[] {"<>"}, StringSplitOptions.None);
            var stuTypeNames = Request["stuTypeName"].Split(new[] {"<>"}, StringSplitOptions.None);
            var eduTypeNames = Request["eduTypeName"].Split(new[] {"<>"}, StringSplitOptions.None);
            var courseNames = Request["courseName"].Split(new[] {"<>"}, StringSplitOptions.None);
            var courseTypes = Request["courseType"].Split(new[] {"<>"}, StringSplitOptions.None);
            var terms = Request["term"].Split(new[] {"<>"}, StringSplitOptions.None);
            var stuTimes = Request["stuTime"].Split(new[] {"<>"}, StringSplitOptions.None);
            var scores = Request["score"].Split(new[] {"<>"}, StringSplitOptions.None);
            var years = Request["year"].Split(new[] {"<>"}, StringSplitOptions.None);
            var teachPlanTypes = Request["teachPlanType"].Split(new[] {"<>"}, StringSplitOptions.None);
            var ids = Request["id"].Split(new[] {"<>"}, StringSplitOptions.None);

            int id = 0;
            int year = 0;
            int majorId = 0;
            int majorLibId = 0;
            string major = "";
            string majorLib = "";
            string majorNameMatch = "";
            int stuTypeId = 0;
            string stuTypeName = "";
            int eduTypeId = 0;
            string eduTypeName = "";
            int term = 0;
            int courseId = 0;
            string courseName = "";
            string courseNameMatch = "";
            int courseTypeId = 0;
            string courseType = "";
            int stuTime = 0;
            int teachPlanType = 0;
            decimal score = 0;
            int isOk = (int) YesOrNo.No;
            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < ids.Count(); i++)
                {
                    majorLibId = 0;
                    major = "";
                    majorNameMatch = "";
                    stuTypeId = 0;
                    eduTypeId = 0;
                    courseId = 0;
                    courseTypeId = 0;
                    courseNameMatch = "";

                    id = Convert.ToInt32(ids[i]);
                    int.TryParse(terms[i], out term);
                    var teachPlan = yunEntities.YD_Edu_TeachPlanTemp.FirstOrDefault(u => u.id == id);
                    if (teachPlan != null)
                    {
                        #region 获取学习形式id

                        stuTypeName = stuTypeNames[i];
                        var entity1 = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.y_name == stuTypeName);
                        if (entity1 != null)
                            stuTypeId = entity1.id;

                        #endregion

                        #region 获取层次id

                        eduTypeName = eduTypeNames[i];
                        var entity2 = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.y_name == eduTypeName);
                        if (entity2 != null)
                            eduTypeId = entity2.id;

                        #endregion

                        #region 获取专业库id

                        majorLib = majorLibs[i];
                        var entity3 = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == majorLib);
                        if (entity3 == null)
                        {
                            var name = majorLib.Replace("专业", "");
                            if (name.Length <= 2)
                            {
                                var entityList = yunEntities.YD_Edu_MajorLibrary.Where(u => u.y_name.Contains(name));
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    if (majorNameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    majorNameMatch += objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        majorNameMatch += ",";
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
                                    yunEntities.Database.SqlQuery<YD_Edu_MajorLibrary>(
                                        "select * from YD_Edu_MajorLibrary where " + sql).ToList();
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    if (majorNameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    majorNameMatch += objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        majorNameMatch += ",";
                                    }
                                }
                            }
                        }
                        else
                        {
                            majorLibId = entity3.id;
                        }

                        #endregion

                        #region 获取专业id

                        if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0)
                        {
                            majorId = GetMajorIds(majorLibId, eduTypeId, stuTypeId);
                        }
                        else
                        {
                            majorId = 0;
                        }

                        #endregion

                        #region 获取课程id

                        courseName = courseNames[i].Replace(" ", "");
                        var entity4 = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == courseName);
                        if (entity4 == null)
                        {
                            var name = courseName;
                            if (name.Length <= 2)
                            {
                                var entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name));
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    if (courseNameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    courseNameMatch += objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        courseNameMatch += ",";
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
                                    yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " +
                                                                                 sql).ToList();
                                var objList = entityList.ToList();
                                for (var j = 0; j < objList.Count; j++)
                                {
                                    if (courseNameMatch.Length > 450)
                                    {
                                        break;
                                    }
                                    courseNameMatch += objList[j].y_name;
                                    if (j + 1 < objList.Count)
                                    {
                                        courseNameMatch += ",";
                                    }
                                }
                            }
                        }
                        else
                        {
                            courseId = entity4.id;
                        }

                        #endregion

                        #region 获取课程类型id

                        courseType = courseTypes[i];
                        var entity = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.y_name == courseType);
                        if (entity != null)
                            courseTypeId = entity.id;

                        #endregion

                        int.TryParse(terms[i], out term);

                        decimal.TryParse(scores[i], out score);

                        int.TryParse(stuTimes[i], out stuTime);

                        int.TryParse(years[i], out year);

                        int.TryParse(teachPlanTypes[i], out teachPlanType);

                        if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0 && majorId != 0 && courseId != 0 &&
                            courseTypeId != 0 && term != 0 && year != 0 && teachPlanType != 0)
                        {
                            isOk = (int) YesOrNo.Yes;
                        }
                        else
                        {
                            isOk = (int) YesOrNo.No;
                        }

                        teachPlan.y_year = year;
                        teachPlan.y_majorId = majorId;
                        teachPlan.y_majorLibId = majorLibId;
                        teachPlan.y_majorLib = majorLib;
                        teachPlan.y_major = major;
                        teachPlan.y_stuTypeId = stuTypeId;
                        teachPlan.y_stuTypeName = stuTypeName;
                        teachPlan.y_eduTypeId = eduTypeId;
                        teachPlan.y_eduTypeName = eduTypeName;
                        teachPlan.y_courseId = courseId;
                        teachPlan.y_term = term;
                        teachPlan.y_courseName = courseName;
                        teachPlan.y_courseTypeId = courseTypeId;
                        teachPlan.y_courseType = courseType;
                        teachPlan.y_stuTime = stuTime;
                        teachPlan.y_teachPlanType = teachPlanType;
                        teachPlan.y_score = score;
                        teachPlan.y_isok = isOk;
                        teachPlan.y_majorNameMatch = majorNameMatch;
                        teachPlan.y_courseNameMatch = courseNameMatch;
                        yunEntities.Entry(teachPlan).State = EntityState.Modified;
                    }
                }
                var t = yunEntities.SaveChanges();
                return Content("ok");
            }
        }

        ///// <summary>
        ///// 提交函授站更新的临时数据（教学计划）
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UpdateSubVerify()
        //{
        //    var subSchools = Request["majorLib"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var majorLibs = Request["majorLib"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var stuTypeNames = Request["stuTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var eduTypeNames = Request["eduTypeName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var courseNames = Request["courseName"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var courseTypes = Request["courseType"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var terms = Request["term"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var stuTimes = Request["stuTime"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var scores = Request["score"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var years = Request["year"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var teachPlanTypes = Request["teachPlanType"].Split(new[] { "<>" }, StringSplitOptions.None);
        //    var ids = Request["id"].Split(new[] { "<>" }, StringSplitOptions.None);

        //    int id = 0;
        //    int year = 0;
        //    int subSchoolId = 0;
        //    string subSchoolName = "";
        //    string subNameMatch = "";
        //    int majorId = 0;
        //    int majorLibId = 0;
        //    string major = "";
        //    string majorLib = "";
        //    string majorNameMatch = "";
        //    int stuTypeId = 0;
        //    string stuTypeName = "";
        //    int eduTypeId = 0;
        //    string eduTypeName = "";
        //    int term = 0;
        //    int courseId = 0;
        //    string courseName = "";
        //    string courseNameMatch = "";
        //    int courseTypeId = 0;
        //    string courseType = "";
        //    int stuTime = 0;
        //    int teachPlanType = 0;
        //    decimal score = 0;
        //    int isOk = (int)YesOrNo.No;
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        for (var i = 0; i < ids.Count(); i++)
        //        {
        //            majorLibId = 0;
        //            major = "";
        //            majorNameMatch = "";
        //            stuTypeId = 0;
        //            eduTypeId = 0;
        //            courseId = 0;
        //            courseTypeId = 0;
        //            courseNameMatch = "";

        //            id = Convert.ToInt32(ids[i]);
        //            int.TryParse(terms[i], out term);
        //            var teachPlan = yunEntities.YD_Edu_SubTeachPlanTemp.FirstOrDefault(u => u.id == id);
        //            if (teachPlan != null)
        //            {
        //                #region 获取学习形式id
        //                stuTypeName = stuTypeNames[i];
        //                var entity1 = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.y_name == stuTypeName);
        //                if (entity1 != null)
        //                    stuTypeId = entity1.id;
        //                #endregion

        //                #region 获取层次id
        //                eduTypeName = eduTypeNames[i];
        //                var entity2 = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.y_name == eduTypeName);
        //                if (entity2 != null)
        //                    eduTypeId = entity2.id;
        //                #endregion
        //                #region 获取函授站id

        //                subSchoolName = subSchools[i].Trim().Replace(" ", "");
        //                var entity5 = yunEntities.YD_Sys_SubSchool.FirstOrDefault(u => u.y_name == subSchoolName);
        //                if (entity5 == null)
        //                {
        //                    var myname = subSchoolName.Replace("函授站", "").Replace("函授", "");

        //                    if (myname.Length <= 2)
        //                    {
        //                        var entityList = yunEntities.YD_Sys_SubSchool.Where(u => u.y_name.Contains(myname));
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
        //                            yunEntities.Database.SqlQuery<YD_Sys_SubSchool>(
        //                                "select * from YD_Sys_SubSchool where " + sql).ToList();
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

        //                #region 获取专业库id
        //                majorLib = majorLibs[i];
        //                var entity3 = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == majorLib);
        //                if (entity3 == null)
        //                {
        //                    var name = majorLib.Replace("专业", "");
        //                    if (name.Length <= 2)
        //                    {
        //                        var entityList = yunEntities.YD_Edu_MajorLibrary.Where(u => u.y_name.Contains(name));
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            if (majorNameMatch.Length > 450)
        //                            {
        //                                break;
        //                            }
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
        //                        for (var j = 0; j < name.Length - 1; j++)
        //                        {
        //                            sql += " y_name like '%" + name.Substring(j, 2) + "%' ";
        //                            if (j + 1 < name.Length - 1)
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
        //                            if (majorNameMatch.Length > 450)
        //                            {
        //                                break;
        //                            }
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

        //                #region 获取专业id
        //                if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0)
        //                {
        //                    majorId = GetMajorId(majorLibId, eduTypeId, stuTypeId);
        //                }
        //                else
        //                {
        //                    majorId = 0;
        //                }
        //                #endregion

        //                #region 获取课程id
        //                courseName = courseNames[i].Replace(" ", "");
        //                var entity4 = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == courseName);
        //                if (entity4 == null)
        //                {
        //                    var name = courseName;
        //                    if (name.Length <= 2)
        //                    {
        //                        var entityList = yunEntities.YD_Edu_Course.Where(u => u.y_name.Contains(name));
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            if (courseNameMatch.Length > 450)
        //                            {
        //                                break;
        //                            }
        //                            courseNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                courseNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var sql = "";
        //                        for (var j = 0; j < name.Length - 1; j++)
        //                        {
        //                            sql += " y_name like '%" + name.Substring(j, 2) + "%' ";
        //                            if (j + 1 < name.Length - 1)
        //                            {
        //                                sql += " or ";
        //                            }
        //                        }
        //                        var entityList =
        //                            yunEntities.Database.SqlQuery<YD_Edu_Course>("select * from YD_Edu_Course where " + sql).ToList();
        //                        var objList = entityList.ToList();
        //                        for (var j = 0; j < objList.Count; j++)
        //                        {
        //                            if (courseNameMatch.Length > 450)
        //                            {
        //                                break;
        //                            }
        //                            courseNameMatch += objList[j].y_name;
        //                            if (j + 1 < objList.Count)
        //                            {
        //                                courseNameMatch += ",";
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    courseId = entity4.id;
        //                }

        //                #endregion

        //                #region 获取课程类型id
        //                courseType = courseTypes[i];
        //                var entity = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.y_name == courseType);
        //                if (entity != null)
        //                    courseTypeId = entity.id;
        //                #endregion

        //                int.TryParse(terms[i], out term);

        //                decimal.TryParse(scores[i], out score);

        //                int.TryParse(stuTimes[i], out stuTime);

        //                int.TryParse(years[i], out year);

        //                int.TryParse(teachPlanTypes[i], out teachPlanType);

        //                if (stuTypeId != 0 && eduTypeId != 0 && majorLibId != 0 && majorId != 0 && courseId != 0 && courseTypeId != 0 && term != 0 && year != 0 && teachPlanType != 0)
        //                {
        //                    isOk = (int)YesOrNo.Yes;
        //                }
        //                else
        //                {
        //                    isOk = (int)YesOrNo.No;
        //                }

        //                teachPlan.y_year = year;
        //                teachPlan.y_subSchoolId = subSchoolId;
        //                teachPlan.y_subSchoolName = subSchoolName;
        //                teachPlan.y_majorId = majorId;
        //                teachPlan.y_majorLibId = majorLibId;
        //                teachPlan.y_majorLib = majorLib;
        //                teachPlan.y_major = major;
        //                teachPlan.y_stuTypeId = stuTypeId;
        //                teachPlan.y_stuTypeName = stuTypeName;
        //                teachPlan.y_eduTypeId = eduTypeId;
        //                teachPlan.y_eduTypeName = eduTypeName;
        //                teachPlan.y_courseId = courseId;
        //                teachPlan.y_term = term;
        //                teachPlan.y_courseName = courseName;
        //                teachPlan.y_courseTypeId = courseTypeId;
        //                teachPlan.y_courseType = courseType;
        //                teachPlan.y_stuTime = stuTime;
        //                teachPlan.y_teachPlanType = teachPlanType;
        //                teachPlan.y_score = score;
        //                teachPlan.y_isok = isOk;
        //                teachPlan.y_subNameMatch = subNameMatch;
        //                teachPlan.y_majorNameMatch = majorNameMatch;
        //                teachPlan.y_courseNameMatch = courseNameMatch;
        //                yunEntities.Entry(teachPlan).State = EntityState.Modified;
        //            }
        //        }
        //        var t = yunEntities.SaveChanges();
        //        return Content("ok");
        //    }
        //}
        ///// <summary>
        ///// 将验证无误的数据进行导入
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UplodTrueSubTeachPlan()
        //{
        //    using (var yunEntities = new IYunEntities())
        //    {
        //        const int isOk = (int)YesOrNo.Yes;
        //        var scoreList = yunEntities.YD_Edu_SubTeachPlanTemp.Where(u => u.y_isok == isOk).OrderBy(u => u.y_subSchoolId).ThenBy(u => u.y_term).ThenBy(u => u.y_teachPlanType).ThenBy(u => u.y_year).ThenBy(u => u.id).ToList();
        //        //同样，首先生成教学计划对象，写死教学计划名。
        //        //在插入教学计划详情表时，根据约定写死的教学计划名得到教学计划id
        //        //同样的在插入函授站教学计划表时，根据约定写死的教学计划名得到教学计划id
        //        var teachPlanName1 = "";
        //        var teachPlanName2 = "";
        //        var teachPlans = new List<YD_Edu_TeachPlan>();
        //        for (var i = 0; i < scoreList.Count; i++)
        //        {
        //            teachPlanName2 = scoreList[i].y_majorLib + scoreList[i].y_eduTypeName +
        //                              scoreList[i].y_stuTypeName + "第" + scoreList[i].y_term + "学期" +
        //                              scoreList[i].y_teachPlanType + "类型" + scoreList[i].y_year + "年";
        //            if (teachPlanName1 == teachPlanName2) continue;
        //            teachPlanName1 = teachPlanName2;
        //            var teachPlan = new YD_Edu_TeachPlan
        //            {
        //                y_teaPlanName = teachPlanName2 + DateTime.Now.ToShortDateString(),
        //                y_term = Convert.ToInt32(scoreList[i].y_term)
        //            };
        //            teachPlans.Add(teachPlan);
        //            //yunEntities.Entry(teachPlan).State = EntityState.Added;
        //        }
        //        yunEntities.Configuration.AutoDetectChangesEnabled = false;
        //        yunEntities.Configuration.ValidateOnSaveEnabled = false;
        //        yunEntities.Set<YD_Edu_TeachPlan>().AddRange(teachPlans);
        //        yunEntities.SaveChanges();
        //        //yunEntities.SaveChanges();
        //        var teachPlanName3 = "";
        //        var teachPlanName4 = "";
        //        var subTeachPlans = new List<YD_Edu_SubTeachPlan>();
        //        var majorTeachPlanDess = new List<YD_Edu_TeachplanDes>();
        //        var teachPlanLib = yunEntities.YD_Edu_TeachPlan.ToList();
        //        for (var i = 0; i < scoreList.Count; i++)
        //        {
        //            teachPlanName3 = scoreList[i].y_majorLib + scoreList[i].y_eduTypeName +
        //                              scoreList[i].y_stuTypeName + "第" + scoreList[i].y_term + "学期" +
        //                              scoreList[i].y_teachPlanType + "类型" + scoreList[i].y_year + "年"
        //                              + DateTime.Now.ToShortDateString();
        //            var entity = teachPlanLib.FirstOrDefault(u => u.y_teaPlanName == teachPlanName3);
        //            if (entity == null) continue;
        //            var teachPlanDes = new YD_Edu_TeachplanDes
        //            {
        //                y_teaPlanId = entity.id,
        //                y_courseId = Convert.ToInt32(scoreList[i].y_courseId),
        //                y_courseTypeId = Convert.ToInt32(scoreList[i].y_courseTypeId),
        //                y_score = Convert.ToDecimal(scoreList[i].y_score),
        //                y_stuTime = Convert.ToInt32(scoreList[i].y_stuTime)
        //            };
        //            if (teachPlanName4 != teachPlanName3)
        //            {
        //                var subTeachPlan = new YD_Edu_SubTeachPlan
        //                {
        //                    y_subSchoolId=Convert.ToInt32(scoreList[i].y_subSchoolId),
        //                    y_majorId = Convert.ToInt32(scoreList[i].y_majorId),
        //                    y_teachPlanId = entity.id,
        //                    y_year = Convert.ToInt32(scoreList[i].y_year),
        //                    y_teaPlanType = Convert.ToInt32(scoreList[i].y_teachPlanType),
        //                };
        //                subTeachPlans.Add(subTeachPlan);
        //                //yunEntities.Entry(majorTeachPlan).State = EntityState.Added;
        //                teachPlanName4 = teachPlanName3;
        //            }
        //            majorTeachPlanDess.Add(teachPlanDes);
        //            //yunEntities.Entry(teachPlanDes).State = EntityState.Added;

        //        }
        //        yunEntities.Set<YD_Edu_SubTeachPlan>().AddRange(subTeachPlans);
        //        yunEntities.Set<YD_Edu_TeachplanDes>().AddRange(majorTeachPlanDess);
        //        yunEntities.SaveChanges();
        //        return Redirect("SubTeachPlan");
        //    }
        //}
        /// <summary>
        /// 将验证无误的数据进行导入--弃用
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadTrueTeachPlan()
        {
            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int) YesOrNo.Yes;
                var scoreList =
                    yunEntities.YD_Edu_TeachPlanTemp.Where(u => u.y_isok == isOk)
                        .OrderBy(u => u.y_majorId)
                        .ThenBy(u => u.y_term)
                        .ThenBy(u => u.y_teachPlanType)
                        .ThenBy(u => u.y_year)
                        .ThenBy(u => u.id)
                        .ToList();
                //笨方法，首先生成教学计划对象，写死教学计划名。
                //在插入教学计划详情表时，根据约定写死的教学计划名得到教学计划id
                //同样的在插入专业教学计划表时，根据约定写死的教学计划名得到教学计划id
                var teachPlanName1 = "";
                var teachPlanName2 = "";
                var teachPlans = new List<YD_Edu_TeachPlan>();
                for (var i = 0; i < scoreList.Count; i++)
                {
                    teachPlanName2 = scoreList[i].y_majorLib + scoreList[i].y_eduTypeName +
                                     scoreList[i].y_stuTypeName + "第" + scoreList[i].y_term + "学期" +
                                     scoreList[i].y_teachPlanType + "类型" + scoreList[i].y_year + "年";
                    if (teachPlanName1 == teachPlanName2) continue;
                    teachPlanName1 = teachPlanName2;
                    var teachPlan = new YD_Edu_TeachPlan
                    {
                        y_teaPlanName = teachPlanName2 + DateTime.Now.ToShortDateString(),
                        y_term = Convert.ToInt32(scoreList[i].y_term)
                    };
                    teachPlans.Add(teachPlan);
                    //yunEntities.Entry(teachPlan).State = EntityState.Added;
                }
                yunEntities.Configuration.AutoDetectChangesEnabled = false;
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                yunEntities.Set<YD_Edu_TeachPlan>().AddRange(teachPlans);
                yunEntities.SaveChanges();
                //yunEntities.SaveChanges();
                var teachPlanName3 = "";
                var teachPlanName4 = "";
                var majorTeachPlans = new List<YD_Edu_MajorTeachPlan>();
                var majorTeachPlanDess = new List<YD_Edu_TeachplanDes>();
                var teachPlanLib = yunEntities.YD_Edu_TeachPlan.ToList();
                for (var i = 0; i < scoreList.Count; i++)
                {
                    teachPlanName3 = scoreList[i].y_majorLib + scoreList[i].y_eduTypeName +
                                     scoreList[i].y_stuTypeName + "第" + scoreList[i].y_term + "学期" +
                                     scoreList[i].y_teachPlanType + "类型" + scoreList[i].y_year + "年"
                                     + DateTime.Now.ToShortDateString();
                    var entity = teachPlanLib.FirstOrDefault(u => u.y_teaPlanName == teachPlanName3);
                    if (entity == null) continue;
                    //if (teachPlanName3 == teachPlanName4) continue;
                    //teachPlanName4 = teachPlanName3;
                    var teachPlanDes = new YD_Edu_TeachplanDes
                    {
                        y_teaPlanId = entity.id,
                        y_courseId = Convert.ToInt32(scoreList[i].y_courseId),
                        y_courseTypeId = Convert.ToInt32(scoreList[i].y_courseTypeId),
                        y_score = Convert.ToDecimal(scoreList[i].y_score),
                        y_stuTime = Convert.ToInt32(scoreList[i].y_stuTime)
                    };
                    if (teachPlanName4 != teachPlanName3)
                    {
                        var majorTeachPlan = new YD_Edu_MajorTeachPlan
                        {
                            y_majorId = Convert.ToInt32(scoreList[i].y_majorId),
                            y_teachPlanId = entity.id,
                            y_year = Convert.ToInt32(scoreList[i].y_year),
                            y_teaPlanType = Convert.ToInt32(scoreList[i].y_teachPlanType),
                        };
                        majorTeachPlans.Add(majorTeachPlan);
                        //yunEntities.Entry(majorTeachPlan).State = EntityState.Added;
                        teachPlanName4 = teachPlanName3;
                    }
                    majorTeachPlanDess.Add(teachPlanDes);
                    //yunEntities.Entry(teachPlanDes).State = EntityState.Added;

                }
                yunEntities.Set<YD_Edu_MajorTeachPlan>().AddRange(majorTeachPlans);
                yunEntities.SaveChanges();
                yunEntities.Set<YD_Edu_TeachplanDes>().AddRange(majorTeachPlanDess);
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return
                        Content("<script type='text/javascript'>alert('导入成功,导入" + r +
                                "条数据'); window.location.href='/Edu/TeachPlan';</script >");
                }
                else
                {
                    return Content("导入失败");
                }
                //return Redirect("TeachPlan");
            }
        }

        #endregion
    }
}
