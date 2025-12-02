using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class CreateNotificationRequest
    {
        [Required(ErrorMessage = "UserID là bắt buộc.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc.")]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error

        [StringLength(255)]
        public string? RelatedLink { get; set; }
    }
}