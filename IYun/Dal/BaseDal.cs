using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using System.Web.Providers.Entities;
using IYun.Common;
using IYun.Models;

namespace IYun.Dal
{
    public class BaseDal<T> where T : class,new()
    {
        //public YD_Sys_Admin YdAdmin = new YD_Sys_Admin();


        public int YdAdminRoleId
        {
            get
            {
                if (HttpContext.Current.Session[KeyValue.Admin_Role_id] == null)
                {
                    HttpContext.Current.Session.RemoveAll();
                    HttpContext.Current.Response.Redirect("/AdminBase/Index");
                }

                return Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_Role_id]);

            }
        }

        public int YdAdminId
        {
            get
            {
                if (HttpContext.Current.Session[KeyValue.Admin_id] == null)
                {
                    HttpContext.Current.Session.RemoveAll();
                    HttpContext.Current.Response.Redirect("/AdminBase/Index");
                }

                return Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]);
            }
        }

        public string YdAdminRelName
        {
            get
            {
                if (HttpContext.Current.Session[KeyValue.AdminRelName] == null)
                {
                    HttpContext.Current.Session.RemoveAll();
                    HttpContext.Current.Response.Redirect("/AdminBase/Index");
                }

                return HttpContext.Current.Session[KeyValue.AdminRelName].ToString();
            }
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <param name="whereLambda">查找条件</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda, IYunEntities yunEntities)
        {
            var t = yunEntities.Set<T>().FirstOrDefault(whereLambda);
            return t;
        }

        /// <summary>
        /// 获取对象集合（默认排序）
        /// </summary>
        /// <param name="whereLambda">查找条件</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public IQueryable<T> List(Expression<Func<T, bool>> whereLambda, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().Where<T>(whereLambda);
            return list;
        }

        /// <summary>
        /// 获取指定页面的对象集合（默认排序）
        /// </summary>
        /// <param name="rows">每页大小</param>
        /// <param name="page">页码</param>
        /// <param name="totalCount">总数</param>
        /// <param name="whereLambda">查找条件</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public IQueryable<T> PageList(int rows, int page, out int totalCount, Expression<Func<T, bool>> whereLambda, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().Where<T>(whereLambda);
            var rowskip = rows * (page - 1);
            totalCount = list.Count();
            list = list.Skip(rowskip).Take(rows);
            return list;
        }

        /// <summary>
        /// 获取对象集合（单字段排序）
        /// </summary>
        /// <typeparam name="TS">排序字段类型</typeparam>
        /// <param name="whereLambda">查找条件</param>
        /// <param name="orderbyLambda">排序条件</param>
        /// <param name="isAsc">是否是正序</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public IQueryable<T> SingleSortList<TS>(Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, TS>> orderbyLambda, bool isAsc, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().Where<T>(whereLambda);
            list = isAsc ? list.OrderBy(orderbyLambda) : list.OrderByDescending(orderbyLambda);
            return list;
        }

        /// <summary>
        /// 获取指定页面的对象集合（单字段排序）
        /// </summary>
        /// <typeparam name="TS">排序字段类型</typeparam>
        /// <param name="rows">每页大小</param>
        /// <param name="page">页码</param>
        /// <param name="totalCount">总数</param>
        /// <param name="whereLambda">查找条件</param>
        /// <param name="orderbyLambda">排序条件</param>
        /// <param name="isAsc">是否是正序</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public IQueryable<T> SingleSortPageList<TS>(int rows, int page, out int totalCount, Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, TS>> orderbyLambda, bool isAsc, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().Where<T>(whereLambda);
            var rowskip = rows * (page - 1);
            totalCount = list.Count();
            list = isAsc ? list.OrderBy(orderbyLambda).Skip(rowskip).Take(rows) : list.OrderByDescending(orderbyLambda).Skip(rowskip).Take(rows);
            return list;
        }

        /// <summary>
        /// 获取对象集合（多字段排序）
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="parameters">sql语句中的参数</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public DbSqlQuery<T> MulSortList(string sql, object[] parameters, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().SqlQuery(sql, parameters);
            return list;
        }

        /// <summary>
        /// 获取指定页面的对象集合（多字段排序）
        /// </summary>
        /// <param name="rows">每页大小</param>
        /// <param name="page">页码</param>
        /// <param name="totalCount">总数</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="parameters">sql语句中的参数</param>
        /// <param name="yunEntities">上下文对象</param>
        /// <returns></returns>
        public IEnumerable<T> MulSortPageList(int rows, int page, out int totalCount, string sql, SqlParameter[] parameters, IYunEntities yunEntities)
        {
            var list = yunEntities.Set<T>().SqlQuery(sql, parameters);
            var rowskip = rows * (page - 1);
            totalCount = list.Count();
            return list.Skip(rowskip).Take(rows);
        }

        /// <summary>
        /// 新增一个实体对象
        /// </summary>
        /// <param name="entity">要新增的实体对象</param>
        /// <param name="moduleUrl">操作该方法的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        public ResultInfo BaseAddEntity(T entity, string moduleUrl, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe(moduleUrl, PowerFlag.Insert) };
            switch (resultInfo.Info)
            {
                case PowerInfo.NoPower:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有权限";
                    return resultInfo;
                case PowerInfo.NoLogin:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有登录";
                    return resultInfo;
                case PowerInfo.Unknow:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
                case PowerInfo.HasPower:
                    break;
                default:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
            }
            yunEntities.Set<T>().Add(entity);
            yunEntities.Configuration.AutoDetectChangesEnabled = false;
            yunEntities.Configuration.ValidateOnSaveEnabled = false;
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            return resultInfo;
        }

        /// <summary>
        /// 修改一个实体对象
        /// </summary>
        /// <param name="entity">要修改的实体对象</param>
        /// <param name="moduleUrl">操作该方法的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        public ResultInfo BaseEditEntity(T entity, string moduleUrl, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe(moduleUrl, PowerFlag.Update) };
            switch (resultInfo.Info)
            {
                case PowerInfo.NoPower:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有权限";
                    return resultInfo;
                case PowerInfo.NoLogin:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有登录";
                    return resultInfo;
                case PowerInfo.Unknow:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
                case PowerInfo.HasPower:
                    break;
                default:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
            }
            yunEntities.Entry(entity).State = EntityState.Modified;
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            return resultInfo;
        }

        /// <summary>
        /// 批量修改实体
        /// </summary>
        /// <param name="list">要修改的实体集合</param>
        /// <param name="moduleUrl">操作该方法的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        public ResultInfo BaseEditEntites(List<T> list, string moduleUrl, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe(moduleUrl, PowerFlag.Update) };
            switch (resultInfo.Info)
            {
                case PowerInfo.NoPower:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有权限";
                    return resultInfo;
                case PowerInfo.NoLogin:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有登录";
                    return resultInfo;
                case PowerInfo.Unknow:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
                case PowerInfo.HasPower:
                    break;
                default:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
            }
            foreach (var entity in list)
            {
                yunEntities.Entry(entity).State = EntityState.Modified;
            }
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            return resultInfo;
        }

        /// <summary>
        /// 删除一个实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        /// <param name="moduleUrl">操作该方法的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        public ResultInfo BaseDeleteEntity(T entity, string moduleUrl, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe(moduleUrl, PowerFlag.Delete) };
            switch (resultInfo.Info)
            {
                case PowerInfo.NoPower:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有权限";
                    return resultInfo;
                case PowerInfo.NoLogin:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有登录";
                    return resultInfo;
                case PowerInfo.Unknow:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
                case PowerInfo.HasPower:
                    break;
                default:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
            }
            yunEntities.Entry(entity).State = EntityState.Deleted;
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            return resultInfo;
        }

        /// <summary>
        /// 批量删除对象集合
        /// </summary>
        /// <param name="list">要删除的对象集合</param>
        /// <param name="moduleUrl">操作该方法的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        public ResultInfo BaseDeleteEntities(List<T> list, string moduleUrl, IYunEntities yunEntities)
        {
            var resultInfo = new ResultInfo { Info = Safe(moduleUrl, PowerFlag.Delete) };
            switch (resultInfo.Info)
            {
                case PowerInfo.NoPower:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有权限";
                    return resultInfo;
                case PowerInfo.NoLogin:
                    resultInfo.Success = false;
                    resultInfo.Message = "没有登录";
                    return resultInfo;
                case PowerInfo.Unknow:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
                case PowerInfo.HasPower:
                    break;
                default:
                    resultInfo.Success = false;
                    resultInfo.Message = "未知错误";
                    return resultInfo;
            }

            foreach (var entity in list)
            {
                yunEntities.Entry(entity).State = EntityState.Deleted;
            }
            resultInfo.Success = yunEntities.SaveChanges() > 0;
            resultInfo.Message = "";
            return resultInfo;
        }
        /// <summary>
        /// 权限验证方法,返回权限验证信息
        /// </summary>
        /// <param name="action">需要验证的栏目url（例：/SysAdmin/Site）</param>
        /// <param name="flag">需要验证的栏目权限（枚举）</param>
        /// <returns>返回权限验证信息（枚举）</returns>
        public PowerInfo Safe(string action, PowerFlag flag)
        {
            if (!IsLogin()) return PowerInfo.NoLogin;
            if (action == null) return PowerInfo.Unknow;
            var module = new YD_Sys_Module();
            //带有参数的链接将参数去掉
            if (action.Contains("/?"))
            {
                action = action.Substring(0, action.IndexOf("/?"));
            }
            action = action.EndsWith("/") ? action.Substring(0, action.Length - 1) : action;

            using (var yunEntities = new IYunEntities())
            {
                var modules = yunEntities.YD_Sys_Module.Where(u => true).ToList();
                for (var i = 0; i < modules.Count(); i++)
                {
                    var moduleName = modules[i].y_url;
                    //带有参数的链接将参数去掉
                    if (moduleName.Contains("/?"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.IndexOf("/?"));
                    }
                    //带有/结尾的链接将/去掉
                    if (moduleName.EndsWith("/"))
                    {
                        moduleName = moduleName.Substring(0, moduleName.Length - 1);
                    }
                    if (moduleName.ToLower() != action.ToLower()) continue;
                    module = modules[i];
                    break;
                }

                if (module == null) return PowerInfo.Unknow;
                var power = yunEntities.YD_Sys_Power.FirstOrDefault(u => u.y_moduleID == module.id && u.y_roleID == YdAdminRoleId);
                if (power == null) return PowerInfo.NoPower;
                switch (flag)
                {
                    case PowerFlag.Delete:
                        return power.y_delete == (int)PowerState.Able ? PowerInfo.HasPower : PowerInfo.NoPower;
                    case PowerFlag.Insert:
                        return power.y_insert == (int)PowerState.Able ? PowerInfo.HasPower : PowerInfo.NoPower;
                    case PowerFlag.Menu:
                        return power.y_menu == (int)PowerState.Able ? PowerInfo.HasPower : PowerInfo.NoPower;
                    case PowerFlag.Select:
                        return power.y_select == (int)PowerState.Able ? PowerInfo.HasPower : PowerInfo.NoPower;
                    case PowerFlag.Update:
                        return power.y_update == (int)PowerState.Able ? PowerInfo.HasPower : PowerInfo.NoPower;
                    default:
                        return PowerInfo.Unknow;
                }
            }
        }
        /// <summary>
        /// 检测用户是否已经登录
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            if (HttpContext.Current.Session[KeyValue.Admin_LoginFlag] == null) return false;
            if (HttpContext.Current.Session[KeyValue.Admin_LoginFlag].ToString() != KeyValue.Admin_LoginOKFlag) return false;
            
            return true;
        }
       
        
       
        /// <summary>
        /// 后台用户退出后台登录
        /// </summary>
        /// <returns></returns>
        public void Exit()
        {
            //LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]), HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.LoginOut, "正常注销登录。");


            //HttpContext.Current.Session[KeyValue.Admin_LoginFlag] = "";
            //HttpContext.Current.Session[KeyValue.Admin_Name] = "";
            //HttpContext.Current.Session[KeyValue.Admin_id] = "";

            HttpContext.Current.Session.Clear();
        }
        /// <summary>
        /// 后台用户退出后台登录
        /// </summary>
        /// <returns></returns>
        public void UserExit()
        {
            //HttpContext.Current.Session[KeyValue.User_LoginFlag] = "";
            //HttpContext.Current.Session[KeyValue.User_Name] = "";
            //HttpContext.Current.Session[KeyValue.User_id] = "";

            HttpContext.Current.Session.Clear();
        }
    }
}