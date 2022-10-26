using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Areas.Admin.Controllers
{
    public class AuthController : BaseController
    {
        private ApplicationUserManager _userManager;
        private DataBaseContext Db = new DataBaseContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            ViewBag.countTrash = UserManager.Users.Where(a => a.Status == "0").Count();
            var list = from a in UserManager.Users
                       where a.Status != "0"
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in UserManager.Users
                       where (a.Email.Contains(search) || a.Id.ToString().Contains(search) || a.FullName.Contains(search)) && a.Status != "0"
                       orderby a.Create_at descending
                       select a;
            }
            var rs = list.ToPagedList(pageNumber, pageSize);

            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Trash(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in UserManager.Users
                       where a.Status == "0"
                       orderby a.Create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in UserManager.Users
                       where (a.Email.Contains(search) || a.Id.ToString().Contains(search) || a.FullName.Contains(search)) && a.Status == "0"
                       orderby a.Create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        //GET:Auth/Details
        public ActionResult Details(int id)
        {
            var user = UserManager.FindById(id);
            ViewBag.ListAddress = Db.AccountAddresses.Where(m => m.UserId == id).ToList();
            if (user == null)
            {
                Notification.SetNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //GET:Auth/ChangeRoles
        public JsonResult ChangeRoles(int accountID, int roleID)
        {
            var user = UserManager.FindById(accountID);
            bool result = false;
            try
            {
                if (user != null && IsAdminRole())
                {
                    var UserRoles = UserManager.GetRoles(accountID).ToArray();
                    UserManager.RemoveFromRoles(accountID, UserRoles);
                    UserManager.AddToRole(accountID, GetRoleName(roleID));
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET:Auth/Disable
        public JsonResult Disable(int id)
        {
            string result = "error";
            var user =UserManager.FindById(id);
            try
            {
                if (User!=null && base.User.Identity.GetUserId<int>() != id)
                {
                    result = "success";
                    user.Status = "0";
                    UserManager.Update(user);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET:Auth/IsActive
        public JsonResult IsActive(int id)
        {
            string result = "error";
            var user = UserManager.FindById(id);
            try
            {
                result = "success";
                user.Status = "1";
                UserManager.Update(user);
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
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }
        #region Helpers
        private bool IsAdminRole()
        {
            int UserId = User.Identity.GetUserId<int>();
            return UserManager.IsInRole(UserId, "Admin");
        }
        private string GetRoleName(int RoleId)
        {
            if (RoleId == 0) return "Admin";
            return "Customer";
        }
        #endregion
    }
}