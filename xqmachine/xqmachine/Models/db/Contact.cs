namespace xqmachine.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Contact")]
    public partial class Contact
    {
        public Contact(Models.Contact model)
        {
            Name = model.FirstName + " " + model.LastName;
            Phone = model.PhoneNumber;
            Email = model.Email;
            Content = model.Subject;
            Create_by = model.Email;
            Create_at = DateTime.Now;
            Status = "1";
            Update_by = model.Email;
            Update_at = DateTime.Now;
        }
        [Key]
        public int Contact_id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(20)]
        public string Create_by { get; set; }

        public DateTime Create_at { get; set; }

        [Required]
        [StringLength(1)]
        public string Status { get; set; }

        [Required]
        [StringLength(20)]
        public string Update_by { get; set; }

        public DateTime? Update_at { get; set; }
    }
}
