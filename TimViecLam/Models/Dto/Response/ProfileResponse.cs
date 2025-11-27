namespace TimViecLam.Models.Dto.Response
{
    public class ProfileResponse
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsGoogleAccount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}