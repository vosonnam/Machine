namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;
    using xqmachine.Models.Helpers;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Feedbacks = new HashSet<Feedback>();
            Oder_Detail = new HashSet<Oder_Detail>();
            ProductImages = new HashSet<ProductImage>();
        }

        public Product(ExcelProductModel model)
        {
            Product_id = model.ProductId;
            Genre_id = model.GenreId;
            Disscount_id = model.DiscountId;
            Brand_id = model.BrandId;
            Product_name = model.ProductName;
            Price = model.Price;
            Quantity = model.Quantity;
            Status = model.Status;
            Type = model.Type;
            Specifications = model.Specifications;
            Image = model.Image;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Product_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Genre_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Disscount_id { get; set; }

        public int Brand_id { get; set; }

        [Required]
        [StringLength(200)]
        public string Product_name { get; set; }

        public double Price { get; set; }

        public long View { get; set; }

        public long Buyturn { get; set; }

        [StringLength(10)]
        public string Quantity { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [Required]
        [StringLength(100)]
        public string Create_by { get; set; }

        public DateTime Create_at { get; set; }

        public int? Type { get; set; }

        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime Update_at { get; set; }

        public string Specifications { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Discount Discount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        public virtual Genre Genre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Oder_Detail> Oder_Detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        //[ValidateFile(ErrorMessage = "Please select a PNG image smaller than 1MB")]
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }


        //[ValidateFile(ErrorMessage = "Please select a PNG image smaller than 1MB")]
        [NotMapped]
        public HttpPostedFileBase[] ImageUploadMulti { get; set; }
    }
}
