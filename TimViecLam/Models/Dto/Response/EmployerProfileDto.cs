namespace TimViecLam.Models.Dto.Response
{
    public class EmployerProfileDto
    {
        public int EmployerID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }
        public string? CompanyLogo { get; set; } // ← ADD THIS PROPERTY
        public string? CompanySize { get; set; }
        public string? Industry { get; set; }
        public string? TaxCode { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? BusinessLicenseFile { get; set; }
        public string VerificationStatus { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? CompanyAddress { get; set; }

        // Thống kê (optional)
        public int TotalJobPostings { get; set; }
        public int ActiveJobPostings { get; set; }
    }
}