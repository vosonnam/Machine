using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using xqmachine.Models.db;

namespace xqmachine.Models.Helpers
{
    #region ExcelHelperMethod
    public class ExcelExport
    {
        private static readonly DataBaseContext db = new DataBaseContext();
        public static Stream CreateExcelFile(Stream stream = null)
        {

            double ColumnWidth = 20;
            var products = db.Products
                .AsEnumerable()
                .Select(i => new ExcelProductModel(i))
                .ToList();
            List<ExcelGenreModel> genres = db.Genres
                .AsEnumerable()
                .Select(i => new ExcelGenreModel(i))
                .ToList();
            List<ExcelBrandModel> brands = db.Brands
                .AsEnumerable()
                .Select(i => new ExcelBrandModel(i))
                .ToList();
            List<ExcelDiscountModel> discounts = db.Discounts
                .AsEnumerable()
                .Select(i => new ExcelDiscountModel(i))
                .ToList();
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "XQ_MACINE";
                excelPackage.Workbook.Properties.Title = "XQ_MACINE warehouse";
                excelPackage.Workbook.Properties.Comments = "Mọi thông tin cần được giữ bảo mật";

                // Add Products Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("Products");
                var FirstSheet = excelPackage.Workbook.Worksheets[1];
                FirstSheet.Cells[1, 1].LoadFromCollection(products, true, TableStyles.Dark9);
                BindingListForProducts(FirstSheet, ColumnWidth);

                // Add Genres Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("Genres");
                var SecondSheet = excelPackage.Workbook.Worksheets[2];
                SecondSheet.Cells[1, 1].LoadFromCollection(genres, true, TableStyles.Dark9);
                BinddingFormat(SecondSheet, "B:B", ColumnWidth);

                // Add Discounts Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("Discounts");
                var FourthSheet = excelPackage.Workbook.Worksheets[3];
                FourthSheet.Cells[1, 1].LoadFromCollection(discounts, true, TableStyles.Dark9);
                BinddingFormat(FourthSheet, "B:G", ColumnWidth);
                FourthSheet.Cells["C:D"].Style.Numberformat.Format = "mm-dd-yy";


                // Add Brands Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("Brands");
                var ThirdSheet = excelPackage.Workbook.Worksheets[4];
                ThirdSheet.Cells[1, 1].LoadFromCollection(brands, true, TableStyles.Dark9);
                BinddingFormat(ThirdSheet, "B:B", ColumnWidth);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private static void BinddingFormat(ExcelWorksheet Sheet, string ColumnUnlocks, double ColumnWidth)
        {
            Sheet.DefaultColWidth = (double)ColumnWidth;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Protection.IsProtected = true;
            Sheet.Cells[ColumnUnlocks].Style.Locked = false;
            Sheet.Column(1).Style.Locked = true;
        }

        private static void BindingListForProducts(ExcelWorksheet worksheet, double ColumnWidth)
        {
            BinddingFormat(worksheet, "B:L", ColumnWidth);

            // add a Genres validation and set values
            BiddingValidation(worksheet, "B:B", "'Genres'!A:A");

            // add a Discounts validation and set values
            BiddingValidation(worksheet, "C:C", "'Discounts'!A:A");

            // add a Brands validation and set values
            BiddingValidation(worksheet, "D:D", "'Brands'!A:A");
        }

        private static void BiddingValidation(ExcelWorksheet worksheet, string AtColumn, string SetValuesColumn)
        {
            var Validation = worksheet.DataValidations.AddListValidation(AtColumn);
            Validation.ShowErrorMessage = true;
            Validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
            Validation.ErrorTitle = "An invalid value was entered";
            Validation.Error = "Select a value from the list";
            Validation.Formula.ExcelFormula = SetValuesColumn;
        }
    }

    public static class ExcelImport
    {
        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet, bool IsHeaderProperties = true) where T : new()
        {
            var convertDateTime = new Func<double, DateTime>(excelDate =>
            {
                if (excelDate < 1)
                    throw new ArgumentException("Excel dates cannot be smaller than 0.");

                var dateOfReference = new DateTime(1900, 1, 1);

                if (excelDate > 60d)
                    excelDate -= 2;
                else
                    excelDate--;
                return dateOfReference.AddDays(excelDate);
            });

            var getColumnAddress = new Func<string, string>(str => {
                Match match = Regex.Match(str, @"^[a-zA-Z]+", RegexOptions.Singleline);
                return match.Value;
            });

            var tprops = (new T())
                .GetType()
                .GetProperties()
                .ToList();

            var groups = worksheet.Cells
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            var types = groups
                .Skip(1)
                .First()
                .Select(rcell => rcell.Value.GetType())
                .ToList();

            var colnames = groups
                .First()
                .Select((hcell, idx) => new { Name = hcell.Value.ToString(), index = idx, colAddress = getColumnAddress(hcell.Address) })
                .ToList();

            var rowvalues = groups
                .Skip(1)
                .Select(cg => cg.Select(c => new { c.Value, colAddress = getColumnAddress(c.Address) }).ToList())
                .ToList();

            var collection = rowvalues
                .Select(row =>
                {
                    var tnew = new T();
                    row.ForEach(item =>
                    {

                        var val = item.Value;
                        var Index = colnames.First(c => c.colAddress.Contains(item.colAddress)).index;
                        var type = types[Index];
                        PropertyInfo prop;
                        if (IsHeaderProperties)
                            prop = tprops.First(p => p.Name == colnames[Index].Name);
                        else
                            prop = tprops[Index];
                        if (val == null)
                        {
                            prop.SetValue(tnew, null);
                        }else if (type == typeof(double))
                        {

                            var unboxedVal = (double)val;

                            if (prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Nullable<int>))
                                prop.SetValue(tnew, (int)unboxedVal);
                            else if (prop.PropertyType == typeof(double))
                                prop.SetValue(tnew, unboxedVal);
                            else if (prop.PropertyType == typeof(DateTime))
                                prop.SetValue(tnew, convertDateTime(unboxedVal));
                            else
                                throw new NotImplementedException(String.Format("Type '{0}' not implemented yet!", prop.PropertyType.Name));
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                            prop.SetValue(tnew, val);
                        else
                        {
                            prop.SetValue(tnew, val.ToString());
                        }
                    });
                    return tnew;
                }).ToList();

            return collection;
        }

        public static ExcelWorksheet ReadWorkSheet(string path, string sheetName)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count < 1)
                {
                    return null;
                }
                ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName) ?? package.Workbook.Worksheets.FirstOrDefault();
                return workSheet;
            }
        }

    }
    #endregion
    #region ExcelImportClass

    public class ExcelProductModel
    {
        public ExcelProductModel()
        {
        }

        public ExcelProductModel(Product p)
        {
            this.ProductId = p.Product_id;
            this.GenreId = p.Genre_id;
            this.DiscountId = p.Disscount_id;
            this.BrandId = p.Brand_id;
            this.ProductName = p.Product_name;
            this.Price = p.Price;
            this.Quantity = p.Quantity;
            this.Status = p.Status;
            Type = p.Type;
            this.Specifications = p.Specifications;
            this.Image = p.Image;
            this.Description = p.Description;
        }

        public int ProductId { get; set; }

        public int GenreId { get; set; }

        public int DiscountId { get; set; }

        public int BrandId { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }

        public string Quantity { get; set; }

        public string Status { get; set; }

        public int? Type { get; set; }

        public string Specifications { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }
    }

    public class ExcelGenreModel
    {
        public ExcelGenreModel()
        {
        }

        public ExcelGenreModel(Genre i)
        {
            GenreId = i.Genre_id;
            GenreName = i.Genre_name;
        }
        public int GenreId { get; set; }

        public string GenreName { get; set; }
    }

    public class ExcelBrandModel
    {
        public ExcelBrandModel()
        {
        }

        public ExcelBrandModel(Brand i)
        {
            BrandId = i.Brand_id;
            BrandName = i.Brand_name;
        }
        public int BrandId { get; set; }

        public string BrandName { get; set; }
    }

    public class ExcelDiscountModel
    {
        public ExcelDiscountModel()
        {
        }

        public ExcelDiscountModel(Discount i)
        {
            DiscountId = i.Disscount_id;
            DiscountName = i.Discount_name;
            DiscountStart = i.Discount_star;
            DiscountEnd = i.Discount_end;
            DiscountPrice = i.Discount_price;
            DiscountCode = i.Discount_code;
            Quantity = i.Quantity;
        }
        public int DiscountId { get; set; }

        public string DiscountName { get; set; }

        public DateTime DiscountStart { get; set; }

        public DateTime DiscountEnd { get; set; }

        public double DiscountPrice { get; set; }

        public string DiscountCode { get; set; }

        public int Quantity { get; set; }
    }
    public class ErrorList
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
    #endregion
}