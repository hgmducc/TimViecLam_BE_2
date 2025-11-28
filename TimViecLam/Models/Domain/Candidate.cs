using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Candidates")]
    public class Candidate
    {
        [Key]
        [ForeignKey("User")]
        public int CandidateID { get; set; }

        [StringLength(100)]
        public string? DesiredPosition { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? DesiredSalary { get; set; }

        public int? YearsOfExperience { get; set; }

        [StringLength(50)]
        public string? JobType { get; set; } // Full-time, Part-time, Remote, Freelance

        [StringLength(255)]
        public string? DesiredLocation { get; set; }

        // CV File
        [StringLength(255)]
        public string? CVFileName { get; set; }

        [StringLength(255)]
        public string? CVFilePath { get; set; }

        public DateTime? CVUploadedAt { get; set; }

        // Skills stored as JSON or comma-separated
        public string? Skills { get; set; } // JSON: ["C#", "ASP.NET", "SQL"]

        // Profile completeness
        [Range(0, 100)]
        public int ProfileCompleteness { get; set; } = 0;

        public DateTime? LastUpdated { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    }
}