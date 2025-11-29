using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class CreateJobPostingRequest
    {
        [Required(ErrorMessage = "Tiêu đề công việc là bắt buộc.")]
        [StringLength(200)]
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công việc là bắt buộc.")]
        public string JobDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu công việc là bắt buộc.")]
        public string Requirements { get; set; } = string.Empty;

        public string? Benefits { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        [StringLength(50)]
        public string? SalaryType { get; set; } = "Negotiable";

        [Required]
        [StringLength(50)]
        public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Remote

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Industry { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ExperienceLevel { get; set; }

        public int? YearsOfExperienceRequired { get; set; }

        [StringLength(100)]
        public string? EducationLevel { get; set; }

        [Range(1, 100)]
        public int VacancyCount { get; set; } = 1;

        public DateOnly? ApplicationDeadline { get; set; }
    }
}