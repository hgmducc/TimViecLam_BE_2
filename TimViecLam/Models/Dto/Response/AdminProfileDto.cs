namespace TimViecLam.Models.Dto.Response
{
    public class AdminProfileDto
    {
        public int AdminID { get; set; }
        public string AdminRole { get; set; } = string.Empty; // SuperAdmin, Moderator
        public DateTime? LastLoginAt { get; set; }

        // Permissions (optional)
        public List<string>? Permissions { get; set; }
    }
}