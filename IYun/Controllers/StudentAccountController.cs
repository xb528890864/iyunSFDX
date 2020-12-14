using IYun.Common;
using IYun.Controllers.ControllerObject;
using IYun.Dal;
using IYun.Models;
using IYun.Models.Dto;
using IYun.Object;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace IYun.Controllers
{
    public class StudentAccountController : AdminBaseController
    {

        private readonly YD_Sts_StuStrangeDal _stuStrangeDal = new YD_Sts_StuStrangeDal();
        //
        // GET: /StudentAccount/
        public ActionResult Login()
        {
            return View();
        }
        public int LoginId
        {
            get
            {
                if (Session[KeyValue.stu_id] == null)
                {
                    Session.RemoveAll();
                    Response.Redirect("/AdminBase/Index");
                }

                return Convert.ToInt32(Session[KeyValue.stu_id]);
            }
        }

        public string Admin_Name
        {
            get
            {
                if (Session[KeyValue.Admin_Name] == null)
                {
                    Session.RemoveAll();
                    Response.Redirect("/AdminBase/Index");
                }

                return Session[KeyValue.Admin_Name].ToString();
            }
        }

        #region Login/Logout



        /// <summary>
        /// 退出后台登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit1()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "AdminBase");
        }
        #endregion

        public ActionResult Index1()
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                IQueryable<YD_Edu_News> list = yunEntities.YD_Edu_News.OrderByDescending(u => u.id);//查新闻，降序排列
                var newslist = list.Where(u => u.y_type == 2).Take(8).ToList();//然后是新闻类型分类
                var listedu = list.Where(u => u.y_type == 1).Take(8).ToList();
                var listtixi = list.Where(u => u.y_type == 3).Take(8).ToList();
                var listqita = list.Where(u => u.y_type == 4).Take(8).ToList();
                ViewBag.newslist = newslist;//放入ViewBag
                ViewBag.listedu = listedu;
                ViewBag.listtixi = listtixi;
                ViewBag.listqita = listqita;
                return View();
            }
        }

        /// <summary>
        /// 顶栏
        /// </summary>
        /// <returns></returns>
        public ActionResult Top1()
        {
            if (!IsLogin())
            {
                return RedirectToAction("Login");
            }
            ViewBag.adminName = Admin_Name;
            return PartialView();
        }

        public ActionResult StudentInfo()
        {
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            using (var yunEntities = new IYunEntities())
            {
                const int status = (int)ApprovaState.HadApprova;

                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == LoginId);
                //获取异动信息
                var strange =
                    yunEntities.VW_Strange.Where(u => u.y_approvalStatus == status)
                        .FirstOrDefault(u => u.y_stuId == LoginId);
                ViewBag.strange = strange;
                ViewData["student"] = student;
            }
            return View();
        }

        public ActionResult EditStudent()
        {

            #region 权限验证
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == LoginId);
                if (student == null)
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewData["student"] = student;
                ViewBag.subSchoolName = yunEntities.YD_Sys_SubSchool.First(x => x.id == student.y_subSchoolId).y_name;
            }
            return View();
        }

        /// <summary>
        /// 学生信息编辑AJAX
        /// </summary>
        /// <param name="stu">学生信息对象</param>
        /// <returns>处理结果json</returns>
        public string EditStudentInfo(YD_Sts_StuInfo stu)
        {
            #region 权限验证

            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }

            #endregion

            using (var yunEntities = new IYunEntities())
            {
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                var stus = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == stu.id);

                var re = new Hashtable();


                //var msg = _stuInfoDal.EditStuInfoExtended(stu, Request, yunEntities);

                stus.y_sex = stu.y_sex;
                stus.y_birthday = stu.y_birthday;
                stus.y_tel = stu.y_tel;
                stus.y_mail = stu.y_mail;
                stus.y_address = stu.y_address;
                stus.y_img = stu.y_img;
                yunEntities.Entry(stus).State = EntityState.Modified;

                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    // LogHelper.DbLog(Convert.ToInt32(Session[KeyValue.Admin_id]),
                    //Session[KeyValue.Admin_Name].ToString(), (int)LogType.Update,
                    // "修改学生，" + ",id：" + stu.id + "姓名，入学年份，身份证号，出生日期，学号，考生号，学籍状态" + stu.y_name + "," + stu.y_inYear + "," + stu.y_cardId + "," + stu.y_birthday + "," + stu.y_stuNum + "," + stu.y_examNum + "," + stu.y_stuStateId);
                    re["msg"] = "修改成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = "修改失败";
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);

            }
        }

        /// <summary>
        /// 补录个税抵扣信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplementaryTaxInfo()
        {
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            using (var yunEntities = new IYunEntities())
            {
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == LoginId);
                ViewBag.stuNum = student.y_stuNum;
                ViewBag.stuName = student.y_name;
                ViewBag.stuCardId = student.y_cardId;
            }
            return View(LoginId);
        }

        [HttpPost]
        public string SupplementaryTax(SupplementaryTaxInput input)
        {
            #region 权限验证

            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            var re = new Hashtable();
            #endregion

            using (var yunEntities = new IYunEntities())
            {
                var stu = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.id == input.Id);
                stu.y_IsWorking = (input.IsWorking == true ? 1 : 0);
                stu.y_cardType = input.CardType;
                stu.y_parentCard1 = input.ParentCard1;
                stu.y_parentCard2 = input.ParentCard2;
                stu.y_parentCardType1 = input.ParentCardType1;
                stu.y_parentCardType2 = input.ParentCardType2;
                stu.y_parentName1 = input.ParentName1;
                stu.y_parentName2 = input.ParentName2;
                if(string.IsNullOrEmpty(input.ParentCard1) || string.IsNullOrEmpty(input.ParentCardType1) || string.IsNullOrEmpty(input.ParentName1 ))
                {
                    re["msg"] = "父母或监护人1信息需要填写完整";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                if (input.ParentCard1.Length != 18)
                {
                    re["msg"] = "父母或监护人1身份证不为18位";
                    re["isok"] = false;
                    return JsonConvert.SerializeObject(re);
                }
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    re["msg"] = "补录完毕";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = "补录失败";
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        public ActionResult TaxInfo()
        {
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            using (var yunEntities = new IYunEntities())
            {
                var student = yunEntities.VW_StuInfo.SingleOrDefault(u => u.id == LoginId);
                if(student.y_IsWorking == null)
                {
                    return Redirect("/StudentAccount/SupplementaryTaxInfo");
                }
                ViewData["student"] = student;
            }
            return View();
        }

        /// <summary>
        /// 缴费信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Payment()
        {
            #region 权限验证
            if (!IsLogin())
            {
                Redirect("/AdminBase/Login");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                const int isnotdel = (int)YesOrNo.No;
                IQueryable<YD_Fee_StuFeeTb> list =
                    yunEntities.YD_Fee_StuFeeTb.Include(u => u.YD_Sts_StuInfo).
                    Include(u => u.YD_Sts_StuInfo.YD_Edu_Major).OrderByDescending(u => u.id).
                    Where(u => u.YD_Sts_StuInfo.y_isdel == isnotdel && u.YD_Sts_StuInfo.id == LoginId);

                var allList = list.ToList();
                return View(allList);
            }
        }

        /// <summary>
        /// 教学计划
        /// </summary>
        /// <returns></returns>
        public ActionResult StuPlan()
        {
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            var yearStr = Request.Form["EnrollYear2"];
            int year;
            if (string.IsNullOrWhiteSpace(yearStr))
            {
                year = Convert.ToInt32(ConfigurationManager.AppSettings["xinsheng"]);
            }
            else
            {
                year = Convert.ToInt32(yearStr);
            }


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select S.*,C.y_name as courseName,M.majorLibraryName as majorName,M.eduTypeName,M.stuTypeName from VW_Major as M join ");
            sb.AppendLine("(select c.y_majorId,c.y_year, des.y_course,des.y_team from YD_TeaPlan_ClassCourseDes as des join ");
            sb.AppendLine("(SELECT y_templetId,y_year,d.y_majorId,MAX(id) as id ");
            sb.AppendLine("FROM YD_TeaPlan_Class as y join ");
            sb.AppendLine("(select y_inYear, [y_majorId]  from VW_StuInfo ");
            sb.AppendLine("where id = " + LoginId);
            sb.AppendLine(" group by[y_majorId], y_inYear)");
            sb.AppendLine("as d on d.y_inYear = y.y_year and d.y_majorId = y.y_majorId ");
            sb.AppendLine("group by y_templetId, y_year, d.y_majorId) ");
            sb.AppendLine("as c on des.y_classTeaPlanId = c.id  where (des.y_team + 1) / 2 + c.y_year = " + (year + 1));
            sb.AppendLine("group by c.y_majorId,c.y_year, des.y_course,des.y_team)");
            sb.AppendLine("as S on S.y_majorId = M.id ");
            sb.AppendLine("join YD_Edu_Course as C on C.id = S.y_course  order by S.y_year, S.y_majorId,S.y_team");


            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.Database.SqlQuery<CoursePlanDto>(sb.ToString()).ToList();

                ViewData["year"] = year;

                return View(list);
            }
        }

        /// <summary>
        /// 历史成绩
        /// </summary>
        /// <returns></returns>
        public ActionResult MyScore()
        {
            #region 权限验证
            if (!IsLogin())
            {
                Redirect("/AdminBase/Index");
            }
            #endregion

            using (var yunEntities = new IYunEntities())
            {
                int term = Convert.ToInt32(Request["term"]);

                ViewBag.term = term;
                var list = yunEntities.YD_Edu_Score.Include(x => x.YD_Sts_StuInfo).Include(x => x.YD_Edu_Course).Where(x => x.y_stuId == LoginId).OrderBy(x => x.y_term).Distinct();
                list = list.Where(x => x.y_term == term);
                List<ScoreListDto> model = list.Select(u => new ScoreListDto
                {
                    StuId = u.y_stuId,
                    StuName = u.YD_Sts_StuInfo.y_name,
                    stunum = u.YD_Sts_StuInfo.y_stuNum,
                    CourseId = u.y_courseId,
                    CourseName = u.YD_Edu_Course.y_name,
                    Team = u.y_term,
                    ScoreId = u.id,
                    NormalScore = u.y_normalScore,
                    TermScore = u.y_termScore,
                    TotalScore = u.y_totalScore,
                    ScoreOk = u.YD_Sts_StuInfo.y_scoreOk == 1 ? "通过" : u.YD_Sts_StuInfo.y_scoreOk == 0 ? "不通过" : "未审核"
                }).ToList();
                return View(model);
            }
        }

        /// <summary>
        /// 学生异动申请
        /// </summary>
        /// <returns></returns>
        public ActionResult StuInfoChangeApply()
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                var student =
                    yunEntities.YD_Sts_StuInfo.Include(u => u.YD_Sts_Nation)
                        .Include(u => u.YD_Edu_Major)
                        .First(u => u.id == LoginId);
                if (student == null)
                {
                    return RedirectToAction("StudentInfo");
                }
                ViewData["student"] = student;
            }
            return View();
        }

        /// <summary>
        /// 学籍异动申请AJAX
        /// </summary>
        /// <param name="stra">学籍异动对象</param>
        /// <returns>处理结果json</returns>
        public string StuStrangeApply(YD_Sts_StuStrange stra)
        {
            using (var yunEntities = new IYunEntities())
            {
                var re = new Hashtable();
                var msg = _stuStrangeDal.AddExtended(stra, yunEntities);
                if (msg == "ok")
                {
                    re["msg"] = "申请成功";
                    re["isok"] = true;
                }
                else
                {
                    re["msg"] = msg;
                    re["isok"] = false;
                }
                return JsonConvert.SerializeObject(re);
            }
        }

        public ActionResult GraduationApplyInfo()
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                var stu = yunEntities.YD_Sts_StuInfo.Include(x => x.YD_Edu_Major).FirstOrDefault(x => x.id == LoginId);
                return View(stu);
            }
        }

        /// <summary>
        /// 学生毕业申请
        /// </summary>
        /// <returns></returns>
        public string StuGradInfoSome()
        {
            using (var yunEntities = new IYunEntities())
            {
                var stu = yunEntities.YD_Sts_StuInfo.Include(x => x.YD_Edu_Major).FirstOrDefault(x => x.id == LoginId);
                stu.y_isgraduate = true;
                yunEntities.Entry(stu).State = EntityState.Modified;
                int r = yunEntities.SaveChanges();
                if (r > 0)
                {
                    return "ok";
                }
                else
                {
                    return "设置失败";
                }
            }
        }

        public ActionResult Activity()
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                var list = yunEntities.YT_ActivityVideo.Where(u => u.y_starttime < DateTime.Now && u.y_endtime > DateTime.Now).OrderByDescending(u => u.id).ToList();
                return View(list);
            }
        }


        public ActionResult LiveVideo(int id)
        {
            if (!IsLogin())//检查登录
            {
                return Redirect("/AdminBase/Index");//没登录就跳转
            }
            using (var yunEntities = new IYunEntities())
            {
                var model = yunEntities.YT_ActivityVideo.FirstOrDefault(u => u.id == id);
                if (model == null)
                {
                    return Content("活动不存在");
                }
                return View(model);
            }
        }

        //跳转青书对应角色账号
        public ActionResult GoToQinshu()
        {
            var qinshukey = ConfigurationManager.AppSettings["QinshuKey"];

            if (string.IsNullOrWhiteSpace(qinshukey))
            {
                return Content("此学校未配置青书对应学校编号");
            }

            string url = "http://www.qingshuxuetang.com/QingShuHomeSvc/v1/auth/login";

            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }

            using (var ad = new IYunEntities())
            {
                var admin = ad.YD_Sts_StuInfo.First(u => u.id == LoginId);

                if (string.IsNullOrWhiteSpace(admin.y_loginName))
                {
                    return Content("此账号未配置青书对应账号");
                }

                var dto = new ApiDto()
                {
                    collegeSymbol = qinshukey,
                    name = admin.y_loginName,
                    password = admin.y_password
                };

                var req = JsonConvert.SerializeObject(dto);

                string str = Post_Request_API(req, url);

                var result = JsonConvert.DeserializeObject<ApiResultDto>(str);

                if (result.hr == 0 && result.data != null)
                {
                    return Redirect(result.data.url);
                }
                return Content(result.message);
            }

        }

        public class ApiDto
        {
            public string collegeSymbol { get; set; }

            public string name { get; set; }

            public string password { get; set; }
        }

        public class ApiResultDto
        {
            public int hr { get; set; }

            public string message { get; set; }

            public ApiResultExt data { get; set; }

            public string extraData { get; set; }
        }
        public class ApiResultExt
        {
            public string url { get; set; }
        }

        #region 调用API接口 

        /// <summary>
        /// Post 请求API接口 
        /// </summary>
        /// <param name="str_Json">json字符串</param>
        /// <returns></returns>
        public static string Post_Request_API(string str_Json, string API_URL)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(API_URL);
                HttpWebRequest req = webRequest as HttpWebRequest;

                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                req.Timeout = 6000000; // 5000;
                byte[] bytebuff = Encoding.UTF8.GetBytes(str_Json);
                req.ContentLength = bytebuff.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(bytebuff, 0, bytebuff.Length);
                requestStream.Close();

                //所请求页面响应
                WebResponse pos = req.GetResponse();
                StreamReader sr = new StreamReader(pos.GetResponseStream(), Encoding.UTF8);
                string strResult = sr.ReadToEnd().Trim();
                sr.Close();
                sr.Dispose();

                return strResult;
            }
            catch (Exception e1)
            {
                string a = e1.Message;
                return e1.Message;
            }

        }

        #endregion


    }
}
