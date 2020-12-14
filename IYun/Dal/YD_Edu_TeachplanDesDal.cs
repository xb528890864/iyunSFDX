using System;
using System.Configuration;
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
    public delegate ResultInfo OperTeachPlanDes(HttpRequestBase request, YD_Edu_TeachplanDes module, IYunEntities yunEntities);
    public class YD_Edu_TeachPlanDesDesDal : BaseDal<YD_Edu_TeachplanDes>
    {
        /// <summary>
        /// 增加方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要新增的层次类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(HttpRequestBase request, YD_Edu_TeachplanDes module, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(module, "/Edu/TeachPlan", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加教学计划课程,id：" + module.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要修改的层次类型对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(HttpRequestBase request, YD_Edu_TeachplanDes module, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(module, "/Edu/TeachPlan", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改教学计划课程,id：" + module.id);
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
        private static string Oper(HttpRequestBase request, YD_Edu_TeachplanDes module, OperTeachPlanDes curd, IYunEntities yunEntities)
        {
            var resultInfo = curd(request, module, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }

        /// <summary>
        /// 编辑层次
        /// </summary>
        /// <param name="module">层次对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditEntity(HttpRequestBase request, YD_Edu_TeachplanDes module, IYunEntities yunEntities)
        {

            return Oper(request, module, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加层次
        /// </summary>
        /// <param name="module">层次对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddEntity(HttpRequestBase request, YD_Edu_TeachplanDes module, IYunEntities yunEntities)
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
            var module = yunEntities.YD_Edu_TeachplanDes.FirstOrDefault(u => u.y_teaPlanId == id);
            if (module != null)
            {
                yunEntities.Entry(module).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除教学计划课程，id：" + module.id);
                
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除教学计划课程失败";
        }
        /// <summary>
        /// 师范大学的添加课程
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string CreateTeachPlanSd(HttpRequestBase request)
        {
            using (var yunEntities = new IYunEntities())
            {
                //获取所有提交过来的数据
                var year = Convert.ToInt32(request["year"]);
                var y_term = Convert.ToInt32(request["term"]);
                var majorId = Convert.ToInt32(request["majorlihidden2"]);
                var teachplantype = 2;
                var y_courseId = Convert.ToInt32(request["coursehidden"]);
                var y_courseTypeId = Convert.ToInt32(request["y_courseTypeId"]);
                var y_stuTime = Convert.ToInt32(request["y_stuTime"]);
                var y_score = Convert.ToDecimal(request["y_score"]);
                //查询出teachPlanID
                var teachplanid =
                    yunEntities.VW_MajorTeachPlan.FirstOrDefault(
                        u =>
                            u.y_teaPlanType == teachplantype && u.y_majorId == majorId && u.y_term == y_term &&
                            u.y_year == year);
                if (teachplanid == null)
                {
                    var majorlist = yunEntities.YD_Edu_Major.Where(u => u.id == majorId).ToList();
                    foreach (var m in majorlist)
                    {
                        //添加教学计划
                        var teachplan = new YD_Edu_TeachPlan()
                        {
                            y_teaPlanName = m.y_name + "第" + y_term + "学期" + teachplantype + "类型" + year + "年" + DateTime.Now.ToString("yyyy/MM/dd"),
                            y_term = y_term
                        };
                        yunEntities.Entry(teachplan).State = EntityState.Added;
                        yunEntities.SaveChanges();
                        //添加专业教学计划
                        var majorthachplan = new YD_Edu_MajorTeachPlan()
                        {
                            y_year = year,
                            y_majorId = majorId,
                            y_teachPlanId = teachplan.id,
                            y_teaPlanType = teachplantype
                        };
                        yunEntities.Entry(majorthachplan).State = EntityState.Added;
                        yunEntities.SaveChanges();
                    }
                    //查询专业教学计划视图得到添加的教学计划id
                    teachplanid =
                     yunEntities.VW_MajorTeachPlan.FirstOrDefault(
                         u =>
                             u.y_teaPlanType == teachplantype && u.y_majorId == majorId && u.y_term == y_term &&
                             u.y_year == year);
                }
                //插入数据
                var TeachplanDes = new YD_Edu_TeachplanDes()
                {
                    y_teaPlanId = teachplanid.y_teachPlanId,
                    y_courseId = y_courseId,
                    y_courseTypeId = y_courseTypeId,
                    y_stuTime = y_stuTime,
                    y_score = y_score
                };
                yunEntities.Entry(TeachplanDes).State = EntityState.Added;
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return "ok";
                }
                else
                {
                    return  "未知错误";
                }
            }
        }
        /// <summary>
        /// 根据学校对应相关的添加课程的方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string CreateTeachPlanCourse(HttpRequestBase request, IYunEntities yunEntities, YD_Edu_TeachplanDes modules)
        {
            var str = ConfigurationManager.AppSettings["SchoolName"].ToString();
            switch (str)
            {
                case "JXSFDX":
                    return CreateTeachPlanSd(request);
                default:
                    return AddEntity(request, modules, yunEntities);
            }
        }

    }
}