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
    /// <param name="stuState"></param>
    /// <returns></returns>
    public delegate ResultInfo OperStuState(YD_Edu_StuState stuState, IYunEntities yunEntities);
    public class YD_Edu_StuStateDal : BaseDal<YD_Edu_StuState>
    {
        /// <summary>
        /// 增加方法,委托：OperstuState类型
        /// </summary>
        /// <param name="stuState">要新增的学籍状态类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Edu_StuState stuState, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(stuState, "/SysAdmin/StuState", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加学籍状态类型,学籍状态类型名为" + stuState.y_name + ",id：" + stuState.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperstuState类型
        /// </summary>
        /// <param name="stuState">要修改的学籍状态类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Edu_StuState stuState, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(stuState, "/SysAdmin/StuState", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改学籍状态类型,学籍状态类型名为" + stuState.y_name + ",id：" + stuState.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="stuState">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Edu_StuState stuState, OperStuState curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_name == stuState.y_name && u.id != stuState.id) != null)
            {
                return "已存在相同学籍状态类型名的学籍状态类型";
            }
            if (yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.y_stuStateCode == stuState.y_stuStateCode && u.id != stuState.id) != null)
            {
                return "已存在相同代码名的学籍状态类型";
            }
            var resultInfo = curd(stuState, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑学籍状态
        /// </summary>
        /// <param name="stuState">学籍状态对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Edu_StuState stuState, IYunEntities yunEntities)
        {

            return Oper(stuState, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加学籍状态
        /// </summary>
        /// <param name="stuState">学籍状态对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Edu_StuState stuState, IYunEntities yunEntities)
        {
            var str = Oper(stuState, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除学籍状态类型
        /// </summary>
        /// <param name="id">学籍状态类型id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var stuState = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.id == id);
            if (stuState != null)
            {
                if (stuState.YD_Sts_StuInfo.Count != 0)
                {
                    return "该类型不能删除，其已经关联了指定学生";
                }
                yunEntities.Entry(stuState).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除学籍状态类型，学籍状态名为" + stuState.y_name + ",id：" + stuState.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除学籍状态类型失败";
        }

    }
}