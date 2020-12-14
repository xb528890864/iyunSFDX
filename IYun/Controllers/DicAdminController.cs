using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IYun.Dal;
using IYun.Models;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    public class DicAdminController : AdminBaseController
    {
        //
        // GET: /DicAdmin/
        private YD_Edu_StuTypeDal _stuTypeDal = new YD_Edu_StuTypeDal();
        private YD_Edu_EduTypeDal _eduEduTypeDal = new YD_Edu_EduTypeDal();
        private YD_Sts_PoliticsDal _politicsDal = new YD_Sts_PoliticsDal();
        private YD_Sts_NationDal _nationDal = new YD_Sts_NationDal();
        private YD_Edu_CourseTypeDal _courseTypeDal = new YD_Edu_CourseTypeDal();
        private YD_Edu_StuStateDal _stuStateDal = new YD_Edu_StuStateDal();
        #region 学习形式
        public ActionResult StuType(int id = 1)
        {
            #region “学习形式管理”权限验证
            var power = SafePowerPage("/DicAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_StuType.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("StuTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加学习形式页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeAddPage()
        {
            #region “添加学习形式”权限验证
            var power = SafePowerPage("/DicAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑学习形式视图
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeEditPage(int id)
        {
            #region “编辑学习形式”权限验证
            var power = SafePowerPage("/DicAdmin/StuType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_StuType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeEdit(YD_Edu_StuType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeAdd(YD_Edu_StuType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除学习形式
        /// </summary>
        /// <returns></returns>
        public ActionResult StuTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuTypeDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion

        #region 层次
        public ActionResult EduType(int id = 1)
        {
            #region “层次管理”权限验证
            var power = SafePowerPage("/DicAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_EduType.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("EduTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加层次页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeAddPage()
        {
            #region “添加层次”权限验证
            var power = SafePowerPage("/DicAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑层次视图
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeEditPage(int id)
        {
            #region “编辑层次”权限验证
            var power = SafePowerPage("/DicAdmin/EduType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_EduType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeEdit(YD_Edu_EduType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeAdd(YD_Edu_EduType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除层次
        /// </summary>
        /// <returns></returns>
        public ActionResult EduTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_eduEduTypeDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion

        #region 政治面貌
        public ActionResult Politics(int id = 1)
        {
            #region “政治面貌管理”权限验证
            var power = SafePowerPage("/DicAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Sts_Politics.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("PoliticsList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加政治面貌页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsAddPage()
        {
            #region “添加政治面貌”权限验证
            var power = SafePowerPage("/DicAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑政治面貌视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsEditPage(int id)
        {
            #region “编辑政治面貌”权限验证
            var power = SafePowerPage("/DicAdmin/Politics");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Sts_Politics.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsEdit(YD_Sts_Politics role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsAdd(YD_Sts_Politics role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除政治面貌
        /// </summary>
        /// <returns></returns>
        public ActionResult PoliticsDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_politicsDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion

        #region 民族类型
        public ActionResult Nation(int id = 1)
        {
            #region “课程类型管理”权限验证
            var power = SafePowerPage("/DicAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Sts_Nation.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("NationList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加课程类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NationAddPage()
        {
            #region “添加课程类型”权限验证
            var power = SafePowerPage("/DicAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑课程类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult NationEditPage(int id)
        {
            #region “编辑课程类型”权限验证
            var power = SafePowerPage("/DicAdmin/Nation");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Sts_Nation.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationEdit(YD_Sts_Nation role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationAdd(YD_Sts_Nation role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult NationDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_nationDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion

        #region 课程类型
        public ActionResult CourseType(int id = 1)
        {
            #region “课程类型管理”权限验证
            var power = SafePowerPage("/DicAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_CourseType.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("CourseTypeList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加课程类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeAddPage()
        {
            #region “添加课程类型”权限验证
            var power = SafePowerPage("/DicAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑课程类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeEditPage(int id)
        {
            #region “编辑课程类型”权限验证
            var power = SafePowerPage("/DicAdmin/CourseType");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_CourseType.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeEdit(YD_Edu_CourseType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeAdd(YD_Edu_CourseType role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除课程类型
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseTypeDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_courseTypeDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion

        #region 学籍状态类型
        public ActionResult StuState(int id = 1)
        {
            #region “学籍状态类型管理”权限验证
            var power = SafePowerPage("/DicAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;

            using (var yunEntities = new IYunEntities())
            {
                var dbLogList = yunEntities.YD_Edu_StuState.OrderByDescending(u => u.id).ToPagedList(id, 15);   //id为pageindex   15 为pagesize;
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
                if (Request.IsAjaxRequest())
                    return PartialView("StuStateList", dbLogList);
                return View(dbLogList);
            }
        }

        /// <summary>
        /// 添加学籍状态类型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateAddPage()
        {
            #region “添加学籍状态类型”权限验证
            var power = SafePowerPage("/DicAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_insert == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }
        /// <summary>
        /// 编辑学籍状态类型视图
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateEditPage(int id)
        {
            #region “编辑学籍状态类型”权限验证
            var power = SafePowerPage("/DicAdmin/StuState");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_update == (int)PowerState.Disable)
            {
                return Content("没有权限");
            }
            #endregion
            ViewBag.power = power;
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.entity = yunEntities.YD_Edu_StuState.FirstOrDefault(u => u.id == id);
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 6);     //根据父栏目ID获取兄弟栏目
            }
            return View();
        }

        /// <summary>
        /// 编辑学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateEdit(YD_Edu_StuState role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.EditEntity(role, yunEntities));
            }
        }

        /// <summary>
        /// 增加学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateAdd(YD_Edu_StuState role)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.AddEntity(role, yunEntities));
            }
        }
        /// <summary>
        /// 删除学籍状态类型
        /// </summary>
        /// <returns></returns>
        public ActionResult StuStateDelete(int id)
        {
            using (var yunEntities = new IYunEntities())
            {
                return Content(_stuStateDal.EntityDelete(id, yunEntities));
            }
        }
        #endregion


    }
}
