using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TimViecLam.Models.Dto.Request
{
    public class RegisterCandidateRequest
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên quá dài, vui lòng thử lại")] 
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }=string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(10, ErrorMessage = "Số điện thoại quá dài, vui lòng thử lại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(20,MinimumLength = 8 , ErrorMessage ="Mật khẩu phải chứ ít nhất 8 kí tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string Gender { get; set; } = string.Empty;
        public string? address { get; set; }
    }
}
