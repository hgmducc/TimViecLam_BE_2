using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class AddExperienceRequest
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vị trí là bắt buộc.")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsCurrent { get; set; } = false;
    }
}