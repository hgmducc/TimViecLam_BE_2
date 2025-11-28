using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("JobApplications")]
    public class JobApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        [ForeignKey("JobPosting")]
        public int JobPostingID { get; set; }

        [Required]
        [ForeignKey("Candidate")]
        public int CandidateID { get; set; }

        [StringLength(255)]
        public string? CVFilePath { get; set; }

        [StringLength(1000)]
        public string? CoverLetter { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Submitted"; // Submitted, Reviewing, Shortlisted, Interviewed, Accepted, Rejected

        public DateTime AppliedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        [StringLength(500)]
        public string? EmployerNotes { get; set; }

        // Navigation properties
        public virtual JobPosting JobPosting { get; set; } = null!;
        public virtual Candidate Candidate { get; set; } = null!;
    }
}