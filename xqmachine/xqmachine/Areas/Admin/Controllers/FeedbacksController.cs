using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;

namespace xqmachine.Areas.Admin.Controllers
{
    public class FeedbacksController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();

        // GET: Admin/Feedbacks
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in Db.Feedbacks
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in Db.Feedbacks
                       where a.UserId.ToString().Contains(search)
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Feedbacks/ReplyComment
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
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
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