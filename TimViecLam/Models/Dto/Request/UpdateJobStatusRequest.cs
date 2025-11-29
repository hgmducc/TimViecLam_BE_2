using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateJobStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Draft|Active|Closed|Expired)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}