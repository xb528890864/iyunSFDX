using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IYun.Models;
using Newtonsoft.Json;

namespace IYun.Dal
{
    public class VW_AdminDal:BaseDal<VW_Admin>
    {
        public const string Y_ADMINNAME = "y_adminName";
        public const string Y_ROLENAME = "y_roleName";
        /// <summary>
        /// 对结果集合进行排序
        /// </summary>
        /// <param name="admins">要排序的集合</param>
        /// <param name="sort">排序字段名</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public IQueryable<VW_Admin> MulSortDbLogs(IQueryable<VW_Admin> admins, string sort, string order)
        {
            switch (sort)
            {
                case Y_ADMINNAME:
                    return order == "asc" ? admins.OrderBy(u => u.id) : admins.OrderByDescending(u => u.id);
                case Y_ROLENAME:
                    return order == "asc" ? admins.OrderBy(u => u.y_roleID) : admins.OrderByDescending(u => u.y_roleID);
                default:
                    return order == "asc" ? admins.OrderBy(u => u.id) : admins.OrderByDescending(u => u.id);
            }
        }
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public string AdminPageList(HttpRequestBase request, IYunEntities yunEntities)
        {
            #region “查询”权限验证
            var powerInfo = Safe("/SysAdmin/Admin", PowerFlag.Select);
            var info = new Hashtable();
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    info.Add("message","无权限");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.NoLogin:
                    info.Add("message","未登录");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.Unknow:
                    info.Add("message","未知错误");
                    return JsonConvert.SerializeObject(info);
                case PowerInfo.HasPower:
                    break;
                default:
                    info.Add("message","未知错误");
                    return JsonConvert.SerializeObject(info);
            }
            #endregion

            var row = Convert.ToInt32(request["rows"]);
            var page = Convert.ToInt32(request["page"]);
            var sort = request["sort"] ?? "id";
            var order = request["order"] ?? "desc";
            var rowskip = row * (page - 1);

            var admins = List(u => true, yunEntities);

            var sorts = sort.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var orders = order.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < sorts.Count(); i++)
            {
                admins = MulSortDbLogs(admins, sorts[i], orders[i]);
            }

            var tol = admins.Count();
            var list = new Hashtable { { "total", tol }, { "rows", admins.Skip(rowskip).Take(row) } };
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public string AdminList(IYunEntities yunEntities)
        {
            #region “查询”权限验证
            var powerInfo = Safe("/SysAdmin/Admin", PowerFlag.Select);
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    return "没有权限";
                case PowerInfo.NoLogin:
                    return "未登录";
                case PowerInfo.Unknow:
                    return "未知错误";
                case PowerInfo.HasPower:
                    break;
                default:
                    return "未知错误";
            }
            #endregion
            var admins =  List(u => true, yunEntities);
            return JsonConvert.SerializeObject(admins);
        }
    }
}