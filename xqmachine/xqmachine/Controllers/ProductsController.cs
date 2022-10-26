using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Controllers
{
    public class ProductsController : Controller
    {
        private DataBaseContext Db = new DataBaseContext();
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        // GET: Products/Machines
        public ActionResult Machines(int? page, string sortOrder, string rangeOrder, int[] brandOrder, int[] genreOrder)
        {
            ViewBag.Type = "Máy";
            ViewBag.SortBy = "Machine";
            ViewBag.CountProduct = Db.Products.Where(m => m.Type == 1).Count();
            //brand oder
            var ls = Db.Products.AsEnumerable<Product>().Where(j => j.Type == 1).ToList();
            ViewBag.lsBrand = Db.Brands
                .AsEnumerable<Brand>()
                .Where(i => ls.Any(j => j.  Brand_id == i.Brand_id))
                .Select(i => Helper.GetTagLinkHTML(i.Brand_id, i.Brand_name, brandOrder));
            //genre oder
            ViewBag.lsGenre = Db.Genres
                .AsEnumerable<Genre>()
                .Where(i => ls.Any(j => j.Genre_id == i.Genre_id))
                .Select(i => Helper.GetTagLinkHTML(i.Genre_id, i.Genre_name, genreOrder));
            //select list sort
            ViewBag.lsSort = new SelectList(Helper.GetListSort(), "val", "txt", sortOrder);
            //slider amount
            ViewBag.rangeBy = Helper.GetNumber(rangeOrder);
            // request model
            string strQuery = " ( P.Status.Equals(\"1\") && P.Type== 1 ) && " + Helper.GetQuery(rangeOrder, brandOrder, genreOrder);
            return View("Product", GetProduct(strQuery, page, sortOrder));
        }

        // GET: Products/Accessories
        public ActionResult Accessories(int? page, string sortOrder, string rangeOrder, int[] brandOrder, int[] genreOrder)
        {
            ViewBag.Type = "Phụ kiện";
            ViewBag.SortBy = "accessory";
            ViewBag.CountProduct = Db.Products.Where(m => m.Type == 2).Count();
            //brand oder
            var ls = Db.Products.AsEnumerable<Product>().Where(j => j.Type == 2).ToList();
            ViewBag.lsBrand = Db.Brands
                .AsEnumerable<Brand>()
                .Where(i => ls.Any(j => j.Brand_id == i.Brand_id))
                .Select(i => Helper.GetTagLinkHTML(i.Brand_id, i.Brand_name, brandOrder));
            //genre oder
            ViewBag.lsGenre = Db.Genres
                .AsEnumerable<Genre>()
                .Where(i => ls.Any(j => j.Genre_id == i.Genre_id))
                .Select(i => Helper.GetTagLinkHTML(i.Genre_id, i.Genre_name, genreOrder));
            //select list sort
            ViewBag.lsSort = new SelectList(Helper.GetListSort(), "val", "txt", sortOrder);
            //slider amount
            ViewBag.rangeBy = Helper.GetNumber(rangeOrder);
            //request model
            string strQuery = " ( P.Status.Equals(\"1\") && P.Type== 2 ) && " + Helper.GetQuery(rangeOrder, brandOrder, genreOrder);
            return View("Product", GetProduct(strQuery, page, sortOrder));
        }

        // GET: Products/ProductDetail
        public ActionResult ProductDetail(int id, int? page)
        {
            int pagesize = 1;
            int cpage = page ?? 1;
            //var product = db.Product.SingleOrDefault(m => m.status == "1" && m.Product_id == id);
            var product = Helper.GetListProduct(Db).SingleOrDefault(i => i.P.Status.Equals("1") && i.P.Product_id == id);
            if (product == null)
            {
                return Redirect("/");
            }
            //sản phẩm liên quan
            ViewBag.relatedproduct = Helper.GetListProduct(Db).Where(item => item.P.Status.Equals("1") && item.P.Product_id != id && item.P.Genre_id == product.P.Genre_id).Take(8).ToList();
            ViewBag.ProductImage = Db.ProductImages.Where(item => item.Product_id == id).ToList();
            ViewBag.ListFeedback = Db.Feedbacks.Where(m => m.Stastus == "2").ToList();
            //ViewBag.ListReplyFeedback = db.ReplyFeedback.Where(m => m.stastus == "2").ToList();
            ViewBag.OrderFeedback = Db.Oder_Detail.ToList();
            var comment = Db.Feedbacks.Where(m => m.Product_id == id && m.Stastus == "2").OrderByDescending(m => m.Create_at).ToList();
            ViewBag.CountFeedback = comment.Count();
            ViewBag.PagerFeedback = comment.ToPagedList(cpage, pagesize);
            ViewBag.ListReplyFeedback = Db.ReplyFeedbacks.Where(m => m.Stastus == "2").ToList();
            product.P.View++;
            Db.SaveChanges();
            return View(product);
        }

        // POST: Products/ProductComment
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult ProductComment(Feedback comment, int productID, int discountID, int genreID, int rateStar, string commentContent)
        {
            bool result = false;
            int userID = User.Identity.GetUserId<int>();
            if (User.Identity.IsAuthenticated)
            {
                comment.UserId = userID;
                comment.Rate_star = rateStar;
                comment.Product_id = productID;
                comment.Disscount_id = discountID;
                comment.Genre_id = genreID;
                comment.Content = commentContent;
                comment.Stastus = "2";
                comment.Create_at = DateTime.Now;
                comment.Update_at = DateTime.Now;
                comment.Create_by = userID.ToString();
                comment.Update_by = userID.ToString();
                Db.Feedbacks.Add(comment);
                Db.SaveChanges();
                result = true;
                Notification.SetNotification3s("Bình luận thành công", "success");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Products/ReplyComment
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ReplyComment(int id, string reply_content, ReplyFeedback reply)
        {
            bool result = false;
            if (User.Identity.IsAuthenticated)
            {
                reply.Feedback_id = id;
                reply.UserId = User.Identity.GetUserId<int>();
                reply.Content = reply_content;
                reply.Stastus = "2";
                reply.Create_at = DateTime.Now;
                Db.ReplyFeedbacks.Add(reply);
                Db.SaveChanges();
                result = true;
                Notification.SetNotification3s("Phản hồi thành công", "success");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Products/SearchResult
        public ActionResult SearchResult(int? page, string s, string sortOrder, string rangeOrder, int[] brandOrder, int[] genreOrder)
        {

            ViewBag.SortBy = "/search";
            ViewBag.Type = "";
            ViewBag.SearchString = s;
            ViewBag.CountProduct = Db.Products.Where(m => m.Type == 2).Count();
            //brand oder
            var ls = Db.Products.AsEnumerable<Product>().Where(j => j.Type == 2).ToList();
            ViewBag.lsBrand = Db.Brands
                .AsEnumerable<Brand>()
                .Where(i => ls.Any(j => j.Brand_id == i.Brand_id))
                .Select(i => Helper.GetTagLinkHTML(i.Brand_id, i.Brand_name, brandOrder));
            //genre oder
            ViewBag.lsGenre = Db.Genres
                .AsEnumerable<Genre>()
                .Where(i => ls.Any(j => j.Genre_id == i.Genre_id))
                .Select(i => Helper.GetTagLinkHTML(i.Genre_id, i.Genre_name, genreOrder));
            //select list sort
            ViewBag.lsSort = new SelectList(Helper.GetListSort(), "val", "txt", sortOrder);
            //slider amount
            ViewBag.rangeBy = Helper.GetNumber(rangeOrder);
            var Products = GetProduct(
                "(p.Status.Equals(\"1\") && (p.Product_name.Contains(\"" + s + "\") || p.Product_id.ToString().Contains(\"" + s + "\"))) &&" +
                Helper.GetQuery(rangeOrder, brandOrder, genreOrder),
                page,
                sortOrder
            );
            ViewBag.Countproduct = Products.Count();
            return View("Product", Products);
        }

        #region Helpers
        //Get product 
        private IPagedList<ListProduct> GetProduct(string expr, int? page, string sortOrder)
        {
            sortOrder = sortOrder ?? "2";
            int.TryParse(sortOrder, out int idOrder);
            int pageSize = 9; //1 trang hiện thỉ tối đa 9 sản phẩm
            int pageNumber = (page ?? 1); //đánh số trang
            //if (idOrder == default || idOrder < 0 || idOrder > 6)
            //    return Helper.GetListProduct(Db).AsQueryable<ListProduct>().Where(expr).ToPagedList(pageNumber, pageSize);
            string[] lsSort = {
                "(P.Buyturn + P.View) desc",
                "P.Create_at",
                "P.Create_at desc",
                "(P.Price - P.Discount.Discount_price)",
                "(P.Price - P.Discount.Discount_price) desc",
                "P.Product_name",
                "P.Product_name desc"
            };
            string exprSort = lsSort[idOrder];
            var list = Helper.GetListProduct(Db).AsQueryable<ListProduct>().Where(expr).OrderBy(exprSort).ToPagedList(pageNumber, pageSize);
            //Helper.GetListProduct(Db).AsQueryable<ListProduct>().Where(expr).OrderBy(i => (i.P.Buyturn + i.P.View)).ToPagedList(pageNumber, pageSize);
            ViewBag.Showing = list.Count();
            return list;
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
        #endregion
    }
}