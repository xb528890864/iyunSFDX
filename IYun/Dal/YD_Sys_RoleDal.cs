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
    /// Role对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <returns></returns>
    public delegate ResultInfo OperRole(YD_Sys_Role role, IYunEntities yunEntities);
    public partial class YD_Sys_RoleDal : BaseDal<YD_Sys_Role>
    {
        
        /// <summary>
        /// 增加方法,委托：OperRole类型
        /// </summary>
        /// <param name="role">要新增的角色对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sys_Role role, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(role, "/SysAdmin/Role", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "新增角色,角色名为" + role.y_name + ",id:" + role.id);
            }
            return resultInfo;
        }
        /// <summary>
        /// 修改方法,委托：OperRole类型
        /// </summary>
        /// <param name="role">要修改的角色对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sys_Role role, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(role, "/SysAdmin/Role", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改角色名，修改为" + role.y_name + ",id：" + role.id);
            }
            return resultInfo;
        }
        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="role">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        private static string Oper(YD_Sys_Role role, OperRole curd, IYunEntities yunEntities)
        {
            if (role.y_name.Length > 20)
            {
                return "角色名字符长度超过20，不符合规定";
            }

            if (yunEntities.YD_Sys_Role.FirstOrDefault(u => u.y_name == role.y_name && u.id != role.id) != null)
            {
                return "已存在相同角色名的角色";
            }
            var resultInfo = curd(role, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }
       
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <returns></returns>
        public string EditRole(YD_Sys_Role role, IYunEntities yunEntities)
        {
            return Oper(role, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加角色
        /// </summary>
        /// <returns></returns>
        public string AddRole(YD_Sys_Role role, IYunEntities yunEntities)
        {
            return Oper(role, AddOper, yunEntities);
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        public string DeleteRole(HttpRequestBase request, IYunEntities yunEntities)
        {
            var ids = request["ids"];
            if (string.IsNullOrEmpty(ids))
            {
                return "数据为空";
            }
            #region “删除”权限验证
            var powerInfo = Safe("/SysAdmin/Role", PowerFlag.Delete);
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

            var roles = yunEntities.YD_Sys_Role.Where(u => idsList.Contains(u.id)).ToList();

            foreach (var role in roles)
            {
                if (yunEntities.YD_Sys_Admin.FirstOrDefault(u => u.y_roleID == role.id) != null)
                {
                    return "删除角色失败，因为该角色下已经存在用户";
                }
                yunEntities.Entry(role).State = EntityState.Deleted;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除角色，角色名为" + role.y_name + ",id：" + role.id);
            }
            var j = yunEntities.SaveChanges();
            return j > 0 ? "ok" : "删除角色失败";
        }

    }
}