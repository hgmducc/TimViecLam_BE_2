using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Avatar { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = null!; // Admin, Candidate, Employer

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = null!; // Active, Locked

        [StringLength(255)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation properties
        public virtual Administrator? Administrator { get; set; }
        public virtual Employer? Employer { get; set; }
        public virtual Candidate? Candidate { get; set; }
    }
}