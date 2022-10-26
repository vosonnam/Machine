using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using xqmachine.Models;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Controllers
{
    public class CartController : Controller
    {
        #region define
        private DataBaseContext Db = new DataBaseContext();
        private ApplicationUserManager _userManager;

        public CartController()
        {
        }

        public CartController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        #endregion

        #region Action

        // GET: Cart/PreviewCart
        public PartialViewResult PreviewCart()
        {
            var cart = this.GetCart();
            ViewBag.Quans = cart.Item2;
            var listProduct = cart.Item1.ToList();
            return PartialView("PreviewCart", listProduct);
        }

        //Xem giỏ hàng
        // GET: Cart/ViewCart
        public ActionResult ViewCart()
        {
            var cart = this.GetCart();
            ViewBag.Quans = cart.Item2;
            double discount = 0d;
            var listProduct = cart.Item1.ToList();
            if (Session["Discount"] != null && Session["Discountcode"] != null)
            {
                var code = Session["Discountcode"].ToString();
                var discountupdatequan = Db.Discounts.Where(d => d.Discount_code == code).FirstOrDefault();
                if (discountupdatequan.Quantity == 0 || discountupdatequan.Discount_star >= DateTime.Now || discountupdatequan.Discount_end <= DateTime.Now)
                {
                    Notification.SetNotification3s("Mã giảm giá không thể sử dụng", "error");
                    return View(listProduct);
                }
                discount = Convert.ToDouble(Session["Discount"].ToString());
                Session.Remove("Discount");
                Session.Remove("Discountcode");
                return View(listProduct);
            }
            return View(listProduct);
        }

        //Thanh toán giỏ hàng
        // GET: Cart/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            int userId = User.Identity.GetUserId<int>();
            //var user = db.Accounts.SingleOrDefault(u => u.account_id == userId);
            var user = UserManager.FindById(userId);
            var cart = this.GetCart();
            ViewBag.Quans = cart.Item2;
            ViewBag.ListProduct = cart.Item1.ToList();
            ViewBag.CountAddress = Db.AccountAddresses.Where(m => m.UserId == userId).Count();
            ViewBag.ListDistrict = Db.Districts.OrderBy(m => m.District_name).ToList();
            ViewBag.ListProvince = Db.Provinces.OrderBy(m => m.Province_name).ToList();
            ViewBag.ListWard = Db.Wards.ToList().OrderBy(m => m.Ward_name).ToList();
            ViewBag.ListAddress = Db.AccountAddresses.Where(m => m.UserId == userId).OrderByDescending(m => m.IsDefault).ToList();
            ViewBag.MyAddress = Db.AccountAddresses.FirstOrDefault(u => u.UserId == userId && u.IsDefault == true);
            if (cart.Item2.Count < 1)
            {
                return RedirectToAction(nameof(ViewCart));
            }
            var products = cart.Item1;
            double discount = 0d;
            double total = TotalPrice();
            // Áp dụng mã giảm giá
            if (Session["Discount"] != null)
            {
                discount = Convert.ToDouble(Session["Discount"].ToString());
                total -= discount;
            }
            ViewBag.Total = total;
            ViewBag.Discount = discount;
            return View();
        }

        // GET: Cart/Payment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Payment(OrderAddress orderAdress)
        {

            var CartInfor = GetCart();
            string url = ConfigurationManager.AppSettings["VNP:Url"];
            string returnUrl = ConfigurationManager.AppSettings["VNP:Returnurl"];
            string tmnCode = ConfigurationManager.AppSettings["VNP:TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["VNP:HashSecret"];

            for (int i = 0; i < CartInfor.Item1.Count(); i++)
            {
                if (!IsStockingItem(CartInfor.Item1[i], CartInfor.Item2[i]))
                {
                    Notification.SetNotification3s("Sản phẩm đã hết hàng " + CartInfor.Item1[i].Product_name, "error");
                    return RedirectToAction("ViewCart", "Cart");
                }
            }

            double total = TotalPrice() + 30000;
            if (Session["Discount"] != null && Session["Discountcode"] != null)
            {
                string check_discount = Session["Discountcode"].ToString();
                var discount = Db.Discounts.Where(d => d.Discount_code == check_discount).SingleOrDefault();
                if (discount.Quantity == 0 || discount.Discount_star >= DateTime.Now || discount.Discount_end <= DateTime.Now)
                {
                    Notification.SetNotification3s("Mã giảm giá không thể sử dụng", "error");
                    return RedirectToAction("ViewCart", "Cart");
                }
                else
                {
                    total -= discount.Discount_price;
                }
            }

            if (Request.Form["option_payment"].Equals("COD"))
            {
                var culture = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
                double priceSum = TotalPrice();
                string orderItem = "";
                if (Session["Discount"] != null && Session["Discountcode"] != null)
                {
                    string check_discount = Session["Discountcode"].ToString();
                    Discount discountupdatequan = Db.Discounts.Where(d => d.Discount_code == check_discount).SingleOrDefault();
                    if (discountupdatequan != null)
                    {
                        discountupdatequan.Quantity--;
                    }
                }
                orderAdress.TimesEdit = 0;
                Db.OrderAddresses.Add(orderAdress);

                var order = new Order()
                {
                    UserId = User.Identity.GetUserId<int>(),
                    Create_at = DateTime.Now,
                    Create_by = User.Identity.GetUserId().ToString(),
                    Status = "1",
                    Order_note = Request.Form["OrderNote"].ToString(),
                    Delivery_id = 1,
                    OrderAddressId = orderAdress.OrderAddressId,
                    Oder_date = DateTime.Now,
                    Update_at = DateTime.Now,
                    Payment_id = 1,
                    Update_by = User.Identity.GetUserId().ToString(),
                    Total = total
                };

                var listProduct = new List<Product>();
                for (int i = 0; i < CartInfor.Item1.Count; i++)
                {
                    var item = CartInfor.Item1[i];
                    var _price = item.Price;
                    if (item.Discount != null)
                    {
                        if (item.Discount.Discount_star < DateTime.Now && item.Discount.Discount_end > DateTime.Now)
                        {
                            _price = item.Price - item.Discount.Discount_price;
                        }
                    }
                    order.Oder_Detail.Add(new Oder_Detail
                    {
                        Create_at = DateTime.Now,
                        Create_by = User.Identity.GetUserId().ToString(),
                        Disscount_id = item.Disscount_id,
                        Genre_id = item.Genre_id,
                        Price = _price,
                        Product_id = item.Product_id,
                        Quantity = CartInfor.Item2[i],
                        Status = "1",
                        Update_at = DateTime.Now,
                        Update_by = User.Identity.GetUserId().ToString(),
                        Transection = "transection"
                    });

                    Response.Cookies["product_" + item.Product_id].Expires = DateTime.Now.AddDays(-10);

                    var product = Db.Products.SingleOrDefault(p => p.Product_id == item.Product_id);
                    product.Buyturn += CartInfor.Item2[i];
                    product.Quantity = (Convert.ToInt32(product.Quantity ?? "0") - CartInfor.Item2[i]).ToString();
                    listProduct.Add(product);

                    orderItem += "<tr style='margin'> <td align='left' width='75%' style=' padding: 6px 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; overflow: hidden; text-overflow: ellipsis; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;' >" +
                                product.Product_name + "</td><td align='left' width='25%' style=' padding: 6px 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; ' >" + product.Price.ToString("#,0₫", culture.NumberFormat) + "</td> </tr>";
                }

                Db.Orders.Add(order);

                Db.Configuration.ValidateOnSaveEnabled = false;
                await Db.SaveChangesAsync();
                Notification.SetNotification3s("Nhân viên sẽ liên hệ và hướng dẫn hoàn tất giao dịch", "success");

                //remove discount code 
                Session.Remove("Discount");
                Session.Remove("Discountcode");

                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                string emailID = user.Email;
                string orderID = order.Order_id.ToString();
                string orderDiscount = (priceSum + 30000 - order.Total).ToString("#,0₫", culture.NumberFormat);
                string orderPrice = priceSum.ToString("#,0₫", culture.NumberFormat);
                string orderTotal = order.Total.ToString("#,0₫", culture.NumberFormat);
                string contentWard = Db.Wards.Where(i => i.Ward_id == orderAdress.Ward_id).Select(i => i.Type + " " + i.Ward_name).FirstOrDefault();
                string district = Db.Districts.Where(i => i.District_id == orderAdress.District_id).Select(i => i.Type + " " + i.District_name).FirstOrDefault();
                string province = Db.Provinces.Where(i => i.Province_id == orderAdress.Province_id).Select(i => i.Type + " " + i.Province_name).FirstOrDefault();
                if (!IsSystemEmail(emailID))
                {
                    SendVerificationLinkEmail(emailID, orderID, orderItem, orderDiscount, orderPrice, orderTotal, contentWard, district, province); //nếu muốn gửi email đơn hàng thì bật lên
                }
                return RedirectToAction("TrackingOrder", "Account");

            }
            else
            {
                TempData["Form"] = Request.Form;
                TempData["orderAddress"] = orderAdress;

                string BankCode = "VNMART".Equals(Request.Form["option_payment"]) ? Request.Form["option_payment"] : Request.Form["paymethod"];

                VnPayLibrary pay = new VnPayLibrary();

                pay.AddRequestData("vnp_Version", "2.1.0");
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", tmnCode);
                total *= 100;
                pay.AddRequestData("vnp_Amount", total.ToString());
                pay.AddRequestData("vnp_BankCode", BankCode);
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", returnUrl);
                pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

                return Redirect(paymentUrl);
            }
        }

        public async Task<ActionResult> PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["VNP:HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                VnPayLibrary pay = new VnPayLibrary();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {

                        var culture = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
                        double priceSum = TotalPrice();
                        double t = Convert.ToDouble(pay.GetResponseData("vnp_Amount"));
                        string orderItem = "";

                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;

                        var form = (NameValueCollection)TempData["Form"];
                        var orderAdress= (OrderAddress)TempData["orderAddress"];

                        Discount discountupdatequan;
                        if (Session["Discount"] != null && Session["Discountcode"] != null)
                        {
                            string check_discount = Session["Discountcode"].ToString();
                            discountupdatequan = Db.Discounts.Where(d => d.Discount_code == check_discount).SingleOrDefault();
                            discountupdatequan.Quantity--;
                        }

                        orderAdress.TimesEdit = 0;
                        Db.OrderAddresses.Add(orderAdress);

                        var order = new Order()
                        {
                            UserId = User.Identity.GetUserId<int>(),
                            Create_at = DateTime.Now,
                            Create_by = User.Identity.GetUserId().ToString(),
                            Status = "1",
                            Order_note = form["OrderNote"].ToString(),
                            Delivery_id = 1,
                            OrderAddressId = orderAdress.OrderAddressId,
                            Oder_date = DateTime.Now,
                            Update_at = DateTime.Now,
                            Payment_id = 1,
                            Update_by = User.Identity.GetUserId().ToString(),
                            Total = Convert.ToDouble(pay.GetResponseData("vnp_Amount"))/100
                        };

                        var CartInfor = GetCart();
                        var listProduct = new List<Product>();
                        for (int i = 0; i < CartInfor.Item1.Count; i++)
                        {
                            var item = CartInfor.Item1[i];
                            var _price = item.Price;
                            if (item.Discount != null)
                            {
                                if (item.Discount.Discount_star < DateTime.Now && item.Discount.Discount_end > DateTime.Now)
                                {
                                    _price = item.Price - item.Discount.Discount_price;
                                }
                            }
                            order.Oder_Detail.Add(new Oder_Detail
                            {
                                Create_at = DateTime.Now,
                                Create_by = User.Identity.GetUserId().ToString(),
                                Disscount_id = item.Disscount_id,
                                Genre_id = item.Genre_id,
                                Price = _price,
                                Product_id = item.Product_id,
                                Quantity = CartInfor.Item2[i],
                                Status = "1",
                                Update_at = DateTime.Now,
                                Update_by = User.Identity.GetUserId().ToString(),
                                Transection = "transection"
                            });

                            Response.Cookies["product_" + item.Product_id].Expires = DateTime.Now.AddDays(-10);

                            var product = Db.Products.SingleOrDefault(p => p.Product_id == item.Product_id);
                            product.Buyturn += CartInfor.Item2[i];
                            product.Quantity = (Convert.ToInt32(product.Quantity ?? "0") - CartInfor.Item2[i]).ToString();
                            listProduct.Add(product);

                            orderItem += "<tr style='margin'> <td align='left' width='75%' style=' padding: 6px 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; overflow: hidden; text-overflow: ellipsis; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;' >" +
                                        product.Product_name + "</td><td align='left' width='25%' style=' padding: 6px 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; ' >" + product.Price.ToString("#,0₫", culture.NumberFormat) + "</td> </tr>";
                        }

                        Db.Orders.Add(order);

                        Db.Configuration.ValidateOnSaveEnabled = false;
                        await Db.SaveChangesAsync();
                        Notification.SetNotification3s("Đặt hàng thành công", "success");

                        //remove discount code 
                        Session.Remove("Discount");
                        Session.Remove("Discountcode");

                        TempData.Remove("Form");
                        TempData.Remove("orderAddress");

                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>()); 
                        string emailID = user.Email;
                        string orderID = order.Order_id.ToString();
                        string orderDiscount = (priceSum + 30000 - order.Total).ToString("#,0₫", culture.NumberFormat);
                        string orderPrice = priceSum.ToString("#,0₫", culture.NumberFormat);
                        string orderTotal = order.Total.ToString("#,0₫", culture.NumberFormat);
                        string contentWard = Db.Wards.Where(i=>i.Ward_id== orderAdress.Ward_id).Select(i=>i.Type+" "+i.Ward_name).FirstOrDefault();
                        string district = Db.Districts.Where(i => i.District_id == orderAdress.District_id).Select(i => i.Type + " " + i.District_name).FirstOrDefault();
                        string province = Db.Provinces.Where(i => i.Province_id == orderAdress.Province_id).Select(i => i.Type + " " + i.Province_name).FirstOrDefault();
                        if (!IsSystemEmail(emailID))
                        {
                            SendVerificationLinkEmail(emailID, orderID, orderItem, orderDiscount, orderPrice, orderTotal, contentWard, district, province); //nếu muốn gửi email đơn hàng thì bật lên
                        }
                        return RedirectToAction("TrackingOrder", "Account");
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

        //Gửi email đơn hàng
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string orderID, string orderItem, string orderDiscount, string orderPrice, string orderTotal, string contentWard, string district, string province)
        {
            int UserId = User.Identity.GetUserId<int>(); 
            string subject = "Thông tin đơn hàng #" + orderID;
            string body = Helper.GetContentOrder(orderID, orderItem, orderDiscount, orderPrice, orderTotal, contentWard, district, province);
            UserManager.SendEmailAsync(UserId, subject, body);
        }

        //Áp dụng mã giảm giá
        // GET: Cart/UseDiscountCode
        public ActionResult UseDiscountCode(string code)
        {
            var discount = Db.Discounts.SingleOrDefault(d => d.Discount_code == code);
            if (discount != null)
            {
                if (discount.Discount_star < DateTime.Now && discount.Discount_end > DateTime.Now && discount.Quantity != 0)
                {
                    Session["Discountcode"] = discount.Discount_code;
                    Session["Discount"] = discount.Discount_price;
                    return Json(new { success = true, discountPrice = discount.Discount_price }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, discountPrice = 0 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //Mapping sản phẩm lên view
        private Tuple<List<Product>, List<int>> GetCart()
        {
            var quantities = new List<int>();
            var listProduct = new List<Product>();
            if (Request.Browser.Cookies)
            {
                var cart = Request.Cookies.AllKeys.Where(c => c.IndexOf("product_") == 0);
                if (cart.Count() == 0)
                    return new Tuple<List<Product>, List<int>>(listProduct, quantities);
                var productIds = new List<int>();
                var errorProduct = new List<string>();
                // Lấy mã sản phẩm & số lượng trong giỏ hàng
                foreach (var item in cart)
                {
                    string pattern = @"^product_\d+$";
                    var cValue = Request.Cookies[item].Value;
                    if (String.IsNullOrEmpty(cValue) || cValue == "0" || !Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                    {
                        Response.Cookies[item].Expires = DateTime.Now.AddDays(-1);
                    }
                    else
                    {
                        productIds.Add(Convert.ToInt32(item.Split('_')[1]));
                        quantities.Add(Convert.ToInt32(cValue));
                    }
                }
                // Select sản phẩm để hiển thị
                listProduct = Db.Products.Where(i => i.Status.Equals("1") && productIds.Any(j => j == i.Product_id)).ToList();
                errorProduct = productIds.Where(i => Db.Products.All(j => j.Product_id != i)).Select(i => $"product_{i}").ToList<string>();
                //Xóa sản phẩm bị lỗi khỏi cart
                foreach (var err in errorProduct)
                {
                    Response.Cookies[err].Expires = DateTime.Now.AddDays(-1);
                }
            }
            else
            {
                Notification.SetNotification3s("Trình duyệt không hỗ trợ Cookie", "error");
            }
            //check null 
            return new Tuple<List<Product>, List<int>>(listProduct, quantities);//lấy id sản phẩm và số lượng
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
        #region Helpers
        private bool IsSystemEmail (string Email)
        {
            string SysEmail = ConfigurationManager.AppSettings["Email:Id"];
            return SysEmail.Equals(Email);
        }
        private bool IsStockingItem(Product product,int num)
        {
            int count = Db.Products.AsEnumerable().Where(p => p.Product_id == product.Product_id && int.Parse(p.Quantity) >= num).Count();
            return count > 0;
        }
        private double TotalPrice()
        {
            var CartInfor = GetCart();
            return CartInfor.Item1
                .Select((item, index) =>
                {
                    double productPrice;
                    if (item.Discount != null)
                    {
                        if (item.Discount.Discount_star < DateTime.Now && item.Discount.Discount_end > DateTime.Now)
                        {
                            return productPrice = (item.Price - item.Discount.Discount_price) * CartInfor.Item2[index];
                        }
                    }
                    return productPrice = item.Price * CartInfor.Item2[index];
                }).Sum();
        }
        #endregion
    }
}