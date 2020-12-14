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
    /// <param name="module"></param>
    /// <returns></returns>
    public delegate ResultInfo OperPolitics(YD_Sts_Politics module, IYunEntities yunEntities);
    public class YD_Sts_PoliticsDal : BaseDal<YD_Sts_Politics>
    {

        /// <summary>
        /// 增加方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要新增的政治面貌类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sts_Politics module, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(module, "/SysAdmin/Politics", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加政治面貌类型,政治面貌类型名为" + module.y_name + ",id：" + module.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要修改的政治面貌类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sts_Politics module, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(module, "/SysAdmin/Politics", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改政治面貌类型,政治面貌类型名为" + module.y_name + ",id：" + module.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="module">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Sts_Politics module, OperPolitics curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.y_name == module.y_name && u.id != module.id) != null)
            {
                return "已存在相同政治面貌类型名的政治面貌类型";
            }
            if (yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.y_code == module.y_code && u.id != module.id) != null)
            {
                return "已存在相同代码名的政治面貌类型";
            }
            var resultInfo = curd(module, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑政治面貌
        /// </summary>
        /// <param name="module">政治面貌对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Sts_Politics module, IYunEntities yunEntities)
        {

            return Oper(module, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加政治面貌
        /// </summary>
        /// <param name="module">政治面貌对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Sts_Politics module, IYunEntities yunEntities)
        {
            var str = Oper(module, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除政治面貌类型
        /// </summary>
        /// <param name="id">政治面貌类型id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var module = yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.id == id);
            if (module != null)
            {
                if (module.YD_Sts_StuInfo.Count != 0)
                {
                    return "该类型不能删除，其已经关联了指定学生";
                }
                yunEntities.Entry(module).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除政治面貌类型，政治面貌名为" + module.y_name + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除政治面貌类型失败";
        }


    }
}