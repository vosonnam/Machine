namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Delivery")]
    public partial class Delivery
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Delivery()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Delivery_id { get; set; }

        [Required]
        [StringLength(100)]
        public string Delivery_name { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(20)]
        public string Create_by { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [Required]
        [StringLength(20)]
        public string Update_by { get; set; }

        public DateTime? Update_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
