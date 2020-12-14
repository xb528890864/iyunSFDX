using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IYun.Models;
using Webdiyer.WebControls.Mvc;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using IYun.Controllers.ControllerObject;
using IYun.Object;
using IYun.Views.TeaPlan.ViewObject;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Configuration;
using IYun.Common;
using System.Text;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using static NPOI.HSSF.Util.HSSFColor;
using log4net;
using System.Reflection;

namespace IYun.Controllers
{
    /// <summary>
    /// 新教学计划管理
    /// </summary>
    public class TeaPlanController : AdminBaseController
    {
        // GET: /TeaPlan/
        public string TeaPlanInsert(string fileName)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };
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
                var list = new List<ls>(); //集合

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var kc = "";
                    var zy = 0;
                    var zx = 0;
                    var ismain = "";
                    var kclx = "";
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < 15; j++)
                    {
                        var cell = row.GetCell(j);

                        #region 针对行操作

                        switch (j)
                        {
                            case 0:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    kc = cell.StringCellValue.Trim();
                                }
                                break;
                            case 1:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    zy = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 2:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    zx = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;

                            case 13:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    ismain = cell.StringCellValue.Trim();
                                }
                                break;
                            case 14:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    kclx = cell.StringCellValue.Trim();
                                }
                                break;

                            case 3:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 1,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 4:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 2,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 5:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 3,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 6:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 4,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 7:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 5,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 8:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 6,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 9:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 7,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 10:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 8,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 11:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 9,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                            case 12:
                                if (cell.CellType != CellType.BLANK) //判断是否为空
                                {
                                    var data = new ls
                                    {
                                        kc = kc,
                                        zyks = zy,
                                        zxks = zx,
                                        kctype = kclx,
                                        zgisok = ismain,
                                        term = 10,
                                        msks = Convert.ToInt32(cell.NumericCellValue)
                                    };
                                    list.Add(data);
                                }
                                break;
                        }
                        #endregion
                    }
                }
                var model =
                        FileHelper.ToDataTable(
                            list.Select(
                                u =>
                                    new ls
                                    {
                                        kc = u.kc,
                                        term = u.term,
                                        zxks = u.zxks,
                                        msks = u.msks,
                                        zyks = u.zyks,
                                        kctype = u.kctype,
                                        zgisok = u.zgisok
                                    }).ToList());
                var dirPath = Server.MapPath("~/File/Dowon");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filename1 = "/转换教学计划表" + Hz;
                var fileName3 = dirPath + filename1;
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable
                        {
                            {"kc", "课程"},
                            {"term", "学期"},
                            {"zyks", "作业课程"},
                            {"kctype", "课程类型"},
                            {"zxks", "自习课程"},
                             {"msks", "面授课程"},
                            {"zgisok", "主干课程"},
                        };
                    int t = excelHelper.DataTableToExcel(model, "sheet1", true, ht);
                    if (t > 0)
                    {
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        result.IsOk = true;
                        result.Message = url;
                        return JsonConvert.SerializeObject(result);
                    }
                    var reurl = Request.UrlReferrer.ToString();
                    result.IsOk = false;
                    return JsonConvert.SerializeObject(result);
                }
                ////将工作簿写入文件
                //    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                //    {
                //        workbook.Write(fs2);
                //        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                //        result.IsOk = true;
                //        result.Message = url;
                //        return JsonConvert.SerializeObject(result);
                //    }               
            }
        }

        public ActionResult Shouye()
        {
            return RedirectToAction("TeaPlanTemplet");
        }

        public ActionResult TeaPlanTemplet(int id = 1)
        {
            var majorLibrary = Request["MajorLibrary"];
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            using (var yunEntities = new IYunEntities())
            {
                var list =
                    yunEntities.YD_TeaPlan_Templet
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                        .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                        .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                        .OrderByDescending(u => u.id)
                        .AsQueryable();
                if (!string.IsNullOrWhiteSpace(majorLibrary) && !majorLibrary.Equals("0"))
                {
                    var majorLibraryint = Convert.ToInt32(majorLibrary);
                    list = list.Where(u => u.YD_Edu_Major.YD_Edu_MajorLibrary.id == majorLibraryint);
                }
                var lists = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.YdAdminRoleId = YdAdminRoleId;
                if (Request.IsAjaxRequest())
                    return PartialView("TeaPlanTempletList", lists);
                return View(lists);

            }
        }
        //专业教学计划添加页面
        public ActionResult TeaplanAddPage()
        {

            #region 权限验证

            var power = SafePowerPage("/teaplan/teaplantemplet");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }

            return View();
        }

        //专业教学计划添加方法
        public JsonResult TeaplanAdd(int y_majorId)
        {


            using (var ad = new IYunEntities())
            {

                var te = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.y_majorId == y_majorId);

                if (te != null) { return Json(new { msg = "此专业教学计划已存在" }); }

                var teaplan = new YD_TeaPlan_Templet()
                {
                    y_majorId = y_majorId,
                    y_teaPlanType = 1,
                    y_name = "",
                    y_remark = ""
                };



                ad.YD_TeaPlan_Templet.Add(teaplan);

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "添加失败" }); }


            }


        }

        //专业教学计划修改页面
        public ActionResult TeaplanEidtPage(int? id)
        {

            #region 权限验证

            var power = SafePowerPage("/teaplan/teaplantemplet");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewBag.entity = yunEntities.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == id);
            }




            return View();
        }

        //专业教学计划修改方法
        public JsonResult TeaplanEidt(int y_majorId, int id)
        {


            using (var ad = new IYunEntities())
            {
                var teaplan2 = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == id);

                var MajorTeaplan = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.y_majorId == y_majorId);

                if (MajorTeaplan != null) { return Json(new { msg = "此专业教学计划已存在" }); }

                teaplan2.y_majorId = y_majorId;


                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "修改失败" }); }


            }
        }

        //专业教学计划删除
        public JsonResult TeaplanDelete(int id)
        {

            using (var ad = new IYunEntities())
            {
                if (id == 0)
                {
                    return Json(new { msg = "未知错误！" });
                }

                //var sql = $"  DELETE  FROM  YD_TeaPlan_TempletCourseDes WHERE y_templetId={id} ";

                //ad.Database.ExecuteSqlCommand(sql);

                var list = ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == id).ToList();

                ad.YD_TeaPlan_TempletCourseDes.RemoveRange(list);


                var TeaPlan = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == id);

                ad.YD_TeaPlan_Templet.Remove(TeaPlan);

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "删除失败！" }); }

            }
        }


        public ActionResult TeaPlanTempletDes(int? id)
        {
            if (id.HasValue)
            {
                using (var ad = new IYunEntities())
                {
                    var list =
                        ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == id.Value)
                            .Include(u => u.YD_Edu_Course);

                    if (list.Any())
                    {
                        ViewBag.id = id.Value;
                        //对应函授站，专业
                        var major = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == id.Value);
                        if (major != null)
                        {
                            ViewBag.MajorLibraryName = major.YD_Edu_Major.YD_Edu_MajorLibrary.y_name;
                            ViewBag.edutype = major.YD_Edu_Major.YD_Edu_EduType.y_name;
                            ViewBag.stutype = major.YD_Edu_Major.YD_Edu_StuType.y_name;
                        }
                        return View(list.ToList());
                    }

                }

            }

            return Content("未找到对应教学计划信息！");
        }

        //专业教学计详情管理
        public ActionResult TeaPlanDes(int? id = 1)
        {
            var a = Request["y_templetId"];

            var term = Request["term"];

            var y_templetId = Convert.ToInt32(Request["y_templetId"]);

            ViewBag.id = y_templetId;
            ViewBag.YdAdminRoleId = YdAdminRoleId;
            using (var ad = new IYunEntities())
            {

                var list = ad.YD_TeaPlan_TempletCourseDes.Where(u => true);

                if (!string.IsNullOrWhiteSpace(term) && term != "0")
                {
                    var y_term = Convert.ToInt32(term);
                    list = list.Where(u => u.y_team == y_term);
                }

                list = list.Where(u => u.y_templetId == y_templetId).Include(u => u.YD_Edu_Course).OrderBy(u => u.id);




                if (list.Any())
                {

                    //对应函授站，专业
                    var major = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == y_templetId);
                    if (major != null)
                    {
                        ViewBag.MajorLibraryName = major.YD_Edu_Major.YD_Edu_MajorLibrary.y_name;
                        ViewBag.edutype = major.YD_Edu_Major.YD_Edu_EduType.y_name;
                        ViewBag.stutype = major.YD_Edu_Major.YD_Edu_StuType.y_name;

                    }

                }

                var lists = list.ToPagedList(id.Value, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(ad, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("TeaplanDesList", lists);
                return View(lists);
            }


        }

        //专业教学计划详情删除
        public JsonResult TeaplanDesDelete(int id)
        {

            using (var ad = new IYunEntities())
            {
                if (id == 0) { return Json(new { msg = "未知错误！" }); }


                var teaplanDes = ad.YD_TeaPlan_TempletCourseDes.FirstOrDefault(u => u.id == id);

                ad.YD_TeaPlan_TempletCourseDes.Remove(teaplanDes);

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "删除失败！" }); }

            }
        }



        //专业教学计划详情添加页面
        public ActionResult TeaPlanTempletDesAddPage(int id)
        {

            #region 权限验证

            var power = SafePowerPage("/teaplan/teaplantemplet");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }

            ViewBag.id = id;

            return View();
        }
        //专业教学计划详情添加方法
        public JsonResult TeaplanDesAdd(YD_TeaPlan_TempletCourseDes yd)
        {

            var course = Request["course"];

            if (course == "0") { return Json(new { msg = "课程不能为空" }); }

            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info($"{JsonConvert.SerializeObject(yd)}");

            using (var ad = new IYunEntities())
            {
                var yd2 = new YD_TeaPlan_TempletCourseDes()
                {
                    y_templetId = yd.y_templetId,
                    y_team = yd.y_team,
                    y_course = Convert.ToInt32(course),
                    y_selfPeriod = yd.y_selfPeriod,
                    y_teaPeriod = yd.y_teaPeriod,
                    y_taskPeriod = yd.y_taskPeriod,
                    y_courseType = yd.y_courseType,
                    y_isMain = yd.y_isMain,
                    y_selfPeriod2 = 0,
                    y_teaPeriod2 = 0
                };
                log.Info($"{JsonConvert.SerializeObject(yd2)}");

                ad.YD_TeaPlan_TempletCourseDes.Add(yd2);
                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "添加失败" }); }
            }
        }


        //专业教学计划详情修改页面
        public ActionResult TeaPlanTempletDesEditPage(int id)
        {

            #region 权限验证

            var power = SafePowerPage("/teaplan/teaplantemplet");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewBag.teaplanDes = yunEntities.YD_TeaPlan_TempletCourseDes.FirstOrDefault(u => u.id == id);
            }


            ViewBag.id = id;

            return View();
        }


        ///教学计划模板详情导出
        public string TeaPlanTempletDesDown(int? id)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

            if (id.HasValue)
            {
                using (var ad = new IYunEntities())
                {
                    var lists =
                        ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == id.Value)
                            .Include(u => u.YD_Edu_Course);

                    if (lists.Any())
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
                        cell.SetCellValue("教学进度表");

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
                        ////设置字体加粗样式
                        //font2.Boldweight = short.MaxValue;
                        //font2.FontHeight = 20 * 20;
                        //font2.Color = HSSFColor.RED.index;

                        styleCell.SetFont(font2);


                        var list = lists.GroupBy(u => u.YD_Edu_Course).OrderBy(u => u.FirstOrDefault().y_courseType).ThenBy(u => u.FirstOrDefault().id).ToList(); //唯一课程

                        var teamlist = lists.OrderBy(u => u.y_team).GroupBy(u => u.y_team).Select(u => u.Key).ToList();
                        //学期唯一

                        TeaPlanDesObj[] periodsum = new TeaPlanDesObj[teamlist.Count + 1]; //合计
                        for (int i = 0; i < periodsum.Length; i++)
                        {
                            periodsum[i] = new TeaPlanDesObj();
                        }

                        var row1 = sheet.CreateRow(1);
                        var row2 = sheet.CreateRow(2);

                        row1.CreateCell(0).SetCellValue("课程类别");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                        row1.CreateCell(1).SetCellValue("序号");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 1, 1));
                        row1.CreateCell(2).SetCellValue("课程名称");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 2));
                        row1.CreateCell(3).SetCellValue("总学时");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 3, 3));
                        row1.CreateCell(4).SetCellValue("其中");
                        row1.CreateCell(5);
                        row2.CreateCell(4).SetCellValue("自学");
                        row2.CreateCell(5).SetCellValue("面授");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 4, 5));
                        row1.CreateCell(6).SetCellValue("作业");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 6, 6));

                        foreach (var team in teamlist)
                        {
                            var cellcount = row1.PhysicalNumberOfCells;
                            row1.CreateCell(cellcount).SetCellValue("第" + team + "学期");
                            row1.CreateCell(cellcount + 1);
                            row1.CreateCell(cellcount + 2);
                            row2.CreateCell(cellcount).SetCellValue("自学");
                            row2.CreateCell(cellcount + 1).SetCellValue("面授");
                            row2.CreateCell(cellcount + 2).SetCellValue("作业");
                            sheet.AddMergedRegion(new CellRangeAddress(1, 1, cellcount, cellcount + 2));
                        }
                        var cellcount1 = row1.PhysicalNumberOfCells;
                        row1.CreateCell(cellcount1).SetCellValue("主干课程");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, cellcount1, cellcount1));
                        row1.CreateCell(cellcount1 + 1).SetCellValue("抽考课程");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, cellcount1 + 1, cellcount1 + 1));
                        //设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                        //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));

                        row1.Cells.ForEach(u => u.CellStyle = styleCell);
                        row2.Cells.ForEach(u => u.CellStyle = styleCell);

                        var index = 0;

                        foreach (CourseType item in Enum.GetValues(typeof(CourseType)))
                        {
                            var count = list.Count(u => u.First().y_courseType == (int)item);
                            if (count != 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    periodsum[0].Total += (list[index].Sum(u => u.y_selfPeriod) ?? 0) + (list[index].Sum(u => u.y_teaPeriod) ?? 0);
                                    periodsum[0].Self += list[index].Sum(u => u.y_selfPeriod) ?? 0;
                                    periodsum[0].Teach += list[index].Sum(u => u.y_teaPeriod) ?? 0;
                                    periodsum[0].Task += list[index].Sum(u => u.y_taskPeriod) ?? 0;

                                    var rowNumber = sheet.PhysicalNumberOfRows;
                                    var rowCourse = sheet.CreateRow(rowNumber);

                                    if (i == 0)
                                    {
                                        rowCourse.CreateCell(0).SetCellValue(item.ToString());
                                        sheet.AddMergedRegion(new CellRangeAddress(rowNumber, rowNumber + count - 1, 0,
                                            0));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(0);
                                    }

                                    rowCourse.CreateCell(1).SetCellValue(index + 1); //序号
                                    rowCourse.CreateCell(2).SetCellValue(list.Select(u => u.Key).ToList()[index].y_name);
                                    //课程名称
                                    rowCourse.CreateCell(3)
                                        .SetCellValue((list[index].Sum(u => u.y_selfPeriod) ?? 0) +
                                                      (list[index].Sum(u => u.y_teaPeriod) ?? 0)); //总和


                                    if (list[index].Sum(u => u.y_selfPeriod2).HasValue &&
                                        list[index].Sum(u => u.y_selfPeriod2) != 0) //判断自学有没有第二数据
                                    {
                                        rowCourse.CreateCell(4)
                                            .SetCellValue((list[index].Sum(u => u.y_selfPeriod) ?? 0) + "/" +
                                                          list[index].Sum(u => u.y_selfPeriod2));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(4)
                                            .SetCellValue(list[index].Sum(u => u.y_selfPeriod) ?? 0);
                                    }

                                    if (list[index].Sum(u => u.y_teaPeriod2).HasValue &&
                                        list[index].Sum(u => u.y_teaPeriod2) != 0) //判断面授有没有第二数据
                                    {
                                        rowCourse.CreateCell(5)
                                            .SetCellValue((list[index].Sum(u => u.y_teaPeriod) ?? 0) + "/" +
                                                          list[index].Sum(u => u.y_teaPeriod2));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(5)
                                            .SetCellValue(list[index].Sum(u => u.y_teaPeriod) ?? 0);
                                    }

                                    rowCourse.CreateCell(6)
                                        .SetCellValue(list[index].Sum(u => u.y_taskPeriod) ?? 0); //作业

                                    for (int j = 0; j < teamlist.Count; j++)
                                    {
                                        var team = teamlist[j];
                                        var data = list[index].SingleOrDefault(u => u.y_team == team);

                                        if (data != null)
                                        {
                                            if (data.y_selfPeriod2.HasValue && data.y_selfPeriod2 != 0) //判断自学有没有第二数据
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue((data.y_selfPeriod ?? 0) + "/" +
                                                                  data.y_selfPeriod2.Value);
                                            }
                                            else
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue(data.y_selfPeriod ?? 0);
                                            }

                                            if (data.y_teaPeriod2.HasValue && data.y_teaPeriod2 != 0) //判断面授有没有第二数据
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue((data.y_teaPeriod ?? 0) + "/" +
                                                                  data.y_teaPeriod2.Value);
                                            }
                                            else
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue(data.y_teaPeriod ?? 0);
                                            }
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                .SetCellValue(data.y_taskPeriod ?? 0);

                                            periodsum[j + 1].Self += data.y_selfPeriod ?? 0;
                                            periodsum[j + 1].Teach += data.y_teaPeriod ?? 0;
                                            periodsum[j + 1].Task += data.y_taskPeriod ?? 0;
                                        }
                                        else
                                        {
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                        }

                                    }

                                    var mi = list[index].FirstOrDefault();
                                    rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                        .SetCellValue(mi == null ? "×" : mi.y_isMain ? "√" : "×");

                                    var me = list[index].FirstOrDefault();
                                    rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                        .SetCellValue(me == null ? "×" : me.y_sampleexam != null ? "√" : "×");

                                    rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式

                                    index++;
                                }
                            }
                        }

                        var rowFoot = sheet.CreateRow(sheet.PhysicalNumberOfRows);
                        rowFoot.CreateCell(0).SetCellValue("小计");
                        rowFoot.CreateCell(1);
                        rowFoot.CreateCell(2);
                        sheet.AddMergedRegion(new CellRangeAddress(sheet.PhysicalNumberOfRows - 1,
                            sheet.PhysicalNumberOfRows - 1, 0, 2));
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Total);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Self);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Teach);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Task);
                        for (int j = 0; j < teamlist.Count; j++)
                        {
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Self);
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Teach);
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Task);
                        }

                        rowFoot.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式

                        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                        if (!Directory.Exists(dirPath)) //todo:改变
                        {
                            Directory.CreateDirectory(dirPath); //todo:改变
                        }
                        var filename1 = "/教学计划表" + ".xls"; //todo:改变
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

                }

            }

            return JsonConvert.SerializeObject(result);
        }

        ///教学计划模板导入
        public string TeaPlanExcelInsert(string fileName)
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

                var list = CoreFunction.TeaPlanTempletValidate(ref errorCount, sheet, styleCell);
                //验证表格的错误情况，并返回错误数量，详情参方法体内

                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/教学计划表" + Hz;
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
                    //处理数据中的专业库
                    CoreFunction.MajorLibHandle(list);
                    //处理数据中的课程
                    CoreFunction.CourseHandle(list);
                    //处理数据中的专业
                    CoreFunction.MajorHandle(list);

                    result["Message"] = list.Count.ToString();
                    result["MajorCount"] = list.GroupBy(u => u.MajorId).Count().ToString();

                    return JsonConvert.SerializeObject(result);
                }
            }
        }

        /// 班级教学计划视图
        public ActionResult SubSchoolTeaPlanTemplet(int id = 1)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }

            var majorLibrary = Request["MajorLibrary"];
            var edutype = Request["EduType"];
            var subSchool = Request["SubSchool"];
            var inyear = Request["EnrollYear"];

            using (var yunEntities = new IYunEntities())
            {
                var list =
                    yunEntities.YD_TeaPlan_Class
                        .Include(u => u.YD_Sys_SubSchool)
                        .Include(u => u.YD_Edu_Major)
                        .Include(u => u.YD_Edu_Major.YD_Edu_StuType)
                        .Include(u => u.YD_Edu_Major.YD_Edu_EduType)
                        .Include(u => u.YD_Edu_Major.YD_Edu_MajorLibrary)
                        .OrderByDescending(u => u.id)
                        .AsQueryable();

                ViewBag.adminrole = YdAdminRoleId;
                if (YdAdminRoleId == 4 || YdAdminRoleId == 5)
                {
                    var subSchoolIds = yunEntities.YD_Sys_AdminSubLink.Where(u => u.y_adminId == YdAdminId).Select(u => u.y_subSchoolId);
                    list = list.Where(u => subSchoolIds.Contains(u.y_subSchoolId));
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
                if (!string.IsNullOrWhiteSpace(edutype) && !edutype.Equals("0"))
                {
                    var edutypeint = Convert.ToInt32(edutype);
                    list = list.Where(u => u.YD_Edu_Major.y_eduTypeId == edutypeint);
                }
                if (!string.IsNullOrWhiteSpace(inyear) && !inyear.Equals("0"))
                {
                    var inyearint = Convert.ToInt32(inyear);
                    list = list.Where(u => u.y_year == inyearint);
                }
                var lists = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("SubSchoolTempletList", lists);
                return View(lists);

            }
        }

        ///班级教学计划详情
        public ActionResult SubTeaPlanTempletDes(int? id)
        {
            if (id.HasValue)
            {
                using (var ad = new IYunEntities())
                {
                    var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                    var list =
                        ad.YD_TeaPlan_ClassCourseDes.Where(u => u.y_classTeaPlanId == id.Value)
                            .Include(u => u.YD_Edu_Course);
                    if (list.Any())
                    {
                        ViewBag.id = id.Value;
                        //对应函授站，专业
                        var major = ad.YD_TeaPlan_Class.FirstOrDefault(u => u.id == id.Value);
                        if (major != null)
                        {
                            if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                            {
                                ViewBag.subschool = major.YD_Sys_SubSchool.y_nameabbreviation;
                            }
                            else
                            {
                                ViewBag.subschool = major.YD_Sys_SubSchool.y_name;
                            }
                            ViewBag.MajorLibraryName = major.YD_Edu_Major.YD_Edu_MajorLibrary.y_name;
                            ViewBag.edutype = major.YD_Edu_Major.YD_Edu_EduType.y_name;
                            ViewBag.stutype = major.YD_Edu_Major.YD_Edu_StuType.y_name;
                            ViewBag.year = major.y_year;
                        }
                        return View(list.ToList());
                    }
                }
            }
            return Content("未找到对应教学计划信息！");
        }

        ///班级教学计划模板详情导出
        public string SubTeaPlanTempletDesDown(int? id)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            if (id.HasValue)
            {
                using (var ad = new IYunEntities())
                {
                    var subschool = "";
                    var MajorLibraryName = "";
                    var edutype = "";
                    var stutype = "";
                    var year = "";

                    var lists =
                        ad.YD_TeaPlan_ClassCourseDes.Where(u => u.y_classTeaPlanId == id.Value)
                            .Include(u => u.YD_Edu_Course);

                    var major = ad.YD_TeaPlan_Class.FirstOrDefault(u => u.id == id.Value);
                    if (major != null)
                    {
                        if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                        {
                            ViewBag.subschool = major.YD_Sys_SubSchool.y_nameabbreviation;
                        }
                        else
                        {
                            ViewBag.subschool = major.YD_Sys_SubSchool.y_name;
                        }
                        MajorLibraryName = major.YD_Edu_Major.YD_Edu_MajorLibrary.y_name;
                        edutype = major.YD_Edu_Major.YD_Edu_EduType.y_name;
                        stutype = major.YD_Edu_Major.YD_Edu_StuType.y_name;
                        year = major.y_year.ToString();
                    }

                    if (lists.Any())
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
                        cell.SetCellValue(year + " " + subschool + " " + MajorLibraryName + " " + edutype + " " + stutype);

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
                        ////设置字体加粗样式
                        //font2.Boldweight = short.MaxValue;
                        //font2.FontHeight = 20 * 20;
                        //font2.Color = HSSFColor.RED.index;

                        styleCell.SetFont(font2);


                        var list = lists.GroupBy(u => u.YD_Edu_Course).OrderBy(u => u.FirstOrDefault().y_courseType).ThenBy(u => u.FirstOrDefault().id).ToList(); //唯一课程

                        var teamlist = lists.OrderBy(u => u.y_team).GroupBy(u => u.y_team).Select(u => u.Key).ToList();
                        //学期唯一

                        TeaPlanDesObj[] periodsum = new TeaPlanDesObj[teamlist.Count + 1]; //合计
                        for (int i = 0; i < periodsum.Length; i++)
                        {
                            periodsum[i] = new TeaPlanDesObj();
                        }

                        var row1 = sheet.CreateRow(1);
                        var row2 = sheet.CreateRow(2);

                        row1.CreateCell(0).SetCellValue("课程类别");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 0, 0));
                        row1.CreateCell(1).SetCellValue("序号");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 1, 1));
                        row1.CreateCell(2).SetCellValue("课程名称");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 2));
                        row1.CreateCell(3).SetCellValue("总学时");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 3, 3));
                        row1.CreateCell(4).SetCellValue("其中");
                        row1.CreateCell(5);
                        row2.CreateCell(4).SetCellValue("自学");
                        row2.CreateCell(5).SetCellValue("面授");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 4, 5));
                        row1.CreateCell(6).SetCellValue("作业");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 6, 6));

                        foreach (var team in teamlist)
                        {
                            var cellcount = row1.PhysicalNumberOfCells;
                            row1.CreateCell(cellcount).SetCellValue("第" + team + "学期");
                            row1.CreateCell(cellcount + 1);
                            row1.CreateCell(cellcount + 2);
                            row2.CreateCell(cellcount).SetCellValue("自学");
                            row2.CreateCell(cellcount + 1).SetCellValue("面授");
                            row2.CreateCell(cellcount + 2).SetCellValue("作业");
                            sheet.AddMergedRegion(new CellRangeAddress(1, 1, cellcount, cellcount + 2));
                        }
                        var cellcount1 = row1.PhysicalNumberOfCells;
                        row1.CreateCell(cellcount1).SetCellValue("主干课程");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, cellcount1, cellcount1));
                        row1.CreateCell(cellcount1 + 1).SetCellValue("抽考课程");
                        sheet.AddMergedRegion(new CellRangeAddress(1, 2, cellcount1 + 1, cellcount1 + 1));
                        //设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                        //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, row1.PhysicalNumberOfCells - 1));

                        row1.Cells.ForEach(u => u.CellStyle = styleCell);
                        row2.Cells.ForEach(u => u.CellStyle = styleCell);

                        var index = 0;

                        foreach (CourseType item in Enum.GetValues(typeof(CourseType)))
                        {
                            var count = list.Count(u => u.First().y_courseType == (int)item);
                            if (count != 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    periodsum[0].Total += (list[index].Sum(u => u.y_selfPeriod) ?? 0) + (list[index].Sum(u => u.y_teaPeriod) ?? 0);
                                    periodsum[0].Self += list[index].Sum(u => u.y_selfPeriod) ?? 0;
                                    periodsum[0].Teach += list[index].Sum(u => u.y_teaPeriod) ?? 0;
                                    periodsum[0].Task += list[index].Sum(u => u.y_taskPeriod) ?? 0;

                                    var rowNumber = sheet.PhysicalNumberOfRows;
                                    var rowCourse = sheet.CreateRow(rowNumber);

                                    if (i == 0)
                                    {
                                        rowCourse.CreateCell(0).SetCellValue(item.ToString());
                                        sheet.AddMergedRegion(new CellRangeAddress(rowNumber, rowNumber + count - 1, 0,
                                            0));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(0);
                                    }

                                    rowCourse.CreateCell(1).SetCellValue(index + 1); //序号
                                    rowCourse.CreateCell(2).SetCellValue(list.Select(u => u.Key).ToList()[index].y_name);
                                    //课程名称
                                    rowCourse.CreateCell(3)
                                        .SetCellValue((list[index].Sum(u => u.y_selfPeriod) ?? 0) +
                                                      (list[index].Sum(u => u.y_teaPeriod) ?? 0)); //总和


                                    if (list[index].Sum(u => u.y_selfPeriod2).HasValue &&
                                        list[index].Sum(u => u.y_selfPeriod2) != 0) //判断自学有没有第二数据
                                    {
                                        rowCourse.CreateCell(4)
                                            .SetCellValue((list[index].Sum(u => u.y_selfPeriod) ?? 0) + "/" +
                                                          list[index].Sum(u => u.y_selfPeriod2));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(4)
                                            .SetCellValue(list[index].Sum(u => u.y_selfPeriod) ?? 0);
                                    }

                                    if (list[index].Sum(u => u.y_teaPeriod2).HasValue &&
                                        list[index].Sum(u => u.y_teaPeriod2) != 0) //判断面授有没有第二数据
                                    {
                                        rowCourse.CreateCell(5)
                                            .SetCellValue((list[index].Sum(u => u.y_teaPeriod) ?? 0) + "/" +
                                                          list[index].Sum(u => u.y_teaPeriod2));
                                    }
                                    else
                                    {
                                        rowCourse.CreateCell(5)
                                            .SetCellValue(list[index].Sum(u => u.y_teaPeriod) ?? 0);
                                    }

                                    rowCourse.CreateCell(6)
                                        .SetCellValue(list[index].Sum(u => u.y_taskPeriod) ?? 0); //作业

                                    for (int j = 0; j < teamlist.Count; j++)
                                    {
                                        var team = teamlist[j];
                                        var data = list[index].SingleOrDefault(u => u.y_team == team);

                                        if (data != null)
                                        {
                                            if (data.y_selfPeriod2.HasValue && data.y_selfPeriod2 != 0) //判断自学有没有第二数据
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue((data.y_selfPeriod ?? 0) + "/" +
                                                                  data.y_selfPeriod2.Value);
                                            }
                                            else
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue(data.y_selfPeriod ?? 0);
                                            }

                                            if (data.y_teaPeriod2.HasValue && data.y_teaPeriod2 != 0) //判断面授有没有第二数据
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue((data.y_teaPeriod ?? 0) + "/" +
                                                                  data.y_teaPeriod2.Value);
                                            }
                                            else
                                            {
                                                rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                    .SetCellValue(data.y_teaPeriod ?? 0);
                                            }
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                                .SetCellValue(data.y_taskPeriod ?? 0);

                                            periodsum[j + 1].Self += data.y_selfPeriod ?? 0;
                                            periodsum[j + 1].Teach += data.y_teaPeriod ?? 0;
                                            periodsum[j + 1].Task += data.y_taskPeriod ?? 0;
                                        }
                                        else
                                        {
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                            rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells);
                                        }

                                    }

                                    var mi = list[index].FirstOrDefault();
                                    rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                        .SetCellValue(mi == null ? "×" : mi.y_isMain ? "√" : "×");

                                    var me = list[index].FirstOrDefault();
                                    rowCourse.CreateCell(rowCourse.PhysicalNumberOfCells)
                                        .SetCellValue(me == null ? "×" : me.y_sampleexam != null ? "√" : "×");

                                    rowCourse.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式

                                    index++;
                                }
                            }
                        }

                        var rowFoot = sheet.CreateRow(sheet.PhysicalNumberOfRows);
                        rowFoot.CreateCell(0).SetCellValue("小计");
                        rowFoot.CreateCell(1);
                        rowFoot.CreateCell(2);
                        sheet.AddMergedRegion(new CellRangeAddress(sheet.PhysicalNumberOfRows - 1,
                            sheet.PhysicalNumberOfRows - 1, 0, 2));
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Total);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Self);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Teach);
                        rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[0].Task);
                        for (int j = 0; j < teamlist.Count; j++)
                        {
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Self);
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Teach);
                            rowFoot.CreateCell(rowFoot.PhysicalNumberOfCells).SetCellValue(periodsum[j + 1].Task);
                        }

                        rowFoot.Cells.ForEach(u => u.CellStyle = styleCell); //给CELL样式

                        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                        if (!Directory.Exists(dirPath)) //todo:改变
                        {
                            Directory.CreateDirectory(dirPath); //todo:改变
                        }
                        var filename1 = "/" + year + "-" + subschool + "-" + MajorLibraryName + "-" + edutype + "-" + stutype + "班级教学计划表" + ".xls"; //todo:改变
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

                }

            }

            return JsonConvert.SerializeObject(result);
        }

        /// 年度课程执行计划-已废用
        public ActionResult YearCourseTeaPlanTemplet(int id = 1)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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

            string sql =
                "select y_course from YD_TeaPlan_ClassCourseDes  as p join " +
                "(select d.* from YD_TeaPlan_Class as d join " +
                "(select y_inYear, [y_majorId]  from YD_Fee_StuFeeTb " +
                "where y_inYear + y_stuYear >= " + (year + 1) +
                "group by[y_majorId], y_inYear) as s on s.y_inYear = d.y_year and s.y_majorId = d.y_majorId " +
                ") as d on p.y_classTeaPlanId = d.id and(p.y_team + 1) / 2 + d.y_year = " + (year + 1) +
                "group by y_course order by y_course ";

            using (var yunEntities = new IYunEntities())
            {
                var ids = yunEntities.Database.SqlQuery<int>(sql).AsQueryable();

                var list = yunEntities.YD_Edu_Course.Where(u => ids.Contains(u.id)).OrderBy(u => u.id).AsQueryable();

                var lists = list.ToPagedList(id, 10000); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("CoursePlanList", lists);
                return View(lists);
            }
        }

        /// 班级年度课程执行计划-已废用
        public ActionResult SubYearCourseTeaPlan(int id = 1)
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
            int school = string.IsNullOrWhiteSpace(yearStr) ? 0 : Convert.ToInt32(schoolStr);
            string sql = "";
            if (school == 0)
            {
                sql =
                    "select y_course from YD_TeaPlan_ClassCourseDes  as p join " +
                    "(select d.* from YD_TeaPlan_Class as d join " +
                    "(select y_inYear, [y_majorId]  from YD_Fee_StuFeeTb " +
                    "where y_inYear + y_stuYear >= " + (year + 1) +
                    "group by[y_majorId], y_inYear) as s on s.y_inYear = d.y_year and s.y_majorId = d.y_majorId " +
                    ") as d on p.y_classTeaPlanId = d.id and(p.y_team + 1) / 2 + d.y_year = " +
                    (year + 1) +
                    "group by y_course order by y_course ";
            }
            else
            {
                sql =
                    "select y_course from YD_TeaPlan_ClassCourseDes  as p join " +
                    "(select d.* from YD_TeaPlan_Class as d join " +
                    "(select y_inYear, [y_majorId]  from YD_Fee_StuFeeTb " +
                    "where y_inYear + y_stuYear >= " + (year + 1) + "  and y_subSchoolId = " + school +
                    "group by[y_majorId], y_inYear) as s on s.y_inYear = d.y_year and s.y_majorId = d.y_majorId " +
                    "and y_subSchoolId = " + school + ") as d on p.y_classTeaPlanId = d.id and(p.y_team + 1) / 2 + d.y_year = " +
                    (year + 1) +
                    "group by y_course order by y_course";
            }

            using (var yunEntities = new IYunEntities())
            {
                var ids = yunEntities.Database.SqlQuery<int>(sql).AsQueryable();

                var list = yunEntities.YD_Edu_Course.Where(u => ids.Contains(u.id)).OrderBy(u => u.id).AsQueryable();

                var lists = list.ToPagedList(id, 10000); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("CoursePlanList", lists);
                return View(lists);
            }
        }

        ///课程执行计划
        public ActionResult CoursePlan()
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }

            var yearStr = Request.Form["EnrollYear"];
            int year;
            if (string.IsNullOrWhiteSpace(yearStr))
            {
                year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            }
            else
            {
                year = Convert.ToInt32(yearStr);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
            sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
            sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
            sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
            sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
            sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
            sb.AppendLine("group by[y_majorId], y_inYear)");
            sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
            sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
            sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
            sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
            sb.AppendLine("as S on S.y_majorId = M.id ");
            sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewData["year"] = year;

                return View(list);
            }
        }

        ///课程执行计划导出
        public string CoursePlanDown(int? year)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

            if (year.HasValue)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
                sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
                sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
                sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
                sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
                sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' group by[y_majorId], y_inYear)");
                sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId  group by y_templetId, y_year, d.y_majorId) ");
                sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
                sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
                sb.AppendLine("as S on S.y_majorId = M.id ");
                sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


                using (var ad = new IYunEntities())
                {
                    var lists = ad.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                    if (lists.Any())
                    {
                        var list = lists.GroupBy(u => new
                        {
                            u.y_year,
                            u.majorName,
                            u.stuTypeName,
                            u.eduTypeName,
                            u.rs
                        }).ToList();


                        var schoolName = ConfigurationManager.AppSettings["SchoolTable"];

                        //建立空白工作簿
                        var workbook = new HSSFWorkbook();
                        //在工作簿中：建立空白工作表
                        var sheet = workbook.CreateSheet();
                        //在工作表中：建立行，参数为行号，从0计
                        var row = sheet.CreateRow(0);
                        //在行中：建立单元格，参数为列号，从0计
                        var cell = row.CreateCell(0);
                        //设置单元格内容
                        cell.SetCellValue(schoolName + year + "年度课程执行计划");

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

                        var rowTitle = sheet.CreateRow(1);
                        rowTitle.CreateCell(0).SetCellValue("年度");
                        rowTitle.CreateCell(1).SetCellValue("专业名");
                        rowTitle.CreateCell(2).SetCellValue("层次");
                        rowTitle.CreateCell(3).SetCellValue("形式");
                        rowTitle.CreateCell(4).SetCellValue("人数");
                        rowTitle.CreateCell(5).SetCellValue("学期");
                        rowTitle.CreateCell(6).SetCellValue("课程");

                        rowTitle.Cells.ToList().ForEach(u =>
                        {
                            u.CellStyle = styleCell;
                        });
                        //设置单元格的高度
                        rowTitle.Height = 20 * 20;

                        foreach (var item in list)
                        {
                            var rownum = sheet.PhysicalNumberOfRows;
                            var datalist = item.ToList();
                            var count = item.Count();
                            var teamcount = datalist.Count(u => u.y_team == datalist[0].y_team);
                            var secordcount = count - teamcount;

                            for (int i = 0; i < count; i++)
                            {
                                var rowNumber = sheet.PhysicalNumberOfRows;
                                var rowNew = sheet.CreateRow(rowNumber);

                                rowNew.CreateCell(0).SetCellValue(item.Key.y_year);
                                rowNew.CreateCell(1).SetCellValue(item.Key.majorName);
                                rowNew.CreateCell(2).SetCellValue(item.Key.eduTypeName);
                                rowNew.CreateCell(3).SetCellValue(item.Key.stuTypeName);
                                rowNew.CreateCell(4).SetCellValue(item.Key.rs);
                                rowNew.CreateCell(5).SetCellValue(datalist[i].y_team % 2 == 1 ? "上" : "下");
                                rowNew.CreateCell(6).SetCellValue(datalist[i].courseName);

                                rowNew.Cells.ToList().ForEach(u =>
                                {
                                    u.CellStyle = styleCell;
                                });
                                //设置单元格的高度
                                rowNew.Height = 20 * 20;
                            }

                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 0, 0));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 1, 1));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 2, 2));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 3, 3));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 4, 4));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + teamcount - 1, 5, 5));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum + teamcount, rownum + count - 1, 5, 5));
                        }

                        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                        if (!Directory.Exists(dirPath)) //todo:改变
                        {
                            Directory.CreateDirectory(dirPath); //todo:改变
                        }
                        var filename1 = "/教学计划表" + ".xls"; //todo:改变
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

                }

            }

            return JsonConvert.SerializeObject(result);
        }

        ///班级课程执行计划
        public ActionResult SchoolCoursePlan()
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
            int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join ");
            sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
            sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs ");
            sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
            sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
            sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by[y_majorId], y_inYear)");
            sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
            sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
            sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
            sb.AppendLine("as S on S.y_majorId = M.id ");
            sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewData["year"] = year;
                ViewData["school"] = school;

                return View(list);
            }
        }

        ///班级课程执行计划导出
        public string SchoolCoursePlanDown(int? year, int? school)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

            if (year.HasValue && school.HasValue)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
                sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
                sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
                sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
                sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
                sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
                if (school != 0)
                {
                    sb.AppendLine("and y_subSchoolId = " + school);
                }
                sb.AppendLine("group by[y_majorId], y_inYear)");
                sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
                if (school != 0)
                {
                    sb.AppendLine("and y_subSchoolId = " + school);
                }
                sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
                sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
                sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
                sb.AppendLine("as S on S.y_majorId = M.id ");
                sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


                using (var ad = new IYunEntities())
                {
                    var lists = ad.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                    if (lists.Any())
                    {
                        var list = lists.GroupBy(u => new
                        {
                            u.y_year,
                            u.majorName,
                            u.stuTypeName,
                            u.eduTypeName,
                            u.rs
                        }).ToList();
                        var schoolName = ConfigurationManager.AppSettings["SchoolTable"];
                        var schoolname = ConfigurationManager.AppSettings["SchoolName"];
                        var subschoolName = "";
                        if (school != 0)
                        {
                            var ydSysSubSchool = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school);
                            if (ydSysSubSchool != null)
                            {
                                if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                                {
                                    subschoolName = ydSysSubSchool.y_nameabbreviation;
                                }
                                else
                                {
                                    subschoolName = ydSysSubSchool.y_name;
                                }
                            }

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
                        cell.SetCellValue(schoolName + " " + subschoolName + " " + year + "年度课程执行计划");

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

                        var rowTitle = sheet.CreateRow(1);
                        rowTitle.CreateCell(0).SetCellValue("年度");
                        rowTitle.CreateCell(1).SetCellValue("专业名");
                        rowTitle.CreateCell(2).SetCellValue("层次");
                        rowTitle.CreateCell(3).SetCellValue("形式");
                        rowTitle.CreateCell(4).SetCellValue("人数");
                        rowTitle.CreateCell(5).SetCellValue("学期");
                        rowTitle.CreateCell(6).SetCellValue("课程");

                        rowTitle.Cells.ToList().ForEach(u =>
                        {
                            u.CellStyle = styleCell;
                        });
                        //设置单元格的高度
                        rowTitle.Height = 20 * 20;

                        foreach (var item in list)
                        {
                            var rownum = sheet.PhysicalNumberOfRows;
                            var datalist = item.ToList();
                            var count = item.Count();
                            var teamcount = datalist.Count(u => u.y_team == datalist[0].y_team);
                            var secordcount = count - teamcount;

                            for (int i = 0; i < count; i++)
                            {
                                var rowNumber = sheet.PhysicalNumberOfRows;
                                var rowNew = sheet.CreateRow(rowNumber);

                                rowNew.CreateCell(0).SetCellValue(item.Key.y_year);
                                rowNew.CreateCell(1).SetCellValue(item.Key.majorName);
                                rowNew.CreateCell(2).SetCellValue(item.Key.eduTypeName);
                                rowNew.CreateCell(3).SetCellValue(item.Key.stuTypeName);
                                rowNew.CreateCell(4).SetCellValue(item.Key.rs);
                                rowNew.CreateCell(5).SetCellValue(datalist[i].y_team % 2 == 1 ? "上" : "下");
                                rowNew.CreateCell(6).SetCellValue(datalist[i].courseName);

                                rowNew.Cells.ToList().ForEach(u =>
                                {
                                    u.CellStyle = styleCell;
                                });
                                //设置单元格的高度
                                rowNew.Height = 20 * 20;
                            }

                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 0, 0));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 1, 1));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 2, 2));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 3, 3));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 4, 4));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + teamcount - 1, 5, 5));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum + teamcount, rownum + count - 1, 5, 5));
                        }

                        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                        if (!Directory.Exists(dirPath)) //todo:改变
                        {
                            Directory.CreateDirectory(dirPath); //todo:改变
                        }
                        var filename1 = "/班级课程执行计划表" + ".xls"; //todo:改变
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

                }

            }

            return JsonConvert.SerializeObject(result);
        }

        //主干课程执行计划显示
        public ActionResult IsMainCoursePlan()
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
            int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
            sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
            sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
            sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
            sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
            sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by[y_majorId], y_inYear)");
            sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
            sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1) + " and des.y_isMain=1 ");
            sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
            sb.AppendLine("as S on S.y_majorId = M.id ");
            sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewData["year"] = year;
                ViewData["school"] = school;

                return View(list);
            }
        }


        //主干课程执行计划显示
        public ActionResult IsMainCoursePlanClass()
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
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
            int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
            sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
            sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
            sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
            sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
            sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by[y_majorId], y_inYear)");
            sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
            if (school != 0)
            {
                sb.AppendLine("and y_subSchoolId = " + school);
            }
            sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
            sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1) + " and des.y_isMain=1 ");
            sb.AppendLine("group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
            sb.AppendLine("as S on S.y_majorId = M.id ");
            sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目

                ViewData["year"] = year;
                ViewData["school"] = school;

                return View(list);
            }
        }




        //主干课程执行计划导出
        public string DownIsMainCoursePlan(int? year, int? school)
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

            if (year.HasValue && school.HasValue)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join");
                sb.AppendLine("(select c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
                sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id,MAX(rs) as rs");
                sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
                sb.AppendLine("(select y_inYear, [y_majorId],COUNT(*) as rs  from VW_StuInfo ");
                sb.AppendLine("where y_inYear + y_stuYear >= " + (year + 1) + " and y_subSchoolId!='' ");
                if (school != 0)
                {
                    sb.AppendLine("and y_subSchoolId = " + school);
                }
                sb.AppendLine("group by[y_majorId], y_inYear)");
                sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
                if (school != 0)
                {
                    sb.AppendLine("and y_subSchoolId = " + school);
                }
                sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
                sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
                sb.AppendLine(" group by c.rs,c.y_majorId,c.y_year, des.y_course,des.y_team)");
                sb.AppendLine("as S on S.y_majorId = M.id ");
                sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");





                using (var ad = new IYunEntities())
                {
                    var lists = ad.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                    if (lists.Any())
                    {
                        var list = lists.GroupBy(u => new
                        {
                            u.y_year,
                            u.majorName,
                            u.stuTypeName,
                            u.eduTypeName,
                            u.rs
                        }).ToList();
                        var schoolName = ConfigurationManager.AppSettings["SchoolTable"];
                        var subschoolName = "本部";
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
                        cell.SetCellValue(schoolName + " " + subschoolName + " " + year + "年度主干课程执行计划");

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

                        var rowTitle = sheet.CreateRow(1);
                        rowTitle.CreateCell(0).SetCellValue("年度");
                        rowTitle.CreateCell(1).SetCellValue("专业名");
                        rowTitle.CreateCell(2).SetCellValue("层次");
                        rowTitle.CreateCell(3).SetCellValue("形式");
                        rowTitle.CreateCell(4).SetCellValue("人数");
                        rowTitle.CreateCell(5).SetCellValue("学期");
                        rowTitle.CreateCell(6).SetCellValue("课程");

                        rowTitle.Cells.ToList().ForEach(u =>
                        {
                            u.CellStyle = styleCell;
                        });
                        //设置单元格的高度
                        rowTitle.Height = 20 * 20;

                        foreach (var item in list)
                        {
                            var rownum = sheet.PhysicalNumberOfRows;
                            var datalist = item.ToList();
                            var count = item.Count();
                            var teamcount = datalist.Count(u => u.y_team == datalist[0].y_team);
                            var secordcount = count - teamcount;

                            for (int i = 0; i < count; i++)
                            {
                                var rowNumber = sheet.PhysicalNumberOfRows;
                                var rowNew = sheet.CreateRow(rowNumber);

                                rowNew.CreateCell(0).SetCellValue(item.Key.y_year);
                                rowNew.CreateCell(1).SetCellValue(item.Key.majorName);
                                rowNew.CreateCell(2).SetCellValue(item.Key.eduTypeName);
                                rowNew.CreateCell(3).SetCellValue(item.Key.stuTypeName);
                                rowNew.CreateCell(4).SetCellValue(item.Key.rs);
                                rowNew.CreateCell(5).SetCellValue(datalist[i].y_team % 2 == 1 ? "上" : "下");
                                rowNew.CreateCell(6).SetCellValue(datalist[i].courseName);

                                rowNew.Cells.ToList().ForEach(u =>
                                {
                                    u.CellStyle = styleCell;
                                });
                                //设置单元格的高度
                                rowNew.Height = 20 * 20;
                            }

                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 0, 0));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 1, 1));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 2, 2));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 3, 3));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + count - 1, 4, 4));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + teamcount - 1, 5, 5));
                            sheet.AddMergedRegion(new CellRangeAddress(rownum + teamcount, rownum + count - 1, 5, 5));
                        }

                        var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                        if (!Directory.Exists(dirPath)) //todo:改变
                        {
                            Directory.CreateDirectory(dirPath); //todo:改变
                        }
                        var filename1 = "/主干课程执行计划表" + ".xls"; //todo:改变
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

                }

            }

            return JsonConvert.SerializeObject(result);
        }
        ///班级成绩导出
        public string ClassScoreDown()
        {
            var result = new ExcelDownDto() { IsOk = false, Message = "未知错误！" };

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

            var termStr = Request.Form["term"];
            int term = string.IsNullOrWhiteSpace(termStr) ? 0 : Convert.ToInt32(termStr);

            using (var ad = new IYunEntities())
            {
                var schoolname = ad.YD_Sys_SubSchool.FirstOrDefault(u => u.id == school)?.y_name;
                schoolname = schoolname ?? "";
                var majorname = ad.YD_Edu_Major.FirstOrDefault(u => u.id == major)?.y_name;
                majorname = majorname ?? "";

                 

                var filedata = year + " " + schoolname + " " + majorname + " " + "班级成绩单";

               
                var model = new List<ScoreListDto>();

                int[] stateList = { 1, 6, 7, 8, 9 };

                var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                if (term != 0)
                {
                    classCourse = classCourse.Where(u => u.y_team == term);
                }
                var stu =
                    ad.YD_Sts_StuInfo.Where(
                        u =>
                            stateList.Contains(u.y_stuStateId) &&
                            u.y_subSchoolId == school && u.y_majorId == major && u.y_inYear == year)
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
              //var scorelist = ad.YD_Edu_Score.OrderBy(u => u.id).AsQueryable();
                var scorelist = ad.YD_Edu_Score.Where(e => stuIds.Contains(e.y_stuId)).AsNoTracking().OrderBy(u => u.id).AsQueryable();
                var lists = list.GroupJoin(scorelist,
                    s => new { s.classCourse.y_course, s.classCourse.y_team, s.stu.id },
                    score => new { y_course = score.y_courseId, y_team = score.y_term, id = score.y_stuId },
                      (s, score) => new { s, score = score.OrderByDescending(u => u.y_type).ThenByDescending(u => u.id).FirstOrDefault(), s.CourseName });


                model =
                    lists.Select(
                        u => new ScoreListDto
                        {
                            StuId = u.s.stu.id,
                            StuName = u.s.stu.y_name,
                            stunum= u.s.stu.y_stuNum ,
                            CourseId = u.s.classCourse == null ? nullint : u.s.classCourse.y_course,
                            CourseName = u.s.classCourse == null ? "" : u.s.classCourse.YD_Edu_Course.y_name,
                            Team = u.s.classCourse == null ? nullint : u.s.classCourse.y_team,
                            ScoreId = u.score == null ? nullint : u.score.id,
                            NormalScore = u.score == null ? 0M : u.score.y_normalScore,
                            TermScore = u.score == null ? 0M : u.score.y_termScore,
                            TotalScore = u.score == null ? 0M : u.score.y_totalScore
                        }).ToList();



               
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
                         
                       }).ToList();


                }








                if (model.Any() && model.All(u => u.CourseId != null))
                {
                    var termlist = model.GroupBy(u => new
                    {
                        u.CourseId,
                        u.CourseName,
                        
                        u.Team

                    }).Select(u => u.Key).GroupBy(u => u.Team).OrderBy(u => u.Key).ToList();

                    var stulist = model.OrderBy(u => u.StuId).GroupBy(u => new
                    {
                        u.StuId,
                        u.StuName,
                        u.stunum                    


                    }).ToList();


                    //建立空白工作簿
                    var workbook = new HSSFWorkbook();
                    //在工作簿中：建立空白工作表
                    var sheet = workbook.CreateSheet();
                    //在工作表中：建立行，参数为行号，从0计
                    var row = sheet.CreateRow(0);
                    var rows = sheet.CreateRow(1);
                    //在行中：建立单元格，参数为列号，从0计
                    var cell = row.CreateCell(0);
                    var cells = rows.CreateCell(0);
                    var timeandsum = "总人数：" + stulist.Count +"           " + "制表时间：" + DateTime.Now.ToString();
                    cells.SetCellValue(timeandsum);
                    //设置单元格内容
                    cell.SetCellValue(filedata); 
                    

                    var style = workbook.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.CENTER;
                    style.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                    style.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                    style.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                    style.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                    //新建一个字体样式对象
                    var font = workbook.CreateFont();
                    //设置字体加粗样式
                    font.Boldweight = short.MaxValue;
                    font.FontHeight = 20 * 20;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style.SetFont(font);
                    //将新的样式赋给单元格
                    cell.CellStyle = style; 
                    var style3 = workbook.CreateCellStyle();
                    style3.Alignment = HorizontalAlignment.CENTER;
                    style3.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                    style3.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                    style3.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                    style3.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;

                    style3.Alignment = HorizontalAlignment.CENTER;
                    var font3 = workbook.CreateFont();
                    font3.FontHeight = 15 * 15;
                    style3.SetFont(font3);
                    cells.CellStyle = style3;


                    var style2 = workbook.CreateCellStyle();
                    style2.VerticalAlignment = VerticalAlignment.CENTER;
                    style2.Alignment = HorizontalAlignment.CENTER;
                    style2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                    //设置单元格的样式：水平对齐居中
                    style2.VerticalAlignment = VerticalAlignment.CENTER;
                    style2.Alignment = HorizontalAlignment.CENTER;
                    //新建一个字体样式对象
                    var font2 = workbook.CreateFont();
                    //设置字体加粗样式
                    font2.FontHeight = 15 * 15;
                    //使用SetFont方法将字体样式添加到单元格样式中 
                    style2.SetFont(font2);


                    //设置单元格的高度
                    row.Height = 30 * 20;
                   
                    var row1 = sheet.CreateRow(2);
                    var row2 = sheet.CreateRow(3);
                    
                    
                    row1.CreateCell(0).SetCellValue("序号");
                    row2.CreateCell(0);
                    row1.CreateCell(1).SetCellValue("学号");
                    row2.CreateCell(1);
                    row1.CreateCell(2).SetCellValue("姓名");
                    sheet.SetColumnWidth(0, 30 * 80);
                    sheet.SetColumnWidth(1, 60 * 80);
                    sheet.AddMergedRegion(new CellRangeAddress(1,1,0,25));
                    sheet.AddMergedRegion(new CellRangeAddress(2,3, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 3, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(2,3,2,2));

                  
                    foreach (var item in termlist)
                    {
                        var cellcount = row1.PhysicalNumberOfCells;
                        for (int i = 0; i < item.Count(); i++)
                        {
                            var iaa = item.ToList()[i];
                          
                            if (i == 0)
                            {
                                string items = "";
                                if (item.Key.Value ==1)
                                {
                                    items = "一";
                                }
                                if (item.Key.Value == 2)
                                {
                                    items = "二";
                                }
                                if (item.Key.Value == 3)
                                {
                                    items = "三";
                                }
                                if (item.Key.Value == 4)
                                {
                                    items = "四";
                                }
                                if (item.Key.Value == 5)
                                {
                                    items = "五";
                                }
                                if (item.Key.Value == 6)
                                {
                                    items = "六";
                                }
                                if (item.Key.Value == 7)
                                {
                                    items = "七";
                                }
                                if (item.Key.Value == 8)
                                {
                                    items = "八";
                                }
                                if (item.Key.Value == 9)
                                {
                                    items = "九";
                                }
                                if (item.Key.Value == 10)
                                {
                                    items = "十";
                                }

                                row1.CreateCell(cellcount + i).SetCellValue("第"+ items + "学期");
                            }
                            else
                            {
                                row1.CreateCell(cellcount + i);
                            }
                            row2.CreateCell(cellcount + i).SetCellValue(iaa.CourseName);
                            sheet.SetColumnWidth(cellcount + i, 30 * 20 * iaa.CourseName.Length);
                        }
                         
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, cellcount, cellcount + item.Count() - 1));
                    }

                    row1.Cells.ForEach(u => u.CellStyle = style2);
                    row2.Cells.ForEach(u => u.CellStyle = style2);
                    row1.Height = 30 * 15;
                    row2.Height = 30 * 15;
                    var index = 0;
                    foreach (var stus in stulist)
                    {
                        var rownub = sheet.PhysicalNumberOfRows;

                        var datarow = sheet.CreateRow(rownub);
                        datarow.CreateCell(0).SetCellValue(index + 1);
                        datarow.CreateCell(1).SetCellValue(stus.Key.stunum);
                        datarow.CreateCell(2).SetCellValue(stus.Key.StuName);
                        index++;
                        foreach (var item in termlist)
                        {
                        
                            foreach (var iaa in item.ToList())
                            {  
                                var cellcount = datarow.PhysicalNumberOfCells;
                                var score = stus.First(u => u.Team == iaa.Team && u.CourseId == iaa.CourseId);

                                if (score.ScoreId != null)
                                {
                                    datarow.CreateCell(cellcount).SetCellValue(Convert.ToInt32(Math.Floor(score.TotalScore)));
                                }
                                else
                                {
                                    datarow.CreateCell(cellcount).SetCellValue(0);
                                }

                            }
                        }


                        datarow.Cells.ForEach(u => u.CellStyle = style2);
                        datarow.Height = 30 * 15;
                    }

                    var totalcount = row1.PhysicalNumberOfCells;
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, totalcount - 1));


                    var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                    if (!Directory.Exists(dirPath)) //todo:改变
                    {
                        Directory.CreateDirectory(dirPath); //todo:改变
                    }
                    var filename1 = "/" + filedata.Replace(" ", "_") + ".xls"; //todo:改变
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
                else
                {
                    result.Message = "未找到教学计划";
                }

            }

            return JsonConvert.SerializeObject(result);
        }
        //勾选学生审核通过
        public JsonResult ClassScoreOk()
        {

            var ids = Request["Ids"].Split(new[] { ',' });
            int HandleCount = 0;
            using (IYunEntities IYun = new IYunEntities())
            {
                foreach(var id in ids)
                {
                    var stu= IYun.YD_Sts_StuInfo.Find(Convert.ToInt32(id));
                    if (stu.y_scoreOk != 1)
                    {
                        stu.y_scoreOk = 1;
                        HandleCount++;
                    }
                }
                IYun.SaveChanges();
                if(HandleCount == 0)
                {
                    return Json(new { status = "ok", msg = "已审核" });
                }
                return Json(new { status = "ok", msg = HandleCount + "条审核完毕" });
            }
            //var yearStr = Request.Form["EnrollYear2"];
            //int year;
            //if (string.IsNullOrWhiteSpace(yearStr))
            //{
            //    year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            //}
            //else
            //{
            //    year = Convert.ToInt32(yearStr);
            //}

            //var schoolStr = Request.Form["SubSchool"];
            //int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);

            //var majorStr = Request.Form["Major"];
            //int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);

            //using (IYunEntities IYun = new IYunEntities())
            //{
            //    int count = 0;
            //    var stus = IYun.YD_Sts_StuInfo.Where
            //        (
            //            e => e.y_inYear == year &&
            //            e.y_subSchoolId == school &&
            //            e.y_majorId == major
            //        );
            //    foreach(var stu in stus)
            //    {
            //        if (stu.y_scoreOk != 0)
            //        {
            //            stu.y_scoreOk = 1;
            //            count++;
            //        }
            //    }
            //    IYun.SaveChanges();
            //    return Json(new { status = "ok", msg = count+"条审核完毕" });
            //}

        }
        //勾选学生撤销审核
        public JsonResult ClassAllRollback()
        {
            var ids = Request["Ids"].Split(new[] { ',' });
            int HandleCount = 0;
            using (IYunEntities IYun = new IYunEntities())
            {
                foreach (var id in ids)
                {
                    var stu = IYun.YD_Sts_StuInfo.Find(Convert.ToInt32(id));
                    if (stu.y_scoreOk != 0)
                    {
                        stu.y_scoreOk = 0;
                        HandleCount++;
                    }
                }
                if (HandleCount == 0)
                {
                    return Json(new { status = "ok", msg = "审核完毕" });
                }
                IYun.SaveChanges();
                return Json(new { status = "ok", msg = HandleCount + "条撤回审核完毕" });
            }
            //var yearStr = Request.Form["EnrollYear2"];
            //int year;
            //if (string.IsNullOrWhiteSpace(yearStr))
            //{
            //    year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            //}
            //else
            //{
            //    year = Convert.ToInt32(yearStr);
            //}

            //var schoolStr = Request.Form["SubSchool"];
            //int school = string.IsNullOrWhiteSpace(schoolStr) ? 0 : Convert.ToInt32(schoolStr);

            //var majorStr = Request.Form["Major"];
            //int major = string.IsNullOrWhiteSpace(majorStr) ? 0 : Convert.ToInt32(majorStr);

            //using (IYunEntities IYun = new IYunEntities())
            //{
            //    var stus = IYun.YD_Sts_StuInfo.Where
            //        (
            //            e => e.y_inYear == year &&
            //            e.y_subSchoolId == school &&
            //            e.y_majorId == major
            //        );
            //    foreach (var stu in stus)
            //    {
            //        stu.y_scoreOk = 0;
            //    }
            //    IYun.SaveChanges();
            //}

            //return Json(new { status = "ok", msg = "全部撤回审核" });
        }
        
        public JsonResult ScoreOk(int Id)
        {
            using (IYunEntities IYun = new IYunEntities())
            {
                var stu = IYun.YD_Sts_StuInfo.SingleOrDefault(e => e.id == Id&&e.y_isdel==1);
                stu.y_scoreOk = 1;
                IYun.SaveChanges();
            }
            return Json(new { status = "ok", msg = "审核通过" });
        }

        public JsonResult ScoreBack(int Id)
        {
            using (IYunEntities IYun = new IYunEntities())
            {
                var stu = IYun.YD_Sts_StuInfo.SingleOrDefault(e => e.id == Id && e.y_isdel == 1);
                stu.y_scoreOk = 0;
                IYun.SaveChanges();
            }

            return Json(new { status = "ok", msg = "撤销审核" });
        }


        ///班级教学计划生成
        public string ClassTeaPlanInsert()
        {
            //var inyear = Request["EnrollYear2"];
            //if (string.IsNullOrWhiteSpace(inyear) && inyear.Equals("0"))
            //{
            //return Content("<script>alert(请选择年份)</script>");
            //}
            var year = 2015;
            using (var ad = new IYunEntities())
            {
                var stulist =
                    ad.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_subSchoolId != null && u.y_majorId== 448)
                        .GroupBy(u => new { u.y_majorId, u.y_subSchoolId })
                        .OrderBy(u => u.Key.y_subSchoolId)
                        .Select(u => u.Key)
                        .ToList();
                if (stulist.Any())
                {
                    var majortean = ad.YD_TeaPlan_Templet.GroupBy(u => new { u.id, u.y_majorId }).Select(u => u.Key).ToList();
                    foreach (var stu in stulist)
                    {
                        #region
                        if (majortean.Any())
                        {
                            foreach (var major in majortean)
                            {
                                if (stu.y_majorId == major.y_majorId)
                                {
                                    var sub =
                                        ad.YD_TeaPlan_Class.Any(
                                            u => u.y_majorId == major.y_majorId && u.y_subSchoolId == stu.y_subSchoolId &&
                                                u.y_templetId == major.id && u.y_year == year);
                                    if (!sub)
                                    {
                                        var classs = new YD_TeaPlan_Class
                                        {
                                            y_majorId = major.y_majorId,
                                            y_subSchoolId = (int)stu.y_subSchoolId,
                                            y_year = year,
                                            y_teaPlanType = 1,
                                            y_name = "",
                                            y_remark = "",
                                            y_templetId = major.id
                                        };
                                        ad.YD_TeaPlan_Class.Add(classs);
                                        
                                        var majordestean = ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == major.id).ToList();
                                        if (majordestean.Any())
                                        {
                                            foreach (var majordes in majordestean)
                                            {
                                                var templetDesList = new YD_TeaPlan_ClassCourseDes()
                                                {
                                                    y_course = majordes.y_course,
                                                    y_courseType = majordes.y_courseType,
                                                    y_isMain = majordes.y_isMain,
                                                    y_selfPeriod = majordes.y_selfPeriod,
                                                    y_selfPeriod2 = majordes.y_selfPeriod2,
                                                    y_taskPeriod = majordes.y_taskPeriod,
                                                    y_team = majordes.y_team,
                                                    y_teaPeriod = majordes.y_teaPeriod,
                                                    y_teaPeriod2 = majordes.y_teaPeriod2,
                                                    y_classTeaPlanId = classs.id
                                                };
                                                ad.YD_TeaPlan_ClassCourseDes.Add(templetDesList);
                                                ad.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            return "不存在教学计划";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return "没有专业教学计划";
                        }
                        #endregion
                    }
                   
                    return "ok";
                }
                else
                {
                    return "没有学生";
                }
            }
        }

        /// 历届班级教学计划生成
        public void ClassTeaPlanInsertOld()
        {
            string fileName = "D://bbb.xlsx";
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
                    return;
                }
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
                    List<TeachPlanTetm> teachlist = new List<TeachPlanTetm>();
                    var edutype = new Dictionary<string, int>();
                    var stutype = new Dictionary<string, int>();
                    var majorlib = new Dictionary<string, int>();

                    ad.YD_Edu_EduType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                      {
                          edutype.Add(u.y_name, u.id);
                      });
                    ad.YD_Edu_StuType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                      {
                          stutype.Add(u.y_name, u.id);
                      });
                    ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                      {
                          majorlib.Add(u.y_name, u.id);
                      });
                    //得到专业教学计划对应id和专业id
                    var majortean = ad.YD_TeaPlan_Templet.GroupBy(u => new { u.id, u.y_majorId }).Select(u => u.Key).ToList();
                    //得到专业id
                    var majorist = ad.YD_Edu_Major.Select(u => new { u.id, u.y_eduTypeId, u.y_stuTypeId, u.y_majorLibId }).ToList();
                    //得到年级所对应的学生专业和站点
                    var year = 2016;
                    var stulist = ad.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_subSchoolId != null)
                        .GroupBy(u => new { u.y_majorId, u.y_subSchoolId })
                        .OrderBy(u => u.Key.y_subSchoolId)
                        .Select(u => u.Key)
                        .ToList();
                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var data = new TeachPlanTetm();
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            switch (j)
                            {
                                case 0:
                                    var cell1 = row.GetCell(j); //专业
                                    data.MajorLibId = majorlib[cell1.StringCellValue.Trim()];
                                    break;
                                case 1:
                                    var cell2 = row.GetCell(j); //层次
                                    data.EduType = edutype[cell2.StringCellValue.Trim()];
                                    break;
                                case 2:
                                    var cell3 = row.GetCell(j); //形式
                                    data.StuType = stutype[cell3.StringCellValue.Trim()];
                                    //得到该专业是否存在
                                    var major = majorist.FirstOrDefault(
                                            u => u.y_eduTypeId == data.EduType && u.y_majorLibId == data.MajorLibId && u.y_stuTypeId == data.StuType);
                                    if (major != null)
                                    {
                                        data.MajorId = major.id;
                                    }
                                    //判断是否存在专业教学计划里
                                    var stuinfo = majortean.FirstOrDefault(u => u.y_majorId == data.MajorId);
                                    if (stuinfo == null)
                                    {
                                        row.Cells[0].CellStyle = styleCell;
                                        row.Cells[1].CellStyle = styleCell;
                                        row.Cells[2].CellStyle = styleCell;
                                        index++;
                                    }
                                    if (stuinfo != null) data.TempletId = stuinfo.id;
                                    var stumajor = stulist.Where(u => u.y_majorId == data.MajorId).ToList();
                                    //查询学生表专业对应的站点
                                    if (stumajor.Any())
                                    {
                                        data.schoollist = stumajor.Select(u => u.y_subSchoolId).ToList();
                                    }
                                    else
                                    {
                                        row.Cells[0].CellStyle = styleCell;
                                        row.Cells[1].CellStyle = styleCell;
                                        row.Cells[2].CellStyle = styleCell;
                                        index++;
                                    }

                                    break;
                                case 3:
                                    data.CourseId = Convert.ToInt32(row.GetCell(j).StringCellValue.Trim());
                                    break;
                                case 4:
                                    data.Team = Convert.ToInt32(row.GetCell(j).StringCellValue.Trim());
                                    break;
                            }
                        }
                        if (index == 0)
                        {
                            foreach (var item in data.schoollist)
                            {
                                var datenew = new TeachPlanTetm();
                                datenew.MajorId = data.MajorId;
                                datenew.TempletId = data.TempletId;
                                datenew.CourseId = data.CourseId;
                                datenew.Team = data.Team;
                                datenew.subschoolId = (int)item;
                                teachlist.Add(datenew);
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
                        var filename1 = "/不存在教学计划表" + Hz;
                        var fileName3 = dirPath + filename1;

                        //将工作簿写入文件
                        using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                        {
                            workbook.Write(fs2);
                            string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                            Response.Write(index);
                        }
                    }
                    var newTeaplanTempletlist =
                    teachlist.GroupBy(u => new { u.subschoolId, u.MajorId, u.TempletId }).Select(
                        u =>
                            new YD_TeaPlan_Class
                            {
                                y_majorId = u.Key.MajorId,
                                y_subSchoolId = u.Key.subschoolId,
                                y_year = year,
                                y_teaPlanType = 1,
                                y_name = "",
                                y_remark = "",
                                y_templetId = u.Key.TempletId
                            }).ToList();
                    ad.YD_TeaPlan_Class.AddRange(newTeaplanTempletlist);
                    ad.SaveChanges();
                    //班级详细教学计划
                    foreach (var item in newTeaplanTempletlist)
                    {
                        teachlist.Where(u => u.MajorId == item.y_majorId && u.subschoolId == item.y_subSchoolId)
                            .ToList()
                            .ForEach(u =>
                            {
                                u.TempletId = item.id;
                            });
                    }
                    var templetDesList = teachlist.Select(u => new YD_TeaPlan_ClassCourseDes()
                    {
                        y_course = u.CourseId,
                        y_courseType = (int)CourseType.公共基础课,
                        y_isMain = false,
                        y_selfPeriod = 0,
                        y_selfPeriod2 = 0,
                        y_taskPeriod = 0,
                        y_team = u.Team,
                        y_teaPeriod = 0,
                        y_teaPeriod2 = 0,
                        y_classTeaPlanId = u.TempletId
                    });
                    ad.YD_TeaPlan_ClassCourseDes.AddRange(templetDesList);
                    ad.SaveChanges();
                }
            }
        }



        /// <summary>
        /// 新班级教学计划导入生成
        /// </summary>
        public void ClassTeaPlanInsertNew()
        {
            string fileName = "D://NNN.xlsx";
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
                    return;
                }
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
                    List<TeachPlanTetm> teachlist = new List<TeachPlanTetm>();
                    var edutype = new Dictionary<string, int>();
                    var stutype = new Dictionary<string, int>();
                    var majorlib = new Dictionary<string, int>();

                    ad.YD_Edu_EduType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        edutype.Add(u.y_name, u.id);
                    });
                    ad.YD_Edu_StuType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        stutype.Add(u.y_name, u.id);
                    });
                    ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        majorlib.Add(u.y_name, u.id);
                    });
                    var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };
                    const double eps = 1e-10; // 精度范围
                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var data = new TeachPlanTetm();
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        for (int j = 0; j < 12; j++)
                        {
                            var cell = row.GetCell(j); //专业
                            switch (j)
                            {
                                case 0:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.MajorName = cell.StringCellValue.Trim();
                                    }
                                    break;
                                case 1:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        else if (!edutype.Keys.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.EduType = edutype[cell.StringCellValue.Trim()];
                                    }
                                    break;
                                case 2:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        else if (!stutype.Keys.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.StuType = stutype[cell.StringCellValue.Trim()];
                                    }
                                    break;
                                case 3:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.CourseName = cell.StringCellValue.Trim();
                                    }
                                    break;
                                case 4:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue <= 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        if (index == 0)
                                            data.Team = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                                case 5:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue < 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.SelfPeriod = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                                case 6:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue < 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.TeaPeriod = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                                case 7:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue < 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.TaskPeriod = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                                case 8: //课程类型
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        CourseType coursetype = CourseType.公共基础课;

                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        else if (!Enum.TryParse(cell.StringCellValue.Trim(), out coursetype))
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        if (index == 0)
                                            data.CourseType = (int)coursetype;
                                    }
                                    break;
                                case 9:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        else if (!mainCourse.Keys.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }
                                        if (index == 0)
                                            data.IsMain = mainCourse[cell.StringCellValue.Trim()];
                                    }
                                    break;
                                case 10:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue < 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.SelfPeriod2 = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                                case 11:
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else
                                    {
                                        if (cell.NumericCellValue < 0 ||
                                            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                        //传说中的判断double是否整数，效果未测试
                                        {
                                            cell.CellStyle = styleCell;
                                            index++;
                                        }

                                        if (index == 0)
                                            data.TeaPeriod2 = Convert.ToInt32(cell.NumericCellValue);
                                    }
                                    break;
                            }
                        }
                        if (index == 0)
                        {
                            teachlist.Add(data);          
                        }
                    }

                    if (index > 0)
                    {
                        var dirPath = Server.MapPath("~/File/Dowon");
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var filename1 = "/不存在教学计划表" + Hz;
                        var fileName3 = dirPath + filename1;

                        //将工作簿写入文件
                        using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                        {
                            workbook.Write(fs2);
                            string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                            Response.Write(index);
                        }
                    }
                    else
                    {
                        //处理数据中的课程
                        CourseHandle(teachlist);
                        //处理数据中的专业
                        MajorLibHandle(teachlist);
                        //处理数据库中的大专业
                        MajorHandle(teachlist);
                    }
                    
                }
            }
        }

        /// <summary>
        /// 处理Excel上传后的课程,没有则添加，有则获取ID
        /// </summary>
        /// <param name="list">list</param>
        public static void CourseHandle(List<TeachPlanTetm> list)
        {
            using (var ad = new IYunEntities())
            {
                var courseDb = ad.YD_Edu_Course.Select(u => new { u.id, u.y_name }).ToList();

                int maxcourseId = courseDb.Any() ? courseDb.Max(u => u.id) : 0;

                var lack_course = new List<string>();

                var courselist = list.GroupBy(u => u.CourseName).ToList();

                foreach (IGrouping<string, TeachPlanTetm> item in courselist)
                {
                    var course = courseDb.FirstOrDefault(u => u.y_name == item.Key);
                    if (course == null)
                    {
                        lack_course.Add(item.Key);
                    }
                    else
                    {
                        item.ToList().ForEach(u => u.CourseId = course.id);
                    }
                }

                var newcourselist =
                    lack_course.Select(item => new YD_Edu_Course() { y_name = item, y_code = "" }).ToList();

                ad.YD_Edu_Course.AddRange(newcourselist);

                ad.SaveChanges();

                string sql = "update YD_Edu_Course set y_code = id where id > " + maxcourseId;
                ad.Database.ExecuteSqlCommand(sql);

                foreach (var item in newcourselist)
                {
                    courselist.First(u => u.Key == item.y_name).ToList().ForEach(u =>
                    {
                        u.CourseId = item.id;
                    });
                }
            }
        }

        /// <summary>
        /// 处理Excel上传后的专业库,没有则添加，有则获取ID
        /// </summary>
        /// <param name="list">list</param>
        public static void MajorLibHandle(List<TeachPlanTetm> list)
        {
            using (var ad = new IYunEntities())
            {
                var majorLibDb = ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name,u.y_code }).ToList();

                int maxLibId = majorLibDb.Any() ? majorLibDb.Max(u => Convert.ToInt32(u.y_code)) : 0; //得到最大code

                var lack_MajorLib = new Dictionary<string, string>();

                var majorLiblist = list.GroupBy(u => u.MajorName).ToList();

                foreach (IGrouping<string, TeachPlanTetm> item in majorLiblist)
                {
                    var majorlib = majorLibDb.FirstOrDefault(u => u.y_name == item.Key);
                    if (majorlib == null)
                    {
                        maxLibId += 1;
                        lack_MajorLib.Add(item.Key, maxLibId.ToString());
                    }
                    else
                    {
                        item.ToList().ForEach(u => u.MajorLibId = majorlib.id);
                    }
                }

                var newmajorliblist =
                    lack_MajorLib.Select(
                        item => new YD_Edu_MajorLibrary() { y_name = item.Key, y_code = item.Value }).ToList();

                ad.YD_Edu_MajorLibrary.AddRange(newmajorliblist);

                ad.SaveChanges();

                //string sql = "update YD_Edu_MajorLibrary set y_code = id where id > " + maxLibId;
                //ad.Database.ExecuteSqlCommand(sql);

                foreach (var item in newmajorliblist)
                {
                    majorLiblist.First(u => u.Key == item.y_name).ToList().ForEach(u =>
                    {
                        u.MajorLibId = item.id;

                    });
                }
            }
        }
        /// <summary>
        /// 处理Excel上传后的专业,没有则添加，有则获取ID,并且生成班级教学计划
        /// </summary>
        /// <param name="list">list</param>
        public static void MajorHandle(List<TeachPlanTetm> list)
        {
            using (var ad = new IYunEntities())
            {

                List<TeachPlanTetm> teachlist = new List<TeachPlanTetm>();
                //得到专业教学计划对应id和专业id
                var majortean = ad.YD_TeaPlan_Templet.GroupBy(u => new { u.id, u.y_majorId }).Select(u => u.Key).ToList();
                //得到年级所对应的学生专业和站点
                var year = 2018;
                var stulist = ad.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_subSchoolId != null && u.y_isdel == 1)
                    .GroupBy(u => new { u.y_majorId, u.y_subSchoolId })
                    .OrderBy(u => u.Key.y_subSchoolId)
                    .Select(u => u.Key)
                    .ToList();


                var majorDb =
                    ad.YD_Edu_Major.Select(u => new { u.id, u.y_majorLibId, u.y_stuTypeId, u.y_eduTypeId }).ToList();

                int maxmajorId = majorDb.Any() ? majorDb.Max(u => u.id) : 0;

                var lack_major = new List<MajorKey>();


                var majorlist =
                    list.GroupBy(
                        u =>
                            new { u.MajorLibId, u.EduType, u.StuType })
                        .ToList();

                foreach (var item in majorlist)
                {
                    var major =
                        majorDb.FirstOrDefault(
                            u =>
                                u.y_majorLibId == item.Key.MajorLibId && u.y_stuTypeId == item.Key.StuType &&
                                u.y_eduTypeId == item.Key.EduType);
                    if (major == null)
                    {
                        lack_major.Add(new MajorKey()
                        {
                            y_eduTypeId = item.Key.EduType,
                            y_majorLibId = item.Key.MajorLibId,
                            y_stuTypeId = item.Key.StuType
                        });
                    }
                    else
                    {
                        item.ToList().ForEach(u =>
                        {
                            u.MajorId = major.id;
                            //判断是否存在专业教学计划里
                            var majoridteach = majortean.FirstOrDefault(m => m.y_majorId == u.MajorId);
                            if (majoridteach != null) u.TempletId = majoridteach.id;

                            var stumajor = stulist.Where(s => s.y_majorId == u.MajorId).ToList();
                            //查询学生表专业对应的站点
                            if (stumajor.Any())
                            {
                                u.schoollist = stumajor.Select(s => s.y_subSchoolId).ToList();

                                foreach (var sub in u.schoollist)
                                {
                                    var datenew = new TeachPlanTetm();
                                    datenew.MajorId = u.MajorId;
                                    datenew.TempletId = u.TempletId;
                                    datenew.CourseId = u.CourseId;
                                    datenew.Team = u.Team;
                                    datenew.subschoolId = (int)sub;
                                    datenew.SelfPeriod = u.SelfPeriod;
                                    datenew.TeaPeriod = u.TeaPeriod;
                                    datenew.TaskPeriod = u.TaskPeriod;
                                    datenew.CourseType = u.CourseType;
                                    datenew.IsMain = u.IsMain;
                                    datenew.SelfPeriod2 = u.SelfPeriod2;
                                    datenew.TeaPeriod2 = u.TeaPeriod2;
                                    teachlist.Add(datenew);
                                }

                            }
                        });

                    }
                }
                var newmajorlist = new List<YD_Edu_Major>();

                foreach (var item in lack_major)
                {
                    maxmajorId++;
                    var newmajor = new YD_Edu_Major()
                    {
                        y_majorLibId = item.y_majorLibId,
                        y_stuTypeId = item.y_stuTypeId,
                        y_eduTypeId = item.y_eduTypeId,
                        y_name = "",
                        y_code = maxmajorId.ToString(),
                        y_stuYear = 3
                    };

                    newmajorlist.Add(newmajor);

                }
                foreach (var item in newmajorlist)
                {
                    majorlist.First(
                        u =>
                            u.Key.EduType == item.y_eduTypeId && u.Key.MajorLibId == item.y_majorLibId &&
                            u.Key.StuType == item.y_stuTypeId).ToList().ForEach(u =>
                            {
                                u.MajorId = item.id;
                                //判断是否存在专业教学计划里
                                var majoridteach = majortean.FirstOrDefault(m => m.y_majorId == u.MajorId);
                                if (majoridteach != null) u.TempletId = majoridteach.id;

                                var stumajor = stulist.Where(s => s.y_majorId == u.MajorId).ToList();
                                //查询学生表专业对应的站点
                                if (stumajor.Any())
                                {
                                    u.schoollist = stumajor.Select(s => s.y_subSchoolId).ToList();

                                    foreach (var sub in u.schoollist)
                                    {
                                        var datenew = new TeachPlanTetm();
                                        datenew.MajorId = u.MajorId;
                                        datenew.TempletId = u.TempletId;
                                        datenew.CourseId = u.CourseId;
                                        datenew.Team = u.Team;
                                        datenew.subschoolId = (int)sub;
                                        datenew.SelfPeriod = u.SelfPeriod;
                                        datenew.TeaPeriod = u.TeaPeriod;
                                        datenew.TaskPeriod = u.TaskPeriod;
                                        datenew.CourseType = u.CourseType;
                                        datenew.IsMain = u.IsMain;
                                        datenew.SelfPeriod2 = u.SelfPeriod2;
                                        datenew.TeaPeriod2 = u.TeaPeriod2;
                                        teachlist.Add(datenew);
                                    }
                                }
                            });
                }

                /********************************************************************
               * 到此为止 专业获取完毕
               *
               * 此处开始 生成教学计划主表
               ********************************************************************/

                var newTeaplanTempletlist =
                  teachlist.GroupBy(u => new { u.subschoolId, u.MajorId, u.TempletId }).Select(
                      u =>
                          new YD_TeaPlan_Class
                          {
                              y_majorId = u.Key.MajorId,
                              y_subSchoolId = u.Key.subschoolId,
                              y_year = year,
                              y_teaPlanType = 1,
                              y_name = "",
                              y_remark = "",
                              y_templetId = u.Key.TempletId
                          }).ToList();
                ad.YD_TeaPlan_Class.AddRange(newTeaplanTempletlist);
                ad.SaveChanges();
                //班级详细教学计划
                foreach (var item in newTeaplanTempletlist)
                {
                    teachlist.Where(u => u.MajorId == item.y_majorId && u.subschoolId == item.y_subSchoolId)
                        .ToList()
                        .ForEach(u =>
                        {
                            u.TempletId = item.id;
                        });
                }
                var templetDesList = teachlist.Select(u => new YD_TeaPlan_ClassCourseDes()
                {
                    y_course = u.CourseId,
                    y_courseType = u.CourseType,
                    y_isMain = u.IsMain,
                    y_selfPeriod = u.SelfPeriod,
                    y_selfPeriod2 = u.SelfPeriod2,
                    y_taskPeriod = u.TaskPeriod,
                    y_team = u.Team,
                    y_teaPeriod = u.TeaPeriod,
                    y_teaPeriod2 = u.TeaPeriod2,
                    y_classTeaPlanId = u.TempletId
                });
                ad.YD_TeaPlan_ClassCourseDes.AddRange(templetDesList);
                ad.SaveChanges();
            }
        }

        /// 添加56民族代码
        public void Nation()
        {
            List<string> list = new List<string>()
            {
                "汉族",
                "蒙古族",
                "回族",
                "藏族",
                "维吾尔族",
                "苗族",
                "彝族",
                "壮族",
                "布依族",
                "朝鲜族",
                "满族",
                "侗族",
                "瑶族",
                "白族",
                "土家族",
                "哈尼族",
                "哈萨克族",
                "傣族",
                "黎族",
                "傈傈族",
                "佤族",
                "畲族",
                "高山族",
                "拉祜族",
                "水族",
                "东乡族",
                "纳西族",
                "景颇族",
                "柯尔族",
                "土族",
                "达斡尔族",
                "仫佬族",
                "羌族",
                "布朗族",
                "撒拉族",
                "毛难族",
                "仡佬族",
                "锡伯族",
                "阿昌族",
                "普米族",
                "塔吉克族",
                "怒族",
                "乌孜别克族",
                "俄罗斯族",
                "鄂温克族",
                "崩龙族",
                "保安族",
                "裕固族",
                "京族",
                "塔塔尔族",
                "独龙族",
                "鄂伦春族",
                "赫哲族",
                "门巴族",
                "珞巴族",
                "基诺族",
                "穿青族",
                "其他",
                "外国血统中国籍"
            };

            using (var ad = new IYunEntities())
            {
                var mzlist = ad.YD_Sts_Nation.Select(u => u.y_name).ToList();

                mzlist.ForEach(u =>
                {
                    if (list.Contains(u))
                    {
                        list.Remove(u);
                    }
                });

                list.ForEach(u =>
                {
                    ad.YD_Sts_Nation.Add(new YD_Sts_Nation() { y_name = u, y_code = "0" });
                });

                int t = ad.SaveChanges();
                if (t > 0)
                {
                    var nationlist = ad.YD_Sts_Nation.ToList();
                    nationlist.ForEach(u =>
                    {
                        u.y_code = u.id.ToString();
                        ad.Entry(u).State = EntityState.Modified;
                    });
                    ad.SaveChanges();
                }
            }

        }

        //根据考生号匹配地址，电话
        public void teladre()
        {
            string fileName = "D://aaa.xlsx";
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
                    return;
                }
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
                    var list = ad.YD_Sts_StuInfo.Where(u =>u.y_isdel==1).ToList();

                    //var list = ad.VW_StuInfo.Where(u => u.y_inYear == 2013 || u.y_inYear==2015).ToList();

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0);

                        var examnub = cell.StringCellValue.Trim();  //考生号

                        var stuinfo = list.FirstOrDefault(u => u.y_examNum == examnub);

                        if (stuinfo == null)
                        {
                            cell.CellStyle = styleCell;
                            index++;
                            continue;
                        }

                        //stuinfo.y_politicsId =Convert.ToInt32(row.GetCell(1).StringCellValue.Trim());
                        //stuinfo.y_nationId =Convert.ToInt32(row.GetCell(2).StringCellValue.Trim());
                        ad.Entry(stuinfo).State = EntityState.Modified;
                    }
                    ad.SaveChanges();
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
                        Response.Write(index);
                    }
                }
            }
        }

        //根据考生号匹配学号
        public void AAA()
        {
            string fileName = "D://aaa.xlsx";
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
                    return;
                }
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
                    //var list = ad.VW_StuInfo.Where(u => u.y_inYear == 2016).Select(u=>new {u.y_examNum,u.y_stuNum,u.schoolName}).ToList();

                    var list = ad.YD_Sts_StuInfo.Where(u => u.y_isdel==1).ToList();

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0);

                        var examnub = cell.StringCellValue.Trim(); 

                        var stuinfo = list.FirstOrDefault(u => u.y_cardId == examnub); //考生号

                        //var cell2 = row.GetCell(1);
                        //var stunum = cell2.StringCellValue.Trim(); //学号
                        //var stunumstuinfo= list.FirstOrDefault(u => u.y_stuNum == stunum);
                        if (stuinfo == null)
                        {
                            cell.CellStyle = styleCell;
                            index++;
                            continue;
                        }
                        //if (stunumstuinfo == null)
                        //{
                        //    cell2.CellStyle = styleCell;
                        //    index++;
                        //    continue;
                        //}
                        //if (stuinfo.y_stuNum == "")
                        //{
                        if (index == 0)
                        {
                            var sb = new StringBuilder("UPDATE [YD_Sts_StuInfo] ");
                            sb.AppendLine(" SET y_stuNum='" + row.GetCell(1).StringCellValue.Trim()+ "' WHERE id=" + stuinfo.id);
                            string sql = sb.ToString();
                            ad.Database.ExecuteSqlCommand(sql);

                            //stuinfo.y_subSchoolId =Convert.ToInt32(row.GetCell(1).StringCellValue.ToString());

                            //stuinfo.y_examNum = row.GetCell(1).StringCellValue.Trim();
                            //stuinfo.y_cardId = row.GetCell(2).StringCellValue.Trim();
                            //ad.Entry(stuinfo).State = EntityState.Modified;
                        }
                        //}
                        //t++;
                        //if (t > 1000)
                        //{
                        //    break;
                        //}
                    }
                    //int r = ad.SaveChanges();

                }
                if (index > 0)
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/学号错误表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);

                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;

                        Response.Write(index);
                    }
                }

            }
        }

        

        /// 生成学号（科技师范生成规则）
        public void CCC()
        {
            using (var yunEntities = new IYunEntities())
            {
                var year = 2018;
                var stulist = yunEntities.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_stuNum == "" && u.y_subSchoolId != null)
                    .Include(u => u.YD_Edu_Major).ToList();

                var schoollist = yunEntities.YD_Sys_SubSchool.ToList();
                foreach (var stu in stulist)
                {
                    var num = "18";
                    var school = schoollist.FirstOrDefault(u => u.id == stu.y_subSchoolId);

                    if (school != null)
                    {
                        num += school.y_code;
                    }

                    switch (stu.YD_Edu_Major.y_stuTypeId)
                    {
                        case 1://函授
                            num += "4";
                            break;//业余
                        case 2:
                            num += "2";
                            break;
                        case 6://脱产
                            num += "1";
                            break;
                        default:
                            break;
                    }
                    num += stu.YD_Edu_Major.YD_Edu_MajorLibrary.y_code;

                    stu.y_stuNum = num;
                }
                stulist.GroupBy(u => u.y_stuNum).ToList().ForEach(u =>
                {
                    var index = 1;

                    u.ToList().ForEach(k =>
                    {
                        k.y_stuNum += DDD(index, 3);
                        yunEntities.Entry(k).State = EntityState.Modified;
                        index++;
                    });
                });
                var indexs = yunEntities.SaveChanges();

                Response.Write(indexs);
            }
        }

        //生成学号（江西理工生成规则）
        public void GGG()
        {
            using (var yunEntities = new IYunEntities())
            {
                var year = 2018;
                var stulist = yunEntities.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_stuNum == "" && u.y_subSchoolId != null)
                    .Include(u => u.YD_Edu_Major).ToList();

                var schoollist = yunEntities.YD_Sys_SubSchool.ToList();
                foreach (var stu in stulist)
                {
                    var num = "18";
                    var school = schoollist.FirstOrDefault(u => u.id == stu.y_subSchoolId);

                    if (school != null)
                    {
                        num += school.y_code;
                    }
                    switch (stu.YD_Edu_Major.y_eduTypeId)
                    {
                        case 1://高起本
                            num += "2";
                            break;
                        case 2: //高起专
                            num += "4";
                            break;
                        case 4://专升本
                            num += "1";
                            break;
                        default:
                            break;
                    }
                    switch (stu.YD_Edu_Major.y_stuTypeId)
                    {
                        case 1://函授
                            num += "4";
                            break;//业余
                        case 2:
                            num += "2";
                            break;
                        case 6://脱产
                            num += "1";
                            break;
                        default:
                            break;
                    }
                    num += stu.YD_Edu_Major.YD_Edu_MajorLibrary.y_code;

                    stu.y_stuNum = num;
                }
                stulist.GroupBy(u => u.y_stuNum).ToList().ForEach(u =>
                {
                    var index = 1;

                    u.ToList().ForEach(k =>
                    {
                        k.y_stuNum += DDD(index, 4);
                        yunEntities.Entry(k).State = EntityState.Modified;
                        index++;
                    });
                });
                var indexs = yunEntities.SaveChanges();

                Response.Write(indexs);
            }
        }

        /// 生成学号随机数
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

        [ValidateInput(false)]
        public ActionResult Init()
        {
            using (var yunEntities = new IYunEntities())
            {
                IQueryable<YD_Edu_News> list = yunEntities.YD_Edu_News.OrderByDescending(u => u.id);

                var newslist = list.Where(u => u.y_type == 2).Take(8).ToList();
                var listedu = list.Where(u => u.y_type == 1).Take(8).ToList();
                var listtixi = list.Where(u => u.y_type == 3).Take(8).ToList();
                var listqita = list.Where(u => u.y_type == 4).Take(8).ToList();
                ViewBag.newslist = newslist;
                ViewBag.listedu = listedu;
                ViewBag.listtixi = listtixi;
                ViewBag.listqita = listqita;
                return View();
            }
        }

        //批量添加异动记录
        public void ADDstrange()
        {
            string fileName = "D://ttt.xlsx";
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
                    return;
                }
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

                    var list = ad.YD_Sts_StuInfo.Where(u =>u.y_isdel==1).ToList();

                  

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        var strange = new YD_Sts_StuStrange();
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0);

                        var examnub = cell.StringCellValue.Trim(); //考生号
                        //var zhuanchu = row.GetCell(1).StringCellValue.Trim(); //转出
                        //var zhuanru = row.GetCell(2).StringCellValue.Trim(); //转入专业

                        var stuinfo = list.FirstOrDefault(u => u.y_stuNum == examnub);


                        if (stuinfo == null)
                        {
                            cell.CellStyle = styleCell;
                            index++;
                            continue;
                        }
                        else
                        {
                            //var zhuanchumajor = ad.YD_Edu_Major.Where(u => u.y_eduTypeId == stuinfo.y_eduTypeId
                            //&& u.y_stuTypeId == stuinfo.y_stuTypeId && u.YD_Edu_MajorLibrary.y_name == zhuanru).Select(u=>u.id).FirstOrDefault().ToString();

                            ////转入
                            //var zhuanrumajor = ad.YD_Edu_Major.Where(u => u.y_eduTypeId == stuinfo.y_eduTypeId
                            //&& u.y_stuTypeId == stuinfo.y_stuTypeId && u.YD_Edu_MajorLibrary.y_name == zhuanru).Select(u => u.id).FirstOrDefault().ToString();

                            if (!ad.YD_Sts_StuStrange.Any(u => u.y_stuId == stuinfo.id && u.y_strangeType == 2 /*&& u.y_contentA == zhuanrumajor*/))
                            {
                                strange.y_strangeType =2;
                                strange.y_applyTime = DateTime.Now;
                                strange.y_approvalTime = DateTime.Now;
                                strange.y_approvalStatus = 2;
                                strange.y_approvalAdmin = 2;
                                strange.y_applyAdmin = 2;
                                strange.y_contentA = 135.ToString();
                                strange.y_contentB = 131.ToString();
                                strange.y_isExecutive = 1;
                                strange.y_stuId = stuinfo.id;
                                strange.y_isdel = 1;
                                ad.Entry(strange).State = EntityState.Added;
                            }
                            //var majorid = Convert.ToInt32(zhuanrumajor);
                            //stuinfo.y_majorId = 101;
                            //stuinfo.y_stuStateId = 4;
                            stuinfo.y_subSchoolId = 135;
                            ad.Entry(stuinfo).State = EntityState.Modified;
                        }
                    }

                    int r = ad.SaveChanges();

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

                        Response.Write(index);
                    }
                }

            }
        }

        //批量添加录分权限
        public string SmallPower()
        {
            string fileName = "E://录分权限最终.xlsx";
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
                    return "";
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return "";
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

                    //var list = ad.YD_Sts_StuInfo.Where(u => u.y_inYear == 2015).ToList();

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        var smallpower = new YD_Edu_SmallPower();
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0).StringCellValue.Trim();//专业id
                        var cell2 = row.GetCell(1).StringCellValue.Trim();//课程id
                        var cell3 = row.GetCell(2).StringCellValue.Trim();//学期
                        smallpower.y_adminId = 2;
                        smallpower.y_year = 2017;
                        smallpower.y_subSchoolId = null;
                        smallpower.y_majorId = Convert.ToInt32(cell);
                        smallpower.y_courseType = null;
                        smallpower.y_courseId = Convert.ToInt32(cell2);
                        smallpower.y_endTime = Convert.ToDateTime("2020-12-25 23:59:59.000");
                        smallpower.y_createTime = DateTime.Now;
                        smallpower.y_term = Convert.ToInt32(cell3); 
                        ad.Entry(smallpower).State = EntityState.Added;

                        //var examnub = cell.StringCellValue.Trim(); //学号号

                        //var stuinfo = list.FirstOrDefault(u => u.y_examNum == examnub);


                        //if (stuinfo == null)
                        //{
                        //    cell.CellStyle = styleCell;
                        //    index++;
                        //    continue;
                        //}
                        //else
                        //{   
                        //    if (!ad.YD_Edu_SmallPower.Any(u => u.y_subSchoolId == stuinfo.y_subSchoolId && u.y_majorId==stuinfo.y_majorId))
                        //    {
                        //        smallpower.y_adminId = 2;
                        //        smallpower.y_subSchoolId = stuinfo.y_subSchoolId;
                        //        smallpower.y_majorId = stuinfo.y_majorId;
                        //        ad.Entry(smallpower).State = EntityState.Added;
                        //    }
                        //}
                    }
                    int r = ad.SaveChanges();
                    if (r > 0)
                        return "添加成功";
                    else
                        return "添加失败";
                }
                //if (index > 0)
                //{
                //    var dirPath = Server.MapPath("~/File/Dowon");
                //    if (!Directory.Exists(dirPath))
                //    {
                //        Directory.CreateDirectory(dirPath);
                //    }
                //    var filename1 = "/录分权限错误表" + Hz;
                //    var fileName3 = dirPath + filename1;

                //    //将工作簿写入文件
                //    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                //    {
                //        workbook.Write(fs2);

                //        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;

                //        Response.Write(index);
                //    }
                //}

            }
        }

        //批量添加对照表
        public void ADDcorrespondence()
        {
            //string fileName = Server.MapPath("~/File/Dowon")+"/中医药16级对照表.xlsx";

            string fileName = @"G:/17级江西师范对照表模板.xlsx";
            var year = 2017;
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
                    return;
                }
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

                    //var list = ad.YD_Sts_StuInfo.Where(u => u.y_inYear == 2015).ToList();

                    var list = new List<YD_Edu_CourseCorrespondence>();

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        var strange = new YD_Edu_CourseCorrespondence();
                        if (row == null)
                        {
                            continue;
                        }

                        strange.y_inyear = year;
                        strange.y_qingshuMajorlib = row.GetCell(0).StringCellValue.Trim();
                        strange.y_qingshuEdutype = row.GetCell(1).StringCellValue.Trim();
                        strange.y_qingshuCourse = row.GetCell(2).StringCellValue.Trim();
                        strange.y_qingshuTerm = Convert.ToInt32(row.GetCell(3).ToString());
                        strange.y_MajorID = Convert.ToInt32(row.GetCell(4).ToString());
                        strange.y_CourseID = Convert.ToInt32(row.GetCell(5).ToString());
                        strange.y_Term = Convert.ToInt32(row.GetCell(6).ToString());

                        var str =
                            ad.YD_Edu_CourseCorrespondence.FirstOrDefault(
                                u => u.y_inyear == year && u.y_qingshuCourse == strange.y_qingshuCourse
                                     && u.y_qingshuEdutype == strange.y_qingshuEdutype &&
                                     u.y_qingshuMajorlib == strange.y_qingshuMajorlib &&
                                     u.y_qingshuTerm == strange.y_qingshuTerm
                                     && u.y_Term == strange.y_Term && u.y_MajorID == strange.y_MajorID &&
                                     u.y_CourseID == strange.y_CourseID
                                );
                        if (str == null)
                        {
                            list.Add(strange);
                        }
                        //ad.Entry(strange).State = EntityState.Added;
                    }
                    ad.YD_Edu_CourseCorrespondence.AddRange(list);
                    int r = ad.SaveChanges();

                }
                if (index > 0)
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/教学计划课程对应错误表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);

                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;

                        Response.Write(index);
                    }
                }

            }
        }

        public string KKK()
        {
            var fileName = "D:/教育部2014年录取新生名单.xlsx";
            var Hz = "";
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
                    return "未找到文件类型";
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return "文件未找到内容";
                }

                using (var ad = new IYunEntities())
                {
                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var ksh = row.GetCell(0).StringCellValue;
                        var score = Convert.ToInt32(row.GetCell(1).NumericCellValue);

                        var stu = ad.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == ksh);

                        if (stu != null)
                        {
                            stu.y_examScore = score;
                            ad.Entry(stu).State = EntityState.Modified;
                        }

                    }
                    var count = ad.SaveChanges();
                    return "" + count;
                }

            }

        }                                         
        //更新注册总金额及学生缴费金额--不使用实时计算
        public string WWW(int? id)
        {
            int schoolId = id.HasValue ? id.Value : 45;

            using (var ad = new IYunEntities())
            {
                IQueryable<YD_Fee_StuRegistrBatch> batchlist = ad.YD_Fee_StuRegistrBatch.Where(u => u.y_subSchoolId == schoolId).AsQueryable();
                int index = 0;

                batchlist.ToList().ForEach(a =>
                {
                    string stuid = a.y_stuid;
                    //将string数组转换成int数组
                    int[] ids =
                        Array.ConvertAll(stuid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            Convert.ToInt32);

                    List<YD_Fee_StuFeeTb> list =
                        ad.YD_Fee_StuFeeTb.OrderByDescending(u => u.id)
                            .Where(u => ids.Contains(u.y_stuId) && u.y_feeYear == a.y_feeyear)
                            .Include(u => u.YD_Sts_StuInfo)
                            .ToList();

                    var idlist = a.y_stuid;
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

                    var lists = ad.Database.SqlQuery<BiliDto>(sqls).ToList();
                    decimal needUpFee = 0;
                    decimal needFee = 0;
                    list.ForEach(u =>
                    {
                        //u.y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                        //u.y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        //ad.Entry(u).State = EntityState.Modified; 

                        var y_needFee = lists.Find(k => k.id == u.y_stuId).y_needFee;
                        var y_needUpFee = Convert.ToInt32(lists.Find(k => k.id == u.y_stuId).money);
                        var sb = new StringBuilder("UPDATE [YD_Fee_StuFeeTb] ");
                        sb.AppendLine(" SET [y_needFee]=" + y_needFee + ",y_needUpFee=" + y_needUpFee + " WHERE id=" + u.id);
                        string sql = sb.ToString();
                        ad.Database.ExecuteSqlCommand(sql);
                        needFee += y_needFee;
                        needUpFee += y_needUpFee;

                    });
                    //a.needtotal = list.Where(u => u.y_feeYear == a.y_feeyear).ToList().Sum(u => u.y_needUpFee);
                    //a.tuitiontotal = list.Where(u => u.y_feeYear == a.y_feeyear).ToList().Sum(u => u.y_needFee);


                    a.needtotal = needUpFee;
                    a.tuitiontotal = needFee;
                    ad.Entry(a).State = EntityState.Modified;
                    index++;
                    LogHelper.WriteDebugLog(typeof(LogType), DateTime.Now.ToString("HH:mm:ss.fff") + "当前执行到:" + index + "条数据");
                });
                int r = ad.SaveChanges();
                return "OK" + "完成" + r + "操作";
            }
        }

        public void PPP()
        {
            string fileName = "D://ppp.xlsx";
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
                    return;
                }
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
                    var list = ad.YD_Graduate_StudentScore.ToList();

                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var cell = row.GetCell(0);

                        var card = cell.StringCellValue.Trim();

                        var stu = list.FirstOrDefault(u => u.y_cardId == card);

                        if (stu == null)
                        {
                            cell.CellStyle = styleCell;
                            index++;
                            continue;
                        }
                        stu.y_namePinyin = row.GetCell(1).StringCellValue.Trim();
                        stu.y_subschoolname = row.GetCell(2).StringCellValue.Trim();
                        stu.y_createtime = DateTime.Now;
                        ad.Entry(stu).State = EntityState.Modified;
                    }
                    ad.SaveChanges();
                }
                if (index > 0)
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/学位外语成绩匹配错误表" + Hz;
                    var fileName3 = dirPath + filename1;

                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);
                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;
                        Response.Write(index);
                    }
                }
            }
        }

        //用于需要详细课程录分情况
        public ActionResult TermScoreStatistics_Course()
        {
            using (var ad = new IYunEntities())
            {
              int[] stateList = { 1, 7, 8 };

                var classCourse = ad.YD_TeaPlan_ClassCourseDes.AsQueryable();

                var stu =
                    ad.VW_StuInfo.Where(u => stateList.Contains(u.y_stuStateId) && u.y_subSchoolId.HasValue)
                        .AsQueryable();  //函授站不为-1，学籍状态为在读，未注册等

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

                //if (school != 0)
                //{
                //lists = lists.Where(u => u.s.stu.y_subSchoolId == 45);
                //}
                //if (major != 0)
                //{
                //    lists = lists.Where(u => u.s.stu.y_majorId == major);
                //}

                var listss =
                    lists.Where(u => u.s.stu.y_inYear == 2016)
                        .GroupBy(
                            u => new { u.s.classCourse.YD_Edu_Course, u.s.classCourse.y_team, u.s.stu.majorName,u.s.stu.schoolName })
                        .Select(
                            u =>
                                new TermScoreStatistics_Course
                                {
                                    schoolName=u.Key.schoolName,
                                    MajorName = u.Key.majorName,
                                    CourseName = u.Key.YD_Edu_Course.y_name,
                                    CourseId = u.Key.YD_Edu_Course.id,
                                    Term = u.Key.y_team,
                                    HasCount = u.Count(k => k.score != null),
                                    TotalCount = u.Count(k => k.s.classCourse != null),
                                    PassCount = u.Count(k => k.score != null && k.score.y_totalScore >= 60M) //临时添加
                                })
                        .OrderBy(u => u.Term).ThenBy(u => u.MajorName)
                        .ToList();
                return View(listss);
            }
        }

        public string UploadStuScore(string fileName)
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

                var list = CoreFunction.TeaPlanTempletValidate(ref errorCount, sheet, styleCell);
                //验证表格的错误情况，并返回错误数量，详情参方法体内

                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/教学计划表" + Hz;
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
                    //处理数据中的专业库
                    CoreFunction.MajorLibHandle(list);
                    //处理数据中的课程
                    CoreFunction.CourseHandle(list);
                    //处理数据中的专业
                    CoreFunction.MajorHandle(list);

                    result["Message"] = list.Count.ToString();
                    result["MajorCount"] = list.GroupBy(u => u.MajorId).Count().ToString();

                    return JsonConvert.SerializeObject(result);
                }

            }
        }

        public void MajorLibraryUpdate()
        {
            string fileName = "D://bbb.xls";
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
                    return;
                }
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

                using (var ad = new IYunEntities())
                {
                    //List<TeachPlanTetm> teachlist = new List<TeachPlanTetm>();
                    //var edutype = new Dictionary<string, int>();
                    //var stutype = new Dictionary<string, int>();
                    var majorlib = new Dictionary<string, int>();

                    //ad.YD_Edu_EduType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    //{
                    //    edutype.Add(u.y_name, u.id);
                    //});
                    //ad.YD_Edu_StuType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    //{
                    //    stutype.Add(u.y_name, u.id);
                    //});
                    ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        majorlib.Add(u.y_name, u.id);
                    });
                    ////得到专业教学计划对应id和专业id
                    //var majortean = ad.YD_TeaPlan_Templet.GroupBy(u => new { u.id, u.y_majorId }).Select(u => u.Key).ToList();
                    ////得到专业id
                    //var majorist = ad.YD_Edu_Major.Select(u => new { u.id, u.y_eduTypeId, u.y_stuTypeId, u.y_majorLibId }).ToList();
                    ////得到年级所对应的学生专业和站点
                    //var year = 2016;
                    //var stulist = ad.YD_Sts_StuInfo.Where(u => u.y_inYear == year && u.y_subSchoolId != null)
                    //    .GroupBy(u => new { u.y_majorId, u.y_subSchoolId })
                    //    .OrderBy(u => u.Key.y_subSchoolId)
                    //    .Select(u => u.Key)
                    //    .ToList();
                    for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                    {
                        var data = new TeachPlanTetm();
                        var row = sheet.GetRow(i);
                        if (row == null)
                        {
                            continue;
                        }
                        var name = row.GetCell(1).StringCellValue;
                        var aa = ad.YD_Edu_MajorLibrary.Where(u => u.y_name == name).FirstOrDefault();

                        aa.y_code = row.GetCell(0).StringCellValue;
                        ad.Entry(aa).State = EntityState.Modified;
                    }
                    ad.SaveChanges();

                }
            }
        }

        public string DeleteStuBatch(int id)
        {
            using (IYunEntities IYun =new IYunEntities())
            {
                var StuBatch= IYun.YD_Fee_StuRegistrBatch.Find(id);
                var ids = StuBatch.y_stuid;
                ids = ids.Substring(0, ids.Length - 1);
                var list = ids.Split(',');
                var stuQuery = IYun.YD_Fee_StuFeeTb;
                foreach(var stuId in list)
                {
                    int IntId = Convert.ToInt32(stuId);
                    var stuTb = stuQuery.SingleOrDefault(e => e.y_stuId == IntId && e.y_feeYear==StuBatch.y_feeyear);
                    stuQuery.Remove(stuTb);
                }
                IYun.YD_Fee_StuRegistrBatch.Remove(StuBatch);
                IYun.SaveChanges();
            }
            return "Ok";
        }

        #region 考务管理

        public ActionResult ExamPlan(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/TeaPlan/ExamPlan");
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

            var major = Request["major"];
            var year = Request["EnrollYear"];
            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.YD_TeaPlan_ExamPlan.OrderByDescending(x => x.id).AsQueryable();
                if (!string.IsNullOrWhiteSpace(major) && major != "0")
                {
                    var majorInt = Convert.ToInt32(major);
                    var majorName = yunEntities.YD_Edu_Major.FirstOrDefault(x => x.id == majorInt).y_name;
                    list = list.Where(x => x.y_majorName == majorName);
                }
                if (!string.IsNullOrWhiteSpace(year) && year != "0")
                {
                    var yearInt = Convert.ToInt32(year);
                    list = list.Where(x => x.y_year == yearInt);
                }
                List<ExamPlanDto> EPDList = new List<ExamPlanDto>();
                var majorList = list.OrderBy(x => x.y_majorName).GroupBy(x => new { x.y_majorName, x.y_year }).ToList();
                foreach (var item in majorList)
                {
                    List<ExamPlan> EPList = new List<ExamPlan>();
                    foreach (var ep in item)
                    {
                        EPList.Add(new ExamPlan()
                        {
                            courseName = ep.y_courseName,
                            id = ep.id,
                            subSchoolName = ep.y_subSchoolName,
                            time = ep.y_time,
                            term = ep.y_term
                        });
                    }
                    EPDList.Add(new ExamPlanDto()
                    {
                        majorName = item.Key.y_majorName,
                        year = item.Key.y_year,
                        ExamPlanList = EPList.OrderBy(x => x.term).ToList()
                    });

                }

                var lists = EPDList.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.YdAdminRoleId = YdAdminRoleId;
                if (Request.IsAjaxRequest())
                    return PartialView("TeaPlanTempletList", lists);
                return View(lists);
            }
        }

        public ActionResult ExamPlanAddPage()
        {
            #region 权限验证

            var power = SafePowerPage("/TeaPlan/ExamPlan");
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

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
            }

            return View();
        }

        public JsonResult ExamPlanAdd(int y_majorName, int y_courseName, DateTime y_time)
        {
            using (var ad = new IYunEntities())
            {
                var EP = new YD_TeaPlan_ExamPlan()
                {
                    y_courseName = ad.YD_Edu_Course.FirstOrDefault(x => x.id == y_courseName).y_name,
                    y_majorName = ad.YD_Edu_Major.FirstOrDefault(x => x.id == y_majorName).y_name,
                    y_time = y_time,
                    y_subSchoolName = ConfigurationManager.AppSettings["SchoolTitle"]
                };
                EP.id = ad.YD_TeaPlan_ExamPlan.Any() ? ad.YD_TeaPlan_ExamPlan.Max(u => u.id) + 1 : 1;
                ad.YD_TeaPlan_ExamPlan.Add(EP);

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "添加失败" }); }

            }
        }

        public ActionResult ExamPlanUpdatePage(int? id)
        {
            #region 权限验证

            var power = SafePowerPage("/TeaPlan/ExamPlan");
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
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 2); //根据父栏目ID获取兄弟栏目
                ViewBag.entity = yunEntities.YD_TeaPlan_ExamPlan.FirstOrDefault(u => u.id == id);
            }

            return View();
        }

        public JsonResult ExamPlanUpdate(int id, string y_majorName, string y_courseName, DateTime y_time)
        {
            using (var ad = new IYunEntities())
            {
                var EP = ad.YD_TeaPlan_ExamPlan.FirstOrDefault(u => u.id == id);

                var MajorTeaplan = ad.YD_TeaPlan_ExamPlan.FirstOrDefault
                    (u => u.y_majorName == y_majorName && u.y_courseName == y_courseName);

                if (MajorTeaplan != null) { return Json(new { msg = "此专业考试计划已存在" }); }

                EP.y_majorName = y_majorName;

                EP.y_courseName = y_courseName;

                EP.y_time = y_time;

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "修改失败" }); }
            }
        }

        public JsonResult ExamPlanDelete(int? id)
        {
            if (id == 0)
            {
                return Json(new { msg = "未知错误！" });
            }
            using (var ad = new IYunEntities())
            {

                var ExamPlan = ad.YD_TeaPlan_ExamPlan.FirstOrDefault(u => u.id == id);

                ad.YD_TeaPlan_ExamPlan.Remove(ExamPlan);

                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "删除失败！" }); }
            }
        }

        public JsonResult ExamPlanDeleteAll()
        {
            using (var ad = new IYunEntities())
            {

                var list = ad.YD_TeaPlan_ExamPlan.Where(x => x.id > 0);
                foreach (var item in list)
                {
                    ad.YD_TeaPlan_ExamPlan.Remove(item);
                }
                var j = ad.SaveChanges();

                if (j > 0) { return Json(new { msg = "ok" }); }
                else { return Json(new { msg = "删除失败！" }); }
            }
        }

        public string ExamPlanExcelInsert(string fileName)
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

                var styleCell = workbook.CreateCellStyle(); //错误的提示样式
                styleCell.FillPattern = FillPatternType.SOLID_FOREGROUND;
                styleCell.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                styleCell.SetFont(font2);
                //处理数据
                result["Message"] = CoreFunction.ExamPlanExcelHandle(sheet);

                return JsonConvert.SerializeObject(result);
            }
        }
        #endregion


    }

}
