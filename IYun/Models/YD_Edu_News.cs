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
    
    public partial class YD_Edu_News
    {
        public int id { get; set; }
        public Nullable<int> y_type { get; set; }
        public Nullable<int> y_subSchoolId { get; set; }
        public string y_title { get; set; }
        public string y_content { get; set; }
        public string y_appyname { get; set; }
        public System.DateTime y_appytime { get; set; }
        public Nullable<int> y_hits { get; set; }
        public Nullable<int> y_usable { get; set; }
    }
}
