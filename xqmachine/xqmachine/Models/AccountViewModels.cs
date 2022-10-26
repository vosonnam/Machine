using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xqmachine.Models
{
    public class ExternalLoginConfirmationViewModel
    {

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Nhập họ tên")]
        [MaxLength(30, ErrorMessage = "Họ tên tối đa 30 ký tự")]
        public string FullName { get; set; }

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
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nhập Email")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Nhập Email")]
        [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Nhập họ tên")]
        [MaxLength(30, ErrorMessage = "Họ tên tối đa 30 ký tự")]
        public string FullName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Nhập Mật khẩu")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Mật khẩu xác nhận")]
        [Required(ErrorMessage = "Nhập Mật khẩu xác nhận")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng nhau")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Nhập số điện thoại")]
        [StringLength(10)]
        [RegularExpression("^(0)([0-9]{9})$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0, chứa ký tự số từ (0 -> 9) và đủ 10 chữ số")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
