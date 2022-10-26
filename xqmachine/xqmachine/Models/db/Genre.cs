namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Genre")]
    public partial class Genre
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Genre()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int Genre_id { get; set; }

        [Required]
        [StringLength(50)]
        public string Genre_name { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(100)]
        public string Create_by { get; set; }

        [Required]
        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime Update_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
