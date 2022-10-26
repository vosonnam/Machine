namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Payment")]
    public partial class Payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Payment_id { get; set; }

        [Required]
        [StringLength(50)]
        public string Payment_name { get; set; }

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
