namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReplyFeedback")]
    public partial class ReplyFeedback
    {
        [Key]
        public int Rep_feedback_id { get; set; }

        public string Content { get; set; }

        [StringLength(1)]
        public string Stastus { get; set; }

        public DateTime Create_at { get; set; }

        public int Feedback_id { get; set; }

        public int UserId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Feedback Feedback { get; set; }
    }
}
