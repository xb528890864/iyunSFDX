using System.Collections.Generic;

namespace IYun.Models
{
    /// <summary>
    /// 栏目权限一级
    /// </summary>
    public class ModulePowerPar : YD_Sys_Module
    {
        public int y_menu { get; set; }
        public int y_insert { get; set; }
        public int y_delete { get; set; }
        public int y_update { get; set; }
        public int y_select { get; set; }

        public int y_menu_F { get; set; }
        public int y_insert_F { get; set; }
        public int y_delete_F { get; set; }
        public int y_update_F { get; set; }
        public int y_select_F { get; set; }
        public List<ModulePowerPar> children { get; set; }
    }
}
