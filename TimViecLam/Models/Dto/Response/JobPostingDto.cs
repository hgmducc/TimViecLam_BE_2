namespace TimViecLam.Models.Dto.Response
{
    public class JobPostingDto
    {
        public int JobPostingID { get; set; }
        public int EmployerID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string? Benefits { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? SalaryType { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string? ExperienceLevel { get; set; }
        public int? YearsOfExperienceRequired { get; set; }
        public string? EducationLevel { get; set; }
        public int VacancyCount { get; set; }
        public DateOnly? ApplicationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
        public int ApplicationCount { get; set; }
    }
}