using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Educations")]
    public class Education
    {
        [Key]
        public int EducationID { get; set; }

        [Required]
        [ForeignKey("Candidate")]
        public int CandidateID { get; set; }

        [Required]
        [StringLength(200)]
        public string InstitutionName { get; set; } = null!; // Tên trường

        [StringLength(100)]
        public string? Degree { get; set; } // Cử nhân, Thạc sĩ, Tiến sĩ

        [StringLength(100)]
        public string? Major { get; set; } // Ngành học

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; } // GPA, Thành tích

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual Candidate Candidate { get; set; } = null!;
    }
}