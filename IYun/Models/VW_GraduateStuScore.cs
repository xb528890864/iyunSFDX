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
    
    public partial class VW_GraduateStuScore
    {
        public int id { get; set; }
        public System.DateTime y_examtime { get; set; }
        public string y_admissionNum { get; set; }
        public string y_stuname { get; set; }
        public int y_inYear { get; set; }
        public string y_cardId { get; set; }
        public int y_subjectivitysore { get; set; }
        public int y_sumsore { get; set; }
        public string y_explain { get; set; }
        public int y_verdict { get; set; }
        public string y_remark { get; set; }
        public string majorName { get; set; }
        public int y_subSchoolId { get; set; }
        public int y_majorLibId { get; set; }
        public string subschoolname { get; set; }
        public int y_sex { get; set; }
        public Nullable<int> y_isdel { get; set; }
        public Nullable<int> y_stuId { get; set; }
    }
}