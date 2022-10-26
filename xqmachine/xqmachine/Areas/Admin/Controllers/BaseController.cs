using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace xqmachine.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        public BaseController()
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Account/Login");
            }
            else
            {
                if (!System.Web.HttpContext.Current.User.IsInRole("Admin"))
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Home/Index");
                }
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("~/Home/Index");
        }

        //chuyển từ trang admin sang trang thông tin cá nhân
        public ActionResult ViewProfile()
        {
            return Redirect("~/Account/Editprofile");
        }

        //chuyển từ trang admin sang trang chủ
        public ActionResult BackToHome()
        {
            return Redirect("~/Home/Index");
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
    }
}