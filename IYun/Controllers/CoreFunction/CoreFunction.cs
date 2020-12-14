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

                                    if (errorCount == 0)
                                        data.MajorName = cell.StringCellValue.Trim();
                                }

                                break;
                            case 1:
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
                            case 2:
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
                            case 3:
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
                            case 4:
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
                            case 8:
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
                            case 9:
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
                ad.YD_Sys_SubSchool.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      sub.Add(u.y_name, u.id);
                  });
                ad.YD_Sts_Nation.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      nation.Add(u.y_name, u.id);
                  });
                ad.YD_Sts_Politics.Select(u => new { u.id, u.y_name }).ToList().ForEach(u =>
                  {
                      pition.Add(u.y_name, u.id);
                  });
                var examNumlist = ad.YD_Sts_StuInfo.Select(u => u.y_examNum).ToList();
                #endregion
                var list = new List<YD_Sts_StuInfoTemp>(); //数据转化List
                var examNum = new List<string>(); //模板的考生号集合
                const double eps = 1e-10; // 精度范围            
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var data = new YD_Sts_StuInfoTemp();
                    for (int j = 0; j < 15; j++)
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
                                    data.y_examNum = null;
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
                            case 3: //入学年度
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
                            case 5: //培养层次
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
                                    //if (errorCount == 0)
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
                                    data.y_subSchoolId = -1;
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
                                        data.y_subSchoolId = -1;
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
                                //else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                //{
                                //    cell.CellStyle = errorStyle;
                                //    errorCount++;
                                //}
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
                                    if (errorCount == 0)
                                        data.y_tel = cell.StringCellValue.Trim();
                                }
                                break;
                            case 10: //地址
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    data.y_address = null;
                                }
                                //else if (cell.CellType != CellType.STRING) //如果不是string 类型
                                // {
                                //     cell.CellStyle = errorStyle;
                                //     errorCount++;
                                // }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = errorStyle;
                                        errorCount++;
                                    }
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
                                    DateTime.TryParse(birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" +
                                 birthdate.Substring(6, 2), out birthday);
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
                    var substulistone = substulist.FirstOrDefault(u => u.y_cardId == stu.y_cardId); //函授站考生导入表

                    #region 如果身份证匹配则归入对应站点否则赋予导入站点内容

                    if (substulistone != null)
                    {
                        stuinfo = new YD_Sts_StuInfo
                        {
                            y_name = stu.y_name,
                            y_stuNum = "",
                            y_examNum = stu.y_examNum,
                            y_sex = Convert.ToInt32(stu.y_sex),
                            y_inYear = Convert.ToInt32(stu.y_inYear),
                            y_cardId = stu.y_cardId,
                            y_majorId = Convert.ToInt32(stu.y_majorId),
                            y_subSchoolId = Convert.ToInt32(substulistone.y_subSchoolId),
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
                        };
                    }
                    else
                    {
                        stuinfo = new YD_Sts_StuInfo
                        {
                            y_name = stu.y_name,
                            y_stuNum = "",
                            y_examNum = stu.y_examNum,
                            y_sex = Convert.ToInt32(stu.y_sex),
                            y_inYear = Convert.ToInt32(stu.y_inYear),
                            y_cardId = stu.y_cardId,
                            y_majorId = Convert.ToInt32(stu.y_majorId),
                            y_subSchoolId = Convert.ToInt32(stu.y_subSchoolId),
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
                        };
                    }
                    #endregion

                    stulist.Add(stuinfo);
                }
                ad.Configuration.AutoDetectChangesEnabled = false;
                ad.Configuration.ValidateOnSaveEnabled = false;
                ad.Set<YD_Sts_StuInfo>().AddRange(stulist);
                int r = ad.SaveChanges();
                if (r > 0)
                {
                    #region 生成学生所有缴费学年的记录

                    var students = ad.YD_Sts_StuInfoTemp.Select(u => u.y_inYear);
                    //生成第一缴费学年的学生名单
                    var oneYearStus = ad.VW_StuInfo.Where(u => students.Contains(u.y_inYear)).ToList();

                    var stuFeeTbsOne = new List<YD_Fee_StuFeeTb>();


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
                    //            if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //            if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //            if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //            if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //                if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //                {
                    //                    stuFee.y_stuYear = 4;
                    //                }
                    //                if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //                {
                    //                    stuFee.y_stuYear = 5;
                    //                }
                    //                if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //                if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //                {
                    //                    stuFee.y_stuYear = 4;
                    //                }
                    //                if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //                {
                    //                    stuFee.y_stuYear = 5;
                    //                }
                    //                if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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
                    //            if (stuFee.y_eduTypeId == 1) //如果培养层次是高起专则学制为4
                    //            {
                    //                stuFee.y_stuYear = 4;
                    //            }
                    //            if (stuFee.y_eduTypeId == 2) //如果培养层次是高起本则学制为5
                    //            {
                    //                stuFee.y_stuYear = 5;
                    //            }
                    //            if (stuFee.y_eduTypeId == 4) //如果培养层次是专升本则学制为3
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

                    ad.Configuration.AutoDetectChangesEnabled = false;
                    ad.Configuration.ValidateOnSaveEnabled = false;
                    ad.Set<YD_Fee_StuFeeTb>().AddRange(stuFeeTbsOne);
                    ad.SaveChanges();
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
                                    //if (cell.CellType != CellType.STRING) //如果不是string 类型
                                    //{
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    //else
                                    //{
                                    //if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    //{
                                    //    cell.SetCellValue("Can't be null");
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                var cou = Convert.ToInt32(cell.StringCellValue.Trim());
                                if (!course.Keys.Contains(cou))
                                {
                                    cell.CellStyle = errorStyle;
                                    errorCount++;
                                }
                                if (errorCount == 0)
                                    data.y_courseId = Convert.ToInt32(cell.StringCellValue.Trim());
                                //data.y_courseId = course[cell.StringCellValue.Trim()];
                                //}
                                break;
                            case 3: //平时成绩
                                if (cell.CellType == CellType.BLANK || cell == null)
                                {
                                    data.y_normalScore = 0;
                                }
                                //if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                //{
                                //    cell.CellStyle = errorStyle;
                                //    errorCount++;
                                //}
                                else
                                {
                                    //    if (cell.NumericCellValue < 0 ||
                                    //        cell.NumericCellValue - (double)((int)cell.NumericCellValue) >= eps)
                                    //    //传说中的判断double是否整数，效果未测试
                                    //    {
                                    //        cell.CellStyle = errorStyle;
                                    //        errorCount++;
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
                                    //if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                    //{
                                    //    cell.CellStyle = errorStyle;
                                    //    errorCount++;
                                    //}
                                    if (errorCount == 0)
                                        data.y_termScore = Convert.ToDecimal(cell.StringCellValue);
                                }
                                break;
                            case 5: //总评成绩
                                if (cell.CellType == CellType.BLANK || cell == null)
                                {
                                    data.y_totalScore = 0;
                                }
                                //if (cell.CellType != CellType.NUMERIC) //如果不是NUMERIC 类型
                                //{
                                //    cell.CellStyle = errorStyle;
                                //    errorCount++;
                                //}
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
                var stulist = ad.VW_StuInfo.GroupBy(u => new { u.id, u.y_inYear, u.y_name, u.y_cardId, u.y_majorLibId,u.y_subSchoolId }).Select(u => u.Key).ToList(); //得到所有学生
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
                            case 0:  //考试时间
                                if (cell == null)
                                {
                                    cell = row.CreateCell(0);
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
                                    var examdate = cell.StringCellValue.Trim();
                                    DateTime yExamtime;
                                    DateTime.TryParse(examdate.Substring(0, 4) + "-" + examdate.Substring(4, 2) + "-" +
                                     examdate.Substring(6, 2), out yExamtime);
                                    if (index == 0)
                                        data.y_examtime = yExamtime;
                                }
                                break;
                            case 1: //入学年度
                                if (cell == null)
                                {
                                    cell = row.CreateCell(1);
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
                                    cell = row.CreateCell(2);
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
                            case 3: //性别
                                if (cell == null)
                                {
                                    cell = row.CreateCell(3);
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
                                    var sex = new Dictionary<string, int> { { "男", 0 }, { "女", 1 } };
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else if (!sex.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }

                                    if (index == 0)
                                        data.y_sex = sex[cell.StringCellValue.Trim()];
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
                                    else if (!cardlist.Contains(cell.StringCellValue) )
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    if (index == 0)
                                    {
                                        data.y_cardId = cell.StringCellValue.Trim();
                                         var stu = stulist.FirstOrDefault(u => u.y_inYear == data.y_inYear && u.y_name == data.y_stuname && u.y_cardId == data.y_cardId);
                                        if (stu != null)
                                        {
                                            data.y_stuId = stu.id;
                                            data.y_subSchoolId = stu.y_subSchoolId;
                                        }
                                        else
                                        {
                                            row.Cells[1].CellStyle = styleCell; //入学年度
                                            row.Cells[2].CellStyle = styleCell; //姓名
                                            row.Cells[4].CellStyle = styleCell; //身份证号
                                            index++;
                                        }
                                    }

                                }
                                break;
                            case 5: //专业名
                                if (cell == null)
                                {
                                    cell = row.CreateCell(5);
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
                                    //else if (!majorlib.Keys.Contains(cell.StringCellValue.Trim()))
                                    //{
                                    //    cell.CellStyle = styleCell;
                                    //    index++;
                                    //}
                                    if (index == 0)
                                    {
                                        data.y_majorLibId = majorlib[cell.StringCellValue.Trim()];                       
                                    }
                                }
                                break;
                            case 6: //准考证号
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(6);
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
                            case 7: //总分
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(7);
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
                            case 8: //主观分
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(8);
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
                            case 9: //结论
                                if (cell == null || cell.CellType == CellType.BLANK)
                                {
                                    cell = row.CreateCell(9);
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
                                    var verdict = new Dictionary<string, int> { { "不通过", 0 }, { "通过", 1 } };
                                    if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                                    {
                                        cell.SetCellValue("Can't be null");
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    else if (!verdict.Keys.Contains(cell.StringCellValue.Trim()))
                                    {
                                        cell.CellStyle = styleCell;
                                        index++;
                                    }
                                    if (index == 0)
                                        data.y_verdict = verdict[cell.StringCellValue.Trim()];
                                }
                                break;
                        }
                    }
                    if (index == 0)
                        list.Add(data); //把数据加载到List里面
                }
                if (index == 0)
                {
                    ad.Configuration.AutoDetectChangesEnabled = false;
                    ad.Configuration.ValidateOnSaveEnabled = false;
                    ad.Set<YD_Graduate_EnglishScoreTemp>().AddRange(list);
                    ad.SaveChanges();
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
                        y_sex = gradscores.y_sex,
                        y_verdict = Convert.ToInt32(gradscores.y_verdict),
                        y_inYear = Convert.ToInt32(gradscores.y_inYear),
                        y_majorLibId = Convert.ToInt32(gradscores.y_majorLibId),
                        y_subSchoolId=Convert.ToInt32(gradscores.y_subSchoolId),
                        y_cardId = gradscores.y_cardId,
                        y_subjectivitysore = Convert.ToInt32(gradscores.y_subjectivitysore),
                        y_sumsore = Convert.ToInt32(gradscores.y_sumsore),
                        y_explain = gradscores.y_explain,
                        y_remark = gradscores.y_remark,
                        y_isdel = (int)YesOrNo.No
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
                var majorLibDb = ad.YD_Edu_MajorLibrary.Select(u => new { u.id, u.y_name }).ToList();

                int maxLibId = majorLibDb.Any() ? majorLibDb.Max(u => u.id) : 0;

                var lack_MajorLib = new List<string>();

                var majorLiblist = list.GroupBy(u => u.MajorName).ToList();

                foreach (IGrouping<string, TeaPlanExcelInsertDto> item in majorLiblist)
                {
                    var majorlib = majorLibDb.FirstOrDefault(u => u.y_name == item.Key);
                    if (majorlib == null)
                    {
                        lack_MajorLib.Add(item.Key);
                    }
                    else
                    {
                        item.ToList().ForEach(u => u.MajorLibId = majorlib.id);
                    }
                }

                var newmajorliblist =
                    lack_MajorLib.Select(
                        item => new YD_Edu_MajorLibrary() { y_name = item, y_code = "" }).ToList();

                ad.YD_Edu_MajorLibrary.AddRange(newmajorliblist);

                ad.SaveChanges();

                string sql = "update YD_Edu_MajorLibrary set y_code = id where id > " + maxLibId;
                ad.Database.ExecuteSqlCommand(sql);

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

                ad.YD_Edu_Major.AddRange(newmajorlist);

                ad.SaveChanges();

                string sql;

                //如果是中医药大学则改变学制
                if (ConfigurationManager.AppSettings["SchoolName"].ToString() == ComEnum.SchoolName.ZYYDX.ToString())
                {
                    sql =
                        "update YD_Edu_Major set y_code = b.id ,y_name = b.name,y_stuYear = b.stuyear FROM YD_Edu_Major, " +
                        "(select m.id,l.y_name+' '+e.y_name+' '+s.y_name as name, (case when e.y_name = '高起本' and m.y_name != '医药营销' " +
                        "then 5 when e.y_name = '高起专' and m.y_name != '医药营销' then 4 else 3 end) " +
                        " as stuyear from YD_Edu_Major as m join YD_Edu_MajorLibrary " +
                        "as l on l.id = m.y_majorLibId join YD_Edu_EduType " +
                        "as e on e.id = m.y_eduTypeId join YD_Edu_StuType as s " +
                        "on s.id = m.y_stuTypeId ) as b WHERE YD_Edu_Major.ID = B.ID and YD_Edu_Major.id > " +
                        maxmajorId; //todo:此SQL未测试
                }
                else
                {
                    sql =
                        "update YD_Edu_Major set y_code = b.id ,y_name = b.name,y_stuYear = b.stuyear FROM YD_Edu_Major, " +
                        "(select m.id,l.y_name+' '+e.y_name+' '+s.y_name as name, (case when e.y_name = '高起本' then 5 else 3 end)" +
                        " as stuyear from YD_Edu_Major as m join YD_Edu_MajorLibrary as l on l.id = m.y_majorLibId join YD_Edu_EduType " +
                        "as e on e.id = m.y_eduTypeId join YD_Edu_StuType as s on s.id = m.y_stuTypeId ) as b WHERE YD_Edu_Major.ID = B.ID and YD_Edu_Major.id > " +
                        maxmajorId;
                }

                ad.Database.ExecuteSqlCommand(sql);

                ad.SaveChanges();

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

                //todo:增加教学计划名称

                ad.YD_TeaPlan_Templet.AddRange(newTeaplanTempletlist);

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
    }
}