using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using IYun.Common;
using IYun.Models;

namespace IYun.Dal
{
    /// <summary>
    /// 对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <param name="courseType"></param>
    /// <returns></returns>
    public delegate ResultInfo OperCourseType(YD_Edu_CourseType courseType, IYunEntities yunEntities);
    public class YD_Edu_CourseTypeDal : BaseDal<YD_Edu_CourseType>
    {
        /// <summary>
        /// 增加方法,委托：OpercourseType类型
        /// </summary>
        /// <param name="courseType">要新增的课程类型类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Edu_CourseType courseType, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(courseType, "/SysAdmin/courseType", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加课程类型类型,课程类型类型名为" + courseType.y_name + ",id：" + courseType.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OpercourseType类型
        /// </summary>
        /// <param name="courseType">要修改的课程类型类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Edu_CourseType courseType, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(courseType, "/SysAdmin/courseType", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改课程类型类型,课程类型类型名为" + courseType.y_name + ",id：" + courseType.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="courseType">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Edu_CourseType courseType, OperCourseType curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.y_name == courseType.y_name && u.id != courseType.id) != null)
            {
                return "已存在相同课程类型类型名的课程类型类型";
            }
            if (yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.y_courseTypeCode == courseType.y_courseTypeCode && u.id != courseType.id) != null)
            {
                return "已存在相同代码名的课程类型类型";
            }
            var resultInfo = curd(courseType, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑课程类型
        /// </summary>
        /// <param name="courseType">课程类型对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Edu_CourseType courseType, IYunEntities yunEntities)
        {

            return Oper(courseType, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加课程类型
        /// </summary>
        /// <param name="courseType">课程类型对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Edu_CourseType courseType, IYunEntities yunEntities)
        {
            var str = Oper(courseType, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除课程类型类型
        /// </summary>
        /// <param name="id">课程类型类型id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var courseType = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.id == id);
            if (courseType != null)
            {
                if (courseType.YD_Edu_TeachplanDes.Count != 0)
                {
                    return "该类型不能删除，其已经关联了教学计划";
                }
                yunEntities.Entry(courseType).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除课程类型类型，课程类型名为" + courseType.y_name + ",id：" + courseType.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除课程类型类型失败";
        }


    }
}