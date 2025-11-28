using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = null!;

        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error

        public bool IsRead { get; set; } = false;

        [StringLength(255)]
        public string? RelatedLink { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}