using System;
using System.Collections;
using System.Linq;
using System.Web;
using IYun.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IYun.Dal
{
    public class VW_DbLogDal : BaseDal<VW_DbLog>
    {
        public const string y_userName = "y_userName";
        public const string y_adminName = "y_adminName";
        public const string y_time = "y_time";
        public const string Y_LOGTYPE = "Y_logType";

        /// <summary>
        /// 对结果集合进行排序
        /// </summary>
        /// <param name="dbLogs">要排序的集合</param>
        /// <param name="sort">排序字段名</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public IQueryable<VW_DbLog> MulSortDbLogs(IQueryable<VW_DbLog> dbLogs, string sort, string order)
        {
            switch (sort)
            {
                case y_adminName:
                    return order == "asc" ? dbLogs.OrderBy(u => u.y_adminName) : dbLogs.OrderByDescending(u => u.y_adminName);
                case y_userName:
                    return order == "asc" ? dbLogs.OrderBy(u => u.y_userID) : dbLogs.OrderByDescending(u => u.y_userID);
                case Y_LOGTYPE:
                    return order == "asc" ? dbLogs.OrderBy(u => u.y_logTypeID) : dbLogs.OrderByDescending(u => u.y_logTypeID);
                case y_time:
                    return order == "asc" ? dbLogs.OrderBy(u => u.y_time) : dbLogs.OrderByDescending(u => u.y_time);
                default:
                    return order == "asc" ? dbLogs.OrderBy(u => u.id) : dbLogs.OrderByDescending(u => u.id);
            }
        }

        /// <summary>
        /// 获得日志分页数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public string DbLogPageList(HttpRequestBase request, IYunEntities yunEntities)
        {
            var powerInfo = Safe("/SysAdmin/Dblog", PowerFlag.Select);
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
           
            var row = Convert.ToInt32(request["rows"]);
            var page = Convert.ToInt32(request["page"]);
            var dateStart = request["dateStart"];
            var dateEnd = request["dateEnd"];
            var dbLogType = request["DbLogType"];
            var operAdmin = request["OperAdmin"];
            var sort = request["sort"] ?? "id";
            var order = request["order"] ?? "desc";
            var rowskip = row * (page - 1);

            var dblogs = List(u => true, yunEntities);

           
            if (!string.IsNullOrWhiteSpace(dbLogType))
            {
                var logTypeId = Convert.ToInt32(dbLogType);
                dblogs = dblogs.Where(u => u.y_logTypeID == logTypeId);
            }
            if (!string.IsNullOrWhiteSpace(operAdmin))
            {
                var userId = Convert.ToInt32(operAdmin);
                dblogs = dblogs.Where(u => u.y_userID == userId);
            }
            if (!string.IsNullOrWhiteSpace(dateStart))
            {
                var dateStartt = Convert.ToDateTime(dateStart);
                dblogs = dblogs.Where(u => u.y_time >= dateStartt);
            }
            if (!string.IsNullOrWhiteSpace(dateEnd))
            {
                var dateEndt = Convert.ToDateTime(dateEnd);
                dblogs = dblogs.Where(u => u.y_time <= dateEndt);
            }
            //排序
            var sorts = sort.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var orders = order.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < sorts.Count(); i++)
            {
                dblogs = MulSortDbLogs(dblogs, sorts[i], orders[i]);
            }
            var tol = dblogs.Count();
            var list = new Hashtable { { "total", tol }, { "rows", dblogs.Skip(rowskip).Take(row) } };
            var dt = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd' 'hh':'mm':'ss" };
            return JsonConvert.SerializeObject(list, dt);
        }
    }
}