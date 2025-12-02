namespace TimViecLam.Models.Dto.Response
{
    public class SavedJobDto
    {
        public int SavedJobID { get; set; }
        public int JobPostingID { get; set; }

        // Job info
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string Location { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? SalaryType { get; set; }
        public DateOnly? ApplicationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string>? Tags { get; set; }
        public DateTime SavedAt { get; set; }
        public bool HasApplied { get; set; }
    }
}