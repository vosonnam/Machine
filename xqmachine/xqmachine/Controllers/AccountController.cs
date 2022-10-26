using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models;
using xqmachine.Models.Helpers;
using xqmachine.Models.db;
using System.IO;
using System.Collections.Generic;
using PagedList;
using System.Configuration;

namespace xqmachine.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private DataBaseContext Db=new DataBaseContext();
        private readonly string RoleName = "Customer";

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindByEmail(model.Email);
            if (user != null)
            {
                if (user.Status.Equals("0"))
                {
                    Notification.SetNotification3s("Tài khoản đã bị vô hiệu, Liên hệ để biết thêm chi tiết!", "error");
                    return RedirectToAction("Login");
                }
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Email, mật khẩu không đúng, hoặc tài khoản bị vô hiệu hóa.");
                    Notification.SetNotification3s("Email, mật khẩu không đúng, hoặc tài khoản bị vô hiệu hóa", "error");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add user admin to Role Admin if not already added
                    var rolesForUser = UserManager.GetRoles(user.Id);
                    if (!rolesForUser.Contains(RoleName))
                    {
                        var roleresult = UserManager.AddToRole(user.Id, RoleName);
                        if (roleresult.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking:<br/>" + callbackUrl);

                            return RedirectToAction("Index", "Home");
                        }
                        AddErrors(roleresult);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId == default || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    Notification.SetNotification3s("Email không hợp lệ hoặc chưa được xác nhận!", "error");
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", Helper.GetContentResetPassword(callbackUrl, callbackUrl));
                Notification.SetNotification3s("Kiểm tra Email để hoàn tất thao tác!", "success");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == default)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = UserManager.FindByEmail(loginInfo.Email);
            if (user != null)
            {
                if (user.Status.Equals("0"))
                {
                    Notification.SetNotification3s("Tài khoản đã bị vô hiệu, Liên hệ để biết thêm chi tiết!", "error");
                    return RedirectToAction("Login");
                }
            }
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, FullName=loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user);
                // Add user admin to Role Admin if not already added
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        // Add user admin to Role Admin if not already added
                        var rolesForUser = UserManager.GetRoles(user.Id);
                        if (!rolesForUser.Contains(RoleName))
                        {
                            var roleresult = UserManager.AddToRole(user.Id, RoleName);
                            if (roleresult.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking:<br/>" + callbackUrl);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Notification.SetNotification1_5s("Đăng xuất thành công", "success");
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        #region OldAction
        //View cập nhật thông tin cá nhân
        public ActionResult Editprofile()
        {
            var userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            if (user != null)
            {
                return View(new AspNetUser(user));
            }
            return View();
        }
        //Code xử lý cập nhật thông tin cá nhân
        public JsonResult UpdateProfile(string userName, string phoneNumber)
        {
            bool result = false;
            var userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            if (user != null)
            {
                user.FullName = userName;
                user.PhoneNumber = phoneNumber;
                var rs = UserManager.UpdateAsync(user);
                result = rs.Result.Succeeded;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //Cập nhật ảnh đại diện
        public JsonResult UpdateAvatar()
        {
            var userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                fileName = fileName + extension;
                user.Avatar = "/Content/Images/" + fileName;
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                UserManager.UpdateAsync(user);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Notification.SetNotification3s("Dữ liệu không hợp lệ!", "error");
                return View(model);
            }
            var userId = User.Identity.GetUserId<int>();
            var result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                Notification.SetNotification3s("Đổi mật khẩu thành công", "success");
                return RedirectToAction("ChangePassword", "Account");
            }
            AddErrors(result);
            Notification.SetNotification3s("Mật khẩu mới và cũ không được trùng!", "error");
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId<int>();
                var result = await UserManager.AddPasswordAsync(userId, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    Notification.SetNotification3s("Đặt mật khẩu thành công!", "success");
                    return RedirectToAction("ChangePassword", "Account");
                }
                AddErrors(result);
            }
            Notification.SetNotification3s("Dữ liệu không hợp lệ!", "error");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //Quản lý sổ địa chỉ
        public ActionResult Address()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userID = User.Identity.GetUserId<int>();
                var address = Db.AccountAddresses.Where(m => m.UserId == userID).ToList();
                ViewBag.Check_address = Db.AccountAddresses.Where(m => m.UserId == userID).Count();
                ViewBag.ProvincesList = Db.Provinces.OrderBy(m => m.Province_name).ToList();
                ViewBag.DistrictsList = Db.Districts.OrderBy(m => m.Type).ThenBy(m => m.District_name).ToList();
                ViewBag.WardsList = Db.Wards.OrderBy(m => m.Type).ThenBy(m => m.Ward_name).ToList();
                return View(address);
            }
            return RedirectToAction("Index", "Home");
        }
        //Thêm mới địa chỉ 
        public ActionResult AddressCreate(AccountAddress address)
        {
            bool result = false;
            int userid = User.Identity.GetUserId<int>();
            var checkdefault = Db.AccountAddresses.Where(m => m.UserId == userid).ToList();
            var limit_address = Db.AccountAddresses.Where(m => m.UserId == userid).ToList();
            try
            {
                if (limit_address.Count() == 4)//tối đa 4 ký tự
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in checkdefault)
                {
                    if (item.IsDefault == true && address.IsDefault == true)
                    {
                        item.IsDefault = false;
                        Db.SaveChanges();
                    }
                }
                address.UserId = userid;
                Db.AccountAddresses.Add(address);
                Db.SaveChanges();
                result = true;
                Notification.SetNotification1_5s("Thêm thành công", "success");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //Sửa địa chỉ
        [HttpPost]
        public JsonResult AddressEdit(int id, string username, string phonenumber, int province_id, int district_id, int ward_id, string address_content)
        {
            var address = Db.AccountAddresses.FirstOrDefault(m => m.Account_address_id == id);
            bool result;
            if (address != null)
            {
                address.Province_id = province_id;
                address.AccountUsername = username;
                address.AccountPhoneNumber = phonenumber;
                address.District_id = district_id;
                address.Ward_id = ward_id;
                address.Content = address_content;
                address.UserId = User.Identity.GetUserId<int>();
                Db.SaveChanges();
                result = true;
            }
            else
            {
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Thay đổi địa chỉ mặc định
        public JsonResult DefaultAddress(int id)
        {
            bool result = false;
            int userid = User.Identity.GetUserId<int>();
            var address = Db.AccountAddresses.FirstOrDefault(m => m.Account_address_id == id);
            var checkdefault = Db.AccountAddresses.ToList();
            if (User.Identity.IsAuthenticated && !address.IsDefault == true)
            {
                foreach (var item in checkdefault)
                {
                    if (item.IsDefault == true && item.UserId == userid)
                    {
                        item.IsDefault = false;
                        Db.SaveChanges();
                    }
                }
                address.IsDefault = true;
                Db.SaveChanges();
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //Xóa địa chỉ
        [HttpPost]
        public JsonResult AddressDelete(int id)
        {
            bool result = false;
            try
            {
                var address = Db.AccountAddresses.FirstOrDefault(m => m.Account_address_id == id);
                Db.AccountAddresses.Remove(address);
                Db.SaveChanges();
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }
        //lấy danh sách quận huyện
        public JsonResult GetDistrictsList(int province_id)
        {
            Db.Configuration.ProxyCreationEnabled = false;
            List<District> districtslist = Db.Districts.Where(m => m.Province_id == province_id).OrderBy(m => m.Type).ThenBy(m => m.District_name).ToList();
            return Json(districtslist, JsonRequestBehavior.AllowGet);
        }
        //lấy danh sách phường xã
        public JsonResult GetWardsList(int district_id)
        {
            Db.Configuration.ProxyCreationEnabled = false;
            List<Ward> wardslist = Db.Wards.Where(m => m.District_id == district_id).OrderBy(m => m.Type).ThenBy(m => m.Ward_name).ToList();
            return Json(wardslist, JsonRequestBehavior.AllowGet);
        }
        //Lịch sử mua hàng
        public ActionResult TrackingOrder(int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("TrackingOrder", GetOrder(page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //Chi tiết đơn hàng đã mua
        public ActionResult TrackingOrderDetail(int id)
        {
            List<Oder_Detail> order = Db.Oder_Detail.Where(m => m.Order_id == id).ToList();
            ViewBag.Order = Db.Orders.FirstOrDefault(m => m.Order_id == id);
            ViewBag.OrderID = id;
            if (User.Identity.IsAuthenticated)
            {
                return View(order);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //đánh số trang
        private IPagedList GetOrder(int? page)
        {
            int userId = User.Identity.GetUserId<int>();
            int pageSize = 10;
            int pageNumber = (page ?? 1); //đánh số trang
            var list = Db.Orders.Where(m => m.UserId == userId).OrderByDescending(m => m.Order_id)
                .ToPagedList(pageNumber, pageSize);
            return list;
        }

        public ActionResult AddAddress()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [AllowAnonymous]
        public ActionResult UserLogged()
        {
            return Json(User.Identity.IsAuthenticated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }

                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }
        #endregion
    }
}