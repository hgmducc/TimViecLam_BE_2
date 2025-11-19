using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Candidates")]
    public class Candidate
    {
        [Key]
        [ForeignKey("User")]
        public int CandidateID { get; set; } // Đây LÀ UserID

        // KHÔNG CẦN thuộc tính 'UserID' riêng biệt

        [StringLength(100)]
        public string? HighestEducation { get; set; }

        [StringLength(150)]
        public string? Major { get; set; }

        // CSDL cho phép NULL, nên dùng int? (nullable int)
        public int? ExperienceYears { get; set; }

        public string? Skills { get; set; } // nvarchar(MAX)

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? ExpectedSalary { get; set; }

        [StringLength(30)]
        public string? JobType { get; set; }

        [StringLength(255)]
        public string? DesiredLocation { get; set; }

        [StringLength(255)]
        public string? CVUrl { get; set; }

        // Thuộc tính điều hướng
        public virtual User User { get; set; } = null!;
    }
}