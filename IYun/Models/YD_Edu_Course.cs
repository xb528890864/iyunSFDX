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
    
    public partial class YD_Edu_Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public YD_Edu_Course()
        {
            this.YD_Edu_Score = new HashSet<YD_Edu_Score>();
            this.YD_Edu_ScoreTest = new HashSet<YD_Edu_ScoreTest>();
            this.YD_Edu_SmallPower = new HashSet<YD_Edu_SmallPower>();
            this.YD_Edu_TeachplanDes = new HashSet<YD_Edu_TeachplanDes>();
            this.YD_Graduate_SampleExamScore = new HashSet<YD_Graduate_SampleExamScore>();
            this.YD_Sys_AdminCourseLink = new HashSet<YD_Sys_AdminCourseLink>();
            this.YD_TeaPlan_ClassCourseDes = new HashSet<YD_TeaPlan_ClassCourseDes>();
            this.YD_TeaPlan_TempletCourseDes = new HashSet<YD_TeaPlan_TempletCourseDes>();
        }
    
        public int id { get; set; }
        public string y_code { get; set; }
        public string y_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_Score> YD_Edu_Score { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_ScoreTest> YD_Edu_ScoreTest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_SmallPower> YD_Edu_SmallPower { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_TeachplanDes> YD_Edu_TeachplanDes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Graduate_SampleExamScore> YD_Graduate_SampleExamScore { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Sys_AdminCourseLink> YD_Sys_AdminCourseLink { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_TeaPlan_ClassCourseDes> YD_TeaPlan_ClassCourseDes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_TeaPlan_TempletCourseDes> YD_TeaPlan_TempletCourseDes { get; set; }
    }
}
