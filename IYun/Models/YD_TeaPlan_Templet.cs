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
    
    public partial class YD_TeaPlan_Templet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public YD_TeaPlan_Templet()
        {
            this.YD_TeaPlan_Class = new HashSet<YD_TeaPlan_Class>();
            this.YD_TeaPlan_TempletCourseDes = new HashSet<YD_TeaPlan_TempletCourseDes>();
        }
    
        public int id { get; set; }
        public int y_majorId { get; set; }
        public int y_teaPlanType { get; set; }
        public string y_name { get; set; }
        public string y_remark { get; set; }
    
        public virtual YD_Edu_Major YD_Edu_Major { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_TeaPlan_Class> YD_TeaPlan_Class { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_TeaPlan_TempletCourseDes> YD_TeaPlan_TempletCourseDes { get; set; }
    }
}
