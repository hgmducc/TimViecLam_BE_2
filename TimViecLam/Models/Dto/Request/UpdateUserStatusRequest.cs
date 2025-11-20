using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateUserStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Active|Locked)$", ErrorMessage = "Trạng thái chỉ có thể là 'Active' hoặc 'Locked'.")]
        public string Status { get; set; } = null!;
    }
}
