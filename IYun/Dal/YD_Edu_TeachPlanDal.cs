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
    public delegate ResultInfo OperTeachPlan(HttpRequestBase request, YD_Edu_TeachPlan module, IYunEntities yunEntities);
    public class YD_Edu_TeachPlanDal : BaseDal<YD_Edu_TeachPlan>
    {
        /// <summary>
        /// 增加方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要新增的层次类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(HttpRequestBase request, YD_Edu_TeachPlan module, IYunEntities yunEntities)
        {
            var yearStr = request["year"];
            var majorIdStr = request["majorId"];
            var teaPlanTypeStr = request["y_teaPlanType"];
            if (string.IsNullOrEmpty(yearStr) || string.IsNullOrEmpty(majorIdStr) || string.IsNullOrEmpty(teaPlanTypeStr))
            {
                return new ResultInfo { Success = false, Info = PowerInfo.Unknow, Message = "未知错误" };
            }
            var year = Convert.ToInt32(yearStr);
            var majorId = Convert.ToInt32(majorIdStr);
            var teaPlanType = Convert.ToInt32(teaPlanTypeStr);
            var resultInfo = BaseAddEntity(module, "/Edu/TeachPlan", yunEntities);
            if (resultInfo.Success)
            {
                var entity = new YD_Edu_MajorTeachPlan()
                {
                    y_majorId = majorId,
                    y_teachPlanId = module.id,
                    y_year = year,
                    y_teaPlanType = teaPlanType
                };
                yunEntities.Entry(entity).State = EntityState.Added;
                yunEntities.SaveChanges();
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加教学计划,教学计划名为" + module.y_teaPlanName + ",id：" + module.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要修改的层次类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(HttpRequestBase request, YD_Edu_TeachPlan module, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(module, "/Edu/TeachPlan", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改教学计划,教学计划名为" + module.y_teaPlanName + ",id：" + module.id);
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
        private static string Oper(HttpRequestBase request, YD_Edu_TeachPlan module, OperTeachPlan curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.y_teaPlanName == module.y_teaPlanName && u.id != module.id) != null)
            {
                return "已存在相同教学计划名的教学计划";
            }

            var resultInfo = curd(request, module, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }




        /// <summary>
        /// 编辑层次
        /// </summary>
        /// <param name="module">层次对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(HttpRequestBase request, YD_Edu_TeachPlan module, IYunEntities yunEntities)
        {

            return Oper(request, module, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加层次
        /// </summary>
        /// <param name="module">层次对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(HttpRequestBase request, YD_Edu_TeachPlan module, IYunEntities yunEntities)
        {
            var str = Oper(request, module, AddOper, yunEntities);
            return str;
        }


        /// <summary>
        /// 删除层次类型
        /// </summary>
        /// <param name="id">层次类型id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EntityDelete(int id, IYunEntities yunEntities)
        {
            var module = yunEntities.YD_Edu_TeachPlan.FirstOrDefault(u => u.id == id);
            if (module != null)
            {
                var majorTeachPlanList = module.YD_Edu_MajorTeachPlan.ToList();
                for (var j = 0; j < majorTeachPlanList.Count; j++)
                {
                    yunEntities.Entry(majorTeachPlanList[j]).State = EntityState.Deleted;
                }
                var teachPlanDesList = module.YD_Edu_TeachplanDes.ToList();
                for (var j = 0; j < teachPlanDesList.Count; j++)
                {
                    yunEntities.Entry(teachPlanDesList[j]).State = EntityState.Deleted;
                }
                yunEntities.Entry(module).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除教学计划，教学计划名为" + module.y_teaPlanName + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除教学计划失败";
        }
    }
}