namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductImage
    {
        [Key]
        public int Product_img_id { get; set; }

        public int Product_id { get; set; }

        public int Genre_id { get; set; }

        public int Disscount_id { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public virtual Product Product { get; set; }
    }
}
