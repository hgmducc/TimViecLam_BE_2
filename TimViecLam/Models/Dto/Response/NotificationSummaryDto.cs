namespace TimViecLam.Models.Dto.Response
{
    public class NotificationSummaryDto
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int ReadCount { get; set; }
        public Dictionary<string, int> TypeCounts { get; set; } = new Dictionary<string, int>();
    }
}