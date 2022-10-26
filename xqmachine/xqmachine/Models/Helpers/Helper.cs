using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using xqmachine.Models.db;

namespace xqmachine.Models.Helpers
{
    public class Helper
    {
        public static bool IsRole(ICollection<CustomUserRole> Roles,int UserId, int RoleId)
        {
            int CountRole = Roles.Where(i => i.RoleId == RoleId).Count();
            return (CountRole > 0);
        }
        public static bool IsRole(ICollection<AspNetRole> Roles, int UserId, int RoleId)
        {
            int CountRole = Roles.Where(i => i.Id == RoleId).Count();
            return (CountRole > 0);
        }

        public static string GetContentOrder(string orderID, string orderItem, string orderDiscount, string orderPrice, string orderTotal, string contentWard, string district, string province)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "EmailOrders" + ".cshtml");
            body = body.Replace("{{order_id}}", orderID);
            body = body.Replace("{{order_item}}", orderItem);
            body = body.Replace("{{order_discount}}", orderDiscount);
            body = body.Replace("{{order_price}}", orderPrice);
            body = body.Replace("{{total}}", orderTotal);
            body = body.Replace("{{contet_ward}}", contentWard);
            body = body.Replace("{{district}}", district);
            body = body.Replace("{{province}}", province);
            return body;
        }

        public static string GetContentResetPassword(string LinkHref, string LinkTxt)
        {
            string Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "ResetPassword" + ".cshtml");
            Body = Body.Replace("{{viewBag.Confirmlink}}", LinkHref); //hiển thị nội dung lên form html
            Body = Body.Replace("{{viewBag.Confirmlink}}", LinkTxt);//hiển thị nội dung lên form html
            return Body;
        }

        public static string GetStar(double i)
        {
            if (i >= 1 && i < 2)
            {
                return "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>";
            }
            else if ((i >= 2) && (i < 3))
            {
                return "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>";
            }
            else if ((i >= 3) && (i < 4))
            {
                return "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>";
            }
            else if ((i >= 4) && (i < 5))
            {
                return "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>";
            }
            else if (i == 5)
            {
                return "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>" +
                       "<li class='fill'><i class='ion-android-star'></i></li>";
            }
            else
            {
                return "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>" +
                       "<li class='empty'><i class='ion-android-star'></i></li>";
            }
        }

        public static IEnumerable<ListProduct> GetListProduct(DataBaseContext db)
        {
            return from p in db.Products
                   join q in (
                       from fb in db.Feedbacks.Where(i => i.Stastus.Equals("2"))
                       join od in db.Oder_Detail.Where(i => i.Status.Equals("3"))
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
                   };
        }
        public static List<object> GetListSort()
        {

            var rs = new[]{
                new {val="2",txt= "Mặc định"},
                new { val = "0", txt = " Sắp Xếp Theo Phổ Biến"},
                new { val = "1", txt = "Sắp Xếp Theo Ngày: Cũ - Mới"},
                new { val = "2", txt = "Sắp Xếp Theo Ngày: Mới - Cũ"},
                new { val = "3", txt = "Sắp Xếp Theo Giá: Cao - Thấp" },
                new { val = "4", txt = "Sắp Xếp Theo Giá: Thấp - Cao" },
                new { val = "5", txt = "Sắp Xếp Theo Tên: A - Z" },
                new { val = "6", txt = "Sắp Xếp Theo Tên: Z - A" }
            };
            return rs.ToList<object>();
        }

        public static string GetQuery(string rangeBy, int[] brandBy, int[] genreBy)
        {
            string rs = " 1==1 ";
            if (brandBy != null && brandBy.Length != 0)
                rs += " && ( P.Brand_id==" + string.Join(" || P.Brand_id==", brandBy) + " ) ";
            if (genreBy != null && genreBy.Length != 0)
                rs += " && ( P.Genre_id==" + string.Join(" || P.Genre_id==", genreBy) + " ) ";
            if (rangeBy != null && rangeBy.Length != 0)
            {
                double minAmount, maxAmount, step = 1000000;
                minAmount = GetNumber(rangeBy.Trim())[0] * step;
                maxAmount = GetNumber(rangeBy.Trim())[1] * step;
                rs += " && ( " + minAmount + " <= (P.Price-P.Discount.Discount_price) && (P.Price-P.Discount.Discount_price)<= " + maxAmount + " ) ";
            }
            return rs;
        }

        public static string GetStringNumber(string str)
        {
            return Regex.Match(str, @"\d+").Value;
        }
        public static double[] GetNumber(string str)
        {
            if (str == null || str.Length == 0)
                return null;
            double[] rs ={
                double.Parse(GetStringNumber(str.Split('-')[0])),
                double.Parse(GetStringNumber(str.Split('-')[1]))
            };
            return rs;
        }
        public static bool IsExists(int i, int[] arr)
        {
            if (arr == null)
                return false;
            if (Array.IndexOf(arr, i) > -1)
                return true;
            return false;
        }

        public static string GetTagLinkHTML(int id, string name, int[] arr)
        {
            if (IsExists(id, arr))
                return $"<a data-id='{id}' class='action'>{name}</a>";
            return $"<a data-id='{id}'>{name}</a>";
        }
    }

    #region HelperModels

    public class ListProduct
    {
        public Product P { get; set; }
        public double Star { get; set; }
    }

    public class ListReplayFeedback
    {
        public IEnumerable<ReplyFeedback> Rp { get; set; }
        public int Feedback_id { get; set; }
    }
    public class LsBrand
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ProductDTOs
    {
        public string Product_name { get; set; }
        public string Genre_name { get; set; }
        public string Brand_name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string Quantity { get; set; }
        public int Product_id { get; set; }
        public string Ceate_by { get; set; }
        public DateTime Create_at { get; set; }
        public string Update_by { get; set; }
        public DateTime Update_at { get; set; }
        public string Status { get; set; }
        public long View { get; set; }
        public double Discount_price { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Discount_start { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Discount_end { get; set; }

        public string Discount_name { get; set; }
        public double Discount_id { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
    #endregion
}