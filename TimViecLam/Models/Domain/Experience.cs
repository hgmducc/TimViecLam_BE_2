using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Experiences")]
    public class Experience
    {
        [Key]
        public int ExperienceID { get; set; }

        [Required]
        [ForeignKey("Candidate")]
        public int CandidateID { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = null!; // Vị trí công việc

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsCurrent { get; set; } = false; // Đang làm việc

        [StringLength(1000)]
        public string? Description { get; set; } // Mô tả công việc

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual Candidate Candidate { get; set; } = null!;
    }
}