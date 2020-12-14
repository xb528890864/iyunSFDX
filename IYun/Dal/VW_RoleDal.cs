using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;
using Newtonsoft.Json;

namespace IYun.Dal
{
    public partial class  YD_Sys_RoleDal 
    {
        public const string Y_ROLENAME = "y_roleName";
        public const string Y_SITENAME = "y_siteName";
        /// <summary>
        /// 对结果集合进行排序
        /// </summary>
        /// <param name="roles">要排序的集合</param>
        /// <param name="sort">排序字段名</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public IQueryable<YD_Sys_Role> MulSortDbLogs(IQueryable<YD_Sys_Role> roles, string sort, string order)
        {
            switch (sort)
            {
                case Y_ROLENAME:
                    return order == "asc" ? roles.OrderBy(u => u.id) : roles.OrderByDescending(u => u.id);
               
                default:
                    return order == "asc" ? roles.OrderBy(u => u.id) : roles.OrderByDescending(u => u.id);
            }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public string RolePageList(HttpRequestBase request, IYunEntities yunEntities)
        {
            #region “查询”权限验证
            var powerInfo = Safe("/SysAdmin/Role", PowerFlag.Select);
            var info = new Hashtable();
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    info.Add("message", "无权限");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.NoLogin:
                    info.Add("message", "未登录");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.Unknow:
                    info.Add("message", "未知错误");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.HasPower:
                    break;
                default:
                    info.Add("message", "未知错误");
                    return JsonConvert.SerializeObject(info);
            }
            #endregion
            var row = Convert.ToInt32(request["rows"]);
            var page = Convert.ToInt32(request["page"]);
            var sort = request["sort"] ?? "id";
            var order = request["order"] ?? "desc";
            var rowskip = row * (page - 1);

            var roles = List(u =>true, yunEntities);

            var sorts = sort.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var orders = order.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < sorts.Count(); i++)
            {
                roles = MulSortDbLogs(roles, sorts[i], orders[i]);
            }

            var tol = roles.Count();
            var list = new Hashtable { { "total", tol }, { "rows", roles.Skip(rowskip).Take(row) } };
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public string RoleList(HttpRequestBase request, IYunEntities yunEntities)
        {
            #region “查询”权限验证
            var powerInfo = Safe("/SysAdmin/Role", PowerFlag.Select);
            var info = new Hashtable();
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    info.Add("message", "无权限");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.NoLogin:
                    info.Add("message", "未登录");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.Unknow:
                    info.Add("message", "未知错误");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.HasPower:
                    break;
                default:
                    info.Add("message", "未知错误");
                    return JsonConvert.SerializeObject(info);
            }
            #endregion

            
            var roles = List(u => true, yunEntities);
            return JsonConvert.SerializeObject(roles);
        }

    }
}