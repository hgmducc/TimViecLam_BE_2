namespace TimViecLam.Models.Dto.Request
{
    public class JobPostingQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? JobType { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public string? Status { get; set; } = "Active";
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";
    }
}