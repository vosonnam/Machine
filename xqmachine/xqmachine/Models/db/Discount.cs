namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Discount")]
    public partial class Discount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Discount()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Disscount_id { get; set; }

        [Required]
        [StringLength(100)]
        public string Discount_name { get; set; }

        public DateTime Discount_star { get; set; }

        public DateTime Discount_end { get; set; }

        public double Discount_price { get; set; }

        [StringLength(10)]
        public string Discount_code { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(100)]
        public string Create_by { get; set; }

        [Required]
        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime Update_at { get; set; }

        public int Quantity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
