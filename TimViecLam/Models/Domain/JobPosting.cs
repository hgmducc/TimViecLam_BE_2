using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("JobPostings")]
    public class JobPosting
    {
        [Key]
        public int JobPostingID { get; set; }

        [Required]
        [ForeignKey("Employer")]
        public int EmployerID { get; set; }

        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string JobDescription { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Requirements { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        public string? Benefits { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? SalaryMax { get; set; }

        [StringLength(50)]
        public string? SalaryType { get; set; } // Negotiable, Fixed, Range

        [Required]
        [StringLength(50)]
        public string JobType { get; set; } = null!; // Full-time, Part-time, Remote

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Industry { get; set; } = null!;

        [StringLength(100)]
        public string? ExperienceLevel { get; set; } // Intern, Junior, Mid, Senior

        public int? YearsOfExperienceRequired { get; set; }

        [StringLength(100)]
        public string? EducationLevel { get; set; } // High School, Bachelor, Master

        public int VacancyCount { get; set; } = 1;

        public DateOnly? ApplicationDeadline { get; set; }

        [StringLength(100)]
        public string? WorkingHours { get; set; }

        [StringLength(20)]
        public string? GenderRequirement { get; set; }

        public string? DetailedLocations { get; set; } // JSON
        public string? RequiredSkills { get; set; }    // JSON
        public string? Tags { get; set; }              // JSON

        [StringLength(500)]
        public string? CareerGrowth { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Active, Closed, Expired
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        // ✅ THÊM: Ngày đóng tuyển
        public DateTime? ClosedAt { get; set; }

        public int ViewCount { get; set; } = 0;
        public int ApplicationCount { get; set; } = 0;

        // Navigation properties
        public virtual Employer Employer { get; set; } = null!;
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}