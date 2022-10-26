using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Areas.Admin.Controllers
{
    public class DashBoardsController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();
        // GET: Admin/DashBoards
        public ActionResult Index()
        {
            ViewBag.Order = Db.Orders.ToList();
            ViewBag.OrderDetail = Db.Oder_Detail.ToList();
            ViewBag.ListOrderDetail = Db.Oder_Detail.OrderByDescending(m => m.Create_at).Take(3).ToList();
            ViewBag.ListOrder = Db.Orders.Take(7).ToList();
            var rs = User.Identity.GetAvatar();
            return View();
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