using IYun.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace IYun.Controllers
{
    /// <summary>
    /// 下载专区
    /// </summary>
    public class FileManagerController : AdminBaseController
    {

        //
        // GET: /File/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FileList(int id = 1)
        {
            #region “文件列表”权限验证
            var power = SafePowerPage("/FileManager/FileList");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            var filename = Request["y_name"];
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 112); //根据父栏目ID获取兄弟栏目
                ViewBag.adminroleid = YdAdminRoleId;
                IQueryable<YD_Sys_File> list = yunEntities.YD_Sys_File.Where(x => x.y_role == 0 || x.y_role == YdAdminRoleId).
                    OrderByDescending(x => x.y_time);
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    list = list.Where(x => x.y_name.Contains(filename));
                }
                var pagedList = list.ToPagedList(id, 15);
                if (Request.IsAjaxRequest())
                    return PartialView("FilePage", pagedList);
                return View(pagedList);
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            #region “文件上传”权限验证

            var power = SafePowerPage("/FileManager/UploadFile");
            if (!IsLogin())
            {
                return Redirect("/AdminBase/Index");
            }
            if (power == null || power.y_menu == (int)PowerState.Disable)
            {
                //var reurl = Request.UrlReferrer.ToString();
                var reurl = "/AdminBase/Index";
                return Content("<script>alert('没有权限');window.location.href='" + reurl + "'</script>");
            }
            #endregion
            using (var yunEntities = new IYunEntities())
            {
                ViewBag.modulePowers = GetChildModulePower(yunEntities, 112); //根据父栏目ID获取兄弟栏目
                ViewBag.adminroleid = YdAdminRoleId;
            }
            return View();
        }

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase filebase)
        {
            string fileName = Request["filename"] as string;
            var role = Request["role"];
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Json("文件名不能为空");
            }
            if (Request.Files.Count == 0)
            {
                return Json("文件不能为空");
            }
            var file = Request.Files[0];
            if (file.ContentLength == 0 || file.ContentLength > 10 * 1024 * 1024)
            {
                return Json("文件大小超出限制范围(10MB)");
            }
            else
            {
                int roleid = 0;
                if (!string.IsNullOrWhiteSpace(role))
                {
                    roleid = Convert.ToInt32(role);
                }
                string target = Server.MapPath("~/File/Upload/");
                string currentFileName = fileName + DateTime.Now.ToString("yyyyMMddhhmmss");
                string path = target + currentFileName + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                file.SaveAs(path);
                using (var yunEntities = new IYunEntities())
                {
                    yunEntities.YD_Sys_File.Add(new YD_Sys_File()
                    {
                        y_name = fileName,
                        y_path = path,
                        y_createdName = YdAdminRelName,
                        y_createduser = YdAdminId,
                        y_role = roleid,
                        y_time = DateTime.Now
                    });
                    int flag = yunEntities.SaveChanges();
                    if (flag > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }

            }

        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadFile(int? id)
        {
            if (id > 0)
            {
                using (var yunEntities = new IYunEntities())
                {
                    var file = yunEntities.YD_Sys_File.FirstOrDefault(x => x.id == id);
                    string filePath = file.y_path;
                    return File(new FileStream(filePath, FileMode.Open), "text/plain",
                        file.y_name + filePath.Substring(filePath.LastIndexOf('.')));
                }
            }
            return null;
        }

        public JsonResult FileDelete(int? id)
        {
            if (id > 0)
            {
                using (var yunEntities = new IYunEntities())
                {
                    var me = yunEntities.YD_Sys_File.FirstOrDefault(u => u.id == id);
                    yunEntities.YD_Sys_File.Remove(me);
                    var j = yunEntities.SaveChanges();
                    if (j > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json("删除失败");
                    }
                }
            }
            else
            {
                return Json("id不能为空");
            }
        }
    }

}
