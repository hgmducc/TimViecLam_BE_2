using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc. ")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp. ")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}