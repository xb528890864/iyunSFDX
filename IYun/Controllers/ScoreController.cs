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
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    /// <summary>
    /// c成绩管理
    /// </summary>
    public class ScoreController : AdminBaseController
    {
        private YD_Edu_ScoreDal _scoreDal = new YD_Edu_ScoreDal();

        /// 成绩列表页--弃用
        public ActionResult Score(int id = 1)
        {
            #region “成绩列表”权限验证

            var power = SafePowerPage("/Score/Score");
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
                var ySubSchoolIdStr = Request["SubSchool"];
                var yMajorIdStr = Request["MajorLibrary"];
                var yStuNum = Request["y_stuNum"];
                var name = Request["name"];
                IQueryable<VW_ScoreFirst> list = yunEntities.VW_ScoreFirst.OrderByDescending(u => u.y_stuId);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_stuName.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(yStuNum))
                {
                    list = list.Where(u => u.y_stuNum.Contains(yStuNum));
                }
                if (!string.IsNullOrWhiteSpace(ySubSchoolIdStr) && !ySubSchoolIdStr.Equals("0"))
                {
                    var ySubSchoolId = Convert.ToInt32(ySubSchoolIdStr);
                    list = list.Where(u => u.y_subSchoolId == ySubSchoolId);
                }
                if (!string.IsNullOrWhiteSpace(yMajorIdStr) && !yMajorIdStr.Equals("0"))
                {
                    var yMajorId = Convert.ToInt32(yMajorIdStr);
                    list = list.Where(u => u.y_majorId == yMajorId);
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {

                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    var stuInfos =
                        yunEntities.VW_StuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value))
                            .DistinctBy(u => new { u.y_majorId, u.y_subSchoolId }).Select(u => u.y_majorId)
                            .ToList();
                    list = list.Where(u => u.y_majorId.HasValue && stuInfos.Contains(u.y_majorId.Value));
                }

                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
                ViewBag.adminrole = YdAdminRoleId;
                if (Request.IsAjaxRequest())
                    return PartialView("ScoreList", dbLogList);
                return View(dbLogList);
            }
        }

        /// 查看学生个人课程成绩详情
        public ActionResult ScoreFirst(int? id)
        {
            #region “成绩列表”权限验证

            var power = SafePowerPage("/Score/Score");
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
            ViewBag.role = YdAdminRoleId;
            using (var yunEntities = new IYunEntities())
            {
                var ySubSchoolIdStr = Request["SubSchool"];
                var yTermStr = Request["y_term"];
                var yCourseIdStr = Request["Course"];
                var name = Request["name"];
                IQueryable<VW_Score> list = yunEntities.VW_Score.OrderBy(u => u.y_term).Where(u => u.y_stuId == id);
                if (!string.IsNullOrWhiteSpace(yCourseIdStr) && !yCourseIdStr.Equals("0")) //课程
                {
                    var yCourseId = Convert.ToInt32(yCourseIdStr);
                    list = list.Where(u => u.y_courseId == yCourseId);
                    ViewBag.course = yCourseId;
                }
                if (!string.IsNullOrWhiteSpace(yTermStr) && !yTermStr.Equals("0"))
                {
                    var yTerm = Convert.ToInt32(yTermStr);
                    list = list.Where(u => u.y_term == yTerm);
                    ViewBag.term = yTerm;
                }
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    var stuInfos = yunEntities.VW_StuInfo.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value))
                        .DistinctBy(u => new { u.y_majorId, u.y_subSchoolId }).Select(U => U.y_majorId).ToList();

                    list = list.Where(u => u.y_majorId.HasValue && stuInfos.Contains(u.y_majorId.Value));
                }
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
                var model = list.ToList();
                ViewBag.stuid = id;
                return View(model);
            }
        }

        /// 成绩列表页--弃用
        public ActionResult DownloadScore()
        {
            #region “成绩列表”权限验证

            var power = SafePowerPage("/Score/Score");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                var ySubSchoolIdStr = Request["SubSchool"];
                var yMajorIdStr = Request["majorlihidden"];
                var yTermStr = Request["y_term"];
                var yStuNum = Request["y_stuNum"];

                IQueryable<VW_Score> list = yunEntities.VW_Score.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(yStuNum))
                {
                    list = list.Where(u => u.y_stuNum.Contains(yStuNum));
                }

                if (!string.IsNullOrWhiteSpace(yTermStr) && !yTermStr.Equals("0"))
                {
                    var yTerm = Convert.ToInt32(yTermStr);
                    list = list.Where(u => u.y_term == yTerm);
                }
                if (!string.IsNullOrWhiteSpace(ySubSchoolIdStr) && !ySubSchoolIdStr.Equals("0"))
                {
                    //var y_subSchoolId = Convert.ToInt32(y_subSchoolIdStr);
                    list = list.Where(u => u.schoolName == ySubSchoolIdStr);
                }
                if (!string.IsNullOrWhiteSpace(yMajorIdStr) && !yMajorIdStr.Equals("0"))
                {
                    var yMajorId = Convert.ToInt32(yMajorIdStr);
                    list = list.Where(u => u.y_majorId == yMajorId);
                }
                var model =
                    FileHelper.ToDataTable(
                        list.Select(
                            u =>
                                new
                                {
                                    u.y_stuName,
                                    u.y_sex,
                                    u.y_stuNum,
                                    u.y_examNum,
                                    u.majorLibraryName,
                                    u.eduTypeName,
                                    u.stuTypeName,
                                    u.schoolName,
                                    u.y_inYear,
                                    u.y_normalScore,
                                    u.y_termScore,
                                    u.y_totalScore
                                }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/学生成绩表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变


                //var filename1 = "File/Dowon/学生成绩表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                    {
                        {"y_stuName", "姓名"},
                        {"y_sex", "性别"},
                        {"y_stuNum", "学号"},
                        {"y_examNum", "考生号"},
                        {"majorLibraryName", "专业名"},
                        {"eduTypeName", "层次"},
                        {"stuTypeName", "学习形式"},
                        {"schoolName", "函授站"},
                        {"y_normalScore", "平时分"},
                        {"y_termScore", "考试分"},
                        {"y_totalScore", "总评分"},
                        {"y_inYear", "入学年份"}
                    };
                    var t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        if (Request.Url != null)
                        {
                            var url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                            return Content(url);
                        }
                    }
                    return Content("no");
                }
            }
        }

        /// 编辑成绩视图--弃用
        public ActionResult ScoreEditPage(int id)
        {
            #region “编辑成绩”权限验证

            var power = SafePowerPage("/Score/Score");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.VW_Score.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// 添加成绩页面--弃用
        public ActionResult ScoreAddPage()
        {
            #region “添加成绩”权限验证

            var power = SafePowerPage("/Score/Score");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }

            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// 编辑成绩--弃用
        public ActionResult ScoreEdit()
        {
            using (var yunEntities = new IYunEntities())
            {
                var idStr = Request["id"];
                var yNormalScoreStr = Request["y_normalScore"];
                var yTermScoreStr = Request["y_termScore"];
                if (string.IsNullOrEmpty(idStr) && string.IsNullOrEmpty(yNormalScoreStr) &&
                    string.IsNullOrEmpty(yTermScoreStr))
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('未知错误');window.location.href='" + reurl + "'</script>");
                }
                var id = Convert.ToInt32(idStr);
                var yTermScore = Convert.ToDecimal(yTermScoreStr);
                var yNormalScore = Convert.ToDecimal(yNormalScoreStr);
                var role = yunEntities.YD_Edu_Score.FirstOrDefault(u => u.id == id);
                if (role != null)
                {
                    role.y_normalScore = yNormalScore;
                    role.y_termScore = yTermScore;
                    return Content(_scoreDal.EditEntity(role, yunEntities));
                }
                else
                {
                    return Content("编辑失败");
                }
            }
        }

        /// 增加成绩--弃用
        public ActionResult ScoreAdd(YD_Edu_Score role)
        {

            using (var yunEntities = new IYunEntities())
            {
                return Content(_scoreDal.AddEntity(role, yunEntities));
            }
        }

        /// 删除成绩--弃用
        public ActionResult ScoreDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_scoreDal.EntityDelete(id, yunEntities));
            }
        }

        /// 批量导入成绩--新开发
        public string UploadNewScore(string fileName)
        {
            var result = new Hashtable
            {
                ["Message"] = "Excel格式不正确",
                ["IsOk"] = false
            };
            //fileName = Server.MapPath(fileName);
            fileName = "D://成绩导入模板.xlsx";
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
                    return JsonConvert.SerializeObject(result);
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return JsonConvert.SerializeObject(result);
                }
                var errorCount = 0;

                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);

                var list = CoreFunction.ScoreTempletValidate(ref errorCount, sheet, styleCell);
                //验证表格的错误情况，并返回错误数量，详情参方法体内
                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/成绩导入信息表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        result["IsOk"] = true;
                        result["Message"] = url;
                        return JsonConvert.SerializeObject(result);
                    }
                }
                else //否则直接导入数据
                {
                    using (var ad = new IYunEntities())
                    {
                        var sb = new StringBuilder("INSERT INTO YD_Edu_Score ");
                        sb.AppendLine("([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                        var inde = 0;

                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];

                            inde++;

                            sb.AppendLine($"({item.y_stuId},{item.y_term},{item.y_normalScore},{item.y_termScore},null,{item.y_totalScore},{item.y_courseId},1,GETDATE()),");

                            if (inde == 999 || i == list.Count - 1)
                            {
                                string sql = sb.ToString(0, sb.Length - 3);

                                ad.Database.ExecuteSqlCommand(sql);

                                inde = 0;

                                sb = new StringBuilder("INSERT INTO YD_Edu_Score ");
                                sb.AppendLine("([y_stuId],[y_term],[y_normalScore],[y_termScore],[y_workScore],[y_totalScore],[y_courseId],[y_type],[y_time]) VALUES");
                            }
                        }
                    }

                    return JsonConvert.SerializeObject(result);
                }
            }


        }

        /// 批量导入成绩页面--弃用
        public ActionResult UploadScore()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/Score");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
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
            using (var excelHelper = new ExcelHelper(fileName))
            {
                var dt = excelHelper.ExcelToDataTable("", true);
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.Database.ExecuteSqlCommand("DELETE  FROM YD_Edu_ScoreTemp");
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 4);     //根据父栏目ID获取兄弟栏目
                    var stuNum = "";
                    var stuName = "";
                    var stuId = 0;
                    var examNum = "";
                    var term = 0;
                    var courseId = 0;
                    var courseName = "";
                    decimal normalScore = 0;
                    decimal termScore = 0;
                    decimal workScore = 0; //作业
                    decimal totalScore = 0;
                    var isOk = (int)YesOrNo.No;
                    var scoreList = new List<YD_Edu_ScoreTemp>();
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        stuNum = "";
                        stuName = "";
                        stuId = 0;
                        examNum = "";
                        term = 0;
                        courseId = 0;
                        courseName = "";
                        normalScore = 0;
                        termScore = 0;
                        workScore = 0; //作业
                        totalScore = 0;
                        isOk = (int)YesOrNo.No;


                        #region 获取学生id
                        if (dt.Rows[i]["学号"] != null)
                        {
                            stuNum = dt.Rows[i]["学号"].ToString();
                            var ydStsStuInfo = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == stuNum);
                            if (ydStsStuInfo != null)
                            {
                                stuId = ydStsStuInfo.id;
                                stuName = ydStsStuInfo.y_name;
                            }
                            else
                            {
                                if (dt.Rows[i]["考生号"] != null)
                                {
                                    examNum = dt.Rows[i]["考生号"].ToString();
                                    if (yunEntities.YD_Sts_StuInfo.Any(u => u.y_examNum == examNum))
                                    {
                                        var stsStuInfo = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == examNum);
                                        if (stsStuInfo != null)
                                        {
                                            stuId = stsStuInfo.id;
                                            stuName = stsStuInfo.y_name;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 获取课程id
                        if (dt.Rows[i]["课程"] != null)
                        {
                            courseName = dt.Rows[i]["课程"].ToString();
                            if (yunEntities.YD_Edu_Course.Any(u => u.y_name == courseName))
                            {
                                var ydEduCourse = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == courseName);
                                if (ydEduCourse != null)
                                    courseId = ydEduCourse.id;
                            }
                        }
                        #endregion
                        if (dt.Rows[i]["学期"] != null)
                        {
                            int.TryParse(dt.Rows[i]["学期"].ToString(), out term);
                        }
                        if (dt.Rows[i]["平时成绩"] != null)
                        {
                            decimal.TryParse(dt.Rows[i]["平时成绩"].ToString(), out normalScore);
                        }
                        if (dt.Rows[i]["期末成绩"] != null)
                        {
                            decimal.TryParse(dt.Rows[i]["期末成绩"].ToString(), out termScore);
                        }
                        if (dt.Rows[i]["作业成绩"] != null)
                        {
                            decimal.TryParse(dt.Rows[i]["作业成绩"].ToString(), out workScore);
                        }
                        if (dt.Rows[i]["总评成绩"] != null)
                        {
                            decimal.TryParse(dt.Rows[i]["总评成绩"].ToString(), out totalScore);
                        }
                        if (courseId != 0 && stuId != 0 && term != 0)
                        {
                            isOk = (int)YesOrNo.Yes;
                        }
                        var scoreTemp = new YD_Edu_ScoreTemp
                        {
                            y_course = courseName,
                            y_courseId = courseId,
                            y_examNum = examNum,
                            y_normalScore = normalScore,
                            y_stuId = stuId,
                            y_stuNum = stuNum,
                            y_term = term,
                            y_termScore = termScore,
                            y_workScore = workScore,
                            y_totalScore = totalScore,
                            y_stuName = stuName,
                            y_isOk = isOk
                        };
                        scoreList.Add(scoreTemp);
                        //yunEntities.Entry(scoreTemp).State = EntityState.Added;
                    }
                    yunEntities.Configuration.AutoDetectChangesEnabled = false;
                    yunEntities.Configuration.ValidateOnSaveEnabled = false;
                    yunEntities.Set<YD_Edu_ScoreTemp>().AddRange(scoreList);
                    yunEntities.SaveChanges();
                    //yunEntities.BulkInsert(scoreList);
                    //yunEntities.BulkSaveChanges();
                    //yunEntities.SaveChanges();
                    ViewBag.scoreList = yunEntities.YD_Edu_ScoreTemp.Where(u => true).OrderBy(u => u.id).Skip(0).Take(15).ToList();
                    return View();
                }
            }
        }

        /// 验证导入的临时成绩--弃用
        public ActionResult VerifyScore()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/Score");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                const int isOk = (int)YesOrNo.No;
                //如果数据库存在导入的专业则不导入
                var alikescpres = yunEntities.YD_Edu_ScoreTemp.ToList();
                var lib = yunEntities.VW_Score.Where(u => u.y_isdel == (int)YesOrNo.No).AsQueryable();
                if (alikescpres.Any())
                {
                    foreach (var item in alikescpres)
                    {
                        if (lib.Any(a => a.y_stuId == item.y_stuId && a.y_examNum == item.y_examNum && a.y_term == item.y_term && a.y_courseId == item.y_courseId && a.y_termScore == item.y_termScore && a.y_normalScore == item.y_normalScore && a.y_totalScore == item.y_totalScore))
                        {
                            yunEntities.YD_Edu_ScoreTemp.Remove(item);
                        }
                    }
                    int r = yunEntities.SaveChanges();
                    if (r > 0)
                    {
                        return Content("<script type='text/javascript'>alert('有重复成绩不导入，重复条数为" + r + "');window.location.href='/Score/VerifyScore';</script >");
                    }
                }
                ViewBag.scoreList = yunEntities.YD_Edu_ScoreTemp.Where(u => u.y_isOk == isOk).ToList();
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4);     //根据父栏目ID获取兄弟栏目
                return View();
            }
        }

        /// 提交用户更新的临时数据--弃用
        public ActionResult UpdateVerify()
        {
            var ids = Request["ids"].Split(new[] { "<>" }, StringSplitOptions.None);
            var stuNums = Request["stuNums"].Split(new[] { "<>" }, StringSplitOptions.None);
            var examNums = Request["examNums"].Split(new[] { "<>" }, StringSplitOptions.None);
            var terms = Request["terms"].Split(new[] { "<>" }, StringSplitOptions.None);
            var courses = Request["courses"].Split(new[] { "<>" }, StringSplitOptions.None);
            var id = 0;
            var term = 0;
            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < ids.Count(); i++)
                {
                    id = Convert.ToInt32(ids[i]);
                    int.TryParse(terms[i], out term);
                    var scoreTemp = yunEntities.YD_Edu_ScoreTemp.FirstOrDefault(u => u.id == id);
                    if (scoreTemp == null) continue;
                    scoreTemp.y_stuNum = stuNums[i];
                    scoreTemp.y_examNum = examNums[i];
                    scoreTemp.y_course = courses[i];
                    scoreTemp.y_term = term;


                    var ydStsStuInfo = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == scoreTemp.y_stuNum);
                    if (ydStsStuInfo == null)
                    {
                        var stuInfo = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == scoreTemp.y_examNum);
                        if (stuInfo != null)
                        {
                            scoreTemp.y_stuId = stuInfo.id;
                            scoreTemp.y_stuName = stuInfo.y_name;
                        }
                        else
                        {
                            scoreTemp.y_stuId = 0;
                            scoreTemp.y_stuName = "";
                        }
                    }
                    else
                    {
                        scoreTemp.y_stuId = ydStsStuInfo.id;
                        scoreTemp.y_stuName = ydStsStuInfo.y_name;
                    }
                    var ydEduCourse = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == scoreTemp.y_course);
                    scoreTemp.y_courseId = ydEduCourse != null ? ydEduCourse.id : 0;


                    if (scoreTemp.y_courseId != 0 && scoreTemp.y_stuId != 0 && scoreTemp.y_term != 0)
                    {
                        scoreTemp.y_isOk = (int)YesOrNo.Yes;
                    }
                    yunEntities.Entry(scoreTemp).State = EntityState.Modified;
                }
                var t = yunEntities.SaveChanges();
                return Content("ok");
            }
        }

        /// 将验证无误的数据进行导入+--弃用
        public ActionResult UploadTrueScore()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/Score");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
                var scoreList = yunEntities.YD_Edu_ScoreTemp.Where(u => u.y_isOk == isOk).ToList();
                var scoreListT = new List<YD_Edu_Score>();
                for (var i = 0; i < scoreList.Count; i++)
                {
                    var score = new YD_Edu_Score
                    {
                        y_courseId = Convert.ToInt32(scoreList[i].y_courseId),
                        y_normalScore = Convert.ToDecimal(scoreList[i].y_normalScore),
                        y_termScore = Convert.ToDecimal(scoreList[i].y_termScore),
                        y_workScore = Convert.ToDecimal(scoreList[i].y_workScore),
                        y_totalScore = Convert.ToDecimal(scoreList[i].y_totalScore),
                        y_stuId = Convert.ToInt32(scoreList[i].y_stuId),
                        y_term = Convert.ToInt32(scoreList[i].y_term)
                    };
                    scoreListT.Add(score);
                    //yunEntities.Entry(score).State = EntityState.Added;
                }
                yunEntities.Configuration.AutoDetectChangesEnabled = false;
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                yunEntities.Set<YD_Edu_Score>().AddRange(scoreListT);
                yunEntities.SaveChanges();
                //yunEntities.BulkInsert(scoreListT);
                //yunEntities.BulkSaveChanges();
                //yunEntities.SaveChanges();
                return Redirect("Score");
            }
        }

        /// 学生数据下载--弃用
        public ActionResult DownloadRecordScore()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScore");
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
                var name = Request["name"];
                var sex = Request["sex"];
                var stuState = Request["StuState"];
                var card = Request["card"];
                var birthday = Request["birthday"];
                var enrollYear = Request["EnrollYear"];
                var subSchool = Request["SubSchool"];
                var majorLibrary = Request["MajorLibrary"];
                var eduType = Request["EduType"];
                var stuType = Request["StuType"];
                var tel = Request["tel"];
                var namenumcard = Request["namenumcard"];
                const int isnotdel = (int)YesOrNo.No;

                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel);
                if (!string.IsNullOrWhiteSpace(namenumcard))
                {
                    list = list.Where(u => u.y_name.Contains(namenumcard) || u.y_stuNum.Contains(namenumcard) || u.y_cardId.Contains(namenumcard));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name.Contains(name));
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

                var model = FileHelper.ToDataTable(
                    list.Select(u => new StuList(u.y_birthday)
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
                        nationName = u.nationName,
                        politicsName = u.politicsName,
                        stuStateName = u.stuStateName,
                        y_address = u.y_address,
                        y_inYear = u.y_inYear
                    }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon");         //todo:改变
                if (!Directory.Exists(dirPath))                       //todo:改变
                {
                    Directory.CreateDirectory(dirPath);               //todo:改变
                }
                var filename1 = "/按学生录成绩表" + ".xls";      //todo:改变
                var fileName3 = dirPath + filename1;                               //todo:改变

                //var filename1 = "File/Dowon/按学生录成绩表" + Guid.NewGuid() + ".xls";
                //var fileName2 = "~/" + filename1;
                //var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable { { "y_name", "姓名" }, { "y_sex", "性别" }, { "y_stuNum", "学号" }, { "y_examNum", "考生号" }, { "majorLibraryName", "专业名" }, { "eduTypeName", "层次" }, { "stuTypeName", "学习形式" }, { "schoolName", "函授站" }, { "y_cardId", "身份证" }, { "y_birthday", "出生日期" }, { "stuStateName", "学籍状态" }, { "y_address", "地址" }, { "y_inYear", "入学年份" } };
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

        /// 更新学生成绩--弃用
        public string UpdateStuScore()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScore");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var normalScoreStr = Request["normalScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var termScoreStr = Request["termScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var totalScoreStr = Request["totalScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var courseIdStr = Request["courseId"].Split(new[] { "<>" }, StringSplitOptions.None);
            var stuIdStr = Request["stuId"];
            var termStr = Request["term"];

            if (string.IsNullOrWhiteSpace(stuIdStr) || string.IsNullOrWhiteSpace(termStr))
            {
                return "未知错误";
            }
            var stuId = Convert.ToInt32(stuIdStr);
            var term = Convert.ToInt32(termStr);
            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < courseIdStr.Count(); i++)
                {
                    var courseId = Convert.ToInt32(courseIdStr[i]);
                    YD_Edu_Score myscore = null;

                    myscore =
                        yunEntities.YD_Edu_Score.FirstOrDefault(
                            u => u.y_courseId == courseId && stuId == u.y_stuId && u.y_term == term);
                    if (myscore != null)
                    {
                        myscore.y_normalScore = Convert.ToDecimal(normalScoreStr[i]);
                        myscore.y_termScore = Convert.ToDecimal(termScoreStr[i]);
                        myscore.y_totalScore = Convert.ToDecimal(totalScoreStr[i]);
                        yunEntities.Entry(myscore).State = EntityState.Modified;
                    }
                    else
                    {
                        myscore = new YD_Edu_Score
                        {
                            y_normalScore = Convert.ToDecimal(normalScoreStr[i]),
                            y_termScore = Convert.ToDecimal(termScoreStr[i]),
                            y_totalScore = Convert.ToDecimal(totalScoreStr[i]),
                            y_courseId = courseId,
                            y_stuId = stuId,
                            y_term = term
                        };
                        yunEntities.Entry(myscore).State = EntityState.Added;
                    }
                }
                var t = yunEntities.SaveChanges();
                return t > 0 ? "ok" : "保存错误";

            }
        }
        //--弃用
        public string UpdateStuScoreOne()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScore");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var normalScoreStr = Request["normalScore"];
            var termScoreStr = Request["termScore"];
            var totalScoreStr = Request["totalScore"];
            var courseIdStr = Request["courseId"];
            var stuIdStr = Request["stuId"];
            var termStr = Request["term"];
            if (string.IsNullOrWhiteSpace(stuIdStr) || string.IsNullOrWhiteSpace(termStr))
            {
                return "未知错误";
            }
            var stuId = Convert.ToInt32(stuIdStr);
            var term = Convert.ToInt32(termStr);
            var courseId = Convert.ToInt32(courseIdStr);
            using (var db = new IYunEntities())
            {
                var myscore =
                    db.YD_Edu_Score.FirstOrDefault(
                        u => u.y_courseId == courseId && stuId == u.y_stuId && u.y_term == term);
                if (myscore != null)
                {
                    myscore.y_normalScore = Convert.ToDecimal(normalScoreStr);
                    myscore.y_termScore = Convert.ToDecimal(termScoreStr);
                    myscore.y_totalScore = Convert.ToDecimal(totalScoreStr);
                    db.Entry(myscore).State = EntityState.Modified;
                }
                else
                {
                    myscore = new YD_Edu_Score
                    {
                        y_normalScore = Convert.ToDecimal(normalScoreStr),
                        y_termScore = Convert.ToDecimal(termScoreStr),
                        y_totalScore = Convert.ToDecimal(totalScoreStr),
                        y_courseId = courseId,
                        y_stuId = stuId,
                        y_term = term
                    };
                    db.Entry(myscore).State = EntityState.Added;
                }
                var t = db.SaveChanges();
                return t > 0 ? "ok" : "保存错误";
            }

        }

        /// 更新学生成绩(以课程为主体)--弃用
        public string UpdateStuScoreCourse()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/StudentScoreCourse");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            var normalScoreStr = Request["normalScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var termScoreStr = Request["termScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var totalScoreStr = Request["totalScore"].Split(new[] { "<>" }, StringSplitOptions.None);
            var stuIdStr = Request["stuId"].Split(new[] { "<>" }, StringSplitOptions.None);
            //var courseIdStr = Request["courseId"];
            //var termStr = Request["term"];
            var courseIdStr = Request["courseId"].Split(new[] { "<>" }, StringSplitOptions.None);
            var termStr = Request["term"].Split(new[] { "<>" }, StringSplitOptions.None);

            //if (string.IsNullOrWhiteSpace(courseIdStr) || string.IsNullOrWhiteSpace(termStr))
            //{
            //    return "未知错误";
            //}
            //var courseId = Convert.ToInt32(courseIdStr);
            //var term = Convert.ToInt32(termStr);
            using (var yunEntities = new IYunEntities())
            {
                for (var i = 0; i < stuIdStr.Count(); i++)
                {
                    var stuId = Convert.ToInt32(stuIdStr[i]);
                    var courseId = Convert.ToInt32(courseIdStr[i]);
                    var term = Convert.ToInt32(termStr[i]);
                    YD_Edu_Score myscore = null;
                    myscore =
                        yunEntities.YD_Edu_Score.FirstOrDefault(
                            u => courseId == u.y_courseId && stuId == u.y_stuId && term == u.y_term);
                    if (myscore != null)
                    {
                        myscore.y_normalScore = Convert.ToDecimal(normalScoreStr[i]);
                        myscore.y_termScore = Convert.ToDecimal(termScoreStr[i]);
                        myscore.y_totalScore = Convert.ToDecimal(totalScoreStr[i]);
                        yunEntities.Entry(myscore).State = EntityState.Modified;
                    }
                    else
                    {
                        myscore = new YD_Edu_Score
                        {
                            y_normalScore = Convert.ToDecimal(normalScoreStr[i]),
                            y_termScore = Convert.ToDecimal(termScoreStr[i]),
                            y_totalScore = Convert.ToDecimal(totalScoreStr[i]),
                            y_courseId = courseId,
                            y_stuId = stuId,
                            y_term = term
                        };
                        yunEntities.Entry(myscore).State = EntityState.Added;
                    }
                }
                var t = yunEntities.SaveChanges();
                return t > 0 ? "ok" : "保存错误";
            }
        }


        public string UpdateStuCourScoreOne()
        {
            #region 权限验证

            var power = SafePowerPage("/Score/StudentScoreCourse");
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }

            #endregion

            var normalScoreStr = Request["normalScore"];
            var termScoreStr = Request["termScore"];
            var totalScoreStr = Request["totalScore"];
            var courseIdStr = Request["courseId"];
            var stuIdStr = Request["stuId"];
            var termStr = Request["term"];
            if (string.IsNullOrWhiteSpace(courseIdStr) || string.IsNullOrWhiteSpace(termStr))
            {
                return "未知错误";
            }
            var courseId = Convert.ToInt32(courseIdStr);
            var term = Convert.ToInt32(termStr);
            using (var yunEntities = new IYunEntities())
            {

                var stuId = Convert.ToInt32(stuIdStr);
                YD_Edu_Score myscore = null;
                myscore =
                    yunEntities.YD_Edu_Score.FirstOrDefault(
                        u => u.y_courseId == courseId && stuId == u.y_stuId && u.y_term == term);
                if (myscore != null)
                {
                    myscore.y_normalScore = Convert.ToDecimal(normalScoreStr);
                    myscore.y_termScore = Convert.ToDecimal(termScoreStr);
                    myscore.y_totalScore = Convert.ToDecimal(totalScoreStr);
                    yunEntities.Entry(myscore).State = EntityState.Modified;
                }
                else
                {
                    myscore = new YD_Edu_Score
                    {
                        y_normalScore = Convert.ToDecimal(normalScoreStr),
                        y_termScore = Convert.ToDecimal(termScoreStr),
                        y_totalScore = Convert.ToDecimal(totalScoreStr),
                        y_courseId = courseId,
                        y_stuId = stuId,
                        y_term = term
                    };
                    yunEntities.Entry(myscore).State = EntityState.Added;
                }

                var t = yunEntities.SaveChanges();

                LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",更新学生成绩,ID:" + myscore.id + ",方法:UpdateStuCourScoreOne");

                return t > 0 ? "ok" : "保存错误";
            }
        }

        /// 分数比例设置
        public ActionResult ScoreScale()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/Scorescale");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
                ViewBag.entity = yunEntities.YD_Edu_ScoreScale.FirstOrDefault(u => true);
                return View();
            }
        }

        public string SetScoreScale(YD_Edu_ScoreScale scoreScale)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/Scorescale");
            if (!IsLogin())
            {
                return "未登录";
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return "没有权限";
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Entry(scoreScale).State = EntityState.Modified;
                var t = yunEntities.SaveChanges();

                LogHelper.WriteInfoLog(MethodBase.GetCurrentMethod().DeclaringType, "用户ID:" + Session[KeyValue.Admin_id] + ",用户名：" + Session[KeyValue.Admin_Name] + ",修改分数比例,ID:" + scoreScale.id + ",方法:SetScoreScale");

                return t > 0 ? "ok" : "设置失败";
            }
        }

        /// (函授站页面)--弃用
        public ActionResult RecordScoreSub(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScoreSub");
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
                var nameStr = Request["y_name"];
                var entityList = new List<YD_Sys_SubSchool>();
                if (!string.IsNullOrWhiteSpace(nameStr))
                {
                    var name = nameStr.Trim().Replace(" ", "").Replace("函授站", "").Replace("函授", "");
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
                        entityList = yunEntities.Database.SqlQuery<YD_Sys_SubSchool>("select * from YD_Sys_SubSchool where " + sql).ToList();
                    }
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                    {
                        var subLinks = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId).ToList();
                        entityList = entityList.Where(u => subLinks.Contains(u.id)).ToList();
                    }
                    var dbLogList = entityList.OrderBy(u => u.y_name).ThenByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 4);     //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("RecordScoreSubList", dbLogList);
                    return View(dbLogList);
                }
                else
                {
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                    {
                        var subLinks = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId).ToList();

                        entityList = yunEntities.YD_Sys_SubSchool.Where(u => subLinks.Contains(u.id)).ToList();

                    }
                    else
                    {
                        entityList = yunEntities.YD_Sys_SubSchool.Where(u => true).ToList();
                    }
                    var dbLogList = entityList.OrderBy(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 4);     //根据父栏目ID获取兄弟栏目
                    if (Request.IsAjaxRequest())
                        return PartialView("RecordScoreSubList", dbLogList);
                    return View(dbLogList);
                }
            }
        }

        /// (课程页面)--弃用
        public ActionResult RecordScoreCourse(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScoreSub");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4);     //根据父栏目ID获取兄弟栏目
                var yearStr = Request["year"];
                var subIdStr = Request["subId"];
                int subId = 0;
                if (!int.TryParse(subIdStr, out subId))
                {
                    var reurl = Request.UrlReferrer.ToString();
                    return Content("<script>alert('参数错误，返回再试');window.location.href='" + reurl + "'</script>");
                }
                if (string.IsNullOrEmpty(yearStr))
                {
                    yearStr = DateTime.Now.Year.ToString();
                }
                var year = Convert.ToInt32(yearStr);
                var majorId = Convert.ToInt32(Request["majorId"]);
                var teachPlanList = yunEntities.VW_MajorTeachPlan.Where(u => u.y_majorId == majorId && u.y_year == year && u.y_teaPlanType == 2).ToList();
                var entityList = new List<VW_TeachPlanDes>();
                for (var i = 0; i < teachPlanList.Count; i++)
                {
                    var tid = teachPlanList[i].y_teachPlanId;
                    entityList.AddRange(yunEntities.VW_TeachPlanDes.Where(u => u.y_teaPlanId == tid).ToList());
                }
                if (YdAdminRoleId != 1 && YdAdminRoleId != 3 && YdAdminRoleId != 7)
                {
                    //获取该函授站录分权限--科目录分权限
                    var smallPowerList = yunEntities.YD_Edu_SmallPower.Where(u => u.y_subSchoolId == subId && u.y_endTime > DateTime.Now).ToList();
                    if (smallPowerList.Count <= 0)
                    {
                        var reurl = Request.UrlReferrer.ToString();
                        return Content("<script>alert('你没有该函授站下的录分权限，有需要请联系学校管理员');window.location.href='" + reurl + "'</script>");
                    }
                    if (smallPowerList.All(u => u.y_courseId != 0))//限制了只有部分课程可以录分
                    {
                        var majorIds = new List<int>();

                        for (var i = 0; i < smallPowerList.Count; i++)
                        {
                            majorIds.Add(Convert.ToInt32(smallPowerList[i].y_courseId));
                        }
                        entityList = entityList.Where(u => majorIds.Contains(u.y_courseId)).ToList();
                    }
                }
                ViewBag.subId = subIdStr;
                ViewBag.year = string.IsNullOrEmpty(yearStr) ? DateTime.Now.Year.ToString() : yearStr;
                ViewBag.majorId = Request["majorId"];
                ViewBag.entityList = entityList;
            }
            return View();
        }

        /// 根据科目录分
        public ActionResult StudentScoreCourse(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/StudentScoreCourse");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Redirect("/AdminBase/Index");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                if (Request.IsAjaxRequest()) //后面请求做的操作
                {
                    try
                    {
                        int stuYear = Convert.ToInt32(Request["year"]);//学生年份 例 2018
                        int schoolid = Convert.ToInt32(Request["SubSchool"]);//函授站
                        int eduid = Convert.ToInt32(Request["EduType"]);//层次 例 高起本
                        int stuTypeid = Convert.ToInt32(Request["StuType"]);//形式 例 函授
                        int majorlibid = Convert.ToInt32(Request["MajorLibrary"]);//专业
                        int term = Convert.ToInt32(Request["team"]);//学期 最大1-10
                        int courseid = Convert.ToInt32(Request["Course"]);//课程 
                        int islf = Convert.ToInt32(Request["islf"]);// 是否录分

                        var termindex = term;
                        var courseName = yunEntities.YD_Edu_Course.Find(courseid).y_name;
                        var students = yunEntities.YD_Sts_StuInfo.OrderBy(u => u.id).AsQueryable();


                        var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                        if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())//如果是华东交通大学,然后筛选学生
                        {
                            List<int> statelist = new List<int> { 1, 6, 7, 8 };
                            students =
                           students.Where(
                               u =>
                                   u.y_inYear == stuYear && u.y_subSchoolId == schoolid &&
                                   u.YD_Edu_Major.y_eduTypeId == eduid && u.YD_Edu_Major.y_stuTypeId == stuTypeid &&
                                   u.YD_Edu_Major.y_majorLibId == majorlibid && statelist.Contains(u.y_stuStateId) && u.y_isdel == 1);
                        }
                        else
                        {
                            List<int> statelist = new List<int> { 1, 7, 8, 9 };
                            students =
                           students.Where(
                               u =>
                                   u.y_inYear == stuYear && u.y_subSchoolId == schoolid &&
                                   u.YD_Edu_Major.y_eduTypeId == eduid && u.YD_Edu_Major.y_stuTypeId == stuTypeid &&
                                   u.YD_Edu_Major.y_majorLibId == majorlibid && statelist.Contains(u.y_stuStateId) && u.y_isdel == 1);
                        }
                        //专业计划从表(里面有外键1.课程表 2.班级教学计划主表) 内联 课程表（用来获得课程名） 内联 班级教学计划主表(获得专业)
                        var clist = yunEntities.YD_TeaPlan_ClassCourseDes.Include(u => u.YD_Edu_Course).Include(u => u.YD_TeaPlan_Class).AsQueryable();

                        //函授站 年份筛选
                        clist = clist.Where(u => u.YD_TeaPlan_Class.y_subSchoolId == schoolid && u.YD_TeaPlan_Class.y_year == stuYear);

                        //专业层次培训形式学年筛选
                        clist =
                            clist.Where(
                                u =>
                                    u.YD_TeaPlan_Class.YD_Edu_Major.y_majorLibId == majorlibid &&
                                    u.YD_TeaPlan_Class.YD_Edu_Major.y_eduTypeId == eduid &&
                                    u.YD_TeaPlan_Class.YD_Edu_Major.y_stuTypeId == stuTypeid &&
                                    u.y_team == term);


                        var smallPowerIds =
                            clist.GroupBy(u => new { u.y_course, u.y_courseType }).Select(u => u.Key).ToList();

                        //
                        var permissionIds = new List<ScorePermissionDto>();

                        if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                        {
                            var dateNow = DateTime.Now;


                            //函授站录分权限表
                            List<YD_Edu_SmallPower> smallPower = yunEntities.YD_Edu_SmallPower.Where(
                                u =>
                                    (!u.y_subSchoolId.HasValue || u.y_subSchoolId == schoolid) &&
                                    (!u.y_majorId.HasValue || (
                                        u.YD_Edu_Major.y_eduTypeId == eduid && u.YD_Edu_Major.y_stuTypeId == stuTypeid &&
                                        u.YD_Edu_Major.y_majorLibId == majorlibid)
                                        ) && u.y_endTime >= dateNow && u.y_year == stuYear).ToList();
                            foreach (var item in smallPower)
                            {

                                List<int> data;
                                if (item.y_courseType != null)
                                {
                                    data =
                                        smallPowerIds.Where(u => u.y_courseType == item.y_courseType.Value)
                                            .Select(u => u.y_course)
                                            .ToList();
                                }
                                else
                                {
                                    data = smallPowerIds.Select(u => u.y_course).ToList();
                                }

                                if (item.y_courseId != null)
                                {
                                    data = data.Where(u => u == item.y_courseId.Value).ToList();
                                }

                                data.ForEach(u =>
                                {
                                    permissionIds.Add(new ScorePermissionDto() { CourseId = u, Type = item.y_scorelimit, Term = item.y_term });
                                });

                            }
                        }
                        else
                        {
                            smallPowerIds.Select(u => u.y_course).ToList().ForEach(u =>
                            {
                                permissionIds.Add(new ScorePermissionDto() { CourseId = u, Type = null, Term = null });
                            });
                        }
                        var scorelist =
                            yunEntities.YD_Edu_Score.Where(u => u.y_courseId == courseid&&u.y_term== termindex).OrderByDescending(u => u.y_type).ThenByDescending(u => u.id).AsQueryable(); 
                        var result2 =
                           students.GroupJoin(scorelist, s => s.id, c => c.y_stuId, (s, c) => new
                           {
                               s = s,
                               c = c.OrderByDescending(u => u.id).FirstOrDefault()
                           }).ToList();


                        var result =
                            result2.Select(u => new ScoreInputDto
                            {
                                stuId = u.s.id,
                                stuName = u.s.y_name,
                                scoreid = u.c == null ? 0 : u.c.id,
                                term = u.c == null ? termindex : u.c.y_term,
                                normalScore = u.c == null ? 0 : u.c.y_normalScore,
                                termScore = u.c == null ? 0 : u.c.y_termScore,
                                totalScore = u.c == null ? 0 : u.c.y_totalScore,
                                courseId = courseid,
                                courseName = courseName
                            }).OrderBy(u => u.stuId).AsQueryable();

                        var totalCount = result.Count();
                        var errorCount = result.Count(u => u.totalScore == 0);
                        ViewBag.totalCount = totalCount;
                        ViewBag.errorCount = errorCount;

                        if (islf == 1)
                        {
                            result = result.Where(u => u.totalScore != 0);
                        }

                        if (islf == 2)
                        {
                            result = result.Where(u => u.totalScore == 0);
                        }

                        //var list = result.ToList();

                        var model = result.ToPagedList(id, 40); //id为pageindex   15 为pagesize

                        model.ForEach(u =>
                        {
                          
                            if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                            {
                                u.hasPermission =
                              permissionIds.Any(
                                  k =>
                                      (k.CourseId == u.courseId && k.Type == null && (k.Term == null || k.Term == u.term)) ||
                                      k.CourseId == u.courseId && k.Type == 1 && (k.Term == null || k.Term == u.term));
                            }
                            else
                            {
                                u.hasPermission = permissionIds.Any(
                                  k =>
                                      (k.CourseId == u.courseId && k.Type == null && (k.Term == null || k.Term == u.term)) ||
                                      k.CourseId == u.courseId && k.Type == 1 && (u.totalScore < 60M || u.totalScore >= 100M) && (k.Term == null || k.Term == u.term));
                            }

                        });

                        return PartialView("StuScoreCourselist", model);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else  //第一请求才做的操作
                {
                    int currentYear;
                    var yearStr = ConfigurationManager.AppSettings["xinsheng"];
                    if (string.IsNullOrWhiteSpace(yearStr))
                    {
                        currentYear = DateTime.Now.Year;
                    }
                    else
                    {
                        currentYear = Convert.ToInt32(yearStr);
                    }

                    ViewBag.scale = yunEntities.YD_Edu_ScoreScale.FirstOrDefault();

                    ViewBag.currentYear = currentYear;

                    ViewBag.totalCount = 0;
                    ViewBag.errorCount = 0;

                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目 
                    var model = new PagedList<ScoreInputDto>(new List<ScoreInputDto>(), id, 40, 0);
                    return View(model);
                }
            }
        }

        ///成绩单导出学生成绩
        public ActionResult StudentScoreDownload()
        {

            //根据下列条件筛选出学生
            //1.学生年份 2.函授站 3.层次 4.形式 5.专业
            //然后从中删去已经被录分的学生
            //然后做成excel
            int stuYear = Convert.ToInt32(Request["year"]);//学生年份 例 2018
            int schoolid = Convert.ToInt32(Request["SubSchool"]);//函授站
            int eduid = Convert.ToInt32(Request["EduType"]);//层次 例 高起本
            int stuTypeid = Convert.ToInt32(Request["StuType"]);//形式 例 函授
            int majorlibid = Convert.ToInt32(Request["MajorLibrary"]);//专业
            int term = Convert.ToInt32(Request["team"]);//学期 最大1-10

            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            List<int> statelist = new List<int>();
            if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())//如果是华东交通大学,然后筛选学生
            {
                statelist = new List<int> { 1, 6, 7, 8 };
            }
            else
            {

                statelist = new List<int> { 1, 7, 8 };
            }

            using (var db = new IYunEntities())
            {
                var termStu = db.YD_Edu_Score.Include(e => e.YD_Sts_StuInfo).Where(
                    e => e.YD_Sts_StuInfo.y_inYear == stuYear &&
                    e.YD_Sts_StuInfo.y_subSchoolId == schoolid &&
                    e.y_term == term
                    ).Select(e => new { stuId = e.y_stuId, term = e.y_term, normalScore = e.y_normalScore, termScore = e.y_termScore, courseId = e.y_courseId }).Distinct().ToList();//已经被添加成绩的学生成绩列表(带学期)


                var students = db.YD_Sts_StuInfo.Include(e => e.YD_Edu_Major)//贪婪加载major表
                    .Where(e =>
                        e.y_inYear == stuYear && //入学年份
                        e.y_subSchoolId == schoolid && //函授站
                        e.YD_Edu_Major.y_eduTypeId == eduid &&//层次
                        e.YD_Edu_Major.y_stuTypeId == stuTypeid &&//形式
                        e.YD_Edu_Major.y_majorLibId == majorlibid &&//专业名
                        e.y_isdel == 1 &&
                        statelist.Contains(e.y_stuStateId)
                    ).ToList();

                var courses = db.YD_TeaPlan_ClassCourseDes.Include(e => e.YD_TeaPlan_Class).Include(e => e.YD_TeaPlan_Class.YD_Edu_Major)//贪婪加载一波
                    .Where(e =>
                    e.YD_TeaPlan_Class.y_subSchoolId == schoolid &&
                    e.YD_TeaPlan_Class.y_year == stuYear &&
                    e.YD_TeaPlan_Class.YD_Edu_Major.y_majorLibId == majorlibid &&
                    e.YD_TeaPlan_Class.YD_Edu_Major.y_eduTypeId == eduid &&
                    e.YD_TeaPlan_Class.YD_Edu_Major.y_stuTypeId == stuTypeid).OrderBy(e => e.y_team).ThenBy(e => e.y_course);

                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow row = sheet.CreateRow(0);
                sheet.SetColumnWidth(0, 10 * 256);
                sheet.SetColumnWidth(1, 10 * 256);
                sheet.SetColumnWidth(2, 10 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 10 * 256);
                sheet.SetColumnWidth(5, 10 * 256);
                sheet.SetColumnWidth(6, 10 * 256);
                sheet.SetColumnWidth(7, 10 * 256);
                sheet.SetColumnWidth(8, 10 * 256);

                row.CreateCell(0).SetCellValue("学号");
                row.CreateCell(1).SetCellValue("姓名");
                row.CreateCell(2).SetCellValue("专业");
                row.CreateCell(3).SetCellValue("层次");
                row.CreateCell(4).SetCellValue("课程ID");
                row.CreateCell(5).SetCellValue("课程");
                row.CreateCell(6).SetCellValue("学期");
                row.CreateCell(7).SetCellValue("平时成绩");
                row.CreateCell(8).SetCellValue("期末成绩");
                int rowNum = 1;
                foreach (var stu in students)
                {
                    foreach (var course in courses)
                    {
                        var model = termStu.FirstOrDefault(s => s.stuId == stu.id && s.term == course.y_team && s.courseId == course.id);
                        if (model != null)
                        {
                            row = sheet.CreateRow(rowNum);
                            row.CreateCell(0).SetCellValue(stu.id.ToString());
                            row.CreateCell(1).SetCellValue(stu.y_name);
                            row.CreateCell(2).SetCellValue(course.YD_TeaPlan_Class.YD_Edu_Major.YD_Edu_MajorLibrary.y_name);
                            row.CreateCell(3).SetCellValue(course.YD_TeaPlan_Class.YD_Edu_Major.YD_Edu_EduType.y_name);
                            row.CreateCell(4).SetCellValue(course.y_course);
                            row.CreateCell(5).SetCellValue(course.YD_Edu_Course.y_name);
                            row.CreateCell(6).SetCellValue(course.y_team);
                            row.CreateCell(7).SetCellValue(model.normalScore.ToString());
                            row.CreateCell(8).SetCellValue(model.termScore.ToString());
                            rowNum++;
                        }
                    }
                }

                JsonResult jsonResult = Json(new { status = "error" });

                var dirPath = Server.MapPath("~/File/");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/成绩导出表" + DateTime.Now.ToString("yyMMdd") + new Random().Next(0, 9999) + ".xls";
                var fileName3 = dirPath + filename1;

                //将工作簿写入文件
                using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs2);
                    string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File" + filename1;
                    jsonResult = Json(new { status = "ok", url = url });
                    return jsonResult;
                }
            }
        }

        //学籍信息页面导出成绩
        public ActionResult StudentInfoScoreDownload()
        {
            int stuYear = Convert.ToInt32(Request["EnrollYear"]);//学生年份 例 2018
            int schoolid = Convert.ToInt32(Request["SubSchool"]);//函授站
            int eduid = Convert.ToInt32(Request["EduType"]);//层次 例 高起本
            int stuTypeid = Convert.ToInt32(Request["StuType"]);//形式 例 函授
            int majorlibid = Convert.ToInt32(Request["MajorLibrary"]);//专业
            int state = Convert.ToInt32(Request["StuState"]);
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            var filterValue = Convert.ToString(Request["namenumcard"]);

            using (var db = new IYunEntities())
            {
                if (stuYear == 0)//如果入学年份选择了"请选择"
                {
                    return Json(new { status = "error" });
                }
                var studentQueryable = db.YD_Sts_StuInfo.Include(e => e.YD_Edu_Major).Include(e => e.YD_Sys_SubSchool)//贪婪加载major表
                    .Where(e =>
                        e.y_inYear == stuYear && //入学年份
                        e.y_isdel == 1 //软删除
                    );
                if (state != 0)//如果学籍状态为"请选择"
                {
                    studentQueryable = studentQueryable.Where(e => e.y_stuStateId == state);
                }
                if (majorlibid != 0)//专业
                {
                    studentQueryable = studentQueryable.Where(e => e.YD_Edu_Major.y_majorLibId == majorlibid);
                }
                if (stuTypeid != 0)//形式
                {
                    studentQueryable = studentQueryable.Where(e => e.YD_Edu_Major.y_stuTypeId == stuTypeid);
                }
                if (eduid != 0)//层次
                {
                    studentQueryable = studentQueryable.Where(e => e.YD_Edu_Major.y_eduTypeId == eduid);
                }
                if (schoolid != 0)//函授站
                {
                    studentQueryable = studentQueryable.Where(e => e.y_subSchoolId == schoolid);
                }
                if (!string.IsNullOrWhiteSpace(filterValue))//姓名、身份证、准考证、学号过滤
                {
                    studentQueryable = studentQueryable.Where(e => e.y_name.Contains(filterValue) || e.y_cardId.Contains(filterValue) || e.y_examNum.Contains(filterValue) || e.y_stuNum.Contains(filterValue));
                }

                var students = studentQueryable.ToList();
                var studentIds = students.Select(e => e.id);

                var scores = db.YD_Edu_Score.Where(e => studentIds.Contains(e.y_stuId))
                    .Include(e => e.YD_Edu_Course).ToList();

                List<YD_Edu_Score> scoreList = new List<YD_Edu_Score>();

                var scoreGroups = scores.GroupBy(e => new { e.y_courseId, e.y_stuId });//分组后取最新的数据
                foreach (var item in scoreGroups)
                {
                    var data = item.OrderByDescending(e => e.id).FirstOrDefault();
                    if (data != null)
                    {
                        scoreList.Add(data);
                    }
                }

                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow row = sheet.CreateRow(0);
                sheet.SetColumnWidth(0, 10 * 256);
                sheet.SetColumnWidth(1, 10 * 256);
                sheet.SetColumnWidth(2, 10 * 256);
                sheet.SetColumnWidth(3, 10 * 256);
                sheet.SetColumnWidth(4, 10 * 256);
                sheet.SetColumnWidth(5, 10 * 256);
                sheet.SetColumnWidth(6, 10 * 256);
                sheet.SetColumnWidth(7, 10 * 256);
                sheet.SetColumnWidth(8, 10 * 256);

                row.CreateCell(0).SetCellValue("学号");
                row.CreateCell(1).SetCellValue("姓名");
                row.CreateCell(2).SetCellValue("函授站");
                row.CreateCell(3).SetCellValue("课程");
                row.CreateCell(4).SetCellValue("学期");
                row.CreateCell(5).SetCellValue("平时成绩");
                row.CreateCell(6).SetCellValue("期末成绩");
                row.CreateCell(7).SetCellValue("总分");
                int rowNum = 1;
                int sheetNum = 0;
                foreach (var stu in students)
                {
                    foreach (var score in scoreList.Where(e => e.y_stuId == stu.id).OrderBy(e => e.y_term))
                    {
                        if (rowNum >= 60000)
                        {
                            sheetNum++;
                            sheet = workbook.CreateSheet(sheetNum.ToString());
                            rowNum = 0;
                        }
                        row = sheet.CreateRow(rowNum);
                        row.CreateCell(0).SetCellValue(stu.y_stuNum);
                        row.CreateCell(1).SetCellValue(stu.y_name);
                        row.CreateCell(2).SetCellValue(stu.YD_Sys_SubSchool.y_name);
                        row.CreateCell(3).SetCellValue(score.YD_Edu_Course.y_name);
                        row.CreateCell(4).SetCellValue(score.y_term);
                        row.CreateCell(5).SetCellValue(score.y_normalScore.ToString());
                        row.CreateCell(6).SetCellValue(score.y_termScore.ToString());
                        row.CreateCell(7).SetCellValue(score.y_totalScore.ToString());
                        rowNum++;
                    }
                }

                JsonResult jsonResult = Json(new { status = "error" });

                var dirPath = Server.MapPath("~/File/");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/成绩导出表" + DateTime.Now.ToString("yyMMdd") + new Random().Next(0, 9999) + ".xls";
                var fileName3 = dirPath + filename1;

                //将工作簿写入文件
                using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs2);
                    string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File" + filename1;
                    jsonResult = Json(new { status = "ok", url = url });
                    return jsonResult;
                }
            }
        }

        public ActionResult GetScoreTest()
        {
            using (var ctx = new IYunEntities())
            {
                var stuInfo = ctx.YD_Sts_StuInfo.Where(e => e.y_inYear == 2015).AsNoTracking().ToList();
                //var stuInfo = ctx.YD_Sts_StuInfo.Where(e => e.id == 884).AsNoTracking().ToList();
                var stuInfoIds = stuInfo.Select(e => e.id);
                var scores = ctx.YD_Edu_Score.Where(e => stuInfoIds.Contains(e.y_stuId)).AsNoTracking().ToList();
                var majorIds = stuInfo.Select(e => e.y_majorId).Distinct().ToList();
                StringBuilder SqlBuilder = new StringBuilder();
                foreach (var majorId in majorIds)
                {
                    var templet = ctx.YD_TeaPlan_Class.Where(e => e.y_majorId == majorId && e.y_year == 2015).AsNoTracking().FirstOrDefault();
                    var templetCourses = ctx.YD_TeaPlan_ClassCourseDes.Where(e => e.y_classTeaPlanId == templet.id).AsNoTracking().ToList();
                    var majorStuIds = stuInfo.Where(e => e.y_majorId == majorId).Select(e => e.id);
                    foreach (var majorStuId in majorStuIds)
                    {
                        var haveScoreList = scores.Where(e => e.y_stuId == majorStuId).ToList();
                        foreach (var item in templetCourses)
                        {
                            if (!haveScoreList.Any(e => e.y_term == item.y_team && e.y_courseId == item.y_course))
                            {
                                int score = new Random().Next(60, 90);
                                Thread.Sleep(10);
                                SqlBuilder.Append($"INSERT INTO [dbo].[YD_Edu_Score]([y_stuId], [y_term], [y_normalScore], [y_termScore], [y_workScore], [y_totalScore], [y_courseId], [y_type], [y_time]) VALUES ( {majorStuId}, {item.y_team}, 63.0, 63.0, NULL, {score}, {item.y_course}, 2, '2018/01/01');");
                            }
                        }
                    }
                }
                ctx.Database.ExecuteSqlCommand(SqlBuilder.ToString());
            }

            return Content("OK");
        }

        [HttpPost]
        public ActionResult UploadUrlHandle(HttpPostedFileBase avatar)
        {
            var fileName = avatar.FileName;

            var date1 = DateTime.Now.ToString("yyyyMMddHHmmsss");


            var dirPath = Server.MapPath("~/File/" + date1);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var filePath = Server.MapPath(string.Format("~/{0}", "File/" + date1));

            string url = Path.Combine(filePath, fileName);
            avatar.SaveAs(url);
            return UpLoadStuScore("/File/" + date1 + "/" + fileName);
        }

        /// <summary>
        /// 批量导入学生成绩
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult UploadAllStudent(string fileName)
        {
            JsonResult jsonResult = Json(new { status = "error", msg = "" });
            fileName = Server.MapPath(fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook;
                if (fileName.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    return jsonResult;
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return jsonResult;
                }

                var stulist = CoreFunction.StuScoreAllExcelHandle(sheet);

                if (stulist == null)
                {
                    return Json(new { status = "error" });
                }

                using (var db = new IYunEntities())
                {
                    foreach (var stu in stulist)
                    {
                        db.YD_Edu_Score.Add(new YD_Edu_Score()
                        {
                            y_stuId = stu.StuId,
                            y_term = stu.Term,
                            y_normalScore = stu.AvgNum,
                            y_termScore = stu.EndNum,
                            y_totalScore = stu.TotalNum,
                            y_courseId = stu.CourseId,
                            y_type = 1,
                            y_time = DateTime.Now
                        });
                    }
                    db.SaveChanges();
                }
            }
            return Json(new { status = "ok" });
        }

        /// 导入学生成绩
        public ActionResult UpLoadStuScore(string fileName)
        {
            JsonResult jsonResult = Json(new { status = "error", msg = "" });
            fileName = Server.MapPath(fileName);

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook;
                if (fileName.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    return jsonResult;
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return jsonResult;
                }

                var stulist = CoreFunction.StuScoreExcelHandle(sheet);





                if (stulist == null)
                {
                    return Json(new { status = "error" });
                }

                using (var db = new IYunEntities())
                {
                    foreach (var stu in stulist)
                    {
                        db.YD_Edu_Score.Add(new YD_Edu_Score()
                        {
                            y_stuId = stu.StuId,
                            y_term = stu.Term,
                            y_normalScore = stu.AvgNum,
                            y_termScore = stu.EndNum,
                            y_totalScore = stu.TotalNum,
                            y_courseId = stu.CourseId,
                            y_type = 1,
                            y_time = DateTime.Now
                        });
                    }
                    db.SaveChanges();
                }
            }
            return Json(new { status = "ok" });
        }
        //按科目保存成绩
        [HttpPost]
        public void SaveScoreAjax(ScoreAjaxModel model)
        {
            var scorelist = model.ScoreList;
            using (var ad = new IYunEntities())
            {
                var scale = ad.YD_Edu_ScoreScale.FirstOrDefault();
                scale = scale ?? new YD_Edu_ScoreScale() { id = 0, y_normalScale = 50, y_termScale = 50 };

                var updatelist = scorelist.Where(u => u.ScoreId != 0).ToList();
                var insertlist = scorelist.Where(u => u.ScoreId == 0).ToList();
                string schoolName = ConfigurationManager.AppSettings["SchoolName"];
                insertlist.ForEach(u =>
                {
                    var user = ad.YD_Sts_StuInfo.FirstOrDefault(e => e.id == u.StuId);
                    if (schoolName != ComEnum.SchoolName.JXSFDX.ToString() || user.y_scoreOk != 1)
                    {
                        var totalscore = (u.NormalScore * Convert.ToDecimal(model.NomalBili) / 100) +
                                     (u.TermScore * Convert.ToDecimal(model.ExamBili) / 100);
                        var score = new YD_Edu_Score
                        {
                            y_courseId = u.CourseId,
                            y_stuId = u.StuId,
                            y_term = u.Term,
                            y_normalScore = u.NormalScore,
                            y_termScore = u.TermScore,
                            y_totalScore = totalscore,
                            id = 0,
                            y_workScore = null,
                            y_type = (int)ScoreType.线下,
                            y_time = DateTime.Now
                        };
                        ad.YD_Edu_Score.Add(score);
                    }
                });

                updatelist.ForEach(u =>
                {
                    var user = ad.YD_Sts_StuInfo.FirstOrDefault(e => e.id == u.StuId);
                    if (schoolName != ComEnum.SchoolName.JXSFDX.ToString() || user.y_scoreOk != 1)
                    {
                        var totalscore = (u.NormalScore * Convert.ToDecimal(model.NomalBili) / 100) +
                                     (u.TermScore * Convert.ToDecimal(model.ExamBili) / 100);

                        var score = new YD_Edu_Score
                        {
                            y_courseId = u.CourseId,
                            y_stuId = u.StuId,
                            y_term = u.Term,
                            y_normalScore = u.NormalScore,
                            y_termScore = u.TermScore,
                            y_totalScore = totalscore,
                            id = u.ScoreId,
                            y_workScore = null,
                            y_type = (int)ScoreType.线下,
                            y_time = DateTime.Now
                        };
                        ad.Entry(score).State = EntityState.Modified;
                    }
                });

                ad.SaveChanges();
            }

        }

        /// 成绩统计
        public ActionResult ScoreStatistics()
        {
            #region 权限验证
            var power = SafePowerPage("/Score/ScoreStatistics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Redirect("/AdminBase/Index");
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

            var majorStr = Request.Form["Major"];
            int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 4); //根据父栏目ID获取兄弟栏目 

                List<ScoreStatistics> resultList = new List<ScoreStatistics>();

                if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                    const int isnotdel = (int)YesOrNo.No;

                    var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();
                    var stu = ad.VW_StuInfo.Where(u => u.y_isdel == isnotdel).AsQueryable();
                    if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                    {
                        int[] stateList = { 1, 6, 7, 8 };
                        stu = ad.VW_StuInfo.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                      .AsQueryable();  //函授站存在，学籍状态为在读，未注册等
                    }
                    else
                    {
                        int[] stateList = { 1, 7, 8 };
                        stu = ad.VW_StuInfo.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                        .AsQueryable();  //函授站存在，学籍状态为在读，未注册等
                    }
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



                    var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();




                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                    if (school != 0)
                    {
                        lists = lists.Where(u => u.s.stu.y_subSchoolId == school);
                    }
                    if (major != 0)
                    {
                        lists = lists.Where(u => u.s.stu.y_majorId == major);
                    }

                    var resultList2 =
                          lists.Where(u => u.s.stu.y_inYear == year)
                              .GroupBy(
                                  u => new { u.s.stu.y_subSchoolId, u.s.stu.y_majorId, u.s.stu.schoolName, u.s.stu.majorName }).ToList();

                    resultList = resultList2.Select(
                                u =>
                                    new ScoreStatistics
                                    {
                                        SubSchoolId = u.Key.y_subSchoolId.Value,
                                        MajorId = u.Key.y_majorId,
                                        SchoolName = u.Key.schoolName,
                                        MajorName = u.Key.majorName,
                                        HasCount = u.Count(k => k.score != null && k.score.y_totalScore != 0M),
                                        TotalCount = u.Count(k => k.s.classCourse != null),
                                        IsSetTeaplan = u.All(k => k.s.classCourse != null),
                                        PassCount = u.Count(k => k.score != null && k.score.y_totalScore >= 60M),
                                    })
                            .OrderBy(u => u.SubSchoolId).ThenBy(u => u.MajorName).ToList();
                }

                ViewData["year"] = year;
                ViewData["school"] = school;
                ViewData["major"] = major;

                return View(resultList);
            }
        }

        public ActionResult ScoreStatistics_Course(int year, int major, int school)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/ScoreStatistics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Redirect("/AdminBase/Index");
            }

            #endregion

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 4); //根据父栏目ID获取兄弟栏目 



                var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu = ad.VW_StuInfo.Where(u => u.y_isdel == 1).AsQueryable();



                var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                if (schoolname == ComEnum.SchoolName.JXSFDX.ToString())
                {
                    int[] stateList = { 1, 6, 7, 8 };
                    stu = stu.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                          .AsQueryable();  //函授站不为-1，学籍状态为在读，未注册等
                }
                else
                {
                    int[] stateList = { 1, 7, 8 };

                    stu = stu.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                          .AsQueryable();  //函授站不为-1，学籍状态为在读，未注册等

                }

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

                var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                var lists = list.GroupJoin(scorelist, s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                    (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                if (school != 0)
                {
                    lists = lists.Where(u => u.s.stu.y_subSchoolId == school);
                }
                if (major != 0)
                {
                    lists = lists.Where(u => u.s.stu.y_majorId == major);
                }

                var listss =
                    lists.Where(u => u.s.stu.y_inYear == year)
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team })
                        .Select(
                            u =>
                                new ScoreStatistics_Course
                                {
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    HasCount = u.Count(k => k.score != null),
                                    TotalCount = u.Count(k => k.s.classCourse != null)
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.CourseId)
                        .ToList();


                var majorName = ad.YD_Edu_Major.Find(major).y_name;
                var schoolName = ad.YD_Sys_SubSchool.Find(school).y_name;

                ViewData["year"] = year;
                ViewData["schoolName"] = schoolName;
                ViewData["majorName"] = majorName;

                return View(listss);
            }
        }

        /// 按学生录成绩
        public ActionResult RecordScore(int id = 1)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScore");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Content("<script>alert('没有权限');window.location.href='/AdminBase/Index'</script>");
            }
            #endregion

            using (var yunEntities = new IYunEntities())
            {
                if (Request.IsAjaxRequest())
                {
                    var key = Request["key"];
                    int stuYear = Convert.ToInt32(Request["year"]);
                    int schoolid = Convert.ToInt32(Request["SubSchool"]);
                    int eduid = Convert.ToInt32(Request["EduType"]);
                    int stuTypeid = Convert.ToInt32(Request["StuType"]);
                    int majorlibid = Convert.ToInt32(Request["MajorLibrary"]);
                    const int isnotdel = (int)YesOrNo.No;
                    var list = yunEntities.VW_StuInfo.OrderByDescending(u => u.id).Where(u => u.y_isdel == isnotdel).AsQueryable();
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                    {
                        var schoolids =
                            yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId)
                                .Select(u => u.y_subSchoolId)
                                .ToList();

                        list = list.Where(u => u.y_subSchoolId.HasValue && schoolids.Contains(u.y_subSchoolId.Value));

                        ////获取该函授站录分权限
                        //var smallPowerList =
                        //    yunEntities.YD_Edu_SmallPower.Where(
                        //        u => u.y_endTime > DateTime.Now).ToList();
                        //if (smallPowerList.Count <= 0)
                        //{
                        //    var reurl = Request.UrlReferrer.ToString();
                        //    return Content("<script>alert('你没有函授站下的录分权限，有需要请联系学校管理员');window.location.href='" + reurl + "'</script>");
                        //}
                    }
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        list =
                            list.Where(
                                u =>
                                    u.y_cardId.Contains(key) || u.y_name.Contains(key) ||
                                    u.y_stuNum.Contains(key));
                    }

                    if (stuYear != 0)
                    {
                        list = list.Where(u => u.y_inYear == stuYear);
                    }
                    if (schoolid != 0)
                    {
                        list = list.Where(u => u.y_subSchoolId == schoolid);
                    }
                    if (eduid != 0)
                    {
                        list = list.Where(u => u.y_eduTypeId == eduid);
                    }
                    if (stuTypeid != 0)
                    {
                        list = list.Where(u => u.y_stuTypeId == stuTypeid);
                    }
                    if (majorlibid != 0)
                    {
                        list = list.Where(u => u.y_majorLibId == majorlibid);
                    }

                    var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize

                    return PartialView("RecordScoreList", model);
                }
                else
                {
                    var model = new PagedList<VW_StuInfo>(new List<VW_StuInfo>(), id, 40, 0);
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
                    return View(model);
                }
            }
        }

        /// 根据某个学生录入成绩
        public ActionResult StudentScore(int id)
        {
            #region 权限验证
            var power = SafePowerPage("/Score/RecordScore");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Content("<script>alert('没有权限');window.location.href='/AdminBase/Index'</script>");
            }
            #endregion

            var term = Request["term"];
            if (string.IsNullOrWhiteSpace(term))
            {
                term = "1";
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目
                var stuInfo = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == id);
                if (stuInfo == null)
                {
                    //var reurl = Request.UrlReferrer.ToString();
                    var reurl = "/AdminBase/Index";
                    return Content("<script>alert('参数有误');window.location.href='" + reurl + "'</script>");
                }
                var majorTeachPlanList = yunEntities.YD_Edu_MajorTeachPlan.Where(u => u.y_majorId == stuInfo.y_majorId && u.y_year == stuInfo.y_inYear && u.y_teaPlanType == 2).ToList();
                YD_Edu_TeachPlan teachPlan = null;
                for (var i = 0; i < majorTeachPlanList.Count; i++)
                {
                    if (majorTeachPlanList[i].YD_Edu_TeachPlan.y_term == Convert.ToInt32(term))
                    {
                        teachPlan = majorTeachPlanList[i].YD_Edu_TeachPlan;
                        break;
                    }
                }
                ViewBag.majorId = stuInfo.y_majorId;
                ViewBag.year = stuInfo.y_inYear;
                ViewBag.stuName = stuInfo.y_name;
                ViewBag.stuId = stuInfo.id;
                ViewBag.term = Convert.ToInt32(term);
                ViewBag.stuId = id;
                ViewBag.scoreScale = yunEntities.YD_Edu_ScoreScale.FirstOrDefault(u => true);
                if (teachPlan == null)
                {
                    ViewBag.isTeach = false;
                }
                else
                {
                    var y_term = Convert.ToInt32(term); //学期
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5 || YdAdminRoleId == 9)
                    {
                        //获取该函授站录分权限
                        var smallPowerList =
                            yunEntities.YD_Edu_SmallPower.Where(
                                u => u.y_endTime > DateTime.Now && u.y_subSchoolId == stuInfo.y_subSchoolId).ToList();
                        if (smallPowerList.Count <= 0)
                        {
                            var reurl = Request.UrlReferrer.ToString();
                            return Content("<script>alert('你没有该学生所在的函授站下的录分权限，有需要请联系学校管理员');window.location.href='" + reurl + "'</script>");
                        }
                        if (smallPowerList.Any(u => u.y_courseId == 0))//不限制科目
                        {
                            ViewBag.isTeach = true;

                            ViewBag.teachPlanDesList = yunEntities.VW_TeachPlanDes.Where(u => u.y_teaPlanId == teachPlan.id).ToList();
                        }
                        else
                        {
                            var courseIds = new List<int>();
                            for (var i = 0; i < smallPowerList.Count; i++)
                            {
                                courseIds.Add(Convert.ToInt32(smallPowerList[i].y_courseId));
                            }
                            ViewBag.isTeach = true;
                            ViewBag.teachPlanDesList = yunEntities.VW_TeachPlanDes.Where(u => u.y_teaPlanId == teachPlan.id && courseIds.Contains(u.y_courseId)).ToList();
                        }

                    }
                    else
                    {
                        ViewBag.isTeach = true;
                        ViewBag.teachPlanDesList = yunEntities.VW_TeachPlanDes.Where(u => u.y_teaPlanId == teachPlan.id).ToList();
                        //if (!string.IsNullOrWhiteSpace(term) && !term.Equals("0"))
                        //{
                        //    var y_term = Convert.ToInt32(term);
                        //    ViewBag.teachPlanDesList = yunEntities.VW_TeachPlanDes.Where(u => u.y_term == y_term).ToList();
                        //}
                    }

                }
                return View();
            }
        }

        public ActionResult ClassScoreAuditList(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/Score/ClassScoreAuditList");
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

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 4); //根据父栏目ID获取兄弟栏目 
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
                var shenhe = Request["shenhe"];

                var EnrollYear = Request["EnrollYear"];





                const int isnotdel = (int)YesOrNo.No;
                //var xinshenyear = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
                var schoolct = Convert.ToInt32(Request["schoolct"]);
                IQueryable<VW_StuInfo> list =
                    yunEntities.VW_StuInfo.OrderByDescending(u => u.y_majorLibId)
                        .Where(u => u.y_isdel == isnotdel && u.y_stuStateId == 1);

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

                if (!string.IsNullOrWhiteSpace(shenhe))
                {
                    var y_scoreOk = Convert.ToInt32(shenhe);
                    if (y_scoreOk == 1)
                    {
                        list = list.Where(u => u.y_scoreOk == y_scoreOk);
                    }
                    else
                    {
                        list = list.Where(u => u.y_scoreOk != 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(EnrollYear) && !EnrollYear.Equals("0"))
                {
                    var y_inYear = Convert.ToInt32(EnrollYear);
                    list = list.Where(u => u.y_inYear == y_inYear);
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
                list = list.OrderBy(e => e.y_subSchoolId);
                var model = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize     
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

        /// 班级成绩单   HttpResponse.RemoveOutputCacheItem("Score");
        //[OutputCache(CacheProfile = "Score", Duration = 300, VaryByParam = "*")]
        public ActionResult ClassScoreList()
        {
            #region 权限验证

            //var _flag = HttpRuntime.Cache["Score"];

            //HttpRuntime.Cache.Remove("Score");


            var power = SafePowerPage("/Score/ClassScoreList");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Redirect("/AdminBase/Index");
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
            var schoolName = Request.Form["subscoolname"];


            var majorStr = Request.Form["Major"];
            int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);


            var termStr = Request.Form["term"];
            int term = string.IsNullOrWhiteSpace(termStr) ? 0 : Convert.ToInt32(termStr);

            using (var ad = new IYunEntities())
            {
                ad.Configuration.LazyLoadingEnabled = false;
                ViewBag.modulePowers = GetChildModulePower(ad, 4); //根据父栏目ID获取兄弟栏目 
                ViewBag.role = YdAdminRoleId;
                var model = new List<ScoreListDto>();

                if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    int[] stateList = { 1, 6, 7, 8, 9 };


                    var classCourse = ad.YD_TeaPlan_ClassCourseDes.Include(x => x.YD_Edu_Course).AsQueryable().AsNoTracking();

                    //只限理工
                    if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString())
                    {
                        int[] classnamelist = { 939, 940, 1053, 941, 1054 };
                        classCourse = classCourse.Where(u => !classnamelist.Contains(u.y_course));
                    }
                    if (term != 0)
                    {
                        classCourse = classCourse.Where(u => u.y_team == term);
                    }

                    var stu =
                        ad.YD_Sts_StuInfo.Where(
                            u =>
                                stateList.Contains(u.y_stuStateId) &&
                                u.y_subSchoolId == school && u.y_majorId == major && u.y_inYear == year && u.y_isdel == 1).AsNoTracking()
                            .AsQueryable();

                    //if (school != 0)
                    //{
                    //    stu.Where(u=>u.y_subSchoolId ==school);
                    //}
                    //if (major !=0)
                    //{
                    //    stu.Where(u => u.y_majorId == major);
                    //}

                    //var clist = ad.YD_TeaPlan_ClassCourseDes.Include(u => u.YD_Edu_Course).Include(u => u.YD_TeaPlan_Class).AsQueryable();
                    //clist = clist.Where(u => u.YD_TeaPlan_Class.y_subSchoolId == school && u.YD_TeaPlan_Class.y_year == year);  //函授站年份筛选
                    //clist =
                    //    clist.Where(
                    //        u =>
                    //            u.YD_TeaPlan_Class.YD_Edu_Major.id == major); //专业层次筛选
                    //var smallPowerIds =
                    //    clist.GroupBy(u => new { u.y_course, u.y_courseType }).Select(u => u.Key).ToList();


                    int? nullint = null;

                    //var permissionIds = new List<ScorePermissionDto>();




                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var subSchoolIds = ad.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).AsNoTracking().Select(u => u.y_subSchoolId).ToList();
                        stu = stu.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));


                        //var dateNow = DateTime.Now;

                        //List<YD_Edu_SmallPower> smallPower = ad.YD_Edu_SmallPower.Where(
                        //    u =>
                        //        (!u.y_subSchoolId.HasValue || u.y_subSchoolId == school) &&
                        //        (!u.y_majorId.HasValue || (
                        //            u.YD_Edu_Major.id == major)
                        //            ) && u.y_endTime >= dateNow && u.y_year == year).ToList();



                        //foreach (var item in smallPower)
                        //{

                        //    List<int> data;
                        //    if (item.y_courseType != null)
                        //    {
                        //        data =
                        //            smallPowerIds.Where(u => u.y_courseType == item.y_courseType.Value)
                        //                .Select(u => u.y_course)
                        //                .ToList();
                        //    }
                        //    else
                        //    {
                        //        data = smallPowerIds.Select(u => u.y_course).ToList();
                        //    }

                        //    if (item.y_courseId != null)
                        //    {
                        //        data = data.Where(u => u == item.y_courseId.Value).ToList();
                        //    }

                        //    data.ForEach(u =>
                        //    {
                        //        permissionIds.Add(new ScorePermissionDto() { CourseId = u, Type = item.y_scorelimit });
                        //    });

                        //}
                    }
                    else
                    {
                        //smallPowerIds.Select(u => u.y_course).ToList().ForEach(u =>
                        //{
                        //    permissionIds.Add(new ScorePermissionDto() { CourseId = u, Type =null });
                        //});
                    }

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
                            (x, y) => new { stu = x.s, classCourse = y, CourseName = y.YD_Edu_Course.y_name }).ToList();

                    var stuIds = list.Select(d => d.stu.id);

                    var scorelist = ad.YD_Edu_Score.Where(e => stuIds.Contains(e.y_stuId)).AsNoTracking().OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.y_type).ThenByDescending(u=>u.id).FirstOrDefault(),s.CourseName });

                    model =
                        lists.Select(
                            u => new ScoreListDto
                            {
                                StuId = u.s.stu.id,
                                StuName = u.s.stu.y_name,
                                stunum = u.s.stu.y_stuNum,
                                CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course,
                                CourseName = u.s.classCourse == null ? "" : u.s.CourseName,
                                Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team,
                                ScoreId = u.score == null ? nullint : u.score.id,
                                NormalScore = u.score == null ? 0M : u.score.y_normalScore,
                                TermScore = u.score == null ? 0M : u.score.y_termScore,
                                TotalScore = u.score == null ? 0M : u.score.y_totalScore,
                                ScoreOk = u.s.stu.y_scoreOk == 1 ? "通过" : u.s.stu.y_scoreOk == 0 ? "不通过" : "未审核"
                            }).ToList();

                    //师大100改成85
                    if (ComEnum.SchoolName.JXSFDX.ToString() == ConfigurationManager.AppSettings["SchoolName"].ToString())
                    {

                        model = model.Select(
                           u => new ScoreListDto
                           {
                               StuId = u.StuId,
                               StuName = u.StuName,
                               stunum = u.stunum,
                               CourseId = u.CourseId,
                               CourseName = u.CourseName,
                               Team = u.Team,
                               ScoreId = u.ScoreId,
                               NormalScore = u.NormalScore,
                               TermScore = u.TermScore,
                               TotalScore = u.TotalScore,
                               //u.TotalScore > 99 ? 85 : u.TotalScore,
                               ScoreOk = u.ScoreOk
                           }).ToList();


                    }

                    //model.ForEach(u =>
                    //{
                    //    u.HasPermission = permissionIds.Any(
                    //        k =>
                    //            (k.CourseId == u.CourseId && k.Type == null) ||
                    //            k.CourseId == u.CourseId && k.Type == 1 && (u.TotalScore < 60M || u.TotalScore >= 100M));
                    //});
                }

                ViewData["year"] = year;
                ViewData["school"] = school;
                ViewData["major"] = major;
                ViewData["term"] = term;

                var schoolname = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school)?.y_name;
                ViewBag.schoolname = schoolname ?? "";
                var majorname = ad.YD_Edu_Major.FirstOrDefault(u => u.id == major)?.y_name;
                ViewBag.majorname = majorname ?? "";


                return View(model);
            }
        }

        public ActionResult SubjectScoreList()
        {
            #region 权限验证

            var power = SafePowerPage("/Score/ClassScoreList");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_select == (int)PowerState.Disable)
            {
                return Redirect("/AdminBase/Index");
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

            //去除函授站选项
            //var schoolStr = Request.Form["SubSchool"];
            //int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);
            //var schoolName = Request.Form["subscoolname"];


            var majorStr = Request.Form["Major"];
            int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);


            var termStr = Request.Form["term"];
            int term = string.IsNullOrWhiteSpace(termStr) ? 0 : Convert.ToInt32(termStr);

            using (var ad = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(ad, 4); //根据父栏目ID获取兄弟栏目 
                ViewBag.role = YdAdminRoleId;
                var model = new List<ScoreListDto>();

                if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    int[] stateList = { 1, 6, 7, 8, 9 };


                    var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                    //只限理工
                    if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString())
                    {
                        int[] classnamelist = { 939, 940, 1053, 941, 1054 };
                        classCourse = classCourse.Where(u => !classnamelist.Contains(u.y_course));
                    }
                    if (term != 0)
                    {
                        classCourse = classCourse.Where(u => u.y_team == term);
                    }

                    var stu =
                        ad.YD_Sts_StuInfo.Where(
                            u =>
                                stateList.Contains(u.y_stuStateId) && u.y_majorId == major && u.y_inYear == year && u.y_isdel == 1)
                            .AsQueryable();

                    int? nullint = null;

                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var subSchoolIds = ad.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).AsNoTracking().Select(u => u.y_subSchoolId).ToList();
                        stu = stu.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));
                    }
                    else
                    {

                    }

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

                    var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();

                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s, score = score.OrderByDescending(u => u.id).FirstOrDefault() });

                    model =
                        lists.Select(
                            u => new ScoreListDto
                            {
                                StuId = u.s.stu.id,
                                StuName = u.s.stu.y_name,
                                stunum = u.s.stu.y_stuNum,
                                CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course,
                                CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name,
                                Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team,
                                ScoreId = u.score == null ? nullint : u.score.id,
                                NormalScore = u.score == null ? 0M : u.score.y_normalScore,
                                TermScore = u.score == null ? 0M : u.score.y_termScore,
                                TotalScore = u.score == null ? 0M : u.score.y_totalScore,
                                ScoreOk = u.s.stu.y_scoreOk == 1 ? "通过" : u.s.stu.y_scoreOk == 0 ? "不通过" : "未审核"
                            }).AsNoTracking().ToList();

                    //师大100改成85
                    if (ComEnum.SchoolName.JXSFDX.ToString() == ConfigurationManager.AppSettings["SchoolName"].ToString())
                    {
                        model = model.Select(
                           u => new ScoreListDto
                           {
                               StuId = u.StuId,
                               StuName = u.StuName,
                               stunum = u.stunum,
                               CourseId = u.CourseId,
                               CourseName = u.CourseName,
                               Team = u.Team,
                               ScoreId = u.ScoreId,
                               NormalScore = u.NormalScore,
                               TermScore = u.TermScore,
                               TotalScore = u.TotalScore > 99 ? 85 : u.TotalScore,
                               ScoreOk = u.ScoreOk
                           }).ToList();
                    }
                }

                ViewData["year"] = year;
                //ViewData["school"] = school;
                ViewData["major"] = major;
                ViewData["term"] = term;

                //var schoolname = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school)?.y_name;
                //ViewBag.schoolname = schoolname ?? "";
                var majorname = ad.YD_Edu_Major.FirstOrDefault(u => u.id == major)?.y_name;
                ViewBag.majorname = majorname ?? "";

                return View(model);
            }
        }

        public ActionResult ClassScoreListPrint()
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

            var majorStr = Request.Form["Major"];
            int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);


            var termStr = Request.Form["term2"];
            int term = string.IsNullOrWhiteSpace(termStr) ? 0 : Convert.ToInt32(termStr);

            using (var ad = new IYunEntities())
            {
                var model = new List<ScoreListDto>();

                if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    int[] stateList = { 1, 6, 7, 8, 9 };


                    var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                    //只限理工
                    if (ConfigurationManager.AppSettings["SchoolName"] == ComEnum.SchoolName.JXLG.ToString())
                    {
                        int[] classnamelist = { 939, 940, 1053, 941, 1054 };
                        classCourse = classCourse.Where(u => !classnamelist.Contains(u.y_course));
                    }
                    if (term != 0)
                    {
                        classCourse = classCourse.Where(u => u.y_team == term);
                    }

                    var stu =
                        ad.YD_Sts_StuInfo.Where(
                            u =>
                                stateList.Contains(u.y_stuStateId) &&
                                u.y_subSchoolId == school && u.y_majorId == major && u.y_inYear == year && u.y_isdel == 1)
                            .AsQueryable();
                    int? nullint = null;
                    if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                    {
                        var subSchoolIds = ad.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId).ToList();
                        stu = stu.Where(u => u.y_subSchoolId.HasValue && subSchoolIds.Contains(u.y_subSchoolId.Value));

                    }
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
                            (x, y) => new { stu = x.s, classCourse = y, CourseName = y.YD_Edu_Course.y_name }).ToList();
                    var stuIds = list.Select(d => d.stu.id);

                      var scorelist = ad.YD_Edu_Score.Where(e => stuIds.Contains(e.y_stuId)).AsNoTracking().OrderBy(u => u.id).AsQueryable();
                    var lists = list.GroupJoin(scorelist,
                        s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                        score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                        (s, score) => new { s,  score = score.OrderByDescending(u => u.y_type).ThenByDescending(u => u.id).FirstOrDefault(), s.CourseName });


                    model =
                        lists.Select(
                            u => new ScoreListDto
                            {
                                StuId = u.s.stu.id,
                                StuName = u.s.stu.y_name,
                                stunum = u.s.stu.y_stuNum,
                                CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course,
                                CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name,
                                Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team,
                                ScoreId = u.score == null ? nullint : u.score.id,
                                NormalScore = u.score == null ? 0M : u.score.y_normalScore,
                                TermScore = u.score == null ? 0M : u.score.y_termScore,
                                TotalScore = u.score == null ? 0M : u.score.y_totalScore
                            }).ToList();
                    //if (ComEnum.SchoolName.JXSFDX.ToString() == ConfigurationManager.AppSettings["SchoolName"].ToString())
                    //{
                    //    model.ForEach(e =>
                    //    {
                    //        if(e.TotalScore > 99)
                    //        {
                    //            e.TotalScore = 85;
                    //        }
                    //    });
                    //}
                }

                ViewData["year"] = year;
                ViewData["school"] = school;
                ViewData["major"] = major;
                ViewData["term"] = term;

                var schoolname = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school)?.y_name;
                ViewBag.schoolname = schoolname ?? "";
                var majorname = ad.YD_Edu_Major.FirstOrDefault(u => u.id == major)?.y_name;
                ViewBag.majorname = majorname ?? "";


                return View(model);
            }
        }

        public ActionResult UploadExcel()
        {
            return View();
        }
    }
}
