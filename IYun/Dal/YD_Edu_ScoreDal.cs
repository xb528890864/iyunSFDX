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
    /// <param name="Course"></param>
    /// <returns></returns>
    public delegate ResultInfo OperScore(YD_Edu_Score Course, IYunEntities yunEntities);
    public class YD_Edu_ScoreDal : BaseDal<YD_Edu_Score>
    {
        /// <summary>
        /// 增加方法,委托：OperCourse类型
        /// </summary>
        /// <param name="Course">要新增的课程对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Edu_Score Course, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(Course, "/Score/RecordScore", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加成绩,id：" + Course.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperCourse类型
        /// </summary>
        /// <param name="Course">要修改的课程对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Edu_Score Course, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(Course, "/Score/Score", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改成绩,id：" + Course.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="Course">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Edu_Score Course, OperScore curd, IYunEntities yunEntities)
        {

            var resultInfo = curd(Course, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑课程
        /// </summary>
        /// <param name="Course">课程对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Edu_Score Course, IYunEntities yunEntities)
        {

            return Oper(Course, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加课程
        /// </summary>
        /// <param name="Course">课程对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Edu_Score Course, IYunEntities yunEntities)
        {
            var str = Oper(Course, AddOper, yunEntities);
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
            var Course = yunEntities.YD_Edu_Score.FirstOrDefault(u => u.id == id);
            if (Course != null)
            {
                yunEntities.Entry(Course).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除成绩，学生id为" + Course.y_stuId + ",id：" + Course.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除成绩失败";
        }

    }
}