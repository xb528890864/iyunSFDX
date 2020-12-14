﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IYunEntities : DbContext
    {
        public IYunEntities()
            : base("name=IYunEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<YD_Edu_Course> YD_Edu_Course { get; set; }
        public virtual DbSet<YD_Edu_CourseCorrespondence> YD_Edu_CourseCorrespondence { get; set; }
        public virtual DbSet<YD_Edu_CourseType> YD_Edu_CourseType { get; set; }
        public virtual DbSet<YD_Edu_CultureExtent> YD_Edu_CultureExtent { get; set; }
        public virtual DbSet<YD_Edu_EduType> YD_Edu_EduType { get; set; }
        public virtual DbSet<YD_Edu_ExamFeature> YD_Edu_ExamFeature { get; set; }
        public virtual DbSet<YD_Edu_ForeignLanguage> YD_Edu_ForeignLanguage { get; set; }
        public virtual DbSet<YD_Edu_FormTemp> YD_Edu_FormTemp { get; set; }
        public virtual DbSet<YD_Edu_GradCondition> YD_Edu_GradCondition { get; set; }
        public virtual DbSet<YD_Edu_Major> YD_Edu_Major { get; set; }
        public virtual DbSet<YD_Edu_MajorLibrary> YD_Edu_MajorLibrary { get; set; }
        public virtual DbSet<YD_Edu_MajorTeachPlan> YD_Edu_MajorTeachPlan { get; set; }
        public virtual DbSet<YD_Edu_News> YD_Edu_News { get; set; }
        public virtual DbSet<YD_Edu_ProfessionType> YD_Edu_ProfessionType { get; set; }
        public virtual DbSet<YD_Edu_QingCourse> YD_Edu_QingCourse { get; set; }
        public virtual DbSet<YD_Edu_QingStuInfo> YD_Edu_QingStuInfo { get; set; }
        public virtual DbSet<YD_Edu_RecruitType> YD_Edu_RecruitType { get; set; }
        public virtual DbSet<YD_Edu_Score> YD_Edu_Score { get; set; }
        public virtual DbSet<YD_Edu_ScoreScale> YD_Edu_ScoreScale { get; set; }
        public virtual DbSet<YD_Edu_ScoreTemp> YD_Edu_ScoreTemp { get; set; }
        public virtual DbSet<YD_Edu_ScoreTest> YD_Edu_ScoreTest { get; set; }
        public virtual DbSet<YD_Edu_SmallPower> YD_Edu_SmallPower { get; set; }
        public virtual DbSet<YD_Edu_StuState> YD_Edu_StuState { get; set; }
        public virtual DbSet<YD_Edu_StuType> YD_Edu_StuType { get; set; }
        public virtual DbSet<YD_Edu_TeachPlan> YD_Edu_TeachPlan { get; set; }
        public virtual DbSet<YD_Edu_TeachplanDes> YD_Edu_TeachplanDes { get; set; }
        public virtual DbSet<YD_Edu_TeachPlanTemp> YD_Edu_TeachPlanTemp { get; set; }
        public virtual DbSet<YD_Fee_AllBili> YD_Fee_AllBili { get; set; }
        public virtual DbSet<YD_Fee_AllFeeSys> YD_Fee_AllFeeSys { get; set; }
        public virtual DbSet<YD_Fee_MajorLiFeeSys> YD_Fee_MajorLiFeeSys { get; set; }
        public virtual DbSet<YD_Fee_StuFeeTb> YD_Fee_StuFeeTb { get; set; }
        public virtual DbSet<YD_Fee_StuInvoicee> YD_Fee_StuInvoicee { get; set; }
        public virtual DbSet<YD_Fee_StuRegistrBatch> YD_Fee_StuRegistrBatch { get; set; }
        public virtual DbSet<YD_Fee_SubFeeBili> YD_Fee_SubFeeBili { get; set; }
        public virtual DbSet<YD_Fee_SubFeeSys> YD_Fee_SubFeeSys { get; set; }
        public virtual DbSet<YD_Graduate_Bachelor> YD_Graduate_Bachelor { get; set; }
        public virtual DbSet<YD_Graduate_EnglishScoreTemp> YD_Graduate_EnglishScoreTemp { get; set; }
        public virtual DbSet<YD_Graduate_SampleExamScore> YD_Graduate_SampleExamScore { get; set; }
        public virtual DbSet<YD_Graduate_StudentApply> YD_Graduate_StudentApply { get; set; }
        public virtual DbSet<YD_Graduate_StudentScore> YD_Graduate_StudentScore { get; set; }
        public virtual DbSet<YD_Sts_Nation> YD_Sts_Nation { get; set; }
        public virtual DbSet<YD_Sts_Politics> YD_Sts_Politics { get; set; }
        public virtual DbSet<YD_Sts_StrangeType> YD_Sts_StrangeType { get; set; }
        public virtual DbSet<YD_Sts_StuInfo> YD_Sts_StuInfo { get; set; }
        public virtual DbSet<YD_Sts_StuInfo_copy1> YD_Sts_StuInfo_copy1 { get; set; }
        public virtual DbSet<YD_Sts_StuInfoTemp> YD_Sts_StuInfoTemp { get; set; }
        public virtual DbSet<YD_Sts_StuNumCol> YD_Sts_StuNumCol { get; set; }
        public virtual DbSet<YD_Sts_StuStrange> YD_Sts_StuStrange { get; set; }
        public virtual DbSet<YD_Sts_SubSchoolStuInfo> YD_Sts_SubSchoolStuInfo { get; set; }
        public virtual DbSet<YD_Sts_SubStuTemp> YD_Sts_SubStuTemp { get; set; }
        public virtual DbSet<YD_Sys_Admin> YD_Sys_Admin { get; set; }
        public virtual DbSet<YD_Sys_AdminCourseLink> YD_Sys_AdminCourseLink { get; set; }
        public virtual DbSet<YD_Sys_AdminSubLink> YD_Sys_AdminSubLink { get; set; }
        public virtual DbSet<YD_Sys_DbLog> YD_Sys_DbLog { get; set; }
        public virtual DbSet<YD_Sys_DbLogType> YD_Sys_DbLogType { get; set; }
        public virtual DbSet<YD_Sys_File> YD_Sys_File { get; set; }
        public virtual DbSet<YD_Sys_Module> YD_Sys_Module { get; set; }
        public virtual DbSet<YD_Sys_Power> YD_Sys_Power { get; set; }
        public virtual DbSet<YD_Sys_PrintCss> YD_Sys_PrintCss { get; set; }
        public virtual DbSet<YD_Sys_PrintTemplate> YD_Sys_PrintTemplate { get; set; }
        public virtual DbSet<YD_Sys_Role> YD_Sys_Role { get; set; }
        public virtual DbSet<YD_Sys_SubSchool> YD_Sys_SubSchool { get; set; }
        public virtual DbSet<YD_TeaPlan_Class> YD_TeaPlan_Class { get; set; }
        public virtual DbSet<YD_TeaPlan_ClassCourseDes> YD_TeaPlan_ClassCourseDes { get; set; }
        public virtual DbSet<YD_TeaPlan_ExamPlan> YD_TeaPlan_ExamPlan { get; set; }
        public virtual DbSet<YD_TeaPlan_Templet> YD_TeaPlan_Templet { get; set; }
        public virtual DbSet<YD_TeaPlan_TempletCourseDes> YD_TeaPlan_TempletCourseDes { get; set; }
        public virtual DbSet<YT_ActivityVideo> YT_ActivityVideo { get; set; }
        public virtual DbSet<YT_Faculty> YT_Faculty { get; set; }
        public virtual DbSet<YT_Grade> YT_Grade { get; set; }
        public virtual DbSet<YT_GraduationRegistration> YT_GraduationRegistration { get; set; }
        public virtual DbSet<YT_Mechanism> YT_Mechanism { get; set; }
        public virtual DbSet<YT_RegisterSettings> YT_RegisterSettings { get; set; }
        public virtual DbSet<YT_Teacher> YT_Teacher { get; set; }
        public virtual DbSet<YT_Term> YT_Term { get; set; }
        public virtual DbSet<Z_test> Z_test { get; set; }
        public virtual DbSet<VW_Admin> VW_Admin { get; set; }
        public virtual DbSet<VW_AllBili> VW_AllBili { get; set; }
        public virtual DbSet<VW_AllFeeSys> VW_AllFeeSys { get; set; }
        public virtual DbSet<VW_DbLog> VW_DbLog { get; set; }
        public virtual DbSet<VW_Major> VW_Major { get; set; }
        public virtual DbSet<VW_MajorLiFeeSys> VW_MajorLiFeeSys { get; set; }
        public virtual DbSet<VW_MajorTeachPlan> VW_MajorTeachPlan { get; set; }
        public virtual DbSet<VW_MajorTeachPlanDes> VW_MajorTeachPlanDes { get; set; }
        public virtual DbSet<VW_Power> VW_Power { get; set; }
        public virtual DbSet<VW_Score> VW_Score { get; set; }
        public virtual DbSet<VW_ScoreFirst> VW_ScoreFirst { get; set; }
        public virtual DbSet<VW_Strange> VW_Strange { get; set; }
        public virtual DbSet<VW_StrangeReportTotal> VW_StrangeReportTotal { get; set; }
        public virtual DbSet<VW_Sts_News> VW_Sts_News { get; set; }
        public virtual DbSet<VW_StuInfo> VW_StuInfo { get; set; }
        public virtual DbSet<VW_SubFeeBili> VW_SubFeeBili { get; set; }
        public virtual DbSet<VW_SubFeeSys> VW_SubFeeSys { get; set; }
        public virtual DbSet<VW_SubFeeTotal> VW_SubFeeTotal { get; set; }
        public virtual DbSet<VW_SubschoolAdmin> VW_SubschoolAdmin { get; set; }
        public virtual DbSet<VW_SubSchoolStuInfo> VW_SubSchoolStuInfo { get; set; }
        public virtual DbSet<VW_TeachPlanDes> VW_TeachPlanDes { get; set; }
    }
}