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
    public class BrandsController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Admin/Brands
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in Db.Brands
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in Db.Brands
                       where a.Brand_name.Contains(search)
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Brands/Create
        [HttpPost]
        public JsonResult Create(string brandName, Brand brand)
        {
            string result = "false";
            try
            {
                Brand checkExist = Db.Brands.SingleOrDefault(m => m.Brand_name == brandName);
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                brand.Brand_name = brandName;
                brand.Create_by = User.Identity.GetUserId();
                brand.Update_by = User.Identity.GetUserId();
                brand.Create_at = DateTime.Now;
                brand.Update_at = DateTime.Now;
                Db.Brands.Add(brand);
                Db.SaveChanges();
                result = "success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Brands/Edit
        public JsonResult Edit(int id, string brandName)
        {
            string result = "error";
            Brand brand = Db.Brands.FirstOrDefault(m => m.Brand_id == id);
            var checkExist = Db.Brands.SingleOrDefault(m => m.Brand_name == brandName);
            try
            {
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result = "success";
                brand.Brand_name = brandName;
                brand.Update_at = DateTime.Now;
                brand.Update_by = User.Identity.GetUserName();
                Db.Entry(brand).State = EntityState.Modified;
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Brands/Delete
        public ActionResult Delete(int id)
        {
            string result = "error";
            Brand brand = Db.Brands.FirstOrDefault(m => m.Brand_id == id);
            try
            {
                result = "delete";
                Db.Brands.Remove(brand);
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