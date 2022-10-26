using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using xqmachine.Models.db;
using xqmachine.Models.Helpers;

namespace xqmachine.Areas.Admin.Controllers
{
    public class WareHouseController : BaseController
    {
        private DataBaseContext Db = new DataBaseContext();
        // GET: Admin/WareHouse
        public ActionResult Index(int? IsSuccess)
        {
            if (IsSuccess == 1)
            {
                ModelState.AddModelError("SuccesExportFile", String.Format("File export successfully at {0} .", DateTime.Now));
            }
            else if (IsSuccess == 2)
            {
                ModelState.AddModelError("SuccesUploadFiles", String.Format("File uploaded successfully at {0} .", DateTime.Now));
            }
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
            if ((System.IO.File.Exists(path)))
            {
                ViewBag.UploadedFile = String.Format("File uploaded at {0}", System.IO.File.GetLastWriteTime(path));
            }
            return View();
        }

        // POST: Admin/WareHouse/ExportFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportFile()
        {
            var stream = ExcelExport.CreateExcelFile();
            var buffer = stream as MemoryStream;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=XQMACHINEWareHouse.xlsx");
            Response.BinaryWrite(buffer.ToArray());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index", new { IsSuccess = 1 });
        }

        // POST: Admin/WareHouse/UploadFiles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFiles(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        var Inputfile = Request.Files[0];

                        if (Inputfile != null && Inputfile.ContentLength > 0)
                        {
                            var filename = Path.GetFileName(Inputfile.FileName);
                            var IsExcel = new Func<string, bool>(str => {
                                return !Regex.IsMatch(str, @"^.+\.(xlsx)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            });
                            if (IsExcel(filename))
                            {
                                ModelState.AddModelError("", "File must be type excel (xlsx).");
                                return View("Index");
                            }
                            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
                            if ((System.IO.File.Exists(path)))
                            {
                                System.IO.File.Delete(path);
                            }
                            Inputfile.SaveAs(path);
                            FileInfo fileInfo = new FileInfo(path);
                            using (ExcelPackage package = new ExcelPackage(fileInfo))
                            {
                                ExcelWorkbook workbook = package.Workbook;
                                var Errors = new List<ErrorList>();

                                ExcelWorksheet SeccondSheet = workbook.Worksheets["Genres"];
                                var genres = SeccondSheet.ConvertSheetToObjects<ExcelGenreModel>().ToList();
                                for (int i = 0; i < genres.Count(); i++)
                                {
                                    if (InsertOrUpdate(genres[i]))
                                        genres.RemoveAt(i);
                                }
                                Errors.AddRange(genres.Select(i => new ErrorList { Id = i.GenreId.ToString(), Type = "Genres" }));

                                ExcelWorksheet ThirdSheet = workbook.Worksheets["Brands"];
                                var brands = ThirdSheet.ConvertSheetToObjects<ExcelBrandModel>().ToList();
                                for (int i = 0; i < brands.Count(); i++)
                                {
                                    if (InsertOrUpdate(brands[i]))
                                        brands.RemoveAt(i);
                                }
                                Errors.AddRange(brands.Select(i => new ErrorList { Id = i.BrandId.ToString(), Type = "Brands" }));

                                ExcelWorksheet FourthSheet = workbook.Worksheets["Discounts"];
                                var discounts = FourthSheet.ConvertSheetToObjects<ExcelDiscountModel>().ToList();
                                for (int i = 0; i < discounts.Count(); i++)
                                {
                                    if (InsertOrUpdate(discounts[i]))
                                        discounts.RemoveAt(i);
                                }
                                Errors.AddRange(discounts.Select(i => new ErrorList { Id = i.DiscountId.ToString(), Type = "Discounts" }));

                                ExcelWorksheet FirstSheet = workbook.Worksheets["Products"];
                                var products = FirstSheet.ConvertSheetToObjects<ExcelProductModel>().ToList();
                                for (int i = 0; i < products.Count(); i++)
                                {
                                    if (InsertOrUpdate(products[i]))
                                        products.RemoveAt(i);
                                }
                                Errors.AddRange(products.Select(i => new ErrorList { Id = String.Format("{0}-{1}-{2}", i.ProductId, i.GenreId, i.DiscountId), Type = "Products" }));

                                workbook.Worksheets.Add("ErrorList");
                                if (Errors.Count() > 0)
                                {
                                    var ErrorSheet = workbook.Worksheets[5];
                                    ErrorSheet.Cells[1, 1].LoadFromCollection(Errors);
                                    package.Save();
                                }
                                else
                                {
                                    var ErrorSheet = package.Workbook.Worksheets.SingleOrDefault(x => x.Name == "ErrorList");
                                    if(ErrorSheet!=null)
                                        package.Workbook.Worksheets.Delete(ErrorSheet);
                                }
                            }
                            return RedirectToAction("Index", new { IsSuccess = 2 });
                        }
                    }
                }
                catch(Exception e)
                {
                    //ModelState.AddModelError("", "Error while file uploading.");
                    ModelState.AddModelError("", e.ToString());
                    return View("Index");
                }

            }
            ModelState.AddModelError("", "Error while file uploading.");
            return View("Index");
        }

        // GET: Admin/WareHouse/Products
        public ActionResult Products(int? size, int? page)
        {
            var pageSize = size ?? 5;
            var pageNumber = page ?? 1;
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
            if ((System.IO.File.Exists(path)))
            {
                FileInfo fileInfo = new FileInfo(path);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ViewBag.UploadedFile = String.Format("File uploaded at {0}", System.IO.File.GetLastWriteTime(path));
                    ExcelWorkbook workbook = package.Workbook;
                    ExcelWorksheet worksheet = workbook.Worksheets["Products"];
                    IEnumerable<ExcelProductModel> product = worksheet.ConvertSheetToObjects<ExcelProductModel>();
                    return View(product.ToPagedList(pageNumber, pageSize));
                }
            }
            return View();
        }

        // GET: Admin/WareHouse/Genres
        public ActionResult Genres(int? size, int? page)
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
            if ((System.IO.File.Exists(path)))
            {
                FileInfo fileInfo = new FileInfo(path);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ViewBag.UploadedFile = String.Format("File uploaded at {0}", System.IO.File.GetLastWriteTime(path));
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets["Genres"];
                    var genre = worksheet.ConvertSheetToObjects<ExcelGenreModel>();
                    return View(genre.ToPagedList(pageNumber, pageSize));
                }
            }
            return View();
        }

        // GET: Admin/WareHouse/Discounts
        public ActionResult Discounts(int? size, int? page)
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
            if ((System.IO.File.Exists(path)))
            {
                FileInfo fileInfo = new FileInfo(path);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ViewBag.UploadedFile = String.Format("File uploaded at {0}", System.IO.File.GetLastWriteTime(path));
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets["Discounts"];
                    var discount = worksheet.ConvertSheetToObjects<ExcelDiscountModel>();
                    return View(discount.ToPagedList(pageNumber, pageSize));
                }
            }
            return View();
        }



        // GET: Admin/WareHouse/Brands
        public ActionResult Brands(int? size, int? page)
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), "fileupload.xlsx");
            if ((System.IO.File.Exists(path)))
            {
                FileInfo fileInfo = new FileInfo(path);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ViewBag.UploadedFile = String.Format("File uploaded at {0}", System.IO.File.GetLastWriteTime(path));
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets["Brands"];
                    var brand = worksheet.ConvertSheetToObjects<ExcelBrandModel>();
                    return View(brand.ToPagedList(pageNumber, pageSize));
                }
            }
            return View();
        }

        // GET: Home  
        public ActionResult UploadImages()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImages(HttpPostedFileBase[] Files)
        {
            List<string> paths = new List<string>();
            if (ModelState.IsValid)
            { 
                foreach (HttpPostedFileBase file in Files)
                {
                    if (file != null && file.ContentLength>0)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);

                        var IsImage = new Func<string, bool>(str => {
                            return !Regex.IsMatch(str, @"^.+\.(gif|jpe?g|tiff?|png|webp|bmp)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        });
                        if (IsImage(InputFileName))
                        {
                            ModelState.AddModelError("", "File must be type image.");
                            return View();
                        }

                        var ServerSavePath = Path.Combine(Server.MapPath("~/Content/Images/") + InputFileName);
                        file.SaveAs(ServerSavePath);
                        if ((System.IO.File.Exists(ServerSavePath)))
                        {
                            paths.Add("/Content/Images/"+ InputFileName);
                        }
                    }

                }
                ViewBag.PathImages = paths;
            }
            return View();
        }
        #region Helpers
        private bool InsertOrUpdate(ExcelProductModel model)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Product product = context.Products
                        .Where(i =>
                            i.Product_id == model.ProductId &&
                            i.Genre_id == model.GenreId &&
                            i.Disscount_id == model.DiscountId
                        )
                        .FirstOrDefault();
                    if (product == null)
                    {
                        context.Products.Add(new Product
                        {
                            Genre_id = model.GenreId,
                            Disscount_id = model.DiscountId,
                            Brand_id = model.BrandId,
                            Product_name = model.ProductName,
                            View = 0,
                            Buyturn = 0,
                            Price = model.Price,
                            Quantity = model.Quantity,
                            Status = model.Status,
                            Type = model.Type,
                            Specifications = model.Specifications,
                            Image = model.Image,
                            Description = model.Description,
                            Create_by = User.Identity.GetUserId(),
                            Create_at = DateTime.Now,
                            Update_by = User.Identity.GetUserId(),
                            Update_at = DateTime.Now
                        });
                    }
                    else
                    {
                        product.Brand_id = model.BrandId;
                        product.Product_name = model.ProductName;
                        product.Price = model.Price;
                        product.Quantity = model.Quantity;
                        product.Status = model.Status;
                        product.Type = model.Type;
                        product.Specifications = model.Specifications;
                        product.Image = model.Image;
                        product.Description = model.Description;
                        product.Update_by = User.Identity.GetUserId();
                        product.Update_at = DateTime.Now;
                        context.Entry(product).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    return false;
                }
            }
            catch (Exception e)
            {
                return true;
            }
            
        }
        private bool InsertOrUpdate(ExcelGenreModel model)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Genre genre = context.Genres
                        .Where(i =>
                            i.Genre_id == model.GenreId
                        )
                        .FirstOrDefault();
                    if (genre == null)
                    {
                        context.Genres.Add(new Genre
                        {
                            Genre_name = model.GenreName,
                            Create_by = User.Identity.GetUserId(),
                            Create_at = DateTime.Now,
                            Update_by = User.Identity.GetUserId(),
                            Update_at = DateTime.Now
                        });
                    }
                    else
                    {
                        genre.Genre_name = model.GenreName;
                        genre.Update_by = User.Identity.GetUserId();
                        genre.Update_at = DateTime.Now;
                        context.Entry(genre).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    return false;
                }
            }
            catch (Exception e)
            {
                return true;
            }
        }
        private bool InsertOrUpdate(ExcelBrandModel model)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Brand brand = context.Brands
                        .Where(i =>
                            i.Brand_id == model.BrandId
                        )
                        .FirstOrDefault();
                    if (brand == null)
                    {
                        context.Brands.Add(new Brand
                        {
                            Brand_name = model.BrandName,
                            Create_by = User.Identity.GetUserId(),
                            Create_at = DateTime.Now,
                            Update_by = User.Identity.GetUserId(),
                            Update_at = DateTime.Now
                        });
                    }
                    else
                    {
                        brand.Brand_name = model.BrandName;
                        brand.Update_by = User.Identity.GetUserId();
                        brand.Update_at = DateTime.Now;
                        context.Entry(brand).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
        private bool InsertOrUpdate(ExcelDiscountModel model)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Discount discount = context.Discounts
                        .Where(i =>
                            i.Disscount_id == model.DiscountId
                        )
                        .FirstOrDefault();
                    if (discount == null)
                    {
                        context.Discounts.Add(new Discount
                        {
                            Discount_name = model.DiscountName,
                            Discount_star = model.DiscountStart,
                            Discount_end = model.DiscountEnd,
                            Discount_code = model.DiscountCode,
                            Discount_price = model.DiscountPrice,
                            Quantity = model.Quantity,
                            Create_by = User.Identity.GetUserId(),
                            Create_at = DateTime.Now,
                            Update_by = User.Identity.GetUserId(),
                            Update_at = DateTime.Now
                        });
                    }
                    else
                    {
                        discount.Discount_name = model.DiscountName;
                        discount.Discount_star = model.DiscountStart;
                        discount.Discount_end = model.DiscountEnd;
                        discount.Discount_code = model.DiscountCode;
                        discount.Discount_price = model.DiscountPrice;
                        discount.Quantity = model.Quantity;
                        discount.Update_by = User.Identity.GetUserId();
                        discount.Update_at = DateTime.Now;
                        context.Entry(discount).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
        #endregion
    }
}