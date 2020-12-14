using System.Collections.Generic;

namespace IYun.Models
{
    /// <summary>
    /// 栏目权限实体类
    /// </summary>
    public class ModulePower : VW_Power
    {
        public List<ModulePower> children { get; set; }
    }
}
