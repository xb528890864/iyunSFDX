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
    
    public partial class YD_Edu_ForeignLanguage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public YD_Edu_ForeignLanguage()
        {
            this.YD_Sts_StuInfo_copy1 = new HashSet<YD_Sts_StuInfo_copy1>();
            this.YD_Sts_StuInfo = new HashSet<YD_Sts_StuInfo>();
        }
    
        public int id { get; set; }
        public string y_code { get; set; }
        public string y_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Sts_StuInfo_copy1> YD_Sts_StuInfo_copy1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Sts_StuInfo> YD_Sts_StuInfo { get; set; }
    }
}
