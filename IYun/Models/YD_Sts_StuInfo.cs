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
    
    public partial class YD_Sts_StuInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public YD_Sts_StuInfo()
        {
            this.YD_Edu_Score = new HashSet<YD_Edu_Score>();
            this.YD_Edu_ScoreTest = new HashSet<YD_Edu_ScoreTest>();
            this.YD_Fee_StuFeeTb = new HashSet<YD_Fee_StuFeeTb>();
            this.YD_Graduate_StudentScore = new HashSet<YD_Graduate_StudentScore>();
            this.YD_Sts_StuStrange1 = new HashSet<YD_Sts_StuStrange>();
        }
    
        public int id { get; set; }
        public string y_name { get; set; }
        public string y_stuNum { get; set; }
        public string y_examNum { get; set; }
        public int y_sex { get; set; }
        public int y_inYear { get; set; }
        public string y_cardId { get; set; }
        public int y_majorId { get; set; }
        public Nullable<int> y_subSchoolId { get; set; }
        public int y_stuStateId { get; set; }
        public string y_img { get; set; }
        public string y_tel { get; set; }
        public string y_mail { get; set; }
        public string y_address { get; set; }
        public string y_registerState { get; set; }
        public int y_isChangePlan { get; set; }
        public Nullable<int> y_changePlanId { get; set; }
        public string y_loginName { get; set; }
        public string y_password { get; set; }
        public Nullable<int> y_stuStrange { get; set; }
        public int y_isdel { get; set; }
        public Nullable<int> y_nationId { get; set; }
        public Nullable<int> y_politicsId { get; set; }
        public System.DateTime y_birthday { get; set; }
        public Nullable<System.DateTime> y_graduationdata { get; set; }
        public string y_graduationschool { get; set; }
        public string y_registrationNum { get; set; }
        public string y_postalcode { get; set; }
        public Nullable<int> y_foreignLanguageId { get; set; }
        public Nullable<int> y_recruitTypeId { get; set; }
        public Nullable<int> y_professionTypeId { get; set; }
        public Nullable<int> y_cultureExtentId { get; set; }
        public Nullable<int> y_examFeatureId { get; set; }
        public Nullable<int> y_isMoneyOk { get; set; }
        public Nullable<int> y_applyOK { get; set; }
        public Nullable<int> y_ischeck { get; set; }
        public Nullable<int> y_degreeOK { get; set; }
        public Nullable<int> y_realYear { get; set; }
        public Nullable<int> y_studentType { get; set; }
        public Nullable<bool> y_isgraduate { get; set; }
        public Nullable<int> y_isPay { get; set; }
        public Nullable<bool> isCheckForSchool { get; set; }
        public string y_graduateNumber { get; set; }
        public Nullable<bool> y_bachelordegree { get; set; }
        public Nullable<bool> isbachelorForcheck { get; set; }
        public Nullable<bool> y_bachelordegreeCheck { get; set; }
        public Nullable<int> y_examScore { get; set; }
        public Nullable<bool> Y_generateWhether { get; set; }
        public string y_origin { get; set; }
        public string y_degreeNum { get; set; }
        public Nullable<int> y_scoreOk { get; set; }
        public Nullable<int> y_ImgIsok { get; set; }
        public Nullable<int> y_IsWorking { get; set; }
        public string y_parentName1 { get; set; }
        public string y_parentCard1 { get; set; }
        public string y_parentCardType1 { get; set; }
        public string y_parentName2 { get; set; }
        public string y_parentCard2 { get; set; }
        public string y_parentCardType2 { get; set; }
        public string y_cardType { get; set; }
        public string y_graduateImg { get; set; }
        public Nullable<int> y_enrollYear { get; set; }
        public string y_classNum { get; set; }
    
        public virtual YD_Edu_CultureExtent YD_Edu_CultureExtent { get; set; }
        public virtual YD_Edu_ExamFeature YD_Edu_ExamFeature { get; set; }
        public virtual YD_Edu_ForeignLanguage YD_Edu_ForeignLanguage { get; set; }
        public virtual YD_Edu_Major YD_Edu_Major { get; set; }
        public virtual YD_Edu_ProfessionType YD_Edu_ProfessionType { get; set; }
        public virtual YD_Edu_RecruitType YD_Edu_RecruitType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_Score> YD_Edu_Score { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Edu_ScoreTest> YD_Edu_ScoreTest { get; set; }
        public virtual YD_Edu_StuState YD_Edu_StuState { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Fee_StuFeeTb> YD_Fee_StuFeeTb { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Graduate_StudentScore> YD_Graduate_StudentScore { get; set; }
        public virtual YD_Sts_Nation YD_Sts_Nation { get; set; }
        public virtual YD_Sts_Politics YD_Sts_Politics { get; set; }
        public virtual YD_Sts_StuStrange YD_Sts_StuStrange { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YD_Sts_StuStrange> YD_Sts_StuStrange1 { get; set; }
        public virtual YD_Sys_SubSchool YD_Sys_SubSchool { get; set; }
    }
}
