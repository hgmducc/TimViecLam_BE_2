using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Domain
{
    [Table("Employers")]
    public class Employer
    {
        [Key] // 1. Đây là Khóa Chính
        [ForeignKey("User")] // 2. Nó CŨNG LÀ Khóa Ngoại trỏ đến User
        public int EmployerID { get; set; } // Sẽ nhận giá trị của UserID

        // 3. KHÔNG CẦN thuộc tính 'UserID' riêng biệt

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!; // NOT NULL

        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        public string? CompanyDescription { get; set; } // nvarchar(MAX)

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(100)]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(255)]
        public string? BusinessLicenseFile { get; set; }

        [Required]
        [StringLength(30)]
        public string VerificationStatus { get; set; } = null!; // NOT NULL

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        // 4. Thuộc tính điều hướng ngược lại
        public virtual User User { get; set; } = null!;
    }
}