namespace TimViecLam.Models.Dto.Request
{
    public class UserQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Role { get; set; } // Admin, Candidate, Employer
        public string? Status { get; set; } // Active, Locked
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc"; // asc, desc
    }
}