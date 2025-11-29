namespace TimViecLam.Models.Dto.Response
{
    public class JobApplicationDto
    {
        public int ApplicationID { get; set; }
        public int JobPostingID { get; set; }
        public int CandidateID { get; set; }

        // Job info
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string Location { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;

        // Candidate info
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string? CandidatePhone { get; set; }
        public string? CandidateAvatar { get; set; }

        // Application info
        public string? CVFilePath { get; set; }
        public string? CoverLetter { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? EmployerNotes { get; set; }
    }
}