using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xqmachine.Models
{
    public class Contact
    {
        [Required(ErrorMessage = "Nhập họ")]
        [Display(Name = "Họ")]
        [MaxLength(30, ErrorMessage = "Họ tên tối đa 30 ký tự")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nhập tên lót và tên")]
        [Display(Name = "Tên")]
        [MaxLength(200, ErrorMessage = "Tên tối đa 200 ký tự")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Nhập Email")]
        [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Nhập số điện thoại")]
        [StringLength(10)]
        [RegularExpression("^(0)([0-9]{9})$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0, chứa ký tự số từ (0 -> 9) và đủ 10 chữ số")]

        public string PhoneNumber { get; set; }

        [Display(Name = "Yêu cầu hỗ trợ")]
        [Required(ErrorMessage = "Không được để trống")]
        public string Subject { get; set; }
    }
}