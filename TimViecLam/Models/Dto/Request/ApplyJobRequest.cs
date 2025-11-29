using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class ApplyJobRequest
    {
        [Required(ErrorMessage = "ID tin tuyển dụng là bắt buộc. ")]
        public int JobPostingID { get; set; }

        [StringLength(1000, ErrorMessage = "Thư xin việc quá dài.")]
        public string? CoverLetter { get; set; }

        // Optional: Upload CV riêng cho đơn ứng tuyển này (nếu khác CV trong profile)
        public IFormFile? CVFile { get; set; }
    }
}