namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderAddress")]
    public partial class OrderAddress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderAddress()
        {
            Orders = new HashSet<Order>();
        }

        public int OrderAddressId { get; set; }

        [StringLength(10)]
        public string OrderPhonenumber { get; set; }

        [StringLength(20)]
        public string OrderUsername { get; set; }

        [StringLength(150)]
        public string Content { get; set; }

        public int TimesEdit { get; set; }

        public int? Province_id { get; set; }

        public int? District_id { get; set; }

        public int? Ward_id { get; set; }

        public virtual District District { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        public virtual Province Province { get; set; }

        public virtual Ward Ward { get; set; }
    }
}
