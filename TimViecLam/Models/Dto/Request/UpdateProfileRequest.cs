using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc. ")]
        [StringLength(100, ErrorMessage = "Tên quá dài.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Địa chỉ quá dài.")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại quá dài.")]
        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
    }
}