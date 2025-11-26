using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Dto.Request
{
    public class RegisterEmployerRequest
    {

        // --- Thông tin cho Bảng [Users] ---

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(150)]
        public string Email { get; set; } = null!; // Sẽ dùng làm email đăng nhập

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15)]
        public string? Phone { get; set; } // SĐT của người đăng ký (User)

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string Gender { get; set; } = string.Empty;
        public string? address { get; set; }

        // --- Thông tin cho Bảng [Employers] ---
        [Required(ErrorMessage = "Vui lòng nhập tên công ty.")]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng tải lên giấy phép kinh doanh.")]
        public IFormFile BusinessLicenseFile { get; set; } = null!; // Tệp giấy phép kinh doanh

        [Url(ErrorMessage = "Website không đúng định dạng (ví dụ: https://example.com)")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; } // Mã số thuế

        // --- Thông tin liên hệ (của Employer) ---
        // nếu null thì mặc định lấy từ FullName, Email, Phone của User

        [StringLength(100)]
        public string? ContactPerson { get; set; } // Người liên hệ (mặc định có thể là FullName)

        [EmailAddress(ErrorMessage = "Email liên hệ không đúng định dạng.")]
        [StringLength(150)]
        public string? ContactEmail { get; set; } // Email liên hệ (mặc định có thể là Email)

        [Phone(ErrorMessage = "SĐT liên hệ không đúng định dạng.")]
        [StringLength(20)]
        public string? ContactPhone { get; set; } // SĐT liên hệ (mặc định có thể là Phone)
    }
}