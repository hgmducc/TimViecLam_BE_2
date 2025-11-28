using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Employers")]
    public class Employer
    {
        [Key]
        [ForeignKey("User")]
        public int EmployerID { get; set; }

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? CompanyDescription { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(100)]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(255)]
        public string? BusinessLicenseFile { get; set; }

        [Required]
        [StringLength(30)]
        public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected

        public DateTime? VerifiedAt { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        // Company Size
        [StringLength(50)]
        public string? CompanySize { get; set; } // 1-50, 51-200, 201-500, 500+

        [StringLength(100)]
        public string? Industry { get; set; } // Ngành nghề

        // Logo
        [StringLength(255)]
        public string? CompanyLogo { get; set; }

        public DateTime? LastUpdated { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}