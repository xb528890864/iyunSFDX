using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IYun.Common;
using IYun.Models;
using Newtonsoft.Json;

namespace IYun.Dal
{
    /// <summary>
    /// Module对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    public delegate ResultInfo OperModule(YD_Sys_Module module, IYunEntities yunEntities);
    public class YD_Sys_ModuleDal : BaseDal<YD_Sys_Module>
    {
        /// <summary>
        /// 增加方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要新增的栏目对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(module, "/SysAdmin/Module", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert, "添加栏目,栏目名为" + module.y_name + ",级别为" + module.y_level + ",id：" + module.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperModule类型
        /// </summary>
        /// <param name="module">要修改的栏目对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(module, "/SysAdmin/Module", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改栏目,栏目名为" + module.y_name + "级别为" + module.y_level + ",id：" + module.id);
            }
            return resultInfo;
        }
        /// <summary>
        /// 移动栏目,委托：OperModule类型
        /// </summary>
        /// <param name="module">要栏目的栏目对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo MoveOper(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe("/SysAdmin/Module", PowerFlag.Update) };
            if (module.y_parentID == module.id)
            {
                resultInfo.Success = false;
                resultInfo.Message = "自身不能成为自身的子栏目";
                return resultInfo;
            }
            var moduleLast = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == module.y_parentID);
            if (IsChildrenModule(module, moduleLast, yunEntities))
            {
                resultInfo.Success = false;
                resultInfo.Message = "自身不能成为自身子栏目的子栏目";
                return resultInfo;
            }
            MoveOperChildrenModule(module, yunEntities);
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "移动栏目,栏目名为" + module.y_name + "级别为" + module.y_level + ",移动到parentID：" + module.y_parentID + ",id：" + module.id);
            }
            return resultInfo;
        }
        /// <summary>
        /// 递归移动栏目
        /// </summary>
        /// <param name="module">要移动的栏目对象</param>
        /// <param name="yunEntities"></param>
        private static void MoveOperChildrenModule(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var parentModule = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == module.y_parentID);
            if (parentModule != null) module.y_level = parentModule.y_level + 1;
            if (parentModule == null && module.y_parentID == 0) module.y_level = 1;
            yunEntities.Entry(module).State = EntityState.Modified;
            var childrenModule = yunEntities.YD_Sys_Module.Where(u => u.y_parentID == module.id).ToList();
            for (var i = 0; i < childrenModule.Count(); i++)
            {
                MoveOperChildrenModule(childrenModule[i], yunEntities);
            }
        }
        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="module">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        private static string Oper(YD_Sys_Module module, OperModule curd, IYunEntities yunEntities)
        {
            if (yunEntities.YD_Sys_Module.FirstOrDefault(u => u.y_name == module.y_name && u.id != module.id&&u.y_parentID==module.y_parentID) != null)
            {
                return "已存在相同栏目名的栏目";
            }
            var resultInfo = curd(module, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }

        /// <summary>
        /// 获取栏目列表
        /// </summary>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public string ModuleList(IYunEntities yunEntities)
        {
            var powerInfo = Safe("/SysAdmin/Module", PowerFlag.Select);
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
            yunEntities.Configuration.ProxyCreationEnabled = false;
            var parentList = yunEntities.YD_Sys_Module.Where(u => u.y_level == 1).OrderByDescending(u => u.y_sort).ToList();
            var modules = parentList.Select(ydModule => new Module
            {
                id = ydModule.id,
                y_url = ydModule.y_url,
                y_sort = ydModule.y_sort,
                y_parentID = ydModule.y_parentID,
                y_name = ydModule.y_name,
                y_level = ydModule.y_level,
                y_vaild = ydModule.y_vaild,
                children = GetChildrenModule(ydModule, yunEntities)
            }).ToList();
            return JsonConvert.SerializeObject(modules);
        }
        /// <summary>
        /// 递归获取子栏目
        /// </summary>
        /// <param name="module">栏目对象</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public List<Module> GetChildrenModule(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var moduleSons = yunEntities.YD_Sys_Module.Where(u => u.y_parentID == module.id).OrderByDescending(u => u.y_sort);
            var children = new List<Module>();
            foreach (var moduleSon in moduleSons)
            {
                var moduleChildren = new Module
                {
                    id = moduleSon.id,
                    y_url = moduleSon.y_url,
                    y_sort = moduleSon.y_sort,
                    y_parentID = moduleSon.y_parentID,
                    y_name = moduleSon.y_name,
                    y_level = moduleSon.y_level,
                    y_vaild = moduleSon.y_vaild
                };
                var moduleSonSon = GetChildrenModule(moduleSon, yunEntities);
                moduleChildren.children = moduleSonSon;
                children.Add(moduleChildren);
            }
            return children;
        }


        /// <summary>
        /// 获取下拉树栏目列表
        /// </summary>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public JsonResult ModuleDropList(IYunEntities yunEntities)
        {
            var res = new JsonResult();
            var parentList = yunEntities.YD_Sys_Module.Where(u => u.y_level == 1).OrderByDescending(u => u.y_sort).ToList();

            var combortrees = new List<Combortree>
            {
                new Combortree {id = 0, text = "顶级栏目"}
            };

            var modules = parentList.Select(ydModule => new Combortree
            {
                id = ydModule.id,
                text = ydModule.y_name,
                children = GetChildrenDropModule(ydModule, yunEntities)
            }).ToList();
            combortrees.AddRange(modules);
            res.Data = combortrees;
            return res;
        }
        /// <summary>
        /// 递归获取下拉树子栏目
        /// </summary>
        /// <param name="module">栏目对象</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public List<Combortree> GetChildrenDropModule(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var moduleSons = yunEntities.YD_Sys_Module.Where(u => u.y_parentID == module.id).OrderByDescending(u => u.y_sort);
            var children = new List<Combortree>();
            foreach (var moduleSon in moduleSons)
            {
                var moduleChildren = new Combortree
                {
                    id = moduleSon.id,
                    text = moduleSon.y_name
                };
                var moduleSonSon = GetChildrenDropModule(moduleSon, yunEntities);
                moduleChildren.children = moduleSonSon;
                children.Add(moduleChildren);
            }
            return children;
        }

        /// <summary>
        /// 获取栏目Id集合
        /// </summary>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public JsonResult ModuleIdList(IYunEntities yunEntities)
        {
            var res = new JsonResult();
            var moduleIds = yunEntities.YD_Sys_Module.Select(u => new { u.id }).Where(u => true).ToList();
            res.Data = moduleIds;
            return res;
        }
        /// <summary>
        /// 编辑栏目
        /// </summary>
        /// <param name="module">栏目对象</param>
        /// <param name="request">请求上下文对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string EditModule(YD_Sys_Module module, HttpRequestBase request, IYunEntities yunEntities)
        {
            var isExit = yunEntities.YD_Sys_Module.Any(u => u.id == module.id && u.y_parentID == module.y_parentID);
            if (!isExit)
            {
                if (!string.IsNullOrEmpty(request["y_vaild"])) return Oper(module, MoveOper, yunEntities);
                module.y_vaild = (Int32)DataState.Disable;
                return Oper(module, MoveOper, yunEntities);
            }
            if (!string.IsNullOrEmpty(request["y_vaild"])) return Oper(module, EditOper, yunEntities);
            if (module != null) module.y_vaild = (Int32)DataState.Disable;
            return Oper(module, EditOper, yunEntities);
        }
        /// <summary>
        /// 增加栏目
        /// </summary>
        /// <param name="module">栏目对象</param>
        /// <param name="request">请求上下文对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string AddModule(YD_Sys_Module module, HttpRequestBase request, IYunEntities yunEntities)
        {
            var modulePar = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == module.y_parentID);
            if (modulePar == null)
            {
                module.y_level = 1;
            }
            else
            {
                module.y_level = modulePar.y_level + 1;
            }
            if (string.IsNullOrEmpty(request["y_vaild"]))
            {
                module.y_vaild = (int)DataState.Able;
            }
            var str = Oper(module, AddOper, yunEntities);
            return str;
        }
        /// <summary>
        /// 移动栏目
        /// </summary>
        /// <param name="request">请求上下文对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string MoveModule(HttpRequestBase request, IYunEntities yunEntities)
        {
            var moduleNowStr = request["moduleNow"];
            var moduleLastStr = request["moduleLast"];
            if (string.IsNullOrEmpty(moduleLastStr) || string.IsNullOrEmpty(moduleNowStr))
            {
                return "请先选择栏目";
            }
            if (moduleLastStr == moduleNowStr)
            {
                return "自身不能成为自身的子栏目";
            }
            var moduleNowId = Convert.ToInt32(moduleNowStr);
            var moduleLastId = Convert.ToInt32(moduleLastStr);
            var moduleNow = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == moduleNowId);
            var moduleLast = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == moduleLastId);
            if (moduleNow == null) return "栏目为空";

            if (IsChildrenModule(moduleNow, moduleLast, yunEntities))
            {
                return "自身不能成为自身子栏目的子栏目";
            }
            moduleNow.y_parentID = moduleLastId == 0 ? 0 : moduleLast.id;
            return Oper(moduleNow, MoveOper, yunEntities);
        }

        /// <summary>
        /// 判断栏目是否是指定栏目的子栏目
        /// </summary>
        /// <param name="moduleNow">栏目</param>
        /// <param name="moduleLast">要判断是否是子栏目的栏目</param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        public bool IsChildrenModule(YD_Sys_Module moduleNow, YD_Sys_Module moduleLast, IYunEntities yunEntities)
        {
            while (true)
            {
                if (moduleLast == null || moduleLast.y_parentID == 0) return false;
                if (moduleLast.y_parentID == moduleNow.id)
                {
                    return true;
                }
                moduleLast = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == moduleLast.y_parentID);
            }
        }

        /// <summary>
        /// 修改栏目排序
        /// </summary>
        /// <param name="request">请求上下文对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string ModuleUpdateSort(HttpRequestBase request, IYunEntities yunEntities)
        {
            var ids = request["ids"];
            var sorts = request["sorts"];
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(sorts))
            {
                return "未知错误";
            }
            var mids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var msorts = sorts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (mids.Count() != msorts.Count())
            {
                return "未知错误";
            }
            for (var j = 0; j < mids.Count(); j++)
            {
                var id = Convert.ToInt32(mids[j]);
                var module = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == id);
                if (module != null)
                {
                    module.y_sort = Convert.ToInt32(msorts[j]);
                }
                yunEntities.Entry(module).State = EntityState.Modified;
                if (module != null)
                    LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "修改栏目排序，栏目名为" + module.y_name + ",级别为" + module.y_level + ",排序修改为：" + module.y_sort + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();
            return i > 0 ? "ok" : "保存栏目排序失败";
        }
        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="id">栏目id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string ModuleDelete(int id, IYunEntities yunEntities)
        {
            if (id ==10||id==11)
            {
                return "该栏目为必要栏目，不能删除";
            }
            var module = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == id);
            //删除对应的权限记录
            if (module != null)
            {
                DeleteModule(module, yunEntities);
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete, "删除栏目，栏目名为" + module.y_name + ",级别为" + module.y_level + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();

            return i > 0 ? "ok" : "删除栏目失败";
        }
        /// <summary>
        /// 递归删除栏目并删除与之对应的权限
        /// </summary>
        /// <param name="module">栏目对象</param>
        /// <param name="yunEntities">EF上下文对象</param>
        public void DeleteModule(YD_Sys_Module module, IYunEntities yunEntities)
        {
            var powerList = yunEntities.YD_Sys_Power.Where(u => u.y_moduleID == module.id);
            foreach (var ydPower in powerList)
            {
                yunEntities.Entry(ydPower).State = EntityState.Deleted;
            }
            yunEntities.Entry(module).State = EntityState.Deleted;
            var moduleSons = yunEntities.YD_Sys_Module.Where(u => u.y_parentID == module.id);
            foreach (var moduleSon in moduleSons)
            {
                DeleteModule(moduleSon, yunEntities);
            }
        }
        /// <summary>
        /// 关闭栏目
        /// </summary>
        /// <param name="id">栏目id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string ModuleClose(int id, IYunEntities yunEntities)
        {
            if (id < 34)
            {
                return "该栏目为必要栏目，不能关闭";
            }

            var module = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == id);
            if (module != null)
            {
                module.y_vaild = (Int32)DataState.Disable;
                yunEntities.Entry(module).State = EntityState.Modified;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "关闭栏目，栏目名为" + module.y_name + ",级别为" + module.y_level + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();
            return i > 0 ? "ok" : "关闭栏目失败";

        }
        /// <summary>
        /// 开启栏目
        /// </summary>
        /// <param name="id">栏目id</param>
        /// <param name="yunEntities">EF上下文对象</param>
        /// <returns></returns>
        public string ModuleOpen(int id, IYunEntities yunEntities)
        {
            var module = yunEntities.YD_Sys_Module.FirstOrDefault(u => u.id == id);
            if (module != null)
            {
                module.y_vaild = (Int32)DataState.Able;
                yunEntities.Entry(module).State = EntityState.Modified;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update, "开启栏目，栏目名为" + module.y_name + ",级别为" + module.y_level + ",id：" + module.id);
            }
            var i = yunEntities.SaveChanges();
            return i > 0 ? "ok" : "开启栏目失败";
        }



    }
}