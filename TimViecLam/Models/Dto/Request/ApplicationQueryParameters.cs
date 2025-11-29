namespace TimViecLam.Models.Dto.Request
{
    public class ApplicationQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; }
        public int? JobPostingID { get; set; }
        public string? SortBy { get; set; } = "AppliedAt";
        public string? SortOrder { get; set; } = "desc";
    }
}