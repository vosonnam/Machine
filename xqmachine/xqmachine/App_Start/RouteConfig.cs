using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace xqmachine
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //rút gọn link tìm kiếm sản phẩm
            routes.MapRoute(
                name: "search",
                url: "search",
                defaults: new { Controller = "Products", action = "SearchResult" },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link chi tiết sản phẩm
            routes.MapRoute(
                name: "chi tiet san pham",
                url: "{slug}-{id}",
                defaults: new { Controller = "Products", action = "ProductDetail" },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link laptop
            routes.MapRoute(
                name: "Machine",
                url: "machine",
                defaults: new { Controller = "Products", action = "Machines" },
                namespaces: new[] { "xqmachine.Controllers" }
            );

            //rút gọn link phụ kiện
            routes.MapRoute(
                name: "phu kien",
                url: "accessory",
                defaults: new { Controller = "Products", action = "Accessories" },
                namespaces: new[] { "xqmachine.Controllers" }
            );

            //rút gọn link giỏ hàng
            routes.MapRoute(
                name: "Thanh toan",
                url: "checkout",
                defaults: new { controller = "Cart", action = "Checkout", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link giỏ hàng
            routes.MapRoute(
                name: "Gio Hang",
                url: "cart",
                defaults: new { controller = "Cart", action = "ViewCart", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );

            //rút gọn link thong tin ca nhan
            routes.MapRoute(
                name: "Thong tin ca nhan",
                url: "infor",
                defaults: new { controller = "Account", action = "Editprofile", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link forgot password
            routes.MapRoute(
                name: "forgotpassword",
                url: "forgot_password",
                defaults: new { controller = "Account", action = "ForgotPassword", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );

            //rút gọn link đăng nhập
            routes.MapRoute(
                name: "Dang nhap",
                url: "login",
                defaults: new { controller = "Account", action = "Login" },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link đăng ký
            routes.MapRoute(
                name: "Dang ky",
                url: "register",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //thay đổi mật khảu
            routes.MapRoute(
                name: "doi mat khau",
                url: "change_password",
                defaults: new { controller = "Account", action = "ChangePassword", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //xem chi tiết đơn hàng
            routes.MapRoute(
                name: "chi tiet don hang",
                url: "order_detail/{id}",
                defaults: new { controller = "Account", action = "TrackingOrderDetail", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //quản lý địa chỉ
            routes.MapRoute(
                name: "quan ly dia chi",
                url: "list_address",
                defaults: new { controller = "Account", action = "Address", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //rút gọn link quản lý đơn hàng
            routes.MapRoute(
                name: "lich su mua hang",
                url: "list_order",
                defaults: new { controller = "Account", action = "TrackingOrder", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //reset password
            routes.MapRoute(
                name: "Reset password",
                url: "reset_password",
                defaults: new { controller = "Account", action = "ResetPassword", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //gửi yêu cầu hồ trợ
            routes.MapRoute(
                name: "sent request",
                url: "request",
                defaults: new { controller = "Home", action = "SentRequest", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //set error 404
            routes.MapRoute(
                name: "Page Not Found",
                url: "pagenotfound/{id}",
                defaults: new { controller = "Home", action = "PageNotFound", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
            //link mặc định khi khởi động
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "xqmachine.Controllers" }
            );
        }
    }
}
