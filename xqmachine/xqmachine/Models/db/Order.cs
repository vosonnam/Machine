namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Oder_Detail = new HashSet<Oder_Detail>();
        }

        [Key]
        public int Order_id { get; set; }

        public int Payment_id { get; set; }

        public int Delivery_id { get; set; }

        public DateTime Oder_date { get; set; }

        public double Total { get; set; }

        public int UserId { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(100)]
        public string Create_by { get; set; }

        [Required]
        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime Update_at { get; set; }

        [StringLength(200)]
        public string Order_note { get; set; }

        public int? OrderAddressId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Delivery Delivery { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Oder_Detail> Oder_Detail { get; set; }

        public virtual OrderAddress OrderAddress { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
