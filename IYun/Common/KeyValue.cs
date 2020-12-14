using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using IYun.Models;

namespace IYun.Common
{
    public  class KeyValue
    {
        /// <summary>
        /// 全局的验证码
        /// </summary>
        public static string All_Code = "All_Code";
        /// <summary>
        /// 后台登录对象名
        /// </summary>
        public static string Admin_Name = "Admin_Name";
        /// 后台登录学生Id
        /// </summary>
        public static string stu_id = "stu_id";
        /// <summary>
        /// 后台登录对象id
        /// </summary>
        public static string Admin_id = "Admin_id";
        /// <summary>
        /// 后台登录对象角色id
        /// </summary>
        public static string Admin_Role_id = "Role_id";
        /// <summary>
        /// 后台登录对象站点id
        /// </summary>
        public static string Admin_Site_id = "Site_id";
        /// <summary>
        /// 后台登录标记，值为“LoginOK”则说明已登录
        /// </summary>
        public static string Admin_LoginFlag = "Admin_LoginFlag";
        /// <summary>
        /// 后台登录成功的标记值
        /// </summary>
        public static string Admin_LoginOKFlag = "LoginOK";
        /// <summary>
        /// 商城模块文件上传标记
        /// </summary>
        public static string SHOPUPLOADFLAG = "SHOPUPLOADFLAG";
        /// <summary>
        /// 网站模块文件上传标记
        /// </summary>
        public static string WEBUPLOADFLAG = "WEBUPLOADFLAG";

        /// <summary>
        /// 用户登陆对象
        /// </summary>
        public static string AdminRelName = "AdminRelName";


        /// <summary>
        /// 商城会员登录对象名
        /// </summary>
        public static string User_Name = "User_Name";
        /// <summary>
        /// 商城会员登录对象id
        /// </summary>
        public static string User_id = "User_id";
        /// <summary>
        /// 商城会员登录对象角色id
        /// </summary>
        public static string User_Type_id = "Type_id";
        /// <summary>
        /// 商城会员登录标记，值为“LoginOK”则说明已登录
        /// </summary>
        public static string User_LoginFlag = "User_LoginFlag";
        /// <summary>
        /// 商城会员登录成功的标记值
        /// </summary>
        public static string User_LoginOKFlag = "LoginOK";
        /// <summary>
        /// 学校添加的政治面貌和民族
        /// </summary>
        public static string[] NationAndPolitics = new string[]{ComEnum.SchoolName.ZYYDX.ToString(),ComEnum.SchoolName.GNSFDX.ToString(), ComEnum.SchoolName.JXLG.ToString(), ComEnum.SchoolName.JXKJSFDX.ToString(),ComEnum.SchoolName.JXSFDX.ToString()};
        /// <summary>
        /// 学校匹配函授站功能
        /// </summary>
        public static string[] SubschoolAndSchool=new string[] {};
        /// <summary>
        /// 学校添加的学生缴费管理和缴费审核的学习形式和层次和学制
        /// </summary>
        public static  string[] StuedutypeAnDyear=new string[]{ComEnum.SchoolName.ZYYDX.ToString()};
        //学校添加的毕业管理栏目id不一样 :第一个是59 ，第二个统一是55
        /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是59
        /// </summary>
        public static  string[] StuGradutateModel=new string[] {ComEnum.SchoolName.JXKJSFDX.ToString(),ComEnum.SchoolName.JXLG.ToString(),ComEnum.SchoolName.NCHKHTDX.ToString()};
        /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是55
        /// </summary>
        public static  string[] StuGradutateModel2=new string[] {ComEnum.SchoolName.JXSFDX.ToString(), ComEnum.SchoolName.DHLGDX.ToString() };
         /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是80
        /// </summary>
        public static  string[] StuGradutateModel3=new string[] {ComEnum.SchoolName.HDJTDX.ToString()};
        /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是81
        /// </summary>
        public static  string[] StuGradutateModel4=new string[] {ComEnum.SchoolName.JDZTCDX.ToString()};
         /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是92
        /// </summary>
        public static  string[] StuGradutateModel5=new string[] {ComEnum.SchoolName.ZYYDX.ToString()};
        /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是57
        /// </summary>
        public static  string[] StuGradutateModel6=new string[] {ComEnum.SchoolName.HNZYY.ToString()};
        
         /// <summary>
        /// 学校添加的毕业管理栏目,该栏目id是77
        /// </summary>
        public static  string[] StuGradutateModel7=new string[] {ComEnum.SchoolName.GNSFDX.ToString()};
        /// <summary>
        /// 得到对应的学生图片
        /// </summary>
        /// <param name="studexamNum"></param>
        /// <returns></returns>
        public static string UpdateStudentImg(string studexamNum)
        {

            using (var yunEntities = new IYunEntities())
            {
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == studexamNum);
                var stucard = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_inYear == 2015 
                && u.y_cardId == studexamNum); //师大15级学生使用身份证号匹配
                string path = "";
                if (studexamNum.Substring(0, 2) == "14")
                {
                    path = "../../Content/StudentImg/2015级相片/" + stu.y_examNum + ".jpg";
                }
                if (studexamNum.Substring(0, 2) == "15")
                {
                    path = "../../Content/StudentImg/2016级相片/" + stu.y_examNum + ".jpg";
                }
                else
                {
                    path = "../../Content/StudentImg/新15级相片/" + stu.y_cardId + ".jpg";
                }
                return path;
            }
        }

        public static string UpdateStudentImgnew(string num,string card)
        {
            using (var yunEntities = new IYunEntities())
            {
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_examNum == num && u.y_inYear == 2015);
                var stucard = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_inYear == 2015
                && u.y_cardId == card); //师大15级学生使用身份证号匹配
                string path = "";
                path = "D:/wwwroot/jxsfdx/wwwroot/Content/StudentImg/新15级相片/" + stu.y_cardId + ".jpg";

                if (File.Exists(path))
                {
                    path = "../../Content/StudentImg/新15级相片/" + stu.y_cardId + ".jpg";                 
                }
                else
                {
                    if (num.Substring(0, 2) == "14")
                    {
                        path = "../../Content/StudentImg/2015级相片/" + stu.y_examNum + ".jpg";
                    }
                }
                        
                return path;
            }
        }
    }

   
}