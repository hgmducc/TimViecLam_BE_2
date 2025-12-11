using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class VerifyEmployerRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc. ")]
        [RegularExpression("^(Verified|Rejected)$", ErrorMessage = "Trạng thái chỉ có thể là 'Verified' hoặc 'Rejected'.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        public string? Notes { get; set; }
    }
}