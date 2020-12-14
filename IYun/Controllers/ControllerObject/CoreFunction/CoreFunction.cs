using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Models;
using IYun.Object;
using NPOI.SS.UserModel;

namespace IYun.Controllers
{
    public static class CoreFunction
    {
        /********************************************************************
        * 作者：廖坤   2017-2-21 16:23
        * 用途：用于EXCEL表格导入教学计划模板，对其中数据的有效性进行校验，并返回错误数量.
        * 最新修改人：廖坤      
        * 最新修改时间：2017-2-23 17:07
        ********************************************************************/

        /// <summary>
        /// 教学计划模板的验证
        /// </summary>
        /// <param name="errorCount">错误数量</param>
        /// <param name="sheet">EXCEL表格</param>
        /// <param name="errorStyle">错误后的EXCEL样式</param>
        public static List<TeaPlanExcelInsertDto> TeaPlanTempletValidate(ref int errorCount, ISheet sheet,
            ICellStyle errorStyle)
        {
            var eduType = new Dictionary<string, int>();
            var stuType = new Dictionary<string, int>();

            using (var ad = new IYunEntities())
            {
                ad.YD_Edu_EduType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      eduType.Add(u.y_name, u.id);
                  });

                ad.YD_Edu_StuType.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      stuType.Add(u.y_name, u.id);
                  });
            }
            var mainCourse = new Dictionary<string, bool> { { "是", true }, { "否", false } };

            var list = new List<TeaPlanExcelInsertDto>(); //数据转化List

            const double eps = 1e-10; // 精度范围

            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {

                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var data = new TeaPlanExcelInsertDto();

                for (int j = 0; j < 12; j++)
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
                            case 0://专业
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

                                    if (errorCount == 0)
                                        data.MajorName = cell.StringCellValue.Trim();
                                }

                                break;
                            case 1://学习形式
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
                                    else if (!eduType.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.EduType = eduType[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 2://层次
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
                                    else if (!stuType.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.StuType = stuType[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 3://课程
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

                                    if (errorCount == 0)
                                        data.CourseName = cell.StringCellValue.Trim();
                                }
                                break;
                            case 4://学期
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue <= 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.Team = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 5:
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue < 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.SelfPeriod = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 6:
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue < 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.TeaPeriod = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 7:
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue < 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.TaskPeriod = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 8://课程类型
                                if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    CourseType coursetype = CourseType.公共基础课;

                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (!Enum.TryParse(cell.StringCellValue.Trim(), out coursetype))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.CourseType = (int)coursetype;
                                }
                                break;
                            case 9://是否主干课程
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
                                    else if (!mainCourse.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.IsMain = mainCourse[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 10:
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue < 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.SelfPeriod2 = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 11:
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue < 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.TeaPeriod2 = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                        }
                    }
                }
                if (errorCount == 0)
                    list.Add(data); //把数据加载到List里面

            }
            return list;
        }
        /********************************************************************
        * 作者：刘天华   2017-11-30  
        * 用途：考生导入模板验证
        ********************************************************************/
        /// <param name="errorCount">错误数量</param>
        /// <param name="sheet">EXCEL表格</param>
        /// <param name="errorStyle">错误后的EXCEL样式</param>
        public static List<YD_Sts_SubStuTemp> SubStudentTempletValidate(ref int errorCount, ISheet sheet,
            ICellStyle errorStyle, int subSchoolId)
        {
            var subNo = new Dictionary<string, int>();
          
            using (var ad = new IYunEntities())
            {

                ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    subNo.Add(u.y_name, u.id);
                });
                var list = new List<YD_Sts_SubStuTemp>(); //数据转化List

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Sts_SubStuTemp();
                    for (int j = 0; j < 5; j++)
                    {

                        data.y_subSchoolId = subSchoolId;

                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0: //姓名

                                data.y_name = cell.StringCellValue.Trim();
                                break;
                            case 1://年份
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else if(errorCount==0)
                                {
                                    data.y_year = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                           
                            case 2: //考生号
                               if(cell.CellType == CellType.NUMERIC)
                                {
                                    data.y_examNum = Math.Floor(cell.NumericCellValue).ToString("f0");
                                }
                                else
                                {
                                    data.y_examNum = cell.StringCellValue.Trim();
                                }
                                break;
                            case 3://身份证

                                data.y_cardId = cell.StringCellValue.Trim();
                                break;
                           
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面
                }
                return list;
              }
            }

        /********************************************************************
        * 作者：胡惠贞   2017-4-5 14:46
        * 用途：学生导入模板验证
        ********************************************************************/

        /// <param name="errorCount">错误数量</param>
        /// <param name="sheet">EXCEL表格</param>
        /// <param name="errorStyle">错误后的EXCEL样式</param>
        public static List<YD_Sts_StuInfoTemp> StudentTempletValidate(ref int errorCount, ISheet sheet,
            ICellStyle errorStyle)
        {
            var edutype = new Dictionary<string, int>();
            var stutype = new Dictionary<string, int>();
            var majorlib = new Dictionary<string, int>();

            var state = new Dictionary<string, int>();
            var sub = new Dictionary<string, int>();
            var nation = new Dictionary<string, int>();
            var pition = new Dictionary<string, int>();
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            using (var ad = new IYunEntities())
            {
                ad.Database.ExecuteSqlCommand("DELETE FROM YD_Sts_StuInfoTemp");
                var majorist = ad.YD_Edu_Major.Select(u => new { u.id, u.y_eduTypeId, u.y_stuTypeId, u.y_majorLibId }).ToList();

                #region 
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
                ad.YD_Edu_StuState.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      state.Add(u.y_name, u.id);
                  });
                
                if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_nameabbreviation }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_nameabbreviation, u.id);
                    });
                }
                else
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_name, u.id);
                    });
                }
                    ad.YD_Sts_Nation.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      nation.Add(u.y_name, u.id);
                  });
                ad.YD_Sts_Politics.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      pition.Add(u.y_name, u.id);
                  });
                var examNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_examNum).ToList();

                var stuNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_stuNum).ToList();
                #endregion
                var list = new List<YD_Sts_StuInfoTemp>(); //数据转化List
                var examNum = new List<string>(); //模板的考生号集合

                var stuNum = new List<string>(); //模板的学号集合
                const double eps = 1e-10; // 精度范围            
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Sts_StuInfoTemp();
                    for (int j = 0; j <= 16; j++)
                    {
                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0:  //姓名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
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

                                    if (errorCount == 0)
                                        data.y_name = cell.StringCellValue.Trim();
                                }

                                break;
                            case 1: //性别
                                if (cell == null)
                                {
                                    cell = row.CreateCell(1);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    var sex = new Dictionary<string, int> { { "男", 0 }, { "女", 1 } };
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (!sex.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.y_sex = sex[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 2:  //考生号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_examNum ="";
                                }
                                else
                                {
                                    //if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    //{
                                    //    cell.SetCellValue("Can't be null");
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    //else if (examNumlist.Contains(cell.StringCellValue.Trim()) ||
                                    //         examNum.Contains(cell.StringCellValue.Trim()))
                                    //{
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    //if (errorCount == 0)
                                    //{
                                        data.y_examNum = cell.StringCellValue.Trim();
                                        examNum.Add(data.y_examNum);
                                    //}
                                }
                                break;
                            case 3: //入学年份
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue <= 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_inYear = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 4: //学习形式
                                if (cell == null)
                                {
                                    cell = row.CreateCell(4);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!stutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_stuTypeId = stutype[cell.StringCellValue.Trim()];
                                    }
                                    //if (errorCount == 0)

                                }
                                break;
                            case 5: //层次
                                if (cell == null)
                                {
                                    cell = row.CreateCell(5);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!edutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_eduTypeId = edutype[cell.StringCellValue.Trim()];
                                    }
                                    //if (errorCount == 0)
                                }
                                break;
                            case 6: //专业名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(6);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!majorlib.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                 
                                    else
                                    {
                                        data.y_majorLibId = majorlib[cell.StringCellValue.Trim()];
                                    }
                                    var major = majorist.FirstOrDefault(
                                            u => u.y_eduTypeId == data.y_eduTypeId &&
                                                 u.y_majorLibId == data.y_majorLibId &&
                                                 u.y_stuTypeId == data.y_stuTypeId);
                                    if (major != null)
                                    {
                                        data.y_majorId = major.id;
                                    }
                                    else
                                    {
                                        row.Cells[4].CellStyle = errorStyle;
                                        row.Cells[5].CellStyle = errorStyle;
                                        row.Cells[6].CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                }
                                break;                              
                            case 7: //站点
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_subSchoolId =null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))  //如果没有站点则为-1
                                    {
                                        data.y_subSchoolId =null;
                                        //cell.SetCellValue("Can't be null");
                                        //cell.CellStyle = errorStyle;
                                        //errorCount++;
                                    }
                                    else if (!sub.Keys.Contains(cell.StringCellValue.Trim()))//站点不匹配错误提示
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (errorCount == 0)
                                        data.y_subSchoolId = sub[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 8: //学籍状态
                                if (cell == null)
                                {
                                    cell = row.CreateCell(8);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!state.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_stuStateId = state[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 9: //电话
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_tel = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_tel = cell.StringCellValue.Trim();
                                }
                                break;
                            case 10: //地址
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_address = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_address = cell.StringCellValue.Trim();
                                }
                                break;
                            case 11: //身份证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_cardId = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    if (errorCount == 0)
                                        data.y_cardId = cell.StringCellValue.Trim();
                                }
                                break;
                            case 12: //出生日期
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_birthday = DateTime.Now;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    var birthdate = cell.StringCellValue.Trim();
                                    if (birthdate.Length != 8)
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    DateTime birthday;

                                    if (!DateTime.TryParseExact(birthdate, "yyyyMMdd",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out birthday))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                 //   DateTime.TryParse(birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" +
                                 //birthdate.Substring(6, 2), out birthday);
                                    if (errorCount == 0)
                                        data.y_birthday = birthday;
                                }
                                break;
                            case 13: //民族
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_nationId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!nation.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_nationId = nation[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 14: //政治面貌
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_politicsId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!pition.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_politicsId = pition[cell.StringCellValue.Trim()];
                                }
                                break;

                            case 15:  //学号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_stuNum = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (/*stuNumlist.Contains(cell.StringCellValue.Trim()) ||*/
                                             stuNum.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_stuNum = cell.StringCellValue.Trim();
                                        stuNum.Add(data.y_stuNum);
                                    }
                                }
                                break;


                            case 16://学位证号
                                if (cell == null)
                                {
                                    cell = row.CreateCell(16);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    if (errorCount == 0)
                                    {
                                        data.y_examFeatureName = cell.StringCellValue.Trim();

                                    }
                                }
                                break;
                        }
                        data.y_graduationdata = null;
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面
                }
                if (errorCount == 0)
                {
                    ad.Configuration.AutoDetectChangesEnabled = false;
                    ad.Configuration.ValidateOnSaveEnabled = false;
                    ad.Set<YD_Sts_StuInfoTemp>().AddRange(list);
                    ad.SaveChanges();
                }
                return list;
            }
        }


        /// <summary>
        /// 焦赞 2018年12月26日 江西理工用身份证匹配出函授站,没匹配出的函授站设置为校本部
        /// 陶杨杨  2019年1月5日 没匹配出的函授站设置为暂无。
        /// </summary>
        /// <param name="errorCount"></param>
        /// <param name="sheet"></param>
        /// <param name="errorStyle"></param>
        /// <returns></returns>
        public static List<YD_Sts_StuInfoTemp> StudentTempletValidateJXLG(ref int errorCount, ISheet sheet,
            ICellStyle errorStyle)
        {
            var edutype = new Dictionary<string, int>();
            var stutype = new Dictionary<string, int>();
            var majorlib = new Dictionary<string, int>();

            var state = new Dictionary<string, int>();
            var sub = new Dictionary<string, int>();
            var nation = new Dictionary<string, int>();
            var pition = new Dictionary<string, int>();
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            using (var ad = new IYunEntities())
            {
                ad.Database.ExecuteSqlCommand("DELETE FROM YD_Sts_StuInfoTemp");
                var majorist = ad.YD_Edu_Major.Select(u => new { u.id, u.y_eduTypeId, u.y_stuTypeId, u.y_majorLibId }).ToList();

                #region 
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
                ad.YD_Edu_StuState.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    state.Add(u.y_name, u.id);
                });

                if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_nameabbreviation }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_nameabbreviation, u.id);
                    });
                }
                else
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_name, u.id);
                    });
                }
                ad.YD_Sts_Nation.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    nation.Add(u.y_name, u.id);
                });
                ad.YD_Sts_Politics.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    pition.Add(u.y_name, u.id);
                });
                var examNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_examNum).ToList();

                var stuNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_stuNum).ToList();
                #endregion
                var list = new List<YD_Sts_StuInfoTemp>(); //数据转化List
                var examNum = new List<string>(); //模板的考生号集合

                var stuNum = new List<string>(); //模板的学号集合
                var stuSchoolIds = ad.YD_Sts_SubSchoolStuInfo.Where(e=>e.y_hide == 0 && e.y_isdel == 1).Select(e => new { e.y_cardId, e.y_subSchoolId });
                const double eps = 1e-10; // 精度范围            
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Sts_StuInfoTemp();
                    for (int j = 0; j < 16; j++)
                    {
                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0:  //姓名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
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

                                    if (errorCount == 0)
                                        data.y_name = cell.StringCellValue.Trim();
                                }

                                break;
                            case 1: //性别
                                if (cell == null)
                                {
                                    cell = row.CreateCell(1);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    var sex = new Dictionary<string, int> { { "男", 0 }, { "女", 1 } };
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (!sex.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.y_sex = sex[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 2:  //考生号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_examNum = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (examNumlist.Contains(cell.StringCellValue.Trim()) ||
                                             examNum.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_examNum = cell.StringCellValue.Trim();
                                        examNum.Add(data.y_examNum);
                                    }
                                }
                                break;
                            case 3: //入学年份
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue <= 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_inYear = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 4: //学习形式
                                if (cell == null)
                                {
                                    cell = row.CreateCell(4);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!stutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_stuTypeId = stutype[cell.StringCellValue.Trim()];
                                    }
                                    //if (errorCount == 0)

                                }
                                break;
                            case 5: //层次
                                if (cell == null)
                                {
                                    cell = row.CreateCell(5);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!edutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_eduTypeId = edutype[cell.StringCellValue.Trim()];
                                    }
                                    //if (errorCount == 0)
                                }
                                break;
                            case 6: //专业名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(6);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!majorlib.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    else
                                    {
                                        data.y_majorLibId = majorlib[cell.StringCellValue.Trim()];
                                    }
                                    var major = majorist.FirstOrDefault(
                                            u => u.y_eduTypeId == data.y_eduTypeId &&
                                                 u.y_majorLibId == data.y_majorLibId &&
                                                 u.y_stuTypeId == data.y_stuTypeId);
                                    if (major != null)
                                    {
                                        data.y_majorId = major.id;
                                    }
                                    else
                                    {
                                        row.Cells[4].CellStyle = errorStyle;
                                        row.Cells[5].CellStyle = errorStyle;
                                        row.Cells[6].CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                }
                                break;
                            case 7: //学籍状态
                                if (cell == null)
                                {
                                    cell = row.CreateCell(8);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!state.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_stuStateId = state[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 8: //电话
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_tel = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_tel = cell.StringCellValue.Trim();
                                }
                                break;
                            case 9: //地址
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_address = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_address = cell.StringCellValue.Trim();
                                }
                                break;
                            case 10: //身份证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_cardId = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    if (errorCount == 0)
                                        data.y_cardId = cell.StringCellValue.Trim();
                                }
                                break;
                            case 11: //出生日期
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_birthday = DateTime.Now;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    var birthdate = cell.StringCellValue.Trim();
                                    if (birthdate.Length != 8)
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    DateTime birthday;

                                    if (!DateTime.TryParseExact(birthdate, "yyyyMMdd",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out birthday))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    //   DateTime.TryParse(birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" +
                                    //birthdate.Substring(6, 2), out birthday);
                                    if (errorCount == 0)
                                        data.y_birthday = birthday;
                                }
                                break;
                            case 12: //民族
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_nationId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!nation.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_nationId = nation[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 13: //政治面貌
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_politicsId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!pition.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_politicsId = pition[cell.StringCellValue.Trim()];
                                }
                                break;

                            case 14:  //学号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_stuNum = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (stuNumlist.Contains(cell.StringCellValue.Trim()) ||
                                             stuNum.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_stuNum = cell.StringCellValue.Trim();
                                        stuNum.Add(data.y_stuNum);
                                    }
                                }
                                break;
                        }
                        data.y_graduationdata = null;
                    }

                    if (errorCount == 0)
                    {
                        var stuSchoolId = stuSchoolIds.FirstOrDefault(e => e.y_cardId == data.y_cardId);
                        if(stuSchoolId == null)
                        {
                            // 暂无
                            data.y_subSchoolId = 75;
                        }
                        else
                        {
                            data.y_subSchoolId = stuSchoolId.y_subSchoolId;
                        }
                        list.Add(data); //把数据加载到List里面
                    }
                }
                if (errorCount == 0)
                {
                    ad.Configuration.AutoDetectChangesEnabled = false;
                    ad.Configuration.ValidateOnSaveEnabled = false;
                    ad.Set<YD_Sts_StuInfoTemp>().AddRange(list);
                    ad.SaveChanges();
                }
                return list;
            }
        }

        /// <summary>
        /// 师范专用 学生导入模板验证   
        /// 不验证教学计划
        /// </summary>
        /// <param name="errorCount"></param>
        /// <param name="sheet"></param>
        /// <param name="errorStyle"></param>
        /// <returns></returns>
        public static List<YD_Sts_StuInfoTemp> StudentTempletValidate2(ref int errorCount, ISheet sheet,
            ICellStyle errorStyle)
        {
            var edutype = new Dictionary<string, int>();
            var stutype = new Dictionary<string, int>();
            var majorlib = new Dictionary<string, int>();

            var state = new Dictionary<string, int>();
            var sub = new Dictionary<string, int>();
            var nation = new Dictionary<string, int>();
            var pition = new Dictionary<string, int>();
            var schoolname = ConfigurationManager.AppSettings["SchoolName"];
            using (var ad = new IYunEntities())
            {
                ad.Database.ExecuteSqlCommand("DELETE FROM YD_Sts_StuInfoTemp");
                var majorist = ad.YD_Edu_Major.Select(u => new { u.id, u.y_eduTypeId, u.y_stuTypeId, u.y_majorLibId }).ToList();

                #region 
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
                ad.YD_Edu_StuState.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    state.Add(u.y_name, u.id);
                });

                if (schoolname == ComEnum.SchoolName.JXLG.ToString())
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_nameabbreviation }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_nameabbreviation, u.id);
                    });
                }
                else
                {
                    ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                    {
                        sub.Add(u.y_name, u.id);
                    });
                }
                ad.YD_Sts_Nation.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    nation.Add(u.y_name, u.id);
                });
                ad.YD_Sts_Politics.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                {
                    pition.Add(u.y_name, u.id);
                });
                var examNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_examNum).ToList();

                var stuNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_stuNum).ToList();
                #endregion
                var list = new List<YD_Sts_StuInfoTemp>(); //数据转化List
                var examNum = new List<string>(); //模板的考生号集合

                var stuNum = new List<string>(); //模板的学号集合
                const double eps = 1e-10; // 精度范围            
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Sts_StuInfoTemp();
                    string stutypename="";
                    string edutypename="";
                    for (int j = 0; j < 16; j++)
                    {
                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0:  //姓名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
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

                                    if (errorCount == 0)
                                        data.y_name = cell.StringCellValue.Trim();
                                }

                                break;
                            case 1: //性别
                                if (cell == null)
                                {
                                    cell = row.CreateCell(1);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    var sex = new Dictionary<string, int> { { "男", 0 }, { "女", 1 } };
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (!sex.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.y_sex = sex[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 2:  //考生号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_examNum = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (examNumlist.Contains(cell.StringCellValue.Trim()) ||
                                             examNum.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_examNum = cell.StringCellValue.Trim();
                                        examNum.Add(data.y_examNum);
                                    }
                                }
                                break;
                            case 3: //入学年份
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue <= 0 ||
                                        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_inYear = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 4: //学习形式
                                if (cell == null)
                                {
                                    cell = row.CreateCell(4);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!stutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_stuTypeId = stutype[cell.StringCellValue.Trim()];
                                        stutypename = cell.StringCellValue.Trim();
                                    }
                                    //if (errorCount == 0)

                                }
                                break;
                            case 5: //层次
                                if (cell == null)
                                {
                                    cell = row.CreateCell(5);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!edutype.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else
                                    {
                                        data.y_eduTypeId = edutype[cell.StringCellValue.Trim()];
                                        edutypename = cell.StringCellValue.Trim();
                                    }
                                    //if (errorCount == 0)
                                }
                                break;
                            case 6: //专业名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(6);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!majorlib.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        //cell.CellStyle = errorStyle;
                                        //errorCount++;
                                        var newlibrary = new YD_Edu_MajorLibrary()
                                        {
                                            y_code = ad.YD_Edu_MajorLibrary.Any() ? (ad.YD_Edu_MajorLibrary.Max(x => x.id) + 1).ToString() : "111",
                                            y_name = cell.StringCellValue.Trim()
                                        };
                                        ad.YD_Edu_MajorLibrary.Add(newlibrary);
                                        ad.SaveChanges();
                                        majorlib.Add(newlibrary.y_name, newlibrary.id);
                                    }
                                    data.y_majorLibId = majorlib[cell.StringCellValue.Trim()];
                                    var major = majorist.FirstOrDefault(
                                            u => u.y_eduTypeId == data.y_eduTypeId &&
                                                 u.y_majorLibId == data.y_majorLibId &&
                                                 u.y_stuTypeId == data.y_stuTypeId);
                                    if (major != null)
                                    {
                                        data.y_majorId = major.id;
                                    }
                                    else
                                    {
                                        int majorId = ad.YD_Edu_Major.Any() ? ad.YD_Edu_Major.Max(u => u.id)+1 : 1;
                                        int needFer, stuYear;
                                        if (edutypename.Trim() == "高起本")
                                        {
                                            needFer = 1700;
                                            stuYear = 5;
                                        }
                                        else
                                        {
                                            needFer = 1300;
                                            stuYear = 3;
                                        }
                                        var newmajor = new YD_Edu_Major()
                                        {
                                            y_majorLibId = data.y_majorLibId.Value,
                                            y_stuTypeId = data.y_stuTypeId.Value,
                                            y_eduTypeId = data.y_eduTypeId.Value,
                                            y_name = $"{cell.StringCellValue.Trim()}{edutypename}{stutypename}",
                                            y_code = majorId.ToString(),
                                            y_needFee = needFer,
                                            y_stuYear = stuYear

                                        };
                                        ad.YD_Edu_Major.Add(newmajor);
                                        ad.SaveChanges();
                                        int id = majorId;
                                        majorist.Add(new { id, newmajor.y_eduTypeId, newmajor.y_stuTypeId, newmajor.y_majorLibId });
                                    }
                                }
                                break;
                            case 7: //站点
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_subSchoolId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))  //如果没有站点则为-1
                                    {
                                        data.y_subSchoolId = null;
                                        //cell.SetCellValue("Can't be null");
                                        //cell.CellStyle = errorStyle;
                                        //errorCount++;
                                    }
                                    else if (!sub.Keys.Contains(cell.StringCellValue.Trim()))//站点不匹配错误提示
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    else if (errorCount == 0)
                                        data.y_subSchoolId = sub[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 8: //学籍状态
                                if (cell == null)
                                {
                                    cell = row.CreateCell(8);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!state.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_stuStateId = state[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 9: //电话
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_tel = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_tel = cell.StringCellValue.Trim();
                                }
                                break;
                            case 10: //地址
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_address = null;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_address = cell.StringCellValue.Trim();
                                }
                                break;
                            case 11: //身份证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_cardId = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        //cell.SetCellValue("Can't be null");
                                        //cell.CellStyle = errorStyle;
                                        //errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_cardId = cell.StringCellValue.Trim();
                                }
                                break;
                            case 12: //出生日期
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_birthday = DateTime.Now;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    var birthdate = cell.StringCellValue.Trim();
                                    if (birthdate.Length != 8)
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    DateTime birthday;

                                    if (!DateTime.TryParseExact(birthdate, "yyyyMMdd",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None, out birthday))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    //   DateTime.TryParse(birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" +
                                    //birthdate.Substring(6, 2), out birthday);
                                    if (errorCount == 0)
                                        data.y_birthday = birthday;
                                }
                                break;
                            case 13: //民族
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_nationId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!nation.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_nationId = nation[cell.StringCellValue.Trim()];
                                }
                                break;
                            case 14: //政治面貌
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_politicsId = null;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是NUMERIC 类型
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
                                    else if (!pition.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_politicsId = pition[cell.StringCellValue.Trim()];
                                }
                                break;

                            case 15:  //学号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_stuNum = "";
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (stuNumlist.Contains(cell.StringCellValue.Trim()) ||
                                             stuNum.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                    {
                                        data.y_stuNum = cell.StringCellValue.Trim();
                                        stuNum.Add(data.y_stuNum);
                                    }
                                }
                                break;
                            case 16:

                        

                                            data.y_examFeatureName = cell.StringCellValue.Trim();//y_examFeatureName暂用为毕业证号

                                break;

                                //case 16://学位证号
                                //    if (cell == null)
                                //    {
                                //        cell = row.createcell(16);
                                //        cell.cellstyle = errorstyle;
                                //        cell.setcellvalue("不能为空");
                                //        errorcount++;
                                //    }
                                //    else if (cell.celltype != celltype.string) //如果不是string 类型
                                //    {
                                //        cell.cellstyle = errorstyle;
                                //        errorcount++;
                                //    }
                                //    else
                                //    {
                                //        if (string.isnullorwhitespace(cell.stringcellvalue))
                                //        {
                                //            cell.setcellvalue("can't be null");
                                //            cell.cellstyle = errorstyle;
                                //            errorcount++;
                                //        }                              
                                //        if (errorcount == 0)
                                //        {
                                //            data.y_examfeaturename = cell.stringcellvalue.trim();

                                //        }
                                //    }
                                //    break;
                        }
                        data.y_graduationdata = null;
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面
                }
                if (errorCount == 0)
                {
                    ad.Configuration.AutoDetectChangesEnabled = false;
                    ad.Configuration.ValidateOnSaveEnabled = false;
                    ad.Set<YD_Sts_StuInfoTemp>().AddRange(list);
                    ad.SaveChanges();
                }
                return list;
            }
        }
        /********************************************************************
      * 作者：刘天华   2017-11-30  
      * 用途：处理Excel添加到考生表里
      ********************************************************************/

        /// <summary>
        /// 处理Excel添加到考生表里
        /// </summary>
        /// <param name="list">list</param>
        public static void UploadSubStu(List<YD_Sts_SubStuTemp> list)
        {
            using (var ad = new IYunEntities())
            {
                
                var stulist = new List<YD_Sts_SubSchoolStuInfo>();
                var stuinfo = new YD_Sts_SubSchoolStuInfo();

                foreach (var stu in list)
                {
                    if (!ad.YD_Sts_SubSchoolStuInfo.Any(
                                u => u.y_cardId == stu.y_cardId && u.y_name == stu.y_name && u.y_examNum == stu.y_examNum && u.y_subSchoolId == stu.y_subSchoolId && u.y_year == stu.y_year && u.y_isdel==1&& u.y_hide==1))
                    {
                        stuinfo = new YD_Sts_SubSchoolStuInfo
                        {
                            y_subSchoolId = stu.y_subSchoolId,
                            y_name = stu.y_name,
                            y_examNum = stu.y_examNum,
                            y_cardId = stu.y_cardId,
                            y_isdel = (int)YesOrNo.No,
                            y_hide=(int)YesOrNo.No,
                            y_year = stu.y_year,
                        };
                        var stucard = ad.YD_Sts_StuInfo.FirstOrDefault(u=>u.y_cardId ==stu.y_cardId && u.y_inYear ==stu.y_year);
                        if (stucard !=null  && stucard.y_subSchoolId==null && stu.y_subSchoolId!=null)
                        {
                            stuinfo.y_hide = (int)YesOrNo.Yes;
                            stucard.y_subSchoolId = stu.y_subSchoolId;
                            ad.Entry(stucard).State = EntityState.Modified;
                        }
                        
                        stulist.Add(stuinfo);
                    }
                }
                ad.Configuration.AutoDetectChangesEnabled = false;
                ad.Configuration.ValidateOnSaveEnabled = false;
                ad.Set<YD_Sts_SubSchoolStuInfo>().AddRange(stulist);
                ad.SaveChanges();
            }
          }

        /********************************************************************
        * 作者：胡惠贞   2017-4-6 9:11
        * 用途：处理Excel添加到学生表里
        ********************************************************************/

        /// <summary>
        /// 处理Excel添加到学生表里
        /// </summary>
        /// <param name="list">list</param>
        public static void UploadTrueStu(List<YD_Sts_StuInfoTemp> list)
        {
            using (var ad = new IYunEntities())
            {
                var substulist = ad.YD_Sts_SubSchoolStuInfo.Where(u => u.y_isdel == 1 && u.y_hide == 1).AsQueryable();
                //获取考生信息
                var stulist = new List<YD_Sts_StuInfo>();
                var stuinfo = new YD_Sts_StuInfo();
                foreach (var stu in list)
                {
                    var substulistone = substulist.Where(u => u.y_cardId == stu.y_cardId).ToList(); //函授站考生导入表
                    
                    if (substulistone.Count==1)
                    {
                        stuinfo = new YD_Sts_StuInfo
                        {
                            y_name = stu.y_name,
                            // y_stuNum = "",
                            y_stuNum = stu.y_stuNum,
                            y_examNum = stu.y_examNum,
                            y_sex = Convert.ToInt32(stu.y_sex),
                            y_inYear = Convert.ToInt32(stu.y_inYear),
                            y_cardId = stu.y_cardId,
                            y_majorId = Convert.ToInt32(stu.y_majorId),
                            //y_subSchoolId = Convert.ToInt32(substulistone[0].y_subSchoolId),
                            y_subSchoolId = Convert.ToInt32(substulistone[0].y_subSchoolId),
                            y_stuStateId = Convert.ToInt32(stu.y_stuStateId),
                            y_tel = stu.y_tel,
                            y_address = stu.y_address,
                            y_isChangePlan = (int)YesOrNo.No,
                            y_isdel = (int)YesOrNo.No,
                            y_nationId = stu.y_nationId,
                            y_politicsId = stu.y_politicsId,
                            y_graduationschool = "",
                            y_graduationdata = null,
                            y_registrationNum = null,
                            y_postalcode = null,
                            y_birthday = Convert.ToDateTime(stu.y_birthday),
                            y_foreignLanguageId = null,
                            y_recruitTypeId = null,
                            y_professionTypeId = null,
                            y_cultureExtentId = null,
                            y_examFeatureId = null,
                            y_graduateNumber = stu.y_examFeatureName
                            //y_degreeNum= stu.y_examFeatureName
                        };
                    }
                    else if (substulistone.Count == 0)
                    {
                        stuinfo = new YD_Sts_StuInfo
                        {
                            y_name = stu.y_name,
                            // y_stuNum = "",
                            y_stuNum = stu.y_stuNum,
                            y_examNum = stu.y_examNum,
                            y_sex = Convert.ToInt32(stu.y_sex),
                            y_inYear = Convert.ToInt32(stu.y_inYear),
                            y_cardId = stu.y_cardId,
                            y_majorId = Convert.ToInt32(stu.y_majorId),
                            y_subSchoolId = stu.y_subSchoolId,
                            y_stuStateId = Convert.ToInt32(stu.y_stuStateId),
                            y_tel = stu.y_tel,
                            y_address = stu.y_address,
                            y_isChangePlan = (int) YesOrNo.No,
                            y_isdel = (int) YesOrNo.No,
                            y_nationId = stu.y_nationId,
                            y_politicsId = stu.y_politicsId,
                            y_graduationschool = "",
                            y_graduationdata = null,
                            y_registrationNum = null,
                            y_postalcode = null,
                            y_birthday = Convert.ToDateTime(stu.y_birthday),
                            y_foreignLanguageId = null,
                            y_recruitTypeId = null,
                            y_professionTypeId = null,
                            y_cultureExtentId = null,
                            y_examFeatureId = null,
                            y_graduateNumber = stu.y_examFeatureName
                            //y_degreeNum = stu.y_examFeatureName
                        };
                    }
                    else
                    {
                        stuinfo = new YD_Sts_StuInfo
                        {
                            y_name = stu.y_name,
                            // y_stuNum = "",
                            y_stuNum = stu.y_stuNum,
                            y_examNum = stu.y_examNum,
                            y_sex = Convert.ToInt32(stu.y_sex),
                            y_inYear = Convert.ToInt32(stu.y_inYear),
                            y_cardId = stu.y_cardId,
                            y_majorId = Convert.ToInt32(stu.y_majorId),
                            y_subSchoolId = stu.y_subSchoolId,
                            y_stuStateId = Convert.ToInt32(stu.y_stuStateId),
                            y_tel = stu.y_tel,
                            y_address = stu.y_address,
                            y_isChangePlan = (int)YesOrNo.No,
                            y_isdel = (int)YesOrNo.No,
                            y_nationId = stu.y_nationId,
                            y_politicsId = stu.y_politicsId,
                            y_graduationschool = "",
                            y_graduationdata = null,
                            y_registrationNum = null,
                            y_postalcode = null,
                            y_birthday = Convert.ToDateTime(stu.y_birthday),
                            y_foreignLanguageId = null,
                            y_recruitTypeId = null,
                            y_professionTypeId = null,
                            y_cultureExtentId = null,
                            y_examFeatureId = null,
                            y_graduateNumber = stu.y_examFeatureName
                            //y_degreeNum = stu.y_examFeatureName
                        };
                    }

                    stulist.Add(stuinfo);
                }
                ad.Configuration.AutoDetectChangesEnabled = false;
                ad.Configuration.ValidateOnSaveEnabled = false;
                ad.Set<YD_Sts_StuInfo>().AddRange(stulist);
                int r = ad.SaveChanges();
                if (r > 0)
                {
                    #region 生成学生所有缴费学年的记录

                    //var students = ad.YD_Sts_StuInfoTemp.Select(u => u.y_inYear);
                    //生成第一缴费学年的学生名单
                    //var oneYearStus = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear)).ToList();

                    //var stuFeeTbsOne = new List<YD_Fee_StuFeeTb>();


                    //#region 收集指定年份需上缴第一个学年的学生

                    //foreach (var oneYearStu in oneYearStus)
                    //{
                    //    //存在记录不用添加
                    //    if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 1))
                    //    {
                    //        int needFee = 0;
                    //        //先查询是否存在函授站特定的收费标准
                    //        var subFee =
                    //            ad.YD_Fee_SubFeeSys.FirstOrDefault(
                    //                u =>
                    //                    u.y_subSchoolId == oneYearStu.y_subSchoolId &&
                    //                    u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //        if (subFee != null)
                    //        {
                    //            needFee = subFee.y_StuFee;
                    //        }
                    //        else
                    //        {
                    //            var fee =
                    //                ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (fee != null)
                    //            {
                    //                needFee = fee.y_stuFee;
                    //            }
                    //        }
                    //        var stuFee = new YD_Fee_StuFeeTb();
                    //        stuFee.y_feeYear = 1;
                    //        stuFee.y_isUp = (int) YesOrNo.No;
                    //        stuFee.y_isCheckFee = (int) YesOrNo.No;
                    //        stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //        stuFee.y_isdel = oneYearStu.y_isdel;
                    //        stuFee.y_loginName = oneYearStu.y_loginName;
                    //        stuFee.y_mail = oneYearStu.y_mail;
                    //        stuFee.y_majorId = oneYearStu.y_majorId;
                    //        stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //        stuFee.y_name = oneYearStu.y_name;
                    //        stuFee.y_nationId = oneYearStu.y_nationId;
                    //        stuFee.y_password = oneYearStu.y_password;
                    //        stuFee.y_politicsId = oneYearStu.y_politicsId;
                    //        stuFee.y_registerState = oneYearStu.y_registerState;
                    //        stuFee.y_sex = oneYearStu.y_sex;
                    //        stuFee.y_stuId = oneYearStu.id;
                    //        stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //        stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //        stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //        stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //        stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;
                    //        stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //        //如果是中医药大学则改变学制
                    //        if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //            ComEnum.SchoolName.ZYYDX.ToString())
                    //        {
                    //            if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //            //如果专业为医药营销则学制为3
                    //            var marsql = "y_name like '医药营销 %'";
                    //            var marlist =
                    //                ad.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + marsql)
                    //                    .ToList()
                    //                    .Select(u => u.y_name);
                    //            if (marlist.Contains(stuFee.majorName))
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //        }

                    //        stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //        stuFee.y_tel = oneYearStu.y_tel;
                    //        stuFee.y_upFee = 0;
                    //        stuFee.y_needFee = needFee;

                    //        //新添加的字段
                    //        stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //        stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //        stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //        stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //        stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //        stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //        stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //        stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //        stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //        stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //        stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //        stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //        stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //        stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //        stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //        stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //        stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //        stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //        stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //        #region 根据分配比例计算需上缴的费用

                    //        if (stuFee.y_needFee != 0)
                    //        {
                    //            var subBili = ad.YD_Fee_SubFeeBili.FirstOrDefault(
                    //                u =>
                    //                    u.y_eduTypeId == stuFee.y_eduTypeId && u.y_subSchoolId == stuFee.y_subSchoolId);
                    //            if (subBili == null)
                    //            {
                    //                var bili = ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                if (bili == null)
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee;
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee*bili.y_bili/100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = stuFee.y_needFee*subBili.y_bili/100;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_needUpFee = 0;
                    //        }

                    //        #endregion

                    //        stuFeeTbsOne.Add(stuFee);
                    //    }
                    //}

                    //#endregion

                    //#region toda:用不上的第二学年以后的缴费记录
                    ////生成第二缴费学年的学生名单
                    //var twoYearStus = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear)).ToList();
                    ////生成第三缴费学年的学生名单
                    //var threeYearStus = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear)).ToList();
                    ////生成第四缴费学年的学生名单
                    //var fourYearStus = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear) && u.y_eduTypeId == 1).ToList();
                    //var fourYearStus2 = new List<VW_StuInfo>();
                    ////如果是中医药大学则需要生成高起专层次的第四缴费学年的名单
                    //if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    //{
                    //    fourYearStus2 = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear) && u.y_eduTypeId == 2).ToList();
                    //}
                    //var majorfourYearStus = new List<VW_StuInfo>();
                    ////生成4年学制专业的第四缴费学年的学生名单
                    //var majors = ad.YD_Edu_Major.Where(u => u.y_stuYear == 4).Select(u => u.id);

                    //majorfourYearStus = ad.VW_StuInfo.Where(u => majors.Contains(u.y_majorId) && students.Contains(u.y_inYear)).ToList();
                    ////生成第五缴费学年的学生名单
                    //var fiveYearStus =
                    //    ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear) && u.y_eduTypeId == 1).ToList();
                    //var stuFeeTbsTwo = new List<YD_Fee_StuFeeTb>();
                    //var stuFeeTbsThree = new List<YD_Fee_StuFeeTb>();
                    //var stuFeeTbsFour = new List<YD_Fee_StuFeeTb>();
                    //var stuFeeTbsFour2 = new List<YD_Fee_StuFeeTb>();
                    //var majorstuFeeTbsFour = new List<YD_Fee_StuFeeTb>();
                    //var stuFeeTbsFive = new List<YD_Fee_StuFeeTb>();
                    //#region 收集指定年份需上缴第二个学年的学生

                    //foreach (var oneYearStu in twoYearStus)
                    //{
                    //    //存在记录不用添加
                    //    if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 2))
                    //    {
                    //        int needFee = 0;
                    //        //先查询是否存在函授站特定的收费标准
                    //        var subFee =ad.YD_Fee_SubFeeSys.FirstOrDefault(u =>u.y_subSchoolId == oneYearStu.y_subSchoolId &&u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //        if (subFee != null)
                    //        {
                    //            needFee = subFee.y_StuFee;
                    //        }
                    //        else
                    //        {
                    //            var fee =ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (fee != null)
                    //            {
                    //                needFee = fee.y_bookFee + fee.y_stuFee;
                    //            }
                    //        }
                    //        var stuFee = new YD_Fee_StuFeeTb();
                    //        stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //        stuFee.majorCode = oneYearStu.majorCode;
                    //        stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //        stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //        stuFee.majorName = oneYearStu.majorName;
                    //        stuFee.nationName = oneYearStu.nationName;
                    //        stuFee.politicsName = oneYearStu.politicsName;
                    //        stuFee.schoolCode = oneYearStu.schoolCode;
                    //        stuFee.schoolName = oneYearStu.schoolName;
                    //        stuFee.stuStateName = oneYearStu.stuStateName;
                    //        stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //        stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //        stuFee.y_address = oneYearStu.y_address;
                    //        stuFee.y_birthday = oneYearStu.y_birthday;
                    //        stuFee.y_cardId = oneYearStu.y_cardId;
                    //        stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //        stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //        stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //        stuFee.y_examNum = oneYearStu.y_examNum;
                    //        stuFee.y_feeYear = 2;
                    //        stuFee.y_isUp = (int)YesOrNo.No;
                    //        stuFee.y_img = oneYearStu.y_img;
                    //        stuFee.y_inYear = oneYearStu.y_inYear;
                    //        stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //        stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //        stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //        stuFee.y_isdel = oneYearStu.y_isdel;
                    //        stuFee.y_loginName = oneYearStu.y_loginName;
                    //        stuFee.y_mail = oneYearStu.y_mail;
                    //        stuFee.y_majorId = oneYearStu.y_majorId;
                    //        stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //        stuFee.y_name = oneYearStu.y_name;
                    //        stuFee.y_nationId = oneYearStu.y_nationId;

                    //        stuFee.y_password = oneYearStu.y_password;
                    //        stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //        stuFee.y_registerState = oneYearStu.y_registerState;
                    //        stuFee.y_sex = oneYearStu.y_sex;
                    //        stuFee.y_stuId = oneYearStu.id;
                    //        stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //        stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //        stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //        stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //        stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //        stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);

                    //        //如果是中医药大学则改变学制
                    //        if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //            ComEnum.SchoolName.ZYYDX.ToString())
                    //        {
                    //            if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //            //如果专业为医药营销则学制为3
                    //            var marsql = "y_name like '医药营销 %'";
                    //            var marlist =
                    //                ad.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + marsql)
                    //                    .ToList()
                    //                    .Select(u => u.y_name);
                    //            if (marlist.Contains(stuFee.majorName))
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //        }

                    //        stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //        stuFee.y_tel = oneYearStu.y_tel;
                    //        stuFee.y_upFee = 0;
                    //        stuFee.y_needFee = needFee;

                    //        //新添加的字段
                    //        stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //        stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //        stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //        stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //        stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //        stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //        stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //        stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //        stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //        stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //        stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //        stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //        stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //        stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //        stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //        stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //        stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //        stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //        stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //        #region 根据分配比例计算需上缴的费用

                    //        if (stuFee.y_needFee != 0)
                    //        {
                    //            var subBili = ad.YD_Fee_SubFeeBili.FirstOrDefault(
                    //                u =>
                    //                    u.y_eduTypeId == stuFee.y_eduTypeId && u.y_subSchoolId == stuFee.y_subSchoolId);
                    //            if (subBili == null)
                    //            {
                    //                var bili = ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                if (bili == null)
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee;
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_needUpFee = 0;
                    //        }

                    //        #endregion

                    //        stuFeeTbsTwo.Add(stuFee);
                    //    }
                    //}

                    //#endregion

                    //#region 收集指定年份需上缴第三个学年的学生

                    //foreach (var oneYearStu in threeYearStus)
                    //{
                    //    //存在记录不用添加
                    //    if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 3))
                    //    {
                    //        int needFee = 0;
                    //        //先查询是否存在函授站特定的收费标准
                    //        var subFee =
                    //            ad.YD_Fee_SubFeeSys.FirstOrDefault(
                    //                u =>
                    //                    u.y_subSchoolId == oneYearStu.y_subSchoolId &&
                    //                    u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //        if (subFee != null)
                    //        {
                    //            needFee = subFee.y_StuFee;
                    //        }
                    //        else
                    //        {
                    //            var fee =
                    //                ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (fee != null)
                    //            {
                    //                needFee = fee.y_bookFee + fee.y_stuFee;
                    //            }
                    //        }
                    //        var stuFee = new YD_Fee_StuFeeTb();
                    //        stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //        stuFee.majorCode = oneYearStu.majorCode;
                    //        stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //        stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //        stuFee.majorName = oneYearStu.majorName;
                    //        stuFee.nationName = oneYearStu.nationName;
                    //        stuFee.politicsName = oneYearStu.politicsName;
                    //        stuFee.schoolCode = oneYearStu.schoolCode;
                    //        stuFee.schoolName = oneYearStu.schoolName;
                    //        stuFee.stuStateName = oneYearStu.stuStateName;
                    //        stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //        stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //        stuFee.y_address = oneYearStu.y_address;
                    //        stuFee.y_birthday = oneYearStu.y_birthday;
                    //        stuFee.y_cardId = oneYearStu.y_cardId;
                    //        stuFee.y_isUp = (int)YesOrNo.No;
                    //        stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //        stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //        stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //        stuFee.y_examNum = oneYearStu.y_examNum;
                    //        stuFee.y_feeYear = 3;
                    //        stuFee.y_img = oneYearStu.y_img;
                    //        stuFee.y_inYear = oneYearStu.y_inYear;
                    //        stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //        stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //        stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //        stuFee.y_isdel = oneYearStu.y_isdel;
                    //        stuFee.y_loginName = oneYearStu.y_loginName;
                    //        stuFee.y_mail = oneYearStu.y_mail;
                    //        stuFee.y_majorId = oneYearStu.y_majorId;
                    //        stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //        stuFee.y_name = oneYearStu.y_name;
                    //        stuFee.y_nationId = oneYearStu.y_nationId;

                    //        stuFee.y_password = oneYearStu.y_password;
                    //        stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //        stuFee.y_registerState = oneYearStu.y_registerState;
                    //        stuFee.y_sex = oneYearStu.y_sex;
                    //        stuFee.y_stuId = oneYearStu.id;
                    //        stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //        stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //        stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //        stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //        stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //        stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //        //如果是中医药大学则改变学制
                    //        if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //            ComEnum.SchoolName.ZYYDX.ToString())
                    //        {
                    //            if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //            //如果专业为医药营销则学制为3
                    //            var marsql = "y_name like '医药营销 %'";
                    //            var marlist =
                    //                ad.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + marsql)
                    //                    .ToList()
                    //                    .Select(u => u.y_name);
                    //            if (marlist.Contains(stuFee.majorName))
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //        }

                    //        stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //        stuFee.y_tel = oneYearStu.y_tel;
                    //        stuFee.y_upFee = 0;
                    //        stuFee.y_needFee = needFee;

                    //        //新添加的字段
                    //        stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //        stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //        stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //        stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //        stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //        stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //        stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //        stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //        stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //        stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //        stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //        stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //        stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //        stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //        stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //        stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //        stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //        stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //        stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //        #region 根据分配比例计算需上缴的费用

                    //        if (stuFee.y_needFee != 0)
                    //        {
                    //            var subBili =
                    //                ad.YD_Fee_SubFeeBili.FirstOrDefault(
                    //                    u =>
                    //                        u.y_eduTypeId == stuFee.y_eduTypeId &&
                    //                        u.y_subSchoolId == stuFee.y_subSchoolId);
                    //            if (subBili == null)
                    //            {
                    //                var bili = ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                if (bili == null)
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee;
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_needUpFee = 0;
                    //        }

                    //        #endregion

                    //        stuFeeTbsThree.Add(stuFee);
                    //    }
                    //}

                    //#endregion

                    //#region 收集指定年份需上缴第四个学年的学生

                    //foreach (var oneYearStu in fourYearStus)
                    //{
                    //    //存在记录不用添加
                    //    if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 4))
                    //    {
                    //        int needFee = 0;
                    //        //先查询是否存在函授站特定的收费标准
                    //        var subFee =
                    //            ad.YD_Fee_SubFeeSys.FirstOrDefault(
                    //                u =>
                    //                    u.y_subSchoolId == oneYearStu.y_subSchoolId &&
                    //                    u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //        if (subFee != null)
                    //        {
                    //            needFee = subFee.y_StuFee;
                    //        }
                    //        else
                    //        {
                    //            var fee =
                    //                ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (fee != null)
                    //            {
                    //                needFee = fee.y_bookFee + fee.y_stuFee;
                    //            }
                    //        }

                    //        var stuFee = new YD_Fee_StuFeeTb();
                    //        stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //        stuFee.majorCode = oneYearStu.majorCode;
                    //        stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //        stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //        stuFee.majorName = oneYearStu.majorName;
                    //        stuFee.nationName = oneYearStu.nationName;
                    //        stuFee.politicsName = oneYearStu.politicsName;
                    //        stuFee.schoolCode = oneYearStu.schoolCode;
                    //        stuFee.schoolName = oneYearStu.schoolName;
                    //        stuFee.stuStateName = oneYearStu.stuStateName;
                    //        stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //        stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //        stuFee.y_address = oneYearStu.y_address;
                    //        stuFee.y_birthday = oneYearStu.y_birthday;
                    //        stuFee.y_cardId = oneYearStu.y_cardId;
                    //        stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //        stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //        stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //        stuFee.y_examNum = oneYearStu.y_examNum;
                    //        stuFee.y_feeYear = 4;
                    //        stuFee.y_isUp = (int)YesOrNo.No;
                    //        stuFee.y_img = oneYearStu.y_img;
                    //        stuFee.y_inYear = oneYearStu.y_inYear;
                    //        stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //        stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //        stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //        stuFee.y_isdel = oneYearStu.y_isdel;
                    //        stuFee.y_loginName = oneYearStu.y_loginName;
                    //        stuFee.y_mail = oneYearStu.y_mail;
                    //        stuFee.y_majorId = oneYearStu.y_majorId;
                    //        stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //        stuFee.y_name = oneYearStu.y_name;
                    //        stuFee.y_nationId = oneYearStu.y_nationId;

                    //        stuFee.y_password = oneYearStu.y_password;
                    //        stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //        stuFee.y_registerState = oneYearStu.y_registerState;
                    //        stuFee.y_sex = oneYearStu.y_sex;
                    //        stuFee.y_stuId = oneYearStu.id;
                    //        stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //        stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //        stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //        stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //        stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //        stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //        //如果是中医药大学则改变学制
                    //        if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //            ComEnum.SchoolName.ZYYDX.ToString())
                    //        {
                    //            if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //            //如果专业为医药营销则学制为3
                    //            var marsql = "y_name like '医药营销 %'";
                    //            var marlist =
                    //                ad.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + marsql)
                    //                    .ToList()
                    //                    .Select(u => u.y_name);
                    //            if (marlist.Contains(stuFee.majorName))
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //        }

                    //        stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //        stuFee.y_tel = oneYearStu.y_tel;
                    //        stuFee.y_upFee = 0;
                    //        stuFee.y_needFee = needFee;

                    //        //新添加的字段
                    //        stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //        stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //        stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //        stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //        stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //        stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //        stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //        stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //        stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //        stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //        stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //        stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //        stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //        stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //        stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //        stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //        stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //        stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //        stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //        #region 根据分配比例计算需上缴的费用

                    //        if (stuFee.y_needFee != 0)
                    //        {
                    //            var subBili =
                    //                ad.YD_Fee_SubFeeBili.FirstOrDefault(
                    //                    u =>
                    //                        u.y_eduTypeId == stuFee.y_eduTypeId &&
                    //                        u.y_subSchoolId == stuFee.y_subSchoolId);
                    //            if (subBili == null)
                    //            {
                    //                var bili = ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                if (bili == null)
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee;
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_needUpFee = 0;
                    //        }

                    //        #endregion

                    //        stuFeeTbsFour.Add(stuFee);
                    //    }
                    //}

                    //#endregion

                    //if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    //{
                    //    #region 收集中医药大学高起专层次指定年份需上缴第四个学年的学生

                    //    foreach (var oneYearStu in fourYearStus2)
                    //    {
                    //        //存在记录不用添加
                    //        if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 4))
                    //        {
                    //            int needFee = 0;
                    //            //先查询是否存在函授站特定的收费标准
                    //            var subFee =
                    //                ad.YD_Fee_SubFeeSys.FirstOrDefault(
                    //                    u =>
                    //                        u.y_subSchoolId == oneYearStu.y_subSchoolId &&
                    //                        u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (subFee != null)
                    //            {
                    //                needFee = subFee.y_StuFee;
                    //            }
                    //            else
                    //            {
                    //                var fee =
                    //                    ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //                if (fee != null)
                    //                {
                    //                    needFee = fee.y_bookFee + fee.y_stuFee;
                    //                }
                    //            }

                    //            var stuFee = new YD_Fee_StuFeeTb();
                    //            stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //            stuFee.majorCode = oneYearStu.majorCode;
                    //            stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //            stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //            stuFee.majorName = oneYearStu.majorName;
                    //            stuFee.nationName = oneYearStu.nationName;
                    //            stuFee.politicsName = oneYearStu.politicsName;
                    //            stuFee.schoolCode = oneYearStu.schoolCode;
                    //            stuFee.schoolName = oneYearStu.schoolName;
                    //            stuFee.stuStateName = oneYearStu.stuStateName;
                    //            stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //            stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //            stuFee.y_address = oneYearStu.y_address;
                    //            stuFee.y_birthday = oneYearStu.y_birthday;
                    //            stuFee.y_cardId = oneYearStu.y_cardId;
                    //            stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //            stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //            stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //            stuFee.y_examNum = oneYearStu.y_examNum;
                    //            stuFee.y_feeYear = 4;
                    //            stuFee.y_isUp = (int)YesOrNo.No;
                    //            stuFee.y_img = oneYearStu.y_img;
                    //            stuFee.y_inYear = oneYearStu.y_inYear;
                    //            stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //            stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //            stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //            stuFee.y_isdel = oneYearStu.y_isdel;
                    //            stuFee.y_loginName = oneYearStu.y_loginName;
                    //            stuFee.y_mail = oneYearStu.y_mail;
                    //            stuFee.y_majorId = oneYearStu.y_majorId;
                    //            stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //            stuFee.y_name = oneYearStu.y_name;
                    //            stuFee.y_nationId = oneYearStu.y_nationId;

                    //            stuFee.y_password = oneYearStu.y_password;
                    //            stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //            stuFee.y_registerState = oneYearStu.y_registerState;
                    //            stuFee.y_sex = oneYearStu.y_sex;
                    //            stuFee.y_stuId = oneYearStu.id;
                    //            stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //            stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //            stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //            stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //            stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //            stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //            //如果是中医药大学则改变学制
                    //            if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //                ComEnum.SchoolName.ZYYDX.ToString())
                    //            {
                    //                if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //                {
                    //                    stuFee.y_stuYear = 4;
                    //                }
                    //                if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //                {
                    //                    stuFee.y_stuYear = 5;
                    //                }
                    //                if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //                {
                    //                    stuFee.y_stuYear = 3;
                    //                }
                    //                //如果专业为医药营销则学制为3
                    //                var marsql = "y_name like '医药营销 %'";
                    //                var marlist =
                    //                    ad.Database.SqlQuery<YD_Edu_Major>(
                    //                        "select * from YD_Edu_Major where " + marsql)
                    //                        .ToList()
                    //                        .Select(u => u.y_name);
                    //                if (marlist.Contains(stuFee.majorName))
                    //                {
                    //                    stuFee.y_stuYear = 3;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //            }

                    //            stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //            stuFee.y_tel = oneYearStu.y_tel;
                    //            stuFee.y_upFee = 0;
                    //            stuFee.y_needFee = needFee;

                    //            //新添加的字段
                    //            stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //            stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //            stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //            stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //            stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //            stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //            stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //            stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //            stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //            stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //            stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //            stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //            stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //            stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //            stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //            stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //            stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //            stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //            stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //            #region 根据分配比例计算需上缴的费用

                    //            if (stuFee.y_needFee != 0)
                    //            {
                    //                var subBili =
                    //                    ad.YD_Fee_SubFeeBili.FirstOrDefault(
                    //                        u =>
                    //                            u.y_eduTypeId == stuFee.y_eduTypeId &&
                    //                            u.y_subSchoolId == stuFee.y_subSchoolId);
                    //                if (subBili == null)
                    //                {
                    //                    var bili =
                    //                        ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                    if (bili == null)
                    //                    {
                    //                        stuFee.y_needUpFee = stuFee.y_needFee;
                    //                    }
                    //                    else
                    //                    {
                    //                        stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = 0;
                    //            }

                    //            #endregion

                    //            stuFeeTbsFour.Add(stuFee);
                    //        }
                    //    }

                    //    #endregion
                    //}
                    //if (majorfourYearStus.Count > 0)
                    //{
                    //    #region 收集4年制专业需上缴第四个学年的学生

                    //    foreach (var oneYearStu in fourYearStus2)
                    //    {
                    //        //存在记录不用添加
                    //        if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 4))
                    //        {
                    //            int needFee = 0;
                    //            //先查询是否存在函授站特定的收费标准
                    //            var subFee = ad.YD_Fee_SubFeeSys.FirstOrDefault(u => u.y_subSchoolId == oneYearStu.y_subSchoolId && u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (subFee != null)
                    //            {
                    //                needFee = subFee.y_StuFee;
                    //            }
                    //            else
                    //            {
                    //                var fee =
                    //                    ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //                if (fee != null)
                    //                {
                    //                    needFee = fee.y_bookFee + fee.y_stuFee;
                    //                }
                    //            }

                    //            var stuFee = new YD_Fee_StuFeeTb();
                    //            stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //            stuFee.majorCode = oneYearStu.majorCode;
                    //            stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //            stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //            stuFee.majorName = oneYearStu.majorName;
                    //            stuFee.nationName = oneYearStu.nationName;
                    //            stuFee.politicsName = oneYearStu.politicsName;
                    //            stuFee.schoolCode = oneYearStu.schoolCode;
                    //            stuFee.schoolName = oneYearStu.schoolName;
                    //            stuFee.stuStateName = oneYearStu.stuStateName;
                    //            stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //            stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //            stuFee.y_address = oneYearStu.y_address;
                    //            stuFee.y_birthday = oneYearStu.y_birthday;
                    //            stuFee.y_cardId = oneYearStu.y_cardId;
                    //            stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //            stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //            stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //            stuFee.y_examNum = oneYearStu.y_examNum;
                    //            stuFee.y_feeYear = 4;
                    //            stuFee.y_isUp = (int)YesOrNo.No;
                    //            stuFee.y_img = oneYearStu.y_img;
                    //            stuFee.y_inYear = oneYearStu.y_inYear;
                    //            stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //            stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //            stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //            stuFee.y_isdel = oneYearStu.y_isdel;
                    //            stuFee.y_loginName = oneYearStu.y_loginName;
                    //            stuFee.y_mail = oneYearStu.y_mail;
                    //            stuFee.y_majorId = oneYearStu.y_majorId;
                    //            stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //            stuFee.y_name = oneYearStu.y_name;
                    //            stuFee.y_nationId = oneYearStu.y_nationId;

                    //            stuFee.y_password = oneYearStu.y_password;
                    //            stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //            stuFee.y_registerState = oneYearStu.y_registerState;
                    //            stuFee.y_sex = oneYearStu.y_sex;
                    //            stuFee.y_stuId = oneYearStu.id;
                    //            stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //            stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //            stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //            stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //            stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //            stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //            //如果是中医药大学则改变学制
                    //            if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //                ComEnum.SchoolName.ZYYDX.ToString())
                    //            {
                    //                if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //                {
                    //                    stuFee.y_stuYear = 4;
                    //                }
                    //                if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //                {
                    //                    stuFee.y_stuYear = 5;
                    //                }
                    //                if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //                {
                    //                    stuFee.y_stuYear = 3;
                    //                }
                    //                //如果专业为医药营销则学制为3
                    //                var marsql = "y_name like '医药营销 %'";
                    //                var marlist =
                    //                    ad.Database.SqlQuery<YD_Edu_Major>(
                    //                        "select * from YD_Edu_Major where " + marsql)
                    //                        .ToList()
                    //                        .Select(u => u.y_name);
                    //                if (marlist.Contains(stuFee.majorName))
                    //                {
                    //                    stuFee.y_stuYear = 3;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //            }

                    //            stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //            stuFee.y_tel = oneYearStu.y_tel;
                    //            stuFee.y_upFee = 0;
                    //            stuFee.y_needFee = needFee;

                    //            //新添加的字段
                    //            stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //            stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //            stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //            stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //            stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //            stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //            stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //            stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //            stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //            stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //            stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //            stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //            stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //            stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //            stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //            stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //            stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //            stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //            stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //            #region 根据分配比例计算需上缴的费用

                    //            if (stuFee.y_needFee != 0)
                    //            {
                    //                var subBili = ad.YD_Fee_SubFeeBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId && u.y_subSchoolId == stuFee.y_subSchoolId);
                    //                if (subBili == null)
                    //                {
                    //                    var bili =
                    //                        ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                    if (bili == null)
                    //                    {
                    //                        stuFee.y_needUpFee = stuFee.y_needFee;
                    //                    }
                    //                    else
                    //                    {
                    //                        stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = 0;
                    //            }

                    //            #endregion

                    //            stuFeeTbsFour.Add(stuFee);
                    //        }
                    //    }

                    //    #endregion
                    //}
                    //#region 收集指定年份需上缴第五个学年的学生

                    //foreach (var oneYearStu in fiveYearStus)
                    //{
                    //    //存在记录不用添加
                    //    if (!ad.YD_Fee_StuFeeTb.Any(u => u.y_stuId == oneYearStu.id && u.y_feeYear == 5))
                    //    {
                    //        int needFee = 0;
                    //        //先查询是否存在函授站特定的收费标准
                    //        var subFee =
                    //            ad.YD_Fee_SubFeeSys.FirstOrDefault(
                    //                u =>
                    //                    u.y_subSchoolId == oneYearStu.y_subSchoolId &&
                    //                    u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //        if (subFee != null)
                    //        {
                    //            needFee = subFee.y_StuFee;
                    //        }
                    //        else
                    //        {
                    //            var fee =
                    //                ad.YD_Fee_AllFeeSys.FirstOrDefault(u => u.y_eduTypeId == oneYearStu.y_eduTypeId);
                    //            if (fee != null)
                    //            {
                    //                needFee = fee.y_bookFee + fee.y_stuFee;
                    //            }
                    //        }

                    //        var stuFee = new YD_Fee_StuFeeTb();
                    //        stuFee.eduTypeName = oneYearStu.eduTypeName;
                    //        stuFee.majorCode = oneYearStu.majorCode;
                    //        stuFee.majorLibraryCode = oneYearStu.majorLibraryCode;
                    //        stuFee.majorLibraryName = oneYearStu.majorLibraryName;
                    //        stuFee.majorName = oneYearStu.majorName;
                    //        stuFee.nationName = oneYearStu.nationName;
                    //        stuFee.politicsName = oneYearStu.politicsName;
                    //        stuFee.schoolCode = oneYearStu.schoolCode;
                    //        stuFee.schoolName = oneYearStu.schoolName;
                    //        stuFee.stuStateName = oneYearStu.stuStateName;
                    //        stuFee.stuTypeName = oneYearStu.stuTypeName;
                    //        stuFee.subSchoolCode = oneYearStu.subSchoolCode;
                    //        stuFee.y_address = oneYearStu.y_address;
                    //        stuFee.y_birthday = oneYearStu.y_birthday;
                    //        stuFee.y_cardId = oneYearStu.y_cardId;
                    //        stuFee.y_changePlanId = oneYearStu.y_changePlanId;
                    //        stuFee.y_eduTypeCode = oneYearStu.y_eduTypeCode;
                    //        stuFee.y_eduTypeId = Convert.ToInt32(oneYearStu.y_eduTypeId);
                    //        stuFee.y_examNum = oneYearStu.y_examNum;
                    //        stuFee.y_feeYear = 5;
                    //        stuFee.y_img = oneYearStu.y_img;
                    //        stuFee.y_inYear = oneYearStu.y_inYear;
                    //        stuFee.y_isChangePlan = oneYearStu.y_isChangePlan;
                    //        stuFee.y_isCheckFee = (int)YesOrNo.No;
                    //        stuFee.y_isMoneyOk = oneYearStu.y_isMoneyOk;
                    //        stuFee.y_isdel = oneYearStu.y_isdel;
                    //        stuFee.y_loginName = oneYearStu.y_loginName;
                    //        stuFee.y_mail = oneYearStu.y_mail;
                    //        stuFee.y_majorId = oneYearStu.y_majorId;
                    //        stuFee.y_majorLibId = Convert.ToInt32(oneYearStu.y_majorLibId);
                    //        stuFee.y_name = oneYearStu.y_name;
                    //        stuFee.y_nationId = oneYearStu.y_nationId;
                    //        stuFee.y_isUp = (int)YesOrNo.No;
                    //        stuFee.y_password = oneYearStu.y_password;
                    //        stuFee.y_politicsId = oneYearStu.y_politicsId;

                    //        stuFee.y_registerState = oneYearStu.y_registerState;
                    //        stuFee.y_sex = oneYearStu.y_sex;
                    //        stuFee.y_stuId = oneYearStu.id;
                    //        stuFee.y_stuNum = oneYearStu.y_stuNum;
                    //        stuFee.y_stuStateCode = oneYearStu.y_stuStateCode;
                    //        stuFee.y_stuStateId = oneYearStu.y_stuStateId;
                    //        stuFee.y_stuStrange = oneYearStu.y_stuStrange;
                    //        stuFee.y_stuTypeCode = oneYearStu.y_stuTypeCode;

                    //        stuFee.y_stuTypeId = Convert.ToInt32(oneYearStu.y_stuTypeId);
                    //        //如果是中医药大学则改变学制
                    //        if (ConfigurationManager.AppSettings["SchoolName"].ToString() ==
                    //            ComEnum.SchoolName.ZYYDX.ToString())
                    //        {
                    //            if (stuFee.y_eduTypeId == 1) //如果层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果层次是专升本则学制为3
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //            //如果专业为医药营销则学制为3
                    //            var marsql = "y_name like '医药营销 %'";
                    //            var marlist =
                    //                ad.Database.SqlQuery<YD_Edu_Major>("select * from YD_Edu_Major where " + marsql)
                    //                    .ToList()
                    //                    .Select(u => u.y_name);
                    //            if (marlist.Contains(stuFee.majorName))
                    //            {
                    //                stuFee.y_stuYear = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_stuYear = oneYearStu.y_stuYear;
                    //        }

                    //        stuFee.y_subSchoolId = oneYearStu.y_subSchoolId;
                    //        stuFee.y_tel = oneYearStu.y_tel;
                    //        stuFee.y_upFee = 0;
                    //        stuFee.y_needFee = needFee;

                    //        //新添加的字段
                    //        stuFee.y_graduationdata = oneYearStu.y_graduationdata;
                    //        stuFee.y_graduationschool = oneYearStu.y_graduationschool;
                    //        stuFee.y_registrationNum = oneYearStu.y_registrationNum;
                    //        stuFee.y_postalcode = oneYearStu.y_postalcode;
                    //        stuFee.y_foreignLanguageId = oneYearStu.y_foreignLanguageId;
                    //        stuFee.y_recruitTypeId = oneYearStu.y_recruitTypeId;
                    //        stuFee.y_professionTypeId = oneYearStu.y_professionTypeId;
                    //        stuFee.y_cultureExtentId = oneYearStu.y_cultureExtentId;
                    //        stuFee.y_examFeatureId = oneYearStu.y_examFeatureId;
                    //        stuFee.y_foreignLanguageCode = oneYearStu.y_foreignLanguageCode;
                    //        stuFee.y_foreignLanguageName = oneYearStu.y_foreignLanguageName;
                    //        stuFee.y_cultureExtentCode = oneYearStu.y_cultureExtentCode;
                    //        stuFee.y_cultureExtentName = oneYearStu.y_cultureExtentName;
                    //        stuFee.y_professionTypeCode = oneYearStu.y_professionTypeCode;
                    //        stuFee.y_professionTypeName = oneYearStu.y_professionTypeName;
                    //        stuFee.y_recruitTypeCode = oneYearStu.y_recruitTypeCode;
                    //        stuFee.y_recruitTypeName = oneYearStu.y_recruitTypeName;
                    //        stuFee.y_examFeatureCode = oneYearStu.y_examFeatureCode;
                    //        stuFee.y_examFeatureName = oneYearStu.y_examFeatureName;

                    //        #region 根据分配比例计算需上缴的费用
                    //        if (stuFee.y_needFee != 0)
                    //        {
                    //            var subBili = ad.YD_Fee_SubFeeBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId && u.y_subSchoolId == stuFee.y_subSchoolId);
                    //            if (subBili == null)
                    //            {
                    //                var bili = ad.YD_Fee_AllBili.FirstOrDefault(u => u.y_eduTypeId == stuFee.y_eduTypeId);
                    //                if (bili == null)
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee;
                    //                }
                    //                else
                    //                {
                    //                    stuFee.y_needUpFee = stuFee.y_needFee * bili.y_bili / 100;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                stuFee.y_needUpFee = stuFee.y_needFee * subBili.y_bili / 100;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            stuFee.y_needUpFee = 0;
                    //        }

                    //        #endregion

                    //        stuFeeTbsFive.Add(stuFee);
                    //    }
                    //}
                    //#endregion
                    //ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsTwo);
                    //ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsThree);
                    //ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFour);
                    //if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                    //{
                    //    ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFour2);
                    //}
                    //if (majorfourYearStus.Count > 0) //如果有4年制专业
                    //{
                    //    ad.Set<YD_Fee_StuFeeTb>().AddRange(majorstuFeeTbsFour);
                    //}
                    //ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsFive);
                    //#endregion          
                    //ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsOne);
                    //ad.SaveChanges();
                    #endregion                  
                    for (var i = 0; i < list.Count; i++)
                    {
                        var cardId = list[i].y_cardId;
                        var substuschool = substulist.FirstOrDefault(u => u.y_cardId == cardId);
                        if (substuschool != null)
                        {
                            substuschool.y_hide = 0;
                            ad.Entry(substuschool).State = EntityState.Modified;
                        }
                    }
                    ad.SaveChanges();
                }
            }
        }



        /********************************************************************
        * 作者：胡惠贞   2017-4-5=18 14:12
        * 用途：成绩导入模板验证
        ********************************************************************/

        /// <param name="errorCount">错误数量</param>
        /// <param name="sheet">EXCEL表格</param>
        /// <param name="errorStyle">错误后的EXCEL样式</param>
        public static List<YD_Edu_ScoreTemp> ScoreTempletValidate(ref int errorCount, ISheet sheet,
           ICellStyle errorStyle)
        {
            var course = new Dictionary<int, string>();
            using (var ad = new IYunEntities())
            {
                ad.Database.ExecuteSqlCommand("DELETE FROM YD_Edu_ScoreTemp");

                ad.YD_Edu_Course.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      course.Add(u.id, u.y_name);
                  });

                var stuNumlist = ad.YD_Sts_StuInfo.Select(u => new { u.id, u.y_stuNum }).ToList();

                var list = new List<YD_Edu_ScoreTemp>(); //数据转化List
                const double eps = 1e-10; // 精度范围            
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Edu_ScoreTemp();
                    for (int j = 0; j < 6; j++)
                    {
                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0:  //学号
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = errorStyle;
                                    cell.SetCellValue("不能为空");
                                    errorCount++;
                                }
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
                                    else
                                    {
                                        data.y_stuNum = cell.StringCellValue.Trim();
                                        var stu = stuNumlist.FirstOrDefault(u => u.y_stuNum == data.y_stuNum);

                                        if (stu == null)
                                        {
                                            cell.CellStyle = errorStyle;
                                            errorCount++;
                                        }
                                        else if (errorCount == 0)
                                        {
                                            data.y_stuId = stu.id;
                                        }
                                    }

                                }
                                break;
                            case 1:  //学期
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (cell.NumericCellValue <= 0 || cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //传说中的判断double是否整数，效果未测试
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }

                                    if (errorCount == 0)
                                        data.y_term = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 2: //课程
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
                                    //以课程id方式导入进来
                                    //var cou = Convert.ToInt32(cell.StringCellValue.Trim());
                                    //if (!course.Keys.Contains(cou))
                                    //{
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    //if (errorCount == 0)
                                    //    data.y_courseId = Convert.ToInt32(cell.StringCellValue.Trim());

                                    //}

                                    //以课程名字的方式导入进来
                                    var name = cell.StringCellValue.Trim();
                                   
                                    var course2 = ad.YD_Edu_Course.FirstOrDefault(u => u.y_name == name);

                                    if (course2 == null)
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)

                                        data.y_courseId = course2.id;
                                }


                                break;
                            case 3: //平时成绩
                                if (cell.CellType == CellType.BLANK || cell == null)
                                {
                                    data.y_normalScore = 0;
                                }
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    //if (cell.NumericCellValue < 0 ||
                                    //    cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    ////传说中的判断double是否整数，效果未测试
                                    //{
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    if (errorCount == 0)
                                        data.y_normalScore = Convert.ToDecimal(cell.StringCellValue);
                                }
                                break;
                            case 4: //期末成绩
                                if (cell.CellType == CellType.BLANK || cell == null)
                                {
                                    data.y_termScore = 0;
                                }
                                else
                                {
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_termScore = Convert.ToDecimal(cell.StringCellValue);
                                }
                                break;
                            case 5: //总评成绩
                                if (cell.CellType == CellType.BLANK || cell == null)
                                {
                                    data.y_totalScore = 0;
                                }
                                if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                else
                                {
                                    if (errorCount == 0)
                                        data.y_totalScore = Convert.ToDecimal(cell.StringCellValue);
                                }
                                break;
                        }
                    }
                    if (errorCount == 0)
                        list.Add(data); //把数据加载到List里面
                }
                if (errorCount == 0)
                {
                    //ad.Configuration.AutoDetectChangesEnabled = false;
                    //ad.Configuration.ValidateOnSaveEnabled = false;
                    //ad.Set<YD_Edu_ScoreTemp>().AddRange(list);
                    //ad.SaveChanges();
                }
                return list;
            }
        }

        /********************************************************************
       * 作者：胡惠贞   2017-4-26 16:35
       * 用途：处理Excel添加到成绩表里
       ********************************************************************/

        /// <summary>
        /// 处理Excel添加到成绩表里
        /// </summary>
        /// <param name="list">list</param>
        public static void UploadTrueScore(List<YD_Edu_ScoreTemp> list)
        {
            using (var ad = new IYunEntities())
            {

                //获取考生信息
                var scorelist = new List<YD_Edu_Score>();
                foreach (var score in list)
                {
                    var scores = new YD_Edu_Score
                    {
                        y_stuId = (int)score.y_stuId,
                        y_courseId = (int)score.y_courseId,
                        y_term = (int)score.y_term,
                        y_normalScore = (decimal)score.y_normalScore,
                        y_termScore = (decimal)score.y_termScore,
                        y_workScore = 0,
                        y_totalScore = (decimal)score.y_totalScore
                    };
                    scorelist.Add(scores);
                }
                ad.Configuration.AutoDetectChangesEnabled = false;
                ad.Configuration.ValidateOnSaveEnabled = false;
                ad.Set<YD_Edu_Score>().AddRange(scorelist);
                ad.SaveChanges();
            }
        }



        /********************************************************************
       * 作者：胡惠贞   2017-5-13 15:21
       * 用途：学位外语成绩导入模板验证
       ********************************************************************/


        /// <param name="index">错误数量</param>
        /// <param name="sheet">EXCEL表格</param>
        /// <param name="styleCell">错误后的EXCEL样式</param>
        public static List<YD_Graduate_EnglishScoreTemp> StuGridEnglishScoreTemplet(ref int index, ISheet sheet,
            ICellStyle styleCell)
        {
            using (var ad = new IYunEntities())
            {
                ad.Database.ExecuteSqlCommand("DELETE FROM YD_Graduate_EnglishScoreTemp");
                var majorlib = new Dictionary<string, int>();
                var list = new List<YD_Graduate_EnglishScoreTemp>(); //数据转化List
                const double eps = 1e-10; // 精度范围            
                ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
               {
                   majorlib.Add(u.y_name, u.id);
               });
                var cardlist = ad.YD_Sts_StuInfo.GroupBy(u => u.y_cardId).Select(u => u.Key).ToList(); //所有学生身份证号 
                var stulist = ad.VW_StuInfo.GroupBy(u => new { u.id, u.y_inYear, u.y_name, u.y_cardId,
                    u.y_majorLibId,u.y_subSchoolId }).Select(u => u.Key).ToList(); //得到所有学生

                    sheet.GetRow(0).CreateCell(11).SetCellValue("失败原因");

                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Graduate_EnglishScoreTemp();
                    for (int j = 0; j < 15; j++)
                    {
                        var cell = row.GetCell(j);
                        switch (j)
                        {
                            case 0: //通过时间
                                data.y_adopttime = cell.StringCellValue.Trim();
                                if (cell == null)
                                {
                                    data.y_adopttime = null;
                                    //cell.CellStyle = styleCell;
                                    //cell.SetCellValue("不能为空");
                                    //index++;
                                }
                                //else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                //{
                                //    cell.CellStyle = styleCell;
                                //    index++;
                                //}
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        data.y_adopttime = null;
                                        //cell.SetCellValue("Can't be null");
                                        //cell.CellStyle = styleCell;
                                        //index++;
                                    }
                                    //else if (!sex.Keys.Contains(cell.StringCellValue.Trim()))
                                    //{
                                    //    cell.CellStyle = styleCell;
                                    //    index++;
                                    //}
                                    if (index == 0)
                                        data.y_adopttime = cell.StringCellValue.Trim();
                                }
                                break;

                            case 1: //入学年份
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                {
                                    cell.CellStyle = styleCell;
                                    index++;
                                }
                                else
                                {
                                    //if (cell.NumericCellValue <= 0 ||
                                    //    cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    ////传说中的判断double是否整数，效果未测试
                                    //{
                                    //    cell.CellStyle = styleCell;
                                    //    index++;
                                    //}
                                    if (index == 0)
                                        data.y_inYear = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;

                            case 2:  //姓名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
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
                                        data.y_stuname = cell.StringCellValue.Trim();
                                }

                                break;

                            case 3:  //性别
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
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
                                    {
                                        var aa= cell.StringCellValue.Trim();
                                        if (aa=="男")
                                        {
                                            data.y_sex = 0;
                                        }
                                        else
                                        {
                                            data.y_sex = 1;
                                        }                                     
                                    }
                                     
                                }
                                break;
                        
                            case 4: //身份证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(4);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!cardlist.Contains(cell.StringCellValue))
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
 
                                    if (index == 0)
                                    {
                                        //验证学生信息是否正确
                                        var stu = stulist.FirstOrDefault(u => u.y_name == data.y_stuname
                                        && u.y_cardId == cell.StringCellValue.Trim());
                                        if (stu != null)
                                        {
                                                data.y_cardId = cell.StringCellValue.Trim();
                                                data.y_stuId = stu.id;
                                                }
                                        else
                                        {
                                            row.CreateCell(11).SetCellValue("学生信息不匹配");
                                            index++;
                                        }
                                    }
                                }
                                break;





                            case 5: //专业名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(2);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    else if (!majorlib.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    if (index == 0)
                                    {
                                      
                                    

                                    }


                                }
                                break;


                            case 6: //总分
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(5);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else
                                {
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    if (index == 0)
                                        data.y_sumsore = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;
                            case 7: //主观分
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(6);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else
                                {
                                    if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    if (index == 0)
                                        data.y_subjectivitysore = Convert.ToInt32(cell.NumericCellValue);
                                }
                                break;

                            case 8: //准考证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(7);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
                                else if (cell.CellType != CellType.STRING) //如果不是string 类型
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
                                    //else if (admissionNum.Contains(cell.StringCellValue)|| admissionNumlist.Contains(cell.StringCellValue))
                                    //{
                                    //    cell.CellStyle = styleCell;
                                    //    index++;
                                    //}
                                    if (index == 0)
                                    {
                                        data.y_admissionNum = cell.StringCellValue.Trim();
                                    }
                                }
                                break;

                            case 9:  //是否通过
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
                                    cell.CellStyle = styleCell;
                                    cell.SetCellValue("不能为空");
                                    index++;
                                }
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
                                    {
                                        var aa = cell.StringCellValue.Trim();
                                        if (aa == "通过")
                                        {
                                            data.y_verdict = 1;
                                        }
                                        else
                                        {
                                            data.y_verdict = 0;
                                        }
                                    }

                                }
                                break;



                                //case 10: //所属站点
                                //    data.y_subschoolname = cell.StringCellValue.Trim();

                                //    break;

                        }
                    }
                    if (index == 0)
                        list.Add(data); //把数据加载到List里面
                }
                return list;
            }
        }

        /********************************************************************
       * 作者：胡惠贞   2017-5-13 16:04
       * 用途：处理Excel添加到学位外语成绩表里
       ********************************************************************/

        /// <summary>
        /// 处理Excel添加到学位外语成绩表里
        /// </summary>
        /// <param name="list">list</param>
        public static void UploadTrueGridEnglishScore(List<YD_Graduate_EnglishScoreTemp> list)
        {
            using (var ad = new IYunEntities())
            {
                var gradscorelist = new List<YD_Graduate_StudentScore>();
                foreach (var gradscores in list)
                {
                    var gradscore = new YD_Graduate_StudentScore
                    {
                        y_stuId = gradscores.y_stuId,
                        y_examtime = gradscores.y_examtime,
                        y_admissionNum = gradscores.y_admissionNum,
                        y_stuname = gradscores.y_stuname,
                        y_namePinyin = gradscores.y_namePinyin,
                        y_subschoolname = gradscores.y_subschoolname,
                        y_sex = gradscores.y_sex,
                        y_verdict = Convert.ToInt32(gradscores.y_verdict),
                        y_inYear = Convert.ToInt32(gradscores.y_inYear),
                        y_cardId = gradscores.y_cardId,
                        y_subjectivitysore = Convert.ToInt32(gradscores.y_subjectivitysore),
                        y_sumsore = Convert.ToInt32(gradscores.y_sumsore),
                        y_explain = gradscores.y_explain,
                        y_remark = gradscores.y_remark,
                        y_adopttime=gradscores.y_adopttime,
                        y_isdel = (int)YesOrNo.No,
                        y_createtime = DateTime.Now
                       

                };
                    gradscorelist.Add(gradscore);
                }
                ad.Configuration.AutoDetectChangesEnabled = false;
                ad.Configuration.ValidateOnSaveEnabled = false;
                ad.Set<YD_Graduate_StudentScore>().AddRange(gradscorelist);
                ad.SaveChanges();
            }
        }

        /********************************************************************
        * 作者：廖坤   2017-2-23 09:34
        * 用途：处理Excel上传后的专业库,没有则添加，有则获取ID,填充到数据list中
        * 最新修改人：廖坤      
        * 最新修改时间：2017-2-24 17:07
        ********************************************************************/

        /// <summary>
        /// 处理Excel上传后的专业库,没有则添加，有则获取ID
        /// </summary>
        /// <param name="list">list</param>
        public static void MajorLibHandle(List<TeaPlanExcelInsertDto> list)
        {
            using (var ad = new IYunEntities())
            {
                var majorLibDb = ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name,u.y_code }).ToList();

                
                int maxLibId =majorLibDb.Any() ? majorLibDb.Max(u => Convert.ToInt32(u.y_code)): 0; //得到最大code

                var lack_MajorLib = new Dictionary<string,string>();

                var majorLiblist = list.GroupBy(u => u.MajorName).ToList();

                foreach (IGrouping<string, TeaPlanExcelInsertDto> item in majorLiblist)
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
                        item => new YD_Edu_MajorLibrary() { y_name = item.Key, y_code = item.Value}).ToList();

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


        /********************************************************************
        * 作者：廖坤   2017-2-23 09:34
        * 用途：处理Excel上传后的课程,没有则添加，有则获取ID,填充到数据list中
        * 最新修改人：廖坤      
        * 最新修改时间：2017-2-24 17:07
        ********************************************************************/

        /// <summary>
        /// 处理Excel上传后的课程,没有则添加，有则获取ID
        /// </summary>
        /// <param name="list">list</param>
        public static void CourseHandle(List<TeaPlanExcelInsertDto> list)
        {
            using (var ad = new IYunEntities())
            {
                var courseDb = ad.YD_Edu_Course.Select(u => new { u.id, u.y_name }).ToList();

                int maxcourseId = courseDb.Any() ? courseDb.Max(u => u.id) : 0;

                var lack_course = new List<string>();

                var courselist = list.GroupBy(u => u.CourseName).ToList();

                foreach (IGrouping<string, TeaPlanExcelInsertDto> item in courselist)
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


        /********************************************************************
        * 作者：廖坤   2017-2-23 09:34
        * 用途：处理Excel上传后的专业,没有则添加，有则获取ID,填充到数据list中,
        * 然后根据专业生成教学计划模板主表
        * 最新修改人：廖坤      
        * 最新修改时间：2017-2-24 17:07
        ********************************************************************/

        /// <summary>
        /// 处理Excel上传后的专业,没有则添加，有则获取ID,生成教学计划模板主表
        /// </summary>
        /// <param name="list">list</param>
        public static void MajorHandle(List<TeaPlanExcelInsertDto> list)
        {
            using (var ad = new IYunEntities())
            {
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
                        item.ToList().ForEach(u => u.MajorId = major.id);
                    }
                }

                var newmajorlist = new List<YD_Edu_Major>();

                using(var db = new IYunEntities())//将信息存到Major表
                {
                    var stuTypeList = db.YD_Edu_StuType.ToList();//全部在循环外取出
                    var eduTypeList = db.YD_Edu_EduType.ToList();
                    var majorList = db.YD_Edu_MajorLibrary.ToList();
                    foreach (var item in lack_major)
                    {
                        maxmajorId++;
                        string majorName = majorList.SingleOrDefault(e => e.id == item.y_majorLibId).y_name;
                        string eduType = eduTypeList.SingleOrDefault(e => e.id == item.y_eduTypeId).y_name;
                        string stuType = stuTypeList.SingleOrDefault(e => e.id == item.y_stuTypeId).y_name;
                        int needFer =0;
                        int stuYear =0;
                        if (eduType.Trim() == "高起本")
                        {
                            needFer = 1700;
                            stuYear = 5;
                        }
                        else
                        {
                            needFer = 1300;
                            stuYear = 3;
                        }
                        var newmajor = new YD_Edu_Major()
                        {
                            y_majorLibId = item.y_majorLibId,
                            y_stuTypeId = item.y_stuTypeId,
                            y_eduTypeId = item.y_eduTypeId,
                            y_name = $"{majorName} {eduType} {stuType}",
                            y_code = maxmajorId.ToString(),
                            y_needFee = needFer,
                            y_stuYear = stuYear
                        };

                        newmajorlist.Add(newmajor);

                    }
                }
                //因为出现一次NAME为空的情况下暂时注释掉了下面的代码，改为使用上面的
                //foreach (var item in lack_major)
                //{
                //    maxmajorId++;
                //    var newmajor = new YD_Edu_Major()
                //    {
                //        y_majorLibId = item.y_majorLibId,
                //        y_stuTypeId = item.y_stuTypeId,
                //        y_eduTypeId = item.y_eduTypeId,
                //        y_name = "",
                //        y_code = maxmajorId.ToString(),
                //        y_stuYear = 3
                //    };

                //    newmajorlist.Add(newmajor);

                //}

                ad.YD_Edu_Major.AddRange(newmajorlist);

                ad.SaveChanges();

                string sql;

                //如果是中医药大学则改变学制
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    sql =
                        "update YD_Edu_Major set y_code = b.id ,y_name = b.name,y_stuYear = b.stuyear,y_needFee=b.needFee FROM YD_Edu_Major, " +
                        "(select m.id,l.y_name+' '+e.y_name+' '+s.y_name as name, (case when e.y_name = '高起本' and m.y_name != '医药营销' " +
                        "then 5 when e.y_name = '高起专' and m.y_name != '医药营销' then 4 else 3 end) " +
                        " as stuyear,(case when e.y_name = '高起本' then 1700 else 1400 end)"+
                        " as needFee from YD_Edu_Major as m join YD_Edu_MajorLibrary " +
                        "as l on l.id = m.y_majorLibId join YD_Edu_EduType " +
                        "as e on e.id = m.y_eduTypeId join YD_Edu_StuType as s " +
                        "on s.id = m.y_stuTypeId ) as b WHERE YD_Edu_Major.ID = B.ID and YD_Edu_Major.id > " +
                        maxmajorId; //todo:此SQL未测试
                }
                else
                {//根据层次更新学制
                    sql =
                        "update YD_Edu_Major set y_code = b.id ,y_name = b.name,y_stuYear = b.stuyear,y_needFee=b.needFee  FROM YD_Edu_Major, " +
                        "(select m.id,l.y_name+' '+e.y_name+' '+s.y_name as name, (case when e.y_name = '高起本' then 5 else 3 end)" +
                        " as stuyear,(case when e.y_name = '高起本' then 1700 else 1400 end)"+ 
                        " as needFee from YD_Edu_Major as m join YD_Edu_MajorLibrary as l on l.id = m.y_majorLibId join YD_Edu_EduType " +
                        "as e on e.id = m.y_eduTypeId join YD_Edu_StuType as s on s.id = m.y_stuTypeId ) as b WHERE YD_Edu_Major.ID = B.ID and YD_Edu_Major.id > " +
                        maxmajorId;
                }

                ad.Database.ExecuteSqlCommand(sql);

                ad.SaveChanges();

                //获取新专业的专业ID
                foreach (var item in newmajorlist)
                {
                    majorlist.First(
                        u =>
                            u.Key.EduType == item.y_eduTypeId && u.Key.MajorLibId == item.y_majorLibId &&
                            u.Key.StuType == item.y_stuTypeId).ToList().ForEach(u =>
                            {
                                u.MajorId = item.id;
                            });
                }

                /********************************************************************
                * 到此为止 专业获取完毕
                *
                * 此处开始 生成教学计划模板主表
                ********************************************************************/

                var newTeaplanTempletlist =
                    list.GroupBy(u => u.MajorId).Select(
                        u =>
                            new YD_TeaPlan_Templet
                            {
                                y_majorId = u.Key,
                                y_name = "",
                                y_remark = "",
                                y_teaPlanType = 1
                            }).ToList();

                //newTeaplanTempletlist.ForEach(u =>
                //{
                //    var tmp = ad.YD_TeaPlan_Templet.Where(m => m.y_majorId == u.y_majorId).FirstOrDefault();
                //    if (tmp != null)
                //    {
                //        ad.Entry(tmp.YD_TeaPlan_TempletCourseDes).State = EntityState.Deleted;
                //        ad.Entry(tmp).State = EntityState.Deleted;
                //    }
                //});

                var addTeaplan = new List<YD_TeaPlan_Templet>();

                foreach (var plan in newTeaplanTempletlist) {
                    var tmp = ad.YD_TeaPlan_Templet.Where(u => u.y_majorId == plan.y_majorId).FirstOrDefault();

                    if (tmp != null)
                    {
                        //var detail = ad.YD_TeaPlan_TempletCourseDes.Where(u => u.y_templetId == tmp.id).ToList();
                        //foreach (var des in detail)
                        //{
                        //    ad.Entry(des).State = EntityState.Deleted;
                        //}
                        ad.Database.ExecuteSqlCommand("delete from YD_TeaPlan_TempletCourseDes where y_templetId="+tmp.id);
                        ad.SaveChanges();
                        plan.id = tmp.id;
                    }
                    else {
                        addTeaplan.Add(plan);

                    }

                }

              

                //todo:增加教学计划名称

                ad.YD_TeaPlan_Templet.AddRange(addTeaplan);

                ad.SaveChanges();

                foreach (var item in newTeaplanTempletlist)
                {
                    list.Where(u => u.MajorId == item.y_majorId).ToList().ForEach(u =>
                    {
                        u.TempletId = item.id;
                    });
                }

                /********************************************************************
                * 到此为止 教学计划模板主表生成完毕
                *
                * 此处开始 生成教学计划模板从表
                ********************************************************************/

                var templetDesList = list.Select(u => new YD_TeaPlan_TempletCourseDes()
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
                    y_templetId = u.TempletId
                    //todo:如果有增加字段需要再次修改
                });

                ad.YD_TeaPlan_TempletCourseDes.AddRange(templetDesList);

                ad.SaveChanges();
            }
        }



        ///处理学生成绩表
        public static List<StuScoreDto> StuScoreExcelHandle(ISheet sheet)
        {
            using (var db = new IYunEntities())
            {
                var Scale = Convert.ToDecimal(db.YD_Edu_ScoreScale.SingleOrDefault().y_normalScale) / 100;//取得数据库中的分数比例
                int cout = sheet.LastRowNum;
                int cout2 = sheet.PhysicalNumberOfRows;
                 IRow row;
                List<StuScoreDto> list = new List<StuScoreDto>();
                var StuScoreEntity = db.YD_Edu_Score;
                for (int i = 1; i < cout2; i++)
                {
                    row = sheet.GetRow(i);
                    try
                    {
                        decimal avgNum = Convert.ToDecimal(row.GetCell(7).NumericCellValue);
                        decimal endNum = Convert.ToDecimal(row.GetCell(8).NumericCellValue);
                        var stuId = Convert.ToInt32(row.GetCell(0).StringCellValue);
                        var courseId = Convert.ToInt32(row.GetCell(4).NumericCellValue);
                        var term = Convert.ToInt32(row.GetCell(6).NumericCellValue);
                        //bool exist =  StuScoreEntity.Any(e => e.y_stuId == stuId && e.y_term == term && e.y_courseId == courseId);
                        //if (!exist)
                        //{
                            list.Add(new StuScoreDto
                            {
                                StuId = stuId,
                                CourseId = courseId,
                                Term = term,
                                AvgNum = avgNum,
                                EndNum = endNum,
                                TotalNum = (avgNum * Scale + endNum * (1 - Scale))
                            });
                        //}
                    }
                    catch
                    {
                        return list = null;
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// 批量导入学生成绩
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<StuScoreDto> StuScoreAllExcelHandle(ISheet sheet)
        {
            using (var db = new IYunEntities())
            {
                var Scale = Convert.ToDecimal(db.YD_Edu_ScoreScale.SingleOrDefault().y_normalScale) / 100;//取得数据库中的分数比例
                int cout = sheet.LastRowNum;
                int cout2 = sheet.PhysicalNumberOfRows;
                IRow row;
                List<StuScoreDto> list = new List<StuScoreDto>();
                var StuScoreEntity = db.YD_Edu_Score;
                for (int i = 1; i < cout2; i++)
                {
                    row = sheet.GetRow(i);
                    try
                    {
                        decimal avgNum = Convert.ToDecimal(row.GetCell(8).StringCellValue);
                        decimal endNum = Convert.ToDecimal(row.GetCell(9).StringCellValue);
                        decimal totalNum = Convert.ToDecimal(row.GetCell(12).StringCellValue);
                        var stuNum = row.GetCell(1).StringCellValue;
                        var stuId = db.YD_Sts_StuInfo.FirstOrDefault(x => x.y_stuNum == stuNum).id;
                        var courseName = row.GetCell(6).StringCellValue.Split('(')[0];
                        int courseId;
                        if (db.YD_Edu_Course.Any(x => x.y_name == courseName))
                        {
                            courseId= db.YD_Edu_Course.FirstOrDefault(x => x.y_name == courseName).id;
                        }
                        else
                        {
                            var maxid = db.YD_Edu_Course.Max(x => x.id);
                            db.YD_Edu_Course.Add(new YD_Edu_Course()
                            {
                                y_code = (maxid+1).ToString(),
                                y_name = courseName,
                            });
                            db.SaveChanges();
                            courseId = maxid;
                        }

                        var term = Convert.ToInt32(row.GetCell(7).StringCellValue);
                        
                        list.Add(new StuScoreDto
                        {
                            StuId = stuId,
                            CourseId = courseId,
                            Term = term,
                            AvgNum = avgNum,
                            EndNum = endNum,
                            TotalNum = totalNum
                        });
                        //}
                    }
                    catch(Exception e)
                    {
                        return list = null;
                    }
                }
                return list;
            }
        }

        public static string ExamPlanExcelHandle(ISheet sheet)
        {
            using (var db = new IYunEntities())
            {
                int cout = sheet.LastRowNum;
                int cout2 = sheet.PhysicalNumberOfRows;
                IRow row;
                for (int i = 1; i < cout2; i++)
                {
                    row = sheet.GetRow(i);
                    try
                    {
                        List<ExamPlanExcelInsertDto> list = new List<ExamPlanExcelInsertDto>();
                        string majorName = Convert.ToString(row.GetCell(0).StringCellValue);
                        string eduType = Convert.ToString(row.GetCell(1).StringCellValue);
                        string stuType = Convert.ToString(row.GetCell(2).StringCellValue);
                        string courseName = Convert.ToString(row.GetCell(3).StringCellValue);
                        DateTime time = Convert.ToDateTime(row.GetCell(4).DateCellValue);
                        int year = Convert.ToInt32(row.GetCell(5).NumericCellValue);
                        int term = Convert.ToInt32(row.GetCell(6).NumericCellValue);
                        list.Add(new ExamPlanExcelInsertDto
                        {
                            MajorName = majorName,
                            EduType = eduType,
                            StuType = stuType,
                            CourseName = courseName,
                            Time = time,
                            Year = year,
                            Term = term
                        });
                        foreach (var item in list)
                        {
                            db.YD_TeaPlan_ExamPlan.Add(new YD_TeaPlan_ExamPlan()
                            {
                                id = db.YD_TeaPlan_ExamPlan.Any() ? db.YD_TeaPlan_ExamPlan.Max(u => u.id) + 1 : 1,
                                y_majorName = item.MajorName + ' ' + item.EduType + ' ' + item.StuType,
                                y_courseName = item.CourseName,
                                y_term = item.Term,
                                y_time = item.Time,
                                y_year = item.Year
                            });
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        return e.Message.ToString();
                    }
                }
                return "ok";
            }

        }

    }
}