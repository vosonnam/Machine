namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Feedback()
        {
            ReplyFeedbacks = new HashSet<ReplyFeedback>();
        }

        [Key]
        public int Feedback_id { get; set; }

        public int UserId { get; set; }

        public int Product_id { get; set; }

        public int Genre_id { get; set; }

        public int Disscount_id { get; set; }

        public int Rate_star { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(100)]
        public string Create_by { get; set; }

        [StringLength(1)]
        public string Stastus { get; set; }

        [Required]
        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime? Update_at { get; set; }

        public string Content { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Product Product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplyFeedback> ReplyFeedbacks { get; set; }
    }
}
