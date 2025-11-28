using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Administrators")]
    public class Administrator
    {
        [Key]
        [ForeignKey("User")]
        public int AdminID { get; set; }

        [Required]
        [StringLength(50)]
        public string AdminRole { get; set; } = null!; // SuperAdmin, Moderator, Support

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? InternalTitle { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}