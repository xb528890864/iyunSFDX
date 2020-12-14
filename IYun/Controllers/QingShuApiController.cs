using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IYun.Common;
using IYun.Models;
using IYun.Object;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Safe;

namespace IYun.Controllers
{
    /// <summary>
    /// 已弃用，早期青书成绩同步接口方法
    /// </summary>
    public class QingShuApiController : AdminBaseController
    {
        //
        // GET: /QingShuApi/
        public string sen = "";
        public string html = "";
        public string baseurl = "http://www.qingshuxuetang.com/QingShuSchoolWebService/jxsf/v1/score/search";
        public string privatekey =
            @"<RSAKeyValue><Modulus>guMsZ5zUJB/eq2G+Zxnxn3A8RSPgF49NjsljQj44TDXZOh4/JIUowuSWU2hsZ/aoHut6uXOXEl5X01q78iSNfEUJeo1KG6i2f1CkYiIqIQlk4RU8H8+jTd0oRGWtldKgbwEebKPYUtYfdd6+sb1TsWg3ED8nm6yaz6QWvEDrtXU=</Modulus><Exponent>AQAB</Exponent><P>ztwlY0raXVV9jYcL6gD7/cV9VyoiJYJ3Hetcb+r7aBkNfudB4yphyqIXbmHRvN3lthYfh63WwErYg+Casi5ddQ==</P><Q>ofrhHeildgYwi9XvNS1EUQpDa5O+5vl/dXC1eSmHzmSy6RlKJI9UwdzxR01gYMd9rRqXRxMpC8Dc/Z2eyOT4AQ==</Q><DP>qH2uQmsk5CMPSij2rlqw5mpnBol+GLlk1szvlQV8U7UcRgKNqz/JOai/lxw8Hy9KvP7WHcieBDCynPBkcL/NRQ==</DP><DQ>BvH02Q1ymr7lMfm4SfVo6nigL2qkUs770hNFFK7dLdJPgYMeFLc4kR9iEQaWTVAAaX2sYtXFesWINC+f8UkwAQ==</DQ><InverseQ>PTdIqAxDM9ugRHUKdewTV+FjZn8wsCe8K62pvqJghcSH//nSisci7TVXgM2vU/dKtI2B/cNj1tKiN1zCs6BwMA==</InverseQ><D>GyCYYHUppr8QOHcOrnG7GW96nl3cISXrTi/BKcaZhnoWpqwELD6I/zO/UFQxO67sk1P84Jjrc7wn+b8xevNGe3Glb1XeDbg/lWMeomQE7kPdipBuZ/WsnrPF5aTbWPcRXRyRC69vzLaZ0GYVNz9SMgtqkHjyW1ZEcoc/EsOMoAE=</D></RSAKeyValue>";
        public string publickey =
            @"<RSAKeyValue><Modulus>guMsZ5zUJB/eq2G+Zxnxn3A8RSPgF49NjsljQj44TDXZOh4/JIUowuSWU2hsZ/aoHut6uXOXEl5X01q78iSNfEUJeo1KG6i2f1CkYiIqIQlk4RU8H8+jTd0oRGWtldKgbwEebKPYUtYfdd6+sb1TsWg3ED8nm6yaz6QWvEDrtXU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public ActionResult SearchTest()
        {
            string content =
                "{\"data\":{\"centerId\":0,\"majorId\":0,\"enrollYear\":0,\"enrollSemester\":0,\"studentId\":0,\"courseId\":0,\"pageIndex\":0,\"pageSize\":100}}";
            sen = sing(content);
            string content1 = "{\"data\":{\"centerId\":0,\"majorId\":0,\"enrollYear\":0,\"enrollSemester\":0,\"studentId\":0,\"courseId\":0,\"pageIndex\":0,\"pageSize\":100},\"signature\": \"" + sen + "\"}";
            html = WebPost(baseurl, content1, Encoding.UTF8);
            if (html.Contains("服务器"))
            {
                var js = new JavaScriptSerializer();
                var results = js.Deserialize<Dictionary<string, string>>(html);
                if (results.ContainsKey("hr"))
                {
                    if (results["hr"] == "0")
                    {
                        return Content(results["data"]);
                    }
                    else
                    {
                        return Content(results["message"]);
                    }
                }
                else
                {
                    return Content(html);
                }
            }
            else
            {
                return Content(html);
            }

        }

        /// <summary>
        /// 同步分数
        /// </summary>
        /// <returns></returns>
        public string TongBuScore()
        {
            if (!IsLogin())
            {
                return "你还未登录";
            }
            if (YdAdminRoleId != 1)
            {
                return "你没有该权限";
            }
            ////先将课程同步过来
            //if (!TongBuCourse())
            //{
            //    return "课程同步失败";
            //}
            //再将学生也同步过来
            //if (!TongBuStudent())
            //{
            //    return "学生信息同步失败";
            //}
            using (var yunEntities = new IYunEntities())
            {
                var coures = yunEntities.YD_Edu_QingCourse.Where(u => true).ToList();

                int pageIndex = 0;
                int count = 1;
                var tongjiCourse = new List<QingShuStuScore>();
                for (int i = 0; i < coures.Count; i++)
                {
                    var qingScores = new List<QingShuStuScore>();
                    pageIndex = 0;
                    count = 1;
                    while (count > 0)
                    {
                        var qingScore = SearchScore(pageIndex, coures[i].y_qingId);
                        qingScores.AddRange(qingScore);
                        count = qingScore.Count;
                        pageIndex++;
                    }
                    //UploadScore(qingScores);
                    tongjiCourse.AddRange(qingScores);
                }
                tongjiCourse = tongjiCourse.Where(u => u.examScore != 0m).ToList();
                UploadScore(tongjiCourse);
            }
            return "同步成绩成功";
        }
        /// <summary>
        /// 同步课程
        /// </summary>
        /// <returns></returns>
        public bool TongBuCourse()
        {
            string content =
                "{\"data\":{\"ids\":[]}}";
            sen = sing(content);
            string content1 = "{\"data\":{\"ids\":[]},\"signature\": \"" + sen + "\"}";
            html = WebPost("http://www.qingshuxuetang.com/QingShuSchoolWebService/jxsf/v1/course/search", content1, Encoding.UTF8);
            if (!html.Contains("服务器"))
            {
                var results = JsonConvert.DeserializeObject<QingShuCourseMessage>(html);

                if (results.hr == 0)
                {
                    var list = results.data;
                    using (var yunEntities = new IYunEntities())
                    {
                        var qingCourses = new List<YD_Edu_QingCourse>();
                        for (var i = 0; i < list.Count; i++)
                        {
                            var course = new YD_Edu_QingCourse();
                            course.y_qingId = list[i].id;
                            course.y_name = list[i].name;
                            var name = list[i].name;
                            var obj = yunEntities.YD_Edu_Course.FirstOrDefault(u => u.y_name == name);
                            if (obj == null)
                            {
                                continue;
                            }
                            course.y_nid = obj.id;
                            if (!yunEntities.YD_Edu_QingCourse.Any(u => u.y_qingId == course.y_qingId))//本地已映射的科目不再映射
                            {
                                qingCourses.Add(course);
                            }

                        }
                        yunEntities.Configuration.AutoDetectChangesEnabled = false;
                        yunEntities.Configuration.ValidateOnSaveEnabled = false;
                        yunEntities.Set<YD_Edu_QingCourse>().AddRange(qingCourses);
                        yunEntities.SaveChanges();
                        //yunEntities.BulkInsert(qingCourses);
                        //yunEntities.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 同步学生
        /// </summary>
        /// <returns></returns>
        public bool TongBuStudent()
        {
            string content =
                "{\"data\":{\"displayName\":\"\",\"gender\":\"\",\"idCard\":\"\",\"phoneNum\":\"\",\"centerId\":0,\"majorId\":0,\"semesterId\":0,\"studentNumber\":\"\",\"studyStatusId\":0,\"pageIndex\":0,\"pageSize\":99999}}";
            sen = sing(content);
            string content1 = "{\"data\":{\"displayName\":\"\",\"gender\":\"\",\"idCard\":\"\",\"phoneNum\":\"\",\"centerId\":0,\"majorId\":0,\"semesterId\":0,\"studentNumber\":\"\",\"studyStatusId\":0,\"pageIndex\":0,\"pageSize\":99999},\"signature\": \"" + sen + "\"}";
            html = WebPost("http://www.qingshuxuetang.com/QingShuSchoolWebService/jxsf/v1/student/search", content1, Encoding.UTF8);
            if (!html.Contains("服务器"))
            {
                var results = JsonConvert.DeserializeObject<QingShuStuMessage>(html);

                if (results.hr == 0)
                {
                    var list = results.data;
                    using (var yunEntities = new IYunEntities())
                    {
                        var qingCourses = new List<YD_Edu_QingStuInfo>();
                        for (var i = 0; i < list.Count; i++)
                        {
                            var qingStu = new YD_Edu_QingStuInfo();
                            qingStu.y_qingId = list[i].id;
                            qingStu.y_name = list[i].displayName;
                            qingStu.y_stuNum = list[i].studentNumber;

                            qingStu.y_qingSubId = list[i].centerId;
                            qingStu.y_qingMajorId = list[i].majorId;
                            var stuNum = list[i].name.Replace("jxsf_", "");
                            var obj = yunEntities.YD_Sts_StuInfo.FirstOrDefault(u => u.y_stuNum == stuNum);
                            if (obj == null)
                            {
                                continue;
                            }
                            qingStu.y_nid = obj.id;
                            qingStu.y_cardId = obj.y_cardId;
                            if (!yunEntities.YD_Edu_QingStuInfo.Any(u => u.y_qingId == qingStu.y_qingId))//本地已映射的学生不再映射
                            {
                                qingCourses.Add(qingStu);
                            }

                        }
                        yunEntities.Configuration.AutoDetectChangesEnabled = false;
                        yunEntities.Configuration.ValidateOnSaveEnabled = false;
                        yunEntities.Set<YD_Edu_QingStuInfo>().AddRange(qingCourses);
                        yunEntities.SaveChanges();
                        //yunEntities.BulkInsert(qingCourses);
                        //yunEntities.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public List<QingShuStuScore> SearchScore(int pageIndex, int courseId)
        {
            var qingShuScores = new List<QingShuStuScore>();
            string content ="{\"data\":{\"centerId\":0,\"majorId\":0,\"semesterId\":0,\"studentId\":0,\"courseId\":" + courseId + ",\"pageIndex\":" + pageIndex + ",\"pageSize\":500}}";
            sen = sing(content);
            string content1 = "{\"data\":{\"centerId\":0,\"majorId\":0,\"semesterId\":0,\"studentId\":0,\"courseId\":" + courseId + ",\"pageIndex\":" + pageIndex + ",\"pageSize\":500},\"signature\": \"" + sen + "\"}";
            html = WebPost(baseurl, content1, Encoding.UTF8);
            if (!html.Contains("服务器"))
            {
                var results = JsonConvert.DeserializeObject<QingShuMessage>(html);
                if (results.hr == 0)
                {
                    return results.data;
                }
                return qingShuScores;
            }
            return qingShuScores;
        }
        /// <summary>
        /// 将成绩数据进行导入
        /// </summary>
        /// <returns></returns>
        public string UploadScore(List<QingShuStuScore> scores)
        {
            scores = scores.Where(u => u.examScore != 0m).ToList();//未参加考试的成绩不同步
            using (var yunEntities = new IYunEntities())
            {
                var scoreList = new List<YD_Edu_Score>();
                for (var i = 0; i < scores.Count; i++)
                {
                    var score = new YD_Edu_Score();
                    var courseId = scores[i].courseId;
                    var obj = yunEntities.YD_Edu_QingCourse.FirstOrDefault(u => u.y_qingId == courseId);
                    if (obj == null)
                    {
                        continue;
                    }
                    score.y_courseId = obj.y_nid;
                    score.y_normalScore = scores[i].usualScore;
                    score.y_termScore = scores[i].examScore;
                    score.y_totalScore = scores[i].finalScore;
                    var stuId = scores[i].studentId;
                    var stu = yunEntities.YD_Edu_QingStuInfo.FirstOrDefault(u => u.y_qingId == stuId);
                    if (stu == null)
                    {
                        continue;
                    }
                    score.y_stuId = stu.y_nid;
                    score.y_term = scores[i].term;
                    if (!yunEntities.YD_Edu_Score.Any(u => u.y_courseId == obj.y_nid && u.y_stuId == stu.y_nid && u.y_term == score.y_term))//本地已有的成绩不同步
                    {
                        scoreList.Add(score);
                    }
                }
                yunEntities.Configuration.AutoDetectChangesEnabled = false;
                yunEntities.Configuration.ValidateOnSaveEnabled = false;
                yunEntities.Set<YD_Edu_Score>().AddRange(scoreList);
                yunEntities.SaveChanges();
                //yunEntities.BulkInsert(scoreList);
                //yunEntities.BulkSaveChanges();
                return "同步完成";
            }
        }

        public string en(string content)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(publickey);
            byte[] bytes = new UTF8Encoding().GetBytes(content);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }

        public string de(string content)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(privatekey);
            byte[] rgb = Convert.FromBase64String(content);
            byte[] buffer2 = provider.Decrypt(rgb, false);
            return new UTF8Encoding().GetString(buffer2);
        }



        public string WebPost(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/json";
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        public string sing(string content)
        {
            byte[] Data = Encoding.UTF8.GetBytes(content);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); ;
            SHA1 sh = new SHA1CryptoServiceProvider();
            rsa.FromXmlString(privatekey);

            byte[] signData = rsa.SignData(Data, sh);
            return Convert.ToBase64String(signData);
        }

    }
}
