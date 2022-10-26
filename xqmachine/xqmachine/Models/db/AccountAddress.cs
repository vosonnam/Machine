namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccountAddress")]
    public partial class AccountAddress
    {
        [Key]
        public int Account_address_id { get; set; }

        public int UserId { get; set; }

        public int Province_id { get; set; }

        public int District_id { get; set; }

        public int Ward_id { get; set; }

        [StringLength(10)]
        public string AccountPhoneNumber { get; set; }

        [StringLength(20)]
        public string AccountUsername { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public bool IsDefault { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual District District { get; set; }

        public virtual Province Province { get; set; }

        public virtual Ward Ward { get; set; }
    }
}
