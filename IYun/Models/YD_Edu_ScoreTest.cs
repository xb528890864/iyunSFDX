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
    
    public partial class YD_Edu_ScoreTest
    {
        public int id { get; set; }
        public int y_stuId { get; set; }
        public int y_term { get; set; }
        public decimal y_normalScore { get; set; }
        public decimal y_termScore { get; set; }
        public Nullable<decimal> y_workScore { get; set; }
        public decimal y_totalScore { get; set; }
        public int y_courseId { get; set; }
        public int y_type { get; set; }
        public Nullable<System.DateTime> y_time { get; set; }
    
        public virtual YD_Edu_Course YD_Edu_Course { get; set; }
        public virtual YD_Sts_StuInfo YD_Sts_StuInfo { get; set; }
    }
}
