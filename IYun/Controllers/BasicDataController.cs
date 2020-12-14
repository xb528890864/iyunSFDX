using IYun.Common;
using IYun.Dal;
using IYun.Models;
using IYun.Models.Dto;
using IYun.Object;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    public class BasicdataController : AdminBaseController
    {
        private YD_Edu_CourseDal _courseDal = new YD_Edu_CourseDal();

        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /BasicData/

        #region 机构管理

        //机构管理
        public ActionResult SubSchool(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/basicdata/SubSchool");
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
                var subschool = Request["y_name"];//机构
                ViewBag.adminid = 0;

                IQueryable<YT_Mechanism> list = yunEntities.YT_Mechanism.OrderByDescending(u => u.id).AsQueryable();
                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    //var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_name.Contains(subschool));
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("SubSchoolList", dbLogList);
                return View(dbLogList);

            }
        }

        //机构修改页
        public ActionResult SubSchoolEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicData/SubSchool");
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
                ViewBag.entity = yunEntities.YT_Mechanism.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        //添加机构
        public ActionResult SubSchoolAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/SubSchool");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }



        //添加机构方法
        public string SubSchoolAddVerify(YT_Mechanism major)
        {
            using (var yunEntities = new IYunEntities())
            {
                var me2 = yunEntities.YT_Mechanism.FirstOrDefault(u => u.y_name == major.y_name);

                if (me2 != null) { return "已存在"; }

                major.y_Createtime = DateTime.Now;

                yunEntities.YT_Mechanism.Add(major);

                var j = yunEntities.SaveChanges();


                if (j > 0)
                {
                    var ma = yunEntities.YT_Mechanism.FirstOrDefault(u => u.id == major.id);
                    ma.y_code = major.id;
                    yunEntities.SaveChanges();

                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }
        //修改机构
        [HttpPost]
        public string SubSchoolEdit(YT_Mechanism role)
        {
            using (var yunEntities = new IYunEntities())
            {
                var me2 = yunEntities.YT_Mechanism.FirstOrDefault(u => u.y_name == role.y_name);

                if (me2 != null) { return "已存在"; }

                var me = yunEntities.YT_Mechanism.FirstOrDefault(u => u.id == role.id);

                me.y_name = role.y_name;


                var j = yunEntities.SaveChanges();

                if (j > 0) { return "ok"; }
                else { return "修改失败"; }
            }
        }


        public string SubSchoolDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_Mechanism.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YT_Mechanism.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }


        public JsonResult UploadMechanism(string fileName)
        {
            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                var Mechanism2 = new Dictionary<int, string>();

                new IYunEntities().YT_Mechanism.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                     Mechanism2.Add(u.id, u.y_name)
                  );


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YT_Mechanism>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YT_Mechanism();

                    for (int j = 0; j < 1; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0://机构名
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }
                                        else if (Mechanism2.Values.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }

                                        if (errorCount == 0)
                                        {
                                            data.y_name = cell.StringCellValue.Trim();
                                            data.y_Createtime = DateTime.Now;
                                        }
                                    }
                                    break;
                                    //case 1://机构代码
                                    //    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (cell.NumericCellValue <= 0 ||
                                    //            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //        //传说中的判断double是否整数，效果未测试
                                    //        {
                                    //            cell.CellStyle = errorStyle;
                                    //            errorCount++;
                                    //        }

                                    //        if (errorCount == 0)
                                    //            data.y_code = Convert.ToInt32(cell.NumericCellValue);
                                    //    }
                                    //    break;


                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }

                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/机构信息导入失败表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        log.Info($"CourseTeaPlanExcelInsert：{JsonConvert.SerializeObject(list)}");

                        ad.YT_Mechanism.AddRange(list);




                        var j = ad.SaveChanges();



                        if (j > 0)
                        {
                            ad.Database.ExecuteSqlCommand(" UPDATE YT_Mechanism SET y_code=id   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }

        #endregion





        #region 学期管理

        //学期管理
        public ActionResult TremManager(int id = 1)
        {
            #region 权限验证

            var power = SafePowerPage("/basicdata/TremManager");
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
                var subschool = Request["y_year"];
                ViewBag.adminid = 0;

                IQueryable<YT_Term> list = yunEntities.YT_Term.OrderByDescending(u => u.id).AsQueryable();
                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_year == subschoolint);
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("TremList", dbLogList);
                return View(dbLogList);

            }
        }


        public ActionResult TremEditPage(int id)
        {
            #region 权限验证

            var power = SafePowerPage("/basicdata/TremManager");
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
                ViewBag.entity = yunEntities.YT_Term.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        //添加机构
        public ActionResult TremAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/TremManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }


        public string TremAddVerify(YT_Term major)
        {
            using (var yunEntities = new IYunEntities())
            {
                var term = yunEntities.YT_Term.FirstOrDefault(u => u.y_name == major.y_name && u.y_year == major.y_year);
                if (term != null) { return "已存在"; }

                var aa = $"{major.y_year}{major.y_name}";

                major.y_code = Convert.ToInt32(aa);

                major.y_createtime = DateTime.Now;

                yunEntities.YT_Term.Add(major);

                var j = yunEntities.SaveChanges();

                if (j > 0)
                {
                    var ma = yunEntities.YT_Term.FirstOrDefault(u => u.id == major.id);

                    ma.y_code = major.id;

                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }

        [HttpPost]
        public string TremEdit(YT_Term role)
        {
            using (var yunEntities = new IYunEntities())
            {
                var me2 = yunEntities.YT_Term.FirstOrDefault(u => u.y_name == role.y_name && u.y_year == role.y_year);
                if (me2 != null) { return "已存在"; }

                var me = yunEntities.YT_Term.FirstOrDefault(u => u.id == role.id);

                var aa = $"{role.y_year}{role.y_name}";

                me.y_code = Convert.ToInt32(aa);
                me.y_name = role.y_name;
                me.y_year = role.y_year;


                log.Info($"{JsonConvert.SerializeObject(me)}");

                try
                {
                    var j = yunEntities.SaveChanges();

                    if (j > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "修改失败";
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }



            }
        }


        public string TremDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_Term.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YT_Term.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }



        public JsonResult UploadTrem(string fileName)
        {
            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                //var Mechanism2 = new Dictionary<int, string>();

                //new IYunEntities().YT_Term.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                //     Mechanism2.Add(u.id, u.y_name)
                //  );

                var termlist = new IYunEntities().YT_Term.Select(u => new { u.id, u.y_name, u.y_year }).ToList();


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YT_Term>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YT_Term();

                    for (int j = 0; j < 2; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0://年份

                                    if (cell == null)
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (cell.CellType != CellType.NUMERIC) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                    {
                                        data.y_year = Convert.ToInt32(cell.NumericCellValue);
                                        data.y_createtime = DateTime.Now;
                                    }

                                    break;

                                case 1://学期

                                    if (cell == null)
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_name = cell.StringCellValue.Trim() == "春季" ? "01" : "09";
                                        var re = termlist.Where(u => u.y_year == data.y_year && u.y_name == data.y_name);
                                        if (re.Count() > 0)
                                        {
                                            cell = row.CreateCell(2);
                                            cell.CellStyle = errorStyle;
                                            cell.SetCellValue("已存在");
                                            errorCount++;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }

                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/学期导入失败表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        ad.YT_Term.AddRange(list);

                        var j = ad.SaveChanges();

                        if (j > 0)
                        {
                            ad.Database.ExecuteSqlCommand(" UPDATE YT_Term SET y_code=CONVERT(varchar, y_year)+y_name   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }

        #endregion



        #region 专业管理


        public ActionResult MajorManager(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/basicdata/MajorManager");
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
                var subschool = Request["y_name"];//机构
                ViewBag.adminid = 0;

                IQueryable<YD_Edu_Major> list = yunEntities.YD_Edu_Major.Include(u => u.YD_Edu_EduType).Include(u => u.YD_Edu_StuType).Include(u => u.YD_Edu_MajorLibrary).OrderByDescending(u => u.id).AsQueryable();

                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    //var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_name.Contains(subschool));
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("MajorList", dbLogList);
                return View(dbLogList);

            }
        }


        public ActionResult MajorEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicdata/MajorManager");
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
                ViewBag.entity = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        public ActionResult MajorAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/MajorManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }




        public string MajorAddVerify(YD_Edu_Major major)
        {
            var MajorLibraryId = Request["MajorLibrary"]; //专业库id
            var StuTypeId = Request["StuType"]; //学习形式id
            var EduTypeId = Request["EduType"]; //层次id



            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            log.Info($"MajorAddVerify:{MajorLibraryId},{StuTypeId},{EduTypeId} ");





            if (MajorLibraryId == "0" || StuTypeId == "0" || EduTypeId == "0")
            {
                return "专业、学习形式、层次是必填项！";
            }

            using (var yunEntities = new IYunEntities())
            {
                var ma = Convert.ToInt32(MajorLibraryId);
                var majorLibray = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == ma);


                var ma2 = Convert.ToInt32(StuTypeId);
                var StuType = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == ma2);

                var ma3 = Convert.ToInt32(EduTypeId);
                var EduType = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == ma3);



                var majorList = yunEntities.YD_Edu_Major.Where(u => u.y_majorLibId == ma && u.y_eduTypeId == ma3 && u.y_stuTypeId == ma2);

                if (majorList.Count() > 0)
                {
                    return "此大专业已存在";
                }

                var majorName = $"{majorLibray.y_name} {EduType.y_name} {StuType.y_name}";

                major.y_name = majorName;
                major.y_majorLibId = Convert.ToInt32(MajorLibraryId);

                major.y_stuTypeId = Convert.ToInt32(StuTypeId);

                major.y_eduTypeId = Convert.ToInt32(EduTypeId);

                yunEntities.YD_Edu_Major.Add(major);

                var j = yunEntities.SaveChanges();

                if (j > 0)
                {
                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }

        [HttpPost]
        public string MajorEdit(YD_Edu_Major role)
        {
            var MajorLibraryId = Request["MajorLibrary"]; //专业库id
            var StuTypeId = Request["StuType"]; //学习形式id
            var EduTypeId = Request["EduType"]; //层次id



            if (MajorLibraryId == "0" || StuTypeId == "0" || EduTypeId == "0")
            {
                return "专业、学习形式、层次是必填项！";
            }
            using (var yunEntities = new IYunEntities())
            {

                var ma = Convert.ToInt32(MajorLibraryId);
                var majorLibray = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == ma);


                var ma2 = Convert.ToInt32(StuTypeId);
                var StuType = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == ma2);

                var ma3 = Convert.ToInt32(EduTypeId);
                var EduType = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == ma3);



                var major = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.y_majorLibId == ma && u.y_eduTypeId == ma3 && u.y_stuTypeId == ma2);

                if (major != null && major.id != role.id)
                {
                    return "此大专业已存在！";
                }


                var majorName = $"{majorLibray.y_name} {EduType.y_name} {StuType.y_name}";

                var me = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == role.id);

                me.y_name = majorName;

                me.y_majorLibId = Convert.ToInt32(MajorLibraryId);

                me.y_stuTypeId = Convert.ToInt32(StuTypeId);

                me.y_eduTypeId = Convert.ToInt32(EduTypeId);

                me.y_needFee = role.y_needFee;

                me.y_majortype = role.y_majortype;

                me.y_stuYear = role.y_stuYear;



                try
                {
                    var j = yunEntities.SaveChanges();

                    if (j > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "修改失败";
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }



            }
        }


        public string MajorDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YD_Edu_Major.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }

        #endregion



        #region 年级管理


        public ActionResult GradeManager(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/basicdata/GradeManager");
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
                var subschool = Request["y_name"];
                ViewBag.adminid = 0;

                IQueryable<YT_Grade> list = yunEntities.YT_Grade.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    //var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_name.Contains(subschool));
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("GradeList", dbLogList);
                return View(dbLogList);

            }
        }


        public ActionResult GradeEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicdata/GradeManager");
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
                ViewBag.entity = yunEntities.YT_Grade.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        public ActionResult GradeAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/GradeManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }




        public string GradeAddVerify(YT_Grade major)
        {

            using (var yunEntities = new IYunEntities())
            {


                var ma = yunEntities.YT_Grade.FirstOrDefault(u => u.y_name == major.y_name);
                if (ma != null) { return "已存在"; }

                major.y_createtime = DateTime.Now;

                yunEntities.YT_Grade.Add(major);

                var j = yunEntities.SaveChanges();

                if (j > 0)
                {
                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }

        [HttpPost]
        public string GradeEdit(YT_Grade role)
        {

            using (var yunEntities = new IYunEntities())
            {
                try
                {

                    var ma = yunEntities.YT_Grade.FirstOrDefault(u => u.y_name == role.y_name);

                    if (ma != null) { return "已存在"; }

                    var me = yunEntities.YT_Grade.FirstOrDefault(u => u.id == role.id);

                    me.y_name = role.y_name;

                    var j = yunEntities.SaveChanges();

                    if (j > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "修改失败";
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }



            }
        }


        public string GradeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_Grade.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YT_Grade.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }



        public JsonResult UploadGrade(string fileName)
        {
            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                var Mechanism2 = new Dictionary<int, string>();

                new IYunEntities().YT_Grade.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                     Mechanism2.Add(u.id, u.y_name)
                  );


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YT_Grade>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YT_Grade();

                    for (int j = 0; j < 1; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0://年份

                                    if (cell == null)
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (cell.CellType != CellType.NUMERIC)
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    else if (Mechanism2.Values.Contains(cell.NumericCellValue.ToString()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                    {
                                        data.y_name = cell.NumericCellValue.ToString();
                                        data.y_createtime = DateTime.Now;
                                    }

                                    break;
                                    //case 1://机构代码
                                    //    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (cell.NumericCellValue <= 0 ||
                                    //            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //        //传说中的判断double是否整数，效果未测试
                                    //        {
                                    //            cell.CellStyle = errorStyle;
                                    //            errorCount++;
                                    //        }

                                    //        if (errorCount == 0)
                                    //            data.y_code = Convert.ToInt32(cell.NumericCellValue);
                                    //    }
                                    //    break;


                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }

                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/年级信息导入失败表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        ad.YT_Grade.AddRange(list);

                        var j = ad.SaveChanges();

                        if (j > 0)
                        {
                            //ad.Database.ExecuteSqlCommand(" UPDATE YT_Grade SET y_code=id   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }



        #endregion



        #region 院系管理


        public ActionResult FacultyManager(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/basicdata/FacultyManager");
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
                var subschool = Request["y_name"];
                ViewBag.adminid = 0;

                IQueryable<YT_Faculty> list = yunEntities.YT_Faculty.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    //var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_name.Contains(subschool));
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("FacultyList", dbLogList);
                return View(dbLogList);

            }
        }


        public ActionResult FacultyEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicdata/FacultyManager");
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
                ViewBag.entity = yunEntities.YT_Faculty.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        public ActionResult FacultyAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/FacultyManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }




        public string FacultyAddVerify(YT_Faculty major)
        {

            using (var yunEntities = new IYunEntities())
            {
                major.y_createtime = DateTime.Now;

                yunEntities.YT_Faculty.Add(major);

                var j = yunEntities.SaveChanges();

                if (j > 0)
                {
                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }

        [HttpPost]
        public string FacultyEdit(YT_Faculty role)
        {

            using (var yunEntities = new IYunEntities())
            {

                try
                {

                    var me = yunEntities.YT_Faculty.FirstOrDefault(u => u.id == role.id);

                    me.y_name = role.y_name;

                    var j = yunEntities.SaveChanges();

                    if (j > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "修改失败";
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }



            }
        }


        public string FacultyDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_Faculty.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YT_Faculty.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }


        public JsonResult UploadFaculty(string fileName)
        {
            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                var Mechanism2 = new Dictionary<int, string>();

                new IYunEntities().YT_Faculty.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                     Mechanism2.Add(u.id, u.y_name)
                  );


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YT_Faculty>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YT_Faculty();

                    for (int j = 0; j < 1; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }
                                        else if (Mechanism2.Values.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }

                                        if (errorCount == 0)
                                        {
                                            data.y_name = cell.StringCellValue.Trim();
                                            data.y_createtime = DateTime.Now;
                                        }
                                    }
                                    break;
                                    //case 1://机构代码
                                    //    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (cell.NumericCellValue <= 0 ||
                                    //            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //        //传说中的判断double是否整数，效果未测试
                                    //        {
                                    //            cell.CellStyle = errorStyle;
                                    //            errorCount++;
                                    //        }

                                    //        if (errorCount == 0)
                                    //            data.y_code = Convert.ToInt32(cell.NumericCellValue);
                                    //    }
                                    //    break;


                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }

                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/院系信息导入表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        ad.YT_Faculty.AddRange(list);

                        var j = ad.SaveChanges();

                        if (j > 0)
                        {
                            //ad.Database.ExecuteSqlCommand(" UPDATE YT_Grade SET y_code=id   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }

        #endregion


        #region 教师管理


        public ActionResult TeacherManager(int id = 1)
        {
            #region “函授站管理”权限验证

            var power = SafePowerPage("/basicdata/TeacherManager");
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
                var subschool = Request["y_name"];
                ViewBag.adminid = 0;

                IQueryable<YT_Teacher> list = yunEntities.YT_Teacher.OrderByDescending(u => u.id);

                if (!string.IsNullOrWhiteSpace(subschool) && !subschool.Equals("0"))
                {
                    //var subschoolint = Convert.ToInt32(subschool);
                    list = list.Where(u => u.y_name.Contains(subschool));
                }
                var dbLogList = list.ToPagedList(id, 15); //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
                ViewBag.adminid = YdAdminId;
                if (Request.IsAjaxRequest())
                    return PartialView("TeacherList", dbLogList);
                return View(dbLogList);

            }
        }


        public ActionResult TeacherEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicdata/TeacherManager");
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
                ViewBag.entity = yunEntities.YT_Teacher.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        public ActionResult TeacherAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/TeacherManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }




        public string TeacherAddVerify(YT_Teacher major)
        {

            //var SubSchoolID = Request["SubSchool"];



            using (var yunEntities = new IYunEntities())
            {
                //if (SubSchoolID != "0")
                //{
                //    major.y_subSchoolId = Convert.ToInt32(SubSchoolID);
                //}

                major.y_createtime = DateTime.Now;

                yunEntities.YT_Teacher.Add(major);

                var j = yunEntities.SaveChanges();

                if (j > 0)
                {
                    return "ok";
                }
                else
                {

                    return "添加失败";
                }

            }
        }

        [HttpPost]
        public string TeacherEdit(YT_Faculty role)
        {
            var SubSchoolID = Request["SubSchool"];
            using (var yunEntities = new IYunEntities())
            {

                try
                {

                    var me = yunEntities.YT_Teacher.FirstOrDefault(u => u.id == role.id);

                    //if (SubSchoolID != "0")
                    //{
                    //    me.y_subSchoolId = Convert.ToInt32(SubSchoolID);
                    //}

                    me.y_name = role.y_name;

                    var j = yunEntities.SaveChanges();

                    if (j > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "修改失败";
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }


        public string TeacherDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_Teacher.FirstOrDefault(u => u.id == id);

                //yunEntities.Entry(me).State = EntityState.Deleted;
                yunEntities.YT_Teacher.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "修改失败";
                }
            }
        }


        public JsonResult UploadTeacher(string fileName)
        {
            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                var Mechanism2 = new Dictionary<int, string>();

                new IYunEntities().YT_Teacher.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                     Mechanism2.Add(u.id, u.y_name)
                  );


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YT_Teacher>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YT_Teacher();

                    for (int j = 0; j < 1; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }
                                        else if (Mechanism2.Values.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }

                                        if (errorCount == 0)
                                        {
                                            data.y_name = cell.StringCellValue.Trim();
                                            data.y_createtime = DateTime.Now;
                                        }
                                    }
                                    break;
                                    //case 1://机构代码
                                    //    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (cell.NumericCellValue <= 0 ||
                                    //            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //        //传说中的判断double是否整数，效果未测试
                                    //        {
                                    //            cell.CellStyle = errorStyle;
                                    //            errorCount++;
                                    //        }

                                    //        if (errorCount == 0)
                                    //            data.y_code = Convert.ToInt32(cell.NumericCellValue);
                                    //    }
                                    //    break;


                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }

                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/教师信息导入失败表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        ad.YT_Teacher.AddRange(list);

                        var j = ad.SaveChanges();

                        if (j > 0)
                        {
                            //ad.Database.ExecuteSqlCommand(" UPDATE YT_Grade SET y_code=id   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }
        #endregion





        #region 课程库  

        public ActionResult CourseManager(int id = 1)
        {
            #region “课程库管理”权限验证

            var power = SafePowerPage("/Basicdata/CourseManager");
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
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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

            var power = SafePowerPage("/basicdata/CourseManager");
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
                    FileHelper.ToDataTable(list.Select(u => new { Course = u.y_name, y_code = u.y_code }).ToList());

                var dirPath = Server.MapPath("~/File/Dowon"); //todo:改变
                if (!Directory.Exists(dirPath)) //todo:改变
                {
                    Directory.CreateDirectory(dirPath); //todo:改变
                }
                var filename1 = "/课程库下载表" + ".xls"; //todo:改变
                var fileName3 = dirPath + filename1; //todo:改变

                //var filename1 = "File/Dowon/课程库下载表" + Guid.NewGuid() + ".xls";
                //    var fileName2 = "~/" + filename1;
                //    var fileName3 = Server.MapPath(fileName2);
                using (var excelHelper = new ExcelHelper(fileName3))
                {
                    var ht = new Hashtable { { "CourseType", "课程名" }, { "y_code", "专业代码" } };
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
        public ActionResult UploadCourse()
        {
            #region 权限验证

            var power = SafePowerPage("/basicdata/Coursemanager");
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
                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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

            var power = SafePowerPage("/basicdata/coursemanager");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
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

            var power = SafePowerPage("/basicdata/coursemanager");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
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
        /// 验证导入的临时课程信息
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCourse()
        {
            #region 权限验证

            var power = SafePowerPage("/basicdata/coursemanager");
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

            using (var db = new IYunEntities())
            {
                const int isOk = (int)YesOrNo.No;
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
                            item.y_isOk = (int)YesOrNo.Yes;
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


                var temp = db.YD_Edu_FormTemp.Where(u => u.y_isOk == isOk).ToList();

                ViewBag.entityList = temp;

                ViewBag.modulePowers = GetChildModulePower(db, 119); //根据父栏目ID获取兄弟栏目




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
                const int isOk = (int)YesOrNo.Yes;
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
                return Redirect("CourseManager");
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

            var power = SafePowerPage("/basicdata/CourseManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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

            var power = SafePowerPage("/basicdata/CourseManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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

            var power = SafePowerPage("/basicdata/CourseManager");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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

            var power = SafePowerPage("/basicdata/CourseManager");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion

            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 119); //根据父栏目ID获取兄弟栏目
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
                var a = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == role.y_name);

                var b = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.id == role.id);


                if (b.y_name == role.y_name && b.y_code == role.y_code)
                {
                    return Content("已存在");
                }

                b.y_name = role.y_name;
                b.y_code = role.y_code;


                var j = yunEntities.SaveChanges();


                if (j > 0) { return Content("ok"); }
                else { return Content("修改失败"); }

                //return Content(_courseDal.EditEntity(role, yunEntities));
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

                yunEntities.YD_Edu_Course.Add(role);

                var j = yunEntities.SaveChanges();

                if (j > 0) { return Content("ok"); } else { return Content("添加失败"); };

                //return Content(_courseDal.AddEntity(role, yunEntities));
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


        public JsonResult UploadCourse2(string fileName)
        {

            fileName = Server.MapPath(fileName);

            string Hz; //后缀名

            #region  接收execl数据，并设置错误样式

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
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return Json(new { IsOk = false, msg = "Execl格式不正确！" });
                }
                var errorCount = 0;

                var errorStyle = workbook.CreateCellStyle(); //错误的提示样式
                errorStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                errorStyle.FillForegroundColor = HSSFColor.RED.index;

                var font2 = workbook.CreateFont();
                font2.Color = HSSFColor.WHITE.index;
                errorStyle.SetFont(font2);

                #endregion

                //var list = CoreFunction.CourseTeaPlanTempletValidate(ref errorCount, sheet, styleCell);

                #region  //验证表格的错误情况，并返回错误数量，详情参方法体内

                var Mechanism2 = new Dictionary<int, string>();

                new IYunEntities().YD_Edu_Course.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>

                     Mechanism2.Add(u.id, u.y_name)
                  );


                //var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

                var list = new List<YD_Edu_Course>(); //数据转化List

                const double eps = 1e-10; // 精度范围

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {

                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Edu_Course();

                    for (int j = 0; j < 1; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)//先判断是否为空
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else if (cell.CellType == CellType.BLANK) //先判断是否为空
                        {
                            cell.SetCellValue("Can't be null");
                            cell.CellStyle = errorStyle;
                            errorCount++;
                        }
                        else
                        {
                            switch (j)
                            {

                                case 0:
                                    if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                        {
                                            cell.SetCellValue("Can't be null");
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }
                                        else if (Mechanism2.Values.Contains(cell.StringCellValue.Trim()))
                                        {
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }

                                        if (errorCount == 0)
                                        {
                                            data.y_name = cell.StringCellValue.Trim();
                                            data.y_code = "111";
                                        }
                                    }
                                    break;
                                    //case 1://机构代码
                                    //    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (cell.NumericCellValue <= 0 ||
                                    //            cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //        //传说中的判断double是否整数，效果未测试
                                    //        {
                                    //            cell.CellStyle = errorStyle;
                                    //            errorCount++;
                                    //        }

                                    //        if (errorCount == 0)
                                    //            data.y_code = Convert.ToInt32(cell.NumericCellValue);
                                    //    }
                                    //    break;


                            }
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面

                }


                log.Info($"{JsonConvert.SerializeObject(list)},{errorCount}");
                #endregion


                #region  返回错误的Execl.或执行成功


                if (errorCount > 0) //如果错误数量大于0就EXCEL导出
                {
                    var dirPath = Server.MapPath("~/File/Dowon");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    var filename1 = "/课程导入失败表" + Hz;
                    var fileName3 = dirPath + filename1;



                    //将工作簿写入文件
                    using (FileStream fs2 = new FileStream(fileName3, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs2);


                        string url = Request.Url.Scheme + "://" + Request.Url.Authority + "/File/Dowon" + filename1;


                        return Json(new { IsOk = true, msg = url });
                    }
                }
                else //否则直接导入数据
                {

                    using (var ad = new IYunEntities())
                    {

                        ad.YD_Edu_Course.AddRange(list);

                        var j = ad.SaveChanges();

                        if (j > 0)
                        {
                            ad.Database.ExecuteSqlCommand(" UPDATE YD_Edu_Course SET y_code=id   ");

                            return Json(new { IsOk = false, msg = $"导入成功！导入了{j}条数据" });
                        }
                        else
                        {
                            return Json(new { IsOk = false, msg = $"导入失败" });
                        }
                    }
                }
                #endregion

            }
        }
        #endregion


        public JsonResult copyTemplet(int templetId, int majorId)
        {
            using (var ad = new IYunEntities())
            {

                var templet = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.id == templetId);


                var templet2 = ad.YD_TeaPlan_Templet.FirstOrDefault(u => u.y_majorId == majorId);



                if (templet2 != null)
                {
                    return Json("此专业教学计划已存在");
                }

                YD_TeaPlan_Templet ne = new YD_TeaPlan_Templet();

                ne.y_teaPlanType = 1;
                ne.y_majorId = majorId;

                ad.YD_TeaPlan_Templet.Add(ne);

                var j = ad.SaveChanges();

                if (j < 1)
                {
                    return Json("失败");
                }

                var templetDes = ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == templetId).ToList();
                List<YD_TeaPlan_TempletCourseDes> neDes = new List<YD_TeaPlan_TempletCourseDes>();



                foreach (var a in neDes)
                {
                    YD_TeaPlan_TempletCourseDes te = new YD_TeaPlan_TempletCourseDes();

                    te.y_course = a.y_course;
                    te.y_templetId = ne.id;
                    te.y_team = a.y_team;
                    te.y_selfPeriod = a.y_selfPeriod;
                    te.y_selfPeriod2 = a.y_selfPeriod2;
                    te.y_taskPeriod = a.y_taskPeriod;

                    te.y_teaPeriod = a.y_teaPeriod2;
                    te.y_teaPeriod2 = a.y_teaPeriod2;
                    te.y_isMain = a.y_isMain;
                    te.y_courseType = a.y_courseType;
                    te.y_sampleexam = a.y_sampleexam;
                    neDes.Add(te);
                }
                ad.YD_TeaPlan_TempletCourseDes.AddRange(neDes);




                var g = ad.SaveChanges();

                if (g > 0)
                {
                    return Json("ok");

                }
                else
                {
                    return Json("失败");
                };
            }



        }

        public ActionResult LiveVideo(int id)
        {
            #region “活动直播”权限验证

            var power = SafePowerPage("/basicdata/ActivityVideo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            ViewBag.power = power;
            
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 106);
                var model = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == id);
                if (model == null)
                {
                    return Content("活动不存在");
                }
                return View(model);
            }
        }

        public ActionResult PhoneLiveVideo(int? id)
        {
            if (!id.HasValue)
            {
                return Content("<h1>未指定进入直播地址</h1>");
            }
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 106);
                var model = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == id);
                if (model == null)
                {
                    return Content("<h1>活动不存在</h1>");
                }
                return View(model);
            }
        }

        public ActionResult ActivityVideo(int id = 1)
        {
            #region “活动直播”权限验证

            var power = SafePowerPage("/basicdata/ActivityVideo");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            ViewBag.power = power;
            string name = Request["y_name"];
            using (var yunEntities = new IYunEntities())
            {
                IQueryable<YT_ActivityVideo> list = yunEntities.YT_ActivityVideo.OrderByDescending(u => u.id).AsQueryable();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(u => u.y_name == name);
                }
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 106);
                return View(list.ToPagedList(id, 15));
            }
        }

        public ActionResult ActivityVideoEditPage(int id)
        {
            #region “编辑函授站”权限验证

            var power = SafePowerPage("/basicData/ActivityVideo");
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
                ViewBag.entity = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 106); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        //添加活动
        public ActionResult ActivityVideoAddPage()
        {
            #region “添加函授站”权限验证

            var power = SafePowerPage("/basicdata/ActivityVideo");
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
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 106); //根据父栏目ID获取兄弟栏目
            }
            return View();
        }


         
        // 添加活动
        public string ActivityVideoAddVerify(YT_ActivityVideo activity)
        {
            using (var yunEntities = new IYunEntities())
            {
                activity.y_name = activity.y_name.Trim();
                activity.y_url = activity.y_url.Trim();
                activity.y_createdtime = DateTime.Now;

                yunEntities.YT_ActivityVideo.Add(activity);

                var j = yunEntities.SaveChanges();


                if (j > 0)
                {
                    return "ok";
                }
                else
                {
                    return "添加失败";
                }

            }
        }
        // 修改活动
        [HttpPost]
        public string ActivityVideoEdit(YT_ActivityVideo role)
        {
            using (var yunEntities = new IYunEntities())
            {
                var me = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == role.id);
                me.y_name = role.y_name.Trim();
                me.y_starttime = role.y_starttime;
                me.y_endtime = role.y_endtime;
                me.y_url = role.y_url.Trim();
                //me.y_isOpen = role.y_isOpen;
                var j = yunEntities.SaveChanges();

                if (j > 0) { return "ok"; }
                else { return "修改失败"; }
            }
        }

        // 删除活动
        public string ActivityVideoDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {

                var me = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == id);

               
                yunEntities.YT_ActivityVideo.Remove(me);
                var j = yunEntities.SaveChanges();

                if (j > 0)

                {
                    return "ok";
                }
                else
                {

                    return "删除失败";
                }
            }
        }

        public ActionResult PrintTemplate(int id = 1)
        {
            #region “打印模板”权限验证

            var power = SafePowerPage("/basicdata/PrintTemplate");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                var reurl = Request.UrlReferrer.ToString();
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            //ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 112); //根据父栏目ID获取兄弟栏目
                ViewBag.adminroleid = YdAdminRoleId;
                IQueryable<YD_Sys_PrintTemplate> list = yunEntities.YD_Sys_PrintTemplate.OrderByDescending(u => u.id).AsQueryable();
                return View(list.ToPagedList(id, 15));
            }
        }

        public ActionResult PrintTemplateAddPage()
        {
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 112); //根据父栏目ID获取兄弟栏目
            }
                return View();
        }

        public ActionResult PrintViewSet(int id=1)
        {
            var tempid = Convert.ToInt32(Request["tempid"]);
            ViewBag.tempid = tempid;
            if (tempid > 0)
            {
                using (var yunEntities = new IYunEntities())
                {

                    ViewBag.modulePowers = GetChildModulePower(yunEntities, 112); //根据父栏目ID获取兄弟栏目
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
                        return PartialView("PrintViewSet", model);
                    return View(model);
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult PrintTemplateEditPage(int id)
        {
            if(id > 0)
            {
                using (var yunEntities = new IYunEntities())
                {
                    YD_Sys_PrintTemplate printTemplate = yunEntities.YD_Sys_PrintTemplate.Include(x => x.YD_Sys_PrintCss).FirstOrDefault(x => x.id == id);
                    return View(printTemplate);
                }
            }
            return RedirectToAction("PrintTemplate");
        }

       

        public JsonResult PrintTemplateSave()
        { 
            PrintTemplateDto dto = JsonConvert.DeserializeObject<PrintTemplateDto>(HttpUtility.UrlDecode(Request["template"].ToString(), System.Text.Encoding.UTF8));
            if(dto.id > 0)
            {
                using (var yunEntities = new IYunEntities())
                {
                    using (var dbContextTransaction = yunEntities.Database.BeginTransaction())
                    {
                        try
                        {
                            YD_Sys_PrintTemplate printTemplate = yunEntities.YD_Sys_PrintTemplate.FirstOrDefault(x => x.id == dto.id);
                            printTemplate.y_dataview = dto.y_dataview;
                            printTemplate.y_name = dto.y_name.Trim();
                            printTemplate.y_time = DateTime.Now.Date;
                            yunEntities.SaveChanges();
                            foreach (var item in dto.divs)
                            {
                                if(item.id > 0)
                                {
                                    YD_Sys_PrintCss printCss = yunEntities.YD_Sys_PrintCss.FirstOrDefault(x => x.id == item.id);
                                    printCss.y_printid = printTemplate.id;
                                    printCss.y_divdatatype = item.type;
                                    printCss.y_range = item.range;
                                    printCss.y_fontsize = item.fontsize;
                                    printCss.y_bold = item.bold;
                                }
                                else
                                {
                                    YD_Sys_PrintCss printCss = new YD_Sys_PrintCss()
                                    {
                                        y_printid = printTemplate.id,
                                        y_divdatatype = item.type,
                                        y_range = item.range,
                                        y_fontsize = item.fontsize,
                                        y_bold = item.bold
                                    };
                                    yunEntities.YD_Sys_PrintCss.Add(printCss);
                                }
                                yunEntities.SaveChanges();
                                
                            }
                            dbContextTransaction.Commit();
                            return Json(true);
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                            return Json(e);
                        }
                    }
                }
            }
            else
            {
                using (var yunEntities = new IYunEntities())
                {
                    using (var dbContextTransaction = yunEntities.Database.BeginTransaction())
                    {
                        try
                        {
                            YD_Sys_PrintTemplate printTemplate = new YD_Sys_PrintTemplate()
                            {
                                y_dataview = dto.y_dataview,
                                y_name = dto.y_name.Trim(),
                                y_time = DateTime.Now.Date,
                            };
                            yunEntities.YD_Sys_PrintTemplate.Add(printTemplate);
                            yunEntities.SaveChanges();
                            foreach (var item in dto.divs)
                            {
                                YD_Sys_PrintCss printCss = new YD_Sys_PrintCss()
                                {
                                    y_printid = printTemplate.id,
                                    y_divdatatype = item.type,
                                    y_range = item.range,
                                    y_fontsize = item.fontsize,
                                    y_bold = item.bold
                                };
                                yunEntities.YD_Sys_PrintCss.Add(printCss);
                                yunEntities.SaveChanges();
                            }
                            dbContextTransaction.Commit();
                            return Json(true);
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                            return Json(e);
                        }
                    }
                }
            }

            
        }

        public static int StrToInt(string str)
        {
            return int.Parse(str);
        }
        public ActionResult PrintView()
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
                var tempid = Convert.ToInt32(Request["tempid"]);
                var template = yunEntities.YD_Sys_PrintTemplate.Include(x => x.YD_Sys_PrintCss).FirstOrDefault(x => x.id == tempid);
                return View(Tuple.Create(template, list.OrderByDescending(x => x.id).ToList()));
            }
        }

        public JsonResult PrintTemplateDelete(int id)
        {
            if(id>0)
            {
                using (var yunEntities = new IYunEntities())
                {
                    var item = yunEntities.YD_Sys_PrintTemplate.FirstOrDefault(x=>x.id==id);
                    yunEntities.YD_Sys_PrintTemplate.Remove(item);
                    yunEntities.SaveChanges();
                    return Json(true);
                }
            }
            return Json(false);
        }

        public JsonResult GetValue<T>(T model, string property)
        {
            Type type = model.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            return Json(propertyInfo.GetValue(model, null));
        }

        public JsonResult GetDataField(string viewName)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                string instanceName = "IYun.Models." + viewName;
                Assembly assembly = Assembly.GetExecutingAssembly(); ;
                var item = assembly.CreateInstance(instanceName);

                List<string> properList = new List<string>();
                foreach (var p in item.GetType().GetProperties())
                {
                    properList.Add(p.Name);
                }
                return Json(L(properList));
            }
            return Json(false);
        }

        public static Dictionary<string, string> dic = new Dictionary<string, string>() {
            {"y_stuNum","学号"},
            {"y_cardId","身份证号"},
            {"y_term","学期"},
            {"y_stuName","学生姓名"},
            {"y_sex","性别"},
            {"y_inYear","入学年份"},
            {"y_tel","联系方式"},
            {"y_mail","邮箱"},
            {"y_address","地址"},
            {"schoolName","函授站名"},
            {"nationName","民族"},
            {"politicsName","政治面貌"},
            {"majorName","专业名"},
            {"majorLibraryName","专业库名称"},
            {"stuTypeName","学习形式"},
            {"eduTypeName","层次"},
        };
        private static Dictionary<string,string> L(List<string> list)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in list)
            {
                if (dic.ContainsKey(item))
                {
                    result.Add(item,dic[item]);
                }
            }
            return result;
        }

        public  static object GetFiledValue<T>(T model,string property)
        {
            Type type = model.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            if(propertyInfo == null)
            {
                return property;
            }
            return propertyInfo.GetValue(model, null);
        }
    }
}
