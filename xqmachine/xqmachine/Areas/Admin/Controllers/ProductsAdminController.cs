using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Areas.Admin.Controllers
{
    public class ProductsAdminController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Admin/ProductsAdmin
        public ActionResult Index(string search, int? size, int? page) // hiển thị tất cả sp online
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            ViewBag.countTrash = Db.Products.Where(a => a.Status == "0").Count(); //  đếm tổng sp có trong thùng rác
            var list = from a in Db.Products
                       join c in Db.Genres on a.Genre_id equals c.Genre_id
                       join d in Db.Brands on a.Brand_id equals d.Brand_id
                       join e in Db.Discounts on a.Disscount_id equals e.Disscount_id
                       where a.Status == "1"
                       orderby a.Create_at descending // giảm dần
                       select new Models.Helpers.ProductDTOs
                       {
                           Discount_start = (DateTime)e.Discount_star,
                           Discount_end = (DateTime)e.Discount_end,
                           Discount_name = e.Discount_name,
                           Discount_price = e.Discount_price,
                           Product_name = a.Product_name,
                           Quantity = a.Quantity,
                           Price = a.Price,
                           Image = a.Image,
                           Genre_name = c.Genre_name,
                           View = a.View,
                           Brand_name = d.Brand_name,
                           Product_id = a.Product_id
                       };
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.Product_name.Contains(search) || s.Product_id.ToString().Contains(search));
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Trash(string search, int? size, int? page) // hiển thị tất cả sp online
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            var list = from a in Db.Products
                       join c in Db.Genres on a.Genre_id equals c.Genre_id
                       join d in Db.Brands on a.Brand_id equals d.Brand_id
                       join e in Db.Discounts on a.Disscount_id equals e.Disscount_id
                       where a.Status == "0"
                       orderby a.Create_at descending // giảm dần
                       select new ProductDTOs
                       {
                           Discount_start = (DateTime)e.Discount_star,
                           Discount_end = (DateTime)e.Discount_end,
                           Discount_name = e.Discount_name,
                           Discount_price = e.Discount_price,
                           Product_name = a.Product_name,
                           Quantity = a.Quantity,
                           Price = a.Price,
                           Image = a.Image,
                           Genre_name = c.Genre_name,
                           View = a.View,
                           Brand_name = d.Brand_name,
                           Product_id = a.Product_id
                       };
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.Product_name.Contains(search) || s.Product_id.ToString().Contains(search));
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: Areas/ProductsAdmin/Details/5

        public ActionResult Details(int? id)
        {
            Product product = Db.Products.FirstOrDefault(m => m.Product_id == id);
            if (product == null)
            {
                Notification.SetNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Areas/ProductsAdmin/Create
        public ActionResult Create() //Tạo sản phẩm
        {
            ViewBag.ListDiscount =
                new SelectList(Db.Discounts.OrderBy(m => m.Discount_price), "Disscount_id", "Discount_name", 0);
            ViewBag.ListBrand = new SelectList(Db.Brands, "Brand_id", "Brand_name", 0);
            ViewBag.ListGenre = new SelectList(Db.Genres, "Genre_id", "Genre_name", 0);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Product product, ProductImage productImage)
        {
            ViewBag.ListDiscount =
                new SelectList(Db.Discounts.OrderBy(m => m.Discount_price), "Disscount_id", "Discount_name", 0);
            ViewBag.ListBrand = new SelectList(Db.Brands, "Brand_id", "Brand_name", 0);
            ViewBag.ListGenre = new SelectList(Db.Genres, "Genre_id", "Genre_name", 0);
            try
            {
                if (product.ImageUpload != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                    var extension = Path.GetExtension(product.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("HH-mm-dd-MM-yyyy") + extension;
                    product.Image = "/Content/Images/" + fileName;
                    product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                }
                else
                {
                    Notification.SetNotification3s("Vui lòng thêm Ảnh Thumbnail!", "error");
                    return View(product);
                }
                product.Status = "1";
                product.View = 0;
                product.Buyturn = 0;
                product.Type = product.Type;
                product.Specifications = product.Specifications;
                product.Description = product.Description;
                product.Create_at = DateTime.Now;
                product.Create_by = User.Identity.GetUserId().ToString();
                product.Update_at = DateTime.Now;
                product.Update_by = User.Identity.GetUserId().ToString();
                Db.Products.Add(product);
                Db.SaveChanges();
                foreach (HttpPostedFileBase image_multi in product.ImageUploadMulti)
                {
                    if (image_multi != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(image_multi.FileName);
                        var extension = Path.GetExtension(image_multi.FileName);
                        fileName = fileName + DateTime.Now.ToString("HH-mm-dd-MM-yyyy") + extension;
                        productImage.Image = "/Content/Images/" + fileName;
                        image_multi.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                        productImage.Product_id = product.Product_id;
                        productImage.Disscount_id = product.Disscount_id;
                        productImage.Genre_id = product.Genre_id;
                        Db.ProductImages.Add(productImage);
                        Db.SaveChanges();
                    }
                }
                Notification.SetNotification1_5s("Thêm mới thành công!", "success");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Notification.SetNotification1_5s("Lỗi", "error");
                ModelState.AddModelError("", e.ToString());
                return View(product);
            }
        }

        // GET: Areas/ProductsAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.ListDiscount = new SelectList(Db.Discounts.OrderBy(m => m.Discount_price), "Disscount_id", "Discount_name", 0);
            ViewBag.ListBrand = new SelectList(Db.Brands, "Brand_id", "Brand_name", 0);
            ViewBag.ListGenre = new SelectList(Db.Genres, "Genre_id", "Genre_name", 0);
            var product = Db.Products.FirstOrDefault(x => x.Product_id == id);
            if (product == null || id == null)
            {
                Notification.SetNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Product model, ProductImage productImage)
        {
            ViewBag.ListDiscount = new SelectList(Db.Discounts.OrderBy(m => m.Discount_price), "Disscount_id", "Discount_name", 0);
            ViewBag.ListBrand = new SelectList(Db.Brands, "Brand_id", "Brand_name", 0);
            ViewBag.ListGenre = new SelectList(Db.Genres, "Genre_id", "Genre_name", 0);
            var product = Db.Products.SingleOrDefault(x => x.Product_id == model.Product_id);
            try
            {
                if (model.ImageUpload != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageUpload.FileName);
                    var extension = Path.GetExtension(model.ImageUpload.FileName);
                    fileName = fileName + extension;
                    product.Image = "/Content/Images/" + fileName;
                    model.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                }
                product.Product_name = model.Product_name;
                product.Quantity = model.Quantity;
                product.Description = model.Description;
                product.Specifications = model.Specifications;
                product.Price = model.Price;
                product.Brand_id = model.Brand_id;
                product.Type = model.Type;
                product.Update_at = DateTime.Now;
                product.Update_by = User.Identity.GetUserName();
                Db.Entry(product).State = EntityState.Modified;
                Db.SaveChanges();
                Notification.SetNotification1_5s("Đã cập nhật lại thông tin!", "success");
                return RedirectToAction("Index");
            }
            catch
            {
                Notification.SetNotification1_5s("Lỗi", "error");
                return View(model);
            }
        }
        public JsonResult Disable(int id)
        {
            string result = "error";
            Product product = Db.Products.FirstOrDefault(m => m.Product_id == id);
            try
            {
                result = "disabled";
                product.Status = "0";
                Db.Entry(product).State = EntityState.Modified;
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Undo(int id)
        {
            string result = "error";
            Product product = Db.Products.FirstOrDefault(m => m.Product_id == id);
            try
            {
                result = "activate";
                product.Status = "1";
                Db.Entry(product).State = EntityState.Modified;
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int id)
        {
            string result = "error";
            Product product = Db.Products.FirstOrDefault(m => m.Product_id == id);
            try
            {
                List<ProductImage> listImage = Db.ProductImages.Where(m => m.Product_id == id).ToList();
                foreach (var item in listImage)
                {
                    Db.ProductImages.Remove(item);
                    Db.SaveChanges();
                }
                result = "delete";
                Db.Products.Remove(product);
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
        #region Helpers
        #endregion
    }
}