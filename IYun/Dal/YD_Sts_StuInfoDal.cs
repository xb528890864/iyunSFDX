using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using IYun.Common;
using IYun.Controllers;
using IYun.Models;
using IYun.Object;

namespace IYun.Dal
{
    /// <summary>
    /// Student对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <param name="student"></param>
    /// <returns></returns>
    public delegate ResultInfo OperStudent(YD_Sts_StuInfo student, IYunEntities yunEntities);
    public class YD_Sts_StuInfoDal : BaseDal<YD_Sts_StuInfo>
    {
        /// <summary>
        /// 增加方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要新增的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sts_StuInfo admin, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(admin, "/Student/StudentInfo", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert,
                    "新增学生，学生名为" + admin.y_name + ",id:" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要修改的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sts_StuInfo admin, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(admin, "/Student/StudentInfo", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update,
                    "修改学生，修改为" + admin.y_name + ",id：" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="student">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        private static string Oper(YD_Sts_StuInfo student, OperStudent curd, IYunEntities yunEntities)
        {
            var resultInfo = curd(student, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        public string EditStudent(YD_Sts_StuInfo student, IYunEntities yunEntities)
        {
            return Oper(student, EditOper, yunEntities);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <returns></returns>
        public ResultInfo UpdatePwd(HttpRequestBase request, IYunEntities yunEntities)
        {
            var nowPwd = request["nowPwd"];
            var newPwd = request["newPwd"];
            var rePwd = request["rePwd"];
            var resultInfo = new ResultInfo();
            if (!IsLogin())
            {
                resultInfo.Success = false;
                resultInfo.Message = "未登录";
                return resultInfo;
            }
            var admin = yunEntities.YD_Sys_Admin.Find(YdAdminId);

            if (string.IsNullOrEmpty(newPwd) || string.IsNullOrEmpty(nowPwd) || string.IsNullOrEmpty(rePwd))
            {
                resultInfo.Success = false;
                resultInfo.Message = "还有未填项";
                return resultInfo;
            }
            if (rePwd != newPwd)
            {
                resultInfo.Success = false;
                resultInfo.Message = "两次密码不一致";
                return resultInfo;
            }
            if (PageValidate.GetMd5StrT(nowPwd) != admin.y_password)
            {
                resultInfo.Success = false;
                resultInfo.Message = "原始密码不正确";
                return resultInfo;
            }
            admin.y_password = PageValidate.GetMd5StrT(newPwd);
            yunEntities.Entry(admin).State = EntityState.Modified;
            var t = yunEntities.SaveChanges();
            if (t > 0)
            {
                resultInfo.Success = true;
                return resultInfo;
            }
            resultInfo.Success = false;
            resultInfo.Message = "未知错误";
            return resultInfo;
        }
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <returns></returns>
        public string AddStudent(YD_Sts_StuInfo studInfo, IYunEntities yunEntities)
        {
            return Oper(studInfo, AddOper, yunEntities);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public string DeleteStudent(HttpRequestBase request, IYunEntities yunEntities)
        {
            var ids = request["ids"];
            if (string.IsNullOrEmpty(ids))
            {
                return "数据为空";
            }

            #region “删除”权限验证

            var powerInfo = Safe("/Student/StudentInfo", PowerFlag.Delete);
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    return "无权限";
                case PowerInfo.NoLogin:
                    return "未登录";
                case PowerInfo.Unknow:
                    return "未知状况";
                case PowerInfo.HasPower:
                    break;
                default:
                    return "未知状况";
            }

            #endregion

            var idsstr = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var idsList = new List<int>();
            for (var i = 0; i < idsstr.Count(); i++)
            {
                idsList.Add(Convert.ToInt32(idsstr[i]));
            }

            var admins = yunEntities.YD_Sts_StuInfo.Where(u => idsList.Contains(u.id)).ToList();

            foreach (var admin in admins)
            {
                yunEntities.Entry(admin).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete,
                    "删除学生，学生名为" + admin.y_name + ",id：" + admin.id);
            }
            var j = yunEntities.SaveChanges();
            return j > 0 ? "ok" : "删除学生失败";
        }


        /// <summary>
        /// 添加学生信息
        /// </summary>
        /// <param name="stuInfo">学生对象</param>
        /// <param name="request">request对象，要包括专业，学习形式，专业层次</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns>处理结果</returns>
        public string AddStuInfoExtended(YD_Sts_StuInfo stuInfo, HttpRequestBase request, IYunEntities yunEntities)
        {
            var majorLibrary = request.Params["MajorLibrary"];
            var eduType = request.Params["EduType"];
            var stuType = request.Params["StuType"];
            var schoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
            int majorId;
            if (schoolName == ComEnum.SchoolName.JXKJSFDX.ToString())
            {
                majorId = AdminBaseController.GetMajorIds(Convert.ToInt32(majorLibrary), Convert.ToInt32(eduType),Convert.ToInt32(stuType));
            }
            else
            {
                majorId = AdminBaseController.GetMajorIds(Convert.ToInt32(majorLibrary), Convert.ToInt32(eduType), Convert.ToInt32(stuType));
            }
         
            if (majorId == 0)
            {
                return "学生添加失败:该层次，学习形式下专业不存在！";
            }
            stuInfo.y_majorId = majorId;
            if (string.IsNullOrWhiteSpace(stuInfo.y_loginName) || string.IsNullOrWhiteSpace(stuInfo.y_password))
            {
                stuInfo.y_loginName = null;
                stuInfo.y_password = null;
            }
            else
            {
                if (yunEntities.YD_Sts_StuInfo.Any(u => u.y_loginName == stuInfo.y_loginName) || yunEntities.YD_Sys_Admin.Any(u => u.y_name == stuInfo.y_loginName))
                {
                    return "学生登录名已经存在！";
                }
                stuInfo.y_password = PageValidate.GetMd5StrL(stuInfo.y_password.Trim());
            }
            if (stuInfo.y_nationId == 0)
            {
                stuInfo.y_nationId = null;
            }
            if (stuInfo.y_politicsId == 0)
            {
                stuInfo.y_politicsId = null;
            }
            stuInfo.y_isChangePlan = (int)YesOrNo.No;
            stuInfo.y_changePlanId = null;
            stuInfo.y_registerState = "";
            stuInfo.y_isdel = (int) YesOrNo.No;
            return AddStudent(stuInfo, yunEntities);
        }

        /// <summary>
        /// 修改学生信息
        /// </summary>
        /// <param name="stuInfo">学生对象</param>
        /// <param name="request">request对象，要包括专业，学习形式，专业层次</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns>处理结果</returns>
        public string EditStuInfoExtended(YD_Sts_StuInfo stuInfo, HttpRequestBase request, IYunEntities yunEntities)
        {
            //var stu = yunEntities.YD_Sts_StuInfo.Find(stuInfo.id);
            string ms = "";
            if (stuInfo.id == 0)
            {
                ms = "学生修改失败：编号错误";
                return ms;
            }
            //stu.id = stuInfo.id;
            //stu.y_address = stuInfo.y_address;
            //stu.y_name = stuInfo.y_name;
            //stu.y_sex = stuInfo.y_sex;
            //stu.y_cardId = stuInfo.y_cardId;
            //stu.y_birthday = stuInfo.y_birthday;
            //stu.y_examNum = stuInfo.y_examNum;
            //stu.y_stuNum = stuInfo.y_stuNum;
            //stu.y_tel = stuInfo.y_tel;
            //stu.y_address = stuInfo.y_address;
            //stu.y_mail = stuInfo.y_mail;
            //stu.y_img = stuInfo.y_img;
            var majorli=request["majorhidden"];
            int majorliid = 0;
            if (majorli == "" && stuInfo.y_majorId == 0)
            {
                var majorlib = yunEntities.VW_StuInfo.FirstOrDefault(u => u.id == stuInfo.id);
                if (majorlib != null)
                {
                    var major = yunEntities.YD_Edu_Major.FirstOrDefault(u => u.id == majorlib.y_majorId);
                    if (major != null)
                        majorliid = major.y_majorLibId;
                }
            }
            else
            {
                majorliid =Convert.ToInt32(majorli);
            }
            if (string.IsNullOrWhiteSpace(stuInfo.y_registerState))
            {
                var majorId = AdminBaseController.GetMajorIds(majorliid, Convert.ToInt32(request.Params["EduType"]), Convert.ToInt32(request.Params["StuType"]));
                if (majorId == 0)
                {
                    ms = "学生修改失败：专业生成参数错误";
                    return ms;
                }
                stuInfo.y_majorId = majorId;
                //stu.y_majorId = majorId;
                //stu.y_nationId = stuInfo.y_nationId;
                //stu.y_politicsId = stuInfo.y_politicsId;
                //stu.y_subSchoolId = stuInfo.y_subSchoolId;
                //stu.y_inYear = stuInfo.y_inYear;
                //stu.y_stuStateId = stuInfo.y_stuStateId;
            }
            if (stuInfo.y_nationId == 0)
            {
                stuInfo.y_nationId = null;
            }
            if (stuInfo.y_politicsId == 0)
            {
                stuInfo.y_politicsId = null;
            }
           yunEntities.Entry(stuInfo).State=EntityState.Modified;

           int r=yunEntities.SaveChanges();

          
            if (r > 0)
            {
                return "ok";
            }
            else
            {
                return ms;
            }
        }

        /// <summary>
        /// 软删除学生信息
        /// </summary>
        /// <param name="id">学生对象id</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns>处理结果</returns>
        public string DeleStudentById(int id, IYunEntities yunEntities)
        {
            var stu = yunEntities.YD_Sts_StuInfo.Find(id);
            if (stu==null)
            {
                return "删除学生失败！编号无效";
            }
            stu.y_isdel = (int)YesOrNo.Yes;
            return EditStudent(stu, yunEntities);
        }
    }
}