using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class AddEducationRequest
    {
        [Required(ErrorMessage = "Tên trường là bắt buộc.")]
        [StringLength(200)]
        public string InstitutionName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Degree { get; set; }

        [StringLength(100)]
        public string? Major { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}