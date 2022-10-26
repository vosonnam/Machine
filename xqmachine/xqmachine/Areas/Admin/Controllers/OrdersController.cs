using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using System.Linq.Dynamic.Core;
using PagedList;
using xqmachine.Models.Helpers;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace xqmachine.Areas.Admin.Controllers
{
    public class OrdersController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Areas/Orders
        public ActionResult Index(string search, string rangeby, string orderby, int? size, int? page)
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            ViewBag.countTrash = Db.Orders.Where(a => a.Status == "0").Count(); //  đếm tổng sp có trong thùng rác
            string Query = "!status.Equals(\"0\")";
            if (!string.IsNullOrEmpty(search))
            {
                Query += String.Format(" && ( Order_id.ToString().Contains(\"{0}\") || OrderAddress.OrderUsername.Contains(\"{0}\") || OrderAddress.OrderPhonenumber.Contains(\"{0}\") )", search);
            }
            if (!string.IsNullOrEmpty(rangeby))
            {
                DateTime FromDate = DateTime.ParseExact(rangeby.Split('-')[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime ToDate = DateTime.ParseExact(rangeby.Split('-')[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Query += String.Format(" && (DateTime({0},{1},{2})<=Create_at && Create_at<=DateTime({3},{4},{5}) )", FromDate.Year, FromDate.Month, FromDate.Day, ToDate.Year, ToDate.Month, ToDate.Day);
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                if (!orderby.Equals("0"))
                {
                    Query += String.Format(" && Status.Equals(\"{0}\")", orderby);
                }
            }
            var list = Db.Orders.Where(Query).Select(i => i).OrderByDescending(i => i.Create_at);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: Areas/Orders/Trash
        public ActionResult Trash(string search, int? size, int? page)
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            var list = from a in Db.Orders
                       where a.Status == "0"
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in Db.Orders
                       where a.Order_id.ToString().Contains(search)
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: Areas/Orders/Details
        public ActionResult Details(int? id)
        {
            Order order = Db.Orders.FirstOrDefault(m => m.Order_id == id);
            ViewBag.ListProduct = Db.Oder_Detail.Where(m => m.Order_id == order.Order_id).ToList();
            ViewBag.OrderHistory = Db.Orders.Where(m => m.UserId == order.UserId).OrderByDescending(m => m.Oder_date).Take(10).ToList();
            if (order == null)
            {
                Notification.SetNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Areas/Orders/UpdateOrder
        public JsonResult UpdateOrder(int id, string status)
        {
            string result = "error";
            Order order = Db.Orders.FirstOrDefault(m => m.Order_id == id);
            try
            {
                if (order.Status != "3")
                {
                    result = "success";
                    order.Status = status;
                    order.Update_at = DateTime.Now;
                    order.Update_by = User.Identity.GetUserId();
                    Db.Entry(order).State = EntityState.Modified;
                    Db.SaveChanges();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = "false";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Areas/Orders/CancleOrder
        public JsonResult CancleOrder(int id)
        {
            string result = "error";
            Order order = Db.Orders.FirstOrDefault(m => m.Order_id == id);
            try
            {
                if (order.Status != "3")
                {
                    result = "success";
                    order.Status = "0";
                    order.Update_at = DateTime.Now;
                    order.Update_by = User.Identity.GetUserName();
                    Db.Entry(order).State = EntityState.Modified;
                    Db.SaveChanges();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = "false";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
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