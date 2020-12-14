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
    /// <param name="majorLib"></param>
    /// <returns></returns>
    public delegate ResultInfo OperMajorLib(YD_Edu_MajorLibrary majorLib, IYunEntities yunEntities);
    public class YD_Edu_MajorLibraryDal : BaseDal<YD_Edu_MajorLibrary>
    {
        /// <summary>
        /// 增加方法,委托：OperMajorLib类型
        /// </summary>
        /// <param name="majorLib">要新增的专业对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Edu_MajorLibrary majorLib, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(majorLib, "/Edu/MajorLib", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加专业,专业名为" + majorLib.y_name + ",id：" + majorLib.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperMajorLib类型
        /// </summary>
        /// <param name="majorLib">要修改的专业对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Edu_MajorLibrary majorLib, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(majorLib, "/Edu/MajorLib", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改专业,专业名为" + majorLib.y_name + ",id：" + majorLib.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="MajorLib">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Edu_MajorLibrary MajorLib, OperMajorLib curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_name == MajorLib.y_name && u.id != MajorLib.id) != null)
            {
                return "已存在相同专业名的专业";
            }
            if (yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.y_code == MajorLib.y_code && u.id != MajorLib.id) != null)
            {
                return "已存在相同代码名的专业";
            }
            var resultInfo = curd(MajorLib, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑专业
        /// </summary>
        /// <param name="MajorLib">专业对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(YD_Edu_MajorLibrary MajorLib, IYunEntities yunEntities)
        {

            return Oper(MajorLib, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加专业
        /// </summary>
        /// <param name="MajorLib">专业对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(YD_Edu_MajorLibrary MajorLib, IYunEntities yunEntities)
        {
            var str = Oper(MajorLib, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除专业
        /// </summary>
        /// <param name="id">专业id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var MajorLib = yunEntities.YD_Edu_MajorLibrary.FirstOrDefault(u => u.id == id);
            if (MajorLib != null)
            {
                if (MajorLib.YD_Edu_Major.Count != 0)
                {
                    return "该类型不能删除，其已经关联了指定专业";
                }
                yunEntities.Entry(MajorLib).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除专业，专业名为" + MajorLib.y_name + ",id：" + MajorLib.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除专业失败";
        }


    }
}