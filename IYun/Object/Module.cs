using System.Collections.Generic;

namespace IYun.Models
{
    /// <summary>
    /// Module的实体类
    /// </summary>
    public class Module:YD_Sys_Module
    {
        public List<Module> children { get; set; }
    }
}