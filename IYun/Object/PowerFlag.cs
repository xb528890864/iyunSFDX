using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IYun.Models
{
    /// <summary>
    /// 权限分类标记
    /// </summary>
    public enum PowerFlag
    {
        /// <summary>
        /// 菜单权限
        /// </summary>
        Menu = 0,
        /// <summary>
        /// 新增权限
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 删除权限
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 查找权限
        /// </summary>
        Select = 3,
        /// <summary>
        /// 修改权限
        /// </summary>
        Update = 4
    }
}