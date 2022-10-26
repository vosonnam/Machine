using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;
using xqmachine.Models;

namespace xqmachine.Controllers
{
    public class HomeController : Controller
    {
        private DataBaseContext Db = new DataBaseContext();

        //GET:Home/Index
        public ActionResult Index()
        {
            ViewBag.HotProduct = (
                from p in Db.Products.Where(i => i.Status.Equals("1") && !i.Quantity.Equals("0"))
                join q in (
                    from fb in Db.Feedbacks.Where(i => i.Stastus.Equals("2"))
                    join od in Db.Oder_Detail.Where(i => i.Status.Equals("3"))
                    on new { fb.Product_id, fb.UserId } equals new { od.Product_id, od.Order.UserId }
                    group fb by fb.Product_id into groupStar
                    select new
                    {
                        Product_id = groupStar.Key,
                        star = groupStar.Sum(i => i.Rate_star) / groupStar.Count()
                    }
                ) on p.Product_id equals q.Product_id into result
                from r in result.DefaultIfEmpty()
                select new ListProduct
                {
                    P = p,
                    Star = r.star == null ? 5 : r.star
                })
                .OrderBy(i => i.P.Buyturn + i.P.View)
                .Take(8)
                .ToList();
            ViewBag.NewProduct = (
                from p in Db.Products.Where(i => i.Status.Equals("1") && !i.Quantity.Equals("0"))
                join q in (
                    from fb in Db.Feedbacks.Where(i => i.Stastus.Equals("2"))
                    join od in Db.Oder_Detail.Where(i => i.Status.Equals("3"))
                    on new { fb.Product_id, fb.UserId } equals new { od.Product_id, od.Order.UserId }
                    group fb by fb.Product_id into groupStar
                    select new
                    {
                        Product_id = groupStar.Key,
                        star = groupStar.Sum(i => i.Rate_star) / groupStar.Count()
                    }
                ) on p.Product_id equals q.Product_id into result
                from r in result.DefaultIfEmpty()
                select new ListProduct
                {
                    P = p,
                    Star = r.star == null ? 5 : r.star
                })
                .OrderByDescending(i => i.P.Create_at)
                .Take(8)
                .ToList();
            ViewBag.Machine = (
                from p in Db.Products.Where(i => i.Status.Equals("1") && !i.Quantity.Equals("0") && i.Type==1)
                join q in (
                    from fb in Db.Feedbacks.Where(i => i.Stastus.Equals("2"))
                    join od in Db.Oder_Detail.Where(i => i.Status.Equals("3"))
                    on new { fb.Product_id, fb.UserId } equals new { od.Product_id, od.Order.UserId }
                    group fb by fb.Product_id into groupStar
                    select new
                    {
                        Product_id = groupStar.Key,
                        star = groupStar.Sum(i => i.Rate_star) / groupStar.Count()
                    }
                ) on p.Product_id equals q.Product_id into result
                from r in result.DefaultIfEmpty()
                select new ListProduct
                {
                    P = p,
                    Star = r.star == null ? 5 : r.star
                })
                .OrderByDescending(i => i.P.Buyturn + i.P.View)
                .Take(8)
                .ToList();
            ViewBag.Accessory = (
                from p in Db.Products.Where(i => i.Status.Equals("1") && !i.Quantity.Equals("0") && i.Type==2)
                join q in (
                    from fb in Db.Feedbacks.Where(i => i.Stastus.Equals("2"))
                    join od in Db.Oder_Detail.Where(i => i.Status.Equals("3"))
                    on new { fb.Product_id, fb.UserId } equals new { od.Product_id, od.Order.UserId }
                    group fb by fb.Product_id into groupStar
                    select new
                    {
                        Product_id = groupStar.Key,
                        star = groupStar.Sum(i => i.Rate_star) / groupStar.Count()
                    }
                ) on p.Product_id equals q.Product_id into result
                from r in result.DefaultIfEmpty()
                select new ListProduct
                {
                    P = p,
                    Star = r.star == null ? 5 : r.star
                })
                .OrderByDescending(i => i.P.Buyturn + i.P.View)
                .Take(8)
                .ToList();
            return View();
        }

        //GET:Home/Contact
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(Models.Contact model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Models.db.Contact contact = new Models.db.Contact(model);
            Db.Contacts.Add(contact);
            Db.SaveChanges();
            return RedirectToAction ("Index","Home");
        }

        //GET:Home/PageNotFound
        public ActionResult PageNotFound()
        {
            return View();
        }

        //GET:Home/PreviewHeader
        public PartialViewResult PreviewHeader()
        {
            var ls = Db.Products.AsEnumerable<Product>().Where(j => j.Type == 1).ToList();
            ViewBag.lsBrand = Db.Brands.AsEnumerable()
                .Where(i => ls.Any(j => j.Brand_id == i.Brand_id))
                .Select(i =>
                    $"<li><a href=\"machine?brandOrder={i.Brand_id}\">{i.Brand_name}</a></li>"
                );
            return PartialView("_PartialHeader");
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