using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateEmployerProfileRequest
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Url(ErrorMessage = "Website không hợp lệ.")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [StringLength(2000)]
        public string? CompanyDescription { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? CompanyAddress { get; set; }
    }
}