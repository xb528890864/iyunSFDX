//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace IYun.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class YD_Sys_AdminSubLink
    {
        public int id { get; set; }
        public int y_adminId { get; set; }
        public int y_subSchoolId { get; set; }
    
        public virtual YD_Sys_Admin YD_Sys_Admin { get; set; }
    }
}
