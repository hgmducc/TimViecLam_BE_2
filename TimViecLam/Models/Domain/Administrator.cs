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

        [StringLength(100)]
        public string? Department { get; set; } 

        [StringLength(100)]
        public string? InternalTitle { get; set; } 

        [Required]
        [StringLength(50)]
        public string AdminRole { get; set; } = null!; 

        // Thuộc tính điều hướng
        public virtual User User { get; set; } = null!;
    }
}