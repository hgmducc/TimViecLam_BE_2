using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class GoogleRegisterRequest
    {
        [Required(ErrorMessage = "Google token là bắt buộc. ")]
        public string GoogleToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        [RegularExpression("^(Candidate|Employer)$", ErrorMessage = "Vai trò chỉ có thể là 'Candidate' hoặc 'Employer'. ")]
        public string Role { get; set; } = "Candidate";

        // ===== CHỈ CẦN THIẾT KHI Role = "Employer" =====
        [StringLength(150)]
        public string? CompanyName { get; set; }

        public IFormFile? BusinessLicenseFile { get; set; }

        [Url(ErrorMessage = "Website không đúng định dạng.")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không đúng định dạng.")]
        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "SĐT liên hệ không đúng định dạng.")]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? CompanyAddress { get; set; }
    }
}