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
    
    public partial class VW_AllBili
    {
        public int id { get; set; }
        public int y_inYear { get; set; }
        public int y_feeYear { get; set; }
        public int y_stuTypeId { get; set; }
        public int y_eduTypeId { get; set; }
        public int y_bili { get; set; }
        public string eduTypeName { get; set; }
        public string y_eduTypeCode { get; set; }
        public string stuTypeName { get; set; }
        public string y_stuTypeCode { get; set; }
        public Nullable<System.DateTime> y_time { get; set; }
        public bool y_Visible { get; set; }
    }
}