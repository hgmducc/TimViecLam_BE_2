namespace TimViecLam.Models.Dto.Response
{
    public class ProfileResponse
    {
        // Thông tin User cơ bản
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
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Nested role-specific data (chỉ 1 trong 3 sẽ có giá trị)
        public CandidateProfileDto? CandidateProfile { get; set; }
        public EmployerProfileDto? EmployerProfile { get; set; }
        public AdminProfileDto? AdminProfile { get; set; }
    }
}