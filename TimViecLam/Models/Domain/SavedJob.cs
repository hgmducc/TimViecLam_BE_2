using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("SavedJobs")]
    public class SavedJob
    {
        [Key]
        public int SavedJobID { get; set; }

        [Required]
        [ForeignKey("Candidate")]
        public int CandidateID { get; set; }

        [Required]
        [ForeignKey("JobPosting")]
        public int JobPostingID { get; set; }

        public DateTime SavedAt { get; set; }

        // Navigation properties
        public virtual Candidate Candidate { get; set; } = null!;
        public virtual JobPosting JobPosting { get; set; } = null!;
    }
}