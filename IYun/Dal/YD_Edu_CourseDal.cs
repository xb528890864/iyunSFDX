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
    /// <param name="course"></param>
    /// <returns></returns>
    public delegate ResultInfo OperCourse(YD_Edu_Course course, IYunEntities yunEntities);
    public class YD_Edu_CourseDal : BaseDal<YD_Edu_Course>
    {
        /// <summary>
        /// 增加方法,委托：OperCourse类型
        /// </summary>
        /// <param name="course">要新增的课程对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Edu_Course course, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(course, "/Edu/course", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加课程,课程名为" + course.y_name + ",id：" + course.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperCourse类型
        /// </summary>
        /// <param name="course">要修改的课程对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Edu_Course course, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(course, "/Edu/course", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改课程,课程名为" + course.y_name + ",id：" + course.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="course">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Edu_Course course, OperCourse curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == course.y_name && u.id != course.id) != null)
            {
                return "已存在相同课程名的课程";
            }
            if (yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_code == course.y_code && u.id != course.id) != null)
            {
                return "已存在相同代码名的课程";
            }
            var resultInfo = curd(course, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑课程
        /// </summary>
        /// <param name="course">课程对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Edu_Course course, IYunEntities yunEntities)
        {

            return Oper(course, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加课程
        /// </summary>
        /// <param name="course">课程对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Edu_Course course, IYunEntities yunEntities)
        {
            var str = Oper(course, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除课程
        /// </summary>
        /// <param name="id">课程id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var course = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.id == id);
            if (course != null)
            {
                if (course.YD_Edu_TeachplanDes.Count != 0)
                {
                    return "该类型不能删除，其已经关联了指定教学计划";
                }
                var adminCourse = yunEntities.YD_Sys_AdminCourseLink.Where(u => u.y_courseId == id).ToList();
                for (int j = 0; j < adminCourse.Count; j++)
                {
                    yunEntities.Entry(adminCourse[j]).State = EntityState.Deleted;
                }
                yunEntities.Entry(course).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除课程，课程名为" + course.y_name + ",id：" + course.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除课程失败";
        }
    }
}