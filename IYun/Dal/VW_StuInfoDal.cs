using System;
using System.Collections;
using System.Linq;
using System.Web;
using IYun.Models;
using Newtonsoft.Json;

namespace IYun.Dal
{
    public class VW_StuInfoDal : BaseDal<VW_StuInfo>
    {
        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public string StuInfoPageList(HttpRequestBase request, IYunEntities yunEntities)
        {
            //#region “查询”权限验证
            //var powerInfo = Safe("/SysAdmin/Admin", PowerFlag.Select);
            //var info = new Hashtable();
            //switch (powerInfo)
            //{
            //    case PowerInfo.NoPower:
            //        info.Add("message", "无权限");
            //        return JsonConvert.SerializeObject(info);
            //    case PowerInfo.NoLogin:
            //        info.Add("message", "未登录");
            //        return JsonConvert.SerializeObject(info);
            //    case PowerInfo.Unknow:
            //        info.Add("message", "未知错误");
            //        return JsonConvert.SerializeObject(info);
            //    case PowerInfo.HasPower:
            //        break;
            //    default:
            //        info.Add("message", "未知错误");
            //        return JsonConvert.SerializeObject(info);
            //}
            //#endregion

            var row = Convert.ToInt32(request["rows"]);
            var page = Convert.ToInt32(request["page"]);
           
            var rowskip = row * (page - 1);
            var admins = List(u => true, yunEntities);
            var tol = admins.Count();
            var list = new Hashtable { { "total", tol }, { "rows", admins.Skip(rowskip).Take(row) } };
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <returns></returns>
        public string StuInfoList(IYunEntities yunEntities)
        {
            //#region “查询”权限验证
            //var powerInfo = Safe("/SysAdmin/Admin", PowerFlag.Select);
            //switch (powerInfo)
            //{
            //    case PowerInfo.NoPower:
            //        return "没有权限";
            //    case PowerInfo.NoLogin:
            //        return "未登录";
            //    case PowerInfo.Unknow:
            //        return "未知错误";
            //    case PowerInfo.HasPower:
            //        break;
            //    default:
            //        return "未知错误";
            //}
            //#endregion
            var admins = List(u => true, yunEntities);
            return JsonConvert.SerializeObject(admins);
        }
    }
}