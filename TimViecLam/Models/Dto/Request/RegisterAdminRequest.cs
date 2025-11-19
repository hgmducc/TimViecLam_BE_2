using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class RegisterAdminRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = null!;

        [StringLength(10)]
        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string AdminRole { get; set; } = null!; // VD: SuperAdmin, Moderator

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự.")]
        public string Password { get; set; } = null!;
    }
}
