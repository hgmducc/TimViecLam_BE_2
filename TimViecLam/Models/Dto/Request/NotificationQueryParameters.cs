namespace TimViecLam.Models.Dto.Request
{
    public class NotificationQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool? IsRead { get; set; } // null = all, true = read, false = unread
        public string? Type { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";
    }
}