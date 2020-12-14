using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using IYun.Common;
using IYun.Controllers;
using IYun.Models;
using IYun.Object;

namespace IYun.Dal
{
    /// <summary>
    /// Strange对象基础操作（增加、修改）类型的委托
    /// </summary>
    /// <param name="strange"></param>
    /// <returns></returns>
    public delegate ResultInfo OperStrange(YD_Sts_StuStrange strange, IYunEntities yunEntities);
    public class YD_Sts_StuStrangeDal : BaseDal<YD_Sts_StuStrange>
    {
        /// <summary>
        /// 增加方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要新增的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo AddOper(YD_Sts_StuStrange admin, IYunEntities yunEntities)
        {
            var resultInfo = BaseAddEntity(admin, "/Student/StudentInfoChangeAll", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Insert,
                    "新增学籍异动,id:" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 修改方法,委托：OperAdmin类型
        /// </summary>
        /// <param name="admin">要修改的用户对象</param>
        /// <param name="yunEntities"></param>
        /// <returns>信息集对象</returns>
        private ResultInfo EditOper(YD_Sts_StuStrange admin, IYunEntities yunEntities)
        {
            var resultInfo = BaseEditEntity(admin, "/Student/StudentInfoChangeAll", yunEntities);
            if (resultInfo.Success)
            {
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update,
                    "修改学籍异动,id：" + admin.id);
            }
            return resultInfo;
        }

        /// <summary>
        /// 针对当前类的实体操作
        /// </summary>
        /// <param name="student">要操作的对象</param>
        /// <param name="curd">操作的类型</param>
        /// <param name="yunEntities"></param>
        /// <returns></returns>
        private static string Oper(YD_Sts_StuStrange student, OperStrange curd, IYunEntities yunEntities)
        {
            var resultInfo = curd(student, yunEntities);
            return resultInfo.Success ? "ok" : resultInfo.Message;
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        public string EditStrange(YD_Sts_StuStrange student, IYunEntities yunEntities)
        {
            return Oper(student, EditOper, yunEntities);
        }


        /// <summary>
        /// 增加用户
        /// </summary>
        /// <returns></returns>
        public string AddStrange(YD_Sts_StuStrange studInfo, IYunEntities yunEntities)
        {
            return Oper(studInfo, AddOper, yunEntities);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public string DeleteStrange(HttpRequestBase request, IYunEntities yunEntities)
        {
            var ids = request["ids"];
            if (string.IsNullOrEmpty(ids))
            {
                return "数据为空";
            }

            #region “删除”权限验证

            var powerInfo = Safe("/Student/StudentInfoChangeAll", PowerFlag.Delete);
            switch (powerInfo)
            {
                case PowerInfo.NoPower:
                    return "无权限";
                case PowerInfo.NoLogin:
                    return "未登录";
                case PowerInfo.Unknow:
                    return "未知状况";
                case PowerInfo.HasPower:
                    break;
                default:
                    return "未知状况";
            }

            #endregion

            var idsstr = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var idsList = new List<int>();
            for (var i = 0; i < idsstr.Count(); i++)
            {
                idsList.Add(Convert.ToInt32(idsstr[i]));
            }
            var admins = yunEntities.YD_Sts_StuStrange.Where(u => idsList.Contains(u.id)).ToList();

            foreach (var admin in admins)
            {
                admin.y_isdel = (int)YesOrNo.Yes;
                yunEntities.Entry(admin).State = EntityState.Modified;
                LogHelper.DbLog(Convert.ToInt32(HttpContext.Current.Session[KeyValue.Admin_id]),
                    HttpContext.Current.Session[KeyValue.Admin_Name].ToString(), (int)LogType.Delete,
                    "删除学籍异动,id：" + admin.id);
            }
            var j = yunEntities.SaveChanges();
            return j > 0 ? "ok" : "删除学籍异动失败";
        }

        public string AddExtended(YD_Sts_StuStrange stra, IYunEntities yunEntity)
        {
            if (!IsLogin()) return "异动申请失败,登陆信息失效！";
            var stu = yunEntity.YD_Sts_StuInfo.FirstOrDefault(x=>x.id==stra.y_stuId);
            if (stu == null)
            {
                return "学生不存在！";
            }
            if (stra.y_strangeType == 1)
            {
                if (stra.y_contentA == stu.y_majorId.ToString())
                {
                    return "不能申请原先的专业";
                }
            }
            if (stra.y_strangeType == 2)
            {
                if (stra.y_contentA == stu.y_subSchoolId.ToString())
                {
                    return "不能申请原先的函授站";
                }
            }
                
            stra.y_isdel = (int)YesOrNo.No;
            stra.y_applyAdmin = YdAdminId;
            stra.y_applyTime = DateTime.Now;
            stra.y_approvalStatus = (int)ApprovaState.WaitApprova;
            stra.y_isExecutive = (int)YesOrNo.No;
            if (stra.y_strangeType == 1)
            {
                if (stra.y_contentA != null)
                {
                    var majorid = Convert.ToInt32(stra.y_contentA);
                    var major = yunEntity.YD_Edu_Major.FirstOrDefault(u => u.id == majorid);
                    if (major != null)
                    {
                        stra.y_contentA = major.id.ToString();
                        yunEntity.Entry(stra).State = EntityState.Modified;
                    }
                    else
                    {
                        return "异动申请添加失败，专业编号错误！";
                    }
                    stra.y_contentB = stu.y_majorId.ToString();
                }
            }
            else if (stra.y_strangeType == 2)
            {
                if (stra.y_contentA != null)
                {
                    var subschoolid = Convert.ToInt32(stra.y_contentA);
                    var subschool = yunEntity.YD_Sys_SubSchool.FirstOrDefault(u => u.id == subschoolid);
                    if (subschool != null)
                    {
                        stra.y_contentA = subschool.id.ToString();
                        yunEntity.Entry(stra).State = EntityState.Modified;
                    }
                    else
                    {
                        return "异动申请添加失败，函授站编号错误！";
                    }
                    stra.y_contentB = stu.y_subSchoolId.ToString();
                }
            }
            //todo:新添加的异动类型:基本信息修改
            else if (stra.y_strangeType == 6)
            {
                if (stra.y_contentA != null)
                {
                    var nationid = Convert.ToInt32(stra.y_contentA);
                    var nation = yunEntity.YD_Sts_Nation.FirstOrDefault(u => u.id == nationid);
                    if (nation != null)
                    {
                        stra.y_contentA = nation.id.ToString();
                        yunEntity.Entry(stra).State = EntityState.Modified;
                    }
                    else
                    {
                        return "异动申请添加失败，民族编号错误！";
                    }
                    stra.y_contentB = stu.y_nationId.ToString();
                }
                if (stra.y_contentG != null)
                {
                    if (stra.y_contentG == stu.y_cardId)
                    {
                        return "已是该申请信息，不需要修改";
                    }
                    stra.y_contentH = stu.y_cardId;
                }
                if (stra.y_contentE != null)
                {
                    var birthday = Convert.ToDateTime(stra.y_contentE);
                    if (birthday == stu.y_birthday)
                    {
                        return "已是该申请信息，不需要修改";
                    }
                    stra.y_contentF = stu.y_birthday.ToString();
                }
                if (stra.y_contentC != null)
                {
                    var sex = Convert.ToInt32(stra.y_contentC);
                    stra.y_contentC = null;
                    if (sex != stu.y_sex)
                    {
                        stra.y_contentC = sex.ToString();
                        stra.y_contentD = stu.y_sex.ToString();
                    }
                }
                if (stra.y_contentI != null)
                {
                    if (stra.y_contentI == stu.y_name)
                    {
                        return "已是该申请信息，不需要修改";
                    }
                    stra.y_contentJ = stu.y_name;

                }
            }
            var oldstra = yunEntity.YD_Sts_StuStrange.FirstOrDefault(u => u.y_stuId == stra.y_stuId
             && u.y_strangeType == stra.y_strangeType && u.y_approvalStatus == (int)ApprovaState.WaitApprova 
             && u.y_isdel == (int)YesOrNo.No);
            if (oldstra != null)
            {
                return "已申请过相同异动，不能再申请";
            }

            return AddStrange(stra, yunEntity);
        }

        public string ApprovaStra(YD_Sts_StuStrange stra, IYunEntities yunEntity)
        {
            if (!IsLogin()) return "异动审批失败,登陆信息失效！";
            var str = yunEntity.YD_Sts_StuStrange.Find(stra.id);
            if (str == null)
            {
                return "异动审批失败，编号错误！";
            }
            str.y_approvalAdmin = YdAdminId;
            str.y_approvalTime = DateTime.Now;
            str.y_approvalStatus = stra.y_approvalStatus;            //审批状态
            str.y_approvalReason = stra.y_approvalReason;
            stra.y_stuId = str.y_stuId;
            if (str.y_approvalStatus == (int)ApprovaState.HadApprova)
            {
                string msg = "";
                switch (str.y_strangeType)
                {
                    case 1:
                        msg = ChangeMajor(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //转专业
                    case 2:
                        msg = ChangeSchool(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //转函授站
                    case 3:
                        msg = Xiuxue(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //休学
                    case 4:
                        stra.y_isExecutive = (int)YesOrNo.Yes;
                        stra.y_ExecutiveTime = DateTime.Now;
                        break; //留级
                    case 5:
                        msg = Tuixue(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //退学
                    case 6:
                        msg = UpdateStuStrange(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break;
                    default:
                        return "未找到对应的异动类型！";
                }
            }
            return EditStrange(str, yunEntity);
        }
        /// <summary>
        /// 审核错误-驳回
        /// </summary>
        /// <param name="stra"></param>
        /// <param name="yunEntity"></param>
        /// <returns></returns>
        public string TurnApprovaStra(HttpRequestBase request, IYunEntities yunEntity)
        {
            var ids = request["ids"];
            if (string.IsNullOrEmpty(ids))
            {
                return "数据为空";
            }
            if (!IsLogin()) return "异动审批失败,登陆信息失效！";
            var id = Convert.ToInt32(ids);
            var str = yunEntity.YD_Sts_StuStrange.Find(id);
            if (str == null)
            {
                return "异动审批失败，编号错误！";
            }
            str.y_approvalAdmin = YdAdminId;
            str.y_approvalTime = DateTime.Now;
            if (str.y_approvalStatus == (int)ApprovaState.HadApprova)
            {
                string msg = "";
                switch (str.y_strangeType)
                {
                    case 1:
                        msg = TrunChangeMajor(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //转专业
                    case 2:
                        msg = TrunChangeSchool(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //转函授站
                    case 3:
                        msg = Xiuxue(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //休学
                    case 4:
                        str.y_isExecutive = (int)YesOrNo.No;
                        str.y_ExecutiveTime = DateTime.Now;
                        break; //留级
                    case 5:
                        msg = TrunTuixue(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break; //退学
                    case 6:
                        msg = TrunUpdateStuStrange(str);
                        if (!msg.Equals("ok"))
                            return msg;
                        break;
                    default:
                        return "未找到对应的异动类型！";
                }
            }
            str.y_approvalStatus = 1;   //审批状态
            return EditStrange(str, yunEntity);
        }
        public string TrunChangeMajor(YD_Sts_StuStrange stra)
        {
            using (var yunEntities = new IYunEntities())
            {
                var majorid = Convert.ToInt32(stra.y_contentB);
                if (!yunEntities.YD_Edu_Major.Any(u => u.id == majorid))
                {
                    return "转专业失败,目标专业不存在！";
                }
                var stu = yunEntities.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "转专业失败,目标学生不存在！";
                }
                stu.y_majorId = majorid;
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntities.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }

        public string TrunChangeSchool(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var schoolid = Convert.ToInt32(stra.y_contentB);
                if (!yunEntity.YD_Sys_SubSchool.Any(u => u.id == schoolid))
                {
                    return "转函授站失败,目标函授站不存在！";
                }
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "转函授站失败,目标学生不存在！";
                }
                stu.y_subSchoolId = schoolid;
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }
        public string TrunXiuxue(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "休学失败,目标学生不存在！";
                }
                stu.y_stuStateId = 1;                  //休学状态
                stra.y_isExecutive = (int)YesOrNo.No;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }
        public string TrunTuixue(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "退学失败,目标学生不存在！";
                }
                stu.y_stuStateId = 1;                  //退学状态
                stra.y_isExecutive = (int)YesOrNo.No;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }
         public string TrunUpdateStuStrange(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "修改信息失败,目标学生不存在！";
                }
                if (stra.y_contentB != null)
                {
                    var nationid = Convert.ToInt32(stra.y_contentB);
                    if (!yunEntity.YD_Sts_Nation.Any(u => u.id == nationid))
                    {
                        return "改民族失败,目标民族不存在！";
                    }
                    stu.y_nationId = nationid;
                }
                if (stra.y_contentJ != null)
                    stu.y_name = stra.y_contentJ;
                if (stra.y_contentH != null)
                    stu.y_cardId = stra.y_contentH;
                if (stra.y_contentD != null)
                {
                    var sex = Convert.ToInt32(stra.y_contentD);
                    stu.y_sex = sex;
                }
                if (stra.y_contentF != null)
                {
                    var birstry = Convert.ToDateTime(stra.y_contentF);
                    stu.y_birthday = birstry;
                }

               stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }

        public string ChangeMajor(YD_Sts_StuStrange stra)
        {
            using (var yunEntities = new IYunEntities())
            {
                var majorid = Convert.ToInt32(stra.y_contentA);
                if (!yunEntities.YD_Edu_Major.Any(u => u.id == majorid))
                {
                    return "转专业失败,目标专业不存在！";
                }
                var stu = yunEntities.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "转专业失败,目标学生不存在！";
                }


                stu.y_majorId = majorid;
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntities.SaveChanges() > 0 ? "ok" : "未知错误！";



            }
        }

        public string ChangeSchool(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var schoolid = Convert.ToInt32(stra.y_contentA);
                if (!yunEntity.YD_Sys_SubSchool.Any(u => u.id == schoolid))
                {
                    return "转函授站失败,目标函授站不存在！";
                }
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "转函授站失败,目标学生不存在！";
                }

                stu.y_subSchoolId = schoolid;
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }

        public string Xiuxue(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "休学失败,目标学生不存在！";
                }
                stu.y_stuStateId = 2;                  //休学状态
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }

        public string Tuixue(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "退学失败,目标学生不存在！";
                }
                stu.y_stuStateId = 4;                  //退学状态
                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }
        public string UpdateStuStrange(YD_Sts_StuStrange stra)
        {
            using (var yunEntity = new IYunEntities())
            {
                var stu = yunEntity.YD_Sts_StuInfo.Find(stra.y_stuId);
                if (stu == null)
                {
                    return "修改信息失败,目标学生不存在！";
                }
                if (stra.y_contentA != null)
                {
                    var nationid = Convert.ToInt32(stra.y_contentA);
                    if (!yunEntity.YD_Sts_Nation.Any(u => u.id == nationid))
                    {
                        return "改民族失败,目标民族不存在！";
                    }
                    stu.y_nationId = nationid;
                }
                if (stra.y_contentI != null)
                    stu.y_name = stra.y_contentI;
                if (stra.y_contentG != null)
                    stu.y_cardId = stra.y_contentG;
                if (stra.y_contentC != null)
                {
                    var sex = Convert.ToInt32(stra.y_contentC);
                    stu.y_sex = sex;
                }
                if (stra.y_contentE != null)
                {
                    var birstry = Convert.ToDateTime(stra.y_contentE);
                    stu.y_birthday = birstry;
                }

                stra.y_isExecutive = (int)YesOrNo.Yes;
                stra.y_ExecutiveTime = DateTime.Now;
                return yunEntity.SaveChanges() > 0 ? "ok" : "未知错误！";
            }
        }
    }
}