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
    /// Admin对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
    public delegate ResultInfo OperAdmin(YD_Sys_Admin role, IYunEntities yunEntities);
    public class YD_Sys_AdminDal : BaseDal<YD_Sys_Admin>
    {
        /// <summary>
        /// 增加方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要新增的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sys_Admin admin, IYunEntities yunEntities)
        {
            admin.y_time = DateTime.Now;
            admin.y_password = PageValidate.GetMd5StrL(admin.y_password);
            var resultInfo = BaseAddEntity(admin, "/SysAdmin/Admin", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert,
                    "新增用户，用户名为" + admin.y_name + ",id:" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要修改的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sys_Admin admin, IYunEntities yunEntities)
        {
            admin.y_time = DateTime.Now;
            var resultInfo = BaseEditEntity(admin, "/SysAdmin/Admin", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update,
                    "修改用户，修改为" + admin.y_name + ",id：" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="admin">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        private static string Oper(YD_Sys_Admin admin, OperAdmin curd, IYunEntities yunEntities)
        {
            var resultInfo = curd(admin, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        public string EditAdmin(YD_Sys_Admin admin, IYunEntities yunEntities)
        {
            return Oper(admin, EditOper, yunEntities);
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
            if (PageValidate.GetMd5StrT(nowPwd).ToUpper() != admin.y_password.ToUpper())
            {
                resultInfo.Success = false;
                resultInfo.Message = "原始密码不正确";
                return resultInfo;
            }
            admin.y_password = PageValidate.GetMd5StrL(newPwd);
            admin.y_realpassword = newPwd;
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
        public string AddAdmin(YD_Sys_Admin admin,IYunEntities yunEntities)
        {
            if (yunEntities.YD_Sys_Admin.Any(u => u.y_roleID == admin.y_roleID && u.y_name == admin.y_name))
            {
                return "已存在相同用户名的用户";
            }

            return Oper(admin, AddOper, yunEntities);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public string DeleteAdmin(HttpRequestBase request, IYunEntities yunEntities)
        {
            var ids = request["ids"];
            if (string.IsNullOrEmpty(ids))
            {
                return "数据为空";
            }

            #region “删除”权限验证

            var powerInfo = Safe("/SysAdmin/Admin", PowerFlag.Delete);
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

            var admins = yunEntities.YD_Sys_Admin.Where(u => idsList.Contains(u.id)).ToList();

            foreach (var admin in admins)
            {
                if (admin.id == YdAdminId) return "不能删除当前账号";
                if (yunEntities.YD_Sys_DbLog.Any(u => u.y_userID == admin.id))
                {
                    return "当前账号存在关联日志，不能删除";
                }
                var adminCourseList = admin.YD_Sys_AdminCourseLink.ToList();
                var adminSubList = admin.YD_Sys_AdminSubLink.ToList();
                for (var i = 0; i < adminCourseList.Count; i++)
                {
                    yunEntities.Entry(adminCourseList[i]).State = EntityState.Deleted;
                }
                for (var i = 0; i < adminSubList.Count; i++)
                {
                    yunEntities.Entry(adminSubList[i]).State = EntityState.Deleted;
                }
                yunEntities.Entry(admin).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete,
                    "删除用户，用户名为" + admin.y_name + ",id：" + admin.id);
            }
            var j = yunEntities.SaveChanges();
            return j > 0 ? "ok" : "删除角色失败";
        }
    }
}