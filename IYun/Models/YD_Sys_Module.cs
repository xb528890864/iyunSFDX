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
    
    public partial class YD_Sys_Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public YD_Sys_Module()
        {
            this.YD_Sys_Power = new HashSet<YD_Sys_Power>();
        }
    
        public int id { get; set; }
        public string y_name { get; set; }
        public string y_url { get; set; }
        public int y_parentID { get; set; }
        public int y_level { get; set; }
        public int y_sort { get; set; }
        public int y_vaild { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Sys_Power> YD_Sys_Power { get; set; }
    }
}