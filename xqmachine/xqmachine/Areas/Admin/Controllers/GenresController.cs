using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;

namespace xqmachine.Areas.Admin.Controllers
{
    public class GenresController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Admin/Genres
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in Db.Genres
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in Db.Genres
                       where a.Genre_name.Contains(search)
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Genres/Create
        [HttpPost]
        public JsonResult Create(string genreName, Genre genre)
        {
            string result = "false";
            try
            {
                Genre checkExist = Db.Genres.SingleOrDefault(m => m.Genre_name == genreName);
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                genre.Genre_name = genreName;
                genre.Create_by = User.Identity.GetUserId();
                genre.Update_by = User.Identity.GetUserId();
                genre.Create_at = DateTime.Now;
                genre.Update_at = DateTime.Now;
                Db.Genres.Add(genre);
                Db.SaveChanges();
                result = "success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Genres/Edit
        public JsonResult Edit(int id, string genreName)
        {
            string result = "error";
            Genre genre = Db.Genres.FirstOrDefault(m => m.Genre_id == id);
            var checkExist = Db.Genres.SingleOrDefault(m => m.Genre_name == genreName);
            try
            {
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result = "success";
                genre.Genre_name = genreName;
                genre.Update_at = DateTime.Now;
                genre.Update_by = User.Identity.GetUserName();
                Db.Entry(genre).State = EntityState.Modified;
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Genres/Delete
        public ActionResult Delete(int id)
        {
            string result = "error";
            Genre genre = Db.Genres.FirstOrDefault(m => m.Genre_id == id);
            try
            {
                result = "delete";
                Db.Genres.Remove(genre);
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}