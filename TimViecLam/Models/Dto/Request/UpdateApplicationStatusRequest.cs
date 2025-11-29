using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateApplicationStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Submitted|Reviewing|Shortlisted|Interviewed|Accepted|Rejected)$",
            ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        public string? EmployerNotes { get; set; }
    }
}