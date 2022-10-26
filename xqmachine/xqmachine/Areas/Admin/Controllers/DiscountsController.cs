using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;

namespace xqmachine.Areas.Admin.Controllers
{
    public class DiscountsController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Admin/Discounts
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in Db.Discounts
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in Db.Discounts
                       where a.Discount_name.Contains(search) || a.Discount_price.ToString().Contains(search)
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Discounts/Create
        [HttpPost]
        public JsonResult Create(string discountStart, string discountEnd, double discountPrice, string discountCode,int quantity)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            try
            {
                var _discount = Db.Discounts.Where(i => i.Discount_code.Equals(discountCode)).SingleOrDefault();
                if(_discount!=null)
                    return Json(false, JsonRequestBehavior.AllowGet);
                DateTime _discountStart = DateTime.ParseExact(discountStart, "MM'-'dd'-'yyyy HH:mm", CultureInfo.InvariantCulture)
                    , _discountEnd = DateTime.ParseExact(discountEnd, "MM'-'dd'-'yyyy HH:mm", CultureInfo.InvariantCulture);
                //double _discountPrice = double.Parse(discountPrice);
                //int _quantity = int.Parse(quantity);
                Db.Discounts.Add(new Discount {
                    Discount_name= "Giảm " +
                        discountPrice.ToString("#,0₫", cul.NumberFormat) + " Từ " +
                        _discountStart.ToString("dd-MM-yyyy") + " => " +
                        _discountEnd.ToString("dd-MM-yyyy"),
                    Discount_price= discountPrice,
                    Quantity= quantity,
                    Discount_star= _discountStart,
                    Discount_end= _discountEnd,
                    Discount_code= discountCode,
                    Create_by = User.Identity.GetUserId(),
                    Create_at = DateTime.Now,
                    Update_by= User.Identity.GetUserId(),
                    Update_at = DateTime.Now
                });
                Db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Discounts/Edit
        public JsonResult Edit(int id, string discountStart, string discountEnd, double discountPrice, string discountCode, int quantity)
        {
            string result = "error";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            Discount discount = Db.Discounts.FirstOrDefault(m => m.Disscount_id == id);
            if (discount == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DateTime _discountStart = DateTime.ParseExact(discountStart, "MM'-'dd'-'yyyy HH:mm", CultureInfo.InvariantCulture)
                    , _discountEnd = DateTime.ParseExact(discountEnd, "MM'-'dd'-'yyyy HH:mm", CultureInfo.InvariantCulture);
                discount.Discount_name = "Giảm " +
                        discountPrice.ToString("#,0₫", cul.NumberFormat) + " Từ " +
                        _discountStart.ToString("dd-MM-yyyy") + " => " +
                        _discountEnd.ToString("dd-MM-yyyy");
                discount.Discount_price = discountPrice;
                discount.Discount_star = _discountStart;
                discount.Discount_end = _discountEnd;
                discount.Quantity = quantity;
                discount.Discount_code = discountCode;
                discount.Update_at = DateTime.Now;
                discount.Update_by = User.Identity.GetUserName();
                Db.Entry(discount).State = EntityState.Modified;
                Db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Discounts/Delete
        public ActionResult Delete(int id)
        {
            string result = "error";
            Discount discount = Db.Discounts.FirstOrDefault(m => m.Disscount_id == id);
            try
            {
                result = "delete";
                Db.Discounts.Remove(discount);
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